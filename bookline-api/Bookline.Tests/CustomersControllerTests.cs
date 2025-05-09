using Bookline.Api.Controllers;
using Bookline.Application.Interfaces;
using Bookline.Domain.DTOs;
using Bookline.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Bookline.Tests
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomerService> _mockService;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mockService = new Mock<ICustomerService>();
            _controller = new CustomersController(_mockService.Object);
        }

        [Fact]
        public async Task Register_ValidRequest_ReturnsOk()
        {
            // Arrange
            CustomerRequestDto request = new CustomerRequestDto
            {
                FullName = "Naufal test",
                Email = "naufal@gmail.com",
                Password = "123123"
            };

            _mockService
                .Setup(service => service.PostCustomerAsync(request))
                .Returns(Task.CompletedTask);

            // Act
            IActionResult result = await _controller.Register(request);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(okResult.Value);
            Assert.Equal("Registration successfully.", response.Message);
            Assert.Equal(request, response.Data);
        }

        [Fact]
        public async Task Register_ServiceThrowsBadRequest_ReturnsBadRequest()
        {
            // Arrange
            CustomerRequestDto request = new CustomerRequestDto
            {
                FullName = "Naufal test",
                Email = "testinvalidemail",
                Password = "123123"
            };

            _mockService
                .Setup(service => service.PostCustomerAsync(request))
                .ThrowsAsync(new BooklineBadRequestException("Invalid data"));

            // Act
            IActionResult result = await _controller.Register(request);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(badRequest.Value);
            Assert.Equal("Invalid data", response.Message);
        }

        [Fact]
        public async Task Register_ServiceThrowsNotFound_ReturnsNotFound()
        {
            // Arrange
            CustomerRequestDto request = new CustomerRequestDto
            {
                FullName = "Naufal test",
                Email = "notfound@testmail.com",
                Password = "123123"
            };

            _mockService
                .Setup(service => service.PostCustomerAsync(request))
                .ThrowsAsync(new BooklineNotFoundException("Customer not found"));

            // Act
            IActionResult result = await _controller.Register(request);

            // Assert
            NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(notFoundResult.Value);
            Assert.Equal("Customer not found", response.Message);
        }

        [Fact]
        public async Task Register_ServiceThrowsGeneralException_ReturnsInternalServerError()
        {
            // Arrange
            CustomerRequestDto request = new CustomerRequestDto
            {
                FullName = "Naufal test",
                Email = "error@example.com",
                Password = "123123"
            };

            _mockService
                .Setup(service => service.PostCustomerAsync(request))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            IActionResult result = await _controller.Register(request);

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(objectResult.Value);
            Assert.Equal("Unexpected error", response.Message);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
        }
    }
}