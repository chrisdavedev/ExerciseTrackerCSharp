using System.Globalization;
using Microsoft.Data.Sqlite;

namespace ConsoleApp2
{
    static class WorkoutManagement
    {
        public static void InsertWorkout(string ExerciseType, string ExerciseName, int Weight, int Reps, int DurationSeconds, int SessionId)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@type", ExerciseType},
                {"@name", ExerciseName},
                {"@weight", Weight},
                {"@reps", Reps},
                {"@duration", DurationSeconds},
                {"@session", SessionId}
            };

            ExecuteSQL(@"
                INSERT INTO workouts (exercise_type, exercise_name, weight, reps, duration_seconds, session_id)
                VALUES (@type, @name, @weight, @reps, @duration, @session);",
                "NonQuery", 
                parameters);
        }

        public static void InsertSession(DateTime Date)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@date", Date },
            };
            Console.WriteLine("Calling ExecuteSQL()");
            ExecuteSQL($@"INSERT INTO sessions (date) VALUES (@date);", "NonQuery", parameters);
        }

        public static int GetNewestSessionId()
        {
            string result = ExecuteSQL(
                "SELECT MAX(session_id) FROM sessions;",
                "Query",
                new Dictionary<string, object>());

            if (int.TryParse(result.Trim(), out int id))
                return id;

            throw new Exception("No sessions found.");
        }
        
        public static string GetAllWorkouts()
        {
                //  "SELECT * FROM workouts", "Query"
                return ExecuteSQL("SELECT * FROM workouts", "Query", new Dictionary<string, object>{});
        }
        static public string ExecuteSQL(string SqlStatement, string CommandType, Dictionary<string, object>parameters)
        {
            try
            {
                using (var Connection = new SqliteConnection("Data Source=test.db"))
                {
                    Connection.Open();

                    var Command = Connection.CreateCommand();
                    Command.CommandText = SqlStatement;
                    foreach (var param in parameters) // YAY FOR NO SQL INJECTION
                    {
                        Command.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    if (CommandType == "Query")
                        using (var Reader = Command.ExecuteReader())
                        {
                            var Result = "";
                            int RowCount = 0;
                            while (Reader.Read())
                            {
                                string Row = "";
                                for (int i = 0; i < Reader.FieldCount; i++)
                                {
                                    Row += $"{Reader[i]} "; // Append all column values in the row
                                }
                                Result += Row.TrimEnd() + "\n"; // add all rows to one output
                                RowCount++;
                            }
                            return Result;
                        }
                    else if (CommandType == "NonQuery")
                    {
                        int RowsAffected = Command.ExecuteNonQuery();
                        return $"Successfully Executed '{SqlStatement}.' \nAffected {RowsAffected} rows.";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error executing statement:{SqlStatement}\nWith Exception: {ex.Message}";
            }
            return $"Unexpected error while executing statement:\n{SqlStatement}"; // need this or compilation error
        }

        public static void InitDB()
        {
            if (!File.Exists("test.db"))
            {
                string ReturnValue = WorkoutManagement.ExecuteSQL(
                    @"
                    CREATE TABLE workouts (
                        workout_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        session_id INTEGER NOT NULL,
                        exercise_type TEXT NOT NULL,
                        exercise_name TEXT NOT NULL,
                        weight INTEGER,
                        reps INTEGER,
                        duration_seconds INTEGER,
                        FOREIGN KEY (session_id) REFERENCES sessions(session_id)
                    );
                    ",
                    "NonQuery",
                    new Dictionary<string, object>{}); // absence of .db file creates one when SQL statement called
                Console.WriteLine(ReturnValue);

                ReturnValue = WorkoutManagement.ExecuteSQL(
                    @"
                    CREATE TABLE sessions (
                        session_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        date DATE NOT NULL
                    );
                    ",
                    "NonQuery",
                    new Dictionary<string, object>{}); // absence of .db file creates one when SQL statement called);
                Console.WriteLine(ReturnValue);
                Thread.Sleep(3000);
            }
        }
    }
}
