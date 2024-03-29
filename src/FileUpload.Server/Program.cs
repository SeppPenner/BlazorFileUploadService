// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Hämmer Electronics">
//   The project is licensed under the MIT license.
// </copyright>
// <summary>
//   This class runs the server part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUpload.Server;

/// <summary>
/// This class runs the server part.
/// </summary>
public class Program
{
    /// <summary>
    /// The main method.
    /// </summary>
    /// <param name="args">Some arguments.</param>
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.Trace()
            .CreateLogger();

        CreateHostBuilder(args).Build().Run();
    }

    /// <summary>
    /// Creates the <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="args">Some arguments.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
