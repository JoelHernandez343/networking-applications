using Store.Helpers.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Helpers
{
    class ServerConnection
    {
        public static RawStreamConnection stream;

        public static bool Initialize(string hostname, int port)
        {
            try
            {
                stream = new RawStreamConnection(hostname, port);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
