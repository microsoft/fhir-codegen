module FHIR

  ##
  # Information about a medication that is used to support knowledge.
  class MedicationKnowledge < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['classification-type', 'classification', 'code', 'doseform', 'ingredient-code', 'ingredient', 'manufacturer', 'monitoring-program-name', 'monitoring-program-type', 'monograph-type', 'monograph', 'source-cost', 'status']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'MedicationKnowledge.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'MedicationKnowledge.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'MedicationKnowledge.implicitRules',
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
        'path'=>'MedicationKnowledge.language',
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
        'path'=>'MedicationKnowledge.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'MedicationKnowledge.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'MedicationKnowledge.extension',
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
        'path'=>'MedicationKnowledge.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Code that identifies this medication
      # A code that specifies this medication, or a textual description if no code is available. Usage note: This could be a standard medication code such as a code from RxNorm, SNOMED CT, IDMP etc. It could also be a national or local formulary code, optionally with translations to other code systems.
      # Depending on the context of use, the code that was actually selected by the user (prescriber, dispenser, etc.) will have the coding.userSelected set to true.  As described in the coding datatype: "A coding may be marked as a "userSelected" if a user selected the particular coded value in a user interface (e.g. the user selects an item in a pick-list). If a user selected coding exists, it is the preferred choice for performing translations etc. Other codes can only be literal translations to alternative code systems, or codes at a lower level of granularity (e.g. a generic code for a vendor-specific primary one).
      'code' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationKnowledge.code',
        'min'=>0,
        'max'=>1
      },
      ##
      # active | inactive | entered-in-error
      # A code to indicate if the medication is in active use.  The status refers to the validity about the information of the medication and not to its medicinal properties.
      # This status is intended to identify if the medication in a local system is in active use within a drug database or inventory.  For example, a pharmacy system may create a new drug file record for a compounded product "ABC Hospital Special Cream" with an active status.  At some point in the future, it may be determined that the drug record was created with an error and the status is changed to "entered in error".   This status is not intended to specify if a medication is part of a particular formulary.  It is possible that the drug record may be referenced by multiple formularies or catalogues and each of those entries would have a separate status.
      'status' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/medicationknowledge-status'=>[ 'active', 'inactive', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'MedicationKnowledge.status',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationknowledge-status'}
      },
      ##
      # Manufacturer of the item
      # Describes the details of the manufacturer of the medication product.  This is not intended to represent the distributor of a medication product.
      'manufacturer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'MedicationKnowledge.manufacturer',
        'min'=>0,
        'max'=>1
      },
      ##
      # powder | tablets | capsule +
      # Describes the form of the item.  Powder; tablets; capsule.
      # When Medication is referenced from MedicationRequest, this is the ordered form.  When Medication is referenced within MedicationDispense, this is the dispensed form.  When Medication is referenced within MedicationAdministration, this is administered form.
      'doseForm' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationKnowledge.doseForm',
        'min'=>0,
        'max'=>1
      },
      ##
      # Amount of drug in package
      # Specific amount of the drug in the packaged product.  For example, when specifying a product that has the same strength (For example, Insulin glargine 100 unit per mL solution for injection), this attribute provides additional clarification of the package amount (For example, 3 mL, 10mL, etc.).
      # This is the quantity of medication in a package.  To specify the strength of the medication, the Ingredient.strength attribute is used.
      'amount' => {
        'type'=>'Quantity',
        'path'=>'MedicationKnowledge.amount',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional names for a medication, for example, the name(s) given to a medication in different countries.  For example, acetaminophen and paracetamol or salbutamol and albuterol.
      'synonym' => {
        'type'=>'string',
        'path'=>'MedicationKnowledge.synonym',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Associated or related medication information
      # Associated or related knowledge about a medication.
      'relatedMedicationKnowledge' => {
        'type'=>'MedicationKnowledge::RelatedMedicationKnowledge',
        'path'=>'MedicationKnowledge.relatedMedicationKnowledge',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A medication resource that is associated with this medication
      # Associated or related medications.  For example, if the medication is a branded product (e.g. Crestor), this is the Therapeutic Moeity (e.g. Rosuvastatin) or if this is a generic medication (e.g. Rosuvastatin), this would link to a branded product (e.g. Crestor).
      'associatedMedication' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Medication'],
        'type'=>'Reference',
        'path'=>'MedicationKnowledge.associatedMedication',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Category of the medication or product (e.g. branded product, therapeutic moeity, generic product, innovator product, etc.).
      'productType' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationKnowledge.productType',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Associated documentation about the medication.
      'monograph' => {
        'type'=>'MedicationKnowledge::Monograph',
        'path'=>'MedicationKnowledge.monograph',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Active or inactive ingredient
      # Identifies a particular constituent of interest in the product.
      'ingredient' => {
        'type'=>'MedicationKnowledge::Ingredient',
        'path'=>'MedicationKnowledge.ingredient',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The instructions for preparing the medication.
      'preparationInstruction' => {
        'type'=>'markdown',
        'path'=>'MedicationKnowledge.preparationInstruction',
        'min'=>0,
        'max'=>1
      },
      ##
      # The intended or approved route of administration.
      'intendedRoute' => {
        'type'=>'CodeableConcept',
        'path'=>'MedicationKnowledge.intendedRoute',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The pricing of the medication
      # The price of the medication.
      'cost' => {
        'type'=>'MedicationKnowledge::Cost',
        'path'=>'MedicationKnowledge.cost',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Program under which a medication is reviewed
      # The program under which the medication is reviewed.
      'monitoringProgram' => {
        'type'=>'MedicationKnowledge::MonitoringProgram',
        'path'=>'MedicationKnowledge.monitoringProgram',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Guidelines for administration of the medication
      # Guidelines for the administration of the medication.
      'administrationGuidelines' => {
        'type'=>'MedicationKnowledge::AdministrationGuidelines',
        'path'=>'MedicationKnowledge.administrationGuidelines',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Categorization of the medication within a formulary or classification system.
      'medicineClassification' => {
        'type'=>'MedicationKnowledge::MedicineClassification',
        'path'=>'MedicationKnowledge.medicineClassification',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Details about packaged medications
      # Information that only applies to packages (not products).
      'packaging' => {
        'type'=>'MedicationKnowledge::Packaging',
        'path'=>'MedicationKnowledge.packaging',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specifies descriptive properties of the medicine, such as color, shape, imprints, etc.
      'drugCharacteristic' => {
        'type'=>'MedicationKnowledge::DrugCharacteristic',
        'path'=>'MedicationKnowledge.drugCharacteristic',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Potential clinical issue with or between medication(s) (for example, drug-drug interaction, drug-disease contraindication, drug-allergy interaction, etc.).
      'contraindication' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DetectedIssue'],
        'type'=>'Reference',
        'path'=>'MedicationKnowledge.contraindication',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Regulatory information about a medication.
      'regulatory' => {
        'type'=>'MedicationKnowledge::Regulatory',
        'path'=>'MedicationKnowledge.regulatory',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The time course of drug absorption, distribution, metabolism and excretion of a medication from the body.
      'kinetics' => {
        'type'=>'MedicationKnowledge::Kinetics',
        'path'=>'MedicationKnowledge.kinetics',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Associated or related medication information
    # Associated or related knowledge about a medication.
    class RelatedMedicationKnowledge < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'RelatedMedicationKnowledge.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'RelatedMedicationKnowledge.extension',
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
          'path'=>'RelatedMedicationKnowledge.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Category of medicationKnowledge
        # The category of the associated medication knowledge reference.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'RelatedMedicationKnowledge.type',
          'min'=>1,
          'max'=>1
        },
        ##
        # Associated documentation about the associated medication knowledge.
        'reference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MedicationKnowledge'],
          'type'=>'Reference',
          'path'=>'RelatedMedicationKnowledge.reference',
          'min'=>1,
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
      # Category of medicationKnowledge
      # The category of the associated medication knowledge reference.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Associated documentation about the associated medication knowledge.
      attr_accessor :reference                      # 1-* [ Reference(http://hl7.org/fhir/StructureDefinition/MedicationKnowledge) ]
    end

    ##
    # Associated documentation about the medication.
    class Monograph < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Monograph.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Monograph.extension',
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
          'path'=>'Monograph.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The category of medication document
        # The category of documentation about the medication. (e.g. professional monograph, patient education monograph).
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Monograph.type',
          'min'=>0,
          'max'=>1
        },
        ##
        # Associated documentation about the medication.
        'source' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference', 'http://hl7.org/fhir/StructureDefinition/Media'],
          'type'=>'Reference',
          'path'=>'Monograph.source',
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
      # The category of medication document
      # The category of documentation about the medication. (e.g. professional monograph, patient education monograph).
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Associated documentation about the medication.
      attr_accessor :source                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference|http://hl7.org/fhir/StructureDefinition/Media)
    end

    ##
    # Active or inactive ingredient
    # Identifies a particular constituent of interest in the product.
    class Ingredient < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'item[x]' => ['CodeableConcept', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Ingredient.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Ingredient.extension',
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
          'path'=>'Ingredient.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Medication(s) or substance(s) contained in the medication
        # The actual ingredient - either a substance (simple ingredient) or another medication.
        'itemCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Ingredient.item[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Medication(s) or substance(s) contained in the medication
        # The actual ingredient - either a substance (simple ingredient) or another medication.
        'itemReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Substance'],
          'type'=>'Reference',
          'path'=>'Ingredient.item[x]',
          'min'=>1,
          'max'=>1
        },
        ##
        # Active ingredient indicator
        # Indication of whether this ingredient affects the therapeutic action of the drug.
        'isActive' => {
          'type'=>'boolean',
          'path'=>'Ingredient.isActive',
          'min'=>0,
          'max'=>1
        },
        ##
        # Quantity of ingredient present
        # Specifies how many (or how much) of the items there are in this Medication.  For example, 250 mg per tablet.  This is expressed as a ratio where the numerator is 250mg and the denominator is 1 tablet.
        'strength' => {
          'type'=>'Ratio',
          'path'=>'Ingredient.strength',
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
      # Medication(s) or substance(s) contained in the medication
      # The actual ingredient - either a substance (simple ingredient) or another medication.
      attr_accessor :itemCodeableConcept            # 1-1 CodeableConcept
      ##
      # Medication(s) or substance(s) contained in the medication
      # The actual ingredient - either a substance (simple ingredient) or another medication.
      attr_accessor :itemReference                  # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Substance)
      ##
      # Active ingredient indicator
      # Indication of whether this ingredient affects the therapeutic action of the drug.
      attr_accessor :isActive                       # 0-1 boolean
      ##
      # Quantity of ingredient present
      # Specifies how many (or how much) of the items there are in this Medication.  For example, 250 mg per tablet.  This is expressed as a ratio where the numerator is 250mg and the denominator is 1 tablet.
      attr_accessor :strength                       # 0-1 Ratio
    end

    ##
    # The pricing of the medication
    # The price of the medication.
    class Cost < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Cost.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Cost.extension',
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
          'path'=>'Cost.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The category of the cost information.  For example, manufacturers' cost, patient cost, claim reimbursement cost, actual acquisition cost.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Cost.type',
          'min'=>1,
          'max'=>1
        },
        ##
        # The source or owner for the price information
        # The source or owner that assigns the price to the medication.
        'source' => {
          'type'=>'string',
          'path'=>'Cost.source',
          'min'=>0,
          'max'=>1
        },
        ##
        # The price of the medication.
        'cost' => {
          'type'=>'Money',
          'path'=>'Cost.cost',
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
      # The category of the cost information.  For example, manufacturers' cost, patient cost, claim reimbursement cost, actual acquisition cost.
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # The source or owner for the price information
      # The source or owner that assigns the price to the medication.
      attr_accessor :source                         # 0-1 string
      ##
      # The price of the medication.
      attr_accessor :cost                           # 1-1 Money
    end

    ##
    # Program under which a medication is reviewed
    # The program under which the medication is reviewed.
    class MonitoringProgram < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'MonitoringProgram.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'MonitoringProgram.extension',
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
          'path'=>'MonitoringProgram.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of program under which the medication is monitored.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'MonitoringProgram.type',
          'min'=>0,
          'max'=>1
        },
        ##
        # Name of the reviewing program.
        'name' => {
          'type'=>'string',
          'path'=>'MonitoringProgram.name',
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
      # Type of program under which the medication is monitored.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Name of the reviewing program.
      attr_accessor :name                           # 0-1 string
    end

    ##
    # Guidelines for administration of the medication
    # Guidelines for the administration of the medication.
    class AdministrationGuidelines < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'indication[x]' => ['CodeableConcept', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'AdministrationGuidelines.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'AdministrationGuidelines.extension',
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
          'path'=>'AdministrationGuidelines.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Dosage for the medication for the specific guidelines.
        'dosage' => {
          'type'=>'MedicationKnowledge::AdministrationGuidelines::Dosage',
          'path'=>'AdministrationGuidelines.dosage',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Indication for use that apply to the specific administration guidelines.
        'indicationCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'AdministrationGuidelines.indication[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Indication for use that apply to the specific administration guidelines.
        'indicationReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ObservationDefinition'],
          'type'=>'Reference',
          'path'=>'AdministrationGuidelines.indication[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Characteristics of the patient that are relevant to the administration guidelines (for example, height, weight, gender, etc.).
        'patientCharacteristics' => {
          'type'=>'MedicationKnowledge::AdministrationGuidelines::PatientCharacteristics',
          'path'=>'AdministrationGuidelines.patientCharacteristics',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Dosage for the medication for the specific guidelines.
      class Dosage < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

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
          # Type of dosage
          # The type of dosage (for example, prophylaxis, maintenance, therapeutic, etc.).
          'type' => {
            'type'=>'CodeableConcept',
            'path'=>'Dosage.type',
            'min'=>1,
            'max'=>1
          },
          ##
          # Dosage for the medication for the specific guidelines.
          'dosage' => {
            'type'=>'Dosage',
            'path'=>'Dosage.dosage',
            'min'=>1,
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
        # Type of dosage
        # The type of dosage (for example, prophylaxis, maintenance, therapeutic, etc.).
        attr_accessor :type                           # 1-1 CodeableConcept
        ##
        # Dosage for the medication for the specific guidelines.
        attr_accessor :dosage                         # 1-* [ Dosage ]
      end

      ##
      # Characteristics of the patient that are relevant to the administration guidelines (for example, height, weight, gender, etc.).
      class PatientCharacteristics < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'characteristic[x]' => ['CodeableConcept', 'Quantity']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'PatientCharacteristics.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'PatientCharacteristics.extension',
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
            'path'=>'PatientCharacteristics.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Specific characteristic that is relevant to the administration guideline (e.g. height, weight, gender).
          'characteristicCodeableConcept' => {
            'type'=>'CodeableConcept',
            'path'=>'PatientCharacteristics.characteristic[x]',
            'min'=>1,
            'max'=>1
          },
          ##
          # Specific characteristic that is relevant to the administration guideline (e.g. height, weight, gender).
          'characteristicQuantity' => {
            'type'=>'Quantity',
            'path'=>'PatientCharacteristics.characteristic[x]',
            'min'=>1,
            'max'=>1
          },
          ##
          # The specific characteristic (e.g. height, weight, gender, etc.).
          'value' => {
            'type'=>'string',
            'path'=>'PatientCharacteristics.value',
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
        # Specific characteristic that is relevant to the administration guideline (e.g. height, weight, gender).
        attr_accessor :characteristicCodeableConcept  # 1-1 CodeableConcept
        ##
        # Specific characteristic that is relevant to the administration guideline (e.g. height, weight, gender).
        attr_accessor :characteristicQuantity         # 1-1 Quantity
        ##
        # The specific characteristic (e.g. height, weight, gender, etc.).
        attr_accessor :value                          # 0-* [ string ]
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
      # Dosage for the medication for the specific guidelines.
      attr_accessor :dosage                         # 0-* [ MedicationKnowledge::AdministrationGuidelines::Dosage ]
      ##
      # Indication for use that apply to the specific administration guidelines.
      attr_accessor :indicationCodeableConcept      # 0-1 CodeableConcept
      ##
      # Indication for use that apply to the specific administration guidelines.
      attr_accessor :indicationReference            # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/ObservationDefinition)
      ##
      # Characteristics of the patient that are relevant to the administration guidelines (for example, height, weight, gender, etc.).
      attr_accessor :patientCharacteristics         # 0-* [ MedicationKnowledge::AdministrationGuidelines::PatientCharacteristics ]
    end

    ##
    # Categorization of the medication within a formulary or classification system.
    class MedicineClassification < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'MedicineClassification.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'MedicineClassification.extension',
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
          'path'=>'MedicineClassification.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The type of category for the medication (for example, therapeutic classification, therapeutic sub-classification).
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'MedicineClassification.type',
          'min'=>1,
          'max'=>1
        },
        ##
        # Specific category assigned to the medication (e.g. anti-infective, anti-hypertensive, antibiotic, etc.).
        'classification' => {
          'type'=>'CodeableConcept',
          'path'=>'MedicineClassification.classification',
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
      # The type of category for the medication (for example, therapeutic classification, therapeutic sub-classification).
      attr_accessor :type                           # 1-1 CodeableConcept
      ##
      # Specific category assigned to the medication (e.g. anti-infective, anti-hypertensive, antibiotic, etc.).
      attr_accessor :classification                 # 0-* [ CodeableConcept ]
    end

    ##
    # Details about packaged medications
    # Information that only applies to packages (not products).
    class Packaging < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Packaging.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Packaging.extension',
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
          'path'=>'Packaging.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A code that defines the specific type of packaging that the medication can be found in (e.g. blister sleeve, tube, bottle).
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/medicationknowledge-package-type'=>[ 'amp', 'bag', 'blstrpk', 'bot', 'box', 'can', 'cart', 'disk', 'doset', 'jar', 'jug', 'minim', 'nebamp', 'ovul', 'pch', 'pkt', 'sash', 'strip', 'tin', 'tub', 'tube', 'vial' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Packaging.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationknowledge-package-type'}
        },
        ##
        # The number of product units the package would contain if fully loaded.
        'quantity' => {
          'type'=>'Quantity',
          'path'=>'Packaging.quantity',
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
      # A code that defines the specific type of packaging that the medication can be found in (e.g. blister sleeve, tube, bottle).
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # The number of product units the package would contain if fully loaded.
      attr_accessor :quantity                       # 0-1 Quantity
    end

    ##
    # Specifies descriptive properties of the medicine, such as color, shape, imprints, etc.
    class DrugCharacteristic < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'value[x]' => ['base64Binary', 'CodeableConcept', 'Quantity', 'string']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'DrugCharacteristic.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'DrugCharacteristic.extension',
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
          'path'=>'DrugCharacteristic.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Code specifying the type of characteristic of medication
        # A code specifying which characteristic of the medicine is being described (for example, colour, shape, imprint).
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/medicationknowledge-characteristic'=>[ 'imprintcd', 'size', 'shape', 'color', 'coating', 'scoring', 'logo' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'DrugCharacteristic.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/medicationknowledge-characteristic'}
        },
        ##
        # Description of the characteristic.
        # The description should be provided as a CodeableConcept, SimpleQuantity or an image.  The description can be a string only when these others are not available.
        'valueBase64Binary' => {
          'type'=>'Base64Binary',
          'path'=>'DrugCharacteristic.value[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Description of the characteristic.
        # The description should be provided as a CodeableConcept, SimpleQuantity or an image.  The description can be a string only when these others are not available.
        'valueCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'DrugCharacteristic.value[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Description of the characteristic.
        # The description should be provided as a CodeableConcept, SimpleQuantity or an image.  The description can be a string only when these others are not available.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'DrugCharacteristic.value[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Description of the characteristic.
        # The description should be provided as a CodeableConcept, SimpleQuantity or an image.  The description can be a string only when these others are not available.
        'valueString' => {
          'type'=>'String',
          'path'=>'DrugCharacteristic.value[x]',
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
      # Code specifying the type of characteristic of medication
      # A code specifying which characteristic of the medicine is being described (for example, colour, shape, imprint).
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Description of the characteristic.
      # The description should be provided as a CodeableConcept, SimpleQuantity or an image.  The description can be a string only when these others are not available.
      attr_accessor :valueBase64Binary              # 0-1 Base64Binary
      ##
      # Description of the characteristic.
      # The description should be provided as a CodeableConcept, SimpleQuantity or an image.  The description can be a string only when these others are not available.
      attr_accessor :valueCodeableConcept           # 0-1 CodeableConcept
      ##
      # Description of the characteristic.
      # The description should be provided as a CodeableConcept, SimpleQuantity or an image.  The description can be a string only when these others are not available.
      attr_accessor :valueQuantity                  # 0-1 Quantity
      ##
      # Description of the characteristic.
      # The description should be provided as a CodeableConcept, SimpleQuantity or an image.  The description can be a string only when these others are not available.
      attr_accessor :valueString                    # 0-1 String
    end

    ##
    # Regulatory information about a medication.
    class Regulatory < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Regulatory.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Regulatory.extension',
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
          'path'=>'Regulatory.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Specifies the authority of the regulation
        # The authority that is specifying the regulations.
        'regulatoryAuthority' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
          'type'=>'Reference',
          'path'=>'Regulatory.regulatoryAuthority',
          'min'=>1,
          'max'=>1
        },
        ##
        # Specifies if changes are allowed when dispensing a medication from a regulatory perspective.
        'substitution' => {
          'type'=>'MedicationKnowledge::Regulatory::Substitution',
          'path'=>'Regulatory.substitution',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Specifies the schedule of a medication in jurisdiction.
        'schedule' => {
          'type'=>'MedicationKnowledge::Regulatory::Schedule',
          'path'=>'Regulatory.schedule',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The maximum number of units of the medication that can be dispensed in a period.
        'maxDispense' => {
          'type'=>'MedicationKnowledge::Regulatory::MaxDispense',
          'path'=>'Regulatory.maxDispense',
          'min'=>0,
          'max'=>1
        }
      }

      ##
      # Specifies if changes are allowed when dispensing a medication from a regulatory perspective.
      class Substitution < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Substitution.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Substitution.extension',
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
            'path'=>'Substitution.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Specifies the type of substitution allowed.
          'type' => {
            'type'=>'CodeableConcept',
            'path'=>'Substitution.type',
            'min'=>1,
            'max'=>1
          },
          ##
          # Specifies if regulation allows for changes in the medication when dispensing.
          'allowed' => {
            'type'=>'boolean',
            'path'=>'Substitution.allowed',
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
        # Specifies the type of substitution allowed.
        attr_accessor :type                           # 1-1 CodeableConcept
        ##
        # Specifies if regulation allows for changes in the medication when dispensing.
        attr_accessor :allowed                        # 1-1 boolean
      end

      ##
      # Specifies the schedule of a medication in jurisdiction.
      class Schedule < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Schedule.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Schedule.extension',
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
            'path'=>'Schedule.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Specifies the specific drug schedule.
          'schedule' => {
            'type'=>'CodeableConcept',
            'path'=>'Schedule.schedule',
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
        # Specifies the specific drug schedule.
        attr_accessor :schedule                       # 1-1 CodeableConcept
      end

      ##
      # The maximum number of units of the medication that can be dispensed in a period.
      class MaxDispense < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'MaxDispense.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'MaxDispense.extension',
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
            'path'=>'MaxDispense.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # The maximum number of units of the medication that can be dispensed.
          'quantity' => {
            'type'=>'Quantity',
            'path'=>'MaxDispense.quantity',
            'min'=>1,
            'max'=>1
          },
          ##
          # The period that applies to the maximum number of units.
          'period' => {
            'type'=>'Duration',
            'path'=>'MaxDispense.period',
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
        # The maximum number of units of the medication that can be dispensed.
        attr_accessor :quantity                       # 1-1 Quantity
        ##
        # The period that applies to the maximum number of units.
        attr_accessor :period                         # 0-1 Duration
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
      # Specifies the authority of the regulation
      # The authority that is specifying the regulations.
      attr_accessor :regulatoryAuthority            # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
      ##
      # Specifies if changes are allowed when dispensing a medication from a regulatory perspective.
      attr_accessor :substitution                   # 0-* [ MedicationKnowledge::Regulatory::Substitution ]
      ##
      # Specifies the schedule of a medication in jurisdiction.
      attr_accessor :schedule                       # 0-* [ MedicationKnowledge::Regulatory::Schedule ]
      ##
      # The maximum number of units of the medication that can be dispensed in a period.
      attr_accessor :maxDispense                    # 0-1 MedicationKnowledge::Regulatory::MaxDispense
    end

    ##
    # The time course of drug absorption, distribution, metabolism and excretion of a medication from the body.
    class Kinetics < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Kinetics.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Kinetics.extension',
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
          'path'=>'Kinetics.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The drug concentration measured at certain discrete points in time.
        'areaUnderCurve' => {
          'type'=>'Quantity',
          'path'=>'Kinetics.areaUnderCurve',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The median lethal dose of a drug.
        'lethalDose50' => {
          'type'=>'Quantity',
          'path'=>'Kinetics.lethalDose50',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Time required for concentration in the body to decrease by half
        # The time required for any specified property (e.g., the concentration of a substance in the body) to decrease by half.
        'halfLifePeriod' => {
          'type'=>'Duration',
          'path'=>'Kinetics.halfLifePeriod',
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
      # The drug concentration measured at certain discrete points in time.
      attr_accessor :areaUnderCurve                 # 0-* [ Quantity ]
      ##
      # The median lethal dose of a drug.
      attr_accessor :lethalDose50                   # 0-* [ Quantity ]
      ##
      # Time required for concentration in the body to decrease by half
      # The time required for any specified property (e.g., the concentration of a substance in the body) to decrease by half.
      attr_accessor :halfLifePeriod                 # 0-1 Duration
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
    # Code that identifies this medication
    # A code that specifies this medication, or a textual description if no code is available. Usage note: This could be a standard medication code such as a code from RxNorm, SNOMED CT, IDMP etc. It could also be a national or local formulary code, optionally with translations to other code systems.
    # Depending on the context of use, the code that was actually selected by the user (prescriber, dispenser, etc.) will have the coding.userSelected set to true.  As described in the coding datatype: "A coding may be marked as a "userSelected" if a user selected the particular coded value in a user interface (e.g. the user selects an item in a pick-list). If a user selected coding exists, it is the preferred choice for performing translations etc. Other codes can only be literal translations to alternative code systems, or codes at a lower level of granularity (e.g. a generic code for a vendor-specific primary one).
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # active | inactive | entered-in-error
    # A code to indicate if the medication is in active use.  The status refers to the validity about the information of the medication and not to its medicinal properties.
    # This status is intended to identify if the medication in a local system is in active use within a drug database or inventory.  For example, a pharmacy system may create a new drug file record for a compounded product "ABC Hospital Special Cream" with an active status.  At some point in the future, it may be determined that the drug record was created with an error and the status is changed to "entered in error".   This status is not intended to specify if a medication is part of a particular formulary.  It is possible that the drug record may be referenced by multiple formularies or catalogues and each of those entries would have a separate status.
    attr_accessor :status                         # 0-1 code
    ##
    # Manufacturer of the item
    # Describes the details of the manufacturer of the medication product.  This is not intended to represent the distributor of a medication product.
    attr_accessor :manufacturer                   # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # powder | tablets | capsule +
    # Describes the form of the item.  Powder; tablets; capsule.
    # When Medication is referenced from MedicationRequest, this is the ordered form.  When Medication is referenced within MedicationDispense, this is the dispensed form.  When Medication is referenced within MedicationAdministration, this is administered form.
    attr_accessor :doseForm                       # 0-1 CodeableConcept
    ##
    # Amount of drug in package
    # Specific amount of the drug in the packaged product.  For example, when specifying a product that has the same strength (For example, Insulin glargine 100 unit per mL solution for injection), this attribute provides additional clarification of the package amount (For example, 3 mL, 10mL, etc.).
    # This is the quantity of medication in a package.  To specify the strength of the medication, the Ingredient.strength attribute is used.
    attr_accessor :amount                         # 0-1 Quantity
    ##
    # Additional names for a medication, for example, the name(s) given to a medication in different countries.  For example, acetaminophen and paracetamol or salbutamol and albuterol.
    attr_accessor :synonym                        # 0-* [ string ]
    ##
    # Associated or related medication information
    # Associated or related knowledge about a medication.
    attr_accessor :relatedMedicationKnowledge     # 0-* [ MedicationKnowledge::RelatedMedicationKnowledge ]
    ##
    # A medication resource that is associated with this medication
    # Associated or related medications.  For example, if the medication is a branded product (e.g. Crestor), this is the Therapeutic Moeity (e.g. Rosuvastatin) or if this is a generic medication (e.g. Rosuvastatin), this would link to a branded product (e.g. Crestor).
    attr_accessor :associatedMedication           # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Medication) ]
    ##
    # Category of the medication or product (e.g. branded product, therapeutic moeity, generic product, innovator product, etc.).
    attr_accessor :productType                    # 0-* [ CodeableConcept ]
    ##
    # Associated documentation about the medication.
    attr_accessor :monograph                      # 0-* [ MedicationKnowledge::Monograph ]
    ##
    # Active or inactive ingredient
    # Identifies a particular constituent of interest in the product.
    attr_accessor :ingredient                     # 0-* [ MedicationKnowledge::Ingredient ]
    ##
    # The instructions for preparing the medication.
    attr_accessor :preparationInstruction         # 0-1 markdown
    ##
    # The intended or approved route of administration.
    attr_accessor :intendedRoute                  # 0-* [ CodeableConcept ]
    ##
    # The pricing of the medication
    # The price of the medication.
    attr_accessor :cost                           # 0-* [ MedicationKnowledge::Cost ]
    ##
    # Program under which a medication is reviewed
    # The program under which the medication is reviewed.
    attr_accessor :monitoringProgram              # 0-* [ MedicationKnowledge::MonitoringProgram ]
    ##
    # Guidelines for administration of the medication
    # Guidelines for the administration of the medication.
    attr_accessor :administrationGuidelines       # 0-* [ MedicationKnowledge::AdministrationGuidelines ]
    ##
    # Categorization of the medication within a formulary or classification system.
    attr_accessor :medicineClassification         # 0-* [ MedicationKnowledge::MedicineClassification ]
    ##
    # Details about packaged medications
    # Information that only applies to packages (not products).
    attr_accessor :packaging                      # 0-1 MedicationKnowledge::Packaging
    ##
    # Specifies descriptive properties of the medicine, such as color, shape, imprints, etc.
    attr_accessor :drugCharacteristic             # 0-* [ MedicationKnowledge::DrugCharacteristic ]
    ##
    # Potential clinical issue with or between medication(s) (for example, drug-drug interaction, drug-disease contraindication, drug-allergy interaction, etc.).
    attr_accessor :contraindication               # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DetectedIssue) ]
    ##
    # Regulatory information about a medication.
    attr_accessor :regulatory                     # 0-* [ MedicationKnowledge::Regulatory ]
    ##
    # The time course of drug absorption, distribution, metabolism and excretion of a medication from the body.
    attr_accessor :kinetics                       # 0-* [ MedicationKnowledge::Kinetics ]

    def resourceType
      'MedicationKnowledge'
    end
  end
end
