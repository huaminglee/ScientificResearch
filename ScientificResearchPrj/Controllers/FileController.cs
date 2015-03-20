using ScientificResearchPrj.Controllers.Base;
using ScientificResearchPrj.IBLL;
using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ScientificResearchPrj.Controllers
{
    public class FileController : ProcessBase<IAttachService>
    {
        public override void SetCurrentService()
        {
            this.CurrentService = SrService.AttachService;
        }

        public ActionResult Upload()
        {
            try
            {
                string work_id = Request.Form["WorkID"];
                string fk_node = Request.Form["FK_Node"];
                string filename = "";
                HttpPostedFileBase postFile = Request.Files[0];//get post file   
                string saveToPath = "";
                if (postFile.ContentLength > 0)
                {
                    filename = Path.GetFileName(postFile.FileName);
                    saveToPath = Server.MapPath("/DataUser/MyFlowFile/Upload/" + work_id + "/ND" + fk_node);
                    if (!Directory.Exists(saveToPath))
                        Directory.CreateDirectory(saveToPath);
                    saveToPath = saveToPath + "/" + filename;
                    
                    if (System.IO.File.Exists(saveToPath)) {
                        throw new Exception("服务器已存在同名文件，请先修改文件名再上传");
                    }
                    
                    postFile.SaveAs(saveToPath);
                }

                return Json(new
                {
                    state = "0",
                    message = "保存成功",
                    filePath = "/DataUser/MyFlowFile/Upload/" + work_id + "/ND" + fk_node + "/" + filename
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    state = "-1",
                    message = e.Message
                });
            }
        }
         
        public ActionResult GetFileHistoryData(string No_OID, string isShenHe)
        {
            List<Process_Attach> fileList = CurrentService.GetHistoryData(No_OID, isShenHe);

            if (fileList.Count == 0)
            {
                return Json(new
                {
                    state = "-1",
                    message = "附件为空"
                });
            }
            else
            {
                return Json(new
                {
                    state = "0",
                    message = "成功加载附件",
                    _Json = EasyUIJson.GetEasyUIJson_File(fileList)
                });
            }
        }

        public ActionResult TianJiaFile(Process_Attach file)
        {
            Dictionary<string, string> dictionary = CurrentService.AddAttach(file);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"],
                OID = dictionary["OID"]
            });
        }

        public ActionResult ShanChuFile(int OID)
        {
            Process_Attach file = CurrentService.GetAttachByOID(OID);
            if (file != null) {
                System.IO.File.Delete(Server.MapPath(file.Path));
            }

            Dictionary<string, string> dictionary = CurrentService.DeleteAttach(OID);

            return Json(new
            {
                state = dictionary["state"],
                message = dictionary["message"]
            });
        }
    }
}
