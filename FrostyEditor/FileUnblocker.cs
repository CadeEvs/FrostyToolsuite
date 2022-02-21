using System;
using System.Runtime.InteropServices;

namespace FrostyEditor
{
    static class FileUnblocker
    {
        // The name of the alternate data stream that has Internet Security Zone details
        private const string StreamName = ":Zone.Identifier:$DATA";

        /// <summary>
        /// Throws an exception if the current Win32 error code is a failure.
        /// </summary>
        private static void CheckNativeError()
        {
            int HResult = Marshal.GetHRForLastWin32Error();
            if (HResult < 0)
            {
                Marshal.ThrowExceptionForHR(HResult);
            }
        }

        /// <summary>
        /// Determines whether a path to a file system object such as a file or directory is valid.
        /// </summary>
        /// <param name="pszPath">A pointer to a null-terminated string of maximum length MAX_PATH that contains the full path of the object to verify.</param>
        /// <returns>Returns TRUE if the file exists, or FALSE otherwise. Call GetLastError for extended error information.</returns>
        [DllImport("shlwapi.dll", EntryPoint = "PathFileExistsW", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PathFileExists([MarshalAs(UnmanagedType.LPTStr)]string pszPath);

        /// <summary>
        /// Determines whether the given file has internet zone information associated with it.
        /// </summary>
        /// <param name="filename">The path to the file to be checked for zone information.</param>
        /// <returns>True if the file has internet zone information associated with it, false otherwise.</returns>
        public static bool IsFileBlocked(string filename)
        {
            bool result = PathFileExists(filename + StreamName);
            // Check if the file is really not there or if there was some other error.
            if (!result)
            {
                try
                {
                    CheckNativeError();
                }
                catch (System.IO.FileNotFoundException)
                {
                    // Swallow FileNotFoundExceptions, those are fine.
                }
            }
            return result;
        }

        [DllImport("kernel32.dll", EntryPoint = "DeleteFileW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFileW([MarshalAs(UnmanagedType.LPWStr)]string lpFileName);

        /// <summary>
        /// Unblocks a single file.
        /// </summary>
        /// <param name="filename">The path to the file.</param>
        public static void UnblockFile(string filename)
        {
            if (IsFileBlocked(filename))
            {
                if (!DeleteFileW(filename + StreamName))
                    CheckNativeError();
            }
        }

        /// <summary>
        /// Recursively unblocks all files in the given folder.
        /// </summary>
        /// <param name="path">The folder to start recursion from.</param>
        public static void UnblockDirectory(string path)
        {
            try
            {
                foreach (string filename in System.IO.Directory.EnumerateFiles(path))
                {
                    try
                    {
                        UnblockFile(filename);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception unblocking file '" + filename + "' : " + ex.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception unblocking files in '" + path + "' : " + e.ToString());
            }
            // Recurse!
            try
            {
                foreach (string directory in System.IO.Directory.EnumerateDirectories(path))
                {
                    UnblockDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception unblocking folders in '" + path + "' : " + ex.ToString());
            }
        }
    }
}
