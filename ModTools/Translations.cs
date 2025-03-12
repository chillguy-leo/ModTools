﻿using Exiled.API.Interfaces;

namespace ModTools
{
    public class Translations : ITranslation
    {
        public string KickNotification { get; set; } = "%USER% kicked with reason \"%REASON%\"";
        public string NoForceClassPerm { get; set; } = "You do not have permission to change your class to Tutorial.";
        public string TagShown { get; set; } = "Tag shown";
        public string NoclipEnabled { get; set; } = "Noclip enabled";
        public string NoclipDisabled { get; set; } = "Noclip disabled";
        public string BypassEnabled { get; set; } = "Bypass enabled";
        public string BypassDisabled { get; set; } = "Bypass disabled";
        public string GodmodeEnabled { get; set; } = "Godmode enabled";
        public string GodmodeDisabled { get; set; } = "Godmode disabled";
        public string InsufficientPermissions { get; set; } = "You do not have sufficient permissions to use this command";
        public string NotSpectatorError { get; set; } = "You must be a spectator to use this command.";
        public string CantFindTargetError { get; set; } = "Please ensure you are spectating a specific player (rather than the default spectator view) first before running this command.";
        public string TargetIsSelfError { get; set; } = "Cannot find valid spectating target (target has same user ID as you). Make sure you are spectating a specific player (rather than the default spectator view) first before running this command.";
        public string TargetDiedError { get; set; } = "The spectated player is no longer alive.";


        public string BroadcastHeader { get; set; } = "<color=yellow>[Chaos Theory Mod Mode]</color>\n";
    }
}