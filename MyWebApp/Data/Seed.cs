using MyWebApp.TableModels;

namespace MyWebApp.Data
{
    public static class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            if (context == null)
            {
                return;
            }

            var users = new List<UserModel>()
            {
                new UserModel()
                {
                    UserId = RandomGenerator.GetRandomString(40),
                    Name = "Blue User",
                    Status = "Just chillin'",
                    ProfilePicturePath = "https://pbs.twimg.com/media/FQFopcEXoAcQRQr.png"
                },
                new UserModel()
                {
                    UserId = RandomGenerator.GetRandomString(40),
                    Name = "Red user",
                    Status = "Just morbin'",
                    ProfilePicturePath = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT4SJUr7VBXp-EOI67T9xqt_Rph8LBVeBDbdQ&usqp=CAU"
                },
            };

            var notes = new List<NoteModel>()
            {
                new NoteModel()
                {
                    NoteId = RandomGenerator.GetRandomString(40),
                    UserId = users[0].UserId,
                    Title = "Kelp Note",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
                },
                new NoteModel()
                {
                    NoteId = RandomGenerator.GetRandomString(40),
                    UserId = users[0].UserId,
                    Title = "Sallad Note",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
                },
                new NoteModel()
                {
                    NoteId = RandomGenerator.GetRandomString(40),
                    UserId = users[1].UserId,
                    Title = "Funky Note",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
                }
            };

            var images = new List<ImageModel>()
            {
                new ImageModel()
                {
                    ImageId = RandomGenerator.GetRandomString(40),
                    NoteId = notes[0].NoteId,
                    ImagePath = "https://i.imgflip.com/4po7dg.jpg?a462504"
                },
                new ImageModel()
                {
                    ImageId = RandomGenerator.GetRandomString(40),
                    NoteId = notes[0].NoteId,
                    ImagePath = "https://i.pinimg.com/736x/2a/73/a9/2a73a9a00e5d695ed861c7654ff9c6e3.jpg"
                },
                new ImageModel()
                {
                    ImageId = RandomGenerator.GetRandomString(40),
                    NoteId = notes[1].NoteId,
                    ImagePath = "https://sun9-67.userapi.com/impf/c855016/v855016651/812a4/TOXJydu3NjE.jpg?size=960x385&quality=96&sign=e808786293031ec0679486bb843a7fac&type=album"
                },
                new ImageModel()
                {
                    ImageId = RandomGenerator.GetRandomString(40),
                    NoteId = notes[2].NoteId,
                    ImagePath = "https://sun9-25.userapi.com/impg/LbwA2FchEFmTFHY0BH8sIeWEHUJ9EOzxA5Y_ag/MLwQPdEqqe0.jpg?size=478x604&quality=96&sign=ee1909e4838abbf8a95e64a975384643&type=album"
                },
                new ImageModel()
                {
                    ImageId = RandomGenerator.GetRandomString(40),
                    NoteId = notes[2].NoteId,
                    ImagePath = "https://sun9-17.userapi.com/impg/9b-gIMBjeem7BHZU9qLyz2vbPmwZX8cwPOiRjg/II2pc2SZ_-o.jpg?size=737x720&quality=96&sign=8d07def39980d2ba084a279513e0e32e&type=album"
                },
            };

            var ratings = new List<RatingModel>()
            {
                new RatingModel()
                {
                    RatingId = RandomGenerator.GetRandomString(40),
                    UserId = users[0].UserId,
                    NoteId = notes[0].NoteId,
                    Score = 1
                },
                new RatingModel()
                {
                    RatingId = RandomGenerator.GetRandomString(40),
                    UserId = users[1].UserId,
                    NoteId = notes[0].NoteId,
                    Score = -1
                },
                new RatingModel()
                {
                    RatingId = RandomGenerator.GetRandomString(40),
                    UserId = users[0].UserId,
                    NoteId = notes[1].NoteId,
                    Score = 1
                },
                new RatingModel()
                {
                    RatingId = RandomGenerator.GetRandomString(40),
                    UserId = users[1].UserId,
                    NoteId = notes[1].NoteId,
                    Score = 1
                },
                new RatingModel()
                {
                    RatingId = RandomGenerator.GetRandomString(40),
                    UserId = users[0].UserId,
                    NoteId = notes[2].NoteId,
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
                    Id = RandomGenerator.GetRandomString(40),
                    Tag = tags[0].Tag,
                    NoteId = notes[0].NoteId
                },
                new TagsForNotesModel()
                {
                    Id = RandomGenerator.GetRandomString(40),
                    Tag = tags[1].Tag,
                    NoteId = notes[0].NoteId
                },
                new TagsForNotesModel()
                {
                    Id = RandomGenerator.GetRandomString(40),
                    Tag = tags[1].Tag,
                    NoteId = notes[1].NoteId
                },
                new TagsForNotesModel()
                {
                    Id = RandomGenerator.GetRandomString(40),
                    Tag = tags[2].Tag,
                    NoteId = notes[2].NoteId
                },
                new TagsForNotesModel()
                {
                    Id = RandomGenerator.GetRandomString(40),
                    Tag = tags[3].Tag,
                    NoteId = notes[2].NoteId
                },
            };

            context.Database.EnsureCreated();

            if (context.Users != null &&
                !context.Users.Any())
            {
                context.Users.AddRange(users);
            }

            if (context.Notes != null &&
                !context.Notes.Any())
            {
                context.Notes.AddRange(notes);
            }

            if (context.Images != null &&
                !context.Images.Any())
            {
                context.Images.AddRange(images);
            }

            if (context.Ratings != null &&
                !context.Ratings.Any())
            {
                context.Ratings.AddRange(ratings);
            }

            if (context.Tags != null &&
                !context.Tags.Any())
            {
                context.Tags.AddRange(tags);
            }

            if (context.TagsForNotes != null &&
                !context.TagsForNotes.Any())
            {
                context.TagsForNotes.AddRange(tagsForNotes);
            }

            context.SaveChanges();
        }
    }
}
