using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Commerce_Connector_Factory.Validator
{
    public class DynamicValidator
    {
        public List<string> ValidateRow(Dictionary<string, string> row)
        {
            var errorMessages = new List<string>();

            // Loop through all key-value pairs in the row and apply generic rules
            foreach (var field in row)
            {
                var fieldName = field.Key;
                var fieldValue = field.Value;

                // Rule 1: Validate if the field is required and not empty
                if (string.IsNullOrWhiteSpace(fieldValue))
                {
                    errorMessages.Add($"{fieldName} is required.");
                }

                // Rule 2: Validate if the field should not be blank (additional rule)
                if (fieldValue == string.Empty)
                {
                    errorMessages.Add($"{fieldName} cannot be blank.");
                }

                // Rule 3: Validate if the field is numeric (e.g., Age or Quantity)
                if (fieldName.ToLower().Contains("age") || fieldName.ToLower().Contains("quantity"))
                {
                    if (!IsNumeric(fieldValue))
                    {
                        errorMessages.Add($"{fieldName} must be a valid number.");
                    }
                }

                // Rule 4: Validate if the field is an email (e.g., Email field)
                if (fieldName.ToLower().Contains("email"))
                {
                    if (!IsValidEmail(fieldValue))
                    {
                        errorMessages.Add($"{fieldName} must be a valid email address.");
                    }
                }

                // Add more dynamic rules based on field name patterns or data type logic...
            }

            return errorMessages;
        }

        // Method to check if a value is numeric
        private bool IsNumeric(string value)
        {
            return int.TryParse(value, out _) || decimal.TryParse(value, out _);
        }

        // Method to check if an email is valid
        private bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
