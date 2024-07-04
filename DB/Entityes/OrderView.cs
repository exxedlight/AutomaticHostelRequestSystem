namespace HostelRequest.DB.Entityes
{
    public class OrderView : ApplicationSettlementEntity
    {
        public StudentEntity student_info { get; set; } = new StudentEntity();
        public RoomEntity room_info { get; set; } = new RoomEntity();
    }
}
