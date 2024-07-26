module FHIR

  ##
  # Provenance of a resource is a record that describes entities and processes involved in producing and delivering or otherwise influencing that resource. Provenance provides a critical foundation for assessing authenticity, enabling trust, and allowing reproducibility. Provenance assertions are a form of contextual metadata and can themselves become important records with their own provenance. Provenance statement indicates clinical significance in terms of confidence in authenticity, reliability, and trustworthiness, integrity, and stage in lifecycle (e.g. Document Completion - has the artifact been legally authenticated), all of which may impact security, privacy, and trust policies.
  class Provenance < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['agent-role', 'agent-type', 'agent', 'entity', 'location', 'patient', 'recorded', 'signature-type', 'target', 'when']
    MULTIPLE_TYPES = {
      'occurred[x]' => ['dateTime', 'Period']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Provenance.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Provenance.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Provenance.implicitRules',
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
        'path'=>'Provenance.language',
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
        'path'=>'Provenance.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Provenance.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Provenance.extension',
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
        'path'=>'Provenance.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Target Reference(s) (usually version specific)
      # The Reference(s) that were generated or updated by  the activity described in this resource. A provenance can point to more than one target if multiple resources were created/updated by the same activity.
      # Target references are usually version specific, but might not be, if a version has not been assigned or if the provenance information is part of the set of resources being maintained (i.e. a document). When using the RESTful API, the identity of the resource might not be known (especially not the version specific one); the client may either submit the resource first, and then the provenance, or it may submit both using a single transaction. See the notes on transaction for further discussion.
      'target' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Provenance.target',
        'min'=>1,
        'max'=>Float::INFINITY
      },
      ##
      # When the activity occurred
      # The period during which the activity occurred.
      # The period can be a little arbitrary; where possible, the time should correspond to human assessment of the activity time.
      'occurredDateTime' => {
        'type'=>'DateTime',
        'path'=>'Provenance.occurred[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the activity occurred
      # The period during which the activity occurred.
      # The period can be a little arbitrary; where possible, the time should correspond to human assessment of the activity time.
      'occurredPeriod' => {
        'type'=>'Period',
        'path'=>'Provenance.occurred[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the activity was recorded / updated
      # The instant of time at which the activity was recorded.
      # This can be a little different from the time stamp on the resource if there is a delay between recording the event and updating the provenance and target resource.
      'recorded' => {
        'type'=>'instant',
        'path'=>'Provenance.recorded',
        'min'=>1,
        'max'=>1
      },
      ##
      # Policy or plan the activity was defined by. Typically, a single activity may have multiple applicable policy documents, such as patient consent, guarantor funding, etc.
      # For example: Where an OAuth token authorizes, the unique identifier from the OAuth token is placed into the policy element Where a policy engine (e.g. XACML) holds policy logic, the unique policy identifier is placed into the policy element.
      'policy' => {
        'type'=>'uri',
        'path'=>'Provenance.policy',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Where the activity occurred, if relevant.
      'location' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Provenance.location',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reason the activity is occurring
      # The reason that the activity was taking place.
      'reason' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ActReason'=>[ 'PurposeOfUse', 'HMARKT', 'HOPERAT', 'CAREMGT', 'DONAT', 'FRAUD', 'GOV', 'HACCRED', 'HCOMPL', 'HDECD', 'HDIRECT', 'HDM', 'HLEGAL', 'HOUTCOMS', 'HPRGRP', 'HQUALIMP', 'HSYSADMIN', 'LABELING', 'METAMGT', 'MEMADMIN', 'MILCDM', 'PATADMIN', 'PATSFTY', 'PERFMSR', 'RECORDMGT', 'SYSDEV', 'HTEST', 'TRAIN', 'HPAYMT', 'CLMATTCH', 'COVAUTH', 'COVERAGE', 'ELIGDTRM', 'ELIGVER', 'ENROLLM', 'MILDCRG', 'REMITADV', 'HRESCH', 'BIORCH', 'CLINTRCH', 'CLINTRCHNPC', 'CLINTRCHPC', 'PRECLINTRCH', 'DSRCH', 'POARCH', 'TRANSRCH', 'PATRQT', 'FAMRQT', 'PWATRNY', 'SUPNWK', 'PUBHLTH', 'DISASTER', 'THREAT', 'TREAT', 'CLINTRL', 'COC', 'ETREAT', 'BTG', 'ERTREAT', 'POPHLTH' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Provenance.reason',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-PurposeOfUse'}
      },
      ##
      # Activity that occurred
      # An activity is something that occurs over a period of time and acts upon or with entities; it may include consuming, processing, transforming, modifying, relocating, using, or generating entities.
      'activity' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'ANONY', 'DEID', 'MASK', 'LABEL', 'PSEUD' ],
          'http://terminology.hl7.org/CodeSystem/v3-DataOperation'=>[ 'CREATE', 'DELETE', 'UPDATE', 'APPEND', 'NULLIFY' ],
          'http://terminology.hl7.org/CodeSystem/v3-DocumentCompletion'=>[ 'LA' ],
          'http://terminology.hl7.org/CodeSystem/v3-ParticipationType'=>[ 'PART', 'ADM', 'ATND', 'CALLBCK', 'CON', 'DIS', 'ESC', 'REF', 'AUT', 'INF', 'TRANS', 'ENT', 'WIT', 'CST', 'DIR', 'ALY', 'BBY', 'CAT', 'CSM', 'TPA', 'DEV', 'NRD', 'RDV', 'DON', 'EXPAGNT', 'EXPART', 'EXPTRGT', 'EXSRC', 'PRD', 'SBJ', 'SPC', 'IND', 'BEN', 'CAGNT', 'COV', 'GUAR', 'HLD', 'RCT', 'RCV', 'IRCP', 'NOT', 'PRCP', 'REFB', 'REFT', 'TRC', 'LOC', 'DST', 'ELOC', 'ORG', 'RML', 'VIA', 'PRF', 'DIST', 'PPRF', 'SPRF', 'RESP', 'VRF', 'AUTHEN', 'LA' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Provenance.activity',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/provenance-activity-type'}
      },
      ##
      # Actor involved
      # An actor taking a role in an activity  for which it can be assigned some degree of responsibility for the activity taking place.
      # Several agents may be associated (i.e. has some responsibility for an activity) with an activity and vice-versa.
      'agent' => {
        'type'=>'Provenance::Agent',
        'path'=>'Provenance.agent',
        'min'=>1,
        'max'=>Float::INFINITY
      },
      ##
      # An entity used in this activity.
      'entity' => {
        'type'=>'Provenance::Entity',
        'path'=>'Provenance.entity',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Signature on target
      # A digital signature on the target Reference(s). The signer should match a Provenance.agent. The purpose of the signature is indicated.
      'signature' => {
        'type'=>'Signature',
        'path'=>'Provenance.signature',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Actor involved
    # An actor taking a role in an activity  for which it can be assigned some degree of responsibility for the activity taking place.
    # Several agents may be associated (i.e. has some responsibility for an activity) with an activity and vice-versa.
    class Agent < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Agent.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Agent.extension',
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
          'path'=>'Agent.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # How the agent participated
        # The participation the agent had with respect to the activity.
        # For example: author, performer, enterer, attester, etc.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/provenance-participant-type'=>[ 'enterer', 'performer', 'author', 'verifier', 'legal', 'attester', 'informant', 'custodian', 'assembler', 'composer' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Agent.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/provenance-agent-type'}
        },
        ##
        # What the agents role was
        # The function of the agent with respect to the activity. The security role enabling the agent with respect to the activity.
        # For example: doctor, nurse, clerk, etc.
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
          'path'=>'Agent.role',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/security-role-type'}
        },
        ##
        # Who participated
        # The individual, device or organization that participated in the event.
        # whoIdentity should be used when the agent is not a Resource type.
        'who' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Agent.who',
          'min'=>1,
          'max'=>1
        },
        ##
        # Who the agent is representing
        # The individual, device, or organization for whom the change was made.
        # onBehalfOfIdentity should be used when the agent is not a Resource type.
        'onBehalfOf' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Agent.onBehalfOf',
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # How the agent participated
      # The participation the agent had with respect to the activity.
      # For example: author, performer, enterer, attester, etc.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # What the agents role was
      # The function of the agent with respect to the activity. The security role enabling the agent with respect to the activity.
      # For example: doctor, nurse, clerk, etc.
      attr_accessor :role                           # 0-* [ CodeableConcept ]
      ##
      # Who participated
      # The individual, device or organization that participated in the event.
      # whoIdentity should be used when the agent is not a Resource type.
      attr_accessor :who                            # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # Who the agent is representing
      # The individual, device, or organization for whom the change was made.
      # onBehalfOfIdentity should be used when the agent is not a Resource type.
      attr_accessor :onBehalfOf                     # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Organization)
    end

    ##
    # An entity used in this activity.
    class Entity < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Entity.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Entity.extension',
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
          'path'=>'Entity.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # derivation | revision | quotation | source | removal
        # How the entity was used during the activity.
        'role' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/provenance-entity-role'=>[ 'derivation', 'revision', 'quotation', 'source', 'removal' ]
          },
          'type'=>'code',
          'path'=>'Entity.role',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/provenance-entity-role'}
        },
        ##
        # Identity of entity
        # Identity of the  Entity used. May be a logical or physical uri and maybe absolute or relative.
        # whatIdentity should be used for entities that are not a Resource type.
        'what' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Entity.what',
          'min'=>1,
          'max'=>1
        },
        ##
        # Entity is attributed to this agent
        # The entity is attributed to an agent to express the agent's responsibility for that entity, possibly along with other agents. This description can be understood as shorthand for saying that the agent was responsible for the activity which generated the entity.
        # A usecase where one Provenance.entity.agent is used where the Entity that was used in the creation/updating of the Target, is not in the context of the same custodianship as the Target, and thus the meaning of Provenance.entity.agent is to say that the entity referenced is managed elsewhere and that this Agent provided access to it.  This would be similar to where the Entity being referenced is managed outside FHIR, such as through HL7 v2, v3, or XDS. This might be where the Entity being referenced is managed in another FHIR resource server. Thus it explains the Provenance of that Entity's use in the context of this Provenance activity.
        'agent' => {
          'type'=>'Provenance::Agent',
          'path'=>'Entity.agent',
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
      # derivation | revision | quotation | source | removal
      # How the entity was used during the activity.
      attr_accessor :role                           # 1-1 code
      ##
      # Identity of entity
      # Identity of the  Entity used. May be a logical or physical uri and maybe absolute or relative.
      # whatIdentity should be used for entities that are not a Resource type.
      attr_accessor :what                           # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
      ##
      # Entity is attributed to this agent
      # The entity is attributed to an agent to express the agent's responsibility for that entity, possibly along with other agents. This description can be understood as shorthand for saying that the agent was responsible for the activity which generated the entity.
      # A usecase where one Provenance.entity.agent is used where the Entity that was used in the creation/updating of the Target, is not in the context of the same custodianship as the Target, and thus the meaning of Provenance.entity.agent is to say that the entity referenced is managed elsewhere and that this Agent provided access to it.  This would be similar to where the Entity being referenced is managed outside FHIR, such as through HL7 v2, v3, or XDS. This might be where the Entity being referenced is managed in another FHIR resource server. Thus it explains the Provenance of that Entity's use in the context of this Provenance activity.
      attr_accessor :agent                          # 0-* [ Provenance::Agent ]
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
    # Target Reference(s) (usually version specific)
    # The Reference(s) that were generated or updated by  the activity described in this resource. A provenance can point to more than one target if multiple resources were created/updated by the same activity.
    # Target references are usually version specific, but might not be, if a version has not been assigned or if the provenance information is part of the set of resources being maintained (i.e. a document). When using the RESTful API, the identity of the resource might not be known (especially not the version specific one); the client may either submit the resource first, and then the provenance, or it may submit both using a single transaction. See the notes on transaction for further discussion.
    attr_accessor :target                         # 1-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # When the activity occurred
    # The period during which the activity occurred.
    # The period can be a little arbitrary; where possible, the time should correspond to human assessment of the activity time.
    attr_accessor :occurredDateTime               # 0-1 DateTime
    ##
    # When the activity occurred
    # The period during which the activity occurred.
    # The period can be a little arbitrary; where possible, the time should correspond to human assessment of the activity time.
    attr_accessor :occurredPeriod                 # 0-1 Period
    ##
    # When the activity was recorded / updated
    # The instant of time at which the activity was recorded.
    # This can be a little different from the time stamp on the resource if there is a delay between recording the event and updating the provenance and target resource.
    attr_accessor :recorded                       # 1-1 instant
    ##
    # Policy or plan the activity was defined by. Typically, a single activity may have multiple applicable policy documents, such as patient consent, guarantor funding, etc.
    # For example: Where an OAuth token authorizes, the unique identifier from the OAuth token is placed into the policy element Where a policy engine (e.g. XACML) holds policy logic, the unique policy identifier is placed into the policy element.
    attr_accessor :policy                         # 0-* [ uri ]
    ##
    # Where the activity occurred, if relevant.
    attr_accessor :location                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Reason the activity is occurring
    # The reason that the activity was taking place.
    attr_accessor :reason                         # 0-* [ CodeableConcept ]
    ##
    # Activity that occurred
    # An activity is something that occurs over a period of time and acts upon or with entities; it may include consuming, processing, transforming, modifying, relocating, using, or generating entities.
    attr_accessor :activity                       # 0-1 CodeableConcept
    ##
    # Actor involved
    # An actor taking a role in an activity  for which it can be assigned some degree of responsibility for the activity taking place.
    # Several agents may be associated (i.e. has some responsibility for an activity) with an activity and vice-versa.
    attr_accessor :agent                          # 1-* [ Provenance::Agent ]
    ##
    # An entity used in this activity.
    attr_accessor :entity                         # 0-* [ Provenance::Entity ]
    ##
    # Signature on target
    # A digital signature on the target Reference(s). The signer should match a Provenance.agent. The purpose of the signature is indicated.
    attr_accessor :signature                      # 0-* [ Signature ]

    def resourceType
      'Provenance'
    end
  end
end
