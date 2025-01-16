using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public interface INoteRepository
    {
        Task CreateNoteAsync(Note note);
        Task DeleteNoteAsync(Guid noteId);
        Task UpdateNoteAsync(Note note);
        Task<Note> GetNoteByIdAsync(Guid noteId, Guid userId);
        Task<List<Note>> GetAllNotesAsync(Guid userId);
    }
}
