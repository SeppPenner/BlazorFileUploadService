// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputFile.razor.cs" company="Hämmer Electronics">
//   The project is licensed under the MIT license.
// </copyright>
// <summary>
//   This class contains the logic for the <see cref="InputFile"/> page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlazorInputFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    /// <inheritdoc cref="IDisposable"/>
    /// <inheritdoc cref="ComponentBase"/>
    /// <summary>
    ///     This class contains the logic for the <see cref="InputFile" /> page.
    /// </summary>
    public abstract class InputFileBase : ComponentBase, IDisposable
    {
        /// <summary>
        ///     Gets or sets the maximum buffer size.
        /// </summary>
        [Parameter]
        public int MaximumBufferSize { get; set; } = 1024 * 1024;

        /// <summary>
        ///     Gets or sets the maximum message size.
        /// </summary>
        [Parameter]
        public int MaximumMessageSize { get; set; } = 20 * 1024;

        /// <summary>
        ///     Gets or sets the on change callback.
        /// </summary>
        [Parameter]
        public EventCallback<FileListEntry[]> OnChange { get; set; }

        /// <summary>
        ///     Gets or sets the unmatched parameters.
        /// </summary>
        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> UnmatchedParameters { get; set; } = new();

        /// <summary>
        ///     Gets or sets the input file reference.
        /// </summary>
        protected ElementReference InputFileElement { get; set; }

        /// <summary>
        ///     Gets or sets the JavaScript runtime.
        /// </summary>
        [Inject]
        private IJSRuntime? JavascriptRuntime { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="IDisposable" /> reference.
        /// </summary>
        private IDisposable? ThisReference { get; set; }

        /// <inheritdoc cref="IDisposable"/>
        void IDisposable.Dispose()
        {
            this.ThisReference?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Opens the file stream.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>A <see cref="Stream" /> with the file content.</returns>
        internal Stream OpenFileStream(FileListEntry file)
        {
            if (this.JavascriptRuntime is null)
            {
                throw new InvalidOperationException("The JavaScript runtime wasn't initialized properly");
            }

            return SharedMemoryFileListEntryStream.IsSupported(this.JavascriptRuntime)
                       ? (Stream)new SharedMemoryFileListEntryStream(this.JavascriptRuntime, this.InputFileElement, file)
                       : new RemoteFileListEntryStream(this.JavascriptRuntime, this.InputFileElement, file, this.MaximumMessageSize, this.MaximumBufferSize);
        }

        /// <inheritdoc cref="ComponentBase"/>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.ThisReference = DotNetObjectReference.Create(this);

                if (this.JavascriptRuntime is not null)
                {
                    await this.JavascriptRuntime.InvokeAsync<object>("BlazorInputFile.init", this.InputFileElement, this.ThisReference);
                }
            }
        }
    }
}
