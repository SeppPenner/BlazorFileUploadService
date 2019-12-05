// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSizeHelper.cs" company="Haemmer Electronics">
//   Copyright (c) 2019 All rights reserved.
// </copyright>
// <summary>
//   This class contains methods to operate with file sizes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FileUpload.Core.Helper
{
    /// <summary>
    /// This class contains methods to operate with file sizes.
    /// </summary>
    public static class FileSizeHelper
    {
        /// <summary>
        /// Gets the file size and unit name formatted properly.
        /// </summary>
        /// <param name="bytes">The amount of bytes.</param>
        /// <returns>A <see cref="string"/> represenating the formatted file size.</returns>
        public static string GetFormattedFileSize(decimal bytes)
        {
            if (bytes < 1024)
            {
                return bytes % 1 == 0 ? $"{bytes} B" : $"{bytes:0.00} B";
            }

            var kiloBytes = NextBiggerUnit(bytes);
            if (kiloBytes < 1024)
            {
                return kiloBytes % 1 == 0 ? $"{kiloBytes} kB" : $"{kiloBytes:0.00} kB";
            }

            var megaBytes = NextBiggerUnit(kiloBytes);
            if (megaBytes < 1024)
            {
                return megaBytes % 1 == 0 ? $"{megaBytes} MB" : $"{megaBytes:0.00} MB";
            }

            var gigaBytes = NextBiggerUnit(megaBytes);
            if (gigaBytes < 1024)
            {
                return gigaBytes % 1 == 0 ? $"{gigaBytes} GB" : $"{gigaBytes:0.00} GB";
            }

            var terraBytes = NextBiggerUnit(gigaBytes);
            if (terraBytes < 1024)
            {
                return terraBytes % 1 == 0 ? $"{terraBytes} TB" : $"{terraBytes:0.00} TB";
            }

            var petaBytes = NextBiggerUnit(gigaBytes);
            if (petaBytes < 1024)
            {
                return petaBytes % 1 == 0 ? $"{petaBytes} PB" : $"{petaBytes:0.00} PB";
            }

            var exaBytes = NextBiggerUnit(gigaBytes);
            if (exaBytes < 1024)
            {
                return exaBytes % 1 == 0 ? $"{exaBytes} EB" : $"{exaBytes:0.00} EB";
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the <see cref="decimal"/> value for the next bigger file size unit.
        /// </summary>
        /// <param name="checkValue">The value to check.</param>
        /// <returns>The <see cref="decimal"/> value for the next bigger file size unit.</returns>
        public static decimal NextBiggerUnit(decimal checkValue)
        {
            return checkValue / 1024M;
        }
    }
}
