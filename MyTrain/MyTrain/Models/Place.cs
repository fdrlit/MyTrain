using System.ComponentModel.DataAnnotations;

namespace MyTrain.Models
{
    public class Place
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public int? UserId { get; set; }

        [Required]
        public int WagonId { get; set; }

        public User User { get; set; }
        public Wagon Wagon { get; set; }
    }
}
