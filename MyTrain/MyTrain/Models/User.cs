using System.ComponentModel.DataAnnotations;

namespace MyTrain.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength]
        public string Surname { get; set; }

        [Required]
        [MaxLength]
        public string Name { get; set; }

        [MaxLength]
        public string MiddleName { get; set; }

        [MaxLength(11)]
        public string NumberPhone { get; set; }

        [MaxLength(10)]
        public string PassportData { get; set; }

        [Required]
        [MaxLength]
        public string Password { get; set; }

        [Required]
        public int RoleID { get; set; }

        public Role Role { get; set; }
    }
}
