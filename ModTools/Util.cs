using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCPReplacer
{
    public static class Util
    {
        /// <summary>
        /// Given a stirng, capitalize the first character and make the rest lowercase.
        /// </summary>
        /// <param name="input">A string in any case</param>
        /// <returns>The string with only the first character capitalized</returns>
        public static string SentenceCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public static bool EnableModMode(this Player player, out string response)
        {
            if (!player.CheckPermission(PlayerPermissions.ForceclassSelf))
            {
                response = Plugin.Singleton.Translation.NoForceClassPerm;
                return false;
            }

            // A list of descriptions of changes that we'll send to the user
            var changes = new List<string>();

            // Forceclass tends not to work when overwatch is enabled
            player.IsOverwatchEnabled = false;
            player.Role.Set(RoleTypeId.Tutorial);
            player.BadgeHidden = false;
            changes.Add(Plugin.Singleton.Translation.TagShown);

            if (player.CheckPermission(PlayerPermissions.Noclip))
            {
                FpcNoclip.PermitPlayer(player.ReferenceHub);
                changes.Add(Plugin.Singleton.Translation.NoclipEnabled);
            }

            if (player.CheckPermission(PlayerPermissions.FacilityManagement))
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

        public static bool DisableModMode(this Player player, out string response)
        {
            // A list of descriptions of changes that we'll send to the user
            var changes = new List<string>();

            // Note: it would be possible to check whether e.g. noclip is already enabled
            // (rather than just whether the user has permission); however, I've elected
            // not to so in order for staff to not have to guess or remember whether it's
            // still enabled. For instance, a moderator might want to use it to double check
            // that noclip is indeed disabled.

            if (player.CheckPermission(PlayerPermissions.Noclip))
            {
                FpcNoclip.UnpermitPlayer(player.ReferenceHub);
                changes.Add(Plugin.Singleton.Translation.NoclipDisabled);
            }

            if (player.CheckPermission(PlayerPermissions.FacilityManagement))
            {
                player.IsBypassModeEnabled = false;
                changes.Add(Plugin.Singleton.Translation.BypassDisabled);
            }

            if (player.CheckPermission(PlayerPermissions.PlayersManagement))
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

        /// <summary>
        /// Find who the player is currently spectating, or null if this is not possible
        /// 
        /// Note: Nullable types would be preferable but aren't available before C# 8.0
        /// </summary>
        /// <param name="player">The spectator</param>
        /// <returns>The target of the spectator</returns>
        public static Player FindSpectatingTargetOrNull(this Player player)
        {
            try
            {
                return Player.List.First(p => p.CurrentSpectatingPlayers.Contains(player));
            }
            catch
            {
                return null;
            }
        }
    }


}