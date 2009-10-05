
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace cognitivedynamics
{
		
	public class dynamicstestyesno		
	{
		public string results_filename = "cogdynamics_results";
		List<string>[] propositions = null;
		List<double> mouse_position;
		List<DateTime> mouse_time;
		Random rnd;
		public int image_width, image_height;
		public byte[] test_image;
		Bitmap bmp;
		private bool yes_left;
		private int coherence_index;
		
		public dynamicstestyesno(
		    string test_propositions_filename,
		    int image_width, int image_height)
		{
			this.image_width = image_width;
			this.image_height = image_height;
			test_image = new byte[image_width * image_height * 3];			
			for (int i = image_width * image_height*3-1; i >= 0; i--) test_image[i]=255;
			bmp = new Bitmap(image_width, image_height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);				
			mouse_position = new List<double>();
			mouse_time = new List<DateTime>();
			
			propositions = new List<string>[10];
			rnd = new Random();
			for (int i = 0; i < 10; i++)
			    propositions[i] = new List<string>();
			
			BitmapArrayConversions.updatebitmap_unsafe(test_image, bmp);
			bmp.Save("test.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);			
			
			if (File.Exists(test_propositions_filename))
			{
				LoadPropositions(test_propositions_filename, "Mind Hack", propositions);
			}
		}
		
		private void SaveResult(bool yes)
		{			
			StreamWriter oWrite = null;			
			bool filefound = true;

            try
            {
				if (File.Exists(results_filename))
                    oWrite = File.AppendText(results_filename);
				else
					oWrite = File.CreateText(results_filename);
            }
            catch
            {
                filefound = false;
            }

            if (filefound)
            {
				oWrite.WriteLine(mouse_time.Count.ToString());
				oWrite.WriteLine(yes.ToString());
				oWrite.WriteLine(yes_left.ToString());
				oWrite.WriteLine(coherence_index.ToString());
				for (int t = 0; t < mouse_time.Count; t++)
				{
					oWrite.Write(mouse_time[t].ToBinary().ToString() + " ");
					oWrite.Write(mouse_position[t*2].ToString() + " ");
					oWrite.WriteLine(mouse_position[t*2+1].ToString());
				}
				oWrite.Close();
			}
			else
			{
				Console.WriteLine("Results could not be saved");
			}
		}
		
		public void ShowAllResults()
		{
			StreamReader oRead = null;			
			bool filefound = true;

			int bucket_pixels=5;
			int group_ms = 60;
			int font_size = 20;
			yesno_y = image_height * 1/10;
			radius = image_height / 12;
			int left_x = image_width / 10;
			int right_x = image_width-1-left_x;
			
			for (int i = image_width*image_height*3-1; i >= 0; i--)
				test_image[i]=255;
			drawing.drawSpot(test_image, image_width, image_height, left_x, yesno_y, radius, 200,255,200);
			drawing.drawSpot(test_image, image_width, image_height, right_x, yesno_y, radius, 200,255,200);
		    drawing.AddText(test_image, image_width, image_height, "Yes", "Courier", font_size, 0,0,0, right_x-font_size, yesno_y-font_size);
		    drawing.AddText(test_image, image_width, image_height, "No", "Courier", font_size, 0,0,0, left_x-font_size, yesno_y-font_size);
			
			int[,,] average = new int[image_height,10,2];
			int histogram_max=1;
			int[] histogram_no = new int[image_width];
			int[] histogram_yes = new int[image_width];
			int[,] histogram_decision_time = new int[5,1000];
			int max_decision_time_mS = 100;
            int decision_time_hist_max = 1;
			float average_decision_time = 0;
			int average_decision_time_hits=0;
			DateTime prev_timestamp=new DateTime(1900,1,1);
			
            try
            {
                oRead = File.OpenText(results_filename);
            }
            catch
            {
                filefound = false;
            }

            if (filefound)
            {
				
				while (!oRead.EndOfStream)
				{
					int steps = Convert.ToInt32(oRead.ReadLine());
					bool yes = Convert.ToBoolean(oRead.ReadLine());
					bool yes_left = Convert.ToBoolean(oRead.ReadLine());
					coherence_index = Convert.ToInt32(oRead.ReadLine());
					int prev_x = 0;
					int prev_y = 0;
					DateTime start_time = DateTime.Now;
					int decision_time_mS = 0;
					float speed_pixels_per_sec = 0;
					int prev_x2 = 9999;
					int prev_y2 = 0;
					
					for (int t = 0; t < steps; t++)
					{
						string s = oRead.ReadLine();
						string[] s2 = s.Split(' ');
						DateTime timestamp = DateTime.FromBinary(Convert.ToInt64(s2[0]));
						if (t == 0)
						{
							start_time = timestamp;
						}
						if (t == steps-1) 
						{
							TimeSpan diff = timestamp.Subtract(start_time);
							int time_mS = (int)diff.TotalMilliseconds;
							if (time_mS/4 < 1000/4)
							{								
								if (time_mS > max_decision_time_mS)
									max_decision_time_mS = time_mS;								
							
							    histogram_decision_time[coherence_index/2,time_mS/250]++;
								if (histogram_decision_time[coherence_index/2,time_mS/250] > decision_time_hist_max)
									decision_time_hist_max = histogram_decision_time[coherence_index/2,time_mS/250];
								average_decision_time += (float)time_mS;
								average_decision_time_hits++;
							}
						}
						int x = Convert.ToInt32(Convert.ToSingle(s2[1]));
						if (yes_left) x = image_width-1-x;
						    
						int y = Convert.ToInt32(Convert.ToSingle(s2[2]));
						if (t > 0)
						{
							drawing.drawLine(test_image, image_width, image_height, prev_x, prev_y, x,y, 0,0,0,0,false);
							
							int step=1;
							if (y < prev_y) step = -1;
							int n = 0;
							if (y - prev_y != 0)
							{
								int yy = prev_y;
								while (yy != y)
								{
									int xx = prev_x + ((x - prev_x) * n / (Math.Abs(y - prev_y)));
									if ((xx > -1) && (xx < image_width) &&
									    (yy > -1) && (yy < image_height))
									{
										
										average[yy,coherence_index,0] += xx;
										average[yy,coherence_index,1]++;											
										
										if (yes)
										{

											int xx2 = xx / bucket_pixels;
					                        histogram_yes[xx2]++;
					                        if (histogram_yes[xx2] > histogram_max)
						                    histogram_max = histogram_yes[xx2];					
											
										}
										else
										{
											int xx2 = xx / bucket_pixels;
											histogram_no[xx2]++;
											if (histogram_no[xx2] > histogram_max)
												histogram_max = histogram_no[xx2];
										}
									}
									
									yy += step;
									n++;
								}
							}
						}
						prev_x = x;
						prev_y = y;
					}
				}
				oRead.Close();
			}

			BitmapArrayConversions.updatebitmap_unsafe(test_image, bmp);
			bmp.Save("test_results.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);
			
			for (int i = image_width*image_height*3-1; i >= 0; i--)
				test_image[i]=255;
			drawing.drawSpot(test_image, image_width, image_height, left_x, yesno_y, radius, 200,255,200);
			drawing.drawSpot(test_image, image_width, image_height, right_x, yesno_y, radius, 200,255,200);
		    drawing.AddText(test_image, image_width, image_height, "Yes", "Courier", font_size, 0,0,0, right_x-font_size, yesno_y-font_size);
		    drawing.AddText(test_image, image_width, image_height, "No", "Courier", font_size, 0,0,0, left_x-font_size, yesno_y-font_size);
			
			drawing.drawLine(test_image, image_width, image_height, image_width/2, 0, image_width/2, image_height-1, 220,220,220,1,false);

			for (int coherence_index = 0; coherence_index < 10; coherence_index++)
			{
				bool labeled = false;
				int r = 255 - (255 * coherence_index/10);
				int g = 255 * coherence_index/10;
				for (int y = 0; y < image_height; y++)
				{
					if (average[y,coherence_index,1] > 0)
					{
						int x = average[y,coherence_index,0] / average[y,coherence_index,1];
						drawing.drawSpot(test_image, image_width, image_height, x,y,1,r,g,0);
						
						if ((!labeled) && (y > image_height*15/100))
						{
							drawing.AddText(test_image,image_width, image_height, "0." + coherence_index.ToString(), "Courier", 10, 0,0,0,x,y + rnd.Next(40)-20);
							labeled=true;
						}
					}
				}
			}
			BitmapArrayConversions.updatebitmap_unsafe(test_image, bmp);
			bmp.Save("test_results_average.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);

			/*
			for (int i = image_width*image_height*3-1; i >= 0; i--)
				test_image[i]=255;
			drawing.drawLine(test_image, image_width, image_height, image_width/2, 0, image_width/2, image_height-1, 220,220,220,1,false);
			for (int x = 1; x < image_width/bucket_pixels; x++)
			{
				int prev_y_yes = image_height - 1 - (histogram_yes[x-1]*(image_height*110/100)/histogram_max);
				int prev_y_no = image_height - 1 - (histogram_no[x-1]*(image_height*110/100)/histogram_max);
				int y_yes = image_height - 1 - (histogram_yes[x]*(image_height*110/100)/histogram_max);
				int y_no = image_height - 1 - (histogram_no[x]*(image_height*110/100)/histogram_max);
				drawing.drawLine(test_image, image_width, image_height, (x-1)*bucket_pixels, prev_y_yes, x*bucket_pixels, y_yes, 0,255,0,0,false);
				drawing.drawLine(test_image, image_width, image_height, (x-1)*bucket_pixels, prev_y_no, x*bucket_pixels, y_no, 255,0,0,0,false);
			}
			BitmapArrayConversions.updatebitmap_unsafe(test_image, bmp);
			bmp.Save("test_results_horizontal.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);
			*/

			/*
			for (int i = image_width*image_height*3-1; i >= 0; i--) test_image[i]=255;
			for (int coherence_index=0; coherence_index<5;coherence_index++)
			{
				int r = 255-(coherence_index*255/5);
				int g = coherence_index*255/5;
				for (int t = 1; t < max_decision_time_mS/250; t++)
				{
					int prev_x = (t-1) * image_width / (max_decision_time_mS/250);
					int x = t * image_width / (max_decision_time_mS/250);
					
					int prev_y = image_height - 1 - ((histogram_decision_time[coherence_index,t-1]) * image_height / decision_time_hist_max);
					int y = image_height - 1 - ((histogram_decision_time[coherence_index,t]) * image_height / decision_time_hist_max);
					drawing.drawLine(test_image, image_width, image_height, prev_x, prev_y, x, y, r,g,0,1,false);
				}
			}
			BitmapArrayConversions.updatebitmap_unsafe(test_image, bmp);
			bmp.Save("test_results_decision_times.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);
			*/
		}
		
		public bool Clicked()
		{
			bool finished=false;
			
			if (mouse_position.Count > 2)
			{
				double x = mouse_position[mouse_position.Count-2];
				double y = mouse_position[mouse_position.Count-1];
				DateTime start_time = mouse_time[0];
				double dx_yes = x - (yes_x);
				double dx_no = x - (no_x);
				double dy = y - (yesno_y);
								
				if (Math.Abs(dy) < radius*2)
				{
					if (Math.Abs(dx_yes) < radius*2)
					{
						Console.WriteLine("Yes");
						SaveResult(true);
						finished = true;
					}
					if (Math.Abs(dx_no) < radius*2)
					{
						Console.WriteLine("No");
						SaveResult(false);
						finished = true;
					}					
				}
			}
			if (finished)
			{
				mouse_position.Clear();
				mouse_time.Clear();
			}
			
			return(finished);
		}
		
		public string GetProposition()
		{
			mouse_position.Clear();
			mouse_time.Clear();
			CreateTestImage();
			string prop = "";
			while ((prop=="") || (prop.Length > 30))
			{
				coherence_index = (int)(rnd.NextDouble()*10);
				int max = propositions[coherence_index].Count-1;
				prop = propositions[coherence_index][rnd.Next(max)];
			}
			return(prop);
		}
		
		private int yesno_y = 0;
		private int yes_x = 0;
		private int no_x = 0;
		private int radius = 0;
		public int offset_x = 50;
		public int offset_y = 50;
		
		private void CreateTestImage()
		{
			for (int i = image_width*image_height*3-1; i >= 0; i--)
				test_image[i]=255;

			BitmapArrayConversions.updatebitmap_unsafe(test_image, bmp);
			bmp.Save("blank.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);
			
			int font_size = 20;
			yesno_y = image_height * 1/10;
			radius = image_height / 12;
			int left_x = image_width / 10;
			int right_x = image_width-1-left_x;
			
			yes_left = true;
			if (rnd.NextDouble() > 0.5) yes_left = false;
			
			drawing.drawSpot(test_image, image_width, image_height, left_x, yesno_y, radius, 200,255,200);
			drawing.drawSpot(test_image, image_width, image_height, right_x, yesno_y, radius, 200,255,200);

			if (yes_left)
			{
				yes_x = left_x;
				no_x = right_x;
			}
			else
			{
				yes_x = right_x;
				no_x = left_x;
			}
		    drawing.AddText(test_image, image_width, image_height, "Yes", "Courier", font_size, 0,0,0, yes_x-font_size, yesno_y-font_size);
		    drawing.AddText(test_image, image_width, image_height, "No", "Courier", font_size, 0,0,0, no_x-font_size, yesno_y-font_size);
			
			BitmapArrayConversions.updatebitmap_unsafe(test_image, bmp);
			bmp.Save("test.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);
		}
				
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
		
		public void StoreMousePosition(double x, double y)
		{
			mouse_position.Add(x - offset_x);
			mouse_position.Add(y - offset_y);
			mouse_time.Add(DateTime.Now);
			//Console.WriteLine(x.ToString() + "  " + y.ToString());
		}
		
        static void LoadPropositions(
		    string filename, 
            string initialstring,
		    List<string>[] propositions)
        {
            bool filefound = true;
            string str, question;
            float coherence;
			StreamReader oRead = null;			

			for (int i = 0; i < 10; i++)
			{
			    propositions[i].Clear();
			}
			
            try
            {
                oRead = File.OpenText(filename);
            }
            catch
            {
                filefound = false;
            }

            if (filefound)
            {
				Console.WriteLine("Loading propositions from " + filename);
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
								
								string question2 = " " + question + " ";
								if (!question2.ToLower().Contains(" gac "))
								{
									int idx = (int)(coherence * 10);
								    propositions[idx].Add(question);
								}
								
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
			
			
        }
		
	    
	}
}
