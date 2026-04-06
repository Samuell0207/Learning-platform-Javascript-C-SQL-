using System.ComponentModel.DataAnnotations.Schema;

namespace LearnMathAPI.Models
{
    public class User
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Grade { get; set; }
        public string? ChaptersJson { get; set; }

        [NotMapped]
        public List<string> Chapters { get; set; } = new();
    }
}