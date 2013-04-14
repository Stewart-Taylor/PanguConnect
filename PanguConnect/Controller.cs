using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PanguConnect
{
    public class Controller
    {

        Connection con = new Connection();
        String hostname = "localhost";
        int port;

            public  Controller()
            {
                port = 12; // Convert.ToInt32(hostname);
            }


            public void connect()
            {
                con.connect(hostname, port);

                Bitmap img = con.getImage(0.2f, 0.4f, 0.4f, 12f, 0f, 0f);
            }


    }
}
