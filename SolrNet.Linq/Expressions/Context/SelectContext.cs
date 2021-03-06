﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SolrNet.Linq.Expressions.Context
{
    public class SelectContext : MemberContext
    {
        public NewExpression NewExpression { get; }
        public MemberContext ParentContext { get; }

        public Dictionary<MemberInfo, string> Members { get; } = new Dictionary<MemberInfo, string>();
        public Dictionary<MemberInfo, string> Aliases { get; } = new Dictionary<MemberInfo, string>();

        public Dictionary<MemberInfo, Expression> Calculated { get; } = new Dictionary<MemberInfo, Expression>();

        public SelectContext(NewExpression expression, MemberContext parentContext)
        {
            NewExpression = expression ?? throw new ArgumentNullException(nameof(expression));
            ParentContext = parentContext ?? throw new ArgumentNullException(nameof(parentContext));
            
            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                Expression argument = expression.Arguments[i];
                if (argument is MemberExpression me && parentContext.IsAccessToMember(me))
                {
                    Members.Add(expression.Members[i], parentContext.GetSolrMemberProduct(argument, true));
                }
                else if (argument is MethodCallExpression mc &&
                         (mc.Method.DeclaringType == typeof(SolrExpr.Transformers) ||
                          mc.Method.DeclaringType == typeof(SolrExpr.Fields)))
                {
                    Aliases.Add(expression.Members[i], parentContext.GetSolrMemberProduct(argument));
                }
                else
                {
                    Calculated.Add(expression.Members[i], argument);
                }
            }            
        }

        public SelectContext(MemberInitExpression expression, MemberContext parentContext)
        {
            NewExpression = expression?.NewExpression ?? throw new ArgumentNullException(nameof(expression));
            ParentContext = parentContext ?? throw new ArgumentNullException(nameof(parentContext));

            foreach (MemberAssignment binding in expression.Bindings.OfType<MemberAssignment>())
            {
                if (binding.Expression is MemberExpression me && parentContext.IsAccessToMember(me))
                {
                    Members.Add(binding.Member, parentContext.GetSolrMemberProduct(binding.Expression, true));
                }
                else if (binding.Expression is MethodCallExpression mc &&
                         (mc.Method.DeclaringType == typeof(SolrExpr.Transformers) ||
                          mc.Method.DeclaringType == typeof(SolrExpr.Fields)))
                {
                    Aliases.Add(binding.Member, parentContext.GetSolrMemberProduct(binding.Expression));
                }
                else
                {
                    Calculated.Add(binding.Member, binding.Expression);
                }
            }
        }

        public override bool HasMemberAccess(Expression expression)
        {
            bool hasMemberAccess = expression.HasMemberAccess(this.NewExpression.Type);
            return hasMemberAccess;
        }

        public override string GetSolrMemberProduct(Expression expression, bool disableFunctions = false)
        {
            if (expression is MemberExpression me)
            {
                if (Members.ContainsKey(me.Member))
                {
                    return Members[me.Member];
                }

                if (Aliases.ContainsKey(me.Member) && disableFunctions == false)
                {
                    return Aliases[me.Member];
                }

                if (Calculated.ContainsKey(me.Member) && disableFunctions == false)
                {
                    return ParentContext.GetSolrMemberProduct(Calculated[me.Member]);
                }
            }

            return expression.GetSolrMemberProduct(this, disableFunctions);
        }

        public override string GetMemberSolrName(MemberInfo info)
        {
            if (Members.ContainsKey(info))
            {
                return Members[info];
            }

            throw new InvalidOperationException($"Member {info.Name} of type {info.DeclaringType} is calculated field and can't be used in methods other than Select");
        }
        public override bool IsAccessToMember(MemberExpression expression)
        {
            return Members.ContainsKey(expression.Member);
        }
    }
}