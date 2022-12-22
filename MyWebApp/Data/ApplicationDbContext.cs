using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;

namespace MyWebApp.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UserImageModel> ProfileImages { get; set; }
        public DbSet<NoteModel> Notes { get; set; }
        public DbSet<PreviousNoteModel> PreviousNotes { get; set; }
        public DbSet<NoteImageModel> NoteImages { get; set; }
        public DbSet<PreviousNoteImageModel> PreviousNoteImages { get; set; }
        public DbSet<ThreadModel> Threads { get; set; }
        public DbSet<NoteThreadModel> NoteThreads { get; set; }
    }
}
