using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlexHelpers.Common
{
    public static class ExtensionMethods
    {
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo directoryInfo, params string[] extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException("extensions");
            }
            IEnumerable<FileInfo> files = directoryInfo.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension.ToLower()));
        }

        public static IEnumerable<FileInfo> GetFilesByExtensionsRecursive(this DirectoryInfo directoryInfo, params string[] extensions)
        {
            if (extensions == null)
            {
                throw new ArgumentNullException("extensions");
            }
            List<FileInfo> files = directoryInfo.EnumerateFiles().ToList();
            foreach (var directory in directoryInfo.EnumerateDirectories())
            {
                files.AddRange(GetFilesByExtensionsRecursive(directory, extensions));
            }
            return files.Where(f => extensions.Contains(f.Extension.ToLower()));
        }
    }
}
