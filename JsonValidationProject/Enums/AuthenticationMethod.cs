using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonValidationProject.Enums
{
    public enum AuthenticationMethod
    {
        [Display(Name = "Card has no restrictions")]
        LackOfRestriction,
        [Display(Name = "Pin required")]
        PinRequired

    }
}
