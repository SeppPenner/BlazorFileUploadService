// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileDownload.razor.cs" company="Hämmer Electronics">
//   The project is licensed under the MIT license.
// </copyright>
// <summary>
//   This class contains the logic for the <see cref="FileDownload"/> page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUpload.Core.Pages
{
    using System;
    using System.Threading.Tasks;

    using global::FileUpload.Core.Database;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using Serilog;

    using IoFile = System.IO.File;

    /// <summary>
    ///     This class contains the logic for the <see cref="FileDownload" /> page.
    /// </summary>
    public class FileDownloadBase : ComponentBase
    {
        /// <summary>
        ///     Gets or sets the file identifier.
        /// </summary>
        [Parameter]
        public string FileId { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the file model.
        /// </summary>
        protected FileModel File { get; set; } = new();

        /// <summary>
        /// Gets or sets the JavaScript runtime.
        /// </summary>
        [Inject]
        private IJSRuntime? JavascriptRuntime { get; set; }

        /// <summary>
        ///     Gets or sets the database helper.
        /// </summary>
        [Inject]
        private IDatabaseHelper DatabaseHelper { get; set; } = new DatabaseHelper();

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger = Log.ForContext<FileDownloadBase>();

        /// <summary>
        /// Method invoked when the component has received parameters from its parent in
        /// the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                this.File = await this.DatabaseHelper.GetFileById(this.FileId);
            }
            catch (Exception ex)
            {
                await this.TryLogError(ex);
            }
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        protected async Task DownloadFile()
        {
            try
            {
                var fileData = IoFile.ReadAllBytes(this.File.FilePath);

                if (this.JavascriptRuntime is null)
                {
                    return;
                }

                await this.JavascriptRuntime.InvokeAsync<string>("window.downloadHelper.downloadFile", fileData, this.File.FileName, this.File.Type);
            }
            catch (Exception ex)
            {
                await this.TryLogError(ex);
            }
        }

        /// <summary>
        /// ´Tries to log the error to the Browser console and Serilog, catches errors and only logs to Serilog if needed.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to catch.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task TryLogError(Exception ex)
        {
            try
            {
                var message = $"Exception: {ex.Message} {ex.StackTrace}";

                if (this.JavascriptRuntime is null)
                {
                    return;
                }

                await this.JavascriptRuntime.InvokeAsync<string>("console.log", message);
                this.logger.Error(message);
            }
            catch (Exception exception)
            {
                this.logger.Error("An error occured: {exception}.", exception);
            }
        }
    }
}
