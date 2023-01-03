using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MyWebApp.PicturesModule;

namespace MyWebApp.Models
{
    public sealed class UserImageModel
    {
        [Key]
        public string ImageId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserModel))]
        public string UserId { get; set; } = string.Empty;

        public string ImageFileName { get; set; } = PicturesLoader.DefaultImageName;
        public DateTimeOffset UploadTime { get; set; }
    }
}
