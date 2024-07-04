using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelRequest.DB.Entityes
{
    [Table("Hostel")]
    public class HostelEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Number { get; set; }

        [Required]
        public string Adres { get; set; }

        [Required]
        [StringLength(10)]
        public string Facultet { get; set; }

        [Required]
        public double PricePerMonth { get; set; }
        
        public virtual ICollection<RoomEntity> Rooms { get; set; }
    }
}
