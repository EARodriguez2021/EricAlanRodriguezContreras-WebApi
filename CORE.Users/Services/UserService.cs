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
using Dapper;

namespace CORE.Users.Services
{
    public class UserService: IUser,IDisposable
    {
        #region Dapper
#if false
        private bool disposedValue;
        private IConnectionDB<UserModel> _conn;
        private List<Tuple<string, object, int>> _parameters = new List<Tuple<string, object, int>>();
        string _connectionString = string.Empty;

        public UserService(IConnectionDB<UserModel> conn, string connectionString)
        {
            _conn = conn;
            this._connectionString = connectionString;
        }

        public UserService(IConnectionDB<UserModel> conn)
        {
            _conn = conn;
        }

        public List<Models.UserModel> GetUsers()
        {
            List<UserModel> list = new List<UserModel>();

            try
            {
                using (var connection = new SqlConnection(this._connectionString))
                {
                    var Json = connection.QueryFirstOrDefault<string>("dbo.[USERS.Get_All]", null, commandType: CommandType.StoredProcedure);

                    if (Json != string.Empty)
                    {
                        JArray arr = JArray.Parse(Json);
                        foreach (JObject jsonOperaciones in arr.Children<JObject>())
                        {
                            list.Add(new UserModel()
                            {
                                Identificador = Convert.ToInt32(jsonOperaciones["Id"].ToString()),
                                Name = jsonOperaciones["Name"].ToString(),
                                LastName = jsonOperaciones["LastName"].ToString(),
                                Nick = jsonOperaciones["Nick"].ToString(),
                                CreateDate = DateTime.Parse(jsonOperaciones["CreateDate"].ToString())
                            });

                        }
                    }


                }
                return list;
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
                _parameters.Clear();
            }
        }
        public Models.UserModel GetUser(int ID)
        {
            Models.UserModel UsuarioResp = null;
            try
            {
                using (var connection = new SqlConnection(this._connectionString))
                {
                    UsuarioResp = (Models.UserModel)connection.Query<UserModel>("dbo.[USERS.Get_Id]", new { Id = ID }, commandType: CommandType.StoredProcedure);
                }
                return UsuarioResp;
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
                _parameters.Clear();
            }
        }
        public long AddUser(Models.UserModel model)
        {
            long id = 0;

            try
            {
                using (var connection = new SqlConnection(this._connectionString))
                {
                    id = connection.QueryFirstOrDefault<long>("dbo.[USERS.Set]", new { p_user_json = JsonConvert.SerializeObject(model) }, commandType: CommandType.StoredProcedure);
                }

                return id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _parameters.Clear();
            }
        }
        public bool UpdateUser(Models.UserModel model)
        {
            try
            {
                bool reply = false;
                using (var connection = new SqlConnection(this._connectionString))
                {
                    var affectedRows = connection.Execute("dbo.[USERS.Update]", new { p_user_json = JsonConvert.SerializeObject(model) }, commandType: CommandType.StoredProcedure);

                    reply = affectedRows < 1 ? false : true;
                }

                return reply;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _parameters.Clear();
            }
        }
        public void DeleteUser(int ID)
        {
            try
            {
                using (var connection = new SqlConnection(this._connectionString))
                {
                    var affectedRows = connection.Execute("dbo.[USERS.Delete]", new { Id = ID }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _parameters.Clear();
            }
        }
#endif
        #endregion

        private bool disposedValue;
        private IConnectionDB<UserModel> _conn;
        private List<Tuple<string, object, int>> _parameters = new List<Tuple<string, object, int>>();

        public UserService(IConnectionDB<UserModel> conn)
        {
            _conn = conn;
        }

        public List<Models.UserModel> GetUsers()
        {
            try
            {
                List<UserModel> list = new List<UserModel>();

                _conn.PrepararProcedimiento("dbo.[USERS.Get_All]", _parameters);

                DataTableReader DTRResultados = _conn.EjecutarTableReader();
                while (DTRResultados.Read())
                {
                    var Json = DTRResultados["Usuario"].ToString();
                    if (Json != string.Empty)
                    {
                        JArray arr = JArray.Parse(Json);
                        foreach (JObject jsonOperaciones in arr.Children<JObject>())
                        {
                            //user = JsonConvert.DeserializeObject<User>(jsonOperaciones);
                            list.Add(new UserModel()
                            {
                                Identificador = Convert.ToInt32(jsonOperaciones["Id"].ToString()),
                                Name = jsonOperaciones["Name"].ToString(),
                                LastName = jsonOperaciones["LastName"].ToString(),
                                Nick = jsonOperaciones["Nick"].ToString(),
                                CreateDate = DateTime.Parse(jsonOperaciones["CreateDate"].ToString())
                            });

                        }
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _parameters.Clear();
            }
        }
        public Models.UserModel GetUser(int ID)
        {
            try
            {
                UserModel UsuarioResp = null;

                _parameters.Add(new Tuple<string, object, int>("@Id", ID, 0));
                _conn.PrepararProcedimiento("dbo.[USERS.Get_Id]", _parameters);

                DataTableReader DTRResultados = _conn.EjecutarTableReader();
                while (DTRResultados.Read())
                {
                    UsuarioResp = new UserModel()
                    {
                        Identificador = ID,
                        Name = DTRResultados["Name"].ToString(),
                        LastName = DTRResultados["LastName"].ToString(),
                        Nick = DTRResultados["Nick"].ToString(),
                        CreateDate = DateTime.Parse(DTRResultados["CreateDate"].ToString()),

                    };
                }

                return UsuarioResp;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _parameters.Clear();
            }
        }
        public long AddUser(Models.UserModel model)
        {
            try
            {
                long id = 0;
                _parameters.Add(new Tuple<string, object, int>("@p_user_json", JsonConvert.SerializeObject(model), 12));
                _conn.PrepararProcedimiento("dbo.[USERS.Set]", _parameters);
                DataTableReader DTRR = _conn.EjecutarTableReader();
                while (DTRR.Read())
                {
                    if (!string.IsNullOrEmpty(DTRR[0].ToString()))
                    {
                        id = long.Parse(DTRR[0].ToString());
                    }
                }

                return id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _parameters.Clear();
            }
        }
        public bool UpdateUser(Models.UserModel model)
        {
            try
            {
                bool reply = false;
                _parameters.Add(new Tuple<string, object, int>("@p_user_json", JsonConvert.SerializeObject(model), 12));
                _conn.PrepararProcedimiento("dbo.[USERS.Update]", _parameters);
                DataTableReader DTRR = _conn.EjecutarTableReader();
                while (DTRR.Read())
                {
                    if (!string.IsNullOrEmpty(DTRR[0].ToString()))
                    {
                        reply = long.Parse(DTRR[0].ToString()) > 0 ? true : false;
                    }
                }

                return reply;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _parameters.Clear();
            }
        }
        public void DeleteUser(int ID)
        {
            try
            {
                _parameters.Add(new Tuple<string, object, int>("@Id", ID, 0));
                _conn.PrepararProcedimiento("dbo.[USERS.Delete]", _parameters);
                int reply = _conn.EjecutarProcedimiento();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _parameters.Clear();
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
