using System.Net.Mail;

namespace Commerce_Connector_Factory.Validator
{
    public class DataPreprocessingAndValidation
    {
        public List<string> ValidateRow(Dictionary<string, string> row)
        {
            var errorMessages = new HashSet<string>(); // Use HashSet here too for row-level uniqueness

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

                //Rule 2: Validate if the field is an email
                if (fieldName.ToLower().Contains("email"))
                {
                    if (!IsValidEmail(fieldValue))
                    {
                        errorMessages.Add($"{fieldName} must be a valid email address.");
                    }
                }

            }

            return errorMessages.ToList(); // Convert HashSet back to List for returning
        }


        //// Method to check if a value is numeric
        //private bool IsNumeric(string value)
        //{
        //    return int.TryParse(value, out _) || decimal.TryParse(value, out _);
        //}

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

        //public async Task<List<Dictionary<string, string>>> PreprocessDataWithFlaskApi(List<Dictionary<string, string>> tableData)
        //{
        //    using (var httpClient = new HttpClient())
        //    {
        //        var jsonContent = JsonConvert.SerializeObject(tableData); // Convert the table data to JSON
        //        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //        // Send POST request to Flask API
        //        var response = await httpClient.PostAsync("http://localhost:5000/preprocess", content);
        //        response.EnsureSuccessStatusCode();

        //        // Receive preprocessed data from the Flask API
        //        var jsonResponse = await response.Content.ReadAsStringAsync();
        //        var preprocessedData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonResponse);

        //        return preprocessedData;  // Return the preprocessed data
        //    }
        //}
    }
}
