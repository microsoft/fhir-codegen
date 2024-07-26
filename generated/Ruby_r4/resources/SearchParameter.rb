module FHIR

  ##
  # A search parameter that defines a named search item that can be used to search/filter on a resource.
  class SearchParameter < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['base', 'code', 'component', 'context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'derived-from', 'description', 'jurisdiction', 'name', 'publisher', 'status', 'target', 'type', 'url', 'version']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'SearchParameter.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'SearchParameter.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'SearchParameter.implicitRules',
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
        'path'=>'SearchParameter.language',
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
        'path'=>'SearchParameter.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'SearchParameter.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'SearchParameter.extension',
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
        'path'=>'SearchParameter.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this search parameter, represented as a URI (globally unique)
      # An absolute URI that is used to identify this search parameter when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this search parameter is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the search parameter is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'SearchParameter.url',
        'min'=>1,
        'max'=>1
      },
      ##
      # Business version of the search parameter
      # The identifier that is used to identify this version of the search parameter when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the search parameter author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
      # There may be different search parameter instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the search parameter with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'SearchParameter.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this search parameter (computer friendly)
      # A natural language name identifying the search parameter. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'SearchParameter.name',
        'min'=>1,
        'max'=>1
      },
      ##
      # Original definition for the search parameter
      # Where this search parameter is originally defined. If a derivedFrom is provided, then the details in the search parameter must be consistent with the definition from which it is defined. i.e. the parameter should have the same meaning, and (usually) the functionality should be a proper subset of the underlying search parameter.
      # The intent of this is that a server can designate that it provides support for a search parameter defined in the specification itself (e.g.  [`value-quantity`](http://hl7.org/fhir/SearchParameter/Observation-value-quantity), but detail how it is supported by the server.
      'derivedFrom' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/SearchParameter'],
        'type'=>'canonical',
        'path'=>'SearchParameter.derivedFrom',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this search parameter. Enables tracking the life-cycle of the content.
      # Allows filtering of search parameters that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'SearchParameter.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this search parameter is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of search parameters that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'SearchParameter.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the search parameter was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the search parameter changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the search parameter. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'SearchParameter.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the search parameter.
      # Usually an organization but may be an individual. The publisher (or steward) of the search parameter is the organization or individual primarily responsible for the maintenance and upkeep of the search parameter. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the search parameter. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'SearchParameter.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'SearchParameter.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the search parameter
      # And how it used.
      # This description can be used to capture details such as why the search parameter was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the search parameter as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the search parameter is presumed to be the predominant language in the place the search parameter was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'SearchParameter.description',
        'min'=>1,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate search parameter instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'SearchParameter.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for search parameter (if applicable)
      # A legal or geographic region in which the search parameter is intended to be used.
      # It may be possible for the search parameter to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'SearchParameter.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this search parameter is defined
      # Explanation of why this search parameter is needed and why it has been designed as it has.
      # This element does not describe the usage of the search parameter. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this search parameter.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'SearchParameter.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Code used in URL
      # The code used in the URL or the parameter name in a parameters resource for this search parameter.
      # For maximum compatibility, use only lowercase ASCII characters.
      'code' => {
        'type'=>'code',
        'path'=>'SearchParameter.code',
        'min'=>1,
        'max'=>1
      },
      ##
      # The resource type(s) this search parameter applies to
      # The base resource type(s) that this search parameter can be used against.
      # A search parameter must always apply to at least one resource type. When search parameters apply to more than one resource type, they can be used against any of the listed resource types, or in a cross-type search (see [Cross Resource Search](http.html#xres-search)).
      'base' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
        },
        'type'=>'code',
        'path'=>'SearchParameter.base',
        'min'=>1,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/resource-types'}
      },
      ##
      # number | date | string | token | reference | composite | quantity | uri | special
      # The type of value that a search parameter may contain, and how the content is interpreted.
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/search-param-type'=>[ 'number', 'date', 'string', 'token', 'reference', 'composite', 'quantity', 'uri', 'special' ]
        },
        'type'=>'code',
        'path'=>'SearchParameter.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/search-param-type'}
      },
      ##
      # FHIRPath expression that extracts the values
      # A FHIRPath expression that returns a set of elements for the search parameter.
      # Note that the elements returned by the expression are sometimes complex elements where logic is required to determine quite how to handle them; e.g. CodeableConcepts may contain text and/or multiple codings, where the codings themselves contain a code and a system. For composite search parameters, the outcome of the expression must a collection of base elements from which the composites are derived.
      'expression' => {
        'type'=>'string',
        'path'=>'SearchParameter.expression',
        'min'=>0,
        'max'=>1
      },
      ##
      # XPath that extracts the values
      # An XPath expression that returns a set of elements for the search parameter.
      # Note that the elements returned by the XPath are sometimes complex elements where logic is required to determine quite how to handle them; e.g. CodeableConcepts may contain text and/or multiple codings, where the codings themselves contain a code and a system.
      'xpath' => {
        'type'=>'string',
        'path'=>'SearchParameter.xpath',
        'min'=>0,
        'max'=>1
      },
      ##
      # normal | phonetic | nearby | distance | other
      # How the search parameter relates to the set of elements returned by evaluating the xpath query.
      'xpathUsage' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/search-xpath-usage'=>[ 'normal', 'phonetic', 'nearby', 'distance', 'other' ]
        },
        'type'=>'code',
        'path'=>'SearchParameter.xpathUsage',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/search-xpath-usage'}
      },
      ##
      # Types of resource (if a resource reference)
      # Types of resource (if a resource is referenced).
      'target' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
        },
        'type'=>'code',
        'path'=>'SearchParameter.target',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/resource-types'}
      },
      ##
      # Allow multiple values per parameter (or)
      # Whether multiple values are allowed for each time the parameter exists. Values are separated by commas, and the parameter matches if any of the values match.
      'multipleOr' => {
        'type'=>'boolean',
        'path'=>'SearchParameter.multipleOr',
        'min'=>0,
        'max'=>1
      },
      ##
      # Allow multiple parameters (and)
      # Whether multiple parameters are allowed - e.g. more than one parameter with the same name. The search matches if all the parameters match.
      'multipleAnd' => {
        'type'=>'boolean',
        'path'=>'SearchParameter.multipleAnd',
        'min'=>0,
        'max'=>1
      },
      ##
      # eq | ne | gt | lt | ge | le | sa | eb | ap
      # Comparators supported for the search parameter.
      # If no comparators are listed, clients should not expect servers to support any comparators.
      'comparator' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/search-comparator'=>[ 'eq', 'ne', 'gt', 'lt', 'ge', 'le', 'sa', 'eb', 'ap' ]
        },
        'type'=>'code',
        'path'=>'SearchParameter.comparator',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/search-comparator'}
      },
      ##
      # missing | exact | contains | not | text | in | not-in | below | above | type | identifier | ofType
      # A modifier supported for the search parameter.
      'modifier' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/search-modifier-code'=>[ 'missing', 'exact', 'contains', 'not', 'text', 'in', 'not-in', 'below', 'above', 'type', 'identifier', 'ofType' ]
        },
        'type'=>'code',
        'path'=>'SearchParameter.modifier',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/search-modifier-code'}
      },
      ##
      # Chained names supported
      # Contains the names of any search parameters which may be chained to the containing search parameter. Chained parameters may be added to search parameters of type reference and specify that resources will only be returned if they contain a reference to a resource which matches the chained parameter value. Values for this field should be drawn from SearchParameter.code for a parameter on the target resource type.
      # Systems are not required to list all the chain names they support, but if they don't list them, clients might not know to use them.
      'chain' => {
        'type'=>'string',
        'path'=>'SearchParameter.chain',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # For Composite resources to define the parts
      # Used to define the parts of a composite search parameter.
      'component' => {
        'type'=>'SearchParameter::Component',
        'path'=>'SearchParameter.component',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # For Composite resources to define the parts
    # Used to define the parts of a composite search parameter.
    class Component < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Component.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Component.extension',
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
          'path'=>'Component.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Defines how the part works
        # The definition of the search parameter that describes this part.
        'definition' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/SearchParameter'],
          'type'=>'canonical',
          'path'=>'Component.definition',
          'min'=>1,
          'max'=>1
        },
        ##
        # Subexpression relative to main expression
        # A sub-expression that defines how to extract values for this component from the output of the main SearchParameter.expression.
        # This expression overrides the expression in the definition and extracts the index values from the outcome of the composite expression.
        'expression' => {
          'type'=>'string',
          'path'=>'Component.expression',
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
      # Defines how the part works
      # The definition of the search parameter that describes this part.
      attr_accessor :definition                     # 1-1 canonical(http://hl7.org/fhir/StructureDefinition/SearchParameter)
      ##
      # Subexpression relative to main expression
      # A sub-expression that defines how to extract values for this component from the output of the main SearchParameter.expression.
      # This expression overrides the expression in the definition and extracts the index values from the outcome of the composite expression.
      attr_accessor :expression                     # 1-1 string
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
    # Canonical identifier for this search parameter, represented as a URI (globally unique)
    # An absolute URI that is used to identify this search parameter when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this search parameter is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the search parameter is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 1-1 uri
    ##
    # Business version of the search parameter
    # The identifier that is used to identify this version of the search parameter when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the search parameter author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
    # There may be different search parameter instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the search parameter with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this search parameter (computer friendly)
    # A natural language name identifying the search parameter. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 1-1 string
    ##
    # Original definition for the search parameter
    # Where this search parameter is originally defined. If a derivedFrom is provided, then the details in the search parameter must be consistent with the definition from which it is defined. i.e. the parameter should have the same meaning, and (usually) the functionality should be a proper subset of the underlying search parameter.
    # The intent of this is that a server can designate that it provides support for a search parameter defined in the specification itself (e.g.  [`value-quantity`](http://hl7.org/fhir/SearchParameter/Observation-value-quantity), but detail how it is supported by the server.
    attr_accessor :derivedFrom                    # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/SearchParameter)
    ##
    # draft | active | retired | unknown
    # The status of this search parameter. Enables tracking the life-cycle of the content.
    # Allows filtering of search parameters that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this search parameter is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of search parameters that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Date last changed
    # The date  (and optionally time) when the search parameter was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the search parameter changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the search parameter. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the search parameter.
    # Usually an organization but may be an individual. The publisher (or steward) of the search parameter is the organization or individual primarily responsible for the maintenance and upkeep of the search parameter. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the search parameter. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the search parameter
    # And how it used.
    # This description can be used to capture details such as why the search parameter was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the search parameter as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the search parameter is presumed to be the predominant language in the place the search parameter was created).
    attr_accessor :description                    # 1-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate search parameter instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for search parameter (if applicable)
    # A legal or geographic region in which the search parameter is intended to be used.
    # It may be possible for the search parameter to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this search parameter is defined
    # Explanation of why this search parameter is needed and why it has been designed as it has.
    # This element does not describe the usage of the search parameter. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this search parameter.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Code used in URL
    # The code used in the URL or the parameter name in a parameters resource for this search parameter.
    # For maximum compatibility, use only lowercase ASCII characters.
    attr_accessor :code                           # 1-1 code
    ##
    # The resource type(s) this search parameter applies to
    # The base resource type(s) that this search parameter can be used against.
    # A search parameter must always apply to at least one resource type. When search parameters apply to more than one resource type, they can be used against any of the listed resource types, or in a cross-type search (see [Cross Resource Search](http.html#xres-search)).
    attr_accessor :base                           # 1-* [ code ]
    ##
    # number | date | string | token | reference | composite | quantity | uri | special
    # The type of value that a search parameter may contain, and how the content is interpreted.
    attr_accessor :type                           # 1-1 code
    ##
    # FHIRPath expression that extracts the values
    # A FHIRPath expression that returns a set of elements for the search parameter.
    # Note that the elements returned by the expression are sometimes complex elements where logic is required to determine quite how to handle them; e.g. CodeableConcepts may contain text and/or multiple codings, where the codings themselves contain a code and a system. For composite search parameters, the outcome of the expression must a collection of base elements from which the composites are derived.
    attr_accessor :expression                     # 0-1 string
    ##
    # XPath that extracts the values
    # An XPath expression that returns a set of elements for the search parameter.
    # Note that the elements returned by the XPath are sometimes complex elements where logic is required to determine quite how to handle them; e.g. CodeableConcepts may contain text and/or multiple codings, where the codings themselves contain a code and a system.
    attr_accessor :xpath                          # 0-1 string
    ##
    # normal | phonetic | nearby | distance | other
    # How the search parameter relates to the set of elements returned by evaluating the xpath query.
    attr_accessor :xpathUsage                     # 0-1 code
    ##
    # Types of resource (if a resource reference)
    # Types of resource (if a resource is referenced).
    attr_accessor :target                         # 0-* [ code ]
    ##
    # Allow multiple values per parameter (or)
    # Whether multiple values are allowed for each time the parameter exists. Values are separated by commas, and the parameter matches if any of the values match.
    attr_accessor :multipleOr                     # 0-1 boolean
    ##
    # Allow multiple parameters (and)
    # Whether multiple parameters are allowed - e.g. more than one parameter with the same name. The search matches if all the parameters match.
    attr_accessor :multipleAnd                    # 0-1 boolean
    ##
    # eq | ne | gt | lt | ge | le | sa | eb | ap
    # Comparators supported for the search parameter.
    # If no comparators are listed, clients should not expect servers to support any comparators.
    attr_accessor :comparator                     # 0-* [ code ]
    ##
    # missing | exact | contains | not | text | in | not-in | below | above | type | identifier | ofType
    # A modifier supported for the search parameter.
    attr_accessor :modifier                       # 0-* [ code ]
    ##
    # Chained names supported
    # Contains the names of any search parameters which may be chained to the containing search parameter. Chained parameters may be added to search parameters of type reference and specify that resources will only be returned if they contain a reference to a resource which matches the chained parameter value. Values for this field should be drawn from SearchParameter.code for a parameter on the target resource type.
    # Systems are not required to list all the chain names they support, but if they don't list them, clients might not know to use them.
    attr_accessor :chain                          # 0-* [ string ]
    ##
    # For Composite resources to define the parts
    # Used to define the parts of a composite search parameter.
    attr_accessor :component                      # 0-* [ SearchParameter::Component ]

    def resourceType
      'SearchParameter'
    end
  end
end
