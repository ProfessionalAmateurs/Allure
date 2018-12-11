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
        // ------------------------------------------------------------------------------------------
        // Name: CheckForExistingUser
        // Abstract: checks to see if user is already registered
        // ------------------------------------------------------------------------------------------
        public bool CheckForExistingUser (string userName)
        {
            SqlConnection cn = new SqlConnection();
            if (GetDBConnection(ref cn) == 1) throw new Exception("Could not establish connection");

            bool exists = false;

            string select = "SELECT * FROM TSystemUsers";

            SqlCommand sql = new SqlCommand(select, cn);

            SqlDataReader reader = sql.ExecuteReader();

            if(reader.HasRows)
            {
                exists = true;
            }

            return exists;
        }


        // ------------------------------------------------------------------------------------------
        // Name: InsertSystemUser
        // Abstract: Adds a customer to the TSystemUsers table
        // ------------------------------------------------------------------------------------------
        public bool InsertSystemUser(SystemUsers user)
        {
            SqlConnection cn = new SqlConnection();
            if (GetDBConnection(ref cn) == 1) throw new Exception("Could not establish connection");

            bool success = false;

            string insert = "INSERT INTO TSystemUser(";
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