using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NHibernate.Cfg;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using MySql.Data.MySqlClient;

namespace PMSS.SqlDataOutput
{
    public class HydrologicReader
    {
        public HydrologicEntity RetrieveEntity(DateTime fromDate, DateTime toDate)
        {
            var entity = new HydrologicEntity();
            entity.Items = new List<HydrologicEntityItem>();

            Configuration myConfigration;
            ISessionFactory mySessionFactory;
            ISession mySession;
            myConfigration = new Configuration();
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

            var datas = mySession.Query<Hydrologicaldata>();
            var stations = mySession.Query<Hydrologicalstation>();
            var query = from e in
                            (from d in datas
                             join s in stations
                             on d.Stationid equals s.Uid
                             where (d.Time < toDate) && (d.Time > fromDate)
                             select new
                                 {
                                     StationId = d.Stationid,
                                     RecordId = d.Recordid,
                                     Time = d.Time,
                                     River = s.River
                                 })
                        //group e by e.StationId into g
                        select new
                        {
                            StationId = e.StationId,
                            RecordId = e.RecordId,
                            Time = e.Time,
                            River = e.River
                        };
            /*                        
*/
            /*g.FirstOrDefault().Stationid equals s.Uid
             * select new
            {
                RecordId = d.Recordid,
                Time = d.Time,
                StationId = d.Stationid,
                Name = s.Name
            };*/
            /*           query = from q in query
                               group q by q.StationId into g
                               select g.OrderByDescending(t => t.Time).FirstOrDefault();*/
            int count = 0;

            foreach (var data in query)
            {
                count++;
            }

            count = 0;


            return entity;
        }

        public HisHydrologicEntity RetrieveHisEntity(DateTime fromDate, DateTime toDate)
        {
            var entity = new HisHydrologicEntity();
            entity.Items = new List<HisHydrologicEntityItem>();

            Configuration myConfigration;
            ISessionFactory mySessionFactory;
            ISession mySession;
            myConfigration = new Configuration();
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

            var datas = mySession.Query<Hydrologicaldata>();
            var stations = mySession.Query<Hydrologicalstation>();
            var query = from e in
                            (from d in datas
                             join s in stations
                             on d.Stationid equals s.Uid
                             where (d.Time < toDate) && (d.Time > fromDate)
                             select new
                             {
                                 StationId = d.Stationid,
                                 RecordId = d.Recordid,
                                 Time = d.Time,
                                 River = s.River
                             })
                            //group e by e.StationId into g
                        select new
                        {
                            StationId = e.StationId,
                            RecordId = e.RecordId,
                            Time = e.Time,
                            River = e.River
                        };
            /*                        
*/
            /*g.FirstOrDefault().Stationid equals s.Uid
             * select new
            {
                RecordId = d.Recordid,
                Time = d.Time,
                StationId = d.Stationid,
                Name = s.Name
            };*/
            /*           query = from q in query
                               group q by q.StationId into g
                               select g.OrderByDescending(t => t.Time).FirstOrDefault();*/
            int count = 0;

            foreach (var data in query)
            {
                count++;
            }

            count = 0;


            return entity;
        }
    }
}
