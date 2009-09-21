/*
    mpexport
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
using sluggish.utilities;
using System.Text;

namespace mpexport
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("mpexport: A utility for exporting mindpixel data to other formats");
			Console.WriteLine("Version 0.2");
			
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
				bool show_mindpixels = false;
                string show_mindpixels_str = commandline.GetParameterValue("show", parameters);
				if (show_mindpixels_str != "")
				{
					show_mindpixels = true;
				}
				
				string filename = "/usr/bin/mindpixels.txt";
                string filename_str = commandline.GetParameterValue("mindpixels", parameters);
				if (filename_str != "")
				{
					if (File.Exists(filename_str)) filename = filename_str;
				}
				
				if (File.Exists(filename))
				{				
				    minimind mind = new minimind();
					Console.WriteLine("Loading mindpixels...");
				    mind.LoadGAC(filename, show_mindpixels);
					Console.WriteLine(mind.mindpixels.Count.ToString() + " mindpixels loaded");
					
					string pop_str = commandline.GetParameterValue("pop", parameters);
					if (pop_str != "")
					{
						Console.WriteLine("Exporting as POP");
						mind.ExportAsPOP(pop_str);
					}
					
					string nars_str = commandline.GetParameterValue("nars", parameters);
					if (nars_str != "")
					{
						Console.WriteLine("Exporting as NARS");
						mind.ExportAsNARS(nars_str);
					}

					string oc_str = commandline.GetParameterValue("oc", parameters);
					if (oc_str != "")
					{
						Console.WriteLine("Exporting as OpenCog (Scheme)");
						mind.ExportAsOpenCogScheme(oc_str);
					}
											
				}
				else
				{
					Console.WriteLine("mindpixel file " + filename + " not found");
					ShowHelp();
				}
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
            ValidParameters.Add("show");
            ValidParameters.Add("pop");
            ValidParameters.Add("nars");
            ValidParameters.Add("oc");
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
            Console.WriteLine("mpexport Help");
            Console.WriteLine("-------------");
            Console.WriteLine("");
            Console.WriteLine("Syntax:  mpexport");
            Console.WriteLine("");
            Console.WriteLine("         -mindpixels <mindpixel file>");
            Console.WriteLine("         -show (whether to display a random sample of loaded mindpixels to the console)");
            Console.WriteLine("         -pop <filename for output in POP format>");
            Console.WriteLine("         -nars <filename for output in NARS format>");
            Console.WriteLine("         -oc <filename for output in OpenCog Scheme format>");
        }

        #endregion	
	}
}