module FHIR

  ##
  # A ValueSet resource instance specifies a set of codes drawn from one or more code systems, intended for use in a particular context. Value sets link between [[[CodeSystem]]] definitions and their use in [coded elements](terminologies.html).
  class ValueSet < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['code', 'context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'description', 'expansion', 'identifier', 'jurisdiction', 'name', 'publisher', 'reference', 'status', 'title', 'url', 'version']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'ValueSet.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ValueSet.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ValueSet.implicitRules',
        'min'=>0,
        'max'=>1
      },
      ##
      # Language of the resource content
      # The base language in which the resource is written.
      # Language is provided to support indexing and accessibility (typically, services such as text to speech use the language tag). The html language tag in the narrative applies  to the narrative. The language tag on the resource may be used to specify the language of other presentations generated from the data in the resource. Not all the content has to be in the base language. The Resource.language should not be assumed to apply to the narrative automatically. If a language is specified, it should it also be specified on the div element in the html (see rules in HTML5 for information about the relationship between xml:lang and the html lang attribute).
      'language' => {
        'valid_codes'=>{
          'urn:ietf:bcp:47'=>[ 'ar', 'bn', 'cs', 'da', 'de', 'de-AT', 'de-CH', 'de-DE', 'el', 'en', 'en-AU', 'en-CA', 'en-GB', 'en-IN', 'en-NZ', 'en-SG', 'en-US', 'es', 'es-AR', 'es-ES', 'es-UY', 'fi', 'fr', 'fr-BE', 'fr-CH', 'fr-FR', 'fy', 'fy-NL', 'hi', 'hr', 'it', 'it-CH', 'it-IT', 'ja', 'ko', 'nl', 'nl-BE', 'nl-NL', 'no', 'no-NO', 'pa', 'pl', 'pt', 'pt-BR', 'ru', 'ru-RU', 'sr', 'sr-RS', 'sv', 'sv-SE', 'te', 'zh', 'zh-CN', 'zh-HK', 'zh-SG', 'zh-TW' ]
        },
        'type'=>'code',
        'path'=>'ValueSet.language',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
      },
      ##
      # Text summary of the resource, for human interpretation
      # A human-readable narrative that contains a summary of the resource and can be used to represent the content of the resource to a human. The narrative need not encode all the structured data, but is required to contain sufficient detail to make it "clinically safe" for a human to just read the narrative. Resource definitions may define what content should be represented in the narrative to ensure clinical safety.
      # Contained resources do not have narrative. Resources that are not contained SHOULD have a narrative. In some cases, a resource may only have text with little or no additional discrete data (as long as all minOccurs=1 elements are satisfied).  This may be necessary for data from legacy systems where information is captured as a "text blob" or where text is additionally entered raw or narrated and encoded information is added later.
      'text' => {
        'type'=>'Narrative',
        'path'=>'ValueSet.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ValueSet.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ValueSet.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Extensions that cannot be ignored
      # May be used to represent additional information that is not part of the basic definition of the resource and that modifies the understanding of the element that contains it and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer is allowed to define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'modifierExtension' => {
        'type'=>'Extension',
        'path'=>'ValueSet.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this value set, represented as a URI (globally unique)
      # An absolute URI that is used to identify this value set when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this value set is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the value set is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'ValueSet.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional identifier for the value set (business identifier)
      # A formal identifier that is used to identify this value set when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this value set outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ValueSet.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business version of the value set
      # The identifier that is used to identify this version of the value set when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the value set author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
      # There may be different value set instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the value set with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'ValueSet.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this value set (computer friendly)
      # A natural language name identifying the value set. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.A name should be provided unless the value set is a contained resource (e.g. an anonymous value set in a profile). Most registries will require a name.
      'name' => {
        'type'=>'string',
        'path'=>'ValueSet.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this value set (human friendly)
      # A short, descriptive, user-friendly title for the value set.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'ValueSet.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this value set. Enables tracking the life-cycle of the content. The status of the value set applies to the value set definition (ValueSet.compose) and the associated ValueSet metadata. Expansions do not have a state.
      # Allows filtering of value sets that are appropriate for use versus not.See also the [valueset-workflowStatus](extension-valueset-workflowstatus.html) extension for additional status information related to the editorial process.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'ValueSet.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this value set is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of value sets that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'ValueSet.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date last changed
      # The date (and optionally time) when the value set was created or revised (e.g. the 'content logical definition').
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the value set. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'ValueSet.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the value set.
      # Usually an organization but may be an individual. The publisher (or steward) of the value set is the organization or individual primarily responsible for the maintenance and upkeep of the value set. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the value set. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'ValueSet.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'ValueSet.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the value set
      # A free text natural language description of the value set from a consumer's perspective. The textual description specifies the span of meanings for concepts to be included within the Value Set Expansion, and also may specify the intended use and limitations of the Value Set.
      # This description can be used to capture details such as why the value set was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the value set as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the value set is presumed to be the predominant language in the place the value set was created).The description is not intended to describe the semantics of the Value Set - there are no intrinsic semantics separate from the codes contained in its expansion. The description should capture its intended use, which is needed for ensuring integrity for its use in models across future changes. A description should be provided unless the value set is a contained resource (e.g. an anonymous value set in a profile). Most registries will require a description.
      'description' => {
        'type'=>'markdown',
        'path'=>'ValueSet.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate value set instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'ValueSet.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for value set (if applicable)
      # A legal or geographic region in which the value set is intended to be used.
      # It may be possible for the value set to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'ValueSet.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Indicates whether or not any change to the content logical definition may occur
      # If this is set to 'true', then no new versions of the content logical definition can be created.  Note: Other metadata might still change.
      # Normally immutability is set to 'false', which is the default assumption if it is not populated.  Note that the implication is that if this is set to 'true', there may be only one ValueSet version for this definition. Immutability tends to be set to 'true' in one of two cases: - Where the value set, by the nature of its usage, cannot change.  For example "All specializations of ACT in ActClassCode" - Where there's no safe way to express the "Purpose" such that someone else could safely make changes to the value set definition. Source workflow control must guarantee that the same URI always yields the same definition.
      'immutable' => {
        'type'=>'boolean',
        'path'=>'ValueSet.immutable',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why this value set is defined
      # Explanation of why this value set is needed and why it has been designed as it has.
      # This element does not describe the usage of the value set. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this value set.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'ValueSet.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the value set and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the value set.
      # nullFrequently, the copyright differs between the value set and the codes that are included. The copyright statement should clearly differentiate between these when required.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'ValueSet.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # Content logical definition of the value set (CLD)
      # A set of criteria that define the contents of the value set by including or excluding codes selected from the specified code system(s) that the value set draws from. This is also known as the Content Logical Definition (CLD).
      'compose' => {
        'type'=>'ValueSet::Compose',
        'path'=>'ValueSet.compose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Used when the value set is "expanded"
      # A value set can also be "expanded", where the value set is turned into a simple collection of enumerated codes. This element holds the expansion, if it has been performed.
      # Expansion is performed to produce a collection of codes that are ready to use for data entry or validation. Value set expansions are always considered to be stateless - they are a record of the set of codes in the value set at a point in time under a given set of conditions, and are not subject to ongoing maintenance.
      # 
      # Expansion.parameter is  a simplified list of parameters - a subset of the features of the [Parameters](parameters.html) resource.
      'expansion' => {
        'type'=>'ValueSet::Expansion',
        'path'=>'ValueSet.expansion',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Content logical definition of the value set (CLD)
    # A set of criteria that define the contents of the value set by including or excluding codes selected from the specified code system(s) that the value set draws from. This is also known as the Content Logical Definition (CLD).
    class Compose < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Compose.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Compose.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'modifierExtension' => {
          'type'=>'Extension',
          'path'=>'Compose.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Fixed date for references with no specified version (transitive)
        # The Locked Date is  the effective date that is used to determine the version of all referenced Code Systems and Value Set Definitions included in the compose that are not already tied to a specific version.
        # With a defined lockedDate the value set is considered "Locked". Otherwise, the value set may have different expansions as underlying code systems and/or value sets evolve.  The interpretation of lockedDate is often dependent on the context - e.g. a SNOMED CT derived value set with a lockedDate will have a different expansion in USA than in UK.  If a value set specifies a version for include and exclude statements, and also specifies a locked date, the specified versions need to be available that date, or the value set will not be usable.
        'lockedDate' => {
          'type'=>'date',
          'path'=>'Compose.lockedDate',
          'min'=>0,
          'max'=>1
        },
        ##
        # Whether inactive codes are in the value set
        # Whether inactive codes - codes that are not approved for current use - are in the value set. If inactive = true, inactive codes are to be included in the expansion, if inactive = false, the inactive codes will not be included in the expansion. If absent, the behavior is determined by the implementation, or by the applicable $expand parameters (but generally, inactive codes would be expected to be included).
        # Note that in the FHIR terminology framework, "deprecated" does not mean inactive, but in some code systems, e.g. LOINC, "deprecated" does mean inactive. Code systems should define what codes are considered to be inactive. If this is not clearly defined (including in the FHIR code system resource), then all codes are assumed to be active.
        # 
        # The Value Set Definition specification defines an ActiveOnly element, which is the reverse of this element e.g. (ValueSet.compose.inactive=FALSE) is the same as (VSD.ActiveOnly=TRUE).
        'inactive' => {
          'type'=>'boolean',
          'path'=>'Compose.inactive',
          'min'=>0,
          'max'=>1
        },
        ##
        # Include one or more codes from a code system or other value set(s).
        # All the conditions in an include must be true. If a system is listed, all the codes from the system are listed. If one or more filters are listed, all of the filters must apply. If one or more value sets are listed, the codes must be in all the value sets. E.g. each include is 'include all the codes that meet all these conditions'.
        'include' => {
          'type'=>'ValueSet::Compose::Include',
          'path'=>'Compose.include',
          'min'=>1,
          'max'=>Float::INFINITY
        },
        ##
        # Explicitly exclude codes from a code system or other value sets
        # Exclude one or more codes from the value set based on code system filters and/or other value sets.
        # Usually this is used to selectively exclude codes that were included by subsumption in the inclusions. Any display names specified for the codes are ignored.
        'exclude' => {
          'type'=>'ValueSet::Compose::Include',
          'path'=>'Compose.exclude',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Include one or more codes from a code system or other value set(s).
      # All the conditions in an include must be true. If a system is listed, all the codes from the system are listed. If one or more filters are listed, all of the filters must apply. If one or more value sets are listed, the codes must be in all the value sets. E.g. each include is 'include all the codes that meet all these conditions'.
      class Include < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Include.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Include.extension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Extensions that cannot be ignored even if unrecognized
          # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
          # 
          # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'modifierExtension' => {
            'type'=>'Extension',
            'path'=>'Include.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # The system the codes come from
          # An absolute URI which is the code system from which the selected codes come from.
          # If there are no codes or filters, the entire code system is included. Note that the set of codes that are included may contain abstract codes. See ''Coding.system'' for further documentation about the correct value for the system element.
          'system' => {
            'type'=>'uri',
            'path'=>'Include.system',
            'min'=>0,
            'max'=>1
          },
          ##
          # Specific version of the code system referred to
          # The version of the code system that the codes are selected from, or the special version '*' for all versions.
          # This is used when selecting the descendants of a concept - they may change between versions. If no version is specified, then the exact contents of the value set might not be known until a context of use binds it to a particular version. The special value '*' means all versions; It is at server discretion regarding expansions and which versions must be supported.
          'version' => {
            'type'=>'string',
            'path'=>'Include.version',
            'min'=>0,
            'max'=>1
          },
          ##
          # A concept defined in the system
          # Specifies a concept to be included or excluded.
          # The list of concepts is considered ordered, though the order might not have any particular significance. Typically, the order of an expansion follows that defined in the compose element.
          'concept' => {
            'type'=>'ValueSet::Compose::Include::Concept',
            'path'=>'Include.concept',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Select codes/concepts by their properties (including relationships)
          # Select concepts by specify a matching criterion based on the properties (including relationships) defined by the system, or on filters defined by the system. If multiple filters are specified, they SHALL all be true.
          # Selecting codes by specifying filters based on properties is only possible where the underlying code system defines appropriate properties. Note that in some cases, the underlying code system defines the logical concepts but not the literal codes for the concepts. In such cases, the literal definitions may be provided by a third party.
          'filter' => {
            'type'=>'ValueSet::Compose::Include::Filter',
            'path'=>'Include.filter',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Select the contents included in this value set
          # Selects the concepts found in this value set (based on its value set definition). This is an absolute URI that is a reference to ValueSet.url.  If multiple value sets are specified this includes the union of the contents of all of the referenced value sets.
          # The value set URI is either a logical reference to a defined value set such as a [SNOMED CT reference set](snomedct.html#implicit), or a direct reference to a value set definition using ValueSet.url. The reference might not refer to an actual FHIR ValueSet resource; in this case, whatever is referred to is an implicit definition of a value set that needs to be clear about how versions are resolved.
          'valueSet' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
            'type'=>'canonical',
            'path'=>'Include.valueSet',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # A concept defined in the system
        # Specifies a concept to be included or excluded.
        # The list of concepts is considered ordered, though the order might not have any particular significance. Typically, the order of an expansion follows that defined in the compose element.
        class Concept < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'id',
              'path'=>'Concept.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Concept.extension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Extensions that cannot be ignored even if unrecognized
            # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
            # 
            # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'modifierExtension' => {
              'type'=>'Extension',
              'path'=>'Concept.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Code or expression from system
            # Specifies a code for the concept to be included or excluded.
            # Expressions are allowed if defined by the underlying code system.
            'code' => {
              'type'=>'code',
              'path'=>'Concept.code',
              'min'=>1,
              'max'=>1
            },
            ##
            # Text to display for this code for this value set in this valueset
            # The text to display to the user for this concept in the context of this valueset. If no display is provided, then applications using the value set use the display specified for the code by the system.
            # The value set resource allows for an alternative display to be specified for when this concept is used in this particular value set. See notes in the value set narrative about the correct use of this element.
            'display' => {
              'type'=>'string',
              'path'=>'Concept.display',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional representations for this concept when used in this value set - other languages, aliases, specialized purposes, used for particular purposes, etc.
            # Concepts have both a ```display``` and an array of ```designation```. The display is equivalent to a special designation with an implied ```designation.use``` of "primary code" and a language equal to the [Resource Language](resource.html#language).
            'designation' => {
              'type'=>'ValueSet::Compose::Include::Concept::Designation',
              'path'=>'Concept.designation',
              'min'=>0,
              'max'=>Float::INFINITY
            }
          }

          ##
          # Additional representations for this concept when used in this value set - other languages, aliases, specialized purposes, used for particular purposes, etc.
          # Concepts have both a ```display``` and an array of ```designation```. The display is equivalent to a special designation with an implied ```designation.use``` of "primary code" and a language equal to the [Resource Language](resource.html#language).
          class Designation < FHIR::Model
            include FHIR::Hashable
            include FHIR::Json
            include FHIR::Xml

            METADATA = {
              ##
              # Unique id for inter-element referencing
              # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
              'id' => {
                'type'=>'id',
                'path'=>'Designation.id',
                'min'=>0,
                'max'=>1
              },
              ##
              # Additional content defined by implementations
              # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
              # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
              'extension' => {
                'type'=>'Extension',
                'path'=>'Designation.extension',
                'min'=>0,
                'max'=>Float::INFINITY
              },
              ##
              # Extensions that cannot be ignored even if unrecognized
              # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
              # 
              # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
              # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
              'modifierExtension' => {
                'type'=>'Extension',
                'path'=>'Designation.modifierExtension',
                'min'=>0,
                'max'=>Float::INFINITY
              },
              ##
              # Human language of the designation
              # The language this designation is defined for.
              # In the absence of a language, the resource language applies.
              'language' => {
                'valid_codes'=>{
                  'urn:ietf:bcp:47'=>[ 'ar', 'bn', 'cs', 'da', 'de', 'de-AT', 'de-CH', 'de-DE', 'el', 'en', 'en-AU', 'en-CA', 'en-GB', 'en-IN', 'en-NZ', 'en-SG', 'en-US', 'es', 'es-AR', 'es-ES', 'es-UY', 'fi', 'fr', 'fr-BE', 'fr-CH', 'fr-FR', 'fy', 'fy-NL', 'hi', 'hr', 'it', 'it-CH', 'it-IT', 'ja', 'ko', 'nl', 'nl-BE', 'nl-NL', 'no', 'no-NO', 'pa', 'pl', 'pt', 'pt-BR', 'ru', 'ru-RU', 'sr', 'sr-RS', 'sv', 'sv-SE', 'te', 'zh', 'zh-CN', 'zh-HK', 'zh-SG', 'zh-TW' ]
                },
                'type'=>'code',
                'path'=>'Designation.language',
                'min'=>0,
                'max'=>1,
                'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
              },
              ##
              # Types of uses of designations
              # A code that represents types of uses of designations.
              # If no use is provided, the designation can be assumed to be suitable for general display to a human user.
              'use' => {
                'type'=>'Coding',
                'path'=>'Designation.use',
                'min'=>0,
                'max'=>1
              },
              ##
              # The text value for this designation.
              'value' => {
                'type'=>'string',
                'path'=>'Designation.value',
                'min'=>1,
                'max'=>1
              }
            }
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            attr_accessor :id                             # 0-1 id
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            attr_accessor :extension                      # 0-* [ Extension ]
            ##
            # Extensions that cannot be ignored even if unrecognized
            # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
            # 
            # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            attr_accessor :modifierExtension              # 0-* [ Extension ]
            ##
            # Human language of the designation
            # The language this designation is defined for.
            # In the absence of a language, the resource language applies.
            attr_accessor :language                       # 0-1 code
            ##
            # Types of uses of designations
            # A code that represents types of uses of designations.
            # If no use is provided, the designation can be assumed to be suitable for general display to a human user.
            attr_accessor :use                            # 0-1 Coding
            ##
            # The text value for this designation.
            attr_accessor :value                          # 1-1 string
          end
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          attr_accessor :id                             # 0-1 id
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          attr_accessor :extension                      # 0-* [ Extension ]
          ##
          # Extensions that cannot be ignored even if unrecognized
          # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
          # 
          # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          attr_accessor :modifierExtension              # 0-* [ Extension ]
          ##
          # Code or expression from system
          # Specifies a code for the concept to be included or excluded.
          # Expressions are allowed if defined by the underlying code system.
          attr_accessor :code                           # 1-1 code
          ##
          # Text to display for this code for this value set in this valueset
          # The text to display to the user for this concept in the context of this valueset. If no display is provided, then applications using the value set use the display specified for the code by the system.
          # The value set resource allows for an alternative display to be specified for when this concept is used in this particular value set. See notes in the value set narrative about the correct use of this element.
          attr_accessor :display                        # 0-1 string
          ##
          # Additional representations for this concept when used in this value set - other languages, aliases, specialized purposes, used for particular purposes, etc.
          # Concepts have both a ```display``` and an array of ```designation```. The display is equivalent to a special designation with an implied ```designation.use``` of "primary code" and a language equal to the [Resource Language](resource.html#language).
          attr_accessor :designation                    # 0-* [ ValueSet::Compose::Include::Concept::Designation ]
        end

        ##
        # Select codes/concepts by their properties (including relationships)
        # Select concepts by specify a matching criterion based on the properties (including relationships) defined by the system, or on filters defined by the system. If multiple filters are specified, they SHALL all be true.
        # Selecting codes by specifying filters based on properties is only possible where the underlying code system defines appropriate properties. Note that in some cases, the underlying code system defines the logical concepts but not the literal codes for the concepts. In such cases, the literal definitions may be provided by a third party.
        class Filter < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'id',
              'path'=>'Filter.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Filter.extension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Extensions that cannot be ignored even if unrecognized
            # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
            # 
            # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'modifierExtension' => {
              'type'=>'Extension',
              'path'=>'Filter.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # A property/filter defined by the code system
            # A code that identifies a property or a filter defined in the code system.
            'property' => {
              'type'=>'code',
              'path'=>'Filter.property',
              'min'=>1,
              'max'=>1
            },
            ##
            # = | is-a | descendent-of | is-not-a | regex | in | not-in | generalizes | exists
            # The kind of operation to perform as a part of the filter criteria.
            # In case filter.property represents a property of the system, the operation applies to the selected property. In case filter.property represents a filter of the system, the operation SHALL match one of the CodeSystem.filter.operator values.
            'op' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/filter-operator'=>[ '=', 'is-a', 'descendent-of', 'is-not-a', 'regex', 'in', 'not-in', 'generalizes', 'exists' ]
              },
              'type'=>'code',
              'path'=>'Filter.op',
              'min'=>1,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/filter-operator'}
            },
            ##
            # Code from the system, or regex criteria, or boolean value for exists
            # The match value may be either a code defined by the system, or a string value, which is a regex match on the literal string of the property value  (if the filter represents a property defined in CodeSystem) or of the system filter value (if the filter represents a filter defined in CodeSystem) when the operation is 'regex', or one of the values (true and false), when the operation is 'exists'.
            # Use regex matching with care - full regex matching on every SNOMED CT term is prohibitive, for example.
            'value' => {
              'type'=>'string',
              'path'=>'Filter.value',
              'min'=>1,
              'max'=>1
            }
          }
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          attr_accessor :id                             # 0-1 id
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          attr_accessor :extension                      # 0-* [ Extension ]
          ##
          # Extensions that cannot be ignored even if unrecognized
          # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
          # 
          # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          attr_accessor :modifierExtension              # 0-* [ Extension ]
          ##
          # A property/filter defined by the code system
          # A code that identifies a property or a filter defined in the code system.
          attr_accessor :property                       # 1-1 code
          ##
          # = | is-a | descendent-of | is-not-a | regex | in | not-in | generalizes | exists
          # The kind of operation to perform as a part of the filter criteria.
          # In case filter.property represents a property of the system, the operation applies to the selected property. In case filter.property represents a filter of the system, the operation SHALL match one of the CodeSystem.filter.operator values.
          attr_accessor :op                             # 1-1 code
          ##
          # Code from the system, or regex criteria, or boolean value for exists
          # The match value may be either a code defined by the system, or a string value, which is a regex match on the literal string of the property value  (if the filter represents a property defined in CodeSystem) or of the system filter value (if the filter represents a filter defined in CodeSystem) when the operation is 'regex', or one of the values (true and false), when the operation is 'exists'.
          # Use regex matching with care - full regex matching on every SNOMED CT term is prohibitive, for example.
          attr_accessor :value                          # 1-1 string
        end
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        attr_accessor :id                             # 0-1 id
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :extension                      # 0-* [ Extension ]
        ##
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :modifierExtension              # 0-* [ Extension ]
        ##
        # The system the codes come from
        # An absolute URI which is the code system from which the selected codes come from.
        # If there are no codes or filters, the entire code system is included. Note that the set of codes that are included may contain abstract codes. See ''Coding.system'' for further documentation about the correct value for the system element.
        attr_accessor :system                         # 0-1 uri
        ##
        # Specific version of the code system referred to
        # The version of the code system that the codes are selected from, or the special version '*' for all versions.
        # This is used when selecting the descendants of a concept - they may change between versions. If no version is specified, then the exact contents of the value set might not be known until a context of use binds it to a particular version. The special value '*' means all versions; It is at server discretion regarding expansions and which versions must be supported.
        attr_accessor :version                        # 0-1 string
        ##
        # A concept defined in the system
        # Specifies a concept to be included or excluded.
        # The list of concepts is considered ordered, though the order might not have any particular significance. Typically, the order of an expansion follows that defined in the compose element.
        attr_accessor :concept                        # 0-* [ ValueSet::Compose::Include::Concept ]
        ##
        # Select codes/concepts by their properties (including relationships)
        # Select concepts by specify a matching criterion based on the properties (including relationships) defined by the system, or on filters defined by the system. If multiple filters are specified, they SHALL all be true.
        # Selecting codes by specifying filters based on properties is only possible where the underlying code system defines appropriate properties. Note that in some cases, the underlying code system defines the logical concepts but not the literal codes for the concepts. In such cases, the literal definitions may be provided by a third party.
        attr_accessor :filter                         # 0-* [ ValueSet::Compose::Include::Filter ]
        ##
        # Select the contents included in this value set
        # Selects the concepts found in this value set (based on its value set definition). This is an absolute URI that is a reference to ValueSet.url.  If multiple value sets are specified this includes the union of the contents of all of the referenced value sets.
        # The value set URI is either a logical reference to a defined value set such as a [SNOMED CT reference set](snomedct.html#implicit), or a direct reference to a value set definition using ValueSet.url. The reference might not refer to an actual FHIR ValueSet resource; in this case, whatever is referred to is an implicit definition of a value set that needs to be clear about how versions are resolved.
        attr_accessor :valueSet                       # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/ValueSet) ]
      end
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      attr_accessor :id                             # 0-1 id
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :extension                      # 0-* [ Extension ]
      ##
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # Fixed date for references with no specified version (transitive)
      # The Locked Date is  the effective date that is used to determine the version of all referenced Code Systems and Value Set Definitions included in the compose that are not already tied to a specific version.
      # With a defined lockedDate the value set is considered "Locked". Otherwise, the value set may have different expansions as underlying code systems and/or value sets evolve.  The interpretation of lockedDate is often dependent on the context - e.g. a SNOMED CT derived value set with a lockedDate will have a different expansion in USA than in UK.  If a value set specifies a version for include and exclude statements, and also specifies a locked date, the specified versions need to be available that date, or the value set will not be usable.
      attr_accessor :lockedDate                     # 0-1 date
      ##
      # Whether inactive codes are in the value set
      # Whether inactive codes - codes that are not approved for current use - are in the value set. If inactive = true, inactive codes are to be included in the expansion, if inactive = false, the inactive codes will not be included in the expansion. If absent, the behavior is determined by the implementation, or by the applicable $expand parameters (but generally, inactive codes would be expected to be included).
      # Note that in the FHIR terminology framework, "deprecated" does not mean inactive, but in some code systems, e.g. LOINC, "deprecated" does mean inactive. Code systems should define what codes are considered to be inactive. If this is not clearly defined (including in the FHIR code system resource), then all codes are assumed to be active.
      # 
      # The Value Set Definition specification defines an ActiveOnly element, which is the reverse of this element e.g. (ValueSet.compose.inactive=FALSE) is the same as (VSD.ActiveOnly=TRUE).
      attr_accessor :inactive                       # 0-1 boolean
      ##
      # Include one or more codes from a code system or other value set(s).
      # All the conditions in an include must be true. If a system is listed, all the codes from the system are listed. If one or more filters are listed, all of the filters must apply. If one or more value sets are listed, the codes must be in all the value sets. E.g. each include is 'include all the codes that meet all these conditions'.
      attr_accessor :include                        # 1-* [ ValueSet::Compose::Include ]
      ##
      # Explicitly exclude codes from a code system or other value sets
      # Exclude one or more codes from the value set based on code system filters and/or other value sets.
      # Usually this is used to selectively exclude codes that were included by subsumption in the inclusions. Any display names specified for the codes are ignored.
      attr_accessor :exclude                        # 0-* [ ValueSet::Compose::Include ]
    end

    ##
    # Used when the value set is "expanded"
    # A value set can also be "expanded", where the value set is turned into a simple collection of enumerated codes. This element holds the expansion, if it has been performed.
    # Expansion is performed to produce a collection of codes that are ready to use for data entry or validation. Value set expansions are always considered to be stateless - they are a record of the set of codes in the value set at a point in time under a given set of conditions, and are not subject to ongoing maintenance.
    # 
    # Expansion.parameter is  a simplified list of parameters - a subset of the features of the [Parameters](parameters.html) resource.
    class Expansion < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Expansion.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Expansion.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'modifierExtension' => {
          'type'=>'Extension',
          'path'=>'Expansion.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Identifies the value set expansion (business identifier)
        # An identifier that uniquely identifies this expansion of the valueset, based on a unique combination of the provided parameters, the system default parameters, and the underlying system code system versions etc. Systems may re-use the same identifier as long as those factors remain the same, and the expansion is the same, but are not required to do so. This is a business identifier.
        # Typically, this uri is a UUID (e.g. urn:uuid:8230ff20-c97a-4167-a59d-dc2cb9df16dd).
        'identifier' => {
          'type'=>'uri',
          'path'=>'Expansion.identifier',
          'min'=>0,
          'max'=>1
        },
        ##
        # Time ValueSet expansion happened
        # The time at which the expansion was produced by the expanding system.
        # This SHOULD be a fully populated instant, but in some circumstances, value sets are expanded by hand, and the expansion is published without that precision.
        'timestamp' => {
          'type'=>'dateTime',
          'path'=>'Expansion.timestamp',
          'min'=>1,
          'max'=>1
        },
        ##
        # Total number of codes in the expansion
        # The total number of concepts in the expansion. If the number of concept nodes in this resource is less than the stated number, then the server can return more using the offset parameter.
        # Paging only applies to flat expansions.
        'total' => {
          'type'=>'integer',
          'path'=>'Expansion.total',
          'min'=>0,
          'max'=>1
        },
        ##
        # Offset at which this resource starts
        # If paging is being used, the offset at which this resource starts.  I.e. this resource is a partial view into the expansion. If paging is not being used, this element SHALL NOT be present.
        # Paging only applies to flat expansions. If a filter is applied, the count is the number of concepts that matched the filter, not the number of concepts in an unfiltered view of the expansion.
        'offset' => {
          'type'=>'integer',
          'path'=>'Expansion.offset',
          'min'=>0,
          'max'=>1
        },
        ##
        # Parameter that controlled the expansion process
        # A parameter that controlled the expansion process. These parameters may be used by users of expanded value sets to check whether the expansion is suitable for a particular purpose, or to pick the correct expansion.
        # The server decides which parameters to include here, but at a minimum, the list SHOULD include all of the parameters that affect the $expand operation. If the expansion will be persisted all of these parameters SHALL be included. If the codeSystem on the server has a specified version then this version SHALL be provided as a parameter in the expansion (note that not all code systems have a version).
        'parameter' => {
          'type'=>'ValueSet::Expansion::Parameter',
          'path'=>'Expansion.parameter',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Codes in the value set
        # The codes that are contained in the value set expansion.
        'contains' => {
          'type'=>'ValueSet::Expansion::Contains',
          'path'=>'Expansion.contains',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Parameter that controlled the expansion process
      # A parameter that controlled the expansion process. These parameters may be used by users of expanded value sets to check whether the expansion is suitable for a particular purpose, or to pick the correct expansion.
      # The server decides which parameters to include here, but at a minimum, the list SHOULD include all of the parameters that affect the $expand operation. If the expansion will be persisted all of these parameters SHALL be included. If the codeSystem on the server has a specified version then this version SHALL be provided as a parameter in the expansion (note that not all code systems have a version).
      class Parameter < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'value[x]' => ['boolean', 'code', 'dateTime', 'decimal', 'integer', 'string', 'uri']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Parameter.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Parameter.extension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Extensions that cannot be ignored even if unrecognized
          # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
          # 
          # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'modifierExtension' => {
            'type'=>'Extension',
            'path'=>'Parameter.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Name as assigned by the client or server
          # Name of the input parameter to the $expand operation; may be a server-assigned name for additional default or other server-supplied parameters used to control the expansion process.
          # The names are assigned at the discretion of the server.
          'name' => {
            'type'=>'string',
            'path'=>'Parameter.name',
            'min'=>1,
            'max'=>1
          },
          ##
          # Value of the named parameter
          # The value of the parameter.
          'valueBoolean' => {
            'type'=>'Boolean',
            'path'=>'Parameter.value[x]',
            'min'=>0,
            'max'=>1
          }
          ##
          # Value of the named parameter
          # The value of the parameter.
          'valueCode' => {
            'type'=>'Code',
            'path'=>'Parameter.value[x]',
            'min'=>0,
            'max'=>1
          }
          ##
          # Value of the named parameter
          # The value of the parameter.
          'valueDateTime' => {
            'type'=>'DateTime',
            'path'=>'Parameter.value[x]',
            'min'=>0,
            'max'=>1
          }
          ##
          # Value of the named parameter
          # The value of the parameter.
          'valueDecimal' => {
            'type'=>'Decimal',
            'path'=>'Parameter.value[x]',
            'min'=>0,
            'max'=>1
          }
          ##
          # Value of the named parameter
          # The value of the parameter.
          'valueInteger' => {
            'type'=>'Integer',
            'path'=>'Parameter.value[x]',
            'min'=>0,
            'max'=>1
          }
          ##
          # Value of the named parameter
          # The value of the parameter.
          'valueString' => {
            'type'=>'String',
            'path'=>'Parameter.value[x]',
            'min'=>0,
            'max'=>1
          }
          ##
          # Value of the named parameter
          # The value of the parameter.
          'valueUri' => {
            'type'=>'Uri',
            'path'=>'Parameter.value[x]',
            'min'=>0,
            'max'=>1
          }
        }
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        attr_accessor :id                             # 0-1 id
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :extension                      # 0-* [ Extension ]
        ##
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :modifierExtension              # 0-* [ Extension ]
        ##
        # Name as assigned by the client or server
        # Name of the input parameter to the $expand operation; may be a server-assigned name for additional default or other server-supplied parameters used to control the expansion process.
        # The names are assigned at the discretion of the server.
        attr_accessor :name                           # 1-1 string
        ##
        # Value of the named parameter
        # The value of the parameter.
        attr_accessor :valueBoolean                   # 0-1 Boolean
        ##
        # Value of the named parameter
        # The value of the parameter.
        attr_accessor :valueCode                      # 0-1 Code
        ##
        # Value of the named parameter
        # The value of the parameter.
        attr_accessor :valueDateTime                  # 0-1 DateTime
        ##
        # Value of the named parameter
        # The value of the parameter.
        attr_accessor :valueDecimal                   # 0-1 Decimal
        ##
        # Value of the named parameter
        # The value of the parameter.
        attr_accessor :valueInteger                   # 0-1 Integer
        ##
        # Value of the named parameter
        # The value of the parameter.
        attr_accessor :valueString                    # 0-1 String
        ##
        # Value of the named parameter
        # The value of the parameter.
        attr_accessor :valueUri                       # 0-1 Uri
      end

      ##
      # Codes in the value set
      # The codes that are contained in the value set expansion.
      class Contains < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Contains.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Contains.extension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Extensions that cannot be ignored even if unrecognized
          # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
          # 
          # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'modifierExtension' => {
            'type'=>'Extension',
            'path'=>'Contains.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # System value for the code
          # An absolute URI which is the code system in which the code for this item in the expansion is defined.
          'system' => {
            'type'=>'uri',
            'path'=>'Contains.system',
            'min'=>0,
            'max'=>1
          },
          ##
          # If user cannot select this entry
          # If true, this entry is included in the expansion for navigational purposes, and the user cannot select the code directly as a proper value.
          # This should not be understood to exclude its use for searching (e.g. by subsumption testing). The client should know whether it is appropriate for the user to select an abstract code or not.
          'abstract' => {
            'type'=>'boolean',
            'path'=>'Contains.abstract',
            'min'=>0,
            'max'=>1
          },
          ##
          # If concept is inactive in the code system
          # If the concept is inactive in the code system that defines it. Inactive codes are those that are no longer to be used, but are maintained by the code system for understanding legacy data. It might not be known or specified whether an concept is inactive (and it may depend on the context of use).
          # This should only have a value if the concept is inactive.
          'inactive' => {
            'type'=>'boolean',
            'path'=>'Contains.inactive',
            'min'=>0,
            'max'=>1
          },
          ##
          # Version in which this code/display is defined
          # The version of the code system from this code was taken. Note that a well-maintained code system does not need the version reported, because the meaning of codes is consistent across versions. However this cannot consistently be assured, and when the meaning is not guaranteed to be consistent, the version SHOULD be exchanged.
          # The exact value of the version string is specified by the system from which the code is derived.
          'version' => {
            'type'=>'string',
            'path'=>'Contains.version',
            'min'=>0,
            'max'=>1
          },
          ##
          # Code - if blank, this is not a selectable code
          # The code for this item in the expansion hierarchy. If this code is missing the entry in the hierarchy is a place holder (abstract) and does not represent a valid code in the value set.
          'code' => {
            'type'=>'code',
            'path'=>'Contains.code',
            'min'=>0,
            'max'=>1
          },
          ##
          # User display for the concept
          # The recommended display for this item in the expansion.
          'display' => {
            'type'=>'string',
            'path'=>'Contains.display',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional representations for this item - other languages, aliases, specialized purposes, used for particular purposes, etc. These are relevant when the conditions of the expansion do not fix to a single correct representation.
          # The designations provided must be based on the value set and code system definitions.
          'designation' => {
            'type'=>'ValueSet::Compose::Include::Concept::Designation',
            'path'=>'Contains.designation',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Codes contained under this entry
          # Other codes and entries contained under this entry in the hierarchy.
          # If the expansion uses this element, there is  no implication about the logical relationship between them, and the  structure cannot be used for logical inferencing. The structure  exists to provide navigational assistance for helping human users to  locate codes in the expansion.
          'contains' => {
            'type'=>'ValueSet::Expansion::Contains',
            'path'=>'Contains.contains',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        attr_accessor :id                             # 0-1 id
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :extension                      # 0-* [ Extension ]
        ##
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :modifierExtension              # 0-* [ Extension ]
        ##
        # System value for the code
        # An absolute URI which is the code system in which the code for this item in the expansion is defined.
        attr_accessor :system                         # 0-1 uri
        ##
        # If user cannot select this entry
        # If true, this entry is included in the expansion for navigational purposes, and the user cannot select the code directly as a proper value.
        # This should not be understood to exclude its use for searching (e.g. by subsumption testing). The client should know whether it is appropriate for the user to select an abstract code or not.
        attr_accessor :abstract                       # 0-1 boolean
        ##
        # If concept is inactive in the code system
        # If the concept is inactive in the code system that defines it. Inactive codes are those that are no longer to be used, but are maintained by the code system for understanding legacy data. It might not be known or specified whether an concept is inactive (and it may depend on the context of use).
        # This should only have a value if the concept is inactive.
        attr_accessor :inactive                       # 0-1 boolean
        ##
        # Version in which this code/display is defined
        # The version of the code system from this code was taken. Note that a well-maintained code system does not need the version reported, because the meaning of codes is consistent across versions. However this cannot consistently be assured, and when the meaning is not guaranteed to be consistent, the version SHOULD be exchanged.
        # The exact value of the version string is specified by the system from which the code is derived.
        attr_accessor :version                        # 0-1 string
        ##
        # Code - if blank, this is not a selectable code
        # The code for this item in the expansion hierarchy. If this code is missing the entry in the hierarchy is a place holder (abstract) and does not represent a valid code in the value set.
        attr_accessor :code                           # 0-1 code
        ##
        # User display for the concept
        # The recommended display for this item in the expansion.
        attr_accessor :display                        # 0-1 string
        ##
        # Additional representations for this item - other languages, aliases, specialized purposes, used for particular purposes, etc. These are relevant when the conditions of the expansion do not fix to a single correct representation.
        # The designations provided must be based on the value set and code system definitions.
        attr_accessor :designation                    # 0-* [ ValueSet::Compose::Include::Concept::Designation ]
        ##
        # Codes contained under this entry
        # Other codes and entries contained under this entry in the hierarchy.
        # If the expansion uses this element, there is  no implication about the logical relationship between them, and the  structure cannot be used for logical inferencing. The structure  exists to provide navigational assistance for helping human users to  locate codes in the expansion.
        attr_accessor :contains                       # 0-* [ ValueSet::Expansion::Contains ]
      end
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      attr_accessor :id                             # 0-1 id
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :extension                      # 0-* [ Extension ]
      ##
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # Identifies the value set expansion (business identifier)
      # An identifier that uniquely identifies this expansion of the valueset, based on a unique combination of the provided parameters, the system default parameters, and the underlying system code system versions etc. Systems may re-use the same identifier as long as those factors remain the same, and the expansion is the same, but are not required to do so. This is a business identifier.
      # Typically, this uri is a UUID (e.g. urn:uuid:8230ff20-c97a-4167-a59d-dc2cb9df16dd).
      attr_accessor :identifier                     # 0-1 uri
      ##
      # Time ValueSet expansion happened
      # The time at which the expansion was produced by the expanding system.
      # This SHOULD be a fully populated instant, but in some circumstances, value sets are expanded by hand, and the expansion is published without that precision.
      attr_accessor :timestamp                      # 1-1 dateTime
      ##
      # Total number of codes in the expansion
      # The total number of concepts in the expansion. If the number of concept nodes in this resource is less than the stated number, then the server can return more using the offset parameter.
      # Paging only applies to flat expansions.
      attr_accessor :total                          # 0-1 integer
      ##
      # Offset at which this resource starts
      # If paging is being used, the offset at which this resource starts.  I.e. this resource is a partial view into the expansion. If paging is not being used, this element SHALL NOT be present.
      # Paging only applies to flat expansions. If a filter is applied, the count is the number of concepts that matched the filter, not the number of concepts in an unfiltered view of the expansion.
      attr_accessor :offset                         # 0-1 integer
      ##
      # Parameter that controlled the expansion process
      # A parameter that controlled the expansion process. These parameters may be used by users of expanded value sets to check whether the expansion is suitable for a particular purpose, or to pick the correct expansion.
      # The server decides which parameters to include here, but at a minimum, the list SHOULD include all of the parameters that affect the $expand operation. If the expansion will be persisted all of these parameters SHALL be included. If the codeSystem on the server has a specified version then this version SHALL be provided as a parameter in the expansion (note that not all code systems have a version).
      attr_accessor :parameter                      # 0-* [ ValueSet::Expansion::Parameter ]
      ##
      # Codes in the value set
      # The codes that are contained in the value set expansion.
      attr_accessor :contains                       # 0-* [ ValueSet::Expansion::Contains ]
    end
    ##
    # Logical id of this artifact
    # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
    # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
    attr_accessor :id                             # 0-1 id
    ##
    # Metadata about the resource
    # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
    attr_accessor :meta                           # 0-1 Meta
    ##
    # A set of rules under which this content was created
    # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
    # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
    attr_accessor :implicitRules                  # 0-1 uri
    ##
    # Language of the resource content
    # The base language in which the resource is written.
    # Language is provided to support indexing and accessibility (typically, services such as text to speech use the language tag). The html language tag in the narrative applies  to the narrative. The language tag on the resource may be used to specify the language of other presentations generated from the data in the resource. Not all the content has to be in the base language. The Resource.language should not be assumed to apply to the narrative automatically. If a language is specified, it should it also be specified on the div element in the html (see rules in HTML5 for information about the relationship between xml:lang and the html lang attribute).
    attr_accessor :language                       # 0-1 code
    ##
    # Text summary of the resource, for human interpretation
    # A human-readable narrative that contains a summary of the resource and can be used to represent the content of the resource to a human. The narrative need not encode all the structured data, but is required to contain sufficient detail to make it "clinically safe" for a human to just read the narrative. Resource definitions may define what content should be represented in the narrative to ensure clinical safety.
    # Contained resources do not have narrative. Resources that are not contained SHOULD have a narrative. In some cases, a resource may only have text with little or no additional discrete data (as long as all minOccurs=1 elements are satisfied).  This may be necessary for data from legacy systems where information is captured as a "text blob" or where text is additionally entered raw or narrated and encoded information is added later.
    attr_accessor :text                           # 0-1 Narrative
    ##
    # Contained, inline Resources
    # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
    # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
    attr_accessor :contained                      # 0-* [ Resource ]
    ##
    # Additional content defined by implementations
    # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
    # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
    attr_accessor :extension                      # 0-* [ Extension ]
    ##
    # Extensions that cannot be ignored
    # May be used to represent additional information that is not part of the basic definition of the resource and that modifies the understanding of the element that contains it and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer is allowed to define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
    # 
    # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
    # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
    attr_accessor :modifierExtension              # 0-* [ Extension ]
    ##
    # Canonical identifier for this value set, represented as a URI (globally unique)
    # An absolute URI that is used to identify this value set when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this value set is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the value set is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 0-1 uri
    ##
    # Additional identifier for the value set (business identifier)
    # A formal identifier that is used to identify this value set when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this value set outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Business version of the value set
    # The identifier that is used to identify this version of the value set when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the value set author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
    # There may be different value set instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the value set with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this value set (computer friendly)
    # A natural language name identifying the value set. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.A name should be provided unless the value set is a contained resource (e.g. an anonymous value set in a profile). Most registries will require a name.
    attr_accessor :name                           # 0-1 string
    ##
    # Name for this value set (human friendly)
    # A short, descriptive, user-friendly title for the value set.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # draft | active | retired | unknown
    # The status of this value set. Enables tracking the life-cycle of the content. The status of the value set applies to the value set definition (ValueSet.compose) and the associated ValueSet metadata. Expansions do not have a state.
    # Allows filtering of value sets that are appropriate for use versus not.See also the [valueset-workflowStatus](extension-valueset-workflowstatus.html) extension for additional status information related to the editorial process.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this value set is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of value sets that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Date last changed
    # The date (and optionally time) when the value set was created or revised (e.g. the 'content logical definition').
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the value set. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the value set.
    # Usually an organization but may be an individual. The publisher (or steward) of the value set is the organization or individual primarily responsible for the maintenance and upkeep of the value set. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the value set. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the value set
    # A free text natural language description of the value set from a consumer's perspective. The textual description specifies the span of meanings for concepts to be included within the Value Set Expansion, and also may specify the intended use and limitations of the Value Set.
    # This description can be used to capture details such as why the value set was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the value set as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the value set is presumed to be the predominant language in the place the value set was created).The description is not intended to describe the semantics of the Value Set - there are no intrinsic semantics separate from the codes contained in its expansion. The description should capture its intended use, which is needed for ensuring integrity for its use in models across future changes. A description should be provided unless the value set is a contained resource (e.g. an anonymous value set in a profile). Most registries will require a description.
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate value set instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for value set (if applicable)
    # A legal or geographic region in which the value set is intended to be used.
    # It may be possible for the value set to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Indicates whether or not any change to the content logical definition may occur
    # If this is set to 'true', then no new versions of the content logical definition can be created.  Note: Other metadata might still change.
    # Normally immutability is set to 'false', which is the default assumption if it is not populated.  Note that the implication is that if this is set to 'true', there may be only one ValueSet version for this definition. Immutability tends to be set to 'true' in one of two cases: - Where the value set, by the nature of its usage, cannot change.  For example "All specializations of ACT in ActClassCode" - Where there's no safe way to express the "Purpose" such that someone else could safely make changes to the value set definition. Source workflow control must guarantee that the same URI always yields the same definition.
    attr_accessor :immutable                      # 0-1 boolean
    ##
    # Why this value set is defined
    # Explanation of why this value set is needed and why it has been designed as it has.
    # This element does not describe the usage of the value set. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this value set.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the value set and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the value set.
    # nullFrequently, the copyright differs between the value set and the codes that are included. The copyright statement should clearly differentiate between these when required.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # Content logical definition of the value set (CLD)
    # A set of criteria that define the contents of the value set by including or excluding codes selected from the specified code system(s) that the value set draws from. This is also known as the Content Logical Definition (CLD).
    attr_accessor :compose                        # 0-1 ValueSet::Compose
    ##
    # Used when the value set is "expanded"
    # A value set can also be "expanded", where the value set is turned into a simple collection of enumerated codes. This element holds the expansion, if it has been performed.
    # Expansion is performed to produce a collection of codes that are ready to use for data entry or validation. Value set expansions are always considered to be stateless - they are a record of the set of codes in the value set at a point in time under a given set of conditions, and are not subject to ongoing maintenance.
    # 
    # Expansion.parameter is  a simplified list of parameters - a subset of the features of the [Parameters](parameters.html) resource.
    attr_accessor :expansion                      # 0-1 ValueSet::Expansion

    def resourceType
      'ValueSet'
    end
  end
end
