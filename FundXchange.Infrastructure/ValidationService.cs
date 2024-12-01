using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace FundXchange.Infrastructure
{
    public class ValidationService
    {
        private string _RuleSet = "GeneralRuleSet";
        public string RuleSet
        {
            get
            {
                return _RuleSet;
            }
            set
            {
                _RuleSet = value;
            }
        }

        public ValidationService()
        {
        }

        public void Validate(List<object> entities)
        {
            ValidationException exception = new ValidationException();

            foreach (object entity in entities)
            {
                Validator validator = ValidationFactory.CreateValidator(entity.GetType(), RuleSet);
                ValidationResults results = validator.Validate(entity);
                foreach (ValidationResult result in results)
                {
                    exception.AddValidationResult(result);
                }
            }
            if (exception.ValidationExceptions.Count > 0)
                throw exception;
        }
    }
}
