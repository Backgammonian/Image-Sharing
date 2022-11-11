using MyWebApp.TableModels;
using System.Diagnostics;

namespace MyWebApp.Data
{
    public static class Seed
    {
        public static async Task SeedData(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            if (dbContext == null)
            {
                Debug.WriteLine("Database context is not available!");

                return;
            }

            var picturesLoader = serviceScope.ServiceProvider.GetService<PicturesLoader>();
            if (picturesLoader == null)
            {
                Debug.WriteLine("Picture loader is not available!");

                return;
            }

            var users = new List<UserModel>()
            {
                new UserModel()
                {
                    UserId = RandomGenerator.GetRandomId(),
                    Name = "Blue User",
                    Status = "Just chillin'"
                },
                new UserModel()
                {
                    UserId = RandomGenerator.GetRandomId(),
                    Name = "Red user",
                    Status = "Just morbin'"
                },
                new UserModel()
                {
                    UserId = RandomGenerator.GetRandomId(),
                    Name = "Green user",
                    Status = "Just mindlessly scrolling the Internet pages until the end of the world. =)"
                }
            };

            var notes = new List<NoteModel>()
            {
                new NoteModel()
                {
                    NoteId = "1",
                    UserId = users[0].UserId,
                    Title = "Kelp Note",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
                },
                new NoteModel()
                {
                    NoteId = "2",
                    UserId = users[0].UserId,
                    Title = "Sallad Note",
                    Description = "There are many variations of passages of Lorem Ipsum available, " +
                    "but the majority have suffered alteration in some form, by injected humour, " +
                    "or randomised words which don't look even slightly believable."
                },
                new NoteModel()
                {
                    NoteId = "3",
                    UserId = users[1].UserId,
                    Title = "Funky Note",
                    Description = "It is a long established fact that a reader will be distracted by " +
                    "the readable content of a page when looking at its layout. " +
                    "\n<a href=\"google.com\">Not a link at all!</a>"
                },
                new NoteModel()
                {
                    NoteId = "4",
                    UserId = users[2].UserId,
                    Title = "Interesting Observation",
                    Description = "If you'll watch long enough how paint dries out, the paint WILL eventually dry out. " +
                    "\n#CelestialThoughts"
                }
            };

            picturesLoader.LoadDefaultImage();
            var noteImages = picturesLoader.LoadDemoNoteImages(notes);
            var userImages = picturesLoader.LoadDemoProfileImages(users);

            var ratings = new List<RatingModel>()
            {
                new RatingModel()
                {
                    RatingId = "1",
                    UserId = users[0].UserId,
                    NoteId = notes[0].NoteId,
                    Score = 1
                },
                new RatingModel()
                {
                    RatingId = "2",
                    UserId = users[1].UserId,
                    NoteId = notes[0].NoteId,
                    Score = -1
                },
                new RatingModel()
                {
                    RatingId = "3",
                    UserId = users[0].UserId,
                    NoteId = notes[1].NoteId,
                    Score = 1
                },
                new RatingModel()
                {
                    RatingId = "4",
                    UserId = users[1].UserId,
                    NoteId = notes[1].NoteId,
                    Score = 1
                },
                new RatingModel()
                {
                    RatingId = "5",
                    UserId = users[0].UserId,
                    NoteId = notes[2].NoteId,
                    Score = 1
                },
                new RatingModel()
                {
                    RatingId = "6",
                    UserId = users[2].UserId,
                    NoteId = notes[3].NoteId,
                    Score = 1
                },
            };

            var tags = new List<TagModel>()
            {
                new TagModel()
                {
                    Tag = "funny"
                },
                new TagModel()
                {
                    Tag = "news"
                },
                new TagModel()
                {
                    Tag = "text"
                },
                new TagModel()
                {
                    Tag = "photos"
                },
            };

            var tagsForNotes = new List<TagsForNotesModel>()
            {
                new TagsForNotesModel()
                {
                    Id = "1",
                    Tag = tags[0].Tag,
                    NoteId = notes[0].NoteId
                },
                new TagsForNotesModel()
                {
                    Id = "2",
                    Tag = tags[1].Tag,
                    NoteId = notes[0].NoteId
                },
                new TagsForNotesModel()
                {
                    Id = "3",
                    Tag = tags[1].Tag,
                    NoteId = notes[1].NoteId
                },
                new TagsForNotesModel()
                {
                    Id = "4",
                    Tag = tags[2].Tag,
                    NoteId = notes[2].NoteId
                },
                new TagsForNotesModel()
                {
                    Id = "5",
                    Tag = tags[3].Tag,
                    NoteId = notes[2].NoteId
                },
                new TagsForNotesModel()
                {
                    Id = "6",
                    Tag = tags[3].Tag,
                    NoteId = notes[0].NoteId
                },
                new TagsForNotesModel()
                {
                    Id = "7",
                    Tag = tags[0].Tag,
                    NoteId = notes[3].NoteId
                },
                new TagsForNotesModel()
                {
                    Id = "8",
                    Tag = tags[2].Tag,
                    NoteId = notes[3].NoteId
                },
            };

            dbContext.Database.EnsureCreated();

            if (dbContext.Users != null &&
                !dbContext.Users.Any())
            {
                await dbContext.Users.AddRangeAsync(users);
            }

            if (dbContext.ProfileImages != null &&
                !dbContext.ProfileImages.Any())
            {
                await dbContext.ProfileImages.AddAsync(picturesLoader.GetDefaultProfileImage());
                await dbContext.ProfileImages.AddRangeAsync(userImages);
            }

            if (dbContext.Notes != null &&
                !dbContext.Notes.Any())
            {
                await dbContext.Notes.AddRangeAsync(notes);
            }

            if (dbContext.NoteImages != null &&
                !dbContext.NoteImages.Any())
            {
                await dbContext.NoteImages.AddAsync(picturesLoader.GetDefaultNoteImage());
                await dbContext.NoteImages.AddRangeAsync(noteImages);
            }

            if (dbContext.Ratings != null &&
                !dbContext.Ratings.Any())
            {
                await dbContext.Ratings.AddRangeAsync(ratings);
            }

            if (dbContext.Tags != null &&
                !dbContext.Tags.Any())
            {
                await dbContext.Tags.AddRangeAsync(tags);
            }

            if (dbContext.TagsForNotes != null &&
                !dbContext.TagsForNotes.Any())
            {
                await dbContext.TagsForNotes.AddRangeAsync(tagsForNotes);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
