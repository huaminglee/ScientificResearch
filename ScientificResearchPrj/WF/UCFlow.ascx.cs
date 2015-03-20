namespace BP.Web.WF
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using BP.WF;
	using BP.DA;
	using BP.En;
    using BP.WF.Data;
    using BP.Port;
	/// <summary>
	///		UCFLow 的摘要说明。
	/// </summary>
	public partial class UCFlow : UC.UCBase
	{
        public void AddHtml(string html)
        {
            this.Text += html;
        }
		public void StartFlow()
		{
			//this.Add(PubClass.GenerLabelStr("业务发起"));
			this.AddTable();

			this.Add("<TR class='TableFlowTR'>");
			this.Add("<TD class='Title' colspan='7' background='./Images/Title_Start.bmp'><b><font color=yellow >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+WebUser.Name+"--工作发起</font></b></TD>");
			this.Add("</TR>");

			this.AddTR();
			//this.AddTDTitle("序号");
//			this.AddTDTitle("类别");
//			this.AddTDTitle("名称");
//			this.AddTDTitle("流程图");
//			this.AddTDTitle("说明");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>序号</TD>");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>流程类别</TD>");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>流程名称</TD>");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>流程图</TD>");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>流程说明</TD>");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>工作流向查询</TD>");
			this.AddTREnd();

			Nodes nds = new Nodes();
			nds.RetrieveStartNode();
			int i = 0 ;
			foreach(Node  nd in nds)
			{
				i++;
                this.AddTRTX("title='点击进入流程' style='cursor: hand; ' "); //("onmouseover='TROver(this)' onmouseout='TROut(this)'");
				//this.AddTDIdx(i);
				this.Add("<TD background='./Images/TdIdx_background.bmp' align='center'>"+i+"</TD> ");
				this.AddTD(nd.HisFlow.FK_FlowSortText);

				if (nd.FK_Flow=="220")
				{
					this.AddTD("onclick=\"window.open( '../../CT/CT/Port/Home.htm' , 'ct',  'width=700,top=100,left=200,height=400,scrollbars=yes,resizable=yes,toolbar=false,location=false')\"  ","<font color=blue>"+nd.FlowName+"</font>" );
				}
				else
				{
					this.AddTD("onclick=\"window.open( 'MyFlow.aspx?FK_Flow="+nd.FK_Flow+"&IsClose=1' , 'f"+nd.NodeID+"',  'width=700,top=100,left=200,height=400,scrollbars=yes,resizable=yes,toolbar=false,location=false')\"  ","<font color=blue>"+nd.FlowName+"</font>" );
				}

				//this.AddTD("<a href=\"javascript:WinOpen('MyFlow.aspx?FK_Flow="+nd.FK_Flow+"&IsClose=1',  'hsd');\"  >"+nd.FlowName+"</a>");
                this.AddTD("<a href=\"javascript:WinOpen('Chart.aspx?FK_Flow=" + nd.FK_Flow + "&DoType=Chart','sd');\"  >" + "打开" + "</a>");

				this.AddTD(nd.HisFlow.Note );
                this.AddTD("  onclick=\"window.open( 'UnComplateFlow.aspx?FK_Flow=" + nd.FK_Flow + "&FK_Emp=" + Web.WebUser.No + "&IsClose=1' , 'f" + nd.NodeID + "',  'width=550,top=200,left=300,height=300,scrollbars=yes,resizable=no,toolbar=false,location=false')\"  ", "<font color=blue>工作查询</font>");
				this.AddTREnd();
			}
			this.AddTableEnd();
		}
		public void BindStationTypeFlow(string flow, FlowShowType fst )
		{
			Flow fl = new Flow(flow);
			//this.Text="<img src='../TA/Images/Today.ico' borer=0 /><a href='../TA/ToDay.aspx'>主页</a><img src='../TA/Images/Flow.ico' borer=0 /><img src='../TA/Images/Flow.ico' borer=0 /><a href='MyWork.aspx'>当前工作</a>=>"+fl.Name+"<br>";
            this.Text = "<img src='../TA/Images/Flow.ico' borer=0 /><img src='../TA/Images/Flow.ico' borer=0 /><a href='MyWork.aspx'>当前工作</a>=>" + fl.Name + "<br>";
			this.Text+="<TABLE class='TableST'  id='strs'  cellSpacing='0' cellPadding='0' >";
			this.Text+="<TR>";
			switch( fst)
			{
				case FlowShowType.MyWorks:
                    this.Text += "<TD class='SelectedTitle' onclick=\"FlowShowType('" + flow + "','0');\" >当前工作</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','1');\" >新建</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','2');\"  >工作步骤</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','3');\" >流程图</TD>";
					break;
				case FlowShowType.WorkNew:
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','0');\" >当前工作</TD>";
					this.Text+="<TD class='SelectedTitle'  onclick=\"FlowShowType('"+flow+"','1');\" >新建</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','2');\"  >工作步骤</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','3');\" >流程图</TD>";
					break;
				case FlowShowType.WorkStep:
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','0');\" >当前工作</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','1');\" >新建</TD>";
					this.Text+="<TD class='SelectedTitle' onclick=\"FlowShowType('"+flow+"','2');\"  >工作步骤</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','3');\" >流程图</TD>";
					break;
				case FlowShowType.WorkImages:
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','0');\" >当前工作</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','1');\" >新建</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','2');\"  >工作步骤</TD>";
					this.Text+="<TD class='SelectedTitle' onclick=\"FlowShowType('"+flow+"','3');\" >流程图</TD>";
					break;
			}
			this.Text+="</TR>";

			this.Text+="<TR>";
			this.Text+=" <TD colspan='4' >";
			
			switch( fst)
			{
				case FlowShowType.MyWorks:
					string sql="SELECT a.FK_Node, c.Name as NodeName, b.WorkID, b.Title,b.Rec,b.RDT FROM WF_GenerWorkerlist a,  WF_GENERWORKFLOW b, WF_Node c";
					sql+=" WHERE a.FK_Node=c.NodeID AND a.WORKID=b.WorkID AND a.FK_Node=b.FK_Node AND b.FK_Flow='"+flow+"' AND a.FK_Emp='"+WebUser.No+"'";
					DataTable dt =DBAccess.RunSQLReturnTable(sql);
					this.Text+="<TABLE class='TableFlow'   >";
					this.Text+="<TR class='TableFlowTR' >";
					this.Text+="<TD class='TableFlowTD' >节点名称</TD>";
					this.Text+="<TD class='TableFlowTD' >标题</TD>";
					this.Text+="<TD class='TableFlowTD' >记录人</TD>";
					this.Text+="<TD class='TableFlowTD' >记录时间</TD>";
					this.Text+="</TR>";
					foreach(DataRow dr in dt.Rows)
					{
						this.Text+="<TR class='TableFlowTR' ondblclick=\" OpenWork('"+flow+"', '"+dr["WorkID"].ToString()+"' )  \"  onmouseover='FlowTROver(this)' onmouseout='FlowTROut(this)' >";
						this.Text+="<TD >"+dr["NodeName"].ToString()+"</TD>";
						this.Text+="<TD >"+dr["Title"].ToString()+"</TD>";
						this.Text+="<TD >"+dr["Rec"].ToString()+"</TD>";
						this.Text+="<TD >"+dr["RDT"].ToString()+"</TD>";
						this.Text+="</TR>";
					}		 
					this.Text+="</TABLE>";
					break;
				case FlowShowType.WorkImages:
					this.Text+="<img src='/DataUser/FlowDesc/"+flow+".gif' border=1 />";
					break;
				case FlowShowType.WorkNew:
					break;
				case FlowShowType.WorkStep:
					break;
			}

			this.Text+=" </TD>";		
			this.Text+="</TR>";
			this.Text+="</TABLE>";
			this.ParseControl();

		}
		
		public void BindBill(Bill bk)
		{

		}
		public void BindMyWork(int FK_Node)
		{
			Node nd = new Node(FK_Node);
			//Works wks = nd.HisWorks;
			//Work wk =wks.GetNewEntity;
            GenerWorkFlows gwfs = new GenerWorkFlows();
			QueryObject qo = new QueryObject(gwfs);
			qo.AddWhereInSQL(GenerWorkFlowAttr.WorkID , "SELECT WorkID FROM WF_GenerWorkerlist WHERE FK_Node="+FK_Node+" AND FK_Emp='"+WebUser.No+"'");
			qo.DoQuery();

			this._Text="<table class='MyWork' id='strs'  cellSpacing='0' cellPadding='0' >";
			this.Text+="<TR>";
			this.Text+="<TD class='MyWorkTitle' nowrap >标题</TD>";
			this.Text+="<TD class='MyWorkTitle' nowrap >记录时间</TD>";
			this.Text+="<TD class='MyWorkTitle' nowrap >记录人</TD>";
			this.Text+="</TR>";

            foreach (GenerWorkFlow gwf in gwfs)
			{
				this.Text+="<TR class='MyWorkTR' >";
				this.Text+="<TD class='MyWorkTD' nowrap  ><a href='FlowSV2.aspx?FK_Flow="+gwf.FK_Flow+"&WorkID="+gwf.WorkID+"&FK_Node="+gwf.FK_Node+"' >"+gwf.Title+"</a></TD>";
				this.Text+="<TD class='MyWorkTD' nowrap >"+gwf.RDT+"</TD>";
                this.Text += "<TD class='MyWorkTD' nowrap >" + gwf.Starter + "</TD>";
				this.Text+="<TR>";
			}

			this.Text+="</TABLE>";	   
			this.ParseControl();
		}
		public void BindWorkList11(Flow fl)
		{
			StartWorks sw = (StartWorks)fl.HisStartNode.HisWorks;
			DataTable dt=	sw.RetrieveMyTaskV2(fl.No);
			if (dt.Rows.Count==1)
			{
				this.Response.Redirect("MyFlow.aspx?FK_Flow="+fl.No+"&WorkID="+dt.Rows[0][WorkAttr.OID ].ToString(),true);
				return;
			}

			this.AddTable();
			this.AddTR();
			this.AddTDTitle("序号");
			this.AddTDTitle("标题");
			this.AddTDTitle("停留节点");
			this.AddTDTitle("发起日期");
			this.AddTDTitle("记录人");
			this.AddTDTitle("WorkID");
			this.AddTREnd();

			int i = 0;
			foreach(DataRow dr in dt.Rows)
			{
				i++;
				//this.AddTR("onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"WinOpen('MyFlow.aspx?FK_Flow="+fl.No+"&WorkID="+dr[WorkAttr.OID ].ToString()+"' )\" " );
				this.AddTR("title='在列表中根据标题选择您的代办工作' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"javascript:window.open('MyFlow.aspx?FK_Flow="+fl.No+"&WorkID="+dr[WorkAttr.OID ].ToString()+"','sdsd','width=700,top=100,left=200,height=400,scrollbars=yes,resizable=yes,toolbar=false,location=false')\" " );
				this.AddTDIdx( i );
				this.AddTD(dr[StartWorkAttr.Title].ToString() );

				this.AddTD(dr[StartWorkAttr.Title].ToString() );

				this.AddTD(dr[StartWorkAttr.RDT].ToString() );
				this.AddTD(dr[StartWorkAttr.Rec].ToString() );
				this.AddTD(dr[StartWorkAttr.OID].ToString() );
				this.AddTREnd();
			}
			this.AddTableEnd();
		}
		 
		public void BindMyWork()
		{
			Stations sts = WebUser.HisEmp.HisStations;

			//this._Text="<table class='MyWork' id='strs'  >";
			this._Text="<table class='MyWork' id='strs'  cellSpacing='0' cellPadding='0' >";
			this.Text+="<TR>";
			this.Text+="<TD class='MyWorkTitle' >岗位</TD>";
			this.Text+="<TD class='MyWorkTitle' >流程</TD>";
			//this.Text+="<TD class='MyWorkTitle' >节点</TD>";
			//this.Text+="<TD class='MyWorkTitle' >活动</TD>";
		//	this.Text+="<TD class='MyWorkTitle' >流程图</TD>";
			this.Text+="</TR>";

			/// 查询出来， 我能启动的流程。
            BP.DA.Paras ps = new Paras();
			foreach(Station st in sts)
			{

				this.Text+="<TR class='MyWorkTR' onmouseover='TROver(this)' onmouseout='TROut(this)' >";
				this.Text+="<TD class='MyWorkTD' nowrap >"+st.Name+"</TD>";
				this.Text+="<TD class='MyWorkTD' >&nbsp;</TD>";
			//	this.Text+="<TD class='MyWorkTD' >&nbsp;</TD>";
			//	this.Text+="<TD class='MyWorkTD' >&nbsp;</TD>";

				//this.Text+="<TD class='MyWorkTD' >&nbsp;</TD>";
				this.Text+="</TR>";

                ps.Clear();

				Flows fls = new Flows();
				QueryObject qo = new QueryObject(fls);
              //  qo.AddWhere(FlowAttr.StartStation, st.No);
				qo.DoQuery();


				//DataTable mytable=DBAccess.RunSQLReturnTable( "SELECT distinct c.No FROM WF_NodeStation a, WF_Node b, WF_Flow c WHERE a.FK_Node=b.NodeID AND b.FK_Flow =c.No AND a.FK_Station in (SELECT FK_Station FROM Port_EmpStation WHERE FK_Emp ='"+WebUser.No+"' AND FK_Station='"+st.No+"')" );
				foreach(Flow fl in fls)
				{
					/* 看看有没有此流程的工作 */
					this.Text+="<TR class='MyWorkTR' onmouseover='TROver(this)' onmouseout='TROut(this)' >";
					this.Text+=" <TD class='MyWorkTD' nowrap >&nbsp;</TD>";
					this.Text+=" <TD class='MyWorkTD' nowrap >"+fl.Name+"</TD>";
					//this.Text+=" <TD class='MyWorkTD' nowrap >&nbsp;</TD>";
					//this.Text+=" <TD class='MyWorkTD' nowrap ><a href=\"javascript:WinOpen( 'FlowSV2.aspx?FK_Flow="+fl.No+"' )\" >新建</a>  <a href=\"../DataUser/FlowDesc/"+fl.No+".gif\" target='flowP' >流程图</a> </TD>";
					this.Text+=" <TD class='MyWorkTD' nowrap ><a href=\"javascript:WinOpen( 'FlowSV2.aspx?FK_Flow="+fl.No+"' )\" >新建</a>  </TD>";

					this.Text+="</TR>";

                    ps.Clear();
                    ps.SQL = "SELECT FK_Node, COUNT(*) FROM WF_GenerWorkerlist WHERE FK_Emp=:FK_Emp AND FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow=:FK_Flow ) GROUP BY FK_Node";
                    ps.Add("FK_Emp",WebUser.No);
                    ps.Add("FK_Flow", fl.No);

				 
					DataTable dt=DBAccess.RunSQLReturnTable(ps);
					/*说明在此工作下面有工作要做。*/
					foreach(DataRow dr in dt.Rows)
					{
						this.Text+="<TR class='MyWorkTR' onmouseover='TROver(this)' onmouseout='TROut(this)' >";
						this.Text+=" <TD class='MyWorkTD' nowrap >&nbsp;</TD>";
						this.Text+=" <TD class='MyWorkTD' nowrap >&nbsp;</TD>";
						Node nd = new Node(int.Parse(dr[0].ToString())) ; 
						this.Text+=" <TD class='MyWorkTD' nowrap ><a href=\"javascript:WinOpen( 'MyWork.aspx?FK_Node="+nd.NodeID+"' )\" > "+nd.Name+"("+dr[1].ToString()+")"+"</a></TD>";
						this.Text+=" <TD class='MyWorkTD' nowrap >&nbsp;</TD>";

						//this.Text+=" <TD class='MyWorkTD' nowrap ><a href=''>操作</a>&nbsp;<a href='' >新建</a>  <a href='' >流程图</a> </TD>";
						this.Text+="</TR>";
					}
				}
			}
			this.Text+="</TABLE>";
			this.ParseControl();
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{
			 
		}

		#region Web 窗体设计器生成的代码
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		设计器支持所需的方法 - 不要使用代码编辑器
		///		修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
