module FHIR

  ##
  # Base StructureDefinition for Extension Type: Optional Extension Element - found in all resources.
  # The ability to add extensions in a structured way is what keeps FHIR resources simple.
  class Extension < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    MULTIPLE_TYPES = {
      'value[x]' => ['Address', 'Age', 'Annotation', 'Attachment', 'base64Binary', 'boolean', 'canonical', 'code', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'date', 'dateTime', 'decimal', 'Distance', 'Dosage', 'Duration', 'Expression', 'HumanName', 'id', 'Identifier', 'instant', 'integer', 'markdown', 'Meta', 'Money', 'oid', 'ParameterDefinition', 'Period', 'positiveInt', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'string', 'time', 'Timing', 'TriggerDefinition', 'unsignedInt', 'uri', 'url', 'UsageContext', 'uuid']
    }
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'id',
        'path'=>'Extension.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Extension.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # identifies the meaning of the extension
      # Source of the definition for the extension code - a logical name or a URL.
      # The definition may point directly to a computable or human-readable definition of the extensibility codes, or it may be a logical URI as declared in some other specification. The definition SHALL be a URI for the Structure Definition defining the extension.
      'url' => {
        'type'=>'uri',
        'path'=>'Extension.url',
        'min'=>1,
        'max'=>1
      },
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueAddress' => {
        'type'=>'Address',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueAge' => {
        'type'=>'Age',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueAnnotation' => {
        'type'=>'Annotation',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueAttachment' => {
        'type'=>'Attachment',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueBase64Binary' => {
        'type'=>'Base64Binary',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueBoolean' => {
        'type'=>'Boolean',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueCanonical' => {
        'type'=>'Canonical',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueCode' => {
        'type'=>'Code',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueCoding' => {
        'type'=>'Coding',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueContactDetail' => {
        'type'=>'ContactDetail',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueContactPoint' => {
        'type'=>'ContactPoint',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueContributor' => {
        'type'=>'Contributor',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueCount' => {
        'type'=>'Count',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueDataRequirement' => {
        'type'=>'DataRequirement',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueDate' => {
        'type'=>'Date',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueDateTime' => {
        'type'=>'DateTime',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueDecimal' => {
        'type'=>'Decimal',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueDistance' => {
        'type'=>'Distance',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueDosage' => {
        'type'=>'Dosage',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueDuration' => {
        'type'=>'Duration',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueExpression' => {
        'type'=>'Expression',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueHumanName' => {
        'type'=>'HumanName',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueId' => {
        'type'=>'Id',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueIdentifier' => {
        'type'=>'Identifier',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueInstant' => {
        'type'=>'Instant',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueInteger' => {
        'type'=>'Integer',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueMarkdown' => {
        'type'=>'Markdown',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueMeta' => {
        'type'=>'Meta',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueMoney' => {
        'type'=>'Money',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueOid' => {
        'type'=>'Oid',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueParameterDefinition' => {
        'type'=>'ParameterDefinition',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valuePeriod' => {
        'type'=>'Period',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valuePositiveInt' => {
        'type'=>'PositiveInt',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueQuantity' => {
        'type'=>'Quantity',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueRange' => {
        'type'=>'Range',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueRatio' => {
        'type'=>'Ratio',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueReference' => {
        'type'=>'Reference',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueRelatedArtifact' => {
        'type'=>'RelatedArtifact',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueSampledData' => {
        'type'=>'SampledData',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueSignature' => {
        'type'=>'Signature',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueString' => {
        'type'=>'String',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueTime' => {
        'type'=>'Time',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueTiming' => {
        'type'=>'Timing',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueTriggerDefinition' => {
        'type'=>'TriggerDefinition',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueUnsignedInt' => {
        'type'=>'UnsignedInt',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueUri' => {
        'type'=>'Uri',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueUrl' => {
        'type'=>'Url',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueUsageContext' => {
        'type'=>'UsageContext',
        'path'=>'Extension.value[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
      'valueUuid' => {
        'type'=>'Uuid',
        'path'=>'Extension.value[x]',
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
    # identifies the meaning of the extension
    # Source of the definition for the extension code - a logical name or a URL.
    # The definition may point directly to a computable or human-readable definition of the extensibility codes, or it may be a logical URI as declared in some other specification. The definition SHALL be a URI for the Structure Definition defining the extension.
    attr_accessor :url                            # 1-1 uri
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueAddress                   # 0-1 Address
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueAge                       # 0-1 Age
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueAnnotation                # 0-1 Annotation
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueAttachment                # 0-1 Attachment
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueBase64Binary              # 0-1 Base64Binary
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueBoolean                   # 0-1 Boolean
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueCanonical                 # 0-1 Canonical
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueCode                      # 0-1 Code
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueCodeableConcept           # 0-1 CodeableConcept
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueCoding                    # 0-1 Coding
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueContactDetail             # 0-1 ContactDetail
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueContactPoint              # 0-1 ContactPoint
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueContributor               # 0-1 Contributor
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueCount                     # 0-1 Count
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueDataRequirement           # 0-1 DataRequirement
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueDate                      # 0-1 Date
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueDateTime                  # 0-1 DateTime
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueDecimal                   # 0-1 Decimal
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueDistance                  # 0-1 Distance
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueDosage                    # 0-1 Dosage
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueDuration                  # 0-1 Duration
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueExpression                # 0-1 Expression
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueHumanName                 # 0-1 HumanName
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueId                        # 0-1 Id
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueIdentifier                # 0-1 Identifier
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueInstant                   # 0-1 Instant
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueInteger                   # 0-1 Integer
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueMarkdown                  # 0-1 Markdown
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueMeta                      # 0-1 Meta
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueMoney                     # 0-1 Money
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueOid                       # 0-1 Oid
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueParameterDefinition       # 0-1 ParameterDefinition
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valuePeriod                    # 0-1 Period
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valuePositiveInt               # 0-1 PositiveInt
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueQuantity                  # 0-1 Quantity
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueRange                     # 0-1 Range
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueRatio                     # 0-1 Ratio
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueReference                 # 0-1 Reference
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueRelatedArtifact           # 0-1 RelatedArtifact
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueSampledData               # 0-1 SampledData
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueSignature                 # 0-1 Signature
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueString                    # 0-1 String
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueTime                      # 0-1 Time
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueTiming                    # 0-1 Timing
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueTriggerDefinition         # 0-1 TriggerDefinition
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueUnsignedInt               # 0-1 UnsignedInt
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueUri                       # 0-1 Uri
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueUrl                       # 0-1 Url
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueUsageContext              # 0-1 UsageContext
    ##
    # Value of extension - must be one of a constrained set of the data types (see [Extensibility](extensibility.html) for a list).
    attr_accessor :valueUuid                      # 0-1 Uuid
  end
end
