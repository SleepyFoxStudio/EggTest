Feature: EggTimer


@mytag
Scenario: Time 25 seconds
	Given the timer is set to 25 seconds
	When the timer is run
	Then the timer counted down from 25 seconds
	Then the timer updated every second
	Then the time decreased by one second each update