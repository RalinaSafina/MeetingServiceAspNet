using Meetings.ForMeetings;
using Meetings.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Meetings.Controllers
{
    /// <summary>
    /// Контроллер для совещаний.
    /// </summary>
    [Authorize(Roles = "user")]
    public class MeetingsController : Controller
    {
        private ApplicationUserManager userManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        ApplicationDbContext db = new ApplicationDbContext();
        MeetingService service = new MeetingService();

        //Цена проведения часа совещания с человека за час
        double pricePerHour = 60; 

        /// <summary>
        /// Метод для вычисления суммы, необходимой для оплаты за совещание.
        /// </summary>
        /// <param name="meeting">Совещание</param>
        /// <param name="price">Цена за час с человека в рублях</param>
        public double CountMeetingPrice(MeetingModel meeting, double price)
        {
            return Math.Round((meeting.StopTime - meeting.StartTime).TotalMilliseconds * price / 3600000.0 * meeting.Participants.Split(' ').Length, 2);
        }

        /// <summary>
        /// Метод, отображающий страницу совещания(поля, организатора, стоимость за час)
        /// </summary>
        [HttpGet]
        public ActionResult Page()
        {
            ViewBag.price = pricePerHour;
            ViewBag.email = HttpContext.User.Identity.Name;
            List<string> userNames = new List<string>();

            foreach (var user in userManager.Users)
            {
                userNames.Add(user.Name);
            }

            foreach (var extra in db.Participants)
            {
                userNames.Add(extra.Name);
            }

            ViewBag.data = userNames;
            return View();
        }

        /// <summary>
        /// Добавление нового совещания в базу данных.
        /// </summary>
        /// <param name="meetingViewModel">Заполненная модель совещания, полученная из представления</param>
        [HttpPost]
        public JsonResult New(MeetingViewModel meetingViewModel)
        {
            var notRegisteredParticipants = meetingViewModel.NotRegisteredParticipants;
            if (notRegisteredParticipants != null)
            {
                foreach (var item in notRegisteredParticipants)
                {
                    NotRegisteredParticipants part = new NotRegisteredParticipants { Name = item };
                    db.Participants.Add(part);
                    db.SaveChanges();
                }
            }

            MeetingModel meeting = service.New(HttpContext.User.Identity.Name, meetingViewModel);

            db.Meetings.Add(meeting);
            db.SaveChanges();
            return Json(meeting);
        }

        /// <summary>
        /// Метод возвращает текущее незавершенное совещание.
        /// </summary>
        [HttpGet]
        public JsonResult Current()
        {
            DateTime nullDate = new DateTime(0001, 1, 1, 0, 0, 0);
            MeetingModel currentMeeting = db.Meetings
                                            .AsEnumerable()
                                            .Where(p => p.Organizer == HttpContext.User.Identity.Name && p.StopTime == nullDate)
                                            .Select(p => new MeetingModel
                                            {
                                                Organizer = p.Organizer,
                                                Participants = p.Participants,
                                                StartTime = p.StartTime,
                                                Topic = p.Topic,
                                                Id = p.Id,
                                                LastRegisteredTime = p.LastRegisteredTime,
                                                StopTime = p.StopTime
                                            })
                                            .OrderByDescending(p => p.Id)
                                            .FirstOrDefault();
            if (currentMeeting != null)
            {
                if ((DateTime.Now - currentMeeting.LastRegisteredTime).TotalMilliseconds > 300000)
                {
                    currentMeeting.StopTime = currentMeeting.LastRegisteredTime;
                    currentMeeting.TotalPrice = CountMeetingPrice(currentMeeting, pricePerHour);
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    currentMeeting.LastRegisteredTime = DateTime.Now;
                    return Json(currentMeeting, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(null, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Обновляет LastRegisteredTime в базе данных.
        /// </summary>
        /// <param name="meetingId">Идентификатор текущего совещания</param>
        [HttpPost]
        public JsonResult Update(long meetingId)
        {
            var meeting = Task.Run(async () => await service.GetMeetingAsync(meetingId)).Result;
            if(meeting != null)
            {
                meeting.LastRegisteredTime = DateTime.Now;
            }
            Task.Run(async () => await service.UpdateMeetingAsync(meeting));
            return Json(null);
        }

        /// <summary>
        /// Завешает совещание, подсчитывает итоговую цену совещания.
        /// </summary>
        /// <param name="meetingId">Идентификатор текущего совещания</param>
        [HttpPost]
        public JsonResult Stop(long meetingId)
        {
            var meeting = Task.Run(async () => await service.GetMeetingAsync(meetingId)).Result;
            if (meeting != null)
            {
                meeting.StopTime = DateTime.Now;
                meeting.TotalPrice = CountMeetingPrice(meeting, pricePerHour);
            }
            Task.Run(async () => await service.UpdateMeetingAsync(meeting));
            return Json(meeting);
        }
    }
}