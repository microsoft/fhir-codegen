module FHIR

  ##
  # The ChargeItemDefinition resource provides the properties that apply to the (billing) codes necessary to calculate costs and prices. The properties may differ largely depending on type and realm, therefore this resource gives only a rough structure and requires profiling for each type of billing code system.
  class ChargeItemDefinition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'description', 'effective', 'identifier', 'jurisdiction', 'publisher', 'status', 'title', 'url', 'version']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'ChargeItemDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ChargeItemDefinition.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ChargeItemDefinition.implicitRules',
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
        'path'=>'ChargeItemDefinition.language',
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
        'path'=>'ChargeItemDefinition.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ChargeItemDefinition.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ChargeItemDefinition.extension',
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
        'path'=>'ChargeItemDefinition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this charge item definition, represented as a URI (globally unique)
      # An absolute URI that is used to identify this charge item definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this charge item definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the charge item definition is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'ChargeItemDefinition.url',
        'min'=>1,
        'max'=>1
      },
      ##
      # Additional identifier for the charge item definition
      # A formal identifier that is used to identify this charge item definition when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this charge item definition outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ChargeItemDefinition.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business version of the charge item definition
      # The identifier that is used to identify this version of the charge item definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the charge item definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active assets.
      # There may be different charge item definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the charge item definition with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'ChargeItemDefinition.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this charge item definition (human friendly)
      # A short, descriptive, user-friendly title for the charge item definition.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'ChargeItemDefinition.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # Underlying externally-defined charge item definition
      # The URL pointing to an externally-defined charge item definition that is adhered to in whole or in part by this definition.
      'derivedFromUri' => {
        'type'=>'uri',
        'path'=>'ChargeItemDefinition.derivedFromUri',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A larger definition of which this particular definition is a component or step.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ChargeItemDefinition'],
        'type'=>'canonical',
        'path'=>'ChargeItemDefinition.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Completed or terminated request(s) whose function is taken by this new request
      # As new versions of a protocol or guideline are defined, allows identification of what versions are replaced by a new instance.
      'replaces' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ChargeItemDefinition'],
        'type'=>'canonical',
        'path'=>'ChargeItemDefinition.replaces',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # draft | active | retired | unknown
      # The current state of the ChargeItemDefinition.
      # Allows filtering of charge item definitions that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'ChargeItemDefinition.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this charge item definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of charge item definitions that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'ChargeItemDefinition.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the charge item definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the charge item definition changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the charge item definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'ChargeItemDefinition.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the charge item definition.
      # Usually an organization but may be an individual. The publisher (or steward) of the charge item definition is the organization or individual primarily responsible for the maintenance and upkeep of the charge item definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the charge item definition. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'ChargeItemDefinition.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'ChargeItemDefinition.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the charge item definition
      # A free text natural language description of the charge item definition from a consumer's perspective.
      # This description can be used to capture details such as why the charge item definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the charge item definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the charge item definition is presumed to be the predominant language in the place the charge item definition was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'ChargeItemDefinition.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate charge item definition instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'ChargeItemDefinition.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for charge item definition (if applicable)
      # A legal or geographic region in which the charge item definition is intended to be used.
      # It may be possible for the charge item definition to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'ChargeItemDefinition.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the charge item definition and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the charge item definition.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'ChargeItemDefinition.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the charge item definition was approved by publisher
      # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
      # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
      'approvalDate' => {
        'type'=>'date',
        'path'=>'ChargeItemDefinition.approvalDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the charge item definition was last reviewed
      # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
      # If specified, this date follows the original approval date.
      'lastReviewDate' => {
        'type'=>'date',
        'path'=>'ChargeItemDefinition.lastReviewDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the charge item definition is expected to be used
      # The period during which the charge item definition content was or is planned to be in active use.
      # The effective period for a charge item definition  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'ChargeItemDefinition.effectivePeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # Billing codes or product types this definition applies to
      # The defined billing details in this resource pertain to the given billing code.
      'code' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/chargeitem-billingcodes'=>[ '1100', '1210', '1320' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ChargeItemDefinition.code',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/chargeitem-billingcodes'}
      },
      ##
      # Instances this definition applies to
      # The defined billing details in this resource pertain to the given product instance(s).
      # In case of highly customized, individually produced or fitted devices/substances, the pricing information may be different for each instance of the product. This reference links pricing details to specific product instances.
      'instance' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Medication', 'http://hl7.org/fhir/StructureDefinition/Substance', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'ChargeItemDefinition.instance',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Whether or not the billing code is applicable
      # Expressions that describe applicability criteria for the billing code.
      # The applicability conditions can be used to ascertain whether a billing item is allowed in a specific context. E.g. some billing codes may only be applicable in out-patient settings, only to male/female patients or only to children.
      'applicability' => {
        'type'=>'ChargeItemDefinition::Applicability',
        'path'=>'ChargeItemDefinition.applicability',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Group of properties which are applicable under the same conditions. If no applicability rules are established for the group, then all properties always apply.
      'propertyGroup' => {
        'type'=>'ChargeItemDefinition::PropertyGroup',
        'path'=>'ChargeItemDefinition.propertyGroup',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Whether or not the billing code is applicable
    # Expressions that describe applicability criteria for the billing code.
    # The applicability conditions can be used to ascertain whether a billing item is allowed in a specific context. E.g. some billing codes may only be applicable in out-patient settings, only to male/female patients or only to children.
    class Applicability < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Applicability.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Applicability.extension',
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
          'path'=>'Applicability.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Natural language description of the condition
        # A brief, natural language description of the condition that effectively communicates the intended semantics.
        'description' => {
          'type'=>'string',
          'path'=>'Applicability.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # Language of the expression
        # The media type of the language for the expression, e.g. "text/cql" for Clinical Query Language expressions or "text/fhirpath" for FHIRPath expressions.
        'language' => {
          'type'=>'string',
          'path'=>'Applicability.language',
          'min'=>0,
          'max'=>1
        },
        ##
        # Boolean-valued expression
        # An expression that returns true or false, indicating whether the condition is satisfied. When using FHIRPath expressions, the %context environment variable must be replaced at runtime with the ChargeItem resource to which this definition is applied.
        # Please note that FHIRPath Expressions can only be evaluated in the scope of the current ChargeItem resource to which this definition is being applied.
        # FHIRPath expressions can traverse into other resources linked from the ChargeItem resource, however, testing rules such as that a billing code may be billed only once per encounter need a wider scope. In such scenarios, CQL may be the appropriate choice.
        'expression' => {
          'type'=>'string',
          'path'=>'Applicability.expression',
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
      # Natural language description of the condition
      # A brief, natural language description of the condition that effectively communicates the intended semantics.
      attr_accessor :description                    # 0-1 string
      ##
      # Language of the expression
      # The media type of the language for the expression, e.g. "text/cql" for Clinical Query Language expressions or "text/fhirpath" for FHIRPath expressions.
      attr_accessor :language                       # 0-1 string
      ##
      # Boolean-valued expression
      # An expression that returns true or false, indicating whether the condition is satisfied. When using FHIRPath expressions, the %context environment variable must be replaced at runtime with the ChargeItem resource to which this definition is applied.
      # Please note that FHIRPath Expressions can only be evaluated in the scope of the current ChargeItem resource to which this definition is being applied.
      # FHIRPath expressions can traverse into other resources linked from the ChargeItem resource, however, testing rules such as that a billing code may be billed only once per encounter need a wider scope. In such scenarios, CQL may be the appropriate choice.
      attr_accessor :expression                     # 0-1 string
    end

    ##
    # Group of properties which are applicable under the same conditions. If no applicability rules are established for the group, then all properties always apply.
    class PropertyGroup < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'PropertyGroup.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'PropertyGroup.extension',
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
          'path'=>'PropertyGroup.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Conditions under which the priceComponent is applicable
        # Expressions that describe applicability criteria for the priceComponent.
        # The applicability conditions can be used to ascertain whether a billing item is allowed in a specific context. E.g. some billing codes may only be applicable in out-patient settings, only to male/female patients or only to children.
        'applicability' => {
          'type'=>'ChargeItemDefinition::Applicability',
          'path'=>'PropertyGroup.applicability',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Components of total line item price
        # The price for a ChargeItem may be calculated as a base price with surcharges/deductions that apply in certain conditions. A ChargeItemDefinition resource that defines the prices, factors and conditions that apply to a billing code is currently under development. The priceComponent element can be used to offer transparency to the recipient of the Invoice of how the prices have been calculated.
        'priceComponent' => {
          'type'=>'ChargeItemDefinition::PropertyGroup::PriceComponent',
          'path'=>'PropertyGroup.priceComponent',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Components of total line item price
      # The price for a ChargeItem may be calculated as a base price with surcharges/deductions that apply in certain conditions. A ChargeItemDefinition resource that defines the prices, factors and conditions that apply to a billing code is currently under development. The priceComponent element can be used to offer transparency to the recipient of the Invoice of how the prices have been calculated.
      class PriceComponent < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'PriceComponent.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'PriceComponent.extension',
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
            'path'=>'PriceComponent.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # base | surcharge | deduction | discount | tax | informational
          # This code identifies the type of the component.
          'type' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/invoice-priceComponentType'=>[ 'base', 'surcharge', 'deduction', 'discount', 'tax', 'informational' ]
            },
            'type'=>'code',
            'path'=>'PriceComponent.type',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/invoice-priceComponentType'}
          },
          ##
          # Code identifying the specific component
          # A code that identifies the component. Codes may be used to differentiate between kinds of taxes, surcharges, discounts etc.
          'code' => {
            'type'=>'CodeableConcept',
            'path'=>'PriceComponent.code',
            'min'=>0,
            'max'=>1
          },
          ##
          # Factor used for calculating this component
          # The factor that has been applied on the base price for calculating this component.
          'factor' => {
            'type'=>'decimal',
            'path'=>'PriceComponent.factor',
            'min'=>0,
            'max'=>1
          },
          ##
          # Monetary amount associated with this component
          # The amount calculated for this component.
          'amount' => {
            'type'=>'Money',
            'path'=>'PriceComponent.amount',
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
        # base | surcharge | deduction | discount | tax | informational
        # This code identifies the type of the component.
        attr_accessor :type                           # 1-1 code
        ##
        # Code identifying the specific component
        # A code that identifies the component. Codes may be used to differentiate between kinds of taxes, surcharges, discounts etc.
        attr_accessor :code                           # 0-1 CodeableConcept
        ##
        # Factor used for calculating this component
        # The factor that has been applied on the base price for calculating this component.
        attr_accessor :factor                         # 0-1 decimal
        ##
        # Monetary amount associated with this component
        # The amount calculated for this component.
        attr_accessor :amount                         # 0-1 Money
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
      # Conditions under which the priceComponent is applicable
      # Expressions that describe applicability criteria for the priceComponent.
      # The applicability conditions can be used to ascertain whether a billing item is allowed in a specific context. E.g. some billing codes may only be applicable in out-patient settings, only to male/female patients or only to children.
      attr_accessor :applicability                  # 0-* [ ChargeItemDefinition::Applicability ]
      ##
      # Components of total line item price
      # The price for a ChargeItem may be calculated as a base price with surcharges/deductions that apply in certain conditions. A ChargeItemDefinition resource that defines the prices, factors and conditions that apply to a billing code is currently under development. The priceComponent element can be used to offer transparency to the recipient of the Invoice of how the prices have been calculated.
      attr_accessor :priceComponent                 # 0-* [ ChargeItemDefinition::PropertyGroup::PriceComponent ]
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
    # Canonical identifier for this charge item definition, represented as a URI (globally unique)
    # An absolute URI that is used to identify this charge item definition when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this charge item definition is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the charge item definition is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 1-1 uri
    ##
    # Additional identifier for the charge item definition
    # A formal identifier that is used to identify this charge item definition when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this charge item definition outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Business version of the charge item definition
    # The identifier that is used to identify this version of the charge item definition when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the charge item definition author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence. To provide a version consistent with the Decision Support Service specification, use the format Major.Minor.Revision (e.g. 1.0.0). For more information on versioning knowledge assets, refer to the Decision Support Service specification. Note that a version is required for non-experimental active assets.
    # There may be different charge item definition instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the charge item definition with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this charge item definition (human friendly)
    # A short, descriptive, user-friendly title for the charge item definition.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # Underlying externally-defined charge item definition
    # The URL pointing to an externally-defined charge item definition that is adhered to in whole or in part by this definition.
    attr_accessor :derivedFromUri                 # 0-* [ uri ]
    ##
    # A larger definition of which this particular definition is a component or step.
    attr_accessor :partOf                         # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/ChargeItemDefinition) ]
    ##
    # Completed or terminated request(s) whose function is taken by this new request
    # As new versions of a protocol or guideline are defined, allows identification of what versions are replaced by a new instance.
    attr_accessor :replaces                       # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/ChargeItemDefinition) ]
    ##
    # draft | active | retired | unknown
    # The current state of the ChargeItemDefinition.
    # Allows filtering of charge item definitions that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this charge item definition is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of charge item definitions that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Date last changed
    # The date  (and optionally time) when the charge item definition was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the charge item definition changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the charge item definition. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the charge item definition.
    # Usually an organization but may be an individual. The publisher (or steward) of the charge item definition is the organization or individual primarily responsible for the maintenance and upkeep of the charge item definition. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the charge item definition. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the charge item definition
    # A free text natural language description of the charge item definition from a consumer's perspective.
    # This description can be used to capture details such as why the charge item definition was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the charge item definition as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the charge item definition is presumed to be the predominant language in the place the charge item definition was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate charge item definition instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for charge item definition (if applicable)
    # A legal or geographic region in which the charge item definition is intended to be used.
    # It may be possible for the charge item definition to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the charge item definition and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the charge item definition.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # When the charge item definition was approved by publisher
    # The date on which the resource content was approved by the publisher. Approval happens once when the content is officially approved for usage.
    # The 'date' element may be more recent than the approval date because of minor changes or editorial corrections.
    attr_accessor :approvalDate                   # 0-1 date
    ##
    # When the charge item definition was last reviewed
    # The date on which the resource content was last reviewed. Review happens periodically after approval but does not change the original approval date.
    # If specified, this date follows the original approval date.
    attr_accessor :lastReviewDate                 # 0-1 date
    ##
    # When the charge item definition is expected to be used
    # The period during which the charge item definition content was or is planned to be in active use.
    # The effective period for a charge item definition  determines when the content is applicable for usage and is independent of publication and review dates. For example, a measure intended to be used for the year 2016 might be published in 2015.
    attr_accessor :effectivePeriod                # 0-1 Period
    ##
    # Billing codes or product types this definition applies to
    # The defined billing details in this resource pertain to the given billing code.
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # Instances this definition applies to
    # The defined billing details in this resource pertain to the given product instance(s).
    # In case of highly customized, individually produced or fitted devices/substances, the pricing information may be different for each instance of the product. This reference links pricing details to specific product instances.
    attr_accessor :instance                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Medication|http://hl7.org/fhir/StructureDefinition/Substance|http://hl7.org/fhir/StructureDefinition/Device) ]
    ##
    # Whether or not the billing code is applicable
    # Expressions that describe applicability criteria for the billing code.
    # The applicability conditions can be used to ascertain whether a billing item is allowed in a specific context. E.g. some billing codes may only be applicable in out-patient settings, only to male/female patients or only to children.
    attr_accessor :applicability                  # 0-* [ ChargeItemDefinition::Applicability ]
    ##
    # Group of properties which are applicable under the same conditions. If no applicability rules are established for the group, then all properties always apply.
    attr_accessor :propertyGroup                  # 0-* [ ChargeItemDefinition::PropertyGroup ]

    def resourceType
      'ChargeItemDefinition'
    end
  end
end
