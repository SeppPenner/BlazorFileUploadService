// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileModel.cs" company="Haemmer Electronics">
//   Copyright (c) 2019 All rights reserved.
// </copyright>
// <summary>
//   This class contains the file model written to the database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUpload.Core.Database
{
    /// <summary>
    /// This class contains the file model written to the database.
    /// </summary>
    public class FileModel
    {
        /// <summary>
        /// Gets or sets the identifier. (A random file name used on the server side).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the path to the file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file size.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the file type.
        /// </summary>
        public string Type { get; set; }
    }
}
