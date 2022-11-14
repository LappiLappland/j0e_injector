using System;
using System.Windows.Forms;
using System.IO;

namespace WindowsForms_revive
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();

            this.ShowIcon = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            comboBoxType.Items.Add("Rewrite existing folders");
            comboBoxType.Items.Add("Create new folders");
            comboBoxType.Items.Add("UNPBO and REPBO");
            comboBoxType.Items.Add("Only UNPBO");
            comboBoxType.Items.Add("Only REPBO");

            openFileDialog1.Filter = "PBO Manager Console|pboc.exe";

            //checkBox4.Checked = Form1.st_semi;
            checkBox3.Checked = Form1.st_isPVP;
            checkBox2.Checked = Form1.st_brackets;
            checkBox1.Checked = Form1.st_multithread;
            comboBoxType.SelectedIndex = Form1.st_type;
            if (Form1.pathToManager != string.Empty)
            {
                textBoxPathManager.Text = Form1.pathToManager;
            }

            ToolTip toolTip = new ToolTip();

            toolTip.AutoPopDelay = 10000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 500;
            toolTip.ShowAlways = true;

            toolTip.SetToolTip(this.comboBoxType, "Choose how to convert missions. Check help menu for more information!");
            toolTip.SetToolTip(this.button1, "Path to PBO manager console app. Check help menu for more information!");
            toolTip.SetToolTip(this.checkBox1, "Speeds up conversion, but only if there is a lot of missions to convert.\nOtherwise, it will be slowed down.");
            toolTip.SetToolTip(this.checkBox2, "Delete first brackets from original mission name.\nExample: [PvE-15][ABC]Assault.Eden will be turned into [Revg-15][ABC]Assault.Eden");
            toolTip.SetToolTip(this.checkBox3, "In case you want PVP mission with revives.\nPlayers from different teams can't:\n - Revive each other\n - Spectate each other\n - See markers of each other\n - See death messages about each other\n - Game Over is disabled");
            toolTip.SetToolTip(this.buttonClose, "Accept and save changes.");


        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            textBoxPathManager.Text = openFileDialog1.FileName;

        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            if (textBoxPathManager.Text == "PATH TO PBO MANAGER") textBoxPathManager.Text = String.Empty;
            //System.ArgumentException
            if (textBoxPathManager.Text != String.Empty)
            {
                try
                {
                    FileInfo PBOManager = new FileInfo(textBoxPathManager.Text);
                    if (!PBOManager.Exists || PBOManager.Name != "pboc.exe")
                    {
                        MessageBox.Show("PBO manager path is incorrect!");
                        return;
                    }
                }
                catch (ArgumentException exception)
                {
                    MessageBox.Show("PBO manager path is incorrect!");
                    return;
                }
            }

            if (comboBoxType.SelectedIndex > 1 && textBoxPathManager.Text == String.Empty)
            {
                MessageBox.Show("Any pbo option requires you to select PBO manager path!");
                return;
            }

            Properties.Settings.Default.STG_pathToManager = textBoxPathManager.Text;
            Properties.Settings.Default.STG_multhithread = checkBox1.Checked;
            Properties.Settings.Default.STG_brackets = checkBox2.Checked;
            Properties.Settings.Default.STG_isPVP = checkBox3.Checked;
            //Properties.Settings.Default.STG_semi = checkBox4.Checked;
            Properties.Settings.Default.STG_type = comboBoxType.SelectedIndex;
            Properties.Settings.Default.Save();

            Form1.pathToManager = textBoxPathManager.Text;
            Form1.st_multithread = checkBox1.Checked;
            Form1.st_brackets = checkBox2.Checked;
            Form1.st_isPVP = checkBox3.Checked;
            //Form1.st_semi = checkBox4.Checked;
            Form1.st_type = comboBoxType.SelectedIndex;
        }
    }
}

