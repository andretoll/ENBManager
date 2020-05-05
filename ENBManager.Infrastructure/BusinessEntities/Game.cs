using System;
using System.Collections.Generic;
using System.Drawing;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class Game
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public bool Active { get; set; }
        public Image Icon { get; set; }

        public ICollection<Preset> Presets { get; set; }
    }
}
