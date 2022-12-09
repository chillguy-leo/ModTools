using Exiled.API.Interfaces;
using System.ComponentModel;

namespace SCPReplacer
{
    public class Config : IConfig
    {
        [Description("Is the plugin enabled?")]
        public bool IsEnabled { get; set; } = true;
    }
}