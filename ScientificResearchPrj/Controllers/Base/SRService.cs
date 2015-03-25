using ScientificResearchPrj.BLL;
using ScientificResearchPrj.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScientificResearchPrj.Controllers.Base
{
    public class SRService
    {
        public IProcessAllService ProcessAllService
        {
            get { return new ProcessAllService(); }
        }

        public IProjectService ProjectService
        {
            get { return new ProjectService(); }
        }

        public ISubjectService SubjectService 
        {
            get { return new SubjectService(); } 
        }

        public IShenHeService ShenHeService
        {
            get { return new ShenHeService(); }
        }

        public ICommonOperationService CommonOperationService
        {
            get { return new CommonOperationService(); }
        }
        
        public IXuQiuFenXiService XuQiuFenXiService
        {
            get { return new XuQiuFenXiService(); }
        }

        public IDiaoYanService DiaoYanService
        {
            get { return new DiaoYanService(); }
        }

        public ITiChuWenTiService TiChuWenTiService
        {
            get { return new TiChuWenTiService(); }
        }

        public IJieJueSiLuService JieJueSiLuService
        {
            get { return new JieJueSiLuService(); }
        }

        public IXingShiHuaService XingShiHuaService
        {
            get { return new XingShiHuaService(); }
        }

        public ISheJiSuanFaService SheJiSuanFaService
        {
            get { return new SheJiSuanFaService(); }
        }

        public ISheJiShiYanService SheJiShiYanService
        {
            get { return new SheJiShiYanService(); }
        }

        public IDuiBiFenXiService DuiBiFenXiService
        {
            get { return new DuiBiFenXiService(); }
        }

        public IDeChuJieLunService DeChuJieLunService
        {
            get { return new DeChuJieLunService(); }
        }
         
        public ILunWenZhuanXieService LunWenZhuanXieService
        {
            get { return new LunWenZhuanXieService(); }
        }

        public IChaoSongService ChaoSongService
        {
            get { return new ChaoSongService(); }
        }

        public IDaiBanService DaiBanService
        {
            get { return new DaiBanService(); }
        }

        public IFaQiService FaQiService
        {
            get { return new FaQiService(); }
        }

        public ILiuChengSheJiService LiuChengSheJiService
        {
            get { return new LiuChengSheJiService(); }
        }

        public IZaiTuService ZaiTuService
        {
            get { return new ZaiTuService(); }
        }

        public ILinkService LinkService
        {
            get { return new LinkService(); }
        }

        public IAttachService AttachService
        {
            get { return new AttachService(); }
        }

        public IJournalService JournalService
        {
            get { return new JournalService(); }
        }
    }
}