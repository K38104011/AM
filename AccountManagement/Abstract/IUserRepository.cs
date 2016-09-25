using AccountManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagement.Abstract
{
	public interface IUserRepository
	{
		IQueryable<User> Users { get; set; }
		bool Create(User user);
        bool Edit(User user);
		bool Move(User user, Group parentGroup);
		bool Delete(User user);
	}
}
