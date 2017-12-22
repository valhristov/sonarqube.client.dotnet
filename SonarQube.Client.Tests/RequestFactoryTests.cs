using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SonarQube.Client.Tests
{
    [TestClass]
    public class RequestFactoryTests
    {
        private RequestFactory requestFactory;

        [TestInitialize]
        public void MyTestMethod()
        {
            requestFactory = new RequestFactory();
        }

        [TestMethod]
        public void Create_No_Registrations()
        {
            // Arrange
            var action = new Action(() => requestFactory.Create<IDummyRequest>());

            // Act and Assert
            action.ShouldThrow<InvalidOperationException>().And
                .Message.Should().Be($"Could not find implementation for '{nameof(IDummyRequest)}'.");
        }

        [TestMethod]
        public void Create_Connected_To_Older_SonarQube()
        {
            // Arrange
            requestFactory.RegisterRequest<IDummyRequest, DummyRequestImpl>("3.3", () => new DummyRequestImpl());

            // Act and Assert
            var action = new Action(() => requestFactory.Create<IDummyRequest>("1.0"));
            action.ShouldThrow<InvalidOperationException>().And
                .Message.Should().Be($"Could not find compatible implementation of '{nameof(IDummyRequest)}' for SonarQube 1.0.");
        }

        [TestMethod]
        public void Create_SameVersion()
        {
            // Arrange
            var request_3_3 = new DummyRequestImpl();
            requestFactory.RegisterRequest<IDummyRequest, DummyRequestImpl>("3.3", () => request_3_3);

            // Act
            var result = requestFactory.Create<IDummyRequest>("3.3");

            // Assert
            result.Should().Be(request_3_3);
        }

        [TestMethod]
        public void Create_LargerVersion()
        {
            // Arrange
            var request_3_3 = new DummyRequestImpl();
            requestFactory.RegisterRequest<IDummyRequest, DummyRequestImpl>("3.3", () => request_3_3);

            // Act
            var result = requestFactory.Create<IDummyRequest>("7.0");

            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void Create_LongerVersionNumber()
        {
            // Arrange
            var request_3_3 = new DummyRequestImpl();
            requestFactory.RegisterRequest<IDummyRequest, DummyRequestImpl>("3.3", () => request_3_3);

            // Act
            var result = requestFactory.Create<IDummyRequest>("3.3.1.1234");

            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void Create_VersionNull_Returns_Latest_Implementation()
        {
            // Arrange
            var request_3_3 = new DummyRequestImpl();
            requestFactory.RegisterRequest<IDummyRequest, DummyRequestImpl>("3.3", () => request_3_3);
            var request_5_5 = new DummyRequestImpl();
            requestFactory.RegisterRequest<IDummyRequest, DummyRequestImpl>("5.5", () => request_5_5);

            // Act
            var result = requestFactory.Create<IDummyRequest>();

            // Assert
            result.Should().Be(request_5_5);
        }

        public interface IDummyRequest : IRequestBase { }

        public class DummyRequestImpl : IDummyRequest { }
    }
}
