﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.Model
{
    public class PageModelForDataTable 
    {
        //结果集  
        private DataTable table;
        //记录数  
        private int totalCount;
        //每页多少条数据  
        private int pageSize;
        //第几页  
        private int pageNo;
        /**  
         * 返回总页数  
         * @return  
         */
        public int GetTotalPages()
        {
            return (totalCount + pageSize - 1) / pageSize;
        }
        /**  
         * 首页  
         * @return  
         */
        public int GetTopPageNo()
        {
            return 1;
        }
        /**  
         * 上一页   
         * @return  
         */
        public int GetPreviousPageNo()
        {
            if (this.pageNo <= 1)
            {
                return 1;
            }
            return this.pageNo - 1;
        }
        /**  
         * 下一页  
         * @return  
         */
        public int GetNextPageNo()
        {
            if (this.pageNo >= GetButtomPageNo())
            {
                return GetButtomPageNo();
            }
            return this.pageNo + 1;
        }
        /**  
         * 尾页  
         * @return  
         */
        public int GetButtomPageNo()
        {
            return GetTotalPages();
        }
        public DataTable  GetTable()
        {
            return table;
        }
        public void SetTable(DataTable table)
        {
            this.table = table;
        }
        public int GetTotalCount()
        {
            return totalCount;
        }
        public void SetTotalCount(int totalCount)
        {
            this.totalCount = totalCount;
        }
        public int GetPageSize()
        {
            return pageSize;
        }
        public void SetPageSize(int pageSize)
        {
            this.pageSize = pageSize;
        }
        public int GetPageNo()
        {
            return pageNo;
        }
        public void SetPageNo(int pageNo)
        {
            this.pageNo = pageNo;
        }
    }  
}
