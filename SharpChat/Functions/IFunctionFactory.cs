using SharpChat.Functions.Model;
using System;

namespace SharpChat.Functions
{
    /// <summary>
    /// Service for creating <see cref="SharpFunction"/> instances
    /// </summary>
    internal interface IFunctionFactory
    {
        /// <summary>
        /// Create a <see cref="SharpFunction"/> from a method delegate
        /// Attention: The delegate must not be anonymous!
        /// </summary>
        Function CreateFunction(object target, string name);
    }
}