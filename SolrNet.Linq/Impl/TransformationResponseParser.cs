﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using SolrNet.Impl;
using SolrNet.Linq.Expressions;

namespace SolrNet.Linq.Impl
{
    public abstract class TransformationResponseParser<TNew, TOld> : ISolrDocumentResponseParser<TNew>
    {
        private readonly ISolrDocumentResponseParser<TOld> _inner;
        private readonly ISolrDocumentResponseParser<Dictionary<string, object>> _dictionaryParser;

        protected TransformationResponseParser(ISolrDocumentResponseParser<TOld> inner,
            ISolrDocumentResponseParser<Dictionary<string, object>> dictionaryParser)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _dictionaryParser = dictionaryParser ?? throw new ArgumentNullException(nameof(dictionaryParser));
        }

        public IList<TNew> ParseResults(XElement parentNode)
        {
            if (parentNode == null)
                return null;

            List<TNew> result = new List<TNew>();
            var docs = this._dictionaryParser.ParseResults(parentNode);
            IList<TOld> olds = this._inner.ParseResults(parentNode);

            for (int i = 0; i < olds.Count; i++)
            {
                result.Add(this.GetResult(olds[i], docs[i]));
            }

            return result;
        }

        protected abstract TNew GetResult(TOld old, Dictionary<string, object> dictionary);
    }
}