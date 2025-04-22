namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkoutManagement.InitDB();
            Menu.BeginLoop();

            Console.WriteLine("\nGoodbye.");
        }
    }
}
