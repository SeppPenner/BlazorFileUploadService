// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DatabaseHelper.cs" company="Haemmer Electronics">
//   Copyright (c) 2019 All rights reserved.
// </copyright>
// <summary>
//   This class contains methods to operate with the database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUpload.Core.Database
{
    using System;
    using System.Data.SQLite;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    using Dapper;

    /// <inheritdoc cref="IDatabaseHelper"/>
    /// <summary>
    /// This class contains methods to operate with the database.
    /// </summary>
    /// <seealso cref="IDatabaseHelper" />
    public class DatabaseHelper : IDatabaseHelper
    {
        /// <summary>
        /// The database file name.
        /// </summary>
        private const string FileName = "files.sqlite";

        /// <summary>
        /// The file folder name.
        /// </summary>
        private const string FileFolder = "files";

        /// <summary>
        /// The database folder name.
        /// </summary>
        private const string DatabaseFolder = "database";

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        /// Creates the files folder if it doesn't exist.
        /// </summary>
        /// <seealso cref="IDatabaseHelper" />
        public void CreateFilesFolderIfNotExist()
        {
            var filesPath = this.GetFilesPath();

            if (!Directory.Exists(filesPath))
            {
                Directory.CreateDirectory(filesPath);
            }
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        /// Creates the database folder if it doesn't exist.
        /// </summary>
        /// <seealso cref="IDatabaseHelper" />
        public void CreateDatabaseFolderIfNotExist()
        {
            var databasePath = this.GetDatabasePath();

            if (!Directory.Exists(databasePath))
            {
                Directory.CreateDirectory(databasePath);
            }
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        /// Creates the database folder and file if it doesn't exist.
        /// </summary>
        /// <seealso cref="IDatabaseHelper" />
        public void CreateDatabaseIfNotExists()
        {
            this.CreateDatabaseFolderIfNotExist();

            var databasePath = this.GetDatabasePath();

            if (string.IsNullOrWhiteSpace(databasePath))
            {
                return;
            }

            var databaseFilePath = Path.Combine(databasePath, FileName);

            if (!File.Exists(databaseFilePath))
            {
                File.Create(databaseFilePath);
            }
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        /// Gets the files path.
        /// </summary>
        /// <returns>The files path as <see cref="string"/>.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public string GetFilesPath()
        {
            var currentLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return currentLocation == null ? string.Empty : Path.Combine(currentLocation, FileFolder);
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        /// Gets the database path.
        /// </summary>
        /// <returns>The database path as <see cref="string"/>.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public string GetDatabasePath()
        {
            var currentLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return currentLocation == null ? string.Empty : Path.Combine(currentLocation, DatabaseFolder);
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        /// Gets a file by its identifier.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public async Task<FileModel> GetFileById(string identifier)
        {
            using var connection = new SQLiteConnection(this.GetConnectionString());
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<FileModel>(SqlStatements.SelectFileById, new { Id = identifier });
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        /// Inserts a <see cref="FileModel"/> into the table.
        /// </summary>
        /// <param name="fileModel">The file model.</param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public async Task InsertFile(FileModel fileModel)
        {
            using var connection = new SQLiteConnection(this.GetConnectionString());
            await connection.OpenAsync();
            await connection.ExecuteAsync(SqlStatements.InsertFile, fileModel);
        }

        /// <inheritdoc cref="IDatabaseHelper" />
        /// <summary>
        /// Creates the files table.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <seealso cref="IDatabaseHelper" />
        public async Task CreateFilesTableIfNotExists()
        {
            using var connection = new SQLiteConnection(this.GetConnectionString());
            await connection.OpenAsync();
            var checkTableExistsResult = connection.ExecuteScalar(SqlStatements.CheckFilesTableExists);
            if (checkTableExistsResult == null || Convert.ToInt32(checkTableExistsResult) == 0)
            {
                await connection.ExecuteAsync(SqlStatements.CreateFilesTable);
            }
        }

        /// <summary>
        /// Gets the database connection <see cref="string"/>.
        /// </summary>
        /// <returns>The database connection <see cref="string"/>.</returns>
        private string GetConnectionString()
        {
            var databasePath = this.GetDatabasePath();
            return databasePath == null ? string.Empty : $"Data Source={Path.Combine(databasePath, FileName)}";
        }
    }
}
