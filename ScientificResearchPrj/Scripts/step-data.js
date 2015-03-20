


var XiangMuOriginResult = undefined;
var XiangMuPropertygridData = [
             {
                 "name": "加载项目", "group": "需求分析-项目结果", "type": "load", "editor": {
                     type: "ex_panel",
                     options: {
                         callback: _LoadHistory.loadProjectHistoryData
                     }
                 }
             },
             { "name": "项目编号", "index": 0, "id": "XM_No", "group": "需求分析-项目结果" },
             { "name": "项目名称", "index": 0, "id": "XM_Name", "group": "需求分析-项目结果" },
             { "name": "拟定者", "index": 0, "id": "XM_ProposerName", "group": "需求分析-项目结果" },
             { "name": "拟定时间", "index": 0, "id": "XM_ProposeTime", "group": "需求分析-项目结果" },
             { "name": "关键字", "index": 0, "id": "XM_Keys", "group": "需求分析-项目结果" },
             { "name": "项目组", "index": 0, "id": "XM_FK_Xmz", "group": "需求分析-项目结果" },
             { "name": "栏目", "index": 0, "id": "XM_Columns", "group": "需求分析-项目结果" },
             { "name": "项目描述", "index": 0, "id": "XM_Description", "group": "需求分析-项目结果", "editor": "ex_wintextareareadonly" },
             { "name": "实现任务", "index": 0, "id": "XM_Tasks", "value": "", "group": "需求分析-项目结果", "editor": "ex_wintextareareadonly" },
             { "name": "问题分解", "index": 0, "id": "XM_Questions", "group": "需求分析-项目结果", "editor": "ex_wintextareareadonly" },
             { "name": "补充说明", "index": 0, "id": "XM_Remarks", "group": "需求分析-项目结果", "editor": "ex_wintextareareadonly" },
             { "name": "上次修改时间", "index": 0, "id": "XM_ModifyTime", "group": "需求分析-项目结果" }
];



var KeTiOriginResult = undefined;
var KeTiPropertygridData = [
              {
                  "name": "加载课题", "group": "需求分析-课题结果", "type": "load", "editor": {
                      type: "ex_panel",
                      options: {
                          callback: _LoadHistory.loadSubjectHistoryData
                      }
                  }
              },
              { "name": "课题编号", "index": 0, "id": "KT_No", "group": "需求分析-课题结果" },
              { "name": "课题名称", "index": 0, "id": "KT_Name", "group": "需求分析-课题结果" },
              { "name": "拟定者", "index": 0, "id": "KT_ProposerName", "group": "需求分析-课题结果" },
              { "name": "拟定时间", "index": 0, "id": "KT_ProposeTime", "group": "需求分析-课题结果" },
              { "name": "关键字", "index": 0, "id": "KT_Keys", "group": "需求分析-课题结果" },
              { "name": "课题描述", "index": 0, "id": "KT_Description", "group": "需求分析-课题结果", "editor": "ex_wintextareareadonly" },
              { "name": "来源描述", "index": 0, "id": "KT_SourceDesc", "group": "需求分析-课题结果", "editor": "ex_wintextareareadonly" },
              { "name": '有效链接及链接说明', "id": "KT_Link", "index": 0, "group": "需求分析-课题结果" },
              { "name": "分析结果", "index": 0, "id": "KT_AnalysisResult", "group": "需求分析-课题结果", "editor": "ex_wintextareareadonly" },
              { "name": "拟实现任务", "index": 0, "id": "KT_TargetTask", "group": "需求分析-课题结果", "editor": "ex_wintextareareadonly" },
              { "name": "理想创新之处", "index": 0, "id": "KT_Innovation", "group": "需求分析-课题结果", "editor": "ex_wintextareareadonly" },
              { "name": '附件清单以及说明', "index": 0, "id": "KT_Attach", "group": "需求分析-课题结果" },
              { "name": "补充说明", "index": 0, "id": "KT_Remarks", "group": "需求分析-课题结果", "editor": "ex_wintextareareadonly" },
              { "name": "上次修改时间", "index": 0, "id": "KT_ModifyTime", "group": "需求分析-课题结果" }
];

var XQFXShenHeOriginResult = undefined;
var XQFXShenHePropertygridData = [
            {
                "name": "加载需求分析审核结果", "group": "需求分析审核结果", "type": "load", "editor": {
                    type: "ex_panel",
                    options: {
                        callback: _LoadHistory.loadXuQiuFenXiShenHeHistoryData
                    }
                }
            },
            { "name": "审核人", "index": 0, "id": "XQFXSH_ShenHeRenName", "group": "需求分析审核结果" },
            { "name": "审核时间", "index": 0, "id": "XQFXSH_ShenHeShiJian", "group": "需求分析审核结果" },
            { "name": "审核结果", "index": 0, "id": "XQFXSH_ShenHeJieGuo", "group": "需求分析审核结果" },
            { "name": "审核意见", "index": 0, "id": "XQFXSH_ShenHeYiJian", "group": "需求分析审核结果", "editor": "ex_wintextareareadonly" },
            { "name": '有效链接及链接说明', "index": 0, "id": "XQFXSH_Link", "group": "需求分析审核结果" },
            { "name": '附件清单以及说明', "index": 0, "id": "XQFXSH_Attach", "group": "需求分析审核结果" },
            { "name": "上次修改时间", "index": 0, "id": "XQFXSH_ModifyTime", "group": "需求分析审核结果" }
];

var DYOriginResult = undefined;
var DYPropertygridData = [
    {
        "name": "加载调研结果", "group": "调研结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadDiaoYanHistoryData
            }
        }
    },
    { "name": "调研编号", "index": 0, "id": "DY_No", "group": "调研结果" },
    { "name": "调研名称", "index": 0, "id": "DY_Name", "group": "调研结果" },
    { "name": "调查人", "index": 0, "id": "DY_ProposerName", "group": "调研结果" },
    { "name": "调查时间", "index": 0, "id": "DY_ProposeTime", "group": "调研结果" },
    { "name": "研究综述类型", "index": 0, "id": "DY_SumType", "group": "调研结果" },
    { "name": "研究综述", "index": 0, "id": "DY_Sum", "group": "调研结果", "editor": "ex_wintextareareadonly" },
    { "name": "调查地点", "index": 0, "id": "DY_SurveryAddr", "group": "调研结果" },
    { "name": '课题研究者', "index": 0, "id": "DY_Investigator", "group": "调研结果" },
    { "name": "关键字", "index": 0, "id": "DY_Keys", "group": "调研结果" },
    { "name": "研究结果", "index": 0, "id": "DY_AnalysisResult", "group": "调研结果", "editor": "ex_wintextareareadonly" },
    { "name": "有效链接及链接说明", "index": 0, "id": "DY_Link", "group": "调研结果" },
    { "name": '对研究结果的评价（优点)', "index": 0, "id": "DY_AdvantageValue", "group": "调研结果", "editor": "ex_wintextareareadonly" },
    { "name": "对研究结果的评价（缺点）", "index": 0, "id": "DY_WeaknessValue", "group": "调研结果", "editor": "ex_wintextareareadonly" },
    { "name": "未解决的问题", "index": 0, "id": "DY_UnsolvedProblem", "group": "调研结果", "editor": "ex_wintextareareadonly" },
    { "name": "目前的技术趋势", "index": 0, "id": "DY_TechTrends", "group": "调研结果", "editor": "ex_wintextareareadonly" },
    { "name": "超越点", "index": 0, "id": "DY_BeyondPoint", "group": "调研结果", "editor": "ex_wintextareareadonly" },
    { "name": "附件清单以及说明", "index": 0, "id": "DY_Attach", "group": "调研结果" },
    { "name": "补充说明", "index": 0, "id": "DY_Remarks", "group": "调研结果", "editor": "ex_wintextareareadonly" },
    { "name": "上次修改时间", "index": 0, "id": "DY_ModifyTime", "group": "调研结果" }
];

var DYShenHeOriginResult = undefined;
var DYShenHePropertygridData = [
    {
        "name": "加载调研审核结果", "group": "调研审核结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadDiaoYanShenHeHistoryData
            }
        }
    },
    { "name": "审核人", "index": 0, "id": "DYSH_ShenHeRenName", "group": "调研审核结果" },
    { "name": "审核时间", "index": 0, "id": "DYSH_ShenHeShiJian", "group": "调研审核结果" },
    { "name": "审核结果", "index": 0, "id": "DYSH_ShenHeJieGuo", "group": "调研审核结果" },
    { "name": "审核意见", "index": 0, "id": "DYSH_ShenHeYiJian", "group": "调研审核结果", "editor": "ex_wintextareareadonly" },
    { "name": '有效链接及链接说明', "index": 0, "id": "DYSH_Link", "group": "调研审核结果" },
    { "name": '附件清单以及说明', "index": 0, "id": "DYSH_Attach", "group": "调研审核结果" },
    { "name": "上次修改时间", "index": 0, "id": "DYSH_ModifyTime", "group": "调研审核结果" }
];

var TCWTOriginResult = undefined;
var TCWTPropertygridData = [
        {
            "name": "加载提出问题结果", "group": "提出问题结果", "type": "load", "editor": {
                type: "ex_panel",
                options: {
                    callback: _LoadHistory.loadTiChuWenTiHistoryData
                }
            }
        },
        { "name": "问题编号", "index": 0, "id": "TCWT_No", "group": "提出问题结果" },
        { "name": "问题名称", "index": 0, "id": "TCWT_Name", "group": "提出问题结果" },
        { "name": "提出者", "index": 0, "id": "TCWT_ProposerName", "group": "提出问题结果" },
        { "name": "提出时间", "index": 0, "id": "TCWT_ProposeTime", "group": "提出问题结果" },
        { "name": "关键字", "index": 0, "id": "TCWT_Keys", "group": "提出问题结果" },
        { "name": "问题轻缓程度", "index": 0, "id": "TCWT_Mitigation", "group": "提出问题结果" },
        { "name": '问题描述', "index": 0, "id": "TCWT_Description", "group": "提出问题结果", "editor": "ex_wintextareareadonly" },
        { "name": '克服方法', "index": 0, "id": "TCWT_OvercomeMethod", "group": "提出问题结果", "editor": "ex_wintextareareadonly" },
        { "name": '论据', "index": 0, "id": "TCWT_Argument", "group": "提出问题结果", "editor": "ex_wintextareareadonly" },
        { "name": "有效链接及链接说明", "index": 0, "id": "TCWT_Link", "group": "提出问题结果" },
        { "name": "附件清单以及说明", "index": 0, "id": "TCWT_Attach", "group": "提出问题结果" },
        { "name": "补充说明", "index": 0, "id": "TCWT_Remarks", "group": "提出问题结果", "editor": "ex_wintextareareadonly" },
        { "name": "上次修改时间", "index": 0, "id": "TCWT_ModifyTime", "group": "提出问题结果" }
];

var TCWTShenHeOriginResult = undefined;
var TCWTShenHePropertygridData = [
    {
        "name": "加载提出问题审核结果", "group": "提出问题审核结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadTiChuWenTiShenHeHistoryData
            }
        }
    },
    { "name": "审核人", "index": 0, "id": "TCWTSH_ShenHeRenName", "group": "提出问题审核结果" },
    { "name": "审核时间", "index": 0, "id": "TCWTSH_ShenHeShiJian", "group": "提出问题审核结果" },
    { "name": "审核结果", "index": 0, "id": "TCWTSH_ShenHeJieGuo", "group": "提出问题审核结果" },
    { "name": "审核意见", "index": 0, "id": "TCWTSH_ShenHeYiJian", "group": "提出问题审核结果", "editor": "ex_wintextareareadonly" },
    { "name": '有效链接及链接说明', "index": 0, "id": "TCWTSH_Link", "group": "提出问题审核结果" },
    { "name": '附件清单以及说明', "index": 0, "id": "TCWTSH_Attach", "group": "提出问题审核结果" },
    { "name": "上次修改时间", "index": 0, "id": "TCWTSH_ModifyTime", "group": "提出问题审核结果" }
];

var JJSLOriginResult = undefined;
var JJSLPropertygridData = [
    {
        "name": "加载解决思路结果", "group": "解决思路结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadJieJueSiLuHistoryData
            }
        }
    },
    { "name": "解决思路编号", "index": 0, "id": "JJSL_No", "group": "解决思路结果" },
    { "name": "解决思路名称", "index": 0, "id": "JJSL_Name", "group": "解决思路结果" },
    { "name": "提出者", "index": 0, "id": "JJSL_ProposerName", "group": "解决思路结果" },
    { "name": "提出时间", "index": 0, "id": "JJSL_ProposeTime", "group": "解决思路结果" },
    { "name": "针对的问题", "index": 0, "id": "JJSL_FK_WTName", "group": "解决思路结果" },
    { "name": "关键字", "index": 0, "id": "JJSL_Keys", "group": "解决思路结果" },
    { "name": "类型", "index": 0, "id": "JJSL_Type", "group": "解决思路结果" },
    { "name": '解决思路描述', "index": 0, "id": "JJSL_Description", "group": "解决思路结果", "editor": "ex_wintextareareadonly" },
    { "name": "有效链接及链接说明", "index": 0, "id": "JJSL_Link", "group": "解决思路结果" },
    { "name": "附件清单以及说明", "index": 0, "id": "JJSL_Attach", "group": "解决思路结果" },
    { "name": "补充说明", "index": 0, "id": "JJSL_Remarks", "group": "解决思路结果", "editor": "ex_wintextareareadonly" },
    { "name": "上次修改时间", "index": 0, "id": "JJSL_ModifyTime", "group": "解决思路结果" }
];


var JJSLShenHeOriginResult = undefined;
var JJSLShenHePropertygridData = [
    {
        "name": "加载解决思路审核结果", "group": "解决思路审核结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadJieJueSiLuShenHeHistoryData
            }
        }
    },
    { "name": "审核人", "index": 0, "id": "JJSLSH_ShenHeRenName", "group": "解决思路审核结果" },
    { "name": "审核时间", "index": 0, "id": "JJSLSH_ShenHeShiJian", "group": "解决思路审核结果" },
    { "name": "审核结果", "index": 0, "id": "JJSLSH_ShenHeJieGuo", "group": "解决思路审核结果" },
    { "name": "审核意见", "index": 0, "id": "JJSLSH_ShenHeYiJian", "group": "解决思路审核结果", "editor": "ex_wintextareareadonly" },
    { "name": '有效链接及链接说明', "index": 0, "id": "JJSLSH_Link", "group": "解决思路审核结果" },
    { "name": '附件清单以及说明', "index": 0, "id": "JJSLSH_Attach", "group": "解决思路审核结果" },
    { "name": "上次修改时间", "index": 0, "id": "JJSLSH_ModifyTime", "group": "解决思路审核结果" }
];


var XSHOriginResult = undefined;
var XSHPropertygridData = [
    {
        "name": "加载形式化结果", "group": "形式化结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadXingShiHuaHistoryData
            }
        }
    },
    { "name": "形式化编号", "index": 0, "id": "XSH_No", "group": "形式化结果" },
    { "name": "形式化名称", "index": 0, "id": "XSH_Name", "group": "形式化结果" },
    { "name": "形式化者", "index": 0, "id": "XSH_ProposerName", "group": "形式化结果" },
    { "name": "形式化时间", "index": 0, "id": "XSH_ProposeTime", "group": "形式化结果" },
    { "name": "针对的解决思路", "index": 0, "id": "XSH_FK_SLName", "group": "形式化结果" },
    { "name": "关键字", "index": 0, "id": "XSH_Keys", "group": "形式化结果" },
    { "name": '形式化描述', "index": 0, "id": "XSH_Description", "group": "形式化结果", "editor": "ex_wintextareareadonly" },
    { "name": "有效链接及链接说明", "index": 0, "id": "XSH_Link", "group": "形式化结果" },
    { "name": "附件清单以及说明", "index": 0, "id": "XSH_Attach", "group": "形式化结果" },
    { "name": "补充说明", "index": 0, "id": "XSH_Remarks", "group": "形式化结果", "editor": "ex_wintextareareadonly" },
    { "name": "上次修改时间", "index": 0, "id": "XSH_ModifyTime", "group": "形式化结果" }
];


var XSHShenHeOriginResult = undefined;//初始课题审核的项目数据
var XSHShenHePropertygridData = [
    {
        "name": "加载形式化审核结果", "group": "形式化审核结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadXingShiHuaShenHeHistoryData
            }
        }
    },
    { "name": "审核人", "index": 0, "id": "XSHSH_ShenHeRenName", "group": "形式化审核结果" },
    { "name": "审核时间", "index": 0, "id": "XSHSH_ShenHeShiJian", "group": "形式化审核结果" },
    { "name": "审核结果", "index": 0, "id": "XSHSH_ShenHeJieGuo", "group": "形式化审核结果" },
    { "name": "审核意见", "index": 0, "id": "XSHSH_ShenHeYiJian", "group": "形式化审核结果", "editor": "ex_wintextareareadonly" },
    { "name": '有效链接及链接说明', "index": 0, "id": "XSHSH_Link", "group": "形式化审核结果" },
    { "name": '附件清单以及说明', "index": 0, "id": "XSHSH_Attach", "group": "形式化审核结果" },
    { "name": "上次修改时间", "index": 0, "id": "XSHSH_ModifyTime", "group": "形式化审核结果" }
];

var SJSFOriginResult = undefined;
var SJSFPropertygridData = [
    {
        "name": "加载求解算法结果", "group": "设计算法结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadSheJiSuanFaHistoryData
            }
        }
    },
    { "name": "求解算法编号", "index": 0, "id": "SF_No", "group": "设计算法结果" },
    { "name": "求解算法名称", "index": 0, "id": "SF_Name", "group": "设计算法结果" },
    { "name": "求解算法提出者", "index": 0, "id": "SF_ProposerName", "group": "设计算法结果" },
    { "name": "求解算法提出时间", "index": 0, "id": "SF_ProposeTime", "group": "设计算法结果" },
    { "name": "针对的形式化结果", "index": 0, "id": "SF_FK_XSHName", "group": "设计算法结果" },
    { "name": "关键字", "index": 0, "id": "SF_Keys", "group": "设计算法结果" },
    { "name": '求解算法描述', "index": 0, "id": "SF_Description", "group": "设计算法结果", "editor": "ex_wintextareareadonly" },
    { "name": '求解算法设计', "index": 0, "id": "SF_Design", "group": "设计算法结果", "editor": "ex_wintextareareadonly" },
    { "name": '求解算法实现步骤', "index": 0, "id": "SF_RealizeStep", "group": "设计算法结果", "editor": "ex_wintextareareadonly" },
    { "name": "有效链接及链接说明", "index": 0, "id": "SF_Link", "group": "设计算法结果" },
    { "name": "附件清单以及说明", "index": 0, "id": "SF_Attach", "group": "设计算法结果" },
    { "name": "补充说明", "index": 0, "id": "SF_Remarks", "group": "设计算法结果", "editor": "ex_wintextareareadonly" },
    { "name": "上次修改时间", "index": 0, "id": "SF_ModifyTime", "group": "设计算法结果" }
];


var SJSFShenHeOriginResult = undefined;//初始课题审核的项目数据
var SJSFShenHePropertygridData = [
    {
        "name": "加载设计算法审核结果", "group": "设计算法审核结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadSheJiSuanFaShenHeHistoryData
            }
        }
    },
    { "name": "审核人", "index": 0, "id": "SJSFSH_ShenHeRenName", "group": "设计算法审核结果" },
    { "name": "审核时间", "index": 0, "id": "SJSFSH_ShenHeShiJian", "group": "设计算法审核结果" },
    { "name": "审核结果", "index": 0, "id": "SJSFSH_ShenHeJieGuo", "group": "设计算法审核结果" },
    { "name": "审核意见", "index": 0, "id": "SJSFSH_ShenHeYiJian", "group": "设计算法审核结果", "editor": "ex_wintextareareadonly" },
    { "name": '有效链接及链接说明', "index": 0, "id": "SJSFSH_Link", "group": "设计算法审核结果" },
    { "name": '附件清单以及说明', "index": 0, "id": "SJSFSH_Attach", "group": "设计算法审核结果" },
    { "name": "上次修改时间", "index": 0, "id": "SJSFSH_ModifyTime", "group": "设计算法审核结果" }
];

var SJSYOriginResult = undefined;
var SJSYPropertygridData = [
    {
        "name": "加载设计实验结果", "group": "设计实验结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadSheJiShiYanHistoryData
            }
        }
    },
    { "name": "实验编号", "index": 0, "id": "SY_No", "group": "设计实验结果" },
    { "name": "实验名称", "index": 0, "id": "SY_Name", "group": "设计实验结果" },
    { "name": "实验设计者", "index": 0, "id": "SY_ProposerName", "group": "设计实验结果" },
    { "name": "实验时间", "index": 0, "id": "SY_ProposeTime", "group": "设计实验结果" },
    { "name": "针对的求解算法", "index": 0, "id": "SY_FK_SFName", "group": "设计实验结果" },
    { "name": "关键字", "index": 0, "id": "SY_Keys", "group": "设计实验结果" },
    { "name": '实验描述', "index": 0, "id": "SY_Description", "group": "设计实验结果", "editor": "ex_wintextareareadonly" },
    { "name": '实验设计', "index": 0, "id": "SY_Design", "group": "设计实验结果", "editor": "ex_wintextareareadonly" },
    { "name": '实验采用的评估指标体系', "index": 0, "id": "SY_IndexSys", "group": "设计实验结果", "editor": "ex_wintextareareadonly" },
    { "name": "实验实现步骤", "index": 0, "id": "SY_RealizeStep", "group": "设计实验结果", "editor": "ex_wintextareareadonly" },
    { "name": "无偏向的测试条件", "index": 0, "id": "SY_TestCondition", "group": "设计实验结果", "editor": "ex_wintextareareadonly" },
    { "name": "实验数据", "index": 0, "id": "SY_Data", "group": "设计实验结果", "editor": "ex_wintextareareadonly" },
    { "name": "量化的统计结果", "index": 0, "id": "SY_StatistacalResult", "group": "设计实验结果", "editor": "ex_wintextareareadonly" },
    { "name": "实验结果", "index": 0, "id": "SY_Result", "group": "设计实验结果", "editor": "ex_wintextareareadonly" },
    { "name": "有效链接及链接说明", "index": 0, "id": "SY_Link", "group": "设计实验结果" },
    { "name": "附件清单以及说明", "index": 0, "id": "SY_Attach", "group": "设计实验结果" },
    { "name": "补充说明", "index": 0, "id": "SY_Remarks", "group": "设计实验结果", "editor": "ex_wintextareareadonly" },
    { "name": "上次修改时间", "index": 0, "id": "SY_ModifyTime", "group": "设计实验结果" }
];

var SJSYShenHeOriginResult = undefined;
var SJSYShenHePropertygridData = [
    {
        "name": "加载设计实验审核结果", "group": "设计实验审核结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadSheJiShiYanShenHeHistoryData
            }
        }
    },
    { "name": "审核人", "index": 0, "id": "SJSYSH_ShenHeRenName", "group": "设计实验审核结果" },
    { "name": "审核时间", "index": 0, "id": "SJSYSH_ShenHeShiJian", "group": "设计实验审核结果" },
    { "name": "审核结果", "index": 0, "id": "SJSYSH_ShenHeJieGuo", "group": "设计实验审核结果" },
    { "name": "审核意见", "index": 0, "id": "SJSYSH_ShenHeYiJian", "group": "设计实验审核结果", "editor": "ex_wintextareareadonly" },
    { "name": '有效链接及链接说明', "index": 0, "id": "SJSYSH_Link", "group": "设计实验审核结果" },
    { "name": '附件清单以及说明', "index": 0, "id": "SJSYSH_Attach", "group": "设计实验审核结果" },
    { "name": "上次修改时间", "index": 0, "id": "SJSYSH_ModifyTime", "group": "设计实验审核结果" }
];

var DBFXOriginResult = undefined;
var DBFXPropertygridData = [
    {
        "name": "加载量化对比分析结果", "group": "量化对比分析结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadDuiBiFenXiHistoryData
            }
        }
    },
    { "name": "量化对比分析编号", "index": 0, "id": "DBFX_No", "group": "量化对比分析结果" },
    { "name": "量化对比分析名称", "index": 0, "id": "DBFX_Name", "group": "量化对比分析结果" },
    { "name": "量化对比分析者", "index": 0, "id": "DBFX_ProposerName", "group": "量化对比分析结果" },
    { "name": "量化对比分析时间", "index": 0, "id": "DBFX_ProposeTime", "group": "量化对比分析结果" },
    { "name": "针对的设计实验", "index": 0, "id": "DBFX_FK_SYName", "group": "量化对比分析结果" },
    { "name": "关键字", "index": 0, "id": "DBFX_Keys", "group": "量化对比分析结果" },
    { "name": '描述', "index": 0, "id": "DBFX_Description", "group": "量化对比分析结果", "editor": "ex_wintextareareadonly" },
    { "name": '前沿方法描述', "index": 0, "id": "DBFX_Methods", "group": "量化对比分析结果", "editor": "ex_wintextareareadonly" },
    { "name": '不同特性数据集', "index": 0, "id": "DBFX_Data", "group": "量化对比分析结果", "editor": "ex_wintextareareadonly" },
    { "name": "量化对比分析结果", "index": 0, "id": "DBFX_AnalysisResult", "group": "量化对比分析结果", "editor": "ex_wintextareareadonly" },
    { "name": "推理类型", "index": 0, "id": "DBFX_InferType", "group": "量化对比分析结果" },
    { "name": "推理内容", "index": 0, "id": "DBFX_InferContent", "group": "量化对比分析结果", "editor": "ex_wintextareareadonly" },
    { "name": "有效链接及链接说明", "index": 0, "id": "DBFX_Link", "group": "量化对比分析结果" },
    { "name": "附件清单以及说明", "index": 0, "id": "DBFX_Attach", "group": "量化对比分析结果" },
    { "name": "补充说明", "index": 0, "id": "DBFX_Remarks", "group": "量化对比分析结果", "editor": "ex_wintextareareadonly" },
    { "name": "上次修改时间", "index": 0, "id": "DBFX_ModifyTime", "group": "量化对比分析结果" }
];

var DBFXShenHeOriginResult = undefined;
var DBFXShenHePropertygridData = [
    {
        "name": "加载量化对比分析审核结果", "group": "量化对比分析审核结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadDuiBiFenXiShenHeHistoryData
            }
        }
    },
    { "name": "审核人", "index": 0, "id": "DBFXSH_ShenHeRenName", "group": "量化对比分析审核结果" },
    { "name": "审核时间", "index": 0, "id": "DBFXSH_ShenHeShiJian", "group": "量化对比分析审核结果" },
    { "name": "审核结果", "index": 0, "id": "DBFXSH_ShenHeJieGuo", "group": "量化对比分析审核结果" },
    { "name": "审核意见", "index": 0, "id": "DBFXSH_ShenHeYiJian", "group": "量化对比分析审核结果", "editor": "ex_wintextareareadonly" },
    { "name": '有效链接及链接说明', "index": 0, "id": "DBFXSH_Link", "group": "量化对比分析审核结果" },
    { "name": '附件清单以及说明', "index": 0, "id": "DBFXSH_Attach", "group": "量化对比分析审核结果" },
    { "name": "上次修改时间", "index": 0, "id": "DBFXSH_ModifyTime", "group": "量化对比分析审核结果" }
];

var DCJLOriginResult = undefined;
var DCJLPropertygridData = [
    {
        "name": "加载得出结论审核结果", "group": "得出结论结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadDeChuJieLunHistoryData
            }
        }
    },
    { "name": "结论编号", "index": 0, "id": "DCJL_No", "group": "得出结论结果" },
    { "name": "结论名称", "index": 0, "id": "DCJL_Name", "group": "得出结论结果" },
    { "name": "结论得出者", "index": 0, "id": "DCJL_ProposerName", "group": "得出结论结果" },
    { "name": "结论得出时间", "index": 0, "id": "DCJL_ProposeTime", "group": "得出结论结果" },
    { "name": "针对的量化对比分析结果", "index": 0, "id": "DCJL_FK_DBFXName", "group": "得出结论结果" },
    { "name": "关键字", "index": 0, "id": "DCJL_Keys", "group": "得出结论结果" },
    { "name": '结论轻缓程度', "index": 0, "id": "DCJL_Mitigation", "group": "得出结论结果" },
    { "name": '结论描述', "index": 0, "id": "DCJL_Description", "group": "得出结论结果", "editor": "ex_wintextareareadonly" },
    { "name": '有效的解决思路', "index": 0, "id": "DCJL_EffectiveSolution", "group": "得出结论结果", "editor": "ex_wintextareareadonly" },
    { "name": "论据", "index": 0, "id": "DCJL_Arguments", "group": "得出结论结果", "editor": "ex_wintextareareadonly" },
    { "name": "有效链接及链接说明", "index": 0, "id": "DCJL_Link", "group": "得出结论结果" },
    { "name": "附件清单以及说明", "index": 0, "id": "DCJL_Attach", "group": "得出结论结果" },
    { "name": "补充说明", "index": 0, "id": "DCJL_Remarks", "group": "得出结论结果", "editor": "ex_wintextareareadonly" },
    { "name": "上次修改时间", "index": 0, "id": "DCJL_ModifyTime", "group": "得出结论结果" }
];


var DCJLShenHeOriginResult = undefined;
var DCJLShenHePropertygridData = [
    {
        "name": "加载得出结论审核结果", "group": "得出结论审核结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadDeChuJieLunShenHeHistoryData
            }
        }
    },
    { "name": "审核人", "index": 0, "id": "DCJLSH_ShenHeRenName", "group": "得出结论审核结果" },
    { "name": "审核时间", "index": 0, "id": "DCJLSH_ShenHeShiJian", "group": "得出结论审核结果" },
    { "name": "审核结果", "index": 0, "id": "DCJLSH_ShenHeJieGuo", "group": "得出结论审核结果" },
    { "name": "审核意见", "index": 0, "id": "DCJLSH_ShenHeYiJian", "group": "得出结论审核结果", "editor": "ex_wintextareareadonly" },
    { "name": '有效链接及链接说明', "index": 0, "id": "DCJLSH_Link", "group": "得出结论审核结果" },
    { "name": '附件清单以及说明', "index": 0, "id": "DCJLSH_Attach", "group": "得出结论审核结果" },
    { "name": "上次修改时间", "index": 0, "id": "DCJLSH_ModifyTime", "group": "得出结论审核结果" }
];

var LWZXOriginResult = undefined;
var LWZXPropertygridData = [
    {
        "name": "加载论文或专利撰写结果", "group": "论文或专利撰写结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadLunWenZhuanXieHistoryData
            }
        }
    },
    { "name": "编号", "index": 0, "id": "LWZX_No", "group": "论文或专利撰写结果" },
    { "name": "名称", "index": 0, "id": "LWZX_Name", "group": "论文或专利撰写结果" },
    { "name": "撰写人", "index": 0, "id": "LWZX_ProposerName", "group": "论文或专利撰写结果" },
    { "name": "撰写时间", "index": 0, "id": "LWZX_ProposeTime", "group": "论文或专利撰写结果", },
    { "name": "序言", "index": 0, "id": "LWZX_Description", "group": "论文或专利撰写结果", "editor": "ex_wintextareareadonly" },
    { "name": "关键字", "index": 0, "id": "LWZX_Keys", "group": "论文或专利撰写结果" },
    { "name": '研究动机', "index": 0, "id": "LWZX_Motivation", "group": "论文或专利撰写结果", "editor": "ex_wintextareareadonly" },
    { "name": '问题', "index": 0, "id": "LWZX_Questions", "group": "论文或专利撰写结果", "editor": "ex_wintextareareadonly" },
    { "name": '设计', "index": 0, "id": "LWZX_Design", "group": "论文或专利撰写结果", "editor": "ex_wintextareareadonly" },
    { "name": "实现", "index": 0, "id": "LWZX_Realize", "group": "论文或专利撰写结果", "editor": "ex_wintextareareadonly" },
    { "name": '测试数据集', "index": 0, "id": "LWZX_TestData", "group": "论文或专利撰写结果", "editor": "ex_wintextareareadonly" },
    { "name": "结论", "index": 0, "id": "LWZX_Result", "group": "论文或专利撰写结果", "editor": "ex_wintextareareadonly" },
    { "name": "有效链接及链接说明", "index": 0, "id": "LWZX_Link", "group": "论文或专利撰写结果" },
    { "name": "附件清单以及说明", "index": 0, "id": "LWZX_Attach", "group": "论文或专利撰写结果" },
    { "name": "补充说明", "index": 0, "id": "LWZX_Remarks", "group": "论文或专利撰写结果", "editor": "ex_wintextareareadonly" },
    { "name": "上次修改时间", "index": 0, "id": "LWZX_ModifyTime", "group": "论文或专利撰写结果" }
];

var LWZXShenHeOriginResult = undefined;
var LWZXShenHePropertygridData = [
    {
        "name": "加载论文或专利撰写审核结果", "group": "论文或专利撰写审核结果", "type": "load", "editor": {
            type: "ex_panel",
            options: {
                callback: _LoadHistory.loadLunWenZhuanXieShenHeHistoryData
            }
        }
    },
    { "name": "审核人", "index": 0, "id": "LWZXSH_ShenHeRenName", "group": "论文或专利撰写审核结果" },
    { "name": "审核时间", "index": 0, "id": "LWZXSH_ShenHeShiJian", "group": "论文或专利撰写审核结果" },
    { "name": "审核结果", "index": 0, "id": "LWZXSH_ShenHeJieGuo", "group": "论文或专利撰写审核结果" },
    { "name": "审核意见", "index": 0, "id": "LWZXSH_ShenHeYiJian", "group": "论文或专利撰写审核结果", "editor": "ex_wintextareareadonly" },
    { "name": '有效链接及链接说明', "index": 0, "id": "LWZXSH_Link", "group": "论文或专利撰写审核结果" },
    { "name": '附件清单以及说明', "index": 0, "id": "LWZXSH_Attach", "group": "论文或专利撰写审核结果" },
    { "name": "上次修改时间", "index": 0, "id": "LWZXSH_ModifyTime", "group": "论文或专利撰写审核结果" }
];


var OthersShenHePropertygridData = [
            {
                "name": "加载其他人的审核结果", "group": "其他人的审核结果", "type": "load", "editor": {
                    type: "ex_panel",
                    options: {
                        callback: _LoadHistory.loadShenHeHistoryDataWithoutCurrentLoginUser
                    }
                }
            },
            { "name": "审核人", "index": 0, "id": "NOTCURRENTLOGINUSERSH_ShenHeRenName", "group": "其他人的审核结果" },
            { "name": "审核时间", "index": 0, "id": "NOTCURRENTLOGINUSERSH_ShenHeShiJian", "group": "其他人的审核结果" },
            { "name": "审核结果", "index": 0, "id": "NOTCURRENTLOGINUSERSH_ShenHeJieGuo", "group": "其他人的审核结果" },
            { "name": "审核意见", "index": 0, "id": "NOTCURRENTLOGINUSERSH_ShenHeYiJian", "group": "其他人的审核结果", "editor": "ex_wintextareareadonly" },
            { "name": '有效链接及链接说明', "index": 0, "id": "NOTCURRENTLOGINUSERSH_Link", "group": "其他人的审核结果" },
            { "name": '附件清单以及说明', "index": 0, "id": "NOTCURRENTLOGINUSERSH_Attach", "group": "其他人的审核结果" },
            { "name": "上次修改时间", "index": 0, "id": "NOTCURRENTLOGINUSERSH_ModifyTime", "group": "其他人的审核结果" }
];