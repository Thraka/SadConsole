using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicTutorial.Items
{
    class Inventory
    {
        public enum ActionResult
        {
            Failure,
            Success
        }

        private List<Item> carriedItems = new List<Item>();
        private Item head;
        private Item leftHand;
        private Item rightHand;
        private Item feet;
        private Item body;

        public const int MaxCarriedItems = 11;

        public IEnumerable<Item> CarriedItems => carriedItems;

        public Item Head => head;
        public Item LeftHand => leftHand;
        public Item RightHand => rightHand;
        public Item Feet => feet;
        public Item Body => body;

        public ActionResult AddItem(Item item, bool carried)
        {
            if (carried || item.Spot == InventorySpot.None)
            {
                if (carriedItems.Count == MaxCarriedItems)
                    return ActionResult.Failure;

                carriedItems.Add(item);
                return ActionResult.Success;
            }

            switch (item.Spot)
            {
                case InventorySpot.Head:
                    head = item;
                    break;
                case InventorySpot.LHand:
                    leftHand = item;
                    break;
                case InventorySpot.RHand:
                    rightHand = item;
                    break;
                case InventorySpot.Body:
                    body = item;
                    break;
                case InventorySpot.Feet:
                    feet = item;
                    break;
                default:
                    break;
            }
            return ActionResult.Success;
        }

        public ActionResult RemoveItem(Item item)
        {
            if (carriedItems.Contains(item))
            {
                carriedItems.Remove(item);
                //drop item
            }
            else
            {
                switch (item.Spot)
                {
                    case InventorySpot.Head:
                        if (head == item)
                            head = null;
                        break;
                    case InventorySpot.LHand:
                        if (leftHand == item)
                            leftHand = null;
                        break;
                    case InventorySpot.RHand:
                        if (rightHand == item)
                            rightHand = null;
                        break;
                    case InventorySpot.Body:
                        if (body == item)
                            body = null;
                        break;
                    case InventorySpot.Feet:
                        if (feet == item)
                            feet = null;
                        break;
                    default:
                        break;
                }
            }

            return ActionResult.Success;
        }

        public bool IsSlotEquipped(InventorySpot spot)
        {
            switch (spot)
            {
                case InventorySpot.Head:
                    return head != null;
                case InventorySpot.LHand:
                    return leftHand != null;
                case InventorySpot.RHand:
                    return rightHand != null;
                case InventorySpot.Body:
                    return body != null;
                case InventorySpot.Feet:
                    return feet != null;
                default:
                    return false;
            }
        }

        public bool IsInventoryFull()
        {
            return carriedItems.Count == MaxCarriedItems;
        }
        public Item GetItem(InventorySpot spot)
        {
            switch (spot)
            {
                case InventorySpot.Head:
                    return head;
                case InventorySpot.LHand:
                    return leftHand;
                case InventorySpot.RHand:
                    return rightHand;
                case InventorySpot.Body:
                    return body;
                case InventorySpot.Feet:
                    return feet;
                default:
                    return null;
            }
        }

        public IEnumerable<Item> GetEquippedItems()
        {
            List<Item> items = new List<Item>(5);

            if (head != null)
                items.Add(head);
            if (leftHand != null)
                items.Add(leftHand);
            if (rightHand != null)
                items.Add(rightHand);
            if (body != null)
                items.Add(body);
            if (feet != null)
                items.Add(feet);

            return items;
        }
    }
}
