[<AutoOpen>]
module FRepl.Common

open FRepl.Types

/// <summary>Display prompt to console and get input from console</summary>
let stdGetInput : GetInput<'TState> =
    (fun state promptStr -> 
        do System.Console.Write(sprintf "\n%s" promptStr)
        System.Console.ReadLine())

/// <summary>Display output to console</summary>
let stdShowOutput : ShowOutput<'TState> =
    (fun state output ->
        System.Console.WriteLine(output))
