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
module WebSharper.AspNetCore.Tests.WebSocketServer

open WebSharper
open WebSharper.AspNetCore.WebSocket.Server
open System.Collections.Generic
open structureType

[<Rpc>] 
let rpcSplit (str: string) = async { return str.Split(",") }

[<Rpc>] 
let append (str1: string,str2: string) = async { return str1+str2 }

[<Rpc>]
let FindTag(content: string) = async {
        let mutable hashtags =new List<string>()
        let len = String.length content
        for i = 0 to len - 1 do
            if content.[i] = '#' then
                    let mutable j = i
                    while j < len && content.[j] <> ' ' do
                        j <- j + 1
                    hashtags.Add(content.[i..j-1])
        return hashtags}

[<Rpc>]
let FindAt(content: string) = async {
        let mutable Ats =new List<string>()
        let len = String.length content
        for i = 0 to len - 1 do
            if content.[i] = '@' then
                    let mutable j = i
                    while j < len && content.[j] <> ' ' do
                        j <- j + 1
                    Ats.Add(content.[i..j-1])
        return Ats}


type [<JavaScript; NamedUnionCases>]
    C2SMessage =
    | Request1 of str: string[]
    | Request2 of int: int[]
    | Request4 of mes4: string
    | Request3 of mes3: string
    | Register of reg: string
    | Tweet of twe: string
    | Subscribe of sub: string
    | SubQuery of subq: string
    | TagQuery of tagq: string
    | AtQuery of atq: string
    | LoginQuery of loginq: string
    | LogoutQuery of logoutq: string
    | ReTweet of ret: string

and [<JavaScript; NamedUnionCases "type">]
    S2CMessage =
    | [<Name "int">] Response2 of value: int
    | [<Name "string">] Response1 of value: string

let mutable clients = new Dictionary<string, WebSocketClient<S2CMessage,C2SMessage>>()

let Start() : StatefulAgent<S2CMessage, C2SMessage, int> =
    /// print to debug output and stdout
    let userList = new List<user>()
    let mutable userState = new Dictionary<string, bool>()
    let globalTweetList = new Dictionary<string, tweet>()
    let tagDict = new Dictionary<string, List<tweet>>()
    let atDict = new Dictionary<string, List<tweet>>()
    let mutable tweetId=1

    
    let dprintfn x =
        Printf.ksprintf (fun s ->
            System.Diagnostics.Debug.WriteLine s
            stdout.WriteLine s
        ) x

    fun client -> async {
        let clientIp = client.Connection.Context.Connection.RemoteIpAddress.ToString()
        let mutable receiver = client



        return 0, fun state msg -> async {
            dprintfn "Received message #%i from %s" state clientIp
            match msg with
            | Message data -> 
                match data with
                | Request1 x -> do! client.PostAsync (Response1 x.[0])
                | Request2 x -> do! client.PostAsync (Response2 x.[0])
                | Request3 x -> do! printfn("Request4")
                                    async{
                                        let! result = rpcSplit(x)
                                        let resultLen = result.Length
                                        let username = result.[0]
                                        if clients.ContainsKey(username) then
                                            receiver <- clients.Item(username)
                                        else
                                            clients.Add(username,client)
                                        receiver.PostAsync (Response1 x) |> ignore
                                    }

                | Request4 x -> do! printfn("Request4")
                                    async{
                                        let! result = rpcSplit(x)
                                        let resultLen = result.Length
                                        let username = result.[0]
                                        if clients.ContainsKey(username) then
                                            receiver <- clients.Item(username)
                                        else
                                            clients.Add(username,client)
                                        receiver.PostAsync (Response1 x) |> ignore
                                    }
                         
                | Register x -> do! 
                                printfn("Register")
                                async{
                                    let! result = rpcSplit(x)
                                    let username = result.[0]
                                    let joinUser=user(result.[0])
                                    let thisclient = clients.Item(username)
                                    
                                    if clients.ContainsKey(username) then
                                        receiver <- clients.Item(username)
                                    else
                                        clients.Add(username,client)
                                    userList.Add(joinUser)
                                    userState.Add(joinUser.userName,true)
                                    for item in userList do
                                        printf "userlist %s" item.userName
                                    printfn ""
                                    let! returnStr=append("\nRegister success for user: ", result.[1])
                                    receiver.PostAsync (Response1 returnStr) |> ignore
                                }
                | Tweet x -> do!
                            printfn("Tweet")
                            async{
                                let! result = rpcSplit(x)
                                let mutable tweet= tweet(result.[0],result.[1])
                                let! Ats=FindAt(tweet.content)
                                let! Tag=FindTag(tweet.content)
                                tweet.Ats<-Ats
                                tweet.hashtags<-Tag
                                tweet.content<-tweetId.ToString()+": "+tweet.content
                                globalTweetList.Add(tweetId.ToString(),tweet)
                                tweetId<-tweetId+1
                                receiver <- clients.Item(result.[0])
                                let returnStr="\nReceived tweet from "+result.[0]+":\n"+tweet.content
                                receiver.PostAsync (Response1 returnStr) |> ignore
                                for item in userList do
                                    if item.userName = result.[0] then
                                        item.TweetList.Add(tweet)
                                        for follower in item.Subscribers do
                                            if userState.Item(follower) then
                                                receiver<- clients.Item(follower)
                                                receiver.PostAsync (Response1 returnStr) |> ignore
                                for item in tweet.hashtags do
                                    printfn("Find hash tag %s") item
                                    if tagDict.ContainsKey(item) then
                                        tagDict.Item(item).Add(tweet)
                                    else
                                        let newList = new List<tweet>()
                                        newList.Add(tweet)
                                        tagDict.Add(item, newList)
                                for item in tweet.Ats do
                                    printfn("Find at %s") item
                                    if atDict.ContainsKey(item) then
                                        atDict.Item(item).Add(tweet)
                                    else
                                        let newList = new List<tweet>()
                                        newList.Add(tweet)
                                        atDict.Add(item, newList)                             
                            }
                | ReTweet x-> do!
                                printfn("ReTweet")
                                async{
                                    let! result = rpcSplit(x)
                                    let re=globalTweetList.Item(result.[1])
                                    receiver <- clients.Item(result.[0])
                                    receiver.PostAsync (Response1 "\nRetweet:") |> ignore
                                    receiver.PostAsync (Response1 re.content) |> ignore
                                    for item in userList do
                                        if item.userName = result.[0] then
                                            item.TweetList.Add(re)
                                            for follower in item.Subscribers do
                                                if userState.Item(follower) then
                                                    let returnStr="\nReceived tweet from "+result.[0]+":\n"+re.content
                                                    receiver<- clients.Item(follower)
                                                    receiver.PostAsync (Response1 returnStr) |> ignore
                                }
                | Subscribe x-> do!
                                printfn("Subscribe")  
                                async{
                                    let! result = rpcSplit(x)
                                    for item in userList do
                                        if item.userName = result.[1] then 
                                            item.Subscribers.Add(result.[0])
                                        elif item.userName = result.[0]
                                        then item.Subscribe.Add(result.[1])
                                    
                                    receiver <- clients.Item(result.[0])
                                    let mutable returnStr="\nSuccessfully subscribe user:" + result.[1]
                                    receiver.PostAsync (Response1 returnStr) |> ignore
                                    receiver <- clients.Item(result.[1])
                                    returnStr<-"\nSubscribed by user:" + result.[0]
                                    receiver.PostAsync (Response1 returnStr) |> ignore
                                }  
                | SubQuery x-> do!
                                printfn("SubQuery")
                                async{
                                    let! result = rpcSplit(x)
                                    receiver <- clients.Item(result.[0])
                                    let mutable returnStr="\nSever receiving query for sucribed tweets of " + result.[0]
                                    receiver.PostAsync (Response1 returnStr) |> ignore
                                    let mutable returnTweet=""
                                    for user1 in userList do
                                        if user1.userName = result.[0]
                                           && user1.Subscribe.Count > 0 then
                                            for user2 in userList do
                                                if user1.Subscribe.Exists(fun x -> x = user2.userName) then
                                                    returnStr<-"Received queried tweets from " + user2.userName
                                                    receiver.PostAsync (Response1 returnStr) |> ignore
                                                    for tweet in user2.TweetList do
                                                        receiver.PostAsync (Response1 tweet.content) |> ignore
                                }
                | AtQuery x-> do!
                            printfn("AtQuery")
                            async{
                                let! result = rpcSplit(x)
                                receiver <- clients.Item(result.[0])
                                let mutable returnStr="\nSever receiving query for at: "+result.[1]+", From: " + result.[0]+"\n"
                                receiver.PostAsync (Response1 returnStr) |> ignore
                                if atDict.ContainsKey(result.[1]) then
                                    for tweet in atDict.Item(result.[1]) do
                                        receiver.PostAsync (Response1 tweet.content) |> ignore

                            }
                | TagQuery x-> do!
                            printfn("TagQuery")
                            async{
                                let! result = rpcSplit(x)
                                receiver <- clients.Item(result.[0])
                                let mutable returnStr="\nSever receiving query for tag: "+result.[1]+", From: " + result.[0]+"\n"
                                receiver.PostAsync (Response1 returnStr) |> ignore
                                if tagDict.ContainsKey(result.[1]) then
                                    for tweet in tagDict.Item(result.[1]) do
                                        receiver.PostAsync (Response1 tweet.content) |> ignore

                            }
                | LoginQuery x-> do!
                            printfn("LoginQuery")
                            async{
                                let! result = rpcSplit(x)
                                receiver <- clients.Item(result.[0])
                                userState.Item(result.[0]) <- true
                                let mutable returnStr="\nLogin: "+result.[0]+""
                                receiver.PostAsync (Response1 returnStr) |> ignore
                            }
                | LogoutQuery x-> do!
                            printfn("LogoutQuery")
                            async{
                                let! result = rpcSplit(x)
                                receiver <- clients.Item(result.[0])
                                userState.Item(result.[0]) <- false
                                let mutable returnStr="\nLog Out: "+result.[0]+""
                                receiver.PostAsync (Response1 returnStr) |> ignore
                            }

                return state + 1
            | Error exn -> 
                eprintfn "Error in WebSocket server connected to %s: %s" clientIp exn.Message
                do! client.PostAsync (Response1 ("Error: " + exn.Message))
                return state
            | Close ->
                dprintfn "Closed connection to %s" clientIp
                return state
        }
    }
