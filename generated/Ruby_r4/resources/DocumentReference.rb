module FHIR

  ##
  # A reference to a document of any kind for any purpose. Provides metadata about the document so that the document can be discovered and managed. The scope of a document is any seralized object with a mime-type, so includes formal patient centric documents (CDA), cliical notes, scanned paper, and non-patient specific documents like policy text.
  class DocumentReference < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['authenticator', 'author', 'category', 'contenttype', 'custodian', 'date', 'description', 'encounter', 'event', 'facility', 'format', 'identifier', 'language', 'location', 'patient', 'period', 'related', 'relatesto', 'relation', 'relationship', 'security-label', 'setting', 'status', 'subject', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'DocumentReference.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'DocumentReference.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'DocumentReference.implicitRules',
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
        'path'=>'DocumentReference.language',
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
        'path'=>'DocumentReference.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'DocumentReference.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'DocumentReference.extension',
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
        'path'=>'DocumentReference.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Master Version Specific Identifier
      # Document identifier as assigned by the source of the document. This identifier is specific to this version of the document. This unique identifier may be used elsewhere to identify this version of the document.
      # CDA Document Id extension and root.
      'masterIdentifier' => {
        'type'=>'Identifier',
        'path'=>'DocumentReference.masterIdentifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # Other identifiers for the document
      # Other identifiers associated with the document, including version independent identifiers.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'DocumentReference.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # current | superseded | entered-in-error
      # The status of this document reference.
      # This is the status of the DocumentReference object, which might be independent from the docStatus element.
      # 
      # This element is labeled as a modifier because the status contains the codes that mark the document or reference as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/document-reference-status'=>[ 'current', 'superseded', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'DocumentReference.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/document-reference-status'}
      },
      ##
      # preliminary | final | amended | entered-in-error
      # The status of the underlying document.
      # The document that is pointed to might be in various lifecycle states.
      'docStatus' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/composition-status'=>[ 'preliminary', 'final', 'amended', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'DocumentReference.docStatus',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/composition-status'}
      },
      ##
      # Kind of document (LOINC if possible)
      # Specifies the particular kind of document referenced  (e.g. History and Physical, Discharge Summary, Progress Note). This usually equates to the purpose of making the document referenced.
      # Key metadata element describing the document that describes he exact type of document. Helps humans to assess whether the document is of interest when viewing a list of documents.
      'type' => {
        'type'=>'CodeableConcept',
        'path'=>'DocumentReference.type',
        'min'=>0,
        'max'=>1
      },
      ##
      # Categorization of document
      # A categorization for the type of document referenced - helps for indexing and searching. This may be implied by or derived from the code specified in the DocumentReference.type.
      # Key metadata element describing the the category or classification of the document. This is a broader perspective that groups similar documents based on how they would be used. This is a primary key used in searching.
      'category' => {
        'type'=>'CodeableConcept',
        'path'=>'DocumentReference.category',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who/what is the subject of the document
      # Who or what the document is about. The document can be about a person, (patient or healthcare practitioner), a device (e.g. a machine) or even a group of subjects (such as a document about a herd of farm animals, or a set of patients that share a common exposure).
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'DocumentReference.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # When this document reference was created
      # When the document reference was created.
      # Referencing/indexing time is used for tracking, organizing versions and searching.
      'date' => {
        'type'=>'instant',
        'path'=>'DocumentReference.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who and/or what authored the document
      # Identifies who is responsible for adding the information to the document.
      # Not necessarily who did the actual data entry (i.e. typist) or who was the source (informant).
      'author' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'DocumentReference.author',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Who/what authenticated the document
      # Which person or organization authenticates that this document is valid.
      # Represents a participant within the author institution who has legally authenticated or attested the document. Legal authentication implies that a document has been signed manually or electronically by the legal Authenticator.
      'authenticator' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'DocumentReference.authenticator',
        'min'=>0,
        'max'=>1
      },
      ##
      # Organization which maintains the document
      # Identifies the organization or group who is responsible for ongoing maintenance of and access to the document.
      # Identifies the logical organization (software system, vendor, or department) to go to find the current version, where to report issues, etc. This is different from the physical location (URL, disk drive, or server) of the document, which is the technical location of the document, which host may be delegated to the management of some other organization.
      'custodian' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'DocumentReference.custodian',
        'min'=>0,
        'max'=>1
      },
      ##
      # Relationships to other documents
      # Relationships that this document has with other document references that already exist.
      # This element is labeled as a modifier because documents that append to other documents are incomplete on their own.
      'relatesTo' => {
        'type'=>'DocumentReference::RelatesTo',
        'path'=>'DocumentReference.relatesTo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Human-readable description of the source document.
      # What the document is about,  a terse summary of the document.
      'description' => {
        'type'=>'string',
        'path'=>'DocumentReference.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # Document security-tags
      # A set of Security-Tag codes specifying the level of privacy/security of the Document. Note that DocumentReference.meta.security contains the security labels of the "reference" to the document, while DocumentReference.securityLabel contains a snapshot of the security labels on the document the reference refers to.
      # The confidentiality codes can carry multiple vocabulary items. HL7 has developed an understanding of security and privacy tags that might be desirable in a Document Sharing environment, called HL7 Healthcare Privacy and Security Classification System (HCS). The following specification is recommended but not mandated, as the vocabulary bindings are an administrative domain responsibility. The use of this method is up to the policy domain such as the XDS Affinity Domain or other Trust Domain where all parties including sender and recipients are trusted to appropriately tag and enforce.   
      # 
      # In the HL7 Healthcare Privacy and Security Classification (HCS) there are code systems specific to Confidentiality, Sensitivity, Integrity, and Handling Caveats. Some values would come from a local vocabulary as they are related to workflow roles and special projects.
      'securityLabel' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'ETH', 'GDIS', 'HIV', 'MST', 'SCA', 'SDV', 'SEX', 'SPI', 'BH', 'COGN', 'DVD', 'EMOTDIS', 'MH', 'PSY', 'PSYTHPN', 'SUD', 'ETHUD', 'OPIOIDUD', 'STD', 'TBOO', 'VIO', 'SICKLE', 'DEMO', 'DOB', 'GENDER', 'LIVARG', 'MARST', 'RACE', 'REL', 'B', 'EMPL', 'LOCIS', 'SSP', 'ADOL', 'CEL', 'DIA', 'DRGIS', 'EMP', 'PDS', 'PHY', 'PRS', 'COMPT', 'ACOCOMPT', 'CTCOMPT', 'FMCOMPT', 'HRCOMPT', 'LRCOMPT', 'PACOMPT', 'RESCOMPT', 'RMGTCOMPT', 'SecurityPolicy', 'AUTHPOL', 'ACCESSCONSCHEME', 'DELEPOL', 'ObligationPolicy', 'ANONY', 'AOD', 'AUDIT', 'AUDTR', 'CPLYCC', 'CPLYCD', 'CPLYJPP', 'CPLYOPP', 'CPLYOSP', 'CPLYPOL', 'DECLASSIFYLABEL', 'DEID', 'DELAU', 'DOWNGRDLABEL', 'DRIVLABEL', 'ENCRYPT', 'ENCRYPTR', 'ENCRYPTT', 'ENCRYPTU', 'HUAPRV', 'LABEL', 'MASK', 'MINEC', 'PERSISTLABEL', 'PRIVMARK', 'PSEUD', 'REDACT', 'UPGRDLABEL', 'RefrainPolicy', 'NOAUTH', 'NOCOLLECT', 'NODSCLCD', 'NODSCLCDS', 'NOINTEGRATE', 'NOLIST', 'NOMOU', 'NOORGPOL', 'NOPAT', 'NOPERSISTP', 'NORDSCLCD', 'NORDSCLCDS', 'NORDSCLW', 'NORELINK', 'NOREUSE', 'NOVIP', 'ORCON' ],
          'http://terminology.hl7.org/CodeSystem/v3-ActReason'=>[ 'PurposeOfUse', 'HMARKT', 'HOPERAT', 'CAREMGT', 'DONAT', 'FRAUD', 'GOV', 'HACCRED', 'HCOMPL', 'HDECD', 'HDIRECT', 'HDM', 'HLEGAL', 'HOUTCOMS', 'HPRGRP', 'HQUALIMP', 'HSYSADMIN', 'LABELING', 'METAMGT', 'MEMADMIN', 'MILCDM', 'PATADMIN', 'PATSFTY', 'PERFMSR', 'RECORDMGT', 'SYSDEV', 'HTEST', 'TRAIN', 'HPAYMT', 'CLMATTCH', 'COVAUTH', 'COVERAGE', 'ELIGDTRM', 'ELIGVER', 'ENROLLM', 'MILDCRG', 'REMITADV', 'HRESCH', 'BIORCH', 'CLINTRCH', 'CLINTRCHNPC', 'CLINTRCHPC', 'PRECLINTRCH', 'DSRCH', 'POARCH', 'TRANSRCH', 'PATRQT', 'FAMRQT', 'PWATRNY', 'SUPNWK', 'PUBHLTH', 'DISASTER', 'THREAT', 'TREAT', 'CLINTRL', 'COC', 'ETREAT', 'BTG', 'ERTREAT', 'POPHLTH' ],
          'http://terminology.hl7.org/CodeSystem/v3-Confidentiality'=>[ 'U', 'L', 'M', 'N', 'R', 'V' ],
          'http://terminology.hl7.org/CodeSystem/v3-ObservationValue'=>[ 'ABSTRED', 'AGGRED', 'ANONYED', 'MAPPED', 'MASKED', 'PSEUDED', 'REDACTED', 'SUBSETTED', 'SYNTAC', 'TRSLT', 'VERSIONED', 'CRYTOHASH', 'DIGSIG', 'HRELIABLE', 'RELIABLE', 'UNCERTREL', 'UNRELIABLE', 'CLINAST', 'DEVAST', 'HCPAST', 'PACQAST', 'PATAST', 'PAYAST', 'PROAST', 'SDMAST', 'CLINRPT', 'DEVRPT', 'HCPRPT', 'PACQRPT', 'PATRPT', 'PAYRPT', 'PRORPT', 'SDMRPT' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'DocumentReference.securityLabel',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/security-labels'}
      },
      ##
      # Document referenced
      # The document and format referenced. There may be multiple content element repetitions, each with a different format.
      'content' => {
        'type'=>'DocumentReference::Content',
        'path'=>'DocumentReference.content',
        'min'=>1,
        'max'=>Float::INFINITY
      },
      ##
      # Clinical context of document
      # The clinical context in which the document was prepared.
      # These values are primarily added to help with searching for interesting/relevant documents.
      'context' => {
        'type'=>'DocumentReference::Context',
        'path'=>'DocumentReference.context',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Relationships to other documents
    # Relationships that this document has with other document references that already exist.
    # This element is labeled as a modifier because documents that append to other documents are incomplete on their own.
    class RelatesTo < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
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
        # The type of relationship that this document has with anther document.
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
        # The target document of this relationship.
        'target' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference'],
          'type'=>'Reference',
          'path'=>'RelatesTo.target',
          'min'=>1,
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
      # replaces | transforms | signs | appends
      # The type of relationship that this document has with anther document.
      # If this document appends another document, then the document cannot be fully understood without also accessing the referenced document.
      attr_accessor :code                           # 1-1 code
      ##
      # Target of the relationship
      # The target document of this relationship.
      attr_accessor :target                         # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference)
    end

    ##
    # Document referenced
    # The document and format referenced. There may be multiple content element repetitions, each with a different format.
    class Content < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Content.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Content.extension',
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
          'path'=>'Content.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Where to access the document
        # The document or URL of the document along with critical metadata to prove content has integrity.
        'attachment' => {
          'type'=>'Attachment',
          'path'=>'Content.attachment',
          'min'=>1,
          'max'=>1
        },
        ##
        # Format/content rules for the document
        # An identifier of the document encoding, structure, and template that the document conforms to beyond the base format indicated in the mimeType.
        # Note that while IHE mostly issues URNs for format types, not all documents can be identified by a URI.
        'format' => {
          'valid_codes'=>{
            'http://ihe.net/fhir/ValueSet/IHE.FormatCode.codesystem'=>[ 'urn:ihe:pcc:xphr:2007', 'urn:ihe:pcc:aps:2007', 'urn:ihe:pcc:xds-ms:2007', 'urn:ihe:pcc:edr:2007', 'urn:ihe:pcc:edes:2007', 'urn:ihe:pcc:apr:handp:2008', 'urn:ihe:pcc:apr:lab:2008', 'urn:ihe:pcc:apr:edu:2008', 'urn:ihe:pcc:crc:2008', 'urn:ihe:pcc:cm:2008', 'urn:ihe:pcc:ic:2008', 'urn:ihe:pcc:tn:2007', 'urn:ihe:pcc:nn:2007', 'urn:ihe:pcc:ctn:2007', 'urn:ihe:pcc:edpn:2007', 'urn:ihe:pcc:hp:2008', 'urn:ihe:pcc:ldhp:2009', 'urn:ihe:pcc:lds:2009', 'urn:ihe:pcc:mds:2009', 'urn:ihe:pcc:nds:2010', 'urn:ihe:pcc:ppvs:2010', 'urn:ihe:pcc:trs:2011', 'urn:ihe:pcc:ets:2011', 'urn:ihe:pcc:its:2011', 'urn:ihe:pcc:ript:2017', 'urn:ihe:iti:bppc:2007', 'urn:ihe:iti:bppc-sd:2007', 'urn:ihe:iti:xds-sd:pdf:2008', 'urn:ihe:iti:xds-sd:text:2008', 'urn:ihe:iti:xdw:2011:workflowDoc', 'urn:ihe:iti:dsg:detached:2014', 'urn:ihe:iti:dsg:enveloping:2014', 'urn:ihe:iti:appc:2016:consent', 'urn:ihe:iti:xds:2017:mimeTypeSufficient', 'urn:ihe:lab:xd-lab:2008', 'urn:ihe:rad:TEXT', 'urn:ihe:rad:PDF', 'urn:ihe:rad:CDA:ImagingReportStructuredHeadings:2013', 'urn:ihe:card:imaging:2011', 'urn:ihe:card:CRC:2012', 'urn:ihe:card:EPRC-IE:2014', 'urn:ihe:dent:TEXT', 'urn:ihe:dent:PDF', 'urn:ihe:dent:CDA:ImagingReportStructuredHeadings:2013', 'urn:ihe:pat:apsr:all:2010', 'urn:ihe:pat:apsr:cancer:all:2010', 'urn:ihe:pat:apsr:cancer:breast:2010', 'urn:ihe:pat:apsr:cancer:colon:2010', 'urn:ihe:pat:apsr:cancer:prostate:2010', 'urn:ihe:pat:apsr:cancer:thyroid:2010', 'urn:ihe:pat:apsr:cancer:lung:2010', 'urn:ihe:pat:apsr:cancer:skin:2010', 'urn:ihe:pat:apsr:cancer:kidney:2010', 'urn:ihe:pat:apsr:cancer:cervix:2010', 'urn:ihe:pat:apsr:cancer:endometrium:2010', 'urn:ihe:pat:apsr:cancer:ovary:2010', 'urn:ihe:pat:apsr:cancer:esophagus:2010', 'urn:ihe:pat:apsr:cancer:stomach:2010', 'urn:ihe:pat:apsr:cancer:liver:2010', 'urn:ihe:pat:apsr:cancer:pancreas:2010', 'urn:ihe:pat:apsr:cancer:testis:2010', 'urn:ihe:pat:apsr:cancer:urinary_bladder:2010', 'urn:ihe:pat:apsr:cancer:lip_oral_cavity:2010', 'urn:ihe:pat:apsr:cancer:pharynx:2010', 'urn:ihe:pat:apsr:cancer:salivary_gland:2010', 'urn:ihe:pat:apsr:cancer:larynx:2010', 'urn:ihe:pharm:pre:2010', 'urn:ihe:pharm:padv:2010', 'urn:ihe:pharm:dis:2010', 'urn:ihe:pharm:pml:2013', 'urn:hl7-org:sdwg:ccda-structuredBody:1.1', 'urn:hl7-org:sdwg:ccda-nonXMLBody:1.1', 'urn:hl7-org:sdwg:ccda-structuredBody:2.1', 'urn:hl7-org:sdwg:ccda-nonXMLBody:2.1' ]
          },
          'type'=>'Coding',
          'path'=>'Content.format',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/formatcodes'}
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
      # Where to access the document
      # The document or URL of the document along with critical metadata to prove content has integrity.
      attr_accessor :attachment                     # 1-1 Attachment
      ##
      # Format/content rules for the document
      # An identifier of the document encoding, structure, and template that the document conforms to beyond the base format indicated in the mimeType.
      # Note that while IHE mostly issues URNs for format types, not all documents can be identified by a URI.
      attr_accessor :format                         # 0-1 Coding
    end

    ##
    # Clinical context of document
    # The clinical context in which the document was prepared.
    # These values are primarily added to help with searching for interesting/relevant documents.
    class Context < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Context.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Context.extension',
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
          'path'=>'Context.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Context of the document  content
        # Describes the clinical encounter or type of care that the document content is associated with.
        'encounter' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter', 'http://hl7.org/fhir/StructureDefinition/EpisodeOfCare'],
          'type'=>'Reference',
          'path'=>'Context.encounter',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Main clinical acts documented
        # This list of codes represents the main clinical acts, such as a colonoscopy or an appendectomy, being documented. In some cases, the event is inherent in the type Code, such as a "History and Physical Report" in which the procedure being documented is necessarily a "History and Physical" act.
        # An event can further specialize the act inherent in the type, such as  where it is simply "Procedure Report" and the procedure was a "colonoscopy". If one or more event codes are included, they shall not conflict with the values inherent in the class or type elements as such a conflict would create an ambiguous situation.
        'event' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'ACCTRECEIVABLE', 'CASH', 'CC', 'AE', 'DN', 'DV', 'MC', 'V', 'PBILLACCT', 'CONT', 'DAY', 'LOC', 'MONTH', 'PERIOD', 'PROV', 'WEEK', 'YEAR', 'AA', 'ANF', 'AR', 'AS', 'DISPLAY', 'FORM', 'CPTM', 'HCPCSA', 'BLK', 'CAP', 'CONTF', 'FINBILL', 'ROST', 'SESS', 'FFS', 'FFPS', 'FFCS', 'TFS', 'ROIFS', 'ROIPS', 'CACC', 'CAIC', 'CAMC', 'CANC', 'CAPC', 'CBGC', 'CCCC', 'CCGC', 'CCPC', 'CCSC', 'CDEC', 'CDRC', 'CEMC', 'CFPC', 'CIMC', 'CMGC', 'CNEC', 'CNMC', 'CNQC', 'CNSC', 'COGC', 'COMC', 'COPC', 'COSC', 'COTC', 'CPEC', 'CPGC', 'CPHC', 'CPRC', 'CPSC', 'CPYC', 'CROC', 'CRPC', 'CSUC', 'CTSC', 'CURC', 'CVSC', 'LGPC', 'AALC', 'AAMC', 'ABHC', 'ACAC', 'ACHC', 'AHOC', 'ALTC', 'AOSC', 'CACS', 'CAMI', 'CAST', 'CBAR', 'CCAD', 'CCAR', 'CDEP', 'CDGD', 'CDIA', 'CEPI', 'CFEL', 'CHFC', 'CHRO', 'CHYP', 'CMIH', 'CMSC', 'COJR', 'CONC', 'COPD', 'CORT', 'CPAD', 'CPND', 'CPST', 'CSDM', 'CSIC', 'CSLD', 'CSPT', 'CTBU', 'CVDC', 'CWMA', 'CWOH', 'AMB', 'EMER', 'FLD', 'HH', 'IMP', 'ACUTE', 'NONAC', 'OBSENC', 'PRENC', 'SS', 'VR', 'ALC', 'CARD', 'CHR', 'DNTL', 'DRGRHB', 'GENRL', 'MED', 'OBS', 'ONC', 'PALL', 'PED', 'PHAR', 'PHYRHB', 'PSYCH', 'SURG', 'AUTOATTCH', 'DOCUMENT', 'HEALTHREC', 'IMG', 'LABRESULTS', 'MODEL', 'WIATTCH', 'XRAY', 'ICOL', 'IDSCL', 'INFA', 'INFAO', 'INFASO', 'IRDSCL', 'RESEARCH', 'RSDID', 'RSREID', 'ID', 'IP', 'L', 'M', 'O', 'R', 'X', 'AUTO', 'ENDC', 'REFLEX', 'AUTH', 'NAUTH', 'ELG', 'NELG', 'COVPRD', 'LFEMX', 'NETAMT', 'PRDMX', 'UNITPRICE', 'UNITQTY', 'COVMX', 'EHCPOL', 'HSAPOL', 'AUTOPOL', 'COL', 'UNINSMOT', 'PUBLICPOL', 'DENTPRG', 'DISEASEPRG', 'CANPRG', 'ENDRENAL', 'HIVAIDS', 'MANDPOL', 'MENTPRG', 'SAFNET', 'SUBPRG', 'SUBSIDIZ', 'SUBSIDMC', 'SUBSUPP', 'WCBPOL', 'DENTAL', 'DISEASE', 'DRUGPOL', 'HIP', 'LTC', 'MCPOL', 'POS', 'HMO', 'PPO', 'MENTPOL', 'SUBPOL', 'VISPOL', 'DIS', 'EWB', 'FLEXP', 'LIFE', 'ANNU', 'TLIFE', 'ULIFE', 'PNC', 'REI', 'SURPL', 'UMBRL', 'CHAR', 'CRIME', 'EAP', 'GOVEMP', 'HIRISK', 'IND', 'MILITARY', 'RETIRE', 'SOCIAL', 'VET', 'EMAUTH', '21', '1', '19', '2', '22', '23', '3', '4', '5', '6', '7', '14', '15', '16', '17', '18', '20', '8', '10', '11', '12', '13', '9', 'CHLDCARE', 'CONVEYNC', 'HLTHCARE', 'HOMECARE', 'HOSPPTNT', 'HOSPVSTR', 'HOUSEHLD', 'INMATE', 'INTIMATE', 'LTRMCARE', 'PLACE', 'PTNTCARE', 'SCHOOL2', 'SOCIAL2', 'SUBSTNCE', 'TRAVINT', 'WORK2', 'CHRG', 'REV', 'MVA', 'SCHOOL', 'SPT', 'WPA', 'ACADR', 'ACALL', 'ACALLG', 'ACCONS', 'ACDEMO', 'ACDI', 'ACIMMUN', 'ACLAB', 'ACMED', 'ACMEDC', 'ACMEN', 'ACOBS', 'ACPOLPRG', 'ACPROV', 'ACPSERV', 'ACSUBSTAB', 'INFAUT', 'INFCON', 'INFCRT', 'INFDNG', 'INFEMER', 'INFPWR', 'INFREG', 'ALLCAT', 'ALLGCAT', 'ARCAT', 'COBSCAT', 'DEMOCAT', 'DICAT', 'IMMUCAT', 'LABCAT', 'MEDCCAT', 'MENCAT', 'PSVCCAT', 'RXCAT', 'ALEC', 'BONUS', 'CFWD', 'EDU', 'EPYMT', 'GARN', 'INVOICE', 'PINV', 'PPRD', 'PROA', 'RECOV', 'RETRO', 'TRAN', 'INVTYPE', 'PAYEE', 'PAYOR', 'SENDAPP', 'UNSPSC', 'GTIN', 'UPC', 'COIN', 'COPAYMENT', 'DEDUCTIBLE', 'PAY', 'SPEND', 'COINS', 'AFTHRS', 'ISOL', 'OOO', 'CANCAPT', 'DSC', 'ESA', 'FFSTOP', 'FNLFEE', 'FRSTFEE', 'MARKUP', 'MISSAPT', 'PERFEE', 'PERMBNS', 'RESTOCK', 'TRAVEL', 'URGENT', 'FST', 'HST', 'PST', 'I', 'P', 'S', 'SP', 'W', 'CPNDDRGING', 'CPNDINDING', 'CPNDSUPING', 'DRUGING', 'FRAMEING', 'LENSING', 'PRDING', 'CPINV', 'CSINV', 'CSPINV', 'FININV', 'OHSINV', 'PAINV', 'RXCINV', 'RXDINV', 'SBFINV', 'VRXINV', 'ADNFPPELAT', 'ADNFPPELCT', 'ADNFPPMNAT', 'ADNFPPMNCT', 'ADNFSPELAT', 'ADNFSPELCT', 'ADNFSPMNAT', 'ADNFSPMNCT', 'ADNPPPELAT', 'ADNPPPELCT', 'ADNPPPMNAT', 'ADNPPPMNCT', 'ADNPSPELAT', 'ADNPSPELCT', 'ADNPSPMNAT', 'ADNPSPMNCT', 'ADPPPPELAT', 'ADPPPPELCT', 'ADPPPPMNAT', 'ADPPPPMNCT', 'ADPPSPELAT', 'ADPPSPELCT', 'ADPPSPMNAT', 'ADPPSPMNCT', 'ADRFPPELAT', 'ADRFPPELCT', 'ADRFPPMNAT', 'ADRFPPMNCT', 'ADRFSPELAT', 'ADRFSPELCT', 'ADRFSPMNAT', 'ADRFSPMNCT', 'PDNFPPELAT', 'PDNFPPELCT', 'PDNFPPMNAT', 'PDNFPPMNCT', 'PDNFSPELAT', 'PDNFSPELCT', 'PDNFSPMNAT', 'PDNFSPMNCT', 'PDNPPPELAT', 'PDNPPPELCT', 'PDNPPPMNAT', 'PDNPPPMNCT', 'PDNPSPELAT', 'PDNPSPELCT', 'PDNPSPMNAT', 'PDNPSPMNCT', 'PDPPPPELAT', 'PDPPPPELCT', 'PDPPPPMNAT', 'PDPPPPMNCT', 'PDPPSPELAT', 'PDPPSPELCT', 'PDPPSPMNAT', 'PDPPSPMNCT', 'SBBLELAT', 'SBBLELCT', 'SBNFELAT', 'SBNFELCT', 'SBPDELAT', 'SBPDELCT', 'COVGE', 'EFORM', 'FAX', 'GFTH', 'LATE', 'MANUAL', 'OOJ', 'ORTHO', 'PAPER', 'PIE', 'PYRDELAY', 'REFNR', 'REPSERV', 'UNRELAT', 'VERBAUTH', 'CARELIST', 'CONDLIST', 'INTOLIST', 'PROBLIST', 'RISKLIST', 'GOALLIST', 'ACU', 'CHRON', 'ONET', 'PRN', 'MEDLIST', 'CURMEDLIST', 'DISCMEDLIST', 'HISTMEDLIST', 'CTLSUB', 'INV', 'LU', 'OTC', 'RX', 'SA', 'SAC', 'IND01', 'IND02', 'IND03', 'IND04', 'IND05', 'VFPAPER', 'ACH', 'CHK', 'DDP', 'NON', 'DF', 'EM', 'SO', 'FF', 'FFC', 'FFP', 'FFSS', 'TF', 'FS', 'MS', 'RF', 'UD', 'RFC', 'RFCS', 'RFF', 'RFFS', 'RFP', 'RFPS', 'RFS', 'TB', 'TBS', 'UDE', 'EMRGONLY', 'GRANTORCHOICE', 'IMPLIED', 'IMPLIEDD', 'NOCONSENT', 'NOPP', 'OPTIN', 'OPTINR', 'OPTOUT', 'OPTOUTE', '_ActUSPrivacyLaw', '42CFRPart2', 'CommonRule', 'HIPAANOPP', 'HIPAAPsyNotes', 'HIPAASelfPay', 'Title38Section7332', 'ETH', 'GDIS', 'HIV', 'MST', 'SCA', 'SDV', 'SEX', 'SPI', 'BH', 'COGN', 'DVD', 'EMOTDIS', 'MH', 'PSY', 'PSYTHPN', 'SUD', 'ETHUD', 'OPIOIDUD', 'STD', 'TBOO', 'VIO', 'SICKLE', 'DEMO', 'DOB', 'GENDER', 'LIVARG', 'MARST', 'RACE', 'REL', 'B', 'EMPL', 'LOCIS', 'SSP', 'ADOL', 'CEL', 'DIA', 'DRGIS', 'EMP', 'PDS', 'PHY', 'PRS', 'COMPT', 'ACOCOMPT', 'CTCOMPT', 'FMCOMPT', 'HRCOMPT', 'LRCOMPT', 'PACOMPT', 'RESCOMPT', 'RMGTCOMPT', 'TRSTACCRD', 'TRSTAGRE', 'TRSTASSUR', 'TRSTCERT', 'TRSTFWK', 'TRSTMEC', 'COVPOL', 'SecurityPolicy', 'AUTHPOL', 'ACCESSCONSCHEME', 'DELEPOL', 'ObligationPolicy', 'ANONY', 'AOD', 'AUDIT', 'AUDTR', 'CPLYCC', 'CPLYCD', 'CPLYJPP', 'CPLYOPP', 'CPLYOSP', 'CPLYPOL', 'DECLASSIFYLABEL', 'DEID', 'DELAU', 'DOWNGRDLABEL', 'DRIVLABEL', 'ENCRYPT', 'ENCRYPTR', 'ENCRYPTT', 'ENCRYPTU', 'HUAPRV', 'LABEL', 'MASK', 'MINEC', 'PERSISTLABEL', 'PRIVMARK', 'PSEUD', 'REDACT', 'UPGRDLABEL', 'RefrainPolicy', 'NOAUTH', 'NOCOLLECT', 'NODSCLCD', 'NODSCLCDS', 'NOINTEGRATE', 'NOLIST', 'NOMOU', 'NOORGPOL', 'NOPAT', 'NOPERSISTP', 'NORDSCLCD', 'NORDSCLCDS', 'NORDSCLW', 'NORELINK', 'NOREUSE', 'NOVIP', 'ORCON', 'LOAN', 'RENT', 'TRANSFER', 'SALE', 'SREC', 'SSTOR', 'STRAN', 'ACID', 'ALK', 'DEFB', 'FILT', 'LDLP', 'NEUT', 'RECA', 'UFIL', 'DRUG', 'FD', 'IMMUNIZ', 'BOOSTER', 'INITIMMUNIZ', 'OE', 'LABOE', 'MEDOE', 'PATDOC', 'ALLERLREV', 'CLINNOTEE', 'DIAGLISTE', 'DISCHINSTE', 'DISCHSUME', 'PATEDUE', 'PATREPE', 'PROBLISTE', 'RADREPE', 'IMMLREV', 'REMLREV', 'WELLREMLREV', 'PATINFO', 'ALLERLE', 'CDSREV', 'CLINNOTEREV', 'DISCHSUMREV', 'DIAGLISTREV', 'IMMLE', 'LABRREV', 'MICRORREV', 'MICROORGRREV', 'MICROSENSRREV', 'MLREV', 'MARWLREV', 'OREV', 'PATREPREV', 'PROBLISTREV', 'RADREPREV', 'REMLE', 'WELLREMLE', 'RISKASSESS', 'FALLRISK', 'AFOOT', 'AMBT', 'AMBAIR', 'AMBGRND', 'AMBHELO', 'LAWENF', 'PRVTRN', 'PUBTRN', 'ARTBLD', 'DILUTION', 'AUTO-HIGH', 'AUTO-LOW', 'PRE', 'RERUN', 'EVNFCTS', 'INTFR', 'FIBRIN', 'HEMOLYSIS', 'ICTERUS', 'LIPEMIA', 'VOLUME', 'AVAILABLE', 'CONSUMPTION', 'CURRENT', 'INITIAL', '_ActPatientAnnotationType', 'ANNDI', 'ANNGEN', 'ANNIMM', 'ANNLAB', 'ANNMED', 'GENE', '_ImmunizationObservationType', 'OBSANTC', 'OBSANTV', 'PAT_ADV_EVNT', 'VAC_PROBLEM', '21611-9', '21612-7', '29553-5', '30525-0', '30972-4', 'REP_HALF_LIFE', 'SPLCOATING', 'SPLCOLOR', 'SPLIMAGE', 'SPLIMPRINT', 'SPLSCORING', 'SPLSHAPE', 'SPLSIZE', 'SPLSYMBOL', 'AIRTRNS', 'ANANTRNS', 'ANHUMTRNS', 'BDYFLDTRNS', 'BLDTRNS', 'DERMTRNS', 'ENVTRNS', 'FECTRNS', 'FOMTRNS', 'FOODTRNS', 'HUMHUMTRNS', 'INDTRNS', 'LACTTRNS', 'NOSTRNS', 'PARTRNS', 'PLACTRNS', 'SEXTRNS', 'TRNSFTRNS', 'VECTRNS', 'WATTRNS', '_ObservationQualityMeasureAttribute', 'AGGREGATE', 'CMPMSRMTH', 'CMPMSRSCRWGHT', 'COPY', 'CRS', 'DEF', 'DISC', 'FINALDT', 'GUIDE', 'IDUR', 'ITMCNT', 'KEY', 'MEDT', 'MSD', 'MSRADJ', 'MSRAGG', 'MSRIMPROV', 'MSRJUR', 'MSRRPTR', 'MSRRPTTIME', 'MSRSCORE', 'MSRSET', 'MSRTOPIC', 'MSRTP', 'MSRTYPE', 'RAT', 'REF', 'SDE', 'STRAT', 'TRANF', 'USE', 'TIME_ABSOLUTE', 'TIME_RELATIVE', 'REPRESENTATIVE_BEAT', 'RHYTHM', '_PatientImmunizationRelatedObservationType', 'CLSSRM', 'GRADE', 'SCHL', 'SCHLDIV', 'TEACHER', 'DENEX', 'DENEXCEP', 'DENOM', 'IPOP', 'IPPOP', 'MSROBS', 'MSRPOPL', 'MSRPOPLEX', 'NUMER', 'NUMEX', 'PREFSTRENGTH', 'ADVERSE_REACTION', 'ASSERTION', 'CASESER', 'CDIO', 'CRIT', 'CTMO', 'DX', 'ADMDX', 'DISDX', 'INTDX', 'NOI', 'GISTIER', 'HHOBS', 'ISSUE', 'NAT', 'SUPPRESSED', 'VALIDAT', 'KEY204', 'KEY205', 'COMPLY', 'DUPTHPY', 'DUPTHPCLS', 'DUPTHPGEN', 'ABUSE', 'FRAUD', 'PLYDOC', 'PLYPHRM', 'DOSE', 'DOSECOND', 'DOSEDUR', 'DOSEDURH', 'DOSEDURHIND', 'DOSEDURL', 'DOSEDURLIND', 'DOSEH', 'DOSEHINDA', 'DOSEHIND', 'DOSEHINDSA', 'DOSEHINDW', 'DOSEIVL', 'DOSEIVLIND', 'DOSEL', 'DOSELINDA', 'DOSELIND', 'DOSELINDSA', 'DOSELINDW', 'MDOSE', 'OBSA', 'AGE', 'ADALRT', 'GEALRT', 'PEALRT', 'COND', 'HGHT', 'LACT', 'PREG', 'WGHT', 'CREACT', 'GEN', 'GEND', 'LAB', 'REACT', 'ALGY', 'INT', 'RREACT', 'RALG', 'RAR', 'RINT', 'BUS', 'CODE_INVAL', 'CODE_DEPREC', 'FORMAT', 'ILLEGAL', 'LEN_RANGE', 'LEN_LONG', 'LEN_SHORT', 'MISSCOND', 'MISSMAND', 'NODUPS', 'NOPERSIST', 'REP_RANGE', 'MAXOCCURS', 'MINOCCURS', 'KEY206', 'OBSOLETE', 'FOOD', 'TPROD', 'DRG', 'NHP', 'NONRX', 'PREVINEF', 'DACT', 'TIME', 'ALRTENDLATE', 'ALRTSTRTLATE', 'ENDLATE', 'STRTLATE', 'ALLDONE', 'FULFIL', 'NOTACTN', 'NOTEQUIV', 'NOTEQUIVGEN', 'NOTEQUIVTHER', 'TIMING', 'INTERVAL', 'MINFREQ', 'HELD', 'TOOLATE', 'TOOSOON', 'HISTORIC', 'PATPREF', 'PATPREFALT', 'KSUBJ', 'KSUBT', 'OINT', 'ALG', 'DALG', 'EALG', 'FALG', 'DINT', 'DNAINT', 'EINT', 'ENAINT', 'FINT', 'FNAINT', 'NAINT', 'SEV', 'FDACOATING', 'FDACOLOR', 'FDAIMPRINTCD', 'FDALOGO', 'FDASCORING', 'FDASHAPE', 'FDASIZE', 'CIRCLE', 'ELLIPSE', 'POINT', 'POLY', 'C', 'DIET', 'BR', 'DM', 'FAST', 'FORMULA', 'GF', 'LF', 'LP', 'LQ', 'LS', 'N', 'NF', 'PAF', 'PAR', 'RD', 'SCH', 'SUPPLEMENT', 'T', 'VLI', 'DRUGPRG', 'F', 'PRLMN', 'SECCATOBS', 'SECCLASSOBS', 'SECCONOBS', 'SECINTOBS', 'SECALTINTOBS', 'SECDATINTOBS', 'SECINTCONOBS', 'SECINTPRVABOBS', 'SECINTPRVRBOBS', 'SECINTSTOBS', 'TRSTACCRDOBS', 'TRSTAGREOBS', 'TRSTCERTOBS', 'TRSTFWKOBS', 'TRSTLOAOBS', 'TRSTMECOBS', 'SUBSIDFFS', 'WRKCOMP', 'COPAY', 'DEDUCT', 'DOSEIND', 'PRA' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Context.event',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActCode'}
        },
        ##
        # Time of service that is being documented
        # The time period over which the service that is described by the document was provided.
        'period' => {
          'type'=>'Period',
          'path'=>'Context.period',
          'min'=>0,
          'max'=>1
        },
        ##
        # Kind of facility where patient was seen
        # The kind of facility where the patient was seen.
        'facilityType' => {
          'type'=>'CodeableConcept',
          'path'=>'Context.facilityType',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional details about where the content was created (e.g. clinical specialty)
        # This property may convey specifics about the practice setting where the content was created, often reflecting the clinical specialty.
        # This element should be based on a coarse classification system for the class of specialty practice. Recommend the use of the classification system for Practice Setting, such as that described by the Subject Matter Domain in LOINC.
        'practiceSetting' => {
          'type'=>'CodeableConcept',
          'path'=>'Context.practiceSetting',
          'min'=>0,
          'max'=>1
        },
        ##
        # Patient demographics from source
        # The Patient Information as known when the document was published. May be a reference to a version specific, or contained.
        'sourcePatientInfo' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
          'type'=>'Reference',
          'path'=>'Context.sourcePatientInfo',
          'min'=>0,
          'max'=>1
        },
        ##
        # Related identifiers or resources associated with the DocumentReference.
        # May be identifiers or resources that caused the DocumentReference or referenced Document to be created.
        'related' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Context.related',
          'min'=>0,
          'max'=>Float::INFINITY
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
      # Context of the document  content
      # Describes the clinical encounter or type of care that the document content is associated with.
      attr_accessor :encounter                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Encounter|http://hl7.org/fhir/StructureDefinition/EpisodeOfCare) ]
      ##
      # Main clinical acts documented
      # This list of codes represents the main clinical acts, such as a colonoscopy or an appendectomy, being documented. In some cases, the event is inherent in the type Code, such as a "History and Physical Report" in which the procedure being documented is necessarily a "History and Physical" act.
      # An event can further specialize the act inherent in the type, such as  where it is simply "Procedure Report" and the procedure was a "colonoscopy". If one or more event codes are included, they shall not conflict with the values inherent in the class or type elements as such a conflict would create an ambiguous situation.
      attr_accessor :event                          # 0-* [ CodeableConcept ]
      ##
      # Time of service that is being documented
      # The time period over which the service that is described by the document was provided.
      attr_accessor :period                         # 0-1 Period
      ##
      # Kind of facility where patient was seen
      # The kind of facility where the patient was seen.
      attr_accessor :facilityType                   # 0-1 CodeableConcept
      ##
      # Additional details about where the content was created (e.g. clinical specialty)
      # This property may convey specifics about the practice setting where the content was created, often reflecting the clinical specialty.
      # This element should be based on a coarse classification system for the class of specialty practice. Recommend the use of the classification system for Practice Setting, such as that described by the Subject Matter Domain in LOINC.
      attr_accessor :practiceSetting                # 0-1 CodeableConcept
      ##
      # Patient demographics from source
      # The Patient Information as known when the document was published. May be a reference to a version specific, or contained.
      attr_accessor :sourcePatientInfo              # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
      ##
      # Related identifiers or resources associated with the DocumentReference.
      # May be identifiers or resources that caused the DocumentReference or referenced Document to be created.
      attr_accessor :related                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
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
    # Master Version Specific Identifier
    # Document identifier as assigned by the source of the document. This identifier is specific to this version of the document. This unique identifier may be used elsewhere to identify this version of the document.
    # CDA Document Id extension and root.
    attr_accessor :masterIdentifier               # 0-1 Identifier
    ##
    # Other identifiers for the document
    # Other identifiers associated with the document, including version independent identifiers.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # current | superseded | entered-in-error
    # The status of this document reference.
    # This is the status of the DocumentReference object, which might be independent from the docStatus element.
    # 
    # This element is labeled as a modifier because the status contains the codes that mark the document or reference as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # preliminary | final | amended | entered-in-error
    # The status of the underlying document.
    # The document that is pointed to might be in various lifecycle states.
    attr_accessor :docStatus                      # 0-1 code
    ##
    # Kind of document (LOINC if possible)
    # Specifies the particular kind of document referenced  (e.g. History and Physical, Discharge Summary, Progress Note). This usually equates to the purpose of making the document referenced.
    # Key metadata element describing the document that describes he exact type of document. Helps humans to assess whether the document is of interest when viewing a list of documents.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # Categorization of document
    # A categorization for the type of document referenced - helps for indexing and searching. This may be implied by or derived from the code specified in the DocumentReference.type.
    # Key metadata element describing the the category or classification of the document. This is a broader perspective that groups similar documents based on how they would be used. This is a primary key used in searching.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # Who/what is the subject of the document
    # Who or what the document is about. The document can be about a person, (patient or healthcare practitioner), a device (e.g. a machine) or even a group of subjects (such as a document about a herd of farm animals, or a set of patients that share a common exposure).
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # When this document reference was created
    # When the document reference was created.
    # Referencing/indexing time is used for tracking, organizing versions and searching.
    attr_accessor :date                           # 0-1 instant
    ##
    # Who and/or what authored the document
    # Identifies who is responsible for adding the information to the document.
    # Not necessarily who did the actual data entry (i.e. typist) or who was the source (informant).
    attr_accessor :author                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson) ]
    ##
    # Who/what authenticated the document
    # Which person or organization authenticates that this document is valid.
    # Represents a participant within the author institution who has legally authenticated or attested the document. Legal authentication implies that a document has been signed manually or electronically by the legal Authenticator.
    attr_accessor :authenticator                  # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Organization which maintains the document
    # Identifies the organization or group who is responsible for ongoing maintenance of and access to the document.
    # Identifies the logical organization (software system, vendor, or department) to go to find the current version, where to report issues, etc. This is different from the physical location (URL, disk drive, or server) of the document, which is the technical location of the document, which host may be delegated to the management of some other organization.
    attr_accessor :custodian                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Relationships to other documents
    # Relationships that this document has with other document references that already exist.
    # This element is labeled as a modifier because documents that append to other documents are incomplete on their own.
    attr_accessor :relatesTo                      # 0-* [ DocumentReference::RelatesTo ]
    ##
    # Human-readable description of the source document.
    # What the document is about,  a terse summary of the document.
    attr_accessor :description                    # 0-1 string
    ##
    # Document security-tags
    # A set of Security-Tag codes specifying the level of privacy/security of the Document. Note that DocumentReference.meta.security contains the security labels of the "reference" to the document, while DocumentReference.securityLabel contains a snapshot of the security labels on the document the reference refers to.
    # The confidentiality codes can carry multiple vocabulary items. HL7 has developed an understanding of security and privacy tags that might be desirable in a Document Sharing environment, called HL7 Healthcare Privacy and Security Classification System (HCS). The following specification is recommended but not mandated, as the vocabulary bindings are an administrative domain responsibility. The use of this method is up to the policy domain such as the XDS Affinity Domain or other Trust Domain where all parties including sender and recipients are trusted to appropriately tag and enforce.   
    # 
    # In the HL7 Healthcare Privacy and Security Classification (HCS) there are code systems specific to Confidentiality, Sensitivity, Integrity, and Handling Caveats. Some values would come from a local vocabulary as they are related to workflow roles and special projects.
    attr_accessor :securityLabel                  # 0-* [ CodeableConcept ]
    ##
    # Document referenced
    # The document and format referenced. There may be multiple content element repetitions, each with a different format.
    attr_accessor :content                        # 1-* [ DocumentReference::Content ]
    ##
    # Clinical context of document
    # The clinical context in which the document was prepared.
    # These values are primarily added to help with searching for interesting/relevant documents.
    attr_accessor :context                        # 0-1 DocumentReference::Context

    def resourceType
      'DocumentReference'
    end
  end
end
