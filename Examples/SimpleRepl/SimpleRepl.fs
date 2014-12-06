open FRepl
open System

[<EntryPoint>]
let main argv = 
    Console.WriteLine("Type 'exit' or 'quit' to exit")

    // define a simple evaluation function
    let simpleEvalFunc state input =
        let output = sprintf "You typed: %s" input
        let exit = input.Equals("quit", StringComparison.OrdinalIgnoreCase)
                || input.Equals("exit", StringComparison.OrdinalIgnoreCase)
        (state,output,exit)

    // define a simple prompt function
    let helloPrompt state = "Hello> "

    // Run REPL using standard input/output
    do stdRepl simpleEvalFunc helloPrompt ()

    Console.WriteLine("Bye!!")
    
    0 // return exit code
