module FHIR

  ##
  # A record of a request for service such as diagnostic investigations, treatments, or operations to be performed.
  class ServiceRequest < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['authored', 'based-on', 'body-site', 'category', 'code', 'encounter', 'identifier', 'instantiates-canonical', 'instantiates-uri', 'intent', 'occurrence', 'patient', 'performer-type', 'performer', 'priority', 'replaces', 'requester', 'requisition', 'specimen', 'status', 'subject']
    MULTIPLE_TYPES = {
      'quantity[x]' => ['Quantity', 'Range', 'Ratio'],
      'occurrence[x]' => ['dateTime', 'Period', 'Timing'],
      'asNeeded[x]' => ['boolean', 'CodeableConcept']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'ServiceRequest.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ServiceRequest.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ServiceRequest.implicitRules',
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
        'path'=>'ServiceRequest.language',
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
        'path'=>'ServiceRequest.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ServiceRequest.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ServiceRequest.extension',
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
        'path'=>'ServiceRequest.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identifiers assigned to this order instance by the orderer and/or the receiver and/or order fulfiller.
      # The identifier.type element is used to distinguish between the identifiers assigned by the orderer (known as the 'Placer' in HL7 v2) and the producer of the observations in response to the order (known as the 'Filler' in HL7 v2).  For further discussion and examples see the resource notes section below.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ServiceRequest.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates FHIR protocol or definition
      # The URL pointing to a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this ServiceRequest.
      # Note: This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'instantiatesCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ActivityDefinition', 'http://hl7.org/fhir/StructureDefinition/PlanDefinition'],
        'type'=>'canonical',
        'path'=>'ServiceRequest.instantiatesCanonical',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates external protocol or definition
      # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this ServiceRequest.
      # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
      'instantiatesUri' => {
        'type'=>'uri',
        'path'=>'ServiceRequest.instantiatesUri',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What request fulfills
      # Plan/proposal/order fulfilled by this request.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CarePlan', 'http://hl7.org/fhir/StructureDefinition/ServiceRequest', 'http://hl7.org/fhir/StructureDefinition/MedicationRequest'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What request replaces
      # The request takes the place of the referenced completed or terminated request(s).
      'replaces' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.replaces',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Composite Request ID
      # A shared identifier common to all service requests that were authorized more or less simultaneously by a single author, representing the composite or group identifier.
      # Requests are linked either by a "basedOn" relationship (i.e. one request is fulfilling another) or by having a common requisition. Requests that are part of the same requisition are generally treated independently from the perspective of changing their state or maintaining them after initial creation.
      'requisition' => {
        'type'=>'Identifier',
        'path'=>'ServiceRequest.requisition',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | on-hold | revoked | completed | entered-in-error | unknown
      # The status of the order.
      # The status is generally fully in the control of the requester - they determine whether the order is draft or active and, after it has been activated, competed, cancelled or suspended. States relating to the activities of the performer are reflected on either the corresponding event (see [Event Pattern](event.html) for general discussion) or using the [Task](task.html) resource.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-status'=>[ 'draft', 'active', 'on-hold', 'revoked', 'completed', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'ServiceRequest.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-status'}
      },
      ##
      # proposal | plan | directive | order | original-order | reflex-order | filler-order | instance-order | option
      # Whether the request is a proposal, plan, an original order or a reflex order.
      # This element is labeled as a modifier because the intent alters when and how the resource is actually applicable.
      'intent' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-intent'=>[ 'proposal', 'plan', 'directive', 'order', 'original-order', 'reflex-order', 'filler-order', 'instance-order', 'option' ]
        },
        'type'=>'code',
        'path'=>'ServiceRequest.intent',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-intent'}
      },
      ##
      # Classification of service
      # A code that classifies the service for searching, sorting and display purposes (e.g. "Surgical Procedure").
      # There may be multiple axis of categorization depending on the context or use case for retrieving or displaying the resource.  The level of granularity is defined by the category concepts in the value set.
      'category' => {
        'type'=>'CodeableConcept',
        'path'=>'ServiceRequest.category',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # routine | urgent | asap | stat
      # Indicates how quickly the ServiceRequest should be addressed with respect to other requests.
      'priority' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-priority'=>[ 'routine', 'urgent', 'asap', 'stat' ]
        },
        'type'=>'code',
        'path'=>'ServiceRequest.priority',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-priority'}
      },
      ##
      # True if service/procedure should not be performed
      # Set this to true if the record is saying that the service/procedure should NOT be performed.
      # In general, only the code and timeframe will be present, though occasional additional qualifiers such as body site or even performer could be included to narrow the scope of the prohibition.  If the ServiceRequest.code and ServiceRequest.doNotPerform both contain negation, that will reinforce prohibition and should not have a double negative interpretation.
      'doNotPerform' => {
        'type'=>'boolean',
        'path'=>'ServiceRequest.doNotPerform',
        'min'=>0,
        'max'=>1
      },
      ##
      # What is being requested/ordered
      # A code that identifies a particular service (i.e., procedure, diagnostic investigation, or panel of investigations) that have been requested.
      # Many laboratory and radiology procedure codes embed the specimen/organ system in the test order name, for example,  serum or serum/plasma glucose, or a chest x-ray. The specimen might not be recorded separately from the test code.
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'ServiceRequest.code',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional order information
      # Additional details and instructions about the how the services are to be delivered.   For example, and order for a urinary catheter may have an order detail for an external or indwelling catheter, or an order for a bandage may require additional instructions specifying how the bandage should be applied.
      # For information from the medical record intended to support the delivery of the requested services, use the `supportingInformation` element.
      'orderDetail' => {
        'type'=>'CodeableConcept',
        'path'=>'ServiceRequest.orderDetail',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Service amount
      # An amount of service being requested which can be a quantity ( for example $1,500 home modification), a ratio ( for example, 20 half day visits per month), or a range (2.0 to 1.8 Gy per fraction).
      'quantityQuantity' => {
        'type'=>'Quantity',
        'path'=>'ServiceRequest.quantity[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Service amount
      # An amount of service being requested which can be a quantity ( for example $1,500 home modification), a ratio ( for example, 20 half day visits per month), or a range (2.0 to 1.8 Gy per fraction).
      'quantityRange' => {
        'type'=>'Range',
        'path'=>'ServiceRequest.quantity[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Service amount
      # An amount of service being requested which can be a quantity ( for example $1,500 home modification), a ratio ( for example, 20 half day visits per month), or a range (2.0 to 1.8 Gy per fraction).
      'quantityRatio' => {
        'type'=>'Ratio',
        'path'=>'ServiceRequest.quantity[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Individual or Entity the service is ordered for
      # On whom or what the service is to be performed. This is usually a human patient, but can also be requested on animals, groups of humans or animals, devices such as dialysis machines, or even locations (typically for environmental scans).
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Location', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.subject',
        'min'=>1,
        'max'=>1
      },
      ##
      # Encounter in which the request was created
      # An encounter that provides additional information about the healthcare context in which this request is made.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # When service should occur
      # The date/time at which the requested service should occur.
      'occurrenceDateTime' => {
        'type'=>'DateTime',
        'path'=>'ServiceRequest.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When service should occur
      # The date/time at which the requested service should occur.
      'occurrencePeriod' => {
        'type'=>'Period',
        'path'=>'ServiceRequest.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # When service should occur
      # The date/time at which the requested service should occur.
      'occurrenceTiming' => {
        'type'=>'Timing',
        'path'=>'ServiceRequest.occurrence[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Preconditions for service
      # If a CodeableConcept is present, it indicates the pre-condition for performing the service.  For example "pain", "on flare-up", etc.
      'asNeededBoolean' => {
        'type'=>'Boolean',
        'path'=>'ServiceRequest.asNeeded[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Preconditions for service
      # If a CodeableConcept is present, it indicates the pre-condition for performing the service.  For example "pain", "on flare-up", etc.
      'asNeededCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'ServiceRequest.asNeeded[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date request signed
      # When the request transitioned to being actionable.
      'authoredOn' => {
        'type'=>'dateTime',
        'path'=>'ServiceRequest.authoredOn',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who/what is requesting service
      # The individual who initiated the request and has responsibility for its activation.
      # This not the dispatcher, but rather who is the authorizer.  This element is not intended to handle delegation which would generally be managed through the Provenance resource.
      'requester' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.requester',
        'min'=>0,
        'max'=>1
      },
      ##
      # Performer role
      # Desired type of performer for doing the requested service.
      # This is a  role, not  a participation type.  In other words, does not describe the task but describes the capacity.  For example, “compounding pharmacy”, “psychiatrist” or “internal referral”.
      'performerType' => {
        'type'=>'CodeableConcept',
        'path'=>'ServiceRequest.performerType',
        'min'=>0,
        'max'=>1
      },
      ##
      # Requested performer
      # The desired performer for doing the requested service.  For example, the surgeon, dermatopathologist, endoscopist, etc.
      # If multiple performers are present, it is interpreted as a list of *alternative* performers without any preference regardless of order.  If order of preference is needed use the [request-performerOrder extension](extension-request-performerorder.html).  Use CareTeam to represent a group of performers (for example, Practitioner A *and* Practitioner B).
      'performer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/HealthcareService', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.performer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Requested location
      # The preferred location(s) where the procedure should actually happen in coded or free text form. E.g. at home or nursing day care center.
      'locationCode' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-RoleCode'=>[ 'DX', 'CVDX', 'CATH', 'ECHO', 'GIDX', 'ENDOS', 'RADDX', 'RADO', 'RNEU', 'HOSP', 'CHR', 'GACH', 'MHSP', 'PSYCHF', 'RH', 'RHAT', 'RHII', 'RHMAD', 'RHPI', 'RHPIH', 'RHPIMS', 'RHPIVS', 'RHYAD', 'HU', 'BMTU', 'CCU', 'CHEST', 'EPIL', 'ER', 'ETU', 'HD', 'HLAB', 'INLAB', 'OUTLAB', 'HRAD', 'HUSCS', 'ICU', 'PEDICU', 'PEDNICU', 'INPHARM', 'MBL', 'NCCS', 'NS', 'OUTPHARM', 'PEDU', 'PHU', 'RHU', 'SLEEP', 'NCCF', 'SNF', 'OF', 'ALL', 'AMPUT', 'BMTC', 'BREAST', 'CANC', 'CAPC', 'CARD', 'PEDCARD', 'COAG', 'CRS', 'DERM', 'ENDO', 'PEDE', 'ENT', 'FMC', 'GI', 'PEDGI', 'GIM', 'GYN', 'HEM', 'PEDHEM', 'HTN', 'IEC', 'INFD', 'PEDID', 'INV', 'LYMPH', 'MGEN', 'NEPH', 'PEDNEPH', 'NEUR', 'OB', 'OMS', 'ONCL', 'PEDHO', 'OPH', 'OPTC', 'ORTHO', 'HAND', 'PAINCL', 'PC', 'PEDC', 'PEDRHEUM', 'POD', 'PREV', 'PROCTO', 'PROFF', 'PROS', 'PSI', 'PSY', 'RHEUM', 'SPMED', 'SU', 'PLS', 'URO', 'TR', 'TRAVEL', 'WND', 'RTF', 'PRC', 'SURF', 'DADDR', 'MOBL', 'AMB', 'PHARM', 'ACC', 'COMM', 'CSC', 'PTRES', 'SCHOOL', 'UPC', 'WORK' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ServiceRequest.locationCode',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v3-ServiceDeliveryLocationRoleType'}
      },
      ##
      # Requested location
      # A reference to the the preferred location(s) where the procedure should actually happen. E.g. at home or nursing day care center.
      'locationReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.locationReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Explanation/Justification for procedure or service
      # An explanation or justification for why this service is being requested in coded or textual form.   This is often for billing purposes.  May relate to the resources referred to in `supportingInfo`.
      # This element represents why the referral is being made and may be used to decide how the service will be performed, or even if it will be performed at all.   Use `CodeableConcept.text` element if the data is free (uncoded) text as shown in the [CT Scan example](servicerequest-example-di.html).
      'reasonCode' => {
        'type'=>'CodeableConcept',
        'path'=>'ServiceRequest.reasonCode',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Explanation/Justification for service or service
      # Indicates another resource that provides a justification for why this service is being requested.   May relate to the resources referred to in `supportingInfo`.
      # This element represents why the referral is being made and may be used to decide how the service will be performed, or even if it will be performed at all.    To be as specific as possible,  a reference to  *Observation* or *Condition* should be used if available.  Otherwise when referencing  *DiagnosticReport*  it should contain a finding  in `DiagnosticReport.conclusion` and/or `DiagnosticReport.conclusionCode`.   When using a reference to *DocumentReference*, the target document should contain clear findings language providing the relevant reason for this service request.  Use  the CodeableConcept text element in `ServiceRequest.reasonCode` if the data is free (uncoded) text as shown in the [CT Scan example](servicerequest-example-di.html).
      'reasonReference' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Condition', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/DiagnosticReport', 'http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.reasonReference',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Associated insurance coverage
      # Insurance plans, coverage extensions, pre-authorizations and/or pre-determinations that may be needed for delivering the requested service.
      'insurance' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Coverage', 'http://hl7.org/fhir/StructureDefinition/ClaimResponse'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.insurance',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional clinical information about the patient or specimen that may influence the services or their interpretations.     This information includes diagnosis, clinical findings and other observations.  In laboratory ordering these are typically referred to as "ask at order entry questions (AOEs)".  This includes observations explicitly requested by the producer (filler) to provide context or supporting information needed to complete the order. For example,  reporting the amount of inspired oxygen for blood gas measurements.
      # To represent information about how the services are to be delivered use the `instructions` element.
      'supportingInfo' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.supportingInfo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Procedure Samples
      # One or more specimens that the laboratory procedure will use.
      # Many diagnostic procedures need a specimen, but the request itself is not actually about the specimen. This element is for when the diagnostic is requested on already existing specimens and the request points to the specimen it applies to.    Conversely, if the request is entered first with an unknown specimen, then the [Specimen](specimen.html) resource points to the ServiceRequest.
      'specimen' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Specimen'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.specimen',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Location on Body
      # Anatomic location where the procedure should be performed. This is the target site.
      # Only used if not implicit in the code found in ServiceRequest.code.  If the use case requires BodySite to be handled as a separate resource instead of an inline coded element (e.g. to identify and track separately)  then use the standard extension [procedure-targetBodyStructure](extension-procedure-targetbodystructure.html).
      'bodySite' => {
        'type'=>'CodeableConcept',
        'path'=>'ServiceRequest.bodySite',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Comments
      # Any other notes and comments made about the service request. For example, internal billing notes.
      'note' => {
        'type'=>'Annotation',
        'path'=>'ServiceRequest.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Patient or consumer-oriented instructions
      # Instructions in terms that are understood by the patient or consumer.
      'patientInstruction' => {
        'type'=>'string',
        'path'=>'ServiceRequest.patientInstruction',
        'min'=>0,
        'max'=>1
      },
      ##
      # Request provenance
      # Key events in the history of the request.
      # This might not include provenances for all versions of the request – only those deemed “relevant” or important.This SHALL NOT include the Provenance associated with this current version of the resource.  (If that provenance is deemed to be a “relevant” change, it will need to be added as part of a later update.  Until then, it can be queried directly as the Provenance that points to this version using _revincludeAll Provenances should have some historical version of this Request as their subject.
      'relevantHistory' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Provenance'],
        'type'=>'Reference',
        'path'=>'ServiceRequest.relevantHistory',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }
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
    # Identifiers assigned to this order instance by the orderer and/or the receiver and/or order fulfiller.
    # The identifier.type element is used to distinguish between the identifiers assigned by the orderer (known as the 'Placer' in HL7 v2) and the producer of the observations in response to the order (known as the 'Filler' in HL7 v2).  For further discussion and examples see the resource notes section below.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Instantiates FHIR protocol or definition
    # The URL pointing to a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this ServiceRequest.
    # Note: This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :instantiatesCanonical          # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/ActivityDefinition|http://hl7.org/fhir/StructureDefinition/PlanDefinition) ]
    ##
    # Instantiates external protocol or definition
    # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this ServiceRequest.
    # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
    attr_accessor :instantiatesUri                # 0-* [ uri ]
    ##
    # What request fulfills
    # Plan/proposal/order fulfilled by this request.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CarePlan|http://hl7.org/fhir/StructureDefinition/ServiceRequest|http://hl7.org/fhir/StructureDefinition/MedicationRequest) ]
    ##
    # What request replaces
    # The request takes the place of the referenced completed or terminated request(s).
    attr_accessor :replaces                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ServiceRequest) ]
    ##
    # Composite Request ID
    # A shared identifier common to all service requests that were authorized more or less simultaneously by a single author, representing the composite or group identifier.
    # Requests are linked either by a "basedOn" relationship (i.e. one request is fulfilling another) or by having a common requisition. Requests that are part of the same requisition are generally treated independently from the perspective of changing their state or maintaining them after initial creation.
    attr_accessor :requisition                    # 0-1 Identifier
    ##
    # draft | active | on-hold | revoked | completed | entered-in-error | unknown
    # The status of the order.
    # The status is generally fully in the control of the requester - they determine whether the order is draft or active and, after it has been activated, competed, cancelled or suspended. States relating to the activities of the performer are reflected on either the corresponding event (see [Event Pattern](event.html) for general discussion) or using the [Task](task.html) resource.
    attr_accessor :status                         # 1-1 code
    ##
    # proposal | plan | directive | order | original-order | reflex-order | filler-order | instance-order | option
    # Whether the request is a proposal, plan, an original order or a reflex order.
    # This element is labeled as a modifier because the intent alters when and how the resource is actually applicable.
    attr_accessor :intent                         # 1-1 code
    ##
    # Classification of service
    # A code that classifies the service for searching, sorting and display purposes (e.g. "Surgical Procedure").
    # There may be multiple axis of categorization depending on the context or use case for retrieving or displaying the resource.  The level of granularity is defined by the category concepts in the value set.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # routine | urgent | asap | stat
    # Indicates how quickly the ServiceRequest should be addressed with respect to other requests.
    attr_accessor :priority                       # 0-1 code
    ##
    # True if service/procedure should not be performed
    # Set this to true if the record is saying that the service/procedure should NOT be performed.
    # In general, only the code and timeframe will be present, though occasional additional qualifiers such as body site or even performer could be included to narrow the scope of the prohibition.  If the ServiceRequest.code and ServiceRequest.doNotPerform both contain negation, that will reinforce prohibition and should not have a double negative interpretation.
    attr_accessor :doNotPerform                   # 0-1 boolean
    ##
    # What is being requested/ordered
    # A code that identifies a particular service (i.e., procedure, diagnostic investigation, or panel of investigations) that have been requested.
    # Many laboratory and radiology procedure codes embed the specimen/organ system in the test order name, for example,  serum or serum/plasma glucose, or a chest x-ray. The specimen might not be recorded separately from the test code.
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # Additional order information
    # Additional details and instructions about the how the services are to be delivered.   For example, and order for a urinary catheter may have an order detail for an external or indwelling catheter, or an order for a bandage may require additional instructions specifying how the bandage should be applied.
    # For information from the medical record intended to support the delivery of the requested services, use the `supportingInformation` element.
    attr_accessor :orderDetail                    # 0-* [ CodeableConcept ]
    ##
    # Service amount
    # An amount of service being requested which can be a quantity ( for example $1,500 home modification), a ratio ( for example, 20 half day visits per month), or a range (2.0 to 1.8 Gy per fraction).
    attr_accessor :quantityQuantity               # 0-1 Quantity
    ##
    # Service amount
    # An amount of service being requested which can be a quantity ( for example $1,500 home modification), a ratio ( for example, 20 half day visits per month), or a range (2.0 to 1.8 Gy per fraction).
    attr_accessor :quantityRange                  # 0-1 Range
    ##
    # Service amount
    # An amount of service being requested which can be a quantity ( for example $1,500 home modification), a ratio ( for example, 20 half day visits per month), or a range (2.0 to 1.8 Gy per fraction).
    attr_accessor :quantityRatio                  # 0-1 Ratio
    ##
    # Individual or Entity the service is ordered for
    # On whom or what the service is to be performed. This is usually a human patient, but can also be requested on animals, groups of humans or animals, devices such as dialysis machines, or even locations (typically for environmental scans).
    attr_accessor :subject                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Location|http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Encounter in which the request was created
    # An encounter that provides additional information about the healthcare context in which this request is made.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # When service should occur
    # The date/time at which the requested service should occur.
    attr_accessor :occurrenceDateTime             # 0-1 DateTime
    ##
    # When service should occur
    # The date/time at which the requested service should occur.
    attr_accessor :occurrencePeriod               # 0-1 Period
    ##
    # When service should occur
    # The date/time at which the requested service should occur.
    attr_accessor :occurrenceTiming               # 0-1 Timing
    ##
    # Preconditions for service
    # If a CodeableConcept is present, it indicates the pre-condition for performing the service.  For example "pain", "on flare-up", etc.
    attr_accessor :asNeededBoolean                # 0-1 Boolean
    ##
    # Preconditions for service
    # If a CodeableConcept is present, it indicates the pre-condition for performing the service.  For example "pain", "on flare-up", etc.
    attr_accessor :asNeededCodeableConcept        # 0-1 CodeableConcept
    ##
    # Date request signed
    # When the request transitioned to being actionable.
    attr_accessor :authoredOn                     # 0-1 dateTime
    ##
    # Who/what is requesting service
    # The individual who initiated the request and has responsibility for its activation.
    # This not the dispatcher, but rather who is the authorizer.  This element is not intended to handle delegation which would generally be managed through the Provenance resource.
    attr_accessor :requester                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Performer role
    # Desired type of performer for doing the requested service.
    # This is a  role, not  a participation type.  In other words, does not describe the task but describes the capacity.  For example, “compounding pharmacy”, “psychiatrist” or “internal referral”.
    attr_accessor :performerType                  # 0-1 CodeableConcept
    ##
    # Requested performer
    # The desired performer for doing the requested service.  For example, the surgeon, dermatopathologist, endoscopist, etc.
    # If multiple performers are present, it is interpreted as a list of *alternative* performers without any preference regardless of order.  If order of preference is needed use the [request-performerOrder extension](extension-request-performerorder.html).  Use CareTeam to represent a group of performers (for example, Practitioner A *and* Practitioner B).
    attr_accessor :performer                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/HealthcareService|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/RelatedPerson) ]
    ##
    # Requested location
    # The preferred location(s) where the procedure should actually happen in coded or free text form. E.g. at home or nursing day care center.
    attr_accessor :locationCode                   # 0-* [ CodeableConcept ]
    ##
    # Requested location
    # A reference to the the preferred location(s) where the procedure should actually happen. E.g. at home or nursing day care center.
    attr_accessor :locationReference              # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Location) ]
    ##
    # Explanation/Justification for procedure or service
    # An explanation or justification for why this service is being requested in coded or textual form.   This is often for billing purposes.  May relate to the resources referred to in `supportingInfo`.
    # This element represents why the referral is being made and may be used to decide how the service will be performed, or even if it will be performed at all.   Use `CodeableConcept.text` element if the data is free (uncoded) text as shown in the [CT Scan example](servicerequest-example-di.html).
    attr_accessor :reasonCode                     # 0-* [ CodeableConcept ]
    ##
    # Explanation/Justification for service or service
    # Indicates another resource that provides a justification for why this service is being requested.   May relate to the resources referred to in `supportingInfo`.
    # This element represents why the referral is being made and may be used to decide how the service will be performed, or even if it will be performed at all.    To be as specific as possible,  a reference to  *Observation* or *Condition* should be used if available.  Otherwise when referencing  *DiagnosticReport*  it should contain a finding  in `DiagnosticReport.conclusion` and/or `DiagnosticReport.conclusionCode`.   When using a reference to *DocumentReference*, the target document should contain clear findings language providing the relevant reason for this service request.  Use  the CodeableConcept text element in `ServiceRequest.reasonCode` if the data is free (uncoded) text as shown in the [CT Scan example](servicerequest-example-di.html).
    attr_accessor :reasonReference                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Condition|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/DiagnosticReport|http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # Associated insurance coverage
    # Insurance plans, coverage extensions, pre-authorizations and/or pre-determinations that may be needed for delivering the requested service.
    attr_accessor :insurance                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Coverage|http://hl7.org/fhir/StructureDefinition/ClaimResponse) ]
    ##
    # Additional clinical information about the patient or specimen that may influence the services or their interpretations.     This information includes diagnosis, clinical findings and other observations.  In laboratory ordering these are typically referred to as "ask at order entry questions (AOEs)".  This includes observations explicitly requested by the producer (filler) to provide context or supporting information needed to complete the order. For example,  reporting the amount of inspired oxygen for blood gas measurements.
    # To represent information about how the services are to be delivered use the `instructions` element.
    attr_accessor :supportingInfo                 # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Procedure Samples
    # One or more specimens that the laboratory procedure will use.
    # Many diagnostic procedures need a specimen, but the request itself is not actually about the specimen. This element is for when the diagnostic is requested on already existing specimens and the request points to the specimen it applies to.    Conversely, if the request is entered first with an unknown specimen, then the [Specimen](specimen.html) resource points to the ServiceRequest.
    attr_accessor :specimen                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Specimen) ]
    ##
    # Location on Body
    # Anatomic location where the procedure should be performed. This is the target site.
    # Only used if not implicit in the code found in ServiceRequest.code.  If the use case requires BodySite to be handled as a separate resource instead of an inline coded element (e.g. to identify and track separately)  then use the standard extension [procedure-targetBodyStructure](extension-procedure-targetbodystructure.html).
    attr_accessor :bodySite                       # 0-* [ CodeableConcept ]
    ##
    # Comments
    # Any other notes and comments made about the service request. For example, internal billing notes.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Patient or consumer-oriented instructions
    # Instructions in terms that are understood by the patient or consumer.
    attr_accessor :patientInstruction             # 0-1 string
    ##
    # Request provenance
    # Key events in the history of the request.
    # This might not include provenances for all versions of the request – only those deemed “relevant” or important.This SHALL NOT include the Provenance associated with this current version of the resource.  (If that provenance is deemed to be a “relevant” change, it will need to be added as part of a later update.  Until then, it can be queried directly as the Provenance that points to this version using _revincludeAll Provenances should have some historical version of this Request as their subject.
    attr_accessor :relevantHistory                # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Provenance) ]

    def resourceType
      'ServiceRequest'
    end
  end
end
