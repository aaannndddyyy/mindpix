using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace mpcreate
{
	/// <summary>
	/// New York State Identification and Intelligence System (NYSIIS) Phonetic Encoder
	/// </summary>
	public class NYSIIS
	{
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
			
	    public static string ToNYSIISStandardised(string text, bool reverse) {
			string standardised_index = "";
			text = TextOnly(text);
			text = RemoveCommonWords(text);
			
			List<string> snd = new List<string>();
			string[] str = text.Trim().Split(' ');
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] != "")
				{
					snd.Add(ToNYSIISCode(str[i]));
				}
			}
			snd.Sort();
			if (reverse) snd.Reverse();
			for (int i = 0; i < snd.Count; i++)
				standardised_index += snd[i];
			return (standardised_index);
		}
				
		public static string ToNYSIISCode(string name)
		{
			int inNameCharOffset = 0;
			int stopSearchforSZ = 0;
			int maxLength = 200;
			int prevCharValue = 0;
			char firstChar = ' ';
			char[] inNameArray = new char[maxLength];
			char[] phonKeyArray = new char[maxLength];
			bool prependVowel = false;

			StringBuilder MyStringBuilder = new StringBuilder(" ");

			name = name.ToUpper();
			
			// copy inName to char array
			name.CopyTo(0, inNameArray, 0, name.Length);

			firstChar = inNameArray[0];

			// remove all 'S' and 'Z' chars from the end of the surname
			for(inNameCharOffset = (name.Length - 1); ((inNameCharOffset >= 1) && (stopSearchforSZ == 0)); inNameCharOffset--)
			{
				if(inNameArray[inNameCharOffset] == 'S' ||
					inNameArray[inNameCharOffset] == 'Z')
				{
					inNameArray[inNameCharOffset] = ' ';
				}
				else
				{
					stopSearchforSZ = 1;
				}
			}

			// copy char array to string
			name = new String(inNameArray, 0, name.Length);  
			MyStringBuilder = new StringBuilder(name);

			MyStringBuilder.Replace(" ", "");

			name = String.Format(MyStringBuilder + "");
			MyStringBuilder = new StringBuilder(name);
			
			// transcode initial strings MAC => MC 
			if(MyStringBuilder.Length > 2)
			{
				MyStringBuilder.Replace("MAC", "MC", 0, 3);
			}

			// transcode initial strings PF => F 

			if(MyStringBuilder.Length > 1)
			{
				MyStringBuilder.Replace("PF", "F", 0, 2);
			}

			// Transcode trailing strings as follows,	
			// IX => IC 
			// EX => EC 
			// YE,EE,IE => Y 
			// NT,ND => D 

			if(MyStringBuilder.Length > 1)
			{
				MyStringBuilder.Replace("IX", "IC", (MyStringBuilder.Length - 2), 2);
				MyStringBuilder.Replace("EX", "EC", (MyStringBuilder.Length - 2), 2);
				MyStringBuilder.Replace("YE", "Y", (MyStringBuilder.Length - 2), 2);
				MyStringBuilder.Replace("EE", "Y", (MyStringBuilder.Length - 2), 2);
				MyStringBuilder.Replace("IE", "Y", (MyStringBuilder.Length - 2), 2);
				MyStringBuilder.Replace("NT", "D", (MyStringBuilder.Length - 2), 2);
				MyStringBuilder.Replace("ND", "D", (MyStringBuilder.Length - 2), 2);
			}

			// transcode 'EV' to 'EF' if not at start of name

			if(MyStringBuilder.Length > 1)
			{
				MyStringBuilder.Replace("EV", "EF", 1, (MyStringBuilder.Length - 1));
			}

			// remove any 'W' that follows a vowel

			MyStringBuilder.Replace("AW", "A");
			MyStringBuilder.Replace("EW", "E");
			MyStringBuilder.Replace("IW", "I");
			MyStringBuilder.Replace("OW", "O");
			MyStringBuilder.Replace("UW", "U");

			// replace all vowels with 'A'

			MyStringBuilder.Replace("A", "A");
			MyStringBuilder.Replace("E", "A");
			MyStringBuilder.Replace("I", "A");
			MyStringBuilder.Replace("O", "A");
			MyStringBuilder.Replace("U", "A");

			// transcode 'GHT' to 'GT'

			MyStringBuilder.Replace("GHT", "GT");

			// transcode 'DG' to 'G'

			MyStringBuilder.Replace("DG", "G");

			// transcode 'PH' to 'F'

			MyStringBuilder.Replace("PH", "F");

			// if not first character, eliminate all 'H' preceded or followed by a vowel

			MyStringBuilder.Replace("AH", "A");
			if(MyStringBuilder.Length > 2)
			{
				MyStringBuilder.Replace("HA", "A", 1, (MyStringBuilder.Length - 1));
			}

			// change 'KN' to 'N', else 'K' to 'C'

			MyStringBuilder.Replace("KN", "N");
			MyStringBuilder.Replace("K", "C");

			// if not first character, change 'M' to 'N'

			if(MyStringBuilder.Length > 1)
			{
				MyStringBuilder.Replace("M", "N", 1, (MyStringBuilder.Length - 1));
			}

			// if not first character, change 'Q' to 'G'

			if(MyStringBuilder.Length > 1)
			{
				MyStringBuilder.Replace("Q", "G", 1, (MyStringBuilder.Length - 1));
			}

			// transcode 'SH' to 'S'

			MyStringBuilder.Replace("SH", "S");

			// transcode 'SCH' to 'S'

			MyStringBuilder.Replace("SCH", "S");

			// transcode 'YW' to 'Y'

			MyStringBuilder.Replace("YW", "Y");

			// if not first or last character, change 'Y' to 'A'

			if(MyStringBuilder.Length > 2)
			{
				MyStringBuilder.Replace("Y", "A", 1, (MyStringBuilder.Length - 2));
			}

			// transcode 'WR' to 'R'

			MyStringBuilder.Replace("WR", "R");

			// if not first character, change 'Z' to 'S'

			if(MyStringBuilder.Length > 1)
			{
				MyStringBuilder.Replace("Z", "S", 1, (MyStringBuilder.Length - 1));
			}

			// transcode terminal 'AY' to 'Y'

			if(MyStringBuilder.Length > 2)
			{
				MyStringBuilder.Replace("AY", "Y", 1, (MyStringBuilder.Length - 1));
			}

			name = String.Format(MyStringBuilder + "");
			
			// copy inName to char array
			name.CopyTo(0, inNameArray, 0, name.Length);   
			stopSearchforSZ = 0;

			// remove traling vowels
			for(inNameCharOffset = (name.Length - 1); ((inNameCharOffset >= 1) && (stopSearchforSZ == 0)); inNameCharOffset--)
			{
				if(inNameArray[inNameCharOffset] == 'A')
				{
					inNameArray[inNameCharOffset] = ' ';
				}
				else					
				{
					stopSearchforSZ = 1;
				}
			}

			// copy char array to string
			name = new String(inNameArray, 0, MyStringBuilder.Length);  
			MyStringBuilder = new StringBuilder(name);

			MyStringBuilder.Replace(" ", "");

			name = String.Format(MyStringBuilder + "");
			
			// copy inName to char array
			name.CopyTo(0, inNameArray, 0, name.Length);   

			// collapse all strings of repeated characters
			// (also removes non-alpha chars)

			for(inNameCharOffset = 0; inNameCharOffset < name.Length; inNameCharOffset++)
			{
				if(prevCharValue == inNameArray[inNameCharOffset] ||
					inNameArray[inNameCharOffset] < 'A' ||
					inNameArray[inNameCharOffset] > 'Z')
				{
					inNameArray[inNameCharOffset] = ' ';
				}
				else
				{
					prevCharValue = inNameArray[inNameCharOffset];
				}
			}

			// if first char of original surname is a vowel, prepend to code (or replace initial transcoded 'A')

			if(firstChar == 'A' ||
				firstChar == 'E' ||
				firstChar == 'I' ||
				firstChar == 'O' ||
				firstChar == 'U')
			{
				if(inNameArray[0] == 'A')
				{
					inNameArray[0] = firstChar;
				}
				else
				{
					prependVowel = true;
				}
			}

			// copy char array to string
			name = new String(inNameArray, 0, name.Length);  

			MyStringBuilder = new StringBuilder(name);

			MyStringBuilder.Replace(" ", "");

			if(prependVowel)
			{
				name = String.Format(firstChar.ToString() + MyStringBuilder + "");
			}
			else
			{
				name = String.Format(MyStringBuilder + "");
			}

			return(name);
		}
	}
}
