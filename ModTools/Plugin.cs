using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using System;
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

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            // Deregister event handlers
            Exiled.Events.Handlers.Player.Kicked -= OnKick;

            // This will prevent commands and other classes from being able to access
            // any state while the plugin is disabled
            Singleton = null;

            base.OnDisabled();
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
    }
}