##OSRSPlayerCountTracker

A small tool that keeps track of the player count of Old School RuneScape.

###Settings

Update interval can be changes in **Settings.cs** file. This determines how often the tool checks the player count.

### TODO:
- Handle collected data, perhaps visualize and present it on a webpage with graphs or generate graph images
- Add more settings, e.g. only keep track of the highest or lowest player count(s)

###Example output

<pre>
  {
    "Date": "13-26-2017",
    "Time": "02:26:52",
    "PlayerCount": 51266
  },
  {
    "Date": "13-26-2017",
    "Time": "02:26:57",
    "PlayerCount": 51299
  },
  {
    "Date": "13-27-2017",
    "Time": "02:27:02",
    "PlayerCount": 51312
  },
  {
    "Date": "13-27-2017",
    "Time": "02:27:07",
    "PlayerCount": 51269
  },
  {
    "Date": "13-27-2017",
    "Time": "02:27:12",
    "PlayerCount": 51224
  }
  </pre>
  
  ##Used libraries
  - AngleSharp
  - Newtonsoft.Json
