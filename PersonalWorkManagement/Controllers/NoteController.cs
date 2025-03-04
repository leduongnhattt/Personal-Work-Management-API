﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Services;

namespace PersonalWorkManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly NoteService _noteService;

        public NoteController(NoteService noteService)
        {
            _noteService = noteService;
        }

        [Authorize]
        [HttpPost("addNote")]
        public async Task<ActionResult> AddNote([FromBody] UpdateNoteDTO noteDTO)
        {
            if (noteDTO == null)
            {
                return BadRequest("Invalid task data.");
            }
            var response = await _noteService.AddNoteAsync(noteDTO);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(new { Status = "Success", Message = response.Message });
        }
        [Authorize]
        [HttpGet("allNote")]
        public async Task<ActionResult> GetAllNotes()
        {
            var response = await _noteService.GetAllNoteAsync();

            if (response.Success)
            {
                return Ok(new { Message = response.Message, Data = response.Data });
            }
            return BadRequest(response.Message);
        }
        [Authorize]
        [HttpPut("updateNote/{noteId}")]
        public async Task<ActionResult> UpdateNote(string noteId, [FromBody] UpdateNoteDTO updateNoteDTO)
        {
            if (updateNoteDTO == null)
            {
                return BadRequest("Invalid task data.");
            }
            if (noteId == "")
            {
                return BadRequest("Invalid task id");
            }
            var response = await _noteService.UpdateNotesAsync(noteId, updateNoteDTO);

            if (response.Success)
            {
                return Ok(new { Message = response.Message });
            }

            return BadRequest(response.Message);
        }
        [Authorize]
        [HttpDelete("deleteNote/{noteId}")]
        public async Task<IActionResult> DeleteNote(string noteId)
        {
            var response = await _noteService.DeleteNoteAsync(noteId);

            if (response.Success)
            {
                return Ok(new { Message = response.Message });
            }

            return BadRequest(response.Message);
        }
        [Authorize]
        [HttpGet("getNote/{noteId}")]
        public async Task<IActionResult> GetNoteById(string noteId)
        {
            var response = await _noteService.GetNoteByIdAsync(noteId);

            if (response.Success)
            {
                return Ok(new { Message = response.Message, Data = response.Data });
            }

            return BadRequest(response.Message);
        }
    }
}
