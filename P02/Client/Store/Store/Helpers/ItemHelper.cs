using Newtonsoft.Json;
using Store.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Helpers
{
    class ItemHelper
    {
        public static async Task<List<Item>> GetItemsFromJson(string doc)
        {
            List<Item> list;

            using (StreamReader reader = new StreamReader(doc))
            {
                string json = await reader.ReadToEndAsync();
                list = JsonConvert.DeserializeObject<List<Item>>(json);
            }

            return list;
        }
    }
}
