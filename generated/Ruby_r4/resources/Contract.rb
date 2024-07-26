module FHIR

  ##
  # Legally enforceable, formally recorded unilateral or bilateral directive i.e., a policy or agreement.
  class Contract < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['authority', 'domain', 'identifier', 'instantiates', 'issued', 'patient', 'signer', 'status', 'subject', 'url']
    MULTIPLE_TYPES = {
      'topic[x]' => ['CodeableConcept', 'Reference'],
      'legallyBinding[x]' => ['Attachment', 'Reference']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Contract.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Contract.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Contract.implicitRules',
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
        'path'=>'Contract.language',
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
        'path'=>'Contract.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Contract.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Contract.extension',
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
        'path'=>'Contract.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Contract number
      # Unique identifier for this Contract or a derivative that references a Source Contract.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Contract.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Basal definition
      # Canonical identifier for this contract, represented as a URI (globally unique).
      # Used in a domain that uses a supplied contract repository.
      'url' => {
        'type'=>'uri',
        'path'=>'Contract.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Business edition
      # An edition identifier used for business purposes to label business significant variants.
      # Note -  This is a business versionId, not a resource version id (see discussion http://build.fhir.org/resource.html#versions) Comments - There may be different contract instances that have the same identifier but different versions. The version can be appended to the url in a reference to allow a reference to a particular business version of the plan definition with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'Contract.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # amended | appended | cancelled | disputed | entered-in-error | executable | executed | negotiable | offered | policy | rejected | renewed | revoked | resolved | terminated
      # The status of the resource instance.
      # This element is labeled as a modifier because the status contains codes that mark the contract as not currently valid or active.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/contract-status'=>[ 'amended', 'appended', 'cancelled', 'disputed', 'entered-in-error', 'executable', 'executed', 'negotiable', 'offered', 'policy', 'rejected', 'renewed', 'revoked', 'resolved', 'terminated' ]
        },
        'type'=>'code',
        'path'=>'Contract.status',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-status'}
      },
      ##
      # Negotiation status
      # Legal states of the formation of a legal instrument, which is a formally executed written document that can be formally attributed to its author, records and formally expresses a legally enforceable act, process, or contractual duty, obligation, or right, and therefore evidences that act, process, or agreement.
      'legalState' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/contract-legalstate'=>[ 'amended', 'appended', 'cancelled', 'disputed', 'entered-in-error', 'executable', 'executed', 'negotiable', 'offered', 'policy', 'rejected', 'renewed', 'revoked', 'resolved', 'terminated' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Contract.legalState',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-legalstate'}
      },
      ##
      # Source Contract Definition
      # The URL pointing to a FHIR-defined Contract Definition that is adhered to in whole or part by this Contract.
      'instantiatesCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Contract'],
        'type'=>'Reference',
        'path'=>'Contract.instantiatesCanonical',
        'min'=>0,
        'max'=>1
      },
      ##
      # External Contract Definition
      # The URL pointing to an externally maintained definition that is adhered to in whole or in part by this Contract.
      'instantiatesUri' => {
        'type'=>'uri',
        'path'=>'Contract.instantiatesUri',
        'min'=>0,
        'max'=>1
      },
      ##
      # Content derived from the basal information
      # The minimal content derived from the basal information source at a specific stage in its lifecycle.
      'contentDerivative' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/contract-content-derivative'=>[ 'registration', 'retrieval', 'statement', 'shareable' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Contract.contentDerivative',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-content-derivative'}
      },
      ##
      # When this Contract was issued
      # When this  Contract was issued.
      'issued' => {
        'type'=>'dateTime',
        'path'=>'Contract.issued',
        'min'=>0,
        'max'=>1
      },
      ##
      # Effective time
      # Relevant time or time-period when this Contract is applicable.
      'applies' => {
        'type'=>'Period',
        'path'=>'Contract.applies',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contract cessation cause
      # Event resulting in discontinuation or termination of this Contract instance by one or more parties to the contract.
      'expirationType' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/contract-expiration-type'=>[ 'breach' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Contract.expirationType',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-expiration-type'}
      },
      ##
      # Contract Target Entity
      # The target entity impacted by or of interest to parties to the agreement.
      # The Contract.subject is an entity that has some role with respect to the Contract.topic and Contract.topic.term, which is of focal interest to the parties to the contract and likely impacted in a significant way by the Contract.action/Contract.action.reason and the Contract.term.action/Contract.action.reason. In many cases, the Contract.subject is a Contract.signer if the subject is an adult; has a legal interest in the contract; and incompetent to participate in the contract agreement.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Contract.subject',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Authority under which this Contract has standing
      # A formally or informally recognized grouping of people, principals, organizations, or jurisdictions formed for the purpose of achieving some form of collective action such as the promulgation, administration and enforcement of contracts and policies.
      'authority' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Contract.authority',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A sphere of control governed by an authoritative jurisdiction, organization, or person
      # Recognized governance framework or system operating with a circumscribed scope in accordance with specified principles, policies, processes or procedures for managing rights, actions, or behaviors of parties or principals relative to resources.
      'domain' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Contract.domain',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Specific Location
      # Sites in which the contract is complied with,  exercised, or in force.
      'site' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Contract.site',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Computer friendly designation
      # A natural language name identifying this Contract definition, derivative, or instance in any legal state. Provides additional information about its content. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'Contract.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Human Friendly name
      # A short, descriptive, user-friendly title for this Contract definition, derivative, or instance in any legal state.t giving additional information about its content.
      'title' => {
        'type'=>'string',
        'path'=>'Contract.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # Subordinate Friendly name
      # An explanatory or alternate user-friendly title for this Contract definition, derivative, or instance in any legal state.t giving additional information about its content.
      'subtitle' => {
        'type'=>'string',
        'path'=>'Contract.subtitle',
        'min'=>0,
        'max'=>1
      },
      ##
      # Acronym or short name
      # Alternative representation of the title for this Contract definition, derivative, or instance in any legal state., e.g., a domain specific contract number related to legislation.
      'alias' => {
        'local_name'=>'local_alias'
        'type'=>'string',
        'path'=>'Contract.alias',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Source of Contract
      # The individual or organization that authored the Contract definition, derivative, or instance in any legal state.
      'author' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Contract.author',
        'min'=>0,
        'max'=>1
      },
      ##
      # Range of Legal Concerns
      # A selector of legal concerns for this Contract definition, derivative, or instance in any legal state.
      'scope' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/contract-scope'=>[ 'policy' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Contract.scope',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-scope'}
      },
      ##
      # Focus of contract interest
      # Narrows the range of legal concerns to focus on the achievement of specific contractual objectives.
      'topicCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'Contract.topic[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Focus of contract interest
      # Narrows the range of legal concerns to focus on the achievement of specific contractual objectives.
      'topicReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Contract.topic[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Legal instrument category
      # A high-level category for the legal instrument, whether constructed as a Contract definition, derivative, or instance in any legal state.  Provides additional information about its content within the context of the Contract's scope to distinguish the kinds of systems that would be interested in the contract.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/contract-type'=>[ 'privacy', 'disclosure', 'healthinsurance', 'supply', 'consent' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Contract.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-type'}
      },
      ##
      # Subtype within the context of type
      # Sub-category for the Contract that distinguishes the kinds of systems that would be interested in the Contract within the context of the Contract's scope.
      'subType' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/contractsubtypecodes'=>[ 'disclosure-ca', 'disclosure-us' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Contract.subType',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-subtype'}
      },
      ##
      # Contract precursor content
      # Precusory content developed with a focus and intent of supporting the formation a Contract instance, which may be associated with and transformable into a Contract.
      'contentDefinition' => {
        'type'=>'Contract::ContentDefinition',
        'path'=>'Contract.contentDefinition',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contract Term List
      # One or more Contract Provisions, which may be related and conveyed as a group, and may contain nested groups.
      'term' => {
        'type'=>'Contract::Term',
        'path'=>'Contract.term',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Extra Information
      # Information that may be needed by/relevant to the performer in their execution of this term action.
      'supportingInfo' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Contract.supportingInfo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Key event in Contract History
      # Links to Provenance records for past versions of this Contract definition, derivative, or instance, which identify key state transitions or updates that are likely to be relevant to a user looking at the current version of the Contract.  The Provence.entity indicates the target that was changed in the update. http://build.fhir.org/provenance-definitions.html#Provenance.entity.
      'relevantHistory' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Provenance'],
        'type'=>'Reference',
        'path'=>'Contract.relevantHistory',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Contract Signatory
      # Parties with legal standing in the Contract, including the principal parties, the grantor(s) and grantee(s), which are any person or organization bound by the contract, and any ancillary parties, which facilitate the execution of the contract such as a notary or witness.
      # Signers who are principal parties to the contract are bound by the Contract.activity related to the Contract.topic, and the Contract.term(s), which either extend or restrict the overall action on the topic by, for example, stipulating specific policies or obligations constraining actions, action reason, or agents with respect to some or all of the topic.For example, specifying how policies or obligations shall constrain actions and action reasons permitted or denied on all or a subset of the Contract.topic (e.g., all or a portion of property being transferred by the contract), agents (e.g., who can resell, assign interests, or alter the property being transferred by the contract), actions, and action reasons; or with respect to Contract.terms, stipulating, extending, or limiting the Contract.period of applicability or valuation of items under consideration.
      'signer' => {
        'type'=>'Contract::Signer',
        'path'=>'Contract.signer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Contract Friendly Language
      # The "patient friendly language" versionof the Contract in whole or in parts. "Patient friendly language" means the representation of the Contract and Contract Provisions in a manner that is readily accessible and understandable by a layperson in accordance with best practices for communication styles that ensure that those agreeing to or signing the Contract understand the roles, actions, obligations, responsibilities, and implication of the agreement.
      'friendly' => {
        'type'=>'Contract::Friendly',
        'path'=>'Contract.friendly',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Contract Legal Language
      # List of Legal expressions or representations of this Contract.
      'legal' => {
        'type'=>'Contract::Legal',
        'path'=>'Contract.legal',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Computable Contract Language
      # List of Computable Policy Rule Language Representations of this Contract.
      'rule' => {
        'type'=>'Contract::Rule',
        'path'=>'Contract.rule',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Binding Contract
      # Legally binding Contract: This is the signed and legally recognized representation of the Contract, which is considered the "source of truth" and which would be the basis for legal action related to enforcement of this Contract.
      'legallyBindingAttachment' => {
        'type'=>'Attachment',
        'path'=>'Contract.legallyBinding[x]',
        'min'=>0,
        'max'=>1
      }
      ##
      # Binding Contract
      # Legally binding Contract: This is the signed and legally recognized representation of the Contract, which is considered the "source of truth" and which would be the basis for legal action related to enforcement of this Contract.
      'legallyBindingReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Composition', 'http://hl7.org/fhir/StructureDefinition/DocumentReference', 'http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse', 'http://hl7.org/fhir/StructureDefinition/Contract'],
        'type'=>'Reference',
        'path'=>'Contract.legallyBinding[x]',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Contract precursor content
    # Precusory content developed with a focus and intent of supporting the formation a Contract instance, which may be associated with and transformable into a Contract.
    class ContentDefinition < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'ContentDefinition.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'ContentDefinition.extension',
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
          'path'=>'ContentDefinition.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Content structure and use
        # Precusory content structure and use, i.e., a boilerplate, template, application for a contract such as an insurance policy or benefits under a program, e.g., workers compensation.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/contract-definition-type'=>[ 'temp' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'ContentDefinition.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-definition-type'}
        },
        ##
        # Detailed Content Type Definition
        # Detailed Precusory content type.
        'subType' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/contract-definition-subtype'=>[ 'temp' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'ContentDefinition.subType',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-definition-subtype'}
        },
        ##
        # Publisher Entity
        # The  individual or organization that published the Contract precursor content.
        'publisher' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'ContentDefinition.publisher',
          'min'=>0,
          'max'=>1
        },
        ##
        # When published
        # The date (and optionally time) when the contract was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the contract changes.
        'publicationDate' => {
          'type'=>'dateTime',
          'path'=>'ContentDefinition.publicationDate',
          'min'=>0,
          'max'=>1
        },
        ##
        # amended | appended | cancelled | disputed | entered-in-error | executable | executed | negotiable | offered | policy | rejected | renewed | revoked | resolved | terminated.
        'publicationStatus' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/contract-publicationstatus'=>[ 'amended', 'appended', 'cancelled', 'disputed', 'entered-in-error', 'executable', 'executed', 'negotiable', 'offered', 'policy', 'rejected', 'renewed', 'revoked', 'resolved', 'terminated' ]
          },
          'type'=>'code',
          'path'=>'ContentDefinition.publicationStatus',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-publicationstatus'}
        },
        ##
        # Publication Ownership
        # A copyright statement relating to Contract precursor content. Copyright statements are generally legal restrictions on the use and publishing of the Contract precursor content.
        'copyright' => {
          'type'=>'markdown',
          'path'=>'ContentDefinition.copyright',
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
      # Content structure and use
      # Precusory content structure and use, i.e., a boilerplate, template, application for a contract such as an insurance policy or benefits under a program, e.g., workers compensation.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Detailed Content Type Definition
      # Detailed Precusory content type.
      attr_accessor :subType                        # 0-1 CodeableConcept
      ##
      # Publisher Entity
      # The  individual or organization that published the Contract precursor content.
      attr_accessor :publisher                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # When published
      # The date (and optionally time) when the contract was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the contract changes.
      attr_accessor :publicationDate                # 0-1 dateTime
      ##
      # amended | appended | cancelled | disputed | entered-in-error | executable | executed | negotiable | offered | policy | rejected | renewed | revoked | resolved | terminated.
      attr_accessor :publicationStatus              # 1-1 code
      ##
      # Publication Ownership
      # A copyright statement relating to Contract precursor content. Copyright statements are generally legal restrictions on the use and publishing of the Contract precursor content.
      attr_accessor :copyright                      # 0-1 markdown
    end

    ##
    # Contract Term List
    # One or more Contract Provisions, which may be related and conveyed as a group, and may contain nested groups.
    class Term < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'topic[x]' => ['CodeableConcept', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Term.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Term.extension',
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
          'path'=>'Term.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Contract Term Number
        # Unique identifier for this particular Contract Provision.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'Term.identifier',
          'min'=>0,
          'max'=>1
        },
        ##
        # Contract Term Issue Date Time
        # When this Contract Provision was issued.
        'issued' => {
          'type'=>'dateTime',
          'path'=>'Term.issued',
          'min'=>0,
          'max'=>1
        },
        ##
        # Contract Term Effective Time
        # Relevant time or time-period when this Contract Provision is applicable.
        'applies' => {
          'type'=>'Period',
          'path'=>'Term.applies',
          'min'=>0,
          'max'=>1
        },
        ##
        # Term Concern
        # The entity that the term applies to.
        'topicCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Term.topic[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Term Concern
        # The entity that the term applies to.
        'topicReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Term.topic[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Contract Term Type or Form
        # A legal clause or condition contained within a contract that requires one or both parties to perform a particular requirement by some specified time or prevents one or both parties from performing a particular requirement by some specified time.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/contracttermtypecodes'=>[ 'statutory', 'subject-to' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Term.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-term-type'}
        },
        ##
        # Contract Term Type specific classification
        # A specialized legal clause or condition based on overarching contract type.
        'subType' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/contracttermsubtypecodes'=>[ 'condition', 'warranty', 'innominate' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Term.subType',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-term-subtype'}
        },
        ##
        # Term Statement
        # Statement of a provision in a policy or a contract.
        'text' => {
          'type'=>'string',
          'path'=>'Term.text',
          'min'=>0,
          'max'=>1
        },
        ##
        # Protection for the Term
        # Security labels that protect the handling of information about the term and its elements, which may be specifically identified..
        'securityLabel' => {
          'type'=>'Contract::Term::SecurityLabel',
          'path'=>'Term.securityLabel',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Context of the Contract term
        # The matter of concern in the context of this provision of the agrement.
        'offer' => {
          'type'=>'Contract::Term::Offer',
          'path'=>'Term.offer',
          'min'=>1,
          'max'=>1
        },
        ##
        # Contract Term Asset List.
        'asset' => {
          'type'=>'Contract::Term::Asset',
          'path'=>'Term.asset',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Entity being ascribed responsibility
        # An actor taking a role in an activity for which it can be assigned some degree of responsibility for the activity taking place.
        # Several agents may be associated (i.e. has some responsibility for an activity) with an activity and vice-versa.For example, in cases of actions initiated by one user for other users, or in events that involve more than one user, hardware device, software, or system process. However, only one user may be the initiator/requestor for the event.
        'action' => {
          'type'=>'Contract::Term::Action',
          'path'=>'Term.action',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Nested Contract Term Group
        # Nested group of Contract Provisions.
        'group' => {
          'type'=>'Contract::Term',
          'path'=>'Term.group',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Protection for the Term
      # Security labels that protect the handling of information about the term and its elements, which may be specifically identified..
      class SecurityLabel < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'SecurityLabel.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'SecurityLabel.extension',
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
            'path'=>'SecurityLabel.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Link to Security Labels
          # Number used to link this term or term element to the applicable Security Label.
          'number' => {
            'type'=>'unsignedInt',
            'path'=>'SecurityLabel.number',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Confidentiality Protection
          # Security label privacy tag that species the level of confidentiality protection required for this term and/or term elements.
          'classification' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/contract-security-classification'=>[ 'policy' ]
            },
            'type'=>'Coding',
            'path'=>'SecurityLabel.classification',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-security-classification'}
          },
          ##
          # Applicable Policy
          # Security label privacy tag that species the applicable privacy and security policies governing this term and/or term elements.
          'category' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/contract-security-category'=>[ 'policy' ]
            },
            'type'=>'Coding',
            'path'=>'SecurityLabel.category',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-security-category'}
          },
          ##
          # Handling Instructions
          # Security label privacy tag that species the manner in which term and/or term elements are to be protected.
          'control' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/contract-security-control'=>[ 'policy' ]
            },
            'type'=>'Coding',
            'path'=>'SecurityLabel.control',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-security-control'}
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
        # Link to Security Labels
        # Number used to link this term or term element to the applicable Security Label.
        attr_accessor :number                         # 0-* [ unsignedInt ]
        ##
        # Confidentiality Protection
        # Security label privacy tag that species the level of confidentiality protection required for this term and/or term elements.
        attr_accessor :classification                 # 1-1 Coding
        ##
        # Applicable Policy
        # Security label privacy tag that species the applicable privacy and security policies governing this term and/or term elements.
        attr_accessor :category                       # 0-* [ Coding ]
        ##
        # Handling Instructions
        # Security label privacy tag that species the manner in which term and/or term elements are to be protected.
        attr_accessor :control                        # 0-* [ Coding ]
      end

      ##
      # Context of the Contract term
      # The matter of concern in the context of this provision of the agrement.
      class Offer < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Offer.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Offer.extension',
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
            'path'=>'Offer.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Offer business ID
          # Unique identifier for this particular Contract Provision.
          'identifier' => {
            'type'=>'Identifier',
            'path'=>'Offer.identifier',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Offer Recipient.
          'party' => {
            'type'=>'Contract::Term::Offer::Party',
            'path'=>'Offer.party',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Negotiable offer asset
          # The owner of an asset has the residual control rights over the asset: the right to decide all usages of the asset in any way not inconsistent with a prior contract, custom, or law (Hart, 1995, p. 30).
          # The Contract.topic may be an application for or offer of a policy or service (e.g., uri to a consent directive form or a health insurance policy), which becomes the Contract once accepted by both the grantor and grantee. The Contract Resource may function simply as the computable representation of the executed contract, which may be the attached to the Contract Resource as the “binding” or as the “friendly” electronic form.  For example, a Contract Resource may be automatically populated with the values expressed in a related QuestionnaireResponse. However, the Contract Resource may be considered the legally binding contract if it is the only “executed” form of this contract, and includes the signatures as *The Contract Resource may function as the computable representation of an application or offer in a pre-executed Contract if the grantor has not entered any values.  In this case, it is populated with values in a “legal” form of the application or offer or by the values in an associated Questionnaire.  If the grantor has filled in the legal form or the associated Questionnaire Response, then these values are used to populate a pre-executed Contract Resource.If the Contract.topic is considered an application or offer, then the policy is often required to be attached as the “legal” basis for the application to ensure “informed consent” to the contract, and that any discrepancy between the application and the policy are interpreted against the policy.  Implementers should check organizational and jurisdictional policies to determine the relationship among multiple representations of a contract pre- and post-execution.
          'topic' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
            'type'=>'Reference',
            'path'=>'Offer.topic',
            'min'=>0,
            'max'=>1
          },
          ##
          # Contract Offer Type or Form
          # Type of Contract Provision such as specific requirements, purposes for actions, obligations, prohibitions, e.g. life time maximum benefit.
          'type' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/contracttermtypecodes'=>[ 'statutory', 'subject-to' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Offer.type',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-term-type'}
          },
          ##
          # Accepting party choice
          # Type of choice made by accepting party with respect to an offer made by an offeror/ grantee.
          'decision' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'EMRGONLY', 'GRANTORCHOICE', 'IMPLIED', 'IMPLIEDD', 'NOCONSENT', 'NOPP', 'OPTIN', 'OPTINR', 'OPTOUT', 'OPTOUTE' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Offer.decision',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'extensible', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ActConsentDirective'}
          },
          ##
          # How decision is conveyed
          # How the decision about a Contract was conveyed.
          'decisionMode' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/contract-decision-mode'=>[ 'policy' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Offer.decisionMode',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-decision-mode'}
          },
          ##
          # Response to offer text.
          'answer' => {
            'type'=>'Contract::Term::Offer::Answer',
            'path'=>'Offer.answer',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Human readable offer text
          # Human readable form of this Contract Offer.
          'text' => {
            'type'=>'string',
            'path'=>'Offer.text',
            'min'=>0,
            'max'=>1
          },
          ##
          # Pointer to text
          # The id of the clause or question text of the offer in the referenced questionnaire/response.
          'linkId' => {
            'type'=>'string',
            'path'=>'Offer.linkId',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Offer restriction numbers
          # Security labels that protects the offer.
          'securityLabelNumber' => {
            'type'=>'unsignedInt',
            'path'=>'Offer.securityLabelNumber',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Offer Recipient.
        class Party < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'id',
              'path'=>'Party.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Party.extension',
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
              'path'=>'Party.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Referenced entity
            # Participant in the offer.
            'reference' => {
              'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Organization'],
              'type'=>'Reference',
              'path'=>'Party.reference',
              'min'=>1,
              'max'=>Float::INFINITY
            },
            ##
            # Participant engagement type
            # How the party participates in the offer.
            'role' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/contract-party-role'=>[ 'flunky' ]
              },
              'type'=>'CodeableConcept',
              'path'=>'Party.role',
              'min'=>1,
              'max'=>1,
              'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-party-role'}
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
          # Referenced entity
          # Participant in the offer.
          attr_accessor :reference                      # 1-* [ Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Organization) ]
          ##
          # Participant engagement type
          # How the party participates in the offer.
          attr_accessor :role                           # 1-1 CodeableConcept
        end

        ##
        # Response to offer text.
        class Answer < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          MULTIPLE_TYPES = {
            'value[x]' => ['Attachment', 'boolean', 'Coding', 'date', 'dateTime', 'decimal', 'integer', 'Quantity', 'Reference', 'string', 'time', 'uri']
          }
          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'id',
              'path'=>'Answer.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Answer.extension',
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
              'path'=>'Answer.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueAttachment' => {
              'type'=>'Attachment',
              'path'=>'Answer.value[x]',
              'min'=>1,
              'max'=>1
            }
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueBoolean' => {
              'type'=>'Boolean',
              'path'=>'Answer.value[x]',
              'min'=>1,
              'max'=>1
            }
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueCoding' => {
              'type'=>'Coding',
              'path'=>'Answer.value[x]',
              'min'=>1,
              'max'=>1
            }
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueDate' => {
              'type'=>'Date',
              'path'=>'Answer.value[x]',
              'min'=>1,
              'max'=>1
            }
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueDateTime' => {
              'type'=>'DateTime',
              'path'=>'Answer.value[x]',
              'min'=>1,
              'max'=>1
            }
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueDecimal' => {
              'type'=>'Decimal',
              'path'=>'Answer.value[x]',
              'min'=>1,
              'max'=>1
            }
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueInteger' => {
              'type'=>'Integer',
              'path'=>'Answer.value[x]',
              'min'=>1,
              'max'=>1
            }
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueQuantity' => {
              'type'=>'Quantity',
              'path'=>'Answer.value[x]',
              'min'=>1,
              'max'=>1
            }
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueReference' => {
              'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
              'type'=>'Reference',
              'path'=>'Answer.value[x]',
              'min'=>1,
              'max'=>1
            }
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueString' => {
              'type'=>'String',
              'path'=>'Answer.value[x]',
              'min'=>1,
              'max'=>1
            }
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueTime' => {
              'type'=>'Time',
              'path'=>'Answer.value[x]',
              'min'=>1,
              'max'=>1
            }
            ##
            # The actual answer response
            # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
            'valueUri' => {
              'type'=>'Uri',
              'path'=>'Answer.value[x]',
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
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueAttachment                # 1-1 Attachment
          ##
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueBoolean                   # 1-1 Boolean
          ##
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueCoding                    # 1-1 Coding
          ##
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueDate                      # 1-1 Date
          ##
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueDateTime                  # 1-1 DateTime
          ##
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueDecimal                   # 1-1 Decimal
          ##
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueInteger                   # 1-1 Integer
          ##
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueQuantity                  # 1-1 Quantity
          ##
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueReference                 # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
          ##
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueString                    # 1-1 String
          ##
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueTime                      # 1-1 Time
          ##
          # The actual answer response
          # Response to an offer clause or question text,  which enables selection of values to be agreed to, e.g., the period of participation, the date of occupancy of a rental, warrently duration, or whether biospecimen may be used for further research.
          attr_accessor :valueUri                       # 1-1 Uri
        end
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
        # Offer business ID
        # Unique identifier for this particular Contract Provision.
        attr_accessor :identifier                     # 0-* [ Identifier ]
        ##
        # Offer Recipient.
        attr_accessor :party                          # 0-* [ Contract::Term::Offer::Party ]
        ##
        # Negotiable offer asset
        # The owner of an asset has the residual control rights over the asset: the right to decide all usages of the asset in any way not inconsistent with a prior contract, custom, or law (Hart, 1995, p. 30).
        # The Contract.topic may be an application for or offer of a policy or service (e.g., uri to a consent directive form or a health insurance policy), which becomes the Contract once accepted by both the grantor and grantee. The Contract Resource may function simply as the computable representation of the executed contract, which may be the attached to the Contract Resource as the “binding” or as the “friendly” electronic form.  For example, a Contract Resource may be automatically populated with the values expressed in a related QuestionnaireResponse. However, the Contract Resource may be considered the legally binding contract if it is the only “executed” form of this contract, and includes the signatures as *The Contract Resource may function as the computable representation of an application or offer in a pre-executed Contract if the grantor has not entered any values.  In this case, it is populated with values in a “legal” form of the application or offer or by the values in an associated Questionnaire.  If the grantor has filled in the legal form or the associated Questionnaire Response, then these values are used to populate a pre-executed Contract Resource.If the Contract.topic is considered an application or offer, then the policy is often required to be attached as the “legal” basis for the application to ensure “informed consent” to the contract, and that any discrepancy between the application and the policy are interpreted against the policy.  Implementers should check organizational and jurisdictional policies to determine the relationship among multiple representations of a contract pre- and post-execution.
        attr_accessor :topic                          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
        ##
        # Contract Offer Type or Form
        # Type of Contract Provision such as specific requirements, purposes for actions, obligations, prohibitions, e.g. life time maximum benefit.
        attr_accessor :type                           # 0-1 CodeableConcept
        ##
        # Accepting party choice
        # Type of choice made by accepting party with respect to an offer made by an offeror/ grantee.
        attr_accessor :decision                       # 0-1 CodeableConcept
        ##
        # How decision is conveyed
        # How the decision about a Contract was conveyed.
        attr_accessor :decisionMode                   # 0-* [ CodeableConcept ]
        ##
        # Response to offer text.
        attr_accessor :answer                         # 0-* [ Contract::Term::Offer::Answer ]
        ##
        # Human readable offer text
        # Human readable form of this Contract Offer.
        attr_accessor :text                           # 0-1 string
        ##
        # Pointer to text
        # The id of the clause or question text of the offer in the referenced questionnaire/response.
        attr_accessor :linkId                         # 0-* [ string ]
        ##
        # Offer restriction numbers
        # Security labels that protects the offer.
        attr_accessor :securityLabelNumber            # 0-* [ unsignedInt ]
      end

      ##
      # Contract Term Asset List.
      class Asset < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Asset.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Asset.extension',
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
            'path'=>'Asset.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Range of asset
          # Differentiates the kind of the asset .
          'scope' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/contract-asset-scope'=>[ 'thing' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Asset.scope',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-assetscope'}
          },
          ##
          # Asset category
          # Target entity type about which the term may be concerned.
          'type' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/contract-asset-type'=>[ 'participation' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Asset.type',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-assettype'}
          },
          ##
          # Associated entities.
          'typeReference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
            'type'=>'Reference',
            'path'=>'Asset.typeReference',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Asset sub-category
          # May be a subtype or part of an offered asset.
          'subtype' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/contract-asset-subtype'=>[ 'participation' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Asset.subtype',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-assetsubtype'}
          },
          ##
          # Kinship of the asset
          # Specifies the applicability of the term to an asset resource instance, and instances it refers to orinstances that refer to it, and/or are owned by the offeree.
          'relationship' => {
            'type'=>'Coding',
            'path'=>'Asset.relationship',
            'min'=>0,
            'max'=>1
          },
          ##
          # Circumstance of the asset.
          'context' => {
            'type'=>'Contract::Term::Asset::Context',
            'path'=>'Asset.context',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Quality desctiption of asset
          # Description of the quality and completeness of the asset that imay be a factor in its valuation.
          'condition' => {
            'type'=>'string',
            'path'=>'Asset.condition',
            'min'=>0,
            'max'=>1
          },
          ##
          # Asset availability types
          # Type of Asset availability for use or ownership.
          'periodType' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/asset-availability'=>[ 'lease' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Asset.periodType',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/asset-availability'}
          },
          ##
          # Time period of the asset
          # Asset relevant contractual time period.
          'period' => {
            'type'=>'Period',
            'path'=>'Asset.period',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Time period of asset use.
          'usePeriod' => {
            'type'=>'Period',
            'path'=>'Asset.usePeriod',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Asset clause or question text
          # Clause or question text (Prose Object) concerning the asset in a linked form, such as a QuestionnaireResponse used in the formation of the contract.
          'text' => {
            'type'=>'string',
            'path'=>'Asset.text',
            'min'=>0,
            'max'=>1
          },
          ##
          # Pointer to asset text
          # Id [identifier??] of the clause or question text about the asset in the referenced form or QuestionnaireResponse.
          'linkId' => {
            'type'=>'string',
            'path'=>'Asset.linkId',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Response to assets.
          'answer' => {
            'type'=>'Contract::Term::Offer::Answer',
            'path'=>'Asset.answer',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Asset restriction numbers
          # Security labels that protects the asset.
          'securityLabelNumber' => {
            'type'=>'unsignedInt',
            'path'=>'Asset.securityLabelNumber',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Contract Valued Item List.
          'valuedItem' => {
            'type'=>'Contract::Term::Asset::ValuedItem',
            'path'=>'Asset.valuedItem',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Circumstance of the asset.
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
            # Creator,custodian or owner
            # Asset context reference may include the creator, custodian, or owning Person or Organization (e.g., bank, repository),  location held, e.g., building,  jurisdiction.
            'reference' => {
              'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
              'type'=>'Reference',
              'path'=>'Context.reference',
              'min'=>0,
              'max'=>1
            },
            ##
            # Codeable asset context
            # Coded representation of the context generally or of the Referenced entity, such as the asset holder type or location.
            'code' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/contract-asset-context'=>[ 'custodian' ]
              },
              'type'=>'CodeableConcept',
              'path'=>'Context.code',
              'min'=>0,
              'max'=>Float::INFINITY,
              'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-assetcontext'}
            },
            ##
            # Context description.
            'text' => {
              'type'=>'string',
              'path'=>'Context.text',
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
          # Creator,custodian or owner
          # Asset context reference may include the creator, custodian, or owning Person or Organization (e.g., bank, repository),  location held, e.g., building,  jurisdiction.
          attr_accessor :reference                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
          ##
          # Codeable asset context
          # Coded representation of the context generally or of the Referenced entity, such as the asset holder type or location.
          attr_accessor :code                           # 0-* [ CodeableConcept ]
          ##
          # Context description.
          attr_accessor :text                           # 0-1 string
        end

        ##
        # Contract Valued Item List.
        class ValuedItem < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          MULTIPLE_TYPES = {
            'entity[x]' => ['CodeableConcept', 'Reference']
          }
          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'id',
              'path'=>'ValuedItem.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'ValuedItem.extension',
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
              'path'=>'ValuedItem.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Contract Valued Item Type
            # Specific type of Contract Valued Item that may be priced.
            'entityCodeableConcept' => {
              'type'=>'CodeableConcept',
              'path'=>'ValuedItem.entity[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Contract Valued Item Type
            # Specific type of Contract Valued Item that may be priced.
            'entityReference' => {
              'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
              'type'=>'Reference',
              'path'=>'ValuedItem.entity[x]',
              'min'=>0,
              'max'=>1
            },
            ##
            # Contract Valued Item Number
            # Identifies a Contract Valued Item instance.
            'identifier' => {
              'type'=>'Identifier',
              'path'=>'ValuedItem.identifier',
              'min'=>0,
              'max'=>1
            },
            ##
            # Contract Valued Item Effective Tiem
            # Indicates the time during which this Contract ValuedItem information is effective.
            'effectiveTime' => {
              'type'=>'dateTime',
              'path'=>'ValuedItem.effectiveTime',
              'min'=>0,
              'max'=>1
            },
            ##
            # Count of Contract Valued Items
            # Specifies the units by which the Contract Valued Item is measured or counted, and quantifies the countable or measurable Contract Valued Item instances.
            'quantity' => {
              'type'=>'Quantity',
              'path'=>'ValuedItem.quantity',
              'min'=>0,
              'max'=>1
            },
            ##
            # Contract Valued Item fee, charge, or cost
            # A Contract Valued Item unit valuation measure.
            'unitPrice' => {
              'type'=>'Money',
              'path'=>'ValuedItem.unitPrice',
              'min'=>0,
              'max'=>1
            },
            ##
            # Contract Valued Item Price Scaling Factor
            # A real number that represents a multiplier used in determining the overall value of the Contract Valued Item delivered. The concept of a Factor allows for a discount or surcharge multiplier to be applied to a monetary amount.
            'factor' => {
              'type'=>'decimal',
              'path'=>'ValuedItem.factor',
              'min'=>0,
              'max'=>1
            },
            ##
            # Contract Valued Item Difficulty Scaling Factor
            # An amount that expresses the weighting (based on difficulty, cost and/or resource intensiveness) associated with the Contract Valued Item delivered. The concept of Points allows for assignment of point values for a Contract Valued Item, such that a monetary amount can be assigned to each point.
            'points' => {
              'type'=>'decimal',
              'path'=>'ValuedItem.points',
              'min'=>0,
              'max'=>1
            },
            ##
            # Total Contract Valued Item Value
            # Expresses the product of the Contract Valued Item unitQuantity and the unitPriceAmt. For example, the formula: unit Quantity * unit Price (Cost per Point) * factor Number  * points = net Amount. Quantity, factor and points are assumed to be 1 if not supplied.
            'net' => {
              'type'=>'Money',
              'path'=>'ValuedItem.net',
              'min'=>0,
              'max'=>1
            },
            ##
            # Terms of valuation.
            'payment' => {
              'type'=>'string',
              'path'=>'ValuedItem.payment',
              'min'=>0,
              'max'=>1
            },
            ##
            # When payment is due.
            'paymentDate' => {
              'type'=>'dateTime',
              'path'=>'ValuedItem.paymentDate',
              'min'=>0,
              'max'=>1
            },
            ##
            # Who will make payment.
            'responsible' => {
              'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
              'type'=>'Reference',
              'path'=>'ValuedItem.responsible',
              'min'=>0,
              'max'=>1
            },
            ##
            # Who will receive payment.
            'recipient' => {
              'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
              'type'=>'Reference',
              'path'=>'ValuedItem.recipient',
              'min'=>0,
              'max'=>1
            },
            ##
            # Pointer to specific item
            # Id  of the clause or question text related to the context of this valuedItem in the referenced form or QuestionnaireResponse.
            'linkId' => {
              'type'=>'string',
              'path'=>'ValuedItem.linkId',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Security Labels that define affected terms
            # A set of security labels that define which terms are controlled by this condition.
            'securityLabelNumber' => {
              'type'=>'unsignedInt',
              'path'=>'ValuedItem.securityLabelNumber',
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
          # Contract Valued Item Type
          # Specific type of Contract Valued Item that may be priced.
          attr_accessor :entityCodeableConcept          # 0-1 CodeableConcept
          ##
          # Contract Valued Item Type
          # Specific type of Contract Valued Item that may be priced.
          attr_accessor :entityReference                # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
          ##
          # Contract Valued Item Number
          # Identifies a Contract Valued Item instance.
          attr_accessor :identifier                     # 0-1 Identifier
          ##
          # Contract Valued Item Effective Tiem
          # Indicates the time during which this Contract ValuedItem information is effective.
          attr_accessor :effectiveTime                  # 0-1 dateTime
          ##
          # Count of Contract Valued Items
          # Specifies the units by which the Contract Valued Item is measured or counted, and quantifies the countable or measurable Contract Valued Item instances.
          attr_accessor :quantity                       # 0-1 Quantity
          ##
          # Contract Valued Item fee, charge, or cost
          # A Contract Valued Item unit valuation measure.
          attr_accessor :unitPrice                      # 0-1 Money
          ##
          # Contract Valued Item Price Scaling Factor
          # A real number that represents a multiplier used in determining the overall value of the Contract Valued Item delivered. The concept of a Factor allows for a discount or surcharge multiplier to be applied to a monetary amount.
          attr_accessor :factor                         # 0-1 decimal
          ##
          # Contract Valued Item Difficulty Scaling Factor
          # An amount that expresses the weighting (based on difficulty, cost and/or resource intensiveness) associated with the Contract Valued Item delivered. The concept of Points allows for assignment of point values for a Contract Valued Item, such that a monetary amount can be assigned to each point.
          attr_accessor :points                         # 0-1 decimal
          ##
          # Total Contract Valued Item Value
          # Expresses the product of the Contract Valued Item unitQuantity and the unitPriceAmt. For example, the formula: unit Quantity * unit Price (Cost per Point) * factor Number  * points = net Amount. Quantity, factor and points are assumed to be 1 if not supplied.
          attr_accessor :net                            # 0-1 Money
          ##
          # Terms of valuation.
          attr_accessor :payment                        # 0-1 string
          ##
          # When payment is due.
          attr_accessor :paymentDate                    # 0-1 dateTime
          ##
          # Who will make payment.
          attr_accessor :responsible                    # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
          ##
          # Who will receive payment.
          attr_accessor :recipient                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
          ##
          # Pointer to specific item
          # Id  of the clause or question text related to the context of this valuedItem in the referenced form or QuestionnaireResponse.
          attr_accessor :linkId                         # 0-* [ string ]
          ##
          # Security Labels that define affected terms
          # A set of security labels that define which terms are controlled by this condition.
          attr_accessor :securityLabelNumber            # 0-* [ unsignedInt ]
        end
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
        # Range of asset
        # Differentiates the kind of the asset .
        attr_accessor :scope                          # 0-1 CodeableConcept
        ##
        # Asset category
        # Target entity type about which the term may be concerned.
        attr_accessor :type                           # 0-* [ CodeableConcept ]
        ##
        # Associated entities.
        attr_accessor :typeReference                  # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
        ##
        # Asset sub-category
        # May be a subtype or part of an offered asset.
        attr_accessor :subtype                        # 0-* [ CodeableConcept ]
        ##
        # Kinship of the asset
        # Specifies the applicability of the term to an asset resource instance, and instances it refers to orinstances that refer to it, and/or are owned by the offeree.
        attr_accessor :relationship                   # 0-1 Coding
        ##
        # Circumstance of the asset.
        attr_accessor :context                        # 0-* [ Contract::Term::Asset::Context ]
        ##
        # Quality desctiption of asset
        # Description of the quality and completeness of the asset that imay be a factor in its valuation.
        attr_accessor :condition                      # 0-1 string
        ##
        # Asset availability types
        # Type of Asset availability for use or ownership.
        attr_accessor :periodType                     # 0-* [ CodeableConcept ]
        ##
        # Time period of the asset
        # Asset relevant contractual time period.
        attr_accessor :period                         # 0-* [ Period ]
        ##
        # Time period of asset use.
        attr_accessor :usePeriod                      # 0-* [ Period ]
        ##
        # Asset clause or question text
        # Clause or question text (Prose Object) concerning the asset in a linked form, such as a QuestionnaireResponse used in the formation of the contract.
        attr_accessor :text                           # 0-1 string
        ##
        # Pointer to asset text
        # Id [identifier??] of the clause or question text about the asset in the referenced form or QuestionnaireResponse.
        attr_accessor :linkId                         # 0-* [ string ]
        ##
        # Response to assets.
        attr_accessor :answer                         # 0-* [ Contract::Term::Offer::Answer ]
        ##
        # Asset restriction numbers
        # Security labels that protects the asset.
        attr_accessor :securityLabelNumber            # 0-* [ unsignedInt ]
        ##
        # Contract Valued Item List.
        attr_accessor :valuedItem                     # 0-* [ Contract::Term::Asset::ValuedItem ]
      end

      ##
      # Entity being ascribed responsibility
      # An actor taking a role in an activity for which it can be assigned some degree of responsibility for the activity taking place.
      # Several agents may be associated (i.e. has some responsibility for an activity) with an activity and vice-versa.For example, in cases of actions initiated by one user for other users, or in events that involve more than one user, hardware device, software, or system process. However, only one user may be the initiator/requestor for the event.
      class Action < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'occurrence[x]' => ['dateTime', 'Period', 'Timing']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Action.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Action.extension',
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
            'path'=>'Action.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # True if the term prohibits the  action.
          'doNotPerform' => {
            'type'=>'boolean',
            'path'=>'Action.doNotPerform',
            'min'=>0,
            'max'=>1
          },
          ##
          # Type or form of the action
          # Activity or service obligation to be done or not done, performed or not performed, effectuated or not by this Contract term.
          'type' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/contractaction'=>[ 'action-a', 'action-b' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Action.type',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-action'}
          },
          ##
          # Entity of the action.
          'subject' => {
            'type'=>'Contract::Term::Action::Subject',
            'path'=>'Action.subject',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Purpose for the Contract Term Action
          # Reason or purpose for the action stipulated by this Contract Provision.
          'intent' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/v3-ActReason'=>[ 'PurposeOfUse', 'HMARKT', 'HOPERAT', 'CAREMGT', 'DONAT', 'FRAUD', 'GOV', 'HACCRED', 'HCOMPL', 'HDECD', 'HDIRECT', 'HDM', 'HLEGAL', 'HOUTCOMS', 'HPRGRP', 'HQUALIMP', 'HSYSADMIN', 'LABELING', 'METAMGT', 'MEMADMIN', 'MILCDM', 'PATADMIN', 'PATSFTY', 'PERFMSR', 'RECORDMGT', 'SYSDEV', 'HTEST', 'TRAIN', 'HPAYMT', 'CLMATTCH', 'COVAUTH', 'COVERAGE', 'ELIGDTRM', 'ELIGVER', 'ENROLLM', 'MILDCRG', 'REMITADV', 'HRESCH', 'BIORCH', 'CLINTRCH', 'CLINTRCHNPC', 'CLINTRCHPC', 'PRECLINTRCH', 'DSRCH', 'POARCH', 'TRANSRCH', 'PATRQT', 'FAMRQT', 'PWATRNY', 'SUPNWK', 'PUBHLTH', 'DISASTER', 'THREAT', 'TREAT', 'CLINTRL', 'COC', 'ETREAT', 'BTG', 'ERTREAT', 'POPHLTH' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Action.intent',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-PurposeOfUse'}
          },
          ##
          # Pointer to specific item
          # Id [identifier??] of the clause or question text related to this action in the referenced form or QuestionnaireResponse.
          'linkId' => {
            'type'=>'string',
            'path'=>'Action.linkId',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # State of the action
          # Current state of the term action.
          'status' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/contract-action-status'=>[ 'complete' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Action.status',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-actionstatus'}
          },
          ##
          # Episode associated with action
          # Encounter or Episode with primary association to specified term activity.
          'context' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter', 'http://hl7.org/fhir/StructureDefinition/EpisodeOfCare'],
            'type'=>'Reference',
            'path'=>'Action.context',
            'min'=>0,
            'max'=>1
          },
          ##
          # Pointer to specific item
          # Id [identifier??] of the clause or question text related to the requester of this action in the referenced form or QuestionnaireResponse.
          'contextLinkId' => {
            'type'=>'string',
            'path'=>'Action.contextLinkId',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # When action happens.
          'occurrenceDateTime' => {
            'type'=>'DateTime',
            'path'=>'Action.occurrence[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # When action happens.
          'occurrencePeriod' => {
            'type'=>'Period',
            'path'=>'Action.occurrence[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # When action happens.
          'occurrenceTiming' => {
            'type'=>'Timing',
            'path'=>'Action.occurrence[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Who asked for action
          # Who or what initiated the action and has responsibility for its activation.
          'requester' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Organization'],
            'type'=>'Reference',
            'path'=>'Action.requester',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Pointer to specific item
          # Id [identifier??] of the clause or question text related to the requester of this action in the referenced form or QuestionnaireResponse.
          'requesterLinkId' => {
            'type'=>'string',
            'path'=>'Action.requesterLinkId',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Kind of service performer
          # The type of individual that is desired or required to perform or not perform the action.
          'performerType' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/provenance-participant-type'=>[ 'enterer', 'performer', 'author', 'verifier', 'legal', 'attester', 'informant', 'custodian', 'assembler', 'composer' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Action.performerType',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/provenance-agent-type'}
          },
          ##
          # Competency of the performer
          # The type of role or competency of an individual desired or required to perform or not perform the action.
          'performerRole' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/provenance-participant-role'=>[ 'enterer', 'performer', 'author', 'verifier', 'legal', 'attester', 'informant', 'custodian', 'assembler', 'composer' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Action.performerRole',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/provenance-agent-role'}
          },
          ##
          # Actor that wil execute (or not) the action
          # Indicates who or what is being asked to perform (or not perform) the ction.
          'performer' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Substance', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Location'],
            'type'=>'Reference',
            'path'=>'Action.performer',
            'min'=>0,
            'max'=>1
          },
          ##
          # Pointer to specific item
          # Id [identifier??] of the clause or question text related to the reason type or reference of this  action in the referenced form or QuestionnaireResponse.
          'performerLinkId' => {
            'type'=>'string',
            'path'=>'Action.performerLinkId',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Why is action (not) needed?
          # Rationale for the action to be performed or not performed. Describes why the action is permitted or prohibited.
          'reasonCode' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/v3-ActReason'=>[ 'PurposeOfUse', 'HMARKT', 'HOPERAT', 'CAREMGT', 'DONAT', 'FRAUD', 'GOV', 'HACCRED', 'HCOMPL', 'HDECD', 'HDIRECT', 'HDM', 'HLEGAL', 'HOUTCOMS', 'HPRGRP', 'HQUALIMP', 'HSYSADMIN', 'LABELING', 'METAMGT', 'MEMADMIN', 'MILCDM', 'PATADMIN', 'PATSFTY', 'PERFMSR', 'RECORDMGT', 'SYSDEV', 'HTEST', 'TRAIN', 'HPAYMT', 'CLMATTCH', 'COVAUTH', 'COVERAGE', 'ELIGDTRM', 'ELIGVER', 'ENROLLM', 'MILDCRG', 'REMITADV', 'HRESCH', 'BIORCH', 'CLINTRCH', 'CLINTRCHNPC', 'CLINTRCHPC', 'PRECLINTRCH', 'DSRCH', 'POARCH', 'TRANSRCH', 'PATRQT', 'FAMRQT', 'PWATRNY', 'SUPNWK', 'PUBHLTH', 'DISASTER', 'THREAT', 'TREAT', 'CLINTRL', 'COC', 'ETREAT', 'BTG', 'ERTREAT', 'POPHLTH' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Action.reasonCode',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-PurposeOfUse'}
          },
          ##
          # Why is action (not) needed?
          # Indicates another resource whose existence justifies permitting or not permitting this action.
          'reasonReference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference', 'http://hl7.org/fhir/StructureDefinition/Questionnaire', 'http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse'],
            'type'=>'Reference',
            'path'=>'Action.reasonReference',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Why action is to be performed
          # Describes why the action is to be performed or not performed in textual form.
          'reason' => {
            'type'=>'string',
            'path'=>'Action.reason',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Pointer to specific item
          # Id [identifier??] of the clause or question text related to the reason type or reference of this  action in the referenced form or QuestionnaireResponse.
          'reasonLinkId' => {
            'type'=>'string',
            'path'=>'Action.reasonLinkId',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Comments about the action
          # Comments made about the term action made by the requester, performer, subject or other participants.
          'note' => {
            'type'=>'Annotation',
            'path'=>'Action.note',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Action restriction numbers
          # Security labels that protects the action.
          'securityLabelNumber' => {
            'type'=>'unsignedInt',
            'path'=>'Action.securityLabelNumber',
            'min'=>0,
            'max'=>Float::INFINITY
          }
        }

        ##
        # Entity of the action.
        class Subject < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'id',
              'path'=>'Subject.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Subject.extension',
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
              'path'=>'Subject.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Entity of the action
            # The entity the action is performed or not performed on or for.
            'reference' => {
              'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Organization'],
              'type'=>'Reference',
              'path'=>'Subject.reference',
              'min'=>1,
              'max'=>Float::INFINITY
            },
            ##
            # Role type of the agent
            # Role type of agent assigned roles in this Contract.
            'role' => {
              'valid_codes'=>{
                'http://terminology.hl7.org/CodeSystem/contractactorrole'=>[ 'practitioner', 'patient' ]
              },
              'type'=>'CodeableConcept',
              'path'=>'Subject.role',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-actorrole'}
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
          # Entity of the action
          # The entity the action is performed or not performed on or for.
          attr_accessor :reference                      # 1-* [ Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Organization) ]
          ##
          # Role type of the agent
          # Role type of agent assigned roles in this Contract.
          attr_accessor :role                           # 0-1 CodeableConcept
        end
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
        # True if the term prohibits the  action.
        attr_accessor :doNotPerform                   # 0-1 boolean
        ##
        # Type or form of the action
        # Activity or service obligation to be done or not done, performed or not performed, effectuated or not by this Contract term.
        attr_accessor :type                           # 1-1 CodeableConcept
        ##
        # Entity of the action.
        attr_accessor :subject                        # 0-* [ Contract::Term::Action::Subject ]
        ##
        # Purpose for the Contract Term Action
        # Reason or purpose for the action stipulated by this Contract Provision.
        attr_accessor :intent                         # 1-1 CodeableConcept
        ##
        # Pointer to specific item
        # Id [identifier??] of the clause or question text related to this action in the referenced form or QuestionnaireResponse.
        attr_accessor :linkId                         # 0-* [ string ]
        ##
        # State of the action
        # Current state of the term action.
        attr_accessor :status                         # 1-1 CodeableConcept
        ##
        # Episode associated with action
        # Encounter or Episode with primary association to specified term activity.
        attr_accessor :context                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter|http://hl7.org/fhir/StructureDefinition/EpisodeOfCare)
        ##
        # Pointer to specific item
        # Id [identifier??] of the clause or question text related to the requester of this action in the referenced form or QuestionnaireResponse.
        attr_accessor :contextLinkId                  # 0-* [ string ]
        ##
        # When action happens.
        attr_accessor :occurrenceDateTime             # 0-1 DateTime
        ##
        # When action happens.
        attr_accessor :occurrencePeriod               # 0-1 Period
        ##
        # When action happens.
        attr_accessor :occurrenceTiming               # 0-1 Timing
        ##
        # Who asked for action
        # Who or what initiated the action and has responsibility for its activation.
        attr_accessor :requester                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Organization) ]
        ##
        # Pointer to specific item
        # Id [identifier??] of the clause or question text related to the requester of this action in the referenced form or QuestionnaireResponse.
        attr_accessor :requesterLinkId                # 0-* [ string ]
        ##
        # Kind of service performer
        # The type of individual that is desired or required to perform or not perform the action.
        attr_accessor :performerType                  # 0-* [ CodeableConcept ]
        ##
        # Competency of the performer
        # The type of role or competency of an individual desired or required to perform or not perform the action.
        attr_accessor :performerRole                  # 0-1 CodeableConcept
        ##
        # Actor that wil execute (or not) the action
        # Indicates who or what is being asked to perform (or not perform) the ction.
        attr_accessor :performer                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Substance|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Location)
        ##
        # Pointer to specific item
        # Id [identifier??] of the clause or question text related to the reason type or reference of this  action in the referenced form or QuestionnaireResponse.
        attr_accessor :performerLinkId                # 0-* [ string ]
        ##
        # Why is action (not) needed?
        # Rationale for the action to be performed or not performed. Describes why the action is permitted or prohibited.
        attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
        ##
        # Why is action (not) needed?
        # Indicates another resource whose existence justifies permitting or not permitting this action.
        attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference|http://hl7.org/fhir/StructureDefinition/Questionnaire|http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse) ]
        ##
        # Why action is to be performed
        # Describes why the action is to be performed or not performed in textual form.
        attr_accessor :reason                         # 0-* [ string ]
        ##
        # Pointer to specific item
        # Id [identifier??] of the clause or question text related to the reason type or reference of this  action in the referenced form or QuestionnaireResponse.
        attr_accessor :reasonLinkId                   # 0-* [ string ]
        ##
        # Comments about the action
        # Comments made about the term action made by the requester, performer, subject or other participants.
        attr_accessor :note                           # 0-* [ Annotation ]
        ##
        # Action restriction numbers
        # Security labels that protects the action.
        attr_accessor :securityLabelNumber            # 0-* [ unsignedInt ]
      end
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
      # Contract Term Number
      # Unique identifier for this particular Contract Provision.
      attr_accessor :identifier                     # 0-1 Identifier
      ##
      # Contract Term Issue Date Time
      # When this Contract Provision was issued.
      attr_accessor :issued                         # 0-1 dateTime
      ##
      # Contract Term Effective Time
      # Relevant time or time-period when this Contract Provision is applicable.
      attr_accessor :applies                        # 0-1 Period
      ##
      # Term Concern
      # The entity that the term applies to.
      attr_accessor :topicCodeableConcept           # 0-1 CodeableConcept
      ##
      # Term Concern
      # The entity that the term applies to.
      attr_accessor :topicReference                 # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
      ##
      # Contract Term Type or Form
      # A legal clause or condition contained within a contract that requires one or both parties to perform a particular requirement by some specified time or prevents one or both parties from performing a particular requirement by some specified time.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Contract Term Type specific classification
      # A specialized legal clause or condition based on overarching contract type.
      attr_accessor :subType                        # 0-1 CodeableConcept
      ##
      # Term Statement
      # Statement of a provision in a policy or a contract.
      attr_accessor :text                           # 0-1 string
      ##
      # Protection for the Term
      # Security labels that protect the handling of information about the term and its elements, which may be specifically identified..
      attr_accessor :securityLabel                  # 0-* [ Contract::Term::SecurityLabel ]
      ##
      # Context of the Contract term
      # The matter of concern in the context of this provision of the agrement.
      attr_accessor :offer                          # 1-1 Contract::Term::Offer
      ##
      # Contract Term Asset List.
      attr_accessor :asset                          # 0-* [ Contract::Term::Asset ]
      ##
      # Entity being ascribed responsibility
      # An actor taking a role in an activity for which it can be assigned some degree of responsibility for the activity taking place.
      # Several agents may be associated (i.e. has some responsibility for an activity) with an activity and vice-versa.For example, in cases of actions initiated by one user for other users, or in events that involve more than one user, hardware device, software, or system process. However, only one user may be the initiator/requestor for the event.
      attr_accessor :action                         # 0-* [ Contract::Term::Action ]
      ##
      # Nested Contract Term Group
      # Nested group of Contract Provisions.
      attr_accessor :group                          # 0-* [ Contract::Term ]
    end

    ##
    # Contract Signatory
    # Parties with legal standing in the Contract, including the principal parties, the grantor(s) and grantee(s), which are any person or organization bound by the contract, and any ancillary parties, which facilitate the execution of the contract such as a notary or witness.
    # Signers who are principal parties to the contract are bound by the Contract.activity related to the Contract.topic, and the Contract.term(s), which either extend or restrict the overall action on the topic by, for example, stipulating specific policies or obligations constraining actions, action reason, or agents with respect to some or all of the topic.For example, specifying how policies or obligations shall constrain actions and action reasons permitted or denied on all or a subset of the Contract.topic (e.g., all or a portion of property being transferred by the contract), agents (e.g., who can resell, assign interests, or alter the property being transferred by the contract), actions, and action reasons; or with respect to Contract.terms, stipulating, extending, or limiting the Contract.period of applicability or valuation of items under consideration.
    class Signer < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Signer.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Signer.extension',
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
          'path'=>'Signer.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Contract Signatory Role
        # Role of this Contract signer, e.g. notary, grantee.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/contractsignertypecodes'=>[ 'AMENDER', 'AUTHN', 'AUT', 'AFFL', 'AGNT', 'ASSIGNED', 'CIT', 'CLAIMANT', 'COAUTH', 'CONSENTER', 'CONSWIT', 'CONT', 'COPART', 'COVPTY', 'DELEGATEE', 'delegator', 'DEPEND', 'DPOWATT', 'EMGCON', 'EVTWIT', 'EXCEST', 'GRANTEE', 'GRANTOR', 'GUAR', 'GUARD', 'GUADLTM', 'INF', 'INTPRT', 'INSBJ', 'HPOWATT', 'HPROV', 'LEGAUTHN', 'NMDINS', 'NOK', 'NOTARY', 'PAT', 'POWATT', 'PRIMAUTH', 'PRIRECIP', 'RECIP', 'RESPRSN', 'REVIEWER', 'TRANS', 'SOURCE', 'SPOWATT', 'VALID', 'VERF', 'WIT' ]
          },
          'type'=>'Coding',
          'path'=>'Signer.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/contract-signer-type'}
        },
        ##
        # Contract Signatory Party
        # Party which is a signator to this Contract.
        'party' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
          'type'=>'Reference',
          'path'=>'Signer.party',
          'min'=>1,
          'max'=>1
        },
        ##
        # Contract Documentation Signature
        # Legally binding Contract DSIG signature contents in Base64.
        'signature' => {
          'type'=>'Signature',
          'path'=>'Signer.signature',
          'min'=>1,
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
      # Contract Signatory Role
      # Role of this Contract signer, e.g. notary, grantee.
      attr_accessor :type                           # 1-1 Coding
      ##
      # Contract Signatory Party
      # Party which is a signator to this Contract.
      attr_accessor :party                          # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
      ##
      # Contract Documentation Signature
      # Legally binding Contract DSIG signature contents in Base64.
      attr_accessor :signature                      # 1-* [ Signature ]
    end

    ##
    # Contract Friendly Language
    # The "patient friendly language" versionof the Contract in whole or in parts. "Patient friendly language" means the representation of the Contract and Contract Provisions in a manner that is readily accessible and understandable by a layperson in accordance with best practices for communication styles that ensure that those agreeing to or signing the Contract understand the roles, actions, obligations, responsibilities, and implication of the agreement.
    class Friendly < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'content[x]' => ['Attachment', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Friendly.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Friendly.extension',
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
          'path'=>'Friendly.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Easily comprehended representation of this Contract
        # Human readable rendering of this Contract in a format and representation intended to enhance comprehension and ensure understandability.
        'contentAttachment' => {
          'type'=>'Attachment',
          'path'=>'Friendly.content[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Easily comprehended representation of this Contract
        # Human readable rendering of this Contract in a format and representation intended to enhance comprehension and ensure understandability.
        'contentReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Composition', 'http://hl7.org/fhir/StructureDefinition/DocumentReference', 'http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse'],
          'type'=>'Reference',
          'path'=>'Friendly.content[x]',
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
      # Easily comprehended representation of this Contract
      # Human readable rendering of this Contract in a format and representation intended to enhance comprehension and ensure understandability.
      attr_accessor :contentAttachment              # 1-1 Attachment
      ##
      # Easily comprehended representation of this Contract
      # Human readable rendering of this Contract in a format and representation intended to enhance comprehension and ensure understandability.
      attr_accessor :contentReference               # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Composition|http://hl7.org/fhir/StructureDefinition/DocumentReference|http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse)
    end

    ##
    # Contract Legal Language
    # List of Legal expressions or representations of this Contract.
    class Legal < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'content[x]' => ['Attachment', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Legal.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Legal.extension',
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
          'path'=>'Legal.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Contract Legal Text
        # Contract legal text in human renderable form.
        'contentAttachment' => {
          'type'=>'Attachment',
          'path'=>'Legal.content[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Contract Legal Text
        # Contract legal text in human renderable form.
        'contentReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Composition', 'http://hl7.org/fhir/StructureDefinition/DocumentReference', 'http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse'],
          'type'=>'Reference',
          'path'=>'Legal.content[x]',
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
      # Contract Legal Text
      # Contract legal text in human renderable form.
      attr_accessor :contentAttachment              # 1-1 Attachment
      ##
      # Contract Legal Text
      # Contract legal text in human renderable form.
      attr_accessor :contentReference               # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Composition|http://hl7.org/fhir/StructureDefinition/DocumentReference|http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse)
    end

    ##
    # Computable Contract Language
    # List of Computable Policy Rule Language Representations of this Contract.
    class Rule < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'content[x]' => ['Attachment', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Rule.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Rule.extension',
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
          'path'=>'Rule.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Computable Contract Rules
        # Computable Contract conveyed using a policy rule language (e.g. XACML, DKAL, SecPal).
        'contentAttachment' => {
          'type'=>'Attachment',
          'path'=>'Rule.content[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Computable Contract Rules
        # Computable Contract conveyed using a policy rule language (e.g. XACML, DKAL, SecPal).
        'contentReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference'],
          'type'=>'Reference',
          'path'=>'Rule.content[x]',
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
      # Computable Contract Rules
      # Computable Contract conveyed using a policy rule language (e.g. XACML, DKAL, SecPal).
      attr_accessor :contentAttachment              # 1-1 Attachment
      ##
      # Computable Contract Rules
      # Computable Contract conveyed using a policy rule language (e.g. XACML, DKAL, SecPal).
      attr_accessor :contentReference               # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference)
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
    # Contract number
    # Unique identifier for this Contract or a derivative that references a Source Contract.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Basal definition
    # Canonical identifier for this contract, represented as a URI (globally unique).
    # Used in a domain that uses a supplied contract repository.
    attr_accessor :url                            # 0-1 uri
    ##
    # Business edition
    # An edition identifier used for business purposes to label business significant variants.
    # Note -  This is a business versionId, not a resource version id (see discussion http://build.fhir.org/resource.html#versions) Comments - There may be different contract instances that have the same identifier but different versions. The version can be appended to the url in a reference to allow a reference to a particular business version of the plan definition with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # amended | appended | cancelled | disputed | entered-in-error | executable | executed | negotiable | offered | policy | rejected | renewed | revoked | resolved | terminated
    # The status of the resource instance.
    # This element is labeled as a modifier because the status contains codes that mark the contract as not currently valid or active.
    attr_accessor :status                         # 0-1 code
    ##
    # Negotiation status
    # Legal states of the formation of a legal instrument, which is a formally executed written document that can be formally attributed to its author, records and formally expresses a legally enforceable act, process, or contractual duty, obligation, or right, and therefore evidences that act, process, or agreement.
    attr_accessor :legalState                     # 0-1 CodeableConcept
    ##
    # Source Contract Definition
    # The URL pointing to a FHIR-defined Contract Definition that is adhered to in whole or part by this Contract.
    attr_accessor :instantiatesCanonical          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Contract)
    ##
    # External Contract Definition
    # The URL pointing to an externally maintained definition that is adhered to in whole or in part by this Contract.
    attr_accessor :instantiatesUri                # 0-1 uri
    ##
    # Content derived from the basal information
    # The minimal content derived from the basal information source at a specific stage in its lifecycle.
    attr_accessor :contentDerivative              # 0-1 CodeableConcept
    ##
    # When this Contract was issued
    # When this  Contract was issued.
    attr_accessor :issued                         # 0-1 dateTime
    ##
    # Effective time
    # Relevant time or time-period when this Contract is applicable.
    attr_accessor :applies                        # 0-1 Period
    ##
    # Contract cessation cause
    # Event resulting in discontinuation or termination of this Contract instance by one or more parties to the contract.
    attr_accessor :expirationType                 # 0-1 CodeableConcept
    ##
    # Contract Target Entity
    # The target entity impacted by or of interest to parties to the agreement.
    # The Contract.subject is an entity that has some role with respect to the Contract.topic and Contract.topic.term, which is of focal interest to the parties to the contract and likely impacted in a significant way by the Contract.action/Contract.action.reason and the Contract.term.action/Contract.action.reason. In many cases, the Contract.subject is a Contract.signer if the subject is an adult; has a legal interest in the contract; and incompetent to participate in the contract agreement.
    attr_accessor :subject                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Authority under which this Contract has standing
    # A formally or informally recognized grouping of people, principals, organizations, or jurisdictions formed for the purpose of achieving some form of collective action such as the promulgation, administration and enforcement of contracts and policies.
    attr_accessor :authority                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization) ]
    ##
    # A sphere of control governed by an authoritative jurisdiction, organization, or person
    # Recognized governance framework or system operating with a circumscribed scope in accordance with specified principles, policies, processes or procedures for managing rights, actions, or behaviors of parties or principals relative to resources.
    attr_accessor :domain                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Location) ]
    ##
    # Specific Location
    # Sites in which the contract is complied with,  exercised, or in force.
    attr_accessor :site                           # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Location) ]
    ##
    # Computer friendly designation
    # A natural language name identifying this Contract definition, derivative, or instance in any legal state. Provides additional information about its content. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 0-1 string
    ##
    # Human Friendly name
    # A short, descriptive, user-friendly title for this Contract definition, derivative, or instance in any legal state.t giving additional information about its content.
    attr_accessor :title                          # 0-1 string
    ##
    # Subordinate Friendly name
    # An explanatory or alternate user-friendly title for this Contract definition, derivative, or instance in any legal state.t giving additional information about its content.
    attr_accessor :subtitle                       # 0-1 string
    ##
    # Acronym or short name
    # Alternative representation of the title for this Contract definition, derivative, or instance in any legal state., e.g., a domain specific contract number related to legislation.
    attr_accessor :local_alias                    # 0-* [ string ]
    ##
    # Source of Contract
    # The individual or organization that authored the Contract definition, derivative, or instance in any legal state.
    attr_accessor :author                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Range of Legal Concerns
    # A selector of legal concerns for this Contract definition, derivative, or instance in any legal state.
    attr_accessor :scope                          # 0-1 CodeableConcept
    ##
    # Focus of contract interest
    # Narrows the range of legal concerns to focus on the achievement of specific contractual objectives.
    attr_accessor :topicCodeableConcept           # 0-1 CodeableConcept
    ##
    # Focus of contract interest
    # Narrows the range of legal concerns to focus on the achievement of specific contractual objectives.
    attr_accessor :topicReference                 # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
    ##
    # Legal instrument category
    # A high-level category for the legal instrument, whether constructed as a Contract definition, derivative, or instance in any legal state.  Provides additional information about its content within the context of the Contract's scope to distinguish the kinds of systems that would be interested in the contract.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # Subtype within the context of type
    # Sub-category for the Contract that distinguishes the kinds of systems that would be interested in the Contract within the context of the Contract's scope.
    attr_accessor :subType                        # 0-* [ CodeableConcept ]
    ##
    # Contract precursor content
    # Precusory content developed with a focus and intent of supporting the formation a Contract instance, which may be associated with and transformable into a Contract.
    attr_accessor :contentDefinition              # 0-1 Contract::ContentDefinition
    ##
    # Contract Term List
    # One or more Contract Provisions, which may be related and conveyed as a group, and may contain nested groups.
    attr_accessor :term                           # 0-* [ Contract::Term ]
    ##
    # Extra Information
    # Information that may be needed by/relevant to the performer in their execution of this term action.
    attr_accessor :supportingInfo                 # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Key event in Contract History
    # Links to Provenance records for past versions of this Contract definition, derivative, or instance, which identify key state transitions or updates that are likely to be relevant to a user looking at the current version of the Contract.  The Provence.entity indicates the target that was changed in the update. http://build.fhir.org/provenance-definitions.html#Provenance.entity.
    attr_accessor :relevantHistory                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Provenance) ]
    ##
    # Contract Signatory
    # Parties with legal standing in the Contract, including the principal parties, the grantor(s) and grantee(s), which are any person or organization bound by the contract, and any ancillary parties, which facilitate the execution of the contract such as a notary or witness.
    # Signers who are principal parties to the contract are bound by the Contract.activity related to the Contract.topic, and the Contract.term(s), which either extend or restrict the overall action on the topic by, for example, stipulating specific policies or obligations constraining actions, action reason, or agents with respect to some or all of the topic.For example, specifying how policies or obligations shall constrain actions and action reasons permitted or denied on all or a subset of the Contract.topic (e.g., all or a portion of property being transferred by the contract), agents (e.g., who can resell, assign interests, or alter the property being transferred by the contract), actions, and action reasons; or with respect to Contract.terms, stipulating, extending, or limiting the Contract.period of applicability or valuation of items under consideration.
    attr_accessor :signer                         # 0-* [ Contract::Signer ]
    ##
    # Contract Friendly Language
    # The "patient friendly language" versionof the Contract in whole or in parts. "Patient friendly language" means the representation of the Contract and Contract Provisions in a manner that is readily accessible and understandable by a layperson in accordance with best practices for communication styles that ensure that those agreeing to or signing the Contract understand the roles, actions, obligations, responsibilities, and implication of the agreement.
    attr_accessor :friendly                       # 0-* [ Contract::Friendly ]
    ##
    # Contract Legal Language
    # List of Legal expressions or representations of this Contract.
    attr_accessor :legal                          # 0-* [ Contract::Legal ]
    ##
    # Computable Contract Language
    # List of Computable Policy Rule Language Representations of this Contract.
    attr_accessor :rule                           # 0-* [ Contract::Rule ]
    ##
    # Binding Contract
    # Legally binding Contract: This is the signed and legally recognized representation of the Contract, which is considered the "source of truth" and which would be the basis for legal action related to enforcement of this Contract.
    attr_accessor :legallyBindingAttachment       # 0-1 Attachment
    ##
    # Binding Contract
    # Legally binding Contract: This is the signed and legally recognized representation of the Contract, which is considered the "source of truth" and which would be the basis for legal action related to enforcement of this Contract.
    attr_accessor :legallyBindingReference        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Composition|http://hl7.org/fhir/StructureDefinition/DocumentReference|http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse|http://hl7.org/fhir/StructureDefinition/Contract)

    def resourceType
      'Contract'
    end
  end
end
