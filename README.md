# FRepl - A REPL Library for F Sharp

## Introduction

### Summary

FRepl is an F# (.NET) library providing REPL (Read-Evaluate-Print-Loop) functionality. It's written in F# and targets F#, but since it takes the form of a .NET library, it should be callable from any .NET-aware langauge.

FRepl allows you to incorporate a REPL into your application in a purely functional manner. You supply the evaluation function, and optionally the means to read input / display output. The library runs the REPL for you. 

**Please note.** FRepl is a work in progress, and nowhere near feature complete. You are welcome to play and send me constructive comments, but please bear this in mind. I take all responsibility for any positive consequences, and waive all responsibility for any negative consequences!

## Building FRepl

### Building in Visual Studio 2013
Open FRepl.sln in Visual Studio 2013, choose Build->Build Solution. This will build the FRepl library project and the Example projects. This has been tested in Visual Studio Express 2013.
The build outputs (assemblies) go in $(SolutionDir)bin\Debug (for Debug builds) and $(SolutionDir)bin\Release (for Release builds).

### Building in other versions of Visual Studio
I haven't tested it, but opening FRepl.sln and choosing Build->Build Solution will probably work with versions from Visual Studio 2010 onwards. You may need to install F# Build tools to get it to work.

### Building with Microsoft build tools outwith Visual Studio
Your best bet is to run MSBuild.exe directly on FRepl.sln. You will need F# Development tools (fsc.exe etc.) installed.

### Building on other platforms.
There's nothing particularly platform-specific about the FRepl library, other than its use of .NET. Therefore it's probably possible for someone who knows what they're doing to build and run it on other platforms which support .NET (e.g. Linux/Mono). Best of luck - let me know how you get on!

## Getting started

FRepl requires .NET 4.5, so make sure it is installed. Reference the FRepl.dll assembly from your project. Use `open FRepl` within an F# file to make the FRepl symbols easily accessible from your code.

### Concepts

A REPL (Read-Evaluate-Print-Loop) is essentially a program (or sub-program) which does repeatedly does the following:

1. Prompt the user for input
2. Read a line of input text from the user
3. Evaluate the input text
4. Display some output text

The above steps repeat until some exit criterion is met (e.g. the user typed 'exit'), at which point the REPL comes to an end.

The REPL may also be *stateful*. That is, it has a *state* which is initially set to some value, and may be updated as the REPL proceeds. In FRepl the state of the REPL may be represented by any type (the REPL's *state type*). To build an essentially stateless REPL, use the `unit` type for the state.

### Evaluation Functions

At the heart of a REPL is an *evaluation function*. This is simply the function which transforms the input text into the output text. It may also depend on, and update the state of the REPL. The evaluation function also indicates when the REPL should exit.

In FRepl, an evaluation function has the type `EvalFunc<'TState>` where `'TState` is the REPL's state type:

    type EvalFunc<'TState> = 'TState -> string -> 'TState * string * bool

An evaluation function is a (curried) function which takes as input:

1. the current REPL state, and
2. the input text.

It returns a 3-element tuple consisting of:

1. the (possibly) updated state,
2. the output text, and
3. a flag which indicates whether the REPL should exit. A value of true for this flag indicates that the REPL should exit.

The following is an example of a trivial evaluation function:

    let trivialEvalFunc state input = (state, "Bonjour, monde!", true)
    
This evaluation function simply ignores the input text, returns the state unaltered, and the output text `"Bonjour, monde!"`, and indicates that the REPL should exit.

A slightly more interesting evaluation function is given in SimpleRepl.fs:

    let simpleEvalFunc state input =
        let output = sprintf "You typed: %s" input
        let exit = input.Equals("quit", StringComparison.OrdinalIgnoreCase)
                || input.Equals("exit", StringComparison.OrdinalIgnoreCase)
        (state,output,exit)

This evaluation function plays back the input text to the user, returns the state unaltered, and indicates the REPL should exit if the input text was `"quit"` or `"exit"`.

### Prompts

A REPL usually displays a short piece of text (the *prompt*) to the user before retrieving the input text. Since the prompt may depend on the REPL's state, FRepl represents a prompt as a function which takes the state as input, and returns the prompt text:

    type Prompt<'TState> = 'TState -> string

A simple prompt might always return the same prompt text, regardless of the state:

    let helloPrompt state = "Hello> "

### Running a REPL

The simplest way to run a REPL in FRepl is to use the `stdRepl` function, which uses the System Console to display output to and read input from the user:

    val stdRepl : EvalFunc<'TState> -> Prompt<'TState> -> 'TState -> 'TState
    
The `stdRepl` function takes as input:

1. the evaluation function,
2. the prompt function, and 
3. the initial state.

It returns the final state of the REPL upon exit.

For example, the following will run a REPL using the evaluation function `simpleEvalFunc` and prompt `helloPrompt` defined above. The initial state is `()`, so the state is of type `unit` representing a stateless REPL:

    do stdRepl simpleEvalFunc helloPrompt ()

See the example project **SimpleFunc** for a complete example.

### A Stateful REPL

The example project **CoutingRepl** shows an example of a REPL with state. In this example, the state is the list of input string typed by the user. It is represented by a value of type `string list`, with the most recent input at the head of the list.

The evaluation function `countingEvalFunc` updates the state by adding the user input to the head of the list. It outputs the most recent three input lines (or up to most recent three), and exits if the input is `"quit"` or `"exit"`:

    let countingEvalFunc state input =
        let newState = input :: state // Prepend input text to state
        let output = newState |> Seq.truncate 3 |> String.concat "\n" // Ouput the last three inputs
        let exit = input.Equals("quit", StringComparison.OrdinalIgnoreCase)
                || input.Equals("exit", StringComparison.OrdinalIgnoreCase)
        (newState,output,exit)


The prompt function `countingPrompt` displays the number of inputs so far as part of the prompt:

    let countingPrompt (state : string list) = sprintf "%i> " state.Length
    
The REPL is run using the `stdRepl` function. The initial state is the empty list `[]`, meaning that initially no inputs have been received.

    let finalState = stdRepl countingEvalFunc countingPrompt []
    
Note that the `stdRepl` function returns the final state of the REPL when it exits. This is then stored as the value `finalState` so that it can be accessed by the rest of the program. 

The following shows an example session of the **CountingRepl** application:
    Type 'exit' or 'quit' to exit

    0> to be
    to be

    1> or
    or
    to be

    2> not
    not
    or
    to be

    3> to be
    to be
    not
    or

    4> exit
    exit
    to be
    not
    Total lines input: 5
