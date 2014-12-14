[<AutoOpen>]
module FRepl.Common

open FRepl.Types

/// <summary>Display prompt to console and get input from console</summary>
let stdGetInput : GetInput<'TState, unit> =
    (fun promptStr ->
        do System.Console.Write(sprintf "\n%s" promptStr)
        consoleReadLine () |> addState)

/// <summary>Display output to console</summary>
let stdShowOutput : ShowOutput<'TState, unit> =
    (fun output ->
        consoleWriteLine output |> addState)
