using HostelRequest.DB.Entityes;

namespace HostelRequest.Models
{
    public class SingleAdminViewModel
    {
        public HostelEntity currentHostel { get; set; } = new HostelEntity();
        public List<OrderView> orders { get; set; } = new List<OrderView>();
        public List<RoomEntity> rooms { get; set; } = new List<RoomEntity>();

        public string? on_new_room_err { get; set; } = null;
    }
}
