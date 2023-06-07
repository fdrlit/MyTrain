using System.ComponentModel.DataAnnotations;

namespace MyTrain.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
