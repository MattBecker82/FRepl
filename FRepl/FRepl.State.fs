[<AutoOpen>]
module FRepl.State

/// <summary>Represents a value whose evaluation may depend on, and/or update some state value</summary>
/// <remarks>Combine this with the StateBuilder Compuation Expression builder</remarks>
type State<'T,'TState> = State of ('TState -> 'T * 'TState)

/// <summary>Convert a value into stateful value</summary>
/// <remarks>This corresponds to a "lift" or "return" function for the State type</summary>
let addState x = State (fun s -> x,s)

/// <summary>Set the state to the given value</summary>
let setState newState = State (fun s -> (),newState)

/// <summary>Obtain the state as the value within a State<> type</summary>
let getState = State (fun s -> s,s)

/// <summary>Apply a stateful computation to given initial state</summary>
let runState (State f) initState = f initState

/// <summary>Computation Expression Builder for type State&lt;'TState&gt;</summary>
type StateBuilder() =
    member inline this.Return(x) = addState x
    member inline this.Bind(x,cont) =
        State (fun st ->
            let (y,st') = runState x st
            runState (cont y) st')
    member inline this.Delay(f) = f ()
    member inline this.ReturnFrom(x) = x

/// <summary>Concrete instance of StateBuilder Computation Expression Builder</summary>
let state = new StateBuilder()

/// <summary>Combine two different states into one</summary>
let extendState (st1 : State<'T , 'TState1>) : State<'T , 'TState1*'TState2> =      
    let (State f) = st1
    State (fun (s1,s2) ->
        let x,s1' = f s1
        x,(s1',s2))
