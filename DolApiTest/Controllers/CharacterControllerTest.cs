using System;
using System.Collections;
using System.Collections.Generic;
using DolApi.Controllers;
using DolApi.POCOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DolApiTest.Controllers
{
    public class CharacterControllerTest
    {
        private readonly CharacterController _sut;

        [Fact]
        public void PutCreatesNewCharacter()
        {
            var result = _sut.Put("Bob");

            result.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public void GetWithNoNameReturnAllOfTheUsersCharacters()
        {
            var result = _sut.Get();
            
            result.Should().BeOfType(typeof(OkObjectResult));
            result.As<OkObjectResult>().Value.Should().BeAssignableTo(typeof(IEnumerable<Character>));
            result.As<OkObjectResult>().Value.As<IEnumerable<Character>>().Should().HaveCount(2).And.Contain("Peter")
                .And.Contain("Louis");
        }

        [Fact]
        public void GetWithNameReturnsSingleCharacter()
        {
            var result = _sut.Get("Peter");
            
            result.Should().BeOfType(typeof(OkObjectResult));
            result.As<OkObjectResult>().Value.Should().BeAssignableTo(typeof(Character));
            result.As<OkObjectResult>().Value.As<Character>().Name.Should().Be("Peter");
        }

        [Fact]
        public void DeleteRemovesGivenCharacter()
        {
            var result = _sut.Delete("Bob");

            result.Should().BeOfType(typeof(NoContentResult));
        }
    }
}
