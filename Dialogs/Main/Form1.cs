using System;
using System.Windows.Forms;
using System.IO;

namespace WindowsForms_revive
{
    public partial class Form1 : Form
    {

        public static string pathToManager = String.Empty;
        private static string pathToFolder = String.Empty;

        public static bool st_multithread = false;
        public static bool st_brackets = true;
        public static bool st_isPVP = false;
        //public static bool st_semi = false;
        public static int st_type = 1;

        public Form1()
        {
            InitializeComponent();

            if (Properties.Settings.Default.STG_pathToFolder != String.Empty)
            {
                pathToFolder = Properties.Settings.Default.STG_pathToFolder;
                textBoxDir.Text = pathToFolder;
            }
            if (Properties.Settings.Default.STG_pathToManager != String.Empty)
            {
                pathToManager = Properties.Settings.Default.STG_pathToManager;
            }
            st_multithread = Properties.Settings.Default.STG_multhithread;
            st_brackets = Properties.Settings.Default.STG_brackets;
            st_isPVP = Properties.Settings.Default.STG_isPVP;
            //st_semi = Properties.Settings.Default.STG_semi;
            st_type = Properties.Settings.Default.STG_type;

            ToolTip toolTip = new ToolTip();

            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 500;
            toolTip.ShowAlways = true;

            toolTip.SetToolTip(this.buttonFolder, "Open windows folder selection");
            toolTip.SetToolTip(this.buttonSettings, "Open window with settings");
            toolTip.SetToolTip(this.buttonHelp, "Open window with instructions");
            toolTip.SetToolTip(this.textBoxDir, "Directory with missions to convert"); // Doesn't work
            toolTip.SetToolTip(this.buttonConvert, "Convert missions in selected directory");

        }

        private void button2_Click(object sender, EventArgs e)
        {

            FormSettings settingsDialog = new FormSettings();

            if (settingsDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }



        }

        private void buttonFolder_Click(object sender, EventArgs e)
        {
            /*if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            textBoxDir.Text = folderBrowserDialog.SelectedPath;*/

            var dlg = new FolderPicker();
            //dlg.InputPath = textBoxDir.Text;
            if (dlg.ShowDialog(IntPtr.Zero) == true)
            {
                textBoxDir.Text = dlg.ResultPath;
            }

        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {

            if (!Directory.Exists(textBoxDir.Text))
            {
                MessageBox.Show("Directory doesn't exist!");
                return;
            }



            Revive_Injector.baza.DEBUG_ENABLE_MULTITHREAD = st_multithread;
            Revive_Injector.baza.DEBUG_REWRITE = (Revive_Injector.baza.REWRITE_TYPE)(st_type + 1);
            Revive_Injector.baza.DEBUG_DELETE_ANY_BRACKETS = st_brackets;
            Revive_Injector.baza.DEBUG_ISPVP = st_isPVP;
            //Revive_Injector.baza.DEBUG_IGNORESEMICOLONSERROR = st_semi;
            Revive_Injector.baza.FinalLog = "";

            bool status = Revive_Injector.Program.ConvertMission(textBoxDir.Text, pathToManager);

            System.Media.SystemSounds.Exclamation.Play();

            if (status)
            {
                MessageBox.Show("Conversion completed!", "J0e_injector", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Conversion completed, but with errors!\nCheck log.txt for more information.", "J0e_injector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.STG_pathToFolder = textBoxDir.Text;
            Properties.Settings.Default.STG_location = Location;
            Properties.Settings.Default.Save();
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            helpWindow helpDialog = new helpWindow();

            helpDialog.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.STG_location.X != -800)
            {
                Location = Properties.Settings.Default.STG_location;
            }
        }
    }
}
