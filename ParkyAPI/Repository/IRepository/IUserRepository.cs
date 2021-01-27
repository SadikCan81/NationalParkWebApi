using ParkyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string email);

        User Authenticate(string email, string password);

        User Register(User user);
    }
}
