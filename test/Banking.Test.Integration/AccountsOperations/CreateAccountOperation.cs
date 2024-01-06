using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Banking.Persistance;
using Banking.Shared.DTOs.Request;
using Banking.Shared.DTOs.Response;
using Banking.Test.Fixtures;
using Newtonsoft.Json;

namespace Banking.Test.Integration.AccountsOperations
{
    [Collection("Account Operations")]
    public class CreateAccountOperation
    {
        private readonly IntegrationTestFactory<Program, BankingDbContext> _factory;

        private UriBuilder AccountURL;

        private UriBuilder AuthenticationURL;

        public CreateAccountOperation(IntegrationTestFactory<Program, BankingDbContext> factory)
        {
            _factory = factory;

            AuthenticationURL = new UriBuilder
            {
                Scheme = "http",
                Host = "localhost",
                Path = "api/users/login"
            };

            AccountURL = new UriBuilder
            {
                Scheme = "http",
                Host = "localhost",
                Path = "api/accounts"

            };
        }

        [Fact]
        public async Task CreateAccount_ShouldReturnForbidden_WhenAccessTokenIsAdmin()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginDTO = new LoginUserDTO()
            {
                UserName = "57416725928",
                Password = "TestAdminUser1Password24"
            };

             // Serialize the createCommandDto to JSON
            var jsonContent = JsonConvert.SerializeObject(loginDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            var response = await client.PostAsync(AuthenticationURL.ToString(), content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenDto = JsonConvert.DeserializeObject<ReadTokenDTO>(responseContent);

            Assert.NotNull(tokenDto);
            Assert.True(tokenDto.AccessToken.Length > 10);
            Assert.True(tokenDto.AccessToken.Length > 5);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);

            // Creating Account
            var createAccountDTO = new CreateAccountDTO
            {
                Balance = 250
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createAccountDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/create-account", content);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreateAccount_ShouldReturnForbidden_WhenAccessTokenIsSupport()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginDTO = new LoginUserDTO()
            {
                UserName = "27295417344",
                Password = "TestManagerPassword12"
            };

             // Serialize the createCommandDto to JSON
            var jsonContent = JsonConvert.SerializeObject(loginDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            var response = await client.PostAsync(AuthenticationURL.ToString(), content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenDto = JsonConvert.DeserializeObject<ReadTokenDTO>(responseContent);

            Assert.NotNull(tokenDto);
            Assert.True(tokenDto.AccessToken.Length > 10);
            Assert.True(tokenDto.AccessToken.Length > 5);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);

            // Creating Account
            var createAccountDTO = new CreateAccountDTO
            {
                Balance = 250
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createAccountDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/create-account", content);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreateAccount_ShouldReturnAccount_WhenAccessTokenIsUser()
        {
            // Arrange
            var client = _factory.CreateClient();

            int balance = 250;

            var loginDTO = new LoginUserDTO()
            {
                UserName = "14253994730",
                Password = "TestUser3Password19"
            };

             // Serialize the createCommandDto to JSON
            var jsonContent = JsonConvert.SerializeObject(loginDTO);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            var response = await client.PostAsync(AuthenticationURL.ToString(), content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenDto = JsonConvert.DeserializeObject<ReadTokenDTO>(responseContent);

            Assert.NotNull(tokenDto);
            Assert.True(tokenDto.AccessToken.Length > 10);
            Assert.True(tokenDto.AccessToken.Length > 5);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);

            // Creating Account
            var createAccountDTO = new CreateAccountDTO
            {
                Balance = balance
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createAccountDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/create-account", content);

            // Assert
            response.EnsureSuccessStatusCode();
            responseContent = await response.Content.ReadAsStringAsync();
            var createdAccount = JsonConvert.DeserializeObject<ReadAccountDTO>(responseContent);

            Assert.NotNull(createdAccount);
            Assert.NotEqual(0, createdAccount.Id);
            Assert.Equal(balance, createdAccount.Balance);
            Assert.Equal(500, createdAccount.DailyLimit);
            Assert.Equal(250, createdAccount.OperationLimit);
            Assert.Equal(6, createdAccount.UserId);


            // Check for CreateAccountRecord
            response = await client.GetAsync($"{AccountURL.ToString()}/transaction-history");

            response.EnsureSuccessStatusCode();
            responseContent = await response.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<IEnumerable<ReadRecordDTO>>(responseContent);

            Assert.NotNull(records);
            Assert.NotEmpty(records);
            Assert.Single(records);

            ReadRecordDTO record = records.Single();

            double diffInSeconds = Math.Abs((DateTime.UtcNow - record.TimeStamp).TotalSeconds);


            Assert.NotEqual(0, record.Id);
            Assert.True(diffInSeconds < 5);
            Assert.Equal("CreateAccount", record.OperationType);
            Assert.Equal(250, record.Amount);
            Assert.Equal(6, record.UserId);
            Assert.Equal(createdAccount.Id, record.AccountId);
            Assert.Null(record.ReceiverAccountId);
            Assert.True(record.IsSuccessfull);
            Assert.Null(record.ErrorMessage);
            Assert.False(record.IsPending);

        }

    }
}