// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Haemmer Electronics">
//   Copyright (c) 2019 All rights reserved.
// </copyright>
// <summary>
//   This class runs the WebAssembly part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUpload.WebAssembly
{
    using Microsoft.AspNetCore.Blazor.Hosting;

    /// <summary>
    /// This class runs the WebAssembly part.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">Some arguments.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates the <see cref="IWebAssemblyHostBuilder"/>.
        /// </summary>
        /// <param name="args">Some arguments.</param>
        /// <returns>The <see cref="IWebAssemblyHostBuilder"/>.</returns>
        public static IWebAssemblyHostBuilder CreateHostBuilder(string[] args) =>
            BlazorWebAssemblyHost.CreateDefaultBuilder()
                .UseBlazorStartup<Startup>();
    }
}
