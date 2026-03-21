using Microsoft.VisualBasic.FileIO;
using PlexHelpers.Common;
using System.Text.RegularExpressions;

class Program
{
    private static bool CanMove = true;
    private static bool CanDelete = true;

    private static string _source = @"J:\Media\TV Shows\";
    private static string _target = @"T:\TVShows\1080P\";

    private static string _sourceLocation = @"K:\Media\H\Media\tv-shows-migration.csv";
    private static string _logLocation = @"K:\Media\H\Media\tv-shows-migration-log.csv";
    private static string _errorLocation = @"K:\Media\H\Media\tv-shows-migration-errors.csv";

    private static Regex _yearMatch = new Regex(@"\((19[5-9]\d|20[0-2]\d|202[0-5])\)", RegexOptions.Compiled);

    public class SourceItem
    {
        public int Year;
        public string Path = "";
        public string Title = "";
        public string Destination = "";
        public string Source = "";
    }

    public class SourceItemComparer : IEqualityComparer<SourceItem>
    {
        public bool Equals(SourceItem x, SourceItem y)
        {
            return string.Equals(x.Path, y.Path) && string.Equals(x.Title, y.Title) && x.Year == y.Year;
        }

        public int GetHashCode(SourceItem obj)
        {
            if (obj == null) return 0;

            unchecked
            {
                int hash = 17;
                hash = hash * 23 + obj.Year.GetHashCode();
                hash = hash * 23 + (obj.Path?.GetHashCode() ?? 0);
                hash = hash * 23 + (obj.Title?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }

    /// <summary>
    /// Finds all empty subdirectories inside the given path (including nested ones).
    /// A directory is considered empty if it contains no files and no subdirectories.
    /// </summary>
    /// <param name="rootPath">The directory to scan</param>
    /// <returns>List of full paths to empty subdirectories</returns>
    public static List<string> FindEmptySubdirectories(string rootPath)
    {
        if (!Directory.Exists(rootPath))
        {
            throw new DirectoryNotFoundException($"Directory not found: {rootPath}");
        }

        var emptyDirs = new List<string>();

        // Use a stack to avoid deep recursion (good for very deep folder structures)
        var directories = new Stack<string>();
        directories.Push(rootPath);

        while (directories.Count > 0)
        {
            string currentDir = directories.Pop();

            try
            {
                // Get all files and subdirectories in current folder
                DirectoryInfo dir = new DirectoryInfo(currentDir);
                var files = dir.GetFilesByExtensions(Settings.VideoFileExtensions.ToArray());
                string[] subDirs = Directory.GetDirectories(currentDir);

                // If no files AND no subdirectories → it's empty
                if (files.Count() == 0 && subDirs.Length == 0)
                {
                    // We don't usually consider the root itself "empty" unless you want to
                    if (currentDir != rootPath)
                    {
                        emptyDirs.Add(currentDir);
                    }
                }

                // Push subdirectories for further checking
                foreach (string subDir in subDirs)
                {
                    directories.Push(subDir);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"Access denied: {currentDir}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing {currentDir}: {ex.Message}");
            }
        }

        return emptyDirs;
    }

    // Convenience method: just tell you if there are any empty folders
    public static bool HasEmptySubdirectories(string rootPath)
    {
        var empty = FindEmptySubdirectories(rootPath);
        return empty.Count > 0;
    }

    /// <summary>
    /// Checks if the folder ONLY contains subfolders named like "Season 1", "Season 02", etc.
    /// Returns true only if:
    ///   - There are NO files directly in this folder
    ///   - ALL subfolders match the "Season ##" pattern
    ///   - There is at least one subfolder (optional — remove check if empty is allowed)
    /// </summary>
    public static bool ContainsOnlySeasonFolders(string folderPath, bool caseSensitive = true)
    {
        if (!Directory.Exists(folderPath))
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");

        try
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            var files = dir.GetFilesByExtensions(Settings.VideoFileExtensions.ToArray());

            // No files allowed directly in this folder
            if (files.Any())
                return false;

            string[] subDirs = Directory.GetDirectories(folderPath);
            if (subDirs.Length == 0)
                return false; // or true — depending on if empty folder is "valid"

            // Define the pattern
            // ^Season\s+\d{1,2}$   → "Season" + space(s) + 1–2 digits, nothing else
            string pattern = caseSensitive
                ? @"^Season\s+\d{1,2}$"
                : @"^(?i)Season\s+\d{1,2}$";

            var regex = new Regex(pattern, RegexOptions.CultureInvariant);

            return subDirs.All(dir =>
            {
                string folderName = Path.GetFileName(dir);
                return regex.IsMatch(folderName);
            });
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Access denied: {folderPath}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking {folderPath}: {ex.Message}");
            return false;
        }
    }


    static void Main(string[] args)
    {
        var tvShows = new List<SourceItem>();

        var sourceItems = File.ReadAllLines(_sourceLocation);
        var completedItems = File.ReadAllLines(_logLocation);

        for (var i = 0; i < sourceItems.Length; i++)
        {
            TextFieldParser parser = new TextFieldParser(new StringReader(sourceItems[i]));
            parser.HasFieldsEnclosedInQuotes = true;
            parser.SetDelimiters(",");

            string[] parts = null;

            while (!parser.EndOfData)
            {
                parts = parser.ReadFields();
            }

            if (!parts[1].StartsWith(_source))
            {
                continue;
            }

            parts[1] = parts[1].Replace(_source, "");

            var sourceItem = new SourceItem
            {
                Path = parts[1].Substring(0, parts[1].IndexOf(@"\")),
                Title = parts[2],
            };
            sourceItem.Source = _source + sourceItem.Path;
            int parseInt;
            if (parts.Length > 0 && int.TryParse(parts[0], out parseInt))
            {
                sourceItem.Year = parseInt;
            }

            tvShows.Add(sourceItem);
        }
        tvShows = tvShows.OrderBy(p => p.Title).Distinct(new SourceItemComparer()).ToList();


        var lines = tvShows.Select(p => p.Year + "," + Helpers.EscapeCsvField(p.Title) + "," + Helpers.EscapeCsvField(p.Source)).ToList();

        File.AppendAllLines(@"K:\Media\H\Media\tv-shows-migration-list.csv", lines);

        foreach (var tvShow in tvShows)
        {
            if (!Directory.Exists(tvShow.Source))
            {
                var error = "ERROR!" + tvShow.Source + " Not Found.";
                if (CanMove)
                {
                    File.AppendAllLines(_errorLocation, new List<string> { error });
                }
                Console.WriteLine(error);
                continue;
            }

            if (completedItems.Contains(tvShow.Source))
            {
                var error = "ERROR! " + tvShow.Source + ". Already Processed.";
                // File.AppendAllLines(_logLocation, new List<string> { error });
                Console.WriteLine(error);
                continue;
            }

            #region Check Source Name and Destination Name

            var matches = _yearMatch.Matches(tvShow.Title);

            if (matches.Count() == 0)
            {
                tvShow.Destination = _target + Helpers.ReplaceInvalidFilePathChars(tvShow.Title) + " (" + tvShow.Year + ")";
            }
            else if (matches.Count() == 1)
            {
                if (string.Equals(matches[0].Value, "(" + tvShow.Year + ")"))
                {
                    tvShow.Destination = _target + Helpers.ReplaceInvalidFilePathChars(tvShow.Title);
                }
                else
                {
                    var error = "ERROR! " + tvShow.Source + " has conflicting years.";
                    if (CanMove)
                    {
                        File.AppendAllLines(_errorLocation, new List<string> { error });
                    }
                    Console.WriteLine(error);
                }
            }
            else
            {
                var error = "ERROR! " + tvShow.Source + " has too many possible years.";
                if (CanMove)
                {
                    File.AppendAllLines(_errorLocation, new List<string> { error });
                }
                Console.WriteLine(error);
            }

            if (Directory.Exists(tvShow.Destination))
            {
                var error = "ERROR!" + tvShow.Source + " : " + tvShow.Destination + " destination already exists.";
                if (CanMove)
                {
                    File.AppendAllLines(_errorLocation, new List<string> { error });
                }
                Console.WriteLine(error);
                continue;
            }

            #endregion

            #region Check Folder Structure

            bool isValid = ContainsOnlySeasonFolders(tvShow.Source, caseSensitive: true);

            if (!isValid)
            {
                var error = "ERROR! " + tvShow.Source + " has misnamed subdirectories";
                if (CanMove)
                {
                    File.AppendAllLines(_errorLocation, new List<string> { error });
                }
                Console.WriteLine(error);
                continue;
            }

            #endregion

            #region check for empty folders

            bool hasEmpty = HasEmptySubdirectories(tvShow.Source);
            if (hasEmpty)
            {
                var error = "ERROR! " + tvShow.Source + " has empty subdirectories.";
                if (CanMove)
                {
                    File.AppendAllLines(_errorLocation, new List<string> { error });
                }
                Console.WriteLine(error);
                continue;
            }

            #endregion

            var result = "Moving: " + tvShow.Source + " to: " + tvShow.Destination;

            Console.WriteLine(result);

            if (CanMove)
            {
                Helpers.DirectoryCopy(tvShow.Source, tvShow.Destination, true, true);
                File.AppendAllLines(_errorLocation, new List<string> { result });
                File.AppendAllLines(_logLocation, new List<string> { tvShow.Source });
            }
        }

        Console.WriteLine("Done");
        Console.ReadLine();
    }
}