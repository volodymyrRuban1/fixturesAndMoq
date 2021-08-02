// CartControllerTest.cs

using System;
using Services;
using Moq;
using NUnit.Framework;
using api.Controllers;
using System.Linq;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;


namespace test
{
    public class AutoDomainDataAttribute : AutoDataAttribute
    {
        public AutoDomainDataAttribute()
            : base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {

        }
    }

  public class Tests
  {
      private CartController controller;

      [Theory]
      [AutoDomainData]
      public void ShouldReturnCharged(
          [Frozen]Mock<IPaymentService> paymentServiceMock, [Frozen]Mock<ICartService> cartServiceMock,
          [Frozen]Mock<IShipmentService> shipmentServiceMock, [Frozen] Mock<ICard> cardMock,
          [Frozen]Mock<IAddressInfo> addressInfoMock, [Frozen]Mock<CartItem> cartItem, List<CartItem> items
          )
      {
          controller = new CartController(cartServiceMock.Object, paymentServiceMock.Object, shipmentServiceMock.Object);
          paymentServiceMock.Setup(p => p.Charge(It.IsAny<double>(), cardMock.Object)).Returns(true);
          cartItem.Setup(c => c.Price).Returns(1231);
          cartServiceMock.Setup(c => c.Items()).Returns(items.AsEnumerable());


          // act
          var result = controller.CheckOut(cardMock.Object, addressInfoMock.Object);

          // assert
          shipmentServiceMock.Verify(s => s.Ship(addressInfoMock.Object, items.AsEnumerable()), Times.Once());

          Assert.AreEqual("charged", result);
      }

      [Theory]
      [AutoDomainData]
        public void ShouldReturnNotCharged(
            [Frozen] Mock<IPaymentService> paymentServiceMock, Mock<ICartService> cartServiceMock,
            [Frozen] Mock<IShipmentService> shipmentServiceMock, [Frozen] Mock<ICard> cardMock,
            [Frozen] Mock<IAddressInfo> addressInfoMock, [Frozen] Mock<CartItem> cartItem, List<CartItem> items
          ) 
      {
          controller = new CartController(cartServiceMock.Object, paymentServiceMock.Object, shipmentServiceMock.Object);
            paymentServiceMock.Setup(p => p.Charge(It.IsAny<double>(), cardMock.Object)).Returns(false);

          // act
          var result = controller.CheckOut(cardMock.Object, addressInfoMock.Object);

          // assert
          shipmentServiceMock.Verify(s => s.Ship(addressInfoMock.Object, items.AsEnumerable()), Times.Never());
          Assert.AreEqual("not charged", result);
      }
  }
}