// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2020 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}
module WebSharper.AspNetCore.Tests.WebSocketClient

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.AspNetCore.WebSocket
open WebSharper.AspNetCore.WebSocket.Client

module Server = WebSocketServer


[<Rpc>] 
let printit (it) = printfn "printit: %s" it 

[<Rpc>] 
let spiltWithNoAsync(str: string) = str.Split(",")

[<Rpc>] 
let rpcSplit (str: string) = async { return str.Split(",") }


[<JavaScript>]
let WebSocketTest (str: string, endpoint : WebSocketEndpoint<Server.S2CMessage, Server.C2SMessage>) =
    // match str with
    // | Some string as message ->
    // printfn "WebSocket start with: %s" str


    let container = Elt.pre [] []
    let writen fmt =
        Printf.ksprintf (fun s ->
            JS.Document.CreateTextNode(s + "\n")
            |> container.Dom.AppendChild
            |> ignore
        ) fmt
    async {
        let! server =
            ConnectStateful endpoint <| fun server -> async {
                return 0, fun state msg -> async {
                    match msg with
                    | Message data ->
                        match data with
                        | Server.Response1 x -> async{
                                                    printit(x)
                                                    if x.Length > 1 then
                                                        writen "%s" x
                                                    //let! result = rpcSplit(x)
                                                    //if result.Length > 0 then
                                                    //    writen "Response1 %s (user: %s)" result.[1] result.[0]
                                                }|> Async.Start
                                                    // writen "Response1 %s (state: %i)" x state
                        | Server.Response2 x -> writen "Response2 %i (state: %i)" x state
                        return (state + 1)
                    | Close ->
                        // writen "WebSocket connection closed."
                        return state
                    | Open ->
                        // writen "WebSocket connection open."
                        return state
                    | Error ->
                        writen "WebSocket connection error!"
                        return state
                }
            }
       

        // let send(str: string) =
        //     server.Post (Server.Request3 str)


        do
            // server.Post (Server.Request1 [| str |])
            //  printit("sssssssss"+str)
            // let msg1 = rpcSplit(str)
            if spiltWithNoAsync(str).Length > 2 then
                match spiltWithNoAsync(str).[2] with
                | "Register" ->
                    server.Post (Server.Register str)
                | "Subscribe" ->
                    server.Post (Server.Subscribe str)
                | "Tweet" ->
                    server.Post (Server.Tweet str)
                | "ReTweet" ->
                    server.Post (Server.ReTweet str)
                | "Query" ->
                    server.Post (Server.SubQuery str)
                | "QueryTag" ->
                    server.Post (Server.TagQuery str)
                | "QueryAt" ->
                    server.Post (Server.AtQuery str)
                | "Login" ->
                    server.Post (Server.LoginQuery str)
                | "Logout" ->
                    server.Post (Server.LogoutQuery str)
                | _ ->
                    printit("Error")
            else server.Post (Server.Request4 str)


            
            // writen "Client receive: %s" str
            JQuery.JQuery.Ajax(
                JQuery.AjaxSettings(
                    Url = "/ws.txt",
                    Method = JQuery.RequestType.GET,
                    Success = (fun x _ _ -> writen "%s" (x :?> _)),
                    Error = (fun _ _ e -> writen "KO: %s." e)
                )
            ) |> ignore

        
        // server.Post (Server.Request1 [| str |])

        //let lotsOfHellos = "HELLO" |> Array.create 1000
        //let lotsOf123s = 123 |> Array.create 1000
        // printfn "mes: %s" mes
        
       // while true do
           // do! Async.Sleep 10000
           // server.Post (Server.Request1 [| "HELLO" |])
            // do! Async.Sleep 10000
            // server.Post (Server.Request2 lotsOf123s)
    }
    |> Async.Start

    container
    // str


open WebSharper.AspNetCore.WebSocket

let MyEndPoint (url: string) : WebSharper.AspNetCore.WebSocket.WebSocketEndpoint<Server.S2CMessage, Server.C2SMessage> = 
    WebSocketEndpoint.Create(url, "/ws", JsonEncoding.Readable)