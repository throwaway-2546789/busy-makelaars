# Busy Makelaars

## Dependencies

To compile and run the project, you will need to have dotnet 6.

Running the project also requires a funda API key. Set this as an environment variable to run the solution.

```
export FUNDA_PARTNER_KEY="[Your key here]" && dotnet run --project bm.api/bm.api.csproj
```

## Structure

```
❯ dotnet sln list
Project(s)
----------
bm.core/bm.core.csproj
bm.test/bm.test.csproj
bm.services/bm.services.csproj
bm.api/bm.api.csproj
```

### bm.core

The main logic, services and interfaces are defined in this project. The main service for the purposes of this assignment is located within `makelaars.cs` and other interfaces required are also defined in there. This service simply requests statistics from it's statistics provider.

The statistics provider used in the makelaars service is located within `data/provider.cs`. It is implemented such that it will first try to retrieve appropriate statistics from a memory cache. If none are available, then it will use the stats calculator service to retrieve the required statistics, and also populate the cache with those statistics.

The calculator fetches listings from the funda partner API, groups them by makelaar and then returns a count of listings per makelaar.

### bm.services

External dependencies are placed outside of the core logic.

The funda partner API code is located here, and it retrieves listings for a given filter (ie: /amsterdam/tuin). As the API returns 25 listings at a time, with paging information, this class recursively trawls through the pages to fetch all listings.

### bm.test

Some tests are located in here.

### bm.api

The API offers two route `/` and `/tuin`, which fetch the statistics related to all listings, and garden listings respectively.

Example output of `curl http://localhost:5215/ | jq`:

```
[
  {
    "makelaarID": 24592,
    "name": "Ramón Mossel Makelaardij o.g. B.V.",
    "propertyCount": 56
  },
  {
    "makelaarID": 24605,
    "name": "Hallie & Van Klooster Makelaardij",
    "propertyCount": 52
  },
  {
    "makelaarID": 24079,
    "name": "Makelaardij Van der Linden Amsterdam",
    "propertyCount": 52
  },
  {
    "makelaarID": 24705,
    "name": "Eefje Voogd Makelaardij",
    "propertyCount": 41
  },
  {
    "makelaarID": 24848,
    "name": "KIJCK. makelaars",
    "propertyCount": 41
  },
  {
    "makelaarID": 24067,
    "name": "Broersma Makelaardij",
    "propertyCount": 38
  },
  {
    "makelaarID": 24783,
    "name": "Hoekstra en van Eck Amsterdam West",
    "propertyCount": 36
  },
  {
    "makelaarID": 24628,
    "name": "Smit & Heinen Makelaars en Taxateurs o/z",
    "propertyCount": 35
  },
  {
    "makelaarID": 24065,
    "name": "Carla van den Brink B.V.",
    "propertyCount": 31
  },
  {
    "makelaarID": 24594,
    "name": "Hoekstra en van Eck Amsterdam Noord",
    "propertyCount": 30
  }
]
```
