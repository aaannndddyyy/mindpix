/*
    mpmist
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
			Console.WriteLine("Minimum Intelligent Signal Test M.I.S.T.");
			Console.WriteLine("Version 0.2");
			
            // default parameters for the database
            string server_name = "localhost";
            string database_name = "mindpixel";
            string user_name = "testuser";
            string password = "password";
            string mp_table_name = "mindpixels";
												
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
				string server_name_str = commandline.GetParameterValue("server", parameters);
				if (server_name_str != "") server_name = server_name_str;
				
				string user_name_str = commandline.GetParameterValue("username", parameters);
				if (user_name_str != "") user_name = user_name_str;

				string password_str = commandline.GetParameterValue("password", parameters);
				if (password_str != "") password = password_str;						
				
				string database_str = commandline.GetParameterValue("db", parameters);
				if (database_str != "") database_name = database_str;
				
				int no_of_test_cases = 100;
                string no_of_test_cases_str = commandline.GetParameterValue("samples", parameters);
				if (no_of_test_cases_str != "")
				{
					no_of_test_cases = Convert.ToInt32(no_of_test_cases_str);
				}
				
                string create_MIST_test_filename = commandline.GetParameterValue("create", parameters);
			    if (create_MIST_test_filename != "")
				{
				    CreateMISTtest(create_MIST_test_filename, server_name, database_name, user_name, password, mp_table_name, no_of_test_cases);
				}
				else
				{				
		            string MIST_test_answers_filename = commandline.GetParameterValue("answers", parameters);
					if (MIST_test_answers_filename != "")
					{
					    MISTscore(MIST_test_answers_filename, server_name, database_name, user_name, password, mp_table_name);
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
		
		#endregion
		
		#region "MIST scores"
				
        public static bool MISTpassed(		    
            string question,
            bool answer,
            string server_name,
            string database_name,
            string user_name,
            string password,
            string table_name)
        {
			bool passed = false;
			
			int hash = GetHashCode(question);
			
            if ((server_name == "") ||
                (server_name == null))
                server_name = "localhost";
						
			string connection_str = 
                "server=" + server_name + ";"
                + "database=" + database_name + ";"
                + "uid=" + user_name + ";"
                + "password=" + password + ";";
			
			string Query = "SELECT * FROM " + table_name + " WHERE Hash = " + hash.ToString() + ";";
			
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
                }
				
				if (best_result != Guid.Empty)
				{
					// update the mindpixel coherence
					float coherence = YesVotes / (float)(YesVotes + NoVotes);
					if ((answer == true) && (coherence >= 0.5)) passed = true;					
				}
			}
			return(passed);
        }
		
        private static void MISTscore(
		    string answers_filename, 
		    string server_name,
		    string database_name, 
		    string user_name, 
		    string password, 
		    string mp_table_name)
		{
			int no_of_answers = 0;
			int score = 0;
			
			if (File.Exists(answers_filename))
			{
	            StreamReader oRead = null;
	            bool allowRead = true;

				try
	            {
	                oRead = File.OpenText(answers_filename);
	            }
	            catch
	            {
	                allowRead = false;
	            }			
				
				if (allowRead)
				{
					string answer = "";
					while ((answer != null) && (!oRead.EndOfStream))
					{
					    answer = oRead.ReadLine();
						if (answer != null)
						{
							string[] s = answer.Split(' ');
							if (s.Length > 3)
							{
								string answer_value_str = s[s.Length-1].ToLower();
								bool answer_value = false;
								if ((answer_value_str == "y") ||
								    (answer_value_str == "yes") ||
								    (answer_value_str == "true") ||
								    (answer_value_str == "1"))
								{
									answer_value = true;
								}
								
								string question = answer.Substring(0, answer.Length - s[s.Length-1].Length - 1).Trim();								
								
								// does GAC agree?								
                                bool passed = MISTpassed(		    
                                    question,
                                    answer_value,
                                    server_name,
                                    database_name,
                                    user_name,
                                    password,
                                    mp_table_name);
								
								Console.WriteLine(question);
								Console.Write("Answered " + answer_value_str.ToUpper() + "   = ");
								if (passed) 
									Console.WriteLine("PASS");
								else
									Console.WriteLine("FAIL");								
								Console.WriteLine("");
								if (passed) score++;
								no_of_answers++;
							}
						}
					}
					oRead.Close();
					
					if (no_of_answers > 0)
					{
					    float score_percent = score * 100 / (float)no_of_answers;
						Console.WriteLine("Out of " + no_of_answers.ToString() + " questions " + score.ToString() + " were correct");
						Console.WriteLine("MIST rating: " + ((int)(score_percent * 100)/100.0f).ToString() + "%");
					}
					else
					{
						Console.WriteLine("The answers file did not contain any answers!");
					}
				}	
				else
				{
					Console.WriteLine("Unable to read file " + answers_filename);
				}
			}
			else
			{
				Console.WriteLine("File " + answers_filename + " not found");
			}
		}
						
		#endregion

		#region "return number of records in a table"

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

		#region "creating a MIST test"
		
        private static void CreateMISTtest(
		    string test_filename, 
		    string server_name,
		    string database_name, 
		    string user_name, 
		    string password, 
		    string mp_table_name,
		    int no_of_samples)
		{
			int no_of_true_records = NoOfTrueRecords(
		        server_name,
		        database_name, 
		        user_name, 
		        password, 
		        mp_table_name);
			
			int no_of_false_records = NoOfFalseRecords(
		        server_name,
		        database_name, 
		        user_name, 
		        password, 
		        mp_table_name);
			             
			if (no_of_samples < 1) no_of_samples = 1;			
			int max_true = no_of_true_records;
			int max_false = no_of_false_records;
			if ((no_of_true_records > 0) && 
			    (no_of_false_records > 0))
			{
			    Random rnd = new Random();
				
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
				
	            try
	            {
	                oWrite = File.CreateText(test_filename);
	            }
	            catch
	            {
	                allowWrite = false;
	            }			
				
				if (allowWrite)
				{
					Console.Write("Creating MIST test consisting of " + no_of_samples.ToString() + " samples...");
						
					int start_row;
					ArrayList result = null;
					for (int sample = 0; sample < no_of_samples; sample++)
					{
						if (rnd.Next(1000) > 500)
						{
					        start_row = rnd.Next(max_true);
										
					        result = RunMySqlCommand(
						        "SELECT * FROM " + mp_table_name + " WHERE YesVotes > NoVotes LIMIT 1 OFFSET " + start_row.ToString(),
					            connection_str, no_of_fields);
						}
						else
						{
					        start_row = rnd.Next(max_false);
										
					        result = RunMySqlCommand(
						        "SELECT * FROM " + mp_table_name + " WHERE YesVotes < NoVotes LIMIT 1 OFFSET " + start_row.ToString(),
					            connection_str, no_of_fields);
						}
						
						if (result.Count > 0)
						{
						    ArrayList row = (ArrayList)result[0];
							string question = (string)row[2];
							oWrite.WriteLine(question);
						}
					}
					
		            oWrite.Close();		
					
					Console.WriteLine("Done");				
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

            ValidParameters.Add("create");
            ValidParameters.Add("answers");
            ValidParameters.Add("samples");
            ValidParameters.Add("username");
            ValidParameters.Add("password");
			ValidParameters.Add("server");
			ValidParameters.Add("db");
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
            Console.WriteLine("mpmist Help");
            Console.WriteLine("-----------");
            Console.WriteLine("");
            Console.WriteLine("Syntax:  mpmist");
            Console.WriteLine("");
            Console.WriteLine("         -create <MIST test text file>");
            Console.WriteLine("         -answers <MIST test text file>");
            Console.WriteLine("         -samples <number of test samples>");
            Console.WriteLine("         -username <username>");
            Console.WriteLine("         -password <password>");
            Console.WriteLine("         -server <server name>");
            Console.WriteLine("         -db <mysql database name>");
            Console.WriteLine("");
            Console.WriteLine("Example: mpmist.exe -create MIST_questions.txt -samples 100");
            Console.WriteLine("         mpmist.exe -rate MIST_answers.txt");
        }

        #endregion	
	}
}