using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using NHibernate;
using MySql.Data.MySqlClient;
using PMSS.SqlDataOutput;

namespace PMSS.SqlDataIn
{
    public class DisasterDataToDatabase
    {
        private Configuration myConfigration;
        private ISessionFactory mySessionFactory;
        private ISession mySession;

        public DisasterDataToDatabase()
        {
            myConfigration = new Configuration();
        }

        public void AddOneRecord(DateTime time, string type, int inWaringArea, string area, string comment, string situation, string process)
        {
            myConfigration.Configure();
            try
            {
                mySessionFactory = myConfigration.BuildSessionFactory();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            mySession = mySessionFactory.OpenSession();

            using (mySession.BeginTransaction())
            {
                Geologicaldisaster disaster = new Geologicaldisaster();
                disaster.Time = time;
                disaster.Type = type;
                disaster.Inwarningarea = inWaringArea;
                disaster.Area = area;
                disaster.Comment = comment;
                disaster.Disastersituation = situation;
                disaster.Process = process;

                try
                {
                    mySession.Save(disaster);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                mySession.Transaction.Commit();
            }
        }
    }
}
