using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountManagement.Models
{
    public class SimpleEntry
    {
        public string Name { get; set; }
        public string fullDn { get; set; }
        public string shortDn { get; set; }
        public int Length { get; set; }
    }
}