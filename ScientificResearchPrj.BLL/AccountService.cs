using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ScientificResearchPrj.BLL 
{
    public class AccountService : BaseService<AccountModel>, IAccountService
    {
        public override void SetCurrentDAL()
        {
            CurrentDAL = null;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Dictionary<string, string> SignIn(AccountModel model)
        {
            //作为密码方式加密   
            string md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(model.Password, "MD5");
            System.Diagnostics.Debug.WriteLine("md5========"+md5);

            MyPort_Emp emp=this.DbSession.EmpDAL.LoadEntities(a => a.EmpNo == model.UserName && a.Pass == md5).FirstOrDefault();
            if (emp != null)
            {
                ///WebUser.SID默认设置为session的id（登陆时SignInOfGener设置），而取出sid的过程先是从session取，
                ///取不到则从cookie中取
                ///
                ///如果数据表一样采取Session.SessionID，则考虑点在于是否在过滤器设置session，session过期时重置cookie
                ///这样做的话每次重新登录的sid不一定会不一样，因为有可能是同一个session，Session.SessionID也就一样了
                /// 
                ///
                ///如果采取新的sid，则考虑点在于过滤器每次访问时，判断session是否过期，如果过期则重新从数据表获取sid，并写入
                ///Web.SID（实际上是写入session），不这样做的话，每次不登录直接进入系统时，session是不存在sid的，获取sid过程是从
                ///cookie获取的（而这个cookie就是上一次登录时写入的Session.SessionID），会导致sid与实际数据表的sid不一致
                ///这样做的话每次重新登录获取到的sid都会是不一样的，但是不会等同于cookie的sid
                
                ///Random rand = new Random(Guid.NewGuid().GetHashCode());
                ///string sid1 =rand.Next().ToString();
                ///emp.SID = sid1;
                emp.SID = System.Web.HttpContext.Current.Session.SessionID;

                this.DbSession.EmpDAL.UpdateEntity(emp);
                this.DbSession.SaveChanges();


              
                //调用ccflow的登录,返回从数据库查询到的sid
                BP.WF.Dev2Interface.Port_Login(emp.EmpNo);
                

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "0");
                dictionary.Add("message", "登录成功");
                return dictionary;
            }
            else
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("state", "-1");
                dictionary.Add("message", "用户名或密码出错");
                return dictionary;
            }
        }
    }
}