using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Services;

namespace PersonalWorkManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkTaskController : ControllerBase
    {
        private readonly WorkTaskServices _workTaskServices;
        public WorkTaskController(WorkTaskServices workTaskServices)
        {
            _workTaskServices = workTaskServices;
        }

        [Authorize]
        [HttpPost("addwork")]
        public async Task<ActionResult> AddWorkTask([FromBody] WorkTaskDTO workTaskDTO)
        {
            if (workTaskDTO == null)
            {
                return BadRequest("Invalid task data.");
            }
            var response = await _workTaskServices.AddWorkTaskAsync(workTaskDTO);
            if (response.Success)
            {
                return Ok(response.Message);
            }

            return BadRequest(response.Message);
        }
        [Authorize]
        [HttpGet("allTask")]
        public async Task<ActionResult> GetAllTask()
        {
            var response = await _workTaskServices.GetAllWorkTaskAsync();

            if (response.Success)
            {
                return Ok(new { Message = response.Message, Data = response.Data });
            }
            return BadRequest(response.Message);
        }
        [Authorize]
        [HttpPut("updateTask/{workTaskId}")]
        public async Task<ActionResult> UpdateWorkTask(Guid workTaskId, [FromBody] WorkTaskUpdateDTO workTaskDTO)
        {
            if (workTaskDTO == null)
            {
                return BadRequest("Invalid task data.");
            }
            if (workTaskId == Guid.Empty)
            {
                return BadRequest("Invalid task id");
            }
            var response = await _workTaskServices.UpdateWorkTaskAsync(workTaskId, workTaskDTO);

            if (response.Success)
            {
                return Ok(new { Message = response.Message });
            }

            return BadRequest(response.Message);
        }
        [Authorize]
        [HttpDelete("deleteTask/{workTaskId}")]
        public async Task<IActionResult> DeleteWorkTask(Guid workTaskId)
        {
            var response = await _workTaskServices.DeleteWorkTaskAsync(workTaskId);

            if (response.Success)
            {
                return Ok(response.Message);
            }

            return BadRequest(response.Message);
        }
        [Authorize]
        [HttpGet("getTaskById/{workTaskId}")]
        public async Task<IActionResult> GetTaskById(Guid workTaskId)
        {
            var response = await _workTaskServices.GetWorkTaskByIdAsync(workTaskId);

            if (response.Success)
            {
                return Ok(new { Message = response.Message, Data = response.Data });
            }

            return BadRequest(response.Message);
        }
    }
}
