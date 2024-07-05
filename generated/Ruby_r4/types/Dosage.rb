module FHIR

  ##
  # Base StructureDefinition for Dosage Type: Indicates how the medication is/was taken or should be taken by the patient.
  class Dosage < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    MULTIPLE_TYPES = {
      'asNeeded[x]' => ['boolean', 'CodeableConcept']
    }
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'string',
        'path'=>'Dosage.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Dosage.extension',
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
        'path'=>'Dosage.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The order of the dosage instructions
      # Indicates the order in which the dosage instructions should be applied or interpreted.
      'sequence' => {
        'type'=>'integer',
        'path'=>'Dosage.sequence',
        'min'=>0,
        'max'=>1
      },
      ##
      # Free text dosage instructions e.g. SIG.
      'text' => {
        'type'=>'string',
        'path'=>'Dosage.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Supplemental instruction or warnings to the patient - e.g. "with meals", "may cause drowsiness"
      # Supplemental instructions to the patient on how to take the medication  (e.g. "with meals" or"take half to one hour before food") or warnings for the patient about the medication (e.g. "may cause drowsiness" or "avoid exposure of skin to direct sunlight or sunlamps").
      # Information about administration or preparation of the medication (e.g. "infuse as rapidly as possibly via intraperitoneal port" or "immediately following drug x") should be populated in dosage.text.
      'additionalInstruction' => {
        'type'=>'CodeableConcept',
        'path'=>'Dosage.additionalInstruction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Patient or consumer oriented instructions
      # Instructions in terms that are understood by the patient or consumer.
      'patientInstruction' => {
        'type'=>'string',
        'path'=>'Dosage.patientInstruction',
        'min'=>0,
        'max'=>1
      },
      ##
      # When medication should be administered.
      # This attribute might not always be populated while the Dosage.text is expected to be populated.  If both are populated, then the Dosage.text should reflect the content of the Dosage.timing.
      'timing' => {
        'type'=>'Timing',
        'path'=>'Dosage.timing',
        'min'=>0,
        'max'=>1
      },
      ##
      # Take "as needed" (for x)
      # Indicates whether the Medication is only taken when needed within a specific dosing schedule (Boolean option), or it indicates the precondition for taking the Medication (CodeableConcept).
      # Can express "as needed" without a reason by setting the Boolean = True.  In this case the CodeableConcept is not populated.  Or you can express "as needed" with a reason by including the CodeableConcept.  In this case the Boolean is assumed to be True.  If you set the Boolean to False, then the dose is given according to the schedule and is not "prn" or "as needed".
      'asNeededBoolean' => {
        'type'=>'Boolean',
        'path'=>'Dosage.asNeeded[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Take "as needed" (for x)
      # Indicates whether the Medication is only taken when needed within a specific dosing schedule (Boolean option), or it indicates the precondition for taking the Medication (CodeableConcept).
      # Can express "as needed" without a reason by setting the Boolean = True.  In this case the CodeableConcept is not populated.  Or you can express "as needed" with a reason by including the CodeableConcept.  In this case the Boolean is assumed to be True.  If you set the Boolean to False, then the dose is given according to the schedule and is not "prn" or "as needed".
      'asNeededCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'Dosage.asNeeded[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Body site to administer to.
      # If the use case requires attributes from the BodySite resource (e.g. to identify and track separately) then use the standard extension [bodySite](extension-bodysite.html).  May be a summary code, or a reference to a very precise definition of the location, or both.
      'site' => {
        'type'=>'CodeableConcept',
        'path'=>'Dosage.site',
        'min'=>0,
        'max'=>1
      },
      ##
      # How drug should enter body.
      'route' => {
        'type'=>'CodeableConcept',
        'path'=>'Dosage.route',
        'min'=>0,
        'max'=>1
      },
      ##
      # Technique for administering medication.
      # Terminologies used often pre-coordinate this term with the route and or form of administration.
      'method' => {
        'local_name'=>'local_method'
        'type'=>'CodeableConcept',
        'path'=>'Dosage.method',
        'min'=>0,
        'max'=>1
      },
      ##
      # Amount of medication administered
      # The amount of medication administered.
      'doseAndRate' => {
        'type'=>'Dosage::DoseAndRate',
        'path'=>'Dosage.doseAndRate',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Upper limit on medication per unit of time.
      # This is intended for use as an adjunct to the dosage when there is an upper cap.  For example "2 tablets every 4 hours to a maximum of 8/day".
      'maxDosePerPeriod' => {
        'type'=>'Ratio',
        'path'=>'Dosage.maxDosePerPeriod',
        'min'=>0,
        'max'=>1
      },
      ##
      # Upper limit on medication per administration.
      # This is intended for use as an adjunct to the dosage when there is an upper cap.  For example, a body surface area related dose with a maximum amount, such as 1.5 mg/m2 (maximum 2 mg) IV over 5 – 10 minutes would have doseQuantity of 1.5 mg/m2 and maxDosePerAdministration of 2 mg.
      'maxDosePerAdministration' => {
        'type'=>'Quantity',
        'path'=>'Dosage.maxDosePerAdministration',
        'min'=>0,
        'max'=>1
      },
      ##
      # Upper limit on medication per lifetime of the patient.
      'maxDosePerLifetime' => {
        'type'=>'Quantity',
        'path'=>'Dosage.maxDosePerLifetime',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Amount of medication administered
    # The amount of medication administered.
    class DoseAndRate < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'dose[x]' => ['Quantity', 'Range'],
        'rate[x]' => ['Quantity', 'Range', 'Ratio']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'DoseAndRate.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'DoseAndRate.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The kind of dose or rate specified, for example, ordered or calculated.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/dose-rate-type'=>[ 'calculated', 'ordered' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'DoseAndRate.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/dose-rate-type'}
        },
        ##
        # Amount of medication per dose.
        # Note that this specifies the quantity of the specified medication, not the quantity for each active ingredient(s). Each ingredient amount can be communicated in the Medication resource. For example, if one wants to communicate that a tablet was 375 mg, where the dose was one tablet, you can use the Medication resource to document that the tablet was comprised of 375 mg of drug XYZ. Alternatively if the dose was 375 mg, then you may only need to use the Medication resource to indicate this was a tablet. If the example were an IV such as dopamine and you wanted to communicate that 400mg of dopamine was mixed in 500 ml of some IV solution, then this would all be communicated in the Medication resource. If the administration is not intended to be instantaneous (rate is present or timing has a duration), this can be specified to convey the total amount to be administered over the period of time as indicated by the schedule e.g. 500 ml in dose, with timing used to convey that this should be done over 4 hours.
        'doseQuantity' => {
          'type'=>'Quantity',
          'path'=>'DoseAndRate.dose[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Amount of medication per dose.
        # Note that this specifies the quantity of the specified medication, not the quantity for each active ingredient(s). Each ingredient amount can be communicated in the Medication resource. For example, if one wants to communicate that a tablet was 375 mg, where the dose was one tablet, you can use the Medication resource to document that the tablet was comprised of 375 mg of drug XYZ. Alternatively if the dose was 375 mg, then you may only need to use the Medication resource to indicate this was a tablet. If the example were an IV such as dopamine and you wanted to communicate that 400mg of dopamine was mixed in 500 ml of some IV solution, then this would all be communicated in the Medication resource. If the administration is not intended to be instantaneous (rate is present or timing has a duration), this can be specified to convey the total amount to be administered over the period of time as indicated by the schedule e.g. 500 ml in dose, with timing used to convey that this should be done over 4 hours.
        'doseRange' => {
          'type'=>'Range',
          'path'=>'DoseAndRate.dose[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Amount of medication per unit of time.
        # It is possible to supply both a rate and a doseQuantity to provide full details about how the medication is to be administered and supplied. If the rate is intended to change over time, depending on local rules/regulations, each change should be captured as a new version of the MedicationRequest with an updated rate, or captured with a new MedicationRequest with the new rate.It is possible to specify a rate over time (for example, 100 ml/hour) using either the rateRatio and rateQuantity.  The rateQuantity approach requires systems to have the capability to parse UCUM grammer where ml/hour is included rather than a specific ratio where the time is specified as the denominator.  Where a rate such as 500ml over 2 hours is specified, the use of rateRatio may be more semantically correct than specifying using a rateQuantity of 250 mg/hour.
        'rateQuantity' => {
          'type'=>'Quantity',
          'path'=>'DoseAndRate.rate[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Amount of medication per unit of time.
        # It is possible to supply both a rate and a doseQuantity to provide full details about how the medication is to be administered and supplied. If the rate is intended to change over time, depending on local rules/regulations, each change should be captured as a new version of the MedicationRequest with an updated rate, or captured with a new MedicationRequest with the new rate.It is possible to specify a rate over time (for example, 100 ml/hour) using either the rateRatio and rateQuantity.  The rateQuantity approach requires systems to have the capability to parse UCUM grammer where ml/hour is included rather than a specific ratio where the time is specified as the denominator.  Where a rate such as 500ml over 2 hours is specified, the use of rateRatio may be more semantically correct than specifying using a rateQuantity of 250 mg/hour.
        'rateRange' => {
          'type'=>'Range',
          'path'=>'DoseAndRate.rate[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Amount of medication per unit of time.
        # It is possible to supply both a rate and a doseQuantity to provide full details about how the medication is to be administered and supplied. If the rate is intended to change over time, depending on local rules/regulations, each change should be captured as a new version of the MedicationRequest with an updated rate, or captured with a new MedicationRequest with the new rate.It is possible to specify a rate over time (for example, 100 ml/hour) using either the rateRatio and rateQuantity.  The rateQuantity approach requires systems to have the capability to parse UCUM grammer where ml/hour is included rather than a specific ratio where the time is specified as the denominator.  Where a rate such as 500ml over 2 hours is specified, the use of rateRatio may be more semantically correct than specifying using a rateQuantity of 250 mg/hour.
        'rateRatio' => {
          'type'=>'Ratio',
          'path'=>'DoseAndRate.rate[x]',
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
      # The kind of dose or rate specified, for example, ordered or calculated.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Amount of medication per dose.
      # Note that this specifies the quantity of the specified medication, not the quantity for each active ingredient(s). Each ingredient amount can be communicated in the Medication resource. For example, if one wants to communicate that a tablet was 375 mg, where the dose was one tablet, you can use the Medication resource to document that the tablet was comprised of 375 mg of drug XYZ. Alternatively if the dose was 375 mg, then you may only need to use the Medication resource to indicate this was a tablet. If the example were an IV such as dopamine and you wanted to communicate that 400mg of dopamine was mixed in 500 ml of some IV solution, then this would all be communicated in the Medication resource. If the administration is not intended to be instantaneous (rate is present or timing has a duration), this can be specified to convey the total amount to be administered over the period of time as indicated by the schedule e.g. 500 ml in dose, with timing used to convey that this should be done over 4 hours.
      attr_accessor :doseQuantity                   # 0-1 Quantity
      ##
      # Amount of medication per dose.
      # Note that this specifies the quantity of the specified medication, not the quantity for each active ingredient(s). Each ingredient amount can be communicated in the Medication resource. For example, if one wants to communicate that a tablet was 375 mg, where the dose was one tablet, you can use the Medication resource to document that the tablet was comprised of 375 mg of drug XYZ. Alternatively if the dose was 375 mg, then you may only need to use the Medication resource to indicate this was a tablet. If the example were an IV such as dopamine and you wanted to communicate that 400mg of dopamine was mixed in 500 ml of some IV solution, then this would all be communicated in the Medication resource. If the administration is not intended to be instantaneous (rate is present or timing has a duration), this can be specified to convey the total amount to be administered over the period of time as indicated by the schedule e.g. 500 ml in dose, with timing used to convey that this should be done over 4 hours.
      attr_accessor :doseRange                      # 0-1 Range
      ##
      # Amount of medication per unit of time.
      # It is possible to supply both a rate and a doseQuantity to provide full details about how the medication is to be administered and supplied. If the rate is intended to change over time, depending on local rules/regulations, each change should be captured as a new version of the MedicationRequest with an updated rate, or captured with a new MedicationRequest with the new rate.It is possible to specify a rate over time (for example, 100 ml/hour) using either the rateRatio and rateQuantity.  The rateQuantity approach requires systems to have the capability to parse UCUM grammer where ml/hour is included rather than a specific ratio where the time is specified as the denominator.  Where a rate such as 500ml over 2 hours is specified, the use of rateRatio may be more semantically correct than specifying using a rateQuantity of 250 mg/hour.
      attr_accessor :rateQuantity                   # 0-1 Quantity
      ##
      # Amount of medication per unit of time.
      # It is possible to supply both a rate and a doseQuantity to provide full details about how the medication is to be administered and supplied. If the rate is intended to change over time, depending on local rules/regulations, each change should be captured as a new version of the MedicationRequest with an updated rate, or captured with a new MedicationRequest with the new rate.It is possible to specify a rate over time (for example, 100 ml/hour) using either the rateRatio and rateQuantity.  The rateQuantity approach requires systems to have the capability to parse UCUM grammer where ml/hour is included rather than a specific ratio where the time is specified as the denominator.  Where a rate such as 500ml over 2 hours is specified, the use of rateRatio may be more semantically correct than specifying using a rateQuantity of 250 mg/hour.
      attr_accessor :rateRange                      # 0-1 Range
      ##
      # Amount of medication per unit of time.
      # It is possible to supply both a rate and a doseQuantity to provide full details about how the medication is to be administered and supplied. If the rate is intended to change over time, depending on local rules/regulations, each change should be captured as a new version of the MedicationRequest with an updated rate, or captured with a new MedicationRequest with the new rate.It is possible to specify a rate over time (for example, 100 ml/hour) using either the rateRatio and rateQuantity.  The rateQuantity approach requires systems to have the capability to parse UCUM grammer where ml/hour is included rather than a specific ratio where the time is specified as the denominator.  Where a rate such as 500ml over 2 hours is specified, the use of rateRatio may be more semantically correct than specifying using a rateQuantity of 250 mg/hour.
      attr_accessor :rateRatio                      # 0-1 Ratio
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
    # The order of the dosage instructions
    # Indicates the order in which the dosage instructions should be applied or interpreted.
    attr_accessor :sequence                       # 0-1 integer
    ##
    # Free text dosage instructions e.g. SIG.
    attr_accessor :text                           # 0-1 string
    ##
    # Supplemental instruction or warnings to the patient - e.g. "with meals", "may cause drowsiness"
    # Supplemental instructions to the patient on how to take the medication  (e.g. "with meals" or"take half to one hour before food") or warnings for the patient about the medication (e.g. "may cause drowsiness" or "avoid exposure of skin to direct sunlight or sunlamps").
    # Information about administration or preparation of the medication (e.g. "infuse as rapidly as possibly via intraperitoneal port" or "immediately following drug x") should be populated in dosage.text.
    attr_accessor :additionalInstruction          # 0-* [ CodeableConcept ]
    ##
    # Patient or consumer oriented instructions
    # Instructions in terms that are understood by the patient or consumer.
    attr_accessor :patientInstruction             # 0-1 string
    ##
    # When medication should be administered.
    # This attribute might not always be populated while the Dosage.text is expected to be populated.  If both are populated, then the Dosage.text should reflect the content of the Dosage.timing.
    attr_accessor :timing                         # 0-1 Timing
    ##
    # Take "as needed" (for x)
    # Indicates whether the Medication is only taken when needed within a specific dosing schedule (Boolean option), or it indicates the precondition for taking the Medication (CodeableConcept).
    # Can express "as needed" without a reason by setting the Boolean = True.  In this case the CodeableConcept is not populated.  Or you can express "as needed" with a reason by including the CodeableConcept.  In this case the Boolean is assumed to be True.  If you set the Boolean to False, then the dose is given according to the schedule and is not "prn" or "as needed".
    attr_accessor :asNeededBoolean                # 0-1 Boolean
    ##
    # Take "as needed" (for x)
    # Indicates whether the Medication is only taken when needed within a specific dosing schedule (Boolean option), or it indicates the precondition for taking the Medication (CodeableConcept).
    # Can express "as needed" without a reason by setting the Boolean = True.  In this case the CodeableConcept is not populated.  Or you can express "as needed" with a reason by including the CodeableConcept.  In this case the Boolean is assumed to be True.  If you set the Boolean to False, then the dose is given according to the schedule and is not "prn" or "as needed".
    attr_accessor :asNeededCodeableConcept        # 0-1 CodeableConcept
    ##
    # Body site to administer to.
    # If the use case requires attributes from the BodySite resource (e.g. to identify and track separately) then use the standard extension [bodySite](extension-bodysite.html).  May be a summary code, or a reference to a very precise definition of the location, or both.
    attr_accessor :site                           # 0-1 CodeableConcept
    ##
    # How drug should enter body.
    attr_accessor :route                          # 0-1 CodeableConcept
    ##
    # Technique for administering medication.
    # Terminologies used often pre-coordinate this term with the route and or form of administration.
    attr_accessor :local_method                   # 0-1 CodeableConcept
    ##
    # Amount of medication administered
    # The amount of medication administered.
    attr_accessor :doseAndRate                    # 0-* [ Dosage::DoseAndRate ]
    ##
    # Upper limit on medication per unit of time.
    # This is intended for use as an adjunct to the dosage when there is an upper cap.  For example "2 tablets every 4 hours to a maximum of 8/day".
    attr_accessor :maxDosePerPeriod               # 0-1 Ratio
    ##
    # Upper limit on medication per administration.
    # This is intended for use as an adjunct to the dosage when there is an upper cap.  For example, a body surface area related dose with a maximum amount, such as 1.5 mg/m2 (maximum 2 mg) IV over 5 – 10 minutes would have doseQuantity of 1.5 mg/m2 and maxDosePerAdministration of 2 mg.
    attr_accessor :maxDosePerAdministration       # 0-1 Quantity
    ##
    # Upper limit on medication per lifetime of the patient.
    attr_accessor :maxDosePerLifetime             # 0-1 Quantity
  end
end
