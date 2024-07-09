module FHIR

  ##
  # Set of definitional characteristics for a kind of observation or measurement produced or consumed by an orderable health care service.
  # In a catalog of health-related services that use or produce observations and measurements, this resource describes the expected characteristics of these observation / measurements.
  class ObservationDefinition < FHIR::Model
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
        'path'=>'ObservationDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ObservationDefinition.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ObservationDefinition.implicitRules',
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
        'path'=>'ObservationDefinition.language',
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
        'path'=>'ObservationDefinition.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ObservationDefinition.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ObservationDefinition.extension',
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
        'path'=>'ObservationDefinition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Category of observation
      # A code that classifies the general type of observation.
      # This element allows various categorization schemes based on the owner’s definition of the category and effectively multiple categories can be used for one instance of ObservationDefinition. The level of granularity is defined by the category concepts in the value set.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/observation-category'=>[ 'social-history', 'vital-signs', 'imaging', 'laboratory', 'procedure', 'survey', 'exam', 'therapy', 'activity' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ObservationDefinition.category',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/observation-category'}
      },
      ##
      # Type of observation (code / type)
      # Describes what will be observed. Sometimes this is called the observation "name".
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'ObservationDefinition.code',
        'min'=>1,
        'max'=>1
      },
      ##
      # Business identifier for this ObservationDefinition instance
      # A unique identifier assigned to this ObservationDefinition artifact.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ObservationDefinition.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Quantity | CodeableConcept | string | boolean | integer | Range | Ratio | SampledData | time | dateTime | Period
      # The data types allowed for the value element of the instance observations conforming to this ObservationDefinition.
      'permittedDataType' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/permitted-data-type'=>[ 'Quantity', 'CodeableConcept', 'string', 'boolean', 'integer', 'Range', 'Ratio', 'SampledData', 'time', 'dateTime', 'Period' ]
        },
        'type'=>'code',
        'path'=>'ObservationDefinition.permittedDataType',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/permitted-data-type'}
      },
      ##
      # Multiple results allowed for observations conforming to this ObservationDefinition.
      # An example of observation allowing multiple results is "bacteria identified by culture". Conversely, the measurement of a potassium level allows a single result.
      'multipleResultsAllowed' => {
        'type'=>'boolean',
        'path'=>'ObservationDefinition.multipleResultsAllowed',
        'min'=>0,
        'max'=>1
      },
      ##
      # Method used to produce the observation
      # The method or technique used to perform the observation.
      # Only used if not implicit in observation code.
      'method' => {
        'local_name'=>'local_method'
        'type'=>'CodeableConcept',
        'path'=>'ObservationDefinition.method',
        'min'=>0,
        'max'=>1
      },
      ##
      # Preferred report name
      # The preferred name to be used when reporting the results of observations conforming to this ObservationDefinition.
      'preferredReportName' => {
        'type'=>'string',
        'path'=>'ObservationDefinition.preferredReportName',
        'min'=>0,
        'max'=>1
      },
      ##
      # Characteristics of quantitative results
      # Characteristics for quantitative results of this observation.
      'quantitativeDetails' => {
        'type'=>'ObservationDefinition::QuantitativeDetails',
        'path'=>'ObservationDefinition.quantitativeDetails',
        'min'=>0,
        'max'=>1
      },
      ##
      # Qualified range for continuous and ordinal observation results
      # Multiple  ranges of results qualified by different contexts for ordinal or continuous observations conforming to this ObservationDefinition.
      'qualifiedInterval' => {
        'type'=>'ObservationDefinition::QualifiedInterval',
        'path'=>'ObservationDefinition.qualifiedInterval',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Value set of valid coded values for the observations conforming to this ObservationDefinition
      # The set of valid coded results for the observations  conforming to this ObservationDefinition.
      'validCodedValueSet' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
        'type'=>'Reference',
        'path'=>'ObservationDefinition.validCodedValueSet',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value set of normal coded values for the observations conforming to this ObservationDefinition
      # The set of normal coded results for the observations conforming to this ObservationDefinition.
      'normalCodedValueSet' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
        'type'=>'Reference',
        'path'=>'ObservationDefinition.normalCodedValueSet',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value set of abnormal coded values for the observations conforming to this ObservationDefinition
      # The set of abnormal coded results for the observation conforming to this ObservationDefinition.
      'abnormalCodedValueSet' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
        'type'=>'Reference',
        'path'=>'ObservationDefinition.abnormalCodedValueSet',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value set of critical coded values for the observations conforming to this ObservationDefinition
      # The set of critical coded results for the observation conforming to this ObservationDefinition.
      'criticalCodedValueSet' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
        'type'=>'Reference',
        'path'=>'ObservationDefinition.criticalCodedValueSet',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Characteristics of quantitative results
    # Characteristics for quantitative results of this observation.
    class QuantitativeDetails < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'QuantitativeDetails.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'QuantitativeDetails.extension',
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
          'path'=>'QuantitativeDetails.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Customary unit for quantitative results
        # Customary unit used to report quantitative results of observations conforming to this ObservationDefinition.
        'customaryUnit' => {
          'type'=>'CodeableConcept',
          'path'=>'QuantitativeDetails.customaryUnit',
          'min'=>0,
          'max'=>1
        },
        ##
        # SI unit for quantitative results
        # SI unit used to report quantitative results of observations conforming to this ObservationDefinition.
        'unit' => {
          'type'=>'CodeableConcept',
          'path'=>'QuantitativeDetails.unit',
          'min'=>0,
          'max'=>1
        },
        ##
        # SI to Customary unit conversion factor
        # Factor for converting value expressed with SI unit to value expressed with customary unit.
        'conversionFactor' => {
          'type'=>'decimal',
          'path'=>'QuantitativeDetails.conversionFactor',
          'min'=>0,
          'max'=>1
        },
        ##
        # Decimal precision of observation quantitative results
        # Number of digits after decimal separator when the results of such observations are of type Quantity.
        'decimalPrecision' => {
          'type'=>'integer',
          'path'=>'QuantitativeDetails.decimalPrecision',
          'min'=>0,
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
      # Customary unit for quantitative results
      # Customary unit used to report quantitative results of observations conforming to this ObservationDefinition.
      attr_accessor :customaryUnit                  # 0-1 CodeableConcept
      ##
      # SI unit for quantitative results
      # SI unit used to report quantitative results of observations conforming to this ObservationDefinition.
      attr_accessor :unit                           # 0-1 CodeableConcept
      ##
      # SI to Customary unit conversion factor
      # Factor for converting value expressed with SI unit to value expressed with customary unit.
      attr_accessor :conversionFactor               # 0-1 decimal
      ##
      # Decimal precision of observation quantitative results
      # Number of digits after decimal separator when the results of such observations are of type Quantity.
      attr_accessor :decimalPrecision               # 0-1 integer
    end

    ##
    # Qualified range for continuous and ordinal observation results
    # Multiple  ranges of results qualified by different contexts for ordinal or continuous observations conforming to this ObservationDefinition.
    class QualifiedInterval < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'QualifiedInterval.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'QualifiedInterval.extension',
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
          'path'=>'QualifiedInterval.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # reference | critical | absolute
        # The category of interval of values for continuous or ordinal observations conforming to this ObservationDefinition.
        'category' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/observation-range-category'=>[ 'reference', 'critical', 'absolute' ]
          },
          'type'=>'code',
          'path'=>'QualifiedInterval.category',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/observation-range-category'}
        },
        ##
        # The interval itself, for continuous or ordinal observations
        # The low and high values determining the interval. There may be only one of the two.
        'range' => {
          'type'=>'Range',
          'path'=>'QualifiedInterval.range',
          'min'=>0,
          'max'=>1
        },
        ##
        # Range context qualifier
        # Codes to indicate the health context the range applies to. For example, the normal or therapeutic range.
        'context' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/referencerange-meaning'=>[ 'type', 'normal', 'recommended', 'treatment', 'therapeutic', 'pre', 'post', 'endocrine', 'pre-puberty', 'follicular', 'midcycle', 'luteal', 'postmenopausal' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'QualifiedInterval.context',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/referencerange-meaning'}
        },
        ##
        # Targetted population of the range
        # Codes to indicate the target population this reference range applies to.
        # If this element is not present then the global population is assumed.
        'appliesTo' => {
          'type'=>'CodeableConcept',
          'path'=>'QualifiedInterval.appliesTo',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # male | female | other | unknown
        # Sex of the population the range applies to.
        'gender' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/administrative-gender'=>[ 'male', 'female', 'other', 'unknown' ]
          },
          'type'=>'code',
          'path'=>'QualifiedInterval.gender',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/administrative-gender'}
        },
        ##
        # Applicable age range, if relevant
        # The age at which this reference range is applicable. This is a neonatal age (e.g. number of weeks at term) if the meaning says so.
        # Some analytes vary greatly over age.
        'age' => {
          'type'=>'Range',
          'path'=>'QualifiedInterval.age',
          'min'=>0,
          'max'=>1
        },
        ##
        # Applicable gestational age range, if relevant
        # The gestational age to which this reference range is applicable, in the context of pregnancy.
        'gestationalAge' => {
          'type'=>'Range',
          'path'=>'QualifiedInterval.gestationalAge',
          'min'=>0,
          'max'=>1
        },
        ##
        # Condition associated with the reference range
        # Text based condition for which the reference range is valid.
        'condition' => {
          'type'=>'string',
          'path'=>'QualifiedInterval.condition',
          'min'=>0,
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
      # reference | critical | absolute
      # The category of interval of values for continuous or ordinal observations conforming to this ObservationDefinition.
      attr_accessor :category                       # 0-1 code
      ##
      # The interval itself, for continuous or ordinal observations
      # The low and high values determining the interval. There may be only one of the two.
      attr_accessor :range                          # 0-1 Range
      ##
      # Range context qualifier
      # Codes to indicate the health context the range applies to. For example, the normal or therapeutic range.
      attr_accessor :context                        # 0-1 CodeableConcept
      ##
      # Targetted population of the range
      # Codes to indicate the target population this reference range applies to.
      # If this element is not present then the global population is assumed.
      attr_accessor :appliesTo                      # 0-* [ CodeableConcept ]
      ##
      # male | female | other | unknown
      # Sex of the population the range applies to.
      attr_accessor :gender                         # 0-1 code
      ##
      # Applicable age range, if relevant
      # The age at which this reference range is applicable. This is a neonatal age (e.g. number of weeks at term) if the meaning says so.
      # Some analytes vary greatly over age.
      attr_accessor :age                            # 0-1 Range
      ##
      # Applicable gestational age range, if relevant
      # The gestational age to which this reference range is applicable, in the context of pregnancy.
      attr_accessor :gestationalAge                 # 0-1 Range
      ##
      # Condition associated with the reference range
      # Text based condition for which the reference range is valid.
      attr_accessor :condition                      # 0-1 string
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
    # Category of observation
    # A code that classifies the general type of observation.
    # This element allows various categorization schemes based on the owner’s definition of the category and effectively multiple categories can be used for one instance of ObservationDefinition. The level of granularity is defined by the category concepts in the value set.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # Type of observation (code / type)
    # Describes what will be observed. Sometimes this is called the observation "name".
    attr_accessor :code                           # 1-1 CodeableConcept
    ##
    # Business identifier for this ObservationDefinition instance
    # A unique identifier assigned to this ObservationDefinition artifact.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Quantity | CodeableConcept | string | boolean | integer | Range | Ratio | SampledData | time | dateTime | Period
    # The data types allowed for the value element of the instance observations conforming to this ObservationDefinition.
    attr_accessor :permittedDataType              # 0-* [ code ]
    ##
    # Multiple results allowed for observations conforming to this ObservationDefinition.
    # An example of observation allowing multiple results is "bacteria identified by culture". Conversely, the measurement of a potassium level allows a single result.
    attr_accessor :multipleResultsAllowed         # 0-1 boolean
    ##
    # Method used to produce the observation
    # The method or technique used to perform the observation.
    # Only used if not implicit in observation code.
    attr_accessor :local_method                   # 0-1 CodeableConcept
    ##
    # Preferred report name
    # The preferred name to be used when reporting the results of observations conforming to this ObservationDefinition.
    attr_accessor :preferredReportName            # 0-1 string
    ##
    # Characteristics of quantitative results
    # Characteristics for quantitative results of this observation.
    attr_accessor :quantitativeDetails            # 0-1 ObservationDefinition::QuantitativeDetails
    ##
    # Qualified range for continuous and ordinal observation results
    # Multiple  ranges of results qualified by different contexts for ordinal or continuous observations conforming to this ObservationDefinition.
    attr_accessor :qualifiedInterval              # 0-* [ ObservationDefinition::QualifiedInterval ]
    ##
    # Value set of valid coded values for the observations conforming to this ObservationDefinition
    # The set of valid coded results for the observations  conforming to this ObservationDefinition.
    attr_accessor :validCodedValueSet             # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/ValueSet)
    ##
    # Value set of normal coded values for the observations conforming to this ObservationDefinition
    # The set of normal coded results for the observations conforming to this ObservationDefinition.
    attr_accessor :normalCodedValueSet            # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/ValueSet)
    ##
    # Value set of abnormal coded values for the observations conforming to this ObservationDefinition
    # The set of abnormal coded results for the observation conforming to this ObservationDefinition.
    attr_accessor :abnormalCodedValueSet          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/ValueSet)
    ##
    # Value set of critical coded values for the observations conforming to this ObservationDefinition
    # The set of critical coded results for the observation conforming to this ObservationDefinition.
    attr_accessor :criticalCodedValueSet          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/ValueSet)

    def resourceType
      'ObservationDefinition'
    end
  end
end
