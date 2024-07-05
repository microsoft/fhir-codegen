module FHIR

  ##
  # Demographics and other administrative information about an individual or animal receiving care or other health-related services.
  # Tracking patient is the center of the healthcare process.
  class Patient < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['active', 'address-city', 'address-country', 'address-postalcode', 'address-state', 'address-use', 'address', 'age', 'birthdate', 'birthOrderBoolean', 'death-date', 'deceased', 'email', 'family', 'gender', 'general-practitioner', 'given', 'identifier', 'language', 'link', 'mothersMaidenName', 'name', 'organization', 'part-agree', 'phone', 'phonetic', 'telecom']
    MULTIPLE_TYPES = {
      'deceased[x]' => ['boolean', 'dateTime'],
      'multipleBirth[x]' => ['boolean', 'integer']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Patient.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Patient.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Patient.implicitRules',
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
        'path'=>'Patient.language',
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
        'path'=>'Patient.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Patient.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Patient.extension',
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
        'path'=>'Patient.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # An identifier for this patient.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Patient.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Whether this patient's record is in active use
      # Whether this patient record is in active use. 
      # Many systems use this property to mark as non-current patients, such as those that have not been seen for a period of time based on an organization's business rules.
      # 
      # It is often used to filter patient lists to exclude inactive patients
      # 
      # Deceased patients may also be marked as inactive for the same reasons, but may be active for some time after death.
      # If a record is inactive, and linked to an active record, then future patient/record updates should occur on the other patient.
      'active' => {
        'type'=>'boolean',
        'path'=>'Patient.active',
        'min'=>0,
        'max'=>1
      },
      ##
      # A name associated with the patient
      # A name associated with the individual.
      # A patient may have multiple names with different uses or applicable periods. For animals, the name is a "HumanName" in the sense that is assigned and used by humans and has the same patterns.
      'name' => {
        'type'=>'HumanName',
        'path'=>'Patient.name',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A contact detail for the individual
      # A contact detail (e.g. a telephone number or an email address) by which the individual may be contacted.
      # A Patient may have multiple ways to be contacted with different uses or applicable periods.  May need to have options for contacting the person urgently and also to help with identification. The address might not go directly to the individual, but may reach another party that is able to proxy for the patient (i.e. home phone, or pet owner's phone).
      'telecom' => {
        'type'=>'ContactPoint',
        'path'=>'Patient.telecom',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # male | female | other | unknown
      # Administrative Gender - the gender that the patient is considered to have for administration and record keeping purposes.
      # The gender might not match the biological sex as determined by genetics or the individual's preferred identification. Note that for both humans and particularly animals, there are other legitimate possibilities than male and female, though the vast majority of systems and contexts only support male and female.  Systems providing decision support or enforcing business rules should ideally do this on the basis of Observations dealing with the specific sex or gender aspect of interest (anatomical, chromosomal, social, etc.)  However, because these observations are infrequently recorded, defaulting to the administrative gender is common practice.  Where such defaulting occurs, rule enforcement should allow for the variation between administrative and biological, chromosomal and other gender aspects.  For example, an alert about a hysterectomy on a male should be handled as a warning or overridable error, not a "hard" error.  See the Patient Gender and Sex section for additional information about communicating patient gender and sex.
      'gender' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/administrative-gender'=>[ 'male', 'female', 'other', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Patient.gender',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/administrative-gender'}
      },
      ##
      # The date of birth for the individual.
      # At least an estimated year should be provided as a guess if the real DOB is unknown  There is a standard extension "patient-birthTime" available that should be used where Time is required (such as in maternity/infant care systems).
      'birthDate' => {
        'type'=>'date',
        'path'=>'Patient.birthDate',
        'min'=>0,
        'max'=>1
      },
      ##
      # Indicates if the individual is deceased or not.
      # If there's no value in the instance, it means there is no statement on whether or not the individual is deceased. Most systems will interpret the absence of a value as a sign of the person being alive.
      'deceasedBoolean' => {
        'type'=>'Boolean',
        'path'=>'Patient.deceased[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Indicates if the individual is deceased or not.
      # If there's no value in the instance, it means there is no statement on whether or not the individual is deceased. Most systems will interpret the absence of a value as a sign of the person being alive.
      'deceasedDateTime' => {
        'type'=>'DateTime',
        'path'=>'Patient.deceased[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # An address for the individual.
      # Patient may have multiple addresses with different uses or applicable periods.
      'address' => {
        'type'=>'Address',
        'path'=>'Patient.address',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Marital (civil) status of a patient
      # This field contains a patient's most recent marital (civil) status.
      'maritalStatus' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-MaritalStatus'=>[ 'A', 'D', 'I', 'L', 'M', 'P', 'S', 'T', 'U', 'W' ],
          'http://terminology.hl7.org/CodeSystem/v3-NullFlavor'=>[ 'UNK' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Patient.maritalStatus',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/marital-status'}
      },
      ##
      # Whether patient is part of a multiple birth
      # Indicates whether the patient is part of a multiple (boolean) or indicates the actual birth order (integer).
      # Where the valueInteger is provided, the number is the birth number in the sequence. E.g. The middle birth in triplets would be valueInteger=2 and the third born would have valueInteger=3 If a boolean value was provided for this triplets example, then all 3 patient records would have valueBoolean=true (the ordering is not indicated).
      'multipleBirthBoolean' => {
        'type'=>'Boolean',
        'path'=>'Patient.multipleBirth[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Whether patient is part of a multiple birth
      # Indicates whether the patient is part of a multiple (boolean) or indicates the actual birth order (integer).
      # Where the valueInteger is provided, the number is the birth number in the sequence. E.g. The middle birth in triplets would be valueInteger=2 and the third born would have valueInteger=3 If a boolean value was provided for this triplets example, then all 3 patient records would have valueBoolean=true (the ordering is not indicated).
      'multipleBirthInteger' => {
        'type'=>'Integer',
        'path'=>'Patient.multipleBirth[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Image of the patient.
      # Guidelines:
      # * Use id photos, not clinical photos.
      # * Limit dimensions to thumbnail.
      # * Keep byte count low to ease resource updates.
      'photo' => {
        'type'=>'Attachment',
        'path'=>'Patient.photo',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A contact party (e.g. guardian, partner, friend) for the patient.
      # Contact covers all kinds of contact parties: family members, business contacts, guardians, caregivers. Not applicable to register pedigree and family ties beyond use of having contact.
      'contact' => {
        'type'=>'Patient::Contact',
        'path'=>'Patient.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A language which may be used to communicate with the patient about his or her health.
      # If no language is specified, this *implies* that the default local language is spoken.  If you need to convey proficiency for multiple modes, then you need multiple Patient.Communication associations.   For animals, language is not a relevant field, and should be absent from the instance. If the Patient does not speak the default local language, then the Interpreter Required Standard can be used to explicitly declare that an interpreter is required.
      'communication' => {
        'type'=>'Patient::Communication',
        'path'=>'Patient.communication',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Patient's nominated primary care provider
      # Patient's nominated care provider.
      # This may be the primary care provider (in a GP context), or it may be a patient nominated care manager in a community/disability setting, or even organization that will provide people to perform the care provider roles.  It is not to be used to record Care Teams, these should be in a CareTeam resource that may be linked to the CarePlan or EpisodeOfCare resources.
      # Multiple GPs may be recorded against the patient for various reasons, such as a student that has his home GP listed along with the GP at university during the school semesters, or a "fly-in/fly-out" worker that has the onsite GP also included with his home GP to remain aware of medical issues.
      # 
      # Jurisdictions may decide that they can profile this down to 1 if desired, or 1 per type.
      'generalPractitioner' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'Patient.generalPractitioner',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Organization that is the custodian of the patient record.
      # There is only one managing organization for a specific patient record. Other organizations will have their own Patient record, and may use the Link property to join the records together (or a Person resource which can include confidence ratings for the association).
      'managingOrganization' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Patient.managingOrganization',
        'min'=>0,
        'max'=>1
      },
      ##
      # Link to another patient resource that concerns the same actual person
      # Link to another patient resource that concerns the same actual patient.
      # There is no assumption that linked patient records have mutual links.
      'link' => {
        'type'=>'Patient::Link',
        'path'=>'Patient.link',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # A contact party (e.g. guardian, partner, friend) for the patient.
    # Contact covers all kinds of contact parties: family members, business contacts, guardians, caregivers. Not applicable to register pedigree and family ties beyond use of having contact.
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
        # The kind of relationship
        # The nature of the relationship between the patient and the contact person.
        'relationship' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v2-0131'=>[ 'C', 'E', 'F', 'I', 'N', 'S', 'U' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Contact.relationship',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/patient-contactrelationship'}
        },
        ##
        # A name associated with the contact person.
        'name' => {
          'type'=>'HumanName',
          'path'=>'Contact.name',
          'min'=>0,
          'max'=>1
        },
        ##
        # A contact detail for the person, e.g. a telephone number or an email address.
        # Contact may have multiple ways to be contacted with different uses or applicable periods.  May need to have options for contacting the person urgently, and also to help with identification.
        'telecom' => {
          'type'=>'ContactPoint',
          'path'=>'Contact.telecom',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Address for the contact person.
        'address' => {
          'type'=>'Address',
          'path'=>'Contact.address',
          'min'=>0,
          'max'=>1
        },
        ##
        # male | female | other | unknown
        # Administrative Gender - the gender that the contact person is considered to have for administration and record keeping purposes.
        'gender' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/administrative-gender'=>[ 'male', 'female', 'other', 'unknown' ]
          },
          'type'=>'code',
          'path'=>'Contact.gender',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/administrative-gender'}
        },
        ##
        # Organization that is associated with the contact
        # Organization on behalf of which the contact is acting or for which the contact is working.
        'organization' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Contact.organization',
          'min'=>0,
          'max'=>1
        },
        ##
        # The period during which this contact person or organization is valid to be contacted relating to this patient.
        'period' => {
          'type'=>'Period',
          'path'=>'Contact.period',
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
      # The kind of relationship
      # The nature of the relationship between the patient and the contact person.
      attr_accessor :relationship                   # 0-* [ CodeableConcept ]
      ##
      # A name associated with the contact person.
      attr_accessor :name                           # 0-1 HumanName
      ##
      # A contact detail for the person, e.g. a telephone number or an email address.
      # Contact may have multiple ways to be contacted with different uses or applicable periods.  May need to have options for contacting the person urgently, and also to help with identification.
      attr_accessor :telecom                        # 0-* [ ContactPoint ]
      ##
      # Address for the contact person.
      attr_accessor :address                        # 0-1 Address
      ##
      # male | female | other | unknown
      # Administrative Gender - the gender that the contact person is considered to have for administration and record keeping purposes.
      attr_accessor :gender                         # 0-1 code
      ##
      # Organization that is associated with the contact
      # Organization on behalf of which the contact is acting or for which the contact is working.
      attr_accessor :organization                   # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # The period during which this contact person or organization is valid to be contacted relating to this patient.
      attr_accessor :period                         # 0-1 Period
    end

    ##
    # A language which may be used to communicate with the patient about his or her health.
    # If no language is specified, this *implies* that the default local language is spoken.  If you need to convey proficiency for multiple modes, then you need multiple Patient.Communication associations.   For animals, language is not a relevant field, and should be absent from the instance. If the Patient does not speak the default local language, then the Interpreter Required Standard can be used to explicitly declare that an interpreter is required.
    class Communication < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Communication.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Communication.extension',
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
          'path'=>'Communication.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The language which can be used to communicate with the patient about his or her health
        # The ISO-639-1 alpha 2 code in lower case for the language, optionally followed by a hyphen and the ISO-3166-1 alpha 2 code for the region in upper case; e.g. "en" for English, or "en-US" for American English versus "en-EN" for England English.
        # The structure aa-BB with this exact casing is one the most widely used notations for locale. However not all systems actually code this but instead have it as free text. Hence CodeableConcept instead of code as the data type.
        'language' => {
          'valid_codes'=>{
            'urn:ietf:bcp:47'=>[ 'ar', 'bn', 'cs', 'da', 'de', 'de-AT', 'de-CH', 'de-DE', 'el', 'en', 'en-AU', 'en-CA', 'en-GB', 'en-IN', 'en-NZ', 'en-SG', 'en-US', 'es', 'es-AR', 'es-ES', 'es-UY', 'fi', 'fr', 'fr-BE', 'fr-CH', 'fr-FR', 'fy', 'fy-NL', 'hi', 'hr', 'it', 'it-CH', 'it-IT', 'ja', 'ko', 'nl', 'nl-BE', 'nl-NL', 'no', 'no-NO', 'pa', 'pl', 'pt', 'pt-BR', 'ru', 'ru-RU', 'sr', 'sr-RS', 'sv', 'sv-SE', 'te', 'zh', 'zh-CN', 'zh-HK', 'zh-SG', 'zh-TW' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Communication.language',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
        },
        ##
        # Language preference indicator
        # Indicates whether or not the patient prefers this language (over other languages he masters up a certain level).
        # This language is specifically identified for communicating healthcare information.
        'preferred' => {
          'type'=>'boolean',
          'path'=>'Communication.preferred',
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
      # The language which can be used to communicate with the patient about his or her health
      # The ISO-639-1 alpha 2 code in lower case for the language, optionally followed by a hyphen and the ISO-3166-1 alpha 2 code for the region in upper case; e.g. "en" for English, or "en-US" for American English versus "en-EN" for England English.
      # The structure aa-BB with this exact casing is one the most widely used notations for locale. However not all systems actually code this but instead have it as free text. Hence CodeableConcept instead of code as the data type.
      attr_accessor :language                       # 1-1 CodeableConcept
      ##
      # Language preference indicator
      # Indicates whether or not the patient prefers this language (over other languages he masters up a certain level).
      # This language is specifically identified for communicating healthcare information.
      attr_accessor :preferred                      # 0-1 boolean
    end

    ##
    # Link to another patient resource that concerns the same actual person
    # Link to another patient resource that concerns the same actual patient.
    # There is no assumption that linked patient records have mutual links.
    class Link < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Link.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Link.extension',
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
          'path'=>'Link.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The other patient or related person resource that the link refers to
        # The other patient resource that the link refers to.
        # Referencing a RelatedPerson here removes the need to use a Person record to associate a Patient and RelatedPerson as the same individual.
        'other' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
          'type'=>'Reference',
          'path'=>'Link.other',
          'min'=>1,
          'max'=>1
        },
        ##
        # replaced-by | replaces | refer | seealso
        # The type of link between this patient resource and another patient resource.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/link-type'=>[ 'replaced-by', 'replaces', 'refer', 'seealso' ]
          },
          'type'=>'code',
          'path'=>'Link.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/link-type'}
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
      # The other patient or related person resource that the link refers to
      # The other patient resource that the link refers to.
      # Referencing a RelatedPerson here removes the need to use a Person record to associate a Patient and RelatedPerson as the same individual.
      attr_accessor :other                          # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson)
      ##
      # replaced-by | replaces | refer | seealso
      # The type of link between this patient resource and another patient resource.
      attr_accessor :type                           # 1-1 code
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
    # An identifier for this patient.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Whether this patient's record is in active use
    # Whether this patient record is in active use. 
    # Many systems use this property to mark as non-current patients, such as those that have not been seen for a period of time based on an organization's business rules.
    # 
    # It is often used to filter patient lists to exclude inactive patients
    # 
    # Deceased patients may also be marked as inactive for the same reasons, but may be active for some time after death.
    # If a record is inactive, and linked to an active record, then future patient/record updates should occur on the other patient.
    attr_accessor :active                         # 0-1 boolean
    ##
    # A name associated with the patient
    # A name associated with the individual.
    # A patient may have multiple names with different uses or applicable periods. For animals, the name is a "HumanName" in the sense that is assigned and used by humans and has the same patterns.
    attr_accessor :name                           # 0-* [ HumanName ]
    ##
    # A contact detail for the individual
    # A contact detail (e.g. a telephone number or an email address) by which the individual may be contacted.
    # A Patient may have multiple ways to be contacted with different uses or applicable periods.  May need to have options for contacting the person urgently and also to help with identification. The address might not go directly to the individual, but may reach another party that is able to proxy for the patient (i.e. home phone, or pet owner's phone).
    attr_accessor :telecom                        # 0-* [ ContactPoint ]
    ##
    # male | female | other | unknown
    # Administrative Gender - the gender that the patient is considered to have for administration and record keeping purposes.
    # The gender might not match the biological sex as determined by genetics or the individual's preferred identification. Note that for both humans and particularly animals, there are other legitimate possibilities than male and female, though the vast majority of systems and contexts only support male and female.  Systems providing decision support or enforcing business rules should ideally do this on the basis of Observations dealing with the specific sex or gender aspect of interest (anatomical, chromosomal, social, etc.)  However, because these observations are infrequently recorded, defaulting to the administrative gender is common practice.  Where such defaulting occurs, rule enforcement should allow for the variation between administrative and biological, chromosomal and other gender aspects.  For example, an alert about a hysterectomy on a male should be handled as a warning or overridable error, not a "hard" error.  See the Patient Gender and Sex section for additional information about communicating patient gender and sex.
    attr_accessor :gender                         # 0-1 code
    ##
    # The date of birth for the individual.
    # At least an estimated year should be provided as a guess if the real DOB is unknown  There is a standard extension "patient-birthTime" available that should be used where Time is required (such as in maternity/infant care systems).
    attr_accessor :birthDate                      # 0-1 date
    ##
    # Indicates if the individual is deceased or not.
    # If there's no value in the instance, it means there is no statement on whether or not the individual is deceased. Most systems will interpret the absence of a value as a sign of the person being alive.
    attr_accessor :deceasedBoolean                # 0-1 Boolean
    ##
    # Indicates if the individual is deceased or not.
    # If there's no value in the instance, it means there is no statement on whether or not the individual is deceased. Most systems will interpret the absence of a value as a sign of the person being alive.
    attr_accessor :deceasedDateTime               # 0-1 DateTime
    ##
    # An address for the individual.
    # Patient may have multiple addresses with different uses or applicable periods.
    attr_accessor :address                        # 0-* [ Address ]
    ##
    # Marital (civil) status of a patient
    # This field contains a patient's most recent marital (civil) status.
    attr_accessor :maritalStatus                  # 0-1 CodeableConcept
    ##
    # Whether patient is part of a multiple birth
    # Indicates whether the patient is part of a multiple (boolean) or indicates the actual birth order (integer).
    # Where the valueInteger is provided, the number is the birth number in the sequence. E.g. The middle birth in triplets would be valueInteger=2 and the third born would have valueInteger=3 If a boolean value was provided for this triplets example, then all 3 patient records would have valueBoolean=true (the ordering is not indicated).
    attr_accessor :multipleBirthBoolean           # 0-1 Boolean
    ##
    # Whether patient is part of a multiple birth
    # Indicates whether the patient is part of a multiple (boolean) or indicates the actual birth order (integer).
    # Where the valueInteger is provided, the number is the birth number in the sequence. E.g. The middle birth in triplets would be valueInteger=2 and the third born would have valueInteger=3 If a boolean value was provided for this triplets example, then all 3 patient records would have valueBoolean=true (the ordering is not indicated).
    attr_accessor :multipleBirthInteger           # 0-1 Integer
    ##
    # Image of the patient.
    # Guidelines:
    # * Use id photos, not clinical photos.
    # * Limit dimensions to thumbnail.
    # * Keep byte count low to ease resource updates.
    attr_accessor :photo                          # 0-* [ Attachment ]
    ##
    # A contact party (e.g. guardian, partner, friend) for the patient.
    # Contact covers all kinds of contact parties: family members, business contacts, guardians, caregivers. Not applicable to register pedigree and family ties beyond use of having contact.
    attr_accessor :contact                        # 0-* [ Patient::Contact ]
    ##
    # A language which may be used to communicate with the patient about his or her health.
    # If no language is specified, this *implies* that the default local language is spoken.  If you need to convey proficiency for multiple modes, then you need multiple Patient.Communication associations.   For animals, language is not a relevant field, and should be absent from the instance. If the Patient does not speak the default local language, then the Interpreter Required Standard can be used to explicitly declare that an interpreter is required.
    attr_accessor :communication                  # 0-* [ Patient::Communication ]
    ##
    # Patient's nominated primary care provider
    # Patient's nominated care provider.
    # This may be the primary care provider (in a GP context), or it may be a patient nominated care manager in a community/disability setting, or even organization that will provide people to perform the care provider roles.  It is not to be used to record Care Teams, these should be in a CareTeam resource that may be linked to the CarePlan or EpisodeOfCare resources.
    # Multiple GPs may be recorded against the patient for various reasons, such as a student that has his home GP listed along with the GP at university during the school semesters, or a "fly-in/fly-out" worker that has the onsite GP also included with his home GP to remain aware of medical issues.
    # 
    # Jurisdictions may decide that they can profile this down to 1 if desired, or 1 per type.
    attr_accessor :generalPractitioner            # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole) ]
    ##
    # Organization that is the custodian of the patient record.
    # There is only one managing organization for a specific patient record. Other organizations will have their own Patient record, and may use the Link property to join the records together (or a Person resource which can include confidence ratings for the association).
    attr_accessor :managingOrganization           # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Link to another patient resource that concerns the same actual person
    # Link to another patient resource that concerns the same actual patient.
    # There is no assumption that linked patient records have mutual links.
    attr_accessor :link                           # 0-* [ Patient::Link ]

    def resourceType
      'Patient'
    end
  end
end
