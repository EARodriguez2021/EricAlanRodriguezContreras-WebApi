using CORE.Users.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
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
        public IEnumerable<CORE.Users.Models.UserModel> GetUsers()
        {
            List<CORE.Users.Models.UserModel> model = new List<CORE.Users.Models.UserModel>();
            using (IUser User = CORE.Users.Services.FactorizerService.Inicializar(CORE.Users.Models.EServer.LOCAL))
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

            CORE.Users.Models.UserModel model = new CORE.Users.Models.UserModel();
            using (IUser User = CORE.Users.Services.FactorizerService.Inicializar(CORE.Users.Models.EServer.LOCAL))
            {
                model = User.GetUser(ID);
            }

            return model;
        }

        //https://localhost:5001/api/User/AddUser
        [HttpPost]
        [Route("[action]")]
        public ActionResult AddUser(CORE.Users.Models.UserModel user)
        {
            if (user == null)
                return BadRequest("Ingrese información de usuario");

            long model = 0;
            using (IUser User = CORE.Users.Services.FactorizerService.Inicializar(CORE.Users.Models.EServer.LOCAL))
            {
                model = User.AddUser(user);
            }

            return model > 0 ? Ok() : BadRequest("Error al insertar");
        }

        //https://localhost:5001/api/User/UpdateUser
        [HttpPost]
        [Route("[action]")]
        public ActionResult UpdateUser(CORE.Users.Models.UserModel user)
        {
            if (user.Identificador == 0)
                return BadRequest("Ingrese un ID válido");

            bool model = false;
            using (IUser User = CORE.Users.Services.FactorizerService.Inicializar(CORE.Users.Models.EServer.LOCAL))
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

            using (IUser User = CORE.Users.Services.FactorizerService.Inicializar(CORE.Users.Models.EServer.LOCAL))
            {
                User.DeleteUser(ID);
            }

            return Ok();
        }
    }
}
