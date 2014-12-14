[<AutoOpen>]
module FRepl.Main

open FRepl.State
open FRepl.Types

/// <summary>Create a REPL from an evaluation function and prompt</summary>
/// <param name="evalFunc">The evaluation function to perform on each iteration of the REPL</param>
/// <param name="prompt">The function to retrive the REPL prompt</param>
/// <returns>A REPL combining the evaluation function with the prompt</param>
let repl (evalFunc: EvalFunc<'TState>) (prompt : Prompt<'TState>) = {
        evalFunc = evalFunc;
        prompt = prompt
     }

/// <summary>Create a stateless REPL from a stateless evaluation function and stateless prompt</summary>
/// <param name="evalFunc">The evaluation function to perform on each iteration of the REPL</param>
/// <param name="prompt">The (constant) prompt</param>
/// <returns>A REPL (with state type unit) combining the evaluation function with the prompt</param>
let statelessRepl (evalFunc: string -> string*bool) (prompt: string) : Repl<unit> =
    let evalFunc' input = evalFunc input |> addState
    repl evalFunc' (addState prompt)

/// <state>Run a repl using the given IO functions and initial state</state>
/// <param name="getInput">The function to get the input</param>
/// <param name="showOutput">The function to show the output</param>
/// <param name="initIoState">The initial IO state passed to getInput / showOutput</param>
/// <param name="repl">The REPL to run</param>
/// <param name="initReplState">The initial state of the REPL</param>
/// <returns>A pair of the the final REPL state and the final IO state</param>
let runRepl
    (getInput : GetInput<'TReplState,'TIoState>)
    (showOutput : ShowOutput<'TReplState,'TIoState>)
    (initIoState : 'TIoState)
    (repl : Repl<'TReplState>)
    (initReplState : 'TReplState) =
    let rec r () = state {
        let! promptStr = repl.prompt |> extendState
        let! input = getInput promptStr
        let! output,exit = repl.evalFunc input |> extendState
        do! showOutput output
        if exit then
            return ()
        else
            return! r ()
    }
    runState (r ()) (initReplState,initIoState) |> snd

/// <summary>Run a REPL using the console for IO</summary>
/// <param name="repl">The REPL to run</param>
/// <param name="initReplState">The initial state of the REPL</param>
/// <returns>The final state of the REPL</returns>
let stdRunRepl
    (repl : Repl<'TState>)
    (initReplState : 'TState) =
    runRepl stdGetInput stdShowOutput () repl initReplState |> fst
