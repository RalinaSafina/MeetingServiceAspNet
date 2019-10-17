using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Meetings.Models
{
    [Serializable]
    [DataContract]
    public class MeetingModel
    {
        [DataMember]
        public long Id { get; set; }

        [Required]
        [DataMember]
        [Display(Name ="Организатор")]
        public string Organizer { get; set; }

        [Required]
        [DataMember]
        [Display(Name = "Тема")]
        public string Topic { get; set; }

        [Required]
        [DataMember]
        [Display(Name = "Участники")]
        public string Participants { get; set; }

        [Required]
        [DataMember]
        [DataType(DataType.Date)]
        public DateTime StartTime { get; set; }

        [DataMember]
        [DataType(DataType.Date)]
        public DateTime StopTime { get; set; }

        [DataMember]
        [DataType(DataType.Date)]
        public DateTime LastRegisteredTime { get; set; }

        public double TotalPrice { get; set; }
    }
}