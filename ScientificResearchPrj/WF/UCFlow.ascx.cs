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
	///		UCFLow ��ժҪ˵����
	/// </summary>
	public partial class UCFlow : UC.UCBase
	{
        public void AddHtml(string html)
        {
            this.Text += html;
        }
		public void StartFlow()
		{
			//this.Add(PubClass.GenerLabelStr("ҵ����"));
			this.AddTable();

			this.Add("<TR class='TableFlowTR'>");
			this.Add("<TD class='Title' colspan='7' background='./Images/Title_Start.bmp'><b><font color=yellow >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+WebUser.Name+"--��������</font></b></TD>");
			this.Add("</TR>");

			this.AddTR();
			//this.AddTDTitle("���");
//			this.AddTDTitle("���");
//			this.AddTDTitle("����");
//			this.AddTDTitle("����ͼ");
//			this.AddTDTitle("˵��");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>���</TD>");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>�������</TD>");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>��������</TD>");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>����ͼ</TD>");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>����˵��</TD>");
			this.Add("<TD background='./Images/TdTop_background.bmp' align='center'>���������ѯ</TD>");
			this.AddTREnd();

			Nodes nds = new Nodes();
			nds.RetrieveStartNode();
			int i = 0 ;
			foreach(Node  nd in nds)
			{
				i++;
                this.AddTRTX("title='�����������' style='cursor: hand; ' "); //("onmouseover='TROver(this)' onmouseout='TROut(this)'");
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
                this.AddTD("<a href=\"javascript:WinOpen('Chart.aspx?FK_Flow=" + nd.FK_Flow + "&DoType=Chart','sd');\"  >" + "��" + "</a>");

				this.AddTD(nd.HisFlow.Note );
                this.AddTD("  onclick=\"window.open( 'UnComplateFlow.aspx?FK_Flow=" + nd.FK_Flow + "&FK_Emp=" + Web.WebUser.No + "&IsClose=1' , 'f" + nd.NodeID + "',  'width=550,top=200,left=300,height=300,scrollbars=yes,resizable=no,toolbar=false,location=false')\"  ", "<font color=blue>������ѯ</font>");
				this.AddTREnd();
			}
			this.AddTableEnd();
		}
		public void BindStationTypeFlow(string flow, FlowShowType fst )
		{
			Flow fl = new Flow(flow);
			//this.Text="<img src='../TA/Images/Today.ico' borer=0 /><a href='../TA/ToDay.aspx'>��ҳ</a><img src='../TA/Images/Flow.ico' borer=0 /><img src='../TA/Images/Flow.ico' borer=0 /><a href='MyWork.aspx'>��ǰ����</a>=>"+fl.Name+"<br>";
            this.Text = "<img src='../TA/Images/Flow.ico' borer=0 /><img src='../TA/Images/Flow.ico' borer=0 /><a href='MyWork.aspx'>��ǰ����</a>=>" + fl.Name + "<br>";
			this.Text+="<TABLE class='TableST'  id='strs'  cellSpacing='0' cellPadding='0' >";
			this.Text+="<TR>";
			switch( fst)
			{
				case FlowShowType.MyWorks:
                    this.Text += "<TD class='SelectedTitle' onclick=\"FlowShowType('" + flow + "','0');\" >��ǰ����</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','1');\" >�½�</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','2');\"  >��������</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','3');\" >����ͼ</TD>";
					break;
				case FlowShowType.WorkNew:
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','0');\" >��ǰ����</TD>";
					this.Text+="<TD class='SelectedTitle'  onclick=\"FlowShowType('"+flow+"','1');\" >�½�</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','2');\"  >��������</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','3');\" >����ͼ</TD>";
					break;
				case FlowShowType.WorkStep:
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','0');\" >��ǰ����</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','1');\" >�½�</TD>";
					this.Text+="<TD class='SelectedTitle' onclick=\"FlowShowType('"+flow+"','2');\"  >��������</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','3');\" >����ͼ</TD>";
					break;
				case FlowShowType.WorkImages:
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','0');\" >��ǰ����</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','1');\" >�½�</TD>";
					this.Text+="<TD class='Title' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"FlowShowType('"+flow+"','2');\"  >��������</TD>";
					this.Text+="<TD class='SelectedTitle' onclick=\"FlowShowType('"+flow+"','3');\" >����ͼ</TD>";
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
					this.Text+="<TD class='TableFlowTD' >�ڵ�����</TD>";
					this.Text+="<TD class='TableFlowTD' >����</TD>";
					this.Text+="<TD class='TableFlowTD' >��¼��</TD>";
					this.Text+="<TD class='TableFlowTD' >��¼ʱ��</TD>";
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
			this.Text+="<TD class='MyWorkTitle' nowrap >����</TD>";
			this.Text+="<TD class='MyWorkTitle' nowrap >��¼ʱ��</TD>";
			this.Text+="<TD class='MyWorkTitle' nowrap >��¼��</TD>";
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
			this.AddTDTitle("���");
			this.AddTDTitle("����");
			this.AddTDTitle("ͣ���ڵ�");
			this.AddTDTitle("��������");
			this.AddTDTitle("��¼��");
			this.AddTDTitle("WorkID");
			this.AddTREnd();

			int i = 0;
			foreach(DataRow dr in dt.Rows)
			{
				i++;
				//this.AddTR("onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"WinOpen('MyFlow.aspx?FK_Flow="+fl.No+"&WorkID="+dr[WorkAttr.OID ].ToString()+"' )\" " );
				this.AddTR("title='���б��и��ݱ���ѡ�����Ĵ��칤��' onmouseover='TROver(this)' onmouseout='TROut(this)' onclick=\"javascript:window.open('MyFlow.aspx?FK_Flow="+fl.No+"&WorkID="+dr[WorkAttr.OID ].ToString()+"','sdsd','width=700,top=100,left=200,height=400,scrollbars=yes,resizable=yes,toolbar=false,location=false')\" " );
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
			this.Text+="<TD class='MyWorkTitle' >��λ</TD>";
			this.Text+="<TD class='MyWorkTitle' >����</TD>";
			//this.Text+="<TD class='MyWorkTitle' >�ڵ�</TD>";
			//this.Text+="<TD class='MyWorkTitle' >�</TD>";
		//	this.Text+="<TD class='MyWorkTitle' >����ͼ</TD>";
			this.Text+="</TR>";

			/// ��ѯ������ �������������̡�
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
					/* ������û�д����̵Ĺ��� */
					this.Text+="<TR class='MyWorkTR' onmouseover='TROver(this)' onmouseout='TROut(this)' >";
					this.Text+=" <TD class='MyWorkTD' nowrap >&nbsp;</TD>";
					this.Text+=" <TD class='MyWorkTD' nowrap >"+fl.Name+"</TD>";
					//this.Text+=" <TD class='MyWorkTD' nowrap >&nbsp;</TD>";
					//this.Text+=" <TD class='MyWorkTD' nowrap ><a href=\"javascript:WinOpen( 'FlowSV2.aspx?FK_Flow="+fl.No+"' )\" >�½�</a>  <a href=\"../DataUser/FlowDesc/"+fl.No+".gif\" target='flowP' >����ͼ</a> </TD>";
					this.Text+=" <TD class='MyWorkTD' nowrap ><a href=\"javascript:WinOpen( 'FlowSV2.aspx?FK_Flow="+fl.No+"' )\" >�½�</a>  </TD>";

					this.Text+="</TR>";

                    ps.Clear();
                    ps.SQL = "SELECT FK_Node, COUNT(*) FROM WF_GenerWorkerlist WHERE FK_Emp=:FK_Emp AND FK_Node IN (SELECT NodeID FROM WF_Node WHERE FK_Flow=:FK_Flow ) GROUP BY FK_Node";
                    ps.Add("FK_Emp",WebUser.No);
                    ps.Add("FK_Flow", fl.No);

				 
					DataTable dt=DBAccess.RunSQLReturnTable(ps);
					/*˵���ڴ˹��������й���Ҫ����*/
					foreach(DataRow dr in dt.Rows)
					{
						this.Text+="<TR class='MyWorkTR' onmouseover='TROver(this)' onmouseout='TROut(this)' >";
						this.Text+=" <TD class='MyWorkTD' nowrap >&nbsp;</TD>";
						this.Text+=" <TD class='MyWorkTD' nowrap >&nbsp;</TD>";
						Node nd = new Node(int.Parse(dr[0].ToString())) ; 
						this.Text+=" <TD class='MyWorkTD' nowrap ><a href=\"javascript:WinOpen( 'MyWork.aspx?FK_Node="+nd.NodeID+"' )\" > "+nd.Name+"("+dr[1].ToString()+")"+"</a></TD>";
						this.Text+=" <TD class='MyWorkTD' nowrap >&nbsp;</TD>";

						//this.Text+=" <TD class='MyWorkTD' nowrap ><a href=''>����</a>&nbsp;<a href='' >�½�</a>  <a href='' >����ͼ</a> </TD>";
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

		#region Web ������������ɵĴ���
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: �õ����� ASP.NET Web ���������������ġ�
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		�����֧������ķ��� - ��Ҫʹ�ô���༭��
		///		�޸Ĵ˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
