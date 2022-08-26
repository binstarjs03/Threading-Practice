/*
 * Threading Practice - Written by Bin Starjs (binstarjs03 @ github, Bintang Jakasurya)
 * 
 * This program simulate processing lots of heavy data using multiple CPU Cores.
 * This program also demonstrate you how to restrict access to shared resource
 * (that is, multiple threads can read and write to same variable)
 * to only single thread at a time, so it won't clashes and make the program state
 * corrupted. This technique is called Thread Synchronization.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace binstarjs03.ThreadingPractice;

public class Program {
    static void Main() {
        Thread.CurrentThread.Name = "Main Thread";
        Queue<int> inputDatas = new();
        List<int> outputDatas = new();

        // Populate input queue data to be processed
        foreach (int value in Enumerable.Range(0, 50))
            inputDatas.Enqueue(value);
        ThreadPrintConsole($"Input data to be processed: {string.Join(", ", inputDatas)}.");

        // Create n cpu coure count of threads, but don't run it yet
        ThreadPrintConsole($"Creating {Environment.ProcessorCount} workers (based on your machine CPU Cores count)");
        Thread[] threads = new Thread[Environment.ProcessorCount];
        for (int i = 0; i < threads.Length; i++) {
            Thread thread = new(Work);
            thread.Name = $"Worker {i + 1}";
            threads[i] = thread;
        }

        // run all threads
        ThreadPrintConsole($"Running {Environment.ProcessorCount} workers...");
        foreach (Thread thread in threads) {
            object boxedInputAndOutputDatas = (inputDatas, outputDatas);
            thread.Start(boxedInputAndOutputDatas);
        }

        // wait until all threads finished processing data
        ThreadPrintConsole($"Waiting for {Environment.ProcessorCount} workers finished working...");
        foreach (Thread thread in threads)
            thread.Join();

        ThreadPrintConsole("All workers finished working.");

        // data may be unsorted because thread inconsistency which one is finished first processing the data
        // so we sort it to make it tidy before printing it
        outputDatas.Sort();
        ThreadPrintConsole($"Processed input data: {string.Join(", ", outputDatas)}.");
        ThreadPrintConsole($"Press any key to terminate this program.");
        Console.ReadKey();
    }

    static void ThreadPrintConsole(string content) {
        Console.WriteLine($"{Thread.CurrentThread.Name}: {content}");
    }

    static void Work(object? args) {
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        // Unbox argument and split it into two variables
        (Queue<int> inputDatas, List<int> outputDatas) = ((Queue<int>, List<int>))args;
        Random random = new();

        // Process all data in queue until queue is exhausted
        while (true) {
            int inputData;

            // Restrict access to queue to only single-thread at time
            // This is called 'Thread Synchronization'
            lock (inputDatas) {
                if (inputDatas.Count == 0)
                    break;
                inputData = inputDatas.Dequeue();
            }
            // Thread synchronization ends here

            // Simulate long-running task
            int sleepDuration = random.Next(500, 3000);
            ThreadPrintConsole($"Processing data {inputData} for {sleepDuration} ms...");
            Thread.Sleep(sleepDuration);
            int outputData = inputData * 2;

            // store and write back processed data to somewhere
            // again, synchronize thread for shared resources to avoid clashes
            lock (outputDatas)
                outputDatas.Add(outputData);
        }
        ThreadPrintConsole("Finished processing all data.");
    }
}
