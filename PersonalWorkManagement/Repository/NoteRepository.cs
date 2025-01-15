using Microsoft.EntityFrameworkCore;
using PersonalWorkManagement.Models;

namespace PersonalWorkManagement.Repository
{
    public class NoteRepository : INoteRepository
    {
        private readonly ApplicationDbContext _context;

        public NoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNoteAsync(Note note)
        {
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNoteAsync(Guid noteId)
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note == null)
            {
                throw new NotImplementedException();
            }
            _context.Notes.Remove(note);
        }

        public async Task<List<Note>> GetAllNotesAsync(Guid userId)
        {
            return await _context.Notes.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<Note> GetNoteByIdAsync(Guid noteId, Guid userId)
        {
            return await _context.Notes.FirstOrDefaultAsync(x => x.UserId == userId && x.NoteId == noteId);
        }

        public async Task UpdateNoteAsync(Note note)
        {
            if (note == null)
            {
                throw new ArgumentNullException(nameof(note));
            }
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
        }
    }
}
