This is a single-page-app dictionary which is responsive. Currently it supports English-Vietnamese dictionary

1. DataAcess
	Data access layer of the Dictionary app using Entity Framework Core.
	It is designed based on tratu page, which is a abandoned website, in order to take it data.

	TODO list:
	- How about uniqueness of word content?

2. Collect Data
	A .NET core console app that collects all dictionary data from http://tratu.soha.vn/ to fill the database.
	It is using multiple threading to speed up the process

	TODO list:
	- Able to cancel processing and start again at stopped point
	- Able to save current state when the app is stopped by the system such as shutdown
	- A better way to implement concurrent process?
	- Justify parsing to reduce error words
	- Adapt to read other dictionaries like Japanese-Vietnamese

3. Dictionary
	A dictionary app includes a service using Asp.net Core API and a client using Angular.

   TODO list:
   - Developing
   - Exception when getting the word Action, About
   - use a pipe for spelling html
   - / in searching words
   - add a spinner