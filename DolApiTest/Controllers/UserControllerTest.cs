using System;
using System.Collections;
using System.Collections.Generic;
using DolApi.Controllers;
using DolApi.POCOs;
using DolApi.Services;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace DolApiTest.Controllers
{
    public class UserControllerTest
    {
        private readonly UserController _sut;
        private readonly IAdminService _adminService;

        public UserControllerTest()
        {
            _adminService = Substitute.For<IAdminService>();
            _sut = new UserController(_adminService);
        }

        [Fact]
        public void PostCreatesUserAndSetsClaimsIfUserDoesNotExist()
        {
            var request = new PlayerRequest
            {
                Authority = "0",
                Email = "test@test.com"
            };

            _adminService.GetUserByEmailAsync("test@test.com").Throws(new Exception());

            _adminService.CreateUserAsync(Arg.Any<UserRecordArgs>()).Returns("1234");
            
            var actual = _sut.Post(request);

            _adminService.Received(1).CreateUserAsync(Arg.Is<UserRecordArgs>(args =>
                args.Email == "test@test.com" && !string.IsNullOrEmpty(args.Password)));

            var claims = new Dictionary<string, object>(new[] {new KeyValuePair<string, object>("Authority", "0")});

            _adminService.Received(1).SetCustomUserClaimsAsync(Arg.Is("1234"),
                Arg.Is<Dictionary<string, object>>(objects => (string) objects["Authority"] == "0"));

            actual.Result.Should().BeOfType<OkResult>();
        }
        
        [Fact]
        public void PostOnlySetsClaimsIfUserExists()
        {
            var request = new PlayerRequest
            {
                Authority = "0",
                Email = "test@test.com"
            };

            _adminService.GetUserByEmailAsync("test@test.com").Returns("1234");

            var actual = _sut.Post(request);

            _adminService.Received(0).CreateUserAsync(Arg.Is<UserRecordArgs>(args =>
                args.Email == "test@test.com" && !string.IsNullOrEmpty(args.Password)));

            var claims = new Dictionary<string, object>(new[] {new KeyValuePair<string, object>("Authority", "0")});

            _adminService.Received(1).SetCustomUserClaimsAsync(Arg.Is("1234"),
                Arg.Is<Dictionary<string, object>>(objects => (string) objects["Authority"] == "0"));

            actual.Result.Should().BeOfType<OkResult>();
        }
    }
}
