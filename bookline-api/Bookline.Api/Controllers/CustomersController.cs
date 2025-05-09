using Bookline.Application.Interfaces;
using Bookline.Domain.DTOs;
using Bookline.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bookline.Api.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CustomerRequestDto request)
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

                await _customerService.PostCustomerAsync(request);

                return Ok(new GlobalResponse("Registration successfully.", request));
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