using PersonalWorkManagement.DTOs;
using PersonalWorkManagement.Models;
using PersonalWorkManagement.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Azure.Core.HttpHeader;

namespace PersonalWorkManagement.Services
{
    public class NoteService
    {
        private readonly INoteRepository _noteRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public NoteService(INoteRepository noteRepository, IHttpContextAccessor contextAccessor)
        {
            _noteRepository = noteRepository;
            _contextAccessor = contextAccessor;
        }
        public async Task<ServiceResponse<string>> AddNoteAsync(UpdateNoteDTO noteDTO)
        {
            var response = new ServiceResponse<string>();
            var currentUserId = await GetAuthenticatedUserIdAsync();
            if (currentUserId == null)
            {
                response.Success = false;
                response.Message = "User note authenticated!";
                return response;
            }
            if (isNull(noteDTO))
            {
                response.Success = false;
                response.Message = "You must to text all fields";
                return response;
            }
            var note = new Note
            {
                NoteId = Guid.NewGuid().ToString(),
                Title = noteDTO.Title,
                Content = noteDTO.Content,
                CreatedAt = DateTime.UtcNow,
                UserId = currentUserId,
            };
            await _noteRepository.CreateNoteAsync(note);
            response.Success = true;
            response.Message = "Note created successfully!";
            return response;
        }
        public async Task<ServiceResponse<string>> UpdateNotesAsync(string noteId, UpdateNoteDTO updateNoteDTO)
        {
            var response = new ServiceResponse<string>();
            var currentUserId = await GetAuthenticatedUserIdAsync();

            var existingUser = await _noteRepository.GetNoteByIdAsync(noteId, currentUserId);

            if (existingUser == null || currentUserId != existingUser.UserId)
            {
                response.Success = false;
                response.Message = "Note not found or you're not authorized to update this note.";
                return response;
            }
            UpdateNote(existingUser, updateNoteDTO);
            await _noteRepository.UpdateNoteAsync(existingUser);
            response.Success = true;
            response.Message = "Note updated successfully!";
            return response;
        }
        public async Task<ServiceResponse<string>> DeleteNoteAsync(string noteId)
        {
            var response = new ServiceResponse<string>();
            if (noteId == null)
            {
                response.Success = false;
                response.Message = "NoteId is not null";
                return response;
            }
            var currentUserId = await GetAuthenticatedUserIdAsync();
            var existingUser = await _noteRepository.GetNoteByIdAsync(noteId, currentUserId);

            if (existingUser == null || currentUserId != existingUser.UserId)
            {
                response.Success = false;
                response.Message = "Note not found or you're not authorized to delete this note.";
                return response;
            }
            await _noteRepository.DeleteNoteAsync(noteId);
            response.Success = true;
            response.Message = "Note deleted successfully!";
            return response;
        }
        public async Task<ServiceResponse<List<NoteDTO>>> GetAllNoteAsync()
        {
            var response = new ServiceResponse<List<NoteDTO>>();
            var currentUserId = await GetAuthenticatedUserIdAsync();
            if (currentUserId == null)
            {
                response.Success = false;
                response.Message = "User note authenticated!";
                return response;
            }
            var notes = await _noteRepository.GetAllNotesAsync(currentUserId);
            response.Success = true;
            response.Message = "Retrived successfully!";
            response.Data = notes.Select(note => new NoteDTO
            {
                NoteId = note.NoteId,
                Title = note.Title,
                Content = note.Content,
                CreatedAt = note.CreatedAt,
            }).ToList();
            return response;
        }
        public async Task<ServiceResponse<NoteDTO>> GetNoteByIdAsync(string noteId)
        {
            var response = new ServiceResponse<NoteDTO>();
            var currentUserId = await GetAuthenticatedUserIdAsync();
            if (currentUserId == null)
            {
                response.Success = false;
                response.Message = "User note authenticated!";
                return response;
            }
            var existingUser = await _noteRepository.GetNoteByIdAsync(noteId, currentUserId);

            if (existingUser == null || currentUserId != existingUser.UserId)
            {
                response.Success = false;
                response.Message = "Note not found or you're not authorized to retrive this note.";
                return response;
            }
            response.Success = true;
            response.Message = "Retrived successfully!";
            response.Data = new NoteDTO
            {
                NoteId = existingUser.NoteId,
                Title = existingUser.Title,
                Content = existingUser.Content,
                CreatedAt = existingUser.CreatedAt,
            };
            return response;
        }
        private async Task<string?> GetAuthenticatedUserIdAsync()
        {
            var userIdClaim = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) ??
                _contextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);
            return userIdClaim?.Value;
        }
        private bool isNull(UpdateNoteDTO noteDTO)
        {
            return String.IsNullOrEmpty(noteDTO.Content) || String.IsNullOrEmpty(noteDTO.Title);
        }
        public void UpdateNote(Note note, UpdateNoteDTO updateNoteDTO)
        {
            note.Title = updateNoteDTO.Title;
            note.Content = updateNoteDTO.Content;
            note.CreatedAt = DateTime.UtcNow;
        }
    }
}
