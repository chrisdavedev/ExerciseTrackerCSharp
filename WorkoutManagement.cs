using System.Globalization;
using Microsoft.Data.Sqlite;

namespace ConsoleApp2
{
    static class WorkoutManagement
    {
        static public string ExecuteSQL(string SqlStatement, string CommandType)
        {
            try
            {
                using (var Connection = new SqliteConnection("Data Source=test.db"))
                {
                    Connection.Open();

                    var Command = Connection.CreateCommand();
                    Command.CommandText = SqlStatement;

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
                    "NonQuery"); // absence of .db file creates one when SQL statement called
                Console.WriteLine(ReturnValue);

                ReturnValue = WorkoutManagement.ExecuteSQL(
                    @"
                    CREATE TABLE sessions (
                        session_id INTEGER PRIMARY KEY AUTOINCREMENT,
                        date DATE NOT NULL
                    );
                    ",
                    "NonQuery");
                Console.WriteLine(ReturnValue);
                Thread.Sleep(3000);
            }
        }
    }
}
