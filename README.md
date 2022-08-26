# Threading Practice

This program simulate processing lots of heavy data using multiple CPU Cores.
This program also demonstrate you how to restrict access to shared resource
(that is, multiple threads can read and write to same variable)
to only single thread at a time, so it won't clashes and make the program state
corrupted. This technique is called Thread Synchronization.

Also note at the time of writing this program, i am using .NET 6 and C# 10