using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Xna.Framework;

namespace Castle
{
    internal class MapReader
    {
        private Dictionary<int, Room> roomList;


        public MapReader()
        {
            roomList = new Dictionary<int, Room>();

            ParseMap();
            CreateWarpRooms();

            // Create Dark Room
            FindRoom(68).Dark = true;
            FindRoom(83).Dark = true;
        }
        private void CreateWarpRooms()
        {
            // Sorcerers room
            UserMessage userMessage = new UserMessage();
            userMessage.AddLine("There is a ");
            userMessage.AddLine("puff of smoke");
            userMessage.AddLine("& you appear");
            userMessage.AddLine("in the");
            userMessage.AddLine("Sorcerers room");
            FindRoom(66).AddWarp(new RoomWarp(66, 67, 16, 9, userMessage));

            // Kings hidden room
            FindRoom(83).AddWarp(new RoomWarp(83, 77, 11, 16, new UserMessage()));

            // Add Doors in Room 68
            Collection<Point> room68Door = new Collection<Point>();
            room68Door.Add(new Point(8, 14));
            room68Door.Add(new Point(9, 14));
            Collection<Point> room68UnlockPoints = new Collection<Point>();
            room68UnlockPoints.Add(new Point(8, 13));
            room68UnlockPoints.Add(new Point(9, 13));
            room68UnlockPoints.Add(new Point(8, 15));
            room68UnlockPoints.Add(new Point(9, 15));
            FindRoom(68).AddDoor(new Door(68, room68Door, room68UnlockPoints));


            // Add Doors in Room 82
            Collection<Point> room82Door = new Collection<Point>();
            room82Door.Add(new Point(13, 8));
            room82Door.Add(new Point(13, 9));
            Collection<Point> room82UnlockPoints = new Collection<Point>();
            room82UnlockPoints.Add(new Point(12, 8));
            room82UnlockPoints.Add(new Point(12, 9));
            room82UnlockPoints.Add(new Point(14, 8));
            room82UnlockPoints.Add(new Point(14, 9));
            FindRoom(82).AddDoor(new Door(82, room82Door, room82UnlockPoints));

            // Add Trap room 76
            Collection<Point> room76Trigger = new Collection<Point>();
            room76Trigger.Add(new Point(13, 12));
            room76Trigger.Add(new Point(13, 13));
            Collection<Point> room76TrapWall = new Collection<Point>();
            room76TrapWall.Add(new Point(10, 12));
            room76TrapWall.Add(new Point(10, 13));
            room76TrapWall.Add(new Point(16, 12));
            room76TrapWall.Add(new Point(16, 13));
            FindRoom(76).AddTrap(new Trap(76, room76Trigger, room76TrapWall, 10, 15, 12, 13));

            // Add Trap room 78
            Collection<Point> room78Trigger = new Collection<Point>();
            room78Trigger.Add(new Point(17, 8));
            room78Trigger.Add(new Point(17, 9));
            Collection<Point> room78TrapWall = new Collection<Point>();
            room78TrapWall.Add(new Point(11, 8));
            room78TrapWall.Add(new Point(11, 9));
            room78TrapWall.Add(new Point(23, 8));
            room78TrapWall.Add(new Point(23, 9));
            FindRoom(78).AddTrap(new Trap(78, room78Trigger, room78TrapWall, 11, 22, 8, 9));

        }

        private void ParseMap()
        {
            using(FileStream fileStream = File.Open("Map/Castle.RAN", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                using(BinaryReader reader = new BinaryReader(fileStream))
                {
                    for(int roomIndex = 1; roomIndex < Room.MaxRooms + 1; roomIndex++)
                    {
                        Room newRoom = new Room();
                        newRoom.RoomBytes = reader.ReadBytes(Room.RoomSize);
                        newRoom.DescriptionBytes = reader.ReadBytes(Room.DescriptionSize);
                        newRoom.ExitBytes = reader.ReadBytes(Room.ExitSize);
                        newRoom.ParseData();
                        roomList.Add(roomIndex, newRoom);
                    }
                }

            }
        }


        public Room FindRoom(int roomIndex)
        {
            return roomList[roomIndex];
        }



    }

    internal class Room
    {
        public const int MaxRooms = 83;
        public const int RoomSize = 432;
        public const int DescriptionSize = 125;
        public const int ExitSize = 18;

        public const int MapWidth = 24;
        public const int MapHeight = 18;
        public const int DescriptionWidth = 25;
        public const int DescriptionHeight = 5;

        public byte[] RoomBytes;
        public byte[] DescriptionBytes;
        public byte[] ExitBytes;
        public string Description;
        public string Exits;
        private Collection<ReplacementPoint> replacementPoints;

        public bool Dark { get; set; }

        public RoomWarp RoomWarp { get; private set; }
        public Door Door { get; private set; }
        public Trap Trap { get; private set; }

        public Room()
        {
            RoomBytes = new Byte[RoomSize];
            DescriptionBytes = new Byte[DescriptionSize];
            ExitBytes = new Byte[ExitSize];
            replacementPoints = new Collection<ReplacementPoint>();
        }

        public void ParseData()
        {
            this.Description = System.Text.Encoding.ASCII.GetString(DescriptionBytes);
            this.Exits = System.Text.Encoding.ASCII.GetString(ExitBytes);
        }

        public string GetDescriptionLine(int lineIndex)
        {
            int start = lineIndex * DescriptionWidth;
            int end = start + DescriptionWidth;
            if(Description.Length < start)
            {
                return String.Empty;
            }
            else if(Description.Length < end)
            {
                return Description.Substring(start, Description.Length - start);
            }
            else
            {
                return Description.Substring(start, end - start);
            }

        }

        public int GetMapPoint(int x, int y)
        {
            if(x < 0)
            {
                return -1;
            }
            else if(x > MapWidth - 1)
            {
                return -1;
            }
            else if (y < 0)
            {
                return -1;
            }
            else if (y > MapHeight - 1)
            {
                return -1;
            }
            else
            {
                foreach(var point in replacementPoints)
                {
                    if(point.X == x && point.Y == y)
                    {
                        return point.Character;
                    }
                }
                if(Door != null)
                {
                    if (Door.Locked)
                    {
                        foreach (var point in Door.DoorPoints)
                        {
                            if (point.X == x && point.Y == y)
                            {
                                return 8;  // Door point
                            }
                        }
                    }
                }
                if(Trap != null)
                {
                    foreach(var point in Trap.TriggerPoints)
                    {
                        if(point.X == x && point.Y == y)
                        {
                            return -2;  // Trap
                        }
                    }
                }

                int index = x + (y * MapWidth);
                return RoomBytes[index];
            }
        }

        public int GetNextRoom(Direction roomDirection)
        {
            Char targetChat;
            switch(roomDirection)
            {
                case Direction.Up:
                    targetChat = 'N';
                    break;
                case Direction.Down:
                    targetChat = 'S';
                    break;
                case Direction.Left:
                    targetChat = 'W';
                    break;
                case Direction.Right:
                    targetChat = 'E';
                    break;
                case Direction.UpStairs:
                    targetChat = 'U';
                    break;
                case Direction.DownStairs:
                    targetChat = 'D';
                    break;
                default:
                    targetChat = 'N';
                    break;
            }

            int index = Exits.IndexOf(targetChat, 0);
            
            if(index >= 0)
            {
                StringBuilder numericValues = new StringBuilder();
                for(int start = index; start < Exits.Length; start++)
                {
                    String value = Exits.Substring(start + 1, 1);
                    int intValue;
                    if(int.TryParse(value, out intValue))
                    {
                        numericValues.Append(intValue);
                    }
                    else
                    {
                        break;
                    }
                    
                }
                return int.Parse(numericValues.ToString());
            }
            else
            {
                return -1;
            }
            
        }


        public void ClearReplacePoints()
        {
            this.replacementPoints.Clear();
        }
        public void ReplaceMapPoint(Collection<ReplacementPoint> replacementPoints)
        {
            this.replacementPoints.Clear();
            this.replacementPoints = replacementPoints;
        }
        public void AddWarp(RoomWarp warp)
        {
            this.RoomWarp = warp;
        }
        public void AddDoor(Door door)
        {
            this.Door = door;
        }
        public void AddTrap(Trap trap)
        {
            this.Trap = trap;
        }

        
        
    }


    internal class ReplacementPoint
    {
        public int X;
        public int Y;
        public byte Character;

        public ReplacementPoint(int x, int y, byte character)
        {
            this.X = x;
            this.Y = y;
            this.Character = character;
        }

    }

    internal class RoomWarp
    {
        public int RoomId { get; private set; }
        public int DestinationRoomId { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public UserMessage UserMessage;

        public RoomWarp(int roomId, int destinationRoomId, int x, int y, UserMessage userMessage)
        {
            this.RoomId = roomId;
            this.DestinationRoomId = destinationRoomId;
            this.X = x;
            this.Y = y;
            this.UserMessage = userMessage;
        }

    }

    internal class Door
    {
        public bool Locked { get; set; }
        public int RoomId { get; private set; }
        public Collection<Point> DoorPoints;
        public Collection<Point> UnlockPoints;

        public Door(int roomId, Collection<Point> doorPoints, Collection<Point> unlockPoints)
        {
            Locked = true;
            this.RoomId = roomId;
            this.DoorPoints = doorPoints;
            this.UnlockPoints = unlockPoints;
        }
    }

    internal class Trap
    {
        public int RoomId { get; private set; }
        public Collection<Point> TriggerPoints;
        public Collection<Point> WallPoints;
        public int WaterFlowXStart;
        public int WaterFlowXEnd;
        public int WaterFlowYStart;
        public int WaterFlowYEnd;

        public Trap(int roomId, Collection<Point> triggerPoints, Collection<Point> wallPoints, int waterFlowXStart, int waterFlowXEnd, int waterFlowYStart, int waterFlowYEnd)
        {
            this.RoomId = roomId;
            this.TriggerPoints = triggerPoints;
            this.WallPoints = wallPoints;
            this.WaterFlowXStart = waterFlowXStart;
            this.WaterFlowXEnd = waterFlowXEnd;
            this.WaterFlowYStart = waterFlowYStart;
            this.WaterFlowYEnd = waterFlowYEnd;
            
        }

        

    }




}
