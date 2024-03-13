Feature: OccupiedDates

A short summary of the feature

@tag1
Scenario: validate fully occupied dates
	Given I want to check how many fully occupied dates in range of dates starting from number of days after today <startDay>
	And The same interval ends in number of days after today <endDay>
	When Number of bookings are calculated
	Then The result is <result>

	Examples: 
	| startDay | endDay | result |
	| '1' | '200'|'11'|
	|'25' | '29' |'0'|
