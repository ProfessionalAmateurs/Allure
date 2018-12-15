using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using AllureRemodeling.Models;

namespace AllureRemodeling.Models
{
	public class DatabaseClass
	{

        // ------------------------------------------------------------------------------------------
        // Name: Get User
        // Abstract: get user info
        // ------------------------------------------------------------------------------------------
        public sbyte GetUser(ref DataSet ds, ref User user)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                if (GetDBConnection(ref cn) == 1) return -1;
                SqlDataAdapter da = new SqlDataAdapter("GetUser", cn);

                var encryptedPassword = SHA.GenerateSHA512String(user.Password);

                SetParameter(ref da, "@UserName", user.UserName, SqlDbType.VarChar);
                SetParameter(ref da, "@UserPassword", encryptedPassword, SqlDbType.VarChar);

                da.Fill(ds);

                CloseDBConnection(ref cn); 

                return 0;
            }

            catch (Exception ex) { throw new Exception(ex.Message); }

        }


        // ------------------------------------------------------------------------------------------
        // Name: CheckForExistingUser
        // Abstract: checks to see if user is already registered
        // ------------------------------------------------------------------------------------------
        public bool CheckForExistingUser (string userName)
        {
            SqlConnection cn = new SqlConnection();
            if (GetDBConnection(ref cn) == 1) throw new Exception("Could not establish connection");

            bool exists = false;

            string select = "SELECT * FROM TSystemUsers WHERE Username = '" + userName +"'";

            SqlCommand sql = new SqlCommand(select, cn);

            SqlDataReader reader = sql.ExecuteReader();

            if(reader.HasRows)
            {
                exists = true;
            }

            return exists;
        }


         //------------------------------------------------------------------------------------------
         //Name: InsertSystemUser
         //Abstract: Adds a customer to the TSystemUsers table
         //------------------------------------------------------------------------------------------
        public bool InsertSystemUser(SystemUsers user)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                if (GetDBConnection(ref cn) == 1) throw new Exception("Could not establish connection");

                bool success = false;

                user.RegistrationDate = DateTime.Now;
                user.LastLogin = DateTime.Now;
                var encryptedPassword = SHA.GenerateSHA512String(user.Password);

                string insert = @"DECLARE @SystemUserID AS INTEGER
                                  SELECT @SystemUserID = MAX( SystemUserID ) + 1 
                                  FROM TSystemUsers
                                  IF( @SystemUserID IS NULL )
                                  SELECT  @SystemUserID = 1
                                  INSERT INTO TSystemUsers(SystemUserID, Username, Password, RegistrationDate, LastLogin, NumberOfLogins)
                                  VALUES (@SystemUserID, '" + user.Username + "' , '" + encryptedPassword + "' , '" + user.RegistrationDate + "' , '" + user.LastLogin + "' , 1 )";

                SqlCommand sql = new SqlCommand(insert, cn);

                int rowsAffected = sql.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    success = true;
                }

                return success;

            }

            catch (Exception ex) { throw new Exception(ex.Message); }


        }

        // ------------------------------------------------------------------------------------------
        // Name: EmailCheck
        // Abstract: Checks to see if email provided is a associated with an account 
        // ------------------------------------------------------------------------------------------
        public bool EmailCheck(ref DataSet ds, string email)
        {
            try
            {
                bool result = false;
                //connect
                SqlConnection cn = new SqlConnection();
                if (GetDBConnection(ref cn) == 1) return false;
                SqlDataAdapter da = new SqlDataAdapter("CheckEmail", cn);

                SetParameter(ref da, "@Email", email, SqlDbType.VarChar);
                try
                {
                    // Fill the DataAdapter with the results
                    da.Fill(ds);
                }

                catch (Exception ex) { throw new Exception(ex.Message); }

                //Close the connection
                finally { CloseDBConnection(ref cn); }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    result = true;
                }

                return result;
            }
            catch (Exception exc) { throw new Exception(exc.Message); }
        }

        // ------------------------------------------------------------------------------------------
        // Name: UpdateResetPasswordCode
        // Abstract: add reset password code to database if user is resetting password
        // ------------------------------------------------------------------------------------------
        public sbyte UpdateResetPasswordCode(string emailID, string resetCode)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                if (GetDBConnection(ref cn) == 1) return -1;

                // Tax payer 
                SqlCommand sql = new SqlCommand("UpdateResetPasswordCode", cn);
                sql.CommandType = CommandType.StoredProcedure;

                sql.Parameters.AddWithValue("@Email", emailID);
                sql.Parameters.AddWithValue("@PasswordCode", resetCode);

                sql.ExecuteNonQuery();

                CloseDBConnection(ref cn);

                return 0;
            }

            catch (Exception exc) { throw new Exception(exc.Message); }

        }

        // ------------------------------------------------------------------------------------------
        // Name: CheckResetCode
        // Abstract: Check to see if the user has requested a reset password code
        // ------------------------------------------------------------------------------------------
        public sbyte CheckResetCode(ref DataSet ds, Guid resetCode)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                if (GetDBConnection(ref cn) == 1) return -1;
                SqlDataAdapter da = new SqlDataAdapter("CheckResetCode", cn);

                SetParameter(ref da, "@ResetCode", resetCode, SqlDbType.UniqueIdentifier);

                try
                {
                    da.Fill(ds);
                }

                finally { CloseDBConnection(ref cn); }

                return 0;
            }


            catch (Exception exc) { throw new Exception(exc.Message); }
        }


        // ------------------------------------------------------------------------------------------
        // Name: UpdateTaxPayerPassword
        // Abstract: Updates the password in the system
        // ------------------------------------------------------------------------------------------
        public bool UpdateUserPassword(string resetCode, string newPassword)
        {
            try
            {
                var result = false;
                SqlConnection cn = new SqlConnection();
                if (GetDBConnection(ref cn) == 1) throw new Exception("Failed to connect to database");

                string updateStatement = @"Update System_Users
                                           SET Password = '" + SHA.GenerateSHA512String(newPassword) + "'" +
                                            " ,ResetPasswordCode = NULL" +
                                          " WHERE ResetPasswordCode = '" + resetCode + "'";

                SqlCommand sql = new SqlCommand(updateStatement, cn);
                int rowsAffected = sql.ExecuteNonQuery();

                if (rowsAffected >= 0)
                {
                    result = true;
                }

                return result;
            }
            catch (Exception exc) { throw new Exception(exc.Message); }
        }

        // ------------------------------------------------------------------------------------------
        // Name: AddCustomerAccount
        // Abstract: Adds a customer to the TUsers table
        // ------------------------------------------------------------------------------------------
        public bool AddCustomerAccount(Users user)
        {
            SqlConnection cn = new SqlConnection();
            if (GetDBConnection(ref cn) == 1) throw new Exception("Could not establish connection");

            bool success = false; 

            string insertStatement = "INSERT INTO TUsers( FirstName, LastName, Address1, Address2, City, State, Zip, PhoneNumber, EmailAddress, SecurityGroupID, AccountTypeID, SystemUserID) ";
                    insertStatement += "Values('" + user.FirstName + "', '" + user.LastName + "', '" + user.Address1 + "', '" + user.Address2 + "', '" + user.City + "', '" + user.State + "', '" + user.Zip + "', '" + user.PhoneNumber + "', '" + user.EmailAddress + "', 1, 1, 1 )";

            SqlCommand sql = new SqlCommand(insertStatement, cn);

            int rowsAffected = sql.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                success = true;
            }

            return success;
        }

        //Sania and shareese Database methods
        //------------------------------------------------------------------------------------------
        // Name: GetEstimateQuestions
        // Abstract: get estimate questions
        // ------------------------------------------------------------------------------------------
        public List<Estimates> GetEstimateQuestions()
        {
            SqlConnection cn = new SqlConnection();
            if (GetDBConnection(ref cn) == 1) throw new Exception("Could not establish connection");

            var estimateQuestions = new List<Estimates>();

            string select = "SELECT QuestionID, Question FROM TQuestions";

            SqlCommand sql = new SqlCommand(select, cn);

            SqlDataReader reader = sql.ExecuteReader();

            while (reader.Read())
            {
                Estimates estimates = new Estimates();

                estimates.QuestionID = Convert.ToInt32(reader["QuestionID"]);
                estimates.Question = reader["Question"].ToString();

                estimateQuestions.Add(estimates);
            }

            CloseDBConnection(ref cn);

            return estimateQuestions;
        }

        //------------------------------------------------------------------------------------------
        // Name: InsertEstimateAnswers
        // Abstract: Insert answers for the estimate questions
        // ------------------------------------------------------------------------------------------
        public bool InsertAnswerData(Estimates estimateAnswers)
        {
            try
            {
                SqlConnection cn = new SqlConnection();
                if (GetDBConnection(ref cn) == 1) throw new Exception("Could not establish connection");

                bool success = false;
                string cmdString = "Insert into TAnswers (QuestionID, Answer) Values (@id, @answer)";

                SqlCommand sql = new SqlCommand(cmdString, cn);
                sql.Parameters.AddWithValue("@id", estimateAnswers.QuestionID);
                sql.Parameters.AddWithValue("@answer", estimateAnswers.Answer);

                int rowsAffected = sql.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    success = true;
                }

                CloseDBConnection(ref cn);
                return success;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        //------------------------------------------------------------------------------------------
        // Name: GetReviews
        // Abstract: get customer reviews
        // ------------------------------------------------------------------------------------------
        public List<Testimonials> GetReviews()
        {
            SqlConnection cn = new SqlConnection();
            if (GetDBConnection(ref cn) == 1) throw new Exception("Could not establish connection");

            var reviews = new List<Testimonials>();

            string select = "SELECT Testimonial, Date, LastName + ',' + FirstName AS Name FROM TTestimonials left join TUsers on TTestimonials.UserID = TUsers.UserID";

            SqlCommand sql = new SqlCommand(select, cn);

            SqlDataReader reader = sql.ExecuteReader();

            while (reader.Read())
            {
                Testimonials testimonials = new Testimonials();

                testimonials.Testimonial = reader["Testimonial"].ToString();
                testimonials.Name = reader["Name"].ToString();
                testimonials.Date = Convert.ToDateTime(reader["Date"]);

                reviews.Add(testimonials);
            }

            CloseDBConnection(ref cn);

            return reviews;
        }

        //------------------------------------------------------------------------------------------
        // Name: InsertReviewsUsingStoreProcedures
        // Abstract: Insert reviews and users in Usertable
        // ------------------------------------------------------------------------------------------
        public bool InsertReviewData(Testimonials reviews)
        {
            try
            {

                SqlConnection conn;
                SqlCommand cmd;
                conn = new
                SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Saniya\Allure.mdf; Integrated Security = True; Connect Timeout = 30"); // Put this string on one line in your code
                cmd = new SqlCommand("AddReview", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = reviews.FirstName;
                cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = reviews.LastName;
                cmd.Parameters.Add("@Review", SqlDbType.VarChar).Value = reviews.Testimonial;

                conn.Open();

                int i = cmd.ExecuteNonQuery();
                conn.Close();

                if (i >= 1)
                    return true;
                else
                    return false;

            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }


        // ------------------------------------------------------------------------------------------
        // Name: GetDBConnection
        // Abstract: Connect to the database.  Here is where connection strings should be changed
        //           for each customers database instance.
        // ------------------------------------------------------------------------------------------
        private int GetDBConnection(ref SqlConnection sqlConn)
        {
            try
            {
                if (sqlConn == null) sqlConn = new SqlConnection();
                if (sqlConn.State != System.Data.ConnectionState.Open)
                {
                    // ---------------------------
                    // Rick's Connection Strings
                    // ---------------------------

                    string ricksComputer =     "user id=capital;" +
                                               "password=capsoft;server=DESKTOP-AG5QMFS;" +
                                               "Trusted_Connection=yes;" +
                                               "database=Allure;" +
                                               "MultipleActiveResultSets=True;" +
                                               "connection timeout=30";

                    // ---------------------------
                    // Shareese's Connection Strings
                    // ---------------------------
                    

                    // ---------------------------
                    // Saniya's Connection Strings
                    // ---------------------------
         

                    // ---------------------------
                    // Francis's Connection String
                    // ---------------------------
                    
                    sqlConn.ConnectionString = ricksComputer;

                    sqlConn.Open();
                }

                return 0;
            }

            catch (Exception ex) { throw new Exception(ex.Message); }
        }



        // ------------------------------------------------------------------------------------------
        // Name: CloseDBConnection
        // Abstract: Closes the database connection
        // ------------------------------------------------------------------------------------------
        private SByte CloseDBConnection(ref SqlConnection sqlConn)
        {
            try
            {
                if (sqlConn.State != ConnectionState.Closed)
                {
                    sqlConn.Close();
                    sqlConn = null;
                }
                return 0;

            }

            catch (Exception ex) { throw new Exception(ex.Message); }

        }



        // ------------------------------------------------------------------------------------------
        // Name: SetParameter
        // Abstract: Sets stored procedure parameters
        // ------------------------------------------------------------------------------------------
        private int SetParameter(ref SqlCommand cm, string ParameterName, Object Value, System.Data.SqlDbType ParameterType, int FieldSize = -1, System.Data.ParameterDirection Direction = System.Data.ParameterDirection.Input, Byte Precision = 0, Byte Scale = 0)
        {
            try
            {
                cm.CommandType = System.Data.CommandType.StoredProcedure;
                if (FieldSize == -1)
                {
                    cm.Parameters.Add(ParameterName, ParameterType);
                }

                else
                {
                    cm.Parameters.Add(ParameterName, ParameterType, FieldSize);
                }

                if (Precision > 0) cm.Parameters[cm.Parameters.Count - 1].Precision = Precision;
                if (Scale > 0) cm.Parameters[cm.Parameters.Count - 1].Scale = Scale;

                cm.Parameters[cm.Parameters.Count - 1].Value = Value;
                cm.Parameters[cm.Parameters.Count - 1].Direction = Direction;

                return 0;
            }

            catch (Exception ex) { throw new Exception(ex.Message); }

        }


        // ------------------------------------------------------------------------------------------
        // Name: SetParameter
        // Abstract: Overload of SetParameter method above
        // ------------------------------------------------------------------------------------------
        private int SetParameter(ref SqlDataAdapter cm, string ParameterName, Object Value, System.Data.SqlDbType ParameterType, int FieldSize = -1, System.Data.ParameterDirection Direction = System.Data.ParameterDirection.Input, Byte Precision = 0, Byte Scale = 0)
        {
            try
            {
                cm.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                if (FieldSize == -1)
                    cm.SelectCommand.Parameters.Add(ParameterName, ParameterType);
                else
                    cm.SelectCommand.Parameters.Add(ParameterName, ParameterType, FieldSize);

                if (Precision > 0) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Precision = Precision;
                if (Scale > 0) cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Scale = Scale;

                cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Value = Value;
                cm.SelectCommand.Parameters[cm.SelectCommand.Parameters.Count - 1].Direction = Direction;

                return 0;
            }

            catch (Exception ex) { throw new Exception(ex.Message); }

        }

    }
}