//链接、附件-------------------------
var IsShenHe = {
    "YES": "1",
    "NO": "0"
}

var CanLinkDelete = {
    "YES": true,
    "NO": false
}

var CanAttachDelete = {
    "YES": true,
    "NO": false
}


//选择人员---------------------------
var Type = {
    DAOSHI: 1,//导师
    XUESHENG: 2,//学生
    BENKESHENG: 3,//本科生
    YANJIUSHENG: 4,//研究生
    BOSHISHENG: 5//博士生
};
//用以查询
var TypeData = [
   { value: Type.DAOSHI, text: "导师" },
   { value: Type.XUESHENG, text: "学生" },
   { value: Type.BENKESHENG, text: "本科生" },
   { value: Type.YANJIUSHENG, text: "研究生" },
   { value: Type.BOSHISHENG, text: "博士生" }
];
//用以datagrid选择
var TypeDataForSelect = [
   { value: Type.DAOSHI, text: "导师" },
   { value: Type.BENKESHENG, text: "本科生" },
   { value: Type.YANJIUSHENG, text: "研究生" },
   { value: Type.BOSHISHENG, text: "博士生" }
];

//以前节点信息，用于抄送等
var PreviousNodesInfoData = [];