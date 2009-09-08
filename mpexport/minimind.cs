/*
    mpexport
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
using System.IO;
using System.Text;
using System.Collections;

namespace mpexport
{
	public class minimind
	{
		public minimind()
		{
            initialstring = "Mind Hack";
			nationality = new string[100];
			nation = new string[100];
			no_of_nationalities=0;
            prefix = new string[200];
            no_of_prefixes = 0;
            bisector = new string[2000];
            bisector_property_flags = new bool[2000,10];
            no_of_bisectors = 0;

            dictionary = new ArrayList();
            dictionary_mindpixel_lookup = new ArrayList();
            //dictionary_entries = 0;
            //dictionary_index = new int[26 * 26];
			
            createPrefixes();
            createBisectors();
			createNationalities();
		}
		
		#region "parsing the mindpixels"

        public ArrayList mindpixels;
		
        //these are common question prefixes
        //prefixes are detected and then removed, prior to chopping
        string[] prefix = null;
        int no_of_prefixes;
				
        //these words or phrases are used to chop each question in half
        string[] bisector = null;		
        bool[,] bisector_property_flags; //used to flag different classifications, such as "isa", "has", etc
        int no_of_bisectors;
        int no_of_parsed_pixels;
				
		string initialstring;
        StreamReader oRead = null;
		bool initialstringFound = true;
		
		#region "dictionary"
		
        // the dictionary stores individual words or short phrases
        //int[] dictionary_index; //used for searching the dictionary
        //int[] dictionary_wordcount; //the number of words in each dictionary entry
        ArrayList dictionary;
        //int dictionary_entries;
        ArrayList dictionary_mindpixel_lookup;		
		
        /// <summary>
        /// adds an entry to the dictionary
        /// </summary>
        /// <param name="word"></param>
        private void addDictionaryEntry(
		    string word, 
		    int mindpixel_index)
        {
            dictionary.Add(word);

            ArrayList mp = new ArrayList();
            mp.Add(mindpixel_index);
            dictionary_mindpixel_lookup.Add(mp);
        }
		
		#endregion

		int no_of_nationalities;
		string[] nationality;
		string[] nation;

        private void addNationality(
		    string Nationality, 
		    string Nation)
        {
			nationality[no_of_nationalities] = Nationality;
			nation[no_of_nationalities] = Nation;
			no_of_nationalities++;
        }
		
        private void addPrefix(string str)
        {
            if (no_of_prefixes < prefix.Length)
            {
                prefix[no_of_prefixes] = str.ToLower() + " ";
                no_of_prefixes++;
            }
        }
		
        //types of bisector property
		const int no_of_properties = 10;
        const int PROPERTY_ISA = 0;
        const int PROPERTY_HAS = 1;
        const int PROPERTY_LOCALITY = 2;
        const int PROPERTY_COMPARISON = 3;
        const int PROPERTY_CAUSALITY = 4;
        const int PROPERTY_PERSONAL = 5;
        const int PROPERTY_EMOTION = 6;
        const int PROPERTY_TEMPORAL = 7;
        const int PROPERTY_UTILITY = 8;
        const int PROPERTY_IDENTITY = 9;
				
        void addBisector(
		    string str, 
		    int emotion, 
		    bool isa, 
		    bool has, 
		    bool locality,
            bool comparison, 
		    bool causality, 
		    bool personal, 
		    bool temporal,
            bool utility, 
		    bool identity)
        {
            if (no_of_bisectors < bisector.Length)
            {
                bisector[no_of_bisectors] = " " + str.ToLower() + " ";
                bisector_property_flags[no_of_bisectors, PROPERTY_ISA] = isa;
                bisector_property_flags[no_of_bisectors, PROPERTY_HAS] = has;
                bisector_property_flags[no_of_bisectors, PROPERTY_LOCALITY] = locality;
                bisector_property_flags[no_of_bisectors, PROPERTY_COMPARISON] = comparison;
                bisector_property_flags[no_of_bisectors, PROPERTY_CAUSALITY] = causality;
                bisector_property_flags[no_of_bisectors, PROPERTY_PERSONAL] = personal;
                bisector_property_flags[no_of_bisectors, PROPERTY_EMOTION] = (Math.Abs(emotion) > 2);
                bisector_property_flags[no_of_bisectors, PROPERTY_TEMPORAL] = temporal;
                bisector_property_flags[no_of_bisectors, PROPERTY_UTILITY] = utility;
                bisector_property_flags[no_of_bisectors, PROPERTY_IDENTITY] = identity;
                no_of_bisectors++;
            }
        }

        void addBisector(
		    string str, 
		    int property)
        {
            if (no_of_bisectors < bisector.Length)
            {
                bisector[no_of_bisectors] = " " + str.ToLower() + " ";
				for (int i = 0; i < no_of_properties; i++) bisector_property_flags[no_of_bisectors, property] = false;
				bisector_property_flags[no_of_bisectors, property] = true;
                no_of_bisectors++;
            }
        }
		
        void loadGAC(int itterations, bool show_mindpixels)
        {
            int i = 0;
            string str, question;
            pixel pix;
            float coherence;
			Random rnd = new Random();

            while (!oRead.EndOfStream)
            {
                str = oRead.ReadLine();
                if (!initialstringFound)
                {
                    /// look for an initial header string after which the data begins
                    if (str.Contains(initialstring)) initialstringFound = true;
                }
                else
                {
                    /// read the data
                    if (str != "")
                    {
                        try
                        {
                            coherence = Convert.ToSingle(ToNumeric(str.Substring(1, 4)));
							if (coherence > 1) coherence = 1;
                            question = str.Substring(6);

                            pix = new pixel();
                            pix.coherence = coherence;
                            pix.question = question.ToLower();
                            pix.update(prefix, no_of_prefixes, bisector, bisector_property_flags, no_of_bisectors);
                            if (pix.object1 != "")
                            {								
                                no_of_parsed_pixels++;

                                /// add words to the dictionary
                                addDictionaryEntry(pix.object1, mindpixels.Count);
                                addDictionaryEntry(pix.object2, mindpixels.Count);
                            }
							
							if (show_mindpixels)
							{
								if (rnd.Next(1000) < 2)
								{								    
									if (pix.object1 == "") 
										Console.WriteLine("Couldn't parse question: " + pix.question);
									else
										Console.WriteLine("(" + pix.object1 + ") " + pix.Bisector + " (" + pix.object2 + ")");
								}
							}
							
                            if (pix.object1 != "") 
							{
								pix.Bisector = pix.Bisector.Trim();
								string[] l1 = pix.object1.Split(' ');
								if (l1.Length < 4)
								{
									if (l1.Length > 1)
									{
										if ((l1[0] == "does") || 
										    (l1[0] == "how"))
										{
											string new_object1 = "";
											for (int j = 1; j < l1.Length; j++)
											{
												new_object1 += l1[j];
												if (j < l1.Length-1) new_object1 += " ";
											}
											pix.object1 = new_object1;
										}
									}
									string[] l2 = pix.object2.Split(' ');
									if ((l2.Length < 4) && 
									    (pix.object2 != "of") && 
									    (pix.Bisector != "and"))
									{
										if (l2.Length > 1)
										{
											if (l2[l2.Length-1] == "?")
											{
												string new_object2 = "";
												for (int j = 0; j < l2.Length-1; j++)
												{
													new_object2 += l2[j];
													if (j < l2.Length-2) new_object2 += " ";
												}
												pix.object2 = new_object2;
											}
										}
										
										if (pix.Bisector == "") pix.Bisector = "is";
										if ((!pix.object1.Contains(Convert.ToString('"'))) &&
										    (!pix.object2.Contains(Convert.ToString('"'))))
										{
											if ((!pix.object2.Contains("deletion")) &&
											    (pix.object1 != "what is"))
											{
											    if (pix.Bisector.Contains(" part of")) pix.Bisector = "part of";
												if (pix.Bisector.StartsWith("the ")) pix.Bisector = pix.Bisector.Substring(4);
											    checkNationality(pix);
									            mindpixels.Add(pix);
											}
										}
									}
								}
							}
                        }
                        catch
                        {
                        }
                    }
                }

                i++;
            }
            if (oRead.EndOfStream)
            {
                oRead.Close();

                //now wire everything up
                //createMind();
            }
        }

		protected string ToNumeric(string str)
		{
			string result = "";
			char[] ch = str.ToCharArray();
			for (int i = 0; i < ch.Length; i++)
			{
				if (((ch[i] >= '0') && (ch[i] <= '9')) || (ch[i]=='.'))
					result += ch[i];
			}
			return(result);
		}
		
        public void LoadGAC(string filename, bool show_mindpixels)
        {
            bool filefound = true;

            try
            {
                oRead = File.OpenText(filename);
            }
            catch
            {
                filefound = false;
            }

            if (filefound)
            {
                no_of_parsed_pixels = 0;
                //dictionary_entries = 0;
                mindpixels = new ArrayList();
                if (initialstring != "") initialstringFound = false;
                loadGAC(99999999, show_mindpixels);
            }
        }

		/// <summary>
		/// probably not a comprehensive list, but all the countries I could think of
		/// </summary>
        private void createNationalities()
        {
			addNationality("french", "france");
			addNationality("english", "england");
			addNationality("welsh", "wales");
			addNationality("scottish", "scotland");
			addNationality("german", "germany");
			addNationality("polish", "poland");
			addNationality("spanish", "spain");
			addNationality("portuguese", "portugal");
			addNationality("african", "africa");
			addNationality("iranian", "iran");
			addNationality("iraqi", "iraq");
			addNationality("russian", "russia");
			addNationality("chinese", "china");
			addNationality("american", "america");
			addNationality("brazilian", "brazil");
			addNationality("argentinian", "argentina");
			addNationality("canadian", "canada");
			addNationality("british", "britain");
			addNationality("italian", "italy");
			addNationality("turkish", "turky");
			addNationality("australian", "australia");
			addNationality("japanese", "japan");
			addNationality("korean", "korea");
			addNationality("pakistani", "pakistan");
			addNationality("indian", "india");
			addNationality("afghani", "afghanistan");
			addNationality("south african", "south africa");
			addNationality("sudanian", "sudan");
			addNationality("kenyan", "kenya");
			addNationality("alaskan", "alaska");
			addNationality("irish", "ireland");
			addNationality("belgian", "belgum");
			addNationality("dutch", "netherlands");
			addNationality("danish", "denmark");
			addNationality("swedish", "sweden");
			addNationality("norse", "norway");
			addNationality("ukranian", "ukrane");
			addNationality("tiwanese", "tiwan");
			addNationality("indonesian", "indonesia");
			addNationality("libyan", "libya");
			addNationality("nigerian", "nigeria");
			addNationality("vietnamese", "vietnam");
			addNationality("mongolian", "mongolia");
			addNationality("icelandean", "iceland");
			addNationality("greenlandean", "greenland");
			addNationality("peruvian", "peru");
			addNationality("chilean", "chile");
			addNationality("serbian", "serbia");
			addNationality("croatian", "croatia");
			addNationality("albanian", "albania");
			addNationality("austrian", "austria");
			addNationality("estonian", "estonia");
			addNationality("greek", "greece");
			addNationality("latvian", "latvia");
			addNationality("lithuanian", "lithuania");
			addNationality("macedonian", "macedonia");
			addNationality("maltese", "malta");
			addNationality("slovakian", "slovakia");
			addNationality("slovenian", "slovenia");
			addNationality("palestinian", "palestine");
			addNationality("israelian", "israel");
			addNationality("israeli", "israel");
			addNationality("jordanian", "jordan");
			addNationality("kosovan", "kosovo");
			addNationality("angolan", "angola");
			addNationality("liberian", "liberia");
			addNationality("moroccan", "moroco");
			addNationality("ugandan", "uganda");
			addNationality("zimbabwean", "zimbabwe");
			addNationality("zambian", "zambia");
			addNationality("tunisian", "tunisia");
			addNationality("senegalese", "senegal");
			addNationality("malawian", "malawi");
			addNationality("ethiopian", "ethiopia");
			addNationality("tasmanian", "tasmania");
			addNationality("mexican", "mexico");
			addNationality("panamanian", "panama");
		}
		
		private bool checkNationality(pixel pix)
		{
			bool found = false;
			string[] str = pix.object2.Split(' ');
			if (str.Length > 1)
			{
				if ((pix.Bisector != "citizen of") &&
				    (!pix.object2.Contains("city")) &&
				    (!pix.object2.Contains("capital")) &&
				    (!pix.object2.Contains("capitol")) &&
				    (!pix.object2.Contains("town")) &&
				    (!pix.object2.Contains("village")) &&
				    (!pix.object2.Contains("company")) &&
				    (!pix.object2.Contains("owned")))
				{
					for (int i = 0; i < no_of_nationalities; i++)
					{
						if (str[0] == nationality[i])
						{
							found = true;
							
							pixel p1 = new pixel();
							p1.object1 = pix.object1;
							p1.Bisector = "citizen of";
							p1.coherence = pix.coherence;
							p1.object2 = nation[i];
							mindpixels.Add(p1);
							
							pix.object2 ="";
							for (int j = 1; j < str.Length; j++)
							{
								pix.object2 += str[j];
								if (j < str.Length-1) pix.object2 += " ";
							}
							break;
						}
					}
				}
			}
			return(found);
		}
		
        private void createBisectors()
        {
            //the following items are deliberately at the beginning of the list
            addBisector("is a", 0, true, false, false, false, false, false, false, false, false);
            addBisector("has", 0, false, true, false, false, false, false, false, false, false);  

            addBisector("belong to the dynasty", 0, false, true, false, false, false, false, false, false, false);  
            addBisector("in the professional field", 0, false, true, false, false, false, false, false, false, false);  
            addBisector("attend", 0, false, true, false, false, false, false, false, false, false);  
            addBisector("influence", 0, false, true, false, false, false, false, false, false, false);  
			addBisector("have the vice president", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have the orbital period", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have the orbital eccentricity", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have the occupation", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have the religious stance", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have the conservation status", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have the government type", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have the latin name", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have the genre", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have a memory of", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have ideas in", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have the official language", 0, false, true, false, false, false, false, false, false, false);
			addBisector("reign between", 0, false, true, false, false, false, false, false, false, false);
			addBisector("win the award", 0, false, true, false, false, false, false, false, false, false);
            addBisector("appointed to the role of", 0, false, true, false, false, false, false, false, false, false);
            addBisector("interested in", 0, false, true, false, false, false, false, false, false, false);
            addBisector("crowned on", 0, false, true, false, false, false, false, false, false, false);
            addBisector("born named", 0, false, true, false, false, false, false, false, false, false);
            addBisector("born on", 0, false, true, false, false, false, false, false, false, false);
            addBisector("born in", 0, false, true, false, false, false, false, false, false, false);
            addBisector("die on", 0, false, true, false, false, false, false, false, false, false);
            addBisector("die in", 0, false, true, false, false, false, false, false, false, false);
            addBisector("die from", 0, false, true, false, false, false, false, false, false, false);
            addBisector("in the time zone", 0, false, true, false, false, false, false, false, false, false);
            addBisector("use the currency", 0, false, true, false, false, false, false, false, false, false);
            addBisector("a member of the", 0, false, true, false, false, false, false, false, false, false);
			addBisector("follow the school of thought", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have the patron saint", 0, false, true, false, false, false, false, false, false, false);
			addBisector("have the media type", 0, false, true, false, false, false, false, false, false, false);
			addBisector("published in", 0, false, true, false, false, false, false, false, false, false);
			addBisector("published on", 0, false, true, false, false, false, false, false, false, false);
			addBisector("published by", 0, false, true, false, false, false, false, false, false, false);
			addBisector("preceded by", 0, false, true, false, false, false, false, false, false, false);
			addBisector("followed by", 0, false, true, false, false, false, false, false, false, false);
			addBisector("launched on", 0, false, true, false, false, false, false, false, false, false);
			addBisector("advised by", 0, false, true, false, false, false, false, false, false, false);
			addBisector("belong to the enthinic group", 0, false, true, false, false, false, false, false, false, false);
			addBisector("belong to the industry", 0, false, true, false, false, false, false, false, false, false);
			addBisector("belong to the subspecies", 0, false, true, false, false, false, false, false, false, false);
			addBisector("occur between", 0, false, true, false, false, false, false, false, false, false);
			addBisector("occur on", 0, false, true, false, false, false, false, false, false, false);
			addBisector("orbit around", 0, false, true, false, false, false, false, false, false, false);
			addBisector("speak the language", 0, false, true, false, false, false, false, false, false, false);
			addBisector("discovered using", 0, false, true, false, false, false, false, false, false, false);

            addBisector("like to play the", 10, false, false, false, false, false, true, false, false, false);
			addBisector("play the", 0, false, true, false, false, false, false, false, false, false);
            addBisector("like to play with", 10, false, false, false, false, false, true, false, false, false);

            addBisector("a part of the", 0, false, false, false, false, false, false, false, false, true);

            addBisector("you will become", 0, false, false, false, false, true, true, true, false, false);
            addBisector("I will become", 0, false, false, false, false, true, true, true, false, false);
            addBisector("they will become", 0, false, false, false, false, true, true, true, false, false);

            addBisector("a part of some", 0, false, false, false, false, false, false, false, false, true);

            addBisector("the opposite of", 0, false, false, false, true, false, false, false, false, false);
            addBisector("the reverse of", 0, false, false, false, true, false, false, false, false, false);
            addBisector("do not like", -10, false, false, false, false, false, true, false, false, false);
            addBisector("first thought of by", 1, false, false, false, true, false, true, true, false, false);
            addBisector("first invented by", 1, false, false, false, true, false, true, true, false, false);
            addBisector("thought of by", 2, false, false, false, false, false, true, false, false, false);
            addBisector("make changes to", 0, false, false, false, false, true, false, false, true, false);
            addBisector("is necessary to", 0, false, false, false, false, true, false, false, true, false);
            addBisector("is needed by", 0, false, false, false, false, true, false, false, true, false);
            addBisector("is something which", 0, false, false, false, false, false, false, false, true, true);
            addBisector("is something that", 0, false, false, false, false, false, false, false, true, true);
            addBisector("make love to", 10, false, false, false, false, false, true, false, false, false);
            addBisector("have sex with", 10, false, false, false, false, false, true, false, false, false);
            addBisector("have some sort of", 0, false, true, false, false, false, false, false, false, false);
            addBisector("have some kind of", 0, false, true, false, false, false, false, false, false, false);
            addBisector("has some kind of", 0, false, true, false, false, false, false, false, false, false);
            addBisector("is used for", 0, false, false, false, false, true, false, false, true, false);
            addBisector("is useful for", 0, false, false, false, false, false, false, false, true, false);
            addBisector("is used with", 0, false, false, false, false, true, false, false, true, false);
            addBisector("is used by", 0, false, false, false, false, false, true, false, true, false);
            addBisector("is needed for", 0, false, false, false, false, true, false, false, true, false);
            addBisector("have more than", 0, false, false, false, true, false, false, false, false, false);
            addBisector("have less than", 0, false, false, false, true, false, false, false, false, false);
            addBisector("has more than", 0, false, false, false, true, false, false, false, false, false);
            addBisector("has less than", 0, false, false, false, true, false, false, false, false, false);
            addBisector("bigger than", 1, false, false, false, true, false, false, false, false, false);
            addBisector("the same speed as", 0, false, false, false, true, false, false, true, false, false);
            addBisector("the same as", 0, false, false, false, true, false, false, false, false, false);
            addBisector("is identical to", 0, false, false, false, true, false, false, false, false, false);
            addBisector("is equivalent to", 0, false, false, false, true, false, false, false, false, false);
            addBisector("identical to", 0, false, false, false, true, false, false, false, false, false);
            addBisector("equivalent to", 0, false, false, false, true, false, false, false, false, false);
            addBisector("larger than", 1, false, false, false, true, false, false, false, false, false);
            addBisector("longer than", 0, false, false, false, true, false, false, true, false, false);
            addBisector("wider than", 0, false, false, false, true, false, false, false, false, false);
            addBisector("know how to", 0, false, false, false, false, false, true, false, false, false);
            addBisector("know how the", 0, false, false, false, false, false, true, false, false, false);
            addBisector("know how a", 0, false, false, false, false, false, true, false, false, false);
            addBisector("know how many", 0, false, false, false, false, false, true, false, false, false);
            addBisector("know how you", 0, false, false, false, false, false, true, false, false, false);
            addBisector("also known as", 0, false, false, false, false, false, true, false, false, false);
            addBisector("otherwise known as", 0, false, false, false, false, false, true, false, false, false);
            addBisector("alternatively known as", 0, false, false, false, false, false, true, false, false, false);
            addBisector("compared to", 0, false, false, false, true, false, false, false, false, false);
            addBisector("smaller than", 0, false, false, false, true, false, false, false, false, false);
            addBisector("shorter than", 0, false, false, false, true, false, false, false, false, false);
            addBisector("as short as", 0, false, false, false, true, false, false, false, false, false);
            addBisector("as long as", 0, false, false, false, true, false, false, false, false, false);
            addBisector("as wide as", 0, false, false, false, true, false, false, false, false, false);
            addBisector("as tall as", 0, false, false, false, true, false, false, false, false, false);
            addBisector("equal to", 0, false, false, false, true, false, false, false, false, false);
            addBisector("less than", -1, false, false, false, true, false, false, false, false, false);
            addBisector("greater than", 1, false, false, false, true, false, false, false, false, false);
            addBisector("smarter than", 10, false, false, false, true, false, false, false, false, false);
            addBisector("stronger than", 10, false, false, false, true, false, false, false, false, false);
            addBisector("weaker than", -10, false, false, false, true, false, false, false, false, false);
            addBisector("not as strong as", -10, false, false, false, true, false, false, false, false, false);
            addBisector("not as good as", -15, false, false, false, true, false, false, false, false, false);
            addBisector("not as big as", -5, false, false, false, true, false, false, false, false, false);
            addBisector("not like", -2, false, false, false, true, false, false, false, false, false);
            addBisector("as strong as", 1, false, false, false, true, false, false, false, false, false);
            addBisector("as good as", 5, false, false, false, true, false, false, false, false, false);
            addBisector("as big as", 5, false, false, false, true, false, false, false, false, false);
            addBisector("as smart as", 10, false, false, false, true, false, true, false, false, false);
            addBisector("as long as", 0, false, false, false, true, false, false, false, false, false);
            addBisector("as short as", 0, false, false, false, true, false, false, false, false, false);
            addBisector("scared of", -15, false, false, false, false, false, true, false, false, false);

            addBisector("made of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("made from", 0, false, false, false, false, false, false, false, false, true);
            addBisector("constructed from", 0, false, false, false, false, false, false, false, true, true);
            addBisector("constructed by", 0, false, false, false, false, false, false, false, true, true);
            addBisector("made out of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("need a", 0, false, false, false, false, false, false, false, true, false);
            addBisector("needs a", 0, false, false, false, false, false, false, false, true, false);
            addBisector("needs some", 0, false, false, false, false, false, false, false, true, false);

            addBisector("require a", 0, false, false, false, false, true, false, false, true, false);
            addBisector("required to", 0, false, false, false, false, true, false, false, true, false);
            addBisector("required by", 0, false, false, false, false, true, false, false, true, false);
            addBisector("a type of", 0, true, false, false, false, false, false, false, false, true);
            addBisector("a class of", 0, true, false, false, false, false, false, false, false, true);
            addBisector("a member of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("a friend of", 10, false, false, false, true, false, true, false, false, false);
            addBisector("an enemy of", -10, false, false, false, true, false, true, false, false, false);
            addBisector("a form of", 0, true, false, false, false, false, false, false, false, true);
            addBisector("similar to", 0, false, false, false, true, false, false, false, false, false);
            addBisector("the same as", 0, false, false, false, true, false, false, false, false, true);
            addBisector("different from", 0, false, false, false, true, false, false, false, false, true);
            addBisector("different to", 0, false, false, false, true, false, false, false, false, true);
            addBisector("play with", 5, false, false, false, false, false, true, false, false, false);
            addBisector("work with", 2, false, false, false, false, false, true, false, true, false);
            addBisector("sleep with", 3, false, false, false, false, false, true, false, false, false);
            addBisector("written by", 0, false, false, false, false, true, true, false, false, false);
            addBisector("made by", 0, false, false, false, false, true, false, false, false, true);
            addBisector("the author of", 0, false, false, false, false, true, true, false, false, true);
            addBisector("sound like", 0, false, false, false, true, false, false, false, false, true);
            addBisector("be eaten by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("be used by", 0, false, false, false, false, false, true, false, true, false);
            addBisector("be read by", 0, false, false, false, false, false, true, false, false, false);
            addBisector("faster than", 1, false, false, false, true, false, false, true, false, false);
            addBisector("slower than", -1, false, false, false, true, false, false, true, false, false);
            addBisector("lose track of", -5, false, false, false, false, false, false, false, false, false);
            addBisector("changed by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("eaten by", 0, false, false, false, false, false, true, false, false, false);
            addBisector("a common way of", 0, false, false, false, false, true, false, false, false, false);
            addBisector("a popular", 5, false, false, false, false, false, true, false, false, false);

            addBisector("a lot of", 1, false, false, false, false, false, false, false, false, false);

            addBisector("a means of", 0, false, false, false, false, true, false, false, true, false);
            addBisector("a means by which", 0, false, false, false, false, true, false, false, true, false);

            addBisector("associated with", 0, false, false, false, false, false, false, false, false, true);
            addBisector("a mode of", 0, false, false, false, false, false, false, false, false, true);

            addBisector("a method of", 0, false, false, false, false, true, false, false, true, true);
            addBisector("be stored on", 0, false, false, true, false, false, false, false, true, false);
            addBisector("located in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("located near", 0, false, false, true, true, false, false, false, false, false);
            addBisector("far away from", 0, false, false, true, true, false, false, false, false, false);
            addBisector("a popular form of", 10, false, false, false, false, false, true, false, false, false);
            addBisector("the founder of", 1, false, false, false, false, true, false, false, false, false);
            addBisector("be caused by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("are caused by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("something you", 0, false, false, false, false, false, true, false, false, true);
            addBisector("something they", 0, false, false, false, false, false, true, false, false, false);
            addBisector("something i", 0, false, false, false, false, false, true, false, false, false);

            addBisector("something with", 0, false, false, false, false, false, false, false, false, true);
            addBisector("can be a", 0, false, false, false, false, false, false, false, true, true);

            addBisector("if you have", 0, false, false, false, false, true, true, false, false, false);
            addBisector("normally inside", 0, false, false, true, false, false, false, false, false, false);
            addBisector("normally outside", 0, false, false, true, false, false, false, false, false, false);

            addBisector("normally with", 0, false, false, false, false, false, false, false, false, false);

            addBisector("normally in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("normally attached to", 0, false, false, true, false, false, false, false, false, false);
            addBisector("normally connected to", 0, false, false, true, false, false, false, false, false, false);
            addBisector("usually inside", 0, false, false, true, false, false, false, false, false, false);
            addBisector("usually outside", 0, false, false, true, false, false, false, false, false, false);
            addBisector("usually with", 0, false, false, true, false, false, false, false, false, false);
            addBisector("usually in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("usually have", 0, false, true, false, false, false, false, false, false, false);
            addBisector("usually attached to", 0, false, false, true, false, false, false, false, false, false);
            addBisector("usually connected to", 0, false, false, true, false, false, false, false, false, false);
            addBisector("which points to", 0, false, false, true, false, false, false, false, false, false);
            addBisector("which points towards", 0, false, false, true, false, false, false, false, false, false);
            addBisector("which goes to", 0, false, false, true, false, false, false, false, false, false);

            addBisector("which goes with", 0, false, false, false, false, false, false, false, false, true);

            addBisector("to learn from", 0, false, false, false, false, true, true, false, false, false);
            addBisector("to learn about", 0, false, false, false, false, true, true, false, false, false);
            addBisector("be killed by", -20, false, false, false, false, true, true, false, false, false);
            addBisector("be killed with", -20, false, false, false, false, true, true, false, false, false);
            addBisector("at the end of", 0, false, false, false, true, false, false, false, false, false);
            addBisector("at the beginning of", 0, false, false, false, true, false, false, false, false, false);
            addBisector("at the start of", 0, false, false, false, true, false, false, false, false, false);
            addBisector("a means of", 0, false, false, false, false, true, false, false, true, false);
            addBisector("a way to", 0, false, false, false, false, true, false, false, true, false);
            addBisector("the way someone", 0, false, false, false, false, true, true, false, false, false);
            addBisector("the way we", 0, false, false, false, false, true, false, false, false, false);
            addBisector("a way we", 0, false, false, false, false, true, false, false, false, false);
            addBisector("originally done to", 0, false, false, false, false, true, false, true, false, false);

            addBisector("originally done with", 0, false, false, false, false, false, false, true, false, false);

            addBisector("originally made with", 0, false, false, false, false, true, false, true, false, false);
            addBisector("originally worn by", 0, false, false, false, false, false, true, true, false, false);
            addBisector("originally worn during", 0, false, false, false, false, false, true, true, false, false);
            addBisector("originally worn when", 0, false, false, false, false, false, true, true, false, false);
            addBisector("originally done by", 0, false, false, false, false, false, true, true, false, false);
            addBisector("originally discovered by", 0, false, false, false, false, false, true, true, false, false);

            addBisector("better known as", 0, false, false, false, false, false, false, true, false, true);
            addBisector("sometimes known as", 0, false, false, false, false, false, false, true, false, true);
            addBisector("sometimes refered to as", 0, false, false, false, false, false, false, true, false, true);
            addBisector("refered to as", 0, false, false, false, false, false, false, false, false, true);
            addBisector("keep track of", 0, false, false, false, false, false, false, false, true, false);

            addBisector("a long way from", -2, false, false, true, false, false, false, false, false, false);
            addBisector("a short way from", 2, false, false, true, false, false, false, false, false, false);
            addBisector("a long distance from", -2, false, false, true, false, false, false, false, false, false);
            addBisector("a short distance from", 2, false, false, true, false, false, false, false, false, false);
            addBisector("assist you to", 5, false, false, false, false, false, true, false, true, false);
            addBisector("help them to", 5, false, false, false, false, false, true, false, true, false);
            addBisector("help you to", 5, false, false, false, false, false, true, false, true, false);

            addBisector("a measure of", 0, false, false, false, false, false, false, false, true, false);

            addBisector("will produce a", 0, false, false, false, false, true, false, true, true, false);
            addBisector("will produce some", 0, false, false, false, false, true, false, true, true, false);
            addBisector("when you have", 0, false, false, false, false, false, true, true, false, false);
            addBisector("a city in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("a town in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("a place in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("to make a", 0, false, false, false, false, true, false, false, false, false);
            addBisector("to make some", 0, false, false, false, false, true, false, false, false, false);
            addBisector("to make someone", 0, false, false, false, false, true, true, false, false, false);
            addBisector("to make my", 0, false, false, false, false, false, true, false, false, false);
            addBisector("to make the", 0, false, false, false, false, true, false, false, false, false);
            addBisector("the real name of", 0, false, false, false, false, false, true, false, false, true);

            addBisector("the name of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("another name for", 0, false, false, false, false, false, false, false, false, true);
            addBisector("the title of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("a title of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("the goal of", 0, false, false, false, false, false, false, true, false, false);
            addBisector("the aim of", 0, false, false, false, false, false, false, true, false, false);
            addBisector("the target of", 0, false, false, false, false, false, false, true, false, false);

            addBisector("declare war on", -10, false, false, false, false, false, true, false, false, false);
            addBisector("declare war with", -10, false, false, false, false, false, true, false, false, false);
            addBisector("go to war with", -10, false, false, true, false, false, false, false, false, false);

            addBisector("the source of", 0, false, false, false, false, false, false, false, false, false);
            addBisector("a source of", 0, false, false, false, false, false, false, false, false, false);
            addBisector("a function of", 0, false, false, false, false, false, false, false, true, false);

            addBisector("a consequence of", 0, false, false, false, false, true, false, true, false, false);

            addBisector("a property of", 0, false, false, false, false, false, false, false, true, true);
            addBisector("are on a", 0, false, false, false, false, false, false, false, false, true);

            addBisector("are in a", 0, false, false, true, false, false, false, false, false, false);
            addBisector("are in some", 0, false, false, true, false, false, false, false, false, false);
            addBisector("are on some", 0, false, false, true, false, false, false, false, false, false);
            addBisector("located at", 0, false, false, true, false, false, false, false, false, false);
            addBisector("something you can", 0, false, false, false, false, false, true, false, false, false);
            addBisector("something i can", 0, false, false, false, false, false, true, false, false, false);

            addBisector("always be the", 1, false, false, false, false, false, false, true, false, false);
            addBisector("always be a", 1, false, false, false, false, false, false, true, false, false);
            addBisector("supplied by", 0, false, false, false, false, false, false, false, true, false);

            addBisector("queen of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("king of", 0, false, false, false, false, false, true, false, false, false);

            addBisector("a real", 0, false, false, false, false, false, false, false, false, true);

            addBisector("believe in", 4, false, false, false, false, false, true, false, false, false);
            addBisector("voted for", 4, false, false, false, false, false, true, true, false, false);
            addBisector("vote for", 4, false, false, false, false, false, true, false, false, false);
            addBisector("campaign for", 4, false, false, false, false, false, true, false, false, false);
            addBisector("campaign against", -4, false, false, false, false, false, true, false, false, false);
            addBisector("fight against", -20, false, false, false, false, false, true, false, false, false);

            addBisector("the only", 0, false, false, false, false, false, false, false, false, true);

            addBisector("nicer than", 10, false, false, false, true, false, false, false, false, false);
            addBisector("worse than", -10, false, false, false, true, false, false, false, false, false);
            addBisector("better than", 10, false, false, false, true, false, false, false, false, false);

            addBisector("name of", 0, false, false, false, false, false, false, false, false, true);

            addBisector("invented by", 1, false, false, false, false, true, true, false, true, false);
            addBisector("the largest", 1, false, false, false, true, false, false, false, false, false);
            addBisector("the smallest", -1, false, false, false, true, false, false, false, false, false);
            addBisector("the biggest", 1, false, false, false, true, false, false, false, false, false);
            addBisector("the richest", 5, false, false, false, true, false, false, false, false, false);
            addBisector("the poorest", -5, false, false, false, true, false, false, false, false, false);
            addBisector("through the", 0, false, false, true, false, false, false, false, false, false);
            addBisector("accused of", -1, false, false, false, false, false, true, false, false, false);
            addBisector("connected with", 0, false, false, false, false, true, false, false, false, false);
            addBisector("connected to", 0, false, false, true, false, false, false, false, false, false);
            addBisector("mounted in", 0, false, false, true, false, false, false, false, true, false);
            addBisector("mounted on", 0, false, false, true, false, false, false, false, false, false);
            addBisector("comsumes some", 0, false, false, false, false, true, false, false, false, false);
            addBisector("produces some", 0, false, false, false, false, true, false, false, true, false);
            addBisector("produces a", 0, false, false, false, false, true, false, false, true, false);

            addBisector("based on", 0, false, false, false, false, false, false, false, false, true);
            addBisector("based upon", 0, false, false, false, false, false, false, false, false, true);
            addBisector("derived from", 0, false, false, false, false, false, false, false, false, true);

            addBisector("have a", 0, false, true, false, false, false, false, false, false, false);
            addBisector("when it", 0, false, false, false, false, true, false, true, false, false);
            addBisector("when they", 0, false, false, false, false, true, false, true, false, false);
            addBisector("when i", 0, false, false, false, false, true, false, true, false, false);
            addBisector("when you", 0, false, false, false, false, true, false, false, false, false);
            addBisector("pick up", 0, false, false, true, false, false, false, false, false, false);
            addBisector("help you", 3, false, false, false, false, false, false, false, false, false);
            addBisector("make you", 0, false, false, false, false, true, false, false, false, false);
            addBisector("equivalent to", 0, false, false, false, true, false, false, false, false, false);
            addBisector("the same as", 0, false, false, false, true, false, false, false, false, true);
            addBisector("different from", 0, false, false, false, true, false, false, false, false, true);
            addBisector("younger than", 1, false, false, false, true, false, true, true, false, false);
            addBisector("older than", -1, false, false, false, true, false, true, true, false, false);
            addBisector("brighter than", 2, false, false, false, true, false, false, false, false, false);
            addBisector("as bright as", 2, false, false, false, true, false, false, false, false, false);
            addBisector("as clever as", 1, false, false, false, true, false, true, false, false, false);
            addBisector("as intelligent as", 1, false, false, false, true, false, true, false, false, false);
            addBisector("as smart as", 1, false, false, false, true, false, true, false, false, false);
            addBisector("more intelligent than", 0, false, false, false, true, false, true, false, false, false);
            addBisector("cleverer than", 1, false, false, false, true, false, true, false, false, false);
            addBisector("smarter than", 1, false, false, false, true, false, true, false, false, false);
            addBisector("heavier than", 0, false, false, false, true, false, false, false, false, false);
            addBisector("harder than", 0, false, false, false, true, false, false, false, false, false);
            addBisector("softer than", 0, false, false, false, true, false, false, false, false, false);
            addBisector("lighter than", 0, false, false, false, true, false, false, false, false, false);
            addBisector("not as heavy as", 0, false, false, false, true, false, false, false, false, false);
            addBisector("as heavy as", 0, false, false, false, true, false, false, false, false, false);

            addBisector("made up of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("go through", 0, false, false, false, false, false, false, false, false, false);

            addBisector("pass through", 0, false, false, true, false, false, false, false, false, false);

            addBisector("consist of", 0, false, false, false, false, false, false, false, false, true);

            addBisector("give off", 0, false, false, false, false, true, false, false, false, false);

            addBisector("sometimes called", 0, false, false, false, false, false, false, false, false, true);
            
            addBisector("with their", 0, false, false, false, false, false, true, false, false, true);

            addBisector("with its", 0, false, false, false, false, false, false, false, false, true);
            addBisector("come in", 0, false, false, false, false, false, false, false, false, false);

            addBisector("afraid of", -10, false, false, false, false, false, true, false, false, false);
            addBisector("scared of", -20, false, false, false, false, false, true, false, false, false);
            addBisector("possible to", 0, false, false, false, false, true, false, false, true, false);

            addBisector("related to", 0, false, false, false, false, false, false, false, false, true);
            addBisector("possible that", 0, false, false, false, false, false, false, false, false, false);
            addBisector("hidden by", -1, false, false, false, false, false, false, false, false, false);

            addBisector("discovered by", 1, false, false, false, false, false, true, false, false, false);
            addBisector("found by", 0, false, false, false, false, false, true, false, false, false);
            addBisector("allergic to", -20, false, false, false, false, true, true, false, false, false);
            addBisector("worn during", 0, false, false, false, false, false, true, false, false, false);
            addBisector("worn when", 0, false, false, false, false, false, true, true, false, false);
            addBisector("the most recent", 1, false, false, false, true, false, false, true, false, false);
            addBisector("the latest", 1, false, false, false, true, false, false, true, false, false);
            addBisector("the last", 0, false, false, false, true, false, false, true, false, false);
            addBisector("the first", 1, false, false, false, true, false, false, true, false, false);
            addBisector("the capital of", 0, false, false, true, false, false, false, false, false, false);
            addBisector("the capitol of", 0, false, false, true, false, false, false, false, false, false);
            addBisector("capitol of", 0, false, false, true, false, false, false, false, false, false);
            addBisector("capital of", 0, false, false, true, false, false, false, false, false, false);
            addBisector("not allowed to", -5, false, false, false, false, false, false, false, false, false);
            addBisector("forbidden to", -5, false, false, false, false, false, false, false, false, false);
            addBisector("illegal to", -10, false, false, false, false, false, false, false, false, false);
            addBisector("allowed to", 1, false, false, false, false, false, true, false, false, false);
            addBisector("legal to", -10, false, false, false, false, false, false, false, false, false);
            addBisector("pass through", 0, false, false, true, false, false, false, false, false, false);
            addBisector("move through", 0, false, false, true, false, false, false, false, false, false);
            addBisector("travel through", 0, false, false, true, false, false, false, false, false, false);
            addBisector("the winner of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("a member of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("the manager of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("the superior of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("the boss of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("the head of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("responsible to", 0, false, false, false, false, false, true, false, false, false);
            addBisector("responsible for", 0, false, false, false, false, false, true, false, false, false);
            addBisector("chairman of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("leader of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("prime minister of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("president of", 0, false, false, false, false, false, true, false, false, false);
            addBisector("come from", 0, false, false, true, false, false, false, false, false, false);
            addBisector("travel through", 0, false, false, true, false, false, false, true, false, false);
            addBisector("travel under", 0, false, false, true, false, false, false, true, false, false);
            addBisector("travel over", 0, false, false, true, false, false, false, true, false, false);
            addBisector("move over", 0, false, false, true, false, false, false, true, false, false);
            addBisector("move through", 0, false, false, true, false, false, false, true, false, false);
            addBisector("good to", 10, false, false, false, false, false, false, false, false, false);
            addBisector("right to", 10, false, false, false, false, false, false, false, false, false);
            addBisector("bad to", -10, false, false, false, false, false, false, false, false, false);
            addBisector("wrong to", -10, false, false, false, false, false, false, false, false, false);
            addBisector("has some", 0, false, true, false, false, false, false, false, false, true);

            addBisector("has no", 0, false, false, false, false, false, false, false, false, true);

            addBisector("a nice", 5, false, false, false, false, false, false, false, false, false);
            addBisector("killed on", -10, false, false, false, false, false, true, false, false, false);
            addBisector("killed with", -10, false, false, false, false, true, true, false, false, false);
            addBisector("killed by", -10, false, false, false, false, true, true, false, false, false);
            addBisector("be regrown", 0, false, false, false, false, false, true, false, false, false);
            addBisector("be grown", 1, false, false, false, false, false, true, false, false, false);
            addBisector("learn about", 1, false, false, false, false, false, true, false, false, false);
            addBisector("learn from", 1, false, false, false, false, true, true, false, false, false);
            addBisector("live with", 0, false, false, false, false, false, true, false, false, false);
            addBisector("live in", 0, false, false, true, false, false, true, false, false, false);
            addBisector("live within", 0, false, false, true, false, false, true, false, false, false);
            addBisector("wish for", 0, false, false, false, false, false, true, false, false, false);
            addBisector("further away from", -1, false, false, true, true, false, false, false, false, false);
            addBisector("further away than", -1, false, false, true, true, false, false, false, false, false);
            addBisector("closer to", 1, false, false, true, true, false, false, false, false, false);
            addBisector("points at", 0, false, false, true, false, false, false, false, false, false);
            addBisector("points to", 0, false, false, true, false, false, false, false, false, false);
            addBisector("go with", 0, false, false, false, false, false, true, false, false, false);
            addBisector("go in", 0, false, false, true, false, false, false, false, false, false);

            addBisector("of an", 0, false, false, false, false, false, false, false, false, true);

            addBisector("has to", 0, false, false, false, false, true, false, false, false, false);
            addBisector("go to", 0, false, false, true, false, false, false, false, false, false);
            addBisector("to break", 0, false, false, false, false, true, false, false, false, false);
            addBisector("to harm", -10, false, false, false, false, false, true, false, false, false);
            addBisector("hate to", -20, false, false, false, false, false, true, false, false, false);
            addBisector("like to", 10, false, false, false, false, false, false, false, false, false);

            addBisector("grown in", 0, false, false, false, false, false, false, false, false, false);
            addBisector("grown on", 0, false, false, true, false, false, false, false, false, false);
            addBisector("come after", 0, false, false, false, false, true, false, false, false, false);
            addBisector("come before", 0, false, false, false, false, true, false, false, false, false);
            addBisector("can change", 0, false, false, false, false, true, false, true, false, false);
            addBisector("cant change", -5, false, false, false, false, true, false, true, false, false);
            addBisector("poor at", -5, false, false, false, false, false, false, false, false, false);
            addBisector("no good at", -10, false, false, false, false, false, false, false, false, false);
            addBisector("bad at", -10, false, false, false, false, false, false, false, false, false);
            addBisector("bad for", -10, false, false, false, false, false, false, false, false, false);
            addBisector("good at", 10, false, false, false, false, false, false, false, false, false);
            addBisector("good with", 10, false, false, false, false, false, false, false, false, false);
            addBisector("good for", 10, false, false, false, false, false, false, false, false, false);
            addBisector("lead to", 0, false, false, false, false, true, false, false, false, false);

            addBisector("will be", 0, false, false, false, false, false, false, true, false, false);
            addBisector("wont be", 0, false, false, false, false, false, false, true, false, false);

            addBisector("spy on", -5, false, false, false, false, false, true, false, false, false);
            addBisector("normally in", 0, false, false, true, false, false, false, true, false, false);
            addBisector("always in", 0, false, false, true, false, false, false, true, false, false);
            addBisector("always have", 1, false, true, false, false, false, false, true, false, false);
            addBisector("normally have", 0, false, true, false, false, false, false, true, false, false);
            addBisector("on the", 0, false, false, true, false, false, false, false, false, false);
            addBisector("near to", 0, false, false, true, true, false, false, false, false, false);
            addBisector("used to", 0, false, false, false, false, false, false, false, true, false);
            addBisector("close to", 0, false, false, true, true, false, false, false, false, false);

            addBisector("an important part of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("considered a part of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("considered part of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("considered to be part of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("a part of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("part of", 0, false, false, false, false, false, false, false, false, true);

            addBisector("north of", 0, false, false, true, true, false, false, false, false, false);
            addBisector("south of", 0, false, false, true, true, false, false, false, false, false);
            addBisector("east of", 0, false, false, true, true, false, false, false, false, false);
            addBisector("west of", 0, false, false, true, true, false, false, false, false, false);
            addBisector("dress in", 0, false, false, false, false, false, true, false, false, false);
            addBisector("know how", 0, false, false, false, false, false, true, false, false, false);
            addBisector("on the", 0, false, false, true, false, false, false, false, false, false);
            addBisector("on a", 0, false, false, true, false, false, false, false, false, false);
            addBisector("done by", 0, false, false, false, false, true, true, false, false, false);
            addBisector("cause harm", -10, false, false, false, false, true, true, false, false, false);
            addBisector("from the", 0, false, false, true, false, false, false, false, false, false);
            addBisector("done with", 0, false, false, false, false, true, false, false, true, false);
            addBisector("made by", 0, false, false, false, false, true, false, false, true, false);
            addBisector("made with", 0, false, false, false, false, true, false, false, true, false);
            addBisector("to make", 0, false, false, false, false, true, false, false, true, false);

            addBisector("with some", 0, false, false, false, false, false, false, false, false, false);
            addBisector("need some", 0, false, false, false, false, false, false, false, false, false);
            addBisector("need a", 0, false, false, false, false, false, false, false, false, false);
            addBisector("need the", 0, false, false, false, false, false, false, false, false, false);
            addBisector("require a", 0, false, false, false, false, false, false, false, false, false);
            addBisector("require some", 0, false, false, false, false, false, false, false, false, false);
            addBisector("with a", 0, false, false, false, false, false, false, false, false, false);
            addBisector("over the", 0, false, false, false, false, false, false, false, false, false);

            addBisector("in a", 0, false, false, true, false, false, false, false, false, false);
            addBisector("be a", 0, true, false, false, false, false, false, false, false, false);
            addBisector("in the", 0, false, false, true, false, false, false, false, false, false);
            addBisector("at a", 0, false, false, true, false, false, false, false, false, false);

            addBisector("a day", 0, false, false, false, false, false, false, true, false, false);
            addBisector("per day", 0, false, false, false, false, false, false, true, false, false);
            addBisector("born with", 0, false, false, false, false, false, false, false, false, false);
            addBisector("with", 0, false, false, false, false, false, false, false, false, false);
            addBisector("use", 0, false, false, false, false, false, false, false, true, false);
            addBisector("from", 0, false, false, false, false, false, false, false, false, false);
            addBisector("when", 0, false, false, false, false, false, false, true, false, false);

            addBisector("have", 0, false, true, false, false, false, false, false, false, false);
            addBisector("more than", 0, false, false, true, false, false, false, false, false, false);
            addBisector("live in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("located in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("born in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("grow in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("grown in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("drink", 0, false, false, true, false, false, false, false, false, false);
            addBisector("drinks", 0, false, false, true, false, false, false, false, false, false);
            addBisector("die in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("in", 0, false, false, true, false, false, false, false, false, false);
            addBisector("without", -1, false, false, false, false, false, false, false, false, false);
            addBisector("wear", 0, false, false, false, false, false, true, false, false, false);
            addBisector("has", 0, false, true, false, false, false, false, false, false, false);

            addBisector("during", 0, false, false, false, false, false, false, true, false, false);
            addBisector("and", 0, false, false, false, false, false, false, false, false, false);
            addBisector("discover", 0, false, false, false, false, false, false, false, false, false);

            addBisector("love", 10, false, false, false, false, false, true, false, false, false);
            addBisector("disslike", -10, false, false, false, false, false, true, false, false, false);
            addBisector("make", 0, false, false, false, false, true, false, false, false, false);

            addBisector("buy", 0, false, false, false, false, false, false, false, false, false);

            addBisector("nearby", 0, false, false, true, false, false, false, false, false, false);
            addBisector("usually contain", 0, false, false, true, false, false, false, false, false, false);
            addBisector("contain", 0, false, false, true, false, false, false, false, false, false);

            addBisector("mean", 0, false, false, false, false, false, false, false, false, false);
            addBisector("kept", 0, false, false, false, false, false, false, false, false, false);
            addBisector("truely", 0, false, false, false, false, false, false, false, false, false);
            addBisector("invent", 0, false, false, false, false, false, false, false, false, false);
            addBisector("detect", 0, false, false, false, false, false, false, false, false, false);

            addBisector("move", 0, false, false, true, false, true, false, false, false, false);
            addBisector("carry", 0, false, false, true, false, false, false, false, false, false);

            addBisector("manufacture", 0, false, false, false, false, false, false, false, true, false);

            addBisector("produce", 0, false, false, false, false, true, false, false, true, false);

            addBisector("emmit", 0, false, false, false, false, true, false, false, true, false);
            addBisector("cause", 0, false, false, false, false, true, false, false, false, false);
            addBisector("to write", 0, false, false, false, false, false, true, false, false, false);
            addBisector("write", 0, false, false, false, false, false, true, false, false, false);
            addBisector("conserve", 1, false, false, false, false, true, false, false, false, false);
            addBisector("equal", 0, false, false, false, true, false, false, false, false, false);

            addBisector("while", 0, false, false, false, false, false, false, false, false, false);
            addBisector("whilst", 0, false, false, false, false, false, false, false, false, false);
            addBisector("normally", 0, false, false, false, false, false, false, true, false, false);
            addBisector("needs", 0, false, false, false, false, false, false, false, false, false);
            addBisector("look like", 0, false, false, false, false, false, false, false, false, false);
            addBisector("like", 0, false, false, false, false, false, false, false, false, false);

            addBisector("enjoy", 10, false, false, false, false, false, true, false, false, false);
            addBisector("kill", -10, false, false, false, false, false, true, false, false, false);
            addBisector("adore", 20, false, false, false, false, false, true, false, false, false);

            addBisector("always", 0, false, false, false, false, false, false, true, false, false);

            addBisector("travel", 0, false, false, true, false, false, false, true, false, false);

            addBisector("start", 0, false, false, false, false, false, false, true, false, false);
            addBisector("started", 0, false, false, false, false, false, false, true, false, false);
            addBisector("really", 0, false, false, false, false, false, false, false, false, false);
            addBisector("repair", 8, false, false, false, false, false, false, false, true, false);
            addBisector("teach", 5, false, false, false, false, false, true, false, false, false);

            addBisector("precede", 0, false, false, false, false, false, false, true, false, false);

            addBisector("educate", 5, false, false, false, false, false, true, false, true, false);
            addBisector("celebrate", 15, false, false, false, false, false, true, false, false, false);
            addBisector("celebrated", 15, false, false, false, false, false, true, true, false, false);

            addBisector("inherently", 0, false, false, false, false, false, false, false, false, false);
            addBisector("consumes", 0, false, false, false, false, false, false, false, false, false);

            addBisector("never", -2, false, false, false, false, false, false, true, false, false);
            addBisector("enjoys", 10, false, false, false, false, false, false, false, false, false);
            addBisector("enjoy", 10, false, false, false, false, false, false, false, false, false);
            addBisector("win", 20, false, false, false, false, false, true, false, false, false);
            addBisector("won", 20, false, false, false, false, false, true, false, false, false);
            addBisector("lost", -20, false, false, false, false, false, false, false, false, false);
            addBisector("hate", -20, false, false, false, false, false, true, false, false, false);
            addBisector("lose", -20, false, false, false, false, false, false, false, false, false);
            addBisector("eat", 0, false, false, false, false, false, true, false, false, false);
            addBisector("travel", 0, false, false, true, false, false, false, true, false, false);
            addBisector("improve", 10, false, false, false, false, true, false, false, true, false);

            addBisector("measure", 0, false, false, false, false, false, false, false, false, false);
            addBisector("sometimes", 0, false, false, false, false, false, false, true, false, false);
            addBisector("eventually", 0, false, false, false, false, false, false, true, false, false);
            addBisector("pronounced", 0, false, false, false, false, false, false, false, false, false);

            addBisector("ruin", -10, false, false, false, false, false, false, false, false, false);
            addBisector("fired", -10, false, false, false, false, false, false, false, false, false);
            addBisector("break", -5, false, false, false, false, false, false, false, false, false);
            addBisector("a", 0, true, false, false, false, false, false, false, false, false);

            addBisector("not", 0, false, false, false, false, false, false, false, false, true);
            addBisector("are made to be", 0, false, false, false, false, false, false, false, false, true);
            addBisector("were made to be", 0, false, false, false, false, false, false, false, false, true);
            addBisector("be", 0, false, false, false, false, false, false, false, false, true);

            addBisector("is called", 0, true, false, false, false, false, false, false, false, false);
            addBisector("is", 0, true, false, false, false, false, false, false, false, false);
            addBisector("an", 0, true, false, false, false, false, false, false, false, false);

            addBisector("called", 0, false, false, false, false, false, false, false, false, true);
            addBisector("nickname of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("capable of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("of", 0, false, false, false, false, false, false, false, false, true);
            addBisector("for", 0, false, false, false, false, false, false, false, false, true);
            addBisector("from", 0, false, false, false, false, false, false, false, false, true);
            addBisector("to", 0, false, false, false, false, false, false, false, false, false);
            addBisector("another word for", 0, false, false, false, false, false, false, false, false, false);
            addBisector("for", 0, false, false, false, false, false, false, false, true, false);
            addBisector("still", 0, false, false, false, false, false, false, true, false, false);
            addBisector("harms", 0, false, false, false, false, true, false, false, false, false);
            addBisector("breathe", 0, false, false, false, false, true, false, false, false, false);
            addBisector("ride", 0, false, false, false, false, true, false, false, false, false);
            addBisector("at", 0, false, false, false, false, true, false, false, false, false);
            addBisector("stick onto", 0, false, false, false, false, true, false, false, false, false);
            addBisector("stick to", 0, false, false, false, false, true, false, false, false, false);
            addBisector("adhere to", 0, false, false, false, false, true, false, false, false, false);
            addBisector("repel", 0, false, false, false, false, true, false, false, false, false);
            addBisector("thicker than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("thinner than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("darker than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("lighter than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("followed by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("followed with", 0, false, false, false, false, true, false, false, false, false);
            addBisector("preceded by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("preceded with", 0, false, false, false, false, true, false, false, false, false);
            addBisector("live by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("under", 0, false, false, false, false, true, false, false, false, false);
            addBisector("grow on", 0, false, false, false, false, true, false, false, false, false);
            addBisector("grow in", 0, false, false, false, false, true, false, false, false, false);
            addBisector("angrier than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("warmer than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("hotter than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("colder than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("better than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("bigger than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("taller than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("smaller than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("injure", 0, false, false, false, false, true, false, false, false, false);
            addBisector("paint", 0, false, false, false, false, true, false, false, false, false);
            addBisector("drink", 0, false, false, false, false, true, false, false, false, false);
            addBisector("land on", 0, false, false, false, false, true, false, false, false, false);
            addBisector("tell", 0, false, false, false, false, true, false, false, false, false);
            addBisector("rule the", 0, false, false, false, false, true, false, false, false, false);
            addBisector("the same", 0, false, false, false, false, true, false, false, false, false);
            addBisector("the", 0, false, false, false, false, true, false, false, false, false);
            addBisector("sell", 0, false, false, false, false, true, false, false, false, false);
            addBisector("reflect", 0, false, false, false, false, true, false, false, false, false);
            addBisector("emit", 0, false, false, false, false, true, false, false, false, false);
            addBisector("raise", 0, false, false, false, false, true, false, false, false, false);
            addBisector("breathe better", 0, false, false, false, false, true, false, false, false, false);
            addBisector("ingested by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("taken by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("affected by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("effected by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("usually", 0, false, false, false, false, true, false, false, false, false);
            addBisector("endanger", 0, false, false, false, false, true, false, false, false, false);
            addBisector("contains", 0, false, false, false, false, true, false, false, false, false);
            addBisector("pollenate", 0, false, false, false, false, true, false, false, false, false);
            addBisector("require", 0, false, false, false, false, true, false, false, false, false);
            addBisector("recommend that", 0, false, false, false, false, true, false, false, false, false);
            addBisector("recommend", 0, false, false, false, false, true, false, false, false, false);
            addBisector("split", 0, false, false, false, false, true, false, false, false, false);
            addBisector("greater than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("more than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("less than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("printed on", 0, false, false, false, false, true, false, false, false, false);
            addBisector("keep on", 0, false, false, false, false, true, false, false, false, false);
            addBisector("continue to", 0, false, false, false, false, true, false, false, false, false);
            addBisector("continue", 0, false, false, false, false, true, false, false, false, false);
            addBisector("used as", 0, false, false, false, false, true, false, false, false, false);
            addBisector("on other", 0, false, false, false, false, true, false, false, false, false);
            addBisector("likes", 0, false, false, false, false, true, false, false, false, false);
            addBisector("on", 0, false, false, false, false, true, false, false, false, false);
            addBisector("often", 0, false, false, false, false, true, false, false, false, false);
            addBisector("after", 0, false, false, false, false, true, false, false, false, false);
            addBisector("see through", 0, false, false, false, false, true, false, false, false, false);
            addBisector("see with", 0, false, false, false, false, true, false, false, false, false);
            addBisector("born", 0, false, false, false, false, true, false, false, false, false);
            addBisector("frequently", 0, false, false, false, false, true, false, false, false, false);
            addBisector("into", 0, false, false, false, false, true, false, false, false, false);
            addBisector("increase your", 0, false, false, false, false, true, false, false, false, false);
            addBisector("increase their", 0, false, false, false, false, true, false, false, false, false);
            addBisector("decrease their", 0, false, false, false, false, true, false, false, false, false);
            addBisector("decrease your", 0, false, false, false, false, true, false, false, false, false);
            addBisector("reduce their", 0, false, false, false, false, true, false, false, false, false);
            addBisector("reduce your", 0, false, false, false, false, true, false, false, false, false);
            addBisector("ruin their", 0, false, false, false, false, true, false, false, false, false);
            addBisector("hope their", 0, false, false, false, false, true, false, false, false, false);
            addBisector("ruin your", 0, false, false, false, false, true, false, false, false, false);
            addBisector("motivate", 0, false, false, false, false, true, false, false, false, false);
            addBisector("provide", 0, false, false, false, false, true, false, false, false, false);
            addBisector("have", 0, false, false, false, false, true, false, false, false, false);
            addBisector("need to be", 0, false, false, false, false, true, false, false, false, false);
            addBisector("need to", 0, false, false, false, false, true, false, false, false, false);
            addBisector("need", 0, false, false, false, false, true, false, false, false, false);
            addBisector("someday be", 0, false, false, false, false, true, false, false, false, false);
            addBisector("someday become", 0, false, false, false, false, true, false, false, false, false);
            addBisector("eventually be", 0, false, false, false, false, true, false, false, false, false);
            addBisector("eventually become", 0, false, false, false, false, true, false, false, false, false);
            addBisector("too many", 0, false, false, false, false, true, false, false, false, false);
            addBisector("too much", 0, false, false, false, false, true, false, false, false, false);
            addBisector("taste", 0, false, false, false, false, true, false, false, false, false);
            addBisector("smell", 0, false, false, false, false, true, false, false, false, false);
            addBisector("take", 0, false, false, false, false, true, false, false, false, false);
            addBisector("jump off of", 0, false, false, false, false, true, false, false, false, false);
            addBisector("jump off", 0, false, false, false, false, true, false, false, false, false);
            addBisector("called", 0, false, false, false, false, true, false, false, false, false);
            addBisector("hurt", 0, false, false, false, false, true, false, false, false, false);
            addBisector("sniff", 0, false, false, false, false, true, false, false, false, false);
            addBisector("lay", 0, false, false, false, false, true, false, false, false, false);
            addBisector("watching", 0, false, false, false, false, true, false, false, false, false);
            addBisector("resign from", 0, false, false, false, false, true, false, false, false, false);
            addBisector("resign", 0, false, false, false, false, true, false, false, false, false);
            addBisector("undergo many", 0, false, false, false, false, true, false, false, false, false);
            addBisector("spoken about", 0, false, false, false, false, true, false, false, false, false);
            addBisector("create", 0, false, false, false, false, true, false, false, false, false);
            addBisector("play", 0, false, false, false, false, true, false, false, false, false);
            addBisector("best eaten", 0, false, false, false, false, true, false, false, false, false);
            addBisector("being", 0, false, false, false, false, true, false, false, false, false);
            addBisector("makes", 0, false, false, false, false, true, false, false, false, false);
            addBisector("burn", 0, false, false, false, false, true, false, false, false, false);
            addBisector("give birth", 0, false, false, false, false, true, false, false, false, false);
            addBisector("spelled backwards", 0, false, false, false, false, true, false, false, false, false);
            addBisector("backwards", 0, false, false, false, false, true, false, false, false, false);
            addBisector("receive", 0, false, false, false, false, true, false, false, false, false);
            addBisector("happier than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("people are", 0, false, false, false, false, true, false, false, false, false);
            addBisector("damage", 0, false, false, false, false, true, false, false, false, false);
            addBisector("run", 0, false, false, false, false, true, false, false, false, false);
            addBisector("follow", 0, false, false, false, false, true, false, false, false, false);
            addBisector("have", 0, false, false, false, false, true, false, false, false, false);
            addBisector("help to", 0, false, false, false, false, true, false, false, false, false);
            addBisector("help", 0, false, false, false, false, true, false, false, false, false);
            addBisector("invade", 0, false, false, false, false, true, false, false, false, false);
            addBisector("cut through", 0, false, false, false, false, true, false, false, false, false);
            addBisector("cut", 0, false, false, false, false, true, false, false, false, false);
            addBisector("betray", 0, false, false, false, false, true, false, false, false, false);
            addBisector("speak", 0, false, false, false, false, true, false, false, false, false);
            addBisector("prefer", 0, false, false, false, false, true, false, false, false, false);
            addBisector("outside", 0, false, false, false, false, true, false, false, false, false);
            addBisector("measured by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("serve as", 0, false, false, false, false, true, false, false, false, false);
            addBisector("serve", 0, false, false, false, false, true, false, false, false, false);
            addBisector("fall", 0, false, false, false, false, true, false, false, false, false);
            addBisector("protect", 0, false, false, false, false, true, false, false, false, false);
            addBisector("generally", 0, false, false, false, false, true, false, false, false, false);
            addBisector("are good at", 0, false, false, false, false, true, false, false, false, false);
            addBisector("are good", 0, false, false, false, false, true, false, false, false, false);
            addBisector("are bad at", 0, false, false, false, false, true, false, false, false, false);
            addBisector("are bad", 0, false, false, false, false, true, false, false, false, false);
            addBisector("more important than", 0, false, false, false, false, true, false, false, false, false);
            addBisector("generate", 0, false, false, false, false, true, false, false, false, false);
            addBisector("manufactured by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("manufactured using", 0, false, false, false, false, true, false, false, false, false);
            addBisector("made using", 0, false, false, false, false, true, false, false, false, false);
            addBisector("made by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("created using", 0, false, false, false, false, true, false, false, false, false);
            addBisector("created by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("powered by", 0, false, false, false, false, true, false, false, false, false);
            addBisector("powered using", 0, false, false, false, false, true, false, false, false, false);
            addBisector("powered with", 0, false, false, false, false, true, false, false, false, false);

        }
				
        /// <summary>
        /// create some common question prefixes
        /// </summary>
        private void createPrefixes()
        {
            addPrefix("would you");
            addPrefix("would you find a");
            addPrefix("would you find some");
            addPrefix("can i");
            addPrefix("should we");
            addPrefix("should a");
            addPrefix("should the");
            addPrefix("should i");
            addPrefix("do most");
            addPrefix("can we");
            addPrefix("can a");
            addPrefix("can there");
            addPrefix("can the");
            addPrefix("has the");
            addPrefix("has a");
            addPrefix("has there");
            addPrefix("were a");
            addPrefix("was there");
            addPrefix("was a");
            addPrefix("did a");
            addPrefix("did the");
            addPrefix("does the");
            addPrefix("does there");
            addPrefix("were there");
            addPrefix("is there");
            addPrefix("are there");
            addPrefix("can most");
            addPrefix("does a");
            addPrefix("do you");
            addPrefix("is the");
            addPrefix("is a");
            addPrefix("is it");
            addPrefix("does the");
            addPrefix("is it");
            addPrefix("will a");
            addPrefix("will an");
            addPrefix("do");
            addPrefix("is");
            addPrefix("was");
            addPrefix("the");
            addPrefix("they");
            addPrefix("about");
            addPrefix("are");
            addPrefix("well");
            addPrefix("due");
            addPrefix("led");
            addPrefix("were");
            addPrefix("did");
            addPrefix("has");
            addPrefix("can");
            addPrefix("should");
            addPrefix("will");
            addPrefix("must");
            addPrefix("usually");
            addPrefix("when");
            addPrefix("not");
			addPrefix("does");
        }
				
		#endregion
		
		#region "exporting"
		
		public void ExportAsPOP(string filename)
		{
			StreamWriter oWrite = null;
			bool success = true;
            try
            {
                oWrite = File.CreateText(filename);
            }
            catch
            {
                success = false;
            }

            if (success)
            {			
				for (int i = mindpixels.Count-1; i >= 0; i--)
				{
					pixel p = (pixel)mindpixels[i];
					if (p.coherence > 0.5)
					{
					    string[] s = p.Bisector.Split(' ');
					    string bisector = "";
					    for (int j = 0; j < s.Length; j++) bisector += s[j];
					    string pop_str = "[[" + p.object1 + "] " + bisector + " [" + p.object2 + "]]";
					    oWrite.WriteLine(pop_str);
					}
				}
				oWrite.Close();
			}
		}

		public void ExportAsNARS(string filename)
		{
			StreamWriter oWrite = null;
			bool success = true;
            try
            {
                oWrite = File.CreateText(filename);
            }
            catch
            {
                success = false;
            }

            if (success)
            {			
				for (int i = mindpixels.Count-1; i >= 0; i--)
				{
					pixel p = (pixel)mindpixels[i];
					if ((p.Bisector == "is a") ||
					    (p.Bisector == "is") ||
					    (p.Bisector == "has") ||
					    (p.Bisector == "in"))
					{
						
						if (p.Bisector == "is a") p.Bisector = "isa";
						
						//string[] s = p.Bisector.Split(' ');
						//string bisector = "";
						//for (int j = 0; j < s.Length; j++) bisector += s[j];
						string nars_str = "<" + p.object1 + " --> " + p.object2 + ">, " + p.coherence.ToString();
						oWrite.WriteLine(nars_str);
					}
				}
				oWrite.Close();
			}
		}
				
		public void ExportAsOpenCogScheme(string filename)
		{
			StreamWriter oWrite = null;
			bool success = true;
            try
            {
                oWrite = File.CreateText(filename);
            }
            catch
            {
                success = false;
            }

            if (success)
            {									
				for (int i = mindpixels.Count-1; i >= 0; i--)
				{
					pixel p = (pixel)mindpixels[i];
					
					if (p.Bisector == "is a") p.Bisector = "isa";
					
                    oWrite.WriteLine("EvaluationLink  (stv 1 " + p.coherence.ToString() + ")");
                    oWrite.WriteLine("    (PredicateNode " + '"' + p.Bisector + '"' +  ")");
                    oWrite.WriteLine("    (ListLink");
                    oWrite.WriteLine("       (ConceptNode " + '"' + p.object1 + '"' + ")");
                    oWrite.WriteLine("       (ConceptNode " + '"' + p.object2 + '"' + ")");
                    oWrite.WriteLine("    )");
                    oWrite.WriteLine("    )");
				}
				oWrite.Close();
			}
		}
		
		#endregion
	}
}
