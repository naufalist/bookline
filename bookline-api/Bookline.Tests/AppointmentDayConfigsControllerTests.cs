using Bookline.Api.Controllers;
using Bookline.Application.Interfaces;
using Bookline.Domain.DTOs;
using Bookline.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Bookline.Tests
{
    public class AppointmentDayConfigsControllerTests
    {
        private readonly Mock<IAppointmentDayConfigService> _mockService;
        private readonly AppointmentDayConfigsController _controller;

        public AppointmentDayConfigsControllerTests()
        {
            _mockService = new Mock<IAppointmentDayConfigService>();
            _controller = new AppointmentDayConfigsController(_mockService.Object);
        }

        [Fact]
        public async Task GetConfigList_ReturnsOkResult_WithData()
        {
            // Arrange
            List<AppointmentDayConfigResponseDto> dummyData = new List<AppointmentDayConfigResponseDto>
            {
                new AppointmentDayConfigResponseDto
                {
                    Date = "2025-05-09",
                    MaxAppointments = 10
                }
            };

            _mockService
                .Setup(service => service.GetAppointmentDayConfigsAsync())
                .ReturnsAsync(dummyData);

            // Act
            IActionResult result = await _controller.GetConfigList();

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(okResult.Value);
            Assert.Equal(dummyData, response.Data);
        }

        [Fact]
        public async Task GetConfigByDate_ReturnsOkResult_WithData()
        {
            // Arrange
            string date = "2025-05-09";
            AppointmentDayConfigResponseDto dummyResponse = new AppointmentDayConfigResponseDto
            {
                Date = date,
                MaxAppointments = 15
            };

            _mockService
                .Setup(service => service.GetAppointmentDayConfigByDateAsync(date))
                .ReturnsAsync(dummyResponse);

            // Act
            IActionResult result = await _controller.GetConfigByDate(date);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(okResult.Value);
            Assert.Equal(dummyResponse, response.Data);
        }

        [Fact]
        public async Task PostConfig_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            AppointmentDayConfigRequestDto request = new AppointmentDayConfigRequestDto
            {
                Date = "2025-05-10",
                MaxAppointments = 20
            };
            AppointmentDayConfigResponseDto responseDto = new AppointmentDayConfigResponseDto
            {
                Date = request.Date,
                MaxAppointments = 20
            };

            _mockService
                .Setup(service => service.PostAppointmentDayConfigAsync(request))
                .ReturnsAsync(responseDto);

            // Act
            IActionResult result = await _controller.PostConfig(request);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(okResult.Value);
            Assert.Equal(responseDto, response.Data);
        }

        [Fact]
        public async Task PatchConfig_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();
            AppointmentDayConfigRequestDto request = new AppointmentDayConfigRequestDto
            {
                Date = "2025-05-11",
                MaxAppointments = 25
            };
            AppointmentDayConfigResponseDto responseDto = new AppointmentDayConfigResponseDto
            {
                Date = request.Date,
                MaxAppointments = 25
            };

            _mockService
                .Setup(service => service.PartialUpdateAppointmentDayConfigAsync(id, request))
                .ReturnsAsync(responseDto);

            // Act
            IActionResult result = await _controller.PatchConfig(id, request);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(okResult.Value);
            Assert.Equal(responseDto, response.Data);
        }

        [Fact]
        public async Task DeleteConfig_ValidId_ReturnsOkResult()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();
            _mockService
                .Setup(service => service.HardDeleteAppointmentDayConfigAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            IActionResult result = await _controller.DeleteConfig(id);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(okResult.Value);
            Assert.Equal("Appointment day config deleted successfully.", response.Message);
        }

        [Fact]
        public async Task GetConfigList_WhenThrowsException_Returns500()
        {
            // Arrange
            _mockService
                .Setup(service => service.GetAppointmentDayConfigsAsync())
                .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            IActionResult result = await _controller.GetConfigList();

            // Assert
            ObjectResult error = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, error.StatusCode);
        }

        [Fact]
        public async Task PostConfig_WhenBadRequestExceptionThrown_ReturnsBadRequest()
        {
            // Arrange
            AppointmentDayConfigRequestDto request = new AppointmentDayConfigRequestDto
            {
                Date = "2025-05-10",
                MaxAppointments = 30
            };

            _mockService
                .Setup(service => service.PostAppointmentDayConfigAsync(request))
                .ThrowsAsync(new BooklineBadRequestException("Invalid config"));

            // Act
            IActionResult result = await _controller.PostConfig(request);

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(badRequest.Value);
            Assert.Equal("Invalid config", response.Message);
        }
    }
}