using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Store.Helpers.Net
{
    class RawStreamConnection
    {
        readonly TcpClient Client;
        readonly NetworkStream Stream;
        byte[] Buffer;

        public RawStreamConnection(string hostname, int port)
        {
            Client = new TcpClient(hostname, port);
            Stream = Client.GetStream();
        }

        public void Close()
        {
            Stream.Close();
            Client.Close();
        }

        private void Write()
        {
            Stream.Write(Buffer, 0, Buffer.Length);
            Stream.Flush();
        }

        public void Write(int val)
        {
            Buffer = System.BitConverter.GetBytes(val);
            Write();
        }

        public void Write(long val)
        {
            Buffer = System.BitConverter.GetBytes(val);
            Write();
        }

        public void Write(string message)
        {
            Write(System.Text.Encoding.ASCII.GetByteCount(message));

            Buffer = System.Text.Encoding.ASCII.GetBytes(message);
            Write();
        }

        public int ReadInt()
        {
            Buffer = new byte[4];
            Stream.Read(Buffer, 0, 4);

            return System.BitConverter.ToInt32(Buffer, 0);
        }

        public long ReadLong()
        {
            Buffer = new byte[8];
            Stream.Read(Buffer, 0, 8);

            return System.BitConverter.ToInt64(Buffer, 0);
        }

        public string ReadUTF()
        {
            int length = ReadInt();
            Buffer = new byte[length];
            Stream.Read(Buffer, 0, Buffer.Length);

            return System.Text.Encoding.ASCII.GetString(Buffer);
        }

        public BitmapImage ReadImageFile()
        {
            var imageName = ReadUTF();
            var length = ReadLong();

            Buffer = new byte[length];
            int i = 0;

            long received = 0;
            while (received < length)
            {
                var buffer = new byte[4096];
                int read = Stream.Read(buffer, 0, 4096);

                received += read;

                for (int j = 0; j < read; ++j, ++i)
                    Buffer[i] = buffer[j];

            }

            Console.WriteLine($"Received {imageName}.");

            BitmapImage image = new BitmapImage();
            using (MemoryStream stream = new MemoryStream(Buffer))
            {
                stream.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = stream;
                image.EndInit();
            }

            image.Freeze();

            return image;
        }
    }
}
