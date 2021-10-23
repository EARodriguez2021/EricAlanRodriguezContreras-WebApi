using CORE.Connection.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CORE.Users.Interfaces;
using CORE.Users.Models;
using System.Data.SqlClient;

namespace CORE.Users.Services
{
    public class LoginService: ILogin,IDisposable
    {
        private bool disposedValue;
        private IConnectionDB<LoginModel> _conn;
        Dapper.DynamicParameters _parameters = new Dapper.DynamicParameters();
        public LoginService(IConnectionDB<LoginModel> conn)
        {
            _conn = conn;
        }

        public Models.LoginModel Login(Models.LoginMinModel user)
        {
            try
            {
                LoginModel model = new LoginModel();
                user.Password = Tools.SHA2.GetSHA256(user.Password); //Encripción en SHA256
                _parameters.Add("@p_login_json", JsonConvert.SerializeObject(user), DbType.String, ParameterDirection.Input);
                _conn.PrepararProcedimiento("dbo.[USERS.Login]", _parameters);
                var Json = (string)_conn.QueryFirstOrDefaultDapper(Connection.Models.TipoDato.Cadena);
                if (Json != string.Empty)
                {
                    JArray arr = JArray.Parse(Json);
                    foreach (JObject jsonOperaciones in arr.Children<JObject>())
                    {
                        model = new LoginModel()
                        {
                            Id = Convert.ToInt32(jsonOperaciones["Id"].ToString()),
                            Name = jsonOperaciones["Name"].ToString(),
                            LastName = jsonOperaciones["LastName"].ToString(),
                        };

                    }
                }
                return model;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(sqlEx.Message);
            }
            catch (MySql.Data.MySqlClient.MySqlException mysqlEx)
            {
                throw new Exception(mysqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _conn.Dispose();
            }
        }

        #region Dispose

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _conn.Dispose();// TODO: eliminar el estado administrado (objetos administrados)
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~MinervaService()
        // {
        //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
