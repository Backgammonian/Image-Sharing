using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.TableModels
{
    public sealed class UserImageModel
    {
        [Key]
        public string ImageId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserModel))]
        public string UserId { get; set; } = string.Empty;
        public string ImageFileName { get; set; } = string.Empty;
        public DateTimeOffset UploadTime { get; set; }
    }
}
