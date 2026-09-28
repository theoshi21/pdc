using System;
using System.Threading;

namespace PDC_Midterm_Assignment
{
    internal class Program
    {
        // =========================================================
        // 3. SHARED DATA
        // =========================================================
        static int counter = 0;


        // =========================================================
        // 4. CONDITION VARIABLE
        // =========================================================

        static readonly object conditionLock = new object();

        // The condition that the waiting thread is checking
        static bool ready = false;


        // =========================================================
        // 5. BARRIER
        // =========================================================

        // Three threads must arrive before any of them can continue
        static Barrier barrier = new Barrier(3);


        // =========================================================
        // 6. LIVENESS PROBLEMS
        // =========================================================

        // Locks used to demonstrate a deadlock situation
        static readonly object lock1 = new object();
        static readonly object lock2 = new object();


        // Variables used for the livelock demonstration
        static bool person1 = false;
        static bool person2 = false;

        // Lock used to safely access the livelock variables
        static readonly object livelockLock = new object();


        // Variables used for the starvation demonstration
        static readonly object starvationLock = new object();
        static bool stopStarvation = false;


        static void Main(string[] args)
        {
            //REQUIREMENT 1: CREATING THREAD

            Console.WriteLine("========================================");
            Console.WriteLine("1. THREAD CREATION");
            Console.WriteLine("========================================");

            // Creates a new thread that will run Hello()
            Thread thread = new Thread(Hello);

            // Starts the thread
            thread.Start();

            // Waits for the thread to finish
            thread.Join();

            Console.WriteLine();
            Console.ReadLine();

            //REQUIREMENT 2: JOIN AND DETACH

            Console.WriteLine("========================================");
            Console.WriteLine("2. JOIN AND DETACH");
            Console.WriteLine("========================================");

            //2.1: JOIN 

            Console.WriteLine("Starting joined thread...");

            Thread joinedThread = new Thread(JoinedWork);

            // Start the thread
            joinedThread.Start();

            // Main waits for the thread to finish
            joinedThread.Join();

            Console.WriteLine("Main continued after Join().");
            Console.WriteLine();


            //2.2: DETACH. NO DETACH IN C#, BUT FOR C# WE CAN USE BACKGROUND THREADS TO SIMULATE DETACHED THREADS

            Console.WriteLine("Starting detached/background thread...");

            Thread detachedThread = new Thread(DetachedWork);

            // In C#, a background thread is used to simulate pthread.detach()

            detachedThread.IsBackground = true;

            // Start the background thread
            detachedThread.Start();

            // Main does NOT call Join() on this thread.
            Console.WriteLine("Main does not wait for the detached thread.");
            Console.WriteLine();


            //REQUIREMENT 3: SHARTED DATA

            Console.WriteLine("========================================");
            Console.WriteLine("3. SHARED DATA / COUNTER");
            Console.WriteLine("========================================");

            // Initialize or Reset the shared counter
            counter = 0;

            // Create the two thread who will intreact with the shared varaible.
            Thread t1 = new Thread(IncrementCounter);
            Thread t2 = new Thread(IncrementCounter);

            // Start the two threads
            t1.Start();
            t2.Start();

            // Wait for both threads to finish
            t1.Join();
            t2.Join();

            Console.WriteLine("Expected counter: 20000");
            Console.WriteLine("Actual counter:   " + counter);
            Console.WriteLine();
            Console.ReadLine();


            //REQUIREMENT 4: Condition Variables

            Console.WriteLine("========================================");
            Console.WriteLine("4. CONDITION VARIABLE");
            Console.WriteLine("========================================");

            // Reset the condition
            ready = false;

            // Creates the thread that will wait for the condition
            Thread waiter = new Thread(WaitForCondition);

            // The thread that will change the condition
            Thread signaler = new Thread(SetCondition);

            // Start the two threads
            waiter.Start();
            signaler.Start();

            // Wait for both threads to finish
            waiter.Join();
            signaler.Join();

            Console.WriteLine();
            Console.ReadLine();


           //REQUIREMENT #5: Barrier

            Console.WriteLine("========================================");
            Console.WriteLine("5. BARRIER");
            Console.WriteLine("========================================");

            // Creates three threads
            Thread b1 = new Thread(Work1);
            Thread b2 = new Thread(Work2);
            Thread b3 = new Thread(Work3);

            // Start all three threads
            b1.Start();
            b2.Start();
            b3.Start();

            // Wait for all three threads to finish
            b1.Join();
            b2.Join();
            b3.Join();

            Console.WriteLine();
            Console.ReadLine();

            // =====================================================
            // 6. LIVENESS PROBLEMS
            // =====================================================

            Console.WriteLine("========================================");
            Console.WriteLine("6. LIVENESS PROBLEMS");
            Console.WriteLine("========================================");


            // =====================================================
            // 6A. DEADLOCK
            // =====================================================

            Console.WriteLine();
            Console.WriteLine("--- DEADLOCK ---");

            // Creates two threads that attempt to acquire
            // the locks in opposite orders.
            Thread d1 = new Thread(Deadlock1);
            Thread d2 = new Thread(Deadlock2);

            // Start both threads
            d1.Start();
            d2.Start();

            // Wait for both threads.
            //
            // The methods use TryEnter with a timeout so that
            // the simulation can detect the deadlock situation
            // without permanently freezing the program.
            d1.Join();
            d2.Join();


            // =====================================================
            // 6B. LIVELOCK
            // =====================================================

            Console.WriteLine();
            Console.WriteLine("--- LIVELOCK ---");

            // Reset livelock state
            person1 = false;
            person2 = false;

            Thread l1 = new Thread(Livelock1);
            Thread l2 = new Thread(Livelock2);

            // Start both threads
            l1.Start();
            l2.Start();

            // Wait for both threads
            l1.Join();
            l2.Join();


            // =====================================================
            // 6C. STARVATION
            // =====================================================

            Console.WriteLine();
            Console.WriteLine("--- STARVATION ---");

            // Reset starvation state
            stopStarvation = false;

            // Creates several threads that repeatedly access
            // the shared resource.
            Thread s1 = new Thread(() => GreedyThread(1));
            Thread s2 = new Thread(() => GreedyThread(2));
            Thread s3 = new Thread(() => GreedyThread(3));

            // Creates the thread that may have difficulty
            // getting access to the resource.
            Thread starving = new Thread(StarvingThread);

            // Start the greedy threads
            s1.Start();
            s2.Start();
            s3.Start();

            // Give the greedy threads a chance to start
            Thread.Sleep(100);

            // Start the starving thread
            starving.Start();

            // Wait for the starving thread to finish
            starving.Join();

            // Tell the greedy threads to stop
            stopStarvation = true;

            // Wait for all greedy threads
            s1.Join();
            s2.Join();
            s3.Join();


            // =====================================================
            // END
            // =====================================================

            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("ALL SIMULATIONS COMPLETE");
            Console.WriteLine("========================================");

            // Gives the detached/background thread a chance
            // to finish before the application exits.
            Thread.Sleep(1000);

            Console.WriteLine("Done.");
        }


        //REQUIREMENT 1: Thread Creation

        static void Hello()
        {
            Console.WriteLine("Hello from thread.");
        }


        //REQUIREMENT 2: Join
        static void JoinedWork()
        {
            Console.WriteLine("Joined thread is running...");

            Thread.Sleep(500);

            Console.WriteLine("Joined thread finished.");
        }


        //REQUIREMENT 2.2: Detach
        static void DetachedWork()
        {
            Thread.Sleep(500);

            Console.WriteLine("Detached/background thread finished.");
        }


        //REQUIREMENT 3: SHARED DATA
        static void IncrementCounter()
        {
            // Each thread attempts to increment the shared
            // counter 10,000 times.
            for (int i = 0; i < 10000; i++)
            {
                // Read the current value
                int temp = counter;

                // Gives another thread an opportunity to run.
                // This makes the race condition easier to observe.
                Thread.Yield();

                // Write the updated value
                counter = temp + 1;
            }
        }


        //REQUIREMENT 4: Condition Variable

        static void WaitForCondition()
        {
            // Acquire the lock before checking the condition
            lock (conditionLock)
            {
                Console.WriteLine(
                    "Worker: waiting for the condition..."
                );

                // Continue checking the condition in a loop.
                //
                // Monitor.Wait() temporarily releases the lock
                // and puts the thread to sleep.
                while (!ready)
                {
                    Monitor.Wait(conditionLock);
                }

                // This executes once the condition becomes true.
                Console.WriteLine(
                    "Worker: condition is true. Continuing..."
                );
            }
        }


        static void SetCondition()
        {
            // Simulates another thread doing some work
            Thread.Sleep(1000);

            // Acquire the same lock
            lock (conditionLock)
            {
                // Change the shared condition
                ready = true;

                Console.WriteLine(
                    "Signaler: condition is now true."
                );

                // Wake the thread waiting for the condition
                Monitor.Pulse(conditionLock);

                Console.WriteLine(
                    "Signaler: waiting thread has been notified."
                );
            }
        }


        //REQUIREMENT 5: Barrier
        static void Work1()
        {
            Console.WriteLine("Thread 1: doing phase 1 work...");

            // Thread 1 finishes first
            Thread.Sleep(500);

            Console.WriteLine("Thread 1: waiting at the barrier...");

            // Wait for all three threads
            barrier.SignalAndWait();

            // Only continues once all three threads arrive
            Console.WriteLine("Thread 1: starting phase 2 work!");
        }


        static void Work2()
        {
            Console.WriteLine("Thread 2: doing phase 1 work...");

            // Thread 2 takes a little longer
            Thread.Sleep(1000);

            Console.WriteLine("Thread 2: waiting at the barrier...");

            // Wait for all three threads
            barrier.SignalAndWait();

            // Continues after all three arrive
            Console.WriteLine("Thread 2: starting phase 2 work!");
        }


        static void Work3()
        {
            Console.WriteLine("Thread 3: doing phase 1 work...");

            // Thread 3 is the slowest
            Thread.Sleep(1500);

            Console.WriteLine("Thread 3: waiting at the barrier...");

            // Wait for all three threads
            barrier.SignalAndWait();

            // Continues after all three arrive
            Console.WriteLine("Thread 3: starting phase 2 work!");
        }


        // =========================================================
        // 6A. DEADLOCK
        // =========================================================

        static void Deadlock1()
        {
            // Thread 1 acquires Lock 1 first
            lock (lock1)
            {
                Console.WriteLine(
                    "Thread A: acquired Lock 1."
                );

                // Give Thread B time to acquire Lock 2
                Thread.Sleep(500);

                Console.WriteLine(
                    "Thread A: trying to acquire Lock 2..."
                );

                // Try to acquire Lock 2, but only wait for
                // a limited amount of time.
                if (!Monitor.TryEnter(lock2, 1000))
                {
                    Console.WriteLine(
                        "Thread A: could not acquire Lock 2."
                    );

                    Console.WriteLine(
                        "Thread A: deadlock situation detected."
                    );

                    return;
                }

                try
                {
                    Console.WriteLine(
                        "Thread A: acquired Lock 2."
                    );
                }
                finally
                {
                    Monitor.Exit(lock2);
                }
            }
        }


        static void Deadlock2()
        {
            // Thread 2 acquires Lock 2 first
            lock (lock2)
            {
                Console.WriteLine(
                    "Thread B: acquired Lock 2."
                );

                // Give Thread A time to acquire Lock 1
                Thread.Sleep(500);

                Console.WriteLine(
                    "Thread B: trying to acquire Lock 1..."
                );

                // Try to acquire Lock 1, but only wait for
                // a limited amount of time.
                if (!Monitor.TryEnter(lock1, 1000))
                {
                    Console.WriteLine(
                        "Thread B: could not acquire Lock 1."
                    );

                    Console.WriteLine(
                        "Thread B: deadlock situation detected."
                    );

                    return;
                }

                try
                {
                    Console.WriteLine(
                        "Thread B: acquired Lock 1."
                    );
                }
                finally
                {
                    Monitor.Exit(lock1);
                }
            }
        }


        // =========================================================
        // 6B. LIVELOCK
        // =========================================================

        static void Livelock1()
        {
            // Repeats several times to demonstrate that the
            // thread is active but is not making useful progress.
            for (int i = 0; i < 10; i++)
            {
                lock (livelockLock)
                {
                    // Thread A sees that Thread B is not currently
                    // using the resource.
                    if (!person2)
                    {
                        person1 = true;

                        Console.WriteLine(
                            "Thread A: moves forward."
                        );

                        // Simulates Thread A noticing Thread B
                        // and deciding to give way.
                        Thread.Sleep(50);

                        person1 = false;

                        Console.WriteLine(
                            "Thread A: backs off."
                        );
                    }
                }

                // Gives Thread B an opportunity to react
                Thread.Yield();
            }

            Console.WriteLine(
                "Thread A: no useful progress was made."
            );
        }


        static void Livelock2()
        {
            // Repeats several times to demonstrate that the
            // thread is active but is not making useful progress.
            for (int i = 0; i < 10; i++)
            {
                lock (livelockLock)
                {
                    // Thread B sees that Thread A is not currently
                    // using the resource.
                    if (!person1)
                    {
                        person2 = true;

                        Console.WriteLine(
                            "Thread B: moves forward."
                        );

                        // Simulates Thread B noticing Thread A
                        // and deciding to give way.
                        Thread.Sleep(50);

                        person2 = false;

                        Console.WriteLine(
                            "Thread B: backs off."
                        );
                    }
                }

                // Gives Thread A an opportunity to react
                Thread.Yield();
            }

            Console.WriteLine(
                "Thread B: no useful progress was made."
            );
        }


        // =========================================================
        // 6C. STARVATION
        // =========================================================

        static void GreedyThread(object obj)
        {
            int id = (int)obj;

            // Continues running until the main thread tells it
            // to stop.
            while (!stopStarvation)
            {
                lock (starvationLock)
                {
                    Console.WriteLine(
                        "Greedy Thread " + id +
                        ": got the shared resource."
                    );

                    // Holds the resource for a short time
                    Thread.Sleep(50);
                }

                // Immediately tries again
                Thread.Yield();
            }
        }


        static void StarvingThread()
        {
            int attempts = 0;

            // Tries repeatedly to obtain the shared resource
            while (attempts < 20)
            {
                // TryEnter does not wait indefinitely.
                if (Monitor.TryEnter(starvationLock))
                {
                    try
                    {
                        Console.WriteLine(
                            "Starving Thread: finally got the resource."
                        );

                        return;
                    }
                    finally
                    {
                        Monitor.Exit(starvationLock);
                    }
                }

                // The thread failed to obtain the resource
                attempts++;

                Console.WriteLine(
                    "Starving Thread: was skipped."
                );

                // Give other threads another opportunity
                Thread.Yield();
            }

            Console.WriteLine(
                "Starving Thread: experienced starvation."
            );
        }
    }
}

