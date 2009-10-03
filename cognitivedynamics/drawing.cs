/*
    Functions for drawing within bitmaps
    Copyright (C) 2000-2007 Bob Mottram
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
using System.Drawing;

namespace cognitivedynamics
{
    public class drawing
    {
        public drawing()
        {
        }

        #region "flood fill algorithms"

        /// <summary>
        /// pops an x,y position from the flood fill stack
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="stackPointer"></param>
        /// <param name="image_height"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static bool floodFillPop(int[] stack, ref int stackPointer, int image_height,
                                         ref int x, ref int y)
        {
            if (stackPointer > 0)
            {
                int p = stack[stackPointer];
                x = p / image_height;
                y = p % image_height;
                stackPointer--;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// pushes an x,y position onto the flood fill stack
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="stackPointer"></param>
        /// <param name="image_height"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static bool floodFillPush(int[] stack, ref int stackPointer,
                                          int image_height, int x, int y)
        {
            if (stackPointer < stack.Length - 1)
            {
                stackPointer++;
                stack[stackPointer] = image_height * x + y;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// flood fill from the given point, updating a boolean array (region)
        /// </summary>
        /// <param name="x">x coordinate to start from</param>
        /// <param name="y">y coordinate to start from</param>
        /// <param name="stack">integer array for the stack</param>
        /// <param name="tx">top x coordinate of the bounding box</param>
        /// <param name="ty">top y coordinate of the bounding box</param>
        /// <param name="bx">bottom x coordinate of the bounding box</param>
        /// <param name="by">bottom y coordinate of the bounding box</param>
        /// <param name="pixels">total number of pixels filled</param>
        /// <param name="av_intensity">average intensity of pixels filled</param>
        /// <param name="source">the source image</param>
        /// <param name="current">currently filled pixels for the entire image</param>
        /// <param name="region">currently filled pixels for this region only</param>
        /// <param name="av_x">average x coordinate of the region</param>
        /// <param name="av_y">average y coordinate of the region</param>
        /// <param name="image_width">width of the image</param>
        /// <param name="image_height">height of the image</param>
        public static void floodFillLinear(int x, int y,
                              int threshold,
                              int[] stack,
                              ref int tx, ref int ty, ref int bx, ref int by,
                              ref long pixels,
                              ref long av_intensity,
                              int[,] source,
                              bool[,] current,
                              bool[,] region,
                              ref long av_x, ref long av_y,
                              int image_width, int image_height)
        {
            if (current[x, y] == false)
            {
                // create a stack and a pointer to the current location
                int stackPointer = 0;  // point to the beginning of the stack
                if (stack == null)
                {
                    // allocating large amounts of memory can be slow, so ideally
                    // this should have been done beforehand
                    stack = new int[image_width * image_height * 10];
                }

                int y1;
                bool spanLeft, spanRight;

                if (floodFillPush(stack, ref stackPointer, image_height, x, y))
                {
                    while (floodFillPop(stack, ref stackPointer, image_height, ref x, ref y))
                    {
                        y1 = y;
                        while (((current[x, y1] == false) && (source[x, y1] > threshold)) && (y1 > 0)) y1--;
                        y1++;
                        spanLeft = false;
                        spanRight = false;
                        while (((current[x, y1] == false) && (source[x, y1] > threshold)) && (y1 < image_height - 1))
                        {
                            current[x, y1] = true;
                            region[x, y1] = true;

                            if (x < tx) tx = x;
                            if (x > bx) bx = x;
                            if (y1 < ty) ty = y1;
                            if (y1 > by) by = y1;

                            av_intensity += source[x, y1];

                            av_x += x;
                            av_y += y1;

                            pixels++;

                            if ((!spanLeft) && (x > 0) && ((current[x - 1, y1] == false) && (source[x - 1, y1] > threshold)))
                            {
                                if (!floodFillPush(stack, ref stackPointer, image_height, x - 1, y1)) return;
                                spanLeft = true;
                            }
                            else if ((spanLeft) && (x > 0) && ((current[x - 1, y1] == false) && (source[x - 1, y1] > threshold)))
                            {
                                spanLeft = false;
                            }
                            if ((!spanRight) && (x < image_width - 1) && ((current[x + 1, y1] == false) && (source[x + 1, y1] > threshold)))
                            {
                                if (!floodFillPush(stack, ref stackPointer, image_height, x + 1, y1)) return;
                                spanRight = true;
                            }
                            else if ((spanRight) && (x < image_width - 1) && ((current[x + 1, y1] == false) && (source[x + 1, y1] > threshold)))
                            {
                                spanRight = false;
                            }
                            y1++;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// flood fill from the given point, updating a boolean array (region)
        /// </summary>
        /// <param name="x">x coordinate to start from</param>
        /// <param name="y">y coordinate to start from</param>
        /// <param name="depth">current search depth</param>
        /// <param name="max_search_depth">maximum search depth</param>
        /// <param name="tx">top x coordinate of the bounding box</param>
        /// <param name="ty">top y coordinate of the bounding box</param>
        /// <param name="bx">bottom x coordinate of the bounding box</param>
        /// <param name="by">bottom y coordinate of the bounding box</param>
        /// <param name="pixels">total number of pixels filled</param>
        /// <param name="av_intensity">average intensity of pixels filled</param>
        /// <param name="source">the source image</param>
        /// <param name="current">currently filled pixels for the entire image</param>
        /// <param name="region">currently filled pixels for this region only</param>
        /// <param name="av_x">average x coordinate of the region</param>
        /// <param name="av_y">average y coordinate of the region</param>
        /// <param name="image_width">width of the image</param>
        /// <param name="image_height">height of the image</param>
        public static void floodFillRecursive(int x, int y,
                              int threshold,
                              int depth, int max_search_depth,
                              ref int tx, ref int ty, ref int bx, ref int by,
                              ref long pixels,
                              ref long av_intensity,
                              int[,] source,
                              bool[,] current,
                              bool[,] region,
                              ref long av_x, ref long av_y,
                              int image_width, int image_height)
        {
            if ((current[x, y] == false) && (source[x, y] > threshold) && (depth < max_search_depth))
            {
                if (x < tx) tx = x;
                if (x > bx) bx = x;
                if (y < ty) ty = y;
                if (y > by) by = y;

                av_intensity += source[x, y];

                av_x += x;
                av_y += y;

                pixels++;

                current[x, y] = true;
                region[x, y] = true;

                if (x > 0)
                {
                    floodFillRecursive(x - 1, y, threshold, depth + 1, max_search_depth, ref tx, ref ty, ref bx, ref by, ref pixels, ref av_intensity, source, current, region, ref av_x, ref av_y, image_width, image_height);

                    if (y > 0)
                    {
                        floodFillRecursive(x - 1, y - 1, threshold, depth + 1, max_search_depth, ref tx, ref ty, ref bx, ref by, ref pixels, ref av_intensity, source, current, region, ref av_x, ref av_y, image_width, image_height);

                        if (x < image_width - 1)
                        {
                            floodFillRecursive(x + 1, y - 1, threshold, depth + 1, max_search_depth, ref tx, ref ty, ref bx, ref by, ref pixels, ref av_intensity, source, current, region, ref av_x, ref av_y, image_width, image_height);
                        }
                    }

                    if (x < image_width - 1)
                    {
                        floodFillRecursive(x + 1, y, threshold, depth + 1, max_search_depth, ref tx, ref ty, ref bx, ref by, ref pixels, ref av_intensity, source, current, region, ref av_x, ref av_y, image_width, image_height);

                        if (y < image_height - 1)
                        {
                            floodFillRecursive(x + 1, y + 1, threshold, depth + 1, max_search_depth, ref tx, ref ty, ref bx, ref by, ref pixels, ref av_intensity, source, current, region, ref av_x, ref av_y, image_width, image_height);
                        }
                    }
                }

                if (y > 0)
                {
                    floodFillRecursive(x, y - 1, threshold, depth + 1, max_search_depth, ref tx, ref ty, ref bx, ref by, ref pixels, ref av_intensity, source, current, region, ref av_x, ref av_y, image_width, image_height);
                }

                if (y < image_height - 1)
                {
                    floodFillRecursive(x, y + 1, threshold, depth + 1, max_search_depth, ref tx, ref ty, ref bx, ref by, ref pixels, ref av_intensity, source, current, region, ref av_x, ref av_y, image_width, image_height);
                }

            }
        }

        #endregion

        #region "boxes"

        public static void drawBox(Byte[] img, int img_width, int img_height,
                           int x, int y, int radius, int r, int g, int b, int line_width)
        {
            int radius_y = radius * img_width / img_height;
            int tx = x - radius;
            int ty = y - radius_y;
            int bx = x + radius;
            int by = y + radius_y;
            drawLine(img, img_width, img_height, tx, ty, bx, ty, r, g, b, line_width, false);
            drawLine(img, img_width, img_height, bx, ty, bx, by, r, g, b, line_width, false);
            drawLine(img, img_width, img_height, tx, by, bx, by, r, g, b, line_width, false);
            drawLine(img, img_width, img_height, tx, by, tx, ty, r, g, b, line_width, false);
        }


        /// <summary>
        /// draw a rotated box
        /// </summary>
        /// <param name="img"></param>
        /// <param name="img_width"></param>
        /// <param name="img_height"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="box_width"></param>
        /// <param name="box_height"></param>
        /// <param name="rotation"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="line_width"></param>
        public static void drawBox(byte[] img, int img_width, int img_height,
                                   int x, int y, int box_width, int box_height,
                                   float rotation, int r, int g, int b, int line_width)
        {
            int tx = -box_width / 2;
            int ty = -box_height / 2;
            int bx = box_width / 2;
            int by = box_height / 2;
            int[] xx = new int[4];
            int[] yy = new int[4];
            xx[0] = tx;
            yy[0] = ty;
            xx[1] = bx;
            yy[1] = ty;
            xx[2] = bx;
            yy[2] = by;
            xx[3] = tx;
            yy[3] = by;

            int prev_x2 = 0, prev_y2 = 0;
            for (int i = 0; i < 5; i++)
            {
                int index = i;
                if (i >= 4) index = 0;
                float dist = (float)Math.Sqrt((xx[index] * xx[index]) + (yy[index] * yy[index]));
                float angle = (float)Math.Acos(xx[index] / dist);
                if (yy[index] < 0) angle = ((float)Math.PI * 2) - angle;

                int x2 = x + (int)(dist * (float)Math.Sin(angle + rotation));
                int y2 = y + (int)(dist * (float)Math.Cos(angle + rotation));
                if (i > 0)
                    drawLine(img, img_width, img_height, x2, y2, prev_x2, prev_y2, r, g, b, line_width, false);
                prev_x2 = x2;
                prev_y2 = y2;
            }
        }

        #endregion

        #region "crosses"

        public static void drawCross(Byte[] img, int img_width, int img_height,
                                     int x, int y, int radius, int r, int g, int b, int line_width)
        {
            int radius_y = radius * img_width / img_height;
            int tx = x - radius;
            int ty = y - radius_y;
            int bx = x + radius;
            int by = y + radius_y;
            drawLine(img, img_width, img_height, x, ty, x, by, r, g, b, line_width, false);
            drawLine(img, img_width, img_height, tx, y, bx, y, r, g, b, line_width, false);
        }

        #endregion

        #region "circles/spots"

        public static void drawCircle(byte[] img, int img_width, int img_height,
                                      int x, int y, int radius, int r, int g, int b, int line_width)
        {
            drawCircle(img, img_width, img_height, (float)x, (float)y, (float)radius, r, g, b, line_width);
        }

        public static void drawCircle(byte[] img, int img_width, int img_height,
                                      float x, float y, float radius, int r, int g, int b, int line_width)
        {
            int points = 20;
            int prev_xx = 0, prev_yy = 0;
            for (int i = 0; i < points + 1; i++)
            {
                float angle = i * 2 * (float)Math.PI / points;
                int xx = (int)Math.Round(x + (radius * Math.Sin(angle)));
                int yy = (int)Math.Round(y + (radius * Math.Cos(angle)));

                if (i > 0)
                    drawLine(img, img_width, img_height, prev_xx, prev_yy, xx, yy, r, g, b, line_width, false);
                prev_xx = xx;
                prev_yy = yy;
            }
        }


        public static void drawSpot(byte[] img, int img_width, int img_height,
                                    int x, int y, int radius, int r, int g, int b)
        {
            for (int rr = 1; rr <= radius; rr++)
                drawCircle(img, img_width, img_height, x, y, rr, r, g, b, 1);
        }

		public static void drawSpotOverlay(
		    byte[] img, int img_width, int img_height,
            int x, int y, int radius, int r, int g, int b)
        {
			int radius_sqr = radius*radius;
			for (int xx = x - radius; xx <= x + radius; xx++)
			{
				if ((xx > -1) && (xx < img_width))
				{
					int dx = xx - x;
			        for (int yy = y - radius; yy <= y + radius; yy++)
			        {
				        if ((yy > -1) && (yy < img_height))
				        {
							int dy = yy - y;
							if (dx*dx + dy*dy < radius_sqr)
							{
							    int n = ((yy * img_width) + xx) * 3;
								if (r != -1) img[n+2] = (byte)r;
								if (g != -1) img[n+1] = (byte)g;
								if (b != -1) img[n] = (byte)b;
							}							
						}
					}
				}
			}
        }

        #endregion

        #region "grids"

        /// <summary>
        /// draw a grid within the given image
        /// </summary>
        /// <param name="img">image to be returned</param>
        /// <param name="img_width">width of the image</param>
        /// <param name="img_height">height of the image</param>
        /// <param name="centre_x">x centre point of the grid</param>
        /// <param name="centre_y">y centre point of the grid</param>
        /// <param name="rotation">rotation angle of the grid in radians</param>
        /// <param name="columns">number of grid columns</param>
        /// <param name="rows">number of grid rows</param>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="linewidth">line width</param>
        public static void drawGrid(byte[] img, int img_width, int img_height,
                                    int centre_x, int centre_y, float rotation,
                                    float size_width, float size_height,
                                    int columns, int rows,
                                    int r, int g, int b, int linewidth)
        {
            // draw the columns
            for (int col = 0; col <= columns; col++)
            {
                float grid_x = ((col * size_width) / (float)columns) - (size_width / 2);
                int prev_x = 0;
                int prev_y = 0;
                for (int row = 0; row <= rows; row += rows)
                {
                    float grid_y = ((row * size_height) / (float)rows) - (size_height / 2);
                    float hyp = (float)Math.Sqrt((grid_x * grid_x) + (grid_y * grid_y));
                    float angle = 0;
                    if (hyp > 0)
                    {
                        angle = (float)Math.Asin(grid_x / hyp);
                        if (grid_y < 0) angle = (float)(Math.PI * 2) - angle;
                    }
                    angle += rotation;

                    int x = (int)(centre_x + (hyp * Math.Sin(angle)));
                    int y = (int)(centre_y + (hyp * Math.Cos(angle)));

                    if (row > 0)
                    {
                        drawLine(img, img_width, img_height, prev_x, prev_y, x, y,
                                 r, g, b, linewidth, false);
                    }

                    prev_x = x;
                    prev_y = y;
                }
            }

            // draw the rows
            for (int row = 0; row <= rows; row++)
            {
                float grid_y = ((row * size_height) / (float)rows) - (size_height / 2);
                int prev_x = 0;
                int prev_y = 0;
                for (int col = 0; col <= columns; col += columns)
                {
                    float grid_x = ((col * size_width) / (float)columns) - (size_width / 2);
                    float hyp = (float)Math.Sqrt((grid_x * grid_x) + (grid_y * grid_y));
                    float angle = 0;
                    if (hyp > 0)
                    {
                        angle = (float)Math.Asin(grid_x / hyp);
                        if (grid_y < 0) angle = (float)(Math.PI * 2) - angle;
                    }
                    angle += rotation;

                    int x = (int)(centre_x + (hyp * Math.Sin(angle)));
                    int y = (int)(centre_y + (hyp * Math.Cos(angle)));

                    if (col > 0)
                    {
                        drawLine(img, img_width, img_height, prev_x, prev_y, x, y,
                                 r, g, b, linewidth, false);
                    }

                    prev_x = x;
                    prev_y = y;
                }
            }

        }

        #endregion

        #region "lines"

        /// <summary>
        /// draw a line within the given image
        /// </summary>
        /// <param name="img">image to be returned</param>
        /// <param name="img_width">width of the image</param>
        /// <param name="img_height">height of the image</param>
        /// <param name="x1">top x</param>
        /// <param name="y1">top y</param>
        /// <param name="x2">bottom x</param>
        /// <param name="y2">bottom y</param>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="linewidth">line width</param>
        public static void drawLine(Byte[] img, int img_width, int img_height,
                                    int x1, int y1, int x2, int y2, int r, int g, int b, int linewidth, bool overwrite)
        {
            if (img != null)
            {
                int w, h, x, y, step_x, step_y, dx, dy, xx2, yy2;
                float m;

                dx = x2 - x1;
                dy = y2 - y1;
                w = Math.Abs(dx);
                h = Math.Abs(dy);
                if (x2 >= x1) step_x = 1; else step_x = -1;
                if (y2 >= y1) step_y = 1; else step_y = -1;

                if ((w < img_width + 50) && (h < img_height + 50))
                {
                    if (w > h)
                    {
                        if (dx != 0)
                        {
                            m = dy / (float)dx;
                            x = x1;
                            int s = 0;
                            while (s * Math.Abs(step_x) <= Math.Abs(dx))
                            {
                                y = (int)(m * (x - x1)) + y1;

                                for (xx2 = x - linewidth; xx2 <= x + linewidth; xx2++)
                                    for (yy2 = y - linewidth; yy2 <= y + linewidth; yy2++)
                                    {
                                        if ((xx2 >= 0) && (xx2 < img_width) && (yy2 >= 0) && (yy2 < img_height))
                                        {
                                            int n = ((img_width * yy2) + xx2) * 3;
                                            if ((img[n] == 0) || (!overwrite))
                                            {
                                                img[n] = (Byte)b;
                                                img[n + 1] = (Byte)g;
                                                img[n + 2] = (Byte)r;
                                            }
                                            else
                                            {
                                                img[n] = (Byte)((img[n] + b) / 2);
                                                img[n + 1] = (Byte)((img[n] + g) / 2);
                                                img[n + 2] = (Byte)((img[n] + r) / 2);
                                            }
                                        }
                                    }

                                x += step_x;
                                s++;
                            }
                        }
                    }
                    else
                    {
                        if (dy != 0)
                        {
                            m = dx / (float)dy;
                            y = y1;
                            int s = 0;
                            while (s * Math.Abs(step_y) <= Math.Abs(dy))
                            {
                                x = (int)(m * (y - y1)) + x1;
                                for (xx2 = x - linewidth; xx2 <= x + linewidth; xx2++)
                                    for (yy2 = y - linewidth; yy2 <= y + linewidth; yy2++)
                                    {
                                        if ((xx2 >= 0) && (xx2 < img_width) && (yy2 >= 0) && (yy2 < img_height))
                                        {
                                            int n = ((img_width * yy2) + xx2) * 3;
                                            if ((img[n] == 0) || (!overwrite))
                                            {
                                                img[n] = (Byte)b;
                                                img[n + 1] = (Byte)g;
                                                img[n + 2] = (Byte)r;
                                            }
                                            else
                                            {
                                                img[n] = (Byte)((img[n] + b) / 2);
                                                img[n + 1] = (Byte)((img[n] + g) / 2);
                                                img[n + 2] = (Byte)((img[n] + r) / 2);
                                            }
                                        }
                                    }

                                y += step_y;
                                s++;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region "text"

        /// <summary>
        /// add some text to the given image
        /// </summary>
        /// <param name="img">colour image into which to insert th text</param>
        /// <param name="img_width">width of the image</param>
        /// <param name="img_height">height of the image</param>
        /// <param name="text">text to be added</param>
        /// <param name="font">font style</param>
        /// <param name="font_size">font size</param>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="position_x">x coordinate at which to insert the text</param>
        /// <param name="position_y">y coordinate at which to insert the text</param>
        public static void AddText(byte[] img, int img_width, int img_height,
                                   String text,
                                   String font, int font_size,
                                   int r, int g, int b,
                                   float position_x, float position_y)
        {
            Bitmap screen_bmp = new Bitmap(img_width, img_height,
                                           System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // insert the existing image into the bitmap
            BitmapArrayConversions.updatebitmap_unsafe(img, screen_bmp);

            Font drawFont = new Font(font, font_size);
            SolidBrush drawBrush = new SolidBrush(Color.FromArgb(r, g, b));

            Graphics grph = Graphics.FromImage(screen_bmp);
            grph.DrawString(text, drawFont, drawBrush, position_x, position_y);
            grph.Dispose();

            // extract the bitmap data
            BitmapArrayConversions.updatebitmap(screen_bmp, img);
        }

        #endregion
    }
}
