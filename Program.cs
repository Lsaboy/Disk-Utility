// Vedant Sharma
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace DiskUtility
{
    public class DiskUtility
    {
        private static String usage = "Usage: du [-s] [-p] [-b] <path> \n" +
                                      "Summarize disk usage of the set of FILES, recursively for directories.\n"+
                                      "You MUST specify one of the parameters, -s, -p, or -b\n"+
                                      "-s  \tRun in single threaded mode\n"+
                                      "-p  \tRun in parallel mode (uses all available processors)\n"+
                                      "-b  \tRun in both parallel and single threaded mode.\n"+
                                      "    \tRuns parallel followed by sequential mode";
        /// <summary>
        /// Main Function
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            bool single = false, multi = false;
            if (args.Length == 0)
            {
                Console.WriteLine(usage);
                return;
            }
            
            switch(args[0])
            {
                case "-s":
                    single = true;
                    break;
                case "-p":
                    multi = true;
                    break;
                case "-b":
                    single = multi = true;
                    break;
                default:
                    Console.WriteLine(usage);
                    return;
            }

            String path = "";
            for (int i = 1; i < args.Length; i++)
            {
                path += args[i] + " ";
            }
            path = path.Trim();
            
            if(!Directory.Exists(@path) && !File.Exists(@path))
            {
                Console.WriteLine("Please enter a valid path");
                return;
            }

            if (multi)
            {
                
            }
            if (single)
            {
                SingleThreaded.Start(@path);
            }

        }
    }

    public static class utilities
    {
        public static string AddQuotesIfRequired(string path)
        {
            return !string.IsNullOrWhiteSpace(path) ? 
                path.Contains(" ") && (!path.StartsWith("\"") && !path.EndsWith("\"")) ? 
                    "\"" + path + "\"" : path : 
                string.Empty;
        }
    }

    public static class SingleThreaded
    {
        private static int _numFiles, _numFolders;
        private static long _totalSize;

        public static void Start(String path)
        {
            var stopWatch = Stopwatch.StartNew();
            _numFiles = _numFolders = 0;
            _totalSize = 0;
            DirectoryTraverser(path);

            Console.WriteLine("Sequential Calculated in: " + (double)stopWatch.ElapsedMilliseconds/1000 + "s");
            stopWatch.Stop();
            Console.WriteLine("Folders: "+_numFolders+"\nFiles: "+_numFiles+"\nSize: "+_totalSize);
            
        }

        
        private static void DirectoryTraverser(String path)
        {
            if(File.Exists(path))
            {
                // This path is a file
                FileInfo fileInfo;
                try
                {
                    fileInfo = new FileInfo(path);
                }
                catch (Exception  e)
                {
                    Console.Error.WriteLine(e);
                    return;
                }
                _totalSize += fileInfo.Length;
                _numFiles++;
                return;
            }

            _numFolders++;
            String[] subdirectories = {""}, files = {""};
            //ar subdirectories = Directory.EnumerateFileSystemEntries(@path);
            try
            {
                subdirectories = Directory.GetDirectories(path);
            }
            catch (Exception  e)
            {
                Console.Error.WriteLine(e);
                return;
            }

            for (int i = 0; i < subdirectories.Length; i++)
            {
                DirectoryTraverser(subdirectories[i]);
            }

            try
            {
                files = Directory.GetFiles(path);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return;
            }

            for (int i = 0; i < files.Length; i++)
            {
                DirectoryTraverser(files[i]);
            }





            /*foreach (var directory in subdirectories)
            {
                DirectoryTraverser(directory);
            }
*/
            return;
        }
    }

    public class MultiThreaded
    {
        private int _numFiles, _numFolders;
        private long _totalSize;

        public void Start(String path)
        {
            var stopWatch = Stopwatch.StartNew();
            _numFiles = _numFolders = 0;
            _totalSize = 0;
            DirectoryTraverser(path);

            Console.WriteLine("Sequential Calculated in: " + (double)stopWatch.ElapsedMilliseconds/1000 + "s");
            stopWatch.Stop();
            Console.WriteLine("Folders: "+_numFolders+"\nFiles: "+_numFiles+"\nSize: "+_totalSize);

        }
    }
}
