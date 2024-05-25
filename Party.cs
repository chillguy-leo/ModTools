using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModTools
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Party : ICommand
    {
        public string Command => "party";

        public string[] Aliases => new string[] { "p" };

        public string Description => "Toggle party mode";

        public static bool partyModeEnabled { get; set; } = false;

        private static IEnumerator<float> ChangeRoomColors(IReadOnlyCollection<Room> rooms) {
            while (partyModeEnabled) {
                foreach (var room in rooms) {
                    if (!partyModeEnabled) break;
                    room.Color = UnityEngine.Random.ColorHSV(0,1,0,1,0.95f,1);
                    yield return Timing.WaitForSeconds(0.07f);
                }
            }
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.RoundEvents))
            {
                response = "You do not have sufficient permissions to use this command";
                return false;
            }
            if (partyModeEnabled) {
                partyModeEnabled = false;
                foreach (var room in Room.List) {
                    room.Color = Color.white;
                }
                response = "Disabled party mode.";
            } else {

                response = "Enabled party mode.";
                foreach (var room in Room.List) {
                    room.Color = UnityEngine.Random.ColorHSV();
                }
                Map.Broadcast(10, "<color=#f9584d>P</color><color=#f4664d>a</color><color=#ef744e>r</color><color=#ea7f4e>t</color><color=#e48b4f>y</color><color=#de944f> </color><color=#d89d50>m</color><color=#d1a850>o</color><color=#c9b150>d</color><color=#c1ba51>e</color><color=#b9c251> </color><color=#b0ca51>e</color><color=#a3d252>n</color><color=#98da52>a</color><color=#8ce252>b</color><color=#7bea53>l</color><color=#68f153>e</color><color=#4df953>d</color>");
                partyModeEnabled = true;
                Timing.RunCoroutine(ChangeRoomColors(Room.List));
            }
            return true;
        }
    }
}

