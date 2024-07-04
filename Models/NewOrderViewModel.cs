using HostelRequest.DB.Entityes;

namespace HostelRequest.Models
{
    public class NewOrderViewModel
    {
        public string PIB { get; set; }
        public string group { get; set; }
        public string facult { get; set; }

        public List<HostelEntity> hostels { get; set; } = new List<HostelEntity>();
        public string? pushed_message { get; set; } = null;

        public int hostel_id { get; set; }
        public int room_id { get; set; }
    }
}
