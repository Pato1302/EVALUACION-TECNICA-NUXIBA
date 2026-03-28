using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaApi.Models
{
    [Table("ccloglogin")]
    public class Login
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int User_id { get; set; }

        [Required]
        public int Extension { get; set; }

        [Required]
        public int TipoMov { get; set; }  // 1 = login, 0 = logout

        [Required]
        public DateTime fecha { get; set; }
    }
}