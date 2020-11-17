#time "on"
//#load "Bootstrap.fsx"

#r "nuget: Akka.FSharp" 
#r "nuget: Akka.TestKit" 

open System
open System.Collections.Generic
open Akka.Actor
open Akka.Configuration
open Akka.FSharp
open Akka.TestKit

type Envelope={n:int; k:int}
let args: string array= fsi.CommandLineArgs
let N=args.[1]|>int
let k=args.[2]|>int


let deciside_size(N)=
    if N<1000000 then   
        1000
    else    
        N/1000

let size=deciside_size(N)

let slp(N)=
    if N<1000000 then
        2500
    else
        N/400

let detect(N,size)=
    if N/size >=1 then
        N/size
    else
        1

let actor_num=detect(N,size)
let system = ActorSystem.Create("Proj1")
//let list_ans = new List<int>()
let list=[0..N+k]
let list_square=[ for a in 0 .. N+k do yield (a * a) ]
let list_pace=[for a in 0.. actor_num-1 do yield (a*size)]
let rand = Random(1)



let caculate(n, k) = 
    let mutable element=int64(0)
    for i=0 to k-1 do
        //element <- element + int64(list_square.Item(n+i))
        element <- element + int64(n+i)*int64(n+i)
    let ans = sqrt(float(element))
    if int64(ans)*int64(ans) = element then
        printf "%d " n
        //list_ans.Add(list.Item(n))


type Worker(name) =
    inherit Actor()
    override x.OnReceive message =
        match box message with
        | :? Envelope as env -> 
            caculate(env.n, env.k)       
        | _ ->  failwith "unknown message"

type Boss(name) =
    inherit Actor()
    override x.OnReceive message = 
        match box message with
        | :? Envelope as env ->
            let workerActors = 
                [1..actor_num]
                |> List.map(fun id -> system.ActorOf(Props(typedefof<Worker>, [| string(id) :> obj |])))
            for i = 1 to env.n do
                let myMessage = {Envelope.n =i; Envelope.k = env.k}
                workerActors.Item(rand.Next() % actor_num) <! myMessage
        | _ -> failwith "unknown message"

let Message={Envelope.n=N; Envelope.k=k}
let The_Boss= system.ActorOf(Props(typedefof<Boss>, [| string("Boss") :> obj |]))
The_Boss <! Message

System.Threading.Thread.Sleep(slp(N))

// if list_ans.Count > 0 then
//     let mutable output = list_ans.[0]
//     for index in [0..list_ans.Count-1] do
//         if list_ans.[index]<output then
//             output <- list_ans.[index]
//     printfn "%d " output
// else 
//     printfn "No number satisfied"
   
system.Terminate().Wait()
//system.WhenTerminated.Wait()
//system.Terminate()