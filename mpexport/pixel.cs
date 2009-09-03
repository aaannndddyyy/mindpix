/*
    mindpixel object
    Copyright (C) 2009 Bob Mottram
    fuzzgun@gmail.com

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections;
using System.Text;

namespace mpexport
{
    class pixel
    {
        //types of bisector property
        public const int PROPERTY_ISA = 0;
        public const int PROPERTY_HAS = 1;
        public const int PROPERTY_LOCALITY = 2;
        public const int PROPERTY_COMPARISON = 3;
        public const int PROPERTY_CAUSALITY = 4;
        public const int PROPERTY_PERSONAL = 5;
        public const int PROPERTY_EMOTION = 6;
        public const int PROPERTY_TEMPORAL = 7;
        public const int PROPERTY_UTILITY = 8;
        public const int PROPERTY_EQUIVALENCE = 9;

        public const int PROPERTY_LIKES = 10;
        public const int PROPERTY_DISSLIKES = 11;
        public const int PROPERTY_MUSIC_PREFERENCE = 12;
        public const int PROPERTY_SKILL = 13;
        public const int PROPERTY_ANECDOTE = 14;
        public const int PROPERTY_RELATIONSHIP = 15;

        public float coherence = 0;     //true or false probability
        public string question = "";    //text of the question
        public string Prefix = "";
        public int Prefix_index = -1;
        public string Bisector = "";
        public int Bisector_index = -1;
        public string object1 = "";
        public bool object1_plural = false;
        public int object1_dictionary_index = 0;
        public string object2 = "";
        public bool object2_plural = false;
        public int object2_dictionary_index = 0;

        public float activation = 0;  //current level of activation

        public pixel()
        {
        }

        /// <summary>
        /// extract the prefix of the question
        /// </summary>
        private void extractPrefix(string[] prefixes, int no_of_prefixes)
        {
            int i;
            bool found = false;

            Prefix_index = -1;

            i = 0;
            while ((i < no_of_prefixes) && (!found))
            {
                if (question.Substring(0, prefixes[i].Length) == prefixes[i])
                {
                    Prefix = prefixes[i];
                    Prefix_index = i;
                    found = true;
                }
                else i++;
            }

        }

        private string cleanObject(string str, int index)
        {
            string[] tests;
            string word_ending;
            int max_test = 13;
            int i, pos;
            bool found = false;

            tests = new string[max_test];
            tests[0] = "a ";
            tests[1] = "of ";
            tests[2] = "his ";
            tests[3] = "her ";
            tests[4] = "an ";
            tests[5] = "the ";
            tests[6] = "to ";
            tests[7] = "type of ";
            tests[8] = "kind of ";
            tests[9] = "all ";
            tests[10] = "some ";
            tests[11] = "many ";
            tests[12] = "their ";

            i = 0;
            while ((i < max_test) && (!found))
            {
                pos = str.IndexOf(tests[i]);
                if (pos == 0)
                {
                    str = str.Substring(tests[i].Length);

                    if (i > 8)
                    {
                        //handle plurals
                        if (index == 0)
                            object1_plural = true;
                        else
                            object2_plural = true;
                    }

                    found = true;
                }
                else i++;
            }

            //is there an "s" at the end?
            if (str.Length > 3)
            {
                /// examine the last thee letters
                word_ending = str.Substring(str.Length - 3);
                if (word_ending != "ies")  //too many different varieties if ies endings
                {
                    if (word_ending.Substring(1) != "us")  //eg. genius, cactus
                    {
                        if (word_ending.Substring(1) != "ss")  //eg. loch ness
                        {
                            if (word_ending.Substring(1) != "is")  //eg. tennis
                            {
                                if (word_ending.Substring(2) == "s")
                                {
                                    str = str.Substring(0, str.Length - 1);
                                }
                            }
                        }
                    }
                }
            }

            //remove trailing spaces
            if (str.Substring(str.Length - 1) == " ") str = str.Substring(0, str.Length - 1);
			
			str = str.Trim();

            return (str);
        }

        /// <summary>
        /// chop the remaining string after the prefix in half
        /// </summary>
        private void chop(string[] bisectors, bool[,] bisectors_property_flags, int no_of_bisectors)
        {
            string qStr;
            int i, pos, pos2;
            bool found = false;

            qStr = question.Substring(Prefix.Length);

            Bisector_index = -1;

            i = 2;
            while ((i < no_of_bisectors) && (!found))
            {
                pos = qStr.IndexOf(bisectors[i]);
                if (pos > 1)
                {
                    object1 = cleanObject(qStr.Substring(0, pos), 0);
                    pos2 = pos + bisectors[i].Length;
                    object2 = cleanObject(qStr.Substring(pos2, qStr.Length - 1 - pos2), 1);
                    Bisector = bisectors[i];
                    Bisector_index = i;
                    if (bisectors_property_flags[i, PROPERTY_ISA])
                    {
                        Bisector = "is a";
                        Bisector_index = 0;
                    }
                    else
                    {
                        if (bisectors_property_flags[i, PROPERTY_HAS])
                        {
                            Bisector = "has";
                            Bisector_index = 1;
                        }
                    }
                    found = true;
                }
                else i++;
            }

            if (!found)
            {
                //karate chop
                string[] words = qStr.Split(' ');
                if (words.Length == 2)
                {
                    object1 = words[0];
                    object2 = words[1];
                    object2 = words[1].Substring(0, words[1].Length - 1);
                }
            }
        }

        public void update(string[] prefixes, int no_of_prefixes,
                           string[] bisectors, bool[,] bisectors_property_flags, int no_of_bisectors)
        {
            extractPrefix(prefixes, no_of_prefixes);
            chop(bisectors, bisectors_property_flags, no_of_bisectors);
        }
    }
}
