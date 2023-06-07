using System;
using System.ComponentModel.DataAnnotations;

namespace MyTrain.Models
{
    public class Route
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DepartureDate { get; set; }

        [Required]
        public int DepartureCityId { get; set; }

        [Required]
        public int ArrivalCityId { get; set; }

        [Required]
        public DateTime ArrivalDate { get; set; }

        [Required]
        public int TrainsId { get; set; }

        [Required]
        public decimal PriceCoupe { get; set; }

        [Required]
        public decimal PriceEconom { get; set; }

        public City DepartureCity { get; set; }
        public City ArrivalCity { get; set; }
        public Train Train { get; set; }
    }
}
