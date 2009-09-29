/*
    mpcreate
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
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using sluggish.utilities;
using System.Text;
using MySql.Data;	
using MySql.Data.MySqlClient;
using ca.guitard.jeff.utility;

namespace mpcreate
{
	class MainClass
	{
		public static void Main(string[] args)
		{	
			/*
			Console.WriteLine("105 = " + phoneme.ConvertNumber(105));
			Console.WriteLine(phoneme.ConvertText("Alan's  psychedelic breakfast"));
			byte[] formants = phoneme.FormantsNormalisedSimple("This is a test", 16);
			Console.WriteLine(phoneme.StringFromFormants(formants));
			for (int i = 0; i < formants.Length; i++)
				Console.Write(formants[i].ToString() + " ");
			Console.WriteLine("");
			
			int diff = phoneme.Difference("Cat food", "Dog loop", 16, 20);
			Console.WriteLine("diff = " + diff.ToString());
			diff = phoneme.Difference("Cake", "Bake", 16, 20);
			Console.WriteLine("diff = " + diff.ToString());
			Console.WriteLine(phoneme.ToNgramStandardised("This is a test",3));
			Console.WriteLine(Soundex.ToSoundexStandardised("This is a test"));
			*/
			
			Console.WriteLine("mpcreate: A utility for creating mindpixels");
			Console.WriteLine("Version 0.3");
			
            // default parameters for the database
            string server_name = "localhost";
            string database_name = "mindpixel";
            string user_name = "testuser";
            string password = "password";
            string mp_table_name = "mindpixels";
            string users_table_name = "users";
			string threegrams_table_name = "threegrams";
			
            string[] mp_fields = {
				"Id", "BINARY(16) NOT NULL", // that's 16 bytes, not 16 bits!
                "Hash", "INT",
                "Question", "TEXT",
                "YesVotes", "INT",
                "NoVotes", "INT",
                "Coherence", "FLOAT",
				"Ngram2", "FLOAT",
				"Ngram3", "FLOAT",
				"Ngram4", "FLOAT",
				"Ngram5", "FLOAT",
				"Ngram6", "FLOAT",
				"Soundex", "FLOAT",
				"NYSIIS", "FLOAT",
				"MetaphonePrimary", "FLOAT",
				"MetaphoneSecondary", "FLOAT",
				"Emotion", "FLOAT"
            };

			string[] users_fields = {
                "TimeStamp", "DATETIME",
				"Username", "VARCHAR(100)",
				"Hash", "INT",
                "Question", "TEXT",
                "Answer", "TINYINT"
            };

            // the character used to indicate that what follows is a parameter name
            const string switch_character = "-";
			
            // extract command line parameters
            ArrayList parameters = commandline.ParseCommandLineParameters(args, switch_character, GetValidParameters());

            // is help required ?
            string show_help = commandline.GetParameterValue("help", parameters);
            if (show_help == "true")
            {
                ShowHelp();
            }
            else
            {
				string conceptnet_rdf = commandline.GetParameterValue("cn", parameters);
				if (conceptnet_rdf != "")
				{
			        conceptnet grab = new conceptnet();
			        grab.ProcessConceptNet(conceptnet_rdf, "conceptnet.txt");					
				}
				else
				{				
					string wikipedia_directory = commandline.GetParameterValue("wp", parameters);
					if (wikipedia_directory != "")
					{
				        WikipediaFactGrabber grab = new WikipediaFactGrabber();
				        grab.ProcessWikipedia(wikipedia_directory, "wikipedia.txt");					
					}
					else
					{				
						string freebase_directory = commandline.GetParameterValue("fb", parameters);
						if (freebase_directory != "")
						{
							freebase fb = new freebase();
							fb.ProccessFreebase(freebase_directory, "freebase.txt");
						}
						else
						{
							CreateEmotionalWords();
							
							string server_name_str = commandline.GetParameterValue("server", parameters);
							if (server_name_str != "") server_name = server_name_str;
							
							string user_name_str = commandline.GetParameterValue("username", parameters);
							if (user_name_str != "") user_name = user_name_str;
			
							string password_str = commandline.GetParameterValue("password", parameters);
							if (password_str != "") password = password_str;						
							
							string database_str = commandline.GetParameterValue("db", parameters);
							if (database_str != "") database_name = database_str;
														
			                string load_filename = commandline.GetParameterValue("load", parameters);
							if (load_filename != "")
							{									
							    loadGAC(
					                load_filename, 
			                        "Mind Hack",
					                mp_fields,
					                users_fields,
					                server_name,
					                database_name, 
					                user_name, 
					                password, 
					                mp_table_name,
					                users_table_name);
							}
							else
							{
				                string random_pixels_filename = commandline.GetParameterValue("random", parameters);
								if (random_pixels_filename != "")
								{					
									int no_of_random_pixels = 1;
								    SaveRandomMindpixels(random_pixels_filename, server_name, database_name, user_name, password, mp_table_name, no_of_random_pixels);
								}
								else
								{				
					                string user_pixels_filename = commandline.GetParameterValue("userpixels", parameters);
									if (user_pixels_filename != "")
									{
									    SaveUserPixels(user_pixels_filename, server_name, database_name, user_name, password, users_table_name);
									}
									else
									{				
						                string save_filename = commandline.GetParameterValue("save", parameters);
										if (save_filename != "")
										{
											SaveMindpixels(save_filename, server_name, database_name, user_name, password, mp_table_name);
										}
										else
										{
							                string question = commandline.GetParameterValue("q", parameters);
											if (question != "")
											{
							                    string answer = commandline.GetParameterValue("a", parameters);
												if (answer != "")
												{
													bool answer_value = false;
													answer = answer.ToLower();
													if ((answer == "yes") || 
													    (answer == "y") ||
													    (answer == "true") ||
													    (answer == "t"))
														answer_value = true;
																			
													int question_hash = GetHashCode(question);
																														
										            // insert the field names into a list so that we can easily search it
										            List<string> mp_fields_to_be_inserted = new List<string>();
										            List<string> mp_field_type = new List<string>();
										            for (int i = 0; i < mp_fields.Length; i += 2)
										            {
										                mp_fields_to_be_inserted.Add(mp_fields[i]);
										                mp_field_type.Add(mp_fields[i + 1]);
										            }
							
										            List<string> users_fields_to_be_inserted = new List<string>();
										            List<string> users_field_type = new List<string>();
										            for (int i = 0; i < users_fields.Length; i += 2)
										            {
										                users_fields_to_be_inserted.Add(users_fields[i]);
										                users_field_type.Add(users_fields[i + 1]);
										            }
													
										            // create tables if necessary
										            CreateTable(
										                server_name,
										                database_name,
										                user_name,
										                password,
										                mp_table_name,
										                mp_fields_to_be_inserted,
										                mp_field_type, 0,1,5);
																			
										            CreateTable(
										                server_name,
										                database_name,
										                user_name,
										                password,
										                users_table_name,
										                users_fields_to_be_inserted,
										                users_field_type,-1,0,1);
													
									                InsertMindpixelIntoMySql(
													    question_hash,
									                    question,
									                    answer_value,
									                    server_name,
									                    database_name,
									                    user_name,
									                    password,
									                    mp_table_name,
									                    mp_fields_to_be_inserted,
									                    mp_field_type);
																			
									                InsertMindpixelUserDataIntoMySql(
													    question_hash,
									                    question,
									                    answer_value,
									                    server_name,
									                    database_name,
									                    user_name,
									                    password,
									                    users_table_name,
									                    users_fields_to_be_inserted,
									                    users_field_type);
													
													Console.WriteLine("Mindpixel added");										
												}
												else
												{
							  					    Console.WriteLine("Please specify a yes/no answer using the -a option");
												}
											}
											else
											{
												//Console.WriteLine("Please specify a question using the -q option");
											}
										}
									}
								}
							}
							
							
							string map_filename = commandline.GetParameterValue("map", parameters);
							if (map_filename != "")
							{
								float[] coherence = null;
								int[] hash1 = null;
								int[] hash2 = null;
								float[] index1 = null;
								float[] index2 = null;
								Console.WriteLine("Saving map...");
								ShowPlot(
					                server_name,
					                database_name, 
					                user_name, 
					                password,
								    1000,
								    map_filename,
								    false,
								    ref coherence,
								    ref hash1,
								    ref hash2,
								    ref index1,
								    ref index2);
								Console.WriteLine("Saved " + map_filename);
							}
							
							string map_mono_filename = commandline.GetParameterValue("mapmono", parameters);
							if (map_mono_filename != "")
							{
								float[] coherence = null;
								int[] hash1 = null;
								int[] hash2 = null;
								float[] index1 = null;
								float[] index2 = null;
								Console.WriteLine("Saving map...");
								ShowPlot(
					                server_name,
					                database_name, 
					                user_name, 
					                password,
								    1000,
								    map_mono_filename,
								    false,
								    ref coherence,
								    ref hash1,
								    ref hash2,
								    ref index1,
								    ref index2);
								Console.WriteLine("Saved " + map_filename);
							}
							
							string lookup_tables_filename = commandline.GetParameterValue("lookup", parameters);
							if (lookup_tables_filename != "")
							{
								float[] coherence = null;
								int[] hash1 = null;
								int[] hash2 = null;
								float[] index1 = null;
								float[] index2 = null;
								Console.Write("Generating map...");
								ShowPlot(
					                server_name,
					                database_name, 
					                user_name, 
					                password,
								    1000,
								    "",
								    true,
								    ref coherence,
								    ref hash1,
								    ref hash2,
								    ref index1,
								    ref index2);
								Console.WriteLine("Done");
								
								SaveLookupTables(
								    lookup_tables_filename,
								    index1, index2,
								    coherence);
								Console.WriteLine("Saved lookup tables to " + lookup_tables_filename);
							}
							
							
						}
					}
				}			
			}
		}
		
		#region "saving lookup tables"
		
		private static void SaveLookupTables(
            string filename,
			float[] index1, 
		    float[] index2,
			float[] coherence)
		{			
			StreamWriter oWrite = null;
			bool file_open = false;
			try
			{
			    oWrite = File.CreateText(filename);
				file_open = true;
			}
			catch
			{
			}
			if (file_open)
			{
				oWrite.WriteLine(index1.Length.ToString());
				for (int i = 0; i < index1.Length; i++)
				{
					oWrite.WriteLine(index1[i].ToString());
					oWrite.WriteLine(index2[i].ToString());
				}
				oWrite.Close();
			}			
		}		
		
		#endregion
		
		#region "generate image"
		
		//static int img_width = 1000;
		static int plot_ctr;
		
		#endregion
		
		/// <summary>
		/// Generates a hash code indicative of the text string
		/// </summary>
		/// <param name="s">string</param>
		/// <returns>hash code</returns>
		private static int GetHashCode(string s)
		{
			s = s.Trim();
			s = s.ToLower();
			
			char[] ch = s.ToCharArray();
			
			int hash = 0;
            for (int i = 0; i < ch.Length; i++) {
				if (((ch[i] >= 'a') &&
				     (ch[i] <= 'z')) ||
				    ((ch[i] >= '0') &&
				     (ch[i] <= '9')))
				{
                    hash = 31*hash + ch[i];
				}
            }

			return(hash);
		}
				
		#region "mysql database"
		
        private static void CreateTable(
            string server_name,
            string database_name,
            string user_name,
            string password,
            string table_name,
            List<string> fields_to_be_inserted,
            List<string> field_type,
		    int primary_key_field,
		    int index_field1,
		    int index_field2)
        {
            MySqlConnection connection = new MySqlConnection();

            if ((server_name == "") ||
                (server_name == null))
                server_name = "localhost";

            connection.ConnectionString =
                "server=" + server_name + ";"
                + "database=" + database_name + ";"
                + "uid=" + user_name + ";"
                + "password=" + password + ";";
			
			//Console.WriteLine("connection string: " + connection.ConnectionString);

            bool connected = false;
            string exception_str = "";
            try
            {
                connection.Open();
                connected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection error: " + ex.Message);
            }

            if (connected)
            {
                string Query = "CREATE TABLE IF NOT EXISTS " + table_name + "(";

                for (int i = 0; i < fields_to_be_inserted.Count; i++)
                {
                    Query += fields_to_be_inserted[i] + " " + field_type[i] + ",";					
                }
			    if (primary_key_field> -1) Query += "PRIMARY KEY (" + fields_to_be_inserted[primary_key_field] + ")";
				if (index_field1 > -1)
				{
					if (primary_key_field> -1) Query += ",";
					Query += "INDEX (" + fields_to_be_inserted[index_field1] + ")";
					if (index_field2 > -1) Query += ",INDEX (" + fields_to_be_inserted[index_field2] + ")";
				}
				if (table_name == "mindpixels")
				{
					for (int g = 2; g <= 6; g++)
					    Query += ",INDEX (Ngram" + g.ToString() + ")";
					Query += ",INDEX (Soundex),INDEX (NYSIIS),INDEX (MetaphonePrimary),INDEX (MetaphoneSecondary),INDEX (Emotion)";
				}
				Query += ")";

                MySqlCommand addxml = new MySqlCommand(Query, connection);

                //Console.WriteLine("Running query:");
                //Console.WriteLine(Query);

                try
                {
                    addxml.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
					Console.WriteLine("Can't create table " + ex.Message);
                }

                connection.Close();
            }
            else
            {
                Console.WriteLine("CreateTable: Couldn't connect to database " + database_name);
                Console.WriteLine(exception_str);
            }
        }
		
		private static ArrayList RunMySqlCommand(
		    string commandText, 
		    string connectionString,
		    int no_of_fields)
	    {
			ArrayList rows = new ArrayList();
	        using (MySqlConnection connection = new MySqlConnection(connectionString))
	        {
	            try
	            {
					//Console.WriteLine("commandtext: " + commandText);
	                MySqlCommand command = new MySqlCommand(commandText, connection);
	
	                connection.Open();
	                IAsyncResult result = command.BeginExecuteReader();
	
	                int count = 0;
	                while (!result.IsCompleted)
	                {
	                    count += 1;
	                    //Console.WriteLine("Waiting ({0})", count);
	                    System.Threading.Thread.Sleep(100);
	                }
	
	                MySqlDataReader query_result = command.EndExecuteReader(result);
					while (query_result.Read())
					{
						ArrayList row = new ArrayList();
						for (int i = 0; i < no_of_fields; i++)
						{
							row.Add(query_result.GetValue(i));
						}
						rows.Add(row);
					}
					
					connection.Close();
	            }
	            catch (MySqlException ex)
	            {
	                Console.WriteLine("Error ({0}): {1}", ex.Number, ex.Message);
					connection.Close();
	            }
	            catch (InvalidOperationException ex)
	            {
	                Console.WriteLine("Error: {0}", ex.Message);
					connection.Close();
	            }
	            catch (Exception ex)
	            {
	                Console.WriteLine("Error: {0}", ex.Message);
					connection.Close();
	            }
	        }
			return(rows);
	    }
		
        static int[] emotional_word_rating;
        static string[] emotional_words;
		static int no_of_emotional_words;
		
		private static void addEmotionalWord(string word, int rating)
		{
			emotional_word_rating[no_of_emotional_words] = rating;
			emotional_words[no_of_emotional_words] = word;
			no_of_emotional_words++;
		}
		
		private static void CreateEmotionalWords()
		{	
			emotional_word_rating = new int[300];
			emotional_words = new string[300];
			no_of_emotional_words = 0;
			
            //words associated with positive emotions
            addEmotionalWord("love", 20);
            addEmotionalWord("loved", 20);
            addEmotionalWord("like", 2);
            addEmotionalWord("enjoy", 5);
            addEmotionalWord("pleasurable", 5);
            addEmotionalWord("pleasure", 5);
            addEmotionalWord("easy", 5);
            addEmotionalWord("enjoys", 5);
            addEmotionalWord("amused", 5);
            addEmotionalWord("amusing", 5);
            addEmotionalWord("entertained", 5);
            addEmotionalWord("entertaining", 5);
            addEmotionalWord("enjoyed", 5);
            addEmotionalWord("treasure", 5);
            addEmotionalWord("treasured", 5);
            addEmotionalWord("pleasant", 5);
            addEmotionalWord("joy", 5);
            addEmotionalWord("happy", 5);
            addEmotionalWord("good", 5);
            addEmotionalWord("good at", 5);
            addEmotionalWord("happiness", 5);
            addEmotionalWord("popular", 5);
            addEmotionalWord("esteem", 5);
            addEmotionalWord("worthy", 5);
            addEmotionalWord("worthwhile", 5);
            addEmotionalWord("loving", 5);
            addEmotionalWord("positive", 5);
            addEmotionalWord("protect", 5);
            addEmotionalWord("enjoyable", 10);
            addEmotionalWord("exciting", 10);
            addEmotionalWord("pleasing", 10);
            addEmotionalWord("pleased", 10);
            addEmotionalWord("encouraging", 5);
            addEmotionalWord("encouraged", 5);
            addEmotionalWord("encourage", 5);
            addEmotionalWord("invest", 4);
            addEmotionalWord("investment", 4);
            addEmotionalWord("growth", 4);
            addEmotionalWord("increase", 10);
            addEmotionalWord("increased", 10);
            addEmotionalWord("joyous", 10);
            addEmotionalWord("tidy", 2);
            addEmotionalWord("neat", 2);
            addEmotionalWord("confident", 10);
            addEmotionalWord("confidence", 10);
            addEmotionalWord("peace", 10);
            addEmotionalWord("peacefull", 10);
            addEmotionalWord("calm", 10);
            addEmotionalWord("bright", 10);
            addEmotionalWord("rosy", 10);
            addEmotionalWord("kindness", 10);
            addEmotionalWord("goodness", 10);
            addEmotionalWord("better", 2);
            addEmotionalWord("welcome", 2);
            addEmotionalWord("amusing", 2);
            addEmotionalWord("lucky", 5);
            addEmotionalWord("correct", 5);
            addEmotionalWord("hired", 5);
            addEmotionalWord("replenish", 5);
            addEmotionalWord("respect", 5);
            addEmotionalWord("respects", 5);
            addEmotionalWord("respected", 5);
            addEmotionalWord("working", 5);
            addEmotionalWord("fascinate", 5);
            addEmotionalWord("fascinated", 5);
            addEmotionalWord("fascinating", 5);
            addEmotionalWord("poetic", 5);
            addEmotionalWord("interested", 5);
            addEmotionalWord("interesting", 5);
            addEmotionalWord("divine", 10);
            addEmotionalWord("divinely", 10);
            addEmotionalWord("fantastic", 10);
            addEmotionalWord("fabulous", 10);
            addEmotionalWord("funny", 10);
            addEmotionalWord("initiative", 2);
            addEmotionalWord("genuis", 5);
            addEmotionalWord("freedom", 5);
            addEmotionalWord("devoted", 10);
            addEmotionalWord("refreshing", 10);
            addEmotionalWord("refreshed", 10);
            addEmotionalWord("enthusiast", 10);
            addEmotionalWord("enthusiastic", 10);
            addEmotionalWord("enthusiastically", 10);
            addEmotionalWord("enthused", 10);
            addEmotionalWord("enthusiasm", 10);
            

            //words associated with negative emotions
            addEmotionalWord("hate", -10);
            addEmotionalWord("hated", -10);
            addEmotionalWord("never", -2);
            addEmotionalWord("hates", -10);
            addEmotionalWord("harm", -10);
            addEmotionalWord("kill", -10);
            addEmotionalWord("kills", -10);
            addEmotionalWord("killed", -10);
            addEmotionalWord("killing", -10);
            addEmotionalWord("death", -10);
            addEmotionalWord("grave", -10);
            addEmotionalWord("victim", -10);
            addEmotionalWord("stole", -5);
            addEmotionalWord("steal", -5);
            addEmotionalWord("stolen", -5);
            addEmotionalWord("surrender", -1);
            addEmotionalWord("killing", -10);
            addEmotionalWord("offensive", -10);
            addEmotionalWord("disslike", -5);
            addEmotionalWord("frustrate", -5);
            addEmotionalWord("frustration", -5);
            addEmotionalWord("exclusion", -5);
            addEmotionalWord("empty", -5);
            addEmotionalWord("sad", -5);
            addEmotionalWord("robbed", -8);
            addEmotionalWord("crooked", -5);
            addEmotionalWord("scam", -5);
            addEmotionalWord("slave", -5);
            addEmotionalWord("evil", -10);
            addEmotionalWord("hysterical", -10);
            addEmotionalWord("hysteria", -10);
            addEmotionalWord("imposed", -5);
            addEmotionalWord("violent", -5);
            addEmotionalWord("violence", -5);
            addEmotionalWord("shot", -10);
            addEmotionalWord("genocide", -10);
            addEmotionalWord("fighting", -5);
            addEmotionalWord("paranoia", -5);
            addEmotionalWord("paranoid", -5);
            addEmotionalWord("propaganda", -5);
            addEmotionalWord("fraud", -5);
            addEmotionalWord("liar", -10);
            addEmotionalWord("lies", -10);
            addEmotionalWord("misslead", -1);
            addEmotionalWord("lied", -10);
            addEmotionalWord("hoax", -1);
            addEmotionalWord("scary", -5);
            addEmotionalWord("scared", -5);
            addEmotionalWord("scare", -5);
            addEmotionalWord("frightening", -10);
            addEmotionalWord("frightened", -10);
            addEmotionalWord("criticism", -5);
            addEmotionalWord("criticise", -5);
            addEmotionalWord("horror", -8);
            addEmotionalWord("disturbing", -8);
            addEmotionalWord("horrible", -8);
            addEmotionalWord("criticised", -5);
            addEmotionalWord("criticising", -5);
            addEmotionalWord("spam", -10);
            addEmotionalWord("damn", -10);
            addEmotionalWord("darn", -10);
            addEmotionalWord("fired", -5);
            addEmotionalWord("sacked", -5);
            addEmotionalWord("redundant", -5);
            addEmotionalWord("bad", -10);
            addEmotionalWord("too bad", -10);
            addEmotionalWord("scream", -10);
            addEmotionalWord("screamed", -10);
            addEmotionalWord("screaming", -10);
            addEmotionalWord("worse", -2);
            addEmotionalWord("stumble", -2);
            addEmotionalWord("stumbling", -2);
            addEmotionalWord("frantic", -2);
            addEmotionalWord("arrested", -10);
            addEmotionalWord("worse than", -2);
            addEmotionalWord("virus", -10);
            addEmotionalWord("bomb", -10);
            addEmotionalWord("boring", -10);
            addEmotionalWord("dumb", -2);
            addEmotionalWord("unnecessary", -2);
            addEmotionalWord("unsophisticated", -2);
            addEmotionalWord("fuck", -10);
            addEmotionalWord("fucked", -10);
            addEmotionalWord("shoot", -10);
            addEmotionalWord("incorrect", -10);
            addEmotionalWord("shot", -10);
            addEmotionalWord("shooter", -10);
            addEmotionalWord("messy", -2);
            addEmotionalWord("untidy", -2);
            addEmotionalWord("shooting", -10);
            addEmotionalWord("fucker", -10);
            addEmotionalWord("fucking", -10);
            addEmotionalWord("shit", -10);
            addEmotionalWord("bitch", -10);
            addEmotionalWord("dont like", -10);
            addEmotionalWord("bitchy", -10);
            addEmotionalWord("bitching", -10);
            addEmotionalWord("gun", -10);
            addEmotionalWord("guns", -10);
            addEmotionalWord("weapon", -10);
            addEmotionalWord("weapons", -10);
            addEmotionalWord("war", -10);
            addEmotionalWord("rifle", -10);
            addEmotionalWord("lonely", -10);
            addEmotionalWord("loser", -10);
            addEmotionalWord("invade", -10);
            addEmotionalWord("invasion", -10);
            addEmotionalWord("disease", -10);
            addEmotionalWord("sick", -10);
            addEmotionalWord("ill", -10);
            addEmotionalWord("illness", -10);
            addEmotionalWord("sickness", -10);
            addEmotionalWord("jealous", -5);
            addEmotionalWord("fear", -5);
            addEmotionalWord("feared", -5);
            addEmotionalWord("fearfull", -5);
            addEmotionalWord("nowhere", -5);
            addEmotionalWord("unpopular", -5);
            addEmotionalWord("angry", -5);
            addEmotionalWord("deluded", -5);
            addEmotionalWord("delusion", -5);
            addEmotionalWord("deserted", -5);
            addEmotionalWord("worthless", -5);
            addEmotionalWord("difficult", -2);
            addEmotionalWord("hard", -2);
            addEmotionalWord("trouble", -2);
            addEmotionalWord("troublesome", -4);
            addEmotionalWord("bicker", -5);
            addEmotionalWord("argue", -5);
            addEmotionalWord("decline", -5);
            addEmotionalWord("reduce", -5);
            addEmotionalWord("cut", -5);
            addEmotionalWord("unlucky", -5);
            addEmotionalWord("wrong", -5);
            addEmotionalWord("go wrong", -5);
            addEmotionalWord("gone wrong", -5);
            addEmotionalWord("going wrong", -5);
            addEmotionalWord("not working", -5);
		}
		
		private static float GetEmotionRating(string text)
		{
			float rating = 0;
			
			text = text.ToLower();
			for (int i = 0; i < no_of_emotional_words; i++)
			{
				if (text.Contains(emotional_words[i]))
					rating += emotional_word_rating[i];
			}
			
			rating /= 40.0f;
			
			return(rating);
		}

		private static float GetNgramIndex(string idx, int max_length)
		{
			double index = 0;
			idx = idx.ToLower();
			if (idx.Length > max_length)
				idx = idx.Substring(0, max_length);
			while (idx.Length < max_length)
				idx += "0";
			
			char[] ch = idx.ToCharArray();
			
			double mult = 1.0/3600000000.0;
			double max = 0;
            for (int i = 0; i < ch.Length; i++) {
				if (((ch[i] >= 'a') &&
				     (ch[i] <= 'z')) ||
				    ((ch[i] >= '0') &&
				     (ch[i] <= '9')))
				{
					if (ch[i] <= '9')
						index = 2*index + ((ch[i] - (int)'0' + 1)*mult);
					else
                        index = 2*index + ((ch[i] - (int)'a' + 11)*mult);
					
					max = 2*max + (36*mult);
				}
            }
			index /= max;
			//Console.WriteLine("index = " + index.ToString());
			
			return((float)index);
		}
		
        public static void InsertMindpixelIntoMySql(
		    int hash,
            string question,
            bool answer,
            string server_name,
            string database_name,
            string user_name,
            string password,
            string table_name,
            List<string> fields_to_be_inserted,
            List<string> field_type)
        {
            if ((server_name == "") ||
                (server_name == null))
                server_name = "localhost";

			float coherence = 0.0f;
			if (answer == true) coherence = 1.0f;
			
			string index_ngram2 = phoneme.ToNgramStandardised(question, 2, false);
			string index_ngram3 = phoneme.ToNgramStandardised(question, 3, false);
			string index_ngram4 = phoneme.ToNgramStandardised(question, 4, false);
			string index_ngram5 = phoneme.ToNgramStandardised(question, 5, false);
			string index_ngram6 = phoneme.ToNgramStandardised(question, 6, false);
			string index_soundex = Soundex.ToSoundexStandardised(question, false);
			string index_metaphone_primary="", index_metaphone_secondary="";
			Metaphone.ToMetaphoneStandardised(question, false, ref index_metaphone_primary, ref index_metaphone_secondary);
			string index_nysiis = NYSIIS.ToNYSIISStandardised(question, false);

			float coordinate_ngram2 = GetNgramIndex(index_ngram2, 80);
			float coordinate_ngram3 = GetNgramIndex(index_ngram3, 80);
			float coordinate_ngram4 = GetNgramIndex(index_ngram4, 80);
			float coordinate_ngram5 = GetNgramIndex(index_ngram5, 80);
			float coordinate_ngram6 = GetNgramIndex(index_ngram6, 80);
			float coordinate_soundex = GetNgramIndex(index_soundex, 80);
			float coordinate_nysiis = GetNgramIndex(index_nysiis, 80);
			float coordinate_metaphone_primary = GetNgramIndex(index_metaphone_primary, 80);
			float coordinate_metaphone_secondary = GetNgramIndex(index_metaphone_secondary, 80);
			float coordinate_emotion = GetEmotionRating(question);
			
			plot_ctr++;
			if (plot_ctr > 10000)
			{
				float[] coherence2 = null;
				int[] hash1 = null;
				int[] hash2 = null;
				float[] index1 = null;
				float[] index2 = null;
                ShowPlot(
                    server_name,
                    database_name,
                    user_name,
                    password,
		            1000,
		            "mindpixels.bmp",
				    false,
				    ref coherence2,
				    ref hash1,
				    ref hash2,
				    ref index1,
				    ref index2);
					
				plot_ctr = 0;
			}
						
			string connection_str = 
                "server=" + server_name + ";"
                + "database=" + database_name + ";"
                + "uid=" + user_name + ";"
                + "password=" + password + ";";
			
			string Query = "SELECT * FROM " + table_name + " WHERE Hash = " + hash.ToString() + ";";
			//Console.WriteLine(Query);
			
			int YesVotes = 0;
			int NoVotes = 0;
			Guid best_result = Guid.Empty;
			ArrayList pixels = RunMySqlCommand(Query, connection_str, 5);
			if (pixels.Count > 0)
			{
				char[] template = Soundex.ToSoundexCode(question).ToCharArray();
				int max_score = -1;
				// update existing pixel
                for (int ctr = 0; ctr < pixels.Count; ctr++)
				{
					ArrayList row = (ArrayList)pixels[ctr];
					string result_question = (string)row[2];
  				    char[] s = Soundex.ToSoundexCode(result_question).ToCharArray();
					int score = 0;
					for (int i = 0; i < s.Length; i++)
					{						
						score += Math.Abs((int)s[i] - (int)template[i]);
					}
					if (score > max_score)
					{
						best_result = (Guid)row[0];
						max_score = score;
						YesVotes = Convert.ToInt32(row[3]);
						NoVotes = Convert.ToInt32(row[4]);
					}
					
					//Guid result_Id = (Guid)pixels.GetValue(0);
					//int result_hash = pixels.GetInt32(1);
					//int result_yes = pixels.GetInt32(3);
					//int result_no = pixels.GetInt32(4);
                }
				
				if (best_result != Guid.Empty)
				{
					// update the mindpixel coherence
					if (answer == true)
						YesVotes++;
					else
						NoVotes++;
					coherence = YesVotes / (float)(YesVotes + NoVotes);
					
				    Query = "UPDATE " + table_name + 
						" SET YesVotes='" + YesVotes.ToString() + "'," +
						" NoVotes='" + NoVotes.ToString() + "'," +
						" Coherence='" + coherence.ToString() + "'," +
					    " Ngram2='" + coordinate_ngram2.ToString() + "'," +
					    " Ngram3='" + coordinate_ngram3.ToString() + "'," +
					    " Ngram4='" + coordinate_ngram4.ToString() + "'," +
					    " Ngram5='" + coordinate_ngram5.ToString() + "'," +
					    " Ngram6='" + coordinate_ngram6.ToString() + "'," +
					    " Soundex='" + coordinate_soundex.ToString() + "'," +
					    " NYSIIS='" + coordinate_nysiis.ToString() + "'," +
					    " MetaphonePrimary='" + coordinate_metaphone_primary.ToString() + "'," +
					    " MetaphoneSecondary='" + coordinate_metaphone_secondary.ToString() + "'," +
					    " Emotion='" + coordinate_emotion.ToString() + "'" +
					    " WHERE Id=CAST(?get_id as BINARY(16));";
					
                    MySqlConnection connection = new MySqlConnection();

                    connection.ConnectionString = connection_str;

                    bool connected = false;
                    try
                    {
                        connection.Open();
                        connected = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
						connection.Close();
                    }

                    if (connected)
                    {
		                MySqlCommand update = new MySqlCommand(Query, connection);
						update.Parameters.Add("get_id", MySqlDbType.Binary).Value = best_result.ToByteArray();
		
		                //Console.WriteLine("Running update query:");
		                //Console.WriteLine(Query);
		
		                try
		                {
		                    update.ExecuteNonQuery();
		                }
		                catch (Exception excp)
		                {
		                    Exception myExcp = new Exception("Could not update mindpixel. Error: " + excp.Message, excp);
		                    throw (myExcp);
		                }
		
		                connection.Close();					
				    }	
				}
			}
			
			if (best_result == Guid.Empty)
			{
	            List<string> field_name = new List<string>();
	            List<string> field_value = new List<string>();
				
				if (answer == true)
				{
					YesVotes = 1;
					coherence = 1;
				}
				else
				{
					NoVotes = 1;
					coherence = 0;
				}
	
				field_name.Add("Id");
				field_value.Add(System.Guid.NewGuid().ToString());
				field_name.Add("Hash");
				field_value.Add(hash.ToString());
				field_name.Add("Question");
				field_value.Add(question);
				field_name.Add("YesVotes");
				field_value.Add(YesVotes.ToString());				
				field_name.Add("NoVotes");
				field_value.Add(NoVotes.ToString());				
				field_name.Add("Coherence");
				field_value.Add(coherence.ToString());
			    field_name.Add("Ngram2");
			    field_value.Add(coordinate_ngram2.ToString());
			    field_name.Add("Ngram3");
			    field_value.Add(coordinate_ngram3.ToString());
			    field_name.Add("Ngram4");
			    field_value.Add(coordinate_ngram4.ToString());
			    field_name.Add("Ngram5");
			    field_value.Add(coordinate_ngram5.ToString());
			    field_name.Add("Ngram6");
			    field_value.Add(coordinate_ngram6.ToString());
			    field_name.Add("Soundex");
			    field_value.Add(coordinate_soundex.ToString());
			    field_name.Add("NYSIIS");
			    field_value.Add(coordinate_nysiis.ToString());
			    field_name.Add("MetaphonePrimary");
			    field_value.Add(coordinate_metaphone_primary.ToString());
			    field_name.Add("MetaphoneSecondary");
			    field_value.Add(coordinate_metaphone_secondary.ToString());
			    field_name.Add("Emotion");
			    field_value.Add(coordinate_emotion.ToString());
				
				// add new pixel
	            MySqlConnection connection = new MySqlConnection();
				
				connection.ConnectionString = connection_str;
		
	            bool connected = false;
	            string exception_str = "";
	            try
	            {
	                connection.Open();
	                connected = true;
	            }
	            catch (Exception ex)
	            {
	                exception_str = ex.Message;
					connection.Close();
	            }
	
	            if (connected)
	            {
	                Query = "INSERT INTO " + table_name + "(";
	
	                for (int i = 0; i < field_name.Count; i++)
	                {
	                    if ((fields_to_be_inserted.Count == 0) ||
	                        (fields_to_be_inserted.Contains(field_name[i])))
	                    {
                            Query += field_name[i];
	                        if (i < field_name.Count - 1) Query += ",";
	                    }
	                }
	                Query += ") values(";
	                for (int i = 0; i < field_value.Count; i++)
	                {
	                    int idx = fields_to_be_inserted.IndexOf(field_name[i]);
	
	                    if ((fields_to_be_inserted.Count == 0) ||
	                        (fields_to_be_inserted.Contains(field_name[i])))
	                    {
	                        if (fields_to_be_inserted.Count > 0)
	                        {
	                            if (idx > -1)
	                            {
									if (field_type[idx].Contains("BINARY"))
									{
										field_value[i] = "?Id";
									}
									
	                                if (field_type[idx] == "DATETIME")
	                                {
	                                    DateTime d = DateTime.Parse(field_value[i]);
	                                    string d_str = d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString() + " " + d.Hour.ToString() + ":" + d.Minute.ToString() + ":" + d.Second.ToString();
	                                    field_value[i] = d_str;
	                                }
	                            }
	                        }
	
							if (!field_type[idx].Contains("BINARY"))
	                            Query += "'" + field_value[i] + "'";
							else
								Query += field_value[i];
	                        if (i < field_value.Count - 1) Query += ",";
	                    }
	                }
	                Query += ")";
	
	                MySqlCommand addxml = new MySqlCommand(Query, connection);
	
	                //Console.WriteLine("Running insert query:");
	                //Console.WriteLine(Query);
	
	                try
	                {
						addxml.Parameters.Add("Id", MySqlDbType.Binary).Value = System.Guid.NewGuid().ToByteArray();
	                    addxml.ExecuteNonQuery();
	                }
	                catch (Exception excp)
	                {
	                    Exception myExcp = new Exception("Could not add new mindpixel. Error: " + excp.Message, excp);
	                    throw (myExcp);
	                }
	
	                connection.Close();
	            }
	            else
	            {
	                Console.WriteLine("InsertMindpixel: Couldn't connect to database " + database_name);
	                Console.WriteLine(exception_str);
	            }
				
			}
        }
		
		private static int GCD(int a, int b)
		{
		     while (a != 0 && b != 0)
		     {
		         if (a > b)
		            a %= b;
		         else
		            b %= a;
		     }
		
		     if (a == 0)
		         return b;
		     else
		         return a;
		}		
		
        private static void InsertMindpixelWithCoherence(
		    int hash,
            string question,
            float coherence,
            string server_name,
            string database_name,
            string user_name,
            string password,
            string table_name,
            List<string> fields_to_be_inserted,
            List<string> field_type)
        {
            if ((server_name == "") ||
                (server_name == null))
                server_name = "localhost";
			
			string index_ngram2 = phoneme.ToNgramStandardised(question, 2, false);
			string index_ngram3 = phoneme.ToNgramStandardised(question, 3, false);
			string index_ngram4 = phoneme.ToNgramStandardised(question, 4, false);
			string index_ngram5 = phoneme.ToNgramStandardised(question, 5, false);
			string index_ngram6 = phoneme.ToNgramStandardised(question, 6, false);
			string index_soundex = Soundex.ToSoundexStandardised(question, false);
			string index_metaphone_primary="", index_metaphone_secondary="";
			Metaphone.ToMetaphoneStandardised(question, false, ref index_metaphone_primary, ref index_metaphone_secondary);
			string index_nysiis = NYSIIS.ToNYSIISStandardised(question, false);

			float coordinate_ngram2 = GetNgramIndex(index_ngram2, 80);
			float coordinate_ngram3 = GetNgramIndex(index_ngram3, 80);
			float coordinate_ngram4 = GetNgramIndex(index_ngram4, 80);
			float coordinate_ngram5 = GetNgramIndex(index_ngram5, 80);
			float coordinate_ngram6 = GetNgramIndex(index_ngram6, 80);
			float coordinate_soundex = GetNgramIndex(index_soundex, 80);
			float coordinate_nysiis = GetNgramIndex(index_nysiis, 80);
			float coordinate_metaphone_primary = GetNgramIndex(index_metaphone_primary, 80);
			float coordinate_metaphone_secondary = GetNgramIndex(index_metaphone_secondary, 80);
			float coordinate_emotion = GetEmotionRating(question);
			
			plot_ctr++;
			if (plot_ctr > 10000)
			{
				float[] coherence2 = null;
				int[] hash1 = null;
				int[] hash2 = null;
				float[] index1 = null;
				float[] index2 = null;
                ShowPlot(
                    server_name,
                    database_name,
                    user_name,
                    password,
		            1000,
		            "mindpixels.bmp",
				    false,
				    ref coherence2,
				    ref hash1,
				    ref hash2,
				    ref index1,
				    ref index2);
				
				plot_ctr = 0;
			}
			
			string connection_str = 
                "server=" + server_name + ";"
                + "database=" + database_name + ";"
                + "uid=" + user_name + ";"
                + "password=" + password + ";";
			
            List<string> field_name = new List<string>();
            List<string> field_value = new List<string>();
				
			int YesVotes = 0;
			int NoVotes = 1;
			
			if (coherence > 0)
			{
				YesVotes = (int)(coherence*100);
				NoVotes = 100 - YesVotes;
				int denom = GCD(YesVotes,100);
				if (denom > 0)
				{
				    YesVotes /= denom;
				    NoVotes /= denom;
				}
			}			

			field_name.Add("Id");
			field_value.Add(System.Guid.NewGuid().ToString());
			field_name.Add("Hash");
			field_value.Add(hash.ToString());
			field_name.Add("Question");
			field_value.Add(question);
			field_name.Add("YesVotes");
			field_value.Add(YesVotes.ToString());				
			field_name.Add("NoVotes");
			field_value.Add(NoVotes.ToString());				
			field_name.Add("Coherence");
			field_value.Add(coherence.ToString());
			field_name.Add("Ngram2");
			field_value.Add(coordinate_ngram2.ToString());
			field_name.Add("Ngram3");
			field_value.Add(coordinate_ngram3.ToString());
			field_name.Add("Ngram4");
			field_value.Add(coordinate_ngram4.ToString());
			field_name.Add("Ngram5");
			field_value.Add(coordinate_ngram5.ToString());
			field_name.Add("Ngram6");
			field_value.Add(coordinate_ngram6.ToString());
			field_name.Add("Soundex");
			field_value.Add(coordinate_soundex.ToString());
			field_name.Add("NYSIIS");
			field_value.Add(coordinate_nysiis.ToString());
		    field_name.Add("MetaphonePrimary");
		    field_value.Add(coordinate_metaphone_primary.ToString());
		    field_name.Add("MetaphoneSecondary");
		    field_value.Add(coordinate_metaphone_secondary.ToString());
			field_name.Add("Emotion");
			field_value.Add(coordinate_emotion.ToString());
			
			// add new pixel
            MySqlConnection connection = new MySqlConnection();
			
			connection.ConnectionString = connection_str;
	
            bool connected = false;
            string exception_str = "";
            try
            {
                connection.Open();
                connected = true;
            }
            catch (Exception ex)
            {
                exception_str = ex.Message;
				connection.Close();
            }

            if (connected)
            {
                string Query = "INSERT INTO " + table_name + "(";

                for (int i = 0; i < field_name.Count; i++)
                {
                    if ((fields_to_be_inserted.Count == 0) ||
                        (fields_to_be_inserted.Contains(field_name[i])))
                    {
                        Query += field_name[i];
                        if (i < field_name.Count - 1) Query += ",";
                    }
                }
                Query += ") values(";
                for (int i = 0; i < field_value.Count; i++)
                {
                    int idx = fields_to_be_inserted.IndexOf(field_name[i]);

                    if ((fields_to_be_inserted.Count == 0) ||
                        (fields_to_be_inserted.Contains(field_name[i])))
                    {
                        if (fields_to_be_inserted.Count > 0)
                        {
                            if (idx > -1)
                            {
								if (field_type[idx].Contains("BINARY"))
								{
									field_value[i] = "?Id";
								}
								
                                if (field_type[idx] == "DATETIME")
                                {
                                    DateTime d = DateTime.Parse(field_value[i]);
                                    string d_str = d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString() + " " + d.Hour.ToString() + ":" + d.Minute.ToString() + ":" + d.Second.ToString();
                                    field_value[i] = d_str;
                                }
                            }
                        }

						if (!field_type[idx].Contains("BINARY"))
                            Query += "'" + field_value[i] + "'";
						else
							Query += field_value[i];
                        if (i < field_value.Count - 1) Query += ",";
                    }
                }
                Query += ")";

                MySqlCommand addxml = new MySqlCommand(Query, connection);

                try
                {
					addxml.Parameters.Add("Id", MySqlDbType.Binary).Value = System.Guid.NewGuid().ToByteArray();
                    addxml.ExecuteNonQuery();
                }
                catch
                {
                }

                connection.Close();
            }
            else
            {
                Console.WriteLine("InsertMindpixel: Couldn't connect to database " + database_name);
                Console.WriteLine(exception_str);
            }
        }
		
		/// <summary>
        /// convert probability to log odds
        /// </summary>
        /// <param name="probability"></param>
        /// <returns></returns>
        private static float LogOdds(float probability)
        {
            if (probability > 0.999f) probability = 0.999f;
            if (probability < 0.001f) probability = 0.001f;
            return ((float)Math.Log10(probability / (1.0f - probability)));
        }
		
        /// <summary>
        /// convert a log odds value back into a probability value
        /// </summary>
        /// <param name="logodds"></param>
        /// <returns></returns>
        private static float LogOddsToProbability(float logodds)
        {
            return(1.0f - (1.0f/(1.0f + (float)Math.Exp(logodds))));
        }		
		
        public static float Gaussian(float fraction)
        {
            fraction *= 3.0f;
            float prob = (float)((1.0f / Math.Sqrt(2.0*Math.PI))*Math.Exp(-0.5*fraction*fraction));

            return (prob*2.5f);
        }
				
        private static void ShowPlot(
            string server_name,
            string database_name,
            string user_name,
            string password,
		    int image_width,
		    string filename,
		    bool mono,
		    ref float[] coherence,
		    ref int[] hash1,
		    ref int[] hash2,
		    ref float[] index1,
		    ref float[] index2)
        {
			int radius = 3;
			Bitmap bmp = new Bitmap(image_width, image_width, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			byte[] img = new byte[image_width * image_width * 3];
			coherence = new float[image_width * image_width];
			bool[] occupied = new bool[image_width * image_width];
			
			float[] fraction_lookup = new float[(radius*2+1)*(radius*2+1)];
			int fctr = 0;
			for (int i = -radius; i <= radius; i++)
			{
			    for (int j = -radius; j <= radius; j++, fctr++)
			    {
					float dist = (float)Math.Sqrt(i*i + j*j);
				    float fraction = dist / (float)radius;
					fraction_lookup[fctr] = Gaussian(fraction);					
				}
			}
			
            if ((server_name == "") ||
                (server_name == null))
                server_name = "localhost";
			
			string connection_str = 
                "server=" + server_name + ";"
                + "database=" + database_name + ";"
                + "uid=" + user_name + ";"
                + "password=" + password + ";";
			
            string Query = "SELECT Hash,YesVotes,NoVotes,Emotion,Ngram3 FROM mindpixels ORDER BY Ngram3;";
			ArrayList result_ngram3 = RunMySqlCommand(Query, connection_str, 5);

			Query = "SELECT Hash,Soundex FROM mindpixels ORDER BY Soundex;";
			ArrayList result_soundex = RunMySqlCommand(Query, connection_str, 2);
			
			if ((result_ngram3 != null) &&
			    (result_soundex != null))
			{
				hash1 = new int[result_ngram3.Count];
				hash2 = new int[result_soundex.Count];
				index1 = new float[result_ngram3.Count];
				index2 = new float[result_soundex.Count];
				float[] pixelcoherence = new float[result_ngram3.Count];
				float[] pixelemotion = new float[result_ngram3.Count];
				
				int max = result_ngram3.Count;
				for (int i = 0; i < max; i++)
				{
					ArrayList row = (ArrayList)result_ngram3[i];
					string s0 = Convert.ToString(row[0]);
					string s1 = Convert.ToString(row[1]);
					string s2 = Convert.ToString(row[2]);
					string s3 = Convert.ToString(row[3]);
					string s4 = Convert.ToString(row[4]);
					hash1[i] = Convert.ToInt32(s0);
					index1[i] = Convert.ToSingle(s4);
					pixelcoherence[i] = Convert.ToSingle(s1) / (Convert.ToSingle(s1)+Convert.ToSingle(s2));
					pixelemotion[i] = Convert.ToSingle(s3);
				}
				for (int i = 0; i < max; i++)
				{
					ArrayList row = (ArrayList)result_soundex[i];
					string s0 = Convert.ToString(row[0]);
					string s1 = Convert.ToString(row[1]);
					hash2[i] = Convert.ToInt32(s0);
					index2[i] = Convert.ToSingle(s1);
				}
				for (int i = 0; i < max; i++)
				{				
					int j = Array.IndexOf(hash2, hash1[i]);
					if (j > -1)
					{
						int x = (int)(i * image_width / (float)max);
						int y = (int)(j * image_width / (float)max);
						
						fctr = 0;
						for (int yy = y - radius; yy <= y + radius; yy++)
						{
							int dy = yy-y;
						    for (int xx = x - radius; xx <= x + radius; xx++, fctr++)
						    {													
								if (((xx > -1) && (xx < image_width)) &&
								    ((yy > -1) && (yy < image_width)))
								{
									int dx = xx-x;
									
								    int n = (yy * image_width) + xx;
									float incr = LogOdds(0.5f + ((pixelcoherence[i]-0.5f) * fraction_lookup[fctr]));
									coherence[n] += incr;
									occupied[n] = true;
								}
							}
							
						}
						
					}
					else
					{
						Console.WriteLine(hash1[i].ToString() + " not found");
					}
				}
			}
			else
			{
				Console.WriteLine("No data");				
			}
			
			float prob;
			int n2 = 0;
			
			for (int k = coherence.Length-1; k >= 0; k--)
				coherence[k] = LogOddsToProbability(coherence[k]);
			
			for (int k = 0; k < img.Length; k+=3, n2++)
			{	
				if (occupied[n2])
				{
				    prob = coherence[n2];
					
					if (mono)
					{
						img[k] = (byte)(prob*255);
						img[k+1] = (byte)(prob*255);
						img[k+2] = (byte)(prob*255);
					}
					else
					{					
						if (prob > 0.5f)
						{
							int v = (int)((prob-0.5f) * 255*2);
							if (v > 255) v = 255;
						    img[k+1] = (byte)v;
						}
						else
						{
							int v = -(int)((prob-0.5f) * 255*2);
							if (v > 255) v = 255;
							img[k+2] = (byte)(v);
							img[k] = (byte)(255-v);							
						}					
					}
				}
			}
			
			if ((filename != "") && (filename != null))
			{
				BitmapArrayConversions.updatebitmap_unsafe(img, bmp);
				if (filename.EndsWith("bmp")) bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Bmp);
				if (filename.EndsWith("jpg")) bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
				if (filename.EndsWith("png")) bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
				if (filename.EndsWith("gif")) bmp.Save(filename, System.Drawing.Imaging.ImageFormat.Gif);
			}
        }		
		
        public static void InsertMindpixelUserDataIntoMySql(
		    int hash,
            string question,
            bool answer,
            string server_name,
            string database_name,
            string user_name,
            string password,
            string table_name,
            List<string> fields_to_be_inserted,
            List<string> field_type)
        {
            List<string> field_name = new List<string>();
            List<string> field_value = new List<string>();

			field_name.Add("TimeStamp");
			field_value.Add(DateTime.Now.ToString());
			field_name.Add("Username");
			field_value.Add(user_name);
			field_name.Add("Hash");
			field_value.Add(hash.ToString());
			field_name.Add("Question");
			field_value.Add(question);
			field_name.Add("Answer");
			if (answer == true)
			    field_value.Add("1");
			else
				field_value.Add("0");

            if ((server_name == "") ||
                (server_name == null))
                server_name = "localhost";
			
            MySqlConnection connection = new MySqlConnection();

            connection.ConnectionString =
                "server=" + server_name + ";"
                + "database=" + database_name + ";"
                + "uid=" + user_name + ";"
                + "password=" + password + ";";

            bool connected = false;
            string exception_str = "";
            try
            {
                connection.Open();
                connected = true;
            }
            catch (Exception ex)
            {
                exception_str = ex.Message;
				connection.Close();
            }

            if (connected)
            {
                string Query = "INSERT INTO " + table_name + "(";

                for (int i = 0; i < field_name.Count; i++)
                {
                    if ((fields_to_be_inserted.Count == 0) ||
                        (fields_to_be_inserted.Contains(field_name[i])))
                    {
                        Query += field_name[i];
                        if (i < field_name.Count - 1) Query += ",";
                    }
                }
                Query += ") values(";
                for (int i = 0; i < field_value.Count; i++)
                {
                    int idx = fields_to_be_inserted.IndexOf(field_name[i]);

                    if ((fields_to_be_inserted.Count == 0) ||
                        (fields_to_be_inserted.Contains(field_name[i])))
                    {
                        if (fields_to_be_inserted.Count > 0)
                        {
                            if (idx > -1)
                            {
                                if (field_type[idx] == "DATETIME")
                                {
                                    DateTime d = DateTime.Parse(field_value[i]);
                                    string d_str = d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString() + " " + d.Hour.ToString() + ":" + d.Minute.ToString() + ":" + d.Second.ToString();
                                    field_value[i] = d_str;
                                }
                            }
                        }

                        Query += "'" + field_value[i] + "'";
                        if (i < field_value.Count - 1) Query += ",";
                    }
                }
                Query += ")";

                MySqlCommand addxml = new MySqlCommand(Query, connection);

                //Console.WriteLine("Running query:");
                //Console.WriteLine(Query);

                try
                {
                    addxml.ExecuteNonQuery();
                }
                catch (Exception excp)
                {
                    Exception myExcp = new Exception("Could not add mindpixel. Error: " + excp.Message, excp);
                    throw (myExcp);
                }

                connection.Close();
            }
            else
            {
                Console.WriteLine("Couldn't connect to database " + database_name);
                Console.WriteLine(exception_str);
            }
        }
		
		#endregion
		
		#region "saving the database to a text file"
		
        private static void SaveMindpixels(
		    string save_filename, 
		    string server_name,
		    string database_name, 
		    string user_name, 
		    string password, 
		    string mp_table_name)
		{
            StreamWriter oWrite = null;
            bool allowWrite = true;
            string GAC_str;
            string coherence_str;
            int coherence_int;			
			
            if ((server_name == "") ||
                (server_name == null))
                server_name = "localhost";
			
			int no_of_fields = 6;
			
            string connection_str =
                "server=" + server_name + ";"
                + "database=" + database_name + ";"
                + "uid=" + user_name + ";"
                + "password=" + password + ";";
			
		    ArrayList result = RunMySqlCommand(
		        "SELECT * FROM " + mp_table_name + ";", 
		        connection_str, no_of_fields);
			
            try
            {
                oWrite = File.CreateText(save_filename);
            }
            catch
            {
                allowWrite = false;
            }			
			
			if (allowWrite)
			{
				Console.WriteLine("Saving " + result.Count.ToString() + " mindpixels");
				
                oWrite.WriteLine("Mindpixelator Data Set");
                oWrite.WriteLine(result.Count.ToString() + " Propositions with a Corresponding Measure of Human Semantic Coherence");
                oWrite.WriteLine("");
                oWrite.WriteLine(">> Mind Hack with GAC <<");
                oWrite.WriteLine("");
				
				for (int i = 0; i < result.Count; i++)
				{
					ArrayList row = (ArrayList)result[i];

					string question = (string)row[2];
					float coherence = (float)row[5];
							
	                coherence_int = (int)(coherence * 100);
	                coherence_str = Convert.ToString(coherence_int);
					for (int j = 0; j < 3; j++)
					    if (coherence_str.Length < 3) coherence_str = "0" + coherence_str;
	                coherence_str = coherence_str.Substring(0, 1) + "." + coherence_str.Substring(1, 2);
	                GAC_str = Convert.ToString((char)9) + coherence_str + Convert.ToString((char)9) + question;
	                oWrite.WriteLine(GAC_str);
				}
				
	            oWrite.Close();		
				
				Console.WriteLine("Done");				
			}
		}
		
		#endregion

		#region "saving pixels for a particular user"
		
        private static void SaveUserPixels(
		    string save_filename, 
		    string server_name,
		    string database_name, 
		    string user_name, 
		    string password, 
		    string users_table_name)
		{
            StreamWriter oWrite = null;
            bool allowWrite = true;
			
            if ((server_name == "") ||
                (server_name == null))
                server_name = "localhost";
			
			int no_of_fields = 5;
			
            string connection_str =
                "server=" + server_name + ";"
                + "database=" + database_name + ";"
                + "uid=" + user_name + ";"
                + "password=" + password + ";";
			
		    ArrayList result = RunMySqlCommand(
		        "SELECT * FROM " + users_table_name + " WHERE Username = '" + user_name + "';", 
		        connection_str, no_of_fields);
			
            try
            {
                oWrite = File.CreateText(save_filename);
            }
            catch
            {
                allowWrite = false;
            }			
			
			if (allowWrite)
			{
				Console.WriteLine("Saving " + result.Count.ToString() + " mindpixels for user " + user_name);
				
				for (int i = 0; i < result.Count; i++)
				{
					ArrayList row = (ArrayList)result[i];

					string question = (string)row[3];
					int answer = Convert.ToInt32(row[4]);
					string answer_str = "Yes";
					if (answer == 0) answer_str = "No";
	                oWrite.WriteLine(answer_str + (char)9 + question);
				}
				
	            oWrite.Close();		
				
				Console.WriteLine("Done");				
			}
		}
		
		#endregion

		#region "return number of records in a table"
		
        private static int NoOfRecords(
		    string server_name,
		    string database_name, 
		    string user_name, 
		    string password, 
		    string table_name)
		{
			int count = 0;
		
            if ((server_name == "") ||
                (server_name == null))
                server_name = "localhost";
			
            string connection_str =
                "server=" + server_name + ";"
                + "database=" + database_name + ";"
                + "uid=" + user_name + ";"
                + "password=" + password + ";";
			
		    ArrayList result = RunMySqlCommand(
			    "SELECT COUNT(1) FROM " + table_name + ";",
		        connection_str, 1);
						
			if (result.Count > 0)
			{
				ArrayList row = (ArrayList)result[0];
				count = Convert.ToInt32(row[0]);
			}
			
			return(count);
		}

        private static int NoOfTrueRecords(
		    string server_name,
		    string database_name, 
		    string user_name, 
		    string password, 
		    string table_name)
		{
			int count = 0;
		
            if ((server_name == "") ||
                (server_name == null))
                server_name = "localhost";
			
            string connection_str =
                "server=" + server_name + ";"
                + "database=" + database_name + ";"
                + "uid=" + user_name + ";"
                + "password=" + password + ";";
			
		    ArrayList result = RunMySqlCommand(
			    "SELECT COUNT(1) FROM " + table_name + " WHERE YesVotes > NoVotes;",
		        connection_str, 1);
						
			if (result.Count > 0)
			{
				ArrayList row = (ArrayList)result[0];
				count = Convert.ToInt32(row[0]);
			}
			
			return(count);
		}

        private static int NoOfFalseRecords(
		    string server_name,
		    string database_name, 
		    string user_name, 
		    string password, 
		    string table_name)
		{
			int count = 0;
		
            if ((server_name == "") ||
                (server_name == null))
                server_name = "localhost";
			
            string connection_str =
                "server=" + server_name + ";"
                + "database=" + database_name + ";"
                + "uid=" + user_name + ";"
                + "password=" + password + ";";
			
		    ArrayList result = RunMySqlCommand(
			    "SELECT COUNT(1) FROM " + table_name + " WHERE YesVotes < NoVotes;",
		        connection_str, 1);
						
			if (result.Count > 0)
			{
				ArrayList row = (ArrayList)result[0];
				count = Convert.ToInt32(row[0]);
			}
			
			return(count);
		}
		
		#endregion

		#region "saving random mindpixels to a text file"
		
        private static void SaveRandomMindpixels(
		    string save_filename, 
		    string server_name,
		    string database_name, 
		    string user_name, 
		    string password, 
		    string mp_table_name,
		    int no_of_pixels)
		{
			Random rnd = new Random();
			bool probably_true = false;
			if (rnd.Next(10000) > 5000) probably_true = true;
			
			int no_of_records;
			if (probably_true)
			    no_of_records = NoOfTrueRecords(
		            server_name,
		            database_name, 
		            user_name, 
		            password, 
		            mp_table_name);
			else
			    no_of_records = NoOfFalseRecords(
		            server_name,
		            database_name, 
		            user_name, 
		            password, 
		            mp_table_name);
							             
			if (no_of_pixels < 1) no_of_pixels = 1;			
			int max = no_of_records-no_of_pixels+1;
			if ((no_of_records > 0) && (max > 0))
			{			    
			    int start_row = rnd.Next(max);				
				
	            StreamWriter oWrite = null;
	            bool allowWrite = true;
								
	            if ((server_name == "") ||
	                (server_name == null))
	                server_name = "localhost";
				
				int no_of_fields = 6;
				
	            string connection_str =
	                "server=" + server_name + ";"
	                + "database=" + database_name + ";"
	                + "uid=" + user_name + ";"
	                + "password=" + password + ";";
				
				string Query = "";
				if (probably_true)
				    Query = "SELECT * FROM " + mp_table_name + " WHERE YesVotes > NoVotes LIMIT " + no_of_pixels.ToString() + " OFFSET " + start_row.ToString();
				else
				    Query = "SELECT * FROM " + mp_table_name + " WHERE YesVotes < NoVotes LIMIT " + no_of_pixels.ToString() + " OFFSET " + start_row.ToString();
				
			    ArrayList result = RunMySqlCommand(
				    Query,
			        connection_str, no_of_fields);
				
	            try
	            {
	                oWrite = File.CreateText(save_filename);
	            }
	            catch
	            {
	                allowWrite = false;					
	            }			
				
				if (allowWrite)
				{
					Console.WriteLine("Saving " + result.Count.ToString() + " random mindpixels");
					
					for (int i = 0; i < result.Count; i++)
					{
						ArrayList row = (ArrayList)result[i];
	
						string question = (string)row[2];
						float coherence = (float)row[5];
						
		                oWrite.WriteLine(((int)(coherence*100)/100.0f).ToString() + (char)9 + question);
					}
					
		            oWrite.Close();		
					
					Console.WriteLine("Done");				
				}
			}
		}
		
		#endregion
				
		#region "load mindpixels from text file"
		
		static string ToNumeric(string str)
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
		
        static void loadGAC(
		    string mindpixels_filename, 
            string initialstring,
		    string[] mp_fields,
		    string[] users_fields,
		    string server_name,
		    string database_name, 
		    string user_name, 
		    string password, 
		    string mp_table_name,
		    string users_table_name)
        {
            int no_of_records = NoOfRecords(
		        server_name,
		        database_name, 
		        user_name, 
		        password, 
		        mp_table_name);	
			
			if (no_of_records > 1)
			{
				Console.WriteLine(mp_table_name + " table is not empty.  Please empty the table before loading");
			}
			else
			{	
				no_of_records = NoOfRecords(
		            server_name,
		            database_name, 
		            user_name, 
		            password, 
		            users_table_name);	
				if (no_of_records > 1)
				{
					Console.WriteLine(users_table_name + " table is not empty.  Please empty the table before loading");
				}
				else
				{				
		            bool filefound = true;
		            string str, question;
		            float coherence;
					StreamReader oRead = null;
					Random rnd = new Random();
					
		            List<string> mp_fields_to_be_inserted = new List<string>();
		            List<string> mp_field_type = new List<string>();
		            for (int i = 0; i < mp_fields.Length; i += 2)
		            {
		                mp_fields_to_be_inserted.Add(mp_fields[i]);
		                mp_field_type.Add(mp_fields[i + 1]);
		            }
					
		            try
		            {
		                oRead = File.OpenText(mindpixels_filename);
		            }
		            catch
		            {
		                filefound = false;
		            }
		
		            if (filefound)
		            {
						Console.WriteLine("WARNING: This may take some time...");
		                bool initialstringFound = false;
						int i = 0;
								
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
										int question_hash = GetHashCode(question);
										
										i++;
										if (rnd.Next(2000) < 2) Console.WriteLine(i.ToString() + (char)9 + question);
										
							            InsertMindpixelWithCoherence(
										    question_hash,
							                question,
							                coherence,
							                server_name,
							                database_name,
							                user_name,
							                password,
							                mp_table_name,
							                mp_fields_to_be_inserted,
							                mp_field_type);								
		
			                        }
			                        catch //(Exception ex)
			                        {
										//Console.WriteLine("str: " + str);
										//Console.WriteLine("error: " + ex.Message);
			                        }
			                    }
			                }
			            }
			            if (oRead.EndOfStream)
			            {
			                oRead.Close();
			            }
						
	                    no_of_records = NoOfRecords(
			                server_name,
			                database_name, 
			                user_name, 
			                password, 
			                mp_table_name);	
						
						Console.WriteLine(no_of_records.ToString() + " records loaded");
					}
				}
			}
        }

		#endregion
		
        #region "validation"

        /// <summary>
        /// returns a list of valid parameter names
        /// </summary>
        /// <returns>list of valid parameter names</returns>
        private static ArrayList GetValidParameters()
        {
            ArrayList ValidParameters = new ArrayList();

            ValidParameters.Add("q");
            ValidParameters.Add("a");
            ValidParameters.Add("username");
            ValidParameters.Add("password");
			ValidParameters.Add("server");
			ValidParameters.Add("wp");
			ValidParameters.Add("fb");
			ValidParameters.Add("db");
			ValidParameters.Add("cn");
			ValidParameters.Add("load");
			ValidParameters.Add("save");
			ValidParameters.Add("map");
			ValidParameters.Add("mapmono");
			ValidParameters.Add("lookup");
			ValidParameters.Add("random");
			ValidParameters.Add("userpixels");
            ValidParameters.Add("help");
            return (ValidParameters);
        }

        #endregion
		
        #region "help information"

        /// <summary>
        /// shows help information
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("mpcreate Help");
            Console.WriteLine("-------------");
            Console.WriteLine("");
            Console.WriteLine("Syntax:  mpcreate");
            Console.WriteLine("");
            Console.WriteLine("         -q <question>");
            Console.WriteLine("         -a <answer>");
            Console.WriteLine("         -username <username>");
            Console.WriteLine("         -password <password>");
            Console.WriteLine("         -server <server name>");
            Console.WriteLine("         -wp <wikipedia schools edition directory>");
            Console.WriteLine("         -fb <Freebase TSV data directory>");
			Console.WriteLine("         -cn <conceptnet RDF file>");
			Console.WriteLine("         -db <mysql database name>");
            Console.WriteLine("         -load <mindpixel file>");
            Console.WriteLine("         -save <mindpixel file>");
            Console.WriteLine("         -map <mindpixel map image file>");
            Console.WriteLine("         -mapmono <mindpixel map image file>");
            Console.WriteLine("         -lookup <lookup tables file>");
            Console.WriteLine("         -random <filename>");
            Console.WriteLine("         -userpixels <filename>");
            Console.WriteLine("");
            Console.WriteLine("Example: mpcreate.exe -q " + '"' + "Is water wet?" + '"' + " -a yes");
        }

        #endregion	
	}
}