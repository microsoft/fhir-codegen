// <copyright file="LanguageHelper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Language
{
    /// <summary>A language helper.</summary>
    public abstract class LanguageHelper
    {
        /// <summary>Language exporters by name.</summary>
        private static Dictionary<string, ILanguage> _languagesByName = new Dictionary<string, ILanguage>();

        /// <summary>True if initialized.</summary>
        private static bool _initialized = false;

        private static void Init()
        {
            if (_initialized)
            {
                return;
            }

            IEnumerable<Type> localTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetInterfaces().Contains(typeof(ILanguage)));

            foreach (Type localType in localTypes)
            {
                ILanguage language = (ILanguage)Activator.CreateInstance(localType);
                _languagesByName.Add(language.LanguageName.ToLowerInvariant(), language);
            }

            _initialized = true;
        }

        /// <summary>Gets a language.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The language.</returns>
        public static ILanguage GetLanguage(string name)
        {
            if (!_initialized)
            {
                Init();
            }

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (!_languagesByName.ContainsKey(name.ToLowerInvariant()))
            {
                return null;
            }

            return _languagesByName[name.ToLowerInvariant()];
        }
    }
}
