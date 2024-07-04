namespace HostelRequest.DB.Entityes
{
    public class AdminEntity
    {
        public int Id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int hostelId { get; set; }
    }
}
