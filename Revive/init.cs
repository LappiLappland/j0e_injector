﻿namespace Revive_Injector
{
    class init
    {

        private string Text;
        private string[] PlayersName;
        public init(string Text, string[] PlayersName)
        {
            this.Text = Text;
            this.PlayersName = PlayersName;
        }

        public string generateInit()
        {

            Text = Text.Insert(0, "\n;===DO NOT EDIT THIS PART===\n");
            execs();
            makeArray();
            Text = Text.Insert(0, "\n;===DO NOT EDIT THIS PART===\n");

            return Text;
        }

        private void execs()
        {
            string classCode = "0 Exec \"j0e_pack\\revive\\main.sqs\"\n0 Exec \"j0e_pack\\miss\\cltrig.sqs\"\nj0e_server globalChat \"§ Server and revive script will lag on start depending on amount of bots alive\"";

            Text = Text.Insert(0, classCode);

        }

        private void makeArray()
        {
            string classCode = "";

            classCode += "_j0e_players=[";

            for (int i = 0; i < PlayersName.Length; i++)
            {
                char end = i != (PlayersName.Length - 1) ? ',' : ']';
                classCode += $"\"{PlayersName[i]}\"{end}";
            }

            //If player was set as vehicle. It will put all crew inside of it as players, not vehicle itself.
            //One downside is even when only 1 crew is playable, 2 others will be set in script too
            //But it's ok
            classCode += "\nj0e_players = [];_j0e_add = [];_unit = objNull;\"_j0e_add = [];_unit=objNull;call format[{_unit = %1},_x];if (_unit == driver _unit) then {j0e_players=j0e_players+[_x]} else {if (!isNull driver _unit) then {_j0e_add = _j0e_add + [format[\"\"%1D\"\",_x]]};if (!isNull gunner _unit) then {_j0e_add = _j0e_add + [format[\"\"%1G\"\",_x]]};if (!isNull commander _unit) then {_j0e_add = _j0e_add + [format[\"\"%1C\"\",_x]]};j0e_players = j0e_players + _j0e_add}\" forEach _j0e_players";
            classCode += '\n';
            Text = Text.Insert(0, classCode);

        }




    }
}
