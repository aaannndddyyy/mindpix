/*
    Functions for grabbing data from Wikipedia Schools edition
    Copyright (C) 2008 Bob Mottram
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
    public class WikipediaFactGrabber
    {
	    /// <summary>
	    /// this class stores information about a wikipedia entry
	    /// </summary>
	    internal class WikipediaData
	    {
	        public string Title = "";
	        public string[] content = new string[2000];
			public int content_entries;
	        public ArrayList categories = new ArrayList();
	    }
				
        //private ScreenScraper screenscrape;
        private ArrayList bisectors;
        private ArrayList finals;
        //private int bisector_position = 0;
        private string bisector_text = "";
        private string current_question = "";
		public int total_pixels_extracted;

        public WikipediaFactGrabber()
        {
            bisectors = new ArrayList();
            finals = new ArrayList();			

			bisectors.Add("occurred on");
			bisectors.Add("occured on");
			bisectors.Add("occurred between");
			bisectors.Add("occured between");
			bisectors.Add("was launched on");
			bisectors.Add("has an orbital eccentricity of");
			bisectors.Add("has an orbital period of");
			bisectors.Add("had the vice president");
			bisectors.Add("discovered");
			bisectors.Add("attended");
			bisectors.Add("born on");
			bisectors.Add("born in");
			bisectors.Add("died on");
			bisectors.Add("died in");
			bisectors.Add("was discovered on");
			bisectors.Add("was advised by");
			bisectors.Add("was crowned on");
			bisectors.Add("reigned between");
			bisectors.Add("follows the school of thought");
			bisectors.Add("has ideas in");
			bisectors.Add("influenced");
			bisectors.Add("has the official language");
			bisectors.Add("has the genre");
			bisectors.Add("lives in");
			bisectors.Add("plays the instrument");
			bisectors.Add("is associated with");
			bisectors.Add("has the government type");
			bisectors.Add("uses the currency");
			bisectors.Add("has the occupation");
			bisectors.Add("belongs to the subspecies");
            bisectors.Add("has the conservation status");
            bisectors.Add("won awards");
            bisectors.Add("has the latin name");
            bisectors.Add("has the occupation");
			bisectors.Add("is in the time zone");
            bisectors.Add("is a common type of");
            bisectors.Add("is a brand of");
			bisectors.Add("is interested in");
            bisectors.Add("is a form of");
            bisectors.Add("is a popular form of");
            bisectors.Add("is a common form of");
            bisectors.Add("is a type of");
            bisectors.Add("is a popular type of");
            bisectors.Add("is a common type of");
            bisectors.Add("is a name used by");
            bisectors.Add("is a first name of");
            bisectors.Add("is a last name of");
            bisectors.Add("is the first name of");
            bisectors.Add("is the last name of");
            bisectors.Add("is the name used by");
            bisectors.Add("is a name given to");
            bisectors.Add("is the name given to");
            bisectors.Add("is the creator of");
            bisectors.Add("was the creator of");
            bisectors.Add("was the author of");
            bisectors.Add("is the author of");
            bisectors.Add("is the name of");
            bisectors.Add("is also known as");
            bisectors.Add("were better known as");
            bisectors.Add("are better known as");
            bisectors.Add("is better known as");
			bisectors.Add("is otherwise known as");
            bisectors.Add("is commonly known as");
            bisectors.Add("is alternatively known as");
            bisectors.Add("is typically known as");
            bisectors.Add("is usually known as");
            bisectors.Add("was the name of");
            bisectors.Add("is a system of");
            bisectors.Add("is a popular");
            bisectors.Add("were a popular");
            bisectors.Add("is a character from");            
            bisectors.Add("is a character in");
            bisectors.Add("were characters from");
            bisectors.Add("were characters in");
            bisectors.Add("is located in");            
            bisectors.Add("is located at");
            bisectors.Add("is located near");
            bisectors.Add("was built in");
            bisectors.Add("is a song from");
            bisectors.Add("is a tune from");
            bisectors.Add("is a station on");
            bisectors.Add("was appointed to the role of");
            bisectors.Add("is a former");
            bisectors.Add("was built");
            bisectors.Add("is located");            
            bisectors.Add("were the");
            bisectors.Add("were an");
            bisectors.Add("were a");
            bisectors.Add("was the");
            bisectors.Add("was an");
            bisectors.Add("was a");
			bisectors.Add("is the oldest");
			bisectors.Add("is the most recent");
			bisectors.Add("is the current");
			bisectors.Add("is the newest");
			bisectors.Add("is the largest");
			bisectors.Add("is the smallest");
			bisectors.Add("is the statement");
            bisectors.Add("is the");
            bisectors.Add("is officially");			
            bisectors.Add("is an");
            bisectors.Add("is a");
            bisectors.Add("was");

            finals.Add(",");
            finals.Add("performed by");
            finals.Add("owned by");
            finals.Add("directed by");
            finals.Add("discovered");
            finals.Add("based in");
            finals.Add("located");
            finals.Add("established");
            finals.Add("which");
            finals.Add("in");
            finals.Add("from");
            finals.Add("by");
            finals.Add("was");            
        }

        /// <summary>
        /// returns the position of a bisector within the given sentence
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        private int getBisector(
		    string sentence)
        {
            int pos = -1;
            int i;

            //bisector_position = -1;
            bisector_text = "";
            i = 0;
            while ((i < bisectors.Count) && (pos<0))
            {
                pos = sentence.IndexOf(" " + (string)bisectors[i] + " ");
                if (pos > 0)
                {
                    //bisector_position = pos;
                    bisector_text = (string)bisectors[i];
                }
                i++;
            }
            return (pos);
        }

        private int getFinal(
		    string sentence)
        {
            int pos = -1;
            int i;

            i = 0;
            while ((i < finals.Count) && (pos < 0))
            {
                if ((string)finals[i] != ",")
                    pos = sentence.IndexOf(" " + (string)finals[i] + " ");
                else
                    pos = sentence.IndexOf((string)finals[i] + " ");
                i++;
            }
			if (pos ==-1) pos = sentence.Length;
            return (pos);
        }


        private static string removeBrackets(
		    string text)
        {
            string newtext = "";
            string ch;
            int i;
            bool reading = true;

            if (text != null)
            {
                for (i = 0; i < text.Length; i++)
                {
                    ch = text.Substring(i, 1);
                    if (ch == "(") reading = false;
                    if (reading) newtext += ch;
                    if (ch == ")") reading = true;
                }
            }
            return (newtext);
        }

        /// <summary>
        /// parse the given statement as a question, which can then be added to the GAC file
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        private string ParseAsQuestion(
		    string title, 
		    string description)
        {
            string question = "";
            string question_str = "";
            int pos, i;
            string[] words;
            string object2;

            //remove any bracketed information
            title = removeBrackets(title);
            description = removeBrackets(description);

            pos = getBisector(description);
            if (pos > -1)
            {
                if (title.Substring(title.Length - 1, 1) == " ") title = title.Substring(0, title.Length - 1);

                words = bisector_text.Split(' ');
				if (words[0] != "")
				{
	                question = words[0].Substring(0, 1).ToUpper();
	                question += words[0].Substring(1,words[0].Length-1) + " " + title + " ";
	                for (i = 1; i < words.Length; i++) question += words[i] + " ";
	                object2 = description.Substring(pos + bisector_text.Length);
	                pos = getFinal(object2);

					if (words[0] == "plays")
					{
						question = "Does " + title + " play the ";
					}
					
					if (words[0] == "has")
					{
						question = "Does " + title + " have ";
						for (i = 1; i < words.Length; i++) question += words[i] + " ";
					}
					
					if (words[0] == "born")
					{
						question = "Was " + title + " born ";
						for (i = 1; i < words.Length; i++) question += words[i] + " ";
					}
					
					if (words[0] == "had")
					{
						question = "Did " + title + " have ";
						for (i = 1; i < words.Length; i++) question += words[i] + " ";
					}

					if ((words[0] == "lives") ||
					    (words[0] == "belongs") ||
					    (words[0] == "follows") ||
					    (words[0] == "uses") ||
					    (words[0] == "influenced"))
					{						
						question = "Does " + title + " " + words[0].Substring(0,words[0].Length-1) + " ";
						for (i = 1; i < words.Length; i++) question += words[i] + " ";
					}

					if ((words[0] == "influenced") ||
					    (words[0] == "died"))
					{						
						question = "Did " + title + " " + words[0].Substring(0,words[0].Length-1) + " ";
						for (i = 1; i < words.Length; i++) question += words[i] + " ";
					}

					if (words[0] == "occurred")
					{						
						question = "Did " + title + " " + words[0].Substring(0,words[0].Length-3) + " ";
						for (i = 1; i < words.Length; i++) question += words[i] + " ";
					}
					
					if ((words[0] == "reigned") ||
					    (words[0] == "occured") ||
					    (words[0] == "attended"))
					{
						question = "Did " + title + " " + words[0].Substring(0,words[0].Length-2) + " ";
						for (i = 1; i < words.Length; i++) question += words[i] + " ";
					}
					
					if (words[0] == "won")
					{
						question = "Did " + title + " win the award ";
					}
					
					
					bool test = false;
					/*
					if (words[0] == "won")
					{
						test = true;
						question = "Did " + title + " win the award ";
					}
					*/
						
	                if (pos > 0)
	                {
	                    object2 = object2.Substring(0, pos);
	                    if (object2.Substring(object2.Length - 1, 1) == " ") object2 = object2.Substring(0, object2.Length - 1);
	                    words = object2.Split(' ');
	                    if ((words.Length > 0) && (words.Length < 10))  //avoid long descriptions
	                    {
	                        if (words[words.Length - 1].Length > 4)
	                        {
	                            if (words[words.Length - 1].Substring(words[words.Length - 1].Length - 2) == "ed")
	                                words[words.Length - 1] = "";
	                            else
	                            {
	                                if (words.Length > 2)
	                                {
	                                    if ((words[words.Length - 1].Substring(words[words.Length - 1].Length - 3) == "ing") &&
	                                        (words[words.Length - 2] != "in"))
	                                        words[words.Length - 1] = "";
	                                }
	                                else
	                                {
	                                    if (words[words.Length - 1].Substring(words[words.Length - 1].Length - 3) == "ing")
	                                        words[words.Length - 1] = "";
	                                }
	                            }
	                        }
	                        if (words[words.Length - 1] == "who") words[words.Length - 1] = "";
	                        if ((words[words.Length - 1] == "wrote") || (words[words.Length - 1] == "writes") || (words[words.Length - 1] == "first"))
	                        {
	                            words[words.Length - 1] = "";
	                            if (words.Length > 2)
	                                if (words[words.Length - 2] == "who") words[words.Length - 2] = "";
	                        }
	                        if (words[words.Length - 1] == "written") words[words.Length - 1] = "";
							if (words[words.Length - 1] == "that") words[words.Length - 1] = "";
							if (words[words.Length - 1] == "used") words[words.Length - 1] = "";
	                        if (words[words.Length - 1] == "in") words[words.Length - 1] = "";
	                        if (words[words.Length - 1] == "which") words[words.Length - 1] = "";
	
	                        object2 = "";
	                        for (i = 1; i < words.Length; i++)
	                        {
	                            object2 += words[i];
	                            if ((words[i] != "") && (i < words.Length-1)) object2 += " ";
	                        }
	                        if (object2 != "")
	                        {
	                            if (object2.Substring(object2.Length - 1, 1) == " ") object2 = object2.Substring(0, object2.Length - 1);
								
								question += object2;
															
								words = question.Split(' ');
								if (words.Length > 3)
								{
							        if (words[words.Length - 1] == "that") words[words.Length - 1] = "";
							        if (words[words.Length - 1] == "used") words[words.Length - 1] = "";
	                                if (words[words.Length - 1] == "in") words[words.Length - 1] = "";
	                                if (words[words.Length - 1] == "found") words[words.Length - 1] = "";
	                                if (words[words.Length - 1] == "written") words[words.Length - 1] = "";
	                                if (words[words.Length - 1] == "which") words[words.Length - 1] = "";
	                                if (words[words.Length - 1] == "who") words[words.Length - 1] = "";
	                                if (words[words.Length - 1] == "in") words[words.Length - 1] = "";
	                                if (words[words.Length - 1] == "made") words[words.Length - 1] = "";
									question = "";
	                                for (i = 0; i < words.Length; i++)
	                                {
	                                    question += words[i];
	                                    if ((words[i] != "") && (i < words.Length-1)) question += " ";
	                                }								
								}							
								
	                            question += "?";
								if (test) Console.WriteLine(question);
	                            question_str = question;
	                        }
	                    }
					}
                }
            }

            return (question_str);
        }
		
		private string ReplaceWord(string str, string word, string replacement)
		{
			int pos = str.IndexOf(word);
			while (pos > -1)
			{
				str = str.Substring(0,pos) + replacement + str.Substring(pos + word.Length);
				pos = str.IndexOf(word);
			}
			
			return(str);
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
			
			question = ReplaceWord(question,"&#39;","'");
			question = ReplaceWord(question,"&amp;","and");
			question = ReplaceWord(question,"&quot;","");
			question = ReplaceWord(question,"&iacute;","");
			question = ReplaceWord(question,"&#x159;","");
			question = ReplaceWord(question,"&#x1E45;","");
			question = ReplaceWord(question,"&#x15A;","");
			question = ReplaceWord(question,"&#x101;","");
			question = ReplaceWord(question,"&ocirc;","");
			question = ReplaceWord(question,"Portal:","");
			if (!question.Contains("&"))
			{
			    //Console.WriteLine(question);
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
        }
		
		private bool ContainString(ArrayList lst, string str)
		{
			bool result = false;
			for (int i = 0; i < lst.Count; i++)
			{
				if ((string)lst[i] == str)
				{
					result = true;
					break;
				}
			}
			return(result);
		}

        private void AddMindpixel(
		    string mindpixel_filename,
		    WikipediaData wiki_data,
		    int entry_number,
		    ref bool man_woman)
        {
            string question;
            string title, category, bisect, question_header;
            string[] words;
            string[] titlewords;
            bool possible_person;
            int i;
			string[] content = new string[3];
			ArrayList to_be_added = new ArrayList();
			
			content[0] = wiki_data.content[entry_number];
			content[1] = "";
			content[2] = "";
			
			int pos2=-1;
			int pos = wiki_data.content[entry_number].IndexOf(", ");
			if (pos > -1)
			{
			    pos2 = wiki_data.content[entry_number].IndexOf(", ", pos+1);
			}
			
			if ((pos < 50) && (pos > -1) && (pos2 > pos))
			{
				string str = wiki_data.content[entry_number].Substring(pos+1, pos2 - pos).Trim();				
				string[] str2 = str.Split(' ');
				if (str2.Length > 0)
				{
					string content1 = "";
					if ((str2[0].StartsWith("a")) ||
					    (str2[0].StartsWith("otherwise")) ||
					    (str2[0] == "the") ||
					    (str2[0].EndsWith("ly")))
					{
						content1 = wiki_data.content[entry_number].Substring(0, pos) + " is " + str;
					}
					else
					{
						if ((str2[0].StartsWith("or")) ||
						    (str2.Length == 1))
						{
							content1 = wiki_data.content[entry_number].Substring(0, pos) + " is otherwise known as " + str;
						}
						else
						{
						    content1 = wiki_data.content[entry_number].Substring(0, pos2);
						}
					}
					content[1] = ReplaceWord(content1,",","");
					string content2 = wiki_data.content[entry_number].Substring(0, pos) + wiki_data.content[entry_number].Substring(pos2);
					content[2] = ReplaceWord(content2,",","");
					//Console.WriteLine("Content1: " + content[1]);
					//Console.WriteLine("Content2: " + content[2]);
				}
			}
			
			for (int s = 0; s < 3; s++)
			{
				if (content[s] != "")
				{
					//if (content[1] != "") Console.WriteLine("question1: " + content[s]);					
					
		            question = ParseAsQuestion(wiki_data.Title, content[s]);
		            current_question = question;
					
					//if (content[1] != "") Console.WriteLine("question2: " + question);
		
		            title = wiki_data.Title.ToLower();
		            title = removeBrackets(title);
		
		            if (!title.Contains("list of "))
		            {
		                if ((question != "") && (!question.Contains("disambiguation")) && (!question.Contains("cleanup"))
		                     && (!question.Contains("wikipedia") && (!question.Contains("wikified")))
		                     && (!question.Contains("article lacking") && (!question.Contains("article needing")))
		                    )
						{
							if (!ContainString(to_be_added, question)) to_be_added.Add(question);
						}
		
		                possible_person = false;
		                titlewords = title.Split(' ');                                
		                for (i = 0; i < wiki_data.categories.Count; i++)
		                {
		                    category = (String)wiki_data.categories[i];
							Console.WriteLine("CATEGORY: " + category);
		                    if ((category != "") && (category != "disambiguation") &&
		                        (!category.Contains("cleanup")) && (!category.Contains("wikipedia") && (!category.Contains("wikified"))) &&
		                        (!category.Contains("article lacking") && (!category.Contains("article needing")) && (!category.Contains("article pending")))
		                        )
		                    {
		                        category = category.ToLower();
		                        words = category.Split(' ');                                
		
		                        bisect = "a type of";
		                        question_header = "Is ";
		
		                        if (category.Contains(" birth"))
		                        {
		                            question_header = "Was ";
		                            bisect = "born in";
		                            category = words[0];
		                            possible_person = true;
		                        }
		
		                        if (category.Contains(" death"))
		                        {
		                            question_header = "Did ";
		                            bisect = "die in";
		                            category = words[0];
		                            possible_person = true;
		                        }
		
		                        if (category.Contains("suicide"))
		                        {
		                            question_header = "Did ";
		                            bisect = "commit";
		                            category = words[0];
		                            possible_person = true;
		                        }
		
		                        if (category.Contains("people")) possible_person = true;
		                        if (category.Contains("living people")) category = "";
		                        if (category.Contains("list of ")) category = "";
		
		                        if ((category != "") && (category != "stub") && (!category.Contains("disambiguation")))
		                        {
		                            question = question_header + title + " " + bisect + " " + category + "?";
									if (!ContainString(to_be_added, question)) to_be_added.Add(question);
		                            if (current_question == "") current_question = question;
		                        }
		                    }
		                }
		
		                if ((!man_woman) &&
						    (((titlewords.Length == 2) ||
						    ((possible_person) && (titlewords.Length > 0)))))
		                {
		                    if (names.IsMaleName(titlewords[0]))
		                    {
		                        question = "Is " + title + " a man?";
								if (!ContainString(to_be_added, question)) to_be_added.Add(question);
								man_woman = true;
		                    }
		                    else
		                    {
		                        if (names.IsFemaleName(titlewords[0]))
		                        {
		                            question = "Is " + title + " a woman?";
									if (!ContainString(to_be_added, question)) to_be_added.Add(question);
									man_woman = true;
		                        }
		                    }
		                }
		                
		            }
  			        
				}
			}
			
			for (i = 0; i < to_be_added.Count; i++)
			{
				question = (string)to_be_added[i];
				string[] s2 = question.Split(' ');
				if (s2[s2.Length-1].Length > 3)
				{
					if ((!question.EndsWith(" born?")) &&
					    (!question.EndsWith(" and?")) &&
					    (!question.EndsWith(" usually?")) &&
					    (!question.EndsWith(" was?")) &&
					    (!question.EndsWith(" small?")) &&
					    (!question.EndsWith(" large?")) &&
					    (!question.EndsWith(" under?"))
					    )
					    SaveQuestion(mindpixel_filename, question);
				}
			}	
            
        }
				
        /// <summary>
        /// remove hypertext formatting
        /// </summary>
        /// <param name="textData"></param>
        private string RemoveFormatting(string textData)
        {
            bool reading = true;
            string newData = "";
            string c;
            int i;

            for (i = 0; i < textData.Length; i++)
            {
                c = textData.Substring(i, 1);
                if (c == "<") reading = false;
                if (reading) newData += c;
                if (c == ">") reading = true;
            }
			textData = newData;
			reading = true;
			newData="";
            for (i = 0; i < textData.Length; i++)
            {
                c = textData.Substring(i, 1);
                if (c == "(") reading = false;
                if (reading) newData += c;
                if (c == ")") reading = true;
            }
            return(newData);
        }
		
		public void ProcessWikipedia(
		    string wikipedia_directory,
		    string mindpixel_filename)
		{
			total_pixels_extracted = 0;
            if (Directory.Exists(wikipedia_directory))
            {
				Console.WriteLine("Processing directory " + wikipedia_directory);
				
				StreamWriter oWrite = File.CreateText(mindpixel_filename);
				oWrite.WriteLine("Mindpixels extracted from Wikipedia");
				oWrite.WriteLine("Another Mind Hack");
				oWrite.Close();
				
				for (int i = 0; i < 26; i++)
				{
					string directory = wikipedia_directory + "/" + (char)('a' + i);
                    string[] filename = Directory.GetFiles(directory, "*.htm");
                    if (filename != null)
                    {
						for (int j = 0; j < filename.Length; j++)
						{
							if (File.Exists(filename[j]))
							{
								StreamReader oRead = File.OpenText(filename[j]);
								string html = oRead.ReadToEnd();
								oRead.Close();
								ProcessWikipediaEntry(html, mindpixel_filename);
							}
						}
				    }
				}
			}	
			Console.WriteLine(total_pixels_extracted.ToString() + " mindpixels extracted");
		}
		
		private bool ContainsMonth(string data)
		{
			bool result = false;
			if ((data.Contains("Jan")) ||
			    (data.Contains("Feb")) ||
			    (data.Contains("Mar")) ||
			    (data.Contains("Apr")) ||
			    (data.Contains("May")) ||
			    (data.Contains("Jun")) ||
			    (data.Contains("Jul")) ||
			    (data.Contains("Aug")) ||
			    (data.Contains("Sep")) ||
			    (data.Contains("Oct")) ||
			    (data.Contains("Nov")) ||
			    (data.Contains("Dec")))
			    result = true;
			
			return(result);				
		}
		
		private void AddHeaderQuestion(WikipediaData wiki_data, string subject, string header, string data, bool probably_person)
		{
			string statement = "";
			if ((header == "Carbohydrates") ||
			    (header == "Fat") ||
			    (header == "Protein"))
			{
				if (header == "Carbohydrates") header = "carbohydrate";
				statement = subject + " has a " + header.ToLower() + " content of " + data;
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Birth name")
			{
				statement = subject + " was born named " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Conservation status")
			{
				statement = subject + " has the conservation status " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Location")
			{
				statement = subject + " is located in " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Date")
			{
				if (ContainsMonth(data))
				{
					if (data.Contains("-"))
					{
						string[] datastr = data.Split('-');
				        statement = subject + " occurred between " + datastr[0].Trim() + " and " + datastr[1].Trim();
					}
					else
					{
						statement = subject + " occurred on " + data;
					}
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if (header == "Binomial name")
			{
				string[] wrds = data.Split(',');
				statement = subject + " has the latin name " + wrds[0];
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Subspecies")
			{
				string[] wrds = data.Split(',');
				if (!wrds[0].ToLower().StartsWith("see "))
				{
				    statement = subject + " belongs to the subspecies " + wrds[0];
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if ((header == "Birthplace") ||
			    (header == "Birth place"))
			{
				statement = subject + " was born in " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Industry")
			{
				statement = subject + " belongs to the industry " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Time zone")
			{
				statement = subject + " is in the time zone " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Currency")
			{
				statement = subject + " uses the currency " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if ((header.ToLower() == "ethnic group") ||
			    (header.ToLower() == "ethnicity"))
			{
				statement = subject + " belongs to the ethnic group " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if ((header == "Preceded by") ||
			    (header == "Precededby"))
			{
				statement = subject + " was preceded by " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Previous mission"))
			{
				statement = subject + " was preceded by " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Memory")
			{
				statement = subject + " has a memory of " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Reign")
			{
				if (data.Contains("-"))
				{
					string[] str = data.Split('-');
				    statement = subject + " reigned between " + str[0].Trim() + " and " + str[1].Trim();
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if (header == "Coronation")
			{
		        statement = subject + " was crowned on " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Religious stance")
			{
		        statement = subject + " has the religious stance " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.ToLower().Contains("awards"))
			{
		        statement = subject + " won awards " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.ToLower().Contains("advisor"))
			{
		        statement = subject + " was advised by " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if ((header == "Followed by") ||
			    (header == "Followedby") ||
			    (header == "Succeededby") ||
			    (header == "Succeeded by"))
			{
				statement = subject + " was followed by " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Next mission"))
			{
				statement = data + " was followed by " + subject;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Launched"))
			{
				statement = subject + " was launched on " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Orbital period"))
			{
				statement = subject + " has an orbital period of " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Eccentricity"))
			{
				statement = subject + " has an orbital eccentricity of " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Discovery date"))
			{
				string[] str = data.Split(' ');
				if (str.Length == 1)
				{
					statement = subject + " was discovered in " + data;
				}
				else
				{
				    statement = subject + " was discovered on " + data;
				}
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Publication date"))
			{
				string[] str = data.Split(' ');
				if (str.Length == 1)
				{
					statement = subject + " was published in " + data;
				}
				else
				{
				    statement = subject + " was published on " + data;
				}
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Publisher"))
			{
			    statement = subject + " was published by " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Media type"))
			{
			    statement = subject + " has the media type " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Author"))
			{
			    statement = subject + " was written by " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Patron saint"))
			{
			    statement = subject + " has the patron saint " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header.Contains("Discovered by"))
			{
				statement = subject + " was discovered by " + data;
				//Console.WriteLine(header + "      " + statement);
				wiki_data.content[wiki_data.content_entries++] = statement;				
			}
			if (header == "Death")
			{
				string[] wrds = data.Split(' ');
				if ((wrds.Length>1) &&
				    (!data.Contains(" BC")) &&
				    (!data.Contains(" B.C.")) &&
                    (!data.Contains(" AD")) &&
				    (!data.Contains(" A.D.")))
				{
					if (ContainsMonth(data))
				        statement = subject + " died on " + data;
					else
						statement = "";
				}
				else
				{
					statement = subject + " died in " + data;
				}
				if (statement != "")
				{
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if (header == "Birth")
			{
				string[] wrds = data.Split(' ');
				if ((wrds.Length>1) &&
				    (!data.Contains(" BC")) &&
				    (!data.Contains(" B.C.")) &&
                    (!data.Contains(" AD")) &&
				    (!data.Contains(" A.D.")))
				{
					if (ContainsMonth(data))
					{
				        statement = subject + " born on " + data;
					}
					else statement = "";
				}
				else
				{
					statement = subject + " born in " + data;
				}
				if (statement != "")
				{
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if (header == "Also known as")
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
				    statement = subject + " is also known as " + names[i].Trim();
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if (header == "School/tradition")
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
				    statement = subject + " follows the school of thought " + names[i].Trim();
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if (header == "Main interests")
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
				    statement = subject + " is interested in " + names[i].Trim();
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if (header == "Notable ideas")
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
				    statement = subject + " has ideas in " + names[i].Trim();
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if (header == "Influenced")
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{	
					if (names[i].Trim() != subject)
					{
				        //statement = names[i].Trim() + " was influenced by " + subject;						
						statement = subject + " influenced " + names[i].Trim();
				        //if (names[i].Contains("Adam")) Console.WriteLine(header + "      " + statement);
				        wiki_data.content[wiki_data.content_entries++] = statement;				
					}
				}
			}
			if (header == "Government")
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
				    statement = subject + " has the government type " + names[i].Trim();
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if ((header == "Politicalparty") ||
			    (header == "Politicalparty"))
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
				    statement = subject + " was a member of the " + names[i].Trim() + " party";
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}			
			if (header == "Alamater")
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
				    statement = subject + " attended " + names[i].Trim();
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}			
			if ((header.ToLower() == "vicepresident") ||
			    (header.ToLower() == "vice president"))
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
					names[i] = names[i].Trim();
					if ((names[i].Length > 3) && (names[i].ToLower() != "none"))
					{
				        statement = subject + " had the vice president " + names[i];
				        //Console.WriteLine(header + "      " + statement);
				        wiki_data.content[wiki_data.content_entries++] = statement;				
					}
				}
			}			
			if ((header == "Official languages") ||
			    (header == "Official language(s)") ||
			    (header == "Official language"))
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
				    statement = subject + " has the official language " + names[i].Trim();
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if (header == "Demonym")
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
				    statement = names[i].Trim() + "s live in " + subject;
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;	
					
				    statement = names[i].Trim() + " is a type of nationality";
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if ((header == "Occupation(s)") ||
			    (header == "Occupation"))
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
				    statement = subject + " has the occupation " + names[i].Trim();
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if ((header == "Genre(s)") ||
			    (header == "Genre"))
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
				    statement = subject + " has the genre " + names[i].Trim();
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if ((header == "Instrument(s)") ||
			    (header == "Instrument"))
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{				
					names[i] = names[i].Trim();
					if (names[i].ToLower() != "vocals")
					{
				        statement = subject + " plays the instrument " + names[i];
					}
					else
					{
						statement = subject + " is a singer";
					}
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
			if ((header.Contains("Associated act")) ||
			    (header.Contains("Related")))
			{
				string[] names = data.Split(',');
				for (int i = 0; i < names.Length; i++)
				{
				    statement = subject + " is associated with " + names[i].Trim();
				    //Console.WriteLine(header + "      " + statement);
				    wiki_data.content[wiki_data.content_entries++] = statement;				
				}
			}
		}
		
		private void ExtractHeaderData(
		    string html, 
		    WikipediaData wiki_data, 
		    bool probably_person)
		{		
			int pos4, pos5, pos6, pos3,pos2, pos = 0;
			while (pos != -1)
			{
				pos = html.IndexOf("<th", pos+1);
				if (pos > -1)
				{
					pos2 = html.IndexOf(">", pos);
					if (pos2 > -1)
					{
						//string test_str = html.Substring(pos, pos2-pos+1);
						//Console.WriteLine("test " + test_str);
						pos3 = html.IndexOf("</th>", pos2+1);
						if (pos3 != -1)
						{
							string header_str = html.Substring(pos2+1, pos3-pos2-1);
							//Console.WriteLine(header_str);
							//if (header_str.Contains("Birth")) Console.WriteLine("Birth: " + header_str);
							header_str = RemoveFormatting(header_str);
							if (header_str == "E&deg") header_str = "East";
							if (header_str == "N&deg") header_str = "Nort";
							header_str = ReplaceWord(header_str, "&amp;", "");
							header_str = ReplaceWord(header_str, "&nbsp;", "");
							header_str = ReplaceWord(header_str, "&ndash;", "-");
							header_str = ReplaceWord(header_str, "&#39;","'");
							header_str = ReplaceWord(header_str, "&deg;F","degrees farenheit");
							header_str = header_str.Trim();
							if (header_str.StartsWith("-")) header_str = header_str.Substring(1).Trim();
							
							if ((!header_str.Contains("&")) && (header_str != ""))
							{			
								if (!header_str.ToLower().Contains("scientific class"))
								{
									string header_data = "";
									int tries = 3;
									while ((header_data == "") && (tries >= 0))
									{
										tries--;
										pos4 = html.IndexOf("<td", pos3);
										if (pos4 > -1)
										{
											pos5 = html.IndexOf(">", pos4);
											if (pos5 > -1)
											{
											    pos6 = html.IndexOf("</td>", pos5);
											    if (pos6 > -1)
											    {
												    header_data = RemoveFormatting(html.Substring(pos5+1, pos6-pos5-1));
													header_data = ReplaceWord(header_data, "&amp;", "");
													header_data = ReplaceWord(header_data, "&quot;", "");
													header_data = ReplaceWord(header_data, "&nbsp;", "");
													header_data = ReplaceWord(header_data, "&ndash;", "-");
													header_data = ReplaceWord(header_data, "&#39;","'");
													header_data = ReplaceWord(header_data, "&sup2;","^2");
													header_data = ReplaceWord(header_data, "&deg;F","degrees farenheit");
													header_data = header_data.Trim();											
													if (header_data == "#") header_data = "";
													if ((!header_data.Contains("&")) && (header_data != ""))
													{
														//Console.WriteLine("Title: " + wiki_data.Title);
														//Console.WriteLine("  Header: " + header_str);
													    //Console.WriteLine("    Data: " + header_data);
														AddHeaderQuestion(wiki_data, wiki_data.Title, header_str, header_data, probably_person);
													}
												}
											}
											pos3 = pos4+1;
										}
									}
								}
							}							
							
							//int pos3 = html.IndexOf("<th>", pos);
							//pos = pos3;
						}
						//pos = pos2;
					}
				}
			}
		}
		
        private void ProcessWikipediaEntry(
		    string html,
		    string mindpixel_filename)
        {
            int pos, pos2, i;
            string firstParagraph;
            string[] sentence;
            string[] words;
            string category;
			bool man_woman = false;
			string html_original = html;

			WikipediaData wiki_data = new WikipediaData();
            wiki_data.Title = "";
            wiki_data.content_entries = 0;
            wiki_data.categories.Clear();
			
            pos = html.IndexOf("firstHeading");
            if (pos > 0)
            {
                html = html.Substring(pos + 14);
                pos2 = html.IndexOf("<");
                if (pos2 > 0)
                {
                    //get the title of the article
                    wiki_data.Title = html.Substring(0, pos2);					
 		            wiki_data.Title = ReplaceWord(wiki_data.Title, "&ndash;", "-");
			        wiki_data.Title = ReplaceWord(wiki_data.Title, "&#39;","'");								
					
					//Console.WriteLine("Title: " + wiki_data.Title);
					
                    html = html.Substring(pos2);
                    pos = html.IndexOf("start content");
                    if (pos > 0)
                    {
                        //get the content
                        html = html.Substring(pos);
						
						
						int pos3 = html.IndexOf("<table");
						
                        pos = html.IndexOf("<p>");
						
						if ((pos3 != -1) && (pos3 < pos))
						{
							// skip table
							pos = html.IndexOf("</table>");
							html = html.Substring(pos);
							pos = html.IndexOf("<p>");
						}
						
                        if (pos > 0)
                        {
                            html = html.Substring(pos+3);
							pos -= (pos+3);
							//Console.WriteLine(html);
                            //pos2 = html.IndexOf("</p>");
							if (pos+20 < html.Length) 
								pos2 = html.IndexOf(". ", pos+20);
							else
								pos2 = -1;
							if ((pos2 == -1) && (pos+10 < html.Length)) pos2 = html.IndexOf(". ", pos+10);
							if ((pos2 == -1) && (pos+20 < html.Length)) pos2 = html.IndexOf(".", pos+20);
							if ((pos2 == -1) && (pos+10 < html.Length)) pos2 = html.IndexOf(".", pos+10);
							if (pos2 == -1) pos2 = html.IndexOf(".");
                            if (pos2-3 > 0)
                            {
                                firstParagraph = html.Substring(0, pos2);                                
								firstParagraph = RemoveFormatting(firstParagraph);
																								
                                sentence = firstParagraph.Split('.');
                                if (sentence.Length > 0)
                                {
									int max = sentence.Length;
									if (max > 1000) max = 1000;
									for (int s = 0; s < max; s++)
									{
                                        wiki_data.content[wiki_data.content_entries++] = sentence[s];
									}
									
                                }
                            }
                        }
                    }

                    // look for a categories                    
                    pos = 1;
                    while (pos > 0)
                    {
                        pos = html.IndexOf(Convert.ToString((char)34) + "Category:");
                        if (pos > 0)
                        {
                            pos += 10;
                            html = html.Substring(pos);
                            pos2 = html.IndexOf(">");
                            if (pos2 > 0)
                            {
                                category = html.Substring(0, pos2-1);
                                if (!category.Contains(" stub"))
                                {

                                    words = category.Split(' ');
                                    for (i = 0; i < words.Length; i++)
                                    {
                                        if (words[i].Length > 4)
                                        {
                                            if (words[i].Substring(words[i].Length - 3, 3) == "ies")
                                                words[i] = words[i].Substring(0, words[i].Length - 3) + "y";
                                        }
                                        if (words[i].Substring(words[i].Length - 1, 1) == "s")
                                            words[i] = words[i].Substring(0, words[i].Length - 1);
                                    }
                                    category = "";
                                    for (i = 0; i < words.Length; i++)
                                    {
                                        category += words[i];
                                        if (i < words.Length) category += " ";
                                    }

                                    wiki_data.categories.Add(category);
                                    html = html.Substring(pos2);
                                }
                            }
                        }
                    }
                }
            }
									
			for (i = 0; i < wiki_data.content_entries; i++)
			{				
			    AddMindpixel(mindpixel_filename, wiki_data, i, ref man_woman);
				wiki_data.categories.Clear();
			}
			
			wiki_data.content_entries = 0;
			ExtractHeaderData(html_original, wiki_data, man_woman);
			
			for (i = 0; i < wiki_data.content_entries; i++)
			{				
			    AddMindpixel(mindpixel_filename, wiki_data, i, ref man_woman);
				wiki_data.categories.Clear();
			}
        }
				
				
    }
}
