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

### Building on other platforms
There's nothing particularly platform-specific about the FRepl library, other than its use of .NET. Therefore it's probably possible for someone who knows what they're doing to build and run it on other platforms which support .NET (e.g. Linux/Mono). Best of luck - let me know how you get on!

## Getting started

FRepl requires .NET 4.5, so make sure it is installed. Reference the FRepl.dll assembly from your project. Use `open FRepl` within an F# file to make the FRepl symbols easily accessible from your code.

### REPLs

A REPL (Read-Evaluate-Print-Loop) is essentially a program (or sub-program) which does repeatedly does the following:

1. Prompt the user for input
2. Read a line of input text from the user
3. Evaluate the input text
4. Display some output text

The above steps repeat until some exit criterion is met (e.g. the user typed 'exit'), at which point the REPL comes to an end.

The REPL may also be *stateful*. That is, it has a *state* which is initially set to some value, and may be updated as the REPL proceeds. In FRepl the state of the REPL may be represented by any type (the REPL's *state type*). To build a stateless REPL, the `unit` type should be used as the state type, either implicitly or explicitly.

### A Simple, Stateless REPL

Let's define simple, statless REPL. That is, a REPL which does not maintain any state. To define a REPL using FRepl we must define a function (the *evaluation function*) which transforms input into output. The evaluation function must also indicate whether the REPL is to exit or not.

The following evaluation function is defined in SimpleRepl.fs:

```F#
    // Define a simple evaluation function
    let simpleEvalFunc input =
        let output = sprintf "You typed: %s" input
        let exit = input.Equals("quit", StringComparison.OrdinalIgnoreCase)
                || input.Equals("exit", StringComparison.OrdinalIgnoreCase)
        (output,exit)
```

The return type of `simpleEvalFunc` is `string*bool`, i.e. a pair of a `string` and a `bool`. The first element of the pair is the output to display, and the second element indicates if the REPL should exit. It is set to `true` if the REPL should exit and `false` if it should continue.

The function `simpleEvalFunc` is defined so that the output consists of the input preceded by the string `"You typed: "`. The return value indicates that the REPL should exit if the input string was `"quit"` or `"exit"`.

As well as defining an evaluation function, we must also define a *prompt* to be displayed to the user in order to get their input. For example:

```F#
    // Define a simple prompt
    let helloPrompt = "Hello> "
```

Having defined an evaluation function and a prompt, we can now combine them to form a REPL. Since we want the REPL to be stateless, we use the `statelessRepl` function:

```F#
    // Combine the evaluation function and the prompt into a stateless REPL
    let simpleRepl = statelessRepl simpleEvalFunc helloPrompt
```

The final step is to run the REPL using a runner function. FRepl defines a number of runner functions, the simplest of which is `stdRunRepl` which uses the standard console for input and output.

```F#
    // Run REPL using console for input/output
    do stdRunRepl simpleRepl ()
```

See the example project **SimpleRepl** for a complete example. The following shows an example session of the **SimpleRepl** application:

```
    Type 'exit' or 'quit' to exit

    Hello> Hello
    You typed: Hello

    Hello> Bonjour
    You typed: Bonjour

    Hello> Bore da
    You typed: Bore da

    Hello> quit
    You typed: quit
    Bye!!
```

### Stateful computations

The previous section showed an example of a stateless REPL. In general, a REPL may maintain state. That is, the evaluation function may depend on - and potentially update - the current state of the REPL. Likewise the prompt may also depend on and/or update the state of the REPL.

To capture the concept of state in a purely functional manner (i.e. without introducing side-effects), FRepl defines a generic `State<>` type:

```F#
	type State<'T,'TState> = State of ('TState -> 'T * 'TState)
```

The type arguments `'T` and `'TState` represent the underlying value type and the state type respectively. A value of type `State<'T,'TState>` is essentially a wrapper for a function which takes a `'TState` and returns a pair of a `'T` and a `'TState`.

Conceptually, a value of `State<'T,'TState>` represents a *stateful computation* which can be invoked by passing in the current state (of type `'TState`). The computation evaluates a value of type `'T` (possibly dependent on the state), and potentially modifies the state.

FRepl defines a number of constructs to assist with manipulating values of type `State<>`:

The `addState` function "lifts" a (stateless) value into a stateful computation which returns the given value (leaving the state unchanged):

```F#
    let addState x = State (fun s -> x,s)
```

The `setState` function represents a stateful computation which changes the state to a given state (and returns the value `unit`):

```F#
   let setState newState = State (fun s -> (),newState)
```

The value `getState` represents a stateful computation which retrieves the current state and returns it as the evaluated value (without modifying the state). This is useful within computation expressions (see below).

```F#
    let getState = State (fun s -> s,s)
```

The `runState` function takes a `State<'T,'TState>` value and an initial state of type `'TState` and runs the stateful computation represented by the `State<>` value using the initial state as the input. It returns a pair of the evaluated value and the updated state:

```F#
    let runState (State f) initState = f initState
```

The computation builder object `state` can be used to construct `State<>` values using F#'s [computation expression](http://msdn.microsoft.com/en-us/library/dd233182.aspx) syntax, threading the state "invisibly" through a sequence of `let!` bindings, `do!` and `return` statements:

```F#
    let myComp = state {
        let! three = addState 3
        let! curState = getState
        let! list = List.replicate three curState |> addState
        do! setState "done"
        return list
    }
    let result,finalState = runState myComp "Education"
```

Evaluating the above code snippet results in:

```F#
    val result : string list = ["Education"; "Education"; "Education"]
    val finalState : string = "done"
```

### Evaluation Functions

At the heart of a REPL is an *evaluation function*. This is simply the function which transforms the input text into the output text. It may also depend on, and update the state of the REPL. The evaluation function also indicates when the REPL should exit.

In FRepl, an evaluation function has the type `EvalFunc<'TState>` where `'TState` is the REPL's state type:

```F#
    type EvalFunc<'TState> = string -> State<string * bool, 'TState>
```

An `EvalFunc<'TState>` is a function which takes the input text as input and computes a pair of a `string` and a `bool` representing the output text and the exit flag respectively.

Since the computation of the output may depend on, and/or update, the REPL's state, the output type is `State<string*bool,'TState>`, representing not a raw `string * bool` value, but a *stateful computation* of a `string * bool` value.

A trivial example of an `EvalFunc<'TState>` is:

```F#
    let trivialEvalFunc input = ("Bonjour, monde!", true) |> addState
```
    
This evaluation function simply ignores the input text, outputs the text `"Bonjour, monde!"`, and indicates that the REPL should exit. The raw result `("Bonjour, monde!", true)` is transformed into a `State<string*bool,'TState>` by passing the result to the `addState` function. This has the effect of making `trivialEvalFunc` a stateful computation which always returns the same result, and leaves the state unchanged.

A slightly more interesting evaluation function is given in CountingRepl.fs:

```F#
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
```

In this example, the REPL state is represented by a `string list` consisting of all the inputs the user has entered so far, with the most recent input first. The function `countingEvalFunc` uses F# [computation expression](http://msdn.microsoft.com/en-us/library/dd233182.aspx) syntax to build a `State<string*bool,string list>` from the given input string.

The returned stateful computation proceeds as follows:
1. Retrieve the current state (list of inputs) using `getState` and binding its value to `curState`.
2. Evaluate the new state `newState` by adding `input` to the head of the list.
3. Update the state to `newState` by using the `setState` function. The `do!` syntax but keeps track of the updated state, but ignores the output value of `setState`  (which is always the `unit` value `()` anyway).
4. Compute the output string by concatenating the most recent three inputs.
5. Work out whether to exit the REPL: exit if the input text was either `"quit"` or `"exit"`.
6. "Return" the pair consisting of the output and the exit flag.

### Prompts

A REPL usually displays a short piece of text (the *prompt*) to the user before retrieving the input text. Since the prompt may depend on the REPL's state, FRepl represents a prompt as a *stateful computation* of the prompt string:

```F#
    type Prompt<'TState> = State<string, 'TState>
```

To define a prompt which is always the same, regardless of the state, we simply need to pass the prompt text to the `addState` function to turn it into a `Prompt<'TState>`:

```F#
    let yesPrompt = addState "Yes?> "
```

The example file CountingRepl.fs defines a prompt which depends on the REPL state, where the state is a `string list` consisting of all the inputs entered by the user so far:

```F#
    let countingPrompt : Prompt<string list> = 
        state {
            let! inputs = getState
            return (sprintf "%i> " inputs.Length)
        }
```

The prompt represented by `countingPrompt` displays the number of inputs the user has entered.

### Defining and running a REPL

Once the `EvalFunc<'TState>` and `Prompt<'TState>` values have been defined, they may be combined into a value of type `Repl<'TState>`:

```F#
    type Repl<'TState> = {
            evalFunc : EvalFunc<'TState>;
            prompt   : Prompt<'TState>
        }
```

A `Repl<'TState>` represents a REPL as the combination of an evaluation function and a prompt, both with the state type `'TState`. The `repl` function may be used to combine an `EvalFunc<'TState>` and a `Prompt<'TState>` into a `Repl<'TState>`, as in CountingRepl.fs:

```F#
    let countingRepl = repl countingEvalFunc countingPrompt
```

Finally, to run the REPL, it must be passed to a *runner function*. The runner function is responsible for initializing the REPL, managing user input/output and the flow of execution.

The simplest runner function in FRepl is `stdRunRepl`, which uses the System Console to display output to and read input from the user:

```F#
    val stdRunRepl : Repl<'TState> -> 'TState -> 'TState
```

The `stdRunRepl` function takes as input the REPL to be run, and the initial state of the REPL. It returns the final state of the REPL upon exit.

For example, the following will run the REPL `countingRepl` defined above. The initial state is `[]`, representing the fact that initially, no user input has been entered:

```F#
    let finalState = stdRunRepl countingRepl []
```

The result is bound to `finalState`, which will contain a list of all the user inputs, with the most recent first.

The example project **CountingRepl** shows a complete example of a REPL with state. The following shows an example session of the **CountingRepl** application:

```
    Type 'exit' or 'quit' to exit

    0> hello
    hello

    1> bonjour
    bonjour
    hello

    2> a bear
    a bear
    bonjour
    hello

    3> pursued by
    pursued by
    a bear
    bonjour

    4> exit
    exit
    pursued by
    a bear
    Total lines input: 5
```

# Customizing Input and Output

The `stdRunRepl` function uses the System Console to retrieve input from the user and display output to the user. This is a common method of interfacing with the user but in certain cases, you might want to use other methods of retrieving input and handling output. For instance, you might want to use a GUI rather than the system console. Or you might want to run the REPL in a non-interactive fashion by using a predefined list of inputs, and simply gathering the outputs rather than displaying them. This could be useful for enabling automated testing of a REPL, for instance.

To control how input/output is performed when running a REPL, you can use the more general `runRepl` function rather than `stdRunRepl`. The `runRepl` takes additional inputs of type `GetInput<>` and `ShowOutput<>`:

```F#
    val runRepl : GetInput<'TReplState,'TIoState> -> ShowOutput<'TReplState,'TIoState> -> Repl<'TState> -> 'TState -> 'TState
```

The full type of `GetInput<>` is:

```F#
    type GetInput<'TReplState, 'TIoState> = string -> State<string, 'TReplState * 'TIoState>
```
It represents a function which takes an input (the prompt string) and returns a stateful computation of the input. The state is composed of two elements: the state type `'TReplState` of the REPL, and a type `'TIoState` representing the state of the input/output mechanism. This means that retrieving the input could depend upon, and/or update both the REPL state and the state of the input/output mechanism. Both type parameters are defined by the user.

The full type of `ShowOutput<>` is:

```F#
    type ShowOutput<'TReplState, 'TIoState> = string -> State<(), 'TReplState * 'TIoState>
```
It represents a function which takes the output string and and returns a stateful computation which displays the output. The state is composed of the same two elements as in `GetInput<>`. This means that displaying the output could depend upon, and/or update both the REPL state and the state of the input/output mechanism.

The example project **CustomIoRepl** uses the same simple REPL as in the **SimpleRepl** example, but shows an example of running the REPL using customised functions for retrieving input and displaying output. In the example, the state of the input/output mechanism is represented as a pair of lists:

```F#
    type IoState = (string list)*(string list)
```

The first element of the pair represents a list of inputs not yet retrieved by the REPL. The second element of the pair represents the list of outputs so far "displayed" by the REPL.

The custom `getInput` function retrieves the input by reading it from the head of the list. It updates the input/output state by setting the input list to consist of the tail of the original list:

```F#
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
```

Note that in the case that there are no more inputs in the input list, an exception is raised.

The custom `showOuput` function does not actually display anything to the user, but simply updates the input/output state by appending the output text to the output list:

```F#
    let showOutput output =
        state {
            let! replState,ioState = getState
            let inps,outps = ioState
            let outps' = outps @ [output]
            let ioState' = inps,outps'
            do! setState (replState,ioState')
        }
```
The REPL is then run using the custom input and output functions by calling the `runRepl` function:

```F#
    // Define a list of inputs
    let inputs = ["Hello"; "friends"; "exit"; "now"]
    let initIoState = (inputs, [])
    
    // Run REPL using custom input/output methods
    let finalState = runRepl getInput showOutput initIoState simpleRepl ()
```

The initial state is set so that the inputs list contains the list of inputs to retrieve, and the output list is empty. The `runRepl` function returns the final state as a pair, where the first element of the pair is the final state of the REPL, and the second element is the final state of the input/output mechansism. In this case, the final I/O state will be a pair consisting of two lists: the unused inputs, and the outputs "displayed" by the REPL.
