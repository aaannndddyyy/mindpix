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
	
    static string[] common_words = {
        "a",
        "an",
        "am",
        "at",
        "to",
        "as",
        "we",
        "i",
        "in",
        "is",
        "it",
        "if",
        "be",
        "by",
        "so",
        "no",
        "last",
        "first",
        "on",
        "of",
        "its",
        "all",
        "can",
        "into",
        "from",
        "just",
        "and",
        "the",
        "over",
        "under",
        "for",
        "then",
        "dont",
        "has",
        "get",
        "got",
        "had",
        "should",
        "hadnt",
        "have",
        "some",
        "come",
        "this",
        "call",
        "that",
        "thats",
        "find",
        "these",
        "them",
        "look",
        "looked",
        "looks",
        "with",
        "but",
        "about",
        "where",
        "possible",
        "sometimes",
        "which",
        "they",
        "just",
        "we",
        "while",
        "whilst",
        "their",
        "perhaps",
        "you",
        "make",
        "any",
        "say",
        "been",
        "like",
        "form",
        "our",
        "give",
        "in the",
        "in a",
        "will",
        "object",
        "shall",
        "will not",
        "until",
        "take",
        "other",
        "now",
        "lead",
        "taken",
        "you can",
        "have to",
        "have some",
        "would",
        "said",
        "one",
        "how",
        "new",
        "we",
        "said",
        "it",
        "was",
        "are",
        "every",
        "such",
        "more",
        "different",
        "example",
        "way",
        "only",
        "often",
        "show",
        "group",
        "itself",
        "part",
        "saw",
        "making",
        "could",
        "need",
        "out",
        "being",
        "been",
        "yet",
        "lack",
        "even",
        "own",
        "much",
        "of this",
        "become",
        "keep",
        "keeps",
        "do",
        "having",
        "normal",
        "this",
        "after",
        "before",
        "during",
        "off",
        "use",
        "same",
        "case",
        "there",
        "through",
        "end",
        "may",
        "made",
        "name",
        "most",
        "many",
        "well",
        "who",
        "is",
        "your",
        "you",
        "owner",
        "around",
        "about",
        "of",
        "process",
        "too",
        "my",
        "why",
        "tell",
        "he",
        "she",
        "what",
        "left",
        "him",
        "her",
        "ever",
        "there"
    };
		
	private static string TextOnly(
	    string text)
	{
		string result = "";
		char[] ch = text.ToCharArray();
		for (int i = 0; i < text.Length; i++)
		{
			if (((ch[i] >= 'a') &&
				(ch[i] <= 'z')) ||
				((ch[i] >= 'A') &&
				(ch[i] <= 'Z')) ||
                ((ch[i] >= '0') &&
				(ch[i] <= '9')) ||
                (ch[i] == ' '))
			result += ch[i];
		}
		return(result);
	}
		
    private static string RemoveCommonWords(
        string text)
    {
		string result = "";
		text = TextOnly(text);
		text = text.ToLower();
			
		string[] str = text.Split(' ');
		
		for (int i = 0; i < str.Length; i++)
		{
			if (str[i] != "")
			{
				if (Array.IndexOf(common_words, str[i]) == -1)
					result += str[i] + " ";
			}
		}
			
		return(result.Trim());
	}
		
    public static string ToSoundexStandardised(string text, bool reverse) {
		string standardised_index = "";
		text = TextOnly(text);
		text = RemoveCommonWords(text);
		
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

