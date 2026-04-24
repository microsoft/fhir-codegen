namespace xver_editor.Data;

public static class Global
{
    internal static string WelcomeMarkdownContent = """"
        # Cross-Version Working Area

        This repository is a working area for the Cross-Version Extensions project.  The goal of this repo is to serve as a location for temporary artifacts, documentation, and questions while working on Cross-Version Extensions.

        Once the extensions are completed, relevant content will have new homes and this repo will be archived.

        ## Goals

        * Ability to use elements from another version of FHIR
        * Encode information **added** in a *newer* version of FHIR
          * Community identified additional data that is needed
          * Need to provide a consistent way of representing it in an earlier version of FHIR
        * Encode information **removed** in a *newer* version of FHIR
          * Community decided data was no longer necessary
          * Need to provide a consistent way of representing it in a later version of FHIR
            * Version migration requires interoperability between versions and cannot break old expectations
            * Scenarios where data needs to round-trip conversion between different versions


        This work is tied up with a few concerns that cannot be easily separated. The primary desired outputs are:
        * `StructureDefinition` extensions for cross-version element use.
        * `ValueSet` definitions based on differentials between versions to allow for binding in extensions.

        ## Process Overview

        The process for generating cross-version extensions has been broken up into a few sections:
        1. [Load](#load): Normalize and load 'base' content into the database
            * FHIR core definitions (`hl7.fhir.r*.core`) and expanded Value Sets
            * Prior mapping data (ConceptMap resources, FML files) from [HL7/fhir-cross-version](https://github.com/HL7/fhir-cross-version)
        1. [Compare](#compare): Iterate across versions and compare contents
        1. [Output](#output): Generate output artifacts (documentation, extension definitions)

        ### Load

        1. Iterate over the FHIR versions (DSTU2, STU3, R4, R4B, R5)
            1. Download package from the registry if missing
            1. Traverse file contents and load `.json` files into memory
        1. Traverse loaded definitions
            1. Expand and add Value Sets
            1. Add Structures
        1. If we have the [HL7/fhir-cross-version](https://github.com/HL7/fhir-cross-version) data
            1. Traverse comparison pairs in order (e.g., `2 <-> 3`, `3 <-> 4`, etc.)
                1. Load Value Set `ConceptMap` resources from `input/codes/ConceptMap-*-[source-version]to[target-version].json`
                1. Load type-map `ConceptMap` from `input/types/ConceptMap-types-[source-version]to[target-version].json`
                1. Load resource-map `ConceptMap` from `input/resources/ConceptMap-resources-[source-version]to[target-version].json`
                1. Load element rename map `ConceptMap` from `input/elements/ConceptMap-elements-[source-version]to[target-version].json`
                1. Load FML files from `input/R[source-version]toR[target-version]/*.fml`
            1. Load initial comparisons into appropriate tables in the database

        ### Compare

        The comparison happens in order across the bidirectional pairs of definitions (e.g., `2 <-> 3`, `3 <-> 4`, etc.).
        When processing a package pair, the lower version is used as the source _first_, then the higher version.

        #### Compare Value Sets

        Iterate over all the Value Sets in the source:
        1. Look for any explicit mappings to establish a relationship to a Value Set in the target version
            1. If we do not have a mapping, look for a matching URL, then Name, then Id.
            1. If we cannot find any mappings, list as a `noMap`.
                * Note that we can **add** the mapping to the database and re-run the comparison.
        1. Iterate over each Value Set pair (note that Value Sets can split/merge across versions)
            1. If the comparison has been reviewed and approved, skip it
            1. Look for an inverse record of our comparison and create one if it does not exist.
            1. Perform the concept comparisons
                1. Iterate over the concepts in the source Value Set
                    1. Look for an existing mapping / comparison
                        1. If there are no existing mappings, look for a matching `code` value
                        1. If we still have no mappings, flag as a `noMap`
                    1. Look for an inverse record of our comparison and create one if it does not exist.
                    1. If the mapping has been approved, skip it
                    1. Check for an 'escape-valve' code (e.g., `other`) that is flagged `equivalent` but represents a different concept domain
                    1. Check for a mapping flagged as `equivalent` that maps to or from multiple codes
                    1. Check the inverse comparison relationship and ensure they make sense
            1. Aggregate the relationships from the concept comparisons into the overall Value Set relationship

        #### Compare Structures

        Iterate over the structures, sequenced as: Primitive Types, Complex Types, Resources, Profiles*, and Logical Models*. (*Only if included in original set):
        1. Look for any explicit mappings to establish a relationship to a Structure Definition in the target version
            1. If we do not have any mappings and this is a primitive type, add all matching comparisons from our internal table of primitive mappings
            1. If we do not have any mappings, look for a matching URL, then Name, then Id.
        1. Iterate over each structure pair
            1. If the comparison has been reviewed and approved, skip it
            1. Look for an inverse record of our comparison and create one if it does not exist.
            1. Perform the element comparisons
                1. Iterate over the elements in the source structure
                    1. Look for an existing mapping / comparison
                        1. If there are no existing mappings, look for a matching `ElementDefinition.id` value
                        1. If we still have no mappings, flag as a `noMap`
                    1. Look for an inverse record of our comparison and create one if it does not exist.
                    1. If the mapping has been approved, skip it
                    1. Check for an `equivalent` mapping that maps to multiple elements
                    1. Perform type comparisons
                        1. Iterate over the types in the source element
                            1. Look for an existing mapping / comparison
                                1. If there are no existing mappings, look for a matching `ElementDefinition.id` value
                                1. If we still have no mappings, flag as a `noMap`
                            1. Check for changes in `typeProfile`
                            1. Check for changes in `targetProfile`
                        1. Check aggregate type information (e.g., added types, removed types)
                        1. Look for an inverse record of our comparison and create one if it does not exist.
                    1. Apply aggregate type relationship to the element relationships
                    1. Check for cardinality changes and update relationships accordingly
                    1. Check for a `binding` on the source or target
                        1. Check for adding or removing the binding completely
                        1. Check for changing to or from `required` binding strength
                        1. If both are `required` and there is a `code` type
                            1. Check compatibility based on _only_ `code` values in the Value Sets. Note that excluded value sets are assumed compatible (e.g., UCUM).
                            1. Update the value set comparison relationship accordingly
                        1. If both are `required` and there is a `coding` type
                            1. Check compatibility based on `code` and `system` values in the Value Sets. Note that excluded value sets are assumed compatible (e.g., UCUM).
                            1. Update the value set comparison relationship accordingly
                    1. Check the inverse comparison relationship and ensure they make sense
            1. Aggregate the relationships from the element comparisons into the overall structure relationship
            1. If this is a Primitive Type, override the relationships based on our internal table

        ### Output

        Output is broken into two sections, generation of Markdown files and generation of Extension definitions.
        Output is generated 'keyed' off of each version so that mappings are understandable.

        #### Writing Markdown Files

        1. Export the primitive mapping key (all primitive types from all versions)
        1. Iterate over the packages
            1. Export Value Set documents
                1. Start the overview file and collect table entries for each section
                1. Iterate over all the value sets in this package
                    1. Explode the Value Set comparisons into a grid of mappings
                    1. Generate the overview table entry
                    1. Write the detailed markdown file
            1. Export Structure documents - Primitive Types
                1. Start the overview file and collect table entries for each section
                1. Iterate over all the primitive types in this package
                    1. Explode the structure comparisons into a grid of mappings
                    1. Generate the overview table entry
                    1. Write the detailed markdown file
            1. Export Structure documents - Complex Types
                1. Start the overview file and collect table entries for each section
                1. Iterate over all the complex types in this package
                    1. Explode the structure comparisons into a grid of mappings
                    1. Generate the overview table entry
                    1. Write the detailed markdown file
            1. Export Structure documents - Resources
                1. Start the overview file and collect table entries for each section
                1. Iterate over all the resources in this package
                    1. Explode the structure comparisons into a grid of mappings
                    1. Generate the overview table entry
                    1. Write the detailed markdown file


        
        """";
}
