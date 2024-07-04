using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelRequest.DB.Entityes
{
    [Table("Room")]
    public class RoomEntity
    {
        [Key]
        public int Id { get; set; }

        [Range(1, int.MaxValue)]
        public int Places { get; set; }

        [Range(1, double.MaxValue)]
        public double Square { get; set; }

        [Range(1, int.MaxValue)]
        public int Floor { get; set; }

        public int Number { get; set; }

        public int HostelId { get; set; }

        [ForeignKey("HostelId")]
        public virtual HostelEntity Hostel { get; set; }

        public virtual ICollection<StudentEntity> Students { get; set; }
    }
}
