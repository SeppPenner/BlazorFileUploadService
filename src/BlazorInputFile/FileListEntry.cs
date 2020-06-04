﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileListEntry.cs" company="Haemmer Electronics">
//   Copyright (c) 2019 All rights reserved.
// </copyright>
// <summary>
//   This class contains the information about the files that are uploaded.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BlazorInputFile
{
    using System;
    using System.IO;

    /// <summary>
    /// This class contains the information about the files that are uploaded.
    /// </summary>
    public class FileListEntry : IFileListEntry
    {
        /// <summary>
        /// The component owner of the file.
        /// </summary>
        internal InputFile Owner { get; set; }

        /// <summary>
        /// The stream.
        /// </summary>
        private Stream stream;

        /// <summary>
        /// The event that is called when the data is read.
        /// </summary>
        public event EventHandler OnDataRead;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the last modified date.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the new file name (The one that is used on the server to provide unique links).
        /// </summary>
        public string NewFileName { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets the data stream.
        /// </summary>
        public Stream Data
        {
            get
            {
                this.stream ??= this.Owner.OpenFileStream(this);
                return this.stream;
            }
        }

        /// <summary>
        /// Raises the <see cref="OnDataRead"/> event.
        /// </summary>
        internal void RaiseOnDataRead()
        {
            this.OnDataRead?.Invoke(this, null);
        }
    }
}
