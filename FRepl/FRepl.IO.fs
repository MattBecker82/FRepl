[<AutoOpen>]
module FRepl.IO

/// <summary>Write a string to the console (no newline)</summary>
let consoleWrite (input : string) : unit =
    System.Console.Write(input)

/// <summary>Write a line to the console (with newline)</summary>
let consoleWriteLine (input : string) : unit =
    System.Console.WriteLine(input)

/// <summary>Read a line from the console (terminated by newline)</summary>
let consoleReadLine () : string =
    System.Console.ReadLine()




