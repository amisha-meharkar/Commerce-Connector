using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransformTableData
{
    public interface IExpressionEvaluator
    {
        /// <summary>
        /// Evaluates a transition expression with placeholders replaced by actual values from a DataRow.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="row">The DataRow containing the values to replace in the expression.</param>
        /// <returns>The evaluated expression result.</returns>
        object EvaluateTransitionExpression(string expression, DataRow row);

        /// <summary>
        /// Evaluates a substring expression in the format SUBSTRING("string", startIndex, length).
        /// </summary>
        /// <param name="expression">The substring expression to evaluate.</param>
        /// <returns>The extracted substring or the original expression if invalid.</returns>
        object EvaluateSubstringExpression(string expression);

        /// <summary>
        /// Evaluates a string concatenation expression.
        /// </summary>
        /// <param name="expression">The concatenation expression to evaluate.</param>
        /// <returns>The concatenated string.</returns>
        string EvaluateStringConcatenation(string expression);

        /// <summary>
        /// Checks if the given expression is a mathematical expression.
        /// </summary>
        /// <param name="expression">The expression to check.</param>
        /// <returns>True if it is a mathematical expression; otherwise, false.</returns>
        bool IsMathExpression(string expression);
        bool IsCriticalExpression(string expression);
        /// <summary>
        /// Evaluates a mathematical expression.
        /// </summary>
        /// <param name="expression">The mathematical expression to evaluate.</param>
        /// <returns>The result of the mathematical expression or the original expression if evaluation fails.</returns>
        object EvaluateMathExpression(string expression);
    }
}
