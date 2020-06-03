using Newtonsoft.Json;
using Store.Helpers.Net;
using Store.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Store.Helpers
{
    class ServerConnection
    {
        public static RawStreamConnection Stream;

        public static bool Initialize(string hostname, int port)
        {
            try
            {
                Stream = new RawStreamConnection(hostname, port);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<List<Item>> GetItemsAsync()
        {
            RequestDb();
            String itemList = await Task.Run(() => Stream.ReadUTF());
            SendOk();

            return JsonConvert.DeserializeObject<List<Item>>(itemList);
        }

        public static async Task<List<BitmapImage>> GetImagesAsync(Item item)
        {
            var images = new List<BitmapImage>();

            RequestImages();
            Stream.Write(item.Key);

            for (int i = 0; i < item.ImageNames.Count; ++i)
            {
                RequestImage();
                Stream.Write(item.ImageNames[i]);

                images.Add(await Task.Run(() => Stream.ReadImageFile()));

                SendOk();
            }

            FinishImageRequest();
            return images;
        }

        public static async Task MakePurchase(String email, int cardNumber, double total, List<PurchaseUpdate> purchased)
        {
            PostPurchase();
            Stream.Write(email);
            Stream.Write(cardNumber);
            Stream.Write(total.ToString());

            await WriteJsonAsync<List<PurchaseUpdate>>(purchased);

            WaitOk();
        }

        public static async Task WriteJsonAsync<T>(T t)
        {
            var json = JsonConvert.SerializeObject(t);
            await Task.Run(() => Stream.Write(json));
        }

        public static void WaitOk() 
        {
            if (Stream.ReadInt() != (int)Instruction.OK)
                throw new InvalidOperationException("Illegal response by the server.");
        }

        public static void RequestDb() => Stream.Write((int)Instruction.REQUEST_DB);
        public static void RequestImages() => Stream.Write((int)Instruction.REQUEST_IMAGES);
        public static void RequestImage() => Stream.Write((int)Instruction.REQUEST_ONE_IMAGE);
        public static void FinishImageRequest() => Stream.Write((int)Instruction.FINISHED_PROCESS);

        public static void SendOk() => Stream.Write((int)Instruction.OK);
        public static void PostPurchase() => Stream.Write((int)Instruction.POST_PURCHASE);

        public static void Close()
        {
            if (Stream != null)
            {
                Stream.Write((int)Instruction.DISCONNECT);
                Stream.Close();
            }
        }
        
    }
}
