// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileListEntry.cs" company="Hämmer Electronics">
//   The project is licensed under the MIT license.
// </copyright>
// <summary>
//   This interface contains the information about the files that are uploaded.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlazorInputFile;

/// <summary>
///     This interface contains the information about the files that are uploaded.
/// </summary>
public interface IFileListEntry
{
    /// <summary>
    ///     The event that is called when the data is read.
    /// </summary>
    event EventHandler OnDataRead;

    /// <summary>
    ///     Gets the data stream.
    /// </summary>
    Stream Data { get; }

    /// <summary>
    ///     Gets the last modified date.
    /// </summary>
    DateTime LastModified { get; }

    /// <summary>
    ///     Gets the name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets or sets the new file name (The one that is used on the server to provide unique links).
    /// </summary>
    string NewFileName { get; set; }

    /// <summary>
    ///     Gets the size.
    /// </summary>
    long Size { get; }

    /// <summary>
    ///     Gets the type.
    /// </summary>
    string Type { get; }
}
