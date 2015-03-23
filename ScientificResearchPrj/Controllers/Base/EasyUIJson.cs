using Newtonsoft.Json;
using ScientificResearchPrj.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace ScientificResearchPrj.Controllers.Base
{
    public class EasyUIJson
    {
        public static string GetEasyUIJsonFromDataTable(DataTable table)
        {
            string json = "{\"total\":" + table.Rows.Count + ",\"rows\":" + JsonConvert.SerializeObject(table) + "}";
            return json;
        }

        public static string GetEasyUIJsonFromList<T>(List<T> vals)
        {
            System.Text.StringBuilder st = new System.Text.StringBuilder();
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                foreach (T city in vals)
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        serializer.WriteObject(ms, city);
                        st.Append(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string json = "{\"total\":" + vals.Count + ",\"rows\":" + st.ToString() + "}";
            return json;
        }

        public static string GetEasyUIJson_Station(List<MyPort_Station> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (MyPort_Station sta in list)
            {
                string temp = 
                    "\"StaNo\":" + "\"" + sta.StaNo + "\"," +
                    "\"Name\":" + "\"" + sta.Name + "\"," +
                    "\"Description\":" + "\"" + sta.Description + "\"," +
                    "\"StaGrade\":" + "\"" + sta.StaGrade + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }


        public static string GetEasyUIJson_Dept(List<MyPort_Dept> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            string nowTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            foreach (MyPort_Dept dept in list)
            {
                string temp = 
                    "\"TreeId\":" + "\"" + (dept.DeptNo + nowTime) + "\"," +
                    "\"DeptNo\":" + "\"" + dept.DeptNo + "\"," +
                    "\"Name\":" + "\"" + dept.Name + "\"," +
                    "\"ParentNo\":" + "\"" + dept.ParentNo + "\"," +
                    "\"_parentId\":" + "\"" + (dept.ParentNo.Equals("0") ? null : (dept.ParentNo + nowTime)) + "\"," + 
                    "\"Description\":" + "\"" + dept.Description + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }


        public static string GetEasyUIJson_Emp(List<MyPort_Emp> list,string type)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (MyPort_Emp emp in list)
            {
                string temp =
                        "\"EmpNo\":" + "\"" + emp.EmpNo + "\"," +
                        "\"Name\":" + "\"" + emp.Name + "\"," +
                        "\"Tel\":" + "\"" + emp.Tel + "\"," +
                        "\"Email\":" + "\"" + emp.Email + "\"," +
                        "\"Type\":" + "\"" + emp.Type + "\"," +
                        "\"FK_Dept\":" + "\"" + emp.FK_Dept + "\"," +
                        "\"FK_DeptName\":" + "\"" + emp.FK_DeptName + "\",";

                if (type!=null && type.Equals(EmpType.DAOSHI))
                {
                    temp +=
                        "\"FK_Station\":" + "\"" + emp.MyPort_Tutor.FK_Station + "\"," +
                        "\"FK_StationName\":" + "\"" + emp.MyPort_Tutor.FK_StationName + "\"," +
                        "\"ChargeWork\":" + "\"" + emp.MyPort_Tutor.ChargeWork + "\"," +
                        "\"OfficeAddr\":" + "\"" + emp.MyPort_Tutor.OfficeAddr + "\"," +
                        "\"OfficeTel\":" + "\"" + emp.MyPort_Tutor.OfficeTel + "\"";
                }
                else if (type != null)
                {
                    temp +=
                        "\"AdmissionYear\":" + "\"" + emp.MyPort_Student.AdmissionYear + "\"," +
                        "\"SchoolingLength\":" + "\"" + emp.MyPort_Student.SchoolingLength + "\"," +
                        "\"FK_Tutor\":" + "\"" + emp.MyPort_Student.FK_Tutor + "\"," +
                        "\"FK_TutorName\":" + "\"" + emp.MyPort_Student.FK_TutorName + "\"," +
                        "\"LabAddr\":" + "\"" + emp.MyPort_Student.LabAddr + "\"";
                }
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_ProjectGroup(List<Process_ProjectGroup> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_ProjectGroup prjGroup in list)
            {
                string temp = 
                    "\"No\":" + "\"" + prjGroup.No + "\"," +
                    "\"Name\":" + "\"" + prjGroup.Name + "\"," +
                    "\"FK_GroupLeader\":" + "\"" + prjGroup.FK_GroupLeader + "\"," +
                    "\"FK_GroupLeaderName\":" + "\"" + prjGroup.FK_GroupLeaderName + "\"," +
                    "\"FK_GroupMember\":" + "\"" + prjGroup.FK_GroupMember + "\"," +
                    "\"FK_GroupMemberName\":" + "\"" + prjGroup.FK_GroupMemberName + "\"," +
                    "\"Description\":" + "\"" + prjGroup.Description + "\","+
                    "\"Projects\":" + "\"" + prjGroup.Projects + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_ProjectGroupForCombobox(List<Process_ProjectGroup> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_ProjectGroup prjGroup in list)
            {
                string temp =
                    "\"value\":" + "\"" + prjGroup.No + "\"," +
                    "\"text\":" + "\"" + prjGroup.Name + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_Project(List<Process_Project> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_Project prj in list)
            {
                string temp = 
                    "\"No\":" + "\"" + prj.Process_BasicData.No + "\"," +
                    "\"Name\":" + "\"" + prj.Process_BasicData.Name + "\"," +
                    "\"FK_Proposer\":" + "\"" + prj.Process_BasicData.FK_Proposer + "\"," +
                    "\"ProposerName\":" + "\"" + prj.Process_BasicData.ProposerName + "\"," +
                    "\"ProposeTime\":" + "\"" + prj.Process_BasicData.ProposeTime + "\"," +
                    "\"Description\":" + "\"" + prj.Process_BasicData.Description + "\"," +
                    "\"Keys\":" + "\"" + prj.Process_BasicData.Keys + "\"," +
                    "\"Remarks\":" + "\"" + prj.Process_BasicData.Remarks + "\"," +
                    "\"ModifyTime\":" + "\"" + prj.Process_BasicData.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + prj.Process_BasicData.FK_Flow + "\"," +
                    "\"WorkId\":" + "\"" + prj.Process_BasicData.WorkId + "\"," +
                    "\"FK_Node\":" + "\"" + prj.Process_BasicData.FK_Node + "\"," +


                    "\"OID\":" + "\"" + prj.OID + "\","+
                    "\"FK_Xmz\":" + "\"" + prj.FK_Xmz + "\","+
                    "\"FK_XMZName\":" + "\"" + prj.FK_XMZName + "\","+
                    "\"Columns\":" + "\"" + prj.Columns + "\","+
                    "\"Tasks\":" + "\"" + prj.Tasks + "\","+
                    "\"Questions\":" + "\"" + prj.Questions + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_Subject(List<Process_Subject_XuQiuFenXi> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_Subject_XuQiuFenXi subject in list)
            {
                string temp =
                    "\"No\":" + "\"" + subject.Process_BasicData.No + "\"," +
                    "\"Name\":" + "\"" + subject.Process_BasicData.Name + "\"," +
                    "\"FK_Proposer\":" + "\"" + subject.Process_BasicData.FK_Proposer + "\"," +
                    "\"ProposerName\":" + "\"" + subject.Process_BasicData.ProposerName + "\"," +
                    "\"ProposeTime\":" + "\"" + subject.Process_BasicData.ProposeTime + "\"," +
                    "\"Description\":" + "\"" + subject.Process_BasicData.Description + "\"," +
                    "\"Keys\":" + "\"" + subject.Process_BasicData.Keys + "\"," +
                    "\"Remarks\":" + "\"" + subject.Process_BasicData.Remarks + "\"," +
                    "\"ModifyTime\":" + "\"" + subject.Process_BasicData.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + subject.Process_BasicData.FK_Flow + "\"," +
                    "\"WorkId\":" + "\"" + subject.Process_BasicData.WorkId + "\"," +
                    "\"FK_Node\":" + "\"" + subject.Process_BasicData.FK_Node + "\"," +

                    "\"OID\":" + "\"" + subject.OID + "\"," +
                    "\"FK_XmOID\":" + "\"" + subject.FK_XmOID + "\"," +
                    "\"FK_XmName\":" + "\"" + subject.FK_XmName + "\"," +
                    "\"SourceDesc\":" + "\"" + subject.SourceDesc + "\"," +
                    "\"AnalysisResult\":" + "\"" + subject.AnalysisResult + "\"," +
                    "\"TargetTask\":" + "\"" + subject.TargetTask + "\"," +
                    "\"Innovation\":" + "\"" + subject.Innovation + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_ShenHeJieGuo(List<Process_ShenHe> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_ShenHe shenhe in list)
            {
                string temp =
                    "\"OID\":" + "\"" + shenhe.OID + "\"," +
                    "\"ShenHeRen\":" + "\"" + shenhe.ShenHeRen + "\"," +
                    "\"ShenHeRenName\":" + "\"" + shenhe.ShenHeRenName + "\"," +
                    "\"ShenHeShiJian\":" + "\"" + shenhe.ShenHeShiJian + "\"," +
                    "\"ShenHeJieGuo\":" + "\"" + shenhe.ShenHeJieGuo + "\"," +
                    "\"ShenHeYiJian\":" + "\"" + shenhe.ShenHeYiJian + "\"," +
                    "\"ModifyTime\":" + "\"" + shenhe.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + shenhe.FK_Flow + "\"," +
                    "\"FK_Node\":" + "\"" + shenhe.FK_Node + "\"," +
                    "\"WorkID\":" + "\"" + shenhe.WorkID + "\"," +
                    "\"StepType\":" + "\"" + shenhe.StepType + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_DiaoYan(List<Process_DiaoYan> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_DiaoYan diaoyan in list)
            {
                string temp =
                    "\"No\":" + "\"" + diaoyan.Process_BasicData.No + "\"," +
                    "\"Name\":" + "\"" + diaoyan.Process_BasicData.Name + "\"," +
                    "\"FK_Proposer\":" + "\"" + diaoyan.Process_BasicData.FK_Proposer + "\"," +
                    "\"ProposerName\":" + "\"" + diaoyan.Process_BasicData.ProposerName + "\"," +
                    "\"ProposeTime\":" + "\"" + diaoyan.Process_BasicData.ProposeTime + "\"," +
                    "\"Description\":" + "\"" + diaoyan.Process_BasicData.Description + "\"," +
                    "\"Keys\":" + "\"" + diaoyan.Process_BasicData.Keys + "\"," +
                    "\"Remarks\":" + "\"" + diaoyan.Process_BasicData.Remarks + "\"," +
                    "\"ModifyTime\":" + "\"" + diaoyan.Process_BasicData.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + diaoyan.Process_BasicData.FK_Flow + "\"," +
                    "\"WorkId\":" + "\"" + diaoyan.Process_BasicData.WorkId + "\"," +
                    "\"FK_Node\":" + "\"" + diaoyan.Process_BasicData.FK_Node + "\"," +

                    "\"OID\":" + "\"" + diaoyan.OID + "\"," +
                    "\"SumType\":" + "\"" + diaoyan.SumType + "\"," +
                    "\"Sum\":" + "\"" + diaoyan.Sum + "\"," +
                    "\"SurveryAddr\":" + "\"" + diaoyan.SurveryAddr + "\"," +
                    "\"Investigator\":" + "\"" + diaoyan.Investigator + "\"," +
                    "\"AnalysisResult\":" + "\"" + diaoyan.AnalysisResult + "\"," +
                    "\"AdvantageValue\":" + "\"" + diaoyan.AdvantageValue + "\"," +
                    "\"WeaknessValue\":" + "\"" + diaoyan.WeaknessValue + "\"," +
                    "\"UnsolvedProblem\":" + "\"" + diaoyan.UnsolvedProblem + "\"," +
                    "\"TechTrends\":" + "\"" + diaoyan.TechTrends + "\"," +
                    "\"BeyondPoint\":" + "\"" + diaoyan.BeyondPoint + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_TiChuWenTi(List<Process_TiChuWenTi> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_TiChuWenTi wenti in list)
            {
                string temp =
                    "\"No\":" + "\"" + wenti.Process_BasicData.No + "\"," +
                    "\"Name\":" + "\"" + wenti.Process_BasicData.Name + "\"," +
                    "\"FK_Proposer\":" + "\"" + wenti.Process_BasicData.FK_Proposer + "\"," +
                    "\"ProposerName\":" + "\"" + wenti.Process_BasicData.ProposerName + "\"," +
                    "\"ProposeTime\":" + "\"" + wenti.Process_BasicData.ProposeTime + "\"," +
                    "\"Description\":" + "\"" + wenti.Process_BasicData.Description + "\"," +
                    "\"Keys\":" + "\"" + wenti.Process_BasicData.Keys + "\"," +
                    "\"Remarks\":" + "\"" + wenti.Process_BasicData.Remarks + "\"," +
                    "\"ModifyTime\":" + "\"" + wenti.Process_BasicData.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + wenti.Process_BasicData.FK_Flow + "\"," +
                    "\"WorkId\":" + "\"" + wenti.Process_BasicData.WorkId + "\"," +
                    "\"FK_Node\":" + "\"" + wenti.Process_BasicData.FK_Node + "\"," +

                    "\"OID\":" + "\"" + wenti.OID + "\"," +
                    "\"Mitigation\":" + "\"" + wenti.Mitigation + "\"," +
                    "\"OvercomeMethod\":" + "\"" + wenti.OvercomeMethod + "\"," +
                    "\"Argument\":" + "\"" + wenti.Argument + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_JieJueSiLu(List<Process_SiLu> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_SiLu silu in list)
            {
                string temp =
                    "\"No\":" + "\"" + silu.Process_BasicData.No + "\"," +
                    "\"Name\":" + "\"" + silu.Process_BasicData.Name + "\"," +
                    "\"FK_Proposer\":" + "\"" + silu.Process_BasicData.FK_Proposer + "\"," +
                    "\"ProposerName\":" + "\"" + silu.Process_BasicData.ProposerName + "\"," +
                    "\"ProposeTime\":" + "\"" + silu.Process_BasicData.ProposeTime + "\"," +
                    "\"Description\":" + "\"" + silu.Process_BasicData.Description + "\"," +
                    "\"Keys\":" + "\"" + silu.Process_BasicData.Keys + "\"," +
                    "\"Remarks\":" + "\"" + silu.Process_BasicData.Remarks + "\"," +
                    "\"ModifyTime\":" + "\"" + silu.Process_BasicData.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + silu.Process_BasicData.FK_Flow + "\"," +
                    "\"WorkId\":" + "\"" + silu.Process_BasicData.WorkId + "\"," +
                    "\"FK_Node\":" + "\"" + silu.Process_BasicData.FK_Node + "\"," +

                    "\"OID\":" + "\"" + silu.OID + "\"," +
                    "\"Type\":" + "\"" + silu.Type + "\"," +
                    "\"FK_WTOID\":" + "\"" + silu.FK_WTOID + "\"," +
                    "\"FK_WTName\":" + "\"" + silu.FK_WTName + "\""; 
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_XingShiHua(List<Process_XingShiHua> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_XingShiHua xingshihua in list)
            {
                string temp =
                    "\"No\":" + "\"" + xingshihua.Process_BasicData.No + "\"," +
                    "\"Name\":" + "\"" + xingshihua.Process_BasicData.Name + "\"," +
                    "\"FK_Proposer\":" + "\"" + xingshihua.Process_BasicData.FK_Proposer + "\"," +
                    "\"ProposerName\":" + "\"" + xingshihua.Process_BasicData.ProposerName + "\"," +
                    "\"ProposeTime\":" + "\"" + xingshihua.Process_BasicData.ProposeTime + "\"," +
                    "\"Description\":" + "\"" + xingshihua.Process_BasicData.Description + "\"," +
                    "\"Keys\":" + "\"" + xingshihua.Process_BasicData.Keys + "\"," +
                    "\"Remarks\":" + "\"" + xingshihua.Process_BasicData.Remarks + "\"," +
                    "\"ModifyTime\":" + "\"" + xingshihua.Process_BasicData.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + xingshihua.Process_BasicData.FK_Flow + "\"," +
                    "\"WorkId\":" + "\"" + xingshihua.Process_BasicData.WorkId + "\"," +
                    "\"FK_Node\":" + "\"" + xingshihua.Process_BasicData.FK_Node + "\"," +

                    "\"OID\":" + "\"" + xingshihua.OID + "\"," +
                    "\"FK_SLOID\":" + "\"" + xingshihua.FK_SLOID + "\","+
                    "\"FK_SLName\":" + "\"" + xingshihua.FK_SLName + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_SuanFa(List<Process_SuanFa> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_SuanFa suanfa in list)
            {
                string temp =
                    "\"No\":" + "\"" + suanfa.Process_BasicData.No + "\"," +
                    "\"Name\":" + "\"" + suanfa.Process_BasicData.Name + "\"," +
                    "\"FK_Proposer\":" + "\"" + suanfa.Process_BasicData.FK_Proposer + "\"," +
                    "\"ProposerName\":" + "\"" + suanfa.Process_BasicData.ProposerName + "\"," +
                    "\"ProposeTime\":" + "\"" + suanfa.Process_BasicData.ProposeTime + "\"," +
                    "\"Description\":" + "\"" + suanfa.Process_BasicData.Description + "\"," +
                    "\"Keys\":" + "\"" + suanfa.Process_BasicData.Keys + "\"," +
                    "\"Remarks\":" + "\"" + suanfa.Process_BasicData.Remarks + "\"," +
                    "\"ModifyTime\":" + "\"" + suanfa.Process_BasicData.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + suanfa.Process_BasicData.FK_Flow + "\"," +
                    "\"WorkId\":" + "\"" + suanfa.Process_BasicData.WorkId + "\"," +
                    "\"FK_Node\":" + "\"" + suanfa.Process_BasicData.FK_Node + "\"," +

                    "\"OID\":" + "\"" + suanfa.OID + "\"," +
                    "\"Design\":" + "\"" + suanfa.Design + "\"," +
                    "\"RealizeStep\":" + "\"" + suanfa.RealizeStep + "\"," +
                    "\"FK_XSHOID\":" + "\"" + suanfa.FK_XSHOID + "\"," +
                    "\"FK_XSHName\":" + "\"" + suanfa.FK_XSHName + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_ShiYan(List<Process_ShiYan> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_ShiYan shiyan in list)
            {
                string temp =
                    "\"No\":" + "\"" + shiyan.Process_BasicData.No + "\"," +
                    "\"Name\":" + "\"" + shiyan.Process_BasicData.Name + "\"," +
                    "\"FK_Proposer\":" + "\"" + shiyan.Process_BasicData.FK_Proposer + "\"," +
                    "\"ProposerName\":" + "\"" + shiyan.Process_BasicData.ProposerName + "\"," +
                    "\"ProposeTime\":" + "\"" + shiyan.Process_BasicData.ProposeTime + "\"," +
                    "\"Description\":" + "\"" + shiyan.Process_BasicData.Description + "\"," +
                    "\"Keys\":" + "\"" + shiyan.Process_BasicData.Keys + "\"," +
                    "\"Remarks\":" + "\"" + shiyan.Process_BasicData.Remarks + "\"," +
                    "\"ModifyTime\":" + "\"" + shiyan.Process_BasicData.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + shiyan.Process_BasicData.FK_Flow + "\"," +
                    "\"WorkId\":" + "\"" + shiyan.Process_BasicData.WorkId + "\"," +
                    "\"FK_Node\":" + "\"" + shiyan.Process_BasicData.FK_Node + "\"," +

                    "\"OID\":" + "\"" + shiyan.OID + "\"," +
                    "\"Design\":" + "\"" + shiyan.Design + "\"," +
                    "\"IndexSys\":" + "\"" + shiyan.IndexSys + "\"," +
                    "\"RealizeStep\":" + "\"" + shiyan.RealizeStep + "\"," +
                    "\"TestCondition\":" + "\"" + shiyan.TestCondition + "\"," +
                    "\"Data\":" + "\"" + shiyan.Data + "\"," +
                    "\"StatistacalResult\":" + "\"" + shiyan.StatistacalResult + "\"," +
                    "\"Result\":" + "\"" + shiyan.Result + "\"," +
                    "\"FK_SFOID\":" + "\"" + shiyan.FK_SFOID + "\"," +
                    "\"FK_SFName\":" + "\"" + shiyan.FK_SFName + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_DuiBiFenXi(List<Process_DuiBiFenXi> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_DuiBiFenXi dbfx in list)
            {
                string temp =
                    "\"No\":" + "\"" + dbfx.Process_BasicData.No + "\"," +
                    "\"Name\":" + "\"" + dbfx.Process_BasicData.Name + "\"," +
                    "\"FK_Proposer\":" + "\"" + dbfx.Process_BasicData.FK_Proposer + "\"," +
                    "\"ProposerName\":" + "\"" + dbfx.Process_BasicData.ProposerName + "\"," +
                    "\"ProposeTime\":" + "\"" + dbfx.Process_BasicData.ProposeTime + "\"," +
                    "\"Description\":" + "\"" + dbfx.Process_BasicData.Description + "\"," +
                    "\"Keys\":" + "\"" + dbfx.Process_BasicData.Keys + "\"," +
                    "\"Remarks\":" + "\"" + dbfx.Process_BasicData.Remarks + "\"," +
                    "\"ModifyTime\":" + "\"" + dbfx.Process_BasicData.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + dbfx.Process_BasicData.FK_Flow + "\"," +
                    "\"WorkId\":" + "\"" + dbfx.Process_BasicData.WorkId + "\"," +
                    "\"FK_Node\":" + "\"" + dbfx.Process_BasicData.FK_Node + "\"," +

                    "\"OID\":" + "\"" + dbfx.OID + "\"," +
                    "\"Data\":" + "\"" + dbfx.Data + "\"," +
                    "\"Methods\":" + "\"" + dbfx.Methods + "\"," +
                    "\"AnalysisResult\":" + "\"" + dbfx.AnalysisResult + "\"," +
                    "\"InferType\":" + "\"" + dbfx.InferType + "\"," +
                    "\"InferContent\":" + "\"" + dbfx.InferContent + "\"," +
                    "\"FK_SYOID\":" + "\"" + dbfx.FK_SYOID + "\"," +
                    "\"FK_SYName\":" + "\"" + dbfx.FK_SYName + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_DeChuJieLun(List<Process_DeChuJieLun> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_DeChuJieLun jielun in list)
            {
                string temp =
                    "\"No\":" + "\"" + jielun.Process_BasicData.No + "\"," +
                    "\"Name\":" + "\"" + jielun.Process_BasicData.Name + "\"," +
                    "\"FK_Proposer\":" + "\"" + jielun.Process_BasicData.FK_Proposer + "\"," +
                    "\"ProposerName\":" + "\"" + jielun.Process_BasicData.ProposerName + "\"," +
                    "\"ProposeTime\":" + "\"" + jielun.Process_BasicData.ProposeTime + "\"," +
                    "\"Description\":" + "\"" + jielun.Process_BasicData.Description + "\"," +
                    "\"Keys\":" + "\"" + jielun.Process_BasicData.Keys + "\"," +
                    "\"Remarks\":" + "\"" + jielun.Process_BasicData.Remarks + "\"," +
                    "\"ModifyTime\":" + "\"" + jielun.Process_BasicData.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + jielun.Process_BasicData.FK_Flow + "\"," +
                    "\"WorkId\":" + "\"" + jielun.Process_BasicData.WorkId + "\"," +
                    "\"FK_Node\":" + "\"" + jielun.Process_BasicData.FK_Node + "\"," +

                    "\"OID\":" + "\"" + jielun.OID + "\"," +
                    "\"Mitigation\":" + "\"" + jielun.Mitigation + "\"," +
                    "\"EffectiveSolution\":" + "\"" + jielun.EffectiveSolution + "\"," +
                    "\"Arguments\":" + "\"" + jielun.Arguments + "\"," +
                    "\"FK_DBFXOID\":" + "\"" + jielun.FK_DBFXOID + "\"," +
                    "\"FK_DBFXName\":" + "\"" + jielun.FK_DBFXName + "\"";
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_LunWen(List<Process_LunWen> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_LunWen lunwen in list)
            {
                string temp =
                    "\"No\":" + "\"" + lunwen.Process_BasicData.No + "\"," +
                    "\"Name\":" + "\"" + lunwen.Process_BasicData.Name + "\"," +
                    "\"FK_Proposer\":" + "\"" + lunwen.Process_BasicData.FK_Proposer + "\"," +
                    "\"ProposerName\":" + "\"" + lunwen.Process_BasicData.ProposerName + "\"," +
                    "\"ProposeTime\":" + "\"" + lunwen.Process_BasicData.ProposeTime + "\"," +
                    "\"Description\":" + "\"" + lunwen.Process_BasicData.Description + "\"," +
                    "\"Keys\":" + "\"" + lunwen.Process_BasicData.Keys + "\"," +
                    "\"Remarks\":" + "\"" + lunwen.Process_BasicData.Remarks + "\"," +
                    "\"ModifyTime\":" + "\"" + lunwen.Process_BasicData.ModifyTime + "\"," +
                    "\"FK_Flow\":" + "\"" + lunwen.Process_BasicData.FK_Flow + "\"," +
                    "\"WorkId\":" + "\"" + lunwen.Process_BasicData.WorkId + "\"," +
                    "\"FK_Node\":" + "\"" + lunwen.Process_BasicData.FK_Node + "\"," +

                    "\"OID\":" + "\"" + lunwen.OID + "\"," +
                    "\"Motivation\":" + "\"" + lunwen.Motivation + "\"," +
                    "\"Questions\":" + "\"" + lunwen.Questions + "\"," +
                    "\"Design\":" + "\"" + lunwen.Design + "\"," +
                    "\"Realize\":" + "\"" + lunwen.Realize + "\"," +
                    "\"TestData\":" + "\"" + lunwen.TestData + "\"," +
                    "\"Result\":" + "\"" + lunwen.Result + "\""; 
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_Link(List<Process_Link> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_Link link in list)
            {
                string temp =
                    "\"OID\":" + "\"" + link.OID + "\"," +
                    "\"No_OID\":" + "\"" + link.No_OID + "\"," +
                    "\"IsShenHe\":" + "\"" + link.IsShenHe + "\"," +
                    "\"LinkHref\":" + "\"" + link.LinkHref + "\"," +
                    "\"LinkDesc\":" + "\"" + link.LinkDesc + "\""; 
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }

        public static string GetEasyUIJson_File(List<Process_Attach> list)
        {
            StringBuilder _Json = new StringBuilder();
            _Json.Append("{\"total\":" + list.Count + ",\"rows\":[");
            foreach (Process_Attach file in list)
            {
                string temp =
                    "\"OID\":" + "\"" + file.OID + "\"," +
                    "\"No_OID\":" + "\"" + file.No_OID + "\"," +
                    "\"IsShenHe\":" + "\"" + file.IsShenHe + "\"," +
                    "\"AttachName\":" + "\"" + file.AttachName + "\"," +
                    "\"AttachDesc\":" + "\"" + file.AttachDesc + "\"," +
                    "\"Path\":" + "\"" + file.Path + "\""; 
                _Json.Append("{" + temp + "},");
            }
            if (_Json[_Json.Length - 1] == ',')
            {
                _Json = _Json.Remove(_Json.Length - 1, 1);
            }
            _Json.Append("]}");
            return _Json.ToString();
        }
    }
}