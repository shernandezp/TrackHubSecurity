// Copyright (c) 2024 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using TrackHub.Security.Application.Users.Commands.Create;
using TrackHub.Security.Application.Users.Events;

namespace Application.UnitTests.Users;
[TestFixture]
public class CreateUserCommandHandlerTests
{
    private Mock<IUserWriter> _mockUserWriter;
    private Mock<IPublisher> _mockPublisher;
    private CreateUserCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockUserWriter = new Mock<IUserWriter>();
        _mockPublisher = new Mock<IPublisher>();
        _handler = new CreateUserCommandHandler(_mockUserWriter.Object, _mockPublisher.Object);
    }

    [Test]
    public async Task Handle_ValidCommand_ReturnsUserVm()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            // Set up the properties of the CreateUserDto object
        };
        var createUserCommand = new CreateUserCommand(createUserDto);
        var user = new UserVm
        {
            UserId = Guid.NewGuid(),
            Username = "username",
            AccountId = Guid.NewGuid()
        };
        var shrankUser = new UserShrankDto(user.UserId, user.Username, user.AccountId);

        _mockUserWriter.Setup(x => x.CreateUserAsync(createUserDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockPublisher.Setup(x => x.Publish(It.IsAny<UserCreated.Notification>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        // Act
        var result = await _handler.Handle(createUserCommand, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(user);
        _mockPublisher.Verify(x => x.Publish(It.Is<UserCreated.Notification>(n => n.User == shrankUser), It.IsAny<CancellationToken>()), Times.Once);
    }
}
