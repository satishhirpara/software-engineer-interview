using AutoMapper;
using AutoMapper.Configuration;
using Castle.Core.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Zip.InstallmentsService.API.Controllers;
using Zip.InstallmentsService.API.Helper;
using Zip.InstallmentsService.Core.Mapping;
using Zip.InstallmentsService.Core.Implementation;
using Zip.InstallmentsService.Core.Interface;
using Zip.InstallmentsService.Data.Interface;
using Zip.InstallmentsService.Data.Models;
using Zip.InstallmentsService.Entity;
using Zip.InstallmentsService.Entity.Dto;

namespace Zip.InstallmentsService.API.Test
{
    [TestClass]
    public class PaymentPlanControllerTest
    {
        private IMapper _mapper;
        //public PaymentPlanControllerTest()
        //{
        //    if (_mapper == null)
        //    {
        //        var mappingConfig = new MapperConfiguration(mc =>
        //        {
        //            mc.AddProfile(new PaymentPlanProfile());
        //        });
        //        IMapper mapper = mappingConfig.CreateMapper();
        //        _mapper = mapper;
        //    }
        //}

        [TestInitialize]
        public void TestInit()
        {
            //var myProfile = new PaymentPlanProfile();

            //MapperConfigurationExpression expression = new MapperConfigurationExpression();
            //expression.CreateMap<CreatePaymentPlanRequest, PaymentPlanDto>();
            //expression.CreateMap<PaymentPlan, PaymentPlanDto>();
            //expression.CreateMap<Installment, InstallmentDto>();

            //var configuration = new MapperConfiguration(expression);
            //_mapper = new Mapper(configuration);
        }


        [TestMethod]
        public void WhenCreatePaymentPlan_WithValidOrderAmount_ShouldReturnValidPaymentPlan()
        {
            //Mock objects
            var mockPaymentPlanRepository = new Mock<IPaymentPlanRepository>();
            var mockInstallmentProvider = new Mock<IInstallmentProvider>();

            // Set up mockInstallmentProvider
            //Guid id = Guid.NewGuid();
            //CreatePaymentPlanRequest request = this.MockRequestObject(id, Guid.Parse("504A683D-B4C3-4770-962B-4B5F3F89BB91"), "2022-01-01", 100, 4, 14);
            //var paymentPlanDto = _mapper.Map<PaymentPlanDto>(request);
            //var response = this.MockResponseObject(id, Guid.Parse("504A683D-B4C3-4770-962B-4B5F3F89BB91"), "2022-01-01", 100, 4, 14);
            //response.Installments = this.MockInstallments(paymentPlanDto.Id, paymentPlanDto.PurchaseAmount);
            //mockInstallmentProvider.Setup(p => p.CalculateInstallments(paymentPlanDto)).Returns(response.Installments);

            //// Arrange
            //IPaymentPlanProvider paymentPlanProvider = new PaymentPlanProvider(mockPaymentPlanRepository.Object, mockInstallmentProvider.Object, _mapper);
            //PaymentPlanDto responseObj = this.MockResponseObject(request.Id, request.UserId, request.PurchaseDate, request.NoOfInstallments, request.FrequencyInDays);

            ////Act
            //var result = paymentPlanProvider.Create(request);

            //// Assert
            //Assert.AreNotEqual(null, result);
        }

        [TestMethod]
        public void WhenCreatePaymentPlan_WithInValidRequest_ShouldReturnNull()
        {
            ////Mock objects
            //var mockPaymentPlanProvider = new Mock<IPaymentPlanProvider>().Object;
            //var mockLogger = new Mock<ILogger>().Object;
            //PaymentPlanController paymentPlanController = new PaymentPlanController(mockPaymentPlanProvider, mockLogger);

            //// Arrange
            //CreatePaymentPlanDto request = this.MockRequestObject(0, 0, 0);

            ////Act
            //var result = mockPaymentPlanProvider.Create(request);

            //// Assert
            //Assert.AreEqual(null, result);
        }

        [TestMethod]
        [ExpectedException(typeof(AppException), AppConstants.BadRequest)]
        public void WhenGetPaymentPlan_WithInValidId_ShouldReturnBadRequest()
        {
            //Mock objects
            var mockPaymentPlanProvider = new Mock<IPaymentPlanProvider>().Object;
            var mockLogger = new Mock<ILogger>().Object;
            PaymentPlanController paymentPlanController = new PaymentPlanController(mockPaymentPlanProvider, mockLogger);

            // Arrange
            Guid request = Guid.Parse("00000000-0000-0000-0000-000000000000");

            //Act
            var result = paymentPlanController.Get(request);

            // Assert
            Assert.AreNotEqual(null, result);
        }


        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException), AppConstants.NoRecordFound)]
        public void WhenGetPaymentPlan_WithValidIdWhichHasNoPaymentPlan_ShouldReturnNotFound()
        {
            //Mock objects
            var mockPaymentPlanProvider = new Mock<IPaymentPlanProvider>().Object;
            var mockLogger = new Mock<ILogger>().Object;
            PaymentPlanController paymentPlanController = new PaymentPlanController(mockPaymentPlanProvider, mockLogger);

            // Arrange
            Guid request = Guid.Parse("404AA08A-D7C9-49D9-A79C-7333B1B4C3D1");

            //Act
            var result = paymentPlanController.Get(request);

            // Assert
            Assert.AreNotEqual(null, result);
        }


        #region Private methods

        private PaymentPlanDto MockResponseObject(Guid id, Guid userId, string date, decimal amount, int noOfInstallments, int frequencyInDays)
        {
            PaymentPlanDto response = new PaymentPlanDto()
            {
                Id = id,
                UserId = userId,
                PurchaseDate = Convert.ToDateTime(date),
                PurchaseAmount = amount,
                NoOfInstallments = noOfInstallments,
                FrequencyInDays = frequencyInDays
            };

            return response;
        }

        private List<InstallmentDto> MockInstallments(Guid paymentPlanId, decimal amount)
        {
            List<InstallmentDto> installmentDtos = new List<InstallmentDto>()
            {
                new InstallmentDto(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-01"), Amount = amount },
                new InstallmentDto(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-15"), Amount = amount },
                new InstallmentDto(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-01-29"), Amount = amount },
                new InstallmentDto(){ Id = Guid.NewGuid(), PaymentPlanId = paymentPlanId, DueDate = Convert.ToDateTime("2022-02-12"), Amount = amount }
            };

            return installmentDtos;
        }

        #endregion


    }
}
