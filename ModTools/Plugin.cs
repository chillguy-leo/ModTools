using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System;

namespace SCPReplacer
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
            var message = Translation.KickNotification
                .Replace("%USER", args.Target.Nickname)
                .Replace("%REASON%", args.Reason);
            Map.Broadcast(new Exiled.API.Features.Broadcast(message, 5, true, Broadcast.BroadcastFlags.AdminChat));
        }
    }
}