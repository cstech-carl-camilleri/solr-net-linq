﻿namespace SolrNet.Linq.Expressions
{
    public enum EnumeratedResult
    {
        None = 0,
        First = 1,
        FirstOrDefault = 2,
        Single = 3,
        SingleOrDefault = 4,
        Any = 5,
        Count = 6,
        LongCount = 7
    }
}