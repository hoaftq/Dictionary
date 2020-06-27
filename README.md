This is a responsive single-page-app dictionary using Angular and Bootstrap. Currently it supports English-Vietnamese dictionary and going to support Japanese-Vietnamese dictionary

1. DataAcess
	Data access layer of the Dictionary app using Entity Framework Core.
	It is designed based on tratu page, which is a abandoned website, in order to take its data.

	TODO list:
	- How about uniqueness of word content?

2. Collect Data
	A .NET core console app that collects all dictionary data from tratu.soha.vn to fill the database.
	Some features:
	- It is using multiple threading to speed up the process
	- Able to cancel processing and start again at stopped point
	
	TODO list:
	- Able to save current state when the app is stopped by the system such as shutdown
	- A better way to implement concurrent process?
	- Justify parsing to reduce error words
	- Adapt to read other dictionaries like Japanese-Vietnamese

3. Dictionary
	A dictionary app includes a service using Asp.net Core API and a client using Angular.

   TODO list:
   - Searching some words are too slow such as Action
   - Searching for a work has not been completed, issue another search -> a bug
   - use a pipe for spelling html?
