using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Meetings.ForMeetings
{
    public class StatisticsViewModel
    {
        [Required]
        [Display(Name ="С какого дня:")]
        public DateTime FromDay { get; set; }

        [Required]
        [Display(Name = "До какого дня:")]
        public DateTime ToDay { get; set; }
        
        public List<MeetingsForStatistics> Meetings { get; set; }
    }

    public class MeetingsForStatistics
    {
        public string Organizer { get; set; }

        public DateTime StartTime { get; set; }

        public int MeetingsAmount { get; set; }

        public double MeetingsTotalPrice { get; set; }

        public double Duration { get; set; }
    }
}