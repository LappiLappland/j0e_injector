using System;
using System.Text.RegularExpressions;

namespace Revive_Injector
{
    class ext
    {

        private string Text;

        private int freeID = -1;

        private string[] j0e_controls =
        {
            "ComboMOD",
            "ButtonMOD",
            "EditMOD",
        };

        public ext(string Text)
        {
            this.Text = Text;
        }

        public int getParamID()
        {
            return freeID;
        }

        public string generateEXT(int id, pohja.MISSION_REVIVE revive)
        {

            Respawns();

            if (id != -1)
            {
                freeID = id;
            }
            else
            {
                Params();
            }

            if (revive == pohja.MISSION_REVIVE.old_j0e)
            {
                EditDialog();
                EnableAI();
            }
            else if (revive != pohja.MISSION_REVIVE.j0e)
                CreateDialog();

            return Text;
        }

        private void Respawns()
        {

            string classCode = "\nRespawn=\"BASE\";\nRespawnDelay=5;\n\n";

            Text = Regex.Replace(Text, "(Respawn)(_|)(delay)( )*=( )*.+( )*(;|\\r\\n)", "\n\r", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

            Text = Regex.Replace(Text, "(Respawn)( )*=( )*.+( )*(;|\\r\\n)", "\n\r", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

            insertAtEnd(classCode);

        }

        private void Params()
        {

            for (int i = 1; i <= 2; i++)
            {
                if (Text.IndexOf($"TitleParam{i}", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    freeID = i;
                    break;
                }
            }

            if (freeID != -1)
            {
                string classCode = "";
                classCode += $"TitleParam{freeID}=\"Revive: \";\n";
                classCode += $"TextsParam{freeID}[]={{\"none\",\"free\",\"5 lifes\",\"5 mins\",\"medic\"}};\n";
                classCode += $"ValuesParam{freeID}[]={{0,1,2,3,4}};\n";
                classCode += $"DefValueParam{freeID}=1;\n";

                insertAtEnd(classCode);
            }

        }

        private void CreateDialog()
        {
            
            string classCode = "/*****************************************************************************\r\n*j0eReviveDialog  v3.0.6  05/02/2012  by j0e\r\n******************************************************************************/\r\nclass j0eText\r\n{\r\n  idc=-1;\r\n  type=0;\r\n  style=2;\r\n  colorText[]={1,1,1,1};\r\n  colorBackground[]={0,0,0,0.2};\r\n  font=\"CourierNewB64\";\r\n  sizeEx=0.020;\r\n};\r\nclass j0eButton\r\n{\r\n  idc=-1;\r\n  type=11;\r\n  style=2;\r\n  color[]={1,1,1,1};\r\n  colorActive[]={0,1,0,1};\r\n  font=\"CourierNewB64\";\r\n  sizeEx=0.020;\r\n  soundEnter[]={\"ui\\ui_over\",0.2,1};\r\n  soundPush[]={\"\",0.2,1};\r\n  soundClick[]={\"ui\\ui_ok\",0.2,1};\r\n  soundEscape[]={\"ui\\ui_cc\",0.2,1};\r\n  default=0;\r\n};\r\nclass j0eComboBox\r\n{\r\n  idc=-1;\r\n  type=4;\r\n  style=0;\r\n  colorText[]={1,1,1,1};\r\n  colorBackground[]={0.5,0.5,0.5,1};\r\n  colorSelect[]={0.5,0.5,0.5,1};\r\n  colorSelectBackground[]={0.2,0.2,0.2,1};\r\n  font=\"CourierNewB64\";\r\n  sizeEx=0.020;\r\n  rowHeight=0.030;\r\n  wholeHeight=0.360;\r\n};\r\nclass j0eSlider\r\n{\r\n  idc=-1;\r\n  type=3;\r\n  style=0x0F;\r\n  color[]={1,1,1,1};\r\n  sizeEx=0.020;\r\n};\r\nclass j0eEditBox\r\n{\r\n  idc=-1;\r\n  type=2;\r\n  style=0;\r\n  colorText[]={1,0,0,1};\r\n  colorSelection[]={0.5,0.5,0.5,1};\r\n  font=\"CourierNewB64\";\r\n  sizeEx=0.020;\r\n  autocomplete=0;\r\n  text=\"\";\r\n};\r\nclass j0eReviveDialog\r\n{\r\n  idd=21000;\r\n  movingEnable=0;\r\n  controls[]={\"ButtonNEXT\",\"ButtonPREV\",\"ButtonRESET\",\"ComboPLAYERS\",\"TextREVIVE\",\"TextPLAYER\",\r\n              \"SliderANGLE\",\"SliderDISTN\",\"SliderHEIGH\",\"TextANGLE\",\"TextDISTN\",\"TextHEIGH\",\r\n			  \"SliderSMOOTH\", \"TextSMOOTH\"};\r\n  class ButtonNEXT: j0eButton\r\n  {\r\n    idc=21007;\r\n    x=0.250; y=0.005; w=0.060; h=0.030;\r\n    text=\"Next\";\r\n    action=\"{nx} Exec {j0e_pack\\revive\\climnu.sqs}\";\r\n  };\r\n  class ButtonPREV: j0eButton\r\n  {\r\n    idc=21008;\r\n    x=0.250; y=0.040; w=0.060; h=0.030;\r\n    text=\"Prev\";\r\n    action=\"{pr} Exec {j0e_pack\\revive\\climnu.sqs}\";\r\n  };\r\n  class ButtonRESET: j0eButton\r\n  {\r\n    idc=21009;\r\n    x=0.250; y=0.075; w=0.060; h=0.030;\r\n    text=\"Reset\";\r\n    action=\"{rs} Exec {j0e_pack\\revive\\climnu.sqs}\";\r\n  };\r\n  class ComboPLAYERS: j0eComboBox\r\n  {\r\n    idc=21001;\r\n    x=0.005; y=0.005; w=0.240; h=0.030;\r\n  };\r\n  class TextREVIVE: j0eText\r\n  {\r\n    idc=21002;\r\n    x=0.320; y=0.005; w=0.090; h=0.030;\r\n    text=\"-\";\r\n  };\r\n  class TextPLAYER: j0eText\r\n  {\r\n    idc=21003;\r\n    x=0.005; y=0.040; w=0.240; h=0.030;\r\n    text=\"<UNK>\";\r\n  };\r\n  class SliderANGLE: j0eSlider\r\n  {\r\n    idc=21004;\r\n    x=0.005; y=0.075; w=0.185; h=0.030;\r\n  };\r\n  class SliderDISTN: j0eSlider\r\n  {\r\n    idc=21005;\r\n    x=0.005; y=0.110; w=0.185; h=0.030;\r\n  };\r\n  class SliderHEIGH: j0eSlider\r\n  {\r\n    idc=21006;\r\n    x=0.005; y=0.145; w=0.185; h=0.030;\r\n  };\r\n  class SliderSMOOTH: j0eSlider\r\n  {\r\n    idc=21017;\r\n    x=0.005; y=0.18; w=0.185; h=0.030;\r\n  };\r\n  class TextANGLE: j0eText\r\n  {\r\n    idc=21014;\r\n    x=0.195; y=0.075; w=0.045; h=0.030;\r\n    text=\"-\";\r\n  };\r\n  class TextDISTN: j0eText\r\n  {\r\n    idc=21015;\r\n    x=0.195; y=0.110; w=0.045; h=0.030;\r\n    text=\"-\";\r\n  };\r\n  class TextHEIGH: j0eText\r\n  {\r\n    idc=21016;\r\n    x=0.195; y=0.145; w=0.045; h=0.030;\r\n    text=\"-\";\r\n  };\r\n  class TextSMOOTH: j0eText\r\n  {\r\n    idc=21018;\r\n    x=0.195; y=0.18; w=0.045; h=0.030;\r\n    text=\"-\";\r\n  };\r\n};";
            insertAtEnd(classCode);

        }

        private void EditDialog()
        {
            string classCode = "\r\n\tclass TextSMOOTH: j0eText\r\n  {\r\n    idc=21018;\r\n    x=0.195; y=0.18; w=0.045; h=0.030;\r\n    text=\"-\";\r\n  };\r\n  class SliderSMOOTH: j0eSlider\r\n  {\r\n    idc=21017;\r\n    x=0.005; y=0.18; w=0.185; h=0.030;\r\n  };";
            int start = Text.IndexOf("TextHEIGH");
            start = Text.IndexOf('}', start);
            Text = Text.Insert(start + 2, classCode);
            Text = Text.Insert(start,",\"SliderSMOOTH\", \"TextSMOOTH\"");

            foreach (string str in j0e_controls)
            {
                Text = Text.Replace($"\"{str}\",", string.Empty);
            }

        }

        private void EnableAI()
        {
            // DisabledAI=1;
            Text = Regex.Replace(Text, "DisabledAI( )*=( )*.+(\\s|;)", string.Empty, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Inserts in the start !!
        /// </summary>
        /// <param name="insert"></param>
        private void insertAtEnd(string insert)
        {

            //int startPos = Text.Length;

            Text = Text.Insert(0, "//##########################\r\n" + insert + "\r\n//##########################\r\n\r\n\r\n");

        }

    }
}
