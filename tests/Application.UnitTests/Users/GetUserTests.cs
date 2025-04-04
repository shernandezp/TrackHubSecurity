﻿// Copyright (c) 2025 Sergio Hernandez. All rights reserved.
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

using TrackHub.Security.Application.Users.Queries.Get;
using TrackHub.Security.Domain.Interfaces;
using TrackHub.Security.Domain.Models;

namespace Application.UnitTests.Users;

[TestFixture]
internal class GetUserTests
{
    private Mock<IUserReader> _readerMock;

    [SetUp]
    public void Setup()
    {
        // Initialize the mock and the object under test before each test
        _readerMock = new Mock<IUserReader>();
    }

    [Test]
    public async Task Handle_ValidQuery_ReturnsUserVm()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        var userVm = new UserVm { AccountId = Guid.NewGuid() };
        _readerMock.Setup(m => m.GetUserAsync(userId, cancellationToken))
                  .ReturnsAsync(userVm);

        var handler = new GetUserQueryHandler(_readerMock.Object);
        var query = new GetUserQuery { Id = userId };

        // Act
        var result = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Default);
        Assert.That(result, Is.EqualTo(userVm));

        // Verify that GetUserAsync was called with the correct arguments
        _readerMock.Verify(m => m.GetUserAsync(userId, cancellationToken), Times.Once);
    }
}
