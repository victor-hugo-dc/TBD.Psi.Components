// Copyright (c) Carnegie Mellon University. All rights reserved.
// Licensed under the MIT license.

namespace TBD.Psi.RosBagStreamReader
{
    using Microsoft.Psi.Data;

    [StreamReader("ROS Bags", ".bag")]
    public class RosBagStreamReaderNET : RosBagStreamReader, IStreamReader
    {
        public RosBagStreamReaderNET(string name, string path)
            : base(name, path, new RosBagReaderNET())
            {
            }
    }
}


