// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SharedMemoryFileListEntryStream.cs" company="Hämmer Electronics">
//   The project is licensed under the MIT license.
// </copyright>
// <summary>
//   This class contains a shared memory data stream for the <see cref="FileListEntry"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlazorInputFile;

/// <summary>
/// This class contains a shared memory data stream for the <see cref="FileListEntry"/>. This is used in WebAssembly.
/// </summary>
internal class SharedMemoryFileListEntryStream : FileListEntryStream
{
    /// <summary>
    /// The mono WebAssembly JavaScript runtime type.
    /// </summary>
    private static readonly Type? MonoWebAssemblyJavascriptRuntimeType
        = Type.GetType("Mono.WebAssembly.Interop.MonoWebAssemblyJSRuntime, Mono.WebAssembly.Interop");

    /// <summary>
    /// The cached invoke unmarshalled method information.
    /// </summary>
    private static MethodInfo? cachedInvokeUnmarshalledMethodInfo;

    /// <inheritdoc cref="FileListEntryStream" />
    /// <summary>
    /// Initializes a new instance of the <see cref="SharedMemoryFileListEntryStream"/> class.
    /// </summary>
    /// <param name="javascriptRuntime">The JavaScript runtime.</param>
    /// <param name="inputFileElement">The input file reference.</param>
    /// <param name="file">The <see cref="FileListEntry"/>.</param>
    /// <seealso cref="FileListEntryStream" />
    public SharedMemoryFileListEntryStream(IJSRuntime javascriptRuntime, ElementReference inputFileElement, FileListEntry file)
        : base(javascriptRuntime, inputFileElement, file)
    {
    }

    /// <summary>
    /// Checks whether the method calls are supported.
    /// </summary>
    /// <param name="javascriptRuntime">The JavaScript runtime.</param>
    /// <returns><c>true</c> if supported, <c>false</c> else.</returns>
    public static bool IsSupported(IJSRuntime javascriptRuntime)
    {
        return MonoWebAssemblyJavascriptRuntimeType != null
            && MonoWebAssemblyJavascriptRuntimeType.IsInstanceOfType(javascriptRuntime);
    }

    /// <inheritdoc cref="FileListEntryStream" />
    /// <summary>
    ///     Copies the file data into the buffer.
    /// </summary>
    /// <param name="sourceOffset">The source offset.</param>
    /// <param name="destination">The destination.</param>
    /// <param name="destinationOffset">The destination offset.</param>
    /// <param name="maxBytes">The maximum number of bytes.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="int" /> representing the copied data.</returns>
    /// <seealso cref="FileListEntryStream" />
    protected override async Task<int> CopyFileDataIntoBuffer(long sourceOffset, byte[] destination, int destinationOffset, int maxBytes, CancellationToken cancellationToken)
    {
        await this.JavascriptRuntime.InvokeAsync<string>(
            "BlazorInputFile.ensureArrayBufferReadyForSharedMemoryInterop",
            cancellationToken,
            this.InputFileElement,
            this.File.Id);

        var methodInfo = GetCachedInvokeUnmarshalledMethodInfo();

        if (methodInfo is null)
        {
            return 0;
        }

        var readRequest = new ReadRequest
        {
            InputFileElementReferenceId = this.InputFileElement.Id,
            FileId = this.File.Id,
            SourceOffset = sourceOffset,
            Destination = destination,
            DestinationOffset = destinationOffset,
            MaximumBytes = maxBytes
        };

        var value = methodInfo.Invoke(this.JavascriptRuntime, new object[] { "BlazorInputFile.readFileDataSharedMemory", readRequest });
        return value is null ? 0 : (int)value;
    }

    /// <summary>
    /// Gets the cached unmarshalled method information.
    /// </summary>
    /// <returns>The <see cref="MethodInfo"/>.</returns>
    private static MethodInfo? GetCachedInvokeUnmarshalledMethodInfo()
    {
        if (cachedInvokeUnmarshalledMethodInfo is not null)
        {
            return cachedInvokeUnmarshalledMethodInfo;
        }

        if (MonoWebAssemblyJavascriptRuntimeType is null)
        {
            return null;
        }

        foreach (var possibleMethodInfo in MonoWebAssemblyJavascriptRuntimeType.GetMethods())
        {
            if (possibleMethodInfo.Name != "InvokeUnmarshalled" || possibleMethodInfo.GetParameters().Length != 2)
            {
                continue;
            }

            cachedInvokeUnmarshalledMethodInfo = possibleMethodInfo
                .MakeGenericMethod(typeof(ReadRequest), typeof(int));
            break;
        }

        if (cachedInvokeUnmarshalledMethodInfo == null)
        {
            throw new InvalidOperationException("Could not find the 2-param overload of InvokeUnmarshalled.");
        }

        return cachedInvokeUnmarshalledMethodInfo;
    }

    /// <summary>
    /// This struct contains read request data for the <see cref="SharedMemoryFileListEntryStream"/>.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    private struct ReadRequest
    {
        /// <summary>
        /// The input file element reference identifier.
        /// </summary>
        [FieldOffset(0)]
        public string InputFileElementReferenceId;

        /// <summary>
        /// The file identifier.
        /// </summary>
        [FieldOffset(4)]
        public int FileId;

        /// <summary>
        /// The source offset.
        /// </summary>
        [FieldOffset(8)]
        public long SourceOffset;

        /// <summary>
        /// The destination.
        /// </summary>
        [FieldOffset(16)]
        public byte[] Destination;

        /// <summary>
        /// The destination offset.
        /// </summary>
        [FieldOffset(20)]
        public int DestinationOffset;

        /// <summary>
        /// The maximum number of bytes.
        /// </summary>
        [FieldOffset(24)]
        public int MaximumBytes;
    }
}
