namespace IAMBot.IAMReference

open System.Net.Http
open FSharp.Json

module Reference =
    let Load () : Async<ReferenceFile> =
        async {
            printfn "Client"
            use client = new HttpClient()

            printfn "Request"

            let! response =
                client.GetStringAsync("https://raw.githubusercontent.com/iann0036/iam-dataset/main/iam_definition.json")
                |> Async.AwaitTask

            printfn "Deserialize"
            return Json.deserialize<ReferenceFile> response
        }
