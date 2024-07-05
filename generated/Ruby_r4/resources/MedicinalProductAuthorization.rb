module FHIR

  ##
  # The regulatory authorization of a medicinal product.
  class MedicinalProductAuthorization < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['country', 'holder', 'identifier', 'status', 'subject']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'MedicinalProductAuthorization.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'MedicinalProductAuthorization.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'MedicinalProductAuthorization.implicitRules',
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
        'path'=>'MedicinalProductAuthorization.language',
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
        'path'=>'MedicinalProductAuthorization.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'MedicinalProductAuthorization.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'MedicinalProductAuthorization.extension',
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
        'path'=>'MedicinalProductAuthorization.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business identifier for the marketing authorization, as assigned by a regulator.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'MedicinalProductAuthorization.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The medicinal product that is being authorized.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicinalProduct', 'http://hl7.org/fhir/StructureDefinition/MedicinalProductPackaged'],
        'type'=>'Reference',
        'path'=>'MedicinalProductAuthorization.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # The country in which the marketing authorization has been granted.
      'country' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicinalProductAuthorization.country',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Jurisdiction within a country.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicinalProductAuthorization.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The status of the marketing authorization.
      'status' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicinalProductAuthorization.status',
        'min'=>0,
        'max'=>1
      },
      ##
      # The date at which the given status has become applicable.
      'statusDate' => {
        'type'=>'dateTime',
        'path'=>'MedicinalProductAuthorization.statusDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # The date when a suspended the marketing or the marketing authorization of the product is anticipated to be restored.
      'restoreDate' => {
        'type'=>'dateTime',
        'path'=>'MedicinalProductAuthorization.restoreDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # The beginning of the time period in which the marketing authorization is in the specific status shall be specified A complete date consisting of day, month and year shall be specified using the ISO 8601 date format.
      'validityPeriod' => {
        'type'=>'Period',
        'path'=>'MedicinalProductAuthorization.validityPeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # A period of time after authorization before generic product applicatiosn can be submitted.
      'dataExclusivityPeriod' => {
        'type'=>'Period',
        'path'=>'MedicinalProductAuthorization.dataExclusivityPeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # The date when the first authorization was granted by a Medicines Regulatory Agency.
      'dateOfFirstAuthorization' => {
        'type'=>'dateTime',
        'path'=>'MedicinalProductAuthorization.dateOfFirstAuthorization',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date of first marketing authorization for a company's new medicinal product in any country in the World.
      'internationalBirthDate' => {
        'type'=>'dateTime',
        'path'=>'MedicinalProductAuthorization.internationalBirthDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # The legal framework against which this authorization is granted.
      'legalBasis' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicinalProductAuthorization.legalBasis',
        'min'=>0,
        'max'=>1
      },
      ##
      # Authorization in areas within a country.
      'jurisdictionalAuthorization' => {
        'type'=>'MedicinalProductAuthorization::JurisdictionalAuthorization',
        'path'=>'MedicinalProductAuthorization.jurisdictionalAuthorization',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Marketing Authorization Holder.
      'holder' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'MedicinalProductAuthorization.holder',
        'min'=>0,
        'max'=>1
      },
      ##
      # Medicines Regulatory Agency.
      'regulator' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'MedicinalProductAuthorization.regulator',
        'min'=>0,
        'max'=>1
      },
      ##
      # The regulatory procedure for granting or amending a marketing authorization.
      'procedure' => {
        'type'=>'MedicinalProductAuthorization::Procedure',
        'path'=>'MedicinalProductAuthorization.procedure',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Authorization in areas within a country.
    class JurisdictionalAuthorization < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'JurisdictionalAuthorization.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'JurisdictionalAuthorization.extension',
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
          'path'=>'JurisdictionalAuthorization.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The assigned number for the marketing authorization.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'JurisdictionalAuthorization.identifier',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Country of authorization.
        'country' => {
          'type'=>'CodeableConcept',
          'path'=>'JurisdictionalAuthorization.country',
          'min'=>0,
          'max'=>1
        },
        ##
        # Jurisdiction within a country.
        'jurisdiction' => {
          'type'=>'CodeableConcept',
          'path'=>'JurisdictionalAuthorization.jurisdiction',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The legal status of supply in a jurisdiction or region.
        'legalStatusOfSupply' => {
          'type'=>'CodeableConcept',
          'path'=>'JurisdictionalAuthorization.legalStatusOfSupply',
          'min'=>0,
          'max'=>1
        },
        ##
        # The start and expected end date of the authorization.
        'validityPeriod' => {
          'type'=>'Period',
          'path'=>'JurisdictionalAuthorization.validityPeriod',
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
      # The assigned number for the marketing authorization.
      attr_accessor :identifier                     # 0-* [ Identifier ]
      ##
      # Country of authorization.
      attr_accessor :country                        # 0-1 CodeableConcept
      ##
      # Jurisdiction within a country.
      attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
      ##
      # The legal status of supply in a jurisdiction or region.
      attr_accessor :legalStatusOfSupply            # 0-1 CodeableConcept
      ##
      # The start and expected end date of the authorization.
      attr_accessor :validityPeriod                 # 0-1 Period
    end

    ##
    # The regulatory procedure for granting or amending a marketing authorization.
    class Procedure < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'date[x]' => ['dateTime', 'Period']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Procedure.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Procedure.extension',
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
          'path'=>'Procedure.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Identifier for this procedure.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'Procedure.identifier',
          'min'=>0,
          'max'=>1
        },
        ##
        # Type of procedure.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Procedure.type',
          'min'=>1,
          'max'=>1
        },
        ##
        # Date of procedure.
        'dateDateTime' => {
          'type'=>'DateTime',
          'path'=>'Procedure.date[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Date of procedure.
        'datePeriod' => {
          'type'=>'Period',
          'path'=>'Procedure.date[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Applcations submitted to obtain a marketing authorization.
        'application' => {
          'type'=>'MedicinalProductAuthorization::Procedure',
          'path'=>'Procedure.application',
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
      # Identifier for this procedure.
      attr_accessor :identifier                     # 0-1 Identifier
      ##
      # Type of procedure.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Date of procedure.
      attr_accessor :dateDateTime                   # 0-1 DateTime
      ##
      # Date of procedure.
      attr_accessor :datePeriod                     # 0-1 Period
      ##
      # Applcations submitted to obtain a marketing authorization.
      attr_accessor :application                    # 0-* [ MedicinalProductAuthorization::Procedure ]
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
    # Business identifier for the marketing authorization, as assigned by a regulator.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # The medicinal product that is being authorized.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/MedicinalProduct|http://hl7.org/fhir/StructureDefinition/MedicinalProductPackaged)
    ##
    # The country in which the marketing authorization has been granted.
    attr_accessor :country                        # 0-* [ CodeableConcept ]
    ##
    # Jurisdiction within a country.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # The status of the marketing authorization.
    attr_accessor :status                         # 0-1 CodeableConcept
    ##
    # The date at which the given status has become applicable.
    attr_accessor :statusDate                     # 0-1 dateTime
    ##
    # The date when a suspended the marketing or the marketing authorization of the product is anticipated to be restored.
    attr_accessor :restoreDate                    # 0-1 dateTime
    ##
    # The beginning of the time period in which the marketing authorization is in the specific status shall be specified A complete date consisting of day, month and year shall be specified using the ISO 8601 date format.
    attr_accessor :validityPeriod                 # 0-1 Period
    ##
    # A period of time after authorization before generic product applicatiosn can be submitted.
    attr_accessor :dataExclusivityPeriod          # 0-1 Period
    ##
    # The date when the first authorization was granted by a Medicines Regulatory Agency.
    attr_accessor :dateOfFirstAuthorization       # 0-1 dateTime
    ##
    # Date of first marketing authorization for a company's new medicinal product in any country in the World.
    attr_accessor :internationalBirthDate         # 0-1 dateTime
    ##
    # The legal framework against which this authorization is granted.
    attr_accessor :legalBasis                     # 0-1 CodeableConcept
    ##
    # Authorization in areas within a country.
    attr_accessor :jurisdictionalAuthorization    # 0-* [ MedicinalProductAuthorization::JurisdictionalAuthorization ]
    ##
    # Marketing Authorization Holder.
    attr_accessor :holder                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Medicines Regulatory Agency.
    attr_accessor :regulator                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # The regulatory procedure for granting or amending a marketing authorization.
    attr_accessor :procedure                      # 0-1 MedicinalProductAuthorization::Procedure

    def resourceType
      'MedicinalProductAuthorization'
    end
  end
end
