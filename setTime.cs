using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Time_Machine
{
    class cSetTime
    {
        public int ID { get; set; }
        public int CTRLAREAID { get; set; }
        public int CARDID { get; set; }
        public int PERSONID { get; set; }
        public DateTime ADATETIME { get; set; }
        public bool IS_ENTRANCE { get; set; }
        public string COMMENT { get; set; }

        public cSetTime()
        {
            ID = -1;
        }

        public cSetTime(int id)
        {
            ID = -1;

            db db = new db();
            DbDataReader dbRdr;

            try
            {
                if (!db.dbOpen()) throw new Exception();
                dbRdr = db.getReader("SELECT * FROM ATTENDANCE WHERE ID = " + id);
                dbRdr.Read();

                setTime(dbRdr);

                dbRdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения из базы данных: " + ex.Message, "Ошибка базы данных");
                id = -1;
            }
            finally { db.dbClose(); }
        }

        public void setTime(DbDataReader dbRdr)
        {
            ID = Convert.ToInt32(dbRdr["ID"]);
            CTRLAREAID = Convert.ToInt32(dbRdr["CTRLAREAID"]);
            CARDID = Convert.ToInt32(dbRdr["CARDID"]);
            PERSONID = Convert.ToInt32(dbRdr["PERSONID"]);
            ADATETIME = Convert.ToDateTime(dbRdr["ADATETIME"]);
            IS_ENTRANCE = Convert.ToBoolean(dbRdr["IS_ENTRANCE"]);
            COMMENT = Convert.ToString(dbRdr["COMMENT"]);
        }

        public string getQuery(string type)
        {
            string formatQuery = "";

            switch (type)
            {
                case "i":
                    formatQuery = "INSERT INTO ATTENDANCE (CTRLAREAID, CARDID, PERSONID, ADATETIME, IS_ENTRANCE, COMMENT) "
                        + "VALUES (@CTRLAREAID, @CARDID, @PERSONID, @ADATETIME, @IS_ENTRANCE, @COMMENT)";
                    break;
                case "u":
                    formatQuery = "UPDATE ATTENDANCE SET CTRLAREAID = @CTRLAREAID, CARDID = @CARDID, " +
                                "PERSONID = @PERSONID, ADATETIME = @ADATETIME, " +
                                "IS_ENTRANCE = @IS_ENTRANCE, COMMENT = @COMMENT " +
                                "WHERE ID = {0}";
                    break;
            }
            return string.Format(formatQuery, ID);
        }

        public bool save()
        {
            db db = new db();
            if (!db.dbOpen()) return false;

            string saveQuery = "";

            if (ID < 0) saveQuery = getQuery("i");
            else { saveQuery = getQuery("u");}

            if (!db.executeParamQuery(saveQuery,
                    new DictionaryEntry("CTRLAREAID", CTRLAREAID),
                    new DictionaryEntry("CARDID", CARDID),
                    new DictionaryEntry("PERSONID", PERSONID),
                    new DictionaryEntry("ADATETIME", ADATETIME),
                    new DictionaryEntry("IS_ENTRANCE", IS_ENTRANCE),
                    new DictionaryEntry("COMMENT", COMMENT)
                )) { db.dbClose(); return false; }
            if (ID < 0)
            {
               // ID = Convert.ToInt32(db.executeScalar("SELECT LAST_INSERT_ID()"));
            }
            db.dbClose();

            return true;
        }

        static public DataSet getPerson()
        {
            string query = "SELECT a.PERSONID, (a.NAME ||' '|| a.FIRSTNAME ||' '|| a.SECONDNAME) FIO FROM PERSON a";
            db db = new db();
            DataSet ds = new DataSet();
            DataTable table = new DataTable();

            db.dbOpen();
            table.Load(db.getReader(query));
            ds.Tables.Add(table);
            db.dbClose();
            return ds;

        }
/*
        public bool delete()
        {
            db db = new db(1);
            if (!db.dbOpen()) return false;
            cLog.log(10, new string[] { id_contact.ToString() }, new string[] { fio });
            if (!db.executeQuery("DELETE FROM contacts WHERE id_contact = " + id_contact)) { db.dbClose(); return false; }
            db.dbClose();
            return true;
        }
*/
    }

  /*  class cViewSetTime
    {
        public int ID { get; set; }

        //public DateTime ADATETIME { get; set; }
        public bool IS_ENTRANCE { get; set; }
        public string FIO { get; set; }
        public int CARDID { get; set; }

        public cViewSetTime()
        {
            ID = -1;
        }

        public void setViewSetTime(DbDataReader dbRdr)
        {
            ID = Convert.ToInt32(dbRdr["ID"]);
           // ADATETIME = Convert.ToString(dbRdr["ADATETIME"]);
            IS_ENTRANCE = Convert.ToBoolean(dbRdr["IS_ENTRANCE"]);
            FIO = Convert.ToString(dbRdr["FIO"]);
            CARDID = Convert.ToInt32(dbRdr["CARDID"]);
        }

    }*/

    class cSetTimeList
    {
        public ArrayList SetTimeArr;

        public cSetTimeList()
        {
            SetTimeArr = new ArrayList();
        }

        /*public void getTime(int cardid)
        {
            db db;
            DbDataReader dbRdr;

        
            cViewSetTime STL;
            SetTimeArr.Clear();
            string query = "SELECT a.id, a.CTRLAREAID, a.CARDID, a.PERSONID, a.ADATETIME, a.IS_ENTRANCE, a.COMMENT, (p.name || ' ' || p.FIRSTNAME || ' ' || p.SECONDNAME) as FIO "+
                            "FROM ATTENDANCE a "+
                            "INNER JOIN PERSON p on a.CARDID = p.PERSONID "+
                            "WHERE CARDID = "+cardid +
                            "order by a.id";
            db = new db();
            try
            {
                if (!db.dbOpen()) throw new Exception("Ошибка подключения к БД.");
                dbRdr = db.getReader(query);
                while (dbRdr.Read())
                {
                    STL = new cViewSetTime();
                    STL.setViewSetTime(dbRdr);
                    SetTimeArr.Add(STL);
                }
                dbRdr.Close();
                db.dbClose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения из базы данных: " + ex.Message, "Ошибка базы данных");
            }
            finally { db.dbClose(); }
        }
        */

        public DataSet getTimeE(int cardid, DateTime? startDate, DateTime? endDate)
        {

            string query = "SELECT a.id, a.CTRLAREAID, a.CARDID, a.PERSONID, a.ADATETIME, a.IS_ENTRANCE, a.COMMENT, (p.name || ' ' || p.FIRSTNAME || ' ' || p.SECONDNAME) as FIO " +
                            "FROM ATTENDANCE a " +
                            "INNER JOIN PERSON p on a.CARDID = p.PERSONID " +
                            "WHERE CARDID = " + cardid;
            if (startDate != null && endDate != null)
            {
                query += " AND (ADATETIME BETWEEN " + String.Format("'{0}' ", startDate.Value) + "AND " + String.Format("'{0}' ) ", endDate.Value.AddHours(23.59));
            }
            query += "ORDER BY a.ADATETIME DESC";

            db db = new db();
            DataSet ds = new DataSet();
            DataTable table = new DataTable();
           
            db.dbOpen();
            table.Load(db.getReader(query));
            ds.Tables.Add(table);
            db.dbClose();
            return ds;
          
        }

    }

}
