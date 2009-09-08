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
			Console.WriteLine("mpcreate: A utility for creating mindpixels");
			Console.WriteLine("Version 0.1");
			
            // default parameters for the database
            string server_name = "localhost";
            string database_name = "mindpixel";
            string user_name = "testuser";
            string password = "password";
            string mp_table_name = "mindpixels";
            string users_table_name = "users";
			
            string[] mp_fields = {
				"Id", "BINARY(16) NOT NULL", // that's 16 bytes, not 16 bits!
                "Hash", "INT",
                "Question", "TEXT",
                "YesVotes", "INT",
                "NoVotes", "INT",
                "Coherence", "FLOAT"
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
											Console.WriteLine("Please specify a question using the -q option");
										}
									}
								}
							}
						}
					}
				}
			}
			
		}
		
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
				Query += ")";

                MySqlCommand addxml = new MySqlCommand(Query, connection);

                //Console.WriteLine("Running query:");
                //Console.WriteLine(Query);

                try
                {
                    addxml.ExecuteNonQuery();
                }
                catch
                {
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
					float coherence = YesVotes / (float)(YesVotes + NoVotes);
					
				    Query = "UPDATE " + table_name + 
						" SET YesVotes='" + YesVotes.ToString() + "'," +
						" NoVotes='" + NoVotes.ToString() + "'," +
						" Coherence='" + coherence.ToString() + "'" +
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
				
				float coherence;
				
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
			int no_of_records = NoOfRecords(
		        server_name,
		        database_name, 
		        user_name, 
		        password, 
		        mp_table_name);
			             
			if (no_of_pixels < 1) no_of_pixels = 1;			
			int max = no_of_records-no_of_pixels+1;
			if ((no_of_records > 0) && (max > 0))
			{
			    Random rnd = new Random();
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
				
			    ArrayList result = RunMySqlCommand(
				    "SELECT * FROM " + mp_table_name + " LIMIT " + no_of_pixels.ToString() + " OFFSET " + start_row.ToString(),
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
			ValidParameters.Add("load");
			ValidParameters.Add("save");
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
			Console.WriteLine("         -db <mysql database name>");
            Console.WriteLine("         -load <mindpixel file>");
            Console.WriteLine("         -save <mindpixel file>");
            Console.WriteLine("         -random <filename>");
            Console.WriteLine("         -userpixels <filename>");
            Console.WriteLine("");
            Console.WriteLine("Example: mpcreate.exe -q " + '"' + "Is water wet?" + '"' + " -a yes");
        }

        #endregion	
	}
}