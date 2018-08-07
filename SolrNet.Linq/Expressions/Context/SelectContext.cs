﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SolrNet.Linq.Expressions.Context
{
    public class SelectContext : MemberContext
    {
        public NewExpression Expression { get; }
        public MemberContext ParentContext { get; }

        public Dictionary<MemberInfo, string> Members { get; } = new Dictionary<MemberInfo, string>();
        public Dictionary<MemberInfo, string> Aliases { get; } = new Dictionary<MemberInfo, string>();

        public SelectContext(NewExpression expression, MemberContext parentContext)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            ParentContext = parentContext ?? throw new ArgumentNullException(nameof(parentContext));

            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                Expression argument = expression.Arguments[i];
                if (argument.NodeType != ExpressionType.MemberAccess)
                {
                    string value = $"v{i}:{parentContext.GetSolrMemberProduct(argument)}";
                    Aliases.Add(expression.Members[i], value);
                }
                else
                {
                    Members.Add(expression.Members[i], parentContext.GetSolrMemberProduct(argument, true));
                }
            }            
        }
        public override bool HasMemberAccess(Expression expression)
        {
            throw new System.NotImplementedException();
        }

        public override string GetSolrMemberProduct(Expression expression, bool disableFunctions = false)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsAccessToMember(MemberExpression expression)
        {
            return Members.ContainsKey(expression.Member);
        }
    }
}