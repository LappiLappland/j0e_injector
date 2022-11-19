using System.IO;

namespace Revive_Injector
{
    class mission
    {

        private string MissionFolder;
        private InformationStorage IFbaza;
        public mission(string MissionFolder, InformationStorage IFbaza)
        {
            this.MissionFolder = MissionFolder;
            this.IFbaza = IFbaza;
        }

        public string Convert()
        {
            string CurrentFile;

            string log;
            string Text = "";

            //mission.sqm

            CurrentFile = Path.Combine(MissionFolder, "mission.sqm");

            if (!File.Exists(CurrentFile))
            {
                log = $"Mission %%%MIS%%%: Conversion failed. Error - mission.sqm was not found.";
                return log;
            }
            using (StreamReader sr = new StreamReader(CurrentFile, pohja.ANSI))
            {

                Text = sr.ReadToEnd();

                sqm missionSQM = new sqm(Text);

                if (!missionSQM.checkIntro() | !missionSQM.checkGroups())
                {
                    log = $"Mission %%%MIS%%%: Conversion failed. Error - mission.sqm has wrong structure.";
                    return log;
                }

                Text = missionSQM.generateSQM();

                IFbaza.PlayersName = missionSQM.getPlayers();

            }

            pohja.AddFiles(CurrentFile, Text);

            //description.ext

            CurrentFile = Path.Combine(MissionFolder, "description.ext");

            //Create description.ext if it doesn't exist
            pohja.CreateEmptyFile(CurrentFile);
            using (StreamReader sr = new StreamReader(CurrentFile, pohja.ANSI))
            {
                Text = sr.ReadToEnd();

                ext descriptionEXT = new ext(Text);

                Text = descriptionEXT.generateEXT();

                IFbaza.paramID = descriptionEXT.getParamID();

            }


            pohja.AddFiles(CurrentFile, Text);

            //Init.sqs

            CurrentFile = Path.Combine(MissionFolder, "init.sqs");

            pohja.CreateEmptyFile(CurrentFile);
            using (StreamReader sr = new StreamReader(CurrentFile, pohja.ANSI))
            {
                Text = sr.ReadToEnd();

                init initSQS = new init(Text, IFbaza.PlayersName);

                Text = initSQS.generateInit();

            }

            pohja.AddFiles(CurrentFile, Text);

            //j0e_scripts

            j0e_scripts j0e_scripts = new j0e_scripts(MissionFolder, IFbaza.paramID);
            j0e_scripts.createScripts();

            //Make list of players for log
            string logPlayers = getStringOfPlayers();

            log = $"Mission %%%MIS%%%: Conversion success. (EndingID = {IFbaza.endID}) (ParamID = {IFbaza.paramID}) (j0e_players = {logPlayers})";
            return log;
        }

        private string getStringOfPlayers()
        {
            string logPlayers = "[ ";
            foreach (string name in IFbaza.PlayersName)
            {
                logPlayers += name + " ";
            }
            logPlayers += "]";
            return logPlayers;
        }

    }
}
