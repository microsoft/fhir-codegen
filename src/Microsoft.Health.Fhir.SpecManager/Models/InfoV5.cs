using Microsoft.Health.Fhir.SpecManager.fhir.v4;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    public class InfoV5
    {

        #region Class Variables . . .

        private static HashSet<string> _excludedResourceTypes;
        private static HashSet<string> _knownResourceTypes;

        #endregion Class Variables . . .

        #region Instance Variables . . .

        public Dictionary<string, CapabilityStatement> Capabilities { get; set; }

        public Dictionary<string, CodeSystem> CodeSystems { get; set; }
        
        public Dictionary<string, CompartmentDefinition> CompartmentDefinitions { get; set; }

        public Dictionary<string, ConceptMap> ConceptMaps { get; set; }

        public Dictionary<string, NamingSystem> NamingSystems { get; set; }

        public Dictionary<string, OperationDefinition> OperationDefinitions { get; set; }

        public Dictionary<string, SearchParameter> SearchParameters { get; set; }

        public Dictionary<string, StructureDefinition> StructureDefinitions { get; set; }

        public Dictionary<string, StructureMap> StructureMaps { get; set; }

        public Dictionary<string, ValueSet> ValueSets { get; set; }
        
        #endregion Instance Variables . . .

        #region Constructors . . .

        static InfoV5()
        {
            _knownResourceTypes = new HashSet<string>()
            {
                "CapabilityStatement",
                "CodeSystem",
                "CompartmentDefinition",
                "ConceptMap",
                "NamingSystem",
                "OperationDefinition",
                "SearchParameter",
                "StructureDefinition",
                "StructureMap",
                "ValueSet"
            };

            _excludedResourceTypes = new HashSet<string>()
            {

            };
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Default constructor.</summary>
        ///
        /// <remarks>Gino Canessa, 2/3/2020.</remarks>
        ///-------------------------------------------------------------------------------------------------

        public InfoV5()
        {
            // **** create dictionaries ****

            Capabilities = new Dictionary<string, CapabilityStatement>();
            CodeSystems = new Dictionary<string, CodeSystem>();
            CompartmentDefinitions = new Dictionary<string, CompartmentDefinition>();
            ConceptMaps = new Dictionary<string, ConceptMap>();
            NamingSystems = new Dictionary<string, NamingSystem>();
            OperationDefinitions = new Dictionary<string, OperationDefinition>();
            SearchParameters = new Dictionary<string, SearchParameter>();
            StructureDefinitions = new Dictionary<string, StructureDefinition>();
            StructureMaps = new Dictionary<string, StructureMap>();
            ValueSets = new Dictionary<string, ValueSet>();
        }

        #endregion Constructors . . .

        #region Class Interface . . .

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Query if 'resourceType' is resource type known.</summary>
        ///
        /// <remarks>Gino Canessa, 2/3/2020.</remarks>
        ///
        /// <param name="resourceType">Type of the resource.</param>
        ///
        /// <returns>True if resource type known, false if not.</returns>
        ///-------------------------------------------------------------------------------------------------

        public static bool IsResourceTypeKnown(string resourceType)
        {
            return _knownResourceTypes.Contains(resourceType);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>Query if 'resourceType' is resource type excluded.</summary>
        ///
        /// <remarks>Gino Canessa, 2/4/2020.</remarks>
        ///
        /// <param name="resourceType">Type of the resource.</param>
        ///
        /// <returns>True if resource type excluded, false if not.</returns>
        ///-------------------------------------------------------------------------------------------------

        public static bool IsResourceTypeExcluded(string resourceType)
        {
            return _excludedResourceTypes.Contains(resourceType);
        }

        #endregion Class Interface . . .

        #region Instance Interface . . .

        #endregion Instance Interface . . .

        #region Internal Functions . . .

        #endregion Internal Functions . . .

    }
}
