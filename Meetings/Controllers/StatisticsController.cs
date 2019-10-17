using Meetings.ForMeetings;
using Meetings.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Meetings.Controllers
{
    /// <summary>
    /// Контроллер для статистики.
    /// </summary>
    [Authorize(Roles = "admin")]
    public class StatisticsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Отображает представление страницы статистики.
        /// </summary>
        [HttpGet]
        public ActionResult GetStatistics()
        {
            StatisticsViewModel model = new StatisticsViewModel();
            model.FromDay = DateTime.Today;
            ViewBag.Today = model.FromDay.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            model.ToDay = model.FromDay.AddDays(1);
            ViewBag.Tomorrow = model.ToDay.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            model.Meetings = db.Meetings
                             .Where(p => p.StartTime >= model.FromDay && p.StopTime <= model.ToDay)
                             .OrderBy(p => p.Organizer)
                             .OrderBy(p => p.StartTime)
                             .AsEnumerable()
                             .Select(meeting => new MeetingsForStatistics
                             {
                                 Organizer = meeting.Organizer,
                                 StartTime = meeting.StartTime,
                                 MeetingsAmount = 1,
                                 MeetingsTotalPrice = meeting.TotalPrice,
                                 Duration = Math.Round((meeting.StopTime - meeting.StartTime).TotalMilliseconds / 3600000, 4)
                             })
                                     .ToList();
            ViewBag.organizers = db.Meetings.Where(p => p.StartTime >= DateTime.Today).Select(p => p.Organizer).Distinct().Count();
            ViewBag.meetings = model.Meetings.Sum(x => x.MeetingsAmount);
            ViewBag.price = model.Meetings.Sum(x => x.MeetingsTotalPrice);
            ViewBag.totalDuration = model.Meetings.Sum(x => x.Duration);
            return View(model);
        }

        /// <summary>
        /// Обновление данных частичного представления(разультат sql-запроса)
        /// </summary>
        /// <param name="statistics">Временные рамки для отображения статистики</param>
        [HttpPost]
        public ActionResult GetStatisticsPost(StatisticsViewModel statistics)
        {
            var from = statistics.FromDay;
            var to = statistics.ToDay;
            DateTime check = new DateTime(0001, 1, 1);

            
            if(to.Date == check)
            {
                to = DateTime.MaxValue;
            }

            List<MeetingsForStatistics> list = new List<MeetingsForStatistics>();
            list = db.Meetings
                             .Where(p => p.StartTime >= from && p.StopTime <= to)
                             .OrderBy(p => p.Organizer)
                             .OrderBy(p => p.StartTime)
                             .AsEnumerable()
                             .Select(meeting => new MeetingsForStatistics
                                    {
                                        Organizer = meeting.Organizer,
                                        StartTime = meeting.StartTime,
                                        MeetingsAmount = 1,
                                        MeetingsTotalPrice = meeting.TotalPrice,
                                        Duration = Math.Round((meeting.StopTime - meeting.StartTime).TotalMilliseconds / 3600000, 4)
                                     })
                                     .ToList();
            ViewBag.organizers = db.Meetings.Where(p => p.StartTime >= from && p.StopTime <= to).Select(p => p.Organizer).Distinct().Count();
            ViewBag.meetings = list.Sum(x => x.MeetingsAmount);
            ViewBag.price = list.Sum(x => x.MeetingsTotalPrice);
            ViewBag.totalDuration = list.Sum(x => x.Duration);
            return PartialView(list);
        }
    }
}