using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScientificResearchPrj.Model
{
    public class AccountModel
    {
        [Required]
        [Display(Name = "用户名")] 
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }
  
    }

    

    
}
