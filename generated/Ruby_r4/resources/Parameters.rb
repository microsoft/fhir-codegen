module FHIR

  ##
  # This resource is a non-persisted resource used to pass information into and back from an [operation](operations.html). It has no other use, and there is no RESTful endpoint associated with it.
  class Parameters < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Parameters.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Parameters.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Parameters.implicitRules',
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
        'path'=>'Parameters.language',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
      },
      ##
      # Operation Parameter
      # A parameter passed to or received from the operation.
      'parameter' => {
        'type'=>'Parameters::Parameter',
        'path'=>'Parameters.parameter',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Operation Parameter
    # A parameter passed to or received from the operation.
    class Parameter < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'value[x]' => ['Address', 'Age', 'Annotation', 'Attachment', 'base64Binary', 'boolean', 'canonical', 'code', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'date', 'dateTime', 'decimal', 'Distance', 'Dosage', 'Duration', 'Expression', 'HumanName', 'id', 'Identifier', 'instant', 'integer', 'markdown', 'Meta', 'Money', 'oid', 'ParameterDefinition', 'Period', 'positiveInt', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'string', 'time', 'Timing', 'TriggerDefinition', 'unsignedInt', 'uri', 'url', 'UsageContext', 'uuid']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
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
        # Name from the definition
        # The name of the parameter (reference to the operation definition).
        'name' => {
          'type'=>'string',
          'path'=>'Parameter.name',
          'min'=>1,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueAddress' => {
          'type'=>'Address',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueAge' => {
          'type'=>'Age',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueAnnotation' => {
          'type'=>'Annotation',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueAttachment' => {
          'type'=>'Attachment',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueBase64Binary' => {
          'type'=>'Base64Binary',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueBoolean' => {
          'type'=>'Boolean',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueCanonical' => {
          'type'=>'Canonical',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueCode' => {
          'type'=>'Code',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueCoding' => {
          'type'=>'Coding',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueContactDetail' => {
          'type'=>'ContactDetail',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueContactPoint' => {
          'type'=>'ContactPoint',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueContributor' => {
          'type'=>'Contributor',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueCount' => {
          'type'=>'Count',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueDataRequirement' => {
          'type'=>'DataRequirement',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueDate' => {
          'type'=>'Date',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueDateTime' => {
          'type'=>'DateTime',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueDecimal' => {
          'type'=>'Decimal',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueDistance' => {
          'type'=>'Distance',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueDosage' => {
          'type'=>'Dosage',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueDuration' => {
          'type'=>'Duration',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueExpression' => {
          'type'=>'Expression',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueHumanName' => {
          'type'=>'HumanName',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueId' => {
          'type'=>'Id',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueIdentifier' => {
          'type'=>'Identifier',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueInstant' => {
          'type'=>'Instant',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueInteger' => {
          'type'=>'Integer',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueMarkdown' => {
          'type'=>'Markdown',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueMeta' => {
          'type'=>'Meta',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueMoney' => {
          'type'=>'Money',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueOid' => {
          'type'=>'Oid',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueParameterDefinition' => {
          'type'=>'ParameterDefinition',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valuePeriod' => {
          'type'=>'Period',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valuePositiveInt' => {
          'type'=>'PositiveInt',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueRange' => {
          'type'=>'Range',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueRatio' => {
          'type'=>'Ratio',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueReference' => {
          'type'=>'Reference',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueRelatedArtifact' => {
          'type'=>'RelatedArtifact',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueSampledData' => {
          'type'=>'SampledData',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueSignature' => {
          'type'=>'Signature',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueString' => {
          'type'=>'String',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueTime' => {
          'type'=>'Time',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueTiming' => {
          'type'=>'Timing',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueTriggerDefinition' => {
          'type'=>'TriggerDefinition',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueUnsignedInt' => {
          'type'=>'UnsignedInt',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueUri' => {
          'type'=>'Uri',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueUrl' => {
          'type'=>'Url',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueUsageContext' => {
          'type'=>'UsageContext',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a data type
        # If the parameter is a data type.
        'valueUuid' => {
          'type'=>'Uuid',
          'path'=>'Parameter.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # If parameter is a whole resource
        # If the parameter is a whole resource.
        # When resolving references in resources, the operation definition may specify how references may be resolved between parameters. If a reference cannot be resolved between the parameters, the application should fall back to it's general resource resolution methods.
        'resource' => {
          'type'=>'Resource',
          'path'=>'Parameter.resource',
          'min'=>0,
          'max'=>1
        },
        ##
        # Named part of a multi-part parameter
        # A named part of a multi-part parameter.
        # Only one level of nested parameters is allowed.
        'part' => {
          'type'=>'Parameters::Parameter',
          'path'=>'Parameter.part',
          'min'=>0,
          'max'=>Float::INFINITY
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
      # Name from the definition
      # The name of the parameter (reference to the operation definition).
      attr_accessor :name                           # 1-1 string
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueAddress                   # 0-1 Address
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueAge                       # 0-1 Age
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueAnnotation                # 0-1 Annotation
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueAttachment                # 0-1 Attachment
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueBase64Binary              # 0-1 Base64Binary
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueBoolean                   # 0-1 Boolean
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueCanonical                 # 0-1 Canonical
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueCode                      # 0-1 Code
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueCodeableConcept           # 0-1 CodeableConcept
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueCoding                    # 0-1 Coding
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueContactDetail             # 0-1 ContactDetail
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueContactPoint              # 0-1 ContactPoint
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueContributor               # 0-1 Contributor
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueCount                     # 0-1 Count
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueDataRequirement           # 0-1 DataRequirement
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueDate                      # 0-1 Date
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueDateTime                  # 0-1 DateTime
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueDecimal                   # 0-1 Decimal
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueDistance                  # 0-1 Distance
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueDosage                    # 0-1 Dosage
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueDuration                  # 0-1 Duration
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueExpression                # 0-1 Expression
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueHumanName                 # 0-1 HumanName
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueId                        # 0-1 Id
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueIdentifier                # 0-1 Identifier
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueInstant                   # 0-1 Instant
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueInteger                   # 0-1 Integer
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueMarkdown                  # 0-1 Markdown
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueMeta                      # 0-1 Meta
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueMoney                     # 0-1 Money
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueOid                       # 0-1 Oid
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueParameterDefinition       # 0-1 ParameterDefinition
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valuePeriod                    # 0-1 Period
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valuePositiveInt               # 0-1 PositiveInt
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueQuantity                  # 0-1 Quantity
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueRange                     # 0-1 Range
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueRatio                     # 0-1 Ratio
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueReference                 # 0-1 Reference
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueRelatedArtifact           # 0-1 RelatedArtifact
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueSampledData               # 0-1 SampledData
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueSignature                 # 0-1 Signature
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueString                    # 0-1 String
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueTime                      # 0-1 Time
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueTiming                    # 0-1 Timing
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueTriggerDefinition         # 0-1 TriggerDefinition
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueUnsignedInt               # 0-1 UnsignedInt
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueUri                       # 0-1 Uri
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueUrl                       # 0-1 Url
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueUsageContext              # 0-1 UsageContext
      ##
      # If parameter is a data type
      # If the parameter is a data type.
      attr_accessor :valueUuid                      # 0-1 Uuid
      ##
      # If parameter is a whole resource
      # If the parameter is a whole resource.
      # When resolving references in resources, the operation definition may specify how references may be resolved between parameters. If a reference cannot be resolved between the parameters, the application should fall back to it's general resource resolution methods.
      attr_accessor :resource                       # 0-1 Resource
      ##
      # Named part of a multi-part parameter
      # A named part of a multi-part parameter.
      # Only one level of nested parameters is allowed.
      attr_accessor :part                           # 0-* [ Parameters::Parameter ]
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
    # Operation Parameter
    # A parameter passed to or received from the operation.
    attr_accessor :parameter                      # 0-* [ Parameters::Parameter ]

    def resourceType
      'Parameters'
    end
  end
end
