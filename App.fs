module Program


printfn $"Loading {__SOURCE_FILE__}..."

open Elmish
open Browser
open Fable.Core

let Tbl = Components.Table.Table

[<JSX.Component>]
let App () =
    JSX.html
        $"""
        <>
            <h1>Welcome</h1>
            <code>version 1</code>
            <p/>
            <Tbl />
        </>
        """


Solid.render ((fun () -> App()), document.getElementById "app-root")
