// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileListEntryStream.cs" company="Hämmer Electronics">
//   The project is licensed under the MIT license.
// </copyright>
// <summary>
//   This class contains a data stream for the <see cref="FileListEntry"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlazorInputFile
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    /// <summary>
    /// This class contains a data stream for the <see cref="FileListEntry"/>.
    /// </summary>
    public abstract class FileListEntryStream : Stream
    {
        /// <summary>
        /// The JavaScript runtime.
        /// </summary>
        protected readonly IJSRuntime JavascriptRuntime;

        /// <summary>
        /// The input file reference.
        /// </summary>
        protected readonly ElementReference InputFileElement;

        /// <summary>
        /// The <see cref="FileListEntry"/>.
        /// </summary>
        protected readonly FileListEntry File;

        /// <summary>
        /// The position.
        /// </summary>
        private long position;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileListEntryStream"/> class.
        /// </summary>
        /// <param name="javascriptRuntime">The JavaScript runtime.</param>
        /// <param name="inputFileElement">The input file reference.</param>
        /// <param name="file">The <see cref="FileListEntry"/>.</param>
        protected FileListEntryStream(IJSRuntime javascriptRuntime, ElementReference inputFileElement, FileListEntry file)
        {
            this.JavascriptRuntime = javascriptRuntime;
            this.InputFileElement = inputFileElement;
            this.File = file;
        }

        /// <summary>
        /// A <see cref="bool"/> value indicating whether the stream can read or not.
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        /// A <see cref="bool"/> value indicating whether the stream can seek or not.
        /// </summary>
        public override bool CanSeek => false;

        /// <summary>
        /// A <see cref="bool"/> value indicating whether the stream can write or not.
        /// </summary>
        public override bool CanWrite => false;

        /// <summary>
        /// The length of the file.
        /// </summary>
        public override long Length => this.File.Size;

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public override long Position
        {
            get => this.position;
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// Flushes the data.
        /// </summary>
        public override void Flush()
            => throw new NotSupportedException();

        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>An <see cref="int"/> representing the read data.</returns>
        public override int Read(byte[] buffer, int offset, int count)
            => throw new NotSupportedException("Synchronous reads are not supported");

        /// <summary>
        /// Seeks the data.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="origin">The seek origin.</param>
        /// <returns>A <see cref="long"/> representing the found data.</returns>
        public override long Seek(long offset, SeekOrigin origin)
            => throw new NotSupportedException();

        /// <summary>
        /// Sets the length.
        /// </summary>
        /// <param name="value">The new value.</param>
        public override void SetLength(long value)
            => throw new NotSupportedException();

        /// <summary>
        /// Writes the buffer to the stream.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();

        /// <summary>
        /// Reads the data.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="int"/> representing the read data.</returns>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var maxBytesToRead = (int)Math.Min(count, this.Length - this.Position);
            if (maxBytesToRead == 0)
            {
                return 0;
            }

            var actualBytesRead = await this.CopyFileDataIntoBuffer(this.position, buffer, offset, maxBytesToRead, cancellationToken);
            this.position += actualBytesRead;
            this.File.RaiseOnDataRead();
            return actualBytesRead;
        }

        /// <summary>
        /// Copies the file data into the buffer.
        /// </summary>
        /// <param name="sourceOffset">The source offset.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="destinationOffset">The destination offset.</param>
        /// <param name="maxBytes">The maximum number of bytes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An <see cref="int"/> representing the copied data.</returns>
        protected abstract Task<int> CopyFileDataIntoBuffer(long sourceOffset, byte[] destination, int destinationOffset, int maxBytes, CancellationToken cancellationToken);
    }
}
