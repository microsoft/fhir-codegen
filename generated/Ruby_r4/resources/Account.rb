module FHIR

  ##
  # A financial tool for tracking value accrued for a particular purpose.  In the healthcare field, used to track charges for a patient, cost centers, etc.
  class Account < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['identifier', 'name', 'owner', 'patient', 'period', 'status', 'subject', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Account.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Account.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Account.implicitRules',
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
        'path'=>'Account.language',
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
        'path'=>'Account.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Account.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Account.extension',
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
        'path'=>'Account.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Account number
      # Unique identifier used to reference the account.  Might or might not be intended for human use (e.g. credit card number).
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Account.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | inactive | entered-in-error | on-hold | unknown
      # Indicates whether the account is presently used/usable or not.
      # This element is labeled as a modifier because the status contains the codes inactive and entered-in-error that mark the Account as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/account-status'=>[ 'active', 'inactive', 'entered-in-error', 'on-hold', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Account.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/account-status'}
      },
      ##
      # E.g. patient, expense, depreciation
      # Categorizes the account for reporting and searching purposes.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'ACCTRECEIVABLE', 'CASH', 'CC', 'AE', 'DN', 'DV', 'MC', 'V', 'PBILLACCT' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Account.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/account-type'}
      },
      ##
      # Human-readable label
      # Name used for the account when displaying it to humans in reports, etc.
      'name' => {
        'type'=>'string',
        'path'=>'Account.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # The entity that caused the expenses
      # Identifies the entity which incurs the expenses. While the immediate recipients of services or goods might be entities related to the subject, the expenses were ultimately incurred by the subject of the Account.
      # Accounts can be applied to non-patients for tracking other non-patient related activities, such as group services (patients not tracked, and costs charged to another body), or might not be allocated.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Location', 'http://hl7.org/fhir/StructureDefinition/HealthcareService', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Account.subject',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Transaction window
      # The date range of services associated with this account.
      # It is possible for transactions to be posted outside the service period, as long as the service was provided within the defined service period.
      'servicePeriod' => {
        'type'=>'Period',
        'path'=>'Account.servicePeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # The party(s) that are responsible for covering the payment of this account, and what order should they be applied to the account.
      # Typically. this may be some form of insurance, internal charges, or self-pay.
      # 
      # Local or jurisdictional business rules may determine which coverage covers which types of billable items charged to the account, and in which order.
      # Where the order is important, a local/jurisdictional extension may be defined to specify the order for the type of charge.
      'coverage' => {
        'type'=>'Account::Coverage',
        'path'=>'Account.coverage',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Entity managing the Account
      # Indicates the service area, hospital, department, etc. with responsibility for managing the Account.
      'owner' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Account.owner',
        'min'=>0,
        'max'=>1
      },
      ##
      # Explanation of purpose/use
      # Provides additional information about what the account tracks and how it is used.
      'description' => {
        'type'=>'string',
        'path'=>'Account.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The parties ultimately responsible for balancing the Account
      # The parties responsible for balancing the account if other payment options fall short.
      'guarantor' => {
        'type'=>'Account::Guarantor',
        'path'=>'Account.guarantor',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Reference to a parent Account.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Account'],
        'type'=>'Reference',
        'path'=>'Account.partOf',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # The party(s) that are responsible for covering the payment of this account, and what order should they be applied to the account.
    # Typically. this may be some form of insurance, internal charges, or self-pay.
    # 
    # Local or jurisdictional business rules may determine which coverage covers which types of billable items charged to the account, and in which order.
    # Where the order is important, a local/jurisdictional extension may be defined to specify the order for the type of charge.
    class Coverage < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Coverage.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Coverage.extension',
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
          'path'=>'Coverage.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The party(s), such as insurances, that may contribute to the payment of this account
        # The party(s) that contribute to payment (or part of) of the charges applied to this account (including self-pay).
        # 
        # A coverage may only be responsible for specific types of charges, and the sequence of the coverages in the account could be important when processing billing.
        'coverage' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Coverage'],
          'type'=>'Reference',
          'path'=>'Coverage.coverage',
          'min'=>1,
          'max'=>1
        },
        ##
        # The priority of the coverage in the context of this account.
        # It is common in some jurisdictions for there to be multiple coverages allocated to an account, and a sequence is required to order the settling of the account (often with insurance claiming).
        'priority' => {
          'type'=>'positiveInt',
          'path'=>'Coverage.priority',
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
      # The party(s), such as insurances, that may contribute to the payment of this account
      # The party(s) that contribute to payment (or part of) of the charges applied to this account (including self-pay).
      # 
      # A coverage may only be responsible for specific types of charges, and the sequence of the coverages in the account could be important when processing billing.
      attr_accessor :coverage                       # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Coverage)
      ##
      # The priority of the coverage in the context of this account.
      # It is common in some jurisdictions for there to be multiple coverages allocated to an account, and a sequence is required to order the settling of the account (often with insurance claiming).
      attr_accessor :priority                       # 0-1 positiveInt
    end

    ##
    # The parties ultimately responsible for balancing the Account
    # The parties responsible for balancing the account if other payment options fall short.
    class Guarantor < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Guarantor.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Guarantor.extension',
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
          'path'=>'Guarantor.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Responsible entity
        # The entity who is responsible.
        'party' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Guarantor.party',
          'min'=>1,
          'max'=>1
        },
        ##
        # Credit or other hold applied
        # A guarantor may be placed on credit hold or otherwise have their role temporarily suspended.
        'onHold' => {
          'type'=>'boolean',
          'path'=>'Guarantor.onHold',
          'min'=>0,
          'max'=>1
        },
        ##
        # Guarantee account during
        # The timeframe during which the guarantor accepts responsibility for the account.
        'period' => {
          'type'=>'Period',
          'path'=>'Guarantor.period',
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
      # Responsible entity
      # The entity who is responsible.
      attr_accessor :party                          # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # Credit or other hold applied
      # A guarantor may be placed on credit hold or otherwise have their role temporarily suspended.
      attr_accessor :onHold                         # 0-1 boolean
      ##
      # Guarantee account during
      # The timeframe during which the guarantor accepts responsibility for the account.
      attr_accessor :period                         # 0-1 Period
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
    # Account number
    # Unique identifier used to reference the account.  Might or might not be intended for human use (e.g. credit card number).
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | inactive | entered-in-error | on-hold | unknown
    # Indicates whether the account is presently used/usable or not.
    # This element is labeled as a modifier because the status contains the codes inactive and entered-in-error that mark the Account as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # E.g. patient, expense, depreciation
    # Categorizes the account for reporting and searching purposes.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # Human-readable label
    # Name used for the account when displaying it to humans in reports, etc.
    attr_accessor :name                           # 0-1 string
    ##
    # The entity that caused the expenses
    # Identifies the entity which incurs the expenses. While the immediate recipients of services or goods might be entities related to the subject, the expenses were ultimately incurred by the subject of the Account.
    # Accounts can be applied to non-patients for tracking other non-patient related activities, such as group services (patients not tracked, and costs charged to another body), or might not be allocated.
    attr_accessor :subject                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Location|http://hl7.org/fhir/StructureDefinition/HealthcareService|http://hl7.org/fhir/StructureDefinition/Organization) ]
    ##
    # Transaction window
    # The date range of services associated with this account.
    # It is possible for transactions to be posted outside the service period, as long as the service was provided within the defined service period.
    attr_accessor :servicePeriod                  # 0-1 Period
    ##
    # The party(s) that are responsible for covering the payment of this account, and what order should they be applied to the account.
    # Typically. this may be some form of insurance, internal charges, or self-pay.
    # 
    # Local or jurisdictional business rules may determine which coverage covers which types of billable items charged to the account, and in which order.
    # Where the order is important, a local/jurisdictional extension may be defined to specify the order for the type of charge.
    attr_accessor :coverage                       # 0-* [ Account::Coverage ]
    ##
    # Entity managing the Account
    # Indicates the service area, hospital, department, etc. with responsibility for managing the Account.
    attr_accessor :owner                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Explanation of purpose/use
    # Provides additional information about what the account tracks and how it is used.
    attr_accessor :description                    # 0-1 string
    ##
    # The parties ultimately responsible for balancing the Account
    # The parties responsible for balancing the account if other payment options fall short.
    attr_accessor :guarantor                      # 0-* [ Account::Guarantor ]
    ##
    # Reference to a parent Account.
    attr_accessor :partOf                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Account)

    def resourceType
      'Account'
    end
  end
end
