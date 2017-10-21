using System;

namespace Meyer.Socrates.Data
{
    internal interface IResolver<in TIn, out TOut>
    {
        TOut Resolve(TIn input);
    }
}
