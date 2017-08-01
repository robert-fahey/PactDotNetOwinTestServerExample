Feature: GetFormattedDate

@Authorised
Scenario: Get formatted date in english
	When I request the formatted date '2017-03-20T12:00:01.00Z' for language 'en-GB'
	Then the response status code should be 'OK'
	And the response formatted date should be
	"""
	{
		"date" : "2017-03-20T12:00:01.00Z",
		"language" : "en-GB",
		"formattedDate" : "20 March 2017 12:00:01"
	}
	"""

@Authorised
Scenario: Get formatted date in spanish
	When I request the formatted date '2017-03-20T12:00:01.00Z' for language 'es-ES'
	Then the response status code should be 'OK'
	And the response formatted date should be
	"""
	{
		"date" : "2017-03-20T12:00:01.00Z",
		"language" : "es-ES",
		"formattedDate" : "lunes, 20 de marzo de 2017 12:00:01"
	}
	"""