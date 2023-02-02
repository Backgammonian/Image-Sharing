using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class ThreadModel
    {
        [Key]
        public string Thread { get; set; } = string.Empty;
        public ICollection<NoteThreadModel>? NoteThreads { get; set; }
    }
}