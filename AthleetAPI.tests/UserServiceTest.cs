using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using FakeItEasy;
using FluentAssertions;
using AthleetAPI.Services;
using AthleetAPI.Repositories;
using AthleetAPI.Models;

public class UserServiceTest
{
    private UserService _userService; // System Under Test
    private IUserRepository _userRepository; // Mock

    [SetUp]
    public void Setup()
    {
        _userRepository = A.Fake<IUserRepository>();

        _userService = new UserService(_userRepository);
    }

    [Test]
    public void ShouldNotReturnOddUserId()
    {
        // Arrange (Given)
        A.CallTo(() => _userRepository.GetAllUsers()).Returns(
            new List<User> {
                new User {
                    UserId = 2,
                    FirebaseUID = "1qaz",
                    UserName = "foobar",
                    UserHeadline = "something"
                },
                new User {
                    UserId = 4,
                    FirebaseUID = "2wsx",
                    UserName = "nathan",
                    UserHeadline = "banana"
                }
            }
        );

        // Act (When)
        var userViewModels = _userService.GetAllUsers();

        // Assert (FluentAssertions) (Then)
        userViewModels.Any(s => s.Special).Should().BeFalse();
    }


    [Test]
    public void ShouldReturnTwoOddUserId()
    {
        // Arrange (Given)
        A.CallTo(() => _userRepository.GetAllUsers()).Returns(
            new List<User> {
                new User {
                    UserId = 1,
                    FirebaseUID = "qwerty",
                    UserName = "nathan",
                    UserHeadline = "banana"
                },
                new User {
                    UserId = 2,
                    FirebaseUID = "asdfg",
                    UserName = "stew",
                    UserHeadline = "banana"
                },
                new User {
                    UserId = 3,
                    FirebaseUID = "zxcvb",
                    UserName = "keri",
                    UserHeadline = "banana"
                }
            }
        );

        // Act (When)
        var userViewModels = _userService.GetAllUsers();

        // Assert (FluentAssertions) (Then)
        userViewModels.Count(s => s.Special).Should().Be(2);
    }

}