using System;
using System.Collections.Generic;
using System.Text;

namespace mpmist
{
    class metastring
    {
        public string str = "";
        public int length = 0;
        public int bufsize = 0;
        public int free_string_on_destroy = 0;
    }

    /// <summary>
    /// Double metaphone class
    /// Based on C++ code originally written by Maurice Aubrey - maurice@hevanet.com
    /// </summary>
    public class Metaphone
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
			
	    public static void ToMetaphoneStandardised(
		    string text, bool reverse,
		    ref string standardised_index_primary,
		    ref string standardised_index_secondary) 
		{
			standardised_index_primary = "";
			standardised_index_secondary = "";
			text = TextOnly(text);
			text = RemoveCommonWords(text);

			string primary_code="", secondary_code="";
			List<string> snd_primary = new List<string>();
			List<string> snd_secondary = new List<string>();
			string[] str = text.Trim().Split(' ');
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] != "")
				{
					ToDoubleMetaphoneCode(str[i], ref primary_code, ref secondary_code);
					snd_primary.Add(primary_code);
					snd_secondary.Add(secondary_code);
				}
			}
			snd_primary.Sort();
			snd_secondary.Sort();
			if (reverse) 
			{
				snd_primary.Reverse();
				snd_secondary.Reverse();
			}
			for (int i = 0; i < snd_primary.Count; i++)
			{
				standardised_index_primary += snd_primary[i];
				standardised_index_secondary += snd_secondary[i];
			}
		}
		
		
        private static metastring NewMetastring(string init_str)
        {
            metastring s = new metastring();

            if (init_str == null) init_str = "";
            s.length  = init_str.Length;

            // preallocate a bit more for potential growth
            s.bufsize = s.length + 7;
            s.str = init_str;
            s.free_string_on_destroy = 1;
            return (s);
        }


        private static void MakeUpper(metastring s)
        {
            s.str = s.str.ToUpper();
        }

        private static bool IsVowel(metastring s, int pos)
        {
            string c;

            if ((pos < 0) || (pos >= s.length)) return (false);

            c = s.str.Substring(pos, 1);
            if ((c == "A") || (c == "E") || (c == "I") || (c == "O") || (c == "U")  || (c == "Y"))
	            return (true);

            return (false);
        }


        private static bool SlavoGermanic(metastring s)
        {
            if (s.str.IndexOf("W") >= 0)
	            return (true);
            else 
                if (s.str.IndexOf("K") >= 0)
                    return (true);
                else 
                    if (s.str.IndexOf("CZ") >= 0)
                        return (true);
                    else 
                        if (s.str.IndexOf("WITZ") >= 0)
                            return (true);
                        else
                            return (false);
        }


        private static int GetLength(metastring s)
        {
            return (s.length);
        }


        private static char GetAt(metastring s, int pos)
        {
            //if ((pos < 0) || (pos >= s.length))
	        //    return '\0';

            string str = s.str.Substring(pos,1);
            char c = str[0];
            return (c);
        }


        private static void SetAt(metastring s, int pos, char c)
        {
            if ((pos < 0) || (pos >= s.length)) return;
            s.str.Insert(pos, Convert.ToString(c));
        }


        //   Caveats: the START value is 0 based
        private static bool StringAt(metastring s, int start, int length, string[] ap)
        {
            bool found = false;
            int i;

            if (start >= 0)
            {
                string test = s.str.Substring(start, length);

                i = 0;
                while ((!found) && (i < ap.Length))
                {
                    if (ap[i] == test) { found = true; }
                    i++;
                }
            }

            //dispose of the array
            ap = null;

            return (found);
        }


        private static void MetaphAdd(metastring s, string new_str)
        {
            int add_length;

            if (new_str == null) return;

            add_length = new_str.Length;
            s.str += new_str;
            s.length += add_length;
        }


        public static string ToMetaphoneCode(string str)
        {
            string primary_code="", secondary_code = "";
            ToDoubleMetaphoneCode(str, ref primary_code, ref secondary_code);
            return (primary_code);
        }

        public static void ToDoubleMetaphoneCode(
		    string str, 
		    ref string primary_code, 
		    ref string secondary_code)
        {
            int length;
            metastring original;
            metastring primary;
            metastring secondary;
            int current;
            int last;

            current = 0;
            // we need the real length and last prior to padding */
            length = str.Length;
            last = length - 1;
            original = new metastring();
            original.str = str;
            original.length = str.Length;

            // Pad original so we can index beyond end
            original.str += "     ";

            primary = new metastring();
            secondary = new metastring();
            primary.free_string_on_destroy = 0;
            secondary.free_string_on_destroy = 0;

            original.str = original.str.ToUpper();

            // skip these when at start of word
            if (StringAt(original, 0, 2, new string[] { "GN", "KN", "PN", "WR", "PS", "" }))
                current += 1;

            // Initial 'X' is pronounced 'Z' e.g. 'Xavier'
            if (GetAt(original, 0) == 'X')
            {
                MetaphAdd(primary, "S");	// 'Z' maps to 'S'
                MetaphAdd(secondary, "S");
                current += 1;
            }

            // main loop
            while ((primary.length < 4) || (secondary.length < 4))
            {
                if (current >= length)
                    break;

                switch (GetAt(original, current))
                {
                    case 'A':
                    case 'E':
                    case 'I':
                    case 'O':
                    case 'U':
                    case 'Y':
                        {
                            if (current == 0)
                            {
                                // all init vowels now map to 'A'
                                MetaphAdd(primary, "A");
                                MetaphAdd(secondary, "A");
                            }
                            current += 1;
                            break;
                        }

                    case 'B':
                        {
                            // "-mb", e.g", "dumb", already skipped over... 
                            MetaphAdd(primary, "P");
                            MetaphAdd(secondary, "P");

                            if (GetAt(original, current + 1) == 'B')
                                current += 2;
                            else
                                current += 1;
                            break;
                        }

                    case 'Ç':
                        {
                            MetaphAdd(primary, "S");
                            MetaphAdd(secondary, "S");
                            current += 1;
                            break;
                        }

                    case 'C':
                        {
                            // various germanic 
                            if ((current > 1)
                                && !IsVowel(original, current - 2)
                                && StringAt(original, (current - 1), 3, new string[] { "ACH", "" })
                                && ((GetAt(original, current + 2) != 'I')
                                && ((GetAt(original, current + 2) != 'E')
                                || StringAt(original, (current - 2), 6, new string[] { "BACHER", "MACHER", "" }))))
                            {
                                MetaphAdd(primary, "K");
                                MetaphAdd(secondary, "K");
                                current += 2;
                                break;
                            }

                            // special case 'caesar'
                            if ((current == 0)
                                && StringAt(original, current, 6, new string[] { "CAESAR", "" }))
                            {
                                MetaphAdd(primary, "S");
                                MetaphAdd(secondary, "S");
                                current += 2;
                                break;
                            }

                            // italian 'chianti'
                            if (StringAt(original, current, 4, new string[] { "CHIA", "" }))
                            {
                                MetaphAdd(primary, "K");
                                MetaphAdd(secondary, "K");
                                current += 2;
                                break;
                            }

                            if (StringAt(original, current, 2, new string[] { "CH", "" }))
                            {
                                // find 'michael' 
                                if ((current > 0)
                                    && StringAt(original, current, 4, new string[] { "CHAE", "" }))
                                {
                                    MetaphAdd(primary, "K");
                                    MetaphAdd(secondary, "X");
                                    current += 2;
                                    break;
                                }

                                // greek roots e.g. 'chemistry', 'chorus'
                                if ((current == 0)
                                    && (StringAt(original, (current + 1), 5, new string[] { "HARAC", "HARIS", "" })
                                    || StringAt(original, (current + 1), 3, new string[] { "HOR", "HYM", "HIA", "HEM", "" }))
                                    && !StringAt(original, 0, 5, new string[] { "CHORE", "" }))
                                {
                                    MetaphAdd(primary, "K");
                                    MetaphAdd(secondary, "K");
                                    current += 2;
                                    break;
                                }

                                // germanic, greek, or otherwise 'ch' for 'kh' sound 
                                if ((StringAt(original, 0, 4, new string[] { "VAN ", "VON ", "" })
                                    || StringAt(original, 0, 3, new string[] { "SCH", "" }))
                                    //  'architect but not 'arch', 'orchestra', 'orchid' 
                                    || StringAt(original, (current - 2), 6, new string[] { "ORCHES", "ARCHIT", "ORCHID", "" })
                                    || StringAt(original, (current + 2), 1, new string[] { "T", "S", "" })
                                    || ((StringAt(original, (current - 1), 1, new string[] { "A", "O", "U", "E", "" })
                                    || (current == 0))
                                    // e.g., 'wachtler', 'wechsler', but not 'tichner' */
                                    && StringAt(original, (current + 2), 1, new string[] { "L", "R", "N", "M", "B", "H", "F", "V", "W", " ", "" })))
                                {
                                    MetaphAdd(primary, "K");
                                    MetaphAdd(secondary, "K");
                                }
                                else
                                {
                                    if (current > 0)
                                    {
                                        if (StringAt(original, 0, 2, new string[] { "MC", "" }))
                                        {
                                            // e.g., "McHugh" 
                                            MetaphAdd(primary, "K");
                                            MetaphAdd(secondary, "K");
                                        }
                                        else
                                        {
                                            MetaphAdd(primary, "X");
                                            MetaphAdd(secondary, "K");
                                        }
                                    }
                                    else
                                    {
                                        MetaphAdd(primary, "X");
                                        MetaphAdd(secondary, "X");
                                    }
                                }
                                current += 2;
                                break;
                            }
                            // e.g, 'czerny' */
                            if (StringAt(original, current, 2, new string[] { "CZ", "" })
                                && !StringAt(original, (current - 2), 4, new string[] { "WICZ", "" }))
                            {
                                MetaphAdd(primary, "S");
                                MetaphAdd(secondary, "X");
                                current += 2;
                                break;
                            }

                            // e.g., 'focaccia' 
                            if (StringAt(original, (current + 1), 3, new string[] { "CIA", "" }))
                            {
                                MetaphAdd(primary, "X");
                                MetaphAdd(secondary, "X");
                                current += 3;
                                break;
                            }

                            // double 'C', but not if e.g. 'McClellan' 
                            if (StringAt(original, current, 2, new string[] { "CC", "" })
                                && !((current == 1) && (GetAt(original, 0) == 'M')))
                                // 'bellocchio' but not 'bacchus' 
                                if (StringAt(original, (current + 2), 1, new string[] { "I", "E", "H", "" })
                                    && !StringAt(original, (current + 2), 2, new string[] { "HU", "" }))
                                {
                                    // 'accident', 'accede' 'succeed' 
                                    if (
                                        ((current == 1)
                                        && (GetAt(original, current - 1) == 'A'))
                                        || StringAt(original, (current - 1), 5, new string[] { "UCCEE", "UCCES", "" }))
                                    {
                                        MetaphAdd(primary, "KS");
                                        MetaphAdd(secondary, "KS");
                                        // 'bacci', 'bertucci', other italian 
                                    }
                                    else
                                    {
                                        MetaphAdd(primary, "X");
                                        MetaphAdd(secondary, "X");
                                    }
                                    current += 3;
                                    break;
                                }
                                else
                                {	  // Pierce's rule 
                                    MetaphAdd(primary, "K");
                                    MetaphAdd(secondary, "K");
                                    current += 2;
                                    break;
                                }

                            if (StringAt(original, current, 2, new string[] { "CK", "CG", "CQ", "" }))
                            {
                                MetaphAdd(primary, "K");
                                MetaphAdd(secondary, "K");
                                current += 2;
                                break;
                            }

                            if (StringAt(original, current, 2, new string[] { "CI", "CE", "CY", "" }))
                            {
                                // italian vs. english 
                                if (StringAt(original, current, 3, new string[] { "CIO", "CIE", "CIA", "" }))
                                {
                                    MetaphAdd(primary, "S");
                                    MetaphAdd(secondary, "X");
                                }
                                else
                                {
                                    MetaphAdd(primary, "S");
                                    MetaphAdd(secondary, "S");
                                }
                                current += 2;
                                break;
                            }

                            // else 
                            MetaphAdd(primary, "K");
                            MetaphAdd(secondary, "K");

                            // name sent in 'mac caffrey', 'mac gregor 
                            if (StringAt(original, (current + 1), 2, new string[] { " C", " Q", " G", "" }))
                                current += 3;
                            else
                                if (StringAt(original, (current + 1), 1, new string[] { "C", "K", "Q", "" })
                                    && !StringAt(original, (current + 1), 2, new string[] { "CE", "CI", "" }))
                                    current += 2;
                                else
                                    current += 1;
                            break;
                        }
                    case 'D':
                        {
                            if (StringAt(original, current, 2, new string[] { "DG", "" }))
                            {
                                if (StringAt(original, (current + 2), 1, new string[] { "I", "E", "Y", "" }))
                                {
                                    // e.g. 'edge' 
                                    MetaphAdd(primary, "J");
                                    MetaphAdd(secondary, "J");
                                    current += 3;
                                    break;
                                }
                            }
                            else
                            {
                                // e.g. 'edgar' 
                                MetaphAdd(primary, "TK");
                                MetaphAdd(secondary, "TK");
                                current += 2;
                                break;
                            }

                            if (StringAt(original, current, 2, new string[] { "DT", "DD", "" }))
                            {
                                MetaphAdd(primary, "T");
                                MetaphAdd(secondary, "T");
                                current += 2;
                                break;
                            }

                            // else 
                            MetaphAdd(primary, "T");
                            MetaphAdd(secondary, "T");
                            current += 1;
                            break;
                        }

                    case 'F':
                        {
                            if (GetAt(original, current + 1) == 'F')
                                current += 2;
                            else
                                current += 1;
                            MetaphAdd(primary, "F");
                            MetaphAdd(secondary, "F");
                            break;
                        }

                    case 'G':
                        {
                            if (GetAt(original, current + 1) == 'H')
                            {
                                if ((current > 0) && !IsVowel(original, current - 1))
                                {
                                    MetaphAdd(primary, "K");
                                    MetaphAdd(secondary, "K");
                                    current += 2;
                                    break;
                                }

                                if (current < 3)
                                {
                                    // 'ghislane', ghiradelli 
                                    if (current == 0)
                                    {
                                        if (GetAt(original, current + 2) == 'I')
                                        {
                                            MetaphAdd(primary, "J");
                                            MetaphAdd(secondary, "J");
                                        }
                                        else
                                        {
                                            MetaphAdd(primary, "K");
                                            MetaphAdd(secondary, "K");
                                        }
                                        current += 2;
                                        break;
                                    }
                                }
                                // Parker's rule (with some further refinements) - e.g., 'hugh' 
                                if (
                                ((current > 1)
                                 && StringAt(original, (current - 2), 1, new string[] { "B", "H", "D", "" }))
                                    // e.g., 'bough' 
                                || ((current > 2)
                                    && StringAt(original, (current - 3), 1, new string[] { "B", "H", "D", "" }))
                                    // e.g., 'broughton' 
                                || ((current > 3)
                                    && StringAt(original, (current - 4), 1, new string[] { "B", "H", "" })))
                                {
                                    current += 2;
                                    break;
                                }
                                else
                                {
                                    // e.g., 'laugh', 'McLaughlin', 'cough', 'gough', 'rough', 'tough' 
                                    if ((current > 2)
                                    && (GetAt(original, current - 1) == 'U')
                                    && StringAt(original, (current - 3), 1, new string[] { "C", "G", "L", "R", "T", "" }))
                                    {
                                        MetaphAdd(primary, "F");
                                        MetaphAdd(secondary, "F");
                                    }
                                    else if ((current > 0)
                                         && GetAt(original, current - 1) != 'I')
                                    {


                                        MetaphAdd(primary, "K");
                                        MetaphAdd(secondary, "K");
                                    }

                                    current += 2;
                                    break;
                                }
                            }

                            if (GetAt(original, current + 1) == 'N')
                            {
                                if ((current == 1) && IsVowel(original, 0)
                                && !SlavoGermanic(original))
                                {
                                    MetaphAdd(primary, "KN");
                                    MetaphAdd(secondary, "N");
                                }
                                else
                                    // not e.g. 'cagney' 
                                    if (!StringAt(original, (current + 2), 2, new string[] { "EY", "" })
                                        && (GetAt(original, current + 1) != 'Y')
                                        && !SlavoGermanic(original))
                                    {
                                        MetaphAdd(primary, "N");
                                        MetaphAdd(secondary, "KN");
                                    }
                                    else
                                    {
                                        MetaphAdd(primary, "KN");
                                        MetaphAdd(secondary, "KN");
                                    }
                                current += 2;
                                break;
                            }

                            // 'tagliaro' 
                            if (StringAt(original, (current + 1), 2, new string[] { "LI", "" })
                                && !SlavoGermanic(original))
                            {
                                MetaphAdd(primary, "KL");
                                MetaphAdd(secondary, "L");
                                current += 2;
                                break;
                            }

                            // -ges-,-gep-,-gel-, -gie- at beginning 
                            if ((current == 0)
                                && ((GetAt(original, current + 1) == 'Y')
                                || StringAt(original, (current + 1), 2, new string[] { "ES", "EP", "EB", "EL", "EY", "IB", "IL", "IN", "IE", "EI", "ER", "" })))
                            {
                                MetaphAdd(primary, "K");
                                MetaphAdd(secondary, "J");
                                current += 2;
                                break;
                            }

                            //  -ger-,  -gy- 
                            if (
                                (StringAt(original, (current + 1), 2, new string[] { "ER", "" })
                                 || (GetAt(original, current + 1) == 'Y'))
                                && !StringAt(original, 0, 6, new string[] { "DANGER", "RANGER", "MANGER", "" })
                                && !StringAt(original, (current - 1), 1, new string[] { "E", "I", "" })
                                && !StringAt(original, (current - 1), 3, new string[] { "RGY", "OGY", "" }))
                            {
                                MetaphAdd(primary, "K");
                                MetaphAdd(secondary, "J");
                                current += 2;
                                break;
                            }

                            //  italian e.g, 'biaggi' 
                            if (StringAt(original, (current + 1), 1, new string[] { "E", "I", "Y", "" })
                                || StringAt(original, (current - 1), 4, new string[] { "AGGI", "OGGI", "" }))
                            {
                                // obvious germanic 
                                if (
                                (StringAt(original, 0, 4, new string[] { "VAN ", "VON ", "" })
                                 || StringAt(original, 0, 3, new string[] { "SCH", "" }))
                                || StringAt(original, (current + 1), 2, new string[] { "ET", "" }))
                                {
                                    MetaphAdd(primary, "K");
                                    MetaphAdd(secondary, "K");
                                }
                                else
                                {
                                    /* always soft if french ending */
                                    if (StringAt(original, (current + 1), 4, new string[] { "IER ", "" }))
                                    {
                                        MetaphAdd(primary, "J");
                                        MetaphAdd(secondary, "J");
                                    }
                                    else
                                    {
                                        MetaphAdd(primary, "J");
                                        MetaphAdd(secondary, "K");
                                    }
                                }
                                current += 2;
                                break;
                            }

                            if (GetAt(original, current + 1) == 'G')
                                current += 2;
                            else
                                current += 1;
                            MetaphAdd(primary, "K");
                            MetaphAdd(secondary, "K");
                            break;
                        }

                    case 'H':
                        {
                            // only keep if first & before vowel or btw. 2 vowels 
                            if (((current == 0) || IsVowel(original, current - 1))
                                && IsVowel(original, current + 1))
                            {
                                MetaphAdd(primary, "H");
                                MetaphAdd(secondary, "H");
                                current += 2;
                            }
                            else		// also takes care of 'HH' 
                                current += 1;
                            break;
                        }

                    case 'J':
                        {
                            // obvious spanish, 'jose', 'san jacinto' 
                            if (StringAt(original, current, 4, new string[] { "JOSE", "" })
                                || StringAt(original, 0, 4, new string[] { "SAN ", "" }))
                            {
                                if (((current == 0)
                                 && (GetAt(original, current + 4) == ' '))
                                || StringAt(original, 0, 4, new string[] { "SAN ", "" }))
                                {
                                    MetaphAdd(primary, "H");
                                    MetaphAdd(secondary, "H");
                                }
                                else
                                {
                                    MetaphAdd(primary, "J");
                                    MetaphAdd(secondary, "H");
                                }
                                current += 1;
                                break;
                            }

                            if ((current == 0)
                                && !StringAt(original, current, 4, new string[] { "JOSE", "" }))
                            {
                                MetaphAdd(primary, "J");	// Yankelovich/Jankelowicz 
                                MetaphAdd(secondary, "A");
                            }
                            else
                            {
                                // spanish pron. of e.g. 'bajador' 
                                if (IsVowel(original, current - 1)
                                && !SlavoGermanic(original)
                                && ((GetAt(original, current + 1) == 'A')
                                    || (GetAt(original, current + 1) == 'O')))
                                {
                                    MetaphAdd(primary, "J");
                                    MetaphAdd(secondary, "H");
                                }
                                else
                                {
                                    if (current == last)
                                    {
                                        MetaphAdd(primary, "J");
                                        MetaphAdd(secondary, "");
                                    }
                                    else
                                    {
                                        if (!StringAt(original, (current + 1), 1, new string[] { "L", "T", "K", "S", "N", "M", "B", "Z", "" })
                                            && !StringAt(original, (current - 1), 1,
                                                 new string[] { "S", "K", "L", "" }))
                                        {
                                            MetaphAdd(primary, "J");
                                            MetaphAdd(secondary, "J");
                                        }
                                    }
                                }
                            }

                            if (GetAt(original, current + 1) == 'J')	// it could happen! 
                                current += 2;
                            else
                                current += 1;
                            break;
                        }

                    case 'K':
                        {
                            if (GetAt(original, current + 1) == 'K')
                                current += 2;
                            else
                                current += 1;
                            MetaphAdd(primary, "K");
                            MetaphAdd(secondary, "K");
                            break;
                        }

                    case 'L':
                        {
                            if (GetAt(original, current + 1) == 'L')
                            {
                                // spanish e.g. 'cabrillo', 'gallegos' 
                                if (((current == (length - 3))
                                 && StringAt(original, (current - 1), 4, new string[] { "ILLO", "ILLA", "ALLE", "" }))
                                || ((StringAt(original, (last - 1), 2, new string[] { "AS", "OS", "" })
                                  || StringAt(original, last, 1, new string[] { "A", "O", "" }))
                                 && StringAt(original, (current - 1), 4, new string[] { "ALLE", "" })))
                                {
                                    MetaphAdd(primary, "L");
                                    MetaphAdd(secondary, "");
                                    current += 2;
                                    break;
                                }
                                current += 2;
                            }
                            else
                                current += 1;
                            MetaphAdd(primary, "L");
                            MetaphAdd(secondary, "L");
                            break;
                        }

                    case 'M':
                        {
                            if ((StringAt(original, (current - 1), 3, new string[] { "UMB", "" })
                                 && (((current + 1) == last)
                                 || StringAt(original, (current + 2), 2, new string[] { "ER", "" })))
                                // 'dumb','thumb' 
                                || (GetAt(original, current + 1) == 'M'))
                                current += 2;
                            else
                                current += 1;
                            MetaphAdd(primary, "M");
                            MetaphAdd(secondary, "M");
                            break;
                        }

                    case 'N':
                        {
                            if (GetAt(original, current + 1) == 'N')
                                current += 2;
                            else
                                current += 1;
                            MetaphAdd(primary, "N");
                            MetaphAdd(secondary, "N");
                            break;
                        }

                    case 'Ñ':
                        {
                            current += 1;
                            MetaphAdd(primary, "N");
                            MetaphAdd(secondary, "N");
                            break;
                        }

                    case 'P':
                        {
                            if (GetAt(original, current + 1) == 'H')
                            {
                                MetaphAdd(primary, "F");
                                MetaphAdd(secondary, "F");
                                current += 2;
                                break;
                            }

                            // also account for "campbell", "raspberry" 
                            if (StringAt(original, (current + 1), 1, new string[] { "P", "B", "" }))
                                current += 2;
                            else
                                current += 1;
                            MetaphAdd(primary, "P");
                            MetaphAdd(secondary, "P");
                            break;
                        }

                    case 'Q':
                        {
                            if (GetAt(original, current + 1) == 'Q')
                                current += 2;
                            else
                                current += 1;
                            MetaphAdd(primary, "K");
                            MetaphAdd(secondary, "K");
                            break;
                        }

                    case 'R':
                        {
                            // french e.g. 'rogier', but exclude 'hochmeier' 
                            if ((current == last)
                                && !SlavoGermanic(original)
                                && StringAt(original, (current - 2), 2, new string[] { "IE", "" })
                                && !StringAt(original, (current - 4), 2, new string[] { "ME", "MA", "" }))
                            {
                                MetaphAdd(primary, "");
                                MetaphAdd(secondary, "R");
                            }
                            else
                            {
                                MetaphAdd(primary, "R");
                                MetaphAdd(secondary, "R");
                            }

                            if (GetAt(original, current + 1) == 'R')
                                current += 2;
                            else
                                current += 1;
                            break;
                        }

                    case 'S':
                        {
                            // special cases 'island', 'isle', 'carlisle', 'carlysle' 
                            if (StringAt(original, (current - 1), 3, new string[] { "ISL", "YSL", "" }))
                            {
                                current += 1;
                                break;
                            }

                            // special case 'sugar-' 
                            if ((current == 0)
                                && StringAt(original, current, 5, new string[] { "SUGAR", "" }))
                            {
                                MetaphAdd(primary, "X");
                                MetaphAdd(secondary, "S");
                                current += 1;
                                break;
                            }

                            if (StringAt(original, current, 2, new string[] { "SH", "" }))
                            {
                                // germanic 
                                if (StringAt(original, (current + 1), 4, new string[] { "HEIM", "HOEK", "HOLM", "HOLZ", "" }))
                                {
                                    MetaphAdd(primary, "S");
                                    MetaphAdd(secondary, "S");
                                }
                                else
                                {
                                    MetaphAdd(primary, "X");
                                    MetaphAdd(secondary, "X");
                                }
                                current += 2;
                                break;
                            }

                            // italian & armenian 
                            if (StringAt(original, current, 3, new string[] { "SIO", "SIA", "" })
                                || StringAt(original, current, 4, new string[] { "SIAN", "" }))
                            {
                                if (!SlavoGermanic(original))
                                {
                                    MetaphAdd(primary, "S");
                                    MetaphAdd(secondary, "X");
                                }
                                else
                                {
                                    MetaphAdd(primary, "S");
                                    MetaphAdd(secondary, "S");
                                }
                                current += 3;
                                break;
                            }

                            // german & anglicisations, e.g. 'smith' match 'schmidt', 'snider' match 'schneider' 
                            //   also, -sz- in slavic language altho in hungarian it is pronounced 's' 
                            if (((current == 0)
                                 && StringAt(original, (current + 1), 1, new string[] { "M", "N", "L", "W", "" }))
                                || StringAt(original, (current + 1), 1, new string[] { "Z", "" }))
                            {
                                MetaphAdd(primary, "S");
                                MetaphAdd(secondary, "X");
                                if (StringAt(original, (current + 1), 1, new string[] { "Z", "" }))
                                    current += 2;
                                else
                                    current += 1;
                                break;
                            }

                            if (StringAt(original, current, 2, new string[] { "SC", "" }))
                            {
                                // Schlesinger's rule 
                                if (GetAt(original, current + 2) == 'H')
                                    // dutch origin, e.g. 'school', 'schooner' 
                                    if (StringAt(original, (current + 3), 2, new string[] { "OO", "ER", "EN", "UY", "ED", "EM", "" }))
                                    {
                                        // 'schermerhorn', 'schenker' 
                                        if (StringAt(original, (current + 3), 2, new string[] { "ER", "EN", "" }))
                                        {
                                            MetaphAdd(primary, "X");
                                            MetaphAdd(secondary, "SK");
                                        }
                                        else
                                        {
                                            MetaphAdd(primary, "SK");
                                            MetaphAdd(secondary, "SK");
                                        }
                                        current += 3;
                                        break;
                                    }
                                    else
                                    {
                                        if ((current == 0) && !IsVowel(original, 3)
                                            && (GetAt(original, 3) != 'W'))
                                        {
                                            MetaphAdd(primary, "X");
                                            MetaphAdd(secondary, "S");
                                        }
                                        else
                                        {
                                            MetaphAdd(primary, "X");
                                            MetaphAdd(secondary, "X");
                                        }
                                        current += 3;
                                        break;
                                    }

                                if (StringAt(original, (current + 2), 1, new string[] { "I", "E", "Y", "" }))
                                {
                                    MetaphAdd(primary, "S");
                                    MetaphAdd(secondary, "S");
                                    current += 3;
                                    break;
                                }
                                // else 
                                MetaphAdd(primary, "SK");
                                MetaphAdd(secondary, "SK");
                                current += 3;
                                break;
                            }

                            // french e.g. 'resnais', 'artois' 
                            if ((current == last)
                                && StringAt(original, (current - 2), 2, new string[] { "AI", "OI", "" }))
                            {
                                MetaphAdd(primary, "");
                                MetaphAdd(secondary, "S");
                            }
                            else
                            {
                                MetaphAdd(primary, "S");
                                MetaphAdd(secondary, "S");
                            }

                            if (StringAt(original, (current + 1), 1, new string[] { "S", "Z", "" }))
                                current += 2;
                            else
                                current += 1;
                            break;
                        }

                    case 'T':
                        {
                            if (StringAt(original, current, 4, new string[] { "TION", "" }))
                            {
                                MetaphAdd(primary, "X");
                                MetaphAdd(secondary, "X");
                                current += 3;
                                break;
                            }

                            if (StringAt(original, current, 3, new string[] { "TIA", "TCH", "" }))
                            {
                                MetaphAdd(primary, "X");
                                MetaphAdd(secondary, "X");
                                current += 3;
                                break;
                            }

                            if (StringAt(original, current, 2, new string[] { "TH", "" })
                                || StringAt(original, current, 3, new string[] { "TTH", "" }))
                            {
                                // special case 'thomas', 'thames' or germanic 
                                if (StringAt(original, (current + 2), 2, new string[] { "OM", "AM", "" })
                                || StringAt(original, 0, 4, new string[] { "VAN ", "VON ", "" })
                                || StringAt(original, 0, 3, new string[] { "SCH", "" }))
                                {
                                    MetaphAdd(primary, "T");
                                    MetaphAdd(secondary, "T");
                                }
                                else
                                {
                                    MetaphAdd(primary, "0"); // yes, zero 
                                    MetaphAdd(secondary, "T");
                                }
                                current += 2;
                                break;
                            }

                            if (StringAt(original, (current + 1), 1, new string[] { "T", "D", "" }))
                                current += 2;
                            else
                                current += 1;
                            MetaphAdd(primary, "T");
                            MetaphAdd(secondary, "T");
                            break;
                        }

                    case 'V':
                        {
                            if (GetAt(original, current + 1) == 'V')
                                current += 2;
                            else
                                current += 1;
                            MetaphAdd(primary, "F");
                            MetaphAdd(secondary, "F");
                            break;
                        }

                    case 'W':
                        {
                            // can also be in middle of word 
                            if (StringAt(original, current, 2, new string[] { "WR", "" }))
                            {
                                MetaphAdd(primary, "R");
                                MetaphAdd(secondary, "R");
                                current += 2;
                                break;
                            }

                            if ((current == 0)
                                && (IsVowel(original, current + 1)
                                || StringAt(original, current, 2, new string[] { "WH", "" })))
                            {
                                // Wasserman should match Vasserman 
                                if (IsVowel(original, current + 1))
                                {
                                    MetaphAdd(primary, "A");
                                    MetaphAdd(secondary, "F");
                                }
                                else
                                {
                                    // need Uomo to match Womo 
                                    MetaphAdd(primary, "A");
                                    MetaphAdd(secondary, "A");
                                }
                            }

                            // Arnow should match Arnoff 
                            if (((current == last) && IsVowel(original, current - 1))
                                || StringAt(original, (current - 1), 5, new string[] { "EWSKI", "EWSKY", "OWSKI", "OWSKY", "" })
                                || StringAt(original, 0, 3, new string[] { "SCH", "" }))
                            {
                                MetaphAdd(primary, "");
                                MetaphAdd(secondary, "F");
                                current += 1;
                                break;
                            }

                            // polish e.g. 'filipowicz' 
                            if (StringAt(original, current, 4, new string[] { "WICZ", "WITZ", "" }))
                            {
                                MetaphAdd(primary, "TS");
                                MetaphAdd(secondary, "FX");
                                current += 4;
                                break;
                            }

                            // else skip it 
                            current += 1;
                            break;
                        }

                    case 'X':
                        {
                            // french e.g. breaux 
                            if (!((current == last)
                                  && (StringAt(original, (current - 3), 3, new string[] { "IAU", "EAU", "" })
                                   || StringAt(original, (current - 2), 2, new string[] { "AU", "OU", "" }))))
                            {
                                MetaphAdd(primary, "KS");
                                MetaphAdd(secondary, "KS");
                            }


                            if (StringAt(original, (current + 1), 1, new string[] { "C", "X", "" }))
                                current += 2;
                            else
                                current += 1;
                            break;
                        }

                    case 'Z':
                        {
                            // chinese pinyin e.g. 'zhao' 
                            if (GetAt(original, current + 1) == 'H')
                            {
                                MetaphAdd(primary, "J");
                                MetaphAdd(secondary, "J");
                                current += 2;
                                break;
                            }
                            else if (StringAt(original, (current + 1), 2, new string[] { "ZO", "ZI", "ZA", "" })
                                || (SlavoGermanic(original)
                                    && ((current > 0)
                                    && GetAt(original, current - 1) != 'T')))
                            {
                                MetaphAdd(primary, "S");
                                MetaphAdd(secondary, "TS");
                            }
                            else
                            {
                                MetaphAdd(primary, "S");
                                MetaphAdd(secondary, "S");
                            }

                            if (GetAt(original, current + 1) == 'Z')
                                current += 2;
                            else
                                current += 1;

                            break;
                        }

                    default:
                        {
                            current += 1;
                            break;
                        }
                }
            }


            if (primary.length > 4)
                SetAt(primary, 4, '\0');

            if (secondary.length > 4)
                SetAt(secondary, 4, '\0');

            primary_code = primary.str.ToLower();
            secondary_code = secondary.str.ToLower();
        }

    }
}
