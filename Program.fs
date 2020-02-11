open System

open FSharp.Data.Sql

FSharp.Data.Sql.Common.QueryEvents.SqlQueryEvent |> Event.add (printfn "Executing SQL: %O")

[<Literal>]
let ConnString  = "Server=localhost;Database=test;User=njeiseck;Password=password"

[<Literal>]
let DbVendor    = Common.DatabaseProviderTypes.MYSQL

[<Literal>]
let ContextSchemaPath =  __SOURCE_DIRECTORY__ + @"/test.schema"

type Sql = SqlDataProvider<DatabaseVendor = DbVendor,
                           ConnectionString = ConnString,
                           ContextSchemaPath = ContextSchemaPath,
                           Owner = "Test",
                           UseOptionTypes = true
                           >

let ctx = Sql.GetDataContext()

let query1 = ctx.Test.People 
          |> Seq.map (fun e -> e.ColumnValues |> Seq.toList)
          |> Seq.toList

let query2 =
    query {
      for t in ctx.Test.People do
        where (t.FirstName.Value.Contains "F") // Name is Nullable
        select (t.FirstName, t.LastName, t.Info, t.Id)
    } |> Seq.toList

[<EntryPoint>]
let main argv =

    // Query some data
    printfn "Q1: %A" query1
    printfn "Q2: %A" query2

    // Add
    let rows = 
      seq { 
        for i in 1 .. 10 do
          let row = ctx.Test.People.Create()
          row.FirstName <- Some "First Name";
          row.LastName <- Some "Last Name"
          yield row
      } |> Seq.toList

    ctx.SubmitUpdates()

    // print generated ids
    for row in rows do
      printfn "New Id %A" row.Id

    // Rather crude way to store the schema data so no actual DB is needed in CI
    //printfn "Context Schema: %s" ContextSchemaPath
    //ctx.SaveContextSchema() |> ignore

    0 // exit code