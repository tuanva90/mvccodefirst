using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Internal;

namespace Insyston.Operations.WPF.ViewModels.Common.Validation
{
    public class RecursiveValidatorSelector : IValidatorSelector
    {
        private readonly IEnumerable<string> memberNames;

        public RecursiveValidatorSelector(IEnumerable<string> memberNames)
        {
            this.memberNames = memberNames;
        }

        public static RecursiveValidatorSelector FromExpressions<T>(params Expression<Func<T, object>>[] propertyExpressions)
        {
            var members = propertyExpressions.Select(MemberFromExpression).ToList();
            return new RecursiveValidatorSelector(members);
        }

        public bool CanExecute(IValidationRule rule, string propertyPath, ValidationContext context)
        {
            bool result;
            string parentPropertyPath;
            string propertyChain;

            result = false;
            propertyChain = context.PropertyChain.BuildPropertyName(null);
            foreach (string memberName in this.memberNames)
            {
                parentPropertyPath = memberName;

                if (propertyChain.Length == 0)
                {
                    result = parentPropertyPath.StartsWith(propertyPath);
                }
                else if (propertyChain.Length < parentPropertyPath.Length)
                {
                    int length = parentPropertyPath.IndexOf(".", propertyChain.Length);
                    if (length > 0)
                    {
                        parentPropertyPath = parentPropertyPath.Substring(0, length);
                    }
                    result = propertyPath == parentPropertyPath;
                }
                
                if (result == true)
                {
                    break;
                }
            }

            return result;
        }

        private static string MemberFromExpression(LambdaExpression expression)
        {
            Expression body = expression.Body;
            if (body.NodeType == ExpressionType.Convert)
            {
                body = (body as UnaryExpression).Operand;
            }
            if (!(body is MemberExpression))
            {
                throw new ArgumentException(string.Format("Expression '{0}' does not specify a valid property or field.", expression));
            }

            return PropertyPathFromExpression((MemberExpression)body);
        }

        private static string PropertyPathFromExpression(MemberExpression expression)
        {
            if (expression.Expression == null || !(expression.Expression is MemberExpression))
            {
                return expression.Member.Name;
            }
            return string.Format("{0}.{1}", PropertyPathFromExpression(expression.Expression as MemberExpression), expression.Member.Name);
        }
    }
}