module FHIR

  ##
  # Details of a Health Insurance product/plan provided by an organization.
  class InsurancePlan < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['address-city', 'address-country', 'address-postalcode', 'address-state', 'address-use', 'address', 'administered-by', 'endpoint', 'identifier', 'name', 'owned-by', 'phonetic', 'status', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'InsurancePlan.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'InsurancePlan.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'InsurancePlan.implicitRules',
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
        'path'=>'InsurancePlan.language',
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
        'path'=>'InsurancePlan.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'InsurancePlan.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'InsurancePlan.extension',
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
        'path'=>'InsurancePlan.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for Product
      # Business identifiers assigned to this health insurance product which remain constant as the resource is updated and propagates from server to server.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'InsurancePlan.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # draft | active | retired | unknown
      # The current state of the health insurance product.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'InsurancePlan.status',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # Kind of product
      # The kind of health insurance product.
      'type' => {
        'type'=>'CodeableConcept',
        'path'=>'InsurancePlan.type',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Official name of the health insurance product as designated by the owner.
      # If the name of the product/plan changes, consider putting the old name in the alias column so that it can still be located through searches.
      'name' => {
        'type'=>'string',
        'path'=>'InsurancePlan.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Alternate names
      # A list of alternate names that the product is known as, or was known as in the past.
      # There are no dates associated with the alias/historic names, as this is not intended to track when names were used, but to assist in searching so that older names can still result in identifying the product/plan.
      'alias' => {
        'local_name'=>'local_alias'
        'type'=>'string',
        'path'=>'InsurancePlan.alias',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # When the product is available
      # The period of time that the health insurance product is available.
      'period' => {
        'type'=>'Period',
        'path'=>'InsurancePlan.period',
        'min'=>0,
        'max'=>1
      },
      ##
      # Plan issuer
      # The entity that is providing  the health insurance product and underwriting the risk.  This is typically an insurance carriers, other third-party payers, or health plan sponsors comonly referred to as 'payers'.
      'ownedBy' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'InsurancePlan.ownedBy',
        'min'=>0,
        'max'=>1
      },
      ##
      # Product administrator
      # An organization which administer other services such as underwriting, customer service and/or claims processing on behalf of the health insurance product owner.
      'administeredBy' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'InsurancePlan.administeredBy',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where product applies
      # The geographic region in which a health insurance product's benefits apply.
      'coverageArea' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'InsurancePlan.coverageArea',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Contact for the product
      # The contact for the health insurance product for a certain purpose.
      # Where multiple contacts for the same purpose are provided there is a standard extension that can be used to determine which one is the preferred contact to use.
      'contact' => {
        'type'=>'InsurancePlan::Contact',
        'path'=>'InsurancePlan.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Technical endpoint
      # The technical endpoints providing access to services operated for the health insurance product.
      'endpoint' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Endpoint'],
        'type'=>'Reference',
        'path'=>'InsurancePlan.endpoint',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What networks are Included
      # Reference to the network included in the health insurance product.
      # Networks are represented as a hierarchy of organization resources.
      'network' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'InsurancePlan.network',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Coverage details
      # Details about the coverage offered by the insurance product.
      'coverage' => {
        'type'=>'InsurancePlan::Coverage',
        'path'=>'InsurancePlan.coverage',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Plan details
      # Details about an insurance plan.
      'plan' => {
        'type'=>'InsurancePlan::Plan',
        'path'=>'InsurancePlan.plan',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Contact for the product
    # The contact for the health insurance product for a certain purpose.
    # Where multiple contacts for the same purpose are provided there is a standard extension that can be used to determine which one is the preferred contact to use.
    class Contact < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Contact.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Contact.extension',
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
          'path'=>'Contact.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The type of contact
        # Indicates a purpose for which the contact can be reached.
        'purpose' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/contactentity-type'=>[ 'BILL', 'ADMIN', 'HR', 'PAYOR', 'PATINF', 'PRESS' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Contact.purpose',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/contactentity-type'}
        },
        ##
        # A name associated with the contact.
        'name' => {
          'type'=>'HumanName',
          'path'=>'Contact.name',
          'min'=>0,
          'max'=>1
        },
        ##
        # Contact details (telephone, email, etc.)  for a contact
        # A contact detail (e.g. a telephone number or an email address) by which the party may be contacted.
        'telecom' => {
          'type'=>'ContactPoint',
          'path'=>'Contact.telecom',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Visiting or postal addresses for the contact.
        'address' => {
          'type'=>'Address',
          'path'=>'Contact.address',
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
      # The type of contact
      # Indicates a purpose for which the contact can be reached.
      attr_accessor :purpose                        # 0-1 CodeableConcept
      ##
      # A name associated with the contact.
      attr_accessor :name                           # 0-1 HumanName
      ##
      # Contact details (telephone, email, etc.)  for a contact
      # A contact detail (e.g. a telephone number or an email address) by which the party may be contacted.
      attr_accessor :telecom                        # 0-* [ ContactPoint ]
      ##
      # Visiting or postal addresses for the contact.
      attr_accessor :address                        # 0-1 Address
    end

    ##
    # Coverage details
    # Details about the coverage offered by the insurance product.
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
        # Type of coverage  (Medical; Dental; Mental Health; Substance Abuse; Vision; Drug; Short Term; Long Term Care; Hospice; Home Health).
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Coverage.type',
          'min'=>1,
          'max'=>1
        },
        ##
        # What networks provide coverage
        # Reference to the network that providing the type of coverage.
        # Networks are represented as a hierarchy of organization resources.
        'network' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Coverage.network',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # List of benefits
        # Specific benefits under this type of coverage.
        'benefit' => {
          'type'=>'InsurancePlan::Coverage::Benefit',
          'path'=>'Coverage.benefit',
          'min'=>1,
          'max'=>Float::INFINITY
        }
      }

      ##
      # List of benefits
      # Specific benefits under this type of coverage.
      class Benefit < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Benefit.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Benefit.extension',
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
            'path'=>'Benefit.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Type of benefit (primary care; speciality care; inpatient; outpatient).
          'type' => {
            'type'=>'CodeableConcept',
            'path'=>'Benefit.type',
            'min'=>1,
            'max'=>1
          },
          ##
          # Referral requirements
          # The referral requirements to have access/coverage for this benefit.
          'requirement' => {
            'type'=>'string',
            'path'=>'Benefit.requirement',
            'min'=>0,
            'max'=>1
          },
          ##
          # Benefit limits
          # The specific limits on the benefit.
          'limit' => {
            'type'=>'InsurancePlan::Coverage::Benefit::Limit',
            'path'=>'Benefit.limit',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Benefit limits
        # The specific limits on the benefit.
        class Limit < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'string',
              'path'=>'Limit.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Limit.extension',
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
              'path'=>'Limit.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Maximum value allowed
            # The maximum amount of a service item a plan will pay for a covered benefit.  For examples. wellness visits, or eyeglasses.
            # May also be called “eligible expense,” “payment allowance,” or “negotiated rate.”.
            'value' => {
              'type'=>'Quantity',
              'path'=>'Limit.value',
              'min'=>0,
              'max'=>1
            },
            ##
            # Benefit limit details
            # The specific limit on the benefit.
            # Use `CodeableConcept.text` element if the data is free (uncoded) text.
            'code' => {
              'type'=>'CodeableConcept',
              'path'=>'Limit.code',
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
          # Maximum value allowed
          # The maximum amount of a service item a plan will pay for a covered benefit.  For examples. wellness visits, or eyeglasses.
          # May also be called “eligible expense,” “payment allowance,” or “negotiated rate.”.
          attr_accessor :value                          # 0-1 Quantity
          ##
          # Benefit limit details
          # The specific limit on the benefit.
          # Use `CodeableConcept.text` element if the data is free (uncoded) text.
          attr_accessor :code                           # 0-1 CodeableConcept
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
        # Type of benefit (primary care; speciality care; inpatient; outpatient).
        attr_accessor :type                           # 1-1 CodeableConcept
        ##
        # Referral requirements
        # The referral requirements to have access/coverage for this benefit.
        attr_accessor :requirement                    # 0-1 string
        ##
        # Benefit limits
        # The specific limits on the benefit.
        attr_accessor :limit                          # 0-* [ InsurancePlan::Coverage::Benefit::Limit ]
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
      # Type of coverage  (Medical; Dental; Mental Health; Substance Abuse; Vision; Drug; Short Term; Long Term Care; Hospice; Home Health).
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # What networks provide coverage
      # Reference to the network that providing the type of coverage.
      # Networks are represented as a hierarchy of organization resources.
      attr_accessor :network                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization) ]
      ##
      # List of benefits
      # Specific benefits under this type of coverage.
      attr_accessor :benefit                        # 1-* [ InsurancePlan::Coverage::Benefit ]
    end

    ##
    # Plan details
    # Details about an insurance plan.
    class Plan < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Plan.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Plan.extension',
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
          'path'=>'Plan.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Business Identifier for Product
        # Business identifiers assigned to this health insurance plan which remain constant as the resource is updated and propagates from server to server.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'Plan.identifier',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of plan. For example, "Platinum" or "High Deductable".
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Plan.type',
          'min'=>0,
          'max'=>1
        },
        ##
        # Where product applies
        # The geographic region in which a health insurance plan's benefits apply.
        'coverageArea' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
          'type'=>'Reference',
          'path'=>'Plan.coverageArea',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # What networks provide coverage
        # Reference to the network that providing the type of coverage.
        # Networks are represented as a hierarchy of organization resources.
        'network' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Plan.network',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Overall costs associated with the plan.
        'generalCost' => {
          'type'=>'InsurancePlan::Plan::GeneralCost',
          'path'=>'Plan.generalCost',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Specific costs
        # Costs associated with the coverage provided by the product.
        'specificCost' => {
          'type'=>'InsurancePlan::Plan::SpecificCost',
          'path'=>'Plan.specificCost',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Overall costs associated with the plan.
      class GeneralCost < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'GeneralCost.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'GeneralCost.extension',
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
            'path'=>'GeneralCost.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Type of cost.
          'type' => {
            'type'=>'CodeableConcept',
            'path'=>'GeneralCost.type',
            'min'=>0,
            'max'=>1
          },
          ##
          # Number of enrollees
          # Number of participants enrolled in the plan.
          'groupSize' => {
            'type'=>'positiveInt',
            'path'=>'GeneralCost.groupSize',
            'min'=>0,
            'max'=>1
          },
          ##
          # Cost value
          # Value of the cost.
          'cost' => {
            'type'=>'Money',
            'path'=>'GeneralCost.cost',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional cost information
          # Additional information about the general costs associated with this plan.
          'comment' => {
            'type'=>'string',
            'path'=>'GeneralCost.comment',
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
        # Type of cost.
        attr_accessor :type                           # 0-1 CodeableConcept
        ##
        # Number of enrollees
        # Number of participants enrolled in the plan.
        attr_accessor :groupSize                      # 0-1 positiveInt
        ##
        # Cost value
        # Value of the cost.
        attr_accessor :cost                           # 0-1 Money
        ##
        # Additional cost information
        # Additional information about the general costs associated with this plan.
        attr_accessor :comment                        # 0-1 string
      end

      ##
      # Specific costs
      # Costs associated with the coverage provided by the product.
      class SpecificCost < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'SpecificCost.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'SpecificCost.extension',
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
            'path'=>'SpecificCost.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # General category of benefit (Medical; Dental; Vision; Drug; Mental Health; Substance Abuse; Hospice, Home Health).
          'category' => {
            'type'=>'CodeableConcept',
            'path'=>'SpecificCost.category',
            'min'=>1,
            'max'=>1
          },
          ##
          # Benefits list
          # List of the specific benefits under this category of benefit.
          'benefit' => {
            'type'=>'InsurancePlan::Plan::SpecificCost::Benefit',
            'path'=>'SpecificCost.benefit',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Benefits list
        # List of the specific benefits under this category of benefit.
        class Benefit < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'string',
              'path'=>'Benefit.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Benefit.extension',
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
              'path'=>'Benefit.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Type of specific benefit (preventative; primary care office visit; speciality office visit; hospitalization; emergency room; urgent care).
            'type' => {
              'type'=>'CodeableConcept',
              'path'=>'Benefit.type',
              'min'=>1,
              'max'=>1
            },
            ##
            # List of the costs associated with a specific benefit.
            'cost' => {
              'type'=>'InsurancePlan::Plan::SpecificCost::Benefit::Cost',
              'path'=>'Benefit.cost',
              'min'=>0,
              'max'=>Float::INFINITY
            }
          }

          ##
          # List of the costs associated with a specific benefit.
          class Cost < FHIR::Model
            include FHIR::Hashable
            include FHIR::Json
            include FHIR::Xml

            METADATA = {
              ##
              # Unique id for inter-element referencing
              # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
              'id' => {
                'type'=>'string',
                'path'=>'Cost.id',
                'min'=>0,
                'max'=>1
              },
              ##
              # Additional content defined by implementations
              # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
              # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
              'extension' => {
                'type'=>'Extension',
                'path'=>'Cost.extension',
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
                'path'=>'Cost.modifierExtension',
                'min'=>0,
                'max'=>Float::INFINITY
              },
              ##
              # Type of cost (copay; individual cap; family cap; coinsurance; deductible).
              'type' => {
                'type'=>'CodeableConcept',
                'path'=>'Cost.type',
                'min'=>1,
                'max'=>1
              },
              ##
              # in-network | out-of-network | other
              # Whether the cost applies to in-network or out-of-network providers (in-network; out-of-network; other).
              'applicability' => {
                'valid_codes'=>{
                  'http://terminology.hl7.org/CodeSystem/applicability'=>[ 'in-network', 'out-of-network', 'other' ]
                },
                'type'=>'CodeableConcept',
                'path'=>'Cost.applicability',
                'min'=>0,
                'max'=>1,
                'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/insuranceplan-applicability'}
              },
              ##
              # Additional information about the cost, such as information about funding sources (e.g. HSA, HRA, FSA, RRA).
              'qualifiers' => {
                'type'=>'CodeableConcept',
                'path'=>'Cost.qualifiers',
                'min'=>0,
                'max'=>Float::INFINITY
              },
              ##
              # The actual cost value. (some of the costs may be represented as percentages rather than currency, e.g. 10% coinsurance).
              'value' => {
                'type'=>'Quantity',
                'path'=>'Cost.value',
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
            # Type of cost (copay; individual cap; family cap; coinsurance; deductible).
            attr_accessor :type                           # 1-1 CodeableConcept
            ##
            # in-network | out-of-network | other
            # Whether the cost applies to in-network or out-of-network providers (in-network; out-of-network; other).
            attr_accessor :applicability                  # 0-1 CodeableConcept
            ##
            # Additional information about the cost, such as information about funding sources (e.g. HSA, HRA, FSA, RRA).
            attr_accessor :qualifiers                     # 0-* [ CodeableConcept ]
            ##
            # The actual cost value. (some of the costs may be represented as percentages rather than currency, e.g. 10% coinsurance).
            attr_accessor :value                          # 0-1 Quantity
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
          # Type of specific benefit (preventative; primary care office visit; speciality office visit; hospitalization; emergency room; urgent care).
          attr_accessor :type                           # 1-1 CodeableConcept
          ##
          # List of the costs associated with a specific benefit.
          attr_accessor :cost                           # 0-* [ InsurancePlan::Plan::SpecificCost::Benefit::Cost ]
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
        # General category of benefit (Medical; Dental; Vision; Drug; Mental Health; Substance Abuse; Hospice, Home Health).
        attr_accessor :category                       # 1-1 CodeableConcept
        ##
        # Benefits list
        # List of the specific benefits under this category of benefit.
        attr_accessor :benefit                        # 0-* [ InsurancePlan::Plan::SpecificCost::Benefit ]
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
      # Business Identifier for Product
      # Business identifiers assigned to this health insurance plan which remain constant as the resource is updated and propagates from server to server.
      attr_accessor :identifier                     # 0-* [ Identifier ]
      ##
      # Type of plan. For example, "Platinum" or "High Deductable".
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Where product applies
      # The geographic region in which a health insurance plan's benefits apply.
      attr_accessor :coverageArea                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Location) ]
      ##
      # What networks provide coverage
      # Reference to the network that providing the type of coverage.
      # Networks are represented as a hierarchy of organization resources.
      attr_accessor :network                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization) ]
      ##
      # Overall costs associated with the plan.
      attr_accessor :generalCost                    # 0-* [ InsurancePlan::Plan::GeneralCost ]
      ##
      # Specific costs
      # Costs associated with the coverage provided by the product.
      attr_accessor :specificCost                   # 0-* [ InsurancePlan::Plan::SpecificCost ]
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
    # Business Identifier for Product
    # Business identifiers assigned to this health insurance product which remain constant as the resource is updated and propagates from server to server.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # draft | active | retired | unknown
    # The current state of the health insurance product.
    attr_accessor :status                         # 0-1 code
    ##
    # Kind of product
    # The kind of health insurance product.
    attr_accessor :type                           # 0-* [ CodeableConcept ]
    ##
    # Official name of the health insurance product as designated by the owner.
    # If the name of the product/plan changes, consider putting the old name in the alias column so that it can still be located through searches.
    attr_accessor :name                           # 0-1 string
    ##
    # Alternate names
    # A list of alternate names that the product is known as, or was known as in the past.
    # There are no dates associated with the alias/historic names, as this is not intended to track when names were used, but to assist in searching so that older names can still result in identifying the product/plan.
    attr_accessor :local_alias                    # 0-* [ string ]
    ##
    # When the product is available
    # The period of time that the health insurance product is available.
    attr_accessor :period                         # 0-1 Period
    ##
    # Plan issuer
    # The entity that is providing  the health insurance product and underwriting the risk.  This is typically an insurance carriers, other third-party payers, or health plan sponsors comonly referred to as 'payers'.
    attr_accessor :ownedBy                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Product administrator
    # An organization which administer other services such as underwriting, customer service and/or claims processing on behalf of the health insurance product owner.
    attr_accessor :administeredBy                 # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Where product applies
    # The geographic region in which a health insurance product's benefits apply.
    attr_accessor :coverageArea                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Location) ]
    ##
    # Contact for the product
    # The contact for the health insurance product for a certain purpose.
    # Where multiple contacts for the same purpose are provided there is a standard extension that can be used to determine which one is the preferred contact to use.
    attr_accessor :contact                        # 0-* [ InsurancePlan::Contact ]
    ##
    # Technical endpoint
    # The technical endpoints providing access to services operated for the health insurance product.
    attr_accessor :endpoint                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Endpoint) ]
    ##
    # What networks are Included
    # Reference to the network included in the health insurance product.
    # Networks are represented as a hierarchy of organization resources.
    attr_accessor :network                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization) ]
    ##
    # Coverage details
    # Details about the coverage offered by the insurance product.
    attr_accessor :coverage                       # 0-* [ InsurancePlan::Coverage ]
    ##
    # Plan details
    # Details about an insurance plan.
    attr_accessor :plan                           # 0-* [ InsurancePlan::Plan ]

    def resourceType
      'InsurancePlan'
    end
  end
end
