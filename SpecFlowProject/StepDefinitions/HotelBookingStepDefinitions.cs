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
    public sealed class HotelBookingStepDefinitions
    {
        private IBookingManager bookingManager;

        private Mock<IRepository<Booking>> mockBookingrepository;
        private Mock<IRepository<Room>> mockRoomRepository;
        public HotelBookingStepDefinitions() {
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
        Booking booking = new Booking();
        bool result;


        [Given(@"I have a booking starting in number of days after today '(.*)'")]
        public void GivenStartdateIsTomorrow(string startDate)
        {
            booking.StartDate = DateTime.Today.AddDays(int.Parse(startDate));

        }

        [Given(@"The booking is ending in number of days after today '(.*)'")]
        public void BookingIsEndingTheDayAfterTomorrow(string endDate)
        {
            booking.EndDate = DateTime.Today.AddDays(int.Parse(endDate));

            
        }

        [When(@"booking is attempted to be created")]
        public void BookingIsAttemptedToBeCreated()
        {
            result = bookingManager.CreateBooking(booking);
        }

        [Then(@"The result of the booking should be '(.*)'")]
        public void TheResultOfTheBookingShouldBe(string tableResult)
        {
            bool expectedResult = bool.Parse(tableResult);
            Assert.Equal(expectedResult, result);
        }
    }
}
