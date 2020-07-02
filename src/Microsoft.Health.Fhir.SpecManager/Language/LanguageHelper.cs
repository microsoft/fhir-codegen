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
                _languagesByName.Add(language.LanguageName.ToUpperInvariant(), language);
            }

            _initialized = true;
        }

        /// <summary>Gets the language interfaces for the requested languages.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The languages.</returns>
        public static List<ILanguage> GetLanguages(string name)
        {
            List<ILanguage> languages = new List<ILanguage>();

            if (!_initialized)
            {
                Init();
            }

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            if (_languagesByName.ContainsKey(name.ToUpperInvariant()))
            {
                languages.Add(_languagesByName[name.ToUpperInvariant()]);
                return languages;
            }

            if (name.Contains('|'))
            {
                string[] names = name.Split('|');

                foreach (string n in names)
                {
                    if (_languagesByName.ContainsKey(n.ToUpperInvariant()))
                    {
                        languages.Add(_languagesByName[n.ToUpperInvariant()]);
                    }
                }
            }

            return languages;
        }
    }
}
