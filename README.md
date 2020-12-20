# DOSP_project
DOSP(Distributed Operating System Principles) class project at UF in 2020 Fall, using F# and Akka.

## Project 1
The goal of this first project is to use F# and the actor model to build a good solution to this problem that runs well on multi-core machines.

**Input:** The input provided (as command line to your program, e.g. my app)
will be two numbers: N and k. The overall goal of your program is to find all
k consecutive numbers starting at 1 and up to N, such that the sum of squares
is itself a perfect square (square of an integer).

**Output:** Print, on independent lines, the first number in the sequence for each solution.

Example 1:  
dotnet fsi proj1.fsx 3 2  
3

## Project 2
The goal of this project is to determine the convergence of **Gossip algorithm for information propagation** and **Push-sum algorithm for sum computation** through a simulator based on actors written in F#.

**Input:** The input provided (as command line to your project2) will be of the form:

project2 numNodes topology algorithm

Where numNodes is the number of actors involved (for 2D based topologies you can round up until you get a square), topology is one of full, 2D, line, imp2D, algorithm is one of gossip, push-sum.

**Output:** Print the amount of time it took to achieve convergence of the algorithm.

## Project 3
The goal of this project is to implement in F# using the actor model the **Pastry protocol** and a simple object access service to prove its usefulness. The specification of the Pastry protocol can be found in the paper **Pastry: Scalable, decentralized object location and routing for large-scale peer-to-peer systems.by A. Rowstron and P. Druschel(http://rowstron.azurewebsites.net/PAST/pastry.pdf)**. The network join and routing as described in the Pastry paper were implemented by the code.

**Input:** The input provided (as command line to yourproject3) will be of the form:

project3 numNodes numRequests

Where numNodesis the number of peers to be created in the peer to peer system and numRequests the number of requests each peer has to make. When all peers performed that many requests, the program can exit. Each peer should send a request/second.

**Output:** Print the average number of hops (node connections) that have to be traversed to deliver a message.

## Project 4.1
In this project, Twitter Clone and a client tester/simulator were implemented.

1. Implement a Twitter like engine with the following functionality:
   -Register account
   -Send tweet. Tweets can have hashtags (e.g. #COP5615isgreat) and mentions (@bestuser)
   -Subscribe to user's tweets
   -Re-tweets (so that your subscribers get an interesting tweet you got by other means)
   -Allow querying tweets subscribed to, tweets with specific hashtags, tweets in which the user is mentioned (my mentions)
   -If the user is connected, deliver the above types of tweets live (without querying)
   
2. Implement a tester/simulator to test the above
   -Simulate as many users as possible
   -Simulate periods of live connection and disconnection for users
   -Simulate a Zipf distribution on the number of subscribers. For accounts with a lot of subscribers, increase the number of tweets. Make some of these messages re-tweets
   
3. Other considerations:
   -The client part (send/receive tweets) and the engine (distribute tweets) are in separate processes.
