// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseHelper.cs" company="Haemmer Electronics">
//   Copyright (c) 2019 All rights reserved.
// </copyright>
// <summary>
//   This interface contains methods to operate with the database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUpload.Core.Database
{
    using System.Threading.Tasks;

    /// <summary>
    /// This interface contains methods to operate with the database.
    /// </summary>
    public interface IDatabaseHelper
    {
        /// <summary>
        /// Creates the files folder if it doesn't exist.
        /// </summary>
        void CreateFilesFolderIfNotExist();

        /// <summary>
        /// Creates the database folder if it doesn't exist.
        /// </summary>
        // ReSharper disable once UnusedMemberInSuper.Global
        void CreateDatabaseFolderIfNotExist();

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        /// Creates the database folder and file if it doesn't exist.
        /// </summary>
        /// <seealso cref="IDatabaseHelper" />
        void CreateDatabaseIfNotExists();

        /// <summary>
        /// Gets a file by its identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task<FileModel> GetFileById(string identifier);

        /// <summary>
        /// Creates the files table.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task CreateFilesTableIfNotExists();

        /// <summary>
        /// Gets the files path.
        /// </summary>
        /// <returns>The files path as <see cref="string"/>.</returns>
        string GetFilesPath();

        /// <summary>
        /// Gets the database path.
        /// </summary>
        /// <returns>The database path as <see cref="string"/>.</returns>
        // ReSharper disable once UnusedMemberInSuper.Global
        string GetDatabasePath();

        /// <summary>
        /// Inserts a <see cref="FileModel"/> into the table.
        /// </summary>
        /// <param name="fileModel">The file model.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        Task InsertFile(FileModel fileModel);
    }
}
