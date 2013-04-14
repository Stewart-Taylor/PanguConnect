/*      ElevationLoader Class
 *	    AUTHOR: STEWART TAYLOR
 *------------------------------------
 * Loads in an elevation model
 * Provides utilities that may be useful
 * 
 * Last Updated: 16/03/2013
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace PanguConnect
{
    class ElevationLoader
    {
        private String format;
        private int width;
        private int height;
        private String[] hexMap;
        private int imageStartIndex;

        private bool p6Valid = true;
        private Bitmap image;
        private double[,] heightMap;
        private float verticalHeight = 0.1f; // 0.1m

        private double maxHeight = -1;
        private double minHeight = 0;


        public ElevationLoader(double[,] grid)
        {
            heightMap = grid;

            width = grid.GetLength(0);
            height = grid.GetLength(0);

            generateBitmap();
        }


        private void generateHeightMap()
        {
            heightMap = new double[width, height];

            int hexMapIndex = imageStartIndex + 7;
            int widthCounter = 0;
            int heightCounter = 0;

            int x = 0;
            int y = 0;

            do
            {
                do
                {
                    int red = getColorValue(hexMap[hexMapIndex]);
                    int green = getColorValue(hexMap[hexMapIndex + 1]);
                    int blue = getColorValue(hexMap[hexMapIndex + 2]);

                    heightMap[x, y] = getHeight(red, green);

                    x++;
                    widthCounter += 3;
                    hexMapIndex += 3;
                } while (x < width);
                heightCounter++;
                widthCounter = 0;
                y++;
                x = 0;

            } while (y < height);
        }

        private float getHeight(int red, int green)
        {
            float height = 0;
            height = ((red * 256 + green));

            return height;
        }

        private void generateBitmap()
        {
            setMaxHeight();
            setMinHeight();
            Bitmap bitmap = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    double heightTemp = heightMap[x, y];
                    int value = getHeightColor(heightTemp);

                    System.Drawing.Color tempColor = System.Drawing.Color.FromArgb(255, value, value, value);
                    bitmap.SetPixel(x, y, tempColor);
                }
            }

            image = bitmap;
        }


        public int getHeightColor(double height)
        {
            double valueR = height - minHeight;
            double valueT = (valueR / (maxHeight - minHeight));
            double tempNumber = valueT * 255f;

            return (int)tempNumber;
        }

        public int getColorValue(String hex)
        {
            int tempNumber = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);

            return tempNumber;
        }

        public Bitmap getBitmap()
        {
            return image;
        }

        public ImageSource getImageSource()
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            ImageSource img = bi;

            return img;
        }

        private void setMaxHeight()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (heightMap[x, y] > maxHeight)
                    {
                        maxHeight = heightMap[x, y];
                    }
                }
            }
        }


        private void setMinHeight()
        {
            minHeight = heightMap[0, 0];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (heightMap[x, y] < minHeight)
                    {
                        minHeight = heightMap[x, y];
                    }
                }
            }
        }

    }
}