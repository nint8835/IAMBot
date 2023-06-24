namespace IAMBot.IAMReference

open System.Net.Http
open FSharp.Json

module Reference =
    let Load () : Async<ReferenceFile> =
        async {
            use client = new HttpClient()

            let! response =
                client.GetStringAsync("https://raw.githubusercontent.com/iann0036/iam-dataset/main/iam_definition.json")
                |> Async.AwaitTask

            return Json.deserialize<ReferenceFile> response
        }
