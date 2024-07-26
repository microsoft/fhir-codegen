module FHIR

  ##
  # A definition of a FHIR structure. This resource is used to describe the underlying resources, data types defined in FHIR, and also for describing extensions and constraints on resources and data types.
  class StructureDefinition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['abstract', 'base-path', 'base', 'context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'derivation', 'description', 'experimental', 'ext-context', 'identifier', 'jurisdiction', 'keyword', 'kind', 'name', 'path', 'publisher', 'status', 'title', 'type', 'url', 'valueset', 'version']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'StructureDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'StructureDefinition.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'StructureDefinition.implicitRules',
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
        'path'=>'StructureDefinition.language',
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
        'path'=>'StructureDefinition.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'StructureDefinition.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'StructureDefinition.extension',
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
        'path'=>'StructureDefinition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this structure definition, represented as a URI (globally unique)
      # An absolute URI that is used to identify this structure definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this structure definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the structure definition is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'StructureDefinition.url',
        'min'=>1,
        'max'=>1
      },
      ##
      # Additional identifier for the structure definition
      # A formal identifier that is used to identify this structure definition when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this structure definition outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'StructureDefinition.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business version of the structure definition
      # The identifier that is used to identify this version of the structure definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the structure definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
      # There may be different structure definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the structure definition with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'StructureDefinition.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this structure definition (computer friendly)
      # A natural language name identifying the structure definition. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.Some Examples: 
      # 
      # * O2SatObservation
      # * PresentationReport
      # * Immunization2
      # * AcmeAdmissionRecordOld.
      'name' => {
        'type'=>'string',
        'path'=>'StructureDefinition.name',
        'min'=>1,
        'max'=>1
      },
      ##
      # Name for this structure definition (human friendly)
      # A short, descriptive, user-friendly title for the structure definition.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.Applications don't have to use this name but can always fall back to it. The title also corresponds to the label for the root element.
      'title' => {
        'type'=>'string',
        'path'=>'StructureDefinition.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this structure definition. Enables tracking the life-cycle of the content.
      # Allows filtering of structure definitions that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'StructureDefinition.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this structure definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of structure definitions that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'StructureDefinition.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the structure definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the structure definition changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the structure definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'StructureDefinition.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the structure definition.
      # Usually an organization but may be an individual. The publisher (or steward) of the structure definition is the organization or individual primarily responsible for the maintenance and upkeep of the structure definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the structure definition. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'StructureDefinition.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'StructureDefinition.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the structure definition
      # A free text natural language description of the structure definition from a consumer's perspective.
      # This description can be used to capture details such as why the structure definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the structure definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the structure definition is presumed to be the predominant language in the place the structure definition was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'StructureDefinition.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate structure definition instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'StructureDefinition.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for structure definition (if applicable)
      # A legal or geographic region in which the structure definition is intended to be used.
      # It may be possible for the structure definition to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'StructureDefinition.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this structure definition is defined
      # Explanation of why this structure definition is needed and why it has been designed as it has.
      # This element does not describe the usage of the structure definition. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this structure definition.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'StructureDefinition.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the structure definition and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the structure definition.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'StructureDefinition.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # Assist with indexing and finding
      # A set of key words or terms from external terminologies that may be used to assist with indexing and searching of templates nby describing the use of this structure definition, or the content it describes.
      'keyword' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/definition-use'=>[ 'fhir-structure', 'custom-resource', 'dam', 'wire-format', 'archetype', 'template' ]
        },
        'type'=>'Coding',
        'path'=>'StructureDefinition.keyword',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/definition-use'}
      },
      ##
      # FHIR Version this StructureDefinition targets
      # The version of the FHIR specification on which this StructureDefinition is based - this is the formal version of the specification, without the revision number, e.g. [publication].[major].[minor], which is 4.0.1. for this version.
      # A StructureDefinition does not need to specify the target it applies to as StructureDefinitions will often be valid across multiple versions of FHIR. FHIR tooling can determine whether a StructureDefinition is consistent with a particular StructureDefinition if desired.
      'fhirVersion' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/FHIR-version'=>[ '0.01', '0.05', '0.06', '0.11', '0.0.80', '0.0.81', '0.0.82', '0.4.0', '0.5.0', '1.0.0', '1.0.1', '1.0.2', '1.1.0', '1.4.0', '1.6.0', '1.8.0', '3.0.0', '3.0.1', '3.3.0', '3.5.0', '4.0.0', '4.0.1' ]
        },
        'type'=>'code',
        'path'=>'StructureDefinition.fhirVersion',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/FHIR-version'}
      },
      ##
      # External specification that the content is mapped to
      # An external specification that the content is mapped to.
      'mapping' => {
        'type'=>'StructureDefinition::Mapping',
        'path'=>'StructureDefinition.mapping',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # primitive-type | complex-type | resource | logical
      # Defines the kind of structure that this definition is describing.
      'kind' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/structure-definition-kind'=>[ 'primitive-type', 'complex-type', 'resource', 'logical' ]
        },
        'type'=>'code',
        'path'=>'StructureDefinition.kind',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/structure-definition-kind'}
      },
      ##
      # Whether the structure is abstract
      # Whether structure this definition describes is abstract or not  - that is, whether the structure is not intended to be instantiated. For Resources and Data types, abstract types will never be exchanged  between systems.
      # Abstract Resources cannot be instantiated - a concrete sub-type must be used. Abstract datatypes and extensions cannot be used in an instance. For logical models, the exact implication of "abstract" will rest with the author, depending how the logical model is used. Flagging a constraint structure as abstract conveys design intent but makes no difference to how the structure definition is handled. Note that inline declared elements that are given the type "Element" in the StructureDefinition, but have children described, are anonymous concrete types that specialize Element.
      'abstract' => {
        'type'=>'boolean',
        'path'=>'StructureDefinition.abstract',
        'min'=>1,
        'max'=>1
      },
      ##
      # If an extension, where it can be used in instances
      # Identifies the types of resource or data type elements to which the extension can be applied.
      'context' => {
        'type'=>'StructureDefinition::Context',
        'path'=>'StructureDefinition.context',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # FHIRPath invariants - when the extension can be used
      # A set of rules as FHIRPath Invariants about when the extension can be used (e.g. co-occurrence variants for the extension). All the rules must be true.
      # The rules are only evaluated when the extension is present. When evaluating the invariant, the FHIRPath focus is the element that holds the extension, and %extension refers to the extension itself.
      'contextInvariant' => {
        'type'=>'string',
        'path'=>'StructureDefinition.contextInvariant',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Type defined or constrained by this structure
      # The type this structure describes. If the derivation kind is 'specialization' then this is the master definition for a type, and there is always one of these (a data type, an extension, a resource, including abstract ones). Otherwise the structure definition is a constraint on the stated type (and in this case, the type cannot be an abstract type).  References are URLs that are relative to http://hl7.org/fhir/StructureDefinition e.g. "string" is a reference to http://hl7.org/fhir/StructureDefinition/string. Absolute URLs are only allowed in logical models.
      # Note that in the case of constraints, the type could be determined by chasing through the baseDefinition references until a concrete structure (derivation = specialization) is reached, or by looking at the path of the first element in the snapshot - if present - but providing the type directly makes for simpler tooling and indexing. 
      # 
      # The type must match the elements defined in the differential and the snapshot. For all FHIR defined types, the path name of the element will start with the type name. For logical models, where the type is a URL, the type name SHOULD start with the tail of the type URL where required.
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/data-types'=>[ 'Address', 'Age', 'Annotation', 'Attachment', 'BackboneElement', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'Distance', 'Dosage', 'Duration', 'Element', 'ElementDefinition', 'Expression', 'Extension', 'HumanName', 'Identifier', 'MarketingStatus', 'Meta', 'Money', 'MoneyQuantity', 'Narrative', 'ParameterDefinition', 'Period', 'Population', 'ProdCharacteristic', 'ProductShelfLife', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'SimpleQuantity', 'SubstanceAmount', 'Timing', 'TriggerDefinition', 'UsageContext', 'base64Binary', 'boolean', 'canonical', 'code', 'date', 'dateTime', 'decimal', 'id', 'instant', 'integer', 'markdown', 'oid', 'positiveInt', 'string', 'time', 'unsignedInt', 'uri', 'url', 'uuid', 'xhtml' ],
          'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
        },
        'type'=>'uri',
        'path'=>'StructureDefinition.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/defined-types'}
      },
      ##
      # Definition that this type is constrained/specialized from
      # An absolute URI that is the base structure from which this type is derived, either by specialization or constraint.
      # If differential constraints are specified in this structure, they are applied to the base in a "differential" fashion. If there is no base, then the differential constraints cannot be provided (snapshot only). Differential structures are useful for the editing perspective, and snapshot structures are suitable for operational use. The FHIR Project provides a number of tools/services to populate snapshots from differential constraints. Logical Models have a base of "Element" or another logical model.
      'baseDefinition' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
        'type'=>'canonical',
        'path'=>'StructureDefinition.baseDefinition',
        'min'=>0,
        'max'=>1
      },
      ##
      # specialization | constraint - How relates to base definition
      # How the type relates to the baseDefinition.
      # If the definition is a specialization, then it adds new elements in the differential, and the snapshot includes the inherited elements.  If the definition is a constraint, then it cannot define new elements, it can only make new rules about existing content (see [Profiling Resources](profiling.html#resources)).
      'derivation' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/type-derivation-rule'=>[ 'specialization', 'constraint' ]
        },
        'type'=>'code',
        'path'=>'StructureDefinition.derivation',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/type-derivation-rule'}
      },
      ##
      # Snapshot view of the structure
      # A snapshot view is expressed in a standalone form that can be used and interpreted without considering the base StructureDefinition.
      'snapshot' => {
        'type'=>'StructureDefinition::Snapshot',
        'path'=>'StructureDefinition.snapshot',
        'min'=>0,
        'max'=>1
      },
      ##
      # Differential view of the structure
      # A differential view is expressed relative to the base StructureDefinition - a statement of differences that it applies.
      'differential' => {
        'type'=>'StructureDefinition::Differential',
        'path'=>'StructureDefinition.differential',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # External specification that the content is mapped to
    # An external specification that the content is mapped to.
    class Mapping < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Mapping.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Mapping.extension',
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
          'path'=>'Mapping.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Internal id when this mapping is used
        # An Internal id that is used to identify this mapping set when specific mappings are made.
        # The specification is described once, with general comments, and then specific mappings are made that reference this declaration.
        'identity' => {
          'type'=>'id',
          'path'=>'Mapping.identity',
          'min'=>1,
          'max'=>1
        },
        ##
        # Identifies what this mapping refers to
        # An absolute URI that identifies the specification that this mapping is expressed to.
        # A formal identity for the specification being mapped to helps with identifying maps consistently.
        'uri' => {
          'type'=>'uri',
          'path'=>'Mapping.uri',
          'min'=>0,
          'max'=>1
        },
        ##
        # Names what this mapping refers to
        # A name for the specification that is being mapped to.
        'name' => {
          'type'=>'string',
          'path'=>'Mapping.name',
          'min'=>0,
          'max'=>1
        },
        ##
        # Versions, Issues, Scope limitations etc.
        # Comments about this mapping, including version notes, issues, scope limitations, and other important notes for usage.
        'comment' => {
          'type'=>'string',
          'path'=>'Mapping.comment',
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
      # Internal id when this mapping is used
      # An Internal id that is used to identify this mapping set when specific mappings are made.
      # The specification is described once, with general comments, and then specific mappings are made that reference this declaration.
      attr_accessor :identity                       # 1-1 id
      ##
      # Identifies what this mapping refers to
      # An absolute URI that identifies the specification that this mapping is expressed to.
      # A formal identity for the specification being mapped to helps with identifying maps consistently.
      attr_accessor :uri                            # 0-1 uri
      ##
      # Names what this mapping refers to
      # A name for the specification that is being mapped to.
      attr_accessor :name                           # 0-1 string
      ##
      # Versions, Issues, Scope limitations etc.
      # Comments about this mapping, including version notes, issues, scope limitations, and other important notes for usage.
      attr_accessor :comment                        # 0-1 string
    end

    ##
    # If an extension, where it can be used in instances
    # Identifies the types of resource or data type elements to which the extension can be applied.
    class Context < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Context.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Context.extension',
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
          'path'=>'Context.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # fhirpath | element | extension
        # Defines how to interpret the expression that defines what the context of the extension is.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/extension-context-type'=>[ 'fhirpath', 'element', 'extension' ]
          },
          'type'=>'code',
          'path'=>'Context.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/extension-context-type'}
        },
        ##
        # Where the extension can be used in instances
        # An expression that defines where an extension can be used in resources.
        'expression' => {
          'type'=>'string',
          'path'=>'Context.expression',
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
      # fhirpath | element | extension
      # Defines how to interpret the expression that defines what the context of the extension is.
      attr_accessor :type                           # 1-1 code
      ##
      # Where the extension can be used in instances
      # An expression that defines where an extension can be used in resources.
      attr_accessor :expression                     # 1-1 string
    end

    ##
    # Snapshot view of the structure
    # A snapshot view is expressed in a standalone form that can be used and interpreted without considering the base StructureDefinition.
    class Snapshot < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Snapshot.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Snapshot.extension',
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
          'path'=>'Snapshot.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Definition of elements in the resource (if no StructureDefinition)
        # Captures constraints on each element within the resource.
        'element' => {
          'type'=>'ElementDefinition',
          'path'=>'Snapshot.element',
          'min'=>1,
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
      # Definition of elements in the resource (if no StructureDefinition)
      # Captures constraints on each element within the resource.
      attr_accessor :element                        # 1-* [ ElementDefinition ]
    end

    ##
    # Differential view of the structure
    # A differential view is expressed relative to the base StructureDefinition - a statement of differences that it applies.
    class Differential < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Differential.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Differential.extension',
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
          'path'=>'Differential.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Definition of elements in the resource (if no StructureDefinition)
        # Captures constraints on each element within the resource.
        'element' => {
          'type'=>'ElementDefinition',
          'path'=>'Differential.element',
          'min'=>1,
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
      # Definition of elements in the resource (if no StructureDefinition)
      # Captures constraints on each element within the resource.
      attr_accessor :element                        # 1-* [ ElementDefinition ]
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
    # Canonical identifier for this structure definition, represented as a URI (globally unique)
    # An absolute URI that is used to identify this structure definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this structure definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the structure definition is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 1-1 uri
    ##
    # Additional identifier for the structure definition
    # A formal identifier that is used to identify this structure definition when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this structure definition outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Business version of the structure definition
    # The identifier that is used to identify this version of the structure definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the structure definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
    # There may be different structure definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the structure definition with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this structure definition (computer friendly)
    # A natural language name identifying the structure definition. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.Some Examples: 
    # 
    # * O2SatObservation
    # * PresentationReport
    # * Immunization2
    # * AcmeAdmissionRecordOld.
    attr_accessor :name                           # 1-1 string
    ##
    # Name for this structure definition (human friendly)
    # A short, descriptive, user-friendly title for the structure definition.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.Applications don't have to use this name but can always fall back to it. The title also corresponds to the label for the root element.
    attr_accessor :title                          # 0-1 string
    ##
    # draft | active | retired | unknown
    # The status of this structure definition. Enables tracking the life-cycle of the content.
    # Allows filtering of structure definitions that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this structure definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of structure definitions that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Date last changed
    # The date  (and optionally time) when the structure definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the structure definition changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the structure definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the structure definition.
    # Usually an organization but may be an individual. The publisher (or steward) of the structure definition is the organization or individual primarily responsible for the maintenance and upkeep of the structure definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the structure definition. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the structure definition
    # A free text natural language description of the structure definition from a consumer's perspective.
    # This description can be used to capture details such as why the structure definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the structure definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the structure definition is presumed to be the predominant language in the place the structure definition was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate structure definition instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for structure definition (if applicable)
    # A legal or geographic region in which the structure definition is intended to be used.
    # It may be possible for the structure definition to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this structure definition is defined
    # Explanation of why this structure definition is needed and why it has been designed as it has.
    # This element does not describe the usage of the structure definition. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this structure definition.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the structure definition and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the structure definition.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # Assist with indexing and finding
    # A set of key words or terms from external terminologies that may be used to assist with indexing and searching of templates nby describing the use of this structure definition, or the content it describes.
    attr_accessor :keyword                        # 0-* [ Coding ]
    ##
    # FHIR Version this StructureDefinition targets
    # The version of the FHIR specification on which this StructureDefinition is based - this is the formal version of the specification, without the revision number, e.g. [publication].[major].[minor], which is 4.0.1. for this version.
    # A StructureDefinition does not need to specify the target it applies to as StructureDefinitions will often be valid across multiple versions of FHIR. FHIR tooling can determine whether a StructureDefinition is consistent with a particular StructureDefinition if desired.
    attr_accessor :fhirVersion                    # 0-1 code
    ##
    # External specification that the content is mapped to
    # An external specification that the content is mapped to.
    attr_accessor :mapping                        # 0-* [ StructureDefinition::Mapping ]
    ##
    # primitive-type | complex-type | resource | logical
    # Defines the kind of structure that this definition is describing.
    attr_accessor :kind                           # 1-1 code
    ##
    # Whether the structure is abstract
    # Whether structure this definition describes is abstract or not  - that is, whether the structure is not intended to be instantiated. For Resources and Data types, abstract types will never be exchanged  between systems.
    # Abstract Resources cannot be instantiated - a concrete sub-type must be used. Abstract datatypes and extensions cannot be used in an instance. For logical models, the exact implication of "abstract" will rest with the author, depending how the logical model is used. Flagging a constraint structure as abstract conveys design intent but makes no difference to how the structure definition is handled. Note that inline declared elements that are given the type "Element" in the StructureDefinition, but have children described, are anonymous concrete types that specialize Element.
    attr_accessor :abstract                       # 1-1 boolean
    ##
    # If an extension, where it can be used in instances
    # Identifies the types of resource or data type elements to which the extension can be applied.
    attr_accessor :context                        # 0-* [ StructureDefinition::Context ]
    ##
    # FHIRPath invariants - when the extension can be used
    # A set of rules as FHIRPath Invariants about when the extension can be used (e.g. co-occurrence variants for the extension). All the rules must be true.
    # The rules are only evaluated when the extension is present. When evaluating the invariant, the FHIRPath focus is the element that holds the extension, and %extension refers to the extension itself.
    attr_accessor :contextInvariant               # 0-* [ string ]
    ##
    # Type defined or constrained by this structure
    # The type this structure describes. If the derivation kind is 'specialization' then this is the master definition for a type, and there is always one of these (a data type, an extension, a resource, including abstract ones). Otherwise the structure definition is a constraint on the stated type (and in this case, the type cannot be an abstract type).  References are URLs that are relative to http://hl7.org/fhir/StructureDefinition e.g. "string" is a reference to http://hl7.org/fhir/StructureDefinition/string. Absolute URLs are only allowed in logical models.
    # Note that in the case of constraints, the type could be determined by chasing through the baseDefinition references until a concrete structure (derivation = specialization) is reached, or by looking at the path of the first element in the snapshot - if present - but providing the type directly makes for simpler tooling and indexing. 
    # 
    # The type must match the elements defined in the differential and the snapshot. For all FHIR defined types, the path name of the element will start with the type name. For logical models, where the type is a URL, the type name SHOULD start with the tail of the type URL where required.
    attr_accessor :type                           # 1-1 uri
    ##
    # Definition that this type is constrained/specialized from
    # An absolute URI that is the base structure from which this type is derived, either by specialization or constraint.
    # If differential constraints are specified in this structure, they are applied to the base in a "differential" fashion. If there is no base, then the differential constraints cannot be provided (snapshot only). Differential structures are useful for the editing perspective, and snapshot structures are suitable for operational use. The FHIR Project provides a number of tools/services to populate snapshots from differential constraints. Logical Models have a base of "Element" or another logical model.
    attr_accessor :baseDefinition                 # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition)
    ##
    # specialization | constraint - How relates to base definition
    # How the type relates to the baseDefinition.
    # If the definition is a specialization, then it adds new elements in the differential, and the snapshot includes the inherited elements.  If the definition is a constraint, then it cannot define new elements, it can only make new rules about existing content (see [Profiling Resources](profiling.html#resources)).
    attr_accessor :derivation                     # 0-1 code
    ##
    # Snapshot view of the structure
    # A snapshot view is expressed in a standalone form that can be used and interpreted without considering the base StructureDefinition.
    attr_accessor :snapshot                       # 0-1 StructureDefinition::Snapshot
    ##
    # Differential view of the structure
    # A differential view is expressed relative to the base StructureDefinition - a statement of differences that it applies.
    attr_accessor :differential                   # 0-1 StructureDefinition::Differential

    def resourceType
      'StructureDefinition'
    end
  end
end
