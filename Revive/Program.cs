using System;
using System.IO;
using System.Threading;

namespace Revive_Injector
{

    static class Program
    {
        public static bool ConvertMission(string currentFolder, string managerPBO)
        {

            if (baza.DEBUG_REWRITE == baza.REWRITE_TYPE.UNPBO_REPBO || baza.DEBUG_REWRITE == baza.REWRITE_TYPE.UNPBO_ONLY)
            {

                baza.Unpbo(currentFolder, managerPBO);

            }

            string[] Directories = Directory.GetDirectories(currentFolder);

            if (baza.DEBUG_ENABLE_MULTITHREAD)
            {
                Thread[] threads = new Thread[Directories.Length];
                int i = 0;
                foreach (string DIRECTORY in Directories)
                {

                    threads[i] = new Thread(() => DoWork(DIRECTORY, currentFolder, new InformationStorage()));
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
                    DoWork(DIRECTORY, currentFolder, new InformationStorage());
                }
            }

            Directory.CreateDirectory(currentFolder + "\\!CONVERTED");

            if (baza.DEBUG_REWRITE == baza.REWRITE_TYPE.REPBO_ONLY || baza.DEBUG_REWRITE == baza.REWRITE_TYPE.UNPBO_REPBO)
            {
                string[] DirectoriesConv = Directory.GetDirectories(currentFolder + "\\!CONVERTED");

                baza.Repbo(currentFolder + "\\!CONVERTED", managerPBO);

                foreach (string dir in DirectoriesConv)
                {
                    Directory.Delete(dir, true);
                }
            }

            using (StreamWriter sw = new StreamWriter(currentFolder + "\\!CONVERTED\\!Log.txt", false))
            {
                sw.Write(baza.FinalLog);
            }

            if (baza.FinalLog.Contains("Conversion failed"))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        private static void DoWork(string DIRECTORY, string currentFolder, InformationStorage IFbaza)
        {

            string MissionFolder = DIRECTORY + '\\';
            string MissionFolderOld = MissionFolder;

            if (!File.Exists(MissionFolder + "mission.sqm"))
            {
                return;
            }

            string alreadyHasRevive = sqm.checkRevive(File.ReadAllText(MissionFolder + "mission.sqm"));
            string mis = DIRECTORY.Remove(0, DIRECTORY.LastIndexOf('\\') + 1);

            if (alreadyHasRevive.Length > 0)
            {
                baza.FinalLog += $"Mission {mis}: Conversion failed. Error - mission.sqm already has revive. ({alreadyHasRevive})\n";
                
                if (baza.DEBUG_REWRITE != baza.REWRITE_TYPE.REWRITE_EXISTING) Directory.Delete(MissionFolder, true);
                return;
            }

            if (baza.DEBUG_REWRITE == baza.REWRITE_TYPE.CREATE_NEW_FOLDER || baza.DEBUG_REWRITE == baza.REWRITE_TYPE.UNPBO_REPBO || baza.DEBUG_REWRITE == baza.REWRITE_TYPE.UNPBO_ONLY || baza.DEBUG_REWRITE == baza.REWRITE_TYPE.REPBO_ONLY)
            {
                int end = MissionFolder.LastIndexOf('\\');
                int start = MissionFolder.LastIndexOf('\\', end - 1);
                string newMissionFolder = MissionFolder;

                if (baza.DEBUG_DELETE_ANY_BRACKETS)
                {
                    int startPos = MissionFolder.IndexOf('[',start);
                    int endPos = MissionFolder.IndexOf(']', start);
                    if (startPos != -1 && endPos != -1)
                    {
                        newMissionFolder = MissionFolder.Remove(startPos, endPos-startPos+1);
                    }
                }

                newMissionFolder = newMissionFolder.Insert(start + 1, "%%%REPLACE%%%");

                if (baza.DEBUG_REWRITE == baza.REWRITE_TYPE.CREATE_NEW_FOLDER || baza.DEBUG_REWRITE == baza.REWRITE_TYPE.REPBO_ONLY)
                {
                    Directory.CreateDirectory(newMissionFolder);
                    baza.CopyDirectory(MissionFolder, newMissionFolder, true);
                }
                else
                {
                    Directory.Move(MissionFolder, newMissionFolder);
                }
                MissionFolder = newMissionFolder;
            }


            mission mission = new mission(MissionFolder, MissionFolderOld, IFbaza);
            string log = "";

            try
            {
                log = mission.doWork();
            }
            catch //Delete temp folder if program crashes
            {
                if (baza.DEBUG_REWRITE != baza.REWRITE_TYPE.REWRITE_EXISTING)
                {
                    Directory.Delete(MissionFolder, true);
                }


                baza.FinalLog += $"Mission {mis}: Conversion failed. Error - unknown error. (Please report)\n";

                return;
            }

            if (!log.Contains("Conversion failed"))
            {
                if (baza.DEBUG_REWRITE == baza.REWRITE_TYPE.CREATE_NEW_FOLDER || baza.DEBUG_REWRITE == baza.REWRITE_TYPE.UNPBO_REPBO || baza.DEBUG_REWRITE == baza.REWRITE_TYPE.REPBO_ONLY || baza.DEBUG_REWRITE == baza.REWRITE_TYPE.UNPBO_ONLY)
                {

                    Directory.CreateDirectory(currentFolder + "\\!CONVERTED");

                    string newName = MissionFolder.Replace("%%%REPLACE%%%", $"[Revg-{IFbaza.PlayersName.Length}]");

                    newName = newName.Insert(newName.IndexOf("[Revg-"), "!CONVERTED\\");

                    if (baza.DEBUG_ISPVP) newName = newName.Insert(newName.IndexOf("[Revg-")+5, "p" );


                    if (!Directory.Exists(newName))
                    {
                        Directory.Move(MissionFolder, newName);

                        //if (baza.DEBUG_REWRITE == baza.REWRITE_TYPE.UNPBO_REPBO || baza.DEBUG_REWRITE == baza.REWRITE_TYPE.REPBO_ONLY)
                        //{

                            //baza.Repbo(currentFolder + "\\!CONVERTED", managerPBO, newName);

                            //Directory.Delete(newName, true);

                        //}

                    }
                    else
                    {
                        Directory.Delete(MissionFolder, true);
                        mis = DIRECTORY.Remove(0, DIRECTORY.LastIndexOf('\\') + 1);
                        log = $"Mission {mis}: Conversion failed. Error - mission folder already exists in !CONVERTED.";

                    }
                }
            }
            else
            {
                if (baza.DEBUG_REWRITE != baza.REWRITE_TYPE.REWRITE_EXISTING)
                {
                    Directory.Delete(MissionFolder, true);
                }
            }

            baza.FinalLog += log + '\n';

        }


    }

}

