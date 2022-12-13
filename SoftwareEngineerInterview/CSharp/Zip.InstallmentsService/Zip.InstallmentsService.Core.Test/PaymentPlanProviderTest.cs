using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Zip.InstallmentsService.Core.Implementation;
using Zip.InstallmentsService.Core.Interface;
using Zip.InstallmentsService.Data.Interface;
using Zip.InstallmentsService.Entity.Dto;

namespace Zip.InstallmentsService.Core.Test
{
    public class PaymentPlanProviderTest
    {
        private readonly PaymentPlanProvider _sut;
        private readonly Mock<IPaymentPlanRepository> _paymentPlanRepositoryMock = new Mock<IPaymentPlanRepository>();
        private readonly Mock<IInstallmentProvider> _installmentProviderMock = new Mock<IInstallmentProvider>();
        private readonly Mock<ILogger> _logger = new Mock<ILogger>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

        public PaymentPlanProviderTest()
        {
            _sut = new PaymentPlanProvider(_paymentPlanRepositoryMock.Object, _installmentProviderMock.Object, _logger.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetById_ShooudReturnPaymentPlan_WhenPaymentPlanExists()
        {

            //Arrange
            var paymentPlanId = Guid.NewGuid();

            //_paymentPlanRepositoryMock.Setup(x => x.GetByIdAsync(paymentPlanId))
            //    .Returns(paymentPlanDto);


            //Act
            var paymentPlan = _sut.GetByIdAsync(paymentPlanId);

            //Assert
            //Assert.Equal(paymentPlanId, paymentPlan.Id);
        }


        


    }
}
