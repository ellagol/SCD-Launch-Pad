using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerProvider
{
    public class ContentStatus : LastUpdate
    {
        public String ID { get; set; }
        public String Name { get; set; }
        public String Icon { get; set; }
    }
}
