module FHIR

  ##
  # A request to supply a diet, formula feeding (enteral) or oral nutritional supplement to a patient/resident.
  class NutritionOrder < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['additive', 'datetime', 'encounter', 'formula', 'identifier', 'instantiates-canonical', 'instantiates-uri', 'oraldiet', 'patient', 'provider', 'status', 'supplement']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'NutritionOrder.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'NutritionOrder.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'NutritionOrder.implicitRules',
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
        'path'=>'NutritionOrder.language',
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
        'path'=>'NutritionOrder.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'NutritionOrder.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'NutritionOrder.extension',
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
        'path'=>'NutritionOrder.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identifiers assigned to this order by the order sender or by the order receiver.
      # The Identifier.type element can be to indicate filler vs. placer if needed.  This is explained in further detail [here](servicerequest.html#notes).
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'NutritionOrder.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates FHIR protocol or definition
      # The URL pointing to a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this NutritionOrder.
      # Note: This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
      'instantiatesCanonical' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ActivityDefinition', 'http://hl7.org/fhir/StructureDefinition/PlanDefinition'],
        'type'=>'canonical',
        'path'=>'NutritionOrder.instantiatesCanonical',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates external protocol or definition
      # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this NutritionOrder.
      # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
      'instantiatesUri' => {
        'type'=>'uri',
        'path'=>'NutritionOrder.instantiatesUri',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Instantiates protocol or definition
      # The URL pointing to a protocol, guideline, orderset or other definition that is adhered to in whole or in part by this NutritionOrder.
      'instantiates' => {
        'type'=>'uri',
        'path'=>'NutritionOrder.instantiates',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # draft | active | on-hold | revoked | completed | entered-in-error | unknown
      # The workflow status of the nutrition order/request.
      # Typically the system placing the order sets the status to "requested". Thereafter, the order is maintained by the receiver that updates the status as the request is handled.  This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-status'=>[ 'draft', 'active', 'on-hold', 'revoked', 'completed', 'entered-in-error', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'NutritionOrder.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-status'}
      },
      ##
      # proposal | plan | directive | order | original-order | reflex-order | filler-order | instance-order | option
      # Indicates the level of authority/intentionality associated with the NutrionOrder and where the request fits into the workflow chain.
      # When resources map to this element, they are free to define as many codes as necessary to cover their space and will map to "proposal, plan or order".  Can have multiple codes that map to one of these.  E.g. "original order", "encoded order", "reflex order" would all map to "order".  Expectation is that the set of codes is mutually exclusive or a strict all-encompassing hierarchy.
      'intent' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/request-intent'=>[ 'proposal', 'plan', 'directive', 'order', 'original-order', 'reflex-order', 'filler-order', 'instance-order', 'option' ]
        },
        'type'=>'code',
        'path'=>'NutritionOrder.intent',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/request-intent'}
      },
      ##
      # The person who requires the diet, formula or nutritional supplement
      # The person (patient) who needs the nutrition order for an oral diet, nutritional supplement and/or enteral or formula feeding.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'NutritionOrder.patient',
        'min'=>1,
        'max'=>1
      },
      ##
      # The encounter associated with this nutrition order
      # An encounter that provides additional information about the healthcare context in which this request is made.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'NutritionOrder.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date and time the nutrition order was requested
      # The date and time that this nutrition order was requested.
      'dateTime' => {
        'type'=>'dateTime',
        'path'=>'NutritionOrder.dateTime',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who ordered the diet, formula or nutritional supplement
      # The practitioner that holds legal responsibility for ordering the diet, nutritional supplement, or formula feedings.
      'orderer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'NutritionOrder.orderer',
        'min'=>0,
        'max'=>1
      },
      ##
      # List of the patient's food and nutrition-related allergies and intolerances
      # A link to a record of allergies or intolerances  which should be included in the nutrition order.
      # Information on a patient's food allergies and intolerances to inform healthcare personnel about the type of foods that the patient shouldn't receive or consume.
      'allergyIntolerance' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/AllergyIntolerance'],
        'type'=>'Reference',
        'path'=>'NutritionOrder.allergyIntolerance',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Order-specific modifier about the type of food that should be given
      # This modifier is used to convey order-specific modifiers about the type of food that should be given. These can be derived from patient allergies, intolerances, or preferences such as Halal, Vegan or Kosher. This modifier applies to the entire nutrition order inclusive of the oral diet, nutritional supplements and enteral formula feedings.
      # Information on a patient's food preferences that inform healthcare personnel about the food that the patient should receive or consume.
      'foodPreferenceModifier' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/diet'=>[ 'vegetarian', 'dairy-free', 'nut-free', 'gluten-free', 'vegan', 'halal', 'kosher' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'NutritionOrder.foodPreferenceModifier',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/encounter-diet'}
      },
      ##
      # Order-specific modifier about the type of food that should not be given
      # This modifier is used to convey Order-specific modifier about the type of oral food or oral fluids that should not be given. These can be derived from patient allergies, intolerances, or preferences such as No Red Meat, No Soy or No Wheat or  Gluten-Free.  While it should not be necessary to repeat allergy or intolerance information captured in the referenced AllergyIntolerance resource in the excludeFoodModifier, this element may be used to convey additional specificity related to foods that should be eliminated from the patient’s diet for any reason.  This modifier applies to the entire nutrition order inclusive of the oral diet, nutritional supplements and enteral formula feedings.
      # Information on a patient's food allergies, intolerances and preferences to inform healthcare personnel about the type  of foods that the patient shouldn't receive or consume.
      'excludeFoodModifier' => {
        'type'=>'CodeableConcept',
        'path'=>'NutritionOrder.excludeFoodModifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Oral diet components
      # Diet given orally in contrast to enteral (tube) feeding.
      'oralDiet' => {
        'type'=>'NutritionOrder::OralDiet',
        'path'=>'NutritionOrder.oralDiet',
        'min'=>0,
        'max'=>1
      },
      ##
      # Supplement components
      # Oral nutritional products given in order to add further nutritional value to the patient's diet.
      'supplement' => {
        'type'=>'NutritionOrder::Supplement',
        'path'=>'NutritionOrder.supplement',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Enteral formula components
      # Feeding provided through the gastrointestinal tract via a tube, catheter, or stoma that delivers nutrition distal to the oral cavity.
      'enteralFormula' => {
        'type'=>'NutritionOrder::EnteralFormula',
        'path'=>'NutritionOrder.enteralFormula',
        'min'=>0,
        'max'=>1
      },
      ##
      # Comments made about the {{title}} by the requester, performer, subject or other participants.
      # This element SHALL NOT be used to supply free text instructions for the diet which are represented in the `.oralDiet.instruction`, `supplement.instruction`, or `enteralFormula.administrationInstruction` elements.
      'note' => {
        'type'=>'Annotation',
        'path'=>'NutritionOrder.note',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Oral diet components
    # Diet given orally in contrast to enteral (tube) feeding.
    class OralDiet < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'OralDiet.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'OralDiet.extension',
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
          'path'=>'OralDiet.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of oral diet or diet restrictions that describe what can be consumed orally
        # The kind of diet or dietary restriction such as fiber restricted diet or diabetic diet.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'OralDiet.type',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Scheduled frequency of diet
        # The time period and frequency at which the diet should be given.  The diet should be given for the combination of all schedules if more than one schedule is present.
        'schedule' => {
          'type'=>'Timing',
          'path'=>'OralDiet.schedule',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Required  nutrient modifications
        # Class that defines the quantity and type of nutrient modifications (for example carbohydrate, fiber or sodium) required for the oral diet.
        'nutrient' => {
          'type'=>'NutritionOrder::OralDiet::Nutrient',
          'path'=>'OralDiet.nutrient',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Required  texture modifications
        # Class that describes any texture modifications required for the patient to safely consume various types of solid foods.
        'texture' => {
          'type'=>'NutritionOrder::OralDiet::Texture',
          'path'=>'OralDiet.texture',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The required consistency of fluids and liquids provided to the patient
        # The required consistency (e.g. honey-thick, nectar-thick, thin, thickened.) of liquids or fluids served to the patient.
        'fluidConsistencyType' => {
          'valid_codes'=>{
            'http://snomed.info/sct'=>[ '439031000124108', '439021000124105', '439041000124103', '439081000124109' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'OralDiet.fluidConsistencyType',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/consistency-type'}
        },
        ##
        # Instructions or additional information about the oral diet
        # Free text or additional instructions or information pertaining to the oral diet.
        # Free text dosage instructions can be used for cases where the instructions are too complex to code.
        'instruction' => {
          'type'=>'string',
          'path'=>'OralDiet.instruction',
          'min'=>0,
          'max'=>1
        }
      }

      ##
      # Required  nutrient modifications
      # Class that defines the quantity and type of nutrient modifications (for example carbohydrate, fiber or sodium) required for the oral diet.
      class Nutrient < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Nutrient.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Nutrient.extension',
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
            'path'=>'Nutrient.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Type of nutrient that is being modified
          # The nutrient that is being modified such as carbohydrate or sodium.
          'modifier' => {
            'type'=>'CodeableConcept',
            'path'=>'Nutrient.modifier',
            'min'=>0,
            'max'=>1
          },
          ##
          # Quantity of the specified nutrient
          # The quantity of the specified nutrient to include in diet.
          'amount' => {
            'type'=>'Quantity',
            'path'=>'Nutrient.amount',
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
        # Type of nutrient that is being modified
        # The nutrient that is being modified such as carbohydrate or sodium.
        attr_accessor :modifier                       # 0-1 CodeableConcept
        ##
        # Quantity of the specified nutrient
        # The quantity of the specified nutrient to include in diet.
        attr_accessor :amount                         # 0-1 Quantity
      end

      ##
      # Required  texture modifications
      # Class that describes any texture modifications required for the patient to safely consume various types of solid foods.
      class Texture < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Texture.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Texture.extension',
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
            'path'=>'Texture.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Code to indicate how to alter the texture of the foods, e.g. pureed
          # Any texture modifications (for solid foods) that should be made, e.g. easy to chew, chopped, ground, and pureed.
          # Coupled with the foodType (Meat).
          'modifier' => {
            'valid_codes'=>{
              'http://snomed.info/sct'=>[ '228053002', '439091000124107', '228049004', '441881000124103', '441761000124103', '441751000124100', '228059003', '441791000124106', '228055009', '228056005', '441771000124105', '228057001', '228058006', '228060008' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Texture.modifier',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/texture-code'}
          },
          ##
          # Concepts that are used to identify an entity that is ingested for nutritional purposes
          # The food type(s) (e.g. meats, all foods)  that the texture modification applies to.  This could be all foods types.
          # Coupled with the `texture.modifier`; could be (All Foods).
          'foodType' => {
            'type'=>'CodeableConcept',
            'path'=>'Texture.foodType',
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
        # Code to indicate how to alter the texture of the foods, e.g. pureed
        # Any texture modifications (for solid foods) that should be made, e.g. easy to chew, chopped, ground, and pureed.
        # Coupled with the foodType (Meat).
        attr_accessor :modifier                       # 0-1 CodeableConcept
        ##
        # Concepts that are used to identify an entity that is ingested for nutritional purposes
        # The food type(s) (e.g. meats, all foods)  that the texture modification applies to.  This could be all foods types.
        # Coupled with the `texture.modifier`; could be (All Foods).
        attr_accessor :foodType                       # 0-1 CodeableConcept
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
      # Type of oral diet or diet restrictions that describe what can be consumed orally
      # The kind of diet or dietary restriction such as fiber restricted diet or diabetic diet.
      attr_accessor :type                           # 0-* [ CodeableConcept ]
      ##
      # Scheduled frequency of diet
      # The time period and frequency at which the diet should be given.  The diet should be given for the combination of all schedules if more than one schedule is present.
      attr_accessor :schedule                       # 0-* [ Timing ]
      ##
      # Required  nutrient modifications
      # Class that defines the quantity and type of nutrient modifications (for example carbohydrate, fiber or sodium) required for the oral diet.
      attr_accessor :nutrient                       # 0-* [ NutritionOrder::OralDiet::Nutrient ]
      ##
      # Required  texture modifications
      # Class that describes any texture modifications required for the patient to safely consume various types of solid foods.
      attr_accessor :texture                        # 0-* [ NutritionOrder::OralDiet::Texture ]
      ##
      # The required consistency of fluids and liquids provided to the patient
      # The required consistency (e.g. honey-thick, nectar-thick, thin, thickened.) of liquids or fluids served to the patient.
      attr_accessor :fluidConsistencyType           # 0-* [ CodeableConcept ]
      ##
      # Instructions or additional information about the oral diet
      # Free text or additional instructions or information pertaining to the oral diet.
      # Free text dosage instructions can be used for cases where the instructions are too complex to code.
      attr_accessor :instruction                    # 0-1 string
    end

    ##
    # Supplement components
    # Oral nutritional products given in order to add further nutritional value to the patient's diet.
    class Supplement < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Supplement.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Supplement.extension',
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
          'path'=>'Supplement.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of supplement product requested
        # The kind of nutritional supplement product required such as a high protein or pediatric clear liquid supplement.
        'type' => {
          'valid_codes'=>{
            'http://snomed.info/sct'=>[ '442901000124106', '443031000124106', '443051000124104', '442911000124109', '443021000124108', '442971000124100', '442981000124102', '442991000124104', '443011000124100', '442961000124107', '442951000124105', '442941000124108', '442921000124101', '442931000124103', '444331000124106', '443361000124100', '443391000124108', '443401000124105', '443491000124103', '443501000124106', '443421000124100', '443471000124104', '444431000124104', '443451000124109', '444321000124108', '441561000124106', '443461000124106', '441531000124102', '443561000124107', '443481000124101', '441571000124104', '441591000124103', '441601000124106', '443351000124102', '443771000124106', '441671000124100', '443111000124101', '443431000124102', '443411000124108', '444361000124102', '444401000124107', '444381000124107', '444371000124109', '443441000124107', '442651000124102' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Supplement.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/supplement-type'}
        },
        ##
        # Product or brand name of the nutritional supplement
        # The product or brand name of the nutritional supplement such as "Acme Protein Shake".
        'productName' => {
          'type'=>'string',
          'path'=>'Supplement.productName',
          'min'=>0,
          'max'=>1
        },
        ##
        # Scheduled frequency of supplement
        # The time period and frequency at which the supplement(s) should be given.  The supplement should be given for the combination of all schedules if more than one schedule is present.
        'schedule' => {
          'type'=>'Timing',
          'path'=>'Supplement.schedule',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Amount of the nutritional supplement
        # The amount of the nutritional supplement to be given.
        'quantity' => {
          'type'=>'Quantity',
          'path'=>'Supplement.quantity',
          'min'=>0,
          'max'=>1
        },
        ##
        # Instructions or additional information about the oral supplement
        # Free text or additional instructions or information pertaining to the oral supplement.
        # Free text dosage instructions can be used for cases where the instructions are too complex to code.
        'instruction' => {
          'type'=>'string',
          'path'=>'Supplement.instruction',
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
      # Type of supplement product requested
      # The kind of nutritional supplement product required such as a high protein or pediatric clear liquid supplement.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Product or brand name of the nutritional supplement
      # The product or brand name of the nutritional supplement such as "Acme Protein Shake".
      attr_accessor :productName                    # 0-1 string
      ##
      # Scheduled frequency of supplement
      # The time period and frequency at which the supplement(s) should be given.  The supplement should be given for the combination of all schedules if more than one schedule is present.
      attr_accessor :schedule                       # 0-* [ Timing ]
      ##
      # Amount of the nutritional supplement
      # The amount of the nutritional supplement to be given.
      attr_accessor :quantity                       # 0-1 Quantity
      ##
      # Instructions or additional information about the oral supplement
      # Free text or additional instructions or information pertaining to the oral supplement.
      # Free text dosage instructions can be used for cases where the instructions are too complex to code.
      attr_accessor :instruction                    # 0-1 string
    end

    ##
    # Enteral formula components
    # Feeding provided through the gastrointestinal tract via a tube, catheter, or stoma that delivers nutrition distal to the oral cavity.
    class EnteralFormula < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'EnteralFormula.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'EnteralFormula.extension',
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
          'path'=>'EnteralFormula.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type of enteral or infant formula
        # The type of enteral or infant formula such as an adult standard formula with fiber or a soy-based infant formula.
        'baseFormulaType' => {
          'valid_codes'=>{
            'http://snomed.info/sct'=>[ '443031000124106', '443051000124104', '442911000124109', '443021000124108', '442971000124100', '442981000124102', '442991000124104', '443011000124100', '442961000124107', '442951000124105', '442941000124108', '442921000124101', '442931000124103', '443361000124100', '443401000124105', '443491000124103', '443501000124106', '443421000124100', '443471000124104', '444431000124104', '443451000124109', '441561000124106', '443461000124106', '441531000124102', '443561000124107', '443481000124101', '441571000124104', '441591000124103', '441601000124106', '443351000124102', '443771000124106', '441671000124100', '443111000124101', '443431000124102', '443411000124108', '442651000124102' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'EnteralFormula.baseFormulaType',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/entformula-type'}
        },
        ##
        # Product or brand name of the enteral or infant formula
        # The product or brand name of the enteral or infant formula product such as "ACME Adult Standard Formula".
        'baseFormulaProductName' => {
          'type'=>'string',
          'path'=>'EnteralFormula.baseFormulaProductName',
          'min'=>0,
          'max'=>1
        },
        ##
        # Type of modular component to add to the feeding
        # Indicates the type of modular component such as protein, carbohydrate, fat or fiber to be provided in addition to or mixed with the base formula.
        'additiveType' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/entformula-additive'=>[ 'lipid', 'protein', 'carbohydrate', 'fiber', 'water' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'EnteralFormula.additiveType',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/entformula-additive'}
        },
        ##
        # Product or brand name of the modular additive
        # The product or brand name of the type of modular component to be added to the formula.
        'additiveProductName' => {
          'type'=>'string',
          'path'=>'EnteralFormula.additiveProductName',
          'min'=>0,
          'max'=>1
        },
        ##
        # Amount of energy per specified volume that is required
        # The amount of energy (calories) that the formula should provide per specified volume, typically per mL or fluid oz.  For example, an infant may require a formula that provides 24 calories per fluid ounce or an adult may require an enteral formula that provides 1.5 calorie/mL.
        'caloricDensity' => {
          'type'=>'Quantity',
          'path'=>'EnteralFormula.caloricDensity',
          'min'=>0,
          'max'=>1
        },
        ##
        # How the formula should enter the patient's gastrointestinal tract
        # The route or physiological path of administration into the patient's gastrointestinal  tract for purposes of providing the formula feeding, e.g. nasogastric tube.
        'routeofAdministration' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v3-RouteOfAdministration'=>[ 'PO', 'EFT', 'ENTINSTL', 'GT', 'NGT', 'OGT', 'GJT', 'JJTINSTL', 'OJJ' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'EnteralFormula.routeofAdministration',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/enteral-route'}
        },
        ##
        # Formula feeding instruction as structured data
        # Formula administration instructions as structured data.  This repeating structure allows for changing the administration rate or volume over time for both bolus and continuous feeding.  An example of this would be an instruction to increase the rate of continuous feeding every 2 hours.
        # See implementation notes below for further discussion on how to order continuous vs bolus enteral feeding using this resource.
        'administration' => {
          'type'=>'NutritionOrder::EnteralFormula::Administration',
          'path'=>'EnteralFormula.administration',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Upper limit on formula volume per unit of time
        # The maximum total quantity of formula that may be administered to a subject over the period of time, e.g. 1440 mL over 24 hours.
        'maxVolumeToDeliver' => {
          'type'=>'Quantity',
          'path'=>'EnteralFormula.maxVolumeToDeliver',
          'min'=>0,
          'max'=>1
        },
        ##
        # Formula feeding instructions expressed as text
        # Free text formula administration, feeding instructions or additional instructions or information.
        # Free text dosage instructions can be used for cases where the instructions are too complex to code.
        'administrationInstruction' => {
          'type'=>'string',
          'path'=>'EnteralFormula.administrationInstruction',
          'min'=>0,
          'max'=>1
        }
      }

      ##
      # Formula feeding instruction as structured data
      # Formula administration instructions as structured data.  This repeating structure allows for changing the administration rate or volume over time for both bolus and continuous feeding.  An example of this would be an instruction to increase the rate of continuous feeding every 2 hours.
      # See implementation notes below for further discussion on how to order continuous vs bolus enteral feeding using this resource.
      class Administration < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'rate[x]' => ['Quantity', 'Ratio']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Administration.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Administration.extension',
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
            'path'=>'Administration.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Scheduled frequency of enteral feeding
          # The time period and frequency at which the enteral formula should be delivered to the patient.
          'schedule' => {
            'type'=>'Timing',
            'path'=>'Administration.schedule',
            'min'=>0,
            'max'=>1
          },
          ##
          # The volume of formula to provide to the patient per the specified administration schedule.
          'quantity' => {
            'type'=>'Quantity',
            'path'=>'Administration.quantity',
            'min'=>0,
            'max'=>1
          },
          ##
          # Speed with which the formula is provided per period of time
          # The rate of administration of formula via a feeding pump, e.g. 60 mL per hour, according to the specified schedule.
          # Ratio is used when the quantity value in the denominator is not "1", otherwise use Quantity. For example, the Ratio datatype is used for "200 mL/4 hrs" versus the Quantity datatype for "50 mL/hr".
          'rateQuantity' => {
            'type'=>'Quantity',
            'path'=>'Administration.rate[x]',
            'min'=>0,
            'max'=>1
          }
          ##
          # Speed with which the formula is provided per period of time
          # The rate of administration of formula via a feeding pump, e.g. 60 mL per hour, according to the specified schedule.
          # Ratio is used when the quantity value in the denominator is not "1", otherwise use Quantity. For example, the Ratio datatype is used for "200 mL/4 hrs" versus the Quantity datatype for "50 mL/hr".
          'rateRatio' => {
            'type'=>'Ratio',
            'path'=>'Administration.rate[x]',
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
        # Scheduled frequency of enteral feeding
        # The time period and frequency at which the enteral formula should be delivered to the patient.
        attr_accessor :schedule                       # 0-1 Timing
        ##
        # The volume of formula to provide to the patient per the specified administration schedule.
        attr_accessor :quantity                       # 0-1 Quantity
        ##
        # Speed with which the formula is provided per period of time
        # The rate of administration of formula via a feeding pump, e.g. 60 mL per hour, according to the specified schedule.
        # Ratio is used when the quantity value in the denominator is not "1", otherwise use Quantity. For example, the Ratio datatype is used for "200 mL/4 hrs" versus the Quantity datatype for "50 mL/hr".
        attr_accessor :rateQuantity                   # 0-1 Quantity
        ##
        # Speed with which the formula is provided per period of time
        # The rate of administration of formula via a feeding pump, e.g. 60 mL per hour, according to the specified schedule.
        # Ratio is used when the quantity value in the denominator is not "1", otherwise use Quantity. For example, the Ratio datatype is used for "200 mL/4 hrs" versus the Quantity datatype for "50 mL/hr".
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
      # Type of enteral or infant formula
      # The type of enteral or infant formula such as an adult standard formula with fiber or a soy-based infant formula.
      attr_accessor :baseFormulaType                # 0-1 CodeableConcept
      ##
      # Product or brand name of the enteral or infant formula
      # The product or brand name of the enteral or infant formula product such as "ACME Adult Standard Formula".
      attr_accessor :baseFormulaProductName         # 0-1 string
      ##
      # Type of modular component to add to the feeding
      # Indicates the type of modular component such as protein, carbohydrate, fat or fiber to be provided in addition to or mixed with the base formula.
      attr_accessor :additiveType                   # 0-1 CodeableConcept
      ##
      # Product or brand name of the modular additive
      # The product or brand name of the type of modular component to be added to the formula.
      attr_accessor :additiveProductName            # 0-1 string
      ##
      # Amount of energy per specified volume that is required
      # The amount of energy (calories) that the formula should provide per specified volume, typically per mL or fluid oz.  For example, an infant may require a formula that provides 24 calories per fluid ounce or an adult may require an enteral formula that provides 1.5 calorie/mL.
      attr_accessor :caloricDensity                 # 0-1 Quantity
      ##
      # How the formula should enter the patient's gastrointestinal tract
      # The route or physiological path of administration into the patient's gastrointestinal  tract for purposes of providing the formula feeding, e.g. nasogastric tube.
      attr_accessor :routeofAdministration          # 0-1 CodeableConcept
      ##
      # Formula feeding instruction as structured data
      # Formula administration instructions as structured data.  This repeating structure allows for changing the administration rate or volume over time for both bolus and continuous feeding.  An example of this would be an instruction to increase the rate of continuous feeding every 2 hours.
      # See implementation notes below for further discussion on how to order continuous vs bolus enteral feeding using this resource.
      attr_accessor :administration                 # 0-* [ NutritionOrder::EnteralFormula::Administration ]
      ##
      # Upper limit on formula volume per unit of time
      # The maximum total quantity of formula that may be administered to a subject over the period of time, e.g. 1440 mL over 24 hours.
      attr_accessor :maxVolumeToDeliver             # 0-1 Quantity
      ##
      # Formula feeding instructions expressed as text
      # Free text formula administration, feeding instructions or additional instructions or information.
      # Free text dosage instructions can be used for cases where the instructions are too complex to code.
      attr_accessor :administrationInstruction      # 0-1 string
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
    # Identifiers assigned to this order by the order sender or by the order receiver.
    # The Identifier.type element can be to indicate filler vs. placer if needed.  This is explained in further detail [here](servicerequest.html#notes).
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Instantiates FHIR protocol or definition
    # The URL pointing to a FHIR-defined protocol, guideline, orderset or other definition that is adhered to in whole or in part by this NutritionOrder.
    # Note: This is a business identifier, not a resource identifier (see [discussion](resource.html#identifiers)).  It is best practice for the identifier to only appear on a single resource instance, however business practices may occasionally dictate that multiple resource instances with the same identifier can exist - possibly even with different resource types.  For example, multiple Patient and a Person resource instance might share the same social insurance number.
    attr_accessor :instantiatesCanonical          # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/ActivityDefinition|http://hl7.org/fhir/StructureDefinition/PlanDefinition) ]
    ##
    # Instantiates external protocol or definition
    # The URL pointing to an externally maintained protocol, guideline, orderset or other definition that is adhered to in whole or in part by this NutritionOrder.
    # This might be an HTML page, PDF, etc. or could just be a non-resolvable URI identifier.
    attr_accessor :instantiatesUri                # 0-* [ uri ]
    ##
    # Instantiates protocol or definition
    # The URL pointing to a protocol, guideline, orderset or other definition that is adhered to in whole or in part by this NutritionOrder.
    attr_accessor :instantiates                   # 0-* [ uri ]
    ##
    # draft | active | on-hold | revoked | completed | entered-in-error | unknown
    # The workflow status of the nutrition order/request.
    # Typically the system placing the order sets the status to "requested". Thereafter, the order is maintained by the receiver that updates the status as the request is handled.  This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # proposal | plan | directive | order | original-order | reflex-order | filler-order | instance-order | option
    # Indicates the level of authority/intentionality associated with the NutrionOrder and where the request fits into the workflow chain.
    # When resources map to this element, they are free to define as many codes as necessary to cover their space and will map to "proposal, plan or order".  Can have multiple codes that map to one of these.  E.g. "original order", "encoded order", "reflex order" would all map to "order".  Expectation is that the set of codes is mutually exclusive or a strict all-encompassing hierarchy.
    attr_accessor :intent                         # 1-1 code
    ##
    # The person who requires the diet, formula or nutritional supplement
    # The person (patient) who needs the nutrition order for an oral diet, nutritional supplement and/or enteral or formula feeding.
    attr_accessor :patient                        # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # The encounter associated with this nutrition order
    # An encounter that provides additional information about the healthcare context in which this request is made.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # Date and time the nutrition order was requested
    # The date and time that this nutrition order was requested.
    attr_accessor :dateTime                       # 1-1 dateTime
    ##
    # Who ordered the diet, formula or nutritional supplement
    # The practitioner that holds legal responsibility for ordering the diet, nutritional supplement, or formula feedings.
    attr_accessor :orderer                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # List of the patient's food and nutrition-related allergies and intolerances
    # A link to a record of allergies or intolerances  which should be included in the nutrition order.
    # Information on a patient's food allergies and intolerances to inform healthcare personnel about the type of foods that the patient shouldn't receive or consume.
    attr_accessor :allergyIntolerance             # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/AllergyIntolerance) ]
    ##
    # Order-specific modifier about the type of food that should be given
    # This modifier is used to convey order-specific modifiers about the type of food that should be given. These can be derived from patient allergies, intolerances, or preferences such as Halal, Vegan or Kosher. This modifier applies to the entire nutrition order inclusive of the oral diet, nutritional supplements and enteral formula feedings.
    # Information on a patient's food preferences that inform healthcare personnel about the food that the patient should receive or consume.
    attr_accessor :foodPreferenceModifier         # 0-* [ CodeableConcept ]
    ##
    # Order-specific modifier about the type of food that should not be given
    # This modifier is used to convey Order-specific modifier about the type of oral food or oral fluids that should not be given. These can be derived from patient allergies, intolerances, or preferences such as No Red Meat, No Soy or No Wheat or  Gluten-Free.  While it should not be necessary to repeat allergy or intolerance information captured in the referenced AllergyIntolerance resource in the excludeFoodModifier, this element may be used to convey additional specificity related to foods that should be eliminated from the patient’s diet for any reason.  This modifier applies to the entire nutrition order inclusive of the oral diet, nutritional supplements and enteral formula feedings.
    # Information on a patient's food allergies, intolerances and preferences to inform healthcare personnel about the type  of foods that the patient shouldn't receive or consume.
    attr_accessor :excludeFoodModifier            # 0-* [ CodeableConcept ]
    ##
    # Oral diet components
    # Diet given orally in contrast to enteral (tube) feeding.
    attr_accessor :oralDiet                       # 0-1 NutritionOrder::OralDiet
    ##
    # Supplement components
    # Oral nutritional products given in order to add further nutritional value to the patient's diet.
    attr_accessor :supplement                     # 0-* [ NutritionOrder::Supplement ]
    ##
    # Enteral formula components
    # Feeding provided through the gastrointestinal tract via a tube, catheter, or stoma that delivers nutrition distal to the oral cavity.
    attr_accessor :enteralFormula                 # 0-1 NutritionOrder::EnteralFormula
    ##
    # Comments made about the {{title}} by the requester, performer, subject or other participants.
    # This element SHALL NOT be used to supply free text instructions for the diet which are represented in the `.oralDiet.instruction`, `supplement.instruction`, or `enteralFormula.administrationInstruction` elements.
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'NutritionOrder'
    end
  end
end
