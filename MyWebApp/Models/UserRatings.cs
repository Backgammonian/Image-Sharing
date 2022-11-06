﻿using MyWebApp.TableModels;

namespace MyWebApp.Models
{
    public sealed class UserRatings
    {
        public UserRatings(UserModel user, UserImageModel profilePicture, IEnumerable<NoteRating> ratingsOfNotes)
        {
            User = user;
            ProfilePicture = profilePicture;
            RatingsOfNotes = ratingsOfNotes;
        }

        public UserModel User { get; }
        public UserImageModel ProfilePicture { get; }
        public IEnumerable<NoteRating> RatingsOfNotes { get; }
    }
}
