using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;

namespace SCPReplacer
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ModMode : ICommand
    {
        public string Command => "modmode";

        public string[] Aliases => new[] { "mm" };

        public string Description => "Shortcut to disable overwatch, show tag, spawn as Tutorial, and enable noclip and bypass if able";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.ForceclassSelf))
            {
                response = Plugin.Singleton.Translation.NoForceClassPerm;
                return false;
            }
            var player = Player.Get(sender);


            // A list of descriptions of changes that we'll send to the user
            var changes = new List<string>();

            // Forceclass tends not to work when overwatch is enabled
            player.IsOverwatchEnabled = false;
            player.SetRole(RoleType.Tutorial);
            player.BadgeHidden = false;
            changes.Add(Plugin.Singleton.Translation.TagShown);

            if (sender.CheckPermission(PlayerPermissions.Noclip))
            {
                player.NoClipEnabled = true;

                // e.g. "Tag shown, noclip enabled"
                changes.Add(Plugin.Singleton.Translation.NoclipEnabled);
            }

            if (sender.CheckPermission(PlayerPermissions.FacilityManagement))
            {
                player.IsBypassModeEnabled = true;
                changes.Add(Plugin.Singleton.Translation.BypassEnabled);
            }

            response = String.Join(", ", changes).SentenceCase();

            player.Broadcast(new Exiled.API.Features.Broadcast(
              Plugin.Singleton.Translation.BroadcastHeader + response
             ));
            return true;
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

            // A list of descriptions of changes that we'll send to the user
            var changes = new List<string>();

            // Note: it would be possible to check whether e.g. noclip is already enabled
            // (rather than just whether the user has permission); however, I've elected
            // not to so in order for staff to not have to guess or remember whether it's
            // still enabled. For instance, a moderator might want to use it to double check
            // that noclip is indeed disabled.

            if (sender.CheckPermission(PlayerPermissions.Noclip))
            {
                player.NoClipEnabled = false;
                changes.Add(Plugin.Singleton.Translation.NoclipDisabled);
            }

            if (sender.CheckPermission(PlayerPermissions.FacilityManagement))
            {
                player.IsBypassModeEnabled = false;
                changes.Add(Plugin.Singleton.Translation.BypassDisabled);
            }

            if (sender.CheckPermission(PlayerPermissions.PlayersManagement))
            {
                player.IsGodModeEnabled = false;
                changes.Add(Plugin.Singleton.Translation.GodmodeDisabled);
            }

            if (changes.IsEmpty())
            {
                response = Plugin.Singleton.Translation.InsufficientPermissions;
                return false;
            }

            response = String.Join(", ", changes).SentenceCase();

            player.Broadcast(new Exiled.API.Features.Broadcast(
              Plugin.Singleton.Translation.BroadcastHeader + response
             ));
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
                Plugin.Singleton.Translation.BroadcastHeader + response
            ));
            return true;
        }
    }