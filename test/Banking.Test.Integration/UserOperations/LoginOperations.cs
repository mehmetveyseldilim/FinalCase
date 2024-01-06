

using System.Net;
using System.Text;
using Banking.Persistance;
using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;
using Banking.Test.Fixtures;
using Newtonsoft.Json;

namespace Banking.Test.Integration.IntegrationTests
{
    [Collection("User Operations")]
    public class LoginOperations
    {
        private readonly IntegrationTestFactory<Program, BankingDbContext> _factory;

        private UriBuilder URL;

        public LoginOperations(IntegrationTestFactory<Program, BankingDbContext> factory)
        {
            _factory = factory;

            URL = new UriBuilder
            {
                Scheme = "http",
                Host = "localhost",
                Path = "api/users/login"
            };
        }

        [Fact]
        public async Task Authenticate_ShouldReturnAccessandRefreshToken_WhenCredentialsAreValid()
        {
            // Arrange
            var client = _factory.CreateClient();

            var createUserRegisterDTO = new LoginUserDTO
            {
                UserName = "57416725928",
                Password = "TestAdminUser1Password24"
            };

            // Serialize the createCommandDto to JSON
            var jsonContent = JsonConvert.SerializeObject(createUserRegisterDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            var response = await client.PostAsync(URL.ToString(), content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenDto = JsonConvert.DeserializeObject<ReadTokenDTO>(responseContent);


            Assert.NotNull(tokenDto);
            Assert.True(tokenDto.AccessToken.Length > 10);
            Assert.True(tokenDto.AccessToken.Length > 5);
        }

        [Fact]
        public async Task Authenticate_ShouldReturnBadRequest_WhenPasswordIsWrong()
        {
            // Arrange
            var client = _factory.CreateClient();

            var createUserRegisterDTO = new LoginUserDTO
            {
                UserName = "57416725928",
                Password = "TestAdminUser"
            };

            // Serialize the createCommandDto to JSON
            var jsonContent = JsonConvert.SerializeObject(createUserRegisterDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            var response = await client.PostAsync(URL.ToString(), content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  
        }

        [Fact]
        public async Task Authenticate_ShouldReturnBadRequest_WhenUserNameIsWrong()
        {
            // Arrange
            var client = _factory.CreateClient();

            var createUserRegisterDTO = new LoginUserDTO
            {
                UserName = "57416725928",
                Password = "TestAdmin1Password25",
            };

            // Serialize the createCommandDto to JSON
            var jsonContent = JsonConvert.SerializeObject(createUserRegisterDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            var response = await client.PostAsync(URL.ToString(), content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  
        }


    }
}