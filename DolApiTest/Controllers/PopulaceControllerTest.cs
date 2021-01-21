using System.Collections.Generic;
using DolApi.Controllers;
using DolApi.POCOs;
using DolApi.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace DolApiTest.Controllers
{
    public class PopulaceControllerTest
    {
        private readonly PopulaceController _sut;
        private readonly IPopulaceRepo _repo;

        public PopulaceControllerTest()
        {
            _repo = Substitute.For<IPopulaceRepo>();
            
            var populaces = GetTestPopulaces();
            _repo.GetAllPops(Arg.Is(1), Arg.Is(1)).Returns(populaces);
            _sut = new PopulaceController(_repo);
        }

        [Fact]
        public void GivenPopulacesAvailableWhenGetByAreaThenReturnPopulaces()
        {
            var actual = _sut.GetPopulaces(1, 1);
            
            actual.Should().BeOfType<OkObjectResult>();
            actual.As<OkObjectResult>().Value.Should().BeOfType<IEnumerable<Populace>>();
            actual.As<OkObjectResult>().Value.As<IEnumerable<Populace>>().Should().HaveCount(2);
        }

        public static IEnumerable<DolApi.POCOs.Populace> GetTestPopulaces()
        {
            return new[]
            {
                new DolApi.POCOs.Populace
                {
                    Description = "A",
                    HasPort = false,
                    Name = "A",
                    Size = 123
                },
                new DolApi.POCOs.Populace
                {
                    Description = "B",
                    HasPort = true,
                    Name = "B",
                    Size = 234
                }
            };
        }
    }
}