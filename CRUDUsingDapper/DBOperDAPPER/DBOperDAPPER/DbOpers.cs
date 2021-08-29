using System;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace DBOperDAPPER
{
    public class DbOpers
    {
        private string _ConnString { get; set; }
        SqlConnection SqlConn;

        public DbOpers(string ConnSting)
        {
            _ConnString = ConnSting;
        }

        public List<User> GetAllUsers()
        {
            List<User> UserList = new List<User>();

            using (SqlConn = new SqlConnection(_ConnString))
            {
                SqlConn.Open();

               var results = SqlConn.Query("[dbo].[SP_GetAllUsers]", commandType: CommandType.StoredProcedure).AsList();

                results.ForEach(result =>
                {
                    User user = new User()
                    {
                        UserId = Convert.ToInt32(result.UserID),
                        UserName = result.UserName,
                        Address = result.Address
                    };

                    UserList.Add(user);
                });
            }

            SqlConn.Close();
            return UserList;
        }

        public User GetUser(int UserId)
        {
            User user = new User();

            using (SqlConn = new SqlConnection(_ConnString))
            {
                SqlConn.Open();

                var param = new { UserID = UserId };
                var result = SqlConn.QueryFirstOrDefault("[dbo].[SP_GetUser]", param, commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    user.UserId = Convert.ToInt32(result.UserID);
                    user.UserName = result.UserName;
                    user.Address = result.Address;
                }
                
            }

            SqlConn.Close();

            return user;
        }

        public int EditUser(User user, string QueryType = "")
        {
            int count = 0;

            using (SqlConn = new SqlConnection(_ConnString))
            {
                SqlConn.Open();
                
                var param = new { UserID = user.UserId, UserName = user.UserName, Address = user.Address };
                var result = 0 ;

                if (QueryType.ToUpper() == "CREATE")
                    result = SqlConn.Execute("[dbo].[SP_CreateUser]", param, commandType: CommandType.StoredProcedure);
                else
                    result = SqlConn.Execute("[dbo].[SP_EditUser]", param, commandType: CommandType.StoredProcedure);
                
                count = result;

            }

            SqlConn.Close();

            return count;
        }

        public int DeleteUser(int UserId)
        {
            int count = 0;

            using (SqlConn = new SqlConnection(_ConnString))
            {
                SqlConn.Open();

                var param = new { UserID = UserId };

                var result = SqlConn.Execute("[dbo].[SP_DeleteUser]", param, commandType: CommandType.StoredProcedure);

                count = result;
            }

            SqlConn.Close();
            return count;
        }
    }
}
