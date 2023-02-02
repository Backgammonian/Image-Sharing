using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Data.Interfaces;
using MyWebApp.Models;

namespace MyWebApp.Tests
{
    public class PlaceholderDatabaseGenerator
    {
        private readonly IRandomGenerator _randomGenerator;

        public PlaceholderDatabaseGenerator()
        {
            _randomGenerator = new RandomGenerator();
        }

        public async Task<ApplicationDbContext> GetDatabase()
        {
            var users = new List<UserModel>();
            for (int i = 0; i < 3; i++)
            {
                var id = i.ToString();

                users.Add(new UserModel()
                {
                    Id = id,
                    UserName = _randomGenerator.GetRandomString(10)
                });
            }

            var threads = new List<ThreadModel>
            {
                new ThreadModel()
                {
                    Thread = "funny"
                },

                new ThreadModel()
                {
                    Thread = "photos"
                },

                new ThreadModel()
                {
                    Thread = "news"
                },
            };

            var notes = new List<NoteModel>();
            var noteThreads = new List<NoteThreadModel>();
            for (int i = 0; i < 10; i++)
            {
                var id = i.ToString();
                var userId = users[i % users.Count].Id;

                notes.Add(new NoteModel()
                {
                    NoteId = id,
                    UserId = userId,
                    Title = "Simple note title",
                    Description = "Sample text of note"
                });

                var thread = threads[i % threads.Count].Thread;

                noteThreads.Add(new NoteThreadModel()
                {
                    NoteId = id,
                    ThreadId = thread
                });
            }

            var noteImages = new List<NoteImageModel>
            {
                new NoteImageModel()
                {
                    ImageId = "0",
                    NoteId = "0",
                    ImageFileName = "image1.jpg",
                    UploadTime = DateTimeOffset.Now.AddDays(-1)
                },

                new NoteImageModel()
                {
                    ImageId = "1",
                    NoteId = "1",
                    ImageFileName = "image2.jpg",
                    UploadTime = DateTimeOffset.Now.AddDays(-2)
                },

                new NoteImageModel()
                {
                    ImageId = "2",
                    NoteId = "2",
                    ImageFileName = "image3.gif",
                    UploadTime = DateTimeOffset.Now.AddDays(-42)
                }
            };

            var profilePictures = new List<UserImageModel>
            {
                new UserImageModel()
                {
                    ImageId = "0",
                    UserId = "0",
                    ImageFileName = "profile_image1.jpg",
                    UploadTime = DateTimeOffset.Now.AddDays(-1)
                },

                new UserImageModel()
                {
                    ImageId = "1",
                    UserId = "0",
                    ImageFileName = "profile_image2.jpg",
                    UploadTime = DateTimeOffset.Now.AddDays(-0.5)
                },

                new UserImageModel()
                {
                    ImageId = "2",
                    UserId = "1",
                    ImageFileName = "profile_image3.jpg",
                    UploadTime = DateTimeOffset.Now.AddDays(-3.5)
                },
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureCreated();

            await dbContext.Users.AddRangeAsync(users);
            await dbContext.Threads.AddRangeAsync(threads);
            await dbContext.Notes.AddRangeAsync(notes);
            await dbContext.NoteThreads.AddRangeAsync(noteThreads);
            await dbContext.NoteImages.AddRangeAsync(noteImages);
            await dbContext.ProfileImages.AddRangeAsync(profilePictures);
            await dbContext.SaveChangesAsync();

            return dbContext;
        }
    }
}
