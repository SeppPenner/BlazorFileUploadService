// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteFileListEntryStream.cs" company="Hämmer Electronics">
//   Copyright (c) 2019 All rights reserved.
// </copyright>
// <summary>
//   This class contains a remote data stream for the <see cref="FileListEntry"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlazorInputFile
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    /// <summary>
    ///     This class contains a remote data stream for the <see cref="FileListEntry" />.
    ///     This class streams data from JS within the existing API limits of IJSRuntime.
    ///     To produce good throughput, it pre-fetches up to buffer_size data from JS
    ///     even when the consumer isn't asking for that much data, and does so by making
    ///     N parallel requests in parallel (N ~= buffer_size / max_message_size).
    ///     This should be understood as a TEMPORARY way to achieve the desired API and
    ///     reasonable performance. Longer term we can surely replace this with something
    ///     simpler and cleaner, either:
    ///     - Extending JS interop to allow streaming responses via SignalR's built-in
    ///     binary streaming support. That should reduce all of this to triviality.
    ///     - Or, failing that, at least use something like System.IO.Pipelines to manage
    ///     the supply/consumption of byte data with less custom code.
    /// </summary>
    internal class RemoteFileListEntryStream : FileListEntryStream
    {
        /// <summary>
        ///     The block sequence.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private readonly PreFetchingSequence<Block> blockSequence;

        /// <summary>
        ///     The current block decoding buffer.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private readonly byte[] currentBlockDecodingBuffer;

        /// <summary>
        ///     The maximum message size.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private readonly int maximumMessageSize;

        /// <summary>
        ///     The current block.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private Block? currentBlock;

        /// <summary>
        ///     The current block decoding buffer already consumed length.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private int currentBlockDecodingBufferConsumedLength;

        /// <inheritdoc cref="FileListEntryStream" />
        /// <summary>
        ///     Initializes a new instance of the <see cref="RemoteFileListEntryStream" /> class.
        /// </summary>
        /// <param name="javascriptRuntime">The JavaScript runtime.</param>
        /// <param name="inputFileElement">The input file reference.</param>
        /// <param name="file">The <see cref="FileListEntry" />.</param>
        /// <param name="maximumMessageSize">The maximum message size.</param>
        /// <param name="maxBufferSize">The maximum buffer size.</param>
        /// <seealso cref="FileListEntryStream" />
        public RemoteFileListEntryStream(IJSRuntime javascriptRuntime, ElementReference inputFileElement, FileListEntry file, int maximumMessageSize, int maxBufferSize)
            : base(javascriptRuntime, inputFileElement, file)
        {
            this.maximumMessageSize = maximumMessageSize;
            this.blockSequence = new PreFetchingSequence<Block>(this.FetchBase64Block, (file.Size + this.maximumMessageSize - 1) / this.maximumMessageSize, Math.Max(1, maxBufferSize / this.maximumMessageSize)); // Degree of parallelism on fetch
            this.currentBlockDecodingBuffer = new byte[this.maximumMessageSize];
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
            var totalBytesCopied = 0;

            while (maxBytes > 0)
            {
                // If we don't yet have a block, or it's fully consumed, get the next one
                if (!this.currentBlock.HasValue || this.currentBlockDecodingBufferConsumedLength == this.currentBlock.Value.LengthBytes)
                {
                    // If we've already read some data, and the next block is still pending,
                    // then just return now rather than awaiting
                    if (totalBytesCopied > 0 && this.blockSequence.TryPeekNext(out var nextBlock) && !nextBlock.Base64.IsCompleted)
                    {
                        break;
                    }

                    this.currentBlock = this.blockSequence.ReadNext(cancellationToken);
                    var currentBlockBase64 = await this.currentBlock.Value.Base64;

                    // As a possible future optimization, if we know the current block will fit entirely in
                    // the remaining destination space, we could decode directly into the destination without
                    // going via currentBlockDecodingBuffer. However that complicates the logic a lot.
                    DecodeBase64ToBuffer(currentBlockBase64, this.currentBlockDecodingBuffer, 0, this.currentBlock.Value.LengthBytes);
                    this.currentBlockDecodingBufferConsumedLength = 0;
                }

                // How much of the current block can we fit into the destination?
                var numUnconsumedBytesInBlock = this.currentBlock == null ? 0 : this.currentBlock.Value.LengthBytes - this.currentBlockDecodingBufferConsumedLength;
                var numBytesToTransfer = Math.Min(numUnconsumedBytesInBlock, maxBytes);

                if (numBytesToTransfer == 0)
                {
                    break;
                }

                // Perform the copy
                Array.Copy(this.currentBlockDecodingBuffer, this.currentBlockDecodingBufferConsumedLength, destination, destinationOffset, numBytesToTransfer);
                maxBytes -= numBytesToTransfer;
                destinationOffset += numBytesToTransfer;
                this.currentBlockDecodingBufferConsumedLength += numBytesToTransfer;
                totalBytesCopied += numBytesToTransfer;
            }

            return totalBytesCopied;
        }

        /// <summary>
        ///     Decodes the base 64 value to a <see cref="T:byte[]" /> buffer.
        /// </summary>
        /// <param name="base64">The base 64 <see cref="string" />.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="maximumBytesToRead">The maximum bytes to read.</param>
        /// <returns>The number of decoded bytes.</returns>
        // ReSharper disable once UnusedMethodReturnValue.Local
        private static int DecodeBase64ToBuffer(string base64, byte[] buffer, int offset, int maximumBytesToRead)
        {
#if NETSTANDARD2_1
            var bufferWithOffset = new Span<byte>(buffer, offset, maximumBytesToRead);
            return Convert.TryFromBase64String(base64, bufferWithOffset, out var actualBytesRead)
                ? actualBytesRead
                : throw new InvalidOperationException("Failed to decode base64 data.");
#else
            var bytes = Convert.FromBase64String(base64);

            if (bytes.Length > maximumBytesToRead)
            {
                throw new InvalidOperationException($"Requested a maximum of {maximumBytesToRead}, but received {bytes.Length}.");
            }

            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
            return bytes.Length;
#endif
        }

        /// <summary>
        ///     Fetches the base 64 block at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The found <see cref="Block" />.</returns>
        private Block FetchBase64Block(long index, CancellationToken cancellationToken)
        {
            var sourceOffset = index * this.maximumMessageSize;
            var blockLength = (int)Math.Min(this.maximumMessageSize, this.File.Size - sourceOffset);
            var task = this.JavascriptRuntime.InvokeAsync<string>("BlazorInputFile.readFileData", cancellationToken, this.InputFileElement, this.File.Id, index * this.maximumMessageSize, blockLength).AsTask();
            return new Block(task, blockLength);
        }
    }
}
