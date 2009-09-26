/*
    functions for converting betweeb byte arrays and bitmap objects
    Copyright (C) 2007 Bob Mottram
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
using System.Drawing;
using System.Drawing.Imaging;

namespace mpcreate
{
    /// <summary>
    /// conversions between bitmap objects and byte arrays
    /// This is based upon GPL code originally used in the Sentience project
    /// </summary>
    public partial class BitmapArrayConversions
    {
        #region bitmap loading/storing

        /// <summary>
        /// Copy the given byte array to a bitmap object
        /// </summary>
        /// <param name="imageData">Array to be inserted</param>
        /// <param name="bmp">Destination bitmap object</param>
        public static unsafe bool updatebitmap_unsafe(byte[] imageData, Bitmap bmp)
        {
            bool success = true;

            // get the number of bytes per pixel
            int bytes_per_pixel = 3;
            if (bmp.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                bytes_per_pixel = 1;

            try
            {
                if (imageData != null)
                {
                    // Lock bitmap and retrieve bitmap pixel data pointer
                    BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);
                    int currStride = bmpData.Stride;

                    // fix the bitmap data in place whilst processing as the garbage collector may move it
                    fixed (byte* pimageData = imageData)
                    {
                        // Copy the image data to the bitmap
                        byte* dst = (byte*)bmpData.Scan0;
                        byte* src = (byte*)pimageData;

                        //if ((currStride != bmp.Width * bytes_per_pixel) && (bytes_per_pixel > 1))
                        if (currStride != bmp.Width * bytes_per_pixel)
                        {
                            // uneven stride length
                            //long real_length = currStride * bmp.Height;
                            long n = 0;
                            long w = bmp.Width * bytes_per_pixel;
                            for (int y = 0; y < bmp.Height; y++)
                            {
                                long n1 = (y * bmp.Width) * bytes_per_pixel;
                                for (int x = 0; x < currStride; x++)
                                {
                                    if (x < w) dst[n] = src[n1];
                                    n++;
                                    n1++;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < imageData.Length; i++) *dst++ = *src++;
                        }
                    }
                    bmp.UnlockBits(bmpData);
                }
            }
            catch //(Exception ex)
            {
                success = false;
                //MessageBox.Show("updatebitmap1/" + ex.Message);
            }
            return(success);
        }

        /// <summary>
        /// Copy the given bitmap object to a byte array
        /// </summary>
        /// <param name="bmp">bitmap object</param>
        /// <param name="imageData">Destination Array</param>
        public static unsafe bool updatebitmap(Bitmap bmp, byte[] imageData)
        {
            BitmapData bmpData = null;

            if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                updatebitmapmono(bmp, imageData);
            }
            else
            {
                
                // Lock bitmap and retrieve bitmap pixel data pointer
				try
				{
                    bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
				}
				catch
				{
				}
                if (bmpData != null)
                {
                    int currStride = bmpData.Stride;

                    // fix the bitmap data in place whilst processing as the garbage collector may move it
                    fixed (byte* pimageData = imageData)
                    {
                        // Copy the bitmap to the image data
                        byte* dst = (byte*)bmpData.Scan0;
                        byte* src = (byte*)pimageData;

                        if (currStride != bmp.Width * 3)
                        {
                            // uneven stride length
                            //long real_length = currStride * bmp.Height;
                            long n = 0;
                            long w = bmp.Width * 3;
                            for (int y = 0; y < bmp.Height; y++)
                            {
                                long n1 = (y * bmp.Width) * 3;
                                for (int x = 0; x < currStride; x++)
                                {
                                    if (x < w) src[n1] = dst[n];
                                    n++;
                                    n1++;
                                }
                            }
                        }
                        else
                        {
                            // even stride length
                            for (int i = 0; i < imageData.Length; i++)
                                *src++ = *dst++;
                        }
                    }

                    bmp.UnlockBits(bmpData);
                }
            }
			if (bmpData != null)
				return(true);
			else
				return(false);
        }
		
		/// <summary>
		///swaps BGR and RGB formats
		/// </summary>
		/// <param name="img">image whose colour is to be reversed</param>
		public static void RGB_BGR(byte[] img)
		{
			for (int i = 0; i < img.Length; i += 3)
			{
				byte temp = img[i];
				img[i] = img[i+2];
				img[i+2] = temp;
			}
		}

        /// <summary>
        /// inserts the given bitmap data into the given byte array
        /// </summary>
        /// <param name="bmp">source bitmap</param>
        /// <param name="imageData">byte array to be updated</param>
        public static unsafe void updatebitmapmono(Bitmap bmp, byte[] imageData)
        {
            int i;

            // Lock bitmap and retrieve bitmap pixel data pointer

            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            //int currStride = bmpData.Stride;

            // create a mono array for the image
            int no_of_bytes = bmp.Width * bmp.Height;
            Byte[] imageData_mono = new Byte[no_of_bytes];

            // fix the bitmap data in place whilst processing as the garbage collector may move it

            fixed (byte* pimageData = imageData_mono)
            {

                // Copy the bitmap to the image data

                byte* dst = (byte*)bmpData.Scan0;

                byte* src = (byte*)pimageData;

                for (i = 0; i < imageData_mono.Length; i++)
                {

                    *src++ = *dst++;

                }

            }

            bmp.UnlockBits(bmpData);

            if (imageData.Length == imageData_mono.Length)
            {
                // insert the mono data into a 8bpp array
                for (i = 0; i < imageData_mono.Length; i++)
                    imageData[i] = imageData_mono[i];
            }
            else
            {
                // insert the mono data into a 24bpp array
                int n = 0;
                for (i = 0; i < imageData_mono.Length; i++)
                {
                    n = i * 3;
                    imageData[n] = imageData_mono[i];
                    imageData[n + 1] = imageData_mono[i];
                    imageData[n + 2] = imageData_mono[i];
                }
            }
        }


        /// <summary>
        /// Copy the given bitmap object to a byte array (slow version)
        /// </summary>
        /// <param name="bmp">bitmap object</param>
        /// <param name="imageData">Destination Array</param>
        public static unsafe void updatebitmapslow(Bitmap bmp, byte[] imageData)
        {
            int x, y, n;
            Color col;

            n = 0;
            for (y = 0; y < bmp.Height; y++)
                for (x = 0; x < bmp.Width; x++)
                {
                    col = bmp.GetPixel(x, y);
                    imageData[n] = col.B;
                    imageData[n + 1] = col.G;
                    imageData[n + 2] = col.R;
                    n += 3;
                }
        }


        /// <summary>
        /// Copy the given byte array to a bitmap object (slow version)
        /// </summary>
        /// <param name="imageData">source Array</param>
        /// <param name="bmp">bitmap object</param>
        public static unsafe void updatebitmapslow(byte[] imageData, Bitmap bmp)
        {
            int x, y, n;

            n = 0;
            for (y = 0; y < bmp.Height; y++)
                for (x = 0; x < bmp.Width; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(imageData[n + 2], imageData[n + 1], imageData[n]));
                    n += 3;
                }
        }

        #endregion
    }

}
