module Components

// open System
// open Fable.Core.JS

printfn $"Loading {__SOURCE_FILE__}..."

open Fable.Core
open Elmish
open Elmish.Solid

module Table =
    type Row = string * int * bool

    type State = {
        Rows: Row array
    }

    let rando () =
        let x = JS.Math.random()
        System.Guid.NewGuid().ToString(), int(x * 100.0), x > 0.5

    [<JSX.Component>]
    let row (name, age, v) =
        JSX.jsx $"""
            <tr>
                <td>{name}</td>
                <td>{age}</td>
                <td>{if v then "YES" else "NO"}</td>
            </tr>
        """

    [<JSX.Component>]
    let Table () =
        let init () =
            { Rows =
                [| "Marcin", 45, true
                   "Masha", 0, false
                   "Kasia", 36, true |]
            }, Cmd.none

        let update _msg state =
            { state with Rows = state.Rows |> Array.append [| rando() |]}, Cmd.none

        let state, _dispatch = Solid.createElmishStore (init, update)

        JSX.jsx $"""
        <>
            <button onclick={fun _ -> _dispatch ()}>
                +
            </button>
            <table>
                <thead>
                    <tr>
                        <td>Name</td>
                        <td>Age</td>
                        <td>RSVP</td>
                    </tr>
                </thead>
                <tbody>
                    {Solid.For(state.Rows, fun p idx -> row p)}
                </tbody>
            </table>
        </>
        """