using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MyWebApp.Data;

namespace MyWebApp.Models
{
    public class UserImageModel
    {
        [Key]
        public string ImageId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserModel))]
        public string UserId { get; set; } = string.Empty;

        public string ImageFileName { get; set; } = Constants.DefaultImageName;
        public DateTimeOffset UploadTime { get; set; }
    }
}
