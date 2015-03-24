using BP.WF;
using Newtonsoft.Json;
using ScientificResearchPrj.BLL;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers.Base
{
    public abstract class ProcessBase<T>  : Controller
    {
        public SRService SrService { get { return SRServiceFactory.GetCurrentSRService(); } }
        protected T CurrentService { get; set; }
        public abstract void SetCurrentService();//定义一个抽象方法让子类实现，设置当前Service
        public ProcessBase()
        {
            SetCurrentService();
        }

        #region 1、接受4大参数(这四大参数是由ccflow传递到对应加载表单页面上的).
        
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        #endregion 接受4大参数(这四大参数是有ccflow传递到此页面上的).

        protected void SetViewData()
        {
            //将当前流程相关参数保存起来
            ViewData["FK_Flow"] = FK_Flow;
            ViewData["WorkID"] = WorkID;
            ViewData["FK_Node"] = FK_Node;
            ViewData["FID"] = FID;

            SetLoginUserData();

            //查询是否有退回消息
            ReturnNodeModel retNode = new ReturnNodeModel();
            retNode.FK_Flow = FK_Flow;
            retNode.ReturnToNode = FK_Node;
            retNode.WorkID = WorkID;
            retNode.FID = FID;
            retNode.ReturnToEmps = BP.Web.WebUser.No;
 
            DataTable returnInfo = SrService.CommonOperationService.GetReturnInfo(retNode);
            ViewData["TuiHuiXiaoXi"] = EasyUIJson.GetEasyUIJsonFromDataTable(returnInfo);

            //本节点信息
            CCFlowArgs args = new CCFlowArgs();
            args.FK_Flow = FK_Flow;
            args.WorkID = WorkID;

            DataTable flowInfo = SrService.CommonOperationService.GetCurrentFlowInfoFromEmpWorks(args);
            ViewData["CurrentFlowInfo"] = EasyUIJson.GetEasyUIJsonFromDataTable(flowInfo);
        }

        protected void SetLoginUserData() {
            //当前登陆者
            ViewData["LoginUser"] = BP.Web.WebUser.No;
            ViewData["LoginUserName"] = BP.Web.WebUser.Name;
        }

        protected bool FaSong(CCFlowArgs args)
        {
            bool flag = true;
            try
            {
                string returnInfo = SrService.CommonOperationService.SendWorks(args);
                SendMessageToBroswer(returnInfo);
            }
            catch (Exception ex)
            {
                flag = false;
                this.Response.Write("<font color=red>发送期间出现异常:" + ex.Message + "</font>");
            }
            return flag;
        }
         
        public ActionResult ChaKanLiuChengTu()
        {
            string url = SrService.CommonOperationService.GetTrackURL(this.FK_Flow);
            return Redirect(url);
        }

        public ActionResult HuoQuTuiHuiJieDian(CCFlowArgs args)
        {
            DataTable canReturnNodes = SrService.CommonOperationService.GetCanReturnNodes(args);

            if (canReturnNodes != null && canReturnNodes.Rows.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "退回错误,系统没有找到可以退回的节点."
                }, JsonRequestBehavior.AllowGet);
            }

            //上次节点
            WorkNode pwn = SrService.CommonOperationService.GetPreviousWorkNode(args);

            return Json(new
            {
                state = "0",
                message = "加载成功",
                _previousNodeID = pwn.HisNode.NodeID.ToString(),
                _previousNodeName = pwn.HisNode.Name.ToString(),
                _Json = EasyUIJson.GetEasyUIJsonFromDataTable(canReturnNodes)
            });
        }

        public ActionResult TuiHui(ReturnNodeModel retNode)
        {
            try
            {
                string rInfo = this.SrService.CommonOperationService.ReturnWork(retNode);
                return Json(new
                {
                    state = "0",
                    message = rInfo,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    state = "-1",
                    message = ex.Message,
                });
            }
        }

        public ActionResult CuiBan(int workID, string msg)
        {
            try
            {
                string info = this.SrService.CommonOperationService.Press(workID,msg);
                return Json(new
                {
                    state = "0",
                    message = info,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    state = "-1",
                    message = ex.Message,
                });
            }
        }

        public ActionResult JieShuLiuCheng()
        {
            FlowOver args = new FlowOver();
            args.FK_Flow = this.FK_Flow;
            args.WorkID = this.WorkID;
            ///ToDo-----结束理由
            args.OverMsg = "";
            this.JieShu(args);
            //重定向到   链接到当前URL的客户端
            return Redirect(this.Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult GetHistoryDataFromTrack(CCFlowArgs args,string stepType)
        {
            if (stepType.Equals(StepType.PROJECT_XUQIUFENXI)) return GetProjectHistoryData(args);
            if (stepType.Equals(StepType.SUBJECT_XUQIUFENXI)) return GetSubjectHistoryData(args);
            if (stepType.Equals(StepType.DIAOYAN)) return GetDiaoYanHistoryData(args);
            if (stepType.Equals(StepType.TICHUWENTI)) return GetTiChuWenTiHistoryData(args);
            if (stepType.Equals(StepType.JIEJUESILU)) return GetJieJueSiLuHistoryData(args);
            if (stepType.Equals(StepType.XINGSHIHUA)) return GetXingShiHuaHistoryData(args);
            if (stepType.Equals(StepType.SHEJISUANFA)) return GetSheJiSuanFaHistoryData(args);
            if (stepType.Equals(StepType.SHEJISHIYAN)) return GetSheJiShiYanHistoryData(args);
            if (stepType.Equals(StepType.LIANGHUADUIBIFENXI)) return GetDuiBiFenXiHistoryData(args);
            if (stepType.Equals(StepType.DECHUJIELUN)) return GetDeChuJieLunHistoryData(args);
            if (stepType.Equals(StepType.LUNWENZHUANXIE)) return GetLunWenZhuanXieHistoryData(args);
            return null;
        }

        private ActionResult GetProjectHistoryData(CCFlowArgs args)
        {
            List<Process_Project> projectList = new List<Process_Project>();
            Process_Project project = this.SrService.ProjectService.GetProjectHistoryDataFromTrack(args);

            projectList.Add(project);

            if (project.OID == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载需求分析项目数据",
                    _Json = EasyUIJson.GetEasyUIJson_Project(projectList)
                });
            }
        }

        private ActionResult GetSubjectHistoryData(CCFlowArgs args)
        {
            List<Process_Subject_XuQiuFenXi> subjectList = new List<Process_Subject_XuQiuFenXi>();
            Process_Subject_XuQiuFenXi subject = this.SrService.SubjectService.GetSubjectHistoryDataFromTrack(args);

            subjectList.Add(subject);

            if (subject.OID == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载需求分析课题数据",
                    _Json = EasyUIJson.GetEasyUIJson_Subject(subjectList)
                });
            }
        }

        private ActionResult GetDiaoYanHistoryData(CCFlowArgs args)
        {
            List<Process_DiaoYan> diaoyanList = this.SrService.DiaoYanService.GetDiaoYanHistoryDataFromTrack(args);

            if (diaoyanList == null || diaoyanList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载调研数据",
                    _Json = EasyUIJson.GetEasyUIJson_DiaoYan(diaoyanList)
                });
            }
        }

        private ActionResult GetTiChuWenTiHistoryData(CCFlowArgs args)
        {
            List<Process_TiChuWenTi> wentiList = this.SrService.TiChuWenTiService.GetTiChuWenTiHistoryDataFromTrack(args);

            if (wentiList == null || wentiList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载提出问题数据",
                    _Json = EasyUIJson.GetEasyUIJson_TiChuWenTi(wentiList)
                });
            }
        }

        private ActionResult GetJieJueSiLuHistoryData(CCFlowArgs args)
        {
            List<Process_SiLu> siluList = this.SrService.JieJueSiLuService.GetJieJueSiLuHistoryDataFromTrack(args);

            if (siluList == null || siluList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载解决思路数据",
                    _Json = EasyUIJson.GetEasyUIJson_JieJueSiLu(siluList)
                });
            }
        }

        private ActionResult GetXingShiHuaHistoryData(CCFlowArgs args)
        {
            List<Process_XingShiHua> xshList = this.SrService.XingShiHuaService.GetXingShiHuaHistoryDataFromTrack(args);

            if (xshList == null || xshList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载形式化数据",
                    _Json = EasyUIJson.GetEasyUIJson_XingShiHua(xshList)
                });
            }
        }

        private ActionResult GetSheJiSuanFaHistoryData(CCFlowArgs args)
        {
            List<Process_SuanFa> sfList = this.SrService.SheJiSuanFaService.GetSuanFaHistoryDataFromTrack(args);

            if (sfList == null || sfList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载算法数据",
                    _Json = EasyUIJson.GetEasyUIJson_SuanFa(sfList)
                });
            }
        }

        private ActionResult GetSheJiShiYanHistoryData(CCFlowArgs args)
        {
            List<Process_ShiYan> syList = this.SrService.SheJiShiYanService.GetShiYanHistoryDataFromTrack(args);

            if (syList == null || syList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载实验数据",
                    _Json = EasyUIJson.GetEasyUIJson_ShiYan(syList)
                });
            }
        }

        private ActionResult GetDuiBiFenXiHistoryData(CCFlowArgs args)
        {
            List<Process_DuiBiFenXi> dbfxList = this.SrService.DuiBiFenXiService.GetDuiBiFenXiHistoryDataFromTrack(args);

            if (dbfxList == null || dbfxList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载量化对比分析数据",
                    _Json = EasyUIJson.GetEasyUIJson_DuiBiFenXi(dbfxList)
                });
            }
        }

        private ActionResult GetDeChuJieLunHistoryData(CCFlowArgs args)
        {
            List<Process_DeChuJieLun> dcjlList = this.SrService.DeChuJieLunService.GetJieLunHistoryDataFromTrack(args);

            if (dcjlList == null || dcjlList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载得出结论数据",
                    _Json = EasyUIJson.GetEasyUIJson_DeChuJieLun(dcjlList)
                });
            }
        }

        private ActionResult GetLunWenZhuanXieHistoryData(CCFlowArgs args)
        {
            List<Process_LunWen> lwList = this.SrService.LunWenZhuanXieService.GetLunWenHistoryDataFromTrack(args);

            if (lwList == null || lwList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "历史数据为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载论文撰写数据",
                    _Json = EasyUIJson.GetEasyUIJson_LunWen(lwList)
                });
            }
        }

        private bool JieShu(FlowOver args)
        {
            bool flag = true;
            try
            {
                string returnStr = this.SrService.CommonOperationService.DoOverFlow(args);
                SendMessageToBroswer(returnStr);
            }
            catch (Exception ex)
            {
                flag = false;
                this.Response.Write("<br><br><font color=red>结束流程期间出现异常:" + ex.Message + "</font>");
            }
            return flag;
        }

        private void SendMessageToBroswer(string returnInfo)
        {
            returnInfo = returnInfo.Replace("\t\n", "<br>@");
            returnInfo = returnInfo.Replace("@", "<br>@");
            this.Response.Write("<font color=blue>" + returnInfo + "</font>");
        }
    }
}