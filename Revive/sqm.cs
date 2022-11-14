using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace Revive_Injector
{
    class sqm
    {

        private string Text = "";
        private int vehiclesID = 5000;
        private int playerNameID = 0;
        private static int[] middlePos = new int[] { -800, 10, -800 };
        private List<string> PlayersName = new List<string>();

        private static string[] coords = new string[]
        {
                        $"{middlePos[0]},{middlePos[1]},{middlePos[2]}",
                        $"{middlePos[0]-15},{middlePos[1]},{middlePos[2]}",
                        $"{middlePos[0]+15},{middlePos[1]},{middlePos[2]}",

                        $"{middlePos[0]},{middlePos[1]},{middlePos[2]+12}",
                        $"{middlePos[0]-15},{middlePos[1]},{middlePos[2]+12}",
                        $"{middlePos[0]+15},{middlePos[1]},{middlePos[2]+12}",

                        $"{middlePos[0]},{middlePos[1]},{middlePos[2]-12}",
                        $"{middlePos[0]-15},{middlePos[1]},{middlePos[2]-12}",
                        $"{middlePos[0]+15},{middlePos[1]},{middlePos[2]-12}",
        };
        private static string[] sides = new string[]
        {
                    "respawn_EAST",
                    "respawn_WEST",
                    "respawn_GUER",
                    "respawn_CIV"
        };
        
        public sqm(string Text)
        {

            this.Text = Text;
        }

        public string[] getPlayers()
        {
            return PlayersName.ToArray();
        }

        public static string checkRevive(string Text)
        {
            string reviveType = "";

            if (Text.Contains("respawn.sqs"))
            {
                reviveType = "Kegetys";
            }
            else if (Text.Contains("j0e_server"))
            {
                reviveType = "j0e";
            }
            else if (Text.Contains("process_killed_event.sqs"))
            {
                reviveType = "Zigo";
            }

            return reviveType;
        }

        public string generateSQM()
        {
            AddonsAndAuto();
            PlayerNames();
            BuildFortress();
            PlayerAndRespawnMarkers();
            GrpendAndServer();
            AddEndTrigger();
            AddIntro();

            return Text;
        }

        /// <summary>
        /// class Intro must be presented in every mission.sqm
        /// </summary>
        /// <returns></returns>
        public bool checkIntro()
        {
            int index = Text.IndexOf("class Intro");
            if (index != -1)
            {
                Text = Text.Remove(Text.IndexOf("class Intro"));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// class Groups must be presented in every mission.sqm
        /// </summary>
        /// <returns></returns>
        public bool checkGroups()
        {
            int index = Text.IndexOf("class Groups");
            if (index != -1)
            {;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddonsAndAuto() // Because we use houses from Nogova
        {

            if (Text.Contains("\"bis_resistance\"")) return;

            foreach (string word in new string[] { "addOns[]=", "addOnsAuto[]=" })
            {
                int TextStart = Text.IndexOf(word, StringComparison.OrdinalIgnoreCase);

                if (TextStart != -1)
                {
                    int startPos = Text.IndexOf('{', TextStart);

                    int endPos = baza.IndexOfEnd(Text, startPos);

                    int hasitem = Text.IndexOf('\"',startPos,endPos-startPos);

                    string comma = Text.IndexOf('\"', startPos, endPos - startPos) != -1 ? "," : string.Empty ;

                    Text = Text.Insert(startPos + 1, $"\n\t\t\"bis_resistance\"{comma}\n\t\t");

                }
                else
                {
                    int startPos = Text.IndexOf('{');

                    Text = Text.Insert(startPos + 1, $"\n{word}{{\"bis_resistance\"}};\n");
                }
            }

        }

        private void PlayerNames()
        {

            int TextStart;

            string[] searchFor = new string[]
            {
                "player=\"PLAY CDG\"",
                "player=\"PLAY C\"",
                "player=\"PLAY D\"",
                "player=\"PLAY G\"",
                "player=\"PLAY CD\"",
                "player=\"PLAY CG\"",
                "player=\"PLAY DG\"",
                "player=\"PLAYER COMMANDER\"",
                "player=\"PLAYER DRIVER\"",
                "player=\"PLAYER GUNNER\"",
            };

            foreach (string item in searchFor)
            {
                TextStart = Text.IndexOf(item, StringComparison.OrdinalIgnoreCase);
                while (TextStart != -1)
                {
                    int classStartIndex = Text.LastIndexOf("class", TextStart);
                    int classCloseIndex = baza.IndexOfEnd(Text, classStartIndex);


                    //We search for name of unit
                    int nameIndex = Text.IndexOf("text=", classStartIndex, classCloseIndex - classStartIndex);
                    //If unit has name, we add it to our PlayersName list
                    if (nameIndex != -1)
                    {
                        int startPos = Text.IndexOf("=\"", nameIndex) + 2;
                        int endPos = Text.IndexOf("\";", startPos);
                        string name = Text.Substring(startPos, endPos - startPos);

                        //If unit has same name as some other one,
                        //We change it
                        //Very rare, but it does exist on some missions
                        if (PlayersName.Contains(name))
                        {
                            name = $"pncbis{playerNameID}";
                            playerNameID++;
                            Text = Text.Remove(startPos, endPos - startPos).Insert(startPos, name);
                        }

                        PlayersName.Add(name);
                    }
                    //If unit doesn't have name, we create one for him and add it to our PlayersName list
                    else
                    {
                        int startPos = Text.IndexOf("position[]=", classStartIndex);

                        //pncbis - doesn't mean anything
                        string name = $"pncbis{playerNameID}";
                        playerNameID++;
                        PlayersName.Add(name);

                        Text = Text.Insert(startPos, $"text=\"{name}\";\n\t\t\t\t\t");
                    }

                    TextStart = Text.IndexOf(item, classCloseIndex, StringComparison.OrdinalIgnoreCase);

                }
            }

        }

        private void BuildFortress()
        {
            //If there is any unit with side "EMPTY", then that means there is class Vehicles for objects
            int TextStart = Text.IndexOf("side=\"EMPTY\";", StringComparison.OrdinalIgnoreCase);
            if (TextStart != -1)
            {

                int classStartIndex = Text.LastIndexOf("class Vehicles", TextStart, StringComparison.OrdinalIgnoreCase);

                int numberOfItems = grabItems(classStartIndex);
                changeItems(classStartIndex, numberOfItems + 9);

                string classCode = "";

                foreach (string coordinate in coords)
                {
                    classCode += $"\t\tclass Item{numberOfItems}\n";
                    classCode += "\t\t{\n";
                    classCode += $"\t\t\tposition[]={{{coordinate}}};\n";
                    classCode += $"\t\t\tid={vehiclesID};\n";
                    classCode += "\t\t\tside=\"EMPTY\";\n";
                    classCode += "\t\t\tvehicle=\"Dum01\";\n";
                    classCode += "\t\t\tskill=1;\n"; // not sure if this is neccessary, but just in case
                    classCode += "\t\t};\n";

                    numberOfItems++;
                    vehiclesID++;
                }
                int classCloseIndex = baza.IndexOfEnd(Text, classStartIndex);
                Text = Text.Insert(classCloseIndex, classCode + "\t");

            }
            //Class doesn't exist, we create it ourselves
            else
            {
                string classCode = "";

                int numberOfItems = 0;

                classCode += "\tclass Vehicles\n";
                classCode += "\t{\n";
                classCode += "\t\titems=9;\n";
                foreach (string coordinate in coords)
                {
                    classCode += $"\t\tclass Item{numberOfItems}\n";
                    classCode += "\t\t{\n";
                    classCode += $"\t\t\tposition[]={{{coordinate}}};\n";
                    classCode += $"\t\t\tid={vehiclesID};\n";
                    classCode += "\t\t\tside=\"EMPTY\";\n";
                    classCode += "\t\t\tvehicle=\"Dum01\";\n";
                    classCode += "\t\t\tskill=1;\n"; // not sure if this is neccessary, but just in case
                    classCode += "\t\t};\n";

                    numberOfItems++;
                    vehiclesID++;
                }
                classCode += "\t};\n";

                insertAtEnd(classCode);

            }

        }

        private void PlayerAndRespawnMarkers()
        {
            int TextStart = Text.IndexOf("class Markers", StringComparison.OrdinalIgnoreCase);

            string classCode = "";

            //If class doesn't exist, we create it first
            if (TextStart == -1)
            {
                classCode = "";

                classCode += "\tclass Markers\n";
                classCode += "\t{\n";
                classCode += "\t\titems=0;\n";
                classCode += "\t};\n";

                insertAtEnd(classCode);
                TextStart = Text.IndexOf("class Markers");
            }

            int classStartIndex = TextStart;
            int classCloseIndex = baza.IndexOfEnd(Text, classStartIndex);



            int startPos;
            int endPos;
            //Grab amount of items
            int numberOfItems = grabItems(TextStart);
            int extraMarkers = 0;

            classCode = "";

            //First we work with respawn markers
            foreach (string markerName in sides)
            {


                int classPos = Text.IndexOf($"name=\"{markerName}\";", classStartIndex, classCloseIndex - classStartIndex, StringComparison.OrdinalIgnoreCase);

                //Marker exists
                //We just edit it then
                if (classPos != -1)
                {
                    int classStart = Text.LastIndexOf("class Item", classPos);
                    int classEnd = baza.IndexOfEnd(Text, classStart);
                    int classLength = classEnd - classStart;

                    //Insert position. Position should be always presented
                    startPos = Text.IndexOf("position[]={", classStart) + 12;
                    endPos = Text.IndexOf("};", startPos);
                    Text = Text.Remove(startPos, endPos - startPos).Insert(startPos, $"{middlePos[0]},{middlePos[1]+100},{middlePos[2]}");
                    classCloseIndex = baza.IndexOfEnd(Text, classStartIndex); //Update it
                    classEnd = baza.IndexOfEnd(Text, classStart); //Update it
                    classLength = classEnd - classStart; //Update it

                    //Marker type must be icon
                    startPos = Text.IndexOf("markerType=\"", classStart, classLength);
                    //Marker type is set... we must remove it
                    //So it will be default as Icon
                    if (startPos != -1)
                    {
                        Text = Text.Remove(startPos, Text.IndexOf("\";", startPos) - startPos + 2);
                        classCloseIndex = baza.IndexOfEnd(Text, classStartIndex); //Update it
                        classEnd = baza.IndexOfEnd(Text, classStart); //Update it
                        classLength = classEnd - classStart; //Update it
                    }

                    //A coordinate must be 1
                    startPos = Text.IndexOf("a=", classStart, classLength);
                    //A coordinate is set... we must remove it
                    //So it will be default as 1
                    if (startPos != -1)
                    {
                        Text = Text.Remove(startPos, Text.IndexOf(";", startPos) - startPos + 2);
                        classCloseIndex = baza.IndexOfEnd(Text, classStartIndex); //Update it
                        classEnd = baza.IndexOfEnd(Text, classStart); //Update it
                        classLength = classEnd - classStart; //Update it
                    }

                    //B coordinate must be 1
                    startPos = Text.IndexOf("b=", classStart, classLength);
                    //B coordinate is set... we must remove it
                    //So it will be default as 1
                    if (startPos != -1)
                    {
                        Text = Text.Remove(startPos, Text.IndexOf(";", startPos) - startPos + 2);
                        classCloseIndex = baza.IndexOfEnd(Text, classStartIndex); //Update it
                        classEnd = baza.IndexOfEnd(Text, classStart); //Update it
                        classLength = classEnd - classStart; //Update it
                    }

                }
                //No respawn markers
                //We must create them
                else
                {
                    classCode += $"\t\tclass Item{numberOfItems}\n";
                    classCode += "\t\t{\n";
                    classCode += $"\t\t\tposition[]={{{ middlePos[0]},{ middlePos[1]+100},{ middlePos[2]}}};\n";
                    classCode += $"\t\t\tname=\"{markerName}\";\n";
                    classCode += "\t\t\ttype=\"EMPTY\";\n";
                    classCode += "\t\t};\n";

                    numberOfItems++;
                }

            }

            //Now we work with player markers

            foreach (string markerName in PlayersName)
            {


                int classPos = Text.IndexOf($"name=\"{markerName}\";", classStartIndex, classCloseIndex - classStartIndex, StringComparison.OrdinalIgnoreCase);

                char[] additional = new char[] { 'D', 'G', 'C' };

                //Player marker exists
                //We just edit it then
                if (classPos != -1)
                {
                    int classStart = Text.LastIndexOf("class Item", classPos);
                    int classEnd = baza.IndexOfEnd(Text, classStart);
                    int classLength = classEnd - classStart;

                    //Insert marker picture. Picture should be always presented
                    startPos = Text.IndexOf("type=\"", classStart) + 6;
                    endPos = Text.IndexOf("\";", startPos);
                    Text = Text.Remove(startPos, endPos - startPos).Insert(startPos, "Marker");
                    classCloseIndex = baza.IndexOfEnd(Text, classStartIndex); //Update it
                    classEnd = baza.IndexOfEnd(Text, classStart); //Update it
                    classLength = classEnd - classStart; //Update it

                    //Marker color must be green
                    startPos = Text.IndexOf("colorName=\"", classStart, classLength);
                    //Marker Color is already set... let's edit it
                    if (startPos != -1)
                    {
                        endPos = Text.IndexOf("\";", startPos);
                        Text = Text.Remove(startPos, endPos - startPos).Insert(startPos, "colorName=\"ColorGreen");
                        classCloseIndex = baza.IndexOfEnd(Text, classStartIndex); //Update it
                        classEnd = baza.IndexOfEnd(Text, classStart); //Update it
                        classLength = classEnd - classStart; //Update it
                    }
                    //Marker color is not set... let's create it
                    else
                    {
                        Text = Text.Insert(classEnd, "\tcolorName=\"ColorGreen\";\n\t\t");
                        classCloseIndex = baza.IndexOfEnd(Text, classStartIndex); //Update it
                        classEnd = baza.IndexOfEnd(Text, classStart); //Update it
                        classLength = classEnd - classStart; //Update it
                    }

                    //Marker type must be icon
                    startPos = Text.IndexOf("markerType=\"", classStart, classLength);
                    //Marker type is set... we must remove it
                    //So it will be default as Icon
                    if (startPos != -1)
                    {
                        Text = Text.Remove(startPos, Text.IndexOf("\";", startPos) - startPos + 2);
                        classCloseIndex = baza.IndexOfEnd(Text, classStartIndex); //Update it
                        classEnd = baza.IndexOfEnd(Text, classStart); //Update it
                        classLength = classEnd - classStart; //Update it
                    }

                    //A coordinate must be 1
                    startPos = Text.IndexOf("a=", classStart, classLength);
                    //A coordinate is set... we must remove it
                    //So it will be default as 1
                    if (startPos != -1)
                    {
                        Text = Text.Remove(startPos, Text.IndexOf(";", startPos) - startPos + 2);
                        classCloseIndex = baza.IndexOfEnd(Text, classStartIndex); //Update it
                        classEnd = baza.IndexOfEnd(Text, classStart); //Update it
                        classLength = classEnd - classStart; //Update it
                    }

                    //B coordinate must be 1
                    startPos = Text.IndexOf("b=", classStart, classLength);
                    //B coordinate is set... we must remove it
                    //So it will be default as 1
                    if (startPos != -1)
                    {
                        Text = Text.Remove(startPos, Text.IndexOf(";", startPos) - startPos + 2);
                        classCloseIndex = baza.IndexOfEnd(Text, classStartIndex); //Update it
                        classEnd = baza.IndexOfEnd(Text, classStart); //Update it
                        classLength = classEnd - classStart; //Update it
                    }

                    //Still have to create markers for vehicles
                    foreach (char letter in additional)
                    {
                        classCode += $"\t\tclass Item{numberOfItems}\n";
                        classCode += "\t\t{\n";
                        classCode += $"\t\t\tposition[]={{{ middlePos[0]},{ middlePos[1]},{ middlePos[2]}}};\n";
                        classCode += $"\t\t\tcolorName=\"ColorGreen\";\n";
                        classCode += "\t\t\ttype=\"Marker\";\n";
                        classCode += $"\t\t\tname=\"{markerName}{letter}\";\n";
                        classCode += "\t\t};\n";
                        numberOfItems++;
                    }


                }
                //No player marker
                //We must create it ourselves
                else
                {
                    classCode += $"\t\tclass Item{numberOfItems}\n";
                    classCode += "\t\t{\n";
                    classCode += $"\t\t\tposition[]={{{ middlePos[0]},{ middlePos[1]},{ middlePos[2]}}};\n";
                    classCode += $"\t\t\tcolorName=\"ColorGreen\";\n";
                    classCode += "\t\t\ttype=\"Marker\";\n";
                    classCode += $"\t\t\tname=\"{markerName}\";\n";
                    classCode += "\t\t};\n";
                    numberOfItems++;

                    foreach (char letter in additional)
                    {
                        classCode += $"\t\tclass Item{numberOfItems}\n";
                        classCode += "\t\t{\n";
                        classCode += $"\t\t\tposition[]={{{ middlePos[0]},{ middlePos[1]},{ middlePos[2]}}};\n";
                        classCode += $"\t\t\tcolorName=\"ColorGreen\";\n";
                        classCode += "\t\t\ttype=\"Marker\";\n";
                        classCode += $"\t\t\tname=\"{markerName}{letter}\";\n";
                        classCode += "\t\t};\n";
                        numberOfItems++;
                    }

                }

            }


            //To make it look more clean
            Text = Text.Insert(classCloseIndex, classCode + "\t");

            changeItems(TextStart, numberOfItems);

            //Delete extra respawn markers
            foreach (string markerName in sides)
            {
                string pattern = string.Format("\"({0}).+\"", Regex.Escape(markerName));
                Regex regex = new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
                Match matche = regex.Match(Text);
                while (matche.Success)
                {
                    Text = regex.Replace(Text, $"\"PNCBISF{extraMarkers}\"", 1);
                    extraMarkers++;
                    matche = regex.Match(Text);
                }
            }

        }

        private void GrpendAndServer()
        {
            int TextStart = Text.IndexOf("class Groups", StringComparison.OrdinalIgnoreCase);

            //entire class Groups
            int classStartIndex = TextStart;

            //Grab amount of items
            int numberOfItems = grabItems(classStartIndex);

            //Swap items amount to the new one. Because we add 2 groups to the list
            changeItems(classStartIndex, numberOfItems + 2);

            string classCode = "";


            classCode += $"\t\tclass Item{numberOfItems}\n";
            classCode += "\t\t{\n";
            classCode += "\t\t\tside=\"LOGIC\";\n"; // Making sure we don't go over limit of groups
            classCode += "\t\t\tclass Vehicles\n";
            classCode += "\t\t\t{\n";
            classCode += "\t\t\t\titems=1;\n";
            classCode += $"\t\t\t\tclass Item0\n";
            classCode += "\t\t\t\t{\n";
            classCode += $"\t\t\t\t\tposition[]={{{ middlePos[0]},{ middlePos[1] + 100},{ middlePos[2]}}};\n";
            classCode += $"\t\t\t\t\tid={vehiclesID};\n";
            classCode += "\t\t\t\t\tside=\"LOGIC\";\n"; // Making sure we don't go over limit of groups
            classCode += "\t\t\t\t\tvehicle=\"Civilian\";\n";
            classCode += "\t\t\t\t\tleader=1;\n";
            classCode += "\t\t\t\t\tskill=1;\n";
            classCode += "\t\t\t\t\ttext=\"j0e_grpend\";\n";
            classCode += "\t\t\t\t};\n";
            classCode += "\t\t\t};\n";
            classCode += "\t\t};\n";
            vehiclesID++;

            classCode += $"\t\tclass Item{numberOfItems + 1}\n";
            classCode += "\t\t{\n";
            classCode += "\t\t\tside=\"LOGIC\";\n";
            classCode += "\t\t\tclass Vehicles\n";
            classCode += "\t\t\t{\n";
            classCode += "\t\t\t\titems=1;\n";
            classCode += $"\t\t\t\tclass Item0\n";
            classCode += "\t\t\t\t{\n";
            classCode += $"\t\t\t\t\tposition[]={{{ middlePos[0]},{ middlePos[1]},{ middlePos[2]}}};\n";
            classCode += $"\t\t\t\t\tid={vehiclesID};\n";
            classCode += "\t\t\t\t\tside=\"Logic\";\n";
            classCode += "\t\t\t\t\tvehicle=\"Logic\";\n";
            classCode += "\t\t\t\t\tleader=1;\n";
            classCode += "\t\t\t\t\tskill=1;\n";
            classCode += "\t\t\t\t\ttext=\"j0e_server\";\n";
            classCode += "\t\t\t\t};\n";
            classCode += "\t\t\t};\n";
            classCode += "\t\t};\n";
            vehiclesID++;

            int classCloseIndex = baza.IndexOfEnd(Text, classStartIndex);
            Text = Text.Insert(classCloseIndex, classCode + "\t");

        }

        private void AddEndTrigger()
        {
            int TextStart = Text.IndexOf("class Sensors", StringComparison.OrdinalIgnoreCase);

            string classCode = "";

            //No class Seonsors, we create it
            if (TextStart == -1)
            {
                classCode += "\tclass Sensors\n";
                classCode += "\t{\n";
                classCode += "\t\titems=1;\n";
                classCode += "\t\tclass Item0\n";
                classCode += "\t\t{\n";
                classCode += "\t\t\tposition[]={0,0,0};\n";
                classCode += "\t\t\ta=5;\n";
                classCode += "\t\t\tb=5;\n";
                classCode += "\t\t\trectangular=1;\n";
                classCode += "\t\t\tactivationBy=\"ANY\";\n";
                classCode += $"\t\t\ttimeoutMin={baza.MISSION_END_TIME};\n";
                classCode += $"\t\t\ttimeoutMid={baza.MISSION_END_TIME};\n";
                classCode += $"\t\t\ttimeoutMax={baza.MISSION_END_TIME};\n";
                classCode += "\t\t\ttype=\"END6\";\n";
                classCode += "\t\t\tage=\"UNKNOWN\";\n";
                classCode += "\t\t\texpCond=\"(j0e_gameis!=0)\";\n";
                classCode += "\t\t\texpActiv=\"forceEnd\";\n";
                classCode += "\t\t\tclass Effects\n";
                classCode += "\t\t\t{\n";
                classCode += "\t\t\t};\n";
                classCode += "\t\t};\n";
                classCode += "\t};\n";

                baza.endID = 6;

                insertAtEnd(classCode);

            }

            else
            {
                //entire class Sensors
                int classStartIndex = TextStart;
                int classCloseIndex = baza.IndexOfEnd(Text, classStartIndex);

                int endID = -1;
                //Check which endings are free
                for (int i = 1; i < 7; i++)
                {
                    endID = Text.IndexOf($"\"END{i}\"",classStartIndex,classCloseIndex-classStartIndex);
                    if (endID == -1)
                    {
                        endID = i;
                        break;
                    }
                }


                //If there is no ending available, we just skip this
                //It's a very rare case that mission maker used all 6 of them
                //But if he did, then we shouldn't edit already existing ones
                //To keep things more stable
                if (endID != -1)
                {

                    //Grab amount of items
                    int numberOfItems = grabItems(classStartIndex);

                    //Swap items amount to the new one. Because we add 1 trigger to the list
                    changeItems(classStartIndex, numberOfItems + 1);
                    classCloseIndex += (numberOfItems + 2).ToString().Length - numberOfItems.ToString().Length;

                    classCode += $"\t\tclass Item{numberOfItems}\n";
                    classCode += "\t\t{\n";
                    classCode += "\t\t\tposition[]={0,0,0};\n";
                    classCode += "\t\t\ta=5;\n";
                    classCode += "\t\t\tb=5;\n";
                    classCode += "\t\t\trectangular=1;\n";
                    classCode += "\t\t\tactivationBy=\"ANY\";\n";
                    classCode += $"\t\t\ttimeoutMin={baza.MISSION_END_TIME};\n";
                    classCode += $"\t\t\ttimeoutMid={baza.MISSION_END_TIME};\n";
                    classCode += $"\t\t\ttimeoutMax={baza.MISSION_END_TIME};\n";
                    classCode += $"\t\t\ttype=\"END{endID}\";\n";
                    classCode += "\t\t\tage=\"UNKNOWN\";\n";
                    classCode += "\t\t\texpCond=\"(j0e_gameis!=0)\";\n";
                    classCode += "\t\t\texpActiv=\"forceEnd\";\n";
                    classCode += "\t\t\tclass Effects\n";
                    classCode += "\t\t\t{\n";
                    classCode += "\t\t\t};\n";
                    classCode += "\t\t};\n";

                    Text = Text.Insert(classCloseIndex, classCode);
                }
                baza.endID = endID;
            }
        }

        private void AddIntro()
        {
            string classCode = "";
            foreach (string str in (new string[] { "Intro", "OutroWin", "OutroLoose" }))
            {
                classCode += $"class {str}\n";
                classCode += "{\n";
                classCode += "\trandomSeed=3102211;\n";
                classCode += "\tclass Intel\n";
                classCode += "\t{\n";
                classCode += "\t};\n";
                classCode += "};\n";
            }

            Text = Text.Insert(Text.Length, classCode);
        }

        private int grabItems(int classStartIndex)
        {

            //Grab amount of items
            Regex regex = new Regex("items\\s*=\\s*([\\d.]+)\\s*(;|\\r\\n)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match matche = regex.Match(Text,classStartIndex);

            int numberOfItems = (int)Math.Round(float.Parse(matche.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture));

            return numberOfItems;
        }

        private void changeItems(int classStartIndex, int Number)
        {

            //Grab amount of items
            Regex regex = new Regex("items\\s*=\\s*([\\d.]+)\\s*(;|\\r\\n)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Text = regex.Replace(Text, $"items={Number};", 1, classStartIndex);

        }

        private void insertAtEnd(string insert)
        {

            int startPos = Text.LastIndexOf("};");

            Text = Text.Insert(startPos, insert);

        }

    }
}
