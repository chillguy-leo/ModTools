using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using MEC;
using PlayerRoles;
using System;
using UnityEngine;

namespace ModTools
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ModMode : ICommand
    {
        public string Command => "modmode";

        public string[] Aliases => new[] { "mm", "tutorial" };

        public string Description => "Shortcut to disable overwatch, show tag, spawn as Tutorial, and enable noclip and bypass if able";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);
            var success = player.EnableModMode(out response);
            return success;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ModModeHere : ICommand
    {
        public string Command => "modmodehere";

        public string[] Aliases => new[] { "mmh", "tutorialhere" };

        public string Description => "Same as .modmode, but retains your current location instead of spawning you in the tower";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);
            var success = player.EnableModMode(out response, true);
            return success;
        }
    }


    [CommandHandler(typeof(ClientCommandHandler))]
    public class UnModMode : ICommand
    {
        public string Command => "unmodmode";

        public string[] Aliases => new[] { "umm" };

        public string Description => "Shortcut to disable noclip, godmode, and bypass all at once";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);
            var success = player.DisableModMode(out response);
            return success;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ModModeTeleport : ICommand
    {
        public string Command => "modmodeteleport";

        public string[] Aliases => new[] { "mmtp" };

        public string Description => "Shortcut to spawn as Tutorial (via .modmode) and teleport to the player you are currently spectating";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);
            if (player.Role != RoleTypeId.Spectator && player.Role != RoleTypeId.Overwatch)
            {
                response = Plugin.Singleton.Translation.NotSpectatorError;
                return false;
            }

            // Now we know that, at the very least, the player can be spawned as spectator.
            // So, we look for the player they are currently spectating
            var target = player.FindSpectatingTargetOrNull();
            if (target == null)
            {
                response = Plugin.Singleton.Translation.CantFindTargetError;
                return false;
            }
            // I don't know why this would happen, but it has (possibly due to some Exiled bugginess or something else)
            // so here is a defense for it.
            if (target.Id == player.Id)
            {
                response = Plugin.Singleton.Translation.TargetIsSelfError;
                return false;
            }

            var spawnSuccess = player.EnableModMode(out response);

            if (!spawnSuccess)
            {
                return false; // response already set
            }

            // Originally, we would teleport the player immediately after spawning.
            // However, this was causing issues wherein sometimes the player wouldn't
            // spawn in fast enough and would "miss" the teleport.
            // A delay of 0.5 seconds was found to alleviate this problem, and has been
            // bumped to 1.0 second in an excess of caution.
            Timing.CallDelayed(1.0f, () =>
            {
                // There is unfortunately a small chance that the target disconnects or dies in this
                // window before the teleport occurs, so we handle that case defensively.
                if (target == null || !target.IsAlive)
                {
                    player.Broadcast(new Exiled.API.Features.Broadcast(
                    Plugin.Singleton.Translation.BroadcastHeader
                        + Plugin.Singleton.Translation.TargetDiedError
                    ));
                    // Due to the async nature of this lambda, we cannot modify the response or
                    // success boolean (though doing so is not strictly necessary as the response
                    // still describes the enabling of modmode)
                }
                else if (target.Id == player.Id)
                {
                    // This should never happen due to the earlier checks. (Added for defensiveness)
                    player.Broadcast(new Exiled.API.Features.Broadcast(
                    Plugin.Singleton.Translation.BroadcastHeader
                        + Plugin.Singleton.Translation.TargetIsSelfError
                    ));
                }
                else
                {
                    // If no problems
                    player.Teleport(target);
                }
            });

            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class ToggleGodmode : ICommand
    {
        public string Command => "godmode";

        public string[] Aliases => new[] { "gm" };

        public string Description => "Toggle godmode for yourself";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            if (!sender.CheckPermission(PlayerPermissions.PlayersManagement))
            {
                response = Plugin.Singleton.Translation.InsufficientPermissions;
                return false;
            }

            if (player.IsGodModeEnabled)
            {
                response = Plugin.Singleton.Translation.GodmodeDisabled;
                player.IsGodModeEnabled = false;
            }
            else
            {
                response = Plugin.Singleton.Translation.GodmodeEnabled;
                player.IsGodModeEnabled = true;
            }

            player.Broadcast(new Exiled.API.Features.Broadcast(
                Plugin.Singleton.Translation.BroadcastHeader + response, 4
            ));
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Die : ICommand
    {
        public string Command => "die";

        public string[] Aliases => new string[] { "spectator" };

        public string Description => "Set your class to spectator, and disable moderation abilities (i.e., call .unmodmode)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            // ForceclassSelf is checked (rather than ForceclassSpectator) because ForceclassSelf
            // is weaker; i.e., ForceclassSelf lets you set yourself to spectator, while ForceclassSpectator
            // lets you set other players to spectator.
            if (!sender.CheckPermission(PlayerPermissions.ForceclassSelf))
            {
                response = Plugin.Singleton.Translation.InsufficientPermissions;
                return false;
            }

            player.Role.Set(RoleTypeId.Spectator);

            var success = player.DisableModMode(out response);
            return success;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Prop : ICommand
    {
        public string Command => "prop";

        public string[] Aliases => new string[] { "pr" };

        public string Description => "Spawn in a prop";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems))
            {
                response = "Unauthorized";
                return false;
            }
            if (arguments.Count < 2)
            {
                response = "Usage: prop <item name> <size>";
                return false;
            }
            if (!Enum.TryParse(arguments.At(0), true, out ItemType itemType))
            {
                response = $"Invalid value for item name: {arguments.At(0)}";
                return false;
            }
            if (!float.TryParse(arguments.At(1), out float scale))
            {
                response = $"Invalid value for item scale: {arguments.At(1)}";
                return false;
            }
            if (!Player.TryGet(sender, out Player player))
            {
                response = "You must be in game to use this command";
                return false;
            }
            if (itemType is ItemType.Flashlight)
            {
                var flashlight = (Flashlight)Item.Create(ItemType.Flashlight);
                flashlight.Active = true;
                var pickup = flashlight.CreatePickup(player.Position, player.GameObject.transform.rotation, true);
                pickup.Scale = -1 * scale * Vector3.one;
                Plugin.props.Add(pickup);
            }
            else
            {
                var pickup = Pickup.CreateAndSpawn(itemType, player.Position, player.GameObject.transform.rotation);
                pickup.Scale = scale * Vector3.one;
                Plugin.props.Add(pickup);
            }
            response = "Spawned in prop";
            return true;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class PurgeProps : ICommand
    {
        public string Command => "purgeprops";

        public string[] Aliases => new string[] { "pp" };

        public string Description => "Purge all props";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems))
            {
                response = "Unauthorized";
                return false;
            }
            var count = 0;
            foreach (var pickup in Pickup.List)
            {
                if (Plugin.props.Contains(pickup))
                {
                    pickup.Destroy();
                    count++;
                }
            }
            response = $"Purged {count} prop{Util.S(count)}";
            return true;
        }
    }


    [CommandHandler(typeof(ClientCommandHandler))]
    public class GotoRoom : ICommand
    {
        public string Command => "gotoroom";

        public string[] Aliases => new string[] { "g" };

        public string Description => "Go to a certain room";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.Noclip))
            {
                response = "Insufficient permissions";
                return false;
            }

            if (!Player.TryGet(sender, out Player player))
            {
                response = "You must be a player to use this command";
                return false;
            }

            var room = RoomInfo.GetRoomByName(string.Join("", arguments));
            if (room is null)
            {
                response = "Can't find room by that name";
                return false;
            }
            else
            {
                player.Teleport(room);
                response = "Teleported";
                return true;
            }
        }
    }


    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class InfiniteAmmo : ICommand
    {
        public string Command => "infinite";

        public string[] Aliases => new string[] { "infiniteammo", "i" };

        public string Description => "Toggle infinite ammo";

        public static void ShowBroadcast(Player player)
        {
            if (Plugin.infiniteAmmoForAllPlayers || Plugin.infiniteAmmoPlayers.Contains(player))
            {
                player.Broadcast(5, "<color=#00ffc0>Infinite ammo enabled</color>");
            } else
            {
                player.Broadcast(5, "<color=#ff4eac>Infinite ammo disabled</color>");
            }
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems))
            {
                response = "You need spawn item permissions to use this command";
                return false;
            }
            if (arguments.IsEmpty())
            {
                if (Player.TryGet(sender, out Player player))
                {
                    Plugin.infiniteAmmoPlayers.Toggle(player);
                    response = Plugin.infiniteAmmoPlayers.Contains(player) ? "Enabled infite ammo" : "Disabled infinite ammo";
                    ShowBroadcast(player);
                    return true;
                }
                else
                {
                    response = "You must be in-game to use this command with no arguments";
                    return false;
                }
            }
            if (arguments.FirstElement().Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                Plugin.infiniteAmmoForAllPlayers.Toggle();
                response = Plugin.infiniteAmmoForAllPlayers ? "Enabled infinite ammo for all players" : "Disabled infintie ammo for all players";
                foreach (Player player in Player.List)
                {
                    ShowBroadcast(player);
                }
                return true;
            }
            if (Player.TryGet(arguments.FirstElement(), out Player target))
            {
                Plugin.infiniteAmmoPlayers.Toggle(target);
                response = Plugin.infiniteAmmoPlayers.Contains(target) ? $"Enabled infinite ammo for {target.Nickname}" : $"Disabled infinite ammo for {target.Nickname}";
                ShowBroadcast(target);
                return true;
            }
            response = "Usage: infinite <username, id, \"all\", or empty>";
            return false;
        }
    }
}