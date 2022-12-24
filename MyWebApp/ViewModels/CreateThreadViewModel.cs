using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels
{
    public sealed class CreateThreadViewModel
    {
        [Required]
        public string NewThreadName { get; set; } = string.Empty;

        public IEnumerable<string> ExistingThreadNames { get; set; } = Enumerable.Empty<string>();
    }
}
