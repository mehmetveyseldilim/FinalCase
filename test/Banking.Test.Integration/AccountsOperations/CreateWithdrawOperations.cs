
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
    public class CreateWithdrawOperations
    {
        private readonly IntegrationTestFactory<Program, BankingDbContext> _factory;

        private UriBuilder AccountURL;

        private UriBuilder AuthenticationURL;

        public CreateWithdrawOperations(IntegrationTestFactory<Program, BankingDbContext> factory)
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
        public async Task Withdraw_ShouldReturnBadRequest_WhenAccountBalanceInsufficient()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginDTO = new LoginUserDTO()
            {
                UserName = "14173994738",
                Password = "TestUser3Password15"
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
            var createWithdrawDTO = new CreateWithdrawDTO
            {
                Amount = 451
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createWithdrawDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/withdraw", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);



            response = await client.GetAsync($"{AccountURL.ToString()}/transaction-history");

            response.EnsureSuccessStatusCode();
            responseContent = await response.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<IEnumerable<ReadRecordDTO>>(responseContent);

            Assert.NotNull(records);
            Assert.NotEmpty(records);
            Assert.Equal(3, records.Count());

            ReadRecordDTO record =  records.OrderByDescending(r => r.Id).First();

            double diffInSeconds = Math.Abs((DateTime.UtcNow - record.TimeStamp).TotalSeconds);


            Assert.NotEqual(0, record.Id);
            Assert.True(diffInSeconds < 5);
            Assert.Equal("Withdrawal", record.OperationType);
            Assert.Equal(451, record.Amount);
            Assert.Equal(5, record.UserId);
            // Assert.Equal(3, record.AccountId);
            Assert.Null(record.ReceiverAccountId);
            Assert.False(record.IsSuccessfull);
            Assert.Equal("Insufficient funds for the withdrawal.", record.ErrorMessage);
            Assert.False(record.IsPending);

        }

        [Fact]
        public async Task Withdraw_ShouldReturnBadRequest_WhenWithdrawAmountExceedsOperationLimit()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginDTO = new LoginUserDTO()
            {
                UserName = "38346001948",
                Password = "TestUser3Password38346001948"
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
            var createWithdrawDTO = new CreateWithdrawDTO
            {
                Amount = 260
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createWithdrawDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/withdraw", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);



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
            Assert.Equal("Withdrawal", record.OperationType);
            Assert.Equal(260, record.Amount);
            Assert.Equal(9, record.UserId);
            Assert.Null(record.ReceiverAccountId);
            Assert.False(record.IsSuccessfull);
            Assert.StartsWith("Operation limit exceeded for account with ", record.ErrorMessage);
            Assert.True(record.IsPending);

        }

        [Fact]
        public async Task Withdraw_ShouldReturnAccount_WhenOperationIsSuccessfull()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginDTO = new LoginUserDTO()
            {
                UserName = "64058491142",
                Password = "TestUser3Password91142"
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
            var createWithdrawDTO = new CreateWithdrawDTO
            {
                Amount = 125
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createWithdrawDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/withdraw", content);

            // Assert
            response.EnsureSuccessStatusCode();
            responseContent = await response.Content.ReadAsStringAsync();
            var readAccountDTO = JsonConvert.DeserializeObject<ReadAccountDTO>(responseContent);

            Assert.NotNull(readAccountDTO);
            Assert.NotEqual(0, readAccountDTO.Id);
            Assert.Equal(1375, readAccountDTO.Balance);
            Assert.Equal(500, readAccountDTO.DailyLimit);
            Assert.Equal(250, readAccountDTO.OperationLimit);
            Assert.Equal(8, readAccountDTO.UserId);



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
            Assert.Equal("Withdrawal", record.OperationType);
            Assert.Equal(125, record.Amount);
            Assert.Equal(8, record.UserId);
            Assert.Equal(readAccountDTO.Id, record.AccountId);
            Assert.Null(record.ReceiverAccountId);
            Assert.True(record.IsSuccessfull);
            Assert.Null(record.ErrorMessage);
            Assert.False(record.IsPending);

        }
    }
}