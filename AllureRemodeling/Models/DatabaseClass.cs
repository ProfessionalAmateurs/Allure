using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AllureRemodeling.Models
{
    public class DatabaseClass
    {
        public bool AddMaterial(Materials material)
        {

            try
            {
                SqlConnection cn = new SqlConnection();
                if (GetDBConnection(ref cn) == 1) throw new Exception("Could not establish connection");

                bool success = false;
                string cmdString = "Insert into TMaterials (Description, Price) Values (@Description, @Price)";

                SqlCommand sql = new SqlCommand(cmdString, cn);
                sql.Parameters.AddWithValue("@Description", material.Description);
                sql.Parameters.AddWithValue("@Price", material.Price);

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
        

        public List<Materials> GetMaterialDetails()
        {
            SqlConnection conn;
            conn = new
            SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Shareese\Allure.mdf; Integrated Security = True; Connect Timeout = 30");
            List<Materials> Materiallist = new List<Materials>();

            SqlCommand cmd = new SqlCommand("GetMaterialDetails", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sd = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            conn.Open();
            sd.Fill(dt);
            conn.Close();

            foreach (DataRow dr in dt.Rows)
            {
                Materiallist.Add(
                    new Materials
                    {
                        MaterialID = Convert.ToInt32(dr["MaterialId"]),
                        Description = Convert.ToString(dr["Description"]),
                        Price = Convert.ToString(dr["Price"]),
                       
                    });
            }
            return Materiallist;
        }

        public bool UpdateMaterialDetails(Materials material)
        {
            try
            { //Not sure about this code but the MaterialId is coming in as 0
                SqlConnection cn = new SqlConnection();
                if (GetDBConnection(ref cn) == 1) throw new Exception("Could not establish connection");

                bool success = false;
                string cmdString = "UPDATE TMaterials SET(Description = @Description, Price = @Price WHERE MaterialId = @MaterialId)";

                SqlCommand sql = new SqlCommand(cmdString, cn);
                sql.Parameters.AddWithValue("@MaterialId", material.MaterialID);
                sql.Parameters.AddWithValue("@Description", material.Description);
                sql.Parameters.AddWithValue("@Price", material.Price);

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

            //SqlConnection conn;
            //conn = new
            //SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Shareese\Allure.mdf; Integrated Security = True; Connect Timeout = 30");

            //SqlCommand cmd = new SqlCommand("UpdateMaterialDetails", conn);
            //cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@MaterialId", material.MaterialID);
            //cmd.Parameters.AddWithValue("@Description", material.Description);
            //cmd.Parameters.AddWithValue("@Price", material.Price);

            //conn.Open();
            //int i = cmd.ExecuteNonQuery();
            //conn.Close();

            //if (i >= 1)
            //    return true;
            //else
            //    return false;
        }

        public bool DeleteMaterial(int MaterialId)
        {
            SqlConnection conn;
            conn = new
            SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Shareese\Allure.mdf; Integrated Security = True; Connect Timeout = 30");

            SqlCommand cmd = new SqlCommand("DeleteMaterial", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MaterialId", MaterialId);

            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();

            if (i >= 1)
                return true;
            else
                return false;
        }
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
                sql.Parameters.AddWithValue("@id",estimateAnswers.QuestionID);
                sql.Parameters.AddWithValue("@answer", estimateAnswers.Answer);

                int rowsAffected = sql.ExecuteNonQuery();

                if(rowsAffected > 0)
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

                    string ricksComputer = "user id=capital;" +
                                               "password=capsoft;server=DESKTOP-AG5QMFS;" +
                                               "Trusted_Connection=yes;" +
                                               "database=Allure;" +
                                               "MultipleActiveResultSets=True;" +
                                               "connection timeout=30";

                    // ---------------------------
                    // Shareese's Connection Strings
                    // ---------------------------
                    string shareeseComputer = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Shareese\Allure.mdf; Integrated Security = True; Connect Timeout = 30";

                    // ---------------------------
                    // Saniya's Connection Strings
                    // ---------------------------
                    string saniaComputer = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Saniya\Allure.mdf; Integrated Security = True; Connect Timeout = 30";

                    // Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Saniya\Allure.mdf; Integrated Security = True; Connect Timeout = 30
                    // ---------------------------
                    // Francis's Connection String
                    // ---------------------------


                    sqlConn.ConnectionString = shareeseComputer;

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


//        // ------------------------------------------------------------------------------------------
//        // Name: Insert Answers TAnswer table 
//        // Abstract: 
//        // ------------------------------------------------------------------------------------------
//        public void InsertAnswerData(int QuestionId, string Answers)
//        {
//            try
//            {
//                SqlConnection conn;
//                SqlCommand cmd;
//               // string cmdString = @"Insert TAnswers (QuestionID, Answer) Values (" + QuestionID +", '"+ Answer +"')";
//               string cmdString = "Insert into TAnswers (QuestionID, Answer) Values (@id, @answer)";
//                conn = new
//                //SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\NORTHWND.MDF;Integrated Security=True;User Instance=True"); // Put this string on one line in your code
//                SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Saniya\Allure.mdf; Integrated Security = True; Connect Timeout = 30"); // Put this string on one line in your code
//                cmd = new SqlCommand(cmdString, conn);
//                cmd.Parameters.AddWithValue("@id", QuestionId);
//                cmd.Parameters.AddWithValue("@answer", Answers);
       
//                conn.Open();

//                cmd.ExecuteNonQuery();
//                conn.Close();
//            }

//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);

//            }
//        }

//        // ------------------------------------------------------------------------------------------
//        // Name: Insert User information in TUser 
//        // Abstract: 
//        // ------------------------------------------------------------------------------------------
//        public void InsertUserData()
//        {
//            try
//            {
//                SqlConnection conn;
//                SqlCommand cmd;
//                string cmdString = @"Insert TUsers (UserID, FirstName, LastName, Address1, Address2, City, State, Zip, PhoneNumber, EmailAddress, SecurityGroupID, AccountTypeID, SystemUserID) Values ('BILLE', 'XYZ Company', 'Bill Evjen')"; 
//                conn = new
//                //SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\NORTHWND.MDF;Integrated Security=True;User Instance=True"); // Put this string on one line in your code
//                SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Saniya\Allure.mdf; Integrated Security = True; Connect Timeout = 30"); // Put this string on one line in your code
//                cmd = new SqlCommand(cmdString, conn);
//                conn.Open();

//                cmd.ExecuteNonQuery();
//                conn.Close();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);

//            }
//        }
       }
}




