
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
    public class CreateDepositOperation
    {
        private readonly IntegrationTestFactory<Program, BankingDbContext> _factory;

        private UriBuilder AccountURL;

        private UriBuilder AuthenticationURL;

        public CreateDepositOperation(IntegrationTestFactory<Program, BankingDbContext> factory)
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
        public async Task Deposit_ShouldReturnBadRequest_WhenNoAccountCreated()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginDTO = new LoginUserDTO()
            {
                UserName = "16173294752",
                Password = "TestUser3Password27"
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
            var createDepositDTO = new CreateDepositDTO
            {
                Amount = 250
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createDepositDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/deposit", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Deposit_ShouldReturnForbidden_WhenRoleIsNotUser()
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
            var createDepositDTO = new CreateDepositDTO
            {
                Amount = 250
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createDepositDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/deposit", content);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Deposit_ShouldReturnValidReadAccountDTO_WhenUserTokenIsValidAndAccountExists()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginDTO = new LoginUserDTO()
            {
                UserName = "95745395412",
                Password = "TestUser1Password11"
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
            var createDepositDTO = new CreateDepositDTO
            {
                Amount = 250
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createDepositDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/deposit", content);

            // Assert
            response.EnsureSuccessStatusCode();
            responseContent = await response.Content.ReadAsStringAsync();
            var readAccountDTO = JsonConvert.DeserializeObject<ReadAccountDTO>(responseContent);

            Assert.NotNull(readAccountDTO);
            Assert.NotEqual(0, readAccountDTO.Id);
            Assert.Equal(500, readAccountDTO.Balance);
            Assert.Equal(500, readAccountDTO.DailyLimit);
            Assert.Equal(250, readAccountDTO.OperationLimit);
            Assert.Equal(3, readAccountDTO.UserId);


            // Check for CreateAccountRecord
            response = await client.GetAsync($"{AccountURL.ToString()}/transaction-history");

            response.EnsureSuccessStatusCode();
            responseContent = await response.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<IEnumerable<ReadRecordDTO>>(responseContent);

            Assert.NotNull(records);
            Assert.NotEmpty(records);
            Assert.Equal(2, records.Count());

            ReadRecordDTO record =  records.OrderByDescending(r => r.Id).First();

            double diffInSeconds = Math.Abs((DateTime.UtcNow - record.TimeStamp).TotalSeconds);


            Assert.NotEqual(0, record.Id);
            Assert.True(diffInSeconds < 5);
            Assert.Equal("Deposit", record.OperationType);
            Assert.Equal(250, record.Amount);
            Assert.Equal(3, record.UserId);
            Assert.Equal(readAccountDTO.Id, record.AccountId);
            Assert.Null(record.ReceiverAccountId);
            Assert.True(record.IsSuccessfull);
            Assert.Null(record.ErrorMessage);
            Assert.False(record.IsPending);

        }
    }
}