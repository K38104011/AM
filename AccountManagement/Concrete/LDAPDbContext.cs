using AccountManagement.Models;
using Novell.Directory.Ldap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountManagement.Concrete
{
    public class LDAPDbContext
    {
        public IQueryable<Group> Groups { get { return GetGroups(); } }

        public IQueryable<User> Users { get { return GetUsers(); } }

        public LDAPDbContext()
        {
            ldapConn = new LdapConnection();
            ldapConn.Connect(ldapHost, ldapPort);
            ldapConn.Bind(userDN, userPasswd);
        }

        public bool CreateUser(User user)
        {
            try
            {
                string hashedPassword = "";
                using (System.Security.Cryptography.SHA256 hash = System.Security.Cryptography.SHA256Managed.Create())
                {
                    hashedPassword = String.Join("", hash
                        .ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.Password))
                        .Select(item => item.ToString("x2")));
                }
                LdapAttributeSet attributeSet = new LdapAttributeSet();
                attributeSet.Add(new LdapAttribute("objectclass", "inetOrgPerson"));
                attributeSet.Add(new LdapAttribute("mail", user.Email));
                attributeSet.Add(new LdapAttribute("givenName", user.LastName));
                attributeSet.Add(new LdapAttribute("sn", user.FirstName));
                attributeSet.Add(new LdapAttribute("telephoneNumber", user.Phone));
                attributeSet.Add(new LdapAttribute("cn", new string[] { "User" }));
                attributeSet.Add(new LdapAttribute("userPassword", hashedPassword));
                string dn = "uid=" + user.Account + "," + user.ParentDn;
                LdapEntry newEntry = new LdapEntry(dn, attributeSet);
                ldapConn.Add(newEntry);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool CreateGroup(Group group)
        {
            LdapAttributeSet attributeSet = new LdapAttributeSet();
            attributeSet.Add(new LdapAttribute("objectclass", "organizationalUnit"));
            string dn = "ou=" + group.name + ", " + group.parentDn;
            LdapEntry newEntry = new LdapEntry(dn, attributeSet);

            try
            {
                ldapConn.Add(newEntry);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// CreateManyGroups
        /// </summary>
        /// <param name="lstGroupName">list group name</param>
        /// <param name="parentDn">name of parent</param>
        /// <returns></returns>
        public bool CreateManyGroups(List<Group> lstGroup)
        {
            if (lstGroup != null)
            {
                int count = lstGroup.Count;
                foreach (var item in lstGroup)
                {
                    if (!CreateGroup(item))
                        return false;                        
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EditGroup(Group group)
        {
            try
            {
                ldapConn.Rename(group.dn, "ou=" + group.name, true);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool DeleteGroup(Group group)
        {
            try
            {
                ldapConn.Delete(group.dn);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool MoveGroup(Group group, Group parentGroup)
        {
            try
            {
                ldapConn.Rename(group.dn, "ou=" + group.name, parentGroup.dn, false);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool MoveUser(User user, Group parentGroup)
        {
            try
            {
                ldapConn.Rename(user.Dn, "uid=" + user.Account, parentGroup.dn, false);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool DeleteUser(User user)
        {
            try
            {
                ldapConn.Delete(user.Dn);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool EditUser(User user)
        {
            string hashedPassword = "";
            using (System.Security.Cryptography.SHA256 hash = System.Security.Cryptography.SHA256Managed.Create())
            {
                hashedPassword = String.Join("", hash
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.Password))
                    .Select(item => item.ToString("x2")));
            }

            ArrayList modList = new ArrayList();
            List<LdapAttribute> AttrList = new List<LdapAttribute>();

            AttrList.Add(new LdapAttribute("mail", user.Email));
            AttrList.Add(new LdapAttribute("givenName", user.LastName));
            AttrList.Add(new LdapAttribute("sn", user.FirstName));
            AttrList.Add(new LdapAttribute("telephoneNumber", user.Phone));
            AttrList.Add(new LdapAttribute("userPassword", hashedPassword));

            try
            {
                foreach (LdapAttribute Attr in AttrList)
                {
                    modList.Add(new LdapModification(LdapModification.REPLACE, Attr));
                }
                LdapModification[] mods = new LdapModification[modList.Count];
                Type mtype = Type.GetType("Novell.Directory.LdapModification");
                mods = (LdapModification[])modList.ToArray(typeof(LdapModification));
                ldapConn.Modify(user.Dn, mods);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public int CountChild(Group group)
        {
            LdapSearchResults lsc = ldapConn.Search(group.dn, LdapConnection.SCOPE_SUB, "objectClass=*", null, false);

            int countChild = 0;

            while (lsc.hasMore())
            {
                countChild++;
                lsc.next();
            }

            return countChild;
        }

        private IQueryable<Group> GetGroups()
        {
            LdapSearchResults lsc = ldapConn.Search(baseDn, LdapConnection.SCOPE_SUB, "objectClass=*", null, false);
            List<Group> list = new List<Group>();

            while (lsc.hasMore())
            {
                LdapEntry nextEntry = null;
                Group group = new Group();

                nextEntry = lsc.next();
                if (nextEntry.getAttribute("ou") == null) continue;

                group.name = nextEntry.getAttribute("ou").StringValue;
                group.dn = nextEntry.DN;
                string[] dnSplit = nextEntry.DN.Trim().Split(',');
                group.parent = dnSplit[1].Substring(3, dnSplit[1].Length - 3);

                list.Add(group);
            }

            return list.AsQueryable();
        }

        private IQueryable<User> GetUsers()
        {
            LdapSearchResults lsc = ldapConn.Search(baseDn, LdapConnection.SCOPE_SUB, "objectClass=*", null, false);
            List<User> list = new List<User>();

            while (lsc.hasMore())
            {
                LdapEntry nextEntry = null;
                nextEntry = lsc.next();

                if (nextEntry.getAttribute("uid") == null) continue;

                string[] levels = nextEntry.DN.Split(',');
                User user = new User
                {
                    Account = nextEntry.getAttribute("uid").StringValue,
                    Email = nextEntry.getAttribute("mail").StringValue,
                    FirstName = nextEntry.getAttribute("sn").StringValue,
                    LastName = nextEntry.getAttribute("givenName").StringValue,
                    Password = nextEntry.getAttribute("userPassword").StringValue,
                    Phone = nextEntry.getAttribute("telephoneNumber").StringValue,
                    Roles = nextEntry.getAttribute("cn").StringValue,
                    Dn = nextEntry.DN,
                    ParentGroup = levels[1].Substring(3, levels[1].Length - 3)
                };

                list.Add(user);
            }

            return list.AsQueryable();
        }

        ~LDAPDbContext()
        {
            ldapConn.Disconnect();
        }

        private const string baseDn = "ou=People, dc=maxcrc,dc=com";
        private const string ldapHost = "localhost";
        private const int ldapPort = 389;
        private const string userDN = "cn=Manager,dc=maxcrc,dc=com";
        private const string userPasswd = "secret";
        private LdapConnection ldapConn;

    }
    
}