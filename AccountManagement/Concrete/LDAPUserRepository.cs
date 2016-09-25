using AccountManagement.Abstract;
using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Concrete
{
    public class LDAPUserRepository : IUserRepository
    {
        private LDAPDbContext context = new LDAPDbContext();

        public IQueryable<User> Users
        {
            set { Users = value; }
            get { return context.Users; }
        }

        public bool Create(User user)
        {
            return context.CreateUser(user);
        }

        public bool Edit(User user)
        {
            return context.EditUser(user);
        }

        public bool Move(User user, Group parentGroup) 
        { 
            return context.MoveUser(user, parentGroup);
        }

        public bool Delete(User user)
        {
            return context.DeleteUser(user);
        }

    }
}
