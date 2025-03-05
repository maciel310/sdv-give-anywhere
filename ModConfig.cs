using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiveAnywhere
{
    public sealed class ModConfig
    {
        public KeybindList TriggerKey { get; set; } = KeybindList.Parse("BigButton + ControllerA, LeftControl + G");
    }
}
