

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
    public class CreateTransferOperation
    {
        private readonly IntegrationTestFactory<Program, BankingDbContext> _factory;

        private UriBuilder AccountURL;

        private UriBuilder AuthenticationURL;

        public CreateTransferOperation(IntegrationTestFactory<Program, BankingDbContext> factory)
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
        public async Task Transfer_ShouldReturnBadRequest_WhenReceiverAccountIdDoesNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginDTO = new LoginUserDTO()
            {
                UserName = "79529879916",
                Password = "TestUser3Password79529879916"
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
            var createWithdrawDTO = new CreateTransferMoneyDTO
            {
                Amount = 125,
                ReceiverAccountId = int.MaxValue
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createWithdrawDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/transfer", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);



            response = await client.GetAsync($"{AccountURL.ToString()}/transaction-history");

            response.EnsureSuccessStatusCode();
            responseContent = await response.Content.ReadAsStringAsync();
            var records = JsonConvert.DeserializeObject<IEnumerable<ReadRecordDTO>>(responseContent);

            Assert.NotNull(records);
            Assert.NotEmpty(records);
            Assert.Single(records);

            ReadRecordDTO record =  records.First();

            Assert.Equal("CreateAccount", record.OperationType);

        }

        [Fact]
        public async Task Transfer_ShouldReturnBothAccounts_WhenTransferIsValid()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginDTO = new LoginUserDTO()
            {
                UserName = "79529879916",
                Password = "TestUser3Password79529879916"
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
            var createWithdrawDTO = new CreateTransferMoneyDTO
            {
                Amount = 125,
                ReceiverAccountId = 7
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createWithdrawDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/transfer", content);

            // Assert
            response.EnsureSuccessStatusCode();
            responseContent = await response.Content.ReadAsStringAsync();
            var readAccountDTOs = JsonConvert.DeserializeObject<Tuple<ReadAccountDTO, ReadAccountDTO>>(responseContent);

            Assert.NotNull(readAccountDTOs);
            var firstReadAccountDTO = readAccountDTOs.Item1;
            var secondReadAccountDTO = readAccountDTOs.Item2;



            Assert.NotNull(firstReadAccountDTO);
            Assert.Equal(6, firstReadAccountDTO.Id);
            Assert.Equal(400 - 125, firstReadAccountDTO.Balance);
            Assert.Equal(500, firstReadAccountDTO.DailyLimit);
            Assert.Equal(250, firstReadAccountDTO.OperationLimit);
            Assert.Equal(10, firstReadAccountDTO.UserId);
            Assert.Equal(125, firstReadAccountDTO.DailySpend);


            Assert.NotNull(secondReadAccountDTO);
            Assert.Equal(7, secondReadAccountDTO.Id);
            Assert.Equal(1000 + 125, secondReadAccountDTO.Balance);
            Assert.Equal(500, secondReadAccountDTO.DailyLimit);
            Assert.Equal(250, secondReadAccountDTO.OperationLimit);
            Assert.Equal(11, secondReadAccountDTO.UserId);
            Assert.Equal(0, secondReadAccountDTO.DailySpend);

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
            Assert.Equal("Transfer", record.OperationType);
            Assert.Equal(125, record.Amount);
            Assert.Equal(10, record.UserId);
            Assert.Equal(firstReadAccountDTO.Id, record.AccountId);
            Assert.Equal(secondReadAccountDTO.Id, record.ReceiverAccountId);
            Assert.True(record.IsSuccessfull);
            Assert.Null(record.ErrorMessage);
            Assert.False(record.IsPending);

        }    

        [Fact]
        public async Task Transfer_ShouldReturnBadRequest_WhenTransferAmountExceedsOperationLimit()
        {
            // Arrange
            var client = _factory.CreateClient();

            var loginDTO = new LoginUserDTO()
            {
                UserName = "83585762228",
                Password = "TestUser3Password83585762228"
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
            var createWithdrawDTO = new CreateTransferMoneyDTO
            {
                Amount = 260,
                ReceiverAccountId = 9
            };

            // Serialize the createCommandDto to JSON
            jsonContent = JsonConvert.SerializeObject(createWithdrawDTO);
            content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act: Send a POST request to create a new command
            response = await client.PostAsync($"{AccountURL.ToString()}/transfer", content);

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
            Assert.Equal("Transfer", record.OperationType);
            Assert.Equal(260, record.Amount);
            Assert.Equal(12, record.UserId);
            Assert.True(record.AccountId > 0);
            Assert.Equal(9, record.ReceiverAccountId);
            Assert.False(record.IsSuccessfull);
            Assert.StartsWith("Operation limit exceeded for account with id", record.ErrorMessage);
            Assert.True(record.IsPending);

        }        
    }
}