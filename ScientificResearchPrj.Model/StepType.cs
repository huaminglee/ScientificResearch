using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScientificResearchPrj.Model
{
    public class StepType 
    {
        public static string PROJECT_XUQIUFENXI { get { return "1"; } }//项目
        public static string SUBJECT_XUQIUFENXI { get { return "2"; } }//课题
        public static string DIAOYAN { get { return "3"; } }//调研
        public static string TICHUWENTI { get { return "4"; } }//提出问题
        public static string JIEJUESILU { get { return "5"; } }//解决思路
        public static string XINGSHIHUA { get { return "6"; } }//形式化
        public static string SHEJISUANFA { get { return "7"; } }//设计算法
        public static string SHEJISHIYAN { get { return "8"; } }//设计实验
        public static string LIANGHUADUIBIFENXI { get { return "9"; } }//量化对比分析
        public static string DECHUJIELUN { get { return "10"; } }//得出结论
        public static string LUNWENZHUANXIE { get { return "11"; } }//论文撰写
    }
}
