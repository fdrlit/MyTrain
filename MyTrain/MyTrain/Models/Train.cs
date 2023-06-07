using System.ComponentModel.DataAnnotations;

namespace MyTrain.Models
{
    public class Train
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Name { get; set; }
    }
}
