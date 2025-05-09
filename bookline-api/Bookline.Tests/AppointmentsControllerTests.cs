using Bookline.Api.Controllers;
using Bookline.Application.Interfaces;
using Bookline.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace Bookline.Tests
{
    public class AppointmentsControllerTests
    {
        private readonly Mock<IAppointmentService> _mockService;
        private readonly AppointmentsController _controller;

        public AppointmentsControllerTests()
        {
            _mockService = new Mock<IAppointmentService>();
            _controller = new AppointmentsController(_mockService.Object);
        }

        [Fact]
        public async Task GetAppointmentList_ReturnsOkResult_WithData()
        {
            // Arrange
            List<AppointmentResponseDto> dummyData = new List<AppointmentResponseDto>
            {
                new AppointmentResponseDto {
                    Date = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Token = "BL001"
                }
            };

            _mockService
                .Setup(service => service.GetAppointments())
                .ReturnsAsync(dummyData);

            // Act
            IActionResult result = await _controller.GetAppointmentList();

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(okResult.Value);
            Assert.NotNull(response.Data);
        }

        [Fact]
        public async Task GetAppointmentByDate_ReturnsOkResult_WithFilteredData()
        {
            // Arrange
            DateTime date = new DateTime(2025, 5, 10);
            List<AppointmentResponseDto> dummyData = new List<AppointmentResponseDto>
            {
                new AppointmentResponseDto {
                    Date = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Token = "BL002"
                }
            };

            _mockService
                .Setup(service => service.GetAppointmentsByDateAsync(date.ToString()))
                .ReturnsAsync(dummyData);

            // Act
            IActionResult result = await _controller.GetAppointmentByDate(date.ToString());

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(okResult.Value);
            Assert.NotNull(response.Data);
        }

        [Fact]
        public async Task PostAppointment_ReturnsOkResult_WithToken()
        {
            // Arrange
            AppointmentRequestDto request = new AppointmentRequestDto
            {
                Email = "test@example.com",
                Date = "2025-05-08"
            };

            AppointmentResponseDto expectedResponse = new AppointmentResponseDto
            {
                Date = request.Date,
                Token = "BL005"
            };

            _mockService
                .Setup(service => service.PostAppointmentAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            IActionResult result = await _controller.PostAppointment(request);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            GlobalResponse response = Assert.IsType<GlobalResponse>(okResult.Value);
            Assert.Equal("BL005", ((AppointmentResponseDto)response.Data).Token);
        }
    }
}