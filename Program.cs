using Serilog;
using Serilog.Events;

// getting setup
// https://github.com/serilog/serilog/wiki/Configuration-Basics

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information) // should only print out info and above to console??
            .WriteTo.File("app.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Verbose)
            .CreateLogger();
            
            Log.Verbose("Begin app.");
            WorkoutManagement.InitDB();
            Menu.BeginLoop();
            Log.Verbose("End app, goodbye!");
            Log.CloseAndFlush(); 
        }
    }
}
