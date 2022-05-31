# Minesweeper: Console Edition

## Introduction
Welcome! This is a super basic implementation of minesweeper for a console using .NET 6. I'm also using a fantastic library called [SpectreConsole](https://github.com/spectreconsole/spectre.console). Highly recommend it for building rich CLIs.

This is still a work in progress. I plan on improving the codebase incrementally, but there's a couple things outstanding:

1. Unit test suite
2. General code cleanup
3. Better commands for starting the application

The unit test suite will help me out a lot. I kinda slopped this together over a day, so it's a little bit brittle. I'll need to improve the code before I'll be really proud of it, and unit tests will help significantly with that development effort.

I'd like to also create some additional customization options for the look and feel and maybe do a bit of cleanup around some of the rendering of the minefield.

## How to play

For now, I don't have any ready to play versions, so you'll have to build the code yourself.

1. Download the code
2. Install the [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) 
3. Navigate to your download folder
4. Run `dotnet build`
5. Ensure that the code builds without error
6. Use the command `dotnet run -- play classic` to start the application