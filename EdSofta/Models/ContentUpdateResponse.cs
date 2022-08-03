﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    internal class ContentUpdateResponse
    {
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool status { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string message { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string url { get; set; }
    }
}
