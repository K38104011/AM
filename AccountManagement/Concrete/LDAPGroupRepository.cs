using AccountManagement.Abstract;
using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountManagement.Concrete
{
    public class LDAPGroupRepository : IGroupRepository
    {
        private LDAPDbContext context = new LDAPDbContext();

        public IQueryable<Group> Groups
        {
            set { Groups = value; }
            get { return context.Groups; }
        }

        public bool Edit(Group group)
        {
            return context.EditGroup(group);
        }

        public bool Create(Group group)
        {
            return context.CreateGroup(group);
        }

        public bool CreateManyGroups(List<Group> lstGroup)
        {
            return context.CreateManyGroups(lstGroup);
        }

        public Group Delete(string Name)
        {
            Group dbEntry = context.Groups.FirstOrDefault(g => g.name == Name);
            if (dbEntry != null)
            {
                context.DeleteGroup(dbEntry);
            }
            return dbEntry;
        }

        public bool Move(Group group, Group parentGroup)
        {
            return context.MoveGroup(group, parentGroup);
        }

        public bool isExists(string Name)
        {
            Group existGroup = context.Groups.FirstOrDefault(g => g.name == Name);
            if (existGroup != null)
            {
                return true;
            }
            return false;
        }

        public bool isHasChild(string Dn)
        {
            Group group = context.Groups.FirstOrDefault(g => g.dn == Dn);
            if (group != null)
            {
                if (context.CountChild(group) > 1) return true;
            }
            return false;
        }

    }
}