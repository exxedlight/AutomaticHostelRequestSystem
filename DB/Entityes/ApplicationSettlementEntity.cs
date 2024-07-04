using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelRequest.DB.Entityes
{
    [Table("ApplicationSettlement")]
    public class ApplicationSettlementEntity
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual StudentEntity Student { get; set; }

        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public virtual RoomEntity Room { get; set; }

        public bool? Accepted { get; set; }
    }
}
