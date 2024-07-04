using HostelRequest.DB;
using HostelRequest.DB.Entityes;
using HostelRequest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HostelRequest.Controllers
{
    public class AdminController : Controller
    {
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            return View(new SignInViewModel());
        }

        public void addCookie(AdminEntity? user)
        {
            if (user == null) return;
            Response.Cookies.Append("user", $"{ComputeSha256Hash(user.login + user.password)}", new CookieOptions { Expires = DateTime.Now.AddDays(7) });
            Response.Cookies.Append("id", $"{user.Id}", new CookieOptions { Expires = DateTime.Now.AddDays(7) });
        }
        public void RemoveCookie()
        {
            Response.Cookies.Delete("user");
            Response.Cookies.Delete("id");
            Response.Cookies.Delete("hos");
        }

        public IActionResult Leave()
        {
            RemoveCookie();
            return RedirectToAction("Index", "Home");
        }

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }


        public IActionResult CheckAdmin()
        {
            AdminEntity? currentUser = isAdminValid();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Admin");
            }
            Response.Cookies.Append("hos", $"{currentUser.hostelId}");

            currentUser = isSuperAdmin();
            if (currentUser != null)
            {
                return RedirectToAction("AllAccessControl");
            }
            else
            {
                return RedirectToAction("SingleAdmin");
            }
        }

        [HttpPost]
        public IActionResult Enter(SignInViewModel model)
        {
            AdminEntity? user;
            HostelEntity? allAccessHostel;
            using (myDbContext context = new myDbContext())
            {
                user = context.Admins.FirstOrDefault(x => x.login == model.login && x.password == model.password);
                allAccessHostel = context.Hostels.FirstOrDefault(x => x.Number == -1 && x.Adres == "-1" && x.Facultet == "-1");
            }

            if (user == null)
            {
                model.form_message = "Не знайдено такого користувача";
                return View("SignIn", model);
            }

            addCookie(user);

            return RedirectToAction("CheckAdmin");
        }

        private AdminEntity? isAdminValid()
        {
            AdminEntity? currentUser = null;
            if (string.IsNullOrEmpty(Request.Cookies["Id"]) || string.IsNullOrEmpty(Request.Cookies["user"])) return null;

            using (myDbContext context = new myDbContext())
            {
                currentUser = context.Admins.FirstOrDefault(x => x.Id.ToString() == Request.Cookies["Id"]);
            }

            if (ComputeSha256Hash(currentUser.login + currentUser.password) != Request.Cookies["user"])
            {
                RemoveCookie();
                return null;
            }
            return currentUser;
        }
        private AdminEntity? isSuperAdmin()
        {
            AdminEntity? currentUser = isAdminValid();
            if (currentUser == null) return null;

            HostelEntity? allAccessHostel;
            using (myDbContext context = new myDbContext())
            {
                allAccessHostel = context.Hostels.FirstOrDefault(x => x.Number == -1 && x.Adres == "-1" && x.Facultet == "-1");
            }

            if (currentUser.hostelId == allAccessHostel.Id)
            {
                return currentUser;
            }
            else return null;
        }

        #region SINGLE ADMIN CONTROL

        public IActionResult SingleAdmin()
        {
            SingleAdminViewModel model = new SingleAdminViewModel();
            List<ApplicationSettlementEntity> orders = new List<ApplicationSettlementEntity>();

            using (myDbContext context = new myDbContext())
            {
                model.currentHostel = context.Hostels.First(x => x.Id == int.Parse(Request.Cookies["hos"]));
                model.rooms = context.Rooms.Where(x => x.HostelId == model.currentHostel.Id).ToList();

                if (Request.Cookies["hos"] == null)
                {
                    RemoveCookie();
                    return RedirectToAction("SignIn");
                }
                orders = context.Applications.Where(x => x.Room.HostelId == int.Parse(Request.Cookies["hos"])).ToList();

                foreach (ApplicationSettlementEntity order in orders)
                {
                    StudentEntity? student = context.Students.First(x => x.Id == order.StudentId);
                    RoomEntity? room = context.Rooms.First(x => x.Id == order.RoomId);

                    if (student == null || room == null) continue;

                    model.orders.Add(new OrderView
                    {
                        Id = order.Id,
                        Accepted = order.Accepted,
                        RoomId = order.RoomId,
                        StudentId = order.StudentId,
                        student_info = student,
                        room_info = room
                    });
                }
            }

            if (TempData["OnRoomErr"] != null) model.on_new_room_err = TempData["OnRoomErr"].ToString();

            return View(model);
        }

        [HttpPost]
        public IActionResult OrderAcceptReject(int order_id, string action)
        {
            using (myDbContext context = new myDbContext())
            {
                ApplicationSettlementEntity? order = context.Applications.FirstOrDefault(x => x.Id == order_id);
                if (order == null) return RedirectToAction("SingleAdmin");

                if (action == "accept")
                {
                    order.Accepted = true;
                }
                if (action == "reject")
                {
                    order.Accepted = false;
                }

                context.Applications.Update(order);
                context.SaveChanges();
            }
            return RedirectToAction("SingleAdmin");
        }

        [HttpPost]
        public IActionResult AddNewRoom(int hostel_id, int number, int floor, double square, int places)
        {
            using (myDbContext context = new myDbContext())
            {
                if (context.Rooms.Any(x => x.Number == number && x.HostelId == hostel_id))
                {
                    TempData["OnRoomErr"] = "Кімната з таким номером вже є!";
                    return RedirectToAction("SingleAdmin");
                }

                context.Rooms.Add(new RoomEntity
                {
                    Number = number,
                    Places = places,
                    Square = square,
                    Floor = floor,
                    HostelId = hostel_id
                });
                context.SaveChanges();
            }
            return RedirectToAction("SingleAdmin");
        }

        [HttpPost]
        public IActionResult RemoveRoom(int room_id)
        {
            using (myDbContext context = new myDbContext())
            {
                RoomEntity? room = context.Rooms.FirstOrDefault(x => x.Id == room_id);

                if(room == null)
                {
                    TempData["OnRoomErr"] = "Щось пішло не так під час видалення...";
                    return RedirectToAction("SingleAdmin");
                }

                context.Rooms.Remove(room);
                context.SaveChanges();
            }
            return RedirectToAction("SingleAdmin");
        }

        #endregion


        #region SUPER ADMIN CONTROL

        public IActionResult AllAccessControl()
        {
            AdminEntity? currentUser = isSuperAdmin();

            if (currentUser == null)
            {
                RemoveCookie();
                return RedirectToAction("SignIn", "Admin");
            }

            AllAccessViewModel model = new AllAccessViewModel();
            using (myDbContext context = new myDbContext())
            {
                model.admins = context.Admins.Where(x => x.login != "allaccess").ToList();
                model.hostels = context.Hostels.Where(x => x.Number != -1).ToList();
            }

            if (TempData["adm_err"] != null) model.adm_err = TempData["adm_err"].ToString();
            if (TempData["hostel_err"] != null) model.hostel_err = TempData["hostel_err"].ToString();

            return View(model);
        }

        [HttpPost]
        public IActionResult RemoveAdmin(AllAccessViewModel model)
        {
            using (myDbContext context = new myDbContext())
            {
                AdminEntity? admin = context.Admins.FirstOrDefault(x => x.Id == model.adm_del_id);
                if (admin != null)
                {
                    context.Admins.Remove(admin);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("AllAccessControl");
        }

        [HttpPost]
        public IActionResult AddAdmin(AllAccessViewModel model)
        {
            using (myDbContext context = new myDbContext())
            {
                if (context.Admins.Any(x => x.login == model.adm_login)) TempData["adm_err"] = "Логін зайнято";
                else
                {
                    context.Admins.Add(new AdminEntity
                    {
                        login = model.adm_login,
                        password = model.adm_password,
                        hostelId = model.adm_hostel_id
                    });
                    context.SaveChanges();
                }
            }
            return RedirectToAction("AllAccessControl");
        }

        [HttpPost]
        public IActionResult RemoveHostel(AllAccessViewModel model)
        {
            using (myDbContext context = new myDbContext())
            {
                HostelEntity? hostel = context.Hostels.FirstOrDefault(x => x.Id == model.hostel_del_id);
                if (hostel != null)
                {
                    context.Hostels.Remove(hostel);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("AllAccessControl");
        }

        [HttpPost]
        public IActionResult AddHostel(AllAccessViewModel model)
        {
            using (myDbContext context = new myDbContext())
            {
                if (context.Hostels.Any(x => x.Number == model.hostel_number)) TempData["hostel_err"] = "Номер гуртожитку зайнято";
                else
                {
                    context.Hostels.Add(new HostelEntity
                    {
                        Number = model.hostel_number,
                        Adres = model.hostel_adres,
                        Facultet = model.hostel_facult,
                        PricePerMonth = model.hostel_price,

                    });
                    context.SaveChanges();
                }
            }
            return RedirectToAction("AllAccessControl");
        }

        #endregion
    }
}
