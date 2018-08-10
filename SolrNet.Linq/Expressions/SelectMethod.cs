﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SolrNet.Commands.Parameters;
using SolrNet.Linq.Expressions.Context;
using SolrNet.Linq.Impl;

namespace SolrNet.Linq.Expressions.NodeTypeHelpers
{
    public static class SelectMethod
    {
        public const string Select = nameof(Queryable.Select);
        public static bool TryVisitSelect(this MethodCallExpression node, SelectExpressionsCollection options, MemberContext context, out SelectContext newContext)
        {
            newContext = null;
            bool result = node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == Select;
            if (result)
            {
                Expression arg = node.Arguments[1];

                if (arg.NodeType == ExpressionType.Quote)
                {
                    LambdaExpression lambda = (LambdaExpression)node.Arguments[1].StripQuotes();

                    if (lambda.Body is NewExpression selectMember)
                    {
                        newContext = new SelectContext(selectMember, context);                        
                    }

                    if (lambda.Body is MemberInitExpression memberInit)
                    {
                        newContext = new SelectContext(memberInit, context);
                    }

                    if (newContext != null)
                    {
                        options.Fields.Clear();
                        SelectFieldsVisitor visitor = new SelectFieldsVisitor(context, options);
                        visitor.Visit(lambda.Body);                                                

                        return result;
                    }
                }

                throw new InvalidOperationException($"Unable to translate '{Select}' method.");
            }

            return result;
        }
    }
}