// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PreFetchingSequence.cs" company="Hämmer Electronics">
//   The project is licensed under the MIT license.
// </copyright>
// <summary>
//   This class contains helper methods for the pre-fetching of data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlazorInputFile
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// This class contains helper methods for the pre-fetching of data.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    internal class PreFetchingSequence<T>
    {
        /// <summary>
        /// The fetch callback.
        /// </summary>
        private readonly Func<long, CancellationToken, T> fetchCallback;

        /// <summary>
        /// The maximum buffer capacity.
        /// </summary>
        private readonly int maximumBufferCapacity;

        /// <summary>
        /// The number of fetchable items.
        /// </summary>
        private readonly long totalFetchableItems;

        /// <summary>
        /// The buffer.
        /// </summary>
        private readonly Queue<T> buffer;

        /// <summary>
        /// The maximum fetched index.
        /// </summary>
        private long maximumFetchedIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreFetchingSequence{T}"/> class.
        /// </summary>
        /// <param name="fetchCallback">The fetch callback.</param>
        /// <param name="totalFetchableItems">The number of fetchable items.</param>
        /// <param name="maximumBufferCapacity">The maximum buffer capacity.</param>
        public PreFetchingSequence(Func<long, CancellationToken, T> fetchCallback, long totalFetchableItems, int maximumBufferCapacity)
        {
            this.fetchCallback = fetchCallback;
            this.buffer = new Queue<T>();
            this.maximumBufferCapacity = maximumBufferCapacity;
            this.totalFetchableItems = totalFetchableItems;
        }

        /// <summary>
        /// Reads the next data.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <result>An <see cref="object"/> of the generic type.</result>
        /// <exception cref="InvalidOperationException">There are no more entries to read.</exception>
        public T ReadNext(CancellationToken cancellationToken)
        {
            this.EnqueueFetches(cancellationToken);

            if (this.buffer.Count == 0)
            {
                throw new InvalidOperationException("There are no more entries to read.");
            }

            var next = this.buffer.Dequeue();
            this.EnqueueFetches(cancellationToken);
            return next;
        }

        /// <summary>
        /// Tries to peek the next data.
        /// </summary>
        /// <param name="result">An <see cref="object"/> of the generic type.</param>
        /// <returns><c>true</c> if successful, <c>false</c> if not.</returns>
        public bool TryPeekNext(out T? result)
        {
            if (this.buffer.Count > 0)
            {
                result = this.buffer.Peek();
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Enqueues the fetches.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        private void EnqueueFetches(CancellationToken cancellationToken)
        {
            while (this.buffer.Count < this.maximumBufferCapacity && this.maximumFetchedIndex < this.totalFetchableItems)
            {
                this.buffer.Enqueue(this.fetchCallback(this.maximumFetchedIndex++, cancellationToken));
            }
        }
    }
}
