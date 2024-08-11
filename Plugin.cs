using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.CustomRoles.API;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModTools
{
    public class Plugin : Plugin<Config, Translations>
    {
        public override string Name => "Mod Tools";
        public override string Author => "Jon M";
        public override Version Version => new Version(1, 0, 0);

        // Singleton pattern allows easy access to the central state from other classes
        // (e.g. commands)
        public static Plugin Singleton { get; private set; }

        private bool restartTriggered = false;

        public override void OnEnabled()
        {
            // Set up the Singleton so we can easily get the instance with all the state
            // from another class.
            Singleton = this;

            // Register event handlers
            Exiled.Events.Handlers.Player.Kicked += OnKick;
            Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
            Exiled.Events.Handlers.Server.RestartingRound += OnRestartingRound;
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.SearchingPickup += OnPickup;
            Exiled.Events.Handlers.Player.Shooting += OnShooting;
            Exiled.Events.Handlers.Player.Verified += OnVerified;

            base.OnEnabled();
        }

        public DateTime serverStartTime = DateTime.Now;

        public static List<Player> infiniteAmmoPlayers = new();
        public static bool infiniteAmmoForAllPlayers = false;

        private void RestartIfNeeded() {
            if (restartTriggered) {
                Log.Info("Skipping redundant restart");
                return;
            }
            var dir = new DirectoryInfo("../.config/EXILED/Plugins");
            var updatedPlugins = dir.EnumerateFiles().Where(f => f.Name.EndsWith(".dll") && f.LastWriteTime > serverStartTime).Select(f => f.Name);
            if (updatedPlugins.IsEmpty())
            {
                Log.Info("No plugins have been updated since round start.");
            }
            else
            {
                restartTriggered = true;
                Log.Info($"The following plugins have been updated since last restart: {updatedPlugins}. Restarting the server to reload the plugins.");
                Server.Restart();
            }
        }

        public void OnVerified(VerifiedEventArgs ev) {
            if (Player.List.Any(x => x != ev.Player && !x.IsNPC))
                return;
            RestartIfNeeded();
        }

        public void OnRestartingRound()
        {
            RestartIfNeeded();
            props.Clear();
            infiniteAmmoPlayers.Clear();
            infiniteAmmoForAllPlayers = false;
        }

        public void OnKick(KickedEventArgs args)
        {
            if (args == null || args.Player == null || args.Player.Nickname == null || args.Reason == null)
                return;

            var message = Translation.KickNotification
                .Replace("%USER%", args.Player.Nickname)
                .Replace("%REASON%", args.Reason);

            var moderators = Player.List.Where(p => p != null && p.Sender != null && p.RemoteAdminAccess);

            foreach (Player moderator in moderators)
            {
                moderator.Broadcast(10, message, Broadcast.BroadcastFlags.AdminChat);
            }
        }

        public void OnPickup(SearchingPickupEventArgs ev)
        {
            if (props.Contains(ev.Pickup))
            {
                if (ev.Player.CheckPermission(PlayerPermissions.GivingItems))
                {
                    ev.Player.ShowHint("Destroying prop");
                    ev.Pickup.Destroy();
                    ev.IsAllowed = false;
                }
                else
                {
                    ev.Player.ShowHint("Props can't be picked up");
                    ev.IsAllowed = false;
                }
            }
        }

        public static List<Pickup> props = new();

        public void OnReceivingEffect(ReceivingEffectEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Tutorial
                && ev.Player.GetCustomRoles().IsEmpty()
                && ev.Effect.GetEffectType() is Exiled.API.Enums.EffectType.Flashed or Exiled.API.Enums.EffectType.Blinded)
                ev.IsAllowed = false;
        }

        public void OnShooting(ShootingEventArgs ev)
        {
            if ((infiniteAmmoPlayers.Contains(ev.Player) || infiniteAmmoForAllPlayers)
                && ev.Player.CurrentItem is Firearm gun)
            {
                gun.Ammo++;
            }
        }

        public void OnWaitingForPlayers() {
            Party.partyModeEnabled = false;
        }
    }
}