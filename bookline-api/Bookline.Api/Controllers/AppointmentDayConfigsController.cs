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
    [Route("api/appointment-day-configs")]
    [ApiController]
    public class AppointmentDayConfigsController : ControllerBase
    {
        private readonly IAppointmentDayConfigService _appointmentDayConfigService;

        public AppointmentDayConfigsController(IAppointmentDayConfigService appointmentDayConfigService)
        {
            _appointmentDayConfigService = appointmentDayConfigService;
        }

        [HttpGet]
        public async Task<IActionResult> GetConfigList()
        {
            try
            {
                List<AppointmentDayConfigResponseDto> configResponses = await _appointmentDayConfigService.GetAppointmentDayConfigsAsync();

                return Ok(new GlobalResponse("Appointment day config list", configResponses));
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
        public async Task<IActionResult> GetConfigByDate([FromRoute] string date)
        {
            try
            {
                AppointmentDayConfigResponseDto configResponse = await _appointmentDayConfigService.GetAppointmentDayConfigByDateAsync(date);

                return Ok(new GlobalResponse($"Appointment day config for {date:yyyy-MM-dd}", configResponse));
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
        public async Task<IActionResult> PostConfig([FromBody] AppointmentDayConfigRequestDto request)
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

                AppointmentDayConfigResponseDto configResponse = await _appointmentDayConfigService.PostAppointmentDayConfigAsync(request);

                return Ok(new GlobalResponse("Appointment day config added successfully.", configResponse));
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchConfig([FromRoute] string id, [FromBody] AppointmentDayConfigRequestDto request)
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

                AppointmentDayConfigResponseDto configResponse = await _appointmentDayConfigService.PartialUpdateAppointmentDayConfigAsync(id, request);

                return Ok(new GlobalResponse("Appointment day config updated successfully.", configResponse));
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfig([FromRoute] string id)
        {
            try
            {
                await _appointmentDayConfigService.HardDeleteAppointmentDayConfigAsync(id);

                return Ok(new GlobalResponse("Appointment day config deleted successfully."));
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