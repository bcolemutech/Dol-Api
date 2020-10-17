namespace DolApiTest.Controllers
{
    using System;
    using System.Threading.Tasks;
    using DolApi.Controllers;
    using DolApi.POCOs;
    using DolApi.Repositories;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class AreaControllerTest
    {
        private readonly AreaController _sut;
        private readonly IAreaRepo _areaRepo;

        public AreaControllerTest()
        {
            _areaRepo = Substitute.For<IAreaRepo>();
            _sut = new AreaController(_areaRepo);
        }
        
        [Fact]
        public async Task GetReturnsOkObjectIfFound()
        {
            const int x = 1;
            const int y = 1;
            
            var area = new Area{ X = x, Y = y};

            _areaRepo.Retrieve(x, y).Returns(Task.FromResult(area));
            
            var actual = await _sut.Get(x, y);

            await _areaRepo.Received(1).Retrieve(Arg.Is(x), Arg.Is(y));

            actual.Should().BeOfType<OkObjectResult>();
            actual.As<OkObjectResult>().Value.Should().BeOfType<Area>();
            actual.As<OkObjectResult>().Value.As<Area>().X.Should().Be(1);
            actual.As<OkObjectResult>().Value.As<Area>().Y.Should().Be(1);
        }

        [Fact]
        public async Task GetReturnsNotFoundIfResourceDoesNotExist()
        {
            const int x = -1;
            const int y = 1;

            _areaRepo.Retrieve(x, y).Returns(Task.FromResult<Area>(null));
            
            var actual = await _sut.Get(x, y);

            await _areaRepo.Received(1).Retrieve(Arg.Is(x), Arg.Is(y));

            actual.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PutGetsExistingRecordAndUpdatesIfExistsThenReturnsOkObject()
        {
            const int x = 1;
            const int y = 1;
            
            var inputArea = new Area{ X = x, Y = y};

            var actual = await _sut.Put(x, y, inputArea);

            await _areaRepo.Received(1).Replace(x, y, Arg.Is<Area>(area => area.X == 1 && area.Y == 1));

            actual.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task PutOverridesXAndYWithResourceIdentifiers()
        {
            const int x = 1;
            const int y = 1;
            
            var areaBad = new Area{ X = x, Y = 5};

            var actual = await _sut.Put(x, y, areaBad);

            await _areaRepo.Received(1).Replace(x, y, Arg.Is<Area>(area => area.X == 1 && area.Y == 1));

            actual.Should().BeOfType<OkResult>();
        }
    }
}