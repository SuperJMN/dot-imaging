#region Licence and Terms
// DotImaging Framework
// https://github.com/dajuric/dot-imaging
//
// Copyright © Darko Jurić, 2014-2016
// darko.juric2@gmail.com
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;
using System.IO;

namespace DotImaging
{
    /// <summary>
    /// <para>Defined functions can be used as object extensions.</para>
    /// Provides methods for string which is treated as file and directory path.
    /// </summary>
    internal static class PathExtensions
    {
        /// <summary>
        /// Returns an enumerable collection of file information that matches a specified search pattern and search subdirectory option.
        /// </summary>
        /// <param name="dirInfo">Directory info.</param>
        /// <param name="searchPatterns">The search strings (e.g. new string[]{ ".jpg", ".bmp" }</param>
        /// <param name="searchOption">
        /// One of the enumeration values that specifies whether the search operation
        /// should include only the current directory or all subdirectories. The default
        /// value is <see cref="System.IO.SearchOption.TopDirectoryOnly"/>.
        ///</param>
        /// <returns>An enumerable collection of files that matches <paramref name="searchPatterns"/> and <paramref name="searchOption"/>.</returns>
        public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo dirInfo, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var fileInfos = new List<FileInfo>();
            foreach (var searchPattern in searchPatterns)
            {
                var dirFileInfos = dirInfo.EnumerateFiles(searchPattern, searchOption);
                fileInfos.AddRange(dirFileInfos);
            }

            return fileInfos;
        }

        /// <summary>
        /// Gets whether the path is child path.
        /// </summary>
        /// <param name="childPath">The child path.</param>
        /// <param name="parentPath">The parent path.</param>
        /// <returns>True if the child path is indeed child path (or the same) as parent path, otherwise false.</returns>
        public static bool IsSubfolder(this string childPath, string parentPath)
        {
            var parentUri = new Uri(parentPath);
            var childUri = new DirectoryInfo(childPath);
                
            while (childUri != null)
            {
                if (new Uri(childUri.FullName) == parentUri)
                {
                    return true;
                }

                childUri = childUri.Parent;
            }

            return false;
        }

        /// <summary>
        /// Gets relative file path regarding specified directory.
        /// </summary>
        /// <param name="fileName">Full file name and path.</param>
        /// <param name="dirInfo">
        /// Directory info of a directory path which serves as root.
        /// </param>
        /// <returns>Relative file path. In case the relative path could not be find the empty string is returned.</returns>
        public static string GetRelativeFilePath(this string fileName, DirectoryInfo dirInfo)
        {
            Stack<string> folders = new Stack<string>();

            var fileDirInfo = new FileInfo(fileName).Directory;

            var currDirInfo = fileDirInfo;
            while (currDirInfo != null)
            {
                if (String.Equals(currDirInfo.FullName.Trim(Path.DirectorySeparatorChar), 
                                  dirInfo.FullName.Trim(Path.DirectorySeparatorChar)))
                    break;

                folders.Push(currDirInfo.Name);
                currDirInfo = currDirInfo.Parent;
            }

            var folderPath = Path.Combine(folders.ToArray());
            var relativeFilePath = Path.Combine(folderPath,  new FileInfo(fileName).Name);

            return (Path.DirectorySeparatorChar + relativeFilePath).NormalizePathDelimiters();
        }

        /// <summary>
        /// Checks whether the path is file or directory.
        /// </summary>
        /// <param name="path">File or directory path.</param>
        /// <returns>
        /// True if the path is directory, false if the path is file. 
        /// Null is returned if the path does not exist or in case of an internal error.
        /// </returns>
        private static bool? IsDirectory(this string path)
        {
            try
            {
                System.IO.FileAttributes fa = System.IO.File.GetAttributes(path);
                bool isDirectory = false;
                if ((fa & FileAttributes.Directory) != 0)
                {
                    isDirectory = true;
                }

                return isDirectory;
            }
            catch 
            {
                return null;
            }
        }
    }
}
