#r "nuget: Akka.FSharp"
#r "nuget: Akka.Remote"
#load "structureType.fsx"


open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open System.Collections.Generic
open System
open structureType


let args: string array = fsi.CommandLineArgs
let action = args.[1] |> string
let rand = System.Random(1234)
let mutable fake = true

let sever (name: string, clientSystem: ActorSystem, clientPath: string, severSystem: ActorSystem, severPath: string) =
    let UserList = new List<user>() //我先把用户列表放sever里了
    let globalTweetList = new List<tweet>()
    let tagDict = new Dictionary<string, List<tweet>>()
    let atDict = new Dictionary<string, List<tweet>>()
    let mutable state = new Dictionary<string, bool>()
    let stopWatch = System.Diagnostics.Stopwatch()

    spawn severSystem name
    <| fun mailbox ->
        let rec loop () =
            actor {
                let! msg = mailbox.Receive()

                match box msg with
                | :? followMessage as message -> //关注
                    for item in UserList do
                        if item.userName = message.followUser then 
                            item.Subscribers.Add(message.user)
                            if action = "showfunction" then
                                printfn "%s followed by %s" item.userName message.user
                        elif item.userName = message.user then item.Subscribe.Add(message.followUser)



                | :? querySub as querySub ->
                    printfn "Sever receive query for sucribed tweets from %s" querySub.userName
                    for user1 in UserList do
                        if user1.userName = querySub.userName
                           && user1.Subscribe.Count > 0 then
                            for user2 in UserList do
                                if user1.Subscribe.Exists(fun x -> x = user2.userName) then
                                    select (clientPath + user1.userName) clientSystem
                                    <! user2.TweetList
                //for tweet in user2.TweetList do
                //select (clientPath + user1.userName) clientSystem
                //<! tweet


                | :? queryTag as queryTag ->
                    printfn "Sever receive query for Tag:%s from:%s" queryTag.tag queryTag.userName 
                    if tagDict.ContainsKey(queryTag.tag) then
                        select (clientPath + queryTag.userName) clientSystem
                        <! tagDict.Item(queryTag.tag)

                | :? queryAt as queryAt ->
                    printfn "Sever receive query user:%s's tweets from:%s" queryAt.at queryAt.userName 
                    if atDict.ContainsKey(queryAt.at) then
                        select (clientPath + queryAt.userName) clientSystem
                        <! atDict.Item(queryAt.at)


                | :? user as JoinUser -> 
                    if action = "showfunction" then
                        printfn "user:%s regist and connect" JoinUser.userName
                    UserList.Add(JoinUser)
                    state.Add(JoinUser.userName,true)
                    // for item in UserList do
                    //     printf "%s" item.userName
                    // printfn ""
                    // printfn "----------------------------------------"
                  

                | :? tweetMessage as tweetMessage ->
                    if action = "showfunction" then
                        if tweetMessage.isRetweet = true then
                            printfn "Sever receive retweeted tweet:%s from %s" tweetMessage.tweet.content tweetMessage.userName
                        else printfn "Sever receive tweet:%s from %s" tweetMessage.tweet.content tweetMessage.userName
                    tweetMessage.tweet.retweet (tweetMessage.userName)
                    globalTweetList.Add(tweetMessage.tweet)
                    for item in UserList do
                        if item.userName = tweetMessage.userName then
                            item.TweetList.Add(tweetMessage.tweet)
                            // printfn "%s's subscribers:%d" item.userName item.Subscribers.Count
                            for follower in item.Subscribers do
                                // printfn "subscriber:%s" follower
                                if state.Item(follower) = true then
                                    select (clientPath + follower) clientSystem
                                    <! tweetMessage.tweet
                    if tweetMessage.tweet.hashtags.Count > 0 then
                        for item in tweetMessage.tweet.hashtags do
                            if tagDict.ContainsKey(item) then
                                tagDict.Item(item).Add(tweetMessage.tweet)
                            else
                                let mutable newList = new List<tweet>()
                                newList.Add(tweetMessage.tweet)
                                tagDict.Add(item, newList)
                    if tweetMessage.tweet.Ats.Count > 0 then
                        for item in tweetMessage.tweet.Ats do
                            if atDict.ContainsKey(item) then
                                atDict.Item(item).Add(tweetMessage.tweet)
                            else
                                let mutable newList = new List<tweet>()
                                newList.Add(tweetMessage.tweet)
                                atDict.Add(item, newList)
                    // printfn "tagDict"
                    // for item in tagDict do
                    //     printfn "key:%s" item.Key
                    //     for thing in item.Value do
                    //         printfn "value:%s" thing.content
                    // printfn "atDict"
                    // for item in atDict do
                    //     printfn "key:%s" item.Key
                    //     for thing in item.Value do
                    //         printfn "value:%s" thing.content
                    
                | :? link as usrLink ->
                    state.Item(usrLink.userName)<- true
                
                | :? unlink as usrLink ->
                    state.Item(usrLink.userName)<- false



                | :? string as receiveStr ->
                    printfn "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!"
                    printfn "sever receive %s" receiveStr
                    let randActNum = rand.Next(0, 100)
                    select (clientPath + randActNum.ToString()) clientSystem
                    <! "123"

                //UserList.Add(JoinUser)
                | :? timer as timer ->
                    stopWatch.Start()
                    fake <- true
                | :? endtimer as endtimer ->
                    printfn "time:%f" stopWatch.Elapsed.TotalMilliseconds
                    fake <- true
                | :? test as testmessage -> 
                    printfn "test"
                | _ -> printfn "sever error"

                return! loop ()
            }

        loop ()



let config =
    ConfigurationFactory.ParseString
        (@"akka {
            log-config-on-start : on
            stdout-loglevel : DEBUG
            loglevel : ERROR
            actor {
                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                debug : {
                    receive : on
                    autoreceive : on
                    lifecycle : on
                    event-stream : on
                    unhandled : on
                }
            }
            remote {
                helios.tcp {
                    port = 8888
                    hostname = localhost
                }
            }
        }")


let severSystem = ActorSystem.Create("RemoteFSharp", config)
let clientSystem = severSystem
let clientPath = "akka.tcp://remoteFSharp@localhost:8777/user/"
let severPath = "akka.tcp://RemoteFSharp@localhost:8888/user/"

sever ("sever", clientSystem, clientPath, severSystem, severPath)


while (true) do
    fake <- true