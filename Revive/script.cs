using System.IO;

namespace Revive_Injector
{
    public class script
    {
        protected string FileName;
        protected string FileText;

        public script() { }
        public script(string FileName, string FileText)
        {
            this.FileName = FileName;
            this.FileText = FileText;
        }

        virtual public void generateFile(string MissionFolder)
        {
            MissionFolder = Path.Combine(MissionFolder,"j0e_pack");
            Directory.CreateDirectory(MissionFolder);

            string[] folders = FileName.Split('\\');
            for (int i = 0; i < folders.Length - 1; i++)
            {
                string path = Path.Combine(MissionFolder, folders[i]);
                Directory.CreateDirectory(path);
            }

            pohja.CreateEmptyFile(Path.Combine(MissionFolder, FileName));
            File.WriteAllText(Path.Combine(MissionFolder, FileName), FileText, pohja.ANSI);

        }

    }

    public class scriptRevive : script
    {

        public scriptRevive() { }

        public scriptRevive(string FileName, int paramID, bool isPVP, bool isDistance2D, string FileText)
        {
            this.FileName = FileName;
            this.FileText = paramID != -1 ? FileText.Replace("%%%PARAM%%%", $"Param{paramID}") : FileText.Replace("%%%PARAM%%%", "1");
            this.FileText = isPVP ? this.FileText.Replace("%%%ISPVP%%%", "TRUE") : this.FileText.Replace("%%%ISPVP%%%", "FALSE");
            this.FileText = isDistance2D ? this.FileText.Replace("%%%DISTANCE_TYPE%%%", "[getPosASL _un, getPosASL _dumy] call j0e_distance") : this.FileText.Replace("%%%DISTANCE_TYPE%%%", "_un Distance _dumy");
        }

    }

}
