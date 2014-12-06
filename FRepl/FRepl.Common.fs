[<AutoOpen>]
module FRepl.Common

open FRepl.Types

/// <summary>Display prompt to standard output and get input from standard input</summary>
let stdGetInput : GetInput<'TState> =
    (fun state promptStr -> 
        do System.Console.Write(sprintf "\n%s" promptStr)
        System.Console.ReadLine())

/// <summary>Display output to standard output</summary>
let stdShowOutput : ShowOutput<'TState> =
    (fun state output ->
        System.Console.WriteLine(output))
