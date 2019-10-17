using Meetings.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Meetings.ForMeetings
{
    /// <summary>
    /// Сервис для работы с совещаниями.
    /// </summary>
    public class MeetingService
    {
        public MeetingService()
        { }
        
        /// <summary>
        /// Создание нового совещания.
        /// </summary>
        /// <param name="user">Организатор совещания</param>
        /// <param name="meetingViewModel">Данные о совещании, возвращенные из представления</param>
        /// <returns>Модель совещания, готовая для сохранения в базу данных</returns>
        public MeetingModel New(string user, MeetingViewModel meetingViewModel)
        {
            DateTime now = DateTime.Now;
            var notRegisteredParticipants = meetingViewModel.NotRegisteredParticipants;
            var participants = new List<string>();
            if (notRegisteredParticipants != null)
            {
                if (meetingViewModel.Participants != null)
                {
                    meetingViewModel.Participants.AddRange(notRegisteredParticipants);
                    participants = meetingViewModel.Participants;
                }
                else
                    participants = notRegisteredParticipants;
            }
            else
            {
                participants = meetingViewModel.Participants;
            }


            MeetingModel meeting = new MeetingModel
            {
                Organizer = user,
                Topic = meetingViewModel.Topic,
                Participants = participants.Aggregate((a, b) => a + ", " + b),
                StartTime = now,
                LastRegisteredTime = now
            };

            return meeting;
        }

        /// <summary>
        /// Обновление состояния совещания.
        /// </summary>
        /// <param name="meeting">Совещание</param>
        public async Task<MeetingModel> UpdateMeetingAsync(MeetingModel meeting)
        {
            using (var meetingContext = new ApplicationDbContext())
            {
                meetingContext.Entry(meeting).State = EntityState.Modified;

                await meetingContext.SaveChangesAsync();
            }

            return meeting;
        }

        /// <summary>
        /// Получение данных о совещании из бпзы данных.
        /// </summary>
        /// <param name="id">Идентификатор совещания</param>
        public async Task<MeetingModel> GetMeetingAsync(long id)
        {
            MeetingModel result = null;
            using (var meetingContext = new ApplicationDbContext())
            {
                result = await meetingContext.Meetings.FirstOrDefaultAsync(f => f.Id == id);
            }
            return result;
        }
    }
}