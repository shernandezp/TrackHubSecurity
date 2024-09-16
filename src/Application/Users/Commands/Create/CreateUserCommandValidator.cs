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

namespace TrackHub.Security.Application.Users.Commands.Create;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    private readonly IUserReader _userReader;

    public CreateUserCommandValidator(IUserReader userReader)
    {
        _userReader = userReader;

        RuleFor(v => v.User.FirstName)
            .NotEmpty();

        RuleFor(v => v.User.LastName)
            .NotEmpty();

        // Validate the maximum length, non-empty, and uniqueness of the username
        RuleFor(v => v.User.Username)
            .MaximumLength(ColumnMetadata.DefaultUserNameLength)
            .NotEmpty()
            .MustAsync(ValidateUsername)
            .WithMessage("Username already in use");

        // Validate the minimum and maximum length, and non-empty of the password
        RuleFor(v => v.User.Password)
            .MinimumLength(ColumnMetadata.MinimumPasswordLength)
            .MaximumLength(ColumnMetadata.DefaultPasswordLength)
            .NotEmpty();

        // Validate the email address format, maximum length, non-empty, and uniqueness
        RuleFor(v => v.User.EmailAddress)
            .EmailAddress()
            .MaximumLength(ColumnMetadata.DefaultEmailLength)
            .NotEmpty()
            .MustAsync(ValidateEmailAddress)
            .WithMessage("Email address already in use");
    }

    // Asynchronously validate the uniqueness of the username
    private async Task<bool> ValidateUsername(string username, CancellationToken cancellationToken)
        => await _userReader.ValidateUsernameAsync(username, cancellationToken);

    // Asynchronously validate the uniqueness of the email address
    private async Task<bool> ValidateEmailAddress(string emailAddress, CancellationToken cancellationToken)
        => await _userReader.ValidateEmailAddressAsync(emailAddress, cancellationToken);

}
