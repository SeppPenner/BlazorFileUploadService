// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputFile.razor.cs" company="Hämmer Electronics">
//   Copyright (c) 2019 All rights reserved.
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
        // ReSharper disable once UnusedMember.Global
        public Dictionary<string, object> UnmatchedParameters { get; set; }

        /// <summary>
        ///     Gets or sets the input file reference.
        /// </summary>
        protected ElementReference InputFileElement { get; set; }

        /// <summary>
        ///     Gets or sets the JavaScript runtime.
        /// </summary>
        [Inject]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private IJSRuntime JavascriptRuntime { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="IDisposable" /> reference.
        /// </summary>
        private IDisposable ThisReference { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.ThisReference?.Dispose();
        }

        /// <summary>
        ///     Opens the file stream.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>A <see cref="Stream" /> with the file content.</returns>
        internal Stream OpenFileStream(FileListEntry file)
        {
            return SharedMemoryFileListEntryStream.IsSupported(this.JavascriptRuntime)
                       ? (Stream)new SharedMemoryFileListEntryStream(this.JavascriptRuntime, this.InputFileElement, file)
                       : new RemoteFileListEntryStream(this.JavascriptRuntime, this.InputFileElement, file, this.MaximumMessageSize, this.MaximumBufferSize);
        }

        /// <summary>
        ///     Method invoked after each time the component has been rendered. Note that the component does
        ///     not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />,
        ///     because
        ///     that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        ///     Set to <c>true</c> if this is the first time
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        ///     on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        /// <remarks>
        ///     The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        ///     are useful for performing interop, or interacting with values received from <c>@ref</c>.
        ///     Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        ///     once.
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.ThisReference = DotNetObjectReference.Create(this);
                await this.JavascriptRuntime.InvokeAsync<object>("BlazorInputFile.init", this.InputFileElement, this.ThisReference);
            }
        }
    }
}
