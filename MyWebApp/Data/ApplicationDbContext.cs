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
        public DbSet<NoteImageModel> NoteImages { get; set; }
        public DbSet<ThreadModel> Threads { get; set; }
        public DbSet<NoteThreadModel> NoteThreads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NoteThreadModel>()
                    .HasKey(nt => new { nt.ThreadId, nt.NoteId });
            modelBuilder.Entity<NoteThreadModel>()
                    .HasOne(nt => nt.Thread)
                    .WithMany(t => t.NoteThreads)
                    .HasForeignKey(nt => nt.ThreadId);
            modelBuilder.Entity<NoteThreadModel>()
                    .HasOne(nt => nt.Note)
                    .WithMany(n => n.NoteThreads)
                    .HasForeignKey(nt => nt.NoteId);
        }
    }
}
