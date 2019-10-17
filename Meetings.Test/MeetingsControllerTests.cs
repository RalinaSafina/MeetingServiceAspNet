using Meetings.Controllers;
using Meetings.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meetings.Test
{
    [TestFixture]
    class MeetingsControllerTests
    {
        [Test]
        public void CountMeetingPriceTest()
        {
            MeetingsController controller = new MeetingsController();

            DateTime start = new DateTime(2019, 9, 1, 10, 10, 0, 0);
            DateTime stop = new DateTime(2019, 9, 1, 11, 10, 0, 0);
            MeetingModel meeting = new MeetingModel()
            {
                StartTime = start,
                StopTime = stop,
                Participants = "Катя, Маша"
            };

            var result = controller.CountMeetingPrice(meeting, 60);
            var expected = 120.0;

            Assert.AreEqual(expected, result);
        }
    }
}
