
using System.Net;
using System.Text;
using Banking.Persistance;
using Banking.Shared.DTOs.Request;
using Banking.Test.Fixtures;
using Newtonsoft.Json;

namespace Banking.Test.Integration.IntegrationTests
{
    [Collection("User Operations")]
    public class RegisterOperations
    {
        private readonly IntegrationTestFactory<Program, BankingDbContext> _factory;

        private UriBuilder URL;

        public RegisterOperations(IntegrationTestFactory<Program, BankingDbContext> factory)
        {
            _factory = factory;

            URL = new UriBuilder
            {
                Scheme = "http",
                Host = "localhost",
                Path = "api/users"
            };

        }

        [Fact]
        public async Task RegisterUser_ShouldReturn201Created_WhenDTOIsValid()
        {
            // Arrange
            var client = _factory.CreateClient();

            var createUserRegisterDTO = new CreateUserDTO
            {
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                Password = "testPassword64128",
                Email = "vysdll@gmail.com",
                UserName = "17813246384",
                PhoneNumber = "+905013252316",
                Roles = new string[] {"user"}
            };
            // Serialize the createCommandDto to JSON
            var jsonContent = JsonConvert.SerializeObject(createUserRegisterDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            // Act: Send a POST request to create a new command
            var response = await client.PostAsync(URL.ToString(), content);
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturn400_WhenNoEmailProvided()
        {
            // Arrange
            var client = _factory.CreateClient();

            var createUserRegisterDTO = new CreateUserDTO
            {
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                Password = "testPassword-64128",
                UserName = "Test2",
                Roles = new string[] {"user"}
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
        public async Task RegisterUser_ShouldReturn400Created_WhenNoInvalidTurkishId()
        {
            // Arrange
            var client = _factory.CreateClient();

            var createUserRegisterDTO = new CreateUserDTO
            {
                FirstName = "TestFirstName",
                LastName = "TestLastName",
                Password = "testPassword64128",
                UserName = "Test2",
                Email = "vysdll@gmail.com",
                PhoneNumber = "+905013252316",
                Roles = new string[] {"user"}
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