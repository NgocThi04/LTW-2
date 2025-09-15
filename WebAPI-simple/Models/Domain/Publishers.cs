using System.ComponentModel.DataAnnotations;

namespace WebAPI_simple.Models.Domain
{
    public class Publishers
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }

        public List<Books> Books { get; set; }

    }
}
