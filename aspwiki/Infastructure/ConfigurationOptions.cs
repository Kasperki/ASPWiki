using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPWiki.Infastructure
{
    public class ConfigurationOptions
    {
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }

        public string TwitterKey { get; set; }
        public string TwitterKeySecret { get; set; }

        public ConfigurationOptions()
        {
            DatabaseUser = "root";
            DatabasePassword = "root";
        }
    }
}
