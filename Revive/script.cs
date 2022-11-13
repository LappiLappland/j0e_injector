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
            MissionFolder += "j0e_pack\\";
            Directory.CreateDirectory(MissionFolder);

            string onlyFolder = FileName.Substring(0, FileName.IndexOf('\\'));

            Directory.CreateDirectory(MissionFolder + onlyFolder);

            baza.CreateEmptyFile(MissionFolder + FileName);
            File.WriteAllText(MissionFolder + FileName, FileText, baza.ANSI);

        }

    }

    public class scriptRevive : script
    {

        public scriptRevive() { }

        public scriptRevive(string FileName, int paramID, bool isPVP, string FileText)
        {
            this.FileName = FileName;
            this.FileText = paramID != -1 ? FileText.Replace("%%%PARAM%%%", $"Param{paramID}") : FileText.Replace("%%%PARAM%%%", "1");
            this.FileText = isPVP ? this.FileText.Replace("%%%ISPVP%%%", "TRUE") : this.FileText.Replace("%%%ISPVP%%%", "FALSE");

        }

    }

}
