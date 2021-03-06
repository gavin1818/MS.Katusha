﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MS.Katusha.Web.Models.Entities;

namespace MS.Katusha.Web.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(14, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        [StringLength(64, ErrorMessage = "The {0} must be at least {2} and at most {1} vcharacters long.", MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(14, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class MailConfirmModel
    {
        public string UserName { get; set; }
    }

    public class RegisterModel : ProfileModel, IValidatableObject
    {
        [Required]
        [Display(Name = "User name")]
        [StringLength(64, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [StringLength(64, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 7)]
        public string Email { get; set; }

        [Display(Name = "Mobile phone number")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        public string Phone { get; set; }

        [Required]
        [StringLength(14, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public HttpPostedFileWrapper Photo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var list = new List<ValidationResult>();
            //if (Photo == null || Photo.ContentLength <= 0) {
            //    list.Add(new ValidationResult("You must add a photo", new[] {"Photo"}));
            //} else if ((byte) Gender <= 0 || (byte) Gender > 2) {
            //    list.Add(new ValidationResult("You must select a gender", new[] {"Gender", "gender2"}));
            //}
            return list;
        }
    }
}
