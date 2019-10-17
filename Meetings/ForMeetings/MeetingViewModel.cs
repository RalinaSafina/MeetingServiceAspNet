using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Meetings.ForMeetings
{
    public class MeetingViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Организатор")]
        public string Organizer { get; set; }

        [Required]
        [Display(Name = "Тема")]
        public string Topic { get; set; }

        [Required]
        [Display(Name = "Участники")]
        public List<string> Participants { get; set; }

        /// <summary>
        /// Список участников совещания, не являющихся пользователями
        /// </summary>
        public List<string> NotRegisteredParticipants { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime StopTime { get; set; }

        /// <summary>
        /// Время последнего обновления
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime LastRegisteredTime { get; set; }

        public double TotalPrice { get; set; }
    }
}