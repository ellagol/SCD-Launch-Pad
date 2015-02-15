using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContentManagerProvider
{
    public class ContentVersionSubVersion : LastUpdate
    {
        public int Order;
        public Content Content;
        public ContentVersion ContentSubVersion;
    }
}
