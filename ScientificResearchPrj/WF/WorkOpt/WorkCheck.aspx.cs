using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.DA;
using BP.WF.Template;
using BP.Sys;

namespace CCFlow.WF.WorkOpt
{
    public partial class FrmWorkCheck : System.Web.UI.Page
    {
        #region 属性
        public bool IsHidden
        {
            get
            {
                try
                {
                    if (DoType == "View")
                        return true;
                    return bool.Parse(Request["IsHidden"]);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        public int NodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                string workid = this.Request.QueryString["OID"];
                if (workid == null)
                    workid = this.Request.QueryString["WorkID"];
                return Int64.Parse(workid);
            }
        }
        public Int64 FID
        {
            get
            {
                string workid = this.Request.QueryString["FID"];
                if (string.IsNullOrEmpty(workid) == true)
                    return 0;
                return Int64.Parse(workid);
            }
        }

        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }

        /// <summary>
        /// 操作View
        /// </summary>
        public string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        /// <summary>
        /// 是否是抄送.
        /// </summary>
        public bool IsCC
        {
            get
            {
                string s = this.Request.QueryString["Paras"];
                if (s == null)
                    return false;

                if (s.Contains("IsCC") == true)
                    return true;
                return false;
            }
        }


        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            //工作流编号不存在绑定空框架.
            if (this.FK_Flow == null)
            {
                ViewEmptyForm();
                return;
            }

            //审批节点.
            BP.Sys.FrmWorkCheck wcDesc = new BP.Sys.FrmWorkCheck(this.NodeID);
            if (wcDesc.HisFrmWorkShowModel == BP.Sys.FrmWorkShowModel.Free)
                this.BindFreeModel(wcDesc);
            else
                this.BindFreeModel(wcDesc);

            



            // this.BindTableModel(wcDesc);
        }
        /// <summary>
        /// 实现的功能：
        /// 1，显示轨迹表。
        /// 2，如果启用了审核，就把审核信息显示出来。
        /// 3，如果启用了抄送，就把抄送的人显示出来。
        /// 4，可以把子流程的信息与处理的结果显示出来。
        /// 5，可以把子线程的信息列出来。
        /// 6，可以把未来到达节点处理人显示出来。
        /// </summary>
        /// <param name="wcDesc"></param>
        public void BindFreeModel(BP.Sys.FrmWorkCheck wcDesc)
        {
            BP.WF.WorkCheck wc = null;
            if (FID != 0)
                wc = new WorkCheck(this.FK_Flow, this.NodeID, this.FID, 0);
            else
                wc = new WorkCheck(this.FK_Flow, this.NodeID, this.WorkID, this.FID);

            bool isCanDo = BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Flow, this.NodeID, this.WorkID,
                BP.Web.WebUser.No);

            #region 处理审核意见框.
            if (IsHidden == false && wcDesc.HisFrmWorkCheckSta == BP.Sys.FrmWorkCheckSta.Enable && isCanDo)
            {
                this.Pub1.AddTable("border=0 style='padding:0px;width:100%;' leftMargin=0 topMargin=0");
                this.Pub1.AddTR();
                this.Pub1.AddTDTitle("<div style='float:left'>" + wcDesc.FWCOpLabel + "</div><div style='float:right'><a href=javascript:TBHelp('TB_Doc')><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Emps.gif' align='middle' border=0 />选择词汇</a>&nbsp;&nbsp;</div>");
                this.Pub1.AddTREnd();

                PostBackTextBox tb = new PostBackTextBox();
                tb.ID = "TB_Doc";
                tb.TextMode = TextBoxMode.MultiLine;
                tb.OnBlur += new EventHandler(btn_Click);

                tb.Style["width"] = "100%";
                tb.Rows = 3;
                if (DoType != null && DoType == "View")
                {
                    tb.ReadOnly = true;
                }

                tb.Text = BP.WF.Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.NodeID);

                if (tb.Text == "同意")
                    tb.Text = "";

                if (string.IsNullOrEmpty(tb.Text))
                {
                    tb.Text = wcDesc.FWCDefInfo;

                    // 以下手机端都不要去处理
                    if (this.IsCC)
                    {
                        /*如果当前工作是抄送. */
                        BP.WF.Dev2Interface.WriteTrackWorkCheck(this.FK_Flow, this.NodeID, this.WorkID, this.FID, tb.Text, "抄送");

                        //设置当前已经审核完成.
                        BP.WF.Dev2Interface.Node_CC_SetSta(this.NodeID, this.WorkID, BP.Web.WebUser.No, CCSta.CheckOver);

                    }
                    else
                    {
                        if (wcDesc.FWCIsFullInfo == true)
                            BP.WF.Dev2Interface.WriteTrackWorkCheck(this.FK_Flow, this.NodeID, this.WorkID, this.FID, tb.Text, wcDesc.FWCOpLabel);
                    }
                    // 以上手机端都不要去处理.

                }
                this.Pub1.AddTR();
                this.Pub1.AddTD(tb);
                this.Pub1.AddTREnd();
                this.Pub1.AddTableEnd();
            }

            if (wcDesc.FWCListEnable == false)
                return;  /*  历史审核信息是否显示? 不显示就return. */

            #endregion 处理审核意见框.

            //求轨迹表.
            BP.WF.Tracks tks = wc.HisWorkChecks;

            //求抄送列表,把抄送的信息与抄送的读取状态显示出来.
            CCLists ccls = new CCLists(this.FK_Flow, this.WorkID, this.FID);

            //查询出来未来节点处理人信息,以方便显示未来没有运动到节点轨迹.
            Int64 wfid = this.WorkID;
            if (this.FID != 0)
                wfid = this.FID;

            //获得 节点处理人数据。
            SelectAccpers accepts = new SelectAccpers(wfid);

            //取出来该流程的所有的节点。
            Nodes nds = new Nodes(this.FK_Flow);
            Nodes ndsOrder = new Nodes();
            //求出已经出现的步骤.
            string nodes = ""; //已经出现的步骤.

            foreach (BP.WF.Track tk in tks)
            {
                switch (tk.HisActionType)
                {
                    case ActionType.Forward:
                    case ActionType.WorkCheck:
                        if (nodes.Contains(tk.NDFrom + ",") == false)
                        {
                            //ndsOrder.AddEntity(nds.GetEntityByKey(tk.NDFrom));
                            nodes += tk.NDFrom + ",";
                        }
                        break;
                    case ActionType.StartChildenFlow:
                        if (nodes.Contains(tk.NDFrom + ",") == false)
                        {
                            //ndsOrder.AddEntity(nds.GetEntityByKey(tk.NDFrom));
                            nodes += tk.NDFrom + ",";
                        }
                        break;
                    default:
                        continue;
                }
            }
            this.Pub1.AddTable("border=0 style='padding:0px;width:100%;' leftMargin=0 topMargin=0");

            int biaoji = 0;
            foreach (Node nd in nds)
            {
                if (nodes.Contains(nd.NodeID + ",") == true)
                {
                    /*已经处理过..*/
                    this.Pub1.AddTR();

                    this.Pub1.AddTDBegin("colspan=4");
                    this.Pub1.AddTable("border=0 style='padding:0px;width:100%;' leftMargin=0 topMargin=0 id='tb" + nd.NodeID + "'");

                    /*未出现的节点.*/
                    this.Pub1.AddTR();
                    this.Pub1.AddTDTitle("colspan=4", "<div style='float:left'><image src='../Img/Tree/Cut.gif' onclick=\"show_and_hide_tr('tb" + nd.NodeID + "',this);\" style='cursor:pointer;'></image>" + nd.Name + "</div>");
                    this.Pub1.AddTREnd();

                    this.Pub1.AddTR("style='font-size:14px;font-weight:bold;'");
                    this.Pub1.AddTD("width='50' style='font-weight:bold;'", "事件");
                    this.Pub1.AddTD("width='150' style='font-weight:bold;'", "时间");
                    this.Pub1.AddTD("width='100' style='font-weight:bold;'", "操作员");
                    this.Pub1.AddTD("style='font-weight:bold;'", "信息");
                    this.Pub1.AddTREnd();

                    //输出发送审核信息与抄送信息.
                    string emps = "";
                    string empsorder = "";    //保存队列显示中的人员，做判断，避免重复显示
                    string empcheck = "";   //记录当前节点已经输出的

                    foreach (Track tk in tks)
                    {
                        if (tk.NDFrom != nd.NodeID)
                            continue;

                        #region 如果是前进，并且当前节点没有启用审核组件
                        if (tk.HisActionType == ActionType.Forward)
                        {
                            BP.Sys.FrmWorkCheck fwc = new BP.Sys.FrmWorkCheck(nd.NodeID);
                            if (fwc.HisFrmWorkCheckSta == BP.Sys.FrmWorkCheckSta.Disable)
                            {
                                this.Pub1.AddTR();
                                this.Pub1.AddTD("<img src='../Img/Mail_Read.png' border=0 />" + tk.ActionTypeText);
                                this.Pub1.AddTD(tk.RDT);

                                if (wcDesc.SigantureEnabel == true)
                                    this.Pub1.AddTD(BP.WF.Glo.GenerUserSigantureHtml(Server.MapPath(""), tk.EmpFrom, tk.EmpFromT));
                                else
                                    this.Pub1.AddTD(BP.WF.Glo.GenerUserImgSmallerHtml(tk.EmpFrom, tk.EmpFromT));

                                this.Pub1.AddTD(tk.MsgHtml);
                                this.Pub1.AddTREnd();
                                continue;
                            }
                        }
                        #endregion

                        if (tk.HisActionType != ActionType.WorkCheck && tk.HisActionType != ActionType.StartChildenFlow)
                            continue;

                        emps += tk.EmpFrom + ",";

                        if (tk.HisActionType == ActionType.WorkCheck)
                        {
                            #region 显示出来队列流程中未审核的那些人.
                            if (nd.TodolistModel == TodolistModel.Order)
                            {
                                /* 如果是队列流程就要显示出来未审核的那些人.*/
                                string empsNodeOrder = "";  //记录当前节点队列访问未执行的人员

                                GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID);
                                foreach (GenerWorkerList item in gwls)
                                {
                                    if (item.FK_Node == nd.NodeID)
                                    {
                                        empsNodeOrder += item.FK_Emp;
                                    }
                                }

                                foreach (SelectAccper accper in accepts)
                                {
                                    if (empsorder.Contains(accper.FK_Emp) == true)
                                        continue;
                                    if (empsNodeOrder.Contains(accper.FK_Emp) == false)
                                        continue;
                                    if (tk.EmpFrom == accper.FK_Emp)
                                    {
                                        /*审核信息,首先输出它.*/
                                        this.Pub1.AddTR();
                                        this.Pub1.AddTD("<img src='../Img/Mail_Read.png' border=0/>" + tk.ActionTypeText);
                                        this.Pub1.AddTD(tk.RDT);
                                        //this.Pub1.AddTD(tk.EmpFromT);
                                        this.Pub1.AddTD(BP.WF.Glo.GenerUserImgSmallerHtml(tk.EmpFrom, tk.EmpFromT));
                                        this.Pub1.AddTD(tk.MsgHtml);
                                        this.Pub1.AddTREnd();
                                        empcheck += tk.EmpFrom;
                                    }
                                    else
                                    {
                                        this.Pub1.AddTR();
                                        if (accper.AccType == 0)
                                            this.Pub1.AddTD("style='color:Red;'", "执行");
                                        else
                                            this.Pub1.AddTD("style='color:Red;'", "抄送");
                                        this.Pub1.AddTD("style='color:Red;'", "无");
                                        this.Pub1.AddTD("style='color:Red;'", BP.WF.Glo.GenerUserImgSmallerHtml(accper.FK_Emp, accper.EmpName));
                                        this.Pub1.AddTD("style='color:Red;'", accper.Info);
                                        this.Pub1.AddTREnd();
                                        empsorder += accper.FK_Emp;
                                    }
                                }
                            }
                            #endregion 显示出来队列流程中未审核的那些人.
                            else
                            {
                                /*审核信息,首先输出它.*/
                                this.Pub1.AddTR();
                                this.Pub1.AddTD("<img src='../Img/Mail_Read.png' border=0/>" + tk.ActionTypeText);
                                this.Pub1.AddTD(tk.RDT);
                                //this.Pub1.AddTD(tk.EmpFromT);

                                if (wcDesc.SigantureEnabel == true)
                                    this.Pub1.AddTD(BP.WF.Glo.GenerUserSigantureHtml(Server.MapPath("../../"), tk.EmpFrom, tk.EmpFromT));
                                else
                                    this.Pub1.AddTD(BP.WF.Glo.GenerUserImgSmallerHtml(tk.EmpFrom, tk.EmpFromT));
                                this.Pub1.AddTDBigDoc(tk.MsgHtml);
                                this.Pub1.AddTREnd();
                                empcheck += tk.EmpFrom;
                            }
                        }

                        #region 检查是否有调用子流程的情况。如果有就输出调用子流程信息. (手机部分的翻译暂时不考虑).
                        // int atTmp = (int)ActionType.StartChildenFlow;
                        BP.WF.WorkCheck wc2 = new WorkCheck(FK_Flow, tk.NDFrom, tk.WorkID, tk.FID);
                        if (wc2.FID != 0)
                        {
                            //Tracks ztks = wc2.HisWorkChecks;    //重复循环！
                            //foreach (BP.WF.Track subTK in ztks)
                            //{
                            if (tk.HisActionType == ActionType.StartChildenFlow)
                            {
                                /*说明有子流程*/
                                /*如果是调用子流程,就要从参数里获取到都是调用了那个子流程，并把他们显示出来.*/
                                string[] paras = tk.Tag.Split('@');
                                string[] p1 = paras[1].Split('=');
                                string fk_flow = p1[1]; //子流程编号

                                string[] p2 = paras[2].Split('=');
                                string workId = p2[1]; //子流程ID.

                                BP.WF.WorkCheck subwc = new WorkCheck(fk_flow, int.Parse(fk_flow + "01"), Int64.Parse(workId), 0);

                                Tracks subtks = subwc.HisWorkChecks;

                                //取出来子流程的所有的节点。
                                Nodes subNds = new Nodes(fk_flow);
                                foreach (Node item in subNds)     //主要按顺序显示
                                {
                                    foreach (BP.WF.Track mysubtk in subtks)
                                    {
                                        if (item.NodeID != mysubtk.NDFrom)
                                            continue;
                                        /*输出该子流程的审核信息，应该考虑子流程的子流程信息, 就不考虑那样复杂了.*/
                                        if (mysubtk.HisActionType == ActionType.WorkCheck)
                                        {
                                            //biaojie  发起多个子流程时，发起人只显示一次
                                            if (mysubtk.NDFrom == int.Parse(fk_flow + "01") && biaoji == 1)
                                                continue;
                                            /*如果是审核.*/
                                            this.Pub1.AddTR();
                                            this.Pub1.AddTD(mysubtk.ActionTypeText + "<img src='../Img/Mail_Read.png' border=0/>");
                                            this.Pub1.AddTD(mysubtk.RDT);
                                            //this.Pub1.AddTD(subtk.EmpFromT);
                                            this.Pub1.AddTD(BP.WF.Glo.GenerUserImgSmallerHtml(mysubtk.EmpFrom, mysubtk.EmpFromT));
                                            this.Pub1.AddTDBigDoc(mysubtk.MsgHtml);
                                            this.Pub1.AddTREnd();
                                            if (mysubtk.NDFrom == int.Parse(fk_flow + "01"))
                                            {
                                                biaoji = 1;
                                            }
                                        }
                                    }
                                }

                            }
                            //}
                        }
                        #endregion 检查是否有调用子流程的情况。如果有就输出调用子流程信息.
                    }

                    foreach (SelectAccper item in accepts)
                    {
                        if (item.FK_Node != nd.NodeID)
                            continue;
                        if (empcheck.Contains(item.FK_Emp) == true)
                            continue;
                        if (item.AccType == 0)
                            continue;
                        if (ccls.IsExits(CCListAttr.FK_Node, nd.NodeID) == true)
                            continue;

                        this.Pub1.AddTR();
                        this.Pub1.AddTD("style='color:Red;'", "执行");
                        //else
                        //this.Pub1.AddTD("style='color:Red;'", "抄送");
                        this.Pub1.AddTD("style='color:Red;'", "无");

                        // 显示要执行的人员。
                        this.Pub1.AddTD("style='color:Red;'", BP.WF.Glo.GenerUserImgSmallerHtml(item.FK_Emp, item.EmpName));

                        //info.
                        this.Pub1.AddTD("style='color:Red;'", item.Info);
                        this.Pub1.AddTREnd();
                    }

                    #region 输出抄送
                    foreach (SelectAccper item in accepts)
                    {
                        if (item.FK_Node != nd.NodeID)
                            continue;
                        if (item.AccType != 1)
                            continue;
                        if (ccls.IsExits(CCListAttr.FK_Node, nd.NodeID) == false)
                        {
                            this.Pub1.AddTR();
                            this.Pub1.AddTD("style='color:Red;'", "抄送");
                            this.Pub1.AddTD("style='color:Red;'", "无");
                            // 显示要执行的人员。
                            this.Pub1.AddTD("style='color:Red;'", BP.WF.Glo.GenerUserImgSmallerHtml(item.FK_Emp, item.EmpName));

                            //info.
                            this.Pub1.AddTD("style='color:Red;'", item.Info);
                            this.Pub1.AddTREnd();
                        }
                        else
                        {
                            foreach (CCList cc in ccls)
                            {
                                if (cc.FK_Node != nd.NodeID)
                                    continue;

                                if (cc.HisSta == CCSta.CheckOver)
                                    continue;
                                if (cc.CCTo != item.FK_Emp)
                                    continue;

                                this.Pub1.AddTR();
                                if (cc.HisSta == CCSta.Read)
                                {
                                    if (nd.IsEndNode == true)
                                    {
                                        this.Pub1.AddTD("<img src='../Img/Mail_Read.png' border=0/>抄送已阅");
                                        this.Pub1.AddTD(cc.CDT); //读取时间.
                                        this.Pub1.AddTD(BP.WF.Glo.GenerUserImgSmallerHtml(cc.CCTo, cc.CCToName));
                                        this.Pub1.AddTD(cc.CheckNoteHtml);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (BP.Web.WebUser.No == cc.CCTo)
                                    {
                                        continue;

                                        /*如果打开的是我,*/
                                        if (cc.HisSta == CCSta.UnRead)
                                            BP.WF.Dev2Interface.Node_CC_SetRead(cc.MyPK);
                                        this.Pub1.AddTD("<img src='../Img/Mail_Read.png' border=0/>正在查阅");
                                    }
                                    else
                                    {
                                        this.Pub1.AddTD("<img src='../Img/Mail_UnRead.png' border=0/>抄送未阅");
                                    }

                                    this.Pub1.AddTD("无");
                                    this.Pub1.AddTD(BP.WF.Glo.GenerUserImgSmallerHtml(cc.CCTo, cc.CCToName));
                                    this.Pub1.AddTD("无");
                                }
                                this.Pub1.AddTREnd();
                            }
                        }
                    }
                    #endregion

                    this.Pub1.AddTableEnd();
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }
                else
                {
                    if (wcDesc.FWCIsShowAllStep == false)
                        continue;

                    /*判断该节点下是否有人访问，或者已经设置了抄送与接收人对象, 如果没有就不输出*/
                    if (accepts.IsExits(SelectAccperAttr.FK_Node, nd.NodeID) == false)
                        continue;

                    /*未出现的节点.*/
                    this.Pub1.AddTR();
                    this.Pub1.AddTDBegin("colspan=4");
                    this.Pub1.AddTable("border=0 style='padding:0px;width:100%;' leftMargin=0 topMargin=0 id='tb" + nd.NodeID + "'");

                    /*未出现的节点.*/
                    this.Pub1.AddTR();
                    this.Pub1.AddTDTitle("colspan=4", "<div style='float:left'><image src='../Img/Tree/Cut.gif' onclick=\"show_and_hide_tr('tb" + nd.NodeID + "',this);\" style='cursor:pointer;'></image>" + nd.Name + "</div>");
                    this.Pub1.AddTREnd();

                    this.Pub1.AddTR("style='font-size:14px;font-weight:bold;'");
                    this.Pub1.AddTD("width='50'", "事件");
                    this.Pub1.AddTD("width='150'", "时间");
                    this.Pub1.AddTD("width='100'", "操作员");
                    this.Pub1.AddTD("信息");
                    this.Pub1.AddTREnd();

                    //是否输出了.
                    bool isHaveIt = false;
                    foreach (SelectAccper item in accepts)
                    {
                        if (item.FK_Node != nd.NodeID)
                            continue;
                        if (item.AccType != 0)
                            continue;
                        this.Pub1.AddTR();
                        this.Pub1.AddTD("style='color:Red;'", "执行");
                        //else
                        //this.Pub1.AddTD("style='color:Red;'", "抄送");
                        this.Pub1.AddTD("style='color:Red;'", "无");

                        // 显示要执行的人员。
                        this.Pub1.AddTD("style='color:Red;'", BP.WF.Glo.GenerUserImgSmallerHtml(item.FK_Emp, item.EmpName));

                        //info.
                        this.Pub1.AddTD("style='color:Red;'", item.Info);
                        this.Pub1.AddTREnd();
                        isHaveIt = true;
                    }

                    #region 输出抄送
                    foreach (SelectAccper item in accepts)
                    {
                        if (item.FK_Node != nd.NodeID)
                            continue;
                        if (item.AccType != 1)
                            continue;
                        if (ccls.IsExits(CCListAttr.FK_Node, nd.NodeID) == false)
                        {
                            this.Pub1.AddTR();
                            this.Pub1.AddTD("style='color:Red;'", "抄送");
                            this.Pub1.AddTD("style='color:Red;'", "无");
                            // 显示要执行的人员。
                            this.Pub1.AddTD("style='color:Red;'", BP.WF.Glo.GenerUserImgSmallerHtml(item.FK_Emp, item.EmpName));

                            //info.
                            this.Pub1.AddTD("style='color:Red;'", item.Info);
                            this.Pub1.AddTREnd();
                            isHaveIt = true;
                        }
                        else
                        {
                            foreach (CCList cc in ccls)
                            {
                                if (cc.FK_Node != nd.NodeID)
                                    continue;

                                if (cc.HisSta == CCSta.CheckOver)
                                    continue;
                                if (cc.CCTo != item.FK_Emp)
                                    continue;

                                this.Pub1.AddTR();
                                if (cc.HisSta == CCSta.Read)
                                {
                                    if (nd.IsEndNode == true)
                                    {
                                        this.Pub1.AddTD("<img src='../Img/Mail_Read.png' border=0/>抄送已阅");
                                        this.Pub1.AddTD(cc.CDT); //读取时间.
                                        this.Pub1.AddTD(BP.WF.Glo.GenerUserImgSmallerHtml(cc.CCTo, cc.CCToName));
                                        this.Pub1.AddTD(cc.CheckNoteHtml);
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (BP.Web.WebUser.No == cc.CCTo)
                                    {
                                        continue;

                                        /*如果打开的是我,*/
                                        if (cc.HisSta == CCSta.UnRead)
                                            BP.WF.Dev2Interface.Node_CC_SetRead(cc.MyPK);
                                        this.Pub1.AddTD("<img src='../Img/Mail_Read.png' border=0/>正在查阅");
                                    }
                                    else
                                    {
                                        this.Pub1.AddTD("<img src='../Img/Mail_UnRead.png' border=0/>抄送未阅");
                                    }

                                    this.Pub1.AddTD("无");
                                    this.Pub1.AddTD(BP.WF.Glo.GenerUserImgSmallerHtml(cc.CCTo, cc.CCToName));
                                    this.Pub1.AddTD("无");
                                }
                                this.Pub1.AddTREnd();
                            }
                        }
                    }
                    #endregion

                    this.Pub1.AddTableEnd();
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }



            }
            this.Pub1.AddTableEnd();

        }
        public void BindFreeModelV1_del(BP.Sys.FrmWorkCheck wcDesc)
        {
            BP.WF.WorkCheck wc = new WorkCheck(this.FK_Flow, this.NodeID, this.WorkID, this.FID);
            this.Pub1.AddTable("border=0 style='padding:0px;width:100%;' leftMargin=0 topMargin=0");
            if (IsHidden == false && wcDesc.HisFrmWorkCheckSta == BP.Sys.FrmWorkCheckSta.Enable)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTD("<div style='float:right'><img src='/WF/Img/Btn/Save.gif' border=0 />保存</div>");
                this.Pub1.AddTREnd();

                PostBackTextBox tb = new PostBackTextBox();
                tb.ID = "TB_Doc";
                tb.TextMode = TextBoxMode.MultiLine;
                tb.OnBlur += new EventHandler(btn_Click);
                tb.Style["width"] = "100%";
                tb.Rows = 3;
                if (DoType != null && DoType == "View")
                    tb.ReadOnly = true;
                tb.Text = BP.WF.Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.NodeID);
                if (tb.Text == "")
                {
                    tb.Text = wcDesc.FWCDefInfo;
                    BP.WF.Dev2Interface.WriteTrackWorkCheck(this.FK_Flow, this.NodeID, this.WorkID, this.FID, tb.Text, wcDesc.FWCOpLabel);
                }

                this.Pub1.AddTR();
                this.Pub1.AddTD(tb);
                this.Pub1.AddTREnd();
            }

            if (wcDesc.FWCListEnable == false)
            {
                this.Pub1.AddTableEnd();
                return;
            }

            int i = 0;
            BP.WF.Tracks tks = wc.HisWorkChecks;
            foreach (BP.WF.Track tk in tks)
            {
                #region 输出审核.
                if (tk.HisActionType == ActionType.WorkCheck)
                {
                    /*如果是审核.*/
                    i++;
                    ActionType at = tk.HisActionType;
                    DateTime dtt = BP.DA.DataType.ParseSysDateTime2DateTime(tk.RDT);

                    this.Pub1.AddTR();
                    this.Pub1.AddTDBegin();
                    this.Pub1.AddB(tk.NDFromT);
                    this.Pub1.AddBR(tk.MsgHtml);
                    this.Pub1.AddBR("<div style='float:right'>" + BP.WF.Glo.GenerUserImgSmallerHtml(tk.EmpFrom, tk.EmpFromT) + " &nbsp;&nbsp;&nbsp; " + dtt.ToString("yy年MM月dd日HH时mm分") + "</div>");
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }
                #endregion 输出审核.

                #region 检查是否有子流程.
                if (tk.HisActionType == ActionType.StartChildenFlow)
                {
                    /*如果是调用子流程,就要从参数里获取到都是调用了那个子流程，并把他们显示出来.*/
                    string[] paras = tk.Tag.Split('@');
                    string[] p1 = paras[1].Split('=');
                    string fk_flow = p1[1];

                    string[] p2 = paras[2].Split('=');
                    string workId = p2[1];

                    BP.WF.WorkCheck subwc = new WorkCheck(fk_flow, int.Parse(fk_flow + "01"), Int64.Parse(workId), 0);
                    Tracks subtks = subwc.HisWorkChecks;
                    foreach (BP.WF.Track subtk in subtks)
                    {
                        if (subtk.HisActionType == ActionType.WorkCheck)
                        {
                            /*如果是审核.*/
                            i++;
                            ActionType at = subtk.HisActionType;
                            DateTime dtt = BP.DA.DataType.ParseSysDateTime2DateTime(subtk.RDT);

                            this.Pub1.AddTR();
                            this.Pub1.AddTDBegin();

                            this.Pub1.AddB(subtk.NDFromT);
                            this.Pub1.AddBR(subtk.MsgHtml);
                            this.Pub1.AddBR("<div style='float:right'>" + BP.WF.Glo.GenerUserImgSmallerHtml(subtk.EmpFrom, subtk.EmpFromT) + " &nbsp;&nbsp;&nbsp; " + dtt.ToString("yy年MM月dd日HH时mm分") + "</div>");

                            this.Pub1.AddTDEnd();
                            this.Pub1.AddTREnd();

                        }
                    }
                }
                #endregion 检查是否有子流程.

            }
            this.Pub1.AddTableEnd();
        }
        public void BindTableModel(BP.Sys.FrmWorkCheck wcDesc)
        {
            BP.WF.WorkCheck wc = new WorkCheck(this.FK_Flow, this.NodeID, this.WorkID, this.FID);

            this.Pub1.AddTable("border=1 style='padding:0px;width:100%;'");
            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=8", "<div style='float:left'>审批意见</div> <div style='float:right'><img src='../Img/Btn/Save.gif' border=0 /></div>");
            this.Pub1.AddTREnd();

            if (!IsHidden)
            {
                PostBackTextBox tb = new PostBackTextBox();
                tb.ID = "TB_Doc";
                tb.TextMode = TextBoxMode.MultiLine;
                tb.OnBlur += new EventHandler(btn_Click);
                tb.Style["width"] = "100%";
                tb.Rows = 3;
                if (DoType != null && DoType == "View") tb.ReadOnly = true;

                tb.Text = BP.WF.Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.NodeID);

                this.Pub1.AddTD("colspan=8", tb);
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTR();
            this.Pub1.AddTD("IDX");
            this.Pub1.AddTD("发生时间");
            this.Pub1.AddTD("发生节点");
            //   this.Pub1.AddTD("人员");
            this.Pub1.AddTD("活动");
            this.Pub1.AddTD("信息/审批意见");
            this.Pub1.AddTD("执行人");
            this.Pub1.AddTREnd();

            int i = 0;
            BP.WF.Tracks tks = wc.HisWorkChecks;
            foreach (BP.WF.Track tk in tks)
            {
                if (tk.HisActionType == ActionType.Forward
                    || tk.HisActionType == ActionType.ForwardFL
                    || tk.HisActionType == ActionType.ForwardHL
                    )
                {
                    string nd = tk.NDFrom.ToString();
                    if (nd.Substring(nd.Length - 2) != "01")
                        continue;
                    //string len=tk.NDFrom.ToString();
                    //if (
                    //if (tk.NDFrom.ToString().Contains
                }

                if (tk.HisActionType != ActionType.WorkCheck)
                    continue;

                i++;
                this.Pub1.AddTR();
                this.Pub1.AddTD(i);
                DateTime dtt = BP.DA.DataType.ParseSysDateTime2DateTime(tk.RDT);
                this.Pub1.AddTD(dtt.ToString("MM月dd日HH时mm分"));
                this.Pub1.AddTD(tk.NDFromT);
                //  this.Pub1.AddTD(tk.EmpFromT);
                ActionType at = tk.HisActionType;
                //this.Pub1.AddTD("<img src='./../Img/Action/" + at.ToString() + ".png' class='ActionType' width='16px' border=0/>" + BP.WF.Track.GetActionTypeT(at));
                this.Pub1.AddTD("<img src='./../Img/Action/" + at.ToString() + ".png' class='ActionType' width='16px' border=0/>" + tk.ActionTypeText);
                this.Pub1.AddTD(tk.MsgHtml);
                this.Pub1.AddTD(tk.Exer); //如果是委托，增加一个  人员(委托)
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }

        //展示空表单
        private void ViewEmptyForm()
        {
            this.Pub1.AddTable(" border=1 style='padding:0px;width:100%;'");
            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=6 style='text-align:left' ", "审批意见");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("IDX");
            this.Pub1.AddTD("发生时间");
            this.Pub1.AddTD("发生节点");
            this.Pub1.AddTD("活动");
            this.Pub1.AddTD("信息/审批意见");
            this.Pub1.AddTD("执行人");
            this.Pub1.AddTREnd();

            this.Pub1.AddTableEnd();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            //查看时取消保存
            if (DoType != null && DoType == "View")
                return;

            //内容为空，取消保存
            if (string.IsNullOrEmpty(this.Pub1.GetTextBoxByID("TB_Doc").Text.Trim())) return;
            // 加入审核信息.
            string msg = this.Pub1.GetTextBoxByID("TB_Doc").Text;

            BP.Sys.FrmWorkCheck wcDesc = new BP.Sys.FrmWorkCheck(this.NodeID);

            // 处理人大的需求，需要把审核意见写入到FlowNote里面去.
            string sql = "UPDATE WF_GenerWorkFlow SET FlowNote='" + msg + "' WHERE WorkID=" + this.WorkID;
            BP.DA.DBAccess.RunSQL(sql);

            // 判断是否是抄送?
            if (this.IsCC)
            {
                // 写入审核信息，有可能是update数据。
                BP.WF.Dev2Interface.WriteTrackWorkCheck(this.FK_Flow, this.NodeID, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);

                //设置抄送状态 - 已经审核完毕.
                BP.WF.Dev2Interface.Node_CC_SetSta(this.NodeID, this.WorkID, BP.Web.WebUser.No, CCSta.CheckOver);
            }
            else
            {
                BP.WF.Dev2Interface.WriteTrackWorkCheck(this.FK_Flow, this.NodeID, this.WorkID, this.FID, msg, wcDesc.FWCOpLabel);
            }

            this.Response.Redirect("WorkCheck.aspx?s=2&OID=" + this.WorkID + "&FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&Paras=" + this.Request.QueryString["Paras"], true);
            //执行审批.
            //BP.Sys.PubClass.Alert("保存成功...");

            //关闭窗口.
            //BP.Sys.PubClass.WinClose();
        }
    }

    //自定义控件
    public class PostBackTextBox : System.Web.UI.WebControls.TextBox, System.Web.UI.IPostBackEventHandler
    {
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            Attributes["onblur"] = Page.GetPostBackEventReference(this);
            base.Render(writer);
        }

        public event EventHandler OnBlur;

        public virtual void RaisePostBackEvent(string eventArgument)
        {
            if (OnBlur != null)
            {
                OnBlur(this, null);
            }
        }
    }
}