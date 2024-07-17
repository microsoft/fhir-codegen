module FHIR

  ##
  # A record of a healthcare consumer’s  choices, which permits or denies identified recipient(s) or recipient role(s) to perform one or more actions within a given policy context, for specific purposes and periods of time.
  class Consent < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['action', 'actor', 'category', 'consentor', 'data', 'date', 'identifier', 'organization', 'patient', 'period', 'purpose', 'scope', 'security-label', 'source-reference', 'status']
    MULTIPLE_TYPES = {
      'source[x]' => ['Attachment', 'Reference']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Consent.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Consent.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Consent.implicitRules',
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
        'path'=>'Consent.language',
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
        'path'=>'Consent.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Consent.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Consent.extension',
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
        'path'=>'Consent.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identifier for this record (external references)
      # Unique identifier for this copy of the Consent Statement.
      # This identifier identifies this copy of the consent. Where this identifier is also used elsewhere as the identifier for a consent record (e.g. a CDA consent document) then the consent details are expected to be the same.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Consent.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # draft | proposed | active | rejected | inactive | entered-in-error
      # Indicates the current state of this consent.
      # This element is labeled as a modifier because the status contains the codes rejected and entered-in-error that mark the Consent as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/consent-state-codes'=>[ 'draft', 'proposed', 'active', 'rejected', 'inactive', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'Consent.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/consent-state-codes'}
      },
      ##
      # Which of the four areas this resource covers (extensible)
      # A selector of the type of consent being presented: ADR, Privacy, Treatment, Research.  This list is now extensible.
      'scope' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/consentscope'=>[ 'adr', 'research', 'patient-privacy', 'treatment' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Consent.scope',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/consent-scope'}
      },
      ##
      # Classification of the consent statement - for indexing/retrieval
      # A classification of the type of consents found in the statement. This element supports indexing and retrieval of consent statements.
      'category' => {
        'type'=>'CodeableConcept',
        'path'=>'Consent.category',
        'min'=>1,
        'max'=>Float::INFINITY
      },
      ##
      # Who the consent applies to
      # The patient/healthcare consumer to whom this consent applies.
      # Commonly, the patient the consent pertains to is the author, but for young and old people, it may be some other person.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'Consent.patient',
        'min'=>0,
        'max'=>1
      },
      ##
      # When this Consent was created or indexed
      # When this  Consent was issued / created / indexed.
      # This is not the time of the original consent, but the time that this statement was made or derived.
      'dateTime' => {
        'type'=>'dateTime',
        'path'=>'Consent.dateTime',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who is agreeing to the policy and rules
      # Either the Grantor, which is the entity responsible for granting the rights listed in a Consent Directive or the Grantee, which is the entity responsible for complying with the Consent Directive, including any obligations or limitations on authorizations and enforcement of prohibitions.
      # Commonly, the patient the consent pertains to is the consentor, but particularly for young and old people, it may be some other person - e.g. a legal guardian.
      'performer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'Consent.performer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Custodian of the consent
      # The organization that manages the consent, and the framework within which it is executed.
      'organization' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Consent.organization',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Source from which this consent is taken
      # The source on which this consent statement is based. The source might be a scanned original paper form, or a reference to a consent that links back to such a source, a reference to a document repository (e.g. XDS) that stores the original consent document.
      # The source can be contained inline (Attachment), referenced directly (Consent), referenced in a consent repository (DocumentReference), or simply by an identifier (Identifier), e.g. a CDA document id.
      'sourceAttachment' => {
        'type'=>'Attachment',
        'path'=>'Consent.source[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Source from which this consent is taken
      # The source on which this consent statement is based. The source might be a scanned original paper form, or a reference to a consent that links back to such a source, a reference to a document repository (e.g. XDS) that stores the original consent document.
      # The source can be contained inline (Attachment), referenced directly (Consent), referenced in a consent repository (DocumentReference), or simply by an identifier (Identifier), e.g. a CDA document id.
      'sourceReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Consent', 'http://hl7.org/fhir/StructureDefinition/DocumentReference', 'http://hl7.org/fhir/StructureDefinition/Contract', 'http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse'],
        'type'=>'Reference',
        'path'=>'Consent.source[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Policies covered by this consent
      # The references to the policies that are included in this consent scope. Policies may be organizational, but are often defined jurisdictionally, or in law.
      'policy' => {
        'type'=>'Consent::Policy',
        'path'=>'Consent.policy',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Regulation that this consents to
      # A reference to the specific base computable regulation or policy.
      # If the policyRule is absent, computable consent would need to be constructed from the elements of the Consent resource.
      'policyRule' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/consentpolicycodes'=>[ 'cric', 'illinois-minor-procedure', 'hipaa-auth', 'hipaa-npp', 'hipaa-restrictions', 'hipaa-research', 'hipaa-self-pay', 'mdhhs-5515', 'nyssipp', 'va-10-0484', 'va-10-0485', 'va-10-5345', 'va-10-5345a', 'va-10-5345a-mhv', 'va-10-10116', 'va-21-4142', 'ssa-827', 'dch-3927', 'squaxin', 'nl-lsp', 'at-elga', 'nih-hipaa', 'nci', 'nih-grdr', 'nih-527', 'ga4gh' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Consent.policyRule',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/consent-policy'}
      },
      ##
      # Consent Verified by patient or family
      # Whether a treatment instruction (e.g. artificial respiration yes or no) was verified with the patient, his/her family or another authorized person.
      'verification' => {
        'type'=>'Consent::Verification',
        'path'=>'Consent.verification',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Constraints to the base Consent.policyRule
      # An exception to the base policy of this consent. An exception can be an addition or removal of access permissions.
      'provision' => {
        'type'=>'Consent::Provision',
        'path'=>'Consent.provision',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Policies covered by this consent
    # The references to the policies that are included in this consent scope. Policies may be organizational, but are often defined jurisdictionally, or in law.
    class Policy < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Policy.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Policy.extension',
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
          'path'=>'Policy.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Enforcement source for policy
        # Entity or Organization having regulatory jurisdiction or accountability for  enforcing policies pertaining to Consent Directives.
        'authority' => {
          'type'=>'uri',
          'path'=>'Policy.authority',
          'min'=>0,
          'max'=>1
        },
        ##
        # Specific policy covered by this consent
        # The references to the policies that are included in this consent scope. Policies may be organizational, but are often defined jurisdictionally, or in law.
        # This element is for discoverability / documentation and does not modify or qualify the policy rules.
        'uri' => {
          'type'=>'uri',
          'path'=>'Policy.uri',
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
      # Enforcement source for policy
      # Entity or Organization having regulatory jurisdiction or accountability for  enforcing policies pertaining to Consent Directives.
      attr_accessor :authority                      # 0-1 uri
      ##
      # Specific policy covered by this consent
      # The references to the policies that are included in this consent scope. Policies may be organizational, but are often defined jurisdictionally, or in law.
      # This element is for discoverability / documentation and does not modify or qualify the policy rules.
      attr_accessor :uri                            # 0-1 uri
    end

    ##
    # Consent Verified by patient or family
    # Whether a treatment instruction (e.g. artificial respiration yes or no) was verified with the patient, his/her family or another authorized person.
    class Verification < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Verification.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Verification.extension',
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
          'path'=>'Verification.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Has been verified
        # Has the instruction been verified.
        'verified' => {
          'type'=>'boolean',
          'path'=>'Verification.verified',
          'min'=>1,
          'max'=>1
        },
        ##
        # Person who verified
        # Who verified the instruction (Patient, Relative or other Authorized Person).
        'verifiedWith' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
          'type'=>'Reference',
          'path'=>'Verification.verifiedWith',
          'min'=>0,
          'max'=>1
        },
        ##
        # When consent verified
        # Date verification was collected.
        'verificationDate' => {
          'type'=>'dateTime',
          'path'=>'Verification.verificationDate',
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
      # Has been verified
      # Has the instruction been verified.
      attr_accessor :verified                       # 1-1 boolean
      ##
      # Person who verified
      # Who verified the instruction (Patient, Relative or other Authorized Person).
      attr_accessor :verifiedWith                   # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
      ##
      # When consent verified
      # Date verification was collected.
      attr_accessor :verificationDate               # 0-1 dateTime
    end

    ##
    # Constraints to the base Consent.policyRule
    # An exception to the base policy of this consent. An exception can be an addition or removal of access permissions.
    class Provision < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Provision.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Provision.extension',
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
          'path'=>'Provision.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # deny | permit
        # Action  to take - permit or deny - when the rule conditions are met.  Not permitted in root rule, required in all nested rules.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/consent-provision-type'=>[ 'deny', 'permit' ]
          },
          'type'=>'code',
          'path'=>'Provision.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/consent-provision-type'}
        },
        ##
        # Timeframe for this rule
        # The timeframe in this rule is valid.
        'period' => {
          'type'=>'Period',
          'path'=>'Provision.period',
          'min'=>0,
          'max'=>1
        },
        ##
        # Who|what controlled by this rule (or group, by role)
        # Who or what is controlled by this rule. Use group to identify a set of actors by some property they share (e.g. 'admitting officers').
        'actor' => {
          'type'=>'Consent::Provision::Actor',
          'path'=>'Provision.actor',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Actions controlled by this rule
        # Actions controlled by this Rule.
        # Note that this is the direct action (not the grounds for the action covered in the purpose element). At present, the only action in the understood and tested scope of this resource is 'read'.
        'action' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/consentaction'=>[ 'collect', 'access', 'use', 'disclose', 'correct' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Provision.action',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/consent-action'}
        },
        ##
        # Security Labels that define affected resources
        # A security label, comprised of 0..* security label fields (Privacy tags), which define which resources are controlled by this exception.
        # If the consent specifies a security label of "R" then it applies to all resources that are labeled "R" or lower. E.g. for Confidentiality, it's a high water mark. For other kinds of security labels, subsumption logic applies. When the purpose of use tag is on the data, access request purpose of use shall not conflict.
        'securityLabel' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'ETH', 'GDIS', 'HIV', 'MST', 'SCA', 'SDV', 'SEX', 'SPI', 'BH', 'COGN', 'DVD', 'EMOTDIS', 'MH', 'PSY', 'PSYTHPN', 'SUD', 'ETHUD', 'OPIOIDUD', 'STD', 'TBOO', 'VIO', 'SICKLE', 'DEMO', 'DOB', 'GENDER', 'LIVARG', 'MARST', 'RACE', 'REL', 'B', 'EMPL', 'LOCIS', 'SSP', 'ADOL', 'CEL', 'DIA', 'DRGIS', 'EMP', 'PDS', 'PHY', 'PRS', 'COMPT', 'ACOCOMPT', 'CTCOMPT', 'FMCOMPT', 'HRCOMPT', 'LRCOMPT', 'PACOMPT', 'RESCOMPT', 'RMGTCOMPT', 'SecurityPolicy', 'AUTHPOL', 'ACCESSCONSCHEME', 'DELEPOL', 'ObligationPolicy', 'ANONY', 'AOD', 'AUDIT', 'AUDTR', 'CPLYCC', 'CPLYCD', 'CPLYJPP', 'CPLYOPP', 'CPLYOSP', 'CPLYPOL', 'DECLASSIFYLABEL', 'DEID', 'DELAU', 'DOWNGRDLABEL', 'DRIVLABEL', 'ENCRYPT', 'ENCRYPTR', 'ENCRYPTT', 'ENCRYPTU', 'HUAPRV', 'LABEL', 'MASK', 'MINEC', 'PERSISTLABEL', 'PRIVMARK', 'PSEUD', 'REDACT', 'UPGRDLABEL', 'RefrainPolicy', 'NOAUTH', 'NOCOLLECT', 'NODSCLCD', 'NODSCLCDS', 'NOINTEGRATE', 'NOLIST', 'NOMOU', 'NOORGPOL', 'NOPAT', 'NOPERSISTP', 'NORDSCLCD', 'NORDSCLCDS', 'NORDSCLW', 'NORELINK', 'NOREUSE', 'NOVIP', 'ORCON' ],
            'http://terminology.hl7.org/CodeSystem/v3-ActReason'=>[ 'PurposeOfUse', 'HMARKT', 'HOPERAT', 'CAREMGT', 'DONAT', 'FRAUD', 'GOV', 'HACCRED', 'HCOMPL', 'HDECD', 'HDIRECT', 'HDM', 'HLEGAL', 'HOUTCOMS', 'HPRGRP', 'HQUALIMP', 'HSYSADMIN', 'LABELING', 'METAMGT', 'MEMADMIN', 'MILCDM', 'PATADMIN', 'PATSFTY', 'PERFMSR', 'RECORDMGT', 'SYSDEV', 'HTEST', 'TRAIN', 'HPAYMT', 'CLMATTCH', 'COVAUTH', 'COVERAGE', 'ELIGDTRM', 'ELIGVER', 'ENROLLM', 'MILDCRG', 'REMITADV', 'HRESCH', 'BIORCH', 'CLINTRCH', 'CLINTRCHNPC', 'CLINTRCHPC', 'PRECLINTRCH', 'DSRCH', 'POARCH', 'TRANSRCH', 'PATRQT', 'FAMRQT', 'PWATRNY', 'SUPNWK', 'PUBHLTH', 'DISASTER', 'THREAT', 'TREAT', 'CLINTRL', 'COC', 'ETREAT', 'BTG', 'ERTREAT', 'POPHLTH' ],
            'http://terminology.hl7.org/CodeSystem/v3-Confidentiality'=>[ 'U', 'L', 'M', 'N', 'R', 'V' ],
            'http://terminology.hl7.org/CodeSystem/v3-ObservationValue'=>[ 'ABSTRED', 'AGGRED', 'ANONYED', 'MAPPED', 'MASKED', 'PSEUDED', 'REDACTED', 'SUBSETTED', 'SYNTAC', 'TRSLT', 'VERSIONED', 'CRYTOHASH', 'DIGSIG', 'HRELIABLE', 'RELIABLE', 'UNCERTREL', 'UNRELIABLE', 'CLINAST', 'DEVAST', 'HCPAST', 'PACQAST', 'PATAST', 'PAYAST', 'PROAST', 'SDMAST', 'CLINRPT', 'DEVRPT', 'HCPRPT', 'PACQRPT', 'PATRPT', 'PAYRPT', 'PRORPT', 'SDMRPT' ]
          },
          'type'=>'Coding',
          'path'=>'Provision.securityLabel',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/security-labels'}
        },
        ##
        # Context of activities covered by this rule
        # The context of the activities a user is taking - why the user is accessing the data - that are controlled by this rule.
        # When the purpose of use tag is on the data, access request purpose of use shall not conflict.
        'purpose' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-ActReason'=>[ 'PurposeOfUse', 'HMARKT', 'HOPERAT', 'CAREMGT', 'DONAT', 'FRAUD', 'GOV', 'HACCRED', 'HCOMPL', 'HDECD', 'HDIRECT', 'HDM', 'HLEGAL', 'HOUTCOMS', 'HPRGRP', 'HQUALIMP', 'HSYSADMIN', 'LABELING', 'METAMGT', 'MEMADMIN', 'MILCDM', 'PATADMIN', 'PATSFTY', 'PERFMSR', 'RECORDMGT', 'SYSDEV', 'HTEST', 'TRAIN', 'HPAYMT', 'CLMATTCH', 'COVAUTH', 'COVERAGE', 'ELIGDTRM', 'ELIGVER', 'ENROLLM', 'MILDCRG', 'REMITADV', 'HRESCH', 'BIORCH', 'CLINTRCH', 'CLINTRCHNPC', 'CLINTRCHPC', 'PRECLINTRCH', 'DSRCH', 'POARCH', 'TRANSRCH', 'PATRQT', 'FAMRQT', 'PWATRNY', 'SUPNWK', 'PUBHLTH', 'DISASTER', 'THREAT', 'TREAT', 'CLINTRL', 'COC', 'ETREAT', 'BTG', 'ERTREAT', 'POPHLTH' ]
          },
          'type'=>'Coding',
          'path'=>'Provision.purpose',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-PurposeOfUse'}
        },
        ##
        # e.g. Resource Type, Profile, CDA, etc.
        # The class of information covered by this rule. The type can be a FHIR resource type, a profile on a type, or a CDA document, or some other type that indicates what sort of information the consent relates to.
        # Multiple types are or'ed together. The intention of the contentType element is that the codes refer to profiles or document types defined in a standard or an implementation guide somewhere.
        'class' => {
          'local_name'=>'local_class'
          'type'=>'Coding',
          'path'=>'Provision.class',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # e.g. LOINC or SNOMED CT code, etc. in the content
        # If this code is found in an instance, then the rule applies.
        # Typical use of this is a Document code with class = CDA.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Provision.code',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Timeframe for data controlled by this rule
        # Clinical or Operational Relevant period of time that bounds the data controlled by this rule.
        # This has a different sense to the Consent.period - that is when the consent agreement holds. This is the time period of the data that is controlled by the agreement.
        'dataPeriod' => {
          'type'=>'Period',
          'path'=>'Provision.dataPeriod',
          'min'=>0,
          'max'=>1
        },
        ##
        # Data controlled by this rule
        # The resources controlled by this rule if specific resources are referenced.
        'data' => {
          'type'=>'Consent::Provision::Data',
          'path'=>'Provision.data',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Nested Exception Rules
        # Rules which provide exceptions to the base rule or subrules.
        'provision' => {
          'type'=>'Consent::Provision',
          'path'=>'Provision.provision',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Who|what controlled by this rule (or group, by role)
      # Who or what is controlled by this rule. Use group to identify a set of actors by some property they share (e.g. 'admitting officers').
      class Actor < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Actor.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Actor.extension',
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
            'path'=>'Actor.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # How the actor is involved
          # How the individual is involved in the resources content that is described in the exception.
          'role' => {
            'valid_codes'=>{
              'http://dicom.nema.org/resources/ontology/DCM'=>[ '110150', '110151', '110152', '110153', '110154', '110155' ],
              'http://terminology.hl7.org/CodeSystem/contractsignertypecodes'=>[ 'AMENDER', 'COAUTH', 'CONT', 'EVTWIT', 'PRIMAUTH', 'REVIEWER', 'SOURCE', 'TRANS', 'VALID', 'VERF' ],
              'http://terminology.hl7.org/CodeSystem/extra-security-role-type'=>[ 'authserver', 'datacollector', 'dataprocessor', 'datasubject', 'humanuser' ],
              'http://terminology.hl7.org/CodeSystem/v3-ParticipationFunction'=>[ 'AUCG', 'AULR', 'AUTM', 'AUWA', 'PROMSK' ],
              'http://terminology.hl7.org/CodeSystem/v3-ParticipationType'=>[ 'AUT', 'CST', 'INF', 'IRCP', 'LA', 'TRC', 'WIT' ],
              'http://terminology.hl7.org/CodeSystem/v3-RoleClass'=>[ 'AFFL', 'AGNT', 'ASSIGNED', 'CLAIM', 'COVPTY', 'DEPEN', 'ECON', 'EMP', 'GUARD', 'INVSBJ', 'NAMED', 'NOK', 'PAT', 'PROV', 'NOT' ],
              'http://terminology.hl7.org/CodeSystem/v3-RoleCode'=>[ 'CLASSIFIER', 'CONSENTER', 'CONSWIT', 'COPART', 'DECLASSIFIER', 'DELEGATEE', 'DELEGATOR', 'DOWNGRDER', 'DPOWATT', 'EXCEST', 'GRANTEE', 'GRANTOR', 'GT', 'GUADLTM', 'HPOWATT', 'INTPRTER', 'POWATT', 'RESPRSN', 'SPOWATT', '_CitizenRoleType', 'CAS', 'CASM', 'CN', 'CNRP', 'CNRPM', 'CPCA', 'CRP', 'CRPM' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Actor.role',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/security-role-type'}
          },
          ##
          # Resource for the actor (or group, by role)
          # The resource that identifies the actor. To identify actors by type, use group to identify a set of actors by some property they share (e.g. 'admitting officers').
          'reference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
            'type'=>'Reference',
            'path'=>'Actor.reference',
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
        # How the actor is involved
        # How the individual is involved in the resources content that is described in the exception.
        attr_accessor :role                           # 1-1 CodeableConcept
        ##
        # Resource for the actor (or group, by role)
        # The resource that identifies the actor. To identify actors by type, use group to identify a set of actors by some property they share (e.g. 'admitting officers').
        attr_accessor :reference                      # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
      end

      ##
      # Data controlled by this rule
      # The resources controlled by this rule if specific resources are referenced.
      class Data < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Data.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Data.extension',
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
            'path'=>'Data.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # instance | related | dependents | authoredby
          # How the resource reference is interpreted when testing consent restrictions.
          'meaning' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/consent-data-meaning'=>[ 'instance', 'related', 'dependents', 'authoredby' ]
            },
            'type'=>'code',
            'path'=>'Data.meaning',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/consent-data-meaning'}
          },
          ##
          # The actual data reference
          # A reference to a specific resource that defines which resources are covered by this consent.
          'reference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
            'type'=>'Reference',
            'path'=>'Data.reference',
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
        # instance | related | dependents | authoredby
        # How the resource reference is interpreted when testing consent restrictions.
        attr_accessor :meaning                        # 1-1 code
        ##
        # The actual data reference
        # A reference to a specific resource that defines which resources are covered by this consent.
        attr_accessor :reference                      # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
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
      # deny | permit
      # Action  to take - permit or deny - when the rule conditions are met.  Not permitted in root rule, required in all nested rules.
      attr_accessor :type                           # 0-1 code
      ##
      # Timeframe for this rule
      # The timeframe in this rule is valid.
      attr_accessor :period                         # 0-1 Period
      ##
      # Who|what controlled by this rule (or group, by role)
      # Who or what is controlled by this rule. Use group to identify a set of actors by some property they share (e.g. 'admitting officers').
      attr_accessor :actor                          # 0-* [ Consent::Provision::Actor ]
      ##
      # Actions controlled by this rule
      # Actions controlled by this Rule.
      # Note that this is the direct action (not the grounds for the action covered in the purpose element). At present, the only action in the understood and tested scope of this resource is 'read'.
      attr_accessor :action                         # 0-* [ CodeableConcept ]
      ##
      # Security Labels that define affected resources
      # A security label, comprised of 0..* security label fields (Privacy tags), which define which resources are controlled by this exception.
      # If the consent specifies a security label of "R" then it applies to all resources that are labeled "R" or lower. E.g. for Confidentiality, it's a high water mark. For other kinds of security labels, subsumption logic applies. When the purpose of use tag is on the data, access request purpose of use shall not conflict.
      attr_accessor :securityLabel                  # 0-* [ Coding ]
      ##
      # Context of activities covered by this rule
      # The context of the activities a user is taking - why the user is accessing the data - that are controlled by this rule.
      # When the purpose of use tag is on the data, access request purpose of use shall not conflict.
      attr_accessor :purpose                        # 0-* [ Coding ]
      ##
      # e.g. Resource Type, Profile, CDA, etc.
      # The class of information covered by this rule. The type can be a FHIR resource type, a profile on a type, or a CDA document, or some other type that indicates what sort of information the consent relates to.
      # Multiple types are or'ed together. The intention of the contentType element is that the codes refer to profiles or document types defined in a standard or an implementation guide somewhere.
      attr_accessor :local_class                    # 0-* [ Coding ]
      ##
      # e.g. LOINC or SNOMED CT code, etc. in the content
      # If this code is found in an instance, then the rule applies.
      # Typical use of this is a Document code with class = CDA.
      attr_accessor :code                           # 0-* [ CodeableConcept ]
      ##
      # Timeframe for data controlled by this rule
      # Clinical or Operational Relevant period of time that bounds the data controlled by this rule.
      # This has a different sense to the Consent.period - that is when the consent agreement holds. This is the time period of the data that is controlled by the agreement.
      attr_accessor :dataPeriod                     # 0-1 Period
      ##
      # Data controlled by this rule
      # The resources controlled by this rule if specific resources are referenced.
      attr_accessor :data                           # 0-* [ Consent::Provision::Data ]
      ##
      # Nested Exception Rules
      # Rules which provide exceptions to the base rule or subrules.
      attr_accessor :provision                      # 0-* [ Consent::Provision ]
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
    # Identifier for this record (external references)
    # Unique identifier for this copy of the Consent Statement.
    # This identifier identifies this copy of the consent. Where this identifier is also used elsewhere as the identifier for a consent record (e.g. a CDA consent document) then the consent details are expected to be the same.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # draft | proposed | active | rejected | inactive | entered-in-error
    # Indicates the current state of this consent.
    # This element is labeled as a modifier because the status contains the codes rejected and entered-in-error that mark the Consent as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Which of the four areas this resource covers (extensible)
    # A selector of the type of consent being presented: ADR, Privacy, Treatment, Research.  This list is now extensible.
    attr_accessor :scope                          # 1-1 CodeableConcept
    ##
    # Classification of the consent statement - for indexing/retrieval
    # A classification of the type of consents found in the statement. This element supports indexing and retrieval of consent statements.
    attr_accessor :category                       # 1-* [ CodeableConcept ]
    ##
    # Who the consent applies to
    # The patient/healthcare consumer to whom this consent applies.
    # Commonly, the patient the consent pertains to is the author, but for young and old people, it may be some other person.
    attr_accessor :patient                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # When this Consent was created or indexed
    # When this  Consent was issued / created / indexed.
    # This is not the time of the original consent, but the time that this statement was made or derived.
    attr_accessor :dateTime                       # 0-1 dateTime
    ##
    # Who is agreeing to the policy and rules
    # Either the Grantor, which is the entity responsible for granting the rights listed in a Consent Directive or the Grantee, which is the entity responsible for complying with the Consent Directive, including any obligations or limitations on authorizations and enforcement of prohibitions.
    # Commonly, the patient the consent pertains to is the consentor, but particularly for young and old people, it may be some other person - e.g. a legal guardian.
    attr_accessor :performer                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/PractitionerRole) ]
    ##
    # Custodian of the consent
    # The organization that manages the consent, and the framework within which it is executed.
    attr_accessor :organization                   # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization) ]
    ##
    # Source from which this consent is taken
    # The source on which this consent statement is based. The source might be a scanned original paper form, or a reference to a consent that links back to such a source, a reference to a document repository (e.g. XDS) that stores the original consent document.
    # The source can be contained inline (Attachment), referenced directly (Consent), referenced in a consent repository (DocumentReference), or simply by an identifier (Identifier), e.g. a CDA document id.
    attr_accessor :sourceAttachment               # 0-1 Attachment
    ##
    # Source from which this consent is taken
    # The source on which this consent statement is based. The source might be a scanned original paper form, or a reference to a consent that links back to such a source, a reference to a document repository (e.g. XDS) that stores the original consent document.
    # The source can be contained inline (Attachment), referenced directly (Consent), referenced in a consent repository (DocumentReference), or simply by an identifier (Identifier), e.g. a CDA document id.
    attr_accessor :sourceReference                # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Consent|http://hl7.org/fhir/StructureDefinition/DocumentReference|http://hl7.org/fhir/StructureDefinition/Contract|http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse)
    ##
    # Policies covered by this consent
    # The references to the policies that are included in this consent scope. Policies may be organizational, but are often defined jurisdictionally, or in law.
    attr_accessor :policy                         # 0-* [ Consent::Policy ]
    ##
    # Regulation that this consents to
    # A reference to the specific base computable regulation or policy.
    # If the policyRule is absent, computable consent would need to be constructed from the elements of the Consent resource.
    attr_accessor :policyRule                     # 0-1 CodeableConcept
    ##
    # Consent Verified by patient or family
    # Whether a treatment instruction (e.g. artificial respiration yes or no) was verified with the patient, his/her family or another authorized person.
    attr_accessor :verification                   # 0-* [ Consent::Verification ]
    ##
    # Constraints to the base Consent.policyRule
    # An exception to the base policy of this consent. An exception can be an addition or removal of access permissions.
    attr_accessor :provision                      # 0-1 Consent::Provision

    def resourceType
      'Consent'
    end
  end
end
