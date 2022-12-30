using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace QOIComarisonImprovement
{
    [SupportedOSPlatform("windows")]
    internal class PNG : CSExistingFormat
    {
        public override ImageFormat Format => ImageFormat.Png;

        public override string Name => "PNG";
    }
}
