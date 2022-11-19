using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace Revive_Injector
{
    /// <summary>
    /// Contains basic functions
    /// </summary>
    public static class pohja
    {

        public static int paramID;
        public static string[] PlayersName;
        public static int endID;
        public static string FinalLog = "";

        public static Assembly assembly = Assembly.GetExecutingAssembly();

        public const int MISSION_END_TIME = 10;
        private const int PBOMANAGER_LIMIT_LINES = 15;

        /// <summary>
        /// ERROR - we found several missions, but conversion failed
        /// SUCCESS - we found several missions and successed
        /// ONE_FOLDER_SUCCESS - we ound only one mission and successed
        /// ONE_FOLDER_ERROR - we found only one mission, but conversion failed
        /// NOTHING - we didn't find any missions
        /// IO_ERROR - IO threw exception
        /// ACCESS - User opened folder :/
        /// </summary>
        public enum CONVERSION_RESULT
        {
            ERROR = 0,
            SUCCESS = 1,
            ONE_FOLDER_SUCCESS = 2,
            ONE_FOLDER_ERROR = 3,
            NOTHING = 4,
            IO_ERROR = 5,
            ACCESS = 6,
        }
        /// <summary>
        /// NO_REWRITE - do nothing. (Use with DEBUG_OUTPUT)
        /// REWRITE_EXISTING - rewrite files inside original folder
        /// CREATE_NEW_FOLDER - keep original files, copy them to another directory and work with it instead. Folder name [Revg-PLAYERS]Originalname
        /// UNPBO_REPBO - turns every pbos into folders, edit them, pack them back into new pbo
        /// UNPBO_ONLY - same as UNPBO_REPBO, but without packing back into new pbo
        /// REPBO_ONLY - same as UNPBO_REPBO, but without turning pbos into folders
        /// </summary>
        public enum REWRITE_TYPE
        {
            REWRITE_EXISTING = 1,
            CREATE_NEW_FOLDER = 2,
            UNPBO_REPBO = 3,

            UNPBO_ONLY = 4,
            REPBO_ONLY = 5,
        }

        /// <summary>
        /// In which way we create new files
        /// Check REWRITE_TYPE for more info
        /// </summary>
        public static REWRITE_TYPE DEBUG_REWRITE = REWRITE_TYPE.UNPBO_REPBO;

        /// <summary>
        /// Work with each mission in separate thread
        /// </summary>
        public static bool DEBUG_ENABLE_MULTITHREAD = false;
        /// <summary>
        /// Remove brackets from mission name
        /// [PvE-10]Mission.island would be Mission.island
        /// </summary>
        public static bool DEBUG_DELETE_ANY_BRACKETS = true;
        /// <summary>
        /// Sets j0e_isPVP variable inside main.sqs
        /// </summary>
        public static bool DEBUG_ISPVP = true;
        /// <summary>
        /// Encoding used by OFP files
        /// </summary>
        public static Encoding ANSI = Encoding.GetEncoding("Windows-1250");
        /// <summary>
        /// Find closing brackets index
        /// </summary>
        /// <param name="Text">String to search in</param>
        /// <param name="startFrom">Starting index to search from. (It will first try to find opening brackets!)</param>
        /// <returns></returns>
        public static int IndexOfEnd(string Text, int startFrom)
        {

            startFrom = Text.IndexOf("{", startFrom);

            int startIndex = Text.IndexOf("{", startFrom + 1);
            int endIndex = Text.IndexOf("};", startFrom);

            if (startIndex == -1)
            {
                return endIndex;
            }

            while (startIndex < endIndex)
            {
                startIndex = Text.IndexOf("{", startIndex + 1);
                endIndex = Text.IndexOf("}", endIndex + 1);

                if (startIndex == -1)
                {
                    break;
                }
            }

            return endIndex;
        }
        /// <summary>
        /// Creates empty text file to work with.
        /// If text file already exists, does nothing
        /// </summary>
        /// <param name="CurrentFile">Path to file</param>
        public static void CreateEmptyFile(string CurrentFile)
        {
            if (!File.Exists(CurrentFile))
            {
                File.Create(CurrentFile).Dispose();
            }
        }
        /// <summary>
        /// Writes string into text file
        /// </summary>
        /// <param name="CurrentFile">File to write in</param>
        /// <param name="Text">String to write in file</param>
        public static void AddFiles(string CurrentFile, string Text)
        {
            using (StreamWriter sw = new StreamWriter(CurrentFile, false, ANSI))
            {
                sw.Write(Text);
            }
        }
        /// <summary>
        /// Copies everything from one folder to another. (Taken from microsoft.com)
        /// </summary>
        /// <param name="sourceDir">Copy what</param>
        /// <param name="destinationDir">Copy where</param>
        /// <param name="recursive">Copy inner folders</param>
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        /// <summary>
        /// Read file inside "Revive\Scripts" folder
        /// </summary>
        /// <param name="path">Path to file starting from "Revive\Scripts"</param>
        /// <returns></returns>
        public static string ReadSQS(string path)
        {
            string file = $"WindowsForms_revive.Revive.Scripts.{path.Replace(@"\",".")}";

            Stream stream = assembly.GetManifestResourceStream(file);
            StreamReader streamReader = new StreamReader(stream, ANSI);

            return streamReader.ReadToEnd();
        }

        public static void Unpbo(string currentFolder, string managerPBO)
        {

            DirectoryInfo currentFolderDI = new DirectoryInfo(currentFolder);
            FileInfo[] PBOs = currentFolderDI.GetFiles("*.pbo");

            double limit = PBOMANAGER_LIMIT_LINES;

            double count = (double)PBOs.Length / limit;

            string[] cmdPaths = new string[(int)Math.Ceiling(count)];

            string Paths = "";
            int i = 0;
            int j = 0;
            foreach (FileInfo pbo in PBOs)
            {
                Paths += $"\"{pbo.FullName}\" ";
                i++;
                if (i % limit == 0) 
                {
                    cmdPaths[j] = Paths;
                    Paths = "";
                    j++; 
                }
            }

            if (i % limit != 0) cmdPaths[j] = Paths;

            if (!DEBUG_ENABLE_MULTITHREAD)
            {
                foreach (string cmdPath in cmdPaths)
                {
                    RunCMD(currentFolder, managerPBO, cmdPath, true);
                }
            }
            else
            {
                Thread[] threads = new Thread[cmdPaths.Length];
                int b = 0;
                foreach (string cmdPath in cmdPaths)
                {
                    threads[b] = new Thread(() => RunCMD(currentFolder, managerPBO, cmdPath, true));
                    threads[b].Start();
                    b++;
                }
                for (b = 0; b < threads.Length; b++)
                {
                    threads[b].Join();
                }
                for (b = 0; b < threads.Length; b++)
                {
                    threads[b].Abort();
                }
            }
        }

        public static void Repbo(string currentFolder, string managerPBO, string whichFolder = "")
        {

            //string cmdPaths = whichFolder.Remove(whichFolder.Length - 1);

            string[] Directories = Directory.GetDirectories(currentFolder);

            double limit = PBOMANAGER_LIMIT_LINES;

            double count = (double)Directories.Length / limit;

            string[] cmdPaths = new string[(int)Math.Ceiling(count)];

            string Paths = "";
            int i = 0;
            int j = 0;
            foreach (string pbo in Directories)
            {
                Paths += $"\"{pbo}\" ";
                i++;
                if (i % limit == 0)
                {
                    cmdPaths[j] = Paths;
                    Paths = "";
                    j++;
                }
            }

            if (i % limit != 0) cmdPaths[j] = Paths;

            if (!DEBUG_ENABLE_MULTITHREAD)
            {
                foreach (string cmdPath in cmdPaths)
                {
                    RunCMD(currentFolder, managerPBO, cmdPath,false);
                }
            }
            else
            {
                Thread[] threads = new Thread[cmdPaths.Length];
                int b = 0;
                foreach (string cmdPath in cmdPaths)
                {
                    threads[b] = new Thread(() => RunCMD(currentFolder, managerPBO, cmdPath,false));
                    threads[b].Start();
                    b++;
                }
                for (b = 0; b < threads.Length; b++)
                {
                    threads[b].Join();
                }
                for (b = 0; b < threads.Length; b++)
                {
                    threads[b].Abort();
                }
            }

        }


        private static void RunCMD(string currentFolder, string managerPBO, string cmdPath, bool isUnpack)
        {
            string cmdText;

            if (isUnpack) cmdText = $"cd /d \"{currentFolder}\" && \"{managerPBO}\" unpack {cmdPath}";
            else cmdText = $"cd /d \"{currentFolder}\" && \"{managerPBO}\" pack {cmdPath}";

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(cmdText);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
        }
        


    }
}
