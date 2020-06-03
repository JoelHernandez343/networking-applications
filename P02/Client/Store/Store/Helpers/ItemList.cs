using Store.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Helpers
{
    class ItemList
    {
        public static List<Item> List;
        
        public static async Task Initialize()
        {
            List = await ServerConnection.GetItemsAsync();
            await GetImages();
        }

        public static async Task Update()
        {
            await Initialize();
        }

        static async Task GetImages()
        {
            foreach (Item item in List)
            {
                item.Images = await ServerConnection.GetImagesAsync(item);
            }
        }
    }
}
