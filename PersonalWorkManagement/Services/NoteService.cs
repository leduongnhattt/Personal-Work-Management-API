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
                NoteId = Guid.NewGuid(),
                Title = noteDTO.Title,
                Content = noteDTO.Content,
                CreatedAt = DateTime.UtcNow,
                UserId = currentUserId.Value,
            };
            await _noteRepository.CreateNoteAsync(note);
            response.Success = true;
            response.Message = "Note created successfully!";
            return response;
        }
        public async Task<ServiceResponse<string>> UpdateNotesAsync(Guid noteId, UpdateNoteDTO updateNoteDTO)
        {
            var response = new ServiceResponse<string>();
            var currentUserId = await GetAuthenticatedUserIdAsync();

            var existingUser = await _noteRepository.GetNoteByIdAsync(noteId, currentUserId.Value);

            if (existingUser == null || currentUserId.Value != existingUser.UserId)
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
        public async Task<ServiceResponse<string>> DeleteNoteAsync(Guid noteId)
        {
            var response = new ServiceResponse<string>();
            if (noteId == null)
            {
                response.Success = false;
                response.Message = "NoteId is not null";
                return response;
            }
            var currentUserId = await GetAuthenticatedUserIdAsync();
            var existingUser = await _noteRepository.GetNoteByIdAsync(noteId, currentUserId.Value);

            if (existingUser == null || currentUserId.Value != existingUser.UserId)
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
            var notes = await _noteRepository.GetAllNotesAsync(currentUserId.Value);
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
        public async Task<ServiceResponse<NoteDTO>> GetNoteByIdAsync(Guid noteId)
        {
            var response = new ServiceResponse<NoteDTO>();
            var currentUserId = await GetAuthenticatedUserIdAsync();
            if (currentUserId == null)
            {
                response.Success = false;
                response.Message = "User note authenticated!";
                return response;
            }
            var existingUser = await _noteRepository.GetNoteByIdAsync(noteId, currentUserId.Value);

            if (existingUser == null || currentUserId.Value != existingUser.UserId)
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
        private async Task<Guid?> GetAuthenticatedUserIdAsync()
        {
            var userIdClaim = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) ??
                _contextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
            {
                return Guid.Empty;
            }
            return Guid.TryParse(userIdClaim.Value, out var userId) ? userId : (Guid.Empty);
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
