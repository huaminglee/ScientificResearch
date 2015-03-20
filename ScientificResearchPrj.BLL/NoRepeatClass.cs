using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScientificResearchPrj.BLL
{
    public class NoRepeatClass
    {
        private Hashtable classHash = new Hashtable();
        public bool IfClassExist(string key)
        {
            bool exist = false;
            try
            {
                exist = classHash.Contains(key);
                return exist;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
        public void AddClass(string key, string value)
        {
            try
            {
                classHash.Add(key, value);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
    }
}
