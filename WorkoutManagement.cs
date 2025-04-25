using System.Globalization;
using Microsoft.Data.Sqlite;
using Serilog;

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
            Log.Verbose("Attempting to insert into workouts DB with values {@parameters}", parameters);
            ExecuteSQL(@"
                INSERT INTO workouts (exercise_type, exercise_name, weight, reps, duration_seconds, session_id)
                VALUES (@type, @name, @weight, @reps, @duration, @session);",
                "NonQuery", 
                parameters);
            Log.Verbose("Successfully inserted into workouts DB, values: {@parameters}", parameters); // wont get here if error
        }

        public static void InsertSession(DateTime Date)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@date", Date },
            };
            Log.Verbose("Attempting to insert into workouts DB with values {@parameters}", parameters);
            ExecuteSQL($@"INSERT INTO sessions (date) VALUES (@date);", "NonQuery", parameters);
            Log.Verbose("Successfully inserted into workouts DB with values {@parameters}", parameters);
        }

        public static int GetNewestSessionId()
        {
            Log.Verbose("Attempting to get the newest session ID.");
            string result = ExecuteSQL(
                "SELECT MAX(session_id) FROM sessions;",
                "Query",
                new Dictionary<string, object>());

            if (int.TryParse(result.Trim(), out int id))
                return id;

            Log.Error("No sessions found when getting newest Session ID.");
            throw new Exception("No sessions found when getting newest Session ID.");
        }
        
        public static string GetAllWorkouts()
        {
            //  "SELECT * FROM workouts", "Query"
            Log.Verbose("Attempting to get all workouts from workouts DB");
            string ReturnValue = ExecuteSQL("SELECT * FROM workouts", "Query", new Dictionary<string, object>{});
            Log.Verbose("Successfully got all workouts from DB");
            return ReturnValue;
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
                            Log.Verbose("Successfully executed {Command.CommandText}", Command.CommandText);
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
                        Log.Verbose("Successfully executed {Command.CommandText}", Command.CommandText);

                        return "";
                    }
                    else // shouldn't call this, just return empty string
                        return "";
                }
            }
            catch (SqliteException ex)
            {
                Log.Error("SQLLITEEXCEPTION, Error executing statement {SqlStatement} with exception {ex.Message}", SqlStatement, ex.Message);
                return "There was an error executing a SQL command.";
            }
            catch (Exception ex)
            {
                Log.Error("Error executing statement:{SqlStatement}\nWith Exception: {ex.Message}", SqlStatement, ex.Message);
                return "There was an error executing a SQL command.";
            }
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
                Log.Verbose("Initialized workouts table with return value \n{ReturnValue}", ReturnValue);
                Log.Information("Initialized workouts table!");

                ReturnValue = WorkoutManagement.ExecuteSQL(
                    @"
                    CREATE TABLE sessions (
                        session_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        date DATE NOT NULL
                    );
                    ",
                    "NonQuery",
                    new Dictionary<string, object>{}); // absence of .db file creates one when SQL statement called);
                Log.Verbose("Initialized sessions table with return value \n{ReturnValue}", ReturnValue);
                Log.Information("Initialized sessions table!");
                Thread.Sleep(3000);
            }
        }
    }
}
