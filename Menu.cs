using System.Security.Principal;
using Microsoft.VisualBasic;

namespace ConsoleApp2
{
    class Menu
    {
        static void PrintHelpMenu()
        {
            Console.WriteLine("========== Help Menu ==========");
            Console.WriteLine("Please select a menu option:");
            Console.WriteLine("logexercise (le) - Log a single exercise (cardio or lift).");
            Console.WriteLine("logsession (ls) - Log an entire session with multiple exercises.");
            Console.WriteLine();
            Console.WriteLine("listall (la) - List all workouts in the DB.");
            Console.WriteLine();
            Console.WriteLine("\nquit (q) - Quit the app");
        }

        public static string GetStringInput(string InputType) // WITH INPUT VALIDATION YAY
        {
            while(true) // keep going til valid input
                try
                {
                    Console.Write(">>>");
                    string Input = Console.ReadLine() ?? "".ToLower();
                    if(InputType == "time")
                    {
                        string[] parts = Input.Split(':'); // make sure right format
                        if (parts.Length != 2)
                            throw new FormatException();
                        else
                            return Input;
                    }
                    else
                    {
                        return Input;
                    }
                }
                catch(FormatException)
                {  
                    if(InputType == "time")
                    {
                        Console.WriteLine("Invalid input. Please enter in MM:SS format.");
                    }
                    else
                        Console.WriteLine("Invalid input, please try again.");
                }
        }

        public static int GetIntInput() // WITH INPUT VALIDATION YAY
        {
            while(true) // keep going til valid input
                try
                {
                    Console.Write(">>>");
                    int Input = Int32.Parse(Console.ReadLine() ?? "");
                    return Input;
                }
                catch(FormatException)
                {  
                    Console.WriteLine("Invalid input, please try again.");
                }
        }

        public static DateTime GetDateInput() // WITH INPUT VALIDATION YAY
        {
            while(true) // keep going til valid input (which returns)
                try
                {
                    Console.Write(">>>");
                    DateTime input = DateTime.Parse(Console.ReadLine() ?? "");
                    return input;
                }
                catch(FormatException)
                {
                    Console.WriteLine("Invalid input, please enter date in format MM/dd/yyyy.");
                }
        }
        public static void BeginLoop()
        {
            string MenuSelection = "";
            while (MenuSelection != "q")
            {   
                PrintHelpMenu();
                MenuSelection = GetStringInput("menu");
                switch (MenuSelection)
                {
                    case "le":
                    case "logexercise":
                        Console.WriteLine("What is the date for this exercise? Please enter in format MM/dd/yyyy:");
                        DateTime Date = GetDateInput();
                        LogExercise(NewSession(Date));
                        break;
                    case "ls":
                    case "logsession":
                        LogSession(); // returns WorkoutDate if success
                        break;
                    case "la":
                    case "listall":
                        ListAllWorkouts();
                        //Console.WriteLine("Would you like to modify a workout?");
                        break;
                    case "q":
                    case "quit":
                        return; // exit Program
                    default: // invalid input
                        Console.WriteLine($"You entered: '{MenuSelection}'. Please select a valid command.");
                        break; // return to help menu
                }
            }
        }

        static void LogExercise(int SessionId)
        {
            string ExerciseType, ExerciseName;

            Console.WriteLine("What type of workout is this: Cardio (c) or Weightlifting (w)?:");
            ExerciseType = GetStringInput("");

            if(ExerciseType == "cardio" || ExerciseType == "c")
            {
                ExerciseType = "cardio"; //if user entered "c", we want to store "cardio" in DB
                Console.WriteLine("Enter name of Cardio: ");
                ExerciseName = GetStringInput("");

                Console.WriteLine("Enter duration in MM:SS format:");
                string[] DurationInput = GetStringInput("time").Split(":");
                int DurationSeconds = (Int32.Parse(DurationInput[0]) * 60) + Int32.Parse(DurationInput[1]);
                
                WorkoutManagement.InsertWorkout(ExerciseType, ExerciseName, 0, 0, DurationSeconds, SessionId);
            }
            else if(ExerciseType == "weightlifting" || ExerciseType == "w")
            {
                int Weight, Reps;
                ExerciseType = "weightlifting"; //if user entered "w", we want to store "weightlifting" in DB
                Console.WriteLine("Enter name of Lift:");
                ExerciseName = GetStringInput("");

                Console.WriteLine("Please enter the amount of weight in pounds:");
                Weight = GetIntInput();

                Console.WriteLine("Please enter number of reps:");
                Reps = GetIntInput();

                WorkoutManagement.InsertWorkout(ExerciseType, ExerciseName, Weight, Reps, 0, SessionId);
            }
        }
        
        static int NewSession(DateTime DateInput)
        {
            WorkoutManagement.InsertSession(DateInput);
            
            int SessionId = WorkoutManagement.GetNewestSessionId(); //assuming only one user
            Console.WriteLine($"New Session ID Created: {SessionId}");
            return SessionId;
        }
        static void LogSession()
        {
            Console.Clear();

            Console.WriteLine("========== Log Session ==========");
            Console.WriteLine("Please enter the date in format MM/dd/yyyy for this session: ");
            DateTime DateInput = GetDateInput();
            Console.WriteLine("How many exercises are in this session? (INTEGER): ");
            int NumExercises = GetIntInput();
            int SessionId = NewSession(DateInput);
            
            for(int i = 1; i <= NumExercises; i++)
            {
                Console.WriteLine($"\n====Exercise #{i}====");
                LogExercise(SessionId);
            }
        }

        static public void ListAllWorkouts()
        {
            string ReturnString = WorkoutManagement.GetAllWorkouts();
            Console.WriteLine(ReturnString);
            Console.ReadLine();
        }
    }
}