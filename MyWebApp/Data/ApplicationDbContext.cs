using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyWebApp.TableModels;

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
        public DbSet<RatingModel> Ratings { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<TagsForNotesModel> TagsForNotes { get; set; }
    }
}
