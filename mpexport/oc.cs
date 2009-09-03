/*
    OpenCog export functions
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

namespace mpexport
{	
	public class oc
	{
        public static void SaveXMLlink(
		    StreamWriter oWrite,
		    string link_type,
		    float strength,
		    float confidence,
		    string object1,
		    string object2,
		    int indent_level)
		{
            string str = "";
			char tab = (char)9;
			for (int i = 0; i < indent_level; i++) str += tab;
			str += "<" + link_type + " strength=" + '"' + strength.ToString() + '"' + " confidence=" + '"' + confidence.ToString() + '"' + ">";
			oWrite.WriteLine(str);
			str="";
			for (int i = 0; i < indent_level+1; i++) str += tab;
            str += "<Element class=" + '"' + "ConceptNode" + '"' + " name=" + '"' + object1 + '"' + "/>";
			oWrite.WriteLine(str);
			str="";
			for (int i = 0; i < indent_level+1; i++) str += tab;			
            str += "<Element class=" + '"' + "ConceptNode" + '"' + " name=" + '"' + object2 + '"' + "/>";
			oWrite.WriteLine(str);
			str="";
			for (int i = 0; i < indent_level; i++) str += tab;
            str += "</" + link_type + ">";
			oWrite.WriteLine(str);
		}
	}
}
