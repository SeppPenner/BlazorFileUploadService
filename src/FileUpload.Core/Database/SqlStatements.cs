// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlStatements.cs" company="Hämmer Electronics">
//   The project is licensed under the MIT license.
// </copyright>
// <summary>
//   This class contains the SQL statements to interact with the database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUpload.Core.Database
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// This class contains the SQL statements to interact with the database.
    /// </summary>
    public static class SqlStatements
    {
        /// <summary>
        /// A SQL query string to create the files table.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string CreateFilesTable =
            @"CREATE TABLE IF NOT EXISTS files (
                id                      TEXT            NOT NULL PRIMARY KEY,
                filepath                TEXT            NOT NULL,
                filename                TEXT            NOT NULL,
                size                    INTEGER         NOT NULL,
                type                    TEXT            NOT NULL
            );";

        /// <summary>
        /// A SQL query string to select a file by its identifier.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string SelectFileById =
            @"SELECT id, filepath, filename, size, type
            FROM files
            WHERE id = @Id;";

        /// <summary>
        /// A SQL query string to insert a file into the database.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string InsertFile =
            @"INSERT INTO files (id, filepath, filename, size, type)
            VALUES (@Id, @FilePath, @FileName, @Size, @Type);";

        /// <summary>
        /// A SQL query string to check whether the files table exists.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static string CheckFilesTableExists =
            @"SELECT count(name) FROM sqlite_master WHERE type='table' AND name='files';";
    }
}
