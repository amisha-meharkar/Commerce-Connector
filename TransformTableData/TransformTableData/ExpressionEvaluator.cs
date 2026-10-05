using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace TransformTableData
{
    public class ExpressionEvaluator : IExpressionEvaluator
    {
        public object EvaluateTransitionExpression(string expression, DataRow row)
        {
            try
            {
                // Replace column placeholders with actual values from the DataRow
                foreach (DataColumn column in row.Table.Columns)
                {
                    string columnName = column.ColumnName;
                    string columnValue = row[columnName]?.ToString() ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(columnValue))
                    {
                        // For math or substring expressions, return blank if any value is missing
                        if (IsCriticalExpression(expression))
                        {
                            return string.Empty;
                        }

                        // Replace the placeholder with an empty string for non-critical expressions
                        expression = expression.Replace("[" + columnName + "]", "\"\"");
                        continue;
                    }



                    // Check if the column value is numeric
                    if (decimal.TryParse(columnValue, out decimal numericValue))
                    {
                        // Replace placeholder with numeric value
                        expression = expression.Replace("[" + columnName + "]", numericValue.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        // Replace placeholder with string value (enclose in quotes if not numeric)
                        expression = expression.Replace("[" + columnName + "]", "\"" + columnValue + "\"");
                    }
                }

                // Determine if the expression is mathematical or string concatenation
                if (IsMathExpression(expression))
                {
                    return EvaluateMathExpression(expression);
                }
                else if (expression.StartsWith("SUBSTRING(", StringComparison.OrdinalIgnoreCase))
                {
                    return EvaluateSubstringExpression(expression);
                }
                else if (expression.Contains("\"") || expression.Contains("+"))
                {
                    return EvaluateStringConcatenation(expression);
                }

                return expression; // Return the evaluated expression
            }
            catch (Exception ex)
            {
                return expression; // Return the original expression in case of error
            }
        }
        public bool IsCriticalExpression(string expression)
        {
            return expression.Contains("*") || expression.Contains("/") ||
                   expression.Contains("-") || expression.StartsWith("SUBSTRING(", StringComparison.OrdinalIgnoreCase);
        }

        public object EvaluateSubstringExpression(string expression)
        {
            try
            {
                var regex = new Regex(@"SUBSTRING\(([^,]+),\s*(\d+),\s*(\d+)\)");
                var match = regex.Match(expression);

                if (match.Success)
                {
                    string inputString = match.Groups[1].Value.Trim().Trim('\"');
                    int startIndex = int.Parse(match.Groups[2].Value);
                    int length = int.Parse(match.Groups[3].Value);

                    return inputString.Substring(startIndex, length);
                }

                return expression;
            }
            catch (Exception ex)
            {
                return expression;
            }
        }

        public string EvaluateStringConcatenation(string expression)
        {
            try
            {
                var parts = expression.Split('+').Select(p => p.Trim()).ToList();
                string result = string.Empty;

                foreach (var part in parts)
                {
                    if (part.StartsWith("\"") && part.EndsWith("\""))
                    {
                        result += part.Trim('"');
                    }
                    else
                    {
                        result += part;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return expression;
            }
        }

        public bool IsMathExpression(string expression)
        {
            return !expression.Contains("\"") &&
                   (expression.Contains("+") || expression.Contains("-") || expression.Contains("*") || expression.Contains("/"));
        }

        public object EvaluateMathExpression(string expression)
        {
            try
            {
                var result = new System.Data.DataTable().Compute(expression, null);
                return result;
            }
            catch (Exception ex)
            {
                return expression;
            }
        }
    }
}
