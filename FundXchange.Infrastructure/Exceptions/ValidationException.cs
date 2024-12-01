using System;
using System.Collections.Generic;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace FundXchange.Infrastructure
{
    public class ValidationException : DomainException
    {
        public Dictionary<object, List<string>> ValidationExceptions { get; private set; }

        public ValidationException()
        {
            ValidationExceptions = new Dictionary<object, List<string>>();
        }

        public void AddValidationResult(ValidationResult result)
        {
            if (!ValidationExceptions.ContainsKey(result.Target))
            {
                ValidationExceptions.Add(result.Target, new List<string>());
            }
            ValidationExceptions[result.Target].Add(result.Message);
        }
    }
}
