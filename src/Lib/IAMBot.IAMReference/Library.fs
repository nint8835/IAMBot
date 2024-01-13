namespace IAMBot.IAMReference

open FSharp.Json
open System.Net.Http

module Reference =
    let Load () : Async<ReferenceFile> =
        async {
            use client = new HttpClient()

            let! response =
                client.GetStringAsync(
                    "https://raw.githubusercontent.com/iann0036/iam-dataset/main/aws/iam_definition.json"
                )
                |> Async.AwaitTask

            let config = JsonConfig.create (jsonFieldNaming = Json.snakeCase)

            return Json.deserializeEx<ReferenceFile> config response
        }
