// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace ExcelMapper;

/// <summary>
///     Specifies the column name that is used when deserializing a property
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class ExcelColumnMultiAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of <see cref="ExcelColumnMultiAttribute" /> with the specified column name.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    public ExcelColumnMultiAttribute(int count)
    {
        if (count == 0) throw new ArgumentException($"Column count:{count}");

        Count = count;
    }

    /// <summary>
    ///     The name of the column.
    /// </summary>
    public int Count { get; }
}