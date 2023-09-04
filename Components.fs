module Components

open Browser.Types

printfn $"Loading {__SOURCE_FILE__}..."

open Fable.Core
open Elmish
open Elmish.Solid
open Fable.Core.JsInterop
open Feliz.JSX.Solid

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
        | Add
        | Modify of int * Field

    let rando () =
        let x = JS.Math.random ()

        { Name = System.Guid.NewGuid().ToString()
          Age = int (x * 100.0)
          Rsvp = x > 0.5 }

    [<JSX.Component>]
    let DivTxt (txt: string) =
        let style, setStyle = Solid.createSignal ""

        let reference = Solid.createRef<HTMLDivElement> ()

        let () =
            Solid.createEffect (fun _ ->
                JS.console.log $"rerender div with {txt}"
                reference.Value?style <- "background-color: red"

                let _ =
                    JS.setTimeout (fun _ -> reference.Value?style <- "background-color: #96D4D4;") 1000

                ())

        Html.div [ Solid.ref reference; Html.children [ Html.text txt ] ]

    [<JSX.Component>]
    let RowCmp (row: Row) idx dispatch =
        JSX.jsx
            $"""
            <tr style={"cursor: pointer"}>
                <td onclick={fun _ -> Modify(idx (), Field.Name) |> dispatch}><Table_DivTxt txt={row.Name}></Table_DivTxt></td>
                <td onclick={fun _ -> Modify(idx (), Field.Age) |> dispatch}><Table_DivTxt txt={row.Age} /></td>
                <td onclick={fun _ -> Modify(idx (), Field.Rsvp) |> dispatch}><Table_DivTxt txt={if row.Rsvp then "YES" else "NO"} /></td>
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
            | Add ->
                { state with
                    Rows = (state.Rows, [| rando () |]) ||> Array.append },
                Cmd.none
            | Modify(rowNo, field) ->
                { state with
                    Rows =
                        state.Rows
                        |> Array.mapi (fun idx row ->
                            if idx = rowNo then
                                let newRow = rando ()

                                match field with
                                | Name -> { row with Name = newRow.Name }
                                | Age -> { row with Age = newRow.Age }
                                | Rsvp -> { row with Rsvp = newRow.Rsvp }
                            else
                                row) },
                Cmd.none

        let state, dispatch = Solid.createElmishStore (init, update)

        JSX.jsx
            $"""
            <>
                <button onclick={fun _ -> dispatch Add}>
                    +
                </button>
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
