// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Block.cs" company="Hämmer Electronics">
//   The project is licensed under the MIT license.
// </copyright>
// <summary>
//   This struct contains block data for the <see cref="RemoteFileListEntryStream"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlazorInputFile
{
    using System.Threading.Tasks;

    /// <summary>
    /// This struct contains block data for the <see cref="RemoteFileListEntryStream"/>.
    /// </summary>
    // ReSharper disable once StructCanBeMadeReadOnly
    public struct Block
    {
        /// <summary>
        /// The base 64 <see cref="Task"/>.
        /// </summary>
        public readonly Task<string> Base64;

        /// <summary>
        /// The length of the bytes.
        /// </summary>
        public readonly int LengthBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Block"/> struct.
        /// </summary>
        /// <param name="base64">The base 64 <see cref="Task"/>.</param>
        /// <param name="lengthBytes">The length of the bytes.</param>
        public Block(Task<string> base64, int lengthBytes)
        {
            this.Base64 = base64;
            this.LengthBytes = lengthBytes;
        }
    }
}
