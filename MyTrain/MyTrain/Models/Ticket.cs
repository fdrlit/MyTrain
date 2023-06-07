using System.ComponentModel.DataAnnotations;

namespace MyTrain.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RouteId { get; set; }

        [Required]
        public int WagonId { get; set; }

        [Required]
        public int PlaceId { get; set; }

        [Required]
        public int UserId { get; set; }

        public Route Route { get; set; }
        public Place Place { get; set; }
        public User User { get; set; }
        public Wagon Wagon { get; set; }
    }
}
