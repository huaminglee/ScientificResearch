using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using ScientificResearchPrj.DALFactory;

namespace ScientificResearchPrj.BLL
{
    public abstract class BaseService<T>where T:class ,new()
    {
        public DBSession DbSession { get { return DBSessionFactory.GetCurrentDbSession(); } }

        public IDAL.IBaseDAL<T> CurrentDAL{ get; set; }//表示当前数据操作类的实例

        public abstract void SetCurrentDAL();//定义一个抽象方法让子类实现，设置当前DAL

        public BaseService()
        {
            SetCurrentDAL();
        } 
        public IQueryable<T> LoadEntities(System.Linq.Expressions.Expression<Func<T, bool>> whereLambda)
        {
            return this.CurrentDAL.LoadEntities(whereLambda).AsQueryable();
        }

        public IQueryable<T> LoadPageEntities<s>(int pageIndex, int pageSize, out int totalCount,
            System.Linq.Expressions.Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, s>> orderLambda, bool isAsc)
        {
            return this.CurrentDAL.LoadPageEntities(pageIndex, pageSize, out totalCount, whereLambda, orderLambda,
                isAsc);

        }

        public T AddEntity(T entity)
        {
            this.CurrentDAL.AddEntity(entity);
            this.DbSession.SaveChanges();
            return entity;
        }

        public bool UpdateEntity(T entity)
        {
            this.CurrentDAL.UpdateEntity(entity);
            return this.DbSession.SaveChanges() > 0;
        }

        public bool DeleteEntity(T entity)
        {
            this.CurrentDAL.DeleteEntity(entity);
            return this.DbSession.SaveChanges()>0;
        }

        public bool DetachEntity(T entity)
        {
            return this.CurrentDAL.DetachEntity(entity);
        }

        public bool UnchangeEntity(T entity)
        {
            return this.CurrentDAL.UnchangeEntity(entity);
        }
    }
}
