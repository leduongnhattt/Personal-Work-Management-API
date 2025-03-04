using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public interface INoteRepository
    {
        Task CreateNoteAsync(Note note);
        Task DeleteNoteAsync(string noteId);
        Task UpdateNoteAsync(Note note);
        Task<Note> GetNoteByIdAsync(string noteId, string userId);
        Task<List<Note>> GetAllNotesAsync(string userId);
    }
}
