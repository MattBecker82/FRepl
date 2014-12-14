open FRepl
open System

[<EntryPoint>]
let main argv = 
    Console.WriteLine("Type 'exit' or 'quit' to exit")

    // Evaluation function: adds input text to state (list), output last three inputs
    let countingEvalFunc input =
        state {
            let! curState = getState // Get the current state
            let newState = input :: curState // Prepend input text to state
            do! setState newState // Update the state
            let output = newState |> Seq.truncate 3 |> String.concat "\n" // Ouput last 3 inputs
            let exit = input.Equals("quit", StringComparison.OrdinalIgnoreCase)
                    || input.Equals("exit", StringComparison.OrdinalIgnoreCase)
            return (output,exit)
        }

    // Prompt function: display count of inputs
    let countingPrompt : Prompt<string list> = 
        state {
            let! inputs = getState
            return (sprintf "%i> " inputs.Length)
        }

    // Create counting REPL
    let countingRepl = repl countingEvalFunc countingPrompt

    // Run REPL using standard input/output
    let finalState = stdRunRepl countingRepl []

    Console.WriteLine("Total lines input: {0}", finalState.Length)
    
    0 // return exit code
