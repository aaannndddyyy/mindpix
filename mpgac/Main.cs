/*
    mpgac: A minimally intelligent AGI capable of answering questions a yes or no answer (1 bit bandwidth)
    Based upon the Mindpixel GAC-80K corpus 2000-2005
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
using System.Drawing;
using ca.guitard.jeff.utility;

namespace mpgac
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				if (args[0] != "")
				{
					string image_filename = "mindpixelmap.png";
					string indexes_filename = "mindpixelindexes.bin";
					
					if (!File.Exists(image_filename))
						image_filename = "/usr/bin/" + image_filename;
					
					if (!File.Exists(indexes_filename))
						indexes_filename = "/usr/bin/" + indexes_filename;
					
					if ((File.Exists(image_filename)) &&
					    (File.Exists(indexes_filename)))
					{	
						int no_of_records = 0;
						float[] index1 = null;
						float[] index2 = null;
						float[] index3 = null;
						
						// load the indexes
						StreamReader oRead = null;
						bool opened = false;
						try
						{
							oRead = File.OpenText(indexes_filename);
							opened = true;
						}
						catch
						{						
						}
						if (opened)
						{
							no_of_records = Convert.ToInt32(oRead.ReadLine());
							index1 = new float[no_of_records];
							index2 = new float[no_of_records];
							index3 = new float[no_of_records];
							for (int i = 0; i < no_of_records; i++)
							{
								index1[i] = Convert.ToSingle(oRead.ReadLine());
								index2[i] = Convert.ToSingle(oRead.ReadLine());
								index3[i] = Convert.ToSingle(oRead.ReadLine());
							}
							oRead.Close();
						}
						
						// load the map image
						Bitmap bmp = (Bitmap)Bitmap.FromFile(image_filename);
						int image_width = bmp.Width;
						byte[] img = new byte[image_width * image_width * 3];
						BitmapArrayConversions.updatebitmap(bmp, img);
						
						string question = "";
						for (int i = 0; i < args.Length; i++)
							question += args[i] + " ";
						question = question.Trim();
						
						if (question.StartsWith("-validate "))
						{
							Validate(index1,index2,index3,image_width,img,args[args.Length-1],"Mind Hack");
						}
						else
						{						
							//Console.WriteLine("args: " + question);
							
							int answer = GetAnswer(question, index1, index2, index3, image_width,img);
							switch(answer)
							{
								case 1: {
								    Console.WriteLine("Yes");
									break;
								}
								case -1: {
								    Console.WriteLine("No");
									break;
								}
								case 0: {
								    Console.WriteLine("Don't know");
									break;
								}
							}
							
						}
					}
				}
			}
		}
		
		private static int GetAnswer(
		    string question,
		    float[] index1,
		    float[] index2,
		    float[] index3,
		    int image_width,
		    byte[] img)
		{
			int no_of_records = index1.Length;
			int result = 0;
			string phoneme_index = phoneme.ToNgramStandardised(question, 3, false);
			float coordinate_phoneme = GetNgramIndex(phoneme_index, 80);
            string index_soundex = Soundex.ToSoundexStandardised(question, false, false);
			float coordinate_soundex = GetNgramIndex(index_soundex, 80);
            string index_verbs = verbs.GetVerbClasses(question);
			float coordinate_verbs = GetNgramIndex(index_verbs, 80);
			
			int idx1 = 0;
			for (idx1 = 0; idx1 < no_of_records; idx1++)
				if (index1[idx1] >= coordinate_phoneme) break;

			int idx2 = 0;
			for (idx2 = 0; idx2 < no_of_records; idx2++)
				if (index2[idx2] >= coordinate_soundex) break;

			int idx3 = 0;
			for (idx3 = 0; idx3 < no_of_records; idx3++)
				if (index3[idx3] >= coordinate_verbs) break;
			
			int x = idx1 * image_width / no_of_records;
			int y = idx2 * image_width / no_of_records;
			if ((x < image_width) && 
			    (y < image_width))
			{
				int z = idx3 * 24 / no_of_records;
			    				
				int n = ((y * image_width) + x) * 3;
				
				result = -1;
				int v = 0;
				if (z < 8)
				{
					v = ((int)img[n]) & (int)Math.Pow(2,z);
					if (v != 0) result = 1;
				}
				else
				{
					if (z < 16)
					{
						v = ((int)img[n+1]) & (int)Math.Pow(2,z-8);
					    if (v != 0) result = 1;
					}
					else
					{
						v = ((int)img[n+2]) & (int)Math.Pow(2,z-16);
						if (v != 0) result = 1;
					}
				}
				
			}						
			return(result);
		}
		
		private static float GetNgramIndex(string idx, int max_length)
		{
			double index = 0;
			idx = idx.ToLower();
			if (idx.Length > max_length)
				idx = idx.Substring(0, max_length);
			while (idx.Length < max_length)
				idx += "0";
			
			char[] ch = idx.ToCharArray();
			
			double mult = 1.0/3600000000.0;
			double max = 0;
            for (int i = 0; i < ch.Length; i++) {
				if (((ch[i] >= 'a') &&
				     (ch[i] <= 'z')) ||
				    ((ch[i] >= '0') &&
				     (ch[i] <= '9')))
				{
					if (ch[i] <= '9')
						index = 2*index + ((ch[i] - (int)'0' + 1)*mult);
					else
                        index = 2*index + ((ch[i] - (int)'a' + 11)*mult);
					
					max = 2*max + (36*mult);
				}
            }
			index /= max;
			//Console.WriteLine("index = " + index.ToString());
			
			return((float)index);
		}
		
		#region "validation"

		static string ToNumeric(string str)
		{
			string result = "";
			char[] ch = str.ToCharArray();
			for (int i = 0; i < ch.Length; i++)
			{
				if (((ch[i] >= '0') && (ch[i] <= '9')) || (ch[i]=='.'))
					result += ch[i];
			}
			return(result);
		}
		
        static void Validate(
		    float[] index1,
		    float[] index2,
		    float[] index3,
		    int image_width,
		    byte[] img,
		    string mindpixels_filename, 
            string initialstring)
        {
            bool filefound = true;
            string str, question;
            float coherence;
			StreamReader oRead = null;			
			int hits=0,misses=0;
			
            try
            {
                oRead = File.OpenText(mindpixels_filename);
            }
            catch
            {
                filefound = false;
            }

            if (filefound)
            {
				Console.WriteLine("Validating map against training corpus");
                bool initialstringFound = false;
				int i = 0;
								
	            while ((!oRead.EndOfStream) && (i < 80000))
	            {						
	                str = oRead.ReadLine();
	                if (!initialstringFound)
	                {
	                    /// look for an initial header string after which the data begins
	                    if (str.Contains(initialstring)) initialstringFound = true;
	                }
	                else
	                {
	                    /// read the data
	                    if (str != "")
	                    {
	                        try
	                        {
	                            coherence = Convert.ToSingle(ToNumeric(str.Substring(1, 4)));
								if (coherence > 1) coherence = 1;
	                            question = str.Substring(6);

								int answer = GetAnswer(question, index1, index2, index3, image_width,img);
								switch(answer)
								{
									case 1: {
									    if (coherence > 0.5f) 
										    hits++;
									    else
										    misses++;
										break;
									}
									case -1: {
									    if (coherence < 0.5f) 
										    hits++;
									    else
										    misses++;
										break;
									}
									case 0: {
									    if ((coherence > 0.45f) && (coherence < 0.55f))
										    hits++;
									    else
										    misses++;
										break;
									}
								}

								if (i % 1000 == 0) Console.Write(".");
								i++;
	                        }
	                        catch //(Exception ex)
	                        {
								//Console.WriteLine("str: " + str);
								//Console.WriteLine("error: " + ex.Message);
	                        }
	                    }
	                }
	            }
	            if (oRead.EndOfStream)
	            {
	                oRead.Close();
	            }						
			}
			Console.WriteLine("");
			if (hits+misses>0)
			{
				int score = hits * 100 / (hits+misses);
				Console.WriteLine("Validation score " + score.ToString() + "%");
			}
        }		
		
		#endregion
		
	}
}