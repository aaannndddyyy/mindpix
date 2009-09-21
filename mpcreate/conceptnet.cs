/*
    Functions for grabbing data from ConceptNet RDF
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
	
	
	public class conceptnet
	{
		public int total_pixels_extracted;
		
		public void ProcessConceptNet(string rdf, string mindpixel_filename)
		{
			StreamWriter oWrite = File.CreateText(mindpixel_filename);
			oWrite.WriteLine("Mindpixels extracted from ConceptNet");
			oWrite.WriteLine("Another Mind Hack");
			oWrite.Close();
			
			ReadRDF(rdf, mindpixel_filename);
			
			Console.WriteLine(total_pixels_extracted.ToString() + " mindpixels extracted");			
		}
		
		private void ReadRDF(string rdf, string mindpixel_filename)
		{
			if (File.Exists(rdf))
			{
				Random rnd = new Random();
		        StreamReader oRead = File.OpenText(rdf);
				string left = "conceptnet:LeftText ";
				string right = "conceptnet:RightText ";
				string reltype = "conceptnet:RelationType <http://conceptnet.media.mit.edu/reltype/";
				string scorestr = "conceptnet:Score ";
				string s = "";
				s += '"';
				while (!oRead.EndOfStream)
				{
				    string rdf_line = oRead.ReadLine();
					if (rdf_line == null) break;
					int pos1 = rdf_line.IndexOf(left);
					if (pos1 > -1)
					{
						pos1 += left.Length+1;
						int pos2 = rdf_line.IndexOf(s, pos1);
						if (pos2 > -1)
						{
							string concept_left = rdf_line.Substring(pos1, pos2-pos1);
							
							pos1 = rdf_line.IndexOf(right);
							if (pos1 > -1)
							{
								pos1 += right.Length+1;
								pos2 = rdf_line.IndexOf(s, pos1);
								if (pos2 > -1)
								{
									string concept_right = rdf_line.Substring(pos1, pos2-pos1);
									
									pos1 = rdf_line.IndexOf(reltype);
									if (pos1 > -1)
									{
										pos1 += reltype.Length;
										pos2 = rdf_line.IndexOf(">", pos1);
										if (pos2 > -1)
										{
											string concept_relation = rdf_line.Substring(pos1, pos2-pos1);
											
											pos1 = rdf_line.IndexOf(scorestr);
											if (pos1 > -1)
											{
												pos1 += scorestr.Length;
												pos2 = rdf_line.IndexOf(";", pos1);
												if (pos2 > -1)
												{
													int concept_score = Convert.ToInt32(rdf_line.Substring(pos1, pos2-pos1));
													float coherence = 0.51f + (concept_score / 10.0f);
													if (coherence > 1) coherence = 1;
													coherence = (int)(coherence*100) / 100.0f;
											
											        SaveMindPixel(concept_left, concept_right, concept_relation, mindpixel_filename, coherence, rnd);
												}
											}
										}
									}
									
								}
							}
						}
					}
					
				}
				oRead.Close();
			}
		}
		
		private void SaveMindPixel(
		    string left, 
		    string right, 
		    string relation, 
		    string mindpixel_filename,
		    float coherence,
		    Random rnd)
		{
			string question = "";
			
			if (relation == "AtLocation")
			{
				question = "Is " + left + " located in " + right + "?";
			}
			if (relation == "IsA")
			{
				question = "Is " + left + " a type of " + right + "?";
			}
			if (relation == "HasProperty")
			{
				question = "Does " + left + " have the property " + right + "?";
			}
			if (relation == "HasPrerequisite")
			{
				question = "Is " + left + " a prerequisite of " + right + "?";
			}			
			if (relation == "UsedFor")
			{
				question = "Is " + left + " used for " + right + "?";
			}			
			if (relation == "Desires")
			{
				question = "Does " + left + " desire " + right + "?";
			}			
			if (relation == "PartOf")
			{
				question = "Is " + left + " a part of " + right + "?";
			}			
			if (relation == "Causes")
			{
				question = "Does " + left + " cause " + right + "?";
			}			
			if (relation == "CausesDesire")
			{
				question = "Does " + left + " cause the desire " + right + "?";
			}			
			if (relation == "HasFirstSubevent")
			{
				question = "Does " + left + " have the first sub event " + right + "?";
			}			
			if (relation == "HasSubevent")
			{
				question = "Does " + left + " have the sub event " + right + "?";
			}			
			if (relation == "CapableOf")
			{
				question = "Is " + left + " capable of " + right + "?";
			}			
			if (relation == "MotivatedByGoal")
			{
				question = "Is " + left + " motivated by the goal " + right + "?";
			}			
			if (question != "")
			{
				SaveQuestion(mindpixel_filename, question, coherence, rnd);
			}
			
		}
		
        /// <summary>
        /// save the given question to the mindpixel file
        /// </summary>
        /// <param name="question"></param>
        private void SaveQuestion(
		    string filename, 
		    string question,
		    float coherence,
		    Random rnd)
        {
            StreamWriter oWrite = null;
            bool allowWrite = true;
            string mindpixel_str;
            string coherence_str;
            int coherence_int;			
			
			if (!question.Contains("&"))
			{
			    if (rnd.Next(1000) < 2) Console.WriteLine(question);
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
					if (coherence < 1) coherence_str = "0" + coherence_str;
					if (coherence < 0.1f) coherence_str = "0" + coherence_str;
	                coherence_str = coherence_str.Substring(0, 1) + "." + coherence_str.Substring(1, 2);
	                mindpixel_str = Convert.ToString((char)9) + coherence_str + Convert.ToString((char)9) + question;
	                oWrite.WriteLine(mindpixel_str);
	                oWrite.Close();
	            }       
			}
        }
		
	}
}
