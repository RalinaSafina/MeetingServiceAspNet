using Meetings.ForMeetings;
using Meetings.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Meetings.Test
{
    [TestFixture]
    class MeetingServiceTests
    {
        [Test]
        public void NewTest()
        {
            MeetingService service = new MeetingService();
            DateTime start = DateTime.Now;
            DateTime last = DateTime.Now;

            string user = "Александр";
            MeetingViewModel model = new MeetingViewModel
            {
                Topic = "Тема",
                Participants = new List<string>(){ "Миша", "Катя" },
                NotRegisteredParticipants = new List<string>() { "Диана" },
                StartTime = start,
                LastRegisteredTime = last
            };

            MeetingModel expected = new MeetingModel
            {
                Organizer = user,
                Topic = "Тема",
                Participants = "Миша, Катя, Диана",
                StartTime = start,
                LastRegisteredTime = last
            };

            MeetingModel actual = service.New(user, model);

            Assert.AreEqual(expected.Organizer, actual.Organizer);
            Assert.AreEqual(expected.Topic, actual.Topic);
            Assert.AreEqual(expected.Participants, actual.Participants);
        }
        
    }
}
