using HostelRequest.DB;
using HostelRequest.DB.Entityes;
using HostelRequest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HostelRequest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            NewOrderViewModel model = new NewOrderViewModel();
            using (myDbContext context = new myDbContext())
            {
                model.hostels = context.Hostels.Where(x => x.Number != -1).ToList();
            }
            if (TempData["OrderPushed"] != null) model.pushed_message = TempData["OrderPushed"].ToString();
            return View(model);
        }

        [HttpPost]
        public JsonResult HostelChanged(int hostel_id)
        {
            HostelEntity? hostel = null;
            List<RoomEntity> rooms = new List<RoomEntity>();

            using (myDbContext context = new myDbContext())
            {
                hostel = context.Hostels.FirstOrDefault(x => x.Id == hostel_id);
            }

            if (hostel == null) return Json(new { success = false });

            using (myDbContext context = new myDbContext())
            {
                rooms = context.Rooms.Where(x => x.HostelId == hostel.Id).ToList();
            }

            return Json(new
            {
                success = true,
                adres = hostel.Adres,
                faculty = hostel.Facultet,
                pricePerMonth = hostel.PricePerMonth,
                rooms = rooms
            });
        }

        [HttpPost]
        public JsonResult RoomChanged(int room_id)
        {
            RoomEntity? room = null;
            using (myDbContext context = new myDbContext())
            {
                room = context.Rooms.FirstOrDefault(x => x.Id == room_id);
            }

            if (room == null) return Json(new { success = false });

            return Json(new
            {
                success = true,
                places = room.Places,
                floor = room.Floor,
                square = room.Square
            });
        }

        [HttpPost]
        public IActionResult pushNewOrder(NewOrderViewModel model)
        {

            using (myDbContext context = new myDbContext())
            {
                StudentEntity student = new StudentEntity
                {
                    PIB = model.PIB,
                    Facultet = model.facult,
                    Group = model.group,
                    RoomId = model.room_id
                };

                context.Students.Add(student);
                context.SaveChanges();

                ApplicationSettlementEntity order = new ApplicationSettlementEntity
                {
                    Accepted = null,
                    RoomId = model.room_id,
                    StudentId = student.Id
                };

                context.Applications.Add(order);
                context.SaveChanges();
            }

            TempData["OrderPushed"] = "Заяву відправлено";
            return RedirectToAction("Index", "Home");
        }








        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
