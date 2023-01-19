using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyWebApp.Credentials;
using MyWebApp.Data.Interfaces;
using MyWebApp.Models;
using MyWebApp.PicturesModule.Interfaces;

namespace MyWebApp.Data
{
    public static class Seeder
    {
        private static async Task SeedRoles(IApplicationBuilder applicationBuilder)
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
        }

        private static async Task<UserModel> SeedAdmin(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();

            var adminUserEmail = "totallynotadmin@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
            if (adminUser == null)
            {
                var newAdminUser = new UserModel()
                {
                    Id = "notadmin",
                    UserName = "TotallyNotAdmin",
                    Email = adminUserEmail,
                    EmailConfirmed = true,
                    Status = "Totally not in charge!"
                };

                var result = await userManager.CreateAsync(newAdminUser, "12345678");
                await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);

                adminUser = newAdminUser;
            }

            return adminUser;
        }

        public static async Task SeedOnlyAdminAndRoles(IApplicationBuilder applicationBuilder)
        {
            await SeedRoles(applicationBuilder);
            await SeedAdmin(applicationBuilder);
        }

        public static async Task<SeedUsersModel> SeedAllUsersWithRolesAndData(IApplicationBuilder applicationBuilder)
        {
            await SeedRoles(applicationBuilder);

            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
            var randomGenetator = serviceScope.ServiceProvider.GetService<IRandomGenerator>();
            if (randomGenetator == null)
            {
                return new SeedUsersModel()
                {
                    Admin = new UserModel(),
                    Users = Array.Empty<UserModel>()
                };
            }

            var adminUser = await SeedAdmin(applicationBuilder);

            var users = new List<UserModel>();
            var usersCredentials = new[] { 
                ("white@gmail.com", "Mr.White", "12345678", "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit."),
                ("pink@gmail.com", "Mr.Pink", "12345678", "No way this is going to work! Just no way!"),
                ("brown@gmail.com", "Mr.Brown", "12345678", "Just mindlessly scrolling the Internet pages until the end of the world. =)") };

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

            return new SeedUsersModel()
            {
                Admin = adminUser,
                Users = users.ToArray()
            };
        }

        public static async Task SeedData(IApplicationBuilder applicationBuilder, UserModel admin, UserModel[] users)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();

            var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            if (dbContext == null)
            {
                return;
            }

            var picturesLoader = serviceScope.ServiceProvider.GetService<IPicturesLoader>();
            if (picturesLoader == null)
            {
                return;
            }

            var notes = new List<NoteModel>()
            {
                new NoteModel()
                {
                    NoteId = "1",
                    UserId = users[0].Id,
                    Title = "Lorem ipsum dolor sit amet.",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam dapibus " +
                    "nunc purus, at fringilla purus semper eget. Suspendisse pretium, lacus at volutpat tristique, " +
                    "augue arcu cursus mauris, id egestas augue eros eu elit. Vivamus imperdiet, nibh ut " +
                    "feugiat vestibulum, ligula dui iaculis mauris, non bibendum lacus magna id metus. " +
                    "Suspendisse potenti. Phasellus quis leo eu ex hendrerit venenatis eu ut eros. Vivamus " +
                    "facilisis faucibus urna eu dictum. Suspendisse potenti. Etiam eros felis, tempus sit " +
                    "amet nibh nec, molestie accumsan erat."
                },

                new NoteModel()
                {
                    NoteId = "2",
                    UserId = users[0].Id,
                    Title = "Nulla facilisi.",
                    Description = "There are many variations of passages of Lorem Ipsum available, " +
                    "but the majority have suffered alteration in some form, by injected humour, " +
                    "or randomised words which don't look even slightly believable."
                },

                new NoteModel()
                {
                    NoteId = "3",
                    UserId = users[1].Id,
                    Title = "Pellentesque non imperdiet diam.",
                    Description = "Nullam ornare metus nisl, sit amet fringilla nisi eleifend sed. " +
                    "Aenean rutrum consequat eros, in egestas leo lacinia quis. Donec consectetur egestas " +
                    "ipsum id pharetra. Ut pharetra id elit in porta. Proin at euismod diam. Vivamus " +
                    "sagittis purus vitae sapien commodo, in iaculis felis congue. In dapibus sem sed " +
                    "ligula facilisis auctor. Aenean tempor pellentesque ultricies. Phasellus ac mi in " +
                    "ligula venenatis sollicitudin. Sed aliquet magna et tempus venenatis. Nam ac diam " +
                    "gravida, facilisis quam eu, fringilla magna. Aenean sit amet interdum lacus. " +
                    "Pellentesque eget odio sit amet ante consequat hendrerit vel eget quam. Suspendisse " +
                    "quis risus massa. Fusce sem tortor, rutrum volutpat elit sit amet, posuere pellentesque " +
                    "sapien.\r\n\r\nFusce lacinia purus non leo consectetur mollis ac sed augue. Pellentesque " +
                    "ante nulla, scelerisque vel sem et, ultrices blandit augue. Cras vulputate sed massa non " +
                    "mollis. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos " +
                    "himenaeos. Curabitur eget quam placerat, dignissim elit a, iaculis nulla. Pellentesque " +
                    "faucibus elit arcu, a fermentum ligula dictum id. Donec luctus condimentum fringilla. "
                },

                new NoteModel()
                {
                    NoteId = "4",
                    UserId = users[2].Id,
                    Title = "Interesting Observation",
                    Description = " Nullam consectetur, lacus non blandit egestas, orci metus consectetur velit, " +
                    "et tempor lacus massa at lorem. Nam lobortis, diam eu consequat aliquet, diam justo " +
                    "feugiat nunc, vel tristique ligula odio at massa. Nam consequat risus diam, at molestie " +
                    "lacus ultricies id. Donec in metus lacinia orci luctus vestibulum. Vestibulum " +
                    "faucibus pulvinar blandit. Orci varius natoque penatibus et magnis dis parturient " +
                    "montes, nascetur ridiculus mus. Sed sed sapien viverra, volutpat erat a, sodales est. " +
                    "Aenean maximus augue a ex pretium hendrerit. Cras leo lectus, molestie sed gravida nec, " +
                    "luctus in enim. Nunc non sapien et mi posuere volutpat et sed nunc. Praesent molestie " +
                    "tristique ligula, id rhoncus tellus luctus vitae. Nam congue facilisis nisi, ut auctor " +
                    "nibh hendrerit vel. Maecenas at auctor nulla. Donec sed ex varius, semper massa a, blandit sem. " +
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin posuere eget nisl a tempus.\r\n\r\n" +
                    "Nulla interdum sapien at eros rutrum, at ornare quam viverra. Aenean odio dolor, tristique eget ligula " +
                    "vitae, condimentum lobortis nibh. Integer vestibulum vitae quam nec imperdiet. Sed venenatis " +
                    "odio in blandit porttitor. Mauris quis ante porta, lacinia diam non, finibus felis. Nam et " +
                    "viverra arcu. Donec ac elit ut nulla accumsan elementum. Vivamus feugiat odio vel sapien " +
                    "imperdiet, eu porttitor metus condimentum. Ut id risus egestas, finibus mauris vel, " +
                    "fermentum arcu. Proin vel commodo tortor. Phasellus vehicula at enim vitae commodo. " +
                    "Vivamus neque lorem, bibendum a turpis non, varius volutpat lacus. Ut at tempus turpis. " +
                    "Nunc dapibus elit turpis, vel luctus dui fermentum et. "
                },

                new NoteModel()
                {
                    NoteId = "5",
                    UserId = admin.Id,
                    Title = "Announcement from Admin",
                    Description = "Maecenas fringilla dignissim lectus. Duis molestie viverra gravida. " +
                    "Quisque varius turpis in ligula accumsan fermentum. Vestibulum dictum mollis tincidunt. " +
                    "Curabitur venenatis ac turpis eu tempor. Donec egestas nunc non erat venenatis, a " +
                    "efficitur leo hendrerit. Pellentesque habitant morbi tristique senectus et netus et " +
                    "malesuada fames ac turpis egestas. Pellentesque sed tempus tellus. Nulla facilisi. " +
                    "Morbi vulputate sodales neque eget fermentum. Nunc eleifend, diam non vulputate viverra, " +
                    "dui neque cursus tortor, sed viverra lacus sapien non orci. Donec nec metus nisi. Duis " +
                    "lacinia dapibus efficitur. Phasellus finibus porttitor ex, a ultrices nunc venenatis " +
                    "semper. Aenean iaculis congue arcu vitae iaculis.\r\n\r\n" +
                    "Aliquam sodales quam sed ligula consequat, et blandit mauris efficitur. Praesent libero " +
                    "tellus, tincidunt nec viverra venenatis, eleifend id quam. Phasellus tempus leo elit, " +
                    "tristique interdum leo iaculis et. In hac habitasse platea dictumst. Curabitur " +
                    "sollicitudin auctor tempus. Suspendisse pharetra lobortis eros aliquet porttitor. " +
                    "Donec interdum porttitor sapien. Proin aliquam urna nec nunc blandit condimentum. " +
                    "Nullam quis ex est. In eget gravida lorem, efficitur facilisis ante. Phasellus rutrum " +
                    "in odio et hendrerit. Ut eu convallis lectus. \r\n\r\n" +
                    "ALL YOUR IMAGES ARE BELONG TO US ඞ"
                },
            };

            var tags = new List<ThreadModel>()
            {
                new ThreadModel()
                {
                    Thread = "funny"
                },

                new ThreadModel()
                {
                    Thread = "news"
                },

                new ThreadModel()
                {
                    Thread = "text"
                },

                new ThreadModel()
                {
                    Thread = "photos"
                },
            };

            var threadsOfNotes = new List<NoteThreadModel>()
            {
                new NoteThreadModel()
                {
                    Id = "1",
                    Thread = tags[0].Thread,
                    NoteId = notes[0].NoteId
                },

                new NoteThreadModel()
                {
                    Id = "2",
                    Thread = tags[1].Thread,
                    NoteId = notes[1].NoteId
                },

                new NoteThreadModel()
                {
                    Id = "3",
                    Thread = tags[2].Thread,
                    NoteId = notes[2].NoteId
                },

                new NoteThreadModel()
                {
                    Id = "4",
                    Thread = tags[0].Thread,
                    NoteId = notes[3].NoteId
                },

                new NoteThreadModel()
                {
                    Id = "5",
                    Thread = tags[1].Thread,
                    NoteId = notes[4].NoteId
                },
            };

            picturesLoader.LoadDefaultImage();
            var noteImages = picturesLoader.LoadDemoNoteImages(notes.ToArray());
            var userImages = picturesLoader.LoadDemoProfileImages(users);

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

            if (dbContext.Threads != null &&
                !dbContext.Threads.Any())
            {
                await dbContext.Threads.AddRangeAsync(tags);
            }

            if (dbContext.NoteThreads != null &&
                !dbContext.NoteThreads.Any())
            {
                await dbContext.NoteThreads.AddRangeAsync(threadsOfNotes);
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
