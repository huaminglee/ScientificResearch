using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ScientificResearchPrj.DAL;
using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;

namespace ScientificResearchPrj.DALFactory
{
    public class DBSession:IDBSession
    {
        public DbContext dbEntities{get { return DbContextFactory.GetCurrentDbContext(); }}


        public IDepartmentDAL DepartmentDAL
        {
            get { return new DepartmentDAL(); }
        }

        public IEmpDAL EmpDAL
        {
            get { return new EmpDAL(); }
        }

        public ITutorDAL TutorDAL
        {
            get { return new TutorDAL(); }
        }

        public IStudentDAL StudentDAL
        {
            get { return new StudentDAL(); }
        }

        public IStationDAL StationDAL
        {
            get { return new StationDAL(); }
        }

        public IProjectGroupDAL ProjectGroupDAL
        {
            get { return new ProjectGroupDAL(); }
        }

        public IBasicDataDAL BasicDataDAL
        {
            get { return new BasicDataDAL(); }
        }

        public IProjectDAL ProjectDAL
        {
            get { return new ProjectDAL(); }
        }

        public ISubjectDAL SubjectDAL
        {
            get { return new SubjectDAL(); }
        }

        public ICommonOperationDAL CommonOperationDAL
        {
            get { return new CommonOperationDAL(); }
        }

        public IXuQiuFenXiDAL XuQiuFenXiDAL
        {
            get { return new XuQiuFenXiDAL(); }
        }

        public IDiaoYanDAL DiaoYanDAL
        {
            get { return new DiaoYanDAL(); }
        }

        public ITiChuWenTiDAL TiChuWenTiDAL
        {
            get { return new TiChuWenTiDAL(); }
        }

        public IJieJueSiLuDAL JieJueSiLuDAL
        {
            get { return new JieJueSiLuDAL(); }
        }

        public IXingShiHuaDAL XingShiHuaDAL
        {
            get { return new XingShiHuaDAL(); }
        }

        public ISheJiSuanFaDAL SheJiSuanFaDAL
        {
            get { return new SheJiSuanFaDAL(); }
        }

        public ISheJiShiYanDAL SheJiShiYanDAL
        {
            get { return new SheJiShiYanDAL(); }
        }

        public IDuiBiFenXiDAL DuiBiFenXiDAL
        {
            get { return new DuiBiFenXiDAL(); }
        }

        public IDeChuJieLunDAL DeChuJieLunDAL
        {
            get { return new DeChuJieLunDAL(); }
        }

        public ILunWenZhuanXieDAL LunWenZhuanXieDAL
        {
            get { return new LunWenZhuanXieDAL(); }
        }

        public ITrackDAL TrackDAL
        {
            get { return new TrackDAL(); }
        }

        public IShenHeDAL ShenHeDAL
        {
            get { return new ShenHeDAL(); }
        }
 
        public IProcessAllDAL ProcessAllDAL
        {
            get { return new ProcessAllDAL(); }
        }

        public IFaQiDAL FaQiDAL
        {
            get { return new FaQiDAL(); }
        }

        public IChaoSongDAL ChaoSongDAL
        {
            get { return new ChaoSongDAL(); }
        }

        public IZaiTuDAL ZaiTuDAL
        {
            get { return new ZaiTuDAL(); }
        }

        public IDaiBanDAL DaiBanDAL
        {
            get { return new DaiBanDAL(); }
        }

        public ILinkDAL LinkDAL
        {
            get { return new LinkDAL(); }
        }

        public IAttachDAL AttachDAL
        {
            get { return new AttachDAL(); }
        }

        public IJournalDAL JournalDAL
        {
            get { return new JournalDAL(); }
        }
         
        public List<string> ExecuteSql(string sql, params SqlParameter[] pars)
        {
            return dbEntities.Database.SqlQuery<string>(sql, pars).ToList();
        }

        public int SaveChanges()
        {
            return dbEntities.SaveChanges();
        }
    }
}
