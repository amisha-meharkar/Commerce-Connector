using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Commerce_Connector_Factory.TransformationLogic
{
    public class TransformationExpressionEvaluator
    {

        public string Evaluate(string expression, Dictionary<string, string> properties)
        {
            try
            {
                // Replace all properties in the expression
                foreach (var property in properties)
                {
                    var placeholder = $"[{property.Key}]";
                    expression = expression.Replace(placeholder, property.Value.Trim('"'));
                }

                // Determine if the expression is numeric or string-based
                if (IsStringExpression(expression))
                {
                    return EvaluateStringConcatenation(expression);
                }
                else if (IsMathExpression(expression))
                {
                    return EvaluateMathExpression(expression)?.ToString() ?? string.Empty;
                }
                else if (expression.StartsWith("SUBSTRING(", StringComparison.OrdinalIgnoreCase))
                {
                    return EvaluateSubstringExpression(expression);
                }

                return expression;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while evaluating expression '{expression}': {ex.Message}");
                return expression;
            }
        }

        // Checks if the expression contains string operations
        private bool IsStringExpression(string expression)
        {
            return expression.Contains("\"") || expression.Contains("+") && !IsMathExpression(expression);
        }

        // Checks if the expression is purely numeric
        private bool IsMathExpression(string expression)
        {
            return !expression.Contains("\"") &&
                   Regex.IsMatch(expression, @"^[0-9+\-*/().\s]+$");
        }

        private object EvaluateMathExpression(string expression)
        {
            try
            {
                var result = new DataTable().Compute(expression, null);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error evaluating math expression '{expression}': {ex.Message}");
                return expression;
            }
        }

        private string EvaluateSubstringExpression(string expression)
        {
            try
            {
                var regex = new Regex(@"SUBSTRING\(([^,]+),\s*(\d+),\s*(\d+)\)");
                var match = regex.Match(expression);

                if (match.Success)
                {
                    string inputString = match.Groups[1].Value.Trim().Trim('"');
                    int startIndex = int.Parse(match.Groups[2].Value);
                    int length = int.Parse(match.Groups[3].Value);

                    return inputString.Substring(startIndex, length);
                }

                return expression;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error evaluating SUBSTRING expression '{expression}': {ex.Message}");
                return expression;
            }
        }

        private string EvaluateStringConcatenation(string expression)
        {
            try
            {
                // Split the expression into parts by the '+' operator
                var parts = expression.Split('+').Select(p => p.Trim()).ToList();

                // Convert each part to a string and concatenate
                var concatenatedResult = string.Concat(parts.Select(part =>
                {
                    if (decimal.TryParse(part.Trim('"'), out var number))
                    {
                        // If the part is a number, convert it to a string
                        return number.ToString();
                    }
                    else
                    {
                        // Otherwise, treat it as a string and remove surrounding quotes
                        return part.Trim('"');
                    }
                }));

                return concatenatedResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error evaluating string concatenation '{expression}': {ex.Message}");
                return expression;
            }
        }

    }
}
