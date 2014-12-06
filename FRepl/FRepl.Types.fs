[<AutoOpen>]
module FRepl.Types

/// <summary>Type EvalFunc&lt;'TState&gt; represents the evaluation function of the REPL loop.</summary>
/// <remarks>Represented as a (curried) function which takes current state &amp; input string as input
///  and returns the updated state, the string to display and a boolean representing whether to terminate
///  the REPL (true = terminate).</remarks>
type EvalFunc<'TState> = 'TState -> string -> 'TState * string * bool

/// <summary>Represents a function to retrieve the REPL prompt string given the current state</summary>
type Prompt<'TState> = 'TState -> string

/// <summary>Represents a function to retrieve the next input string given the current state and prompt string</summary>
type GetInput<'TState> = 'TState -> string -> string

/// <summary>Represents a function to display output string given the current state and the string to display</summary>
type ShowOutput<'TState> = 'TState -> string -> unit




