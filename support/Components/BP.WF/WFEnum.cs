using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 用户信息显示格式
    /// </summary>
    public enum UserInfoShowModel
    {
        /// <summary>
        /// 用户ID,用户名
        /// </summary>
        UserIDUserName = 0,
        /// <summary>
        /// 用户ID
        /// </summary>
        UserIDOnly = 1,
        /// <summary>
        /// 用户名
        /// </summary>
        UserNameOnly = 2
    }
    /// <summary>
    /// 组织结构模式
    /// </summary>
    public enum OSModel
    {
        WorkFlow = 0,
        BPM = 1
    }
}
