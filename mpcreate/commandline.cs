/*
    Command line utility functions
    Copyright (C) 2000-2007 Bob Mottram
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

namespace sluggish.utilities
{
    public class commandline
    {
        #region "release stages"

        public const int ALPHA = 0;
        public const int BETA = 1;
        public const int RELEASE = 2;

        #endregion

        #region "adding and removing parameters"

        /// <summary>
        /// removes the parameter with the given name
        /// </summary>
        /// <param name="param_name">name of the parameter to be removed</param>
        /// <param name="parameters">list of parameters</param>
        /// <returns>true if removed</returns>
        public static bool RemoveParameter(string param_name, ArrayList parameters)
        {
            bool found = false;
            int i = 0;
            while ((i < parameters.Count) && (!found))
            {
                ArrayList param = (ArrayList)parameters[i];
                String name = (String)param[0];
                if (name == param_name)
                {
                    parameters.RemoveAt(i);
                    found = true;
                }
                else i++;
            }
            return (found);
        }

        /// <summary>
        /// edits a parameter value
        /// </summary>
        /// <param name="param_name"></param>
        /// <param name="new_param_value"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static bool EditParameter(string param_name, string new_param_value, ArrayList parameters)
        {
            bool found = false;
            int i = 0;
            while ((i < parameters.Count) && (!found))
            {
                ArrayList param = (ArrayList)parameters[i];
                String name = (String)param[0];
                if (name == param_name)
                {
                    param[1] = new_param_value;
                    found = true;
                }
                else i++;
            }
            return (found);
        }

        /// <summary>
        /// adds a new parameter to the list
        /// </summary>
        /// <param name="param_name">name of the parameter to be added</param>
        /// <param name="param_value">value of the parameter</param>
        /// <param name="parameters">list of parameters</param>
        public static void AddParameter(string param_name, string param_value, ArrayList parameters)
        {
            ArrayList new_parameter = new ArrayList();
            new_parameter.Add(param_name);
            new_parameter.Add(param_value);
            parameters.Add(new_parameter);
        }

        #endregion

        #region "parsing parameters"

        /// <summary>
        /// returns the value associated with the given parameter name
        /// </summary>
        /// <param name="param_name">name of the parameter</param>
        /// <param name="parameters">list containing all parameters</param>
        /// <returns>value of the parameter</returns>
        public static string GetParameterValue(string param_name, ArrayList parameters)
        {
            string param_value = "";
            
            int i = 0;
            while ((i < parameters.Count) && (param_value == ""))
            {
                ArrayList param = (ArrayList)parameters[i];
                string name = (string)param[0];
                if (name == param_name)
                {
                    if (param.Count > 1)
                    {
                        if ((param_name != "text") || (param.Count == 2))
                        {
                            param_value = (String)param[1];

                            if (param_name == "text")
                            {
                                if (param_value.IndexOf(",") > -1)
                                {
                                    // convert a list of numbers separated by commas into a string
                                    string[] bytes = param_value.Split(',');
                                    param_value = "";
                                    for (int j = 0; j < bytes.Length; j++)
                                    {
                                        int v = Convert.ToInt32(bytes[j]);
                                        param_value += Convert.ToChar(v);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (param_name == "text")
                            {
                                // convert a list of numbers into a string (separated by spaces)
                                //char[] decoded_text = new char[param.Count - 1];
                                param_value = "";
                                for (int j = 1; j < param.Count; j++)
                                {
                                    int v = Convert.ToInt32((String)param[j]);
                                    param_value += Convert.ToChar(v);
                                    //decoded_text[j - 1] = Convert.ToChar(v);
                                }
                                //param_value = decoded_text.ToString();
                            }
                        }
                    }
                    else
                        param_value = "true";
                }
                i++;
            }
            return (param_value);
        }

        /// <summary>
        /// removes leading and trailing quotation marks from the given string
        /// </summary>
        /// <param name="str">string to be processed</param>
        /// <returns>string with quotations removed</returns>
        private static string Unquote(string str)
        {
            if (str.Length > 2)
            {
                // convert to characters
                char[] ch = str.ToCharArray();
                if (ch[0] == '"') // leading quote
                    if (ch[ch.Length - 1] == '"') //trailing quote
                        str = str.Substring(1, str.Length - 2); // remove quotes
            }

            return (str);
        }

        /// <summary>
        /// parses the command line arguments into a set of parameters
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <param name="switch_character">the character to be used to indicate a switch (typically '-')</param>
        /// <param name="valid_parameter_names">valid parameter names.  If null then all parameters are valid</param>
        /// <returns>list of parameters.  Each list element is an array list typically containing a parameter name and a parameter value</returns>
        public static ArrayList ParseCommandLineParameters(string[] args,
                                                            string switch_character,
                                                            ArrayList valid_parameter_names)
        {
            ArrayList parameters = new ArrayList();

            // default switch
            if (switch_character == "") switch_character = "-";

            if (args != null)
            {
                ArrayList current_parameter = null;

                for (int i = 0; i < args.Length; i++)
                {
                    string s = args[i];
                    bool isSwitch = false;
                    
                    if (s.StartsWith(switch_character))
                    {
                        isSwitch = true;

                        // determine if this is actually just a negative number
                        // rather than a switch
                        if (s.Length > 1)
                        {
                            char first_char = Convert.ToChar(s.Substring(1, 1));
                            if ((first_char >= 48) && (first_char <= 57))
                                isSwitch = false; // first character is a number
                        }
                    }

                    if (isSwitch)
                    {
                        // this is a switch

                        string param = s.Substring(1).ToLower();
                        
                        // is this a valid parameter ?
                        bool isValid = false;
                        if (valid_parameter_names == null)
                            isValid = true;
                        else
                        {
                            if (valid_parameter_names.Contains(param))
                                isValid = true;
                        }

                        if (isValid)
                        {
                            // add the command
                            if (current_parameter != null)
                                parameters.Add(current_parameter);

                            current_parameter = new ArrayList();
                            current_parameter.Add(param);
                        }
                        else
                        {
                            Console.WriteLine("'" + param + "' is not a valid parameter");
                            current_parameter = null;
                        }
                    }
                    else
                    {
                        // add the parameter value
                        if (current_parameter != null)
                        {
                            String param_value = Unquote(args[i]);
                            current_parameter.Add(param_value);
                        }
                    }
                }
                // add the final parameter to the list
                if (current_parameter != null)
                    parameters.Add(current_parameter);
            }

            return (parameters);
        }

        #endregion
		
        #region "spinner animation"

		public static string SpinnerAnimation(ref int state)
		{
			char backspace = (char)8;
			string spinner = "";
			spinner += backspace;
			
            switch(state)
			{
			    case 0: { spinner += "|"; break; }
				case 1: { spinner += "/"; break; }
				case 2: { spinner += "-"; break; }
				case 3: { spinner += @"\"; break; }
			}
			
			state++;
			if (state > 3) state = 0;
			return(spinner);
		}
		
        #endregion

    }
}
