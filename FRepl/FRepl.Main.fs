[<AutoOpen>]
module FRepl.Main

open FRepl.Types

/// <summary>Run a REPL</summary>
/// <param name="evalFunc">The evaluation function to perform on each iteration of the REPL</param>
/// <param name="prompt">The function to retrive the repl prompt</param>
/// <param name="getInput">The function to get the input</param>
/// <param name="showOutput">The function to show the output</param>
/// <param name="initState">The initial state of the repl</param>
/// <returns>The final state of the REPL</param>
let rec repl
    (evalFunc : EvalFunc<'TState>)
    (prompt : Prompt<'TState>)
    (getInput : GetInput<'TState>)
    (showOutput : ShowOutput<'TState>)
    (initState : 'TState) =
    let promptStr = prompt initState
    let input = getInput initState promptStr
    let (newState,output,exit) = evalFunc initState input
    do showOutput newState output
    if exit then
        newState
    else
        repl evalFunc prompt getInput showOutput newState

/// <summary>Run a REPL using standard input/output</summary>
/// <param name="evalFunc">The evaluation function to perform on each iteration of the repl</param>
/// <param name="prompt">The function to retrive the repl prompt</param>
/// <param name="initState">The initial state of the repl</param>
/// <returns>The final state of the REPL</param>
let stdRepl
    (evalFunc : EvalFunc<'TState>) 
    (prompt : Prompt<'TState>)
    (initState : 'TState) =
    repl evalFunc prompt stdGetInput stdShowOutput initState


