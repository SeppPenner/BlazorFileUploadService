// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileDownload.razor.cs" company="Haemmer Electronics">
//   Copyright (c) 2019 All rights reserved.
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

    /// <summary>
    ///     This class contains the logic for the <see cref="FileDownload" /> page.
    /// </summary>
    public class FileDownloadBase : ComponentBase
    {
        /// <summary>
        ///     Gets or sets the file identifier.
        /// </summary>
        [Parameter]

        // ReSharper disable once UnusedMember.Global
        public string FileId { get; set; }

        /// <summary>
        ///     Gets or sets the file model.
        /// </summary>
        protected FileModel File { get; set; }

        /// <summary>
        /// Gets or sets the JavaScript runtime.
        /// </summary>
        [Inject]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private IJSRuntime JavascriptRuntime { get; set; }

        /// <summary>
        ///     Gets or sets the database helper.
        /// </summary>
        [Inject]
        private IDatabaseHelper DatabaseHelper { get; set; }

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
                await this.JavascriptRuntime.InvokeAsync<string>("console.log", $"Exception: {ex.Message} {ex.StackTrace}");
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
                // Todo: Download
            }
            catch (Exception ex)
            {
                await this.JavascriptRuntime.InvokeAsync<string>("console.log", $"Exception: {ex.Message} {ex.StackTrace}");
            }
        }
    }

}
