using AccountManagement.Abstract;
using AccountManagement.Concrete;
using AccountManagement.Models;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountManagement.Infrastructure
{
    public class LDAPHelper
    {
        private IGroupRepository groupRepository;
        private IUserRepository userRepository;

        public LDAPHelper() : this(new LDAPGroupRepository(), new LDAPUserRepository()) { }

        public LDAPHelper(IGroupRepository groupRepo, IUserRepository userRepo)
        {
            this.groupRepository = groupRepo;
            this.userRepository = userRepo;
        }

        /// <summary>
        /// Get root tree node with icon
        /// </summary>
        /// <param name="baseDn">Dn of People ou</param>
        /// <param name="imageGroup">icon of group</param>
        /// <param name="imageUser">icon of user</param>
        /// <returns></returns>
        public TreeNode GetRootTreeNode(string iconGroup, string iconUser)
        {
            List<SimpleEntry> dnList = new List<SimpleEntry>();

            #region Sort dn by level
            foreach (User user in userRepository.Users)
            {
                SimpleEntry simpleEntry = new SimpleEntry();

                int position = user.Dn.ToString().IndexOf(",dc=maxcrc,dc=com");
                string shortDN = user.Dn.ToString().Substring(0, position);

                simpleEntry.shortDn = shortDN;
                simpleEntry.fullDn = user.Dn;

                string[] level = shortDN.Split(',');
                simpleEntry.Length = level.Length;
                simpleEntry.Name = level[0].Substring(4, level[0].Length - 4);

                dnList.Add(simpleEntry);
            }

            foreach (Group group in groupRepository.Groups)
            {
                SimpleEntry simpleEntry = new SimpleEntry();

                int position = group.dn.ToString().IndexOf(",dc=maxcrc,dc=com");
                string shortDN = group.dn.ToString().Substring(0, position);

                simpleEntry.shortDn = shortDN;
                simpleEntry.fullDn = group.dn;

                string[] level = shortDN.Split(',');
                simpleEntry.Length = level.Length;
                simpleEntry.Name = level[0].Substring(3, level[0].Length - 3);

                dnList.Add(simpleEntry);
            }
            
            List<SimpleEntry> entries = dnList.OrderBy(s => s.Length).ToList<SimpleEntry>();
            #endregion

            TreeNode root = new TreeNode
            {
                text = entries[0].Name,
                icon = iconGroup,
                dn = entries[0].fullDn,
                parent = "",
            };

            TreeNode item, parent;
            root.nodes = new List<TreeNode>();
            
            #region create root TreeNode with icon
            for (int i = 0; i < entries.Count(); i++)
            {
                item = new TreeNode();
                string[] element = entries[i].shortDn.Split(',');

                if (entries[i].Length > 1)
                {
                    if (entries[i].shortDn.Substring(0, 3) == "uid")
                    {
                        item.text = element[0].Substring(4, element[0].Length - 4);
                        item.icon = iconUser;
                        item.dn = entries[i].fullDn;
                        item.parent = element[1].Substring(3, element[1].Length - 3);
                    }
                    else if (entries[i].shortDn.Substring(0, 2) == "ou")
                    {
                        item.text = element[0].Substring(3, element[0].Length - 3);
                        item.icon = iconGroup;
                        item.dn = entries[i].fullDn;
                        item.parent = element[1].Substring(3, element[1].Length - 3);
                    }
                }

                if (entries[i].Length == 2)
                {
                    root.nodes.Add(item);
                }
                else if (entries[i].Length > 2)
                {
                    parent = new TreeNode();
                    string textParent, nameGroupLv2;

                    for (int ii = 0; ii < root.nodes.Count(); ii++)
                    {
                        nameGroupLv2 = element[element.Count() - 2].Substring(3, element[element.Count() - 2].Length - 3);
                        parent = root.nodes.Find(e => e.text == nameGroupLv2);
                        parent.nodes = parent.nodes ?? new List<TreeNode>();
                    }

                    for (int ii = element.Count() - 3; ii >= 1; ii--)
                    {
                        textParent = element[ii].Substring(3, element[ii].Length - 3);
                        parent = parent.nodes.Find(e => e.text == textParent);
                        parent.nodes = parent.nodes ?? new List<TreeNode>();
                    }
                    parent.nodes.Add(item);
                }
            }
            #endregion

            return root;
        }

        public bool ExportGroupToExcel(Group group)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                System.IO.FileInfo file = new System.IO.FileInfo(path + @"\" + group.name + ".xlsx");

                using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage(file))
                {
                    var ws = pck.Workbook.Worksheets.Add("Users");
                    ws.Cells["A1"].Value = "Account";
                    ws.Cells["B1"].Value = "Role";
                    ws.Cells["C1"].Value = "Email";
                    ws.Cells["D1"].Value = "First Name";
                    ws.Cells["E1"].Value = "Last Name";
                    ws.Cells["F1"].Value = "Phone";
                    
                    var header = ws.Cells["A1:F1"];
                    header.Style.Font.Bold = true;
                    var fill = header.Style.Fill;
                    fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    
                    IEnumerable<User> users = userRepository.Users.Where(u => u.ParentGroup == group.name);
                    int index = 1, prevRow = 0;
                    foreach (User u in users)
                    {
                        ws.Cells["A" + ++index].Value = u.Account;
                        ws.Cells["B" + index].Value = u.Roles;
                        ws.Cells["C" + index].Value = u.Email;
                        ws.Cells["D" + index].Value = u.FirstName;
                        ws.Cells["E" + index].Value = u.LastName;
                        ws.Cells["F" + index].Value = u.Phone;
                        prevRow = index;
                    }
                    
                    ws.Cells[prevRow + 1, 5].Value = "The number of users:";
                    ws.Cells[prevRow + 1, 6].Formula = string.Format("COUNTA({0})", OfficeOpenXml.ExcelCellBase.GetAddress(2, 1, prevRow, 1));
                    ws.Cells[1, 1, prevRow + 1, 6].AutoFitColumns(15);

                    pck.SaveAs(file);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}