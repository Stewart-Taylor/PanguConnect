using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;

namespace PanguConnect
{
    class Connection
    {
        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void* getSocket([MarshalAs(UnmanagedType.LPStr)]String hName, int port);

        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void pan_protocol_start(void* sock);

        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void pan_protocol_finish(void* sock);

        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern char* pan_protocol_get_viewpoint_by_angle(void* sock, float x, float y, float z, float yw, float pi, float rl, ulong* size);

        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern char* pan_protocol_set_viewpoint_by_angle(void* sock, float x, float y, float z, float yw, float pi, float rl);

        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void pan_protocol_set_field_of_view(void* sock, float f);
 
        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void pan_protocol_set_aspect_ratio(void* sock, float r);
  
        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void pan_protocol_set_boulder_view(void* sock, ulong t, int tex);
  
        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void pan_protocol_get_lidar_pulse_result(void* sock, float x, float y, float z, float dx, float dy, float dz, float* r, float* a);
  
        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void pan_protocol_quit(void* sock);

        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void pan_protocol_get_surface_elevations(void* s, char boulders, ulong n, float* posv, float* resultv, char* errorv);

        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void pan_protocol_get_elevations(void* s, ulong n, float* posv, float* resultv, char* errorv);

        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern float pan_protocol_get_elevation(void* s, char* perr);

        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern char* pan_protocol_get_image(void* sock, ulong* s);

        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void pan_protocol_get_surface_patch(void* sock, char boulders, float cx, float cy, ulong nx, ulong ny, float d, float theta, float* rv, char* ev);

        [DllImport(@"PanguDLLs\DLLpangu.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void getElevationModel(void*  s,char b, float d, int sizeX , int sizeY , float[] elevationGrid);

        private bool connected = false;
        private unsafe void* sock;

        public void connect(string hostname, int port)
        {
            unsafe
            {
                byte[] host = new byte[hostname.Length];
                host = Encoding.ASCII.GetBytes(hostname + "\0"); 
                try
                {
                    sock = getSocket(hostname, port); 

                    pan_protocol_start(sock); 
                    connected = true;
                    Console.WriteLine("Connected to PANGU");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public double[,] getDEM(string fpath, float horzRes , int width , int height)
        {
            
               double[,] d = null;

               unsafe
               {
                   
                   int xT = 0;
                   int yT = 0;
                   int tempT = 0;
               
                   try
                   {

                       float cx = 0.0f;
                       float cy = 0.0f;
                       int nx = width;
                       int ny = height;
                       float distance = horzRes;
                       float theta = 0.0f;
                       char b = '1';

                       float[] elevationGrid = new float[(nx * ny)];
                       
                       int sizeX = 10;

                       d = new double[nx, ny];


                       getElevationModel(sock,b, distance, nx, ny, elevationGrid);

                       int temp = 0;
                       for (int x = 0; x < (int)nx; x++)
                       {
                           for (int y = 0; y < (int)ny; y++)
                           {
                              
                               d[x, y] = elevationGrid[temp];
                               temp++;
                               tempT = temp;
                               xT = x;
                               yT = y;
                           }
                           
                           tempT = temp;
                       }

                       distance = 2;
                   }
                   catch (Exception ex)
                   {
                       xT = xT;
                       yT = yT;
                       tempT = tempT;
                   }

               }
            

              return d;
        }//attempt to use the surface patch to fetch terrain data, does not work*/



        public void disconnect()
        {
            unsafe
            {
                pan_protocol_finish(sock);//disconnect
                connected = false;
                Console.WriteLine("Disconnected from PANGU server successfully");
            }
        }//disconnect from server


        public Bitmap getImage(float x, float y, float z, float yaw, float pitch, float roll)
        {
            unsafe
            {
                pan_protocol_set_aspect_ratio(sock, 1); //set aspect ratio
                pan_protocol_set_boulder_view(sock, 0, 0);//turn boulders off
                pan_protocol_set_field_of_view(sock, 30.0f);//set field of view
                

                ulong todo =1024;
                char* img;
                //  pan_protocol_set_viewpoint_by_angle(sock, 0, 0, 1866, -90, -90, 0); //set the image
                img = pan_protocol_get_viewpoint_by_angle(sock, x, y, z, yaw, pitch, roll, &todo); //get the image
                UnmanagedMemoryStream readStream = new UnmanagedMemoryStream((byte*)img, (long)todo);

                PPM ppm = new PPM(readStream);//convert the image
                readStream.Close(); //tidy up
                readStream.Dispose();//tidy up
                return ppm.getBitmap;
            }
        }//returns bitmap image converted from PANGU stream



        public float getHeight(float x, float y, float z, float yaw, float pitch, float roll)
        {
            float height = 0;
            float[,] heightMap = new float[512, 512];
            unsafe
            {
                ulong todo = 1024;
                char* perr = null;


                pan_protocol_get_viewpoint_by_angle(sock, x, y, z, yaw, pitch, roll, &todo); //get the image
                height = pan_protocol_get_elevation(sock, perr);


               


                for (int a = 0; a < 512; a++)
                {
                    for (int b = 0; b < 512; b++)
                    {
                     //   pan_protocol_get_viewpoint_by_angle(sock, a, b, 100, 0, 90, 0, &todo);
                        heightMap[a,b] = pan_protocol_get_elevation(sock, perr);
                    }
                    Console.Out.WriteLine("New Raster");
                }



            }

            heightMap[0, 0] = heightMap[0, 0];

            return height;
        }


        public Bitmap getCollected(int rows, int columns, float halflength, float halffov)
        {
            float xoffset = halflength / (rows); //calculate the offset
            float yoffset = halflength / (columns);
            double degtorad = halffov * (Math.PI / 180); //convert fov to rads
            float zoom = (float)((xoffset) / Math.Tan(degtorad)); //calculate the required zoom

            float[] xcoords = new float[columns];
            float[] ycoords = new float[rows];

            //set the coords for our columns, this si the x coord the image will be taken from
            for (int c = 0; c < columns; c++)
            {
                if (c == 0)
                    xcoords[c] = (-halflength + xoffset);
                else
                    xcoords[c] = (xcoords[c - 1] + (xoffset * 2));
            }

            //this gets the y coord the image will be taken from
            for (int r = 0; r < rows; r++)
            {
                if (r == 0)
                    ycoords[r] = (halflength - yoffset);
                else
                    ycoords[r] = (ycoords[r - 1] - (yoffset * 2));
            }

            Image[,] coll = new Image[rows, columns];//collection of images
            //get the images
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    coll[r, c] = getImage(xcoords[c], ycoords[r], zoom, -90, -90, 0); // -90 yaw and -90 pitch to orient the camera
                }
            }
            Bitmap collection = null;
            try
            {
                collection = Combine(coll, rows, columns);
                return collection;
            }
            catch (Exception ex)
            {
                if (collection != null)
                    collection.Dispose();

                Console.WriteLine("Error fetching image: " + ex);
                Console.WriteLine("Try restarting server");
            }
            finally
            {
                // collection.Dispose();//cleanup
            }
            return collection;
        }//gets collection of images and returns them combined into a single image
        private Bitmap Combine(Image[,] collection, int rows, int columns)
        {
            List<Bitmap> images = new List<Bitmap>();
            Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        Bitmap bit = new Bitmap(collection[r, c]);
                        images.Add(bit);
                    }
                    Bitmap bit2 = new Bitmap(collection[r, 0]);
                    height += bit2.Height;//modify the height and width of the total image so everything fits
                    width += bit2.Width;
                }

                finalImage = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(finalImage))
                {
                    //iterate through list and add the images to our final image
                    g.Clear(Color.Black);
                    int xoffset = 0;
                    int yoffset = 0;
                    for (int r = 0; r < rows; r++)
                    {
                        for (int c = 0; c < columns; c++)
                        {
                            g.DrawImage(collection[r, c], new Rectangle(xoffset, yoffset, collection[r, c].Width, collection[r, c].Height));
                            xoffset += (int)(collection[r, c].Width);
                        }
                        xoffset = 0;//reset column
                        yoffset += collection[0, 0].Height;//offset the rows
                    }
                }


                return finalImage;
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                    finalImage.Dispose();

                throw ex;
            }
            finally
            {
                //clean up
                foreach (Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }//combines the images into a single image
        public bool getConnectStatus
        {
            get
            {
                return connected;
            }
        }//returns connection status
    }
}