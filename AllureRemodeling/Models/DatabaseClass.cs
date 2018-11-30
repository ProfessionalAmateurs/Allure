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
        //------------------------------------------------------------------------------------------
        // Name: GetEstimateQuestions
        // Abstract: get estimate questions
        // ------------------------------------------------------------------------------------------
        //public List<Estimates> GetEstimateQuestions()
        //{
        //    SqlConnection cn = new SqlConnection();
        //    if (GetDBConnection(ref cn) == 1) throw new Exception("Could not establish connection");

        //    List<Estimates> questions = new List<Estimates>();

        //    string selectStatement = "Select QuestionID, Question from TQuestions";

        //    SqlCommand sql = new SqlCommand(selectStatement, cn);

        //    SqlDataReader reader = sql.ExecuteReader();

        //    while (reader.Read())
        //    {
        //        Estimates estimateQuestions = new Estimates();

        //        estimateQuestions.QuestionID = reader.GetValue<int>("TRAN_CODE");
        //        trans.Payment_Source = reader.GetValue<string>("Payment_Source");
        //        trans.LineItemPaymentAmount = reader.GetValue<decimal>("LineItemPaymentAmount");
        //        trans.CardType = reader.GetValue<string>("CardType");
        //    }



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

                    //string ricksComputer = "user id=capital;" +
                    //                           "password=capsoft;server=DESKTOP-AG5QMFS;" +
                    //                           "Trusted_Connection=yes;" +
                    //                           "database=Allure;" +
                    //                           "MultipleActiveResultSets=True;" +
                    //                           "connection timeout=30";

                    // ---------------------------
                    // Shareese's Connection Strings
                    // ---------------------------


                    // ---------------------------
                    // Saniya's Connection Strings
                    // ---------------------------
                    string saniaComputer = "Server=(LocalDB)\\v11.0;" +
                                              "Trusted_Connection=yes;" +
                                              "database=Allure;" +
                                              "MultipleActiveResultSets=True;" +
                                              "connection timeout=30";

                    // Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Saniya\Allure.mdf; Integrated Security = True; Connect Timeout = 30
                    // ---------------------------
                    // Francis's Connection String
                    // ---------------------------


                    sqlConn.ConnectionString = saniaComputer;

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

        // ------------------------------------------------------------------------------------------
        // Name: GetQuestions
        // Abstract: 
        // ------------------------------------------------------------------------------------------
        public IEnumerable<Estimates> GetQuestions()
        {
            List<Estimates> lstquestion= new List<Estimates>();
            try
            {
                SqlConnection conn;
                SqlCommand cmd;
                string cmdString = "Select QuestionID, Question from TQuestions";
                conn = new
                //SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\NORTHWND.MDF;Integrated Security=True;User Instance=True"); // Put this string on one line in your code
                SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Saniya\Allure.mdf; Integrated Security = True; Connect Timeout = 30"); // Put this string on one line in your code
                cmd = new SqlCommand(cmdString, conn);
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Estimates estimate = new Estimates();
                    estimate.QuestionID = Convert.ToInt32(rdr["QuestionID"]);
                    estimate.Question = rdr["Question"].ToString();
                    lstquestion.Add(estimate);
                }
                conn.Close();
                return lstquestion; 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        // ------------------------------------------------------------------------------------------
        // Name: Insert Answers TAnswer table 
        // Abstract: 
        // ------------------------------------------------------------------------------------------
        public void InsertAnswerData(int QuestionId, string Answers)
        {
            try
            {
                SqlConnection conn;
                SqlCommand cmd;
               // string cmdString = @"Insert TAnswers (QuestionID, Answer) Values (" + QuestionID +", '"+ Answer +"')";
               string cmdString = "Insert into TAnswers (QuestionID, Answer) Values (@id, @answer)";
                conn = new
                //SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\NORTHWND.MDF;Integrated Security=True;User Instance=True"); // Put this string on one line in your code
                SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Saniya\Allure.mdf; Integrated Security = True; Connect Timeout = 30"); // Put this string on one line in your code
                cmd = new SqlCommand(cmdString, conn);
                cmd.Parameters.AddWithValue("@id", QuestionId);
                cmd.Parameters.AddWithValue("@answer", Answers);
       
                conn.Open();

                cmd.ExecuteNonQuery();
                conn.Close();
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        // ------------------------------------------------------------------------------------------
        // Name: Insert User information in TUser 
        // Abstract: 
        // ------------------------------------------------------------------------------------------
        public void InsertUserData()
        {
            try
            {
                SqlConnection conn;
                SqlCommand cmd;
                string cmdString = @"Insert TUsers (UserID, FirstName, LastName, Address1, Address2, City, State, Zip, PhoneNumber, EmailAddress, SecurityGroupID, AccountTypeID, SystemUserID) Values ('BILLE', 'XYZ Company', 'Bill Evjen')"; 
                conn = new
                //SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\NORTHWND.MDF;Integrated Security=True;User Instance=True"); // Put this string on one line in your code
                SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Saniya\Allure.mdf; Integrated Security = True; Connect Timeout = 30"); // Put this string on one line in your code
                cmd = new SqlCommand(cmdString, conn);
                conn.Open();

                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
    }
}




