// See https://aka.ms/new-console-template for more information
using System;

//Console.WriteLine("Hello, World!");
namespace ConsoleRecipe00
{
    class Program
    {

        static void Main(string[] args)
        {
            bool endApp = false;
            if (args.Length !=2)
            {
                Console.WriteLine("RecipeWatcher Must Include Parameters: (Folder to Watch) , (IP)");
                return;
            }
            Console.WriteLine("RecipeWatcher " + args[0] +" "+ args[1]);
            FileWatcher watcher = new();
            watcher.FileWatcherStart(args[0], args[1]);
            while (!endApp)
            {
                Console.Write("V2 Press 'n' and Enter to close the app \n\r");
                if (Console.ReadLine() == "n") endApp = true;
            }
        }
    }
}

