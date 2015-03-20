using ScientificResearchPrj.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.IBLL 
{
    public interface ISubjectService : IBaseService<Process_Subject_XuQiuFenXi>
    {
        PageModel<Process_Subject_XuQiuFenXi> GetSubjects(int pageSize, int pageNow);

        Process_Subject_XuQiuFenXi GetSubjectByNo(string subNo);

        Dictionary<string, string> AddSubject(Process_Subject_XuQiuFenXi subject);

        Dictionary<string, string> DeleteSubject(string subNo);

        Dictionary<string, string> ModifySubject(string oldNo, Process_Subject_XuQiuFenXi subject);

        Process_Subject_XuQiuFenXi GetSubjectHistoryDataFromTrack(CCFlowArgs args);
    }
}