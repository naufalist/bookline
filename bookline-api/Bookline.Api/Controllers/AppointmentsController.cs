using Bookline.Application.Interfaces;
using Bookline.Domain.DTOs;
using Bookline.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bookline.Api.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointmentList()
        {
            try
            {
                List<AppointmentResponseDto> appointmentResponses = await _appointmentService.GetAppointments();

                return Ok(new GlobalResponse("Appointment list", appointmentResponses));
            }
            catch (BooklineBadRequestException ex)
            {
                Console.WriteLine(ex.Message);

                return BadRequest(new GlobalResponse(ex.Message));
            }
            catch (BooklineNotFoundException ex)
            {
                Console.WriteLine(ex.Message);

                return NotFound(new GlobalResponse(ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new GlobalResponse(ex.Message));
            }
        }

        [HttpGet("{date}")]
        public async Task<IActionResult> GetAppointmentByDate([FromRoute] string date)
        {
            try
            {
                List<AppointmentResponseDto> appointmentResponses = await _appointmentService.GetAppointmentsByDateAsync(date);

                return Ok(new GlobalResponse($"Appointments for {date:yyyy-MM-dd}", appointmentResponses));
            }
            catch (BooklineBadRequestException ex)
            {
                Console.WriteLine(ex.Message);

                return BadRequest(new GlobalResponse(ex.Message));
            }
            catch (BooklineNotFoundException ex)
            {
                Console.WriteLine(ex.Message);

                return NotFound(new GlobalResponse(ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new GlobalResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostAppointment([FromBody] AppointmentRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string firstErrorMessage = ModelState.Values
                        .SelectMany(stateEntry => stateEntry.Errors)
                        .Select(errorModel => errorModel.ErrorMessage)
                        .FirstOrDefault();

                    return BadRequest(new GlobalResponse(firstErrorMessage ?? "Invalid request body.", request));
                }

                AppointmentResponseDto appointmentResponse = await _appointmentService.PostAppointmentAsync(request);

                return Ok(new GlobalResponse("Appointment booked successfully.", appointmentResponse));
            }
            catch (BooklineBadRequestException ex)
            {
                Console.WriteLine(ex.Message);

                return BadRequest(new GlobalResponse(ex.Message));
            }
            catch (BooklineNotFoundException ex)
            {
                Console.WriteLine(ex.Message);

                return NotFound(new GlobalResponse(ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return StatusCode((int)HttpStatusCode.InternalServerError, new GlobalResponse(ex.Message));
            }
        }
    }
}