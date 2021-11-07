using CORE.Users.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CORE.Users.Models;
using CORE.Users.Services;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;

        public UserController(IUser user)
        {
            _user = user;
        }

        ///https://localhost:5001/api/User/GetUsers
        [HttpGet]
        [Route("[action]")]
        public IEnumerable<UserModel> GetUsers()
        {
            List<UserModel> model = new List<UserModel>();
            using (IUser User = FactorizerService.Inicializar(EServer.CLOUD))
            {
                model = User.GetUsers();
            }

            return model;
        }

        //https://localhost:5001/api/User/GetUser?ID=2
        [HttpGet]
        [Route("GetUser")]
        public ActionResult<CORE.Users.Models.UserModel> GetUser(int ID)
        {
            if (ID == 0)
                return BadRequest("Ingrese un ID válido");

            UserModel model = new UserModel();
            using (IUser User = FactorizerService.Inicializar(EServer.CLOUD))
            {
                model = User.GetUser(ID);
            }

            return model;
        }

        //https://localhost:5001/api/User/AddUser
        [HttpPost]
        [Route("[action]")]
        public ActionResult AddUser(UserModel user)
        {
            if (user == null)
                return BadRequest("Ingrese información de usuario");

            long model = 0;
            using (IUser User = FactorizerService.Inicializar(EServer.CLOUD))
            {
                model = User.AddUser(user);
            }

            return model > 0 ? Ok() : BadRequest("Error al insertar");
        }

        //https://localhost:5001/api/User/UpdateUser
        [HttpPost]
        [Route("[action]")]
        public ActionResult UpdateUser(UserModel user)
        {
            if (user.Identificador == 0)
                return BadRequest("Ingrese un ID válido");

            bool model = false;
            using (IUser User = FactorizerService.Inicializar(EServer.CLOUD))
            {
                model = User.UpdateUser(user);
            }

            return model == true ? Ok() : BadRequest("Error al actualizar");
        }

        //https://localhost:5001/api/user/deleteuser?ID=2
        [HttpDelete]
        [Route("[action]")]
        public ActionResult DeleteUser(int ID)
        {
            if (ID == 0)
                return BadRequest("Ingrese un ID válido");

            using (IUser User = FactorizerService.Inicializar(EServer.CLOUD))
            {
                User.DeleteUser(ID);
            }

            return Ok();
        }
    }
}
