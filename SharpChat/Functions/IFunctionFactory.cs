using SharpChat.Functions.Model;
using System;

namespace SharpChat.Functions
{
    /// <summary>
    /// Service for creating <see cref="Function"/> instances
    /// </summary>
    internal interface IFunctionFactory
    {
        /// <summary>
        /// Create a <see cref="Function"/> from a method delegate
        /// Attention: The delegate must not be anonymous!
        /// </summary>
        Function CreateFunction(object target, string name);
    }
}