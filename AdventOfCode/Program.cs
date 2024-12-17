using AdventOfCode;

Console.WriteLine("Hello, Advent of Code enthusiast!");
var formatted = SampleFormatter.Format(DateOnly.FromDateTime(DateTime.Now).Year);
Console.WriteLine($"The event of this year is called {formatted}!");
