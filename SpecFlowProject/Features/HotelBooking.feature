Feature: HotelBooking

A short summary of the feature

@tag1
Scenario Template: Book available room
	Given I have a booking starting in number of days after today <startDay>
	And The booking is ending in number of days after today <endDay>
	When booking is attempted to be created
	Then The result of the booking should be <result>

	Examples: 
	| startDay | endDay | result |
	| '1'|'2'|'True'|
	|'11'|'12'|'False'|
	| '1' | '1' | 'True' |
	| '21' | '22' | 'True' |
	| '1' | '22' | 'False' |
	| '1' | '10' | 'False' |
	| '1' | '20' | 'False' |
	| '10' | '22' | 'False' |
	| '20' |' 22' | 'False' |
	| '10' | '10'| 'False' |
	| '20' | '20' | 'False' |
	| '10' | '20' | 'False' |
