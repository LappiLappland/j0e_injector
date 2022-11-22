using System.IO;

namespace Revive_Injector
{
    class Mission
    {

        private string MissionFolder;
        private InformationStorage IFstor;
        public Mission(string MissionFolder, InformationStorage IFstor)
        {
            this.MissionFolder = MissionFolder;
            this.IFstor = IFstor;
            if (IFstor.revive == pohja.MISSION_REVIVE.j0e ||
                IFstor.revive == pohja.MISSION_REVIVE.old_j0e)
            {
                this.IFstor.paramID = getParamID();
            }
        }

        public string Convert()
        {
            string CurrentFile;

            string log;
            string Text = "";

            //mission.sqm

            CurrentFile = Path.Combine(MissionFolder, "mission.sqm");

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

                IFstor.PlayersName = missionSQM.getPlayers();

            }

            pohja.AddFiles(CurrentFile, Text);

            //description.ext

            CurrentFile = Path.Combine(MissionFolder, "description.ext");

            pohja.CreateEmptyFile(CurrentFile);
            using (StreamReader sr = new StreamReader(CurrentFile, pohja.ANSI))
            {
                Text = sr.ReadToEnd();

                ext descriptionEXT = new ext(Text);

                Text = descriptionEXT.generateEXT(IFstor.paramID, IFstor.revive);

                IFstor.paramID = descriptionEXT.getParamID();

            }


            pohja.AddFiles(CurrentFile, Text);

            //Init.sqs

            CurrentFile = Path.Combine(MissionFolder, "init.sqs");

            pohja.CreateEmptyFile(CurrentFile);
            using (StreamReader sr = new StreamReader(CurrentFile, pohja.ANSI))
            {
                Text = sr.ReadToEnd();

                init initSQS = new init(Text, IFstor.PlayersName);

                Text = initSQS.generateInit(IFstor.revive);

            }

            pohja.AddFiles(CurrentFile, Text);

            //j0e_scripts

            j0e_scripts j0e_scripts = new j0e_scripts(MissionFolder, IFstor.paramID);
            j0e_scripts.createScripts();

            //Make list of players for log
            string logPlayers = getStringOfPlayers();

            string extra = string.Empty;
            if (IFstor.revive == pohja.MISSION_REVIVE.old_j0e)
                extra = " (old j0e converted to injected version)";
            if (IFstor.revive == pohja.MISSION_REVIVE.j0e)
                extra = " (injection updated)";

            log = $"Mission %%%MIS%%%: Conversion success. (EndingID = {IFstor.endID}) (ParamID = {IFstor.paramID}) (j0e_players = {logPlayers}){extra}";
            return log;
        }

        private string getStringOfPlayers()
        {
            string logPlayers = "[ ";
            foreach (string name in IFstor.PlayersName)
            {
                logPlayers += name + " ";
            }
            logPlayers += "]";
            return logPlayers;
        }

        private int getParamID()
        {
            int give = -1;
            string path = Path.Combine(MissionFolder, @"j0e_pack\Revive\main.sqs");

            if (File.Exists(path))
            {
                string t = File.ReadAllText(path, pohja.ANSI);
                int start = t.IndexOf("<param>", System.StringComparison.OrdinalIgnoreCase);
                if (start == -1) start = 0;
                start = t.IndexOf("param", start+3, System.StringComparison.OrdinalIgnoreCase);
                give = (int)char.GetNumericValue(t[start+5]);
            }

            return give;
        }

        public static pohja.MISSION_REVIVE CheckRevive(string MissionFolder)
        {
            pohja.MISSION_REVIVE reviveType = pohja.MISSION_REVIVE.NONE;

            string f = Path.Combine(MissionFolder, "init.sqs");

            if (!File.Exists(f)) return pohja.MISSION_REVIVE.NONE;

            string i = File.ReadAllText(f);
            string s = File.ReadAllText(Path.Combine(MissionFolder, "mission.sqm"));

            if (i.Contains("j0e_players"))
            {
                reviveType = i.Contains("_j0e_players") ? pohja.MISSION_REVIVE.j0e : pohja.MISSION_REVIVE.old_j0e;

            }
            else if (i.Contains("ReviveList") || i.Contains("ReviveListTmp"))
            {
                reviveType = pohja.MISSION_REVIVE.Kegetys;
            }
            else if (s.Contains("process_killed_event.sqs"))
            {
                reviveType = pohja.MISSION_REVIVE.Zigo;
            }

            return reviveType;

        }

    }
}
