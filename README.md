# Advent of Code
[Advent of Code](https://adventofcode.com) is an advent calendar that has
a two-part coding challenge for reach day from December 1st through 25th.  
This repo ~~contains~~ will contain my solutions to it, using C#.

There are no solutions yet; I found some templates for Advent of Code projects,
but wanted to make my own, so that comes first.  
I did solve the challenges of days 1-4, but they're not in this repo yet.

## Usage (WIP)
**The project is such a WIP that it is currently not runnable.**

### Automatic input downloading
You'll need a session cookie, see explanation & how to find yours
[here](https://github.com/GreenLightning/advent-of-code-downloader/blob/403fb308ab0c8c6b5104d2d7b8368a44005edb4d/readme.markdown#what-is-a-session-cookie).  
Set an environment variable called `AOC_SESSION` to it, for example `AOC_SESSION=abcd1234`,
before running the program.

## AoC guidelines *are attempted to be* followed
This repo currently **may not fully** follow [AoC's automation guidelines](https://www.reddit.com/r/adventofcode/wiki/faqs/automation/),
as the structure is still a work in progress,
and as a developer, I'm new to dealing with network requests.  
My intention is to structure the repo in such a way where it does,
and send very few real requests while developing the structure.

All network functionality is contained within [PuzzleInputProvider](AdventOfCode/PuzzleInputProvider.cs).  
(And related tests in [PuzzleInputProviderTests](AdventOfCode.Tests/PuzzleInputProviderTests.cs),
though those all use a fake.)

### Guidelines followed
- No inputs in repo, except for small example inputs within [AdventOfCode.Tests](AdventOfCode.Tests).
- *Not technically in guidelines...* but, all HTTP-related tests use a fake that
  doesn't actually connect online (see [Flurl's documentation](https://flurl.dev/docs/testable-http/)),
  preventing unnecessary traffic
- `User-Agent` header with contact info
- Caching inputs after initial download

### Guidelines not yet necessarily followed
- Throttling outbound requests
  - Plan: One input at a time, minimum 3 minutes between requests
      - Advise user to download inputs manually if they try downloading input for more than 1

## Credits
- [Eric Wastl](https://was.tl/) for making [Advent of Code](https://adventofcode.com) in the first place
- [viceroypenguin](https://github.com/viceroypenguin) for providing [a template](https://github.com/viceroypenguin/adventofcode.template)
  I've drawn heavy inspiration from for the runner part of the project
