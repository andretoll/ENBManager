using System;
using System.Collections.Generic;
using System.Drawing;

namespace ENBManager.Infrastructure.BusinessEntities
{
    public class Preset
    {
        public Guid Uid { get; set; }
        public string Title { get; set; }

        public ICollection<Image> Screenshots { get; set; }
    }
}
