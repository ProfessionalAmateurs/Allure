using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AllureRemodeling.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }

        public string AccountID { get; set; }

        public int AccountType { get; set; }

        public int RoleID { get; set; }

        public string RoleName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        /// &lt;summary>
        /// Checks if user with given password exists in the database
        /// &lt;/summary>
        /// &lt;param name="_username">User name&lt;/param>
        /// &lt;param name="_password">User password&lt;/param>
        /// &lt;returns>True if user exist and password is correct&lt;/returns>
        public bool IsValid(ref User user)
        {
            using (var cn = new SqlConnection("user id = capital; " +
                                       "password=capsoft;server=DESKTOP-AG5QMFS;" +
                                       "Trusted_Connection=yes;" +
                                       "database=Allure; " +
                                       "connection timeout=30")
             )
            {
                string _sql = @"SELECT [SystemUserID], [Username] FROM [dbo].[SystemUsers] " +
                       @"WHERE [Username] = @u AND [Password] = @p";
                var cmd = new SqlCommand(_sql, cn);
                cmd.Parameters
                    .Add(new SqlParameter("@u", SqlDbType.NVarChar))
                    .Value = user.UserName;
                cmd.Parameters
                    .Add(new SqlParameter("@p", SqlDbType.NVarChar))
                    .Value = SHA.GenerateSHA512String(user.Password);
                cn.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return true;
                }
                else
                {
                    reader.Dispose();
                    cmd.Dispose();
                    return false;
                }
            }
        }
    }
}

