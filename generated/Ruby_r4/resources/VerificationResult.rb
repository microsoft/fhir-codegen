module FHIR

  ##
  # Describes validation requirements, source(s), status and dates for one or more elements.
  class VerificationResult < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['target']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'VerificationResult.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'VerificationResult.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'VerificationResult.implicitRules',
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
        'path'=>'VerificationResult.language',
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
        'path'=>'VerificationResult.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'VerificationResult.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'VerificationResult.extension',
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
        'path'=>'VerificationResult.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A resource that was validated.
      'target' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'VerificationResult.target',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The fhirpath location(s) within the resource that was validated.
      'targetLocation' => {
        'type'=>'string',
        'path'=>'VerificationResult.targetLocation',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # none | initial | periodic
      # The frequency with which the target must be validated (none; initial; periodic).
      'need' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/need'=>[ 'none', 'initial', 'periodic' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'VerificationResult.need',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/verificationresult-need'}
      },
      ##
      # attested | validated | in-process | req-revalid | val-fail | reval-fail
      # The validation status of the target (attested; validated; in process; requires revalidation; validation failed; revalidation failed).
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/CodeSystem/status'=>[ 'attested', 'validated', 'in-process', 'req-revalid', 'val-fail', 'reval-fail' ]
        },
        'type'=>'code',
        'path'=>'VerificationResult.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/verificationresult-status'}
      },
      ##
      # When the validation status was updated.
      'statusDate' => {
        'type'=>'dateTime',
        'path'=>'VerificationResult.statusDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # nothing | primary | multiple
      # What the target is validated against (nothing; primary source; multiple sources).
      'validationType' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/validation-type'=>[ 'nothing', 'primary', 'multiple' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'VerificationResult.validationType',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/verificationresult-validation-type'}
      },
      ##
      # The primary process by which the target is validated (edit check; value set; primary source; multiple sources; standalone; in context).
      'validationProcess' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/validation-process'=>[ 'edit-check', 'valueset', 'primary', 'multi', 'standalone', 'in-context' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'VerificationResult.validationProcess',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/verificationresult-validation-process'}
      },
      ##
      # Frequency of revalidation.
      'frequency' => {
        'type'=>'Timing',
        'path'=>'VerificationResult.frequency',
        'min'=>0,
        'max'=>1
      },
      ##
      # The date/time validation was last completed (including failed validations).
      'lastPerformed' => {
        'type'=>'dateTime',
        'path'=>'VerificationResult.lastPerformed',
        'min'=>0,
        'max'=>1
      },
      ##
      # The date when target is next validated, if appropriate.
      'nextScheduled' => {
        'type'=>'date',
        'path'=>'VerificationResult.nextScheduled',
        'min'=>0,
        'max'=>1
      },
      ##
      # fatal | warn | rec-only | none
      # The result if validation fails (fatal; warning; record only; none).
      'failureAction' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/failure-action'=>[ 'fatal', 'warn', 'rec-only', 'none' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'VerificationResult.failureAction',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/verificationresult-failure-action'}
      },
      ##
      # Information about the primary source(s) involved in validation.
      'primarySource' => {
        'type'=>'VerificationResult::PrimarySource',
        'path'=>'VerificationResult.primarySource',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Information about the entity attesting to information.
      'attestation' => {
        'type'=>'VerificationResult::Attestation',
        'path'=>'VerificationResult.attestation',
        'min'=>0,
        'max'=>1
      },
      ##
      # Information about the entity validating information.
      'validator' => {
        'type'=>'VerificationResult::Validator',
        'path'=>'VerificationResult.validator',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Information about the primary source(s) involved in validation.
    class PrimarySource < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'PrimarySource.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'PrimarySource.extension',
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
          'path'=>'PrimarySource.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Reference to the primary source.
        'who' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
          'type'=>'Reference',
          'path'=>'PrimarySource.who',
          'min'=>0,
          'max'=>1
        },
        ##
        # Type of primary source (License Board; Primary Education; Continuing Education; Postal Service; Relationship owner; Registration Authority; legal source; issuing source; authoritative source).
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/primary-source-type'=>[ 'lic-board', 'prim', 'cont-ed', 'post-serv', 'rel-own', 'reg-auth', 'legal', 'issuer', 'auth-source' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'PrimarySource.type',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/verificationresult-primary-source-type'}
        },
        ##
        # Method for exchanging information with the primary source
        # Method for communicating with the primary source (manual; API; Push).
        'communicationMethod' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/verificationresult-communication-method'=>[ 'manual', 'portal', 'pull', 'push' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'PrimarySource.communicationMethod',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/verificationresult-communication-method'}
        },
        ##
        # successful | failed | unknown
        # Status of the validation of the target against the primary source (successful; failed; unknown).
        'validationStatus' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/validation-status'=>[ 'successful', 'failed', 'unknown' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'PrimarySource.validationStatus',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/verificationresult-validation-status'}
        },
        ##
        # When the target was validated against the primary source.
        'validationDate' => {
          'type'=>'dateTime',
          'path'=>'PrimarySource.validationDate',
          'min'=>0,
          'max'=>1
        },
        ##
        # yes | no | undetermined
        # Ability of the primary source to push updates/alerts (yes; no; undetermined).
        'canPushUpdates' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/can-push-updates'=>[ 'yes', 'no', 'undetermined' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'PrimarySource.canPushUpdates',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/verificationresult-can-push-updates'}
        },
        ##
        # specific | any | source
        # Type of alerts/updates the primary source can send (specific requested changes; any changes; as defined by source).
        'pushTypeAvailable' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/push-type-available'=>[ 'specific', 'any', 'source' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'PrimarySource.pushTypeAvailable',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/verificationresult-push-type-available'}
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
      # Reference to the primary source.
      attr_accessor :who                            # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
      ##
      # Type of primary source (License Board; Primary Education; Continuing Education; Postal Service; Relationship owner; Registration Authority; legal source; issuing source; authoritative source).
      attr_accessor :type                           # 0-* [ CodeableConcept ]
      ##
      # Method for exchanging information with the primary source
      # Method for communicating with the primary source (manual; API; Push).
      attr_accessor :communicationMethod            # 0-* [ CodeableConcept ]
      ##
      # successful | failed | unknown
      # Status of the validation of the target against the primary source (successful; failed; unknown).
      attr_accessor :validationStatus               # 0-1 CodeableConcept
      ##
      # When the target was validated against the primary source.
      attr_accessor :validationDate                 # 0-1 dateTime
      ##
      # yes | no | undetermined
      # Ability of the primary source to push updates/alerts (yes; no; undetermined).
      attr_accessor :canPushUpdates                 # 0-1 CodeableConcept
      ##
      # specific | any | source
      # Type of alerts/updates the primary source can send (specific requested changes; any changes; as defined by source).
      attr_accessor :pushTypeAvailable              # 0-* [ CodeableConcept ]
    end

    ##
    # Information about the entity attesting to information.
    class Attestation < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Attestation.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Attestation.extension',
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
          'path'=>'Attestation.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The individual or organization attesting to information.
        'who' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Attestation.who',
          'min'=>0,
          'max'=>1
        },
        ##
        # When the who is asserting on behalf of another (organization or individual).
        'onBehalfOf' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
          'type'=>'Reference',
          'path'=>'Attestation.onBehalfOf',
          'min'=>0,
          'max'=>1
        },
        ##
        # The method by which attested information was submitted/retrieved (manual; API; Push).
        'communicationMethod' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/verificationresult-communication-method'=>[ 'manual', 'portal', 'pull', 'push' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Attestation.communicationMethod',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/verificationresult-communication-method'}
        },
        ##
        # The date the information was attested to.
        'date' => {
          'type'=>'date',
          'path'=>'Attestation.date',
          'min'=>0,
          'max'=>1
        },
        ##
        # A digital identity certificate associated with the attestation source.
        'sourceIdentityCertificate' => {
          'type'=>'string',
          'path'=>'Attestation.sourceIdentityCertificate',
          'min'=>0,
          'max'=>1
        },
        ##
        # A digital identity certificate associated with the proxy entity submitting attested information on behalf of the attestation source.
        'proxyIdentityCertificate' => {
          'type'=>'string',
          'path'=>'Attestation.proxyIdentityCertificate',
          'min'=>0,
          'max'=>1
        },
        ##
        # Proxy signature
        # Signed assertion by the proxy entity indicating that they have the right to submit attested information on behalf of the attestation source.
        'proxySignature' => {
          'type'=>'Signature',
          'path'=>'Attestation.proxySignature',
          'min'=>0,
          'max'=>1
        },
        ##
        # Attester signature
        # Signed assertion by the attestation source that they have attested to the information.
        'sourceSignature' => {
          'type'=>'Signature',
          'path'=>'Attestation.sourceSignature',
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
      # The individual or organization attesting to information.
      attr_accessor :who                            # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # When the who is asserting on behalf of another (organization or individual).
      attr_accessor :onBehalfOf                     # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
      ##
      # The method by which attested information was submitted/retrieved (manual; API; Push).
      attr_accessor :communicationMethod            # 0-1 CodeableConcept
      ##
      # The date the information was attested to.
      attr_accessor :date                           # 0-1 date
      ##
      # A digital identity certificate associated with the attestation source.
      attr_accessor :sourceIdentityCertificate      # 0-1 string
      ##
      # A digital identity certificate associated with the proxy entity submitting attested information on behalf of the attestation source.
      attr_accessor :proxyIdentityCertificate       # 0-1 string
      ##
      # Proxy signature
      # Signed assertion by the proxy entity indicating that they have the right to submit attested information on behalf of the attestation source.
      attr_accessor :proxySignature                 # 0-1 Signature
      ##
      # Attester signature
      # Signed assertion by the attestation source that they have attested to the information.
      attr_accessor :sourceSignature                # 0-1 Signature
    end

    ##
    # Information about the entity validating information.
    class Validator < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Validator.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Validator.extension',
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
          'path'=>'Validator.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Reference to the organization validating information.
        'organization' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Validator.organization',
          'min'=>1,
          'max'=>1
        },
        ##
        # A digital identity certificate associated with the validator.
        'identityCertificate' => {
          'type'=>'string',
          'path'=>'Validator.identityCertificate',
          'min'=>0,
          'max'=>1
        },
        ##
        # Validator signature
        # Signed assertion by the validator that they have validated the information.
        'attestationSignature' => {
          'type'=>'Signature',
          'path'=>'Validator.attestationSignature',
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
      # Reference to the organization validating information.
      attr_accessor :organization                   # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # A digital identity certificate associated with the validator.
      attr_accessor :identityCertificate            # 0-1 string
      ##
      # Validator signature
      # Signed assertion by the validator that they have validated the information.
      attr_accessor :attestationSignature           # 0-1 Signature
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
    # A resource that was validated.
    attr_accessor :target                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # The fhirpath location(s) within the resource that was validated.
    attr_accessor :targetLocation                 # 0-* [ string ]
    ##
    # none | initial | periodic
    # The frequency with which the target must be validated (none; initial; periodic).
    attr_accessor :need                           # 0-1 CodeableConcept
    ##
    # attested | validated | in-process | req-revalid | val-fail | reval-fail
    # The validation status of the target (attested; validated; in process; requires revalidation; validation failed; revalidation failed).
    attr_accessor :status                         # 1-1 code
    ##
    # When the validation status was updated.
    attr_accessor :statusDate                     # 0-1 dateTime
    ##
    # nothing | primary | multiple
    # What the target is validated against (nothing; primary source; multiple sources).
    attr_accessor :validationType                 # 0-1 CodeableConcept
    ##
    # The primary process by which the target is validated (edit check; value set; primary source; multiple sources; standalone; in context).
    attr_accessor :validationProcess              # 0-* [ CodeableConcept ]
    ##
    # Frequency of revalidation.
    attr_accessor :frequency                      # 0-1 Timing
    ##
    # The date/time validation was last completed (including failed validations).
    attr_accessor :lastPerformed                  # 0-1 dateTime
    ##
    # The date when target is next validated, if appropriate.
    attr_accessor :nextScheduled                  # 0-1 date
    ##
    # fatal | warn | rec-only | none
    # The result if validation fails (fatal; warning; record only; none).
    attr_accessor :failureAction                  # 0-1 CodeableConcept
    ##
    # Information about the primary source(s) involved in validation.
    attr_accessor :primarySource                  # 0-* [ VerificationResult::PrimarySource ]
    ##
    # Information about the entity attesting to information.
    attr_accessor :attestation                    # 0-1 VerificationResult::Attestation
    ##
    # Information about the entity validating information.
    attr_accessor :validator                      # 0-* [ VerificationResult::Validator ]

    def resourceType
      'VerificationResult'
    end
  end
end
