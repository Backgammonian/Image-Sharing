using Microsoft.AspNetCore.Identity;
using MyWebApp.Models;
using System.Diagnostics;

namespace MyWebApp.Data
{
    public static class Seed
    {
        public static async Task<(UserModel, UserModel[])> SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }

            var randomGenetator = serviceScope.ServiceProvider.GetService<RandomGenerator>();
            if (randomGenetator == null)
            {
                Debug.WriteLine("(SeedUsersAndRolesAsync) Random generator is not available!");

                return (new UserModel(), Array.Empty<UserModel>());
            }

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
            string adminUserEmail = "totallynotadmin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
            if (adminUser == null)
            {
                var newAdminUser = new UserModel()
                {
                    Id = "notadmin",
                    UserName = "totallynotadmin",
                    Email = adminUserEmail,
                    EmailConfirmed = true,
                    Status = "Totally not in charge!"
                };
                await userManager.CreateAsync(newAdminUser, "qwerty");
                await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);

                adminUser = newAdminUser;
            }

            var users = new List<UserModel>();
            var usersCredentials = new[] { ("blue@gmail.com", "Mr. Blue", "12345678", "Just chillin'"),
                ("green@gmail.com", "Mr. Green", "12345678", "Just morbin'"),
                ("red@gmail.com", "Mr. Red", "12345678", "Just mindlessly scrolling the Internet pages until the end of the world. =)") };

            foreach (var userInfo in usersCredentials)
            {
                var email = userInfo.Item1;
                var nickname = userInfo.Item2;
                var password = userInfo.Item3;
                var status = userInfo.Item4;

                var appUser = await userManager.FindByEmailAsync(email);
                if (appUser == null)
                {
                    var newUser = new UserModel()
                    {
                        Id = randomGenetator.GetRandomId(),
                        UserName = nickname,
                        Email = email,
                        EmailConfirmed = true,
                        Status = status
                    };

                    await userManager.CreateAsync(newUser, password);
                    await userManager.AddToRoleAsync(newUser, UserRoles.User);
                    users.Add(newUser);
                }
            }

            return (adminUser, users.ToArray());
        }

        public static async Task SeedData(IApplicationBuilder applicationBuilder, UserModel admin, UserModel[] users)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            if (dbContext == null)
            {
                Debug.WriteLine("(SeedData) Database context is not available!");

                return;
            }

            var picturesLoader = serviceScope.ServiceProvider.GetService<PicturesLoader>();
            if (picturesLoader == null)
            {
                Debug.WriteLine("(SeedData) Picture loader is not available!");

                return;
            }

            var notes = new List<NoteModel>()
            {
                new NoteModel()
                {
                    NoteId = "1",
                    UserId = users[0].Id,
                    Title = "Kelp Note",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
                },
                new NoteModel()
                {
                    NoteId = "2",
                    UserId = users[0].Id,
                    Title = "Sallad Note",
                    Description = "There are many variations of passages of Lorem Ipsum available, " +
                    "but the majority have suffered alteration in some form, by injected humour, " +
                    "or randomised words which don't look even slightly believable."
                },
                new NoteModel()
                {
                    NoteId = "3",
                    UserId = users[1].Id,
                    Title = "Funky Note",
                    Description = "It is a long established <b>fact</b> that a reader will be distracted by " +
                    "the readable content of a page when looking at its layout. " +
                    "\n<a href=\"google.com\">Not a link at all!</a>"
                },
                new NoteModel()
                {
                    NoteId = "4",
                    UserId = users[2].Id,
                    Title = "Interesting Observation",
                    Description = "If you'll watch long enough how paint dries out, the paint WILL eventually dry out. " +
                    "\n#CelestialThoughts"
                },
                new NoteModel()
                {
                    NoteId = "5",
                    UserId = admin.Id,
                    Title = "Announcement from Admin",
                    Description = "Hello every1! Just a friendly reminder: ALL YOUR IMAGES ARE BELONG TO US ඞ"
                },
            };

            picturesLoader.LoadDefaultImage();
            var noteImages = picturesLoader.LoadDemoNoteImages(notes.ToArray());
            var userImages = picturesLoader.LoadDemoProfileImages(users);

            var ratings = new List<RatingModel>()
            {
                new RatingModel()
                {
                    RatingId = "1",
                    UserId = users[0].Id,
                    NoteId = notes[0].NoteId,
                    Score = 1
                },
                new RatingModel()
                {
                    RatingId = "2",
                    UserId = users[1].Id,
                    NoteId = notes[0].NoteId,
                    Score = -1
                },
                new RatingModel()
                {
                    RatingId = "3",
                    UserId = users[0].Id,
                    NoteId = notes[1].NoteId,
                    Score = 1
                },
                new RatingModel()
                {
                    RatingId = "4",
                    UserId = users[1].Id,
                    NoteId = notes[1].NoteId,
                    Score = 1
                },
                new RatingModel()
                {
                    RatingId = "5",
                    UserId = users[0].Id,
                    NoteId = notes[2].NoteId,
                    Score = 1
                },
                new RatingModel()
                {
                    RatingId = "6",
                    UserId = users[2].Id,
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
