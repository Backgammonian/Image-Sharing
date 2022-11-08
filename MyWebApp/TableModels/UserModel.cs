using System.ComponentModel.DataAnnotations;

namespace MyWebApp.TableModels
{
    public sealed class UserModel
    {
        [Key]
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
