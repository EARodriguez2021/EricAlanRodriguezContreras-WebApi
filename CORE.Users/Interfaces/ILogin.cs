using System;
using System.Collections.Generic;
using System.Text;

namespace CORE.Users.Interfaces
{
    public interface ILogin : IDisposable
    {
        Models.LoginModel Login(Models.LoginMinModel user);
    }
}
