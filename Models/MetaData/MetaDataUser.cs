using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace projectdbfirst.Models.MetaData
{
    public class MetaDataUser
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "UserName is Null")]
        public string Username { get; set; }
        [Required(ErrorMessage = "PassWord is Null")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}