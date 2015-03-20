using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.DA;
using BP.WF;
using BP.Port;

namespace BP.WF
{
    /// <summary>
    /// 工作流定义管理
    /// </summary>
    public class WorkflowDefintionManager
    {
        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodes">节点信息，格式为:@NodeID=xxxx@X=xxx@Y=xxx@Name=xxxx</param>
        /// <param name="dirs">方向信息，格式为:@Node=xxxx@ToNode=xxx@Y=xxx@Name=xxxx</param>
        /// <param name="labes">标签信息，格式为:@MyPK=xxxxx@Label=xxx@X=xxx@Y=xxxx</param>
        public static string SaveFlow(string fk_flow, string nodes, string dirs, string labes)
        {
            try
            {
                //处理方向。
                string sql = "Delete FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
                DBAccess.RunSQL(sql);

                string[] mydirs = dirs.Split('~');
                foreach (string dir in mydirs)
                {
                    if (string.IsNullOrEmpty(dir))
                        continue;

                    AtPara ap = new AtPara(dir);

                    string dots = ap.GetValStrByKey("Dots").Replace('#', '@');
                    sql = "INSERT INTO WF_Direction (Node,ToNode,FK_Flow,DirType,IsCanBack,Dots,MyPK) VALUES ("
                        + ap.GetValIntByKey("Node") + "," + ap.GetValIntByKey("ToNode") + ",'" + fk_flow
                        + "'," + ap.GetValIntByKey("DirType") + "," + ap.GetValIntByKey("IsCanBack")
                        + "," + (dots == string.Empty ? "null" : "'" + dots + "'") + ",'" + ap.GetValStrByKey("MyPK") + "')";

                    try
                    {
                        DBAccess.RunSQL(sql);
                    }
                    catch
                    {
                    }
                }

                //处理节点。
                Flow f1 = new Flow();
                f1.No = fk_flow;
                f1.RetrieveFromDBSources();
                string name = f1.Name;
                string[] nds = nodes.Split('~');
                foreach (string nd in nds)
                {
                    if (string.IsNullOrEmpty(nd))
                        continue;

                    // 处理网友反馈的bug, 当NodeID=0 , =1 出现保存错误。
                    AtPara ap = new AtPara(nd);
                    int nodeID = ap.GetValIntByKey("NodeID");
                    Node mynode = new Node();
                    mynode.NodeID = nodeID;
                    mynode.RetrieveFromDBSources();

                    if (mynode.NodeID == 0)
                    {
                        f1.Paras = string.Format("@StartNodeX={0}@StartNodeY={1}", ap.GetValStrByKey("X"), ap.GetValStrByKey("Y"));
                    }
                    else if (mynode.NodeID == 1)
                    {
                        f1.Paras += string.Format("@EndNodeX={0}@EndNodeY={1}", ap.GetValStrByKey("X"), ap.GetValStrByKey("Y"));
                    }
                    else
                    {
                        mynode.Name = ap.GetValStrByKey("Name");
                        mynode.X = ap.GetValIntByKey("X");
                        mynode.Y = ap.GetValIntByKey("Y");
                        mynode.DirectUpdate();
                    }
                }

                f1.Save();
                Flow.UpdateVer(fk_flow);

            
                //处理标签。
                string[] mylabs = labes.Split('~');
                foreach (string lab in mylabs)
                {
                    if (string.IsNullOrEmpty(lab))
                        continue;

                    AtPara ap = new AtPara(lab);
                    LabNote ln = new LabNote();
                    ln.MyPK = ap.GetValStrByKey("MyPK");
                    ln.FK_Flow = fk_flow;
                    ln.Name = ap.GetValStrByKey("Label");
                    ln.X = ap.GetValIntByKey("X");
                    ln.Y = ap.GetValIntByKey("Y");
                    ln.Save();
                }

                // 备份文件
                f1.WriteToXml();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }
        /// <summary>
        /// 导出流程模版
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="saveToPath"></param>
        public static void ExpWorkFlowTemplete(string flowNo,string saveToPath)
        {
        }
        /// <summary>
        /// 导入流程模版
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="filePath">文件路径</param>
        public static void ImpWorkFlowTemplete(string flowNo, string filePath)
        {
        }
        /// <summary>
        /// 执行新建一个流程模版
        /// </summary>
        /// <param name="flowSort">流程类别</param>
        public static void CreateFlowTemplete(string flowSort)
        {
        }
        /// <summary>
        /// 删除一个流程模版
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        public static string DeleteFlowTemplete(string flowNo)
        {
            BP.WF.Flow fl1 = new BP.WF.Flow(flowNo);
            try
            {
                fl1.DoDelete();
                return null;
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError("Do Method DelFlow Branch has a error , para:\t" + flowNo + ex.Message);
                return ex.Message;
            }
        }
    }
}
