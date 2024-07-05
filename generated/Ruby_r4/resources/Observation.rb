module FHIR

  ##
  # Measurements and simple assertions made about a patient, device or other subject.
  # Observations are a key aspect of healthcare.  This resource is used to capture those that do not require more sophisticated mechanisms.
  class Observation < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['amino-acid-change', 'based-on', 'category', 'code-value-concept', 'code-value-date', 'code-value-quantity', 'code-value-string', 'code', 'combo-code-value-concept', 'combo-code-value-quantity', 'combo-code', 'combo-data-absent-reason', 'combo-value-concept', 'combo-value-quantity', 'component-code-value-concept', 'component-code-value-quantity', 'component-code', 'component-data-absent-reason', 'component-value-concept', 'component-value-quantity', 'data-absent-reason', 'date', 'derived-from', 'device', 'dna-variant', 'encounter', 'focus', 'gene-amino-acid-change', 'gene-dnavariant', 'gene-identifier', 'has-member', 'identifier', 'method', 'part-of', 'patient', 'performer', 'specimen', 'status', 'subject', 'value-concept', 'value-date', 'value-quantity', 'value-string']
    MULTIPLE_TYPES = {
      'effective[x]' => ['dateTime', 'instant', 'Period', 'Timing'],
      'value[x]' => ['boolean', 'CodeableConcept', 'dateTime', 'integer', 'Period', 'Quantity', 'Range', 'Ratio', 'SampledData', 'string', 'time']
    }
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'Observation.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Observation.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Observation.implicitRules',
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
        'path'=>'Observation.language',
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
        'path'=>'Observation.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Observation.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Observation.extension',
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
        'path'=>'Observation.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for observation
      # A unique identifier assigned to this observation.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Observation.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Fulfills plan, proposal or order
      # A plan, proposal or order that is fulfilled in whole or in part by this event.  For example, a MedicationRequest may require a patient to have laboratory test performed before  it is dispensed.
      'basedOn' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CarePlan', 'http://hl7.org/fhir/StructureDefinition/DeviceRequest', 'http://hl7.org/fhir/StructureDefinition/ImmunizationRecommendation', 'http://hl7.org/fhir/StructureDefinition/MedicationRequest', 'http://hl7.org/fhir/StructureDefinition/NutritionOrder', 'http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'Observation.basedOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Part of referenced event
      # A larger event of which this particular Observation is a component or step.  For example,  an observation as part of a procedure.
      # To link an Observation to an Encounter use `encounter`.  See the  [Notes](observation.html#obsgrouping) below for guidance on referencing another Observation.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicationAdministration', 'http://hl7.org/fhir/StructureDefinition/MedicationDispense', 'http://hl7.org/fhir/StructureDefinition/MedicationStatement', 'http://hl7.org/fhir/StructureDefinition/Procedure', 'http://hl7.org/fhir/StructureDefinition/Immunization', 'http://hl7.org/fhir/StructureDefinition/ImagingStudy'],
        'type'=>'Reference',
        'path'=>'Observation.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # registered | preliminary | final | amended +
      # The status of the result value.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/observation-status'=>[ 'registered', 'preliminary', 'final', 'amended', 'corrected', 'cancelled', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'Observation.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/observation-status'}
      },
      ##
      # Classification of  type of observation
      # A code that classifies the general type of observation being made.
      # In addition to the required category valueset, this element allows various categorization schemes based on the owner’s definition of the category and effectively multiple categories can be used at once.  The level of granularity is defined by the category concepts in the value set.
      'category' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/observation-category'=>[ 'social-history', 'vital-signs', 'imaging', 'laboratory', 'procedure', 'survey', 'exam', 'therapy', 'activity' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Observation.category',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/observation-category'}
      },
      ##
      # Type of observation (code / type)
      # Describes what was observed. Sometimes this is called the observation "name".
      # *All* code-value and, if present, component.code-component.value pairs need to be taken into account to correctly understand the meaning of the observation.
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'Observation.code',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who and/or what the observation is about
      # The patient, or group of patients, location, or device this observation is about and into whose record the observation is placed. If the actual focus of the observation is different from the subject (or a sample of, part, or region of the subject), the `focus` element or the `code` itself specifies the actual focus of the observation.
      # One would expect this element to be a cardinality of 1..1. The only circumstance in which the subject can be missing is when the observation is made by a device that does not know the patient. In this case, the observation SHALL be matched to a patient through some context/channel matching technique, and at this point, the observation should be updated.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Observation.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # What the observation is about, when it is not about the subject of record
      # The actual focus of an observation when it is not the patient of record representing something or someone associated with the patient such as a spouse, parent, fetus, or donor. For example, fetus observations in a mother's record.  The focus of an observation could also be an existing condition,  an intervention, the subject's diet,  another observation of the subject,  or a body structure such as tumor or implanted device.   An example use case would be using the Observation resource to capture whether the mother is trained to change her child's tracheostomy tube. In this example, the child is the patient of record and the mother is the focus.
      # Typically, an observation is made about the subject - a patient, or group of patients, location, or device - and the distinction between the subject and what is directly measured for an observation is specified in the observation code itself ( e.g., "Blood Glucose") and does not need to be represented separately using this element.  Use `specimen` if a reference to a specimen is required.  If a code is required instead of a resource use either  `bodysite` for bodysites or the standard extension [focusCode](extension-observation-focuscode.html).
      'focus' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'Observation.focus',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Healthcare event during which this observation is made
      # The healthcare event  (e.g. a patient and healthcare provider interaction) during which this observation is made.
      # This will typically be the encounter the event occurred within, but some events may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter (e.g. pre-admission laboratory tests).
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'Observation.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Clinically relevant time/time-period for observation
      # The time or time-period the observed value is asserted as being true. For biological subjects - e.g. human patients - this is usually called the "physiologically relevant time". This is usually either the time of the procedure or of specimen collection, but very often the source of the date/time is not known, only the date/time itself.
      # At least a date should be present unless this observation is a historical report.  For recording imprecise or "fuzzy" times (For example, a blood glucose measurement taken "after breakfast") use the [Timing](datatypes.html#timing) datatype which allow the measurement to be tied to regular life events.
      'effectiveDateTime' => {
        'type'=>'DateTime',
        'path'=>'Observation.effective[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Clinically relevant time/time-period for observation
      # The time or time-period the observed value is asserted as being true. For biological subjects - e.g. human patients - this is usually called the "physiologically relevant time". This is usually either the time of the procedure or of specimen collection, but very often the source of the date/time is not known, only the date/time itself.
      # At least a date should be present unless this observation is a historical report.  For recording imprecise or "fuzzy" times (For example, a blood glucose measurement taken "after breakfast") use the [Timing](datatypes.html#timing) datatype which allow the measurement to be tied to regular life events.
      'effectiveInstant' => {
        'type'=>'Instant',
        'path'=>'Observation.effective[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Clinically relevant time/time-period for observation
      # The time or time-period the observed value is asserted as being true. For biological subjects - e.g. human patients - this is usually called the "physiologically relevant time". This is usually either the time of the procedure or of specimen collection, but very often the source of the date/time is not known, only the date/time itself.
      # At least a date should be present unless this observation is a historical report.  For recording imprecise or "fuzzy" times (For example, a blood glucose measurement taken "after breakfast") use the [Timing](datatypes.html#timing) datatype which allow the measurement to be tied to regular life events.
      'effectivePeriod' => {
        'type'=>'Period',
        'path'=>'Observation.effective[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Clinically relevant time/time-period for observation
      # The time or time-period the observed value is asserted as being true. For biological subjects - e.g. human patients - this is usually called the "physiologically relevant time". This is usually either the time of the procedure or of specimen collection, but very often the source of the date/time is not known, only the date/time itself.
      # At least a date should be present unless this observation is a historical report.  For recording imprecise or "fuzzy" times (For example, a blood glucose measurement taken "after breakfast") use the [Timing](datatypes.html#timing) datatype which allow the measurement to be tied to regular life events.
      'effectiveTiming' => {
        'type'=>'Timing',
        'path'=>'Observation.effective[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date/Time this version was made available
      # The date and time this version of the observation was made available to providers, typically after the results have been reviewed and verified.
      # For Observations that don’t require review and verification, it may be the same as the [`lastUpdated` ](resource-definitions.html#Meta.lastUpdated) time of the resource itself.  For Observations that do require review and verification for certain updates, it might not be the same as the `lastUpdated` time of the resource itself due to a non-clinically significant update that doesn’t require the new version to be reviewed and verified again.
      'issued' => {
        'type'=>'instant',
        'path'=>'Observation.issued',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who is responsible for the observation
      # Who was responsible for asserting the observed value as "true".
      'performer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Organization', 'http://hl7.org/fhir/StructureDefinition/CareTeam', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson'],
        'type'=>'Reference',
        'path'=>'Observation.performer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Actual result
      # The information determined as a result of making the observation, if the information has a simple value.
      # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      'valueBoolean' => {
        'type'=>'Boolean',
        'path'=>'Observation.value[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Actual result
      # The information determined as a result of making the observation, if the information has a simple value.
      # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      'valueCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'Observation.value[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Actual result
      # The information determined as a result of making the observation, if the information has a simple value.
      # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      'valueDateTime' => {
        'type'=>'DateTime',
        'path'=>'Observation.value[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Actual result
      # The information determined as a result of making the observation, if the information has a simple value.
      # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      'valueInteger' => {
        'type'=>'Integer',
        'path'=>'Observation.value[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Actual result
      # The information determined as a result of making the observation, if the information has a simple value.
      # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      'valuePeriod' => {
        'type'=>'Period',
        'path'=>'Observation.value[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Actual result
      # The information determined as a result of making the observation, if the information has a simple value.
      # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      'valueQuantity' => {
        'type'=>'Quantity',
        'path'=>'Observation.value[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Actual result
      # The information determined as a result of making the observation, if the information has a simple value.
      # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      'valueRange' => {
        'type'=>'Range',
        'path'=>'Observation.value[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Actual result
      # The information determined as a result of making the observation, if the information has a simple value.
      # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      'valueRatio' => {
        'type'=>'Ratio',
        'path'=>'Observation.value[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Actual result
      # The information determined as a result of making the observation, if the information has a simple value.
      # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      'valueSampledData' => {
        'type'=>'SampledData',
        'path'=>'Observation.value[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Actual result
      # The information determined as a result of making the observation, if the information has a simple value.
      # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      'valueString' => {
        'type'=>'String',
        'path'=>'Observation.value[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Actual result
      # The information determined as a result of making the observation, if the information has a simple value.
      # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      'valueTime' => {
        'type'=>'Time',
        'path'=>'Observation.value[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why the result is missing
      # Provides a reason why the expected value in the element Observation.value[x] is missing.
      # Null or exceptional values can be represented two ways in FHIR Observations.  One way is to simply include them in the value set and represent the exceptions in the value.  For example, measurement values for a serology test could be  "detected", "not detected", "inconclusive", or  "specimen unsatisfactory".   
      # 
      # The alternate way is to use the value element for actual observations and use the explicit dataAbsentReason element to record exceptional values.  For example, the dataAbsentReason code "error" could be used when the measurement was not completed. Note that an observation may only be reported if there are values to report. For example differential cell counts values may be reported only when > 0.  Because of these options, use-case agreements are required to interpret general observations for null or exceptional values.
      'dataAbsentReason' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/data-absent-reason'=>[ 'unknown', 'asked-unknown', 'temp-unknown', 'not-asked', 'asked-declined', 'masked', 'not-applicable', 'unsupported', 'as-text', 'error', 'not-a-number', 'negative-infinity', 'positive-infinity', 'not-performed', 'not-permitted' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Observation.dataAbsentReason',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/data-absent-reason'}
      },
      ##
      # High, low, normal, etc.
      # A categorical assessment of an observation value.  For example, high, low, normal.
      # Historically used for laboratory results (known as 'abnormal flag' ),  its use extends to other use cases where coded interpretations  are relevant.  Often reported as one or more simple compact codes this element is often placed adjacent to the result value in reports and flow sheets to signal the meaning/normalcy status of the result.
      'interpretation' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ObservationInterpretation'=>[ 'CAR', 'B', 'D', 'U', 'W', '<', '>', 'IE', 'A', 'AA', 'HH', 'LL', 'H', 'HU', 'L', 'LU', 'N', 'I', 'NCL', 'NS', 'R', 'SYN-R', 'S', 'SDD', 'SYN-S', 'EX', 'HX', 'LX', 'IND', 'E', 'NEG', 'ND', 'POS', 'DET', 'EXP', 'UNE', 'NR', 'RR', 'WR' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Observation.interpretation',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/observation-interpretation'}
      },
      ##
      # Comments about the observation or the results.
      # May include general statements about the observation, or statements about significant, unexpected or unreliable results values, or information about its source when relevant to its interpretation.
      'note' => {
        'type'=>'Annotation',
        'path'=>'Observation.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Observed body part
      # Indicates the site on the subject's body where the observation was made (i.e. the target site).
      # Only used if not implicit in code found in Observation.code.  In many systems, this may be represented as a related observation instead of an inline component.   
      # 
      # If the use case requires BodySite to be handled as a separate resource (e.g. to identify and track separately) then use the standard extension[ bodySite](extension-bodysite.html).
      'bodySite' => {
        'type'=>'CodeableConcept',
        'path'=>'Observation.bodySite',
        'min'=>0,
        'max'=>1
      },
      ##
      # How it was done
      # Indicates the mechanism used to perform the observation.
      # Only used if not implicit in code for Observation.code.
      'method' => {
        'local_name'=>'local_method'
        'type'=>'CodeableConcept',
        'path'=>'Observation.method',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specimen used for this observation
      # The specimen that was used when this observation was made.
      # Should only be used if not implicit in code found in `Observation.code`.  Observations are not made on specimens themselves; they are made on a subject, but in many cases by the means of a specimen. Note that although specimens are often involved, they are not always tracked and reported explicitly. Also note that observation resources may be used in contexts that track the specimen explicitly (e.g. Diagnostic Report).
      'specimen' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Specimen'],
        'type'=>'Reference',
        'path'=>'Observation.specimen',
        'min'=>0,
        'max'=>1
      },
      ##
      # (Measurement) Device
      # The device used to generate the observation data.
      # Note that this is not meant to represent a device involved in the transmission of the result, e.g., a gateway.  Such devices may be documented using the Provenance resource where relevant.
      'device' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/DeviceMetric'],
        'type'=>'Reference',
        'path'=>'Observation.device',
        'min'=>0,
        'max'=>1
      },
      ##
      # Provides guide for interpretation
      # Guidance on how to interpret the value by comparison to a normal or recommended range.  Multiple reference ranges are interpreted as an "OR".   In other words, to represent two distinct target populations, two `referenceRange` elements would be used.
      # Most observations only have one generic reference range. Systems MAY choose to restrict to only supplying the relevant reference range based on knowledge about the patient (e.g., specific to the patient's age, gender, weight and other factors), but this might not be possible or appropriate. Whenever more than one reference range is supplied, the differences between them SHOULD be provided in the reference range and/or age properties.
      'referenceRange' => {
        'type'=>'Observation::ReferenceRange',
        'path'=>'Observation.referenceRange',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Related resource that belongs to the Observation group
      # This observation is a group observation (e.g. a battery, a panel of tests, a set of vital sign measurements) that includes the target as a member of the group.
      # When using this element, an observation will typically have either a value or a set of related resources, although both may be present in some cases.  For a discussion on the ways Observations can assembled in groups together, see [Notes](observation.html#obsgrouping) below.  Note that a system may calculate results from [QuestionnaireResponse](questionnaireresponse.html)  into a final score and represent the score as an Observation.
      'hasMember' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse', 'http://hl7.org/fhir/StructureDefinition/MolecularSequence'],
        'type'=>'Reference',
        'path'=>'Observation.hasMember',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Related measurements the observation is made from
      # The target resource that represents a measurement from which this observation value is derived. For example, a calculated anion gap or a fetal measurement based on an ultrasound image.
      # All the reference choices that are listed in this element can represent clinical observations and other measurements that may be the source for a derived value.  The most common reference will be another Observation.  For a discussion on the ways Observations can assembled in groups together, see [Notes](observation.html#obsgrouping) below.
      'derivedFrom' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference', 'http://hl7.org/fhir/StructureDefinition/ImagingStudy', 'http://hl7.org/fhir/StructureDefinition/Media', 'http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse', 'http://hl7.org/fhir/StructureDefinition/Observation', 'http://hl7.org/fhir/StructureDefinition/MolecularSequence'],
        'type'=>'Reference',
        'path'=>'Observation.derivedFrom',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Component results
      # Some observations have multiple component observations.  These component observations are expressed as separate code value pairs that share the same attributes.  Examples include systolic and diastolic component observations for blood pressure measurement and multiple component observations for genetics observations.
      # For a discussion on the ways Observations can be assembled in groups together see [Notes](observation.html#notes) below.
      'component' => {
        'type'=>'Observation::Component',
        'path'=>'Observation.component',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Provides guide for interpretation
    # Guidance on how to interpret the value by comparison to a normal or recommended range.  Multiple reference ranges are interpreted as an "OR".   In other words, to represent two distinct target populations, two `referenceRange` elements would be used.
    # Most observations only have one generic reference range. Systems MAY choose to restrict to only supplying the relevant reference range based on knowledge about the patient (e.g., specific to the patient's age, gender, weight and other factors), but this might not be possible or appropriate. Whenever more than one reference range is supplied, the differences between them SHOULD be provided in the reference range and/or age properties.
    class ReferenceRange < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'ReferenceRange.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'ReferenceRange.extension',
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
          'path'=>'ReferenceRange.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Low Range, if relevant
        # The value of the low bound of the reference range.  The low bound of the reference range endpoint is inclusive of the value (e.g.  reference range is >=5 - <=9). If the low bound is omitted,  it is assumed to be meaningless (e.g. reference range is <=2.3).
        'low' => {
          'type'=>'Quantity',
          'path'=>'ReferenceRange.low',
          'min'=>0,
          'max'=>1
        },
        ##
        # High Range, if relevant
        # The value of the high bound of the reference range.  The high bound of the reference range endpoint is inclusive of the value (e.g.  reference range is >=5 - <=9). If the high bound is omitted,  it is assumed to be meaningless (e.g. reference range is >= 2.3).
        'high' => {
          'type'=>'Quantity',
          'path'=>'ReferenceRange.high',
          'min'=>0,
          'max'=>1
        },
        ##
        # Reference range qualifier
        # Codes to indicate the what part of the targeted reference population it applies to. For example, the normal or therapeutic range.
        # This SHOULD be populated if there is more than one range.  If this element is not present then the normal range is assumed.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/referencerange-meaning'=>[ 'type', 'normal', 'recommended', 'treatment', 'therapeutic', 'pre', 'post', 'endocrine', 'pre-puberty', 'follicular', 'midcycle', 'luteal', 'postmenopausal' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'ReferenceRange.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/referencerange-meaning'}
        },
        ##
        # Reference range population
        # Codes to indicate the target population this reference range applies to.  For example, a reference range may be based on the normal population or a particular sex or race.  Multiple `appliesTo`  are interpreted as an "AND" of the target populations.  For example, to represent a target population of African American females, both a code of female and a code for African American would be used.
        # This SHOULD be populated if there is more than one range.  If this element is not present then the normal population is assumed.
        'appliesTo' => {
          'type'=>'CodeableConcept',
          'path'=>'ReferenceRange.appliesTo',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Applicable age range, if relevant
        # The age at which this reference range is applicable. This is a neonatal age (e.g. number of weeks at term) if the meaning says so.
        'age' => {
          'type'=>'Range',
          'path'=>'ReferenceRange.age',
          'min'=>0,
          'max'=>1
        },
        ##
        # Text based reference range in an observation which may be used when a quantitative range is not appropriate for an observation.  An example would be a reference value of "Negative" or a list or table of "normals".
        'text' => {
          'type'=>'string',
          'path'=>'ReferenceRange.text',
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
      # Low Range, if relevant
      # The value of the low bound of the reference range.  The low bound of the reference range endpoint is inclusive of the value (e.g.  reference range is >=5 - <=9). If the low bound is omitted,  it is assumed to be meaningless (e.g. reference range is <=2.3).
      attr_accessor :low                            # 0-1 Quantity
      ##
      # High Range, if relevant
      # The value of the high bound of the reference range.  The high bound of the reference range endpoint is inclusive of the value (e.g.  reference range is >=5 - <=9). If the high bound is omitted,  it is assumed to be meaningless (e.g. reference range is >= 2.3).
      attr_accessor :high                           # 0-1 Quantity
      ##
      # Reference range qualifier
      # Codes to indicate the what part of the targeted reference population it applies to. For example, the normal or therapeutic range.
      # This SHOULD be populated if there is more than one range.  If this element is not present then the normal range is assumed.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Reference range population
      # Codes to indicate the target population this reference range applies to.  For example, a reference range may be based on the normal population or a particular sex or race.  Multiple `appliesTo`  are interpreted as an "AND" of the target populations.  For example, to represent a target population of African American females, both a code of female and a code for African American would be used.
      # This SHOULD be populated if there is more than one range.  If this element is not present then the normal population is assumed.
      attr_accessor :appliesTo                      # 0-* [ CodeableConcept ]
      ##
      # Applicable age range, if relevant
      # The age at which this reference range is applicable. This is a neonatal age (e.g. number of weeks at term) if the meaning says so.
      attr_accessor :age                            # 0-1 Range
      ##
      # Text based reference range in an observation which may be used when a quantitative range is not appropriate for an observation.  An example would be a reference value of "Negative" or a list or table of "normals".
      attr_accessor :text                           # 0-1 string
    end

    ##
    # Component results
    # Some observations have multiple component observations.  These component observations are expressed as separate code value pairs that share the same attributes.  Examples include systolic and diastolic component observations for blood pressure measurement and multiple component observations for genetics observations.
    # For a discussion on the ways Observations can be assembled in groups together see [Notes](observation.html#notes) below.
    class Component < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'value[x]' => ['boolean', 'CodeableConcept', 'dateTime', 'integer', 'Period', 'Quantity', 'Range', 'Ratio', 'SampledData', 'string', 'time']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Component.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Component.extension',
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
          'path'=>'Component.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of component observation (code / type)
        # Describes what was observed. Sometimes this is called the observation "code".
        # *All* code-value and  component.code-component.value pairs need to be taken into account to correctly understand the meaning of the observation.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Component.code',
          'min'=>1,
          'max'=>1
        },
        ##
        # Actual component result
        # The information determined as a result of making the observation, if the information has a simple value.
        # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
        'valueBoolean' => {
          'type'=>'Boolean',
          'path'=>'Component.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual component result
        # The information determined as a result of making the observation, if the information has a simple value.
        # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
        'valueCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Component.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual component result
        # The information determined as a result of making the observation, if the information has a simple value.
        # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
        'valueDateTime' => {
          'type'=>'DateTime',
          'path'=>'Component.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual component result
        # The information determined as a result of making the observation, if the information has a simple value.
        # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
        'valueInteger' => {
          'type'=>'Integer',
          'path'=>'Component.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual component result
        # The information determined as a result of making the observation, if the information has a simple value.
        # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
        'valuePeriod' => {
          'type'=>'Period',
          'path'=>'Component.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual component result
        # The information determined as a result of making the observation, if the information has a simple value.
        # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'Component.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual component result
        # The information determined as a result of making the observation, if the information has a simple value.
        # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
        'valueRange' => {
          'type'=>'Range',
          'path'=>'Component.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual component result
        # The information determined as a result of making the observation, if the information has a simple value.
        # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
        'valueRatio' => {
          'type'=>'Ratio',
          'path'=>'Component.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual component result
        # The information determined as a result of making the observation, if the information has a simple value.
        # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
        'valueSampledData' => {
          'type'=>'SampledData',
          'path'=>'Component.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual component result
        # The information determined as a result of making the observation, if the information has a simple value.
        # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
        'valueString' => {
          'type'=>'String',
          'path'=>'Component.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual component result
        # The information determined as a result of making the observation, if the information has a simple value.
        # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
        'valueTime' => {
          'type'=>'Time',
          'path'=>'Component.value[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Why the component result is missing
        # Provides a reason why the expected value in the element Observation.component.value[x] is missing.
        # "Null" or exceptional values can be represented two ways in FHIR Observations.  One way is to simply include them in the value set and represent the exceptions in the value.  For example, measurement values for a serology test could be  "detected", "not detected", "inconclusive", or  "test not done". 
        # 
        # The alternate way is to use the value element for actual observations and use the explicit dataAbsentReason element to record exceptional values.  For example, the dataAbsentReason code "error" could be used when the measurement was not completed.  Because of these options, use-case agreements are required to interpret general observations for exceptional values.
        'dataAbsentReason' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/data-absent-reason'=>[ 'unknown', 'asked-unknown', 'temp-unknown', 'not-asked', 'asked-declined', 'masked', 'not-applicable', 'unsupported', 'as-text', 'error', 'not-a-number', 'negative-infinity', 'positive-infinity', 'not-performed', 'not-permitted' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Component.dataAbsentReason',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/data-absent-reason'}
        },
        ##
        # High, low, normal, etc.
        # A categorical assessment of an observation value.  For example, high, low, normal.
        # Historically used for laboratory results (known as 'abnormal flag' ),  its use extends to other use cases where coded interpretations  are relevant.  Often reported as one or more simple compact codes this element is often placed adjacent to the result value in reports and flow sheets to signal the meaning/normalcy status of the result.
        'interpretation' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-ObservationInterpretation'=>[ 'CAR', 'B', 'D', 'U', 'W', '<', '>', 'IE', 'A', 'AA', 'HH', 'LL', 'H', 'HU', 'L', 'LU', 'N', 'I', 'NCL', 'NS', 'R', 'SYN-R', 'S', 'SDD', 'SYN-S', 'EX', 'HX', 'LX', 'IND', 'E', 'NEG', 'ND', 'POS', 'DET', 'EXP', 'UNE', 'NR', 'RR', 'WR' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Component.interpretation',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/observation-interpretation'}
        },
        ##
        # Provides guide for interpretation of component result
        # Guidance on how to interpret the value by comparison to a normal or recommended range.
        # Most observations only have one generic reference range. Systems MAY choose to restrict to only supplying the relevant reference range based on knowledge about the patient (e.g., specific to the patient's age, gender, weight and other factors), but this might not be possible or appropriate. Whenever more than one reference range is supplied, the differences between them SHOULD be provided in the reference range and/or age properties.
        'referenceRange' => {
          'type'=>'Observation::ReferenceRange',
          'path'=>'Component.referenceRange',
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
      # Type of component observation (code / type)
      # Describes what was observed. Sometimes this is called the observation "code".
      # *All* code-value and  component.code-component.value pairs need to be taken into account to correctly understand the meaning of the observation.
      attr_accessor :code                           # 1-1 CodeableConcept
      ##
      # Actual component result
      # The information determined as a result of making the observation, if the information has a simple value.
      # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      attr_accessor :valueBoolean                   # 0-1 Boolean
      ##
      # Actual component result
      # The information determined as a result of making the observation, if the information has a simple value.
      # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      attr_accessor :valueCodeableConcept           # 0-1 CodeableConcept
      ##
      # Actual component result
      # The information determined as a result of making the observation, if the information has a simple value.
      # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      attr_accessor :valueDateTime                  # 0-1 DateTime
      ##
      # Actual component result
      # The information determined as a result of making the observation, if the information has a simple value.
      # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      attr_accessor :valueInteger                   # 0-1 Integer
      ##
      # Actual component result
      # The information determined as a result of making the observation, if the information has a simple value.
      # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      attr_accessor :valuePeriod                    # 0-1 Period
      ##
      # Actual component result
      # The information determined as a result of making the observation, if the information has a simple value.
      # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      attr_accessor :valueQuantity                  # 0-1 Quantity
      ##
      # Actual component result
      # The information determined as a result of making the observation, if the information has a simple value.
      # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      attr_accessor :valueRange                     # 0-1 Range
      ##
      # Actual component result
      # The information determined as a result of making the observation, if the information has a simple value.
      # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      attr_accessor :valueRatio                     # 0-1 Ratio
      ##
      # Actual component result
      # The information determined as a result of making the observation, if the information has a simple value.
      # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      attr_accessor :valueSampledData               # 0-1 SampledData
      ##
      # Actual component result
      # The information determined as a result of making the observation, if the information has a simple value.
      # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      attr_accessor :valueString                    # 0-1 String
      ##
      # Actual component result
      # The information determined as a result of making the observation, if the information has a simple value.
      # Used when observation has a set of component observations. An observation may have both a value (e.g. an  Apgar score)  and component observations (the observations from which the Apgar score was derived). If a value is present, the datatype for this element should be determined by Observation.code. A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
      attr_accessor :valueTime                      # 0-1 Time
      ##
      # Why the component result is missing
      # Provides a reason why the expected value in the element Observation.component.value[x] is missing.
      # "Null" or exceptional values can be represented two ways in FHIR Observations.  One way is to simply include them in the value set and represent the exceptions in the value.  For example, measurement values for a serology test could be  "detected", "not detected", "inconclusive", or  "test not done". 
      # 
      # The alternate way is to use the value element for actual observations and use the explicit dataAbsentReason element to record exceptional values.  For example, the dataAbsentReason code "error" could be used when the measurement was not completed.  Because of these options, use-case agreements are required to interpret general observations for exceptional values.
      attr_accessor :dataAbsentReason               # 0-1 CodeableConcept
      ##
      # High, low, normal, etc.
      # A categorical assessment of an observation value.  For example, high, low, normal.
      # Historically used for laboratory results (known as 'abnormal flag' ),  its use extends to other use cases where coded interpretations  are relevant.  Often reported as one or more simple compact codes this element is often placed adjacent to the result value in reports and flow sheets to signal the meaning/normalcy status of the result.
      attr_accessor :interpretation                 # 0-* [ CodeableConcept ]
      ##
      # Provides guide for interpretation of component result
      # Guidance on how to interpret the value by comparison to a normal or recommended range.
      # Most observations only have one generic reference range. Systems MAY choose to restrict to only supplying the relevant reference range based on knowledge about the patient (e.g., specific to the patient's age, gender, weight and other factors), but this might not be possible or appropriate. Whenever more than one reference range is supplied, the differences between them SHOULD be provided in the reference range and/or age properties.
      attr_accessor :referenceRange                 # 0-* [ Observation::ReferenceRange ]
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
    # Business Identifier for observation
    # A unique identifier assigned to this observation.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Fulfills plan, proposal or order
    # A plan, proposal or order that is fulfilled in whole or in part by this event.  For example, a MedicationRequest may require a patient to have laboratory test performed before  it is dispensed.
    attr_accessor :basedOn                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/CarePlan|http://hl7.org/fhir/StructureDefinition/DeviceRequest|http://hl7.org/fhir/StructureDefinition/ImmunizationRecommendation|http://hl7.org/fhir/StructureDefinition/MedicationRequest|http://hl7.org/fhir/StructureDefinition/NutritionOrder|http://hl7.org/fhir/StructureDefinition/ServiceRequest) ]
    ##
    # Part of referenced event
    # A larger event of which this particular Observation is a component or step.  For example,  an observation as part of a procedure.
    # To link an Observation to an Encounter use `encounter`.  See the  [Notes](observation.html#obsgrouping) below for guidance on referencing another Observation.
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/MedicationAdministration|http://hl7.org/fhir/StructureDefinition/MedicationDispense|http://hl7.org/fhir/StructureDefinition/MedicationStatement|http://hl7.org/fhir/StructureDefinition/Procedure|http://hl7.org/fhir/StructureDefinition/Immunization|http://hl7.org/fhir/StructureDefinition/ImagingStudy) ]
    ##
    # registered | preliminary | final | amended +
    # The status of the result value.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Classification of  type of observation
    # A code that classifies the general type of observation being made.
    # In addition to the required category valueset, this element allows various categorization schemes based on the owner’s definition of the category and effectively multiple categories can be used at once.  The level of granularity is defined by the category concepts in the value set.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # Type of observation (code / type)
    # Describes what was observed. Sometimes this is called the observation "name".
    # *All* code-value and, if present, component.code-component.value pairs need to be taken into account to correctly understand the meaning of the observation.
    attr_accessor :code                           # 1-1 CodeableConcept
    ##
    # Who and/or what the observation is about
    # The patient, or group of patients, location, or device this observation is about and into whose record the observation is placed. If the actual focus of the observation is different from the subject (or a sample of, part, or region of the subject), the `focus` element or the `code` itself specifies the actual focus of the observation.
    # One would expect this element to be a cardinality of 1..1. The only circumstance in which the subject can be missing is when the observation is made by a device that does not know the patient. In this case, the observation SHALL be matched to a patient through some context/channel matching technique, and at this point, the observation should be updated.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # What the observation is about, when it is not about the subject of record
    # The actual focus of an observation when it is not the patient of record representing something or someone associated with the patient such as a spouse, parent, fetus, or donor. For example, fetus observations in a mother's record.  The focus of an observation could also be an existing condition,  an intervention, the subject's diet,  another observation of the subject,  or a body structure such as tumor or implanted device.   An example use case would be using the Observation resource to capture whether the mother is trained to change her child's tracheostomy tube. In this example, the child is the patient of record and the mother is the focus.
    # Typically, an observation is made about the subject - a patient, or group of patients, location, or device - and the distinction between the subject and what is directly measured for an observation is specified in the observation code itself ( e.g., "Blood Glucose") and does not need to be represented separately using this element.  Use `specimen` if a reference to a specimen is required.  If a code is required instead of a resource use either  `bodysite` for bodysites or the standard extension [focusCode](extension-observation-focuscode.html).
    attr_accessor :focus                          # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Healthcare event during which this observation is made
    # The healthcare event  (e.g. a patient and healthcare provider interaction) during which this observation is made.
    # This will typically be the encounter the event occurred within, but some events may be initiated prior to or after the official completion of an encounter but still be tied to the context of the encounter (e.g. pre-admission laboratory tests).
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Clinically relevant time/time-period for observation
    # The time or time-period the observed value is asserted as being true. For biological subjects - e.g. human patients - this is usually called the "physiologically relevant time". This is usually either the time of the procedure or of specimen collection, but very often the source of the date/time is not known, only the date/time itself.
    # At least a date should be present unless this observation is a historical report.  For recording imprecise or "fuzzy" times (For example, a blood glucose measurement taken "after breakfast") use the [Timing](datatypes.html#timing) datatype which allow the measurement to be tied to regular life events.
    attr_accessor :effectiveDateTime              # 0-1 DateTime
    ##
    # Clinically relevant time/time-period for observation
    # The time or time-period the observed value is asserted as being true. For biological subjects - e.g. human patients - this is usually called the "physiologically relevant time". This is usually either the time of the procedure or of specimen collection, but very often the source of the date/time is not known, only the date/time itself.
    # At least a date should be present unless this observation is a historical report.  For recording imprecise or "fuzzy" times (For example, a blood glucose measurement taken "after breakfast") use the [Timing](datatypes.html#timing) datatype which allow the measurement to be tied to regular life events.
    attr_accessor :effectiveInstant               # 0-1 Instant
    ##
    # Clinically relevant time/time-period for observation
    # The time or time-period the observed value is asserted as being true. For biological subjects - e.g. human patients - this is usually called the "physiologically relevant time". This is usually either the time of the procedure or of specimen collection, but very often the source of the date/time is not known, only the date/time itself.
    # At least a date should be present unless this observation is a historical report.  For recording imprecise or "fuzzy" times (For example, a blood glucose measurement taken "after breakfast") use the [Timing](datatypes.html#timing) datatype which allow the measurement to be tied to regular life events.
    attr_accessor :effectivePeriod                # 0-1 Period
    ##
    # Clinically relevant time/time-period for observation
    # The time or time-period the observed value is asserted as being true. For biological subjects - e.g. human patients - this is usually called the "physiologically relevant time". This is usually either the time of the procedure or of specimen collection, but very often the source of the date/time is not known, only the date/time itself.
    # At least a date should be present unless this observation is a historical report.  For recording imprecise or "fuzzy" times (For example, a blood glucose measurement taken "after breakfast") use the [Timing](datatypes.html#timing) datatype which allow the measurement to be tied to regular life events.
    attr_accessor :effectiveTiming                # 0-1 Timing
    ##
    # Date/Time this version was made available
    # The date and time this version of the observation was made available to providers, typically after the results have been reviewed and verified.
    # For Observations that don’t require review and verification, it may be the same as the [`lastUpdated` ](resource-definitions.html#Meta.lastUpdated) time of the resource itself.  For Observations that do require review and verification for certain updates, it might not be the same as the `lastUpdated` time of the resource itself due to a non-clinically significant update that doesn’t require the new version to be reviewed and verified again.
    attr_accessor :issued                         # 0-1 instant
    ##
    # Who is responsible for the observation
    # Who was responsible for asserting the observed value as "true".
    attr_accessor :performer                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Organization|http://hl7.org/fhir/StructureDefinition/CareTeam|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/RelatedPerson) ]
    ##
    # Actual result
    # The information determined as a result of making the observation, if the information has a simple value.
    # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
    attr_accessor :valueBoolean                   # 0-1 Boolean
    ##
    # Actual result
    # The information determined as a result of making the observation, if the information has a simple value.
    # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
    attr_accessor :valueCodeableConcept           # 0-1 CodeableConcept
    ##
    # Actual result
    # The information determined as a result of making the observation, if the information has a simple value.
    # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
    attr_accessor :valueDateTime                  # 0-1 DateTime
    ##
    # Actual result
    # The information determined as a result of making the observation, if the information has a simple value.
    # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
    attr_accessor :valueInteger                   # 0-1 Integer
    ##
    # Actual result
    # The information determined as a result of making the observation, if the information has a simple value.
    # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
    attr_accessor :valuePeriod                    # 0-1 Period
    ##
    # Actual result
    # The information determined as a result of making the observation, if the information has a simple value.
    # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
    attr_accessor :valueQuantity                  # 0-1 Quantity
    ##
    # Actual result
    # The information determined as a result of making the observation, if the information has a simple value.
    # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
    attr_accessor :valueRange                     # 0-1 Range
    ##
    # Actual result
    # The information determined as a result of making the observation, if the information has a simple value.
    # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
    attr_accessor :valueRatio                     # 0-1 Ratio
    ##
    # Actual result
    # The information determined as a result of making the observation, if the information has a simple value.
    # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
    attr_accessor :valueSampledData               # 0-1 SampledData
    ##
    # Actual result
    # The information determined as a result of making the observation, if the information has a simple value.
    # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
    attr_accessor :valueString                    # 0-1 String
    ##
    # Actual result
    # The information determined as a result of making the observation, if the information has a simple value.
    # An observation may have; 1)  a single value here, 2)  both a value and a set of related or component values,  or 3)  only a set of related or component values. If a value is present, the datatype for this element should be determined by Observation.code.  A CodeableConcept with just a text would be used instead of a string if the field was usually coded, or if the type associated with the Observation.code defines a coded value.  For additional guidance, see the [Notes section](observation.html#notes) below.
    attr_accessor :valueTime                      # 0-1 Time
    ##
    # Why the result is missing
    # Provides a reason why the expected value in the element Observation.value[x] is missing.
    # Null or exceptional values can be represented two ways in FHIR Observations.  One way is to simply include them in the value set and represent the exceptions in the value.  For example, measurement values for a serology test could be  "detected", "not detected", "inconclusive", or  "specimen unsatisfactory".   
    # 
    # The alternate way is to use the value element for actual observations and use the explicit dataAbsentReason element to record exceptional values.  For example, the dataAbsentReason code "error" could be used when the measurement was not completed. Note that an observation may only be reported if there are values to report. For example differential cell counts values may be reported only when > 0.  Because of these options, use-case agreements are required to interpret general observations for null or exceptional values.
    attr_accessor :dataAbsentReason               # 0-1 CodeableConcept
    ##
    # High, low, normal, etc.
    # A categorical assessment of an observation value.  For example, high, low, normal.
    # Historically used for laboratory results (known as 'abnormal flag' ),  its use extends to other use cases where coded interpretations  are relevant.  Often reported as one or more simple compact codes this element is often placed adjacent to the result value in reports and flow sheets to signal the meaning/normalcy status of the result.
    attr_accessor :interpretation                 # 0-* [ CodeableConcept ]
    ##
    # Comments about the observation or the results.
    # May include general statements about the observation, or statements about significant, unexpected or unreliable results values, or information about its source when relevant to its interpretation.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Observed body part
    # Indicates the site on the subject's body where the observation was made (i.e. the target site).
    # Only used if not implicit in code found in Observation.code.  In many systems, this may be represented as a related observation instead of an inline component.   
    # 
    # If the use case requires BodySite to be handled as a separate resource (e.g. to identify and track separately) then use the standard extension[ bodySite](extension-bodysite.html).
    attr_accessor :bodySite                       # 0-1 CodeableConcept
    ##
    # How it was done
    # Indicates the mechanism used to perform the observation.
    # Only used if not implicit in code for Observation.code.
    attr_accessor :local_method                   # 0-1 CodeableConcept
    ##
    # Specimen used for this observation
    # The specimen that was used when this observation was made.
    # Should only be used if not implicit in code found in `Observation.code`.  Observations are not made on specimens themselves; they are made on a subject, but in many cases by the means of a specimen. Note that although specimens are often involved, they are not always tracked and reported explicitly. Also note that observation resources may be used in contexts that track the specimen explicitly (e.g. Diagnostic Report).
    attr_accessor :specimen                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Specimen)
    ##
    # (Measurement) Device
    # The device used to generate the observation data.
    # Note that this is not meant to represent a device involved in the transmission of the result, e.g., a gateway.  Such devices may be documented using the Provenance resource where relevant.
    attr_accessor :device                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/DeviceMetric)
    ##
    # Provides guide for interpretation
    # Guidance on how to interpret the value by comparison to a normal or recommended range.  Multiple reference ranges are interpreted as an "OR".   In other words, to represent two distinct target populations, two `referenceRange` elements would be used.
    # Most observations only have one generic reference range. Systems MAY choose to restrict to only supplying the relevant reference range based on knowledge about the patient (e.g., specific to the patient's age, gender, weight and other factors), but this might not be possible or appropriate. Whenever more than one reference range is supplied, the differences between them SHOULD be provided in the reference range and/or age properties.
    attr_accessor :referenceRange                 # 0-* [ Observation::ReferenceRange ]
    ##
    # Related resource that belongs to the Observation group
    # This observation is a group observation (e.g. a battery, a panel of tests, a set of vital sign measurements) that includes the target as a member of the group.
    # When using this element, an observation will typically have either a value or a set of related resources, although both may be present in some cases.  For a discussion on the ways Observations can assembled in groups together, see [Notes](observation.html#obsgrouping) below.  Note that a system may calculate results from [QuestionnaireResponse](questionnaireresponse.html)  into a final score and represent the score as an Observation.
    attr_accessor :hasMember                      # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse|http://hl7.org/fhir/StructureDefinition/MolecularSequence) ]
    ##
    # Related measurements the observation is made from
    # The target resource that represents a measurement from which this observation value is derived. For example, a calculated anion gap or a fetal measurement based on an ultrasound image.
    # All the reference choices that are listed in this element can represent clinical observations and other measurements that may be the source for a derived value.  The most common reference will be another Observation.  For a discussion on the ways Observations can assembled in groups together, see [Notes](observation.html#obsgrouping) below.
    attr_accessor :derivedFrom                    # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference|http://hl7.org/fhir/StructureDefinition/ImagingStudy|http://hl7.org/fhir/StructureDefinition/Media|http://hl7.org/fhir/StructureDefinition/QuestionnaireResponse|http://hl7.org/fhir/StructureDefinition/Observation|http://hl7.org/fhir/StructureDefinition/MolecularSequence) ]
    ##
    # Component results
    # Some observations have multiple component observations.  These component observations are expressed as separate code value pairs that share the same attributes.  Examples include systolic and diastolic component observations for blood pressure measurement and multiple component observations for genetics observations.
    # For a discussion on the ways Observations can be assembled in groups together see [Notes](observation.html#notes) below.
    attr_accessor :component                      # 0-* [ Observation::Component ]

    def resourceType
      'Observation'
    end
  end
end
