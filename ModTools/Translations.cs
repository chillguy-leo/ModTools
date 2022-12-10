using Exiled.API.Interfaces;

namespace SCPReplacer
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


        public string BroadcastHeader { get; set; } = "<color=yellow>[Chaos Theory Mod Tools]</color>\n";
    }
}