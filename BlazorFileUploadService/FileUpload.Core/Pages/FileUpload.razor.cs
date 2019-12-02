// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileUpload.razor.cs" company="Haemmer Electronics">
//   Copyright (c) 2019 All rights reserved.
// </copyright>
// <summary>
//   This class contains the logic for the <see cref="FileUpload"/> page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUpload.Core.Pages
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using BlazorInputFile;

    using global::FileUpload.Core.Database;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;


    /// <summary>
    ///     This class contains the logic for the <see cref="FileUpload" /> page.
    /// </summary>
    public class FileUploadBase : ComponentBase
    {
        /// <summary>
        /// Gets or sets the selected files.
        /// </summary>
        protected IFileListEntry[] SelectedFiles { get; set; }

        /// <summary>
        ///     Gets or sets the database helper.
        /// </summary>
        [Inject]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private IDatabaseHelper DatabaseHelper { get; set; }

        /// <summary>
        /// Gets or sets the JavaScript runtime.
        /// </summary>
        [Inject]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private IJSRuntime JavascriptRuntime { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        [Inject]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Handles the selected files.
        /// </summary>
        /// <param name="files">The files.</param>
        // ReSharper disable once UnusedMember.Global
        protected void HandleSelection(IFileListEntry[] files)
        {
            this.SelectedFiles = files;
        }

        /// <summary>
        /// Gets the download link for the given file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected string GetDownloadLink(IFileListEntry file)
        {
            return $"{this.NavigationManager.BaseUri}file/{file.NewFileName}";
        }

        /// <summary>
        /// Method invoked after each time the component has been rendered. Note that the component does
        /// not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />, because
        /// that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        /// Set to <c>true</c> if this is the first time <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        /// on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        /// <remarks>
        /// The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        /// are useful for performing interop, or interacting with values recieved from <c>@ref</c>.
        /// Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        /// once.
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            try
            {
                this.DatabaseHelper.CreateFilesFolderIfNotExist();
                this.DatabaseHelper.CreateDatabaseIfNotExists();
                await this.DatabaseHelper.CreateFilesTableIfNotExists();
            }
            catch (Exception ex)
            {
                await this.JavascriptRuntime.InvokeAsync<string>("console.log", $"Exception: {ex.Message} {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        // ReSharper disable once UnusedMember.Global
        protected async Task UploadFile(IFileListEntry file)
        {
            try
            {
                file.OnDataRead += (sender, eventArgs) => this.InvokeAsync(this.StateHasChanged);

                var fileEnding = Path.GetExtension(file.Name);
                var randomFileName = Path.GetRandomFileName().Replace(".", string.Empty);
                var fullRandomFileName = $"{randomFileName}{fileEnding}";
                var filePath = Path.Combine(this.DatabaseHelper.GetFilesPath(), fullRandomFileName);

                file.NewFileName = randomFileName;

                using Stream stream = File.Create(filePath);
                await file.Data.CopyToAsync(stream);
                await this.DatabaseHelper.InsertFile(new FileModel { Id = randomFileName, FileName = file.Name, FilePath = filePath });
            }
            catch (Exception ex)
            {
                await this.JavascriptRuntime.InvokeAsync<string>("console.log", $"Exception: {ex.Message} {ex.StackTrace}");
            }
        }
    }
}
