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
							for (int i = 0; i < no_of_records; i++)
							{
								index1[i] = Convert.ToSingle(oRead.ReadLine());
								index2[i] = Convert.ToSingle(oRead.ReadLine());
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
						
						//Console.WriteLine("args: " + question);
						
						string phoneme_index = phoneme.ToNgramStandardised(question, 3, false);
						float coordinate_phoneme = GetNgramIndex(phoneme_index, 80);
		                string index_soundex = Soundex.ToSoundexStandardised(question, false);
						float coordinate_soundex = GetNgramIndex(index_soundex, 80);
						
						int idx1 = 0;
						for (idx1 = 0; idx1 < no_of_records; idx1++)
							if (index1[idx1] >= coordinate_phoneme) break;

						int idx2 = 0;
						for (idx2 = 0; idx2 < no_of_records; idx2++)
							if (index2[idx2] >= coordinate_soundex) break;
						
						int x = idx1 * image_width / no_of_records;
						int y = idx2 * image_width / no_of_records;
						if ((x < image_width) && (y < image_width))
						{
						    int n = ((y * image_width) + x)*3;
							int r = img[n+2];
							int g = img[n+1];
							int b = img[n];
							
							if (!((r==0) && (g==0) && (b==0)))
							{
								if (g > 0)
									Console.WriteLine("Yes");
								else
									Console.WriteLine("No");
							}
							else
							{
								Console.WriteLine("Don't know");
							}
						}						
					}
				}
			}
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
		
	}
}