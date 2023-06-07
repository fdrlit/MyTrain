using System.ComponentModel.DataAnnotations;

namespace MyTrain.Models
{
    public class Wagon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Count { get; set; }

        [Required]
        public int TrainsId { get; set; }

        [Required]
        public int TypeId { get; set; }

        public Train Train { get; set; }
        public Type Type { get; set; }
    }
}