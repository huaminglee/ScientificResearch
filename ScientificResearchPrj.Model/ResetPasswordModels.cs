using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScientificResearchPrj.Model
{
    public class ResetPasswordModels
    {
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

       
        [DataType(DataType.EmailAddress)]
        [Display(Name = "邮箱")]
        public string Email { get; set; }
  
    }

    

    
}
