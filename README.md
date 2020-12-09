This is a responsive single-page-app dictionary using Angular and Bootstrap. Currently it supports English-Vietnamese dictionary and going to support Japanese-Vietnamese dictionary

1. DataAcess
  Data access layer of the Dictionary app using Entity Framework Core.
  It is designed based on tratu page, which is a abandoned website, in order to take its data.

    To-do list:
    - [ ] How about uniqueness of word content?

2. Collect Data
  A .NET core console app that collects all dictionary data from tratu.soha.vn to fill the database.
  Some features:
   - It is using multiple threading to speed up the process
   - Able to cancel processing and start again at stopped point

    To-do list:
    - [ ] Able to save current state when the app is stopped by the system such as shutdown
    - [ ] A better way to implement concurrent process?
    - [ ] Justify parsing to reduce error words
    - [ ] Adapt to read other dictionaries like Japanese-Vietnamese
    - [ ] Dulicate words exist in the db, so in some cases it's imposible to get the best definition such as when looking up for the word Go

3. Dictionary
  A dictionary app includes a service using Asp.net Core API and a client using Angular.

    To-do list:
    - [x] Searching some words are too slow such as Action
    - [x] Searching for a work has not been completed, issue another search -> a bug
    - [ ] Use a pipe for spelling html?
    - [ ] Issue when suggestion response comes after searching response of the same word, the suggestion panel shouldn't show up
    - [ ] Suggestion content should be from the primary sub directory (see the word Action)
    - [ ] Use asynchoronous programming
    - [ ] Set focus on search textbox when the page is first shown
    - [ ] Recent words feature
    - [ ] Spelling shows escape characters in the suggestion panel. The word Palindrome is an example
   
