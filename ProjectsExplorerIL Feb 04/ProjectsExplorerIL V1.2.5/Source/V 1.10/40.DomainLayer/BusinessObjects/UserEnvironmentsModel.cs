using System;
using System.IO;
using Infra.Domain;
namespace ATSBusinessObjects
{
    public class UserEnvironmentsModel
    {
        public int environmentId { get; set; }
        public int userId { get; set; }
        public string environmentName { get; set; }
        public string userName { get; set; }
        public string connectionString { get; set; }
    }
}// end of namespace ATSBusinessObjects


