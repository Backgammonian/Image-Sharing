using Microsoft.AspNetCore.Identity;
using MyWebApp.Credentials;
using MyWebApp.Models;
using MyWebApp.PicturesModule.Interfaces;

namespace MyWebApp.Data
{
    public class Seeder
    {
        private readonly IApplicationBuilder _applicationBuilder;

        public Seeder(IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
        }

        public void EnsureCreated()
        {
            using var serviceScope = _applicationBuilder.ApplicationServices.CreateScope();

            var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            if (dbContext == null)
            {
                Console.WriteLine("(EnsureCreated) Can't get the ApplicationDbContext");

                return;
            }

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        }

        public async Task SeedData()
        {
            using var serviceScope = _applicationBuilder.ApplicationServices.CreateScope();

            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();
            var adminUserEmail = "notadmin@gmail.com";
            var admin = await userManager.FindByEmailAsync(adminUserEmail);
            if (admin == null)
            {
                var newAdmin = new UserModel()
                {
                    Id = "notadmin",
                    UserName = "TotallyNotAdmin",
                    Email = adminUserEmail,
                    EmailConfirmed = true,
                    Status = "Totally not in charge!"
                };

                var result = await userManager.CreateAsync(newAdmin, "12345678");
                await userManager.AddToRoleAsync(newAdmin, UserRoles.Admin);

                admin = newAdmin;
            }

            var users = new List<UserModel>();
            var usersCredentials = new[] {
                ("white@gmail.com", "Mr.White", "12345678", "Neque porro quisquam amet, consectetur, adipisci velit."),
                ("pink@gmail.com", "Mr.Pink", "12345678", "No way this is going to work! Just no way!"),
                ("brown@gmail.com", "Mr.Brown", "12345678", "Just mindlessly scrolling the Internet pages =)") };

            var i = 1;
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
                        Id = "user" + i,
                        UserName = nickname,
                        Email = email,
                        EmailConfirmed = true,
                        Status = status
                    };

                    await userManager.CreateAsync(newUser, password);
                    await userManager.AddToRoleAsync(newUser, UserRoles.User);
                    users.Add(newUser);
                }

                i += 1;
            }

            var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            if (dbContext == null)
            {
                Console.WriteLine("(SeedData) Can't get the ApplicationDbContext");

                return;
            }

            var picturesLoader = serviceScope.ServiceProvider.GetService<IPicturesLoader>();
            if (picturesLoader == null)
            {
                Console.WriteLine("(SeedData) Can't get the IPicturesLoader");

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
                    ThreadId = tags[0].Thread,
                    NoteId = notes[0].NoteId
                },

                new NoteThreadModel()
                {
                    ThreadId = tags[1].Thread,
                    NoteId = notes[1].NoteId
                },

                new NoteThreadModel()
                {
                    ThreadId = tags[2].Thread,
                    NoteId = notes[2].NoteId
                },

                new NoteThreadModel()
                {
                    ThreadId = tags[0].Thread,
                    NoteId = notes[3].NoteId
                },

                new NoteThreadModel()
                {
                    ThreadId = tags[1].Thread,
                    NoteId = notes[4].NoteId
                },
            };

            picturesLoader.LoadDefaultImage();
            var noteImages = picturesLoader.LoadDemoNoteImages(notes);
            var userImages = picturesLoader.LoadDemoProfileImages(users);

            await dbContext.Notes.AddRangeAsync(notes);
            await dbContext.NoteImages.AddRangeAsync(noteImages);
            await dbContext.ProfileImages.AddRangeAsync(userImages);
            await dbContext.Threads.AddRangeAsync(tags);
            await dbContext.NoteThreads.AddRangeAsync(threadsOfNotes);
            
            await dbContext.SaveChangesAsync();
        }
    }
}
