using System;
using System.Threading;

namespace Workshop6
{

    class Program
    {
        static int _nb_thread_in_progress = 0;

        static void FctA(string name)
        {
            Console.WriteLine("Thread {0} is at the start of FctA : {1}",
                              name, ++_nb_thread_in_progress);

            Thread.Sleep(30);

            Console.WriteLine("Thread {0} is at the End of FctA : {1}",
                              name, --_nb_thread_in_progress);
        }

        static void Main()
        {
            for (int i = 1; i <= 300; i++)
            {
                string threadName = "Thread_" + i;

                Thread t = new Thread(() => FctA(threadName));
                t.Start();

                Thread.Sleep(10); 
            }

            Console.WriteLine("Fin");
            Console.ReadLine();
        }
    }

}
