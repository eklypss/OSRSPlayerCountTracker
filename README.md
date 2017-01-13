##OSRSPlayerCountTracker

A small tool that keeps track of the player count of Old School RuneScape.

###Settings

Update interval can be changed in **Settings.cs** file. This determines how often the tool checks the player count.

### Planned updates / changes
- Handle collected data, perhaps visualize and present it on a webpage with graphs or generate graph images
- Add more settings, e.g. only keep track of the highest or lowest player count(s)

###Example output

<pre>
[
  {
    "Date": "13-01-2017",
    "Time": "02:38:26",
    "PlayerCount": 51494
  },
  {
    "Date": "13-01-2017",
    "Time": "02:38:31",
    "PlayerCount": 51525
  },
  {
    "Date": "13-01-2017",
    "Time": "02:38:36",
    "PlayerCount": 51472
  }
]
  </pre>
  
##Used libraries
- AngleSharp
- Json.NET
