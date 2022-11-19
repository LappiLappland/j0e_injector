using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Revive_Injector
{

    static class Program
    {
        public static pohja.CONVERSION_RESULT ConvertMission(string currentFolder, string managerPBO)
        {

            currentFolder = currentFolder.TrimEnd('\\');

            string[] Directories = { };

            //UNPBO files, if need
            if (pohja.DEBUG_REWRITE == pohja.REWRITE_TYPE.UNPBO_REPBO || pohja.DEBUG_REWRITE == pohja.REWRITE_TYPE.UNPBO_ONLY)
            {

                DirectoryInfo cF = new DirectoryInfo(currentFolder);
                FileInfo[] PBOs = cF.GetFiles("*.pbo");

                List<string> PBOd = new List<string>();

                foreach  (FileInfo f in PBOs)
                {
                    PBOd.Add(Path.Combine(f.DirectoryName,Path.GetFileNameWithoutExtension(f.FullName)));
                }

                Directories = PBOd.ToArray();

                pohja.Unpbo(currentFolder, managerPBO);

            }
            else
            {
                Directories = Directory.GetDirectories(currentFolder);
            }

            //Convert missions
            if (pohja.DEBUG_ENABLE_MULTITHREAD)
            {
                Thread[] threads = new Thread[Directories.Length];
                int i = 0;
                foreach (string DIRECTORY in Directories)
                {

                    threads[i] = new Thread(() => ConvertMissionInFolder(DIRECTORY, currentFolder));
                    threads[i].Start();
                    i++;
                }
                for (int j = 0; j < threads.Length; j++)
                {
                    threads[j].Join();
                    Console.WriteLine(Directories[j]);
                }
                for (int j = 0; j < threads.Length; j++)
                {
                    threads[j].Abort();
                }
            }
            else
            {
                foreach (string DIRECTORY in Directories)
                {
                    Console.WriteLine(DIRECTORY);
                    ConvertMissionInFolder(DIRECTORY, currentFolder);
                }
            }

            //Nothing was converted
            //Maybe user selected mission folder?
            //Try to convert it instead
            if (pohja.FinalLog == String.Empty)
            {
                //No, he didn't
                if (!File.Exists(Path.Combine(currentFolder,"mission.sqm")))
                {
                    return pohja.CONVERSION_RESULT.NOTHING;
                }

                pohja.DEBUG_REWRITE = pohja.REWRITE_TYPE.REWRITE_EXISTING;

                //We want to create backup of mission
                string fol = Path.GetDirectoryName(currentFolder);
                string mis = Path.GetFileName(currentFolder) + ".backup";
                string backupFolder = Path.Combine(fol, mis);
                if (Directory.Exists(backupFolder)) Directory.Delete(backupFolder, true);
                Directory.CreateDirectory(backupFolder);
                pohja.CopyDirectory(currentFolder, backupFolder, true);

                bool result = ConvertMissionInFolder(currentFolder, currentFolder);

                //Conversion failed. Turn backup into original
                if (!result)
                {
                    Directory.Delete(currentFolder, true);
                    Directory.Move(backupFolder, currentFolder);
                    using (StreamWriter sw = new StreamWriter(Path.Combine(currentFolder, "!Log.txt"), false))
                    {
                        sw.Write(pohja.FinalLog);
                    }
                    return pohja.CONVERSION_RESULT.ONE_FOLDER_ERROR;
                }


                return pohja.CONVERSION_RESULT.ONE_FOLDER_SUCCESS;
            }

            //Create !CONVERTED folder
            string convertedPath = Path.Combine(currentFolder, "!CONVERTED");
            if (pohja.DEBUG_REWRITE != pohja.REWRITE_TYPE.REWRITE_EXISTING)
                Directory.CreateDirectory(convertedPath);

            //Repbo folders, if need
            if (pohja.DEBUG_REWRITE == pohja.REWRITE_TYPE.REPBO_ONLY || pohja.DEBUG_REWRITE == pohja.REWRITE_TYPE.UNPBO_REPBO)
            {
                string[] DirectoriesConv = Directory.GetDirectories(convertedPath);

                pohja.Repbo(convertedPath, managerPBO);

                foreach (string dir in DirectoriesConv)
                {
                    Directory.Delete(dir, true);
                }
            }

            //Create log file
            string logPath = 
                pohja.DEBUG_REWRITE != pohja.REWRITE_TYPE.REWRITE_EXISTING ? 
                Path.Combine(convertedPath, "!Log.txt") : Path.Combine(currentFolder, "!Log.txt");

            using (StreamWriter sw = new StreamWriter(logPath, false))
            {
                sw.Write(pohja.FinalLog);
            }

            //Report status
            if (pohja.FinalLog.Contains("Conversion failed"))
            {
                return pohja.CONVERSION_RESULT.ERROR;
            }
            else
            {
                return pohja.CONVERSION_RESULT.SUCCESS;
            }

        }

        private static bool ConvertMissionInFolder(string DIRECTORY, string currentFolder)
        {

            string MissionFolder = DIRECTORY;
            InformationStorage IFbaza = new InformationStorage();

            if (!File.Exists(Path.Combine(MissionFolder, "mission.sqm")))
            {
                return false;
            }

            //Check if there is already revive in the mission
            string alreadyHasRevive = sqm.checkRevive(File.ReadAllText(Path.Combine(MissionFolder, "mission.sqm")));
            string mis = Path.GetFileName(MissionFolder); //Mission name

            if (alreadyHasRevive.Length > 0)
            {
                if (alreadyHasRevive != "j0e")
                {
                    pohja.FinalLog += $"Mission {mis}: Conversion failed. Error - mission.sqm already has revive. ({alreadyHasRevive})\n";
                    return false;
                }
                else
                {
                    if (!File.Exists(Path.Combine(MissionFolder, "init.sqs")) || init.isOldj0e(File.ReadAllText(Path.Combine(MissionFolder, "init.sqs"))))
                    {
                        //Delete folders from UNPBO
                        if (pohja.DEBUG_REWRITE == pohja.REWRITE_TYPE.UNPBO_ONLY ||
                            pohja.DEBUG_REWRITE == pohja.REWRITE_TYPE.UNPBO_REPBO)
                        {
                            Directory.Delete(MissionFolder, true);
                        }

                        pohja.FinalLog += $"Mission {mis}: Conversion failed. Error - mission.sqm already has revive. (old version of j0e, remove j0e script manually and try again)\n";
                        return false;
                    }
                } 
            }

            //Create new directory for mission
            int start = MissionFolder.LastIndexOf('\\');
            string newMissionFolder = MissionFolder;

            newMissionFolder = newMissionFolder.Insert(start + 1, "%%%REPLACE%%%");

            if (pohja.DEBUG_REWRITE == pohja.REWRITE_TYPE.CREATE_NEW_FOLDER || 
                pohja.DEBUG_REWRITE == pohja.REWRITE_TYPE.REPBO_ONLY ||
                pohja.DEBUG_REWRITE == pohja.REWRITE_TYPE.REWRITE_EXISTING)
            {
                Directory.CreateDirectory(newMissionFolder);
                pohja.CopyDirectory(MissionFolder, newMissionFolder, true);
            }
            else
            {
                Directory.Move(MissionFolder, newMissionFolder);
            }

            MissionFolder = newMissionFolder;

            //Create and edit all the neccessary files
            mission mission = new mission(MissionFolder, IFbaza);
            string log = "";
            try
            {
                log = mission.Convert();
            }
            catch //Delete temp folder if program crashes
            {

                if (Directory.Exists(MissionFolder))
                {
                    Directory.Delete(MissionFolder, true);
                }

                pohja.FinalLog += $"Mission {mis}: Conversion failed. Error - unknown error. (Please report)\n";

                return false;
            }

            if (!log.Contains("Conversion failed"))
            {

                //Create !CONVERTED folder
                string convertedPath = Path.Combine(currentFolder, "!CONVERTED");
                if (pohja.DEBUG_REWRITE != pohja.REWRITE_TYPE.REWRITE_EXISTING)
                    Directory.CreateDirectory(convertedPath);

                if (pohja.DEBUG_REWRITE == pohja.REWRITE_TYPE.REWRITE_EXISTING)
                {
                    Directory.Delete(MissionFolder.Replace("%%%REPLACE%%%", String.Empty), true);
                }

                string newName = MissionFolder;

                //Remove brackets
                if (pohja.DEBUG_DELETE_ANY_BRACKETS)
                {
                    int startPos = MissionFolder.IndexOf('[', start);
                    int endPos = MissionFolder.IndexOf(']', start);
                    if (startPos != -1 && endPos != -1)
                    {
                        newName = MissionFolder.Remove(startPos, endPos - startPos + 1);
                    }
                }

                newName = newName.Replace("%%%REPLACE%%%", $"[Revg-{IFbaza.PlayersName.Length}]");

                if (pohja.DEBUG_REWRITE != pohja.REWRITE_TYPE.REWRITE_EXISTING) 
                    newName = newName.Insert(newName.IndexOf("[Revg-"), @"!CONVERTED\");

                if (pohja.DEBUG_ISPVP) 
                    newName = newName.Insert(newName.IndexOf("[Revg-")+5, "p" );

                if (Directory.Exists(newName))
                {
                    Directory.Delete(newName, true);
                }

                Directory.Move(MissionFolder, newName);

            }
            else
            {
                Directory.Delete(MissionFolder, true);
            }

            pohja.FinalLog += log.Replace("%%%MIS%%%", mis) + '\n';

            return true;
        }

    }

}

