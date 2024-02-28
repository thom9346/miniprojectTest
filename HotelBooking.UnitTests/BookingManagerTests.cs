using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Moq;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        //IRepository<Booking> bookingRepository;
        private Mock<IRepository<Booking>> mockBookingrepository;
        private Mock<IRepository<Room>> mockRoomRepository;

        public BookingManagerTests(){
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
        public static IEnumerable<object[]> GetOccupiedDates()
        {
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            var data = new List<object[]>
            {
                new object[] { start, end },
                new object[] { DateTime.Today.AddDays(11), DateTime.Today.AddDays(14) },
                new object[] { DateTime.Today.AddDays(14), DateTime.Today.AddDays(16) },
                new object[] { DateTime.Today.AddDays(16), DateTime.Today.AddDays(19) }
            };

            return data;
        }
        public static IEnumerable<object[]> GetFreeDates()
        {
            var data = new List<object[]>
            {
                new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(1) },
                new object[] { DateTime.Today.AddDays(21), DateTime.Today.AddDays(21) },

            };
            return data;
        }
        [Fact]
        private void GetFullyOccupiedDates_11OccupiedDates_ReturnsListOf11()
        {
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(200);

            List<DateTime> list = bookingManager.GetFullyOccupiedDates(startDate, endDate);

            Assert.Equal(list.Count, 11);
        }
        [Fact]
        private void GetFullyOccupiedDates_NotOccupiedDay_ReturnsEmptyList()
        {
            DateTime startDate = DateTime.Today.AddDays(25);
            DateTime endDate = DateTime.Today.AddDays(29);

            List<DateTime> list = bookingManager.GetFullyOccupiedDates(startDate, endDate);

            Assert.Empty(list);
        }
        [Fact]
        private void GetFullyOccupiedDates_StartDateBeforeEndDate_ThrowsArgumentException() 
        {
            DateTime startDate = DateTime.Today.AddDays(25);
            DateTime endDate = DateTime.Today.AddDays(22);

            Action act = () => bookingManager.GetFullyOccupiedDates(startDate, endDate);

            Assert.Throws<ArgumentException>(act);
        }
        [Fact]
        private void CreateBooking_RoomAvailable_ReturnsTrue()
        {
            //arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2)
            };
            //act
            var act = bookingManager.CreateBooking(booking);

            //assert
            Assert.True(act);
        }
        [Fact]
        private void CreateBooking_RoomNotAvailable_ReturnsFalse()
        {
            //arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(11),
                EndDate = DateTime.Today.AddDays(12)
            };
            //act
            var act = bookingManager.CreateBooking(booking);

            //assert
            Assert.False(act);
        }

        [Fact]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = DateTime.Today;

            // Act
            Action act = () => bookingManager.FindAvailableRoom(date, date);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }
        [Fact]
        public void FindAvailableRoom_StartDateInThePast_ThrowsArgumentException()
        {
            // Arrange
            DateTime yesterday = DateTime.Today.AddDays(-1);
            DateTime today = DateTime.Today;

            // Act
            Action act = () => bookingManager.FindAvailableRoom(yesterday, today);

            // Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Theory]
        [MemberData(nameof(GetFreeDates))]
        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne(DateTime startDate, DateTime endDate)
        {
            // Act
            int roomId = bookingManager.FindAvailableRoom(startDate, endDate);
            // Assert
            Assert.NotEqual(-1, roomId);
        }

        [Fact]
        public void FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom()
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);

            // Assert
            var bookingForReturnedRoomId = mockBookingrepository.Object.GetAll().Where(
                b => b.RoomId == roomId
                && b.StartDate <= date
                && b.EndDate >= date
                && b.IsActive);

            Assert.Empty(bookingForReturnedRoomId);
        }

        [Theory]
        [MemberData(nameof(GetOccupiedDates))]
        public void FindAvailableRoom_RoomNotAvailable_ReturnsMinusOne(DateTime startDate, DateTime endDate)
        {
            // Act
            int roomId = bookingManager.FindAvailableRoom(startDate, endDate);
            // Assert
            Assert.Equal(-1, roomId);
        }
    }
}
