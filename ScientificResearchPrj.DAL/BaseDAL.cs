using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using ScientificResearchPrj.IDAL;
using ScientificResearchPrj.Model;
using System.Data;

namespace ScientificResearchPrj.DAL
{
   public class BaseDAL<T>where T:class,new()
   {
       DbContext dbEntities=DbContextFactory.GetCurrentDbContext();
       public IQueryable<T> LoadEntities(System.Linq.Expressions.Expression<Func<T, bool>> whereLambda)
        {
            return dbEntities.Set<T>().Where(whereLambda).AsQueryable();
        }

        public IQueryable<T> LoadPageEntities<s>(int pageIndex, int pageSize, out int totalCount,
           System.Linq.Expressions.Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, s>> orderLambda, bool isAsc)
        {
            var temp = dbEntities.Set<T>().Where(whereLambda);
            totalCount = temp.Count();
            if (isAsc)
            {
                return temp.OrderBy(orderLambda).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsQueryable();
            }
            else
            {
                return temp.OrderByDescending(orderLambda)
                     .Skip((pageIndex - 1) * pageSize)
                     .Take(pageSize)
                     .OrderBy(orderLambda).AsQueryable();
            }

        }

        public T AddEntity(T entity)
        {
            dbEntities.Set<T>().Add(entity);
            return entity;
        }

        public bool UpdateEntity(T entity)
        {
            dbEntities.Entry<T>(entity).State = EntityState.Modified;
            return true;
        }

        public bool DeleteEntity(T entity)
        {
            dbEntities.Set<T>().Remove(entity);
            return true;
        }

        public bool DetachEntity(T entity)
        {
            dbEntities.Entry<T>(entity).State = EntityState.Detached;
            return true;
        }

        public bool UnchangeEntity(T entity)
        {
            dbEntities.Entry<T>(entity).State = EntityState.Unchanged;
            return true;
        }
    }
}
