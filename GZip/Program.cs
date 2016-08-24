using System;
using System.Diagnostics;
using System.Threading;

namespace GZipTest
{
    public sealed class Program
    {
        private const string COMPRESS = "compress";
        private const string DECOMPRESS = "decompress";
        private const int ERROR_CODE = 1;

        static void Main(string[] args)
        {
            HandleControlC();
            Console.WriteLine("Press CTRL+C to exit.");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            if (!IsEnoughParameters(args)) 
                Environment.Exit(ERROR_CODE);
            string command = args[0].ToLower();
            switch (command) {
                case COMPRESS:
                    Compress(args[1], args[2]);
                    break;
                case DECOMPRESS:
                    Decompress(args[1], args[2]);
                    break;
                default:
                    Console.WriteLine("Error! Unknown command.");
                    PrintHelpInformation();
                    Environment.Exit(ERROR_CODE);
                    break;
            }
            timer.Stop();
            Console.WriteLine("Time passed: " + timer.Elapsed);
        }

        private static void HandleControlC()
        {
            Thread background = new Thread(() =>
            {
                Console.TreatControlCAsInput = true;
                int waitTime = 25;
                do {
                    while (!Console.KeyAvailable) {
                        Thread.Sleep(waitTime);
                    }
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                    if ((consoleKeyInfo.Key == ConsoleKey.C) && (ConsoleModifiers.Control != 0)) {
                        Environment.Exit(ERROR_CODE);
                    }
                } while (true);
            });
            background.IsBackground = true;
            background.Start();
        }

        private static bool IsEnoughParameters(string[] args)
        {
            if (args.Length < 3) {
                Console.WriteLine("Error! Use following commands:");
                PrintHelpInformation();
                return false;
            }
            return true;
        }

        private static void PrintHelpInformation()
        {
            Console.WriteLine("To compress use: GZipTest.exe compress [filename] [archive name]");
            Console.WriteLine("To decompress use: GZipTest.exe decompress [archive name] [filename]");
        }

        private static void Compress(string fileToCompress, string resultArchiveName)
        {
            new GZip().Compress(fileToCompress, resultArchiveName);
        }

        private static void Decompress(string archiveToDecompress, string resultFileName)
        {
            new GZip().Decompress(archiveToDecompress, resultFileName);
        }
    }
}
