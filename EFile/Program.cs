using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EFile
{
    internal class Program
    {
        const string indexFilename = ".fileIndex";
        static void GetFiles(ref List<string> files, string directory, string extensions)
        {
            var extensionsList = extensions.Split('|').ToList();
            var filesInThisDir = new List<string>();
            extensionsList.ForEach(x => filesInThisDir.AddRange(Directory.GetFiles(directory, "*." + x)));
            var subdirs = Directory.GetDirectories(directory);
            foreach (var dir in subdirs)
            {
                GetFiles(ref files, dir, extensions);
            }
            files.AddRange(filesInThisDir);
        }
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length == 1 && args[0] == "/index")
                {
                    var listOfFiles = new List<string>();
                    var currentDir = Directory.GetCurrentDirectory();
                    GetFiles(ref listOfFiles, currentDir, "c|h|cpp");
                    var filedic = listOfFiles.ToLookup(x => Path.GetFileName(x));
                    using (var streamWriter = new StreamWriter(indexFilename))
                    {
                        foreach (var groupEntry in filedic)
                        {
                            foreach (var filename in groupEntry)
                            {
                                streamWriter.WriteLine($"{groupEntry.Key}|{filename}");
                            }
                        }
                    }
                }
                else if (args.Length > 0)
                {
                    var indexPath = GetIndexPath();
                    if (indexPath != null)
                    {
                        var lines = File.ReadAllLines(indexPath);
                        var lookup = lines.ToLookup(x => x.Split('|').First());
                        foreach (var filename in args)
                        {
                            var results = lookup[filename];
                            if (results.Count() > 1)
                            {
                                foreach (var matchedFilename in results)
                                {
                                    Console.WriteLine($"{matchedFilename}");
                                }
                            }
                            else if (results.Count() == 0)
                            {
                                var pattern = "^" + filename;
                                var matches = lookup.Where(x => Regex.Match(x.Key, pattern).Success);
                                foreach (var matchgroup in matches)
                                {
                                    foreach (var match in matchgroup)
                                    {
                                        var matchedFilename = match.Split('|').Last();
                                        Console.WriteLine($"{matchedFilename}");
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }
        }

        private static string GetIndexPath(string directory=".")
        {
            string result = "";
            var pathname = Path.Combine(directory, indexFilename);
            if (File.Exists(pathname))
            {
                result = pathname;
            }
            else
            {
                var parentInfo = Directory.GetParent(directory);
                if (parentInfo != null)
                {
                    result = GetIndexPath(parentInfo.Name);
                }
            }
            return result;
        }
    }
}
