using System;
using System.Collections.Generic;

namespace GildedRose.Console
{
    /// <summary>
    /// Alternative implementation to the reference `LegacyInventoryManager`.
    /// The focus of this implementation is to provide code readability.
    /// </summary>
    public class HAL9000InventoryManager : IInventoryManager
    {
        /// <summary>
        /// A dictionary of pre-defined items that have specific behavior.
        /// Items are first matched to this map, before anything else.
        /// </summary>
        public IDictionary<string, MerchandiseType> MerchandiseTypeMap { get; set; }
        public HAL9000InventoryManager()
        {
            MerchandiseTypeMap = GenerateMerchandiseMap();
        }

        /// <summary>
        /// Implementation of the IInventoryManager interface
        /// </summary>
        /// <param name="inventory">Input inventory list</param>
        /// <returns>Processed inventory list</returns>
        public IList<Item> ProcessInventory(IList<Item> inventory)
        {
            var processedInventory = new List<Item>();
            foreach (var item in inventory)
            {
                MerchandiseType type = MerchandiseType.Normal;

                // Lookup in map to see if item has a specified category.
                if (!MerchandiseTypeMap.TryGetValue(item.Name, out type))
                {
                    // Unless specified in the map, apply the following assumptions
                    if (item.Name.StartsWith("Conjured"))
                    {
                        type = MerchandiseType.Conjured;
                    }
                    else if (item.Name.StartsWith("Backstage passes"))
                    {
                        type = MerchandiseType.EventPass;
                    }
                    else
                    {
                        type = MerchandiseType.Normal;
                    }
                    
                }
                Merchandise merchandise = null;
                switch (type)
                {
                    case MerchandiseType.AgedWell:
                        merchandise = new AgedWellItem(item);
                        break;

                    case MerchandiseType.Conjured:
                        merchandise = new ConjuredItem(item);
                        break;

                    case MerchandiseType.EventPass:
                        merchandise = new EventPassItem(item);
                        break;

                    case MerchandiseType.Legendary:
                        merchandise = new LegendaryItem(item);
                        break;

                    default:
                    case MerchandiseType.Normal:
                        merchandise = new NormalItem(item);
                        break;
                }
                merchandise.UpdateQuality();
                processedInventory.Add(merchandise);
            }
            return processedInventory;
        }

        /// <summary>
        /// Map of currently known Item types.
        /// </summary>
        /// <remarks>Does not include Conjured and Backstage passes - these are handled on the basis of their prefix</remarks>
        /// <returns>A dictionary that matches item names with MerchandiseTypes</returns>
        private IDictionary<string, MerchandiseType> GenerateMerchandiseMap()
        {
            return new Dictionary<string, MerchandiseType>()
            {
                { "Aged Brie", MerchandiseType.AgedWell },
                { "Sulfuras, Hand of Ragnaros", MerchandiseType.Legendary }
            };
        }
    }


}