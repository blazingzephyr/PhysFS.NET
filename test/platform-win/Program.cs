using System.Text;

namespace Icculus.PhysFS.NET;

internal class Program
{
    static void Main()
    {
        Console.Write(PhysFsTest.OutputArchivers());

        do
        {
            Console.WriteLine("Enter commands. Enter \"help\" for instructions.");
            Console.WriteLine("Press Enter to exit.");
            Console.Write("> ");

            string? line = Console.ReadLine();
            if (string.IsNullOrEmpty(line)) return;

            string? commandResult = PhysFsTest.ProcessCommand(line);
            if (commandResult == null) return;

            Console.WriteLine(commandResult);
            Console.WriteLine();
        }
        while (true);
    }
}
