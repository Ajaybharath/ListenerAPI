using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ListenerAPI.Models
{
    public class SqlHelper
    {
        private string mstr_ConnectionString;
        private SqlConnection mobj_SqlConnection;
        private SqlCommand mobj_SqlCommand;
        private int mint_CommandTimeout = 30;

        public enum ExpectedType
        {
            StringType = 0,
            NumberType = 1,
            DateType = 2,
            BooleanType = 3,
            ImageType = 4
        }

        public void InitializeDataConnecion()
        {
            try
            {
                //if (!string.IsNullOrEmpty(applicationName))
                //{
                mstr_ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                //}

                mobj_SqlConnection = new SqlConnection(mstr_ConnectionString);
                mobj_SqlCommand = new SqlCommand();
                mobj_SqlCommand.CommandTimeout = mint_CommandTimeout;
                mobj_SqlCommand.Connection = mobj_SqlConnection;

                //ParseConnectionString();
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing data class." + Environment.NewLine + ex.Message);
            }
        }

        public void InitializeDataConnecion(string dbName)
        {
            try
            {
                //if (!string.IsNullOrEmpty(applicationName))
                //{
                mstr_ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
                //}

                string[] arrConStr = mstr_ConnectionString.Split(';');

                string extDB = string.Empty;

                for (int i = 0; i < arrConStr.Length; i++)
                {
                    if (arrConStr[i].ToLower().Contains("database"))
                    {
                        arrConStr = arrConStr[i].Split('=');
                        extDB = arrConStr[1];
                    }
                }

                mstr_ConnectionString = mstr_ConnectionString.Replace(extDB, dbName);

                mobj_SqlConnection = new SqlConnection(mstr_ConnectionString);
                mobj_SqlCommand = new SqlCommand();
                mobj_SqlCommand.CommandTimeout = mint_CommandTimeout;
                mobj_SqlCommand.Connection = mobj_SqlConnection;

                //ParseConnectionString();
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing data class." + Environment.NewLine + ex.Message);
            }
        }


        public void Dispose()
        {
            try
            {
                //Clean Up Connection Object
                if (mobj_SqlConnection != null)
                {
                    if (mobj_SqlConnection.State != ConnectionState.Closed)
                    {
                        mobj_SqlConnection.Close();
                    }
                    mobj_SqlConnection.Dispose();
                }

                //Clean Up Command Object
                if (mobj_SqlCommand != null)
                {
                    mobj_SqlCommand.Dispose();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error disposing data class." + Environment.NewLine + ex.Message);
            }

        }

        public void CloseConnection()
        {
            if (mobj_SqlConnection.State != ConnectionState.Closed) mobj_SqlConnection.Close();
        }

        public string GetExecuteScalarByCommand(string Command)
        {

            object identity = 0;
            try
            {
                mobj_SqlCommand.CommandText = Command;
                mobj_SqlCommand.CommandTimeout = mint_CommandTimeout;
                mobj_SqlCommand.CommandType = CommandType.StoredProcedure;

                mobj_SqlCommand.Connection = mobj_SqlConnection;

                mobj_SqlConnection.Open();

                identity = mobj_SqlCommand.ExecuteScalar();
                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
            return Convert.ToString(identity);
        }

        public void GetExecuteNonQueryByCommand(string Command)
        {
            try
            {
                mobj_SqlCommand.CommandText = Command;
                mobj_SqlCommand.CommandTimeout = mint_CommandTimeout;
                mobj_SqlCommand.CommandType = CommandType.StoredProcedure;

                mobj_SqlCommand.Connection = mobj_SqlConnection;
                mobj_SqlConnection.Open();

                mobj_SqlCommand.ExecuteNonQuery();

                CloseConnection();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }

        public DataSet GetDatasetByCommand(string Command)
        {
            try
            {
                mobj_SqlCommand.CommandText = Command;
                mobj_SqlCommand.CommandTimeout = mint_CommandTimeout;
                mobj_SqlCommand.CommandType = CommandType.StoredProcedure;

                mobj_SqlConnection.Open();

                SqlDataAdapter adpt = new SqlDataAdapter(mobj_SqlCommand);
                DataSet ds = new DataSet();
                adpt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }

        public SqlDataReader GetReaderBySQL(string strSQL)
        {
            mobj_SqlConnection.Open();
            try
            {
                SqlCommand myCommand = new SqlCommand(strSQL, mobj_SqlConnection);
                return myCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }
        }

        public SqlDataReader GetReaderByCmd(string Command)
        {
            SqlDataReader objSqlDataReader = null;
            try
            {
                mobj_SqlCommand.CommandText = Command;
                mobj_SqlCommand.CommandType = CommandType.StoredProcedure;
                mobj_SqlCommand.CommandTimeout = mint_CommandTimeout;

                mobj_SqlConnection.Open();
                mobj_SqlCommand.Connection = mobj_SqlConnection;

                objSqlDataReader = mobj_SqlCommand.ExecuteReader();
                return objSqlDataReader;
            }
            catch (Exception ex)
            {
                CloseConnection();
                throw ex;
            }

        }

        public void AddParameterToSQLCommand(string ParameterName, SqlDbType ParameterType)
        {
            try
            {
                mobj_SqlCommand.Parameters.Add(new SqlParameter(ParameterName, ParameterType));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddParameterToSQLCommand(string ParameterName, SqlDbType ParameterType, int ParameterSize)
        {
            try
            {
                mobj_SqlCommand.Parameters.Add(new SqlParameter(ParameterName, ParameterType, ParameterSize));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetSQLCommandParameterValue(string ParameterName, object Value)
        {
            try
            {
                mobj_SqlCommand.Parameters[ParameterName].Value = Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}