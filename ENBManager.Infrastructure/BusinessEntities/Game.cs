using System;
using System.Collections.Generic;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class Game
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }
        public bool Active { get; set; }

        public ICollection<Preset> Presets { get; set; }
    }
}
