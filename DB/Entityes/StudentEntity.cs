using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelRequest.DB.Entityes
{
    [Table("Student")]
    public class StudentEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string PIB { get; set; }

        [Required]
        [StringLength(10)]
        public string Group { get; set; }

        [Required]
        [StringLength(10)]
        public string Facultet { get; set; }

        public int? RoomId { get; set; }

        [ForeignKey("RoomId")]
        public virtual RoomEntity Room { get; set; }
    }
}
