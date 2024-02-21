using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using System.Collections.Generic;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        IRepository<Booking> bookingRepository;

        public BookingManagerTests(){
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository);
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
            var bookingForReturnedRoomId = bookingRepository.GetAll().Where(
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
