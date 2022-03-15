namespace DolApiTest.Controllers;

using System;
using System.Collections.Generic;
using dol_sdk.Enums;
using DolApi.Controllers;
using DolApi.Repositories;
using DolApi.Services;
using FirebaseAdmin.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using dol_sdk.POCOs;

public class UserControllerTest
{
    private readonly UserController _sut;
    private readonly IAdminService _adminService;
    private readonly IPlayerRepo _playerRepo;

    public UserControllerTest()
    {
        _playerRepo = Substitute.For<IPlayerRepo>();
        _adminService = Substitute.For<IAdminService>();
        _sut = new UserController(_adminService, _playerRepo);
    }

    [Fact]
    public void PostCreatesUserAndSetsClaimsIfUserDoesNotExist()
    {
        var request = new PlayerRequest("test@test.com", Authority.Admin);

        _adminService.GetUserByEmailAsync("test@test.com").Throws(new Exception());

        _adminService.CreateUserAsync(Arg.Any<UserRecordArgs>()).Returns("1234");

        var actual = _sut.Post(request);

        _adminService.Received(1).CreateUserAsync(Arg.Is<UserRecordArgs>(args =>
            args.Email == "test@test.com" && !string.IsNullOrEmpty(args.Password)));

        _playerRepo.Received(1).Add("1234");

        _adminService.Received(1).SetCustomUserClaimsAsync(Arg.Is("1234"),
            Arg.Is<Dictionary<string, object>>(objects => (int)objects["Authority"] == 0));

        actual.Result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public void PostOnlySetsClaimsIfUserExists()
    {
        var request = new PlayerRequest("test@test.com", Authority.Admin);

        _adminService.GetUserByEmailAsync("test@test.com").Returns("1234");

        var actual = _sut.Post(request);

        _adminService.Received(0).CreateUserAsync(Arg.Is<UserRecordArgs>(args =>
            args.Email == "test@test.com" && !string.IsNullOrEmpty(args.Password)));

        _adminService.Received(1).SetCustomUserClaimsAsync(Arg.Is("1234"),
            Arg.Is<Dictionary<string, object>>(objects => (int)objects["Authority"] == 0));

        actual.Result.Should().BeOfType<OkResult>();
    }
}
