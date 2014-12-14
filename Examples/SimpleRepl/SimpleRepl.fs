open FRepl
open System

[<EntryPoint>]
let main argv = 
    Console.WriteLine("Type 'exit' or 'quit' to exit")

    // Define a simple evaluation function
    let simpleEvalFunc input =
        let output = sprintf "You typed: %s" input
        let exit = input.Equals("quit", StringComparison.OrdinalIgnoreCase)
                || input.Equals("exit", StringComparison.OrdinalIgnoreCase)
        (output,exit)

    // Define a simple prompt
    let helloPrompt = "Hello> "

    // Combine the evaluation function and the prompt into a stateless REPL
    let simpleRepl = statelessRepl simpleEvalFunc helloPrompt

    // Run REPL using console for input/output
    do stdRunRepl simpleRepl ()

    Console.WriteLine("Bye!!")
    
    0 // return exit code
