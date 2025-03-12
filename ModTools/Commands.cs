using CommandSystem;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System;
using System.Collections.Generic;

namespace ModTools
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ModMode : ICommand
    {
        public string Command => "mm";

        public string[] Aliases => new[] { "modmode", "tutorial" };

        public string Description => "Disable overwatch, show tag, spawn as Tutorial, and enable noclip and bypass";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);
            var success = player.EnableModMode(out response);
            return success;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]

    public class ModModeHere : ICommand
    {
        public string Command => "mmh";

        public string[] Aliases => new[] { "modmodehere", "tutorialhere" };

        public string Description => "Same as .modmode, but keeps current location";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);
            var success = player.EnableModMode(out response, true);
            return success;
        }
    }


    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]

    public class UnModMode : ICommand
    {
        public string Command => "unm";

        public string[] Aliases => new[] { "unmodmode" };

        public string Description => "Undo .modmode";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);
            var success = player.DisableModMode(out response);
            return success;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]

    public class Die : ICommand
    {
        public string Command => "reset";

        public string[] Aliases => new string[] { "spectator" };

        public string Description => "Set your class to spectator, and disable moderation abilities. Soft rejoin basically.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);
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

    public class InfiniteAmmo : ICommand
    {
        public string Command => "infinite";

        public string[] Aliases => new string[] { "infiniteammo", "i" };

        public string Description => "[BROKEN] Toggle infinite ammo for a player id or all players";

        public static void ShowBroadcast(Player player)
        {
            if (Plugin.infiniteAmmoForAllPlayers || Plugin.infiniteAmmoPlayers.Contains(player))
            {
                player.Broadcast(5, "<color=#00ffc0>Infinite ammo enabled</color>");
            }
            else
            {
                player.Broadcast(5, "<color=#ff4eac>Infinite ammo disabled</color>");
            }
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.ForceclassWithoutRestrictions)) // junior mod
            {
                response = "You do not have sufficient permissions to use this command";
                return false;
            }
            if (arguments.IsEmpty())
            {
                if (!sender.CheckPermission(PlayerPermissions.GivingItems)) // admins+
                {
                    response = "Usage: infinite <\"all\" or player id/name>";
                    return false;
                }
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
            if (!sender.CheckPermission(PlayerPermissions.Effects))
            {
                response = "Usage: \"infinite all\"";
                return false;
            }
            if (Player.TryGet(arguments.FirstElement(), out Player target))
            {
                if (!sender.CheckPermission(PlayerPermissions.GivingItems) // admin+
                    && Player.TryGet(sender, out Player playerSender)
                    && playerSender == target)
                {
                    response = "You cannot give infinite ammo to yourself";
                    return false;
                }
                Plugin.infiniteAmmoPlayers.Toggle(target);
                response = Plugin.infiniteAmmoPlayers.Contains(target) ? $"Enabled infinite ammo for {target.Nickname}" : $"Disabled infinite ammo for {target.Nickname}";
                ShowBroadcast(target);
                return true;
            }
            response = "Usage: infinite <username, id, \"all\", or empty>";
            return false;
        }
    }

    [CommandHandler(typeof(RemoteAdminCommandHandler))]

    public class TelsaImmune : ICommand
    {
        public string Command => "teslaimmune";

        public string[] Aliases => new string[] { "ti" };

        public string Description => "Toggle telsa gate immunity for a player or all players";
        static List<Player> teslaImmunePlayers = new();
        static bool telsaImmunityForAllPlayers = false;
        static DateTime lastUsed = new();

        // needs to be registered
        public static void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (telsaImmunityForAllPlayers || teslaImmunePlayers.Contains(ev.Player))
            {
                ev.IsAllowed = false;
            }
        }

        public static void ShowBroadcast(Player player)
        {
            if (telsaImmunityForAllPlayers || teslaImmunePlayers.Contains(player))
            {
                player.Broadcast(5, "<color=#00ffc0>Tesla immunity enabled</color>");
            }
            else
            {
                player.Broadcast(5, "<color=#ff4eac>Tesla immunity disabled</color>");
            }
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (lastUsed < Round.StartedTime)
            {
                teslaImmunePlayers.Clear();
                telsaImmunityForAllPlayers = false;
            }
            lastUsed = DateTime.Now;

            if (!sender.CheckPermission(PlayerPermissions.Effects))
            {
                response = "You do not have sufficient permissions to use this command";
                return false;
            }
            if (arguments.IsEmpty())
            {
                if (Player.TryGet(sender, out Player player))
                {
                    teslaImmunePlayers.Toggle(player);
                    response = teslaImmunePlayers.Contains(player) ? "Enabled tesla immunity" : "Disabled tesla immunity";
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
                telsaImmunityForAllPlayers.Toggle();
                response = telsaImmunityForAllPlayers ? "Enabled tesla immunity for all players" : "Disabled tesla immunity for all players";
                foreach (Player player in Player.List)
                {
                    ShowBroadcast(player);
                }
                return true;
            }
            if (Player.TryGet(arguments.FirstElement(), out Player target))
            {
                teslaImmunePlayers.Toggle(target);
                response = teslaImmunePlayers.Contains(target) ? $"Enabled tesla immunity for {target.Nickname}" : $"Disabled tesla immunity for {target.Nickname}";
                ShowBroadcast(target);
                return true;
            }
            response = "Usage: teslaimmune <username, id, \"all\", or empty>";
            return false;
        }
    }
}