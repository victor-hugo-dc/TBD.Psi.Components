using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBD.Psi.RosBagStreamReader.Windows
{
    public class RosBagReaderWindows : RosBagReader
    {
        public RosBagReaderWindows()
            : base()
        {
            this.loadDeserializer(new Deserializers.SensorMsgsCompressedImageAsDepthImageDeserializer(true), "depth");
        }
    }
}
