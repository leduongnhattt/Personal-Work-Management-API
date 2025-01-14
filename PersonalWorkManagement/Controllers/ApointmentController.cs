using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Services;

namespace PersonalWorkManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApointmentController : ControllerBase
    {
        private readonly ApointmentService _apointmentService;

        public ApointmentController(ApointmentService apointmentService)
        {
            _apointmentService = apointmentService;
        }

        [Authorize]
        [HttpPost("addApointment")]
        public async Task<ActionResult> AddApointment([FromBody] UpdateApointmentDTO apointmentDTO)
        {
            if (apointmentDTO == null)
            {
                return BadRequest("Invalid task data.");
            }
            var response = await _apointmentService.AddApointmentAsync(apointmentDTO);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(new { Status = "Success", Message = response.Message });
        }
        [Authorize]
        [HttpGet("allApointment")]
        public async Task<ActionResult> GetAllApointments()
        {
            var response = await _apointmentService.GetAllApointmentAsync();

            if (response.Success)
            {
                return Ok(new { Message = response.Message, Data = response.Data });
            }
            return BadRequest(response.Message);
        }
        [Authorize]
        [HttpPut("updateApointment/{apointmentId}")]
        public async Task<ActionResult> UpdateApointment(Guid apointmentId, [FromBody] UpdateApointmentDTO updateApointmentDTO)
        {
            if (updateApointmentDTO == null)
            {
                return BadRequest("Invalid task data.");
            }
            if (apointmentId == Guid.Empty)
            {
                return BadRequest("Invalid task id");
            }
            var response = await _apointmentService.UpdateApointmentAsync(apointmentId, updateApointmentDTO);

            if (response.Success)
            {
                return Ok(new { Message = response.Message });
            }

            return BadRequest(response.Message);
        }
        [Authorize]
        [HttpDelete("deleteApointment/{apointmentId}")]
        public async Task<IActionResult> DeleteApointment(Guid apointmentId)
        {
            var response = await _apointmentService.DeleteApointmentAsync(apointmentId);

            if (response.Success)
            {
                return Ok(response.Message);
            }

            return BadRequest(response.Message);
        }
        [Authorize]
        [HttpGet("getApointment/{apointmentId}")]
        public async Task<IActionResult> GetApointmentById(Guid apointmentId)
        {
            var response = await _apointmentService.GetApointmentByIdAsync(apointmentId);

            if (response.Success)
            {
                return Ok(new { Message = response.Message, Data = response.Data });
            }

            return BadRequest(response.Message);
        }
    }
}
