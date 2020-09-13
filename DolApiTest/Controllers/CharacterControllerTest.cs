using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DolApi.Controllers;
using DolApi.POCOs;
using DolApi.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace DolApiTest.Controllers
{
    public class CharacterControllerTest
    {
        private readonly CharacterController _sut;
        private readonly ICharacterRepo _characterRepo;

        public CharacterControllerTest()
        {
            var accessor = Substitute.For<IHttpContextAccessor>();

            accessor.HttpContext.User.Claims.Returns(new[] {new Claim("user_id", "qwerty")});
            
            _characterRepo = Substitute.For<ICharacterRepo>();

            _characterRepo.Add("qwerty", "Bob").Returns(new Character {Name = "Bob"});
            
            _sut = new CharacterController(accessor, _characterRepo);
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
    }
}
