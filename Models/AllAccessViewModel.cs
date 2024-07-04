using HostelRequest.DB.Entityes;

namespace HostelRequest.Models
{
    public class AllAccessViewModel
    {
        public List<HostelEntity>? hostels { get; set; }
        public List<AdminEntity>? admins { get; set; }


        public int hostel_number { get; set; }
        public string hostel_adres { get; set; }
        public string hostel_facult { get; set; }
        public double hostel_price { get; set; }
        public string? hostel_err { get; set; } = null;


        public string adm_login { get; set; }
        public string adm_password { get; set; }
        public int adm_hostel_id { get; set; }
        public string adm_err { get; set; }

        public int adm_del_id { get; set; }
        public int hostel_del_id { get; set; }
    }
}
