using CommandSystem;
using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPReplacer
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
            if (player.Role != RoleType.Spectator)
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
                if (target.IsAlive)
                {
                    player.Teleport(target);
                }
                else
                {
                    player.Broadcast(new Exiled.API.Features.Broadcast(
                        Plugin.Singleton.Translation.BroadcastHeader
                        + Plugin.Singleton.Translation.TargetDiedError
                    ));
                    // Due to the async nature of this lambda, we cannot modify the response or
                    // success boolean (though doing so is not strictly necessary as the response
                    // still describes the enabling of modmode)
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

            player.SetRole(RoleType.Spectator);

            var success = player.DisableModMode(out response);
            return success;
        }
    }
}