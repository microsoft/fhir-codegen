// <copyright file="EventIdConverter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Data.Converters;
using Microsoft.Extensions.Logging;

namespace fhir_codegen_app.Logging;

public class EventIdConverter : IValueConverter
{
    /// <summary>  
    /// Converts an <see cref="EventId"/> to its string representation.  
    /// </summary>  
    /// <param name="value">The value to convert, expected to be of type <see cref="EventId"/>.</param>  
    /// <param name="targetType">The type of the binding target property.</param>  
    /// <param name="parameter">The converter parameter to use.</param>  
    /// <param name="culture">The culture to use in the converter.</param>  
    /// <returns>A string representation of the <see cref="EventId"/>.</returns>  
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return "0";

        EventId eventId = (EventId)value;

        return eventId.ToString();
    }

    /// <summary>  
    /// Converts a string back to an <see cref="EventId"/>.  
    /// </summary>  
    /// <param name="value">The value to convert back, expected to be a string.</param>  
    /// <param name="targetType">The type to convert to.</param>  
    /// <param name="parameter">The converter parameter to use.</param>  
    /// <param name="culture">The culture to use in the converter.</param>  
    /// <returns>An <see cref="EventId"/> with the specified name and an ID of 0.</returns>  
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => new EventId(0, value?.ToString() ?? string.Empty);
}
