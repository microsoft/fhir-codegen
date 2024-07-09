module FHIR

  ##
  # The Measure resource provides the definition of a quality measure.
  class Measure < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['composed-of', 'context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'depends-on', 'derived-from', 'description', 'effective', 'identifier', 'jurisdiction', 'name', 'predecessor', 'publisher', 'status', 'successor', 'title', 'topic', 'url', 'version']
    MULTIPLE_TYPES = {
      'subject[x]' => ['CodeableConcept', 'Reference']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Measure.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Measure.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Measure.implicitRules',
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
        'path'=>'Measure.language',
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
        'path'=>'Measure.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Measure.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Measure.extension',
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
        'path'=>'Measure.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this measure, represented as a URI (globally unique)
      # An absolute URI that is used to identify this measure when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this measure is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the measure is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'Measure.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional identifier for the measure
      # A formal identifier that is used to identify this measure when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this measure outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Measure.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business version of the measure
      # The identifier that is used to identify this version of the measure when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the measure author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active artifacts.
      # There may be different measure instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the measure with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'Measure.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this measure (computer friendly)
      # A natural language name identifying the measure. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'Measure.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this measure (human friendly)
      # A short, descriptive, user-friendly title for the measure.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'Measure.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # Subordinate title of the measure
      # An explanatory or alternate title for the measure giving additional information about its content.
      'subtitle' => {
        'type'=>'string',
        'path'=>'Measure.subtitle',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this measure. Enables tracking the life-cycle of the content.
      # Allows filtering of measures that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Measure.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this measure is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of measures that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'Measure.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
      # The intended subjects for the measure. If this element is not provided, a Patient subject is assumed, but the subject of the measure can be anything.
      # The subject of the measure is critical in interpreting the criteria definitions, as the logic in the measures is evaluated with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what content is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
      'subjectCodeableConcept' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Measure.subject[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
      },
      ##
      # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
      # The intended subjects for the measure. If this element is not provided, a Patient subject is assumed, but the subject of the measure can be anything.
      # The subject of the measure is critical in interpreting the criteria definitions, as the logic in the measures is evaluated with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what content is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
      'subjectReference' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/resource-types'=>[ 'Patient', 'Practitioner', 'Organization', 'Location', 'Device' ]
        },
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'Measure.subject[x]',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/subject-type'}
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the measure was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the measure changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the measure. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'Measure.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the measure.
      # Usually an organization but may be an individual. The publisher (or steward) of the measure is the organization or individual primarily responsible for the maintenance and upkeep of the measure. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the measure. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'Measure.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'Measure.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the measure
      # A free text natural language description of the measure from a consumer's perspective.
      # This description can be used to capture details such as why the measure was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the measure as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the measure is presumed to be the predominant language in the place the measure was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'Measure.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate measure instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'Measure.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for measure (if applicable)
      # A legal or geographic region in which the measure is intended to be used.
      # It may be possible for the measure to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'Measure.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this measure is defined
      # Explanation of why this measure is needed and why it has been designed as it has.
      # This element does not describe the usage of the measure. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this measure.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'Measure.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Describes the clinical usage of the measure
      # A detailed description, from a clinical perspective, of how the measure is used.
      'usage' => {
        'type'=>'string',
        'path'=>'Measure.usage',
        'min'=>0,
        'max'=>1
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the measure and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the measure.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'Measure.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the measure was approved by publisher
      # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
      # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
      'approvalDate' => {
        'type'=>'date',
        'path'=>'Measure.approvalDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the measure was last reviewed
      # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
      # If specified, this date follows the original approval date.
      'lastReviewDate' => {
        'type'=>'date',
        'path'=>'Measure.lastReviewDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the measure is expected to be used
      # The period during which the measure content was or is planned to be in active use.
      # The effective period for a measure  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'Measure.effectivePeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # The category of the measure, such as Education, Treatment, Assessment, etc.
      # Descriptive topics related to the content of the measure. Topics provide a high-level categorization grouping types of measures that can be useful for filtering and searching.
      'topic' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/definition-topic'=>[ 'treatment', 'education', 'assessment' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Measure.topic',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/definition-topic'}
      },
      ##
      # Who authored the content
      # An individiual or organization primarily involved in the creation and maintenance of the content.
      'author' => {
        'type'=>'ContactDetail',
        'path'=>'Measure.author',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who edited the content
      # An individual or organization primarily responsible for internal coherence of the content.
      'editor' => {
        'type'=>'ContactDetail',
        'path'=>'Measure.editor',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who reviewed the content
      # An individual or organization primarily responsible for review of some aspect of the content.
      'reviewer' => {
        'type'=>'ContactDetail',
        'path'=>'Measure.reviewer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who endorsed the content
      # An individual or organization responsible for officially endorsing the content for use in some setting.
      'endorser' => {
        'type'=>'ContactDetail',
        'path'=>'Measure.endorser',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional documentation, citations, etc.
      # Related artifacts such as additional documentation, justification, or bibliographic references.
      # Each related artifact is either an attachment, or a reference to another resource, but not both.
      'relatedArtifact' => {
        'type'=>'RelatedArtifact',
        'path'=>'Measure.relatedArtifact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Logic used by the measure
      # A reference to a Library resource containing the formal logic used by the measure.
      'library' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Library'],
        'type'=>'canonical',
        'path'=>'Measure.library',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Disclaimer for use of the measure or its referenced content
      # Notices and disclaimers regarding the use of the measure or related to intellectual property (such as code systems) referenced by the measure.
      'disclaimer' => {
        'type'=>'markdown',
        'path'=>'Measure.disclaimer',
        'min'=>0,
        'max'=>1
      },
      ##
      # proportion | ratio | continuous-variable | cohort
      # Indicates how the calculation is performed for the measure, including proportion, ratio, continuous-variable, and cohort. The value set is extensible, allowing additional measure scoring types to be represented.
      'scoring' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/measure-scoring'=>[ 'proportion', 'ratio', 'continuous-variable', 'cohort' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Measure.scoring',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/measure-scoring'}
      },
      ##
      # opportunity | all-or-nothing | linear | weighted
      # If this is a composite measure, the scoring method used to combine the component measures to determine the composite score.
      'compositeScoring' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/composite-measure-scoring'=>[ 'opportunity', 'all-or-nothing', 'linear', 'weighted' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Measure.compositeScoring',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/composite-measure-scoring'}
      },
      ##
      # process | outcome | structure | patient-reported-outcome | composite
      # Indicates whether the measure is used to examine a process, an outcome over time, a patient-reported outcome, or a structure measure such as utilization.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/measure-type'=>[ 'process', 'outcome', 'structure', 'patient-reported-outcome', 'composite' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Measure.type',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/measure-type'}
      },
      ##
      # How risk adjustment is applied for this measure
      # A description of the risk adjustment factors that may impact the resulting score for the measure and how they may be accounted for when computing and reporting measure results.
      # Describes the method of adjusting for clinical severity and conditions present at the start of care that can influence patient outcomes for making valid comparisons of outcome measures across providers. Indicates whether a measure is subject to the statistical process for reducing, removing, or clarifying the influences of confounding factors to allow for more useful comparisons.
      'riskAdjustment' => {
        'type'=>'string',
        'path'=>'Measure.riskAdjustment',
        'min'=>0,
        'max'=>1
      },
      ##
      # How is rate aggregation performed for this measure
      # Describes how to combine the information calculated, based on logic in each of several populations, into one summarized result.
      # The measure rate for an organization or clinician is based upon the entities’ aggregate data and summarizes the performance of the entity over a given time period (e.g., monthly, quarterly, yearly). The aggregated data are derived from the results of a specific measure algorithm and, if appropriate, the application of specific risk adjustment models.  Can also be used to describe how to risk adjust the data based on supplemental data elements described in the eMeasure (e.g., pneumonia hospital measures antibiotic selection in the ICU versus non-ICU and then the roll-up of the two). This could be applied to aggregated cohort measure definitions (e.g., CDC's aggregate reporting for TB at the state level).
      'rateAggregation' => {
        'type'=>'string',
        'path'=>'Measure.rateAggregation',
        'min'=>0,
        'max'=>1
      },
      ##
      # Detailed description of why the measure exists
      # Provides a succinct statement of the need for the measure. Usually includes statements pertaining to importance criterion: impact, gap in care, and evidence.
      'rationale' => {
        'type'=>'markdown',
        'path'=>'Measure.rationale',
        'min'=>0,
        'max'=>1
      },
      ##
      # Summary of clinical guidelines
      # Provides a summary of relevant clinical guidelines or other clinical recommendations supporting the measure.
      'clinicalRecommendationStatement' => {
        'type'=>'markdown',
        'path'=>'Measure.clinicalRecommendationStatement',
        'min'=>0,
        'max'=>1
      },
      ##
      # increase | decrease
      # Information on whether an increase or decrease in score is the preferred result (e.g., a higher score indicates better quality OR a lower score indicates better quality OR quality is within a range).
      'improvementNotation' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/measure-improvement-notation'=>[ 'increase', 'decrease' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Measure.improvementNotation',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/measure-improvement-notation'}
      },
      ##
      # Defined terms used in the measure documentation
      # Provides a description of an individual term used within the measure.
      'definition' => {
        'type'=>'markdown',
        'path'=>'Measure.definition',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional guidance for implementers
      # Additional guidance for the measure including how it can be used in a clinical context, and the intent of the measure.
      'guidance' => {
        'type'=>'markdown',
        'path'=>'Measure.guidance',
        'min'=>0,
        'max'=>1
      },
      ##
      # Population criteria group
      # A group of population criteria for the measure.
      'group' => {
        'type'=>'Measure::Group',
        'path'=>'Measure.group',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What other data should be reported with the measure
      # The supplemental data criteria for the measure report, specified as either the name of a valid CQL expression within a referenced library, or a valid FHIR Resource Path.
      # Note that supplemental data are reported as observations for each patient and included in the evaluatedResources bundle. See the MeasureReport resource or the Quality Reporting topic for more information.
      'supplementalData' => {
        'type'=>'Measure::SupplementalData',
        'path'=>'Measure.supplementalData',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Population criteria group
    # A group of population criteria for the measure.
    class Group < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Group.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Group.extension',
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
          'path'=>'Group.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Meaning of the group
        # Indicates a meaning for the group. This can be as simple as a unique identifier, or it can establish meaning in a broader context by drawing from a terminology, allowing groups to be correlated across measures.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Group.code',
          'min'=>0,
          'max'=>1
        },
        ##
        # Summary description
        # The human readable description of this population group.
        'description' => {
          'type'=>'string',
          'path'=>'Group.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # Population criteria
        # A population criteria for the measure.
        'population' => {
          'type'=>'Measure::Group::Population',
          'path'=>'Group.population',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Stratifier criteria for the measure
        # The stratifier criteria for the measure report, specified as either the name of a valid CQL expression defined within a referenced library or a valid FHIR Resource Path.
        'stratifier' => {
          'type'=>'Measure::Group::Stratifier',
          'path'=>'Group.stratifier',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Population criteria
      # A population criteria for the measure.
      class Population < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Population.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Population.extension',
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
            'path'=>'Population.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # initial-population | numerator | numerator-exclusion | denominator | denominator-exclusion | denominator-exception | measure-population | measure-population-exclusion | measure-observation
          # The type of population criteria.
          'code' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/measure-population'=>[ 'initial-population', 'numerator', 'numerator-exclusion', 'denominator', 'denominator-exclusion', 'denominator-exception', 'measure-population', 'measure-population-exclusion', 'measure-observation' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Population.code',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/measure-population'}
          },
          ##
          # The human readable description of this population criteria.
          'description' => {
            'type'=>'string',
            'path'=>'Population.description',
            'min'=>0,
            'max'=>1
          },
          ##
          # The criteria that defines this population
          # An expression that specifies the criteria for the population, typically the name of an expression in a library.
          # In the case of a continuous-variable or ratio measure, this may be the name of a function that calculates the value of the individual observation for each patient or event in the population. For these types of measures, individual observations are reported as observation resources included in the evaluatedResources bundle for each patient. See the MeasureReport resource or the Quality Reporting topic for more information.
          'criteria' => {
            'type'=>'Expression',
            'path'=>'Population.criteria',
            'min'=>1,
            'max'=>1
          }
        }
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        attr_accessor :id                             # 0-1 string
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
        # initial-population | numerator | numerator-exclusion | denominator | denominator-exclusion | denominator-exception | measure-population | measure-population-exclusion | measure-observation
        # The type of population criteria.
        attr_accessor :code                           # 0-1 CodeableConcept
        ##
        # The human readable description of this population criteria.
        attr_accessor :description                    # 0-1 string
        ##
        # The criteria that defines this population
        # An expression that specifies the criteria for the population, typically the name of an expression in a library.
        # In the case of a continuous-variable or ratio measure, this may be the name of a function that calculates the value of the individual observation for each patient or event in the population. For these types of measures, individual observations are reported as observation resources included in the evaluatedResources bundle for each patient. See the MeasureReport resource or the Quality Reporting topic for more information.
        attr_accessor :criteria                       # 1-1 Expression
      end

      ##
      # Stratifier criteria for the measure
      # The stratifier criteria for the measure report, specified as either the name of a valid CQL expression defined within a referenced library or a valid FHIR Resource Path.
      class Stratifier < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Stratifier.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Stratifier.extension',
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
            'path'=>'Stratifier.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Meaning of the stratifier
          # Indicates a meaning for the stratifier. This can be as simple as a unique identifier, or it can establish meaning in a broader context by drawing from a terminology, allowing stratifiers to be correlated across measures.
          'code' => {
            'type'=>'CodeableConcept',
            'path'=>'Stratifier.code',
            'min'=>0,
            'max'=>1
          },
          ##
          # The human readable description of this stratifier criteria.
          'description' => {
            'type'=>'string',
            'path'=>'Stratifier.description',
            'min'=>0,
            'max'=>1
          },
          ##
          # How the measure should be stratified
          # An expression that specifies the criteria for the stratifier. This is typically the name of an expression defined within a referenced library, but it may also be a path to a stratifier element.
          'criteria' => {
            'type'=>'Expression',
            'path'=>'Stratifier.criteria',
            'min'=>0,
            'max'=>1
          },
          ##
          # Stratifier criteria component for the measure
          # A component of the stratifier criteria for the measure report, specified as either the name of a valid CQL expression defined within a referenced library or a valid FHIR Resource Path.
          # Stratifiers are defined either as a single criteria, or as a set of component criteria.
          'component' => {
            'type'=>'Measure::Group::Stratifier::Component',
            'path'=>'Stratifier.component',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Stratifier criteria component for the measure
        # A component of the stratifier criteria for the measure report, specified as either the name of a valid CQL expression defined within a referenced library or a valid FHIR Resource Path.
        # Stratifiers are defined either as a single criteria, or as a set of component criteria.
        class Component < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'string',
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
            # Meaning of the stratifier component
            # Indicates a meaning for the stratifier component. This can be as simple as a unique identifier, or it can establish meaning in a broader context by drawing from a terminology, allowing stratifiers to be correlated across measures.
            'code' => {
              'type'=>'CodeableConcept',
              'path'=>'Component.code',
              'min'=>0,
              'max'=>1
            },
            ##
            # The human readable description of this stratifier component
            # The human readable description of this stratifier criteria component.
            'description' => {
              'type'=>'string',
              'path'=>'Component.description',
              'min'=>0,
              'max'=>1
            },
            ##
            # Component of how the measure should be stratified
            # An expression that specifies the criteria for this component of the stratifier. This is typically the name of an expression defined within a referenced library, but it may also be a path to a stratifier element.
            'criteria' => {
              'type'=>'Expression',
              'path'=>'Component.criteria',
              'min'=>1,
              'max'=>1
            }
          }
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          attr_accessor :id                             # 0-1 string
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
          # Meaning of the stratifier component
          # Indicates a meaning for the stratifier component. This can be as simple as a unique identifier, or it can establish meaning in a broader context by drawing from a terminology, allowing stratifiers to be correlated across measures.
          attr_accessor :code                           # 0-1 CodeableConcept
          ##
          # The human readable description of this stratifier component
          # The human readable description of this stratifier criteria component.
          attr_accessor :description                    # 0-1 string
          ##
          # Component of how the measure should be stratified
          # An expression that specifies the criteria for this component of the stratifier. This is typically the name of an expression defined within a referenced library, but it may also be a path to a stratifier element.
          attr_accessor :criteria                       # 1-1 Expression
        end
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        attr_accessor :id                             # 0-1 string
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
        # Meaning of the stratifier
        # Indicates a meaning for the stratifier. This can be as simple as a unique identifier, or it can establish meaning in a broader context by drawing from a terminology, allowing stratifiers to be correlated across measures.
        attr_accessor :code                           # 0-1 CodeableConcept
        ##
        # The human readable description of this stratifier criteria.
        attr_accessor :description                    # 0-1 string
        ##
        # How the measure should be stratified
        # An expression that specifies the criteria for the stratifier. This is typically the name of an expression defined within a referenced library, but it may also be a path to a stratifier element.
        attr_accessor :criteria                       # 0-1 Expression
        ##
        # Stratifier criteria component for the measure
        # A component of the stratifier criteria for the measure report, specified as either the name of a valid CQL expression defined within a referenced library or a valid FHIR Resource Path.
        # Stratifiers are defined either as a single criteria, or as a set of component criteria.
        attr_accessor :component                      # 0-* [ Measure::Group::Stratifier::Component ]
      end
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      attr_accessor :id                             # 0-1 string
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
      # Meaning of the group
      # Indicates a meaning for the group. This can be as simple as a unique identifier, or it can establish meaning in a broader context by drawing from a terminology, allowing groups to be correlated across measures.
      attr_accessor :code                           # 0-1 CodeableConcept
      ##
      # Summary description
      # The human readable description of this population group.
      attr_accessor :description                    # 0-1 string
      ##
      # Population criteria
      # A population criteria for the measure.
      attr_accessor :population                     # 0-* [ Measure::Group::Population ]
      ##
      # Stratifier criteria for the measure
      # The stratifier criteria for the measure report, specified as either the name of a valid CQL expression defined within a referenced library or a valid FHIR Resource Path.
      attr_accessor :stratifier                     # 0-* [ Measure::Group::Stratifier ]
    end

    ##
    # What other data should be reported with the measure
    # The supplemental data criteria for the measure report, specified as either the name of a valid CQL expression within a referenced library, or a valid FHIR Resource Path.
    # Note that supplemental data are reported as observations for each patient and included in the evaluatedResources bundle. See the MeasureReport resource or the Quality Reporting topic for more information.
    class SupplementalData < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'SupplementalData.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'SupplementalData.extension',
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
          'path'=>'SupplementalData.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Meaning of the supplemental data
        # Indicates a meaning for the supplemental data. This can be as simple as a unique identifier, or it can establish meaning in a broader context by drawing from a terminology, allowing supplemental data to be correlated across measures.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'SupplementalData.code',
          'min'=>0,
          'max'=>1
        },
        ##
        # supplemental-data | risk-adjustment-factor
        # An indicator of the intended usage for the supplemental data element. Supplemental data indicates the data is additional information requested to augment the measure information. Risk adjustment factor indicates the data is additional information used to calculate risk adjustment factors when applying a risk model to the measure calculation.
        'usage' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/measure-data-usage'=>[ 'supplemental-data', 'risk-adjustment-factor' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'SupplementalData.usage',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/measure-data-usage'}
        },
        ##
        # The human readable description of this supplemental data.
        'description' => {
          'type'=>'string',
          'path'=>'SupplementalData.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # Expression describing additional data to be reported
        # The criteria for the supplemental data. This is typically the name of a valid expression defined within a referenced library, but it may also be a path to a specific data element. The criteria defines the data to be returned for this element.
        'criteria' => {
          'type'=>'Expression',
          'path'=>'SupplementalData.criteria',
          'min'=>1,
          'max'=>1
        }
      }
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      attr_accessor :id                             # 0-1 string
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
      # Meaning of the supplemental data
      # Indicates a meaning for the supplemental data. This can be as simple as a unique identifier, or it can establish meaning in a broader context by drawing from a terminology, allowing supplemental data to be correlated across measures.
      attr_accessor :code                           # 0-1 CodeableConcept
      ##
      # supplemental-data | risk-adjustment-factor
      # An indicator of the intended usage for the supplemental data element. Supplemental data indicates the data is additional information requested to augment the measure information. Risk adjustment factor indicates the data is additional information used to calculate risk adjustment factors when applying a risk model to the measure calculation.
      attr_accessor :usage                          # 0-* [ CodeableConcept ]
      ##
      # The human readable description of this supplemental data.
      attr_accessor :description                    # 0-1 string
      ##
      # Expression describing additional data to be reported
      # The criteria for the supplemental data. This is typically the name of a valid expression defined within a referenced library, but it may also be a path to a specific data element. The criteria defines the data to be returned for this element.
      attr_accessor :criteria                       # 1-1 Expression
    end
    ##
    # Logical id of this artifact
    # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
    # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
    attr_accessor :id                             # 0-1 string
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
    # Canonical identifier for this measure, represented as a URI (globally unique)
    # An absolute URI that is used to identify this measure when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this measure is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the measure is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 0-1 uri
    ##
    # Additional identifier for the measure
    # A formal identifier that is used to identify this measure when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this measure outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Business version of the measure
    # The identifier that is used to identify this version of the measure when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the measure author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active artifacts.
    # There may be different measure instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the measure with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this measure (computer friendly)
    # A natural language name identifying the measure. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 0-1 string
    ##
    # Name for this measure (human friendly)
    # A short, descriptive, user-friendly title for the measure.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # Subordinate title of the measure
    # An explanatory or alternate title for the measure giving additional information about its content.
    attr_accessor :subtitle                       # 0-1 string
    ##
    # draft | active | retired | unknown
    # The status of this measure. Enables tracking the life-cycle of the content.
    # Allows filtering of measures that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this measure is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of measures that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
    # The intended subjects for the measure. If this element is not provided, a Patient subject is assumed, but the subject of the measure can be anything.
    # The subject of the measure is critical in interpreting the criteria definitions, as the logic in the measures is evaluated with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what content is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
    attr_accessor :subjectCodeableConcept         # 0-1 CodeableConcept
    ##
    # E.g. Patient, Practitioner, RelatedPerson, Organization, Location, Device
    # The intended subjects for the measure. If this element is not provided, a Patient subject is assumed, but the subject of the measure can be anything.
    # The subject of the measure is critical in interpreting the criteria definitions, as the logic in the measures is evaluated with respect to a particular subject. This corresponds roughly to the notion of a Compartment in that it limits what content is available based on its relationship to the subject. In CQL, this corresponds to the context declaration.
    attr_accessor :subjectReference               # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Group)
    ##
    # Date last changed
    # The date  (and optionally time) when the measure was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the measure changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the measure. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the measure.
    # Usually an organization but may be an individual. The publisher (or steward) of the measure is the organization or individual primarily responsible for the maintenance and upkeep of the measure. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the measure. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the measure
    # A free text natural language description of the measure from a consumer's perspective.
    # This description can be used to capture details such as why the measure was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the measure as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the measure is presumed to be the predominant language in the place the measure was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate measure instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for measure (if applicable)
    # A legal or geographic region in which the measure is intended to be used.
    # It may be possible for the measure to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this measure is defined
    # Explanation of why this measure is needed and why it has been designed as it has.
    # This element does not describe the usage of the measure. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this measure.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Describes the clinical usage of the measure
    # A detailed description, from a clinical perspective, of how the measure is used.
    attr_accessor :usage                          # 0-1 string
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the measure and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the measure.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # When the measure was approved by publisher
    # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
    # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
    attr_accessor :approvalDate                   # 0-1 date
    ##
    # When the measure was last reviewed
    # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
    # If specified, this date follows the original approval date.
    attr_accessor :lastReviewDate                 # 0-1 date
    ##
    # When the measure is expected to be used
    # The period during which the measure content was or is planned to be in active use.
    # The effective period for a measure  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
    attr_accessor :effectivePeriod                # 0-1 Period
    ##
    # The category of the measure, such as Education, Treatment, Assessment, etc.
    # Descriptive topics related to the content of the measure. Topics provide a high-level categorization grouping types of measures that can be useful for filtering and searching.
    attr_accessor :topic                          # 0-* [ CodeableConcept ]
    ##
    # Who authored the content
    # An individiual or organization primarily involved in the creation and maintenance of the content.
    attr_accessor :author                         # 0-* [ ContactDetail ]
    ##
    # Who edited the content
    # An individual or organization primarily responsible for internal coherence of the content.
    attr_accessor :editor                         # 0-* [ ContactDetail ]
    ##
    # Who reviewed the content
    # An individual or organization primarily responsible for review of some aspect of the content.
    attr_accessor :reviewer                       # 0-* [ ContactDetail ]
    ##
    # Who endorsed the content
    # An individual or organization responsible for officially endorsing the content for use in some setting.
    attr_accessor :endorser                       # 0-* [ ContactDetail ]
    ##
    # Additional documentation, citations, etc.
    # Related artifacts such as additional documentation, justification, or bibliographic references.
    # Each related artifact is either an attachment, or a reference to another resource, but not both.
    attr_accessor :relatedArtifact                # 0-* [ RelatedArtifact ]
    ##
    # Logic used by the measure
    # A reference to a Library resource containing the formal logic used by the measure.
    attr_accessor :library                        # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/Library) ]
    ##
    # Disclaimer for use of the measure or its referenced content
    # Notices and disclaimers regarding the use of the measure or related to intellectual property (such as code systems) referenced by the measure.
    attr_accessor :disclaimer                     # 0-1 markdown
    ##
    # proportion | ratio | continuous-variable | cohort
    # Indicates how the calculation is performed for the measure, including proportion, ratio, continuous-variable, and cohort. The value set is extensible, allowing additional measure scoring types to be represented.
    attr_accessor :scoring                        # 0-1 CodeableConcept
    ##
    # opportunity | all-or-nothing | linear | weighted
    # If this is a composite measure, the scoring method used to combine the component measures to determine the composite score.
    attr_accessor :compositeScoring               # 0-1 CodeableConcept
    ##
    # process | outcome | structure | patient-reported-outcome | composite
    # Indicates whether the measure is used to examine a process, an outcome over time, a patient-reported outcome, or a structure measure such as utilization.
    attr_accessor :type                           # 0-* [ CodeableConcept ]
    ##
    # How risk adjustment is applied for this measure
    # A description of the risk adjustment factors that may impact the resulting score for the measure and how they may be accounted for when computing and reporting measure results.
    # Describes the method of adjusting for clinical severity and conditions present at the start of care that can influence patient outcomes for making valid comparisons of outcome measures across providers. Indicates whether a measure is subject to the statistical process for reducing, removing, or clarifying the influences of confounding factors to allow for more useful comparisons.
    attr_accessor :riskAdjustment                 # 0-1 string
    ##
    # How is rate aggregation performed for this measure
    # Describes how to combine the information calculated, based on logic in each of several populations, into one summarized result.
    # The measure rate for an organization or clinician is based upon the entities’ aggregate data and summarizes the performance of the entity over a given time period (e.g., monthly, quarterly, yearly). The aggregated data are derived from the results of a specific measure algorithm and, if appropriate, the application of specific risk adjustment models.  Can also be used to describe how to risk adjust the data based on supplemental data elements described in the eMeasure (e.g., pneumonia hospital measures antibiotic selection in the ICU versus non-ICU and then the roll-up of the two). This could be applied to aggregated cohort measure definitions (e.g., CDC's aggregate reporting for TB at the state level).
    attr_accessor :rateAggregation                # 0-1 string
    ##
    # Detailed description of why the measure exists
    # Provides a succinct statement of the need for the measure. Usually includes statements pertaining to importance criterion: impact, gap in care, and evidence.
    attr_accessor :rationale                      # 0-1 markdown
    ##
    # Summary of clinical guidelines
    # Provides a summary of relevant clinical guidelines or other clinical recommendations supporting the measure.
    attr_accessor :clinicalRecommendationStatement # 0-1 markdown
    ##
    # increase | decrease
    # Information on whether an increase or decrease in score is the preferred result (e.g., a higher score indicates better quality OR a lower score indicates better quality OR quality is within a range).
    attr_accessor :improvementNotation            # 0-1 CodeableConcept
    ##
    # Defined terms used in the measure documentation
    # Provides a description of an individual term used within the measure.
    attr_accessor :definition                     # 0-* [ markdown ]
    ##
    # Additional guidance for implementers
    # Additional guidance for the measure including how it can be used in a clinical context, and the intent of the measure.
    attr_accessor :guidance                       # 0-1 markdown
    ##
    # Population criteria group
    # A group of population criteria for the measure.
    attr_accessor :group                          # 0-* [ Measure::Group ]
    ##
    # What other data should be reported with the measure
    # The supplemental data criteria for the measure report, specified as either the name of a valid CQL expression within a referenced library, or a valid FHIR Resource Path.
    # Note that supplemental data are reported as observations for each patient and included in the evaluatedResources bundle. See the MeasureReport resource or the Quality Reporting topic for more information.
    attr_accessor :supplementalData               # 0-* [ Measure::SupplementalData ]

    def resourceType
      'Measure'
    end
  end
end
