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
            List = await ItemHelper.GetItemsFromJson("Assets/Json/db.json");
        }

        public static async Task Update()
        {
            await Initialize();
        }
    }
}
