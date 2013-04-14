/*      PPM Class
 *	    AUTHOR: STEWART TAYLOR
 *------------------------------------
 * 
 * 
 * Last Updated: 16/03/2013
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace PanguConnect
{
    class PPM
    {
        private string filePath;
        UnmanagedMemoryStream memStream;

        private string id;
        private int width;
        private int height;
        private int max;
        private float horzRes;

        private byte[] rgbValues = null;
        private double[,] heightData = null;
        private Bitmap ppmBitmap;

        public PPM()
        {
        }

        public PPM(string fPath, float hRes)
        {
            filePath = fPath;
            horzRes = hRes;
            getHeightMap();
        }

        public PPM(UnmanagedMemoryStream mStream)
        {
            memStream = mStream;
            readFromStream();
            mStream.Dispose();
        }

        protected void getHeightMap()
        {
            if (File.Exists(filePath))
            {
                FileStream fs = File.OpenRead(filePath);
                int buffer;
                do
                {
                    buffer = fs.ReadByte();
                    id += (char)buffer;
                } while (buffer != '\n' && buffer != ' ');

                string dimension = "";
                do
                {
                    buffer = fs.ReadByte();
                    dimension += (char)buffer;
                } while (buffer != '\n' && buffer != ' ');

                width = Convert.ToInt16(dimension);
                dimension = "";
                do
                {
                    buffer = fs.ReadByte();
                    dimension += (char)buffer;
                } while (buffer != '\n' && buffer != ' ');

                height = Convert.ToInt16(dimension);
                string maxRGB = "";
                do
                {
                    buffer = fs.ReadByte();
                    maxRGB += (char)buffer;
                } while (buffer != '\n' && buffer != ' ');

                max = Convert.ToInt16(maxRGB);
                rgbValues = new byte[height * width * 3];
                for (int i = 0; i < rgbValues.Length; i++)
                {
                    rgbValues[i] = (byte)fs.ReadByte();

                }

                heightData = new double[height + 1, width + 1];
                int pos = 0;
                float maxH = 0;
                float minH = 0;
                height += 1;
                width += 1;
                for (int x = 0; x < height - 1; x++)
                {
                    for (int y = 0; y < width - 1; y++)
                    {
                        heightData[x, y] = ((((rgbValues[pos] * 256) + rgbValues[pos + 1]) - Math.Pow(2, 15)) * horzRes);
                        if (heightData[x, y] > maxH)
                            maxH = (float)heightData[x, y];

                        if (heightData[x, y] < minH)
                            minH = (float)heightData[x, y];

                        pos += 3;
                    }
                }
                Console.WriteLine("max " + maxH + " min " + minH);
                fs.Close();
                fs.Dispose();
            }
            else
            {
                Console.WriteLine("DEM not found!");
            }

        }

        protected void readFromStream()
        {
            int buffer;
            do
            {
                buffer = memStream.ReadByte();
                id += (char)buffer;
            } while (buffer != '\n' && buffer != ' ');

            string dimension = "";
            do
            {
                buffer = memStream.ReadByte();
                dimension += (char)buffer;
            } while (buffer != '\n' && buffer != ' ');

            width = Convert.ToInt16(dimension);
            dimension = "";
            do
            {
                buffer = memStream.ReadByte();
                dimension += (char)buffer;
            } while (buffer != '\n' && buffer != ' ');

            height = Convert.ToInt16(dimension);
            string maxRGB = "";
            do
            {
                buffer = memStream.ReadByte();
                maxRGB += (char)buffer;
            } while (buffer != '\n' && buffer != ' ');

            max = Convert.ToInt16(maxRGB);
            rgbValues = new byte[height * width * 3];
            for (int i = 0; i < rgbValues.Length; i++)
            {
                rgbValues[i] = (byte)memStream.ReadByte();
            }

            memStream.Close();

            createBitmap();
        }


        protected void createBitmap()
        {
            try
            {
                ppmBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Rectangle rect = new Rectangle(0, 0, width, height);
                System.Drawing.Imaging.BitmapData bmpData = ppmBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, ppmBitmap.PixelFormat);
                IntPtr ptr = bmpData.Scan0;

                byte[] bgrValues = new byte[rgbValues.Length];
                for (int i = 0; i < rgbValues.Length; i += 3)
                {
                    bgrValues[i] = rgbValues[i + 2];
                    bgrValues[i + 1] = rgbValues[i + 1];
                    bgrValues[i + 2] = rgbValues[i];
                }

                System.Runtime.InteropServices.Marshal.Copy(bgrValues, 0, ptr, bgrValues.Length);
                ppmBitmap.UnlockBits(bmpData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating new bitmap: " + ex.ToString());
            }
        }


        public Bitmap getBitmap
        {
            get
            {
                return ppmBitmap;
            }
        }

        public double[,] getDEM
        {
            get
            {
                return heightData;
            }
        }

        public int getWidth
        {
            get
            {
                return width;
            }
        }
        public int getHeight
        {
            get
            {
                return height;
            }
        }
        public string getFilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
            }
        }
    }
}