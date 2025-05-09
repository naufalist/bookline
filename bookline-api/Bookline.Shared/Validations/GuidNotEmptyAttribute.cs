using System;
using System.ComponentModel.DataAnnotations;

namespace Bookline.Shared.Validations
{
    public class GuidNotEmptyAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (value is Guid guid)
            {
                return guid != Guid.Empty;
            }

            return false;
        }
    }
}