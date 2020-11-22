#r "nuget: Akka.FSharp"

open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open System.Collections.Generic
open System

let args: string array = fsi.CommandLineArgs
let numNodes = args.[1] |> int
let numRequests = args.[2] |> int
let base_num = 4.
let b = Math.Log(base_num, 2.)

let row =int (ceil (Math.Log(float (numNodes), Math.Log(2. ** b))))

let col = int (2. ** b)
let M = int ((2. ** b) * 2.)
let L = int ((2. ** b) / 2.)
let globalactornum = new List<string>()

let system = ActorSystem.Create("a1")
let path = "akka://a1/user/"

let mutable jointimes = 0
let mutable needjointimes = 0
for i=0 to numNodes-1 do
    needjointimes <- needjointimes+i
let mutable finish_num = 0
let devide = Array.create numNodes 0.0
let mutable total_hop = 0
let mutable fake=0
let mutable s=new List<string>()
let mutable testing = false
for i = 0 to numNodes - 1 do
    devide.[i] <- float (i)


type Message =
    struct
        val mutable hop: string
        val mutable key: string
        val mutable hoptime: int
        new(hop: string, message: string, hoptime: int) = { hop = hop; key = message ; hoptime = hoptime}
    end

let inline charToInt c = int c - int '0'

let boss (name: string) =
    spawn system name
    <| fun mailbox ->
        let rec loop () =
            actor {
                let! msg = mailbox.Receive()
                match box msg with
                | :? Message as myMessage ->
                    total_hop<-total_hop+myMessage.hoptime
                    finish_num <- finish_num + 1
                | :? String as sMessage ->
                    if sMessage = "join" then
                        jointimes<- jointimes+1
                | _ -> printfn "error"
                return! loop ()
            }
        loop ()



let worker (name: string, actorNumber: string) =
    spawn system name
    <| fun mailbox ->
        let routing_table = Array2D.create col row ""
        let neigibor_set = new List<string>()
        let mutable leaf_set_small = []
        let mutable leaf_set_large = []
        let actorNumber_int = actorNumber |> int
        let mutable set_small = Array.create L "" 
        let mutable set_large = Array.create L "" 

        for j = 0 to row - 1 do
            routing_table.[(actorNumber.[j] |> charToInt), j] <- actorNumber
        
        for item in globalactornum do
            if item <> actorNumber then
                let mutable j=0
                while item.[j]= actorNumber.[j] do
                    j<-j+1
                if routing_table.[(item.[j] |> charToInt),j]="" then
                    routing_table.[(item.[j] |> charToInt),j]<-item


        
        for i in globalactornum do
            if i <> actorNumber then
                if actorNumber_int - (i |> int) > 0 then
                    leaf_set_small <- i :: leaf_set_small
                else 
                    leaf_set_large <- i :: leaf_set_large
                let mutable j = (-1)
                let mutable temp = i.[0] |> charToInt
                while j < row-1
                          && routing_table.[temp, j + 1] = ""
                          && i.[j + 1] = actorNumber.[j + 1] do
                    j <- j + 1
                    temp <- i.[j] |> charToInt
                    if j > (-1) then 
                        routing_table.[(i.[j] |> charToInt), j] <- i
        
        let send (message: Message) =
            let mutable mymessage = message
            let receiver = select (path + (mymessage.hop.ToString())) system
            mymessage.hoptime<-mymessage.hoptime+1
            if mymessage.hop.ToString()=mymessage.key then
                select (path + "boss") system <! mymessage
            else
                receiver <! mymessage



        let shl (i: string, actorNumber: string) =
            let mutable l = 0
            let mutable t = 0
            while t < row do
                if i.[t] = actorNumber.[t] then
                    l <- l + 1
                    t <- t + 1
                else
                    t <- row
            l

        let routing (message : Message) =
            let mutable mymessage = message
            let mutable sent = false
            let D = message.key
            let mutable small = false
            let mutable large = false
            let mutable temp = ""
            let mutable l = 0
            small <- Array.contains D set_small
            large <- Array.contains D set_large
            let leaf_set = Array.concat [ set_small; set_large ]
            let mutable mark = true
            let enumerator = Seq.toList (Seq.cast<string> routing_table)
            let len = List.length enumerator
            let mutable t = 0
            if small || large then
                mymessage.hop <- D
                send (mymessage)
                sent <- true
            else
                l <- shl (D, actorNumber)
                temp <- routing_table.[(D.[l] |> charToInt), l]
                if temp <> "" && temp <> actorNumber then   
                    mymessage.hop<-temp
                    send (mymessage)
                    sent<- true
                else
                    for i in leaf_set do
                        if i <> "" then
                            if shl (D,i.ToString())>= l
                               && abs ((i |> int) - (D|> int)) < abs (actorNumber_int - (D |> int)) then
                                mymessage.hop<-i
                                send (mymessage)
                                sent<- true
                                mark <- false
                    
                    while mark && t < len - 1 do
                        if enumerator.[t]="" then
                            t<- t+1
                        else if shl (D, enumerator.[t]) >= l && abs ((enumerator.[t] |> int) - (D |> int)) < abs (actorNumber_int - (D |> int)) && enumerator.[t]<>actorNumber then
                            mymessage.hop<-enumerator.[t]
                            send (mymessage)
                            sent<- true
                            mark <- false
                        else
                            t<-t+1
            if sent = false then
                select (path + "boss") system <! mymessage




        let rec loop () =
            actor {
                let! msg = mailbox.Receive()
                let enumerator = Seq.toList (Seq.cast<string> routing_table)
                match box msg with
                | :? Message as myMessage -> routing (myMessage)
                | :? int as tests ->
                    if tests = 0 then
                        testing <- true
                        testing <- false
                | :? (List<string>) as send_list ->
                    let mutable m=Message("","",0)
                    for i in send_list do
                        m.key<-i
                        routing (m)
                        System.Threading.Thread.Sleep(1000)
                | :? string as joined_actor ->
                    let joined_actor_str = joined_actor.ToString()
                    if actorNumber_int - (joined_actor |> int) > 0
                    then leaf_set_small <- joined_actor :: leaf_set_small
                    else leaf_set_large <- joined_actor :: leaf_set_large

                    let leaf_set_small_s =
                        List.sortBy (fun x -> -( x|> int) - 1) leaf_set_small

                    let leaf_set_large_s = List.sortBy (fun x -> x) leaf_set_large
                    
                    set_small <- List.toArray leaf_set_small_s.[..L]
                    set_large <- List.toArray leaf_set_large_s.[..L]
                    let mutable i = 0
                    let mutable temp = joined_actor_str.[0] |> charToInt
                    while i < row
                          && joined_actor_str.[i] = actorNumber.[i] do
                        i <- i + 1
                        temp <- joined_actor_str.[i] |> charToInt
                    if routing_table.[(joined_actor_str.[i] |> charToInt), i]="" then 
                        routing_table.[(joined_actor_str.[i] |> charToInt), i] <- joined_actor_str
                    select (path + "boss") system <! "join"

                | _ -> printfn "error"

                return! loop ()
            }

        loop ()


let return_list (actornum) =
    let rand = System.Random(1234)
    let mutable number = 0
    let str_list = new List<string>()
    let mutable i = 0
    let temp = globalactornum.ToArray()

    let global_array =
        Array.filter (fun x -> x <> actornum) temp

    if global_array.Length > 0 then
        for i = 0 to numRequests - 1 do
            number <- rand.Next(0, global_array.Length)
            str_list.Add(global_array.[number])
    str_list


let return_number () =
    let rand = System.Random(1234)
    let mutable number = 0
    let mutable strnum = ""
    while globalactornum.Contains(strnum) || strnum="" do
        strnum<-""
        for i = 0 to row - 1 do
            number <- rand.Next(0, int (base_num))
            strnum <- strnum + string (number)
    strnum


let pastry (numNodes, numRequests) =
    let actorList =
        [| for i = 0 to numNodes - 1 do
            let newactorNumber = return_number ()
            if i%100 = 0 then
                printfn "%d actor" i
            globalactornum.Add(newactorNumber)
            worker (newactorNumber, newactorNumber)
            System.Threading.Thread.Sleep(5)
            
             |]
    for act in globalactornum do
        for j in globalactornum do
            if j <> act then
                if jointimes%100 = 0 then
                    printfn "jointimes=%d" jointimes
                select (path + act) system <! j

    while jointimes < needjointimes do
        fake <- -1


    printfn "program running, please wait"
    for i in globalactornum do
        s<-return_list(i)
        select (path + i) system <! s

    while finish_num < numRequests * numNodes do
        fake<-1
            
boss ("boss")



let stopWatch = System.Diagnostics.Stopwatch.StartNew()

pastry (numNodes, numRequests)

let ans =
    float (total_hop) / float (numNodes * numRequests)

printfn "time:%f" stopWatch.Elapsed.TotalMilliseconds
printfn "answer:%f" ans

system.Terminate().Wait()
