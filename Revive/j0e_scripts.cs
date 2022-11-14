
namespace Revive_Injector
{
    public class j0e_scripts
    {

        private string MissionFolder;

        script[] j0e_files;



        public j0e_scripts(string MissionFolder, int paramID)
        {
            this.MissionFolder = MissionFolder;

            this.j0e_files = new script[] {
            new script("fwatch\\fwkia.sqs",baza.ReadSQS(@"fwatch\fwkia.sqs")),
            new script("miss\\cltrig.sqs",baza.ReadSQS(@"miss\cltrig.sqs")),
            new script("net\\mon.sqs",baza.ReadSQS(@"net\mon.sqs")),
            new script("net\\read.sqs",baza.ReadSQS(@"net\read.sqs")),
            new script("net\\send.sqs",baza.ReadSQS(@"net\send.sqs")),
            new script("revive\\bodies.sqs",baza.ReadSQS(@"revive\bodies.sqs")),
            new script("revive\\camdlg.sqs",baza.ReadSQS(@"revive\camdlg.sqs")),
            new script("revive\\climnu.sqs",baza.ReadSQS(@"revive\climnu.sqs")),
            new scriptRevive("revive\\main.sqs", paramID, baza.DEBUG_ISPVP,baza.ReadSQS(@"revive\main.sqs")),
        };
        }

        public void createScripts()
        {
            foreach (script script in j0e_files)
            {
                script.generateFile(MissionFolder);
            }
        }

    }
}
