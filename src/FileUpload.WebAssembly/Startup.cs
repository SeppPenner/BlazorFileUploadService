// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Hämmer Electronics">
//   Copyright (c) 2019 All rights reserved.
// </copyright>
// <summary>
//   This class does all the startup configuration for the WebAssembly part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUpload.WebAssembly
{
    using FileUpload.Core;
    using FileUpload.Core.Database;

    using Microsoft.AspNetCore.Components.Builder;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// This class does all the startup configuration for the WebAssembly part.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDatabaseHelper, DatabaseHelper>();
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The <see cref="IComponentsApplicationBuilder"/>.</param>
        // ReSharper disable once UnusedMember.Global
        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
