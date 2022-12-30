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
    internal class JPEG : CSExistingFormat
    {
        public override ImageFormat Format => ImageFormat.Jpeg;

        public override string Name => "JPEG (lossy)";

        protected override bool LossyCompression => true;
    }
}
