open FRepl
open System

[<EntryPoint>]
let main argv = 
    Console.WriteLine("Type 'exit' or 'quit' to exit")

    // Evaluation function: adds input text to state (list), output last three inputs
    let countingEvalFunc state input =
        let newState = input :: state // Prepend input text to state
        let output = newState |> Seq.truncate 3 |> String.concat "\n" // Ouput the last three inputs
        let exit = input.Equals("quit", StringComparison.OrdinalIgnoreCase)
                || input.Equals("exit", StringComparison.OrdinalIgnoreCase)
        (newState,output,exit)

    // Prompt function: display count of inputs
    let countingPrompt (state : string list) = sprintf "%i> " state.Length

    // Run REPL using standard input/output
    let finalState = stdRepl countingEvalFunc countingPrompt []

    Console.WriteLine("Total lines input: {0}", finalState.Length)
    
    0 // return exit code
