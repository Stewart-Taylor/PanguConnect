using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace PanguConnect
{
    public partial class MainWindow : Window
    {
        Connection Pangu;

        static string DEMfile, hostname; 
        static float DEMHorzRes, maxGrad, hRes;
        static int LZRadius, port; 

        static bool DEMcheck = true;

        static double[,] data; 

        static float x = 0;
        static float y = 0;
        static float z = -70;
        static float yaw = 180;
        static float pitch = 90;
        static float roll = 0;

        public MainWindow()
        {
            InitializeComponent();

            startPANGU();
        }


        private void startPANGU()
        {
            String filename = "C:/Users/Stewart/Desktop/Pangu3.30/Pangu3.30/models/PathPlanner_Model/viewer.bat";

            System.Diagnostics.Process proc = new System.Diagnostics.Process(); // Declare New Process
            proc.StartInfo.FileName = filename;
            proc.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName("C:/Users/Stewart/Desktop/Pangu3.30/Pangu3.30/models/PathPlanner_Model/");
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();



        }


        private void btn_Connect_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            Pangu = new Connection();    

            configFileRead("config.ini"); 

            try
            {
                Pangu.connect(hostname, port);

                Bitmap bitmap = Pangu.getImage(x, y, z, yaw, pitch, roll);

                img_main.Source = getImageFromBitmap(bitmap);

                String temp = txt_distance.Text;
                hRes = float.Parse(temp);

                temp = txt_width.Text;

                int width = int.Parse(temp);

                temp = txt_height.Text;
                int height = int.Parse(temp);

                data = Pangu.getDEM(@"DEM/" + DEMfile, hRes ,  width ,  height);

                Pangu.disconnect();

                ElevationLoader eL = new ElevationLoader(data);

                img_elv.Source = eL.getImageSource();
            }
            catch (Exception ex)
            {
                Pangu.disconnect();
                throw ex;
            } 
        }


        public ImageSource getImageFromBitmap(Bitmap b)
        {
            MemoryStream ms = new MemoryStream();
            b.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            ImageSource img = bi;

            return img;
        }

        static bool configFileRead(string fp)
        {
            FileStream FS;
            try
            {
                FS = new FileStream(fp, FileMode.Open);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message + "cannot open file: " + fp.ToString());
                setDefaultVariables();
                return false;
            }

            StreamReader fstr_in = new StreamReader(FS);
            String line;
            while ((line = fstr_in.ReadLine()) != null)
            {
                setVariable(line);
            }
            fstr_in.Close();
            hostname = "localhost";
            return true;
        }

        static void setVariable(String line)
        {
            string[] t;
            string tt = null;
            line = line.ToLower();
            t = line.Split('\t');

            foreach (string s in t)
            {
                if (s != t[0] && s != "")
                    tt = s;
            }
            switch (t[0])
            {
                case "demfile":
                    if (File.Exists(@"DEM\" + tt.ToString()))
                    {
                        DEMfile = tt;
                        DEMcheck = true;
                    }
                    else
                    {
                        Console.WriteLine("couldn't find " + tt + " in DEM folder");
                        DEMcheck = false;
                    }
                    break;
                case "hres":
                    hRes = (float)Convert.ToDecimal(tt);
                    break;
                case "demhorzres":
                    DEMHorzRes = Convert.ToSingle(tt);
                    break;
                case "maxgrad":
                    maxGrad = Convert.ToSingle(tt);
                    break;
                case "lzradius":
                    LZRadius = Convert.ToInt32(tt);
                    break;
                case "hostname":
                    hostname = tt;
                    break;
                case "port":
                    port = Convert.ToInt32(tt);
                    break;
                default:
                    break;
            }
        }

        static void setDefaultVariables()
        {
            if (File.Exists(@"DEM\400.ppm"))
            {
                DEMfile = "400.ppm";
                DEMcheck = true;
            }
            else
            {
                DEMcheck = false;
                Console.WriteLine("couldn't find 400.ppm in DEM folder");
            }
            hostname = "localhost";
            port = 10363;
 
        } 



        private void btn_Disconnect_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	
        }

        private void btn_UpdateImage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Pangu = new Connection();

            try
            {
                Pangu.connect(hostname, port);


                float x = float.Parse(txt_X.Text);
                float y = float.Parse(txt_Y.Text);
                float z = float.Parse(txt_Z.Text);
                float yaw = float.Parse(txt_Yaw.Text);
                float pitch = float.Parse(txt_Pitch.Text);
                float roll = float.Parse(txt_Roll.Text);

                Bitmap bitmap = Pangu.getImage(x, y, z, yaw, pitch, roll);

                img_main.Source = getImageFromBitmap(bitmap);

                Pangu.disconnect();

            }
            catch (Exception ex)
            {
                Pangu.disconnect();
                //throw ex;
            }
        }
    }
}
