using System.ComponentModel.DataAnnotations;

namespace MyTrain.Models
{
    public class Type
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
