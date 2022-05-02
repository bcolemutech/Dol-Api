namespace DolApiTest.Controllers;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;

public class UserControllerTest
{
    private readonly UserController _sut;
    private readonly IAdminService _adminService;
    private readonly IPlayerRepo _playerRepo;

    public UserControllerTest()
    {
        var accessor = Substitute.For<IHttpContextAccessor>();

        accessor.HttpContext?.User.Claims.Returns(new[] { new Claim("user_id", "1234"), new Claim("Authority", "2") });

        _playerRepo = Substitute.For<IPlayerRepo>();
        _adminService = Substitute.For<IAdminService>();
        _sut = new UserController(accessor, _adminService, _playerRepo);
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

        _playerRepo.Received(1).Add("1234", request);

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

    [Theory]
    [InlineData("add", false)]
    [InlineData("remove", false)]
    [InlineData("replace", true)]
    [InlineData("move", false)]
    [InlineData("copy", false)]
    [InlineData("test", true)]
    public async Task GivenAnOperationWhenPatchReturnAppropriateResponse(string operation, bool allow)
    {
        var doc = new JsonPatchDocument<User>(new List<Operation<User>>
        {
            new(operation, nameof(IUser.CurrentCharacter), "bob", "bob")
        }, new DefaultContractResolver());

        _playerRepo.Get("1234").Returns(Task.FromResult(new User{CurrentCharacter = "bob"}));

        var actual = await _sut.Patch(doc);

        var result = allow ? typeof(OkResult) : typeof(UnprocessableEntityResult);
        
        actual.Should().BeOfType(result);
    }

    [Fact]
    public async Task GivenUserDoesNotExistWhenPatchingThenReturnNotFound()
    {
        var doc = new JsonPatchDocument<User>(new List<Operation<User>>
        {
            new("replace", nameof(IUser.CurrentCharacter), "bob", "charlie")
        }, new DefaultContractResolver());

        _playerRepo.Get("1234").Returns(Task.FromResult<User>(null));

        var actual = await _sut.Patch(doc);

        await _playerRepo.Received(1).Get(Arg.Is("1234"));
        await _playerRepo.Received(0).Update(Arg.Any<string>(), Arg.Any<User>());

        actual.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GivenUserDoesExistWhenPatchingThenUpdatePlayer()
    {
        var doc = new JsonPatchDocument<User>(new List<Operation<User>>
        {
            new("replace", nameof(IUser.CurrentCharacter), "bob", "charlie")
        }, new DefaultContractResolver());

        var baseUser = new User
        {
            Authority = Authority.Player,
            Email = "test@test.com",
            CurrentCharacter = "bob",
            SessionId = "54321",
            UserId = "1234"
        };

        object receivedUser = null;

        _playerRepo.Get("1234").Returns(Task.FromResult(baseUser));
        _playerRepo.When(x => x.Update(Arg.Any<string>(), Arg.Any<User>())).Do(y => receivedUser = y.Args()[1]);


        var actionResult = await _sut.Patch(doc);

        await _playerRepo.Received(1).Get(Arg.Is("1234"));
        await _playerRepo.Received(1).Update(Arg.Is("1234"), Arg.Any<User>());

        actionResult.Should().BeOfType<OkResult>();

        receivedUser.Should().NotBeNull();
        receivedUser.Should().BeOfType<User>();
        receivedUser.As<User>().UserId.Should().Be("1234");
        receivedUser.As<User>().SessionId.Should().BeNullOrEmpty();
        receivedUser.As<User>().CurrentCharacter.Should().Be("charlie");
    }

    [Fact]
    public async Task GivenUserWithPlayerStatusWhenPatchingSessionIdThenDisallowAndReturnForbidden()
    {
        var doc = new JsonPatchDocument<User>(new List<Operation<User>>
        {
            new("replace", nameof(IUser.SessionId), "54321", "78945")
        }, new DefaultContractResolver());

        var baseUser = new User
        {
            Authority = Authority.Player,
            Email = "test@test.com",
            CurrentCharacter = "bob",
            SessionId = "54321",
            UserId = "1234"
        };

        _playerRepo.Get("1234").Returns(Task.FromResult(baseUser));


        var actionResult = await _sut.Patch(doc);

        await _playerRepo.Received(0).Get(Arg.Any<string>());
        await _playerRepo.Received(0).Update(Arg.Any<string>(), Arg.Any<User>());

        actionResult.Should().BeOfType<UnprocessableEntityResult>();
    }
}
