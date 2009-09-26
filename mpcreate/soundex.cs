/*
    soundex
    Copyright (C) 2002 Jeff Guitard

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
using System.Text;

namespace ca.guitard.jeff.utility {

  /// <summary>
  /// Utility class for performing soundex algorithm.
  /// 
  /// The Soundex algorithm is used to convert a word to a
  /// code based upon the phonetic sound of the word.
  /// 
  /// The soundex algorithm is outlined below:
  ///     Rule 1. Keep the first character of the name.
  ///     Rule 2. Perform a transformation on each remaining characters:
  ///                 A,E,I,O,U,Y     = A
  ///                 H,W             = S
  ///                 B,F,P,V         = 1
  ///                 C,G,J,K,Q,S,X,Z = 2
  ///                 D,T             = 3
  ///                 L               = 4
  ///                 M,N             = 5
  ///                 R               = 6
  ///     Rule 3. If a character is the same as the previous, do not include in the code.
  ///     Rule 4. If character is "A" or "S" do not include in the code.
  ///     Rule 5. If a character is blank, then do not include in the code.
  ///     Rule 6. A soundex code must be exactly 4 characters long.  If the
  ///             code is too short then pad with zeros, otherwise truncate.
  /// 
  /// Jeff Guitard
  /// October 2002
  /// </summary>

  public class Soundex {

    private Soundex() {
    }

	private static string RemoveShortWords(
	    string text, 
	    int minimum_percent)
	{
		string result = "";
		
		string[] str = text.Split(' ');
		int average_length = 0;
		for (int i = 0; i < str.Length; i++)
		{
			average_length += str[i].Length;
		}
		if (str.Length > 0) average_length /= str.Length;
		average_length = average_length * 2 * minimum_percent / 100;
		for (int i = 0; i < str.Length; i++)
		{
			if (str[i].Length > average_length)
				result += str[i] + " ";
		}			
		return(result.Trim());
	}
		
    public static string ToSoundexStandardised(string text, bool reverse) {
		string standardised_index = "";
		text = RemoveShortWords(text, 50);
		
		List<string> snd = new List<string>();
		string[] str = text.Trim().Split(' ');
		for (int i = 0; i < str.Length; i++)
		{
			if (str[i] != "")
			{
				snd.Add(ToSoundexCode(str[i]));
			}
		}
		snd.Sort();
		if (reverse) snd.Reverse();
		for (int i = 0; i < snd.Count; i++)
			standardised_index += snd[i];
		return (standardised_index);
	}
		
    /// <summary>
    /// Return the soundex code for a given string.
    /// </summary>
    public static string ToSoundexCode(string aString) {

      String word = aString.ToUpper();
      StringBuilder soundexCode = new StringBuilder();
      int wordLength = word.Length;

      // Rule 1. Keep the first character of the word
      soundexCode.Append(word.Substring(0,1));

      // Rule 2. Perform a transformation on each remaining characters
      for (int i=1; i<wordLength; i++) {
        String transformedChar = Transform(word.Substring(i,1));

        // Rule 3. If a character is the same as the previous, do not include in code
        if (!transformedChar.Equals( soundexCode.ToString().Substring(soundexCode.Length - 1) )) {

          // Rule 4. If character is "A" or "S" do not include in code
          if (!transformedChar.Equals("A") && !transformedChar.Equals("S")) { 

            // Rule 5. If a character is blank, then do not include in code 
            if (!transformedChar.Equals(" ")) {
              soundexCode.Append(transformedChar);
            }
          }  
        }
      }

      // Rule 6. A soundex code must be exactly 4 characters long.  If the
      //         code is too short then pad with zeros, otherwise truncate.
      soundexCode.Append("0000");

      return soundexCode.ToString().Substring(0,4);
    }

    /// <summary>
    /// Transform the A-Z alphabetic characters to the appropriate soundex code.
    /// </summary>
    private static string Transform(string aString) {
      
      switch (aString) {
        case "A":
        case "E":
        case "I":
        case "O":
        case "U":
        case "Y":
          return "A";
        case "H":
        case "W":
          return "S";
        case "B":
        case "F":
        case "P":
        case "V":
          return "1";
        case "C":
        case "G":
        case "J":
        case "K":
        case "Q":
        case "S":
        case "X":
        case "Z":
          return "2";
        case "D":
        case "T":
          return "3";
        case "L":
          return "4";
        case "M":
        case "N":
          return "5";
        case "R":
          return "6";
      }

      return " ";  
    }
  }
}

