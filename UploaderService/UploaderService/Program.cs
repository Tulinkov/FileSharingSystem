using Serilog;

namespace UploaderService
{
    public class Program
    {       
        public static void Main(string[] args)
        {
            string logPath = Environment.CurrentDirectory;
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(logPath, "logs.txt"))
                .CreateLogger();

            IHost host = Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}