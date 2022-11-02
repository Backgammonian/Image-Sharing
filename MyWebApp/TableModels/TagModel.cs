using System.ComponentModel.DataAnnotations;

namespace MyWebApp.TableModels
{
    public sealed class TagModel
    {
        [Key]
        public string Tag { get; set; } = string.Empty;
    }
}
