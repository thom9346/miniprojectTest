using HotelBooking.Core;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecFlowProject.StepDefinitions
{
    [Binding]
    public sealed class OccupiedDatesStepDefinitions
    {
        private IBookingManager bookingManager;

        private Mock<IRepository<Booking>> mockBookingrepository;
        private Mock<IRepository<Room>> mockRoomRepository;
        public OccupiedDatesStepDefinitions()
        {
            DateTime fullyOccupiedStartDate = DateTime.Today.AddDays(10);
            DateTime fullyOccupiedEndDate = DateTime.Today.AddDays(20);
            //bookingRepository = new FakeBookingRepository(start, end);
            //IRepository<Room> roomRepository = new FakeRoomRepository();

            mockBookingrepository = new Mock<IRepository<Booking>>();
            mockRoomRepository = new Mock<IRepository<Room>>();

            var rooms = new List<Room>
            {
                new Room { Id=1, Description="A" },
                new Room { Id=2, Description="B" },
            };
            mockRoomRepository.Setup(x => x.GetAll()).Returns(rooms);
            var bookings = new List<Booking>
            {
                new Booking { Id=1, StartDate=DateTime.Today.AddDays(1), EndDate=DateTime.Today.AddDays(1), IsActive=true, CustomerId=1, RoomId=1 },
                new Booking { Id=1, StartDate=fullyOccupiedStartDate, EndDate=fullyOccupiedEndDate, IsActive=true, CustomerId=1, RoomId=1 },
                new Booking { Id=2, StartDate=fullyOccupiedStartDate, EndDate=fullyOccupiedEndDate, IsActive=true, CustomerId=2, RoomId=2 },
            };

            mockBookingrepository.Setup(x => x.GetAll()).Returns(bookings);

            bookingManager = new BookingManager(mockBookingrepository.Object, mockRoomRepository.Object);
        }

        DateTime startDate;
        DateTime endDate;
        int result;

        [Given(@"I want to check how many fully occupied dates in range of dates starting from number of days after today '(.*)'")]
        public void GivenCheckNumberOfFullyOccupiedDatesInRange(int number)
        {
            startDate = DateTime.Today.AddDays(number);

        }

        [Given(@"The same interval ends in number of days after today '(.*)'")]
        public void AndIntervalEndsInNumberOfDaysAfterToday(int number)
        {
            endDate = DateTime.Today.AddDays(number);

        }

        [When(@"Number of bookings are calculated")]
        public void WhenNumberOfBookingsAreCalculated()
        {
            result = bookingManager.GetFullyOccupiedDates(startDate,endDate).Count;

        }
        [Then(@"The result is '(.*)'")]
        public void ThenTheResultIs(int number)
        {
            Assert.Equal(number, result);
        }
    }
}
