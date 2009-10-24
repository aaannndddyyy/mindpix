using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using sluggish.utilities;

namespace mpblog
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			string prefix = "mindpixel";
			
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
				bool test_only = false;
				string test_only_str = commandline.GetParameterValue("test", parameters);
				if (test_only_str != "") test_only = true;
				
                string prefix_str = commandline.GetParameterValue("prefix", parameters);
				if (prefix_str != "")
				{
					prefix = prefix_str;
					if (prefix.StartsWith("#")) prefix = prefix.Substring(1, prefix.Length-1);
					if (prefix.StartsWith("!")) prefix = prefix.Substring(1, prefix.Length-1);
				}

				string filename = "/usr/bin/mindpixelsblog.txt";
				if (!File.Exists(filename)) filename = "mindpixelsblog.txt";
                string filename_str = commandline.GetParameterValue("mindpixels", parameters);
				if (filename_str != "")
				{
					if (File.Exists(filename_str)) 
						filename = filename_str;
				}
				
				if (File.Exists(filename))
				{						
					int no_of_mindpixels = 80000;
					string random_pixel = GetRandom(filename, no_of_mindpixels);
					while ((random_pixel.Length > 50) ||
					       (random_pixel == "") ||
						   (ContainsBannedWord(random_pixel)))
					{
						random_pixel = GetRandom(filename, no_of_mindpixels);
					}
					
					random_pixel = "#" + prefix + " " + random_pixel;
					       
					Console.WriteLine(random_pixel);
					random_pixel = '"' + random_pixel + '"';
					if (!test_only) RunCommand("twidge","update " + random_pixel);
					
				}
				else
				{
					Console.WriteLine("mindpixel file " + filename + " not found");
					ShowHelp();
				}
			}
		}
		
		/// <summary>
		/// Might be a good idea not to blog anything which might be considered to be offensive
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private static bool ContainsBannedWord(string str)
		{
		    bool banned = false;

			string[] banned_words = {
			    "fuck",
				"sex",
				"rape",
				"shit",
				"penis",
				"cock",
				"dick",
				"vagina",
				"mckinstry",
				"gac",
				"orgasm",
				"ejac",
				"tits",
				"mindpixel",
				"mind pixel",
				"fsck",
				"nude",
				"naked",
				"testic",
				"horny",
				"pedo",
				"paedo",
				"gay",
				"lesb",
				"porn",
				"cum",
				"semen",
				"sperm",
				"piss",
				"urine",
				"perver",
				"masturb",
				"arous",
			};
					
			str = str.Trim().ToLower();
			if (str.EndsWith("?")) str = str.Substring(0, str.Length-1);
			if (str.EndsWith(".")) str = str.Substring(0, str.Length-1);
			if (str.EndsWith("!")) str = str.Substring(0, str.Length-1);
			str = " " + str + " ";							
			for (int i = 0; i < banned_words.Length; i++)
			{
				if ((str.Contains(" " + banned_words[i] + " ")) ||
                    (str.Contains(" " + banned_words[i])) ||
					(str.Contains(banned_words[i] + " ")))
				{
					banned = true;
					break;
				}
			}
			return(banned);
		}		
		
		private static string GetRandom(
		    string filename,
		    int no_of_mindpixels)
		{
			string question = "";
			Random rnd = new Random();
            bool filefound = true;
			int i = 0;
			string str;
			StreamReader oRead = null;
			
			int index = rnd.Next(no_of_mindpixels);

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
				bool initialstringFound = false;
				string initialstring = "Mind Hack";
	            while ((!oRead.EndOfStream) && (question == ""))
	            {
	                str = oRead.ReadLine();
	                if (!initialstringFound)
	                {
	                    /// look for an initial header string after which the data begins
	                    if (str.Contains(initialstring)) initialstringFound = true;
	                }
	                else
	                {
	                    if (str != "")
	                    {
	                        try
	                        {
								if (i == index) question = str.Substring(6);
							    i++;
							}
							catch
							{
							}
						}
					}
				}
				oRead.Close();
			}
			return(question);
		}
		
		private static void RunCommand(string command, string arguments)
		{
            Process proc = new Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = command;
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.RedirectStandardOutput = false;            
            try
            {
                proc.Start();
            }
            catch
            {
            }
		}
		
        #region "validation"

        /// <summary>
        /// returns a list of valid parameter names
        /// </summary>
        /// <returns>list of valid parameter names</returns>
        private static ArrayList GetValidParameters()
        {
            ArrayList ValidParameters = new ArrayList();

            ValidParameters.Add("mindpixels");
			ValidParameters.Add("prefix");
			ValidParameters.Add("test");
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
            Console.WriteLine("mpblog Help");
            Console.WriteLine("-----------");
            Console.WriteLine("");
            Console.WriteLine("Syntax:  mpblog");
            Console.WriteLine("");
            Console.WriteLine("         -mindpixels <mindpixel file>");
            Console.WriteLine("         -prefix <hash prefix>");
			Console.WriteLine("         -test (test output only)");
        }

        #endregion	
		
	}
}