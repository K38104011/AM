using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountManagement.Models
{
    public class TreeNode
    {
        public string text { get; set; }
        public string icon { get; set; }
        public string dn { get; set; }
        public string parent { get; set; }
        public List<TreeNode> nodes { get; set; }
    }
}