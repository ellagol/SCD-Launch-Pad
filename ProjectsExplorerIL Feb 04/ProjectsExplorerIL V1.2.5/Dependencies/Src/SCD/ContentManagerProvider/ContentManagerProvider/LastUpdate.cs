using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ContentManagerProvider
{
    public class LastUpdate
    {
        public DateTime LastUpdateTime { get; set; }
        public String LastUpdateUser { get; set; }
        public String LastUpdateComputer { get; set; }
        public String LastUpdateApplication { get; set; }

        public static void UpdateLastUpdate(LastUpdate source, LastUpdate destination)
        {
            destination.LastUpdateUser = source.LastUpdateUser;
            destination.LastUpdateTime = source.LastUpdateTime;
            destination.LastUpdateComputer = source.LastUpdateComputer;
            destination.LastUpdateApplication = source.LastUpdateApplication;
        }
    }
}
