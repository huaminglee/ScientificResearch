using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web;
using BP.Web.Controls;

namespace CCFlow.WF.CCForm
{
    public partial class FrmExcel : BP.Web.WebPage
    {
        #region 属性
        //http://localhost:3381/WF/CCForm/FrmExcel.aspx?1=2&FK_MapData=ExcelCSBD&IsEdit=1&IsPrint=0&FK_Flow=122&WorkID=167&NodeID=12201&FK_Node=12201&UserNo=fuhui&SID=c03oofsp2zk5xuxsc3tqa0jg&IsLoadData=1
        public string FK_Flow
        {
            get
            {
                var fk_flow = this.Request.QueryString["FK_Flow"];
                //if (string.IsNullOrEmpty(fk_flow))
                //    return "122";
                return fk_flow;
            }
        }

        public int FK_Node
        {
            get
            {
                try
                {
                    string nodeid = this.Request.QueryString["NodeID"];
                    if (nodeid == null)
                        nodeid = this.Request.QueryString["FK_Node"];
                    //if (string.IsNullOrEmpty(nodeid))
                    //    nodeid = "12201";
                    return int.Parse(nodeid);
                }
                catch
                {
                    if (string.IsNullOrEmpty(this.FK_Flow))
                        return 0;
                    else
                        return int.Parse(this.FK_Flow); // 0; 有可能是流程调用流程表单。
                }
            }
        }
        public string WorkID
        {
            get
            {
                var workid = this.Request.QueryString["WorkID"];
                //if (string.IsNullOrEmpty(workid))
                //    workid = "167";
                return workid;
            }
        }

        public int OID
        {
            get
            {
                string cworkid = this.Request.QueryString["CWorkID"];
                if (string.IsNullOrEmpty(cworkid) == false && int.Parse(cworkid) != 0)
                    return int.Parse(cworkid);

                string oid = this.Request.QueryString["WorkID"];
                if (string.IsNullOrEmpty(oid))
                    oid = this.Request.QueryString["OID"];
                //if (string.IsNullOrEmpty(oid))
                //    oid = "167";
                return int.Parse(oid);
            }
        }
        /// <summary>
        /// 延续流程ID
        /// </summary>
        public int CWorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["CWorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 父流程ID
        /// </summary>
        public int PWorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["PWorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 流程ID
        /// </summary>
        public int FID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public string FK_MapData
        {
            get
            {
                string s = this.Request.QueryString["FK_MapData"];
                //if (s == null)
                //    return "ExcelCSBD";
                return s;
            }
        }
        /// <summary>
        /// SID
        /// </summary>
        public string SID
        {
            get
            {
                return this.Request.QueryString["SID"];
            }
        }
        /// <summary>
        /// 是否打印？
        /// </summary>
        public bool IsPrint { get; set; }
        /// <summary>
        /// 是否留痕？
        /// </summary>
        public bool IsMarks { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsCheck { get; set; }
        /// <summary>
        /// 是否是第一次打开Word表单
        /// </summary>
        public bool IsFirst { get; set; }
        /// <summary>
        /// 填充的主表JSON数据，为含有key,value的数组
        /// </summary>
        public string ReplaceParams { get; set; }
        /// <summary>
        /// 填充的主表属性名组织的JSON数据,为String数组
        /// </summary>
        public string ReplaceFields { get; set; }
        /// <summary>
        /// 填充的明细表数据JSON数据，为对象数组
        /// </summary>
        public string ReplaceDtls { get; set; }
        /// <summary>
        /// 填充的明细表编号JSON数据，为String数组。
        /// </summary>
        public string ReplaceDtlNos { get; set; }
        /// <summary>
        /// 定义一个权限控制变量.
        /// </summary>
        public ToolbarExcel toolbar = new ToolbarExcel();
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            //WebUser.SignInOfGener(new BP.Port.Emp("fuhui"));

            UserName = WebUser.Name;
            if (string.IsNullOrEmpty(this.FK_MapData))
            {
                divMenu.InnerHtml = "<h1 style='color:red'>必须传入参数FK_Mapdata!<h1>";
                return;
            }

            //获得外部的标记。
            string type = Request["action"];
            if (string.IsNullOrEmpty(type))
            {
                /** 第一次进来，的时候，没有标记。
                 */
                //初始化它的解决方案.  add by stone. 2015-01-25. 增加权限控制方案，以在不同的节点实现不同的控制.
                if (string.IsNullOrEmpty(this.FK_Flow) == false)
                {
                    /*接受到了流程编号，就要找到他的控制方案.*/
                    BP.WF.Template.FrmNode fn = new BP.WF.Template.FrmNode(this.FK_Flow, this.FK_Node, this.FK_MapData);
                    if (fn.FrmSln == 1)
                    {
                        /* 如果是自定义方案.*/
                        ToolbarExcelSln toobarsln = new ToolbarExcelSln(this.FK_Flow, this.FK_Node, this.FK_MapData);
                        toolbar.Row = toobarsln.Row;
                    }
                    else
                    {
                        //非自定义方案就取默认方案.
                        toolbar = new ToolbarExcel(this.FK_MapData);
                    }
                }
                else
                {
                    /*没有找打他的控制方案，就取默认方案.*/
                    toolbar = new ToolbarExcel(this.FK_MapData);
                }

                //初始化表单信息?
                InitOffice_Toolbar(toolbar);
            }
            else
            {
                if (type.Equals("LoadFile"))
                {
                    LoadFile();
                    return;
                }

                if (type.Equals("SaveFile"))
                {
                    SaveFile();
                    SaveFieldInfos();
                    return;
                }
                throw new Exception("@没有处理的标记错误:" + type);
            }

            //创建excel数据实体.
            GEEntityExcelFrm en = new GEEntityExcelFrm(this.FK_MapData);

            //检查数据文件是否存在？如果存在并打开不存在并copy模版。
            var root = SystemConfig.PathOfDataUser + "\\FrmOfficeTemplate\\";
            var rootInfo = new DirectoryInfo(root);
            if (!rootInfo.Exists)
                rootInfo.Create();

            var files = rootInfo.GetFiles(en.FK_MapData + ".*");
            // 判断是否有这个数据文件.
            if (files.Length == 0)
            {
                Response.Write("<h3>Excel表单模板文件不存在，请确认已经上传Excel表单模板，该模版的位于服务器：" + rootInfo.FullName + "</h3>");
                Response.End();
                return;
            }

            FileInfo tmpFile = null;
            FileInfo wordFile = null;

            // 检查数据目录文件是否存在？
            var pathDir = SystemConfig.PathOfDataUser + @"\FrmOfficeFiles\" + this.FK_MapData;
            if (!Directory.Exists(pathDir))
                Directory.CreateDirectory(pathDir);

            // 初始化数据文件. 
            tmpFile = files[0];
            wordFile = new FileInfo(pathDir + "\\" + this.OID + tmpFile.Extension);
            if (wordFile.Exists == false)
            {
                /*如果不存在就copy 一个副本。*/
                IsFirst = true;
                File.Copy(tmpFile.FullName, wordFile.FullName);
            }
            else
            {
                IsFirst = false;
            }

            //edited by liuxc,2015-1-30,如果在构造中使用传递OID的构造函数，则下面的Save时，第一次会插入不成功，此处是因为insert时判断OID不为0则认为是已经存在的记录，实际上此处还没有存在，所以使用下面的逻辑进行判断，如果没有该条记录，则插入新记录
            en.OID = this.OID;

            if (en.IsExits == false)
                en.InsertAsOID(this.OID);

            //给实体赋值.
            en.FilePath = wordFile.FullName;
            en.RDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            en.LastEditer = WebUser.Name;
            en.ResetDefaultVal();

            //接受外部参数数据。
            string[] paras = this.RequestParas.Split('&');
            foreach (string str in paras)
            {
                if (string.IsNullOrEmpty(str) || str.Contains("=") == false)
                    continue;
                string[] kvs = str.Split('=');
                en.SetValByKey(kvs[0], kvs[1]);
            }

            en.Save(); //执行保存.

            //装载数据。
            this.LoadFrmData(en);

            //替换掉 word 里面的数据.
            fileName.Text = string.Format(@"\{0}\{1}{2}", en.FK_MapData, this.OID, wordFile.Extension);
            fileType.Text = wordFile.Extension.TrimStart('.');
        }


        /// <summary>
        /// 保存从word中提取的数据
        /// </summary>
        private void SaveFieldInfos()
        {
            var mes = new MapExts(this.FK_MapData);
            if (mes.Count == 0) return;

            var item = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull) as MapExt;
            if (item == null) return;

            var fieldCount = 0;
            foreach (var key in Request.Form.AllKeys)
            {
                var idx = 0;
                if (key.StartsWith("field") && key.Length > 5 && int.TryParse(key.Substring(5), out idx))
                {
                    fieldCount++;
                }
            }

            var fieldsJson = string.Empty;
            for (var i = 0; i < fieldCount; i++)
            {
                fieldsJson += Request["field" + i];
            }

            //var fieldsJson = Request["field"];
            var fields = LitJson.JsonMapper.ToObject<List<ReplaceField>>(HttpUtility.UrlDecode(fieldsJson));

            //更新主表数据
            var en = new GEEntityExcelFrm(this.FK_MapData);
            en.OID = this.OID;

            if (en.RetrieveFromDBSources() == 0)
            {
                throw new Exception("OID=" + this.OID + "的数据在" + this.FK_MapData + "中不存在，请检查！");
            }

            //此处因为weboffice在上传的接口中，只有上传成功与失败的返回值，没有具体的返回信息参数，所以未做异常处理
            foreach (var field in fields)
            {
                en.SetValByKey(field.key, field.value);
            }

            en.LastEditer = WebUser.Name;
            en.RDT = DataType.CurrentDataTime;
            en.Update();

            //todo:更新明细表数据，此处逻辑可能还有待商榷
            var mdtls = new MapDtls(this.FK_MapData);
            if (mdtls.Count == 0) return;

            var dtlsCount = 0;
            foreach (var key in Request.Form.AllKeys)
            {
                var idx = 0;
                if (key.StartsWith("dtls") && key.Length > 4 && int.TryParse(key.Substring(4), out idx))
                {
                    dtlsCount++;
                }
            }

            var dtlsJson = string.Empty;
            for (var i = 0; i < dtlsCount; i++)
            {
                dtlsJson += Request["dtls" + i];
            }

            //var dtlsJson = Request["dtls"];
            var dtls = LitJson.JsonMapper.ToObject<List<ReplaceDtlTable>>(HttpUtility.UrlDecode(dtlsJson));
            GEDtls gedtls = null;
            GEDtl gedtl = null;
            ReplaceDtlTable wdtl = null;

            foreach (MapDtl mdtl in mdtls)
            {
                wdtl = dtls.FirstOrDefault(o => o.dtlno == mdtl.No);

                if (wdtl == null || wdtl.dtl.Count == 0) continue;

                //此处不是真正意义上的更新，因为不知道明细表的主键，只能将原明细表中的数据删除掉，然后再重新插入新的数据
                gedtls = new GEDtls(mdtl.No);
                gedtls.Delete(GEDtlAttr.RefPK, en.PKVal);

                foreach (var d in wdtl.dtl)
                {
                    gedtl = gedtls.GetNewEntity as GEDtl;

                    foreach (var cell in d.cells)
                    {
                        gedtl.SetValByKey(cell.key, cell.value);
                    }

                    gedtl.RefPK = en.PKVal.ToString();
                    gedtl.RDT = DataType.CurrentDataTime;
                    gedtl.Rec = WebUser.No;
                    gedtl.Insert();
                }
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        /// <param name="toolbar"></param>
        private void InitOffice_Toolbar(ToolbarExcel toolbar)
        {
            if (toolbar.OfficeSaveEnable)
                AddBtn(NamesOfBtn.Save, toolbar.OfficeSaveLab, "SaveOffice", "icon-save"); //调用保存方法.
            else
                return; //就不要向下显示其他的按钮了。

            if (toolbar.OfficeIsMarks)
                divMenu.InnerHtml += "<select id='marks' onchange='ShowUserName()' style='width: 100px'><option value='1'>全部</option><select>&nbsp;&nbsp;";

            if (toolbar.OfficeOpenEnable)
                AddBtn(NamesOfBtn.Open, toolbar.OfficeOpenLab, "OpenTempLate", "icon-open");

            if (toolbar.OfficeRefuseEnable)
            {
                AddBtn(NamesOfBtn.Accept, "接受修订", "acceptOffice", "icon-accept");
                AddBtn(NamesOfBtn.Refuse, "拒绝修订", "refuseOffice", "icon-refuse");
            }

            if (toolbar.OfficeTHEnable)
                AddBtn("over", "套红文件", "overOffice", "");

            if (toolbar.OfficePrintEnable)
                AddBtn(NamesOfBtn.Print, toolbar.OfficePrintLab, "printOffice", "icon-print");

            if (toolbar.OfficeSealEnable)
                AddBtn(NamesOfBtn.Seal, "签章", "sealOffice", "icon-seal");

            //if (toolbar.OfficeInsertFlowEnable)
            //    AddBtn(NamesOfBtn.FlowImage, toolbar.OfficeInsertFlowLab, "InsertFlow");

            //if (toolbar.OfficeIsMarks)
            //    AddBtn("fegnxian", toolbar.OfficeIsMarks, "InsertFengXian");

            if (toolbar.OfficeDownEnable)
                AddBtn(NamesOfBtn.Download, toolbar.OfficeDownLab, "DownLoad", "icon-download");
        }
        /// <summary>
        /// 装载文件.
        /// </summary>
        private void LoadFile()
        {
            string name = Request.QueryString["fileName"];
            var path = SystemConfig.PathOfDataUser + "\\FrmOfficeFiles" + name;
            var result = File.ReadAllBytes(path);
            Response.Clear();
            Response.BinaryWrite(result); //这是什么意思？
            Response.End();
        }
        /// <summary>
        /// 生成要返回给page的Json数据.
        /// </summary>
        /// <param name="en"></param>
        public void LoadFrmData(Entity en)
        {
            var dictParams = new ReplaceFieldList(); //主表参数值集合
            var fields = new List<string>(); // 主表参数名集合

            dictParams.Add("No", WebUser.No, "string");
            dictParams.Add("Name", WebUser.Name, "string");
            dictParams.Add("FK_Dept", WebUser.FK_Dept, "string");
            dictParams.Add("FK_DeptName", WebUser.FK_DeptName, "string");

            var mes = new MapExts(this.FK_MapData);
            MapExt item = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull) as MapExt;
            //把数据装载到表里，包括从表数据，主表数据未存储.
            MapDtls dtls = new MapDtls(this.FK_MapData);
            MapAttrs mattrs = new MapAttrs(this.FK_MapData);
            en = BP.WF.Glo.DealPageLoadFull(en, item, mattrs, dtls); // 处理表单装载数据.

            //MapData md=new MapData(this.FK_MapData);
            foreach (MapAttr mapattr in mattrs)
            {
                fields.Add(mapattr.KeyOfEn);
                dictParams.Add(mapattr.KeyOfEn, en.GetValStringByKey(mapattr.KeyOfEn), mapattr.IsSigan ? "sign" : "string");
            }

            ReplaceParams = IsFirst ? GenerateParamsJsonString(dictParams) : "[]";

            //生成json格式。
            ReplaceFields = GenerateFieldsJsonString(fields);

            if (item == null || string.IsNullOrEmpty(item.Tag1)
                || item.Tag1.Length < 15)
            {
                ReplaceDtls = "[]";
                ReplaceDtlNos = "[]";
                return;
            }

            var replaceDtlNos = new List<string>();
            DataSet ds = new DataSet();
            DataTable table = null;
            var sql = string.Empty;

            // 填充从表.
            foreach (MapDtl dtl in dtls)
            {
                replaceDtlNos.Add(dtl.No);

                if (!IsFirst)
                    continue;

                sql = "SELECT * FROM " + dtl.PTable + " WHERE RefPK='" + this.WorkID + "'";
                table = BP.DA.DBAccess.RunSQLReturnTable(sql);
                table.TableName = dtl.No;
                ds.Tables.Add(table);
            }

            // 从表数据.
            ReplaceDtls = IsFirst ? BP.DA.DataTableConvertJson.Dataset2Json(ds) : "[]";
            ReplaceDtlNos = GenerateFieldsJsonString(replaceDtlNos);
        }
        /// <summary>
        /// 转换键值集合为json字符串
        /// </summary>
        /// <param name="dictParams">键值集合</param>
        /// <returns></returns>
        private string GenerateParamsJsonString(ReplaceFieldList dictParams)
        {
            return "[" + dictParams.Aggregate(string.Empty, (curr, next) => curr + string.Format("{{\"key\":\"{0}\",\"value\":\"{1}\",\"type\":\"{2}\"}},", next.key, (next.value ?? "").Replace("\\", "\\\\").Replace("'", "\'"), next.type)).TrimEnd(',') + "]";
        }

        /// <summary>
        /// 转换String集合为json字符串
        /// </summary>
        /// <param name="fields">String集合</param>
        /// <returns></returns>
        private string GenerateFieldsJsonString(List<string> fields)
        {
            return LitJson.JsonMapper.ToJson(fields);
        }

        private void SaveFile()
        {
            try
            {
                HttpFileCollection files = HttpContext.Current.Request.Files;

                if (files.Count > 0)
                {
                    //检查文件扩展名字
                    HttpPostedFile postedFile = files[0];
                    var fileName = Path.GetFileName(Request.QueryString["filename"]);
                    var path = SystemConfig.PathOfDataUser + @"\FrmOfficeFiles\" + this.FK_MapData;

                    if (fileName != "")
                    {
                        postedFile.SaveAs(path + "\\" + fileName);

                        var en = new GEEntityExcelFrm(this.FK_MapData);
                        en.RetrieveFromDBSources();

                        en.LastEditer = WebUser.Name;
                        en.RDT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        en.Update();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AddBtn(string id, string label, string clickEvent, string iconCls)
        {
            //var btn = new LinkBtn(true);
            //btn.ID = id;
            //btn.Text = label;
            //btn.Attributes["onclick"] = "return " + clickEvent + "()";
            //btn.PostBackUrl = "javascript:void(0)";
            //divMenu.Controls.Add(btn);

            divMenu.InnerHtml +=
                string.Format(
                    "<a href=\"javascript:void(0)\" id=\"{0}\" onclick=\"return {1}()\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'{2}'\">{3}</a>&nbsp;&nbsp;",
                    id, clickEvent, iconCls, label);
        }
    }
}