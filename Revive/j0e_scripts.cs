
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
            new script(@"fwatch\fwkia.sqs",pohja.ReadSQS(@"fwatch\fwkia.sqs")),
            new script(@"miss\cltrig.sqs",pohja.ReadSQS(@"miss\cltrig.sqs")),
            new script(@"net\mon.sqs",pohja.ReadSQS(@"net\mon.sqs")),
            new script(@"net\read.sqs",pohja.ReadSQS(@"net\read.sqs")),
            new script(@"net\send.sqs",pohja.ReadSQS(@"net\send.sqs")),
            new script(@"revive\bodies.sqs",pohja.ReadSQS(@"revive\bodies.sqs")),
            new script(@"revive\camdlg.sqs",pohja.ReadSQS(@"revive\camdlg.sqs")),
            new script(@"revive\climnu.sqs",pohja.ReadSQS(@"revive\climnu.sqs")),
            new scriptRevive(@"revive\main.sqs", paramID, pohja.DEBUG_ISPVP, pohja.DEBUG_ISDISTANCE2D, pohja.ReadSQS(@"revive\main.sqs")),
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
