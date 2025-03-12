using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModTools
{
    public static class Util
    {
        public static string SentenceCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public static bool EnableModMode(this Player player, out string response, bool retainPosition = false)
        {
            if (!player.CheckPermission(PlayerPermissions.ForceclassSelf))
            {
                response = Plugin.Singleton.Translation.NoForceClassPerm;
                return false;
            }

            foreach (CustomRole role in player.GetCustomRoles())
                role.RemoveRole(player);

            // A list of descriptions of changes that we'll send to the user
            var changes = new List<string>();

            // Forceclass tends not to work when overwatch is enabled
            player.IsOverwatchEnabled = false;

            var spawnFlags = retainPosition ? RoleSpawnFlags.AssignInventory : RoleSpawnFlags.All;
            player.Role.Set(RoleTypeId.Tutorial, Exiled.API.Enums.SpawnReason.ForceClass, spawnFlags);
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

            if (player.CheckPermission(PlayerPermissions.PlayersManagement))
            {
                player.IsGodModeEnabled = true;
                changes.Add(Plugin.Singleton.Translation.GodmodeEnabled);
            }

            response = String.Join(", ", changes).SentenceCase();

            player.Broadcast(new Exiled.API.Features.Broadcast(
                Plugin.Singleton.Translation.BroadcastHeader + response
            ));
            return true;
        }

        public static bool DisableModMode(this Player player, out string response)
        {
            var changes = new List<string>();

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
        public static Player FindSpectatingTargetOrNull(this Player player)
        {
            try
            {
                return Player.List.First(p => p != player && p.CurrentSpectatingPlayers.Contains(player));
            }
            catch
            {
                return null;
            }
        }

        public static string S(int count)
        {
            return count switch
            {
                1 => "",
                _ => "s"
            };
        }

        public static void Toggle<T>(this List<T> list, T elem)
        {
            if (!list.Remove(elem))
            {
                list.Add(elem);
            }
        }

        public static void Toggle(this ref bool val)
        {
            val = !val;
        }

    }


}