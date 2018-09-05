using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Asp.netcore2.Models
{
    public class UserDataAccessLayer
    {
        public static IConfiguration Configuration { get; set; }

        string connectionstring = GetConnectionstring();

        //To Read ConnectionString from appsettings.json file  
        private static string GetConnectionstring()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            string connectionstring = Configuration["Connectionstring:connectionstring"];
            return connectionstring;
        }

        //To Register a new user 
        public string RegisterUser(UserDetails user)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand("spRegisterUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@UserID", user.UserID);
                cmd.Parameters.AddWithValue("@UserPassword", user.Password);

                con.Open();
                string result = cmd.ExecuteScalar().ToString();
                con.Close();
                return result;
            }
        }

        public string validatelogin(UserDetails user)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand("spValidateUserLogin", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@LoginID", user.UserID);
                cmd.Parameters.AddWithValue("@LoginPassword", user.Password);

                con.Open();
                string result = cmd.ExecuteScalar().ToString();
                con.Close();

                return result;
            }
        }


    }
}
