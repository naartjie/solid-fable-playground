module Components

open System
open Browser.Types

printfn $"Loading {__SOURCE_FILE__}..."

open Fable.Core
open Elmish
open Elmish.Solid
open Fable.Core.JsInterop

[<JSX.Component>]
let FlashOnChangeDiv (txt: string) =
    let style, setStyle = Solid.createSignal ""

    let ref = Solid.createRef<HTMLDivElement> ()

    let () =
        Solid.createEffect (fun _ ->
            // JS.console.log txt
            ref.Value?style <- $"background-color: red; {txt}"

            let _ =
                1000 |> JS.setTimeout (fun _ -> ref.Value?style <- "background-color: #96D4D4;")

            ())

    JSX.jsx
        $"""
            <div ref={fun el -> ref.Value <- el}>{txt}</div>
            """


module Table =
    type Row =
        { Name: string
          Age: int
          Rsvp: bool }

        static member Make(name: string, age: int, rsvp: bool) = { Name = name; Age = age; Rsvp = rsvp }


    type State = { Rows: Row array }

    type Field =
        | Name
        | Age
        | Rsvp

    type Msg =
        | Add1
        | AddX of int
        | Reset
        | Modify of int * Field

    let randomNewRow () =
        let x = JS.Math.random ()

        { Name = System.Guid.NewGuid().ToString()
          Age = int (x * 100.0)
          Rsvp = x > 0.5 }


    [<JSX.Component>]
    let RowCmp (row: Row) idx dispatch =
        JS.console.log("RowCmp")
        JSX.jsx
            $"""
            <tr style={"cursor: pointer"}>
                <td onMouseOver={fun _ ->
                    Modify(idx (), Field.Age) |> dispatch
                    Modify(idx (), Field.Name) |> dispatch

                    }><FlashOnChangeDiv txt={row.Name} /></td>
                <td onclick={fun _ -> Modify(idx (), Field.Age) |> dispatch}><input value={row.Age} /></td>
                <td onclick={fun _ -> Modify(idx (), Field.Rsvp) |> dispatch}><FlashOnChangeDiv txt={if row.Rsvp then "YES" else "NO"}/></td>
            </tr>
            """


    [<JSX.Component>]
    let Table () =
        let init () =
            { Rows =
                [| Row.Make("Marcin", 45, true)
                   Row.Make("Masha", 0, false)
                   Row.Make("Kasia", 36, true) |] },
            Cmd.none

        let update msg state =
            match msg with
            | Reset -> init ()
            | Add1 ->
                { state with
                    Rows = (state.Rows, [| randomNewRow () |]) ||> Array.append },
                Cmd.none
            | AddX numberOfRows ->
                let newRows = (fun _idx -> randomNewRow ()) |> Array.init numberOfRows

                { state with
                    Rows = (state.Rows, newRows) ||> Array.append },
                Cmd.none
            | Modify(rowNo, field) ->
                { state with
                    Rows =
                        state.Rows
                        |> Array.mapi (fun idx row ->
                            if idx = rowNo then
                                let newRow = randomNewRow ()

                                match field with
                                | Name -> { row with Name = newRow.Name }
                                | Age -> { row with Age = newRow.Age }
                                | Rsvp -> { row with Rsvp = newRow.Rsvp }
                            else
                                row) },
                Cmd.none

        let state, dispatch = Solid.createElmishStore (init, update)

        // using signal
        // let rows, _ = init ()
        // let rows, setRows = Solid.createSignal rows
        //
        // let dispatch msg =
        //     let newRows, _ = update msg (rows ())
        //     setRows newRows


        let input = Solid.createRef<HTMLInputElement> ()

        JSX.jsx
            $"""
            <>
                <input ref={fun el -> input.Value <- el} />
                <button onclick={fun _ -> input.Value.value |> int |> AddX |> dispatch} >
                    +
                </button>
                <button onClick={fun _ -> dispatch Reset}>Reset</button>
                <table>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Age</th>
                            <th>RSVP</th>
                        </tr>
                    </thead>
                    <tbody>
                        {Solid.For(state.Rows, (fun p idx -> RowCmp p idx dispatch))}
                    </tbody>
                </table>
            </>
            """
