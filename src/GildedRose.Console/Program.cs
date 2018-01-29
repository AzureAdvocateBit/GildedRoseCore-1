using System.Collections.Generic;

namespace GildedRose.Console
{
    class Program
    {
        IList<Item> Items;
        IInventoryManager InventoryManager;
        ISupplyManager SupplyManager;

        static void Main(string[] args)
        {
            System.Console.WriteLine("OMGHAI!");

            var app = new Program()
            {
                InventoryManager = new HAL9000InventoryManager(),
                SupplyManager = new SupplyManager()
            };
            app.PurchaseNewSupplies();

            app.UpdateQuality();

            System.Console.ReadKey();
        }

        public void UpdateQuality()
        {
            Items = InventoryManager.ProcessInventory(Items);
        }

        public void PurchaseNewSupplies()
        {
            Items = SupplyManager.PurchaseDefaultSupplies();
        }
    }

    public class Item
    {
        public string Name { get; set; }

        public int SellIn { get; set; }

        public int Quality { get; set; }
    }
}
