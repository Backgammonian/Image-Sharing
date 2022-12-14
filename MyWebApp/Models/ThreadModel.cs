using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public sealed class ThreadModel
    {
        [Key]
        public string Thread { get; set; } = string.Empty;
    }
}
