using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificResearchPrj.Model
{
    public class EmpForJson
    {
        public string EmpNo { get; set; }
        public string Name { get; set; }
        public string Pass { get; set; }
        public string SID { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }

        public string AdmissionYear { get; set; }
        public string SchoolingLength { get; set; }
        public string FK_Tutor { get; set; }
        public string LabAddr { get; set; }

        public string FK_Dept { get; set; }
        public string FK_Station { get; set; }
        public string ChargeWork { get; set; }
        public string OfficeAddr { get; set; }
        public string OfficeTel { get; set; }
    }
}
