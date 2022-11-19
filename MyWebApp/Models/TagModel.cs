using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public sealed class TagModel
    {
        [Key]
        public string Tag { get; set; } = string.Empty;
    }
}
