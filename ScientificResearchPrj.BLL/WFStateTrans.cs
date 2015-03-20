using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScientificResearchPrj.BLL
{
    public class WFStateTrans
    {
         // 设置WFState对应的状态信息
        public static string GetWFStateStr(string sfState)
        {
            switch (sfState)
            {
                case "0":
                    return "空白";
                case "1":
                    return "草稿";
                case "2":
                    return "运行中";
                case "3":
                    return "已完成";
                case "4":
                    return "挂起";
                case "5":
                    return "退回";
                case "6":
                    return "转发";
                case "7":
                    return "删除";
                case "8":
                    return "加签";
                case "9":
                    return "冻结";
                case "10":
                    return "批处理";
                default:
                    return "";
            }
        }
    }
}
