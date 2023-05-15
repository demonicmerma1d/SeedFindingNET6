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
        //redo all this to have a wrapper fn for CartPairs, and then have use a threadpool which adds the outputs to the queue to write to file
        private BlockingCollection<string> OutSolnsQueue = new();
        static bool IsWriting = false;
        public void SeedSearch(int MinSeed)
        {
            int MaxSeed = MinSeed + 10000; //hardcoding the interval
            CartPairs cartPairs = new(); //create instance of CartPairs
            var Solns = cartPairs.SeedRange(MinSeed, Math.Min(MaxSeed,int.MaxValue));
            foreach (var Soln in Solns) OutSolnsQueue.Add(Soln);
        }
        
        public void WriteOutput()
        {
            if (IsWriting) return;
            IsWriting = true; //blocks multiple writing tasks from happening simultaniously
            string filename = "2DayCC.txt";
            using (StreamWriter file = new(filename, append: true))
            {
                foreach (var Soln in OutSolnsQueue.GetConsumingEnumerable()) file.WriteLine(Soln);
            }
            IsWriting = false;
        }
        static void Main(string[] args)
        {

            Stopwatch stopwatch = Stopwatch.StartNew();
            var PG = new Program();
            for (int i = 0; i < 100;  i++) //214000 covers entire seedspace
            {
                ThreadPool.QueueUserWorkItem((MinSeed) => PG.SeedSearch((int)MinSeed));
                if (i % 10 == 0 && i > 0) ThreadPool.QueueUserWorkItem(WriteOutput);
            }
        }
    }
}
