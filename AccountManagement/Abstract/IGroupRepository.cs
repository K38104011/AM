using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManagement.Models;

namespace AccountManagement.Abstract
{
    public interface IGroupRepository
    {
        IQueryable<Group> Groups { get; set; }
        bool Edit(Group group);
        bool Create(Group group);
        bool CreateManyGroups(List<Group> lstGroup);
        Group Delete(string Name);
        bool Move(Group group, Group parentGroup);
        bool isExists(string Name);
        bool isHasChild(string Name);
    }
}
