using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalWorkManagement.Services;

namespace PersonalWorkManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public ScheduleController(ScheduleService scheduleService)
        {
                _scheduleService = scheduleService;
        }
        [Authorize]
        [HttpGet("getAll/{userId}")]
        public async Task<IActionResult> GetAllSchedule(Guid userId)
        {
            if (userId == null)
            {
                return BadRequest("UserId is not null!");
            }
            var response = await _scheduleService.GetScheduleAsync(userId);

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(new { Status = "Success", Message = response.Message, Data = response.Data });
        }
    }
}
