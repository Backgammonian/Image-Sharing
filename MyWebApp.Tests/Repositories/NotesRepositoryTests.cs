using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWebApp.Tests.Repositories
{
    public class NotesRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureCreated();

            if (!dbContext.Notes.Any())
            {
                var notes = new List<NoteModel>();
                for (int i = 0; i < 10; i++)
                {
                    notes.Add(new NoteModel()
                    {
                        NoteId = (i + 1).ToString(),
                        UserId = "1",
                        Title = "Simple note title",
                        Description = "Sample text of note"
                    });
                }

                await dbContext.AddRangeAsync(notes);
                await dbContext.SaveChangesAsync();
            }

            return dbContext;
        }

        public NotesRepositoryTests() 
        {

        }
    }
}
