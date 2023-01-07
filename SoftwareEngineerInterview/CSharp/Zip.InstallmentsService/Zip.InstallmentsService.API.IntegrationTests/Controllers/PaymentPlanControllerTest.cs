using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Xunit;
using Zip.InstallmentsService.API.IntegrationTests.Models;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Entity.V1.Request;
using Zip.InstallmentsService.Entity.V1.Response;

namespace Zip.InstallmentsService.API.IntegrationTests.Controllers
{
    /// <summary>
    /// Class which defines all the XUnit Test Cases of the PaymentPlanController
    /// </summary>
    public class PaymentPlanControllerTest
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Intialization in Constructor 
        /// </summary>
        public PaymentPlanControllerTest()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configuration = (new ConfigurationBuilder().SetBasePath(projectDir).AddJsonFile("appsettings.json")).Build();
            var builder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .UseEnvironment("Development")
                .UseStartup<TestStartup>();
            TestServer testServer = new TestServer(builder);
            _client = testServer.CreateClient();
        }


        /// <summary>
        /// Test case for CreatePaymentPlanAsync Method when valid order amount is provided
        /// </summary>
        [Fact]
        public async void CreatePaymentPlanAsync_ShouldReturnNewCreatedPaymentPlan_WhenValidOrderAmountGiven()
        {
            //Arrange
            var paymentPlanId = Guid.Parse("515d0e18-6aa8-458a-9992-3ae46f21d9fc");
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

            //Prepare request object
            string date = "2022-01-01";
            var createPaymentPlanRequest = this.PrepareCreatePaymentPlanRequestObject(paymentPlanId, userId, date, 100, 4, 14);

            //Act
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "api/paymentplan")
            {
                Content = new JsonContent(createPaymentPlanRequest)
            };

            HttpResponseMessage response = await _client.SendAsync(postRequest);
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<PaymentPlanResponse>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.True(result?.Installments?.Any(k => k.Amount == 25 && k.DueDate == Convert.ToDateTime("2022-01-01")));
            Assert.True(result?.Installments?.Any(k => k.Amount == 25 && k.DueDate == Convert.ToDateTime("2022-01-15")));
            Assert.True(result?.Installments?.Any(k => k.Amount == 25 && k.DueDate == Convert.ToDateTime("2022-01-29")));
            Assert.True(result?.Installments?.Any(k => k.Amount == 25 && k.DueDate == Convert.ToDateTime("2022-02-12")));

        }

        /// <summary>
        /// Test case for CreatePaymentPlanAsync Method when in-valid order amount is provided
        /// </summary>
        [Fact]
        public async void CreatePaymentPlanAsync_ShouldReturnNothing_WhenInValidOrderAmountGiven()
        {
            //Arrange
            var paymentPlanId = Guid.NewGuid();
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            string date = "2022-01-01";
            var createPaymentPlanRequest = this.PrepareCreatePaymentPlanRequestObject(paymentPlanId, userId, date, 0, 4, 14);

            //Act
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "api/paymentplan")
            {
                Content = new JsonContent(createPaymentPlanRequest)
            };

            HttpResponseMessage response = await _client.SendAsync(postRequest);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Test case for CreatePaymentPlanAsync Method when in-valid input data is provided
        /// </summary>
        [Fact]
        public async void CreatePaymentPlanAsync_ShouldReturnNothing_WhenInValidInputGiven()
        {
            //Arrange
            var paymentPlanId = Guid.NewGuid();
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            string date = "2022-01-01";
            var createPaymentPlanRequest = this.PrepareCreatePaymentPlanRequestObject(paymentPlanId, userId, date, 0, 0, 0);

            //Act
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "api/paymentplan")
            {
                Content = new JsonContent(createPaymentPlanRequest)
            };

            HttpResponseMessage response = await _client.SendAsync(postRequest);

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        /// <summary>
        /// Test case for GetByIdAsync Method when Payment Plan exist
        /// </summary>
        [Fact]
        public async void GetByIdAsync_ShouldReturnPaymentPlan_WhenPaymentPlanExists()
        {
            //Arrange
            var paymentPlanId = Guid.Parse("515d0e18-6aa8-458a-9992-3ae46f21d9fc");
            var userId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");

            //PreSetup or post api call, as this is In-Memory database, first let us make create api call so we get record for get call
            //but in real time this code will not be needed
            var createPaymentPlanRequest = this.PrepareCreatePaymentPlanRequestObject(paymentPlanId, userId, "2022-01-01", 100, 4, 14);
            HttpRequestMessage postRequest = new HttpRequestMessage(HttpMethod.Post, "api/paymentplan")
            {
                Content = new JsonContent(createPaymentPlanRequest)
            };
            await _client.SendAsync(postRequest);


            //Act
            HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, $"api/paymentplan/{paymentPlanId}");
            HttpResponseMessage response = await _client.SendAsync(getRequest);
            var result = JsonConvert.DeserializeObject<PaymentPlanResponse>(await response.Content.ReadAsStringAsync());

            //Assert
            Assert.Equal(paymentPlanId, result.Id);
        }

        /// <summary>
        /// Test case for GetByIdAsync Method when Payment Plan does not exist
        /// </summary>
        [Fact]
        public async void GetByIdAsync_ShouldReturnNothing_WhenPaymentPlanDoesNotExist()
        {
            //Arrange
            var paymentPlanId = Guid.NewGuid();

            //Act
            HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, $"api/paymentplan/{paymentPlanId}");
            HttpResponseMessage response = await _client.SendAsync(getRequest);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        private CreatePaymentPlanRequest PrepareCreatePaymentPlanRequestObject(Guid id, Guid userId, string date, decimal amount, int noOfInstallments, int frequencyInDays)
        {
            CreatePaymentPlanRequest createPaymentPlanRequest = new CreatePaymentPlanRequest()
            {
                Id = id,
                UserId = userId,
                PurchaseDate = Convert.ToDateTime(date),
                PurchaseAmount = amount,
                NoOfInstallments = noOfInstallments,
                FrequencyInDays = frequencyInDays
            };

            return createPaymentPlanRequest;
        }

    }
}
