/*
    Converts words and sentences into phonemes and permits phoneme based comparissons of text
    Copyright (C) 2009 Bob Mottram

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
using System.Text.RegularExpressions;

namespace mpgac
{	
	public class phoneme
	{
		/*
		**	English to Phoneme rules.
		**
		**	Derived from: 
		**
		**	     AUTOMATIC TRANSLATION OF ENGLISH TEXT TO PHONETICS
		**	            BY MEANS OF LETTER-TO-SOUND RULES
		**
		**			NRL Report 7948
		**
		**		      January 21st, 1976
		**	    Naval Research Laboratory, Washington, D.C.
		**
		**
		**	Published by the National Technical Information Service as
		**	document "AD/A021 929".
		**
		**
		**
		**	The Phoneme codes:
		**
		**		IY	bEEt		IH	bIt
		**		EY	gAte		EH	gEt
		**		AE	fAt		AA	fAther
		**		AO	lAWn		OW	lOne
		**		UH	fUll		UW	fOOl
		**		ER	mURdER		AX	About
		**		AH	bUt		AY	hIde
		**		AW	hOW		OY	tOY
		**	
		**		p	Pack		b	Back
		**		t	Time		d	Dime
		**		k	Coat		g	Goat
		**		f	Fault		v	Vault
		**		TH	eTHer		DH	eiTHer
		**		s	Sue		z	Zoo
		**		SH	leaSH		ZH	leiSure
		**		HH	How		m	suM
		**		n	suN		NG	suNG
		**		l	Laugh		w	Wear
		**		y	Young		r	Rate
		**		CH	CHar		j	Jar
		**		WH	WHere
		**
		**
		**	strings are made up of four parts:
		**	
		**		The left context.
		**		The text to match.
		**		The right context.
		**		The phonemes to substitute for the matched text.
		**
		**	Procedure:
		**
		**		Seperate each block of letters (apostrophes included) 
		**		and add a space on each side.  For each unmatched 
		**		letter in the word, look through the rules where the 
		**		text to match starts with the letter in the word.  If 
		**		the text to match is found and the right and left 
		**		context patterns also match, output the phonemes for 
		**		that rule and skip to the next unmatched letter.
		**
		**
		**	Special Context Symbols:
		**
		**		#	One or more vowels
		**		:	Zero or more consonants
		**		^	One consonant.
		**		.	One of B, D, V, G, J, L, M, N, R, W or Z (voiced 
		**			consonants)
		**		%	One of ER, E, ES, ED, ING, ELY (a suffix)
		**			(Found in right context only)
		**		+	One of E, I or Y (a "front" vowel)
		**
		*/
		
		private static bool ValidateChar(string c, string match)
		{
			if (c == match)
			{
				return(true);
			}
			else
			{
				bool valid = false;				
				char[] match_char = match.ToCharArray();
				
				switch(match_char[0])
				{
				    case '#':
				    {
					    if ((c == "A") ||
					        (c == "E") ||
					        (c == "I") ||
					        (c == "O") ||
					        (c == "U")) valid = true;
					    break;
				    }
				    case ':':
				    {
					    valid = true;
					    if ((c == "A") ||
					        (c == "E") ||
					        (c == "I") ||
					        (c == "O") ||
					        (c == "U")) valid = false;
					    break;
				    }
				    case '^':
				    {
					    valid = true;
					    if ((c == " ") ||
					        (c == "A") ||
					        (c == "E") ||
					        (c == "I") ||
					        (c == "O") ||
					        (c == "U")) valid = false;
					    break;
				    }
				    case '.':
				    {
					    if ((c == "B") ||
					        (c == "D") ||
					        (c == "V") ||
					        (c == "G") ||
					        (c == "J") ||
						    (c == "L") ||
						    (c == "M") ||
						    (c == "N") ||
						    (c == "R") ||
						    (c == "W") ||
						    (c == "Z")) valid = true;
					    break;
				    }
				    case '+':
				    {
					    if ((c == "E") ||
					        (c == "I") ||
					        (c == "Y")) valid = true;
					    break;
				    }
				}
				return(valid);
			}
		}
		
		
		/* Context definitions */
		static string Anything = "";	/* No context requirement */
		static string Nothing = " ";	/* Context is beginning or end of word */
		
		/* Phoneme definitions */
		static string Pause = " ";	/* Short silence */
		static string Silent = "";	/* No phonemes */
		static string PhonemeSeparator = "|";
		
		const int LEFT_PART	= 0;
		const int MATCH_PART = 1;
		const int RIGHT_PART = 2;
		const int OUT_PART = 3;
		
		static string[] phonemeNames =
		{
		    "eee", "ihh", "ehh", "aaa",
		    "ahh", "aww", "ohh", "uhh",
		    "uuu", "ooo", "rrr", "lll",
		    "mmm", "nnn", "nng", "ngg",
		    "fff", "sss", "thh", "shh",
		    "xxx", "hee", "hoo", "hah",
		    "bbb", "ddd", "jjj", "ggg",
		    "vvv", "zzz", "thz", "zhh"
		};

		static string[] phonemeLookup =
		{
			"a", "aaa",
			"b", "bbb",
			"c", "",
			"d", "ddd",
			"e", "eee",
			"f", "fff",
			"g", "ggg",
			"h", "",
			"i", "ihh",
			"j", "jjj",
			"k", "",
			"l", "lll",
			"m", "mmm",
			"n", "nnn",
			"o", "ooo",
			"p", "",
			"q", "",
			"r", "rrr",
			"s", "sss",
			"t", "thh",
			"u", "uuu",
			"v", "vvv",
			"w", "aww",
			"x", "xxx",
			"y", "",
			"z", "zzz",
		    "AA", "aaa",
			"AE", "eee",
			"AH", "ahh",
			"AO", "ooo",
			"AW", "aww",
			"AX", "ahh",
			"AY", "ehh",
		    "CH", "",
		    "DH", "",
		    "EH", "ehh",
			"ER", "ehh",
			"EY", "ehh",
	        "IH", "ihh",
			"IY", "",
		    "NG", "ngg",
		    "OW", "ooo",
		    "SH", "shh",
		    "TH", "thh",
		    "UH", "uhh",
			"UW", "uuu",
		    "WH", "hoo",
		    "ZH", "zhh",
		};

		
		// formant frequency, vocal tract radius and gain
		static double[,,] phonemeParam =
		{
		    {    { 273, 0.996,  10},       // eee (beet)
		         {2086, 0.945, -16},
		         {2754, 0.979, -12},
		         {3270, 0.440, -17}    },
		    {    { 385, 0.987,  10},       // ihh (bit)
		         {2056, 0.930, -20},
		         {2587, 0.890, -20},
		         {3150, 0.400, -20}},
		    {    { 515, 0.977,  10},       // ehh (bet)
		         {1805, 0.810, -10},
		         {2526, 0.875, -10},
		         {3103, 0.400, -13}},
		    {    { 773, 0.950,  10},       // aaa (bat)
		         {1676, 0.830,  -6},
		         {2380, 0.880, -20},
		         {3027, 0.600, -20}},
		
		    {    { 770, 0.950,   0},       // ahh (father)
		         {1153, 0.970,  -9},
		         {2450, 0.780, -29},
		         {3140, 0.800, -39}},
		    {    { 637, 0.910,   0},       // aww (bought)
		         { 895, 0.900,  -3},
		         {2556, 0.950, -17},
		         {3070, 0.910, -20}},
		    {    { 637, 0.910,   0},       // ohh (bone)  NOTE::  same as aww (bought)
		         { 895, 0.900,  -3},
		         {2556, 0.950, -17},
		         {3070, 0.910, -20}},
		    {    { 561, 0.965,   0},       // uhh (but)
		         {1084, 0.930, -10},
		         {2541, 0.930, -15},
		         {3345, 0.900, -20}},
		
		    {    { 515, 0.976,   0},       // uuu (foot)
		         {1031, 0.950,  -3},
		         {2572, 0.960, -11},
		         {3345, 0.960, -20}},
		    {    { 349, 0.986, -10},       // ooo (boot)
		         { 918, 0.940, -20},
		         {2350, 0.960, -27},
		         {2731, 0.950, -33}},
		    {    { 394, 0.959, -10},       // rrr (bird)
		         {1297, 0.780, -16},
		         {1441, 0.980, -16},
		         {2754, 0.950, -40}},
		    {    { 462, 0.990,  +5},       // lll (lull)
		         {1200, 0.640, -10},
		         {2500, 0.200, -20},
		         {3000, 0.100, -30}},
		
		    {    { 265, 0.987, -10},       // mmm (mom)
		         {1176, 0.940, -22},
		         {2352, 0.970, -20},
		         {3277, 0.940, -31}},
		    {    { 204, 0.980, -10},       // nnn (nun)
		         {1570, 0.940, -15},
		         {2481, 0.980, -12},
		         {3133, 0.800, -30}},
		    {    { 204, 0.980, -10},       // nng (sang)    NOTE:: same as nnn
		         {1570, 0.940, -15},
		         {2481, 0.980, -12},
		         {3133, 0.800, -30}},
		    {    { 204, 0.980, -10},       // ngg (bong)    NOTE:: same as nnn
		         {1570, 0.940, -15},
		         {2481, 0.980, -12},
		         {3133, 0.800, -30}},
		
		    {    {1000, 0.300,   0},       // fff
		         {2800, 0.860, -10},
		         {7425, 0.740,   0},
		         {8140, 0.860,   0}},
		    {    {0,    0.000,   0},       // sss
		         {2000, 0.700, -15},
		         {5257, 0.750,  -3},
		         {7171, 0.840,   0}},
		    {    { 100, 0.900,   0},       // thh
		         {4000, 0.500, -20},
		         {5500, 0.500, -15},
		         {8000, 0.400, -20}},
		    {    {2693, 0.940,   0},       // shh
		         {4000, 0.720, -10},
		         {6123, 0.870, -10},
		         {7755, 0.750, -18}},
		
		    {    {1000, 0.300, -10},       // xxx
		         {2800, 0.860, -10},
		         {7425, 0.740,   0},
		         {8140, 0.860,   0}},
		    {    { 273, 0.996, -40},       // hee (beet)    (noisy eee)
		         {2086, 0.945, -16},
		         {2754, 0.979, -12},
		         {3270, 0.440, -17}},
		    {    { 349, 0.986, -40},       // hoo (boot)    (noisy ooo)
		         { 918, 0.940, -10},
		         {2350, 0.960, -17},
		         {2731, 0.950, -23}},
		    {    { 770, 0.950, -40},       // hah (father)  (noisy ahh)
		         {1153, 0.970,  -3},
		         {2450, 0.780, -20},
		         {3140, 0.800, -32}},
		
		    {    {2000, 0.700, -20},       // bbb
		         {5257, 0.750, -15},
		         {7171, 0.840,  -3},
		         {9000, 0.900,   0}},
		    {    { 100, 0.900,   0},       // ddd
		         {4000, 0.500, -20},
		         {5500, 0.500, -15},
		         {8000, 0.400, -20}},
		    {    {2693, 0.940,   0},       // jjj
		         {4000, 0.720, -10},
		         {6123, 0.870, -10},
		         {7755, 0.750, -18}},
		    {    {2693, 0.940,   0},       // ggg
		         {4000, 0.720, -10},
		         {6123, 0.870, -10},
		         {7755, 0.750, -18}},
		
		    {    {2000, 0.700, -20},       // vvv
		         {5257, 0.750, -15},
		         {7171, 0.840,  -3},
		         {9000, 0.900,   0}},
		    {    { 100, 0.900,   0},       // zzz
		         {4000, 0.500, -20},
		         {5500, 0.500, -15},
		         {8000, 0.400, -20}},
		    {    {2693, 0.940,   0},       // thz
		         {4000, 0.720, -10},
		         {6123, 0.870, -10},
		         {7755, 0.750, -18}},
		    {    {2693, 0.940,   0},       // zhh
		         {4000, 0.720, -10},
		         {6123, 0.870, -10},
		         {7755, 0.750, -18}}
		};
		
		
		/*0 = Punctuation */
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] punct_rules =
		{
			{Anything,	" ",		Anything,	Pause	},
			{Anything,	"-",		Anything,	Silent	},
			{".",		"'S",		Anything,	"z"	},
			{"#:.E",	"'S",		Anything,	"z"	},
			{"#",		"'S",		Anything,	"z"	},
			{Anything,	"'",		Anything,	Silent	},
			{Anything,	",",		Anything,	Pause	},
			{Anything,	".",		Anything,	Pause	},
			{Anything,	"?",		Anything,	Pause	},
			{Anything,	"!",		Anything,	Pause	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		
		static string[,] A_rules =
		{
			{Anything,	"A",		Nothing,	"AX"	},
			{Nothing,	"ARE",		Nothing,	"AAr"	},
			{Nothing,	"AR",		"O",		"AXr"	},
			{Anything,	"AR",		"#",		"EHr"	},
			{"^",		"AS",		"#",		"EYs"	},
			{Anything,	"A",		"WA",		"AX"	},
			{Anything,	"AW",		Anything,	"AO"	},
			{" :",		"ANY",		Anything,	"EHnIY"	},
			{Anything,	"A",		"^+#",		"EY"	},
			{"#:",		"ALLY",		Anything,	"AXlIY"	},
			{Nothing,	"AL",		"#",		"AXl"	},
			{Anything,	"AGAIN",	Anything,	"AXgEHn"},
			{"#:",		"AG",		"E",		"IHj"	},
			{Anything,	"A",		"^+:#",		"AE"	},
			{" :",		"A",		"^+ ",		"EY"	},
			{Anything,	"A",		"^%",		"EY"	},
			{Nothing,	"ARR",		Anything,	"AXr"	},
			{Anything,	"ARR",		Anything,	"AEr"	},
			{" :",		"AR",		Nothing,	"AAr"	},
			{Anything,	"AR",		Nothing,	"ER"	},
			{Anything,	"AR",		Anything,	"AAr"	},
			{Anything,	"AIR",		Anything,	"EHr"	},
			{Anything,	"AI",		Anything,	"EY"	},
			{Anything,	"AY",		Anything,	"EY"	},
			{Anything,	"AU",		Anything,	"AO"	},
			{"#:",		"AL",		Nothing,	"AXl"	},
			{"#:",		"ALS",		Nothing,	"AXlz"	},
			{Anything,	"ALK",		Anything,	"AOk"	},
			{Anything,	"AL",		"^",		"AOl"	},
			{" :",		"ABLE",		Anything,	"EYbAXl"},
			{Anything,	"ABLE",		Anything,	"AXbAXl"},
			{Anything,	"ANG",		"+",		"EYnj"	},
			{Anything,	"A",		Anything,	"AE"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] B_rules =
		{
			{Nothing,	"BE",		"^#",		"bIH"	},
			{Anything,	"BEING",	Anything,	"bIYIHNG"},
			{Nothing,	"BOTH",		Nothing,	"bOWTH"	},
			{Nothing,	"BUS",		"#",		"bIHz"	},
			{Anything,	"BUIL",		Anything,	"bIHl"	},
			{Anything,	"B",		Anything,	"b"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] C_rules =
		{
			{Nothing,	"CH",		"^",		"k"	},
			{"^E",		"CH",		Anything,	"k"	},
			{Anything,	"CH",		Anything,	"CH"	},
			{" S",		"CI",		"#",		"sAY"	},
			{Anything,	"CI",		"A",		"SH"	},
			{Anything,	"CI",		"O",		"SH"	},
			{Anything,	"CI",		"EN",		"SH"	},
			{Anything,	"C",		"+",		"s"	},
			{Anything,	"CK",		Anything,	"k"	},
			{Anything,	"COM",		"%",		"kAHm"	},
			{Anything,	"C",		Anything,	"k"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] D_rules =
		{
			{"#:",		"DED",		Nothing,	"dIHd"	},
			{".E",		"D",		Nothing,	"d"	},
			{"#:^E",	"D",		Nothing,	"t"	},
			{Nothing,	"DE",		"^#",		"dIH"	},
			{Nothing,	"DO",		Nothing,	"dUW"	},
			{Nothing,	"DOES",		Anything,	"dAHz"	},
			{Nothing,	"DOING",	Anything,	"dUWIHNG"},
			{Nothing,	"DOW",		Anything,	"dAW"	},
			{Anything,	"DU",		"A",		"jUW"	},
			{Anything,	"D",		Anything,	"d"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] E_rules =
		{
			{"#:",		"E",		Nothing,	Silent	},
			{"':^",		"E",		Nothing,	Silent	},
			{" :",		"E",		Nothing,	"IY"	},
			{"#",		"ED",		Nothing,	"d"	},
			{"#:",		"E",		"D ",		Silent	},
			{Anything,	"EV",		"ER",		"EHv"	},
			{Anything,	"E",		"^%",		"IY"	},
			{Anything,	"ERI",		"#",		"IYrIY"	},
			{Anything,	"ERI",		Anything,	"EHrIH"	},
			{"#:",		"ER",		"#",		"ER"	},
			{Anything,	"ER",		"#",		"EHr"	},
			{Anything,	"ER",		Anything,	"ER"	},
			{Nothing,	"EVEN",		Anything,	"IYvEHn"},
			{"#:",		"E",		"W",		Silent	},
			{"T",		"EW",		Anything,	"UW"	},
			{"S",		"EW",		Anything,	"UW"	},
			{"R",		"EW",		Anything,	"UW"	},
			{"D",		"EW",		Anything,	"UW"	},
			{"L",		"EW",		Anything,	"UW"	},
			{"Z",		"EW",		Anything,	"UW"	},
			{"N",		"EW",		Anything,	"UW"	},
			{"J",		"EW",		Anything,	"UW"	},
			{"TH",		"EW",		Anything,	"UW"	},
			{"CH",		"EW",		Anything,	"UW"	},
			{"SH",		"EW",		Anything,	"UW"	},
			{Anything,	"EW",		Anything,	"yUW"	},
			{Anything,	"E",		"O",		"IY"	},
			{"#:S",		"ES",		Nothing,	"IHz"	},
			{"#:C",		"ES",		Nothing,	"IHz"	},
			{"#:G",		"ES",		Nothing,	"IHz"	},
			{"#:Z",		"ES",		Nothing,	"IHz"	},
			{"#:X",		"ES",		Nothing,	"IHz"	},
			{"#:J",		"ES",		Nothing,	"IHz"	},
			{"#:CH",	"ES",		Nothing,	"IHz"	},
			{"#:SH",	"ES",		Nothing,	"IHz"	},
			{"#:",		"E",		"S ",		Silent	},
			{"#:",		"ELY",		Nothing,	"lIY"	},
			{"#:",		"EMENT",	Anything,	"mEHnt"	},
			{Anything,	"EFUL",		Anything,	"fUHl"	},
			{Anything,	"EE",		Anything,	"IY"	},
			{Anything,	"EARN",		Anything,	"ERn"	},
			{Nothing,	"EAR",		"^",		"ER"	},
			{Anything,	"EAD",		Anything,	"EHd"	},
			{"#:",		"EA",		Nothing,	"IYAX"	},
			{Anything,	"EA",		"SU",		"EH"	},
			{Anything,	"EA",		Anything,	"IY"	},
			{Anything,	"EIGH",		Anything,	"EY"	},
			{Anything,	"EI",		Anything,	"IY"	},
			{Nothing,	"EYE",		Anything,	"AY"	},
			{Anything,	"EY",		Anything,	"IY"	},
			{Anything,	"EU",		Anything,	"yUW"	},
			{Anything,	"E",		Anything,	"EH"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] F_rules =
		{
			{Anything,	"FUL",		Anything,	"fUHl"	},
			{Anything,	"F",		Anything,	"f"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] G_rules =
		{
			{Anything,	"GIV",		Anything,	"gIHv"	},
			{Nothing,	"G",		"I^",		"g"	},
			{Anything,	"GE",		"T",		"gEH"	},
			{"SU",		"GGES",		Anything,	"gjEHs"	},
			{Anything,	"GG",		Anything,	"g"	},
			{" B#",		"G",		Anything,	"g"	},
			{Anything,	"G",		"+",		"j"	},
			{Anything,	"GREAT",	Anything,	"grEYt"	},
			{"#",		"GH",		Anything,	Silent	},
			{Anything,	"G",		Anything,	"g"	}
		};
			
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] H_rules =
		{
			{Nothing,	"HAV",		Anything,	"hAEv"	},
			{Nothing,	"HERE",		Anything,	"hIYr"	},
			{Nothing,	"HOUR",		Anything,	"AWER"	},
			{Anything,	"HOW",		Anything,	"hAW"	},
			{Anything,	"H",		"#",		"h"	},
			{Anything,	"H",		Anything,	Silent	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] I_rules =
		{
			{Nothing,	"IN",		Anything,	"IHn"	},
			{Nothing,	"I",		Nothing,	"AY"	},
			{Anything,	"IN",		"D",		"AYn"	},
			{Anything,	"IER",		Anything,	"IYER"	},
			{"#:R",		"IED",		Anything,	"IYd"	},
			{Anything,	"IED",		Nothing,	"AYd"	},
			{Anything,	"IEN",		Anything,	"IYEHn"	},
			{Anything,	"IE",		"T",		"AYEH"	},
			{" :",		"I",		"%",		"AY"	},
			{Anything,	"I",		"%",		"IY"	},
			{Anything,	"IE",		Anything,	"IY"	},
			{Anything,	"I",		"^+:#",		"IH"	},
			{Anything,	"IR",		"#",		"AYr"	},
			{Anything,	"IZ",		"%",		"AYz"	},
			{Anything,	"IS",		"%",		"AYz"	},
			{Anything,	"I",		"D%",		"AY"	},
			{"+^",		"I",		"^+",		"IH"	},
			{Anything,	"I",		"T%",		"AY"	},
			{"#:^",		"I",		"^+",		"IH"	},
			{Anything,	"I",		"^+",		"AY"	},
			{Anything,	"IR",		Anything,	"ER"	},
			{Anything,	"IGH",		Anything,	"AY"	},
			{Anything,	"ILD",		Anything,	"AYld"	},
			{Anything,	"IGN",		Nothing,	"AYn"	},
			{Anything,	"IGN",		"^",		"AYn"	},
			{Anything,	"IGN",		"%",		"AYn"	},
			{Anything,	"IQUE",		Anything,	"IYk"	},
			{Anything,	"I",		Anything,	"IH"	}
		};
				
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] J_rules =
		{
			{Anything,	"J",		Anything,	"j"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] K_rules =
		{
			{Nothing,	"K",		"N",		Silent	},
			{Anything,	"K",		Anything,	"k"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] L_rules =
		{
			{Anything,	"LO",		"C#",		"lOW"	},
			{"L",		"L",		Anything,	Silent	},
			{"#:^",		"L",		"%",		"AXl"	},
			{Anything,	"LEAD",		Anything,	"lIYd"	},
			{Anything,	"L",		Anything,	"l"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] M_rules =
		{
			{Anything,	"MOV",		Anything,	"mUWv"	},
			{Anything,	"M",		Anything,	"m"	}
		};
				
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] N_rules =
		{
			{"E",		"NG",		"+",		"nj"	},
			{Anything,	"NG",		"R",		"NGg"	},
			{Anything,	"NG",		"#",		"NGg"	},
			{Anything,	"NGL",		"%",		"NGgAXl"},
			{Anything,	"NG",		Anything,	"NG"	},
			{Anything,	"NK",		Anything,	"NGk"	},
			{Nothing,	"NOW",		Nothing,	"nAW"	},
			{Anything,	"N",		Anything,	"n"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] O_rules =
		{
			{Anything,	"OF",		Nothing,	"AXv"	},
			{Anything,	"OROUGH",	Anything,	"EROW"	},
			{"#:",		"OR",		Nothing,	"ER"	},
			{"#:",		"ORS",		Nothing,	"ERz"	},
			{Anything,	"OR",		Anything,	"AOr"	},
			{Nothing,	"ONE",		Anything,	"wAHn"	},
			{Anything,	"OW",		Anything,	"OW"	},
			{Nothing,	"OVER",		Anything,	"OWvER"	},
			{Anything,	"OV",		Anything,	"AHv"	},
			{Anything,	"O",		"^%",		"OW"	},
			{Anything,	"O",		"^EN",		"OW"	},
			{Anything,	"O",		"^I#",		"OW"	},
			{Anything,	"OL",		"D",		"OWl"	},
			{Anything,	"OUGHT",	Anything,	"AOt"	},
			{Anything,	"OUGH",		Anything,	"AHf"	},
			{Nothing,	"OU",		Anything,	"AW"	},
			{"H",		"OU",		"S#",		"AW"	},
			{Anything,	"OUS",		Anything,	"AXs"	},
			{Anything,	"OUR",		Anything,	"AOr"	},
			{Anything,	"OULD",		Anything,	"UHd"	},
			{"^",		"OU",		"^L",		"AH"	},
			{Anything,	"OUP",		Anything,	"UWp"	},
			{Anything,	"OU",		Anything,	"AW"	},
			{Anything,	"OY",		Anything,	"OY"	},
			{Anything,	"OING",		Anything,	"OWIHNG"},
			{Anything,	"OI",		Anything,	"OY"	},
			{Anything,	"OOR",		Anything,	"AOr"	},
			{Anything,	"OOK",		Anything,	"UHk"	},
			{Anything,	"OOD",		Anything,	"UHd"	},
			{Anything,	"OO",		Anything,	"UW"	},
			{Anything,	"O",		"E",		"OW"	},
			{Anything,	"O",		Nothing,	"OW"	},
			{Anything,	"OA",		Anything,	"OW"	},
			{Nothing,	"ONLY",		Anything,	"OWnlIY"},
			{Nothing,	"ONCE",		Anything,	"wAHns"	},
			{Anything,	"ON'T",		Anything,	"OWnt"	},
			{"C",		"O",		"N",		"AA"	},
			{Anything,	"O",		"NG",		"AO"	},
			{" :^",		"O",		"N",		"AH"	},
			{"I",		"ON",		Anything,	"AXn"	},
			{"#:",		"ON",		Nothing,	"AXn"	},
			{"#^",		"ON",		Anything,	"AXn"	},
			{Anything,	"O",		"ST ",		"OW"	},
			{Anything,	"OF",		"^",		"AOf"	},
			{Anything,	"OTHER",	Anything,	"AHDHER"},
			{Anything,	"OSS",		Nothing,	"AOs"	},
			{"#:^",		"OM",		Anything,	"AHm"	},
			{Anything,	"O",		Anything,	"AA"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] P_rules =
		{
			{Anything,	"PH",		Anything,	"f"	},
			{Anything,	"PEOP",		Anything,	"pIYp"	},
			{Anything,	"POW",		Anything,	"pAW"	},
			{Anything,	"PUT",		Nothing,	"pUHt"	},
			{Anything,	"P",		Anything,	"p"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] Q_rules =
		{
			{Anything,	"QUAR",		Anything,	"kwAOr"	},
			{Anything,	"QU",		Anything,	"kw"	},
			{Anything,	"Q",		Anything,	"k"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] R_rules =
		{
			{Nothing,	"RE",		"^#",		"rIY"	},
			{Anything,	"R",		Anything,	"r"	}
		};
				
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] S_rules =
		{
			{Anything,	"SH",		Anything,	"SH"	},
			{"#",		"SION",		Anything,	"ZHAXn"	},
			{Anything,	"SOME",		Anything,	"sAHm"	},
			{"#",		"SUR",		"#",		"ZHER"	},
			{Anything,	"SUR",		"#",		"SHER"	},
			{"#",		"SU",		"#",		"ZHUW"	},
			{"#",		"SSU",		"#",		"SHUW"	},
			{"#",		"SED",		Nothing,	"zd"	},
			{"#",		"S",		"#",		"z"	},
			{Anything,	"SAID",		Anything,	"sEHd"	},
			{"^",		"SION",		Anything,	"SHAXn"	},
			{Anything,	"S",		"S",		Silent	},
			{".",		"S",		Nothing,	"z"	},
			{"#:.E",	"S",		Nothing,	"z"	},
			{"#:^##",	"S",		Nothing,	"z"	},
			{"#:^#",	"S",		Nothing,	"s"	},
			{"U",		"S",		Nothing,	"s"	},
			{" :#",		"S",		Nothing,	"z"	},
			{Nothing,	"SCH",		Anything,	"sk"	},
			{Anything,	"S",		"C+",		Silent	},
			{"#",		"SM",		Anything,	"zm"	},
			{"#",		"SN",		"'",		"zAXn"	},
			{Anything,	"S",		Anything,	"s"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] T_rules =
		{
			{Nothing,	"THE",		Nothing,	"DHAX"	},
			{Anything,	"TO",		Nothing,	"tUW"	},
			{Anything,	"THAT",		Nothing,	"DHAEt"	},
			{Nothing,	"THIS",		Nothing,	"DHIHs"	},
			{Nothing,	"THEY",		Anything,	"DHEY"	},
			{Nothing,	"THERE",	Anything,	"DHEHr"	},
			{Anything,	"THER",		Anything,	"DHER"	},
			{Anything,	"THEIR",	Anything,	"DHEHr"	},
			{Nothing,	"THAN",		Nothing,	"DHAEn"	},
			{Nothing,	"THEM",		Nothing,	"DHEHm"	},
			{Anything,	"THESE",	Nothing,	"DHIYz"	},
			{Nothing,	"THEN",		Anything,	"DHEHn"	},
			{Anything,	"THROUGH",	Anything,	"THrUW"	},
			{Anything,	"THOSE",	Anything,	"DHOWz"	},
			{Anything,	"THOUGH",	Nothing,	"DHOW"	},
			{Nothing,	"THUS",		Anything,	"DHAHs"	},
			{Anything,	"TH",		Anything,	"TH"	},
			{"#:",		"TED",		Nothing,	"tIHd"	},
			{"S",		"TI",		"#N",		"CH"	},
			{Anything,	"TI",		"O",		"SH"	},
			{Anything,	"TI",		"A",		"SH"	},
			{Anything,	"TIEN",		Anything,	"SHAXn"	},
			{Anything,	"TUR",		"#",		"CHER"	},
			{Anything,	"TU",		"A",		"CHUW"	},
			{Nothing,	"TWO",		Anything,	"tUW"	},
			{Anything,	"T",		Anything,	"t"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] U_rules =
		{
			{Nothing,	"UN",		"I",		"yUWn"	},
			{Nothing,	"UN",		Anything,	"AHn"	},
			{Nothing,	"UPON",		Anything,	"AXpAOn"},
			{"T",		"UR",		"#",		"UHr"	},
			{"S",		"UR",		"#",		"UHr"	},
			{"R",		"UR",		"#",		"UHr"	},
			{"D",		"UR",		"#",		"UHr"	},
			{"L",		"UR",		"#",		"UHr"	},
			{"Z",		"UR",		"#",		"UHr"	},
			{"N",		"UR",		"#",		"UHr"	},
			{"J",		"UR",		"#",		"UHr"	},
			{"TH",		"UR",		"#",		"UHr"	},
			{"CH",		"UR",		"#",		"UHr"	},
			{"SH",		"UR",		"#",		"UHr"	},
			{Anything,	"UR",		"#",		"yUHr"	},
			{Anything,	"UR",		Anything,	"ER"	},
			{Anything,	"U",		"^ ",		"AH"	},
			{Anything,	"U",		"^^",		"AH"	},
			{Anything,	"UY",		Anything,	"AY"	},
			{" G",		"U",		"#",		Silent	},
			{"G",		"U",		"%",		Silent	},
			{"G",		"U",		"#",		"w"	},
			{"#N",		"U",		Anything,	"yUW"	},
			{"T",		"U",		Anything,	"UW"	},
			{"S",		"U",		Anything,	"UW"	},
			{"R",		"U",		Anything,	"UW"	},
			{"D",		"U",		Anything,	"UW"	},
			{"L",		"U",		Anything,	"UW"	},
			{"Z",		"U",		Anything,	"UW"	},
			{"N",		"U",		Anything,	"UW"	},
			{"J",		"U",		Anything,	"UW"	},
			{"TH",		"U",		Anything,	"UW"	},
			{"CH",		"U",		Anything,	"UW"	},
			{"SH",		"U",		Anything,	"UW"	},
			{Anything,	"U",		Anything,	"yUW"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] V_rules =
		{
			{Anything,	"VIEW",		Anything,	"vyUW"	},
			{Anything,	"V",		Anything,	"v"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] W_rules =
		{
			{Nothing,	"WERE",		Anything,	"wER"	},
			{Anything,	"WA",		"S",		"wAA"	},
			{Anything,	"WA",		"T",		"wAA"	},
			{Anything,	"WHERE",	Anything,	"WHEHr"	},
			{Anything,	"WHAT",		Anything,	"WHAAt"	},
			{Anything,	"WHOL",		Anything,	"hOWl"	},
			{Anything,	"WHO",		Anything,	"hUW"	},
			{Anything,	"WH",		Anything,	"WH"	},
			{Anything,	"WAR",		Anything,	"wAOr"	},
			{Anything,	"WOR",		"^",		"wER"	},
			{Anything,	"WR",		Anything,	"r"	},
			{Anything,	"W",		Anything,	"w"	}
		};
				
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] X_rules =
		{
			{Anything,	"X",		Anything,	"ks"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] Y_rules =
		{
			{Anything,	"YOUNG",	Anything,	"yAHNG"	},
			{Nothing,	"YOU",		Anything,	"yUW"	},
			{Nothing,	"YES",		Anything,	"yEHs"	},
			{Nothing,	"Y",		Anything,	"y"	},
			{"#:^",		"Y",		Nothing,	"IY"	},
			{"#:^",		"Y",		"I",		"IY"	},
			{" :",		"Y",		Nothing,	"AY"	},
			{" :",		"Y",		"#",		"AY"	},
			{" :",		"Y",		"^+:#",		"IH"	},
			{" :",		"Y",		"^#",		"AY"	},
			{Anything,	"Y",		Anything,	"IH"	}
		};
		
		/*
		**	LEFT_PART	MATCH_PART	RIGHT_PART	OUT_PART
		*/
		static string[,] Z_rules =
		{
			{Anything,	"Z",		Anything,	"z"	}
		};
		
		static string[][,] Rules =
		{
			punct_rules,
			A_rules, B_rules, C_rules, D_rules, E_rules, F_rules, G_rules, 
			H_rules, I_rules, J_rules, K_rules, L_rules, M_rules, N_rules, 
			O_rules, P_rules, Q_rules, R_rules, S_rules, T_rules, U_rules, 
			V_rules, W_rules, X_rules, Y_rules, Z_rules
		};

		static string[] Cardinals = 
		{
			"zIHrOW",	"wAHn",	"tUW",		"THrIY",
			"fOWr",	"fAYv",	"sIHks",	"sEHvAXn",
			"EYt",		"nAYn",		
			"tEHn",	"IYlEHvAXn",	"twEHlv",	"THERtIYn",
			"fOWrtIYn",	"fIHftIYn", 	"sIHkstIYn",	"sEHvEHntIYn",
			"EYtIYn",	"nAYntIYn"
		};
		
		static string[] Tens = 
		{
			"",	"tEHn",	"twEHntIY",	"THERtIY",	"fAOrtIY",	"fIHftIY",
			"sIHkstIY",	"sEHvEHntIY",	"EYtIY",	"nAYntIY"
		};
		
		static string[] Ordinals = 
		{
			"zIHrOWEHTH",	"fERst",	"sEHkAHnd",	"THERd",
			"fOWrTH",	"fIHfTH",	"sIHksTH",	"sEHvEHnTH",
			"EYtTH",	"nAYnTH",		
			"tEHnTH",	"IYlEHvEHnTH",	"twEHlvTH",	"THERtIYnTH",
			"fAOrtIYnTH",	"fIHftIYnTH", 	"sIHkstIYnTH",	"sEHvEHntIYnTH",
			"EYtIYnTH",	"nAYntIYnTH"
		};
		
		static string[] Ord_twenties = 
		{
			"twEHntIYEHTH","THERtIYEHTH",	"fOWrtIYEHTH",	"fIHftIYEHTH",
			"sIHkstIYEHTH","sEHvEHntIYEHTH","EYtIYEHTH",	"nAYntIYEHTH"
		};
		
		private static bool IsNumeric(string text)
		{           
		    Regex objNotWholePattern = new Regex("[^0-9]");
		    return (!objNotWholePattern.IsMatch(text) && (text != ""));
		}		

		public static string ConvertNumber(int n)
		{
			string phonemes = "";
			
			if (n < 0) phonemes = ConvertWord("minus") + " ";
			n = Math.Abs(n);
			
			int millions = n / 1000000;
			n -= (millions * 1000000);
			int thousands = n / 1000;
			n -= (thousands * 1000);
			int hundreds = n / 100;
			n -= (hundreds * 100);
			int tens = n / 10;
			int ones = n % 10;
			if (millions > 0) phonemes += Cardinals[millions] + " " + ConvertWord("million") + " ";
			if (thousands > 0) phonemes += Cardinals[thousands] + " " + ConvertWord("thousand") + " ";
			if (hundreds > 0) phonemes += Cardinals[hundreds] + " " + ConvertWord("hundred") + " ";
			if ((hundreds > 0) && (n > 0))
			{
				phonemes += ConvertWord("and") + " ";
			}
			if (tens < 2)
			{
				phonemes += Cardinals[n];
			}
			else
			{
				if (tens > 0)
				{
				    phonemes += Tens[tens];
				}
				if (ones > 0)
				{
				    phonemes += " " + Cardinals[ones];
				}
			}
			
			return(phonemes);
		}
		
		public static string ConvertWord(string text)
		{
			string phonemes = "";
			
			text = " " + text.ToUpper() + " ";
			
			if (IsNumeric(text))
			{
				return(ConvertNumber(Convert.ToInt32(text)));
			}
			
			if ((text.EndsWith("st")) || 
			    (text.EndsWith("nd")) ||
			    (text.EndsWith("rd")))
			{
				string s = text.Substring(0, text.Length-2);
				if (IsNumeric(s))
				{
					int n = Convert.ToInt32(s);
					if ((n > -1) && (n < 20))
					{
						return(Ordinals[n]);
					}
					else
					{
						if ((n >= 20) && (n < 100))
						{
							int tens = n / 10;
							phonemes = Tens[tens];
							int ones = n - (tens*10);
							if (ones > 0)
							    phonemes += " " + Ordinals[ones];
							return(phonemes);
						}
					}
				}
			}
			
			char[] ch = text.ToCharArray();
			for (int i = 0; i < ch.Length; i++)
			{	
				bool matched = false;
				int c = (int)ch[i];
				if ((c >= 'A') && (c <= 'Z'))
				{					
					c  = c - (int)'A' + 1;
				    string[,] rules = Rules[c];
					int max = rules.GetLength(0);
					for (int r = 0; r < max; r++)
					{
						string left = rules[r, 0];												
						if (i > left.Length)
						{
							string right = rules[r, 2];
							string match = rules[r, 1];		
														
						    if (i < ch.Length - (right.Length + match.Length))
						    {
								bool valid = true;
								int start_idx = i - left.Length;
								for (int j = start_idx; j < i; j++)
								{
									valid = ValidateChar(text.Substring(j,1), left.Substring(j - start_idx, 1));
									if (!valid) break;
								}
								
								if ((left == Anything) ||
								    (left == Silent) ||
								    (valid))
								{									
									valid = true;
									start_idx = i + match.Length;
									for (int j = start_idx; j < start_idx+right.Length; j++)
									{
										valid = ValidateChar(text.Substring(j,1), right.Substring(j - start_idx, 1));
										if (!valid) break;
									}
																		
								    if ((right == Anything) ||
									    (right == Silent) ||
									    (valid))
								    {
						                if (text.Substring(i, match.Length) == match)
										{
						                    string output = rules[r, 3];
											matched = true;
											phonemes += output;
											i += match.Length-1;
											r = max;
										}
									}
								}
							}
						}
					}
				}
				if (!matched)
				{
					string str = "";
					str += ch[i];
					phonemes += str.ToLower();
				}
			}
			phonemes = phonemes.Trim();
			return(phonemes);
		}
		
		public static string ConvertText(string text)
		{
			string phonemes = "";
			string[] word = text.Split(' ');
			for (int w = 0; w < word.Length; w++)
			{
				if (word[w] != "")
				    phonemes += ConvertWord(word[w]) + " ";
			}
			return(phonemes.Trim());
		}
		
		public static float[] FormantsNormalised(
		    string text, 
		    int dimension)
		{			
			float[] result = new float[dimension*4*3];
			List<float> formants = Formants(text, true);
			int no_of_formants = formants.Count  / (4*3);
			int n = 0;
			for (int i = 0; i < dimension; i++)
			{
				float idx = (i * (no_of_formants-1)) / (float)dimension;
				float fraction = idx - (int)idx;
				
				int idx_start = (int)idx * 4 * 3;
				int idx_end = ((int)idx + 1) * 4 * 3;
				
				for (int j = 0; j < 4; j++)
				{
				    for (int k = 0; k < 3; k++, idx_start++, idx_end++)
				    {
						result[n++] = formants[idx_start] + ((formants[idx_end] - formants[idx_start]) * fraction);
					}
				}
			}
			
			return(result);
		}

		public static string ToNgramStandardised(
		    string text, 
		    int n, 
		    bool reverse)
		{
			text = RemoveCommonWords(text);
			string str = "";
			List<string> ngram = new List<string>();
			ngrams(text,n,ngram, true, false, reverse);
			for (int i = 0; i < ngram.Count; i++)
				str += ngram[i] + " ";
			
			return(str.Trim());
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
			"than",
	        "last",
	        "first",
	        "on",
	        "of",
	        "its",
	        "all",
			"does",
			"did",
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
				
		private static void ngrams(
		    string text, int n,
		    List<string> ngram,
		    bool sort,
		    bool no_duplicates,
		    bool reverse)
		{
			ngram.Clear();
			string[] buffer = new string[n];
			int buffer_index = 0;
			int ctr = 0;
			
			string phonemes = ConvertText(text);
			char[] ch = phonemes.ToCharArray();
			
			for (int i = 0; i < ch.Length; i++)
			{
				string ph = "";
				if ((ch[i] >= 'a') && (ch[i] <= 'z'))
				{
					ph += ch[i];
				}
				else
				{
					if ((ch[i] >= 'A') && (ch[i] <= 'Z'))
					{
						ph += ch[i];
						ph += ch[i + 1];
						i++;
					}
				}
				
				if (ph != "")
				{
					buffer[buffer_index++] = ph;
					if (buffer_index >= n) buffer_index = 0;
					if (ctr > 1)
					{
						int idx = buffer_index - n;
						if (idx < 0) idx += n;
						string ngram_str = "";
						for (int j = 0; j < n; j++)
						{
						    ngram_str += buffer[idx];
							if (j < n-1) ngram_str += "-";
							idx++;
							if (idx >= n) idx -= n;
						}
						if (!no_duplicates)
						{
						    ngram.Add(ngram_str);
						}
						else
						{
							if (!ngram.Contains(ngram_str))
								ngram.Add(ngram_str);
						}
					}
					ctr++;
				}
			}
			if (sort)
			{
				ngram.Sort();
				if (reverse) ngram.Reverse();
			}
		}	
		
		public static List<float> Formants(
		    string text, 
		    bool normalised)
		{
			List<float> formants = new List<float>();
			
			string phonemes = ConvertText(text);
			char[] ch = phonemes.ToCharArray();
			
			for (int i = 0; i < ch.Length; i++)
			{
				string ph = "";
				if ((ch[i] >= 'a') && (ch[i] <= 'z'))
				{
					ph += ch[i];
				}
				else
				{
					if ((ch[i] >= 'A') && (ch[i] <= 'Z'))
					{
						ph += ch[i];
						ph += ch[i + 1];
						i++;
					}
				}
				
				if (ph != "")
				{					
					string formnt = "";
					for (int j = 0; j < phonemeLookup.Length; j += 2)
					{
						if (phonemeLookup[j] == ph)
						{
							formnt = phonemeLookup[j+1];							
							break;
						}
					}
					if (formnt != "")
					{
						int idx = -1;
						for (int j = 0; j < phonemeNames.Length; j++)
						{
							if (phonemeNames[j] == formnt)
							{
								idx = j;
								break;
							}
						}
						
						if (idx > -1)
						{
							for (int j = 0; j < 4; j++)
							{
								for (int k = 0; k < 3; k++)
								{
									if (!normalised)
									{
									    formants.Add((float)phonemeParam[idx,j,k]);
									}
									else
									{
										float divisor = 1;
										if (k == 0) divisor = 10000;
										if (k == 2) divisor = 30;
										formants.Add((float)phonemeParam[idx,j,k] / divisor);
									}
								}
							}
						}
					}
				}
				else
				{
					for (int j = 0; j < 4*3; j++)
						formants.Add(0.0f);
				}
			}
			return(formants);
		}
		
		public static int Difference(
		    byte[] vect1, 
		    byte[] vect2,
		    int tollerance_percent)
		{
			int dimension = vect1.Length;
			int diff = 0;
			int tollerance = dimension * tollerance_percent / 100;
			
			if (tollerance == 0)
			{
				for (int i = 0; i < dimension; i++)
					diff += Math.Abs(vect2[i] - vect1[i])*10;
			}
			else
			{
				for (int i = 0; i < dimension; i++)
				{
					int min_diff = int.MaxValue;
					for (int j = i - tollerance; j <= i + tollerance; j++)
					{
						if ((j > -1) && (j < dimension))
						{
					        int d = Math.Abs(vect2[i] - vect1[i])*10;
							if (d < min_diff)
							{
								min_diff = d;
							}
						}
					}
					diff += min_diff;
				}
			}
			diff /= dimension;
			return(diff);
		}
		    
		public static int Difference(
		    string str1, 
		    string str2,
		    int dimension,
		    int tollerance_percent)
		{
			int diff = 0;
			int tollerance = dimension * tollerance_percent / 100;
			byte[] vect1 = FormantsNormalisedSimple(str1, dimension);
			byte[] vect2 = FormantsNormalisedSimple(str2, dimension);
			
			if (tollerance == 0)
			{
				for (int i = 0; i < dimension; i++)
					diff += Math.Abs(vect2[i] - vect1[i])*10;
			}
			else
			{
				for (int i = 0; i < dimension; i++)
				{
					int min_diff = int.MaxValue;
					for (int j = i - tollerance; j <= i + tollerance; j++)
					{
						if ((j > -1) && (j < dimension))
						{
					        int d = Math.Abs(vect2[i] - vect1[i])*10;
							if (d < min_diff)
							{
								min_diff = d;
							}
						}
					}
					diff += min_diff;
				}
			}
			diff /= dimension;
			return(diff);
		}
		
		/// <summary>
		/// Convert a string containing formants into a byte array
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static byte[] FormantsFromString(string str)
		{
			int no_of_formants = str.Length/3;
			byte[] f = new byte[no_of_formants];
			for (int i = 0; i < no_of_formants; i++)
			{
				f[i] = Convert.ToByte(str.Substring(i*3,3));
			}
			return(f);
		}
		
		/// <summary>
		/// convert the formant values to something which can be written to file or to a database
		/// </summary>
		/// <param name="formants"></param>
		/// <returns></returns>
		public static string StringFromFormants(byte[] formants)
		{
			string str = "";
			for (int i  =0; i < formants.Length; i++)
			{
				string s = "";
				if (formants[i] < 10) s += "0";
				if (formants[i] < 100) s += "0";
				str += s + formants[i].ToString();
			}
			return(str);
		}
		
		public static byte[] FormantsNormalisedSimple(
		    string text, 
		    int dimension)
		{
			byte[] result = new byte[dimension];
			float[] normalised = FormantsNormalised(text, dimension);
			int no_of_formants = dimension;
			for (int i = 0; i < no_of_formants-1; i++)
			{
				int idx0 = i * 4 * 3;
				int idx1 = (i+1) * 4 * 3;
				float tot = 0;
				float gain = 0;
				for (int j = 0; j < 4; j++)
				{
				    for (int k = 0; k < 3; k++, idx0++, idx1++)
				    {
						float diff = normalised[idx1] - normalised[idx0];
						if (k == 1) gain += diff;
						tot += diff*diff;						
					}
				}
				float dist = (float)Math.Sqrt(tot) * 0.5f;
				if (gain < 0) dist = -dist;
				dist = (0.5f + (dist * 0.5f)) * 255;
				result[i] = (byte)dist;
			}
			return(result);
		}		
		
	}
}
