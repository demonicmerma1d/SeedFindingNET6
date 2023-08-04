using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Concurrent;

namespace SeedFindingNET6
{
    class Program
    {

        static double CartPairSearch(int startSeed, int numSeeds, int BlockSize)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var bag = new ConcurrentBag<string>();
            var partitioner = Partitioner.Create(startSeed, numSeeds, BlockSize); 
            Parallel.ForEach(partitioner, (range, loopState) =>
            {
                CartPairs CartPair = new();
                double startTime = stopwatch.Elapsed.TotalSeconds;
                //Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}: {range.Item1} -> {range.Item2}");
                for (int seed = range.Item1; seed < range.Item2; seed++)
                {
                    var Solns = CartPair.EvaluatePairs(seed);
                    foreach (string soln in Solns)
                    {
                        bag.Add(soln);
                        Console.WriteLine(soln);
                    }
                    //Console.WriteLine(seed);
                }
                double elapsed = stopwatch.Elapsed.TotalSeconds - startTime;
                Console.WriteLine($"{elapsed}: {range.Item1} -> {range.Item2}");
            });
            double seconds = stopwatch.Elapsed.TotalSeconds;
            Console.WriteLine($"Found: {bag.Count} sols in {seconds:F2} s");
            var items = bag.ToList();
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
            using (StreamWriter file = new($"2DayCC_{startSeed}.txt", append: true))
            {
                foreach (var item in items) file.WriteLine(item);
            }
            return seconds;
        }
        static void HasItemSearch(int MinSeed, int MaxSeed,int BlockSize)
        {
            HashSet<int> Items = new()
            {
                283,416,418
            };
            Stopwatch stopwatch = Stopwatch.StartNew();
            CartPairs cartPairs = new();
            var bag = new ConcurrentBag<int>();
            var partitioner = Partitioner.Create(MinSeed, MaxSeed,BlockSize);
            Parallel.ForEach(partitioner, (range, loopState) =>
            {
                for (int seed = range.Item1;seed < range.Item2; seed++)
                {
                    int toAdd = cartPairs.DoesCartHaveItems(Items, seed);
                    if (toAdd > 0) bag.Add(toAdd);
                }
            });
            double seconds = stopwatch.Elapsed.TotalSeconds;
            Console.WriteLine("End");
            foreach (var item in bag) Console.WriteLine(item);
            Console.WriteLine(seconds.ToString());
        }
        static void Main(string[] args)
        {
            var seconds = CartPairSearch(0,1<<21, 1<<16);
            Console.WriteLine(seconds);
            Console.ReadLine();
            using StreamWriter file = new("2DayCC.txt", append: true); file.WriteLine(seconds);
            //HasItemSearch(0, 1<<31 -1, 20000);
        }
    }
}
