#r "nuget: Akka.FSharp"

open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open System.Collections.Generic
open System
// open Math

let args: string array = fsi.CommandLineArgs
let numNodes = args.[1] |> int
let topology = args.[2] |> string
let algorithm = args.[3] |> string

let system = ActorSystem.Create("a1")
let path = "akka://a1/user/"
let roumor = "roumor"

// let numNodes = 10
let stopNum = 10
let count = Array.create numNodes 0
let tempCount = Array.create numNodes -1
// let topologies = "Imperfect_2D"
let isFinish = Array.create numNodes false
let mutable allFinish = false
let mutable round = 0
let mutable roundFinish = false
let devideSave = Array.create 3 0.0
let devide= Array.create numNodes 0.0
for i=0 to numNodes-1 do
    devide.[i] <- float(i)


type Message = struct
    val mutable s: float
    val mutable w: float 
    new(s:float, w:float) ={s = s; w = w}
end

let worker (name, actorNumber, stopNum) =
    spawn system name <| fun mailbox ->
        let neighbors = new List<int>()
        let col = int (ceil (sqrt (float (numNodes))))
        if topology = "full" then
            for i = 0 to numNodes - 1 do
                if i <> actorNumber then neighbors.Add(i)

        elif topology = "line" then
            if actorNumber = 0 then neighbors.Add(1)
            elif actorNumber = numNodes - 1 then neighbors.Add(actorNumber - 1)
            else neighbors.Add(actorNumber-1)
                 neighbors.Add(actorNumber+1)


        elif topology = "2D" || topology = "imp2D" then
            if actorNumber - col >= 0 && actorNumber - col < numNodes then
                neighbors.Add(actorNumber - col)
            if actorNumber % col <>0 then
                neighbors.Add(actorNumber - 1)
            if actorNumber + 1 % col <> 0 && actorNumber <> numNodes-1 then
                neighbors.Add(actorNumber + 1)
            if actorNumber + col >= 0 && actorNumber + col < numNodes then
                neighbors.Add(actorNumber + col)


            if topology = "imp2D" then
                let rand = System.Random()
                let mutable number = rand.Next(0, numNodes)
                while neighbors.Count < numNodes-1 && (neighbors.Contains(number) || number = actorNumber) do
                    number <- rand.Next(0, numNodes)

                neighbors.Add(number)



        let send_gossip (actorNumber) =
            let random = System.Random()
            let receiverNum = random.Next(neighbors.Count)

            let receiver =
                select (path + (neighbors.[receiverNum]).ToString()) system


            receiver <! roumor
            // printfn "send_gossip from %d to %d" actorNumber neighbors.[receiverNum]
        // for i=0 to neighbors.Count-1 do
        //     let receiver = select (path+i.ToString()) system
        //     receiver <! roumor


        let mutable m = Message(float(actorNumber), w=1.0)

        let send_push_sum =
            let random = System.Random()
            let receiverNum = random.Next(neighbors.Count)

            let receiver =
                select (path + (neighbors.[receiverNum]).ToString()) system

            m.s <- m.s/2.0
            m.w <- m.w/2.0

            receiver <! m
            // printfn "send from %d to %d" actorNumber receiverNum
        


        let rec loop () =
            actor {
                let! msg = mailbox.Receive()
                match box msg with
                    | :? string as myMessage ->
                        if myMessage = roumor then
                            if count.[actorNumber] < stopNum then
                                count.[actorNumber] <- count.[actorNumber] + 1
                                // printfn "%d receive %d" actorNumber count.[actorNumber]
                            if count.[actorNumber] >= stopNum then
                                isFinish.[actorNumber] <- true
                        else if myMessage = "GossipTellSelfSend" then
                            if not isFinish.[actorNumber] then
                                send_gossip (actorNumber)
                        else if myMessage = "PushSumTellSelfSend" then
                            send_push_sum
                        // if myMessage = roumor then
                        //     if count.[actorNumber] = 0 then
                        //         select (path + actorNumber.ToString()) system <! "tellselfsend"
                        //     count.[actorNumber] <- count.[actorNumber] + 1
                        //     // printfn "%d receive %d" actorNumber count.[actorNumber]
                        //     if count.[actorNumber] >= stopNum then
                        //         isFinish.[actorNumber] <- true
                        // else if myMessage = "tellselfsend" then
                        //     if not isFinish.[actorNumber] then
                        //         send_gossip (actorNumber)
                        //         select (path + actorNumber.ToString()) system <! "tellselfsend"
                    | :? Message as myMessage ->
                        m.s <- myMessage.s+m.s
                        m.w <- myMessage.w+m.w
                        devide.[actorNumber] <- m.s/m.w
                    | _-> printfn "error"
                if count.[actorNumber] < stopNum then
                    send_gossip (actorNumber)

                return! loop ()
            }
        loop ()




let gossip (topology, numNodes, stopNum) =
    let actorList =
        [| for i = 0 to numNodes - 1 do
            worker (i.ToString(), i, stopNum) |]

    select (path + "0") system <! roumor
    while (not allFinish) do
        let mutable finishNum = 0
        for i = 0 to numNodes - 1 do
            if count.[i]>0 && count.[i]<stopNum then
                select (path + i.ToString()) system <! "GossipTellSelfSend"
            if isFinish.[i] then finishNum <- finishNum + 1
        if finishNum = numNodes then allFinish <- true
        else if count = tempCount then allFinish <- true
        for i=0 to numNodes-1 do
            tempCount.[i] <- count.[i]


    for i in 0 .. numNodes - 1 do
        // printfn "actor%d : %d" i count.[i]
        system.Terminate().Wait()

let pushSum (topology, numNodes, stopNum) =
    let actorList =
        [| for i = 0 to numNodes - 1 do
            worker (i.ToString(), i, stopNum) |]

    while (not roundFinish) do
        let mutable sum = 0.0
        for i = 0 to numNodes - 1 do
            select (path + i.ToString()) system <! "PushSumTellSelfSend"
        for i=0 to numNodes - 1 do
            sum <- sum + devide.[i]
        if round>2 && abs(sum-devideSave.[round%3])<10e-10 then
            roundFinish <- true
        devideSave.[round%3] <- sum
        sum <- 0.0
        round <- round + 1


let stopWatch = System.Diagnostics.Stopwatch.StartNew()

if algorithm = "gossip" then
    gossip (topology, numNodes, stopNum)
else if algorithm = "push-sum" then
    pushSum (topology, numNodes, stopNum)

stopWatch.Stop()
printfn "%f" stopWatch.Elapsed.TotalMilliseconds

system.Terminate().Wait()
