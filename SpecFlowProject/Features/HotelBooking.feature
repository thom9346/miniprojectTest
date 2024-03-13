Feature: HotelBooking

A short summary of the feature

@tag1
Scenario: Book available room
	Given I have a booking starting in number of days after today <startDay>
	And The booking is ending in number of days after today <endDay>
	When booking is attempted to be created
	Then The result of the booking should be <result>


	Examples: 
	| startDay | endDay | result |
	| '1'|'2'|'True'|
	|'11'|'12'|'False'|