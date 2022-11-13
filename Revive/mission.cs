using System.IO;

namespace Revive_Injector
{
    class mission
    {

        private string MissionFolder;
        private string MissionName;
        private InformationStorage IFbaza;
        public mission(string MissionFolder, string MissionFolderOld, InformationStorage IFbaza)
        {
            this.MissionFolder = MissionFolder;
            this.IFbaza = IFbaza;

            MissionName = MissionFolderOld.Remove(MissionFolderOld.Length - 1);
            MissionName = MissionName.Remove(0, MissionName.LastIndexOf('\\')+1);
        }

        public string doWork()
        {
            string CurrentFile;

            string log;
            string Text = "";

            CurrentFile = MissionFolder + "mission.sqm";

            if (!File.Exists(CurrentFile))
            {
                log = $"Mission {MissionName}: Conversion failed. Error - mission.sqm was not found.";
                return log;
            }
            using (StreamReader sr = new StreamReader(CurrentFile, baza.ANSI))
            {

                Text = sr.ReadToEnd();

                sqm missionSQM = new sqm(Text);

                if (!missionSQM.checkIntro() | !missionSQM.checkGroups())
                {
                    log = $"Mission {MissionName}: Conversion failed. Error - mission.sqm has wrong structure.";
                    return log;
                }

                Text = missionSQM.generateSQM();

                IFbaza.PlayersName = missionSQM.getPlayers();

                //DEBUG OUTPUT
                if (baza.DEBUG_OUTPUT)
                {
                    using (StreamWriter sw = new StreamWriter(MissionFolder + "test2.txt"))
                    {
                        sw.Write(Text);
                    }
                }
            }

            baza.AddFiles(CurrentFile, Text);

            CurrentFile = MissionFolder + "description.ext";

            //Create description.ext if it doesn't exist
            baza.CreateEmptyFile(CurrentFile);
            using (StreamReader sr = new StreamReader(CurrentFile, baza.ANSI))
            {
                Text = sr.ReadToEnd();

                ext descriptionEXT = new ext(Text);

                /*if (!baza.DEBUG_IGNORESEMICOLONSERROR)
                {
                    string checkDesc = descriptionEXT.isDangerousFile();

                    if (checkDesc.Length > 0)
                    {
                        log = $"Mission {MissionName}: Conversion failed. Error - description.ext misses semicolons. {checkDesc}";
                        return log;
                    }
                }*/

                Text = descriptionEXT.generateEXT();

                IFbaza.paramID = descriptionEXT.getParamID();

                //DEBUG OUTPUT
                if (baza.DEBUG_OUTPUT)
                {
                    using (StreamWriter sw = new StreamWriter(MissionFolder + "testEXT.txt"))
                    {
                        sw.WriteLine(Text);
                    }
                }
            }


            baza.AddFiles(CurrentFile, Text);

            //Init.sqs

            CurrentFile = MissionFolder + "init.sqs";

            baza.CreateEmptyFile(CurrentFile);
            using (StreamReader sr = new StreamReader(CurrentFile, baza.ANSI))
            {
                Text = sr.ReadToEnd();

                init initSQS = new init(Text, IFbaza.PlayersName);

                Text = initSQS.generateInit();

                if (baza.DEBUG_OUTPUT)
                {
                    //DEBUG OUTPUT
                    using (StreamWriter sw = new StreamWriter(MissionFolder + "testInit.txt"))
                    {
                        sw.WriteLine(Text);
                    }
                }
            }

            baza.AddFiles(CurrentFile, Text);

            //j0e_scripts

            j0e_scripts j0e_scripts = new j0e_scripts(MissionFolder, IFbaza.paramID);
            if (baza.DEBUG_REWRITE > 0)
            {
                j0e_scripts.createScripts();
            }

            //Make list of players for log
            string logPlayers = "[ ";
            foreach (string name in IFbaza.PlayersName)
            {
                logPlayers += name + " ";
            }
            logPlayers += "]";

            log = $"Mission {MissionName}: Conversion success. (EndingID = {IFbaza.endID}) (ParamID = {IFbaza.paramID}) (j0e_players = {logPlayers})";
            return log;
        }

    }
}
