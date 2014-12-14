[<AutoOpen>]
module FRepl.Types

/// <summary>Type EvalFunc&lt;'TState&gt; represents the evaluation function of the REPL loop.</summary>
/// <remarks>Represented as a function which takes an input string as input and (statefully) returns the
/// string to display and a boolean representing whether to terminate the REPL (true = terminate).</remarks>
type EvalFunc<'TState> = string -> State<string * bool, 'TState>

/// <summary>Represents a (stateful) REPL prompt string</summary>
type Prompt<'TState> = State<string, 'TState>

/// <summary>Represents a REPL - a combination of an evaluation function and a prompt</summary>
type Repl<'TState> = {
        evalFunc : EvalFunc<'TState>;
        prompt   : Prompt<'TState>
     }

/// <summary>Represents a function to retrieve (statefully) the next input string given prompt string</summary>
/// <remarks>May update the IO state</remarks>
type GetInput<'TReplState, 'TIoState> = string -> State<string, 'TReplState * 'TIoState>

/// <summary>Represents a function to display output string (statefully)</summary>
/// <remarks>May perform IO</remarks>
type ShowOutput<'TReplState, 'TIoState> = string -> State<unit, 'TReplState * 'TIoState>
