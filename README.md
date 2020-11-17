# DOSP_project
DOSP class project at UF in 2020 Fall, using F#

## Project 1
The goal of this first project is to use F# and the actor model to build a good solution to this problem that runs well on multi-core machines.

## Project 2
The goal of this project is to determine the convergence of **Gossip algorithm for information propagation** and **Push-sum algorithm for sum computation** through a simulator based on actors written in F#.

**Input:** The input provided (as command line to your project2) will be of the form:

project2 numNodes topology algorithm

Where numNodes is the number of actors involved (for 2D based topologies you can round up until you get a square), topology is one of full, 2D, line, imp2D, algorithm is one of gossip, push-sum.

**Output:** Print the amount of time it took to achieve convergence of the algorithm.
