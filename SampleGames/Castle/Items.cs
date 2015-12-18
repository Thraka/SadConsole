using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole.Instructions;

namespace Castle
{

    internal class UserMessage
    {
        public Collection<String> Messages { get; private set; }
        public int? Character { get; set; }

        public UserMessage()
        {

            Messages = new Collection<string>();
        }
        public UserMessage(params String[] messages) : 
            this()
        {
            Messages = new Collection<string>();
            if(messages != null)
            {
                foreach(var message in messages)
                {
                    AddLine(message);
                }
            }
        }

        public void AddLine(String line)
        {
            int index = 0;
            while (index < line.Length)
            {
                if (line.Length - index < 14)
                {
                    this.Messages.Add(line.Substring(index));
                }
                else
                {
                    this.Messages.Add(line.Substring(index, 14));
                }
                index += 14;
            }
        }


        
    }

    internal class ItemManager
    {

        private const int maxItems = 6;
        private const string upStairsDescription = " The Staircase leads up.";
        private const string downStairsDescription = " The Staircase leads down.";
        private const string upDownStairsDescription = " The Staircase leads both    up & Down..";
        private const string goldDescription = " It's made of  Gold!";
        private const string expensiveDescrition = " It looks      Expensive!";



        public Collection<CastleItem> CastleItems { get; private set; }
        private Collection<CastleFurnature> castleFurnatureItems;

        private Collection<CastleItem> currentInventory;
        public Collection<CastleItem> CurrentRoomItems;
        private Collection<CastleFurnature> currentRoomFurnatureItems;

        public Collection<Monster> CastleMonsters { get; private set; }
        public Collection<Monster> CurrentRoomMonsters { get; private set; }

        private int currentRoomIndex;
        private UpdateMapCb updateMapCb;
        

        public ItemManager(UpdateMapCb updateMapCb)
        {
            currentRoomIndex = 1;
            this.updateMapCb = updateMapCb;
            CastleItems = new Collection<CastleItem>();
            castleFurnatureItems = new Collection<CastleFurnature>();
            currentInventory = new Collection<CastleItem>();
            CurrentRoomItems = new Collection<CastleItem>();
            currentRoomFurnatureItems = new Collection<CastleFurnature>();

            CastleMonsters = new Collection<Monster>();
            CurrentRoomMonsters = new Collection<Monster>();

            CreateCastleItems();
            CreateCastleMonsters();
        }

        private void CreateCastleItems()
        {

            // Items that can be picked up

            CastleItems.Add(new CastleItem("SWORD", 197, new Point(10, 4), 0, " It looks      Sharp!", "SWORD", 12, "Sword"));
            CastleItems.Add(new CastleItem("FANCY GOBLET", 230, new Point(5, 13), 50, goldDescription, "GOBLET", 8, "Goblet"));
            CastleItems.Add(new CastleItem("LAMP",6, new Point(18, 13), 0, " It's lit      Magically",  "LAMP", 17, "Lamp"));
            CastleItems.Add(new CastleItem("CROWN", 127, new Point(8, 4), 50, goldDescription, "CROWN", 14, "Crown"));
            CastleItems.Add(new CastleItem("HOURGLASS", 232, new Point(8, 4), 50, goldDescription, "HOURGLASS", 20, "Hourglass"));
            CastleItems.Add(new CastleItem("SILVER BARS", 240, new Point(6, 7), 50, expensiveDescrition, "BARS", 26, "Bars"));
            CastleItems.Add(new CastleItem("JADE FIGURINE", 157, new Point(12, 9), 50, expensiveDescrition, "FIGURINE", 29, "Figurine"));
            CastleItems.Add(new CastleItem("HOLY CROSS", 116, new Point(14, 12), 50, goldDescription, "CROSS", 37, "Cross"));
            CastleItems.Add(new CastleItem("DIAMOND", 4, new Point(12, 9), 50, expensiveDescrition, "DIAMOND", 57, "Diamond"));
            CastleItems.Add(new CastleItem("RUBYS", 58, new Point(3, 15), 50, expensiveDescrition, "RUBYS", 60, "RUBYS"));
            CastleItems.Add(new PlayableCastleItem("HARP", 14, new Point(15, 7), goldDescription, "The Harp makesBeautiful     Music!", 50, "HARP", 65, "HARP"));
            CastleItems.Add(new WearableCastleItem("HELMET", 155, new Point(11, 5), " It looks      Tough!", "OK.           I'm wearing it", 0, "HELMET", 22, "Helmet"));
            CastleItems.Add(new CastleItem("CRYSTAL BALL", 248, new Point(9, 11), 0, "In the crystal Ball....     You see a man in a winding  passage,wavinga Wand.", "BALL", 66, "BALL"));
            CastleItems.Add(new CastleItem("GOLDBAR", 254, new Point(22, 13), 50, expensiveDescrition, "GOLDBAR", 74, "GOLDBAR"));

            WaveItem scepterItem = new WaveItem(updateMapCb, "SCEPTER", 225, new Point(11, 9), expensiveDescrition, 50, "SCEPTER", 83, "SCEPTER");
            scepterItem.AddSpecialRoomAction(new GateRoom());
            CastleItems.Add(scepterItem);

            WaveItem wandItem = new WaveItem(updateMapCb, "MAGIC WAND", 196, new Point(3, 8), " It looks      Magical!", 0, "WAND", 24, "Wand");
            wandItem.AddSpecialRoomAction(new SorcerersRoom());
            wandItem.AddSpecialRoomAction(new WindingPasage2());
            CastleItems.Add(wandItem);

            // Items that cannot be picked up
            castleFurnatureItems.Add(new CastleFurnature(null, "It looks      very Strong", null, 1, "Gate"));
            castleFurnatureItems.Add(new CastleFurnature(null, "Writing on thewall says...                The First Day    2006", null, 1, "Wall"));
            castleFurnatureItems.Add(new CastleFurnature(null, upStairsDescription, null, 4, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upStairsDescription, null, 5, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upStairsDescription, null ,7, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature("a Table.", "It's made of  Stone.", "TABLES", 9, "Table"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 11, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature("", " They're made  of wood.", "SHELVES", 11, "Shelves"));
            castleFurnatureItems.Add(new CastleFurnature("a throne!", "The throne is made of stone.", "THRONES, ", 14, "Throne"));
            castleFurnatureItems.Add(new CastleFurnature(null, upStairsDescription, null, 15, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upStairsDescription, null, 16, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 20, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 21, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 23, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upStairsDescription, null, 25, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upStairsDescription, null, 26, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 30, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 31, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upStairsDescription, null, 32, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature("a Balcony!", "The balcony ismade of Stone.", null, 33, "Balcony"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 34, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature("a Bed.", " It's Red.", "BEDS", 37, "Bed"));
            castleFurnatureItems.Add(new CastleFurnature("a Bed.", " It's Blue.", "BEDS", 38, "Bed"));
            castleFurnatureItems.Add(new CastleFurnature("a Bed.", " It's Purple.", "BEDS", 39, "Bed"));
            castleFurnatureItems.Add(new CastleFurnature("a Bed.", " It's Yellow.", "BEDS", 40, "Bed"));
            castleFurnatureItems.Add(new CastleFurnature(null, upStairsDescription, null, 41, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upDownStairsDescription, null, 42, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 43, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upDownStairsDescription, null, 45, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 48, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 49, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upDownStairsDescription, null, 50, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 53, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature("", " They're made  of wood.", "SHELVES", 59, "Shelves"));
            castleFurnatureItems.Add(new CastleFurnature(null, upDownStairsDescription, null, 61, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upDownStairsDescription, null, 62, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upDownStairsDescription, null, 63, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, downStairsDescription, null, 64, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, upDownStairsDescription, null, 65, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature("a Bed.", " It's Old.", "BEDS", 67, "Bed"));
            castleFurnatureItems.Add(new CastleFurnature("A Mirror.", " It looks      Magical", "MIRRORS", 67, "Mirror"));
            castleFurnatureItems.Add(new CastleFurnature(null, upStairsDescription, null, 68, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature("a Barrel.", " They look     old!", "BARRELS", 68, "Barrels"));
            castleFurnatureItems.Add(new CastleFurnature("a Door.", " It looks      very Strong!", "DOORS", 68, "Door"));
            castleFurnatureItems.Add(new CastleFurnature("a Table.", " It's covered  with Blood!", "TABLES", 79, "Table"));
            castleFurnatureItems.Add(new CastleFurnature(null, " They look     Magical!", "CHAINS", 79, "Chains"));
            castleFurnatureItems.Add(new CastleFurnature("a Door.", " It looks      very Strong!", "DOORS", 82, "Door"));
            castleFurnatureItems.Add(new CastleFurnature(null, upStairsDescription, null, 82, "Stairs"));
            castleFurnatureItems.Add(new CastleFurnature(null, "Writing on thewall says...                Kevin Bales    1550", null, 83, "Wall"));
            castleFurnatureItems.Add(new CastleFurnature(null, "The walls are many Colors!", null, 83, "Wall"));

            // Universal Items
            castleFurnatureItems.Add(new CastleFurnature(null, "The WALLS     are made of   Gray Stone.", null, 0, "Wall"));
            castleFurnatureItems.Add(new CastleFurnature(null, "The FLOORS    are made of   Gray Stone.", null, 0, "Floor"));
            castleFurnatureItems.Add(new CastleFurnature(null, "The CEILINGS  are made of   Gray Stone.", null, 0, "Ceiling"));
            castleFurnatureItems.Add(new CastleFurnature(null, " I don't see   any stairs!", null, 0, "Stairs"));


            // Desk / Flask 
            CastleItem flaskItem = new DrinkableItem("FLASK", 33, new Point(0, 0), " It's empty.", " It's filled   with water.", 0, "FLASK", -1, "Flask");
            CastleItems.Add(flaskItem);
            Collection<Point> deskFlaskPoints = new Collection<Point>();
            deskFlaskPoints.Add(new Point(6, 9));
            deskFlaskPoints.Add(new Point(7, 9));
            deskFlaskPoints.Add(new Point(8, 9));
            deskFlaskPoints.Add(new Point(9, 9));
            deskFlaskPoints.Add(new Point(6, 12));
            deskFlaskPoints.Add(new Point(7, 12));
            deskFlaskPoints.Add(new Point(8, 12));
            deskFlaskPoints.Add(new Point(9, 12));
            CastleFurnatureContainer deskWithFlask = new CastleFurnatureContainer("a desk!", " There is a    Wine Flask    on top.", " It's made     of Wood.", flaskItem, deskFlaskPoints, "DESKS", 10, "Desk");
            castleFurnatureItems.Add(deskWithFlask);

            // Gem / Fountain / Garden
            CastleItem gem = new CastleItem("GEM", 233, new Point(0, 0), 50, expensiveDescrition, "GEM", -1, "Gem");
            CastleItems.Add(gem);
            Collection<Point> gemFountainPoints = new Collection<Point>();
            gemFountainPoints.Add(new Point(10, 7));
            gemFountainPoints.Add(new Point(11, 7));
            gemFountainPoints.Add(new Point(12, 7));
            gemFountainPoints.Add(new Point(9, 8));
            gemFountainPoints.Add(new Point(9, 9));
            gemFountainPoints.Add(new Point(9, 10));
            gemFountainPoints.Add(new Point(10, 11));
            gemFountainPoints.Add(new Point(11, 11));
            gemFountainPoints.Add(new Point(12, 11));
            gemFountainPoints.Add(new Point(13, 8));
            gemFountainPoints.Add(new Point(13, 9));
            gemFountainPoints.Add(new Point(13, 10));
            CastleFurnature garden = new CastleFurnature("A Garden!", " There is a    BIG gem in    the Garden    Fountain!", "GARDENS, ", 33, "Garden");
            CastleFurnatureFountain fountainWithGem = new CastleFurnatureFountain("A fountain!", "The fountain  is filled     with water.Butyou can't see In it.", "The Garden    Has A Fountainin the middle.", gem, garden, gemFountainPoints, "FOUNTAINS", 18, "Fountain");
            castleFurnatureItems.Add(garden);
            castleFurnatureItems.Add(fountainWithGem);

            // Statue / Necklace
            CastleItem necklace = new WearableCastleItem("NECKLACE", 21, new Point(0, 0), " On the back   it says:                   Protection    from Traps.", "OK.           I'm wearing it", 50, "NECKLACE", -1, "Necklace");
            CastleItems.Add(necklace);
            Collection<Point> necklacePoints = new Collection<Point>();
            necklacePoints.Add(new Point(10, 6));
            necklacePoints.Add(new Point(11, 6));
            necklacePoints.Add(new Point(12, 6));
            necklacePoints.Add(new Point(9, 7));
            necklacePoints.Add(new Point(9, 8));
            necklacePoints.Add(new Point(9, 9));
            necklacePoints.Add(new Point(10, 10));
            necklacePoints.Add(new Point(11, 10));
            necklacePoints.Add(new Point(12, 10));
            necklacePoints.Add(new Point(13, 7));
            necklacePoints.Add(new Point(13, 8));
            necklacePoints.Add(new Point(13, 9));
            CastleFurnatureContainer statueWithNecklace = new CastleFurnatureContainer("a statue!", " The statue is wearing a     Necklace", " The Statue    looks like    The King!", necklace, necklacePoints, "STATUE", 19, "Statue");
            castleFurnatureItems.Add(statueWithNecklace);

            // Desk / Glasses
            CastleItem glasses = new WearableCastleItem("EYE GLASSES", 236, new Point(0, 0), " They're       Bifocals", " OK.           I'm wearing   them.", 0, "GLASSES", -1, "Glasses");
            CastleItems.Add(glasses);
            Collection<Point> glassesPoint = new Collection<Point>();
            glassesPoint.Add(new Point(16, 10));
            glassesPoint.Add(new Point(17, 10));
            glassesPoint.Add(new Point(18, 10));
            glassesPoint.Add(new Point(16, 13));
            glassesPoint.Add(new Point(17, 13));
            glassesPoint.Add(new Point(18, 13));
            CastleFurnatureContainer deskWithGlasses = new CastleFurnatureContainer("a desk!", " There is a    Pair of Eye   Glasses on    Top.", " It's made     of Wood.", glasses, glassesPoint, "DESKS", 40, "Desk");
            castleFurnatureItems.Add(deskWithGlasses);

            // Desk /Key
            CastleItem key = new CastleItem("KEY", 231, new Point(0, 0), 0, " It looks Old!", "KEY", -1, "Key");
            CastleItems.Add(key);
            Collection<Point> keyPoint = new Collection<Point>();
            keyPoint.Add(new Point(9, 4));
            keyPoint.Add(new Point(10, 4));
            keyPoint.Add(new Point(11, 4));
            keyPoint.Add(new Point(12, 4));
            keyPoint.Add(new Point(13, 4));
            keyPoint.Add(new Point(14, 4));
            keyPoint.Add(new Point(8, 5));
            keyPoint.Add(new Point(8, 6));
            keyPoint.Add(new Point(8, 7));
            keyPoint.Add(new Point(9, 8));
            keyPoint.Add(new Point(10, 8));
            keyPoint.Add(new Point(11, 8));
            keyPoint.Add(new Point(12, 8));
            keyPoint.Add(new Point(13, 8));
            keyPoint.Add(new Point(14, 8));
            keyPoint.Add(new Point(15, 5));
            keyPoint.Add(new Point(15, 6));
            keyPoint.Add(new Point(15, 7));
            CastleFurnatureContainer deskWithKey = new CastleFurnatureContainer("a desk!", " There is a    Key on top", " It's made     of Wood.", key, keyPoint, "DESKS", 56, "Desk");
            castleFurnatureItems.Add(deskWithKey);

            // Shelf / Book
            CastleItem book = new BookItem("BOOK", 220, new Point(0, 0), " It is titled   'The Gate'", 0, "BOOK", -1, "Book");
            CastleItems.Add(book);
            Collection<Point> bookPoints = new Collection<Point>();
            for (int x = 6; x < 15; x++)
            {
                for (int y = 6; y < 15; y++)
                {
                    bookPoints.Add(new Point(x, y));
                }
            }
            for (int x = 16; x < 24; x++)
            {
                for (int y = 6; y < 15; y++)
                {
                    bookPoints.Add(new Point(x, y));
                }
            }
            CastleFurnatureContainer shelvesWithBook = new CastleFurnatureContainer("", "There's a bookon one.", " It's made     of Wood.", book, bookPoints, "Shelves", 58, "Shelves");
            castleFurnatureItems.Add(shelvesWithBook);


        }
        private void CreateCastleMonsters()
        {
            CastleMonsters.Add(new Monster("SNAKE", "SNAKE", 17, 15, 12, 235, 10, "The SNAKE     looks Mean!", "The SNAKE     looks dead!"));
            CastleMonsters.Add(new Monster("ANGRY BALROG", "BALROG", 14, 15, 12, 1, 20, "The BALROG     looks Angry!", "The BALROG     looks dead!"));
            CastleMonsters.Add(new Monster("UGLY OGRE", "OGRE", 4, 13, 10, 2, 15, "The OGRE     looks Angry!", "The OGRE       looks dead!"));
            CastleMonsters.Add(new Monster("UGLY OGRE", "OGRE", 21, 10, 11, 2, 15, "The OGRE     looks Angry!", "The OGRE       looks dead!"));
            CastleMonsters.Add(new Monster("ANGRY BALROG", "BALROG", 24, 11, 7, 1, 20, "The BALROG     looks Angry!", "The BALROG     looks dead!"));
            CastleMonsters.Add(new Monster("BAT", "BAT", 68, 16, 7, 40, 2, "The BAT        looks Angry!", "The BAT        looks dead!"));
            CastleMonsters.Add(new Monster("SMALL SPIDER", "SPIDER", 70, 14, 4, 42, 10, "The SPIDER     looks Angry!", "The SPIDER     looks dead!"));
            CastleMonsters.Add(new Monster("BIG SPIDER", "SPIDER", 73, 18, 11, 15, 15, "The SPIDER     looks Angry!", "The SPIDER     looks dead!"));

            CastleMonsters.Add(new Guard("VAMPIRE", "VAMPIRE", 28, 23, 9, 5, "The VAMPIRE   looks Mean!", "HOLY CROSS", null, "The vampire   sees the holy cross and     disappears!"));
            CastleMonsters.Add(new Guard("FAIRY", "FAIRY", 54, 11, 0, 37, " The Fairy     looks pretty  but unhappy.", null, "HARP", "The Harp makesBeautiful     Music!        The fairy     likes the     music and     leaves!"));
            CastleMonsters.Add(new Guard("FAIRY", "FAIRY", 52, 0, 9, 37,  " The Fairy     looks pretty  but unhappy.", null, "HARP", "The Harp makesBeautiful     Music!        The fairy     likes the     music and     leaves!"));
        }

        private static CastleItem FindItemInList(Collection<CastleItem> list, CastleItem castleObject)
        {
            foreach(var item in list)
            {
                if (item == castleObject)
                {
                    return item;
                }

            }
            return null;
        }
        private static CastleItem FindItemInList(Collection<CastleItem> list, String searchName)
        {
            foreach (var item in list)
            {
                foreach (var name in item.parsingNames)
                {
                    if (searchName.StartsWith(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        private static CastleFurnature FindItemInList(Collection<CastleFurnature> list, String searchName)
        {
            foreach (var item in list)
            {
                foreach (var name in item.parsingNames)
                {
                    if (searchName.StartsWith(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return item;
                    }
                }
            }
            return null;
        }
        
        private CastleItem FindItemInInventory(CastleItem castleObject)
        {
            return FindItemInList(currentInventory, castleObject);
        }
        public CastleItem FindItemInInventory(String searchName)
        {
            return FindItemInList(currentInventory, searchName);
        }

        private CastleItem FindItemInRoom(CastleItem castleObject)
        {
            return FindItemInList(CurrentRoomItems, castleObject);
        }
        private CastleItem FindItemInRoom(String searchName)
        {
            return FindItemInList(CurrentRoomItems, searchName);
        }
        private CastleFurnature FindFurnatureInRoom(String searchName)
        {
            return FindItemInList(currentRoomFurnatureItems, searchName);
        }
        private CastleItem FindItem(String searchName)
        {
            return FindItemInList(CastleItems, searchName);
        }

        private CastleFurnature FindFurnature(String searchName)
        {
            return FindItemInList(castleFurnatureItems, searchName);
        }
        private CastleFurnatureContainer FindHiddenItem(String searchName)
        {
            foreach(CastleFurnature furnature in currentRoomFurnatureItems)
            {

                CastleFurnatureContainer container = furnature as CastleFurnatureContainer;
                if(container != null)
                {
                    if (container.Item != null)
                    {
                        foreach (string name in container.Item.parsingNames)
                        {
                            if (searchName.StartsWith(name, StringComparison.CurrentCultureIgnoreCase))
                            {
                                return container;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public void LoadRoomItems(int roomIndex, Point playerPosition)
        {
            this.currentRoomIndex = roomIndex;
            CurrentRoomItems.Clear();
            foreach (var item in CastleItems)
            {
                if(item.RoomIndex == roomIndex)
                {
                    CurrentRoomItems.Add(item);
                }

            }
            
            currentRoomFurnatureItems.Clear();
            foreach (var item in castleFurnatureItems)
            {
                if (item.RoomIndex == roomIndex || item.RoomIndex == 0)
                {
                    currentRoomFurnatureItems.Add(item);
                }

            }

            CurrentRoomMonsters.Clear();
            foreach(var monster in CastleMonsters)
            {
                if(monster.RoomId == roomIndex)
                {
                    if (monster.IsGuard)
                    {
                        if(monster.Position.X != playerPosition.X || monster.Position.Y != playerPosition.Y)
                        {
                            CurrentRoomMonsters.Add(monster);
                        }
                    }
                    else
                    {
                        CurrentRoomMonsters.Add(monster);
                    }
                }
            }



        }

        private Monster FindMonsterInRoom()
        {
            foreach (Monster monster in CurrentRoomMonsters)
            {
                return monster;
            }
            return null;
        }

        public bool IsItemAtPoint(Point point)
        {
            foreach(var item in CurrentRoomItems)
            {
                if(item.Location.X == point.X && item.Location.Y == point.Y)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsMonsterAtPoint(Point point)
        {
            foreach (var item in CurrentRoomMonsters)
            {
                if (item.Position.X == point.X && item.Position.Y == point.Y)
                {
                    return true;
                }
            }
            return false;
        }

        private UserMessage RunSpecialCommand(CommandVerb verb, String subject, Point location)
        {
            UserMessage returnMessage = null;
            foreach (var item in currentInventory)
            {
                if (item.IsSpecialCommandSupported(verb))
                {
                    returnMessage = item.RunCommand(verb, subject, currentRoomIndex, location, FindItemInInventory);
                }
            }
            return returnMessage;
        }

        public UserMessage PickupItem(Point point)
        {
            UserMessage returnMessage = new UserMessage();

            if (currentInventory.Count >= maxItems)
            {
                returnMessage.AddLine("Can't carry   any more!");
            }
            else
            {
                CastleItem pickupItem = null;
                foreach (var item in CurrentRoomItems)
                {
                    if (item.Location.X == point.X && item.Location.Y == point.Y)
                    {
                        pickupItem = item;
                        break;
                    }
                }
                CurrentRoomItems.Remove(pickupItem);
                currentInventory.Add(pickupItem);
                pickupItem.PickUp();

                
            }

            return returnMessage;
        }
        public UserMessage AttackMonster(Point point)
        {
            UserMessage returnMessage = new UserMessage();

            CastleItem sword = FindItemInInventory("Sword");
            Monster monster = FindMonsterInRoom();
            if(monster.Position.X == point.X && monster.Position.Y == point.Y)
            {
                returnMessage = monster.Hit(sword);
            }


            return returnMessage;
        }


        public UserMessage CommandInventory()
        {
            UserMessage returnMessage = new UserMessage();
            returnMessage.AddLine("- Inventory -");
            
            if(currentInventory.Count == 0)
            {
                returnMessage.AddLine(String.Empty);
                returnMessage.AddLine("   Nothing");
            }
            else
            {
                foreach(var item in currentInventory)
                {
                    StringBuilder newLine = new StringBuilder();
                    newLine.Append(Convert.ToChar(item.Character));
                    newLine.Append(" ");
                    newLine.Append(item.InventoryName);
                    returnMessage.AddLine(newLine.ToString());
                }
           }

            return returnMessage;
        }

        public UserMessage CommandLook(Command command)
        {
            UserMessage returnMessage = new UserMessage();

            CastleObject item = null;
            
            // Find all inventory objects
            item = FindItemInInventory(command.Subject);
            if(item != null)
            {
                returnMessage.AddLine(item.Description);
                return returnMessage;
            }

            // Find all Furnature objects
            item = FindFurnatureInRoom(command.Subject);
            if(item != null)
            {
                returnMessage.AddLine(item.Description);
                return returnMessage;
            }

            // Find all items
            item = FindItem(command.Subject);
            if (item != null)
            {
                returnMessage.AddLine(" You Don't");
                returnMessage.AddLine(" Have it!");
                return returnMessage;
            }

            // Find univesral
            item = FindFurnature(command.Subject);
            if (item != null)
            {
                CastleFurnature castleFurnature = item as CastleFurnature;
                if (castleFurnature.LookFailMessage != null)
                {
                    returnMessage.AddLine("I don't see");
                    returnMessage.AddLine(castleFurnature.LookFailMessage);
                    return returnMessage;
                }
            }

            // Find Monster
            Monster monster = FindMonsterInRoom();
            if(monster != null)
            {
                if(monster.IsAlive)
                {
                    returnMessage.AddLine(monster.Description);
                }
                else
                {
                    returnMessage.AddLine(monster.DeadDescription);
                }
                return returnMessage;
            }
            

            returnMessage.AddLine("I don't see");
            returnMessage.AddLine(String.Format("a {0}", command.Subject));
            returnMessage.AddLine("and I don't");
            returnMessage.AddLine("have a");
            returnMessage.AddLine(String.Format("{0}!", command.Subject));
            return returnMessage;
        }
        public UserMessage CommandTake(Command command, Point location)
        {
            UserMessage returnMessage = new UserMessage();

            // Look in inventory
            CastleItem item = FindItemInInventory(command.Subject);
            if (item != null)
            {
                returnMessage.AddLine(" You already");
                returnMessage.AddLine(" have it!");
                return returnMessage;
            }


            // Look for hidden items
            CastleFurnatureContainer container = FindHiddenItem(command.Subject);
            if(container != null)
            {
                if (container.CanTakeItem(location))
                {
                    if (currentInventory.Count >= maxItems)
                    {
                        returnMessage.AddLine("Can't carry   any more!");
                        return returnMessage;
                    }

                    CastleItem getItem = container.TakeItem();
                    currentInventory.Add(getItem);
                    getItem.PickUp();
                    returnMessage.AddLine("Done.");
                    return returnMessage;
                }
                else
                {
                    returnMessage.AddLine(" I can't");
                    returnMessage.AddLine(" reach it!");
                    return returnMessage;
                }
            }

            // Look for items in the room we can take
            item = FindItemInRoom(command.Subject);
            if(item != null)
            {
                returnMessage.AddLine(" Get it");
                returnMessage.AddLine(" Yourself!");
                return returnMessage;

            }

            // Look for items in the room we can not take
            item = FindItem(command.Subject);
            if (item != null)
            {
                returnMessage.AddLine(" I don't see");
                returnMessage.AddLine(String.Format(" a {0}", item.GetDescription));
                return returnMessage;
            }

            // Look for furnature in the room
            CastleFurnature castleFurnature = FindFurnatureInRoom(command.Subject);
            if(castleFurnature != null)
            {
                if (castleFurnature.GetDescription != null)
                {
                    returnMessage.AddLine(String.Format("{0}", castleFurnature.GetDescription));
                    returnMessage.AddLine("are too heavy!");
                    return returnMessage;
                }
            }

            // Look for furnature in any room
            castleFurnature = FindFurnature(command.Subject);
            if (castleFurnature != null)
            {
                if (castleFurnature.GetDescription != null)
                {
                    returnMessage.AddLine(String.Format("{0}", castleFurnature.GetDescription));
                    returnMessage.AddLine("are too heavy!");
                    returnMessage.AddLine("and I don't");
                    returnMessage.AddLine("see any!");
                    return returnMessage;
                }
            }

            // Look for monsters in the room
            Monster monster = FindMonsterInRoom();
            if(monster != null)
            {
                returnMessage.AddLine("I don't think");
                returnMessage.AddLine("that would be");
                returnMessage.AddLine("very safe!");
                return returnMessage;

            }

            // Look for special Cases
            foreach(var roomItem in currentRoomFurnatureItems)
            {
                if (roomItem.IsSpecialCommandSupported(CommandVerb.Take))
                {
                    UserMessage message = roomItem.RunCommand(CommandVerb.Take, command.Subject, this.currentRoomIndex, location, FindItemInInventory);
                    if(message != null)
                    {
                        return message;
                    }
                }
            }

            returnMessage.AddLine("Impossible!!");

            return returnMessage;
        }

        public UserMessage CommandDrop(Command command, Point location, bool collision)
        {
            UserMessage returnMessage = new UserMessage();

            // Verify item is in our inventory
            CastleItem item = FindItemInInventory(command.Subject);
            if (item == null)
            {
                returnMessage.AddLine("You don't have");
                returnMessage.AddLine(String.Format("a {0}", command.Subject));
                return returnMessage;
            }

            // Verify that nothing is in the way
            if(collision)
            {
                returnMessage.AddLine(" Something is");
                returnMessage.AddLine(" in the way!");
                return returnMessage;
            }

            // Drop the item
            currentInventory.Remove(item);
            CurrentRoomItems.Add(item);
            item.PutDown(currentRoomIndex, location);
            returnMessage.Character = item.Character;
            returnMessage.AddLine(" Done");
            return returnMessage;
                

        }

        public UserMessage CommandDrink(Command command, Point location)
        {
            UserMessage returnMessage = RunSpecialCommand(command.Verb, command.Subject, location);

            if( returnMessage == null)
            {
                returnMessage = new UserMessage();
                returnMessage.AddLine("You don't have");
                returnMessage.AddLine(String.Format("a {0}!", command.Subject));
                
            }
            return returnMessage;

        }

        public UserMessage CommandWear(Command command, Point position)
        {
            // Run the wear command
            UserMessage returnMessage = RunSpecialCommand(command.Verb, command.Subject, position);
            if (returnMessage == null)
            {
                returnMessage = new UserMessage();

                CastleItem item = FindItemInInventory(command.Subject);
                if (item == null)
                {
                    returnMessage.AddLine(" You don't");
                    returnMessage.AddLine(" have it!");
                    return returnMessage;
                }
                
                // Determine if it is a real item
                item = FindItem(command.Subject);
                if(item == null)
                {
                    returnMessage.AddLine("You don't have");
                    returnMessage.AddLine(String.Format("a {0}!", command.Subject));
                }
                else if(item.IsSpecialCommandSupported(CommandVerb.Wear))
                {
                    returnMessage.AddLine(" You don't");
                    returnMessage.AddLine(" have it!");
                }
                else
                {
                    returnMessage.AddLine("That could be");
                    returnMessage.AddLine("Difficult!");

                }

            }
            return returnMessage;
        }

        public UserMessage CommandRead(Command command, Point position)
        {
            // Run the read command
            UserMessage returnMessage = RunSpecialCommand(command.Verb, command.Subject, position);
            if (returnMessage == null)
            {
                returnMessage = new UserMessage();
                CastleItem item = FindItem(command.Subject);
                if (item != null)
                {
                    returnMessage.AddLine("You don't");
                    returnMessage.AddLine(" have a book!");
                }
                else
                {
                    returnMessage.AddLine(" You can't");
                    returnMessage.AddLine(" read That!");

                }
            }
            return returnMessage;
        }
        public UserMessage CommandWave(Command command, Point position)
        {
            UserMessage returnMessage = RunSpecialCommand(command.Verb, command.Subject, position);
            if(returnMessage == null)
            {
                returnMessage = new UserMessage();
                CastleItem item = FindItemInInventory(command.Subject);
                if(item != null)
                {
                    returnMessage.AddLine("You look awful");
                    returnMessage.AddLine("Silly waving");
                    returnMessage.AddLine(String.Format("that {0}", item.GetDescription));
                }
                else
                {
                    item = FindItem(command.Subject);
                    if(item != null)
                    {
                        returnMessage.AddLine("You don't have");
                        returnMessage.AddLine(String.Format("a {0}", item.GetDescription));
                    }
                    else
                    {
                        returnMessage.AddLine("You don't have");
                        returnMessage.AddLine(String.Format("a {0}", command.Subject));
                    }
                }
            }

            return returnMessage;
        }
        public UserMessage CommandOpen(Command command, Point position, Room currentRoom)
        {
            // Run the Open command
            UserMessage returnMessage = new UserMessage();
            

            CastleItem key = FindItemInInventory("Key");
            if(key == null)
            {
                returnMessage.AddLine(" You don't");
                returnMessage.AddLine(" have a key!");
                return returnMessage;
            }
            
            if(command.Subject.StartsWith("Door", StringComparison.CurrentCultureIgnoreCase) == false)
            {
                returnMessage.AddLine("Impossible!!");
                return returnMessage;
            }

            // Is door in room 
            if (currentRoom.Door == null)
            {
                returnMessage.AddLine(" I don't");
                returnMessage.AddLine(" see a door.");
                return returnMessage;
            }


            if (currentRoom.Door.Locked)
            {
                foreach (Point point in currentRoom.Door.UnlockPoints)
                {
                    if (position.X == point.X && position.Y == point.Y)
                    {
                        currentRoom.Door.Locked = false;
                        returnMessage.AddLine("Done");
                        return returnMessage;
                    }
                }
            }

            returnMessage.AddLine(" I can't");
            returnMessage.AddLine(" reach it!.");
            return returnMessage;

        }

        public UserMessage CommandShow(Command command)
        {
            UserMessage returnMessage = new UserMessage();
            CastleItem item = FindItemInInventory(command.Subject);
            if(item == null)
            {
                returnMessage.AddLine("I don't have");
                returnMessage.AddLine(String.Format("a {0}", command.Subject));
                return returnMessage;
            }

            Monster monster = FindMonsterInRoom();
            if(monster == null)
            {
                returnMessage.AddLine("There's not");
                returnMessage.AddLine("anyone to");
                returnMessage.AddLine("show it to!");
                return returnMessage;
            }

            returnMessage = monster.Show(item);
            if (monster.IsVisible == false)
            {
                CurrentRoomMonsters.Remove(monster);
            }
            return returnMessage;


        }
        public UserMessage CommandPlay(Command command, Point position)
        {
            // Run the play command
            var returnMessage = new UserMessage();
            CastleItem item = FindItemInInventory(command.Subject);
            if (item == null)
            {
                returnMessage.AddLine(" You don't");
                returnMessage.AddLine(" have it!");
                return returnMessage;
            }

            Monster monster = FindMonsterInRoom();
            if (monster != null)
            {
                UserMessage monsterMessage = monster.Play(item);
                if(monsterMessage != null)
                {
                    if (monster.IsVisible == false)
                    {
                        CurrentRoomMonsters.Remove(monster);
                    }
                    return monsterMessage;
                }
            }

            returnMessage = RunSpecialCommand(command.Verb, command.Subject, position);
            if (returnMessage != null)
            {
                return returnMessage;
            }


            // Determine if it is a real item
            item = FindItem(command.Subject);
            if (item == null)
            {
                returnMessage.AddLine("You don't have");
                returnMessage.AddLine(String.Format("a {0}!", command.Subject));
            }
            else if (item.IsSpecialCommandSupported(CommandVerb.Play))
            {
                returnMessage.AddLine(" You don't");
                returnMessage.AddLine(" have it!");
            }
            else
            {
                returnMessage.AddLine(String.Format("The {0}", item.InventoryName));
                returnMessage.AddLine("makes Horrible");
                returnMessage.AddLine("music!!");

            }


            return returnMessage;
        }

    }




    internal delegate UserMessage SpecialCommandCallback(string subject, int currentRoomIndex, Point location, FindItemInInventoryCb findItemInInventoryCb);
    internal delegate CastleItem FindItemInInventoryCb(string subject);
    internal delegate void UpdateMapCb(Collection<ReplacementPoint> replacementPoints);

    internal abstract class CastleObject
    {
        public int RoomIndex { get; protected set; }
        public Collection<String> parsingNames;
        public String Description {get; set; }
        public String GetDescription { get; protected set; }
        protected Dictionary<CommandVerb, SpecialCommandCallback> SpecialCommandList { get; private set; }

        protected CastleObject (String description, String getDescription, int roomIndex, params String[] parsingName)
        {
            this.RoomIndex = roomIndex;

            this.Description = description;
            this.GetDescription = getDescription;

            parsingNames= new Collection<string>();
            if(parsingName != null)
            {
                foreach(var name in parsingName)
                {
                    parsingNames.Add(name);
                }
            }

            SpecialCommandList = new Dictionary<CommandVerb, SpecialCommandCallback>();

        }


        public bool IsSpecialCommandSupported(CommandVerb verb)
        {
            if(SpecialCommandList.ContainsKey(verb))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public UserMessage RunCommand(CommandVerb verb, String subject, int currentRoomIndex, Point location, FindItemInInventoryCb findItemInInventoryCb)
        {
            return SpecialCommandList[verb](subject, currentRoomIndex, location, findItemInInventoryCb);
        }


    }


    internal class CastleFurnature : CastleObject
    {
        public string LookFailMessage { get; private set; }

        public CastleFurnature(String lookFailMessage, String description, String getDescription, int roomIndex, params String[] parsingName)
            : base(description, getDescription, roomIndex, parsingName)
        {
            this.LookFailMessage = lookFailMessage;
        }

    }

    internal class CastleFurnatureContainer : CastleFurnature
    {
        protected string EmptyDescription { get; private set; }
        public CastleItem Item { get; protected set; }
        private Collection<Point> getPoints;


        public CastleFurnatureContainer(String lookFailMessage, String fullDescription, String emptyDescription, CastleItem item, Collection<Point> getPoints, String getDescription, int roomIndex, params String[] parsingName)
            : base(lookFailMessage, fullDescription, getDescription, roomIndex, parsingName)
        {
            this.EmptyDescription = emptyDescription;
            this.Item = item;
            this.getPoints = getPoints;
            
        }

        public bool CanTakeItem(Point point)
        {
            foreach (Point findPoint in getPoints)
            {
                if(point.X == findPoint.X && point.Y == findPoint.Y)
                {
                    return true;
                }
            }
            return false;
        }
        public virtual CastleItem TakeItem()
        {
            CastleItem returnItem = Item;
            this.Description = EmptyDescription;
            Item = null;
            return returnItem;
        }
    }

    internal class CastleFurnatureFountain : CastleFurnatureContainer
    {
        private CastleFurnature otherFurnature;
        public CastleFurnatureFountain(String lookFailMessage, String fullDescription, String emptyDescription, CastleItem item, CastleFurnature otherFurnature, Collection<Point> getPoints, String getDescription, int roomIndex, params String[] parsingName)
            : base(lookFailMessage, fullDescription, emptyDescription, item, getPoints, getDescription, roomIndex, parsingName)
        {
            this.otherFurnature = otherFurnature;
            this.SpecialCommandList.Add(CommandVerb.Take, GetWater);
        }

        public override CastleItem TakeItem()
        {
            CastleItem returnItem = Item;

            otherFurnature.Description = EmptyDescription;
            Item = null;
            return returnItem;
        }


        private UserMessage GetWater(string subject, int currentRoomIndex, Point location, FindItemInInventoryCb findItemInInventoryCb)
        {
            if (subject.StartsWith("water", StringComparison.CurrentCultureIgnoreCase) == false)
            {
                return null;
            }

            UserMessage returnMessage = new UserMessage();
            
            if (this.CanTakeItem(location))
            {
                CastleItem castleItem = findItemInInventoryCb("flask");
                if(castleItem.IsSpecialCommandSupported(CommandVerb.Drink))
                {
                    DrinkableItem drinkableItem = castleItem as DrinkableItem;
                    if (drinkableItem.Full == false)
                    {
                        drinkableItem.Fill();
                        returnMessage.AddLine("Done");
                    }
                    else
                    {
                        returnMessage.AddLine(" The flask is  full!");
                    }
                }
                else
                {
                    returnMessage.AddLine("You don't have a Container!");
                }

            }
            else
            {
                returnMessage.AddLine(" I can't       reach it!");
            }


            return returnMessage;
        }
    }

    internal class CastleItem : CastleObject
    {
        public string InventoryName { get; private set; }
        public int Character { get; private set; }
        public Point Location { get; private set; }
        public bool Collected { get; private set; }
        public int Value;

        public CastleItem(String inventoryName, int character, Point location, int value, String description, String getDescription, int roomIndex, params String[] parsingName)
            : base(description, getDescription, roomIndex, parsingName)
        {
            this.Collected = false;
            this.InventoryName = inventoryName;
            this.Character = character;
            this.Location = location;
            this.Value = value;

        }


        public void PickUp()
        {
            RoomIndex = 0;
            this.Collected = true;
        }
        public void PutDown(int roomIndex, Point newLocation)
        {
            this.RoomIndex = roomIndex;
            this.Location = newLocation;
        }


    }

    internal class WearableCastleItem : CastleItem
    {
        private string wearMessage;

        public WearableCastleItem(String inventoryName, int character, Point location, String description, string wearMessage, int value, String getDescription, int roomIndex, params String[] parsingName)
            : base(inventoryName, character, location, value, description, getDescription, roomIndex, parsingName)
        {

            this.wearMessage = wearMessage;

            this.SpecialCommandList.Add(CommandVerb.Wear, Wear);
        }

        private UserMessage Wear(string subject, int currentRoomIndex, Point location, FindItemInInventoryCb findItemInInventoryCb)
        {
            return new UserMessage(wearMessage);

        }


        

    }
    internal class DrinkableItem : CastleItem
    {
        private string emptyDescription;
        private string fullDescription;
        public bool Full { get; private set; }

        public static bool Thirsty = true;


        public DrinkableItem(String inventoryName, int character, Point location, String description, String fullDescription, int value, String getDescription, int roomIndex, params String[] parsingName)
            : base(inventoryName, character, location, value, description, getDescription, roomIndex, parsingName)
        {
            this.emptyDescription = description;
            this.fullDescription = fullDescription;
            this.SpecialCommandList.Add(CommandVerb.Drink, Drink);
        }

        private UserMessage Drink(string subject, int currentRoomIndex, Point location, FindItemInInventoryCb findItemInInventoryCb)
        {
            if(subject.StartsWith("water", StringComparison.CurrentCultureIgnoreCase) == false)
            {
                return null;
            }
            UserMessage returnMessage = new UserMessage();
            if (Full)
            {
                returnMessage.AddLine("That was Good!");
                if (Thirsty)
                {
                    returnMessage.AddLine("I feel much");
                    returnMessage.AddLine("better now!");
                    Thirsty = false;
                    Empty();
                }
            }
            else
            {
                returnMessage.AddLine("You don't have");
                returnMessage.AddLine("any WATER");
            }

            return returnMessage;
        }
        public void Fill()
        {
            Full = true;
            this.Description = fullDescription;
        }
        public void Empty()
        {
            Full = false;
            this.Description = emptyDescription;
        }
        
    }
    internal class BookItem : CastleItem
    {

        public BookItem(String inventoryName, int character, Point location, String description, int value, String getDescription, int roomIndex, params String[] parsingName)
            : base(inventoryName, character, location, value, description, getDescription, roomIndex, parsingName)
        {

            this.SpecialCommandList.Add(CommandVerb.Read, Read);
        }

        private UserMessage Read(string subject, int currentRoomIndex, Point location, FindItemInInventoryCb findItemInInventoryCb)
        {
            UserMessage userMessage = new UserMessage();

            CastleItem glasses = findItemInInventoryCb("Glasses");
            if(glasses == null)
            {
                userMessage.AddLine(" You can't see");
                userMessage.AddLine(" well enough");
                userMessage.AddLine(" It's all");
                userMessage.AddLine(" Blurry.");
            }
            else
            {
                userMessage.AddLine("The book reads");
                userMessage.AddLine("  Wave Scepter");
            }


            return userMessage;

        }
    }
    internal class WaveItem : CastleItem
    {
        private Collection<SpecialRoomAction> specialRoomActions;
        private UpdateMapCb updateMapCb;

        public WaveItem(UpdateMapCb updateMapCb, String inventoryName, int character, Point location, String description, int value, String getDescription, int roomIndex, params String[] parsingName)
            : base(inventoryName, character, location, value, description, getDescription, roomIndex, parsingName)
        {

            this.SpecialCommandList.Add(CommandVerb.Wave, Wave);
            specialRoomActions = new Collection<SpecialRoomAction>();
            this.updateMapCb = updateMapCb;
        }

        public void AddSpecialRoomAction(SpecialRoomAction action)
        {
            specialRoomActions.Add(action);
        }

        private UserMessage Wave(string subject, int currentRoomIndex, Point location, FindItemInInventoryCb findItemInInventoryCb)
        {
            UserMessage userMessage = null;
            foreach(var specialRoomAction in specialRoomActions)
            {
                if(specialRoomAction.RoomId == currentRoomIndex)
                {
                    userMessage = specialRoomAction.RunAction(updateMapCb);
                }
            }
            if (userMessage == null)
            {
                userMessage = new UserMessage();

                userMessage.AddLine("As you wave");
                userMessage.AddLine("the WAND....");
                userMessage.AddLine(" ");
                userMessage.AddLine("  Nothing");
                userMessage.AddLine("  Happens!");
            }
            return userMessage;

        }
    }

    internal class PlayableCastleItem : CastleItem
    {
        private string playMessage;

        public PlayableCastleItem(String inventoryName, int character, Point location, String description, string playMessage, int value, String getDescription, int roomIndex, params String[] parsingName)
            : base(inventoryName, character, location, value, description, getDescription, roomIndex, parsingName)
        {

            this.playMessage = playMessage;

            this.SpecialCommandList.Add(CommandVerb.Play, Play);
        }

        private UserMessage Play(string subject, int currentRoomIndex, Point location, FindItemInInventoryCb findItemInInventoryCb)
        {
            return new UserMessage(playMessage);

        }




    }
    
    internal abstract class SpecialRoomAction
    {
        public int RoomId { get; protected set; }

        public SpecialRoomAction()
        {

        }

        public abstract UserMessage RunAction(UpdateMapCb updateMapCb);

    }
    internal class SorcerersRoom : SpecialRoomAction
    {

        public SorcerersRoom()
        {
            this.RoomId = 67;
            
        }

        public override UserMessage RunAction(UpdateMapCb updateMapCb)
        {
            UserMessage returnMessage = new UserMessage();
            returnMessage.AddLine("As you wave");
            returnMessage.AddLine("the WAND....");
            returnMessage.AddLine("  There is a");
            returnMessage.AddLine("Puff of smoke");
            returnMessage.AddLine("revealing");
            returnMessage.AddLine("a Secret");
            returnMessage.AddLine("passage!");

            Collection<ReplacementPoint> replacementPoints = new Collection<ReplacementPoint>();

            // Erase mirror
            replacementPoints.Add(new ReplacementPoint(18, 6, 32));
            replacementPoints.Add(new ReplacementPoint(18, 7, 32));
            replacementPoints.Add(new ReplacementPoint(18, 8, 32));
            replacementPoints.Add(new ReplacementPoint(18, 9, 32));
            replacementPoints.Add(new ReplacementPoint(18, 10, 32));
            replacementPoints.Add(new ReplacementPoint(18, 11, 32));

            // Erase Wall
            replacementPoints.Add(new ReplacementPoint(19, 8, 32));
            replacementPoints.Add(new ReplacementPoint(19, 9, 32));

            // Add Wall
            replacementPoints.Add(new ReplacementPoint(20, 7, 178));
            replacementPoints.Add(new ReplacementPoint(21, 7, 178));
            replacementPoints.Add(new ReplacementPoint(22, 7, 178));
            replacementPoints.Add(new ReplacementPoint(23, 7, 178));
            replacementPoints.Add(new ReplacementPoint(20, 10, 178));
            replacementPoints.Add(new ReplacementPoint(21, 10, 178));
            replacementPoints.Add(new ReplacementPoint(22, 10, 178));
            replacementPoints.Add(new ReplacementPoint(23, 10, 178));

            updateMapCb(replacementPoints);

            return returnMessage;
            

        }
    }

    internal class WindingPasage2 : SpecialRoomAction
    {

        public WindingPasage2()
        {
            this.RoomId = 77;

        }

        public override UserMessage RunAction(UpdateMapCb updateMapCb)
        {
            UserMessage returnMessage = new UserMessage();
            returnMessage.AddLine("As you wave");
            returnMessage.AddLine("the WAND....");
            returnMessage.AddLine("  There is a");
            returnMessage.AddLine("Puff of smoke");
            returnMessage.AddLine("revealing");
            returnMessage.AddLine("a Secret");
            returnMessage.AddLine("passage!");

            Collection<ReplacementPoint> replacementPoints = new Collection<ReplacementPoint>();

            // Erase Wall
            replacementPoints.Add(new ReplacementPoint(11, 17, 32));
            replacementPoints.Add(new ReplacementPoint(12, 17, 32));

            // Add Wall
            replacementPoints.Add(new ReplacementPoint(10, 17, 221));
            replacementPoints.Add(new ReplacementPoint(13, 17, 222));

            updateMapCb(replacementPoints);

            return returnMessage;


        }
    }
    internal class GateRoom : SpecialRoomAction
    {

        public GateRoom()
        {
            this.RoomId = 1;

        }

        public override UserMessage RunAction(UpdateMapCb updateMapCb)
        {
            UserMessage returnMessage = new UserMessage();
            returnMessage.AddLine("As you wave");
            returnMessage.AddLine("the SCEPTER...");
            returnMessage.AddLine(" ");
            returnMessage.AddLine("The Gate opens");
            returnMessage.AddLine("by itself!");

            Collection<ReplacementPoint> replacementPoints = new Collection<ReplacementPoint>();

            // Erase Wall
            replacementPoints.Add(new ReplacementPoint(10, 17, 32));
            replacementPoints.Add(new ReplacementPoint(11, 17, 32));
            replacementPoints.Add(new ReplacementPoint(12, 17, 32));
            replacementPoints.Add(new ReplacementPoint(13, 17, 32));

            updateMapCb(replacementPoints);

            return returnMessage;


        }
    }





}
