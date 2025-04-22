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

        public static void BeginLoop()
        {
            PrintHelpMenu();
            string MenuSelection = "";
            while (MenuSelection != "q")
            {   
                Console.Write(">>>");
                MenuSelection = Console.ReadLine() ?? ""; // empty string if error in readline
                switch (MenuSelection)
                {
                    case "le":
                    case "logexercise":
                        Console.Write("What is the date for this exercise? Please enter in format MM/dd/yyyy: ");
                        DateTime Date = DateTime.Parse(Console.ReadLine() ?? ""); 
                        LogExercise(Date, NewSession(Date));
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

        static void LogExercise(DateTime Date, int SessionId)
        {
            string ExerciseType, ExerciseName;

            Console.Write("What type of workout is this: Cardio (c) or Weightlifting (w)?: ");
            ExerciseType = Console.ReadLine() ?? "".ToLower();

            if(ExerciseType == "cardio" || ExerciseType == "c")
            {
                ExerciseType = "cardio"; //if user entered "c", we want to store "cardio" in DB
                Console.Write("Enter name of Cardio: ");
                ExerciseName = Console.ReadLine() ?? "";

                Console.Write("Enter duration in MM:SS format: ");
                string[] DurationInput = (Console.ReadLine() ?? "").Split(":");
                int DurationSeconds = (Int32.Parse(DurationInput[0]) * 60) + Int32.Parse(DurationInput[1]);
                string ReturnString = WorkoutManagement.ExecuteSQL($@"INSERT INTO workouts (date, exercise_type, exercise_name, weight, reps, duration_seconds) 
                                                VALUES ('{ExerciseType}', '{ExerciseName}', 0, 0, {DurationSeconds}, {SessionId});", "NonQuery");
                Console.WriteLine(ReturnString);
            }
            else if(ExerciseType == "weightlifting" || ExerciseType == "w")
            {
                int Weight, Reps;
                ExerciseType = "weightlifting"; //if user entered "w", we want to store "weightlifting" in DB
                Console.Write("Enter name of Lift: ");
                ExerciseName = Console.ReadLine() ?? "";

                Console.Write("Please enter the amount of weight in pounds: ");
                Weight = Int32.Parse(Console.ReadLine() ?? "");

                Console.Write("Please enter number of reps: ");
                Reps = Int32.Parse(Console.ReadLine() ?? "");

                string ReturnString = WorkoutManagement.ExecuteSQL($@"INSERT INTO workouts (exercise_type, exercise_name, weight, reps, duration_seconds, session_id) 
                                                VALUES ('{ExerciseType}', '{ExerciseName}', {Weight}, {Reps}, 0, {SessionId});", "NonQuery");
                Console.WriteLine(ReturnString);

            }
        }
        
        static int NewSession(DateTime DateInput)
        {
            WorkoutManagement.ExecuteSQL($@"INSERT INTO sessions (date) 
                                                VALUES ('{DateInput}');", "NonQuery");
            
            string SessionId = WorkoutManagement.ExecuteSQL($@"SELECT MAX(session_id) FROM sessions;", "Query"); //assuming only one user
            Console.WriteLine($"New Session ID Created: {SessionId}");
            return Int32.Parse(SessionId);
        }
        static void LogSession()
        {
            Console.Clear();

            Console.WriteLine("========== Log Session ==========");
            Console.Write("Please enter the date in format MM/dd/yyyy for this session: ");
            DateTime DateInput = DateTime.Parse(Console.ReadLine() ?? "");
            Console.Write("How many exercises are in this session? (INTEGER): ");
            int NumExercises = Int32.Parse(Console.ReadLine() ?? "");
            int SessionId = NewSession(DateInput);
            
            for(int i = 1; i < NumExercises; i++)
            {
                Console.WriteLine($"\n====Exercise #{i}====");
                LogExercise(DateInput, SessionId);
            }
        }

        static public void ListAllWorkouts()
        {
            string ReturnString = WorkoutManagement.ExecuteSQL("SELECT * FROM workouts", "Query");
            Console.WriteLine(ReturnString);
            Console.ReadLine();
        }
    }
}