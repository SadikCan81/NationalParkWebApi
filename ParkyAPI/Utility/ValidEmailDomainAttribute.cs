using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Utility
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string _allowedDomain;

        public ValidEmailDomainAttribute(string allowedDomain)
        {
            _allowedDomain = allowedDomain;
        }

        public override bool IsValid(object value)
        {
            string[] valueString = value.ToString().Split('@');

            var domain = valueString[1];

            return _allowedDomain.ToLower() == domain.ToLower();
        }
    }
}
