using System;
using System.Linq;
using System.Linq.Expressions;

namespace MS.IoT.Repositories.Helpers
{
    public static class PredicateBuilder
    {
        public static Func<T, bool> True<T>() { return f => true; }
        public static Func<T, bool> False<T>() { return f => false; }

        public static Func<T, bool> Or<T>(this Func<T, bool> expr1,
                                                            Func<T, bool> expr2)
        {
            return t => expr1(t) || expr2(t);
        }

        public static Func<T, bool> And<T>(this Func<T, bool> expr1,
                                                             Func<T, bool> expr2)
        {
            return t => expr1(t) && expr2(t);
        }
    }
}
