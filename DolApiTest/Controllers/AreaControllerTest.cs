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
            var x = 1;
            var y = 1;
            var actual = await _sut.Get(x, y);

            actual.Should().BeOfType<OkObjectResult>();
            actual.As<OkObjectResult>().Value.Should().BeOfType<Area>();
            actual.As<OkObjectResult>().Value.As<Area>().X.Should().Be(1);
            actual.As<OkObjectResult>().Value.As<Area>().Y.Should().Be(1);
        }

        [Fact]
        public void GetReturnsNotFoundIfResourceDoesNotExist()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void PutGetsExistingRecordIfExistsAndReturnsOkObject()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void PutReturnsNotFoundIfResourceDoesNotExist()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void PutUpdatesResource()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void PutValidatesRecordAndReturnBadRequestIfInvalid()
        {
            throw new NotImplementedException();
        }
    }
}