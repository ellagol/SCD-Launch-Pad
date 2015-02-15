using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerProvider
{
    public enum PathType
    {
        Relative,
        Full
    }

    public class PathFS
    {
        public String Name { get; set; }
        public PathType Type { get; set; }
    }
}
