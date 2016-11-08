using System;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using System.Data.Common;
using System.Collections;
using System.Windows;

namespace Time_Machine
{
    class db
    {
        //  string for release
        //string connectionString = @"User=SYSDBA;Password=masterkey;Database=C:\ElsysPass\Data\BPROT.gdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime = 15; Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size = 8192; ServerType=0;";
        string connectionString = @"User=SYSDBA;Password=masterkey;Database=C:\Users\User\Desktop\firebird\BPROT.gdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime = 15; Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size = 8192; ServerType=0;";
        private DbConnection dbConn = null;
        private DbCommand dbCmd = null;
        private DbTransaction dbTr = null;

        public db()
        {
           dbConn = new FbConnection(connectionString);
           dbCmd = new FbCommand();
        }

        public bool dbOpen()
        {
            if (dbConn == null) return false;
            try
            {
                dbConn.Open();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void dbClose()
        {
            try { dbConn.Close(); }
            catch { }
        }

        public DbDataReader getReader(string query)
        {
            if (dbConn != null)
            {
                dbCmd.Connection = dbConn;
                dbCmd.CommandText = query;
                dbCmd.CommandType = System.Data.CommandType.Text;
                try
                {
                    return dbCmd.ExecuteReader();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            return null;
        }

        public DbDataReader getParamReader(string query, params DictionaryEntry[] args)
        {
            dbCmd.Connection = dbConn;
            dbCmd.CommandText = query;
            dbCmd.CommandType = System.Data.CommandType.Text;

            foreach (DictionaryEntry de in args)
            { 
                dbCmd.Parameters.Add(new FbParameter(de.Key.ToString(), de.Value));
            }
            try
            {
                return dbCmd.ExecuteReader();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message);}
            return null;
        }

        public DbDataAdapter getAdapter(string query)
        {
          return new FbDataAdapter(query, (FbConnection)dbConn);
        }

        public bool executeQuery(string query)
        {
            dbCmd.Connection = dbConn;
            dbCmd.CommandText = query;
            dbCmd.CommandType = System.Data.CommandType.Text;

            try
            {
                dbCmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return false;
        }

        public bool executeParamQuery(string query, params DictionaryEntry[] args)
        {
            dbCmd.Connection = dbConn;
            dbCmd.CommandText = query;
            dbCmd.CommandType = System.Data.CommandType.Text;
            dbCmd.Parameters.Clear();

            foreach (DictionaryEntry de in args)
            {
                dbCmd.Parameters.Add(new FbParameter(de.Key.ToString(), de.Value));
            }
            try
            {
                dbCmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return false;
        }

        public object executeScalar(string query)
        {
            dbCmd.Connection = dbConn;
            dbCmd.CommandText = query;
            dbCmd.CommandType = System.Data.CommandType.Text;
            try
            {
                return dbCmd.ExecuteScalar();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            return null;
        }

        public bool executeProcedure(string nameProc, params DictionaryEntry[] args)
        {
            bool result = true;

            dbCmd.Connection = dbConn;
            dbCmd.CommandText = nameProc;
            dbCmd.CommandType = System.Data.CommandType.StoredProcedure;

            foreach (DictionaryEntry de in args)
            {
                dbCmd.Parameters.Add(new FbParameter(de.Key.ToString(), de.Value));
            }
            try
            {
                dbCmd.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); result = false; }
            return result;
        }

        public DbDataReader getReaderFromProcedure(string nameProc, params DictionaryEntry[] args)
        {
            DbDataReader dbr = null;

            dbCmd.Connection = dbConn;
            dbCmd.CommandText = nameProc;
            dbCmd.CommandType = System.Data.CommandType.StoredProcedure;

            foreach (DictionaryEntry de in args)
            {
                dbCmd.Parameters.Add(new FbParameter(de.Key.ToString(), de.Value));
            }
            try
            {
                dbr = dbCmd.ExecuteReader();
            }
            catch (Exception ex) { if (ex.Message.Contains("No data")) return null; MessageBox.Show(ex.Message); }
            return dbr;
        }

        public bool executeProcedure(string nameProc, Dictionary<string, object> args)
        {
            bool result = true;

            dbCmd.Connection = dbConn;
            dbCmd.CommandText = nameProc;
            dbCmd.CommandType = System.Data.CommandType.StoredProcedure;

            foreach (KeyValuePair<string, object> de in args)
            {
                        dbCmd.Parameters.Add(new FbParameter(de.Key, de.Value));
            }
            try
            {
                dbCmd.ExecuteNonQuery();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); result = false; }
            return result;
        }

        public DbDataReader getReaderFromProcedure(string nameProc, Dictionary<string, object> args)
        {
            DbDataReader dbr = null;

            dbCmd.Connection = dbConn;
            dbCmd.CommandText = nameProc;
            dbCmd.CommandType = System.Data.CommandType.StoredProcedure;

            foreach (KeyValuePair<string, object> de in args)
            {
                        dbCmd.Parameters.Add(new FbParameter(de.Key, de.Value));
            }
            try
            {
                dbr = dbCmd.ExecuteReader();
            }
            catch (Exception ex) { /*if (ex.Message.Contains("No data")) return null;*/ MessageBox.Show(ex.Message); }
            return dbr;
        }

        public void BeginTransaction()
        {
            dbTr = dbConn.BeginTransaction();
        }

        public void EndTransaction()
        {
            dbTr.Commit();
        }

    }
}
