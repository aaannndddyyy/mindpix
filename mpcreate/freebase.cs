/*
    Functions for grabbing data from Freebase TSV
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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace mpcreate
{
	public class freebase
	{
		int total_pixels_extracted;
		
		public void ProccessFreebase(string directory, string mindpixel_filename)
		{
			StreamWriter oWrite = File.CreateText(mindpixel_filename);
			oWrite.WriteLine("Mindpixels extracted from Freebase");
			oWrite.WriteLine("Another Mind Hack");
			oWrite.Close();
			
			ReadDirectory(directory, mindpixel_filename);
			
			Console.WriteLine(total_pixels_extracted.ToString() + " mindpixels extracted");			
		}
		
        /// <summary>
        /// save the given question to the mindpixel file
        /// </summary>
        /// <param name="question"></param>
        private void SaveQuestion(
		    string filename, 
		    string question)
        {
			float coherence = 1;
            StreamWriter oWrite = null;
            bool allowWrite = true;
            string mindpixel_str;
            string coherence_str;
            int coherence_int;
			
		    Console.WriteLine(question);
		    total_pixels_extracted++;
						
            try
            {
				if (File.Exists(filename))
                    oWrite = File.AppendText(filename);
				else
					oWrite = File.CreateText(filename);
            }
            catch
            {
                allowWrite = false;
            }
	
            if (allowWrite)
            {
                coherence_int = (int)(coherence * 100);
                coherence_str = Convert.ToString(coherence_int);
                coherence_str = coherence_str.Substring(0, 1) + "." + coherence_str.Substring(1, 2);
                mindpixel_str = Convert.ToString((char)9) + coherence_str + Convert.ToString((char)9) + question;
                oWrite.WriteLine(mindpixel_str);
                oWrite.Close();
            }       
        }

		private ArrayList GetFields(string tsv)
		{
			ArrayList field = new ArrayList();
			
			string txt = "";
			char[] ch = tsv.ToCharArray();
			
			for (int i = 0; i < ch.Length; i++)
			{
				if (ch[i] == 9)
				{
					field.Add(txt);
					txt = "";
				}
				else
				{
					if ((ch[i] != 13) && (ch[i] != 10))
					    txt += ch[i];							
				}
			}
			field.Add(txt);
			return(field);
		}		
		
		private void ProcessFreebaseEntry(string tsv_filename, string tsv, ArrayList fieldname, string mindpixel_filename)
		{
			string txt = "";

			char[] ch2 = tsv_filename.ToCharArray();
			string object_type = "";
			for (int i = ch2.Length-1; i >= 0; i--)
			{
				if (ch2[i]=='.') 
				{
					object_type = "";
				}
				else
				{
					if ((ch2[i] == '/') || (ch2[i] == '\\'))
                    {
						break;
                    }
					else
					{
						if (ch2[i] == '_') ch2[i] = ' ';
				        object_type = ch2[i] + object_type;
					}
				}
			}
			string[] str2 = object_type.Split(' ');
			if (str2.Length>1)
			{
				if (str2[str2.Length-1] == "type")
				{
					str2[str2.Length-1] = "";
				}
				object_type = "";
				for (int i = 0; i < str2.Length; i++)
				{
					object_type += str2[i];
					if (i < str2.Length-1)
					{
						if (str2[i+1] != "") object_type += " ";
					}
				}
			}

			
			string[] field = tsv.Split((char)9);
			
			string object_name = "";
			for (int j = 0; j < field.Length; j++)
			{
				string field_name = (string)fieldname[j];
				if ((field_name != "") && (field_name != "id"))
				{
					if (j == 0)
					{
						object_name = ((string)field[j]).Trim();
						if (object_name != "")
						{
							if ((object_type != "deceased person") &&
							    (object_type != "measured person") &&
							    (!object_name.Contains("guid/")))
							{
		                        string question = "Is " + object_name + " a type of " + object_type + "?";
							    //Console.WriteLine(question);
		                        SaveQuestion(mindpixel_filename, question);
							}
						}
					}
					else
					{			
						if (!((string)field[j]).Contains("guid/"))
						{
							string question = "";
							string f = (string)fieldname[j];
							string[] str = ((string)field[j]).Split(',');
							for (int k = 0; k < str.Length; k++)
							{
								if (str[k].Length>3)
								{
									string[] str3 = str[k].Split(' ');
									question = "";
									if (field_name == "date_of_birth")
									{
										if (str3.Length == 1)
										{
											if (str[k].Contains("-"))
										        question = "Was " + object_name + " born on " + str[k] + "?";
											else
												question = "Was " + object_name + " born in " + str[k] + "?";
										}
										else
											question = "Was " + object_name + " born on " + str[k] + "?";
										//Console.WriteLine(question);
									}
									if (field_name == "date_of_death")
									{
										if (str3.Length == 1)
										{
											if (str[k].Contains("-"))
										        question = "Did " + object_name + " die on " + str[k] + "?";
											else
												question = "Did " + object_name + " die in " + str[k] + "?";
										}
										else
											question = "Did " + object_name + " die on " + str[k] + "?";
										//Console.WriteLine(question);
									}
									if (field_name == "cause_of_death")
									{
										question = "Did " + object_name + " die from " + str[k] + "?";
										//Console.WriteLine(question);
									}
									if (field_name == "place_of_burial")
									{
										question = "Was " + object_name + " buried in " + str[k] + "?";
										//Console.WriteLine(question);
									}
									if (field_name == "discoverer")
									{
										question = "Did " + str[k] + " discover " + object_name + "?";
										//Console.WriteLine(question);
									}
									if (field_name == "exoplanets_discovered")
									{
										question = "Is " + str[k] + " an exoplanet?";
										question = "Was " + str[k] + " discovered using " + object_name + "?";
										//Console.WriteLine(question);
									}
									if (object_type == "cause of death")
									{
										question = "Did " + str[k] + " die from " + object_name + "?";
										//Console.WriteLine(question);
									}
									if (field_name == "languages_spoken")
									{
										question = "Does " + object_name + " speak the language " + str[k] + "?";
										//Console.WriteLine(question);
									}
									if (field_name == "orbits")
									{
										question = "Does " + object_name + " orbit around " + str[k] + "?";
										//Console.WriteLine(question);
									}
									if (object_type == "ethnicity")
									{
										question = "Did " + str[k] + " belong to the ethnic group " + object_name + "?";
										//Console.WriteLine(question);
									}
									if ((object_type == "family") && (field_name == "members"))
									{
										question = "Did " + str[k] + " belong to the dynasty " + object_name + "?";
										//Console.WriteLine(question);
									}
									if ((object_type == "profession") && (field_name == "people_with_this_profession"))
									{
										question = "Does " + str[k] + " have the occupation " + object_name + "?";
										//Console.WriteLine(question);
									}
									if ((object_type == "professional field") && (field_name == "professions_in_this_field"))
									{
										question = "Is " + str[k] + " in the professional field " + object_name + "?";
										//Console.WriteLine(question);
									}								
									
									//question = field_name + "(" + object_name + ", " + str[k] + ")";
									//Console.WriteLine(question);
									
									if (question != "")
									{
										//Console.WriteLine(question);
					                    SaveQuestion(mindpixel_filename, question);
									}
								}
							}
						}
					}
				}
			}
		}
		
		private void ReadDirectory(string directory, string mindpixel_filename)
		{
			total_pixels_extracted = 0;
            if (Directory.Exists(directory))
            {
				Console.WriteLine("Processing directory " + directory);
				
				string[] dir = Directory.GetDirectories(directory);
				if (dir != null)
				{
					for (int j = 0; j < dir.Length; j++)
					{
						ReadDirectory(dir[j], mindpixel_filename);
				    }
				}
				
                string[] filename = Directory.GetFiles(directory, "*.tsv");
                if (filename != null)
                {
					for (int j = 0; j < filename.Length; j++)
					{
						if (File.Exists(filename[j]))
						{
							Console.WriteLine("");
							Console.WriteLine("Reading: " + filename[j]);
							Console.WriteLine("");
							
							string tsv = "";
							bool firstline = true;
							ArrayList fieldname = null;
							StreamReader oRead = File.OpenText(filename[j]);
							while ((tsv != null) && (!oRead.EndOfStream))
							{
							    tsv = oRead.ReadLine();
								if (tsv != null)
								{
									if (firstline)
										fieldname = GetFields(tsv);
									else
							            ProcessFreebaseEntry(filename[j], tsv, fieldname, mindpixel_filename);
									firstline = false;
								}
							}
							oRead.Close();
						}
					}
			    }
			}	
		}
	}
}
