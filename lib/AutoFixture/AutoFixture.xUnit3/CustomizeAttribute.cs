﻿using System;
using System.Reflection;

namespace AutoFixture.Xunit3
{
    /// <summary>
    /// Base class for customizing parameters in methods decorated with
    /// <see cref="AutoDataAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public abstract class CustomizeAttribute : Attribute, IParameterCustomizationSource
    {
        /// <summary>
        /// Gets a customization for a parameter.
        /// </summary>
        /// <param name="parameter">The parameter for which the customization is requested.</param>
        /// <returns>A customization for the parameter.</returns>
        public abstract ICustomization GetCustomization(ParameterInfo parameter);
    }
}