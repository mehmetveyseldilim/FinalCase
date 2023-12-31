using Banking.API.Validations;
using Banking.Shared.DTOs.Request;

namespace Banking.Test.API.ValidationTests
{
    public class CreateUserDTOValidatorTests
    {
        private readonly CreateUserDTOValidator _validator;

        public CreateUserDTOValidatorTests()
        {
            _validator = new CreateUserDTOValidator();
        }

        [Fact]
        public void Should_Validate_Required_Fields()
        {
            // Arrange
            var dto = new CreateUserDTO();

            // Act
            var result = _validator.Validate(dto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(9, result.Errors.Count);

            var expectedErrorMessages = new [] 
            {
                "First Name is required",
                "Last Name is required",
                "Turkish Id is required",
                "Password is required",
                "Email is required",
                "Phone Number is required",
                "At least one role is required",
                "User Name must be a string consisting of 11 numbers",
                "Invalid password format"
            };

            foreach (var errorMessage in expectedErrorMessages)
            {
                Assert.Contains(errorMessage, result.Errors.Select(e => e.ErrorMessage));
            }

            // Assert.True(expectedErrorMessages.All(expected => result.Errors.Any(error => string.Equals(error.ErrorMessage, expected))));
        }

        [Fact]
        public void Should_Validate_UserName_Format()
        {
            // Arrange
            var invalidUsernames = new[] { "invalidUsername", "1234567890123", "123ab456789" };

            foreach (var username in invalidUsernames)
            {
                var dto = new CreateUserDTO 
                { 
                    UserName = username,
                    FirstName = "Test",
                    LastName = "Test",
                    Password = "testPassword64128",
                    Email = "veyselusser@gmail.com",
                    PhoneNumber = "5538654218",
                    Roles = new string[] { "User" }
                
                };

                // Act
                var result = _validator.Validate(dto);

                // Assert
                Assert.False(result.IsValid);
                Assert.Single(result.Errors);
                Assert.Equal("User Name must be a string consisting of 11 numbers", result.Errors.First().ErrorMessage);
            }

            
        }

        [Fact]
        public void ShouldValidate_WhenUserNameValidFormat()
        {
            var validUsername = "12345678901";
            var dtoWithValidUsername = new CreateUserDTO 
            { 
                UserName = validUsername,
                FirstName = "Test",
                LastName = "Test",
                Password = "testPassword64128",
                Email = "veyselusser@gmail.com",
                PhoneNumber = "5538654218",
                Roles = new string[] { "User", "Admin"}
                
            };

            // Act
            var resultWithValidUsername = _validator.Validate(dtoWithValidUsername);

            // Assert
            Assert.True(resultWithValidUsername.IsValid);
        }

        [Fact]
        public void Should_Validate_Password_Format()
        {
            // Arrange
            var invalidPasswords = new[] { "invalidPassword", "1234567890", "Password123!" };
            foreach (var password in invalidPasswords)
            {
                var dto = new CreateUserDTO 
                { 
                    UserName = "16813246382",
                    FirstName = "Test",
                    LastName = "Test",
                    Email = "veyselusser@gmail.com",
                    PhoneNumber = "5538654218",
                    Roles = new string[] { "User", "Admin"},
                    Password = password 
                };

                // Act
                var result = _validator.Validate(dto);

                // Assert
                Assert.False(result.IsValid);
                Assert.Single(result.Errors);
                Assert.Equal("Invalid password format", result.Errors.First().ErrorMessage);
            }


        }

        [Fact]
        public void ShouldValidate_Password_WhenPasswordIsValid()
        {
            var validPassword = "Password12345";
            var dtoWithValidPassword = new CreateUserDTO 
            { 
                Password = validPassword,
                UserName = "16813246382",
                FirstName = "Test",
                LastName = "Test",
                Email = "veyselusser@gmail.com",
                PhoneNumber = "5538654218",
                Roles = new string[] { "User", "Admin"},

            };

            // Act
            var resultWithValidPassword = _validator.Validate(dtoWithValidPassword);

            // Assert
            Assert.True(resultWithValidPassword.IsValid);
        }

        [Fact]
        public void Should_Validate_Email_Address()
        {
            // Arrange
            var invalidEmails = new[] { "invalid@@email", "userexample", "user@example.com@" };

            foreach (var email in invalidEmails)
            {
                var dto = new CreateUserDTO 
                { 
                    Email = email,
                    UserName = "16813246382",
                    FirstName = "Test",
                    LastName = "Test",
                    PhoneNumber = "5538654218",
                    Password = "TestPassword12",
                    Roles = new string[] { "User"},
                };

                // Act
                var result = _validator.Validate(dto);

                // Assert
                Assert.False(result.IsValid);
                Assert.Single(result.Errors);
                Assert.Equal("Invalid Email Address", result.Errors.First().ErrorMessage);
            }


        }

        public void ShouldValidate_Email_WhenEmailFormatIsValid()
        {
            var validEmail = "user@example.com";
            var dtoWithValidEmail = new CreateUserDTO { Email = validEmail };

            // Act
            var resultWithValidEmail = _validator.Validate(dtoWithValidEmail);

            // Assert
            Assert.True(resultWithValidEmail.IsValid);
        }

       
    }
}