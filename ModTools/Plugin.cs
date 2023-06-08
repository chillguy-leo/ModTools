using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System;
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


        public override void OnEnabled()
        {
            // Set up the Singleton so we can easily get the instance with all the state
            // from another class.
            Singleton = this;

            // Register event handlers
            Exiled.Events.Handlers.Player.Kicked += OnKick;
            Exiled.Events.Handlers.Player.ReceivingEffect += OnReceivingEffect;
            Exiled.Events.Handlers.Server.RestartingRound += OnRestartingRound;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            // Deregister event handlers
            Exiled.Events.Handlers.Player.Kicked -= OnKick;
            Exiled.Events.Handlers.Player.ReceivingEffect -= OnReceivingEffect;
            Exiled.Events.Handlers.Server.RestartingRound -= OnRestartingRound;


            // This will prevent commands and other classes from being able to access
            // any state while the plugin is disabled
            Singleton = null;

            base.OnDisabled();
        }

        const string ServerStartTimestampFilename = "RoundStartTimestamp";

        public DateTime serverStartTime = DateTime.Now;
        public bool restartTriggered = false;

        public void OnRestartingRound()
        {
            if (restartTriggered)
                return;
            var dir = new DirectoryInfo("../.config/EXILED/Plugins");
            var updatedPlugins = dir.EnumerateFiles().Where(f => f.Name.EndsWith(".dll") && f.LastWriteTime > serverStartTime).Select(f => f.Name);
            if (updatedPlugins.IsEmpty())
            {
                Log.Info("No plugins have been updated since round start.");
            }
            else
            {
                Log.Info($"The following plugins have been updated since last restart: {updatedPlugins}. Restarting the server to reload the plugins.");
                restartTriggered = true;
                Server.Restart();
            }
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

        public void OnReceivingEffect(ReceivingEffectEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Tutorial && ev.Player.GetCustomRoles().IsEmpty() && ev.Effect.Classification is CustomPlayerEffects.StatusEffectBase.EffectClassification.Negative)
                ev.IsAllowed = false;
        }
    }
}