module FHIR

  ##
  # A set of healthcare-related information that is assembled together into a single logical package that provides a single coherent statement of meaning, establishes its own context and that has clinical attestation with regard to who is making the statement. A Composition defines the structure and narrative content necessary for a document. However, a Composition alone does not constitute a document. Rather, the Composition must be the first entry in a Bundle where Bundle.type=document, and any other resources referenced from Composition must be included as subsequent entries in the Bundle (for example Patient, Practitioner, Encounter, etc.).
  # To support documents, and also to capture the EN13606 notion of an attested commit to the patient EHR, and to allow a set of disparate resources at the information/engineering level to be gathered into a clinical statement.
  class Composition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['attester', 'author', 'category', 'confidentiality', 'context', 'date', 'encounter', 'entry', 'identifier', 'patient', 'period', 'related-id', 'related-ref', 'section', 'status', 'subject', 'title', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Composition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Composition.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Composition.implicitRules',
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
        'path'=>'Composition.language',
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
        'path'=>'Composition.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Composition.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Composition.extension',
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
        'path'=>'Composition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Version-independent identifier for the Composition
      # A version-independent identifier for the Composition. This identifier stays constant as the composition is changed over time.
      # Similar to ClinicalDocument/setId in CDA. See discussion in resource definition for how these relate.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Composition.identifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # preliminary | final | amended | entered-in-error
      # The workflow/clinical status of this composition. The status is a marker for the clinical standing of the document.
      # If a composition is marked as withdrawn, the compositions/documents in the series, or data from the composition or document series, should never be displayed to a user without being clearly marked as untrustworthy. The flag "entered-in-error" is why this element is labeled as a modifier of other elements.   
      # 
      # Some reporting work flows require that the original narrative of a final document never be altered; instead, only new narrative can be added. The composition resource has no explicit status for explicitly noting whether this business rule is in effect. This would be handled by an extension if required.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/composition-status'=>[ 'preliminary', 'final', 'amended', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'Composition.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/composition-status'}
      },
      ##
      # Kind of composition (LOINC if possible)
      # Specifies the particular kind of composition (e.g. History and Physical, Discharge Summary, Progress Note). This usually equates to the purpose of making the composition.
      # For Composition type, LOINC is ubiquitous and strongly endorsed by HL7. Most implementation guides will require a specific LOINC code, or use LOINC as an extensible binding.
      'type' => {
        'type'=>'CodeableConcept',
        'path'=>'Composition.type',
        'min'=>1,
        'max'=>1
      },
      ##
      # Categorization of Composition
      # A categorization for the type of the composition - helps for indexing and searching. This may be implied by or derived from the code specified in the Composition Type.
      # This is a metadata field from [XDS/MHD](http://wiki.ihe.net/index.php?title=Mobile_access_to_Health_Documents_(MHD)).
      'category' => {
        'type'=>'CodeableConcept',
        'path'=>'Composition.category',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who and/or what the composition is about
      # Who or what the composition is about. The composition can be about a person, (patient or healthcare practitioner), a device (e.g. a machine) or even a group of subjects (such as a document about a herd of livestock, or a set of patients that share a common exposure).
      # For clinical documents, this is usually the patient.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Composition.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Context of the Composition
      # Describes the clinical encounter or type of care this documentation is associated with.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'Composition.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Composition editing time
      # The composition editing time, when the composition was last logically changed by the author.
      # The Last Modified Date on the composition may be after the date of the document was attested without being changed.
      'date' => {
        'type'=>'dateTime',
        'path'=>'Composition.date',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who and/or what authored the composition
      # Identifies who is responsible for the information in the composition, not necessarily who typed it in.
      'author' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Composition.author',
        'min'=>1,
        'max'=>Float::INFINITY
      },
      ##
      # Human Readable name/title
      # Official human-readable label for the composition.
      # For many compositions, the title is the same as the text or a display name of Composition.type (e.g. a "consultation" or "progress note"). Note that CDA does not make title mandatory, but there are no known cases where it is useful for title to be omitted, so it is mandatory here. Feedback on this requirement is welcome during the trial use period.
      'title' => {
        'type'=>'string',
        'path'=>'Composition.title',
        'min'=>1,
        'max'=>1
      },
      ##
      # As defined by affinity domain
      # The code specifying the level of confidentiality of the Composition.
      # The exact use of this element, and enforcement and issues related to highly sensitive documents are out of scope for the base specification, and delegated to implementation profiles (see security section).  This element is labeled as a modifier because highly confidential documents must not be treated as if they are not.
      'confidentiality' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-Confidentiality'=>[ 'U', 'L', 'M', 'N', 'R', 'V' ]
        },
        'type'=>'code',
        'path'=>'Composition.confidentiality',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ConfidentialityClassification'}
      },
      ##
      # Attests to accuracy of composition
      # A participant who has attested to the accuracy of the composition/document.
      # Only list each attester once.
      'attester' => {
        'type'=>'Composition::Attester',
        'path'=>'Composition.attester',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Organization which maintains the composition
      # Identifies the organization or group who is responsible for ongoing maintenance of and access to the composition/document information.
      # This is useful when documents are derived from a composition - provides guidance for how to get the latest version of the document. This is optional because this is sometimes not known by the authoring system, and can be inferred by context. However, it is important that this information be known when working with a derived document, so providing a custodian is encouraged.
      'custodian' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Composition.custodian',
        'min'=>0,
        'max'=>1
      },
      ##
      # Relationships to other compositions/documents
      # Relationships that this composition has with other compositions or documents that already exist.
      # A document is a version specific composition.
      'relatesTo' => {
        'type'=>'Composition::RelatesTo',
        'path'=>'Composition.relatesTo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The clinical service(s) being documented
      # The clinical service, such as a colonoscopy or an appendectomy, being documented.
      # The event needs to be consistent with the type element, though can provide further information if desired.
      'event' => {
        'type'=>'Composition::Event',
        'path'=>'Composition.event',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Composition is broken into sections
      # The root of the sections that make up the composition.
      'section' => {
        'type'=>'Composition::Section',
        'path'=>'Composition.section',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Attests to accuracy of composition
    # A participant who has attested to the accuracy of the composition/document.
    # Only list each attester once.
    class Attester < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Attester.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Attester.extension',
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
          'path'=>'Attester.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # personal | professional | legal | official
        # The type of attestation the authenticator offers.
        'mode' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/composition-attestation-mode'=>[ 'personal', 'professional', 'legal', 'official' ]
          },
          'type'=>'code',
          'path'=>'Attester.mode',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/composition-attestation-mode'}
        },
        ##
        # When the composition was attested by the party.
        'time' => {
          'type'=>'dateTime',
          'path'=>'Attester.time',
          'min'=>0,
          'max'=>1
        },
        ##
        # Who attested the composition in the specified way.
        'party' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Attester.party',
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
      # personal | professional | legal | official
      # The type of attestation the authenticator offers.
      attr_accessor :mode                           # 1-1 code
      ##
      # When the composition was attested by the party.
      attr_accessor :time                           # 0-1 dateTime
      ##
      # Who attested the composition in the specified way.
      attr_accessor :party                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    end

    ##
    # Relationships to other compositions/documents
    # Relationships that this composition has with other compositions or documents that already exist.
    # A document is a version specific composition.
    class RelatesTo < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'target[x]' => ['Identifier', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'RelatesTo.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'RelatesTo.extension',
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
          'path'=>'RelatesTo.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # replaces | transforms | signs | appends
        # The type of relationship that this composition has with anther composition or document.
        # If this document appends another document, then the document cannot be fully understood without also accessing the referenced document.
        'code' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/document-relationship-type'=>[ 'replaces', 'transforms', 'signs', 'appends' ]
          },
          'type'=>'code',
          'path'=>'RelatesTo.code',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/document-relationship-type'}
        },
        ##
        # Target of the relationship
        # The target composition/document of this relationship.
        'targetIdentifier' => {
          'type'=>'Identifier',
          'path'=>'RelatesTo.target[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Target of the relationship
        # The target composition/document of this relationship.
        'targetReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Composition'],
          'type'=>'Reference',
          'path'=>'RelatesTo.target[x]',
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
      # replaces | transforms | signs | appends
      # The type of relationship that this composition has with anther composition or document.
      # If this document appends another document, then the document cannot be fully understood without also accessing the referenced document.
      attr_accessor :code                           # 1-1 code
      ##
      # Target of the relationship
      # The target composition/document of this relationship.
      attr_accessor :targetIdentifier               # 1-1 Identifier
      ##
      # Target of the relationship
      # The target composition/document of this relationship.
      attr_accessor :targetReference                # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Composition)
    end

    ##
    # The clinical service(s) being documented
    # The clinical service, such as a colonoscopy or an appendectomy, being documented.
    # The event needs to be consistent with the type element, though can provide further information if desired.
    class Event < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Event.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Event.extension',
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
          'path'=>'Event.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Code(s) that apply to the event being documented
        # This list of codes represents the main clinical acts, such as a colonoscopy or an appendectomy, being documented. In some cases, the event is inherent in the typeCode, such as a "History and Physical Report" in which the procedure being documented is necessarily a "History and Physical" act.
        # An event can further specialize the act inherent in the typeCode, such as where it is simply "Procedure Report" and the procedure was a "colonoscopy". If one or more eventCodes are included, they SHALL NOT conflict with the values inherent in the classCode, practiceSettingCode or typeCode, as such a conflict would create an ambiguous situation. This short list of codes is provided to be used as key words for certain types of queries.
        'code' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'ACCTRECEIVABLE', 'CASH', 'CC', 'AE', 'DN', 'DV', 'MC', 'V', 'PBILLACCT', 'CONT', 'DAY', 'LOC', 'MONTH', 'PERIOD', 'PROV', 'WEEK', 'YEAR', 'AA', 'ANF', 'AR', 'AS', 'DISPLAY', 'FORM', 'CPTM', 'HCPCSA', 'BLK', 'CAP', 'CONTF', 'FINBILL', 'ROST', 'SESS', 'FFS', 'FFPS', 'FFCS', 'TFS', 'ROIFS', 'ROIPS', 'CACC', 'CAIC', 'CAMC', 'CANC', 'CAPC', 'CBGC', 'CCCC', 'CCGC', 'CCPC', 'CCSC', 'CDEC', 'CDRC', 'CEMC', 'CFPC', 'CIMC', 'CMGC', 'CNEC', 'CNMC', 'CNQC', 'CNSC', 'COGC', 'COMC', 'COPC', 'COSC', 'COTC', 'CPEC', 'CPGC', 'CPHC', 'CPRC', 'CPSC', 'CPYC', 'CROC', 'CRPC', 'CSUC', 'CTSC', 'CURC', 'CVSC', 'LGPC', 'AALC', 'AAMC', 'ABHC', 'ACAC', 'ACHC', 'AHOC', 'ALTC', 'AOSC', 'CACS', 'CAMI', 'CAST', 'CBAR', 'CCAD', 'CCAR', 'CDEP', 'CDGD', 'CDIA', 'CEPI', 'CFEL', 'CHFC', 'CHRO', 'CHYP', 'CMIH', 'CMSC', 'COJR', 'CONC', 'COPD', 'CORT', 'CPAD', 'CPND', 'CPST', 'CSDM', 'CSIC', 'CSLD', 'CSPT', 'CTBU', 'CVDC', 'CWMA', 'CWOH', 'AMB', 'EMER', 'FLD', 'HH', 'IMP', 'ACUTE', 'NONAC', 'OBSENC', 'PRENC', 'SS', 'VR', 'ALC', 'CARD', 'CHR', 'DNTL', 'DRGRHB', 'GENRL', 'MED', 'OBS', 'ONC', 'PALL', 'PED', 'PHAR', 'PHYRHB', 'PSYCH', 'SURG', 'AUTOATTCH', 'DOCUMENT', 'HEALTHREC', 'IMG', 'LABRESULTS', 'MODEL', 'WIATTCH', 'XRAY', 'ICOL', 'IDSCL', 'INFA', 'INFAO', 'INFASO', 'IRDSCL', 'RESEARCH', 'RSDID', 'RSREID', 'ID', 'IP', 'L', 'M', 'O', 'R', 'X', 'AUTO', 'ENDC', 'REFLEX', 'AUTH', 'NAUTH', 'ELG', 'NELG', 'COVPRD', 'LFEMX', 'NETAMT', 'PRDMX', 'UNITPRICE', 'UNITQTY', 'COVMX', 'EHCPOL', 'HSAPOL', 'AUTOPOL', 'COL', 'UNINSMOT', 'PUBLICPOL', 'DENTPRG', 'DISEASEPRG', 'CANPRG', 'ENDRENAL', 'HIVAIDS', 'MANDPOL', 'MENTPRG', 'SAFNET', 'SUBPRG', 'SUBSIDIZ', 'SUBSIDMC', 'SUBSUPP', 'WCBPOL', 'DENTAL', 'DISEASE', 'DRUGPOL', 'HIP', 'LTC', 'MCPOL', 'POS', 'HMO', 'PPO', 'MENTPOL', 'SUBPOL', 'VISPOL', 'DIS', 'EWB', 'FLEXP', 'LIFE', 'ANNU', 'TLIFE', 'ULIFE', 'PNC', 'REI', 'SURPL', 'UMBRL', 'CHAR', 'CRIME', 'EAP', 'GOVEMP', 'HIRISK', 'IND', 'MILITARY', 'RETIRE', 'SOCIAL', 'VET', 'EMAUTH', '21', '1', '19', '2', '22', '23', '3', '4', '5', '6', '7', '14', '15', '16', '17', '18', '20', '8', '10', '11', '12', '13', '9', 'CHLDCARE', 'CONVEYNC', 'HLTHCARE', 'HOMECARE', 'HOSPPTNT', 'HOSPVSTR', 'HOUSEHLD', 'INMATE', 'INTIMATE', 'LTRMCARE', 'PLACE', 'PTNTCARE', 'SCHOOL2', 'SOCIAL2', 'SUBSTNCE', 'TRAVINT', 'WORK2', 'CHRG', 'REV', 'MVA', 'SCHOOL', 'SPT', 'WPA', 'ACADR', 'ACALL', 'ACALLG', 'ACCONS', 'ACDEMO', 'ACDI', 'ACIMMUN', 'ACLAB', 'ACMED', 'ACMEDC', 'ACMEN', 'ACOBS', 'ACPOLPRG', 'ACPROV', 'ACPSERV', 'ACSUBSTAB', 'INFAUT', 'INFCON', 'INFCRT', 'INFDNG', 'INFEMER', 'INFPWR', 'INFREG', 'ALLCAT', 'ALLGCAT', 'ARCAT', 'COBSCAT', 'DEMOCAT', 'DICAT', 'IMMUCAT', 'LABCAT', 'MEDCCAT', 'MENCAT', 'PSVCCAT', 'RXCAT', 'ALEC', 'BONUS', 'CFWD', 'EDU', 'EPYMT', 'GARN', 'INVOICE', 'PINV', 'PPRD', 'PROA', 'RECOV', 'RETRO', 'TRAN', 'INVTYPE', 'PAYEE', 'PAYOR', 'SENDAPP', 'UNSPSC', 'GTIN', 'UPC', 'COIN', 'COPAYMENT', 'DEDUCTIBLE', 'PAY', 'SPEND', 'COINS', 'AFTHRS', 'ISOL', 'OOO', 'CANCAPT', 'DSC', 'ESA', 'FFSTOP', 'FNLFEE', 'FRSTFEE', 'MARKUP', 'MISSAPT', 'PERFEE', 'PERMBNS', 'RESTOCK', 'TRAVEL', 'URGENT', 'FST', 'HST', 'PST', 'I', 'P', 'S', 'SP', 'W', 'CPNDDRGING', 'CPNDINDING', 'CPNDSUPING', 'DRUGING', 'FRAMEING', 'LENSING', 'PRDING', 'CPINV', 'CSINV', 'CSPINV', 'FININV', 'OHSINV', 'PAINV', 'RXCINV', 'RXDINV', 'SBFINV', 'VRXINV', 'ADNFPPELAT', 'ADNFPPELCT', 'ADNFPPMNAT', 'ADNFPPMNCT', 'ADNFSPELAT', 'ADNFSPELCT', 'ADNFSPMNAT', 'ADNFSPMNCT', 'ADNPPPELAT', 'ADNPPPELCT', 'ADNPPPMNAT', 'ADNPPPMNCT', 'ADNPSPELAT', 'ADNPSPELCT', 'ADNPSPMNAT', 'ADNPSPMNCT', 'ADPPPPELAT', 'ADPPPPELCT', 'ADPPPPMNAT', 'ADPPPPMNCT', 'ADPPSPELAT', 'ADPPSPELCT', 'ADPPSPMNAT', 'ADPPSPMNCT', 'ADRFPPELAT', 'ADRFPPELCT', 'ADRFPPMNAT', 'ADRFPPMNCT', 'ADRFSPELAT', 'ADRFSPELCT', 'ADRFSPMNAT', 'ADRFSPMNCT', 'PDNFPPELAT', 'PDNFPPELCT', 'PDNFPPMNAT', 'PDNFPPMNCT', 'PDNFSPELAT', 'PDNFSPELCT', 'PDNFSPMNAT', 'PDNFSPMNCT', 'PDNPPPELAT', 'PDNPPPELCT', 'PDNPPPMNAT', 'PDNPPPMNCT', 'PDNPSPELAT', 'PDNPSPELCT', 'PDNPSPMNAT', 'PDNPSPMNCT', 'PDPPPPELAT', 'PDPPPPELCT', 'PDPPPPMNAT', 'PDPPPPMNCT', 'PDPPSPELAT', 'PDPPSPELCT', 'PDPPSPMNAT', 'PDPPSPMNCT', 'SBBLELAT', 'SBBLELCT', 'SBNFELAT', 'SBNFELCT', 'SBPDELAT', 'SBPDELCT', 'COVGE', 'EFORM', 'FAX', 'GFTH', 'LATE', 'MANUAL', 'OOJ', 'ORTHO', 'PAPER', 'PIE', 'PYRDELAY', 'REFNR', 'REPSERV', 'UNRELAT', 'VERBAUTH', 'CARELIST', 'CONDLIST', 'INTOLIST', 'PROBLIST', 'RISKLIST', 'GOALLIST', 'ACU', 'CHRON', 'ONET', 'PRN', 'MEDLIST', 'CURMEDLIST', 'DISCMEDLIST', 'HISTMEDLIST', 'CTLSUB', 'INV', 'LU', 'OTC', 'RX', 'SA', 'SAC', 'IND01', 'IND02', 'IND03', 'IND04', 'IND05', 'VFPAPER', 'ACH', 'CHK', 'DDP', 'NON', 'DF', 'EM', 'SO', 'FF', 'FFC', 'FFP', 'FFSS', 'TF', 'FS', 'MS', 'RF', 'UD', 'RFC', 'RFCS', 'RFF', 'RFFS', 'RFP', 'RFPS', 'RFS', 'TB', 'TBS', 'UDE', 'EMRGONLY', 'GRANTORCHOICE', 'IMPLIED', 'IMPLIEDD', 'NOCONSENT', 'NOPP', 'OPTIN', 'OPTINR', 'OPTOUT', 'OPTOUTE', '_ActUSPrivacyLaw', '42CFRPart2', 'CommonRule', 'HIPAANOPP', 'HIPAAPsyNotes', 'HIPAASelfPay', 'Title38Section7332', 'ETH', 'GDIS', 'HIV', 'MST', 'SCA', 'SDV', 'SEX', 'SPI', 'BH', 'COGN', 'DVD', 'EMOTDIS', 'MH', 'PSY', 'PSYTHPN', 'SUD', 'ETHUD', 'OPIOIDUD', 'STD', 'TBOO', 'VIO', 'SICKLE', 'DEMO', 'DOB', 'GENDER', 'LIVARG', 'MARST', 'RACE', 'REL', 'B', 'EMPL', 'LOCIS', 'SSP', 'ADOL', 'CEL', 'DIA', 'DRGIS', 'EMP', 'PDS', 'PHY', 'PRS', 'COMPT', 'ACOCOMPT', 'CTCOMPT', 'FMCOMPT', 'HRCOMPT', 'LRCOMPT', 'PACOMPT', 'RESCOMPT', 'RMGTCOMPT', 'TRSTACCRD', 'TRSTAGRE', 'TRSTASSUR', 'TRSTCERT', 'TRSTFWK', 'TRSTMEC', 'COVPOL', 'SecurityPolicy', 'AUTHPOL', 'ACCESSCONSCHEME', 'DELEPOL', 'ObligationPolicy', 'ANONY', 'AOD', 'AUDIT', 'AUDTR', 'CPLYCC', 'CPLYCD', 'CPLYJPP', 'CPLYOPP', 'CPLYOSP', 'CPLYPOL', 'DECLASSIFYLABEL', 'DEID', 'DELAU', 'DOWNGRDLABEL', 'DRIVLABEL', 'ENCRYPT', 'ENCRYPTR', 'ENCRYPTT', 'ENCRYPTU', 'HUAPRV', 'LABEL', 'MASK', 'MINEC', 'PERSISTLABEL', 'PRIVMARK', 'PSEUD', 'REDACT', 'UPGRDLABEL', 'RefrainPolicy', 'NOAUTH', 'NOCOLLECT', 'NODSCLCD', 'NODSCLCDS', 'NOINTEGRATE', 'NOLIST', 'NOMOU', 'NOORGPOL', 'NOPAT', 'NOPERSISTP', 'NORDSCLCD', 'NORDSCLCDS', 'NORDSCLW', 'NORELINK', 'NOREUSE', 'NOVIP', 'ORCON', 'LOAN', 'RENT', 'TRANSFER', 'SALE', 'SREC', 'SSTOR', 'STRAN', 'ACID', 'ALK', 'DEFB', 'FILT', 'LDLP', 'NEUT', 'RECA', 'UFIL', 'DRUG', 'FD', 'IMMUNIZ', 'BOOSTER', 'INITIMMUNIZ', 'OE', 'LABOE', 'MEDOE', 'PATDOC', 'ALLERLREV', 'CLINNOTEE', 'DIAGLISTE', 'DISCHINSTE', 'DISCHSUME', 'PATEDUE', 'PATREPE', 'PROBLISTE', 'RADREPE', 'IMMLREV', 'REMLREV', 'WELLREMLREV', 'PATINFO', 'ALLERLE', 'CDSREV', 'CLINNOTEREV', 'DISCHSUMREV', 'DIAGLISTREV', 'IMMLE', 'LABRREV', 'MICRORREV', 'MICROORGRREV', 'MICROSENSRREV', 'MLREV', 'MARWLREV', 'OREV', 'PATREPREV', 'PROBLISTREV', 'RADREPREV', 'REMLE', 'WELLREMLE', 'RISKASSESS', 'FALLRISK', 'AFOOT', 'AMBT', 'AMBAIR', 'AMBGRND', 'AMBHELO', 'LAWENF', 'PRVTRN', 'PUBTRN', 'ARTBLD', 'DILUTION', 'AUTO-HIGH', 'AUTO-LOW', 'PRE', 'RERUN', 'EVNFCTS', 'INTFR', 'FIBRIN', 'HEMOLYSIS', 'ICTERUS', 'LIPEMIA', 'VOLUME', 'AVAILABLE', 'CONSUMPTION', 'CURRENT', 'INITIAL', '_ActPatientAnnotationType', 'ANNDI', 'ANNGEN', 'ANNIMM', 'ANNLAB', 'ANNMED', 'GENE', '_ImmunizationObservationType', 'OBSANTC', 'OBSANTV', 'PAT_ADV_EVNT', 'VAC_PROBLEM', '21611-9', '21612-7', '29553-5', '30525-0', '30972-4', 'REP_HALF_LIFE', 'SPLCOATING', 'SPLCOLOR', 'SPLIMAGE', 'SPLIMPRINT', 'SPLSCORING', 'SPLSHAPE', 'SPLSIZE', 'SPLSYMBOL', 'AIRTRNS', 'ANANTRNS', 'ANHUMTRNS', 'BDYFLDTRNS', 'BLDTRNS', 'DERMTRNS', 'ENVTRNS', 'FECTRNS', 'FOMTRNS', 'FOODTRNS', 'HUMHUMTRNS', 'INDTRNS', 'LACTTRNS', 'NOSTRNS', 'PARTRNS', 'PLACTRNS', 'SEXTRNS', 'TRNSFTRNS', 'VECTRNS', 'WATTRNS', '_ObservationQualityMeasureAttribute', 'AGGREGATE', 'CMPMSRMTH', 'CMPMSRSCRWGHT', 'COPY', 'CRS', 'DEF', 'DISC', 'FINALDT', 'GUIDE', 'IDUR', 'ITMCNT', 'KEY', 'MEDT', 'MSD', 'MSRADJ', 'MSRAGG', 'MSRIMPROV', 'MSRJUR', 'MSRRPTR', 'MSRRPTTIME', 'MSRSCORE', 'MSRSET', 'MSRTOPIC', 'MSRTP', 'MSRTYPE', 'RAT', 'REF', 'SDE', 'STRAT', 'TRANF', 'USE', 'TIME_ABSOLUTE', 'TIME_RELATIVE', 'REPRESENTATIVE_BEAT', 'RHYTHM', '_PatientImmunizationRelatedObservationType', 'CLSSRM', 'GRADE', 'SCHL', 'SCHLDIV', 'TEACHER', 'DENEX', 'DENEXCEP', 'DENOM', 'IPOP', 'IPPOP', 'MSROBS', 'MSRPOPL', 'MSRPOPLEX', 'NUMER', 'NUMEX', 'PREFSTRENGTH', 'ADVERSE_REACTION', 'ASSERTION', 'CASESER', 'CDIO', 'CRIT', 'CTMO', 'DX', 'ADMDX', 'DISDX', 'INTDX', 'NOI', 'GISTIER', 'HHOBS', 'ISSUE', 'NAT', 'SUPPRESSED', 'VALIDAT', 'KEY204', 'KEY205', 'COMPLY', 'DUPTHPY', 'DUPTHPCLS', 'DUPTHPGEN', 'ABUSE', 'FRAUD', 'PLYDOC', 'PLYPHRM', 'DOSE', 'DOSECOND', 'DOSEDUR', 'DOSEDURH', 'DOSEDURHIND', 'DOSEDURL', 'DOSEDURLIND', 'DOSEH', 'DOSEHINDA', 'DOSEHIND', 'DOSEHINDSA', 'DOSEHINDW', 'DOSEIVL', 'DOSEIVLIND', 'DOSEL', 'DOSELINDA', 'DOSELIND', 'DOSELINDSA', 'DOSELINDW', 'MDOSE', 'OBSA', 'AGE', 'ADALRT', 'GEALRT', 'PEALRT', 'COND', 'HGHT', 'LACT', 'PREG', 'WGHT', 'CREACT', 'GEN', 'GEND', 'LAB', 'REACT', 'ALGY', 'INT', 'RREACT', 'RALG', 'RAR', 'RINT', 'BUS', 'CODE_INVAL', 'CODE_DEPREC', 'FORMAT', 'ILLEGAL', 'LEN_RANGE', 'LEN_LONG', 'LEN_SHORT', 'MISSCOND', 'MISSMAND', 'NODUPS', 'NOPERSIST', 'REP_RANGE', 'MAXOCCURS', 'MINOCCURS', 'KEY206', 'OBSOLETE', 'FOOD', 'TPROD', 'DRG', 'NHP', 'NONRX', 'PREVINEF', 'DACT', 'TIME', 'ALRTENDLATE', 'ALRTSTRTLATE', 'ENDLATE', 'STRTLATE', 'ALLDONE', 'FULFIL', 'NOTACTN', 'NOTEQUIV', 'NOTEQUIVGEN', 'NOTEQUIVTHER', 'TIMING', 'INTERVAL', 'MINFREQ', 'HELD', 'TOOLATE', 'TOOSOON', 'HISTORIC', 'PATPREF', 'PATPREFALT', 'KSUBJ', 'KSUBT', 'OINT', 'ALG', 'DALG', 'EALG', 'FALG', 'DINT', 'DNAINT', 'EINT', 'ENAINT', 'FINT', 'FNAINT', 'NAINT', 'SEV', 'FDACOATING', 'FDACOLOR', 'FDAIMPRINTCD', 'FDALOGO', 'FDASCORING', 'FDASHAPE', 'FDASIZE', 'CIRCLE', 'ELLIPSE', 'POINT', 'POLY', 'C', 'DIET', 'BR', 'DM', 'FAST', 'FORMULA', 'GF', 'LF', 'LP', 'LQ', 'LS', 'N', 'NF', 'PAF', 'PAR', 'RD', 'SCH', 'SUPPLEMENT', 'T', 'VLI', 'DRUGPRG', 'F', 'PRLMN', 'SECCATOBS', 'SECCLASSOBS', 'SECCONOBS', 'SECINTOBS', 'SECALTINTOBS', 'SECDATINTOBS', 'SECINTCONOBS', 'SECINTPRVABOBS', 'SECINTPRVRBOBS', 'SECINTSTOBS', 'TRSTACCRDOBS', 'TRSTAGREOBS', 'TRSTCERTOBS', 'TRSTFWKOBS', 'TRSTLOAOBS', 'TRSTMECOBS', 'SUBSIDFFS', 'WRKCOMP', 'COPAY', 'DEDUCT', 'DOSEIND', 'PRA' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Event.code',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActCode'}
        },
        ##
        # The period covered by the documentation
        # The period of time covered by the documentation. There is no assertion that the documentation is a complete representation for this period, only that it documents events during this time.
        'period' => {
          'type'=>'Period',
          'path'=>'Event.period',
          'min'=>0,
          'max'=>1
        },
        ##
        # The event(s) being documented
        # The description and/or reference of the event(s) being documented. For example, this could be used to document such a colonoscopy or an appendectomy.
        'detail' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Event.detail',
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
      # Code(s) that apply to the event being documented
      # This list of codes represents the main clinical acts, such as a colonoscopy or an appendectomy, being documented. In some cases, the event is inherent in the typeCode, such as a "History and Physical Report" in which the procedure being documented is necessarily a "History and Physical" act.
      # An event can further specialize the act inherent in the typeCode, such as where it is simply "Procedure Report" and the procedure was a "colonoscopy". If one or more eventCodes are included, they SHALL NOT conflict with the values inherent in the classCode, practiceSettingCode or typeCode, as such a conflict would create an ambiguous situation. This short list of codes is provided to be used as key words for certain types of queries.
      attr_accessor :code                           # 0-* [ CodeableConcept ]
      ##
      # The period covered by the documentation
      # The period of time covered by the documentation. There is no assertion that the documentation is a complete representation for this period, only that it documents events during this time.
      attr_accessor :period                         # 0-1 Period
      ##
      # The event(s) being documented
      # The description and/or reference of the event(s) being documented. For example, this could be used to document such a colonoscopy or an appendectomy.
      attr_accessor :detail                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    end

    ##
    # Composition is broken into sections
    # The root of the sections that make up the composition.
    class Section < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Section.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Section.extension',
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
          'path'=>'Section.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Label for section (e.g. for ToC)
        # The label for this particular section.  This will be part of the rendered content for the document, and is often used to build a table of contents.
        # The title identifies the section for a human reader. The title must be consistent with the narrative of the resource that is the target of the section.content reference. Generally, sections SHOULD have titles, but in some documents, it is unnecessary or inappropriate. Typically, this is where a section has subsections that have their own adequately distinguishing title,  or documents that only have a single section. Most Implementation Guides will make section title to be a required element.
        'title' => {
          'type'=>'string',
          'path'=>'Section.title',
          'min'=>0,
          'max'=>1
        },
        ##
        # Classification of section (recommended)
        # A code identifying the kind of content contained within the section. This must be consistent with the section title.
        # The code identifies the section for an automated processor of the document. This is particularly relevant when using profiles to control the structure of the document.   
        # 
        # If the section has content (instead of sub-sections), the section.code does not change the meaning or interpretation of the resource that is the content of the section in the comments for the section.code.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Section.code',
          'min'=>0,
          'max'=>1
        },
        ##
        # Who and/or what authored the section
        # Identifies who is responsible for the information in this section, not necessarily who typed it in.
        'author' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Section.author',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Who/what the section is about, when it is not about the subject of composition
        # The actual focus of the section when it is not the subject of the composition, but instead represents something or someone associated with the subject such as (for a patient subject) a spouse, parent, fetus, or donor. If not focus is specified, the focus is assumed to be focus of the parent section, or, for a section in the Composition itself, the subject of the composition. Sections with a focus SHALL only include resources where the logical subject (patient, subject, focus, etc.) matches the section focus, or the resources have no logical subject (few resources).
        # Typically, sections in a doument are about the subject of the document, whether that is a  patient, or group of patients, location, or device, or whatever. For some kind of documents, some sections actually contain data about related entities. Typical examples are  a section in a newborn discharge summary concerning the mother, or family history documents, with a section about each family member, though there are many other examples.
        'focus' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Section.focus',
          'min'=>0,
          'max'=>1
        },
        ##
        # Text summary of the section, for human interpretation
        # A human-readable narrative that contains the attested content of the section, used to represent the content of the resource to a human. The narrative need not encode all the structured data, but is required to contain sufficient detail to make it "clinically safe" for a human to just read the narrative.
        # Document profiles may define what content should be represented in the narrative to ensure clinical safety.
        'text' => {
          'type'=>'Narrative',
          'path'=>'Section.text',
          'min'=>0,
          'max'=>1
        },
        ##
        # working | snapshot | changes
        # How the entry list was prepared - whether it is a working list that is suitable for being maintained on an ongoing basis, or if it represents a snapshot of a list of items from another source, or whether it is a prepared list where items may be marked as added, modified or deleted.
        # This element is labeled as a modifier because a change list must not be misunderstood as a complete list.
        'mode' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/list-mode'=>[ 'working', 'snapshot', 'changes' ]
          },
          'type'=>'code',
          'path'=>'Section.mode',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/list-mode'}
        },
        ##
        # Order of section entries
        # Specifies the order applied to the items in the section entries.
        # Applications SHOULD render ordered lists in the order provided, but MAY allow users to re-order based on their own preferences as well. If there is no order specified, the order is unknown, though there may still be some order.
        'orderedBy' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/list-order'=>[ 'user', 'system', 'event-date', 'entry-date', 'priority', 'alphabetic', 'category', 'patient' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Section.orderedBy',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/list-order'}
        },
        ##
        # A reference to data that supports this section
        # A reference to the actual resource from which the narrative in the section is derived.
        # If there are no entries in the list, an emptyReason SHOULD be provided.
        'entry' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Section.entry',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Why the section is empty
        # If the section is empty, why the list is empty. An empty section typically has some text explaining the empty reason.
        # The various reasons for an empty section make a significant interpretation to its interpretation. Note that this code is for use when the entire section content has been suppressed, and not for when individual items are omitted - implementers may consider using a text note or a flag on an entry in these cases.
        'emptyReason' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/list-empty-reason'=>[ 'nilknown', 'notasked', 'withheld', 'unavailable', 'notstarted', 'closed' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Section.emptyReason',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/list-empty-reason'}
        },
        ##
        # Nested Section
        # A nested sub-section within this section.
        # Nested sections are primarily used to help human readers navigate to particular portions of the document.
        'section' => {
          'type'=>'Composition::Section',
          'path'=>'Section.section',
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
      # Label for section (e.g. for ToC)
      # The label for this particular section.  This will be part of the rendered content for the document, and is often used to build a table of contents.
      # The title identifies the section for a human reader. The title must be consistent with the narrative of the resource that is the target of the section.content reference. Generally, sections SHOULD have titles, but in some documents, it is unnecessary or inappropriate. Typically, this is where a section has subsections that have their own adequately distinguishing title,  or documents that only have a single section. Most Implementation Guides will make section title to be a required element.
      attr_accessor :title                          # 0-1 string
      ##
      # Classification of section (recommended)
      # A code identifying the kind of content contained within the section. This must be consistent with the section title.
      # The code identifies the section for an automated processor of the document. This is particularly relevant when using profiles to control the structure of the document.   
      # 
      # If the section has content (instead of sub-sections), the section.code does not change the meaning or interpretation of the resource that is the content of the section in the comments for the section.code.
      attr_accessor :code                           # 0-1 CodeableConcept
      ##
      # Who and/or what authored the section
      # Identifies who is responsible for the information in this section, not necessarily who typed it in.
      attr_accessor :author                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Organization) ]
      ##
      # Who/what the section is about, when it is not about the subject of composition
      # The actual focus of the section when it is not the subject of the composition, but instead represents something or someone associated with the subject such as (for a patient subject) a spouse, parent, fetus, or donor. If not focus is specified, the focus is assumed to be focus of the parent section, or, for a section in the Composition itself, the subject of the composition. Sections with a focus SHALL only include resources where the logical subject (patient, subject, focus, etc.) matches the section focus, or the resources have no logical subject (few resources).
      # Typically, sections in a doument are about the subject of the document, whether that is a  patient, or group of patients, location, or device, or whatever. For some kind of documents, some sections actually contain data about related entities. Typical examples are  a section in a newborn discharge summary concerning the mother, or family history documents, with a section about each family member, though there are many other examples.
      attr_accessor :focus                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
      ##
      # Text summary of the section, for human interpretation
      # A human-readable narrative that contains the attested content of the section, used to represent the content of the resource to a human. The narrative need not encode all the structured data, but is required to contain sufficient detail to make it "clinically safe" for a human to just read the narrative.
      # Document profiles may define what content should be represented in the narrative to ensure clinical safety.
      attr_accessor :text                           # 0-1 Narrative
      ##
      # working | snapshot | changes
      # How the entry list was prepared - whether it is a working list that is suitable for being maintained on an ongoing basis, or if it represents a snapshot of a list of items from another source, or whether it is a prepared list where items may be marked as added, modified or deleted.
      # This element is labeled as a modifier because a change list must not be misunderstood as a complete list.
      attr_accessor :mode                           # 0-1 code
      ##
      # Order of section entries
      # Specifies the order applied to the items in the section entries.
      # Applications SHOULD render ordered lists in the order provided, but MAY allow users to re-order based on their own preferences as well. If there is no order specified, the order is unknown, though there may still be some order.
      attr_accessor :orderedBy                      # 0-1 CodeableConcept
      ##
      # A reference to data that supports this section
      # A reference to the actual resource from which the narrative in the section is derived.
      # If there are no entries in the list, an emptyReason SHOULD be provided.
      attr_accessor :entry                          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
      ##
      # Why the section is empty
      # If the section is empty, why the list is empty. An empty section typically has some text explaining the empty reason.
      # The various reasons for an empty section make a significant interpretation to its interpretation. Note that this code is for use when the entire section content has been suppressed, and not for when individual items are omitted - implementers may consider using a text note or a flag on an entry in these cases.
      attr_accessor :emptyReason                    # 0-1 CodeableConcept
      ##
      # Nested Section
      # A nested sub-section within this section.
      # Nested sections are primarily used to help human readers navigate to particular portions of the document.
      attr_accessor :section                        # 0-* [ Composition::Section ]
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
    # Version-independent identifier for the Composition
    # A version-independent identifier for the Composition. This identifier stays constant as the composition is changed over time.
    # Similar to ClinicalDocument/setId in CDA. See discussion in resource definition for how these relate.
    attr_accessor :identifier                     # 0-1 Identifier
    ##
    # preliminary | final | amended | entered-in-error
    # The workflow/clinical status of this composition. The status is a marker for the clinical standing of the document.
    # If a composition is marked as withdrawn, the compositions/documents in the series, or data from the composition or document series, should never be displayed to a user without being clearly marked as untrustworthy. The flag "entered-in-error" is why this element is labeled as a modifier of other elements.   
    # 
    # Some reporting work flows require that the original narrative of a final document never be altered; instead, only new narrative can be added. The composition resource has no explicit status for explicitly noting whether this business rule is in effect. This would be handled by an extension if required.
    attr_accessor :status                         # 1-1 code
    ##
    # Kind of composition (LOINC if possible)
    # Specifies the particular kind of composition (e.g. History and Physical, Discharge Summary, Progress Note). This usually equates to the purpose of making the composition.
    # For Composition type, LOINC is ubiquitous and strongly endorsed by HL7. Most implementation guides will require a specific LOINC code, or use LOINC as an extensible binding.
    attr_accessor :type                           # 1-1 CodeableConcept
    ##
    # Categorization of Composition
    # A categorization for the type of the composition - helps for indexing and searching. This may be implied by or derived from the code specified in the Composition Type.
    # This is a metadata field from [XDS/MHD](http://wiki.ihe.net/index.php?title=Mobile_access_to_Health_Documents_(MHD)).
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # Who and/or what the composition is about
    # Who or what the composition is about. The composition can be about a person, (patient or healthcare practitioner), a device (e.g. a machine) or even a group of subjects (such as a document about a herd of livestock, or a set of patients that share a common exposure).
    # For clinical documents, this is usually the patient.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
    ##
    # Context of the Composition
    # Describes the clinical encounter or type of care this documentation is associated with.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Composition editing time
    # The composition editing time, when the composition was last logically changed by the author.
    # The Last Modified Date on the composition may be after the date of the document was attested without being changed.
    attr_accessor :date                           # 1-1 dateTime
    ##
    # Who and/or what authored the composition
    # Identifies who is responsible for the information in the composition, not necessarily who typed it in.
    attr_accessor :author                         # 1-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Organization) ]
    ##
    # Human Readable name/title
    # Official human-readable label for the composition.
    # For many compositions, the title is the same as the text or a display name of Composition.type (e.g. a "consultation" or "progress note"). Note that CDA does not make title mandatory, but there are no known cases where it is useful for title to be omitted, so it is mandatory here. Feedback on this requirement is welcome during the trial use period.
    attr_accessor :title                          # 1-1 string
    ##
    # As defined by affinity domain
    # The code specifying the level of confidentiality of the Composition.
    # The exact use of this element, and enforcement and issues related to highly sensitive documents are out of scope for the base specification, and delegated to implementation profiles (see security section).  This element is labeled as a modifier because highly confidential documents must not be treated as if they are not.
    attr_accessor :confidentiality                # 0-1 code
    ##
    # Attests to accuracy of composition
    # A participant who has attested to the accuracy of the composition/document.
    # Only list each attester once.
    attr_accessor :attester                       # 0-* [ Composition::Attester ]
    ##
    # Organization which maintains the composition
    # Identifies the organization or group who is responsible for ongoing maintenance of and access to the composition/document information.
    # This is useful when documents are derived from a composition - provides guidance for how to get the latest version of the document. This is optional because this is sometimes not known by the authoring system, and can be inferred by context. However, it is important that this information be known when working with a derived document, so providing a custodian is encouraged.
    attr_accessor :custodian                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Relationships to other compositions/documents
    # Relationships that this composition has with other compositions or documents that already exist.
    # A document is a version specific composition.
    attr_accessor :relatesTo                      # 0-* [ Composition::RelatesTo ]
    ##
    # The clinical service(s) being documented
    # The clinical service, such as a colonoscopy or an appendectomy, being documented.
    # The event needs to be consistent with the type element, though can provide further information if desired.
    attr_accessor :event                          # 0-* [ Composition::Event ]
    ##
    # Composition is broken into sections
    # The root of the sections that make up the composition.
    attr_accessor :section                        # 0-* [ Composition::Section ]

    def resourceType
      'Composition'
    end
  end
end
