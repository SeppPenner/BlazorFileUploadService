// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Hämmer Electronics">
//   The project is licensed under the MIT license.
// </copyright>
// <summary>
//   This class does all the startup configuration for the server part.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUpload.Server;

/// <summary>
/// This class does all the startup configuration for the server part.
/// </summary>
public class Startup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public IConfiguration Configuration { get; }

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="services">The services collection.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddSingleton<IDatabaseHelper, DatabaseHelper>();
        services.AddLocalization(options => options.ResourcesPath = "Translations");
    }

    /// <summary>
    /// Configures the application.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
    /// <param name="env">The <see cref="IWebHostEnvironment"/>.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.Use((context, next) =>
        {
            var service = context.Features.Get<IHttpMaxRequestBodySizeFeature>();

            if (service is not null)
            {
                service.MaxRequestBodySize = null;
            }

            return next.Invoke();
        });

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRequestLocalization(options =>
        {
            options.AddSupportedCultures("en-US", "de-DE");
            options.AddSupportedUICultures("en-US", "de-DE");
            options.RequestCultureProviders.Clear();
            options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
            options.SetDefaultCulture("de-DE");
        });

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}
