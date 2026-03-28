using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaApi.Models
{
    [Table("ccUsers")]
    public class User
    {
        [Key]
        public int User_id { get; set; }

        [Required]
        public string Login { get; set; } = string.Empty;
        public string? Nombres { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Password { get; set; }
        public int? TipoUser_id { get; set; }
        public int? Status { get; set; }
        public DateTime? fCreate { get; set; }
        public int? IDArea { get; set; }
        public DateTime? LastLoginAttempt { get; set; }
    }
}