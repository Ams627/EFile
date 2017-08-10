using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFile
{
    internal class Program
    {
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
                var listOfFiles = new List<string>();
                GetFiles(ref listOfFiles, @"Q:\classictvm", "c|h|cpp");
                var filedic = listOfFiles.ToLookup(x => Path.GetFileName(x));
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }
        }
    }
}
