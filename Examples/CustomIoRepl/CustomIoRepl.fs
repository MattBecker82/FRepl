open FRepl
open System

// IO state consists of a pair of lists: inputs remaining and ouputs received
type IoState = (string list)*(string list)

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

    // Get Input by retrieving next item from input list    
    let getInput prompt =
        state {
            let! replState,ioState = getState
            let inps,outps = ioState
            let input,inps' =
                match inps with
                | x::xs -> x,xs
                | []    -> raise (new InvalidOperationException("Inputs exhausted"))
            let ioState' = inps',outps
            do! setState (replState,ioState')
            return inps.Head
        }        
    
    // "Show" Output by adding it to the list of outputs
    let showOutput output =
        state {
            let! replState,ioState = getState
            let inps,outps = ioState
            let outps' = outps @ [output]
            let ioState' = inps,outps'
            do! setState (replState,ioState')
        }

    // Define a list of inputs
    let inputs = ["Hello"; "friends"; "exit"; "now"]
    let initIoState = (inputs, [])
    
    // Run REPL using custom input/output methods
    let finalState = runRepl getInput showOutput initIoState simpleRepl ()

    // Show information about the final state
    let finalInps,finalOutps = snd finalState
    Console.WriteLine("Unused inputs: {0}", finalInps);
    Console.WriteLine("Ouputs received: {0}", finalOutps);

    Console.WriteLine("Bye!!")
    
    0 // return exit code
