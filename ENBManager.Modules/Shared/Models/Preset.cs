using System.Collections.Generic;

namespace ENBManager.Modules.Shared.Models
{
    public class Preset
    {
        public string Name { get; set; }
        public IEnumerable<string> Files { get; set; }
    }
}
