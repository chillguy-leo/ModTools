using Exiled.API.Enums;
using Exiled.API.Features;
using System.Linq;

namespace ModTools
{
    public class RoomInfo
    {
        static RoomInfo[] rooms = new RoomInfo[]
        {
            new RoomInfo(new string[] {"Tesla","TeslaGate"}, RoomType.HczTesla),
            new RoomInfo(new string[] {"ClassDSpawn","ClassD","cd"}, RoomType.LczClassDSpawn),
            new RoomInfo(new string[] {"PocketDimension","Pocket","pd"}, RoomType.Pocket),
            new RoomInfo(new string[] {"Surface"}, RoomType.Surface),
            new RoomInfo(new string[] {"SCP914","914"}, RoomType.Lcz914),
            new RoomInfo(new string[] {"LczArmory","Armory1","LightArmory","la"}, RoomType.LczArmory),
            new RoomInfo(new string[] {"HczArmory","Armory2","HeavyArmory","ha"}, RoomType.HczArmory),
            new RoomInfo(new string[] {"HczNuke","hn","NukeSilo"}, RoomType.HczNuke),
            new RoomInfo(new string[] {"SurfaceNuke","sn"}, RoomType.HczNuke),
            new RoomInfo(new string[] {"SCP330","330","Candy","CandyRoom","c","cr"}, RoomType.Lcz330),
            new RoomInfo(new string[] {"SCP173","173","nut","peanut"}, RoomType.Lcz173),
            new RoomInfo(new string[] {"SCP049","049","49","doctor","doc"}, RoomType.Hcz049),
            new RoomInfo(new string[] {"SCP079","079","79"}, RoomType.Hcz079),
            new RoomInfo(new string[] {"SCP096","096","96","shyguy"}, RoomType.Hcz096),
            new RoomInfo(new string[] {"SCP106","106","Larry"}, RoomType.Hcz106),
            new RoomInfo(new string[] {"WC","Toilet","Bathroom","Restroom"}, RoomType.LczToilets),
            new RoomInfo(new string[] {"MicroHID","Micro","HID"}, RoomType.HczHid),
            new RoomInfo(new string[] {"RedRoom","red"}, RoomType.EzVent),
            new RoomInfo(new string[] {"Collapsed"}, RoomType.EzCollapsedTunnel),
            new RoomInfo(new string[] {"ezPC"}, RoomType.EzDownstairsPcs),
            new RoomInfo(new string[] {"lczPC","PC","Cafe"}, RoomType.LczCafe),
            new RoomInfo(new string[] {"Old939","Test","Pit"}, RoomType.HczTestRoom),
            new RoomInfo(new string[] {"SCP939","939","Dog"}, RoomType.Hcz939),
            new RoomInfo(new string[] {"airlock"}, RoomType.LczAirlock),
            new RoomInfo(new string[] {"glass","GR18","GR","18"}, RoomType.LczGlassBox),
            new RoomInfo(new string[] {"plants","plant","greenhouse","green","VT"}, RoomType.LczPlants),
            new RoomInfo(new string[] {"servers","server"}, RoomType.HczServers),
            new RoomInfo(new string[] {"lczA","lczCheckpointA"}, RoomType.LczCheckpointA),
            new RoomInfo(new string[] {"lczB","lczCheckpointB"}, RoomType.LczCheckpointB),
            new RoomInfo(new string[] {"hczA","hczCheckpointA"}, RoomType.HczEzCheckpointA),
            new RoomInfo(new string[] {"hczB","hczCheckpointB"}, RoomType.HczEzCheckpointB),
            new RoomInfo(new string[] {"ezA","gateA","exitA"}, RoomType.EzGateA),
            new RoomInfo(new string[] {"ezB","gateB","exitB"}, RoomType.EzGateB),


        };

        public RoomInfo(string[] names, RoomType room)
        {
            Names = names;
            Room = room;
        }

        public static Room? GetRoomByName(string name)
        {
            //var strippedName = name.Replace(" ", "").Replace("-","").Replace("SCP","").Replace("scp","").Replace("Scp","");
            foreach (RoomInfo roomInfo in rooms)
            {
                if (roomInfo.Names.Any(n => name.Equals(n, System.StringComparison.OrdinalIgnoreCase)))
                {
                    return Exiled.API.Features.Room.List.First(r => r.Type == roomInfo.Room);
                }
            }
            return null;
        }

        public string[] Names { get; }
        public RoomType Room { get; }
    }
}
