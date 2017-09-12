using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{

    public class Item
    {
        static List<Item> itemList = new List<Item>();

        private string name;
        private int price;
        private int bidId;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public int Price
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
            }
        }

        public Item()
        {

        }
        public Item(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public Item ReturnItem(string name)
        {

            Item names = itemList.Find(x => x.Name == name);
            return names;
        }
       
        public int GetId(Item item)
        { 
            int id = itemList.Find(x => x.name == item.name).bidId;
            return id;
        }

        public void ChangePrice(Item item, int id)
        {
            Item it = itemList.Find(x => x.name == item.name);
            it.price = item.price;
            it.bidId = id;

            Gravel gr = Gravel.Instance;
            gr.gravelNow(item);
        }

        public void DeleteItem(Item item)
        {
            itemList.Remove(item);
        }

        public void fillList()
        {
            itemList.Clear();
            itemList.Add(new Item("Stol", 50));
            itemList.Add(new Item("Bord", 100));
            itemList.Add(new Item("Jakke", 500));
            itemList.Add(new Item("TV", 1000));
            itemList.Add(new Item("Computer", 5000));
        }

        public void showItemList(StreamWriter writer, StreamReader reader)
        {
            writer.WriteLine("Skriv en vares navn og dit bud for at lave et ");
            foreach (var items in itemList)
            {
                writer.WriteLine("Genstanden: {0} har prisen: {1}", items.Name, items.Price);
            }
        }
    }
}
