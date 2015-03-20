using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.WF
{
	/// <summary>
	/// 节点调用子流程属性
	/// </summary>
    public class NodeFlowAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 调用子流程
        /// </summary>
        public const string FK_Flow = "FK_Flow";
    }
	/// <summary>
	/// 节点调用子流程
	/// 节点的调用子流程有两部分组成.	 
	/// 记录了从一个节点调用其他的多个节点.
	/// 也记录了调用这个节点的其他的节点.
	/// </summary>
    public class NodeFlow : EntityMM
    {
        #region 基本属性
        /// <summary>
        ///节点
        /// </summary>
        public string FK_Node
        {
            get
            {
                return this.GetValStringByKey(NodeFlowAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(NodeFlowAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 调用子流程
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(NodeFlowAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(NodeFlowAttr.FK_Flow, value);
            }
        }
        public string FK_FlowT
        {
            get
            {
                return this.GetValRefTextByKey(NodeFlowAttr.FK_Flow);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点调用子流程
        /// </summary>
        public NodeFlow() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeFlow");
                map.EnDesc = "节点调用子流程";

                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBIntPK(NodeFlowAttr.FK_Node, 0, "节点", true, true);
                // map.AddDDLEntitiesPK(NodeFlowAttr.FK_Node, null, "节点", new NodeSheets(), true);
                map.AddDDLEntitiesPK(NodeFlowAttr.FK_Flow, null, "子流程", new Flows(), true);

                //map.AddSearchAttr(NodeFlowAttr.FK_Node);
                map.AddSearchAttr(NodeFlowAttr.FK_Flow);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 节点调用子流程
	/// </summary>
    public class NodeFlows : EntitiesMM
    {
        /// <summary>
        /// 他的调用子流程
        /// </summary>
        public Emps HisEmps
        {
            get
            {
                Emps ens = new Emps();
                foreach (NodeFlow ns in this)
                {
                    ens.AddEntity(new Emp(ns.FK_Flow));
                }
                return ens;
            }
        }
        /// <summary>
        /// 他的工作节点
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (NodeFlow ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Node));
                }
                return ens;

            }
        }
        /// <summary>
        /// 节点调用子流程
        /// </summary>
        public NodeFlows() { }
        /// <summary>
        /// 节点调用子流程
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public NodeFlows(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeFlowAttr.FK_Node, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 节点调用子流程
        /// </summary>
        /// <param name="EmpNo">EmpNo </param>
        public NodeFlows(string EmpNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeFlowAttr.FK_Flow, EmpNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得调用它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeFlow();
            }
        }
        /// <summary>
        /// 取调用一个调用子流程集合能够访问调用的节点s
        /// </summary>
        /// <param name="sts">调用子流程集合</param>
        /// <returns></returns>
        public Nodes GetHisNodes(Emps sts)
        {
            Nodes nds = new Nodes();
            Nodes tmp = new Nodes();
            foreach (Emp st in sts)
            {
                tmp = this.GetHisNodes(st.No);
                foreach (Node nd in tmp)
                {
                    if (nds.Contains(nd))
                        continue;
                    nds.AddEntity(nd);
                }
            }
            return nds;
        }
        /// <summary>
        /// 调用子流程对应的节点
        /// </summary>
        /// <param name="EmpNo">调用子流程编号</param>
        /// <returns>节点s</returns>
        public Nodes GetHisNodes(string EmpNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeFlowAttr.FK_Flow, EmpNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (NodeFlow en in this)
            {
                ens.AddEntity(new Node(en.FK_Node));
            }
            return ens;
        }
        /// <summary>
        /// 转向此节点的集合的 Nodes
        /// </summary>
        /// <param name="nodeID">此节点的ID</param>
        /// <returns>转向此节点的集合的Nodes (FromNodes)</returns> 
        public Emps GetHisEmps(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeFlowAttr.FK_Node, nodeID);
            qo.DoQuery();

            Emps ens = new Emps();
            foreach (NodeFlow en in this)
            {
                ens.AddEntity(new Emp(en.FK_Flow));
            }
            return ens;
        }
    }
}
