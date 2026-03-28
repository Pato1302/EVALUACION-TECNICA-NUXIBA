using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaApi.Models
{
    [Table("ccRIACat_Areas")]
    public class Area
    {
        [Key]
        public int IDArea { get; set; }

        public string? AreaName { get; set; }

        public int? StatusArea { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}