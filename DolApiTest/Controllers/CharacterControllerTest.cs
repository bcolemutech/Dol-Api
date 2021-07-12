using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using dol_sdk.Enums;
using dol_sdk.POCOs;
using DolApi.Controllers;
using DolApi.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;
using Action = dol_sdk.Enums.Action;
using Character = DolApi.POCOs.Character;

namespace DolApiTest.Controllers
{
    public class CharacterControllerTest
    {
        private readonly CharacterController _sut;
        private readonly ICharacterRepo _characterRepo;
        private readonly IAreaRepo _areaRepo;

        public CharacterControllerTest()
        {
            var accessor = Substitute.For<IHttpContextAccessor>();

            accessor.HttpContext?.User.Claims.Returns(new[] {new Claim("user_id", "qwerty")});

            _characterRepo = Substitute.For<ICharacterRepo>();
            _areaRepo = Substitute.For<IAreaRepo>();

            _characterRepo.Add("qwerty", "Bob").Returns(new Character {Name = "Bob"});
            _areaRepo.Retrieve(1, 2).Returns(new DolApi.POCOs.Area {X = 1, Y = 2, Navigation = Navigation.Roads});
            _areaRepo.Retrieve(3, 3).Returns(new DolApi.POCOs.Area {X = 3, Y = 3, Navigation = Navigation.Impassable});

            var config = Substitute.For<IConfiguration>();
            config["EncounterEngineUrl"].Returns(@"https://bologna.com");
            
            _sut = new CharacterController(accessor, _characterRepo, _areaRepo, config);
        }

        [Fact]
        public async Task PutCreatesNewCharacter()
        {
            var result = await _sut.Put("Bob");

            await _characterRepo.Received(1).Add(Arg.Is("qwerty"),Arg.Is("Bob"));

            result.Should().BeOfType(typeof(CreatedResult));
            result.As<CreatedResult>().Value.Should().BeOfType(typeof(Character));
            result.As<CreatedResult>().Value.As<Character>().Name.Should().Be("Bob");
        }

        [Fact]
        public async Task GetWithNoNameReturnAllOfTheUsersCharacters()
        {
            _characterRepo.RetrieveAll("qwerty").Returns(
                new List<Character>
                {
                    new Character {Name = "Louis"},
                    new Character {Name = "Peter"}
                }
            );
            
            var result = await _sut.Get();
            
            await _characterRepo.Received(1).RetrieveAll(Arg.Is("qwerty"));
            
            result.Should().BeOfType(typeof(OkObjectResult));
            result.As<OkObjectResult>().Value.Should().BeAssignableTo(typeof(List<Character>));
            result.As<OkObjectResult>().Value.As<List<Character>>().Should().HaveCount(2).And
                .Contain(x => x.Name == "Peter")
                .And.Contain(x => x.Name == "Louis");
        }

        [Fact]
        public async Task GetWithNameReturnsSingleCharacter()
        {
            _characterRepo.Retrieve("qwerty","Peter").Returns(new Character {Name = "Peter"});
            
            var result = await _sut.Get("Peter");
            
            await _characterRepo.Received(1).Retrieve(Arg.Is("qwerty"),Arg.Is("Peter"));
            
            result.Should().BeOfType(typeof(OkObjectResult));
            result.As<OkObjectResult>().Value.Should().BeAssignableTo(typeof(Character));
            result.As<OkObjectResult>().Value.As<Character>().Name.Should().Be("Peter");
        }

        [Fact]
        public async Task DeleteRemovesGivenCharacter()
        {
            var result = await _sut.Delete("Bob");

            await _characterRepo.Received(1).Remove(Arg.Is("qwerty"),Arg.Is("Bob"));

            result.Should().BeOfType(typeof(NoContentResult));
        }
        
        [Fact]
        public async Task PutMoveUpdatesPositionForNow()
        {
            var move = new Position {X = 1, Y = 2, Location = "House", Populace = "Township", Action = Action.Move};
            var result = await _sut.PutMove("Bob", move);
            
            await _areaRepo.Received(1).Retrieve(1, 2);
            await _characterRepo.Received(1).SetMove(Arg.Is("qwerty"),Arg.Is("Bob"), Arg.Any<IPosition>());
            await _characterRepo.Received(1).SetPosition(Arg.Is("qwerty"), Arg.Is("Bob"),
                Arg.Is<IPosition>(i =>
                    i.Action == Action.Idle && i.X == 1 && i.Y == 2 && i.Location == "House" &&
                    i.Populace == "Township"));

            result.Should().BeOfType(typeof(AcceptedResult));
            result.As<AcceptedResult>().Location.Should().MatchRegex(
                @"https:\/\/bologna\.com\/[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}");
        }

        [Theory]
        [InlineData(-1, 2,  "House", "Township", Action.Move, "Area -1,2 does not exist")]
        [InlineData(1, -2,  "House", "Township", Action.Move, "Area 1,-2 does not exist")]
        [InlineData(-1, -2,  "House", "Township", Action.Move, "Area -1,-2 does not exist")]
        [InlineData(5, 5,  "", "", Action.Move, "Area 5,5 does not exist")]
        [InlineData(3, 3,  "", "", Action.Move, "Area 3,3 is impassable")]
        public async Task GivenPutToMoveWhenRequestLocationOrAreaDoesntExistThenReturnUnprocessableEntity(int x,
            int y,
            string location,
            string populace,
            Action action,
            string message)
        {
            _areaRepo.ClearReceivedCalls();
            _characterRepo.ClearReceivedCalls();
            
            var move = new Position() {X = x, Y = y, Location = location, Populace = populace, Action = action};
            
            var result = await _sut.PutMove("Bob", move);

            await _areaRepo.Received(1).Retrieve(x, y);
            await _characterRepo.Received(0).SetMove(Arg.Is("qwerty"),Arg.Is("Bob"), Arg.Any<IPosition>());
            await _characterRepo.Received(0).SetPosition(Arg.Is("qwerty"), Arg.Is("Bob"), Arg.Any<IPosition>());
            
            result.Should().BeOfType(typeof(BadRequestObjectResult));
            result.As<BadRequestObjectResult>().Value.Should().BeOfType(typeof(string));
            result.As<BadRequestObjectResult>().Value.As<string>().Should().Be($"Position object is not valid. {message}");
        }
    }
}
