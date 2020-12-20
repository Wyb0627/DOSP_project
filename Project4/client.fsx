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
let actorNum = args.[2] |> int
let numRequests = args.[3] |> int

let mutable fake = true

let client (name: string,
            actorNumber: string,
            clientSystem: ActorSystem,
            clientPath: string,
            severSystem: ActorSystem,
            severPath: string) =
    // printfn "%s" name
    // printfn "client:%s" actorNumber
    if action = "showfunction" then
        let mutable newuser = user(name)
        printfn "user:%s regists" name
    select (severPath + "sever") severSystem <! user(name)//newuser
    // select (severPath + "sever") severSystem <! test("test")

    spawn clientSystem name
    <| fun mailbox ->

        let rec loop () =
            actor {
                let! msg = mailbox.Receive()

                match box msg with
                | :? tweetMessage as tweetMessage ->
                    if action = "showfunction" then
                        printfn "client:%s tweets:%s" name tweetMessage.tweet.content
                    select (severPath + "sever") severSystem
                    <! tweetMessage
                | :? followMessage as message ->
                    if action = "showfunction" then
                        printfn "user:%s follows user:%s" message.user message.followUser
                    select (severPath + "sever") severSystem
                    <! message
                | :? int as tests -> if tests = 0 then printf ""
                | :? (List<string>) as send_list -> printf ""

                | :? string as receiveStr ->
                    if receiveStr = "abc" then
                        printfn "client %s receive %s" name receiveStr
                        select (severPath + "sever") severSystem <! receiveStr
                    else printfn "client %s receive %s" name receiveStr
                | :? tweet as tweet ->
                    if action = "showfunction" then
                        printfn "%s Received tweet:%s from Sever sent by %s " name tweet.content tweet.retweet_list.[tweet.retweet_list.Count - 1]
                        printfn "%s retweet:%s" name tweet.content
                    let mutable retweet = tweet
                    retweet.isRetweet <- true
                    if action = "showfunction" then
                        let retweetmes = tweetMessage(name, retweet, true)
                        select (severPath + "sever") severSystem <! retweetmes
                | :? List<tweet> as tweetList ->
                    for item in tweetList do
                        printfn "%s receive queried tweet:%s from Sever" name item.content
                | :? querySub as querySub ->
                    printfn "%s query subcribed tweets" name
                    select (severPath + "sever") severSystem
                    <! querySub
                | :? queryTag as queryTag ->
                    printfn "%s query Tag: %s" name queryTag.tag
                    select (severPath + "sever") severSystem
                    <! queryTag
                | :? queryAt as queryAt ->
                    printfn "%s query user:%s's tweets" name queryAt.at
                    select (severPath + "sever") severSystem
                    <! queryAt
                | :? link as usrLink ->
                    printfn "user:%s connect" usrLink.userName
                    select (severPath + "sever") severSystem
                    <! usrLink
                | :? unlink as usrUnLink ->
                    printfn "user:%s disconnect" usrUnLink.userName
                    select (severPath + "sever") severSystem
                    <! usrUnLink
                | _ -> printfn "client error"

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
                    port = 8777
                    hostname = localhost
                }
            }
        }")



let clientSystem = ActorSystem.Create("remoteFSharp", config)
let severSystem = clientSystem
let clientPath = "akka.tcp://remoteFSharp@localhost:8777/user/"
let severPath = "akka.tcp://RemoteFSharp@localhost:8888/user/"



let ctweet(tweet_owner: string, content: string) = 
    let mutable newtweet = tweet(tweet_owner, content)
    newtweet.FindTagandAt()
    newtweet

// let stopWatch = System.Diagnostics.Stopwatch.StartNew()

if action = "performance" then

    let actorList =
        [| for i = 0 to actorNum - 1 do
            let newactorNumber = i.ToString()
            client (newactorNumber, newactorNumber, clientSystem, clientPath, severSystem, severPath) |]

    for i = 1 to actorNum-1 do
        let follow = followMessage(i.ToString(),"0")
        select (clientPath + i.ToString()) clientSystem <! follow
        // System.Threading.Thread.Sleep(1)

    let timer = timer()
    select (severPath + "sever") severSystem <! timer

    for m=1 to numRequests do
        let testtweet = ctweet("0",m.ToString())
        let testtweetmes = tweetMessage("0", testtweet, false)
        select (clientPath + "0") clientSystem <! testtweetmes


    let endtimer = endtimer()
    select (severPath + "sever") severSystem <! endtimer
elif action = "showfunction" then
    let actorList =
        [| for i = 0 to actorNum - 1 do
            let newactorNumber = i.ToString()
            client (newactorNumber, newactorNumber, clientSystem, clientPath, severSystem, severPath) |]


    let follow1 = followMessage("2","1")
    let follow2 = followMessage("1","0")
    select (clientPath + "2") clientSystem <! follow1
    select (clientPath + "1") clientSystem <! follow2

    System.Threading.Thread.Sleep(1000)

    let testtweet = ctweet("0","123 #abc @7")
    let testtweetmes = tweetMessage("0", testtweet, false)

    select (clientPath + "0") clientSystem <! testtweetmes

    System.Threading.Thread.Sleep(1000)

    let unlink2 = unlink("2")
    select (clientPath + "2") clientSystem <! unlink2

    System.Threading.Thread.Sleep(1000)

    let testtweet2 = ctweet("0","#ddd @5 #rrr")
    let testtweetmes2 = tweetMessage("0", testtweet2, false)
    select (clientPath + "0") clientSystem <! testtweetmes2

    System.Threading.Thread.Sleep(1000)

    let link2 = link("2")
    select (clientPath + "2") clientSystem <! link2

    System.Threading.Thread.Sleep(1000)

    let queryname = querySub("1")
    select (clientPath + "1") clientSystem <! queryname

    System.Threading.Thread.Sleep(1000)

    let querytag = queryTag("#abc","0")
    select (clientPath + "0") clientSystem <! querytag

    System.Threading.Thread.Sleep(1000)

    let queryat = queryAt("@5","2")
    select (clientPath + "2") clientSystem <! queryat


// System.Threading.Thread.Sleep(1000)

// printfn "time:%f" stopWatch.Elapsed.TotalMilliseconds

while (true) do
    fake <- true