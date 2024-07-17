module FHIR

  ##
  # The detailed description of a substance, typically at a level beyond what is used for prescribing.
  class SubstanceSpecification < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['code']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'SubstanceSpecification.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'SubstanceSpecification.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'SubstanceSpecification.implicitRules',
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
        'path'=>'SubstanceSpecification.language',
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
        'path'=>'SubstanceSpecification.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'SubstanceSpecification.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'SubstanceSpecification.extension',
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
        'path'=>'SubstanceSpecification.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identifier by which this substance is known.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'SubstanceSpecification.identifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # High level categorization, e.g. polymer or nucleic acid.
      'type' => {
        'type'=>'CodeableConcept',
        'path'=>'SubstanceSpecification.type',
        'min'=>0,
        'max'=>1
      },
      ##
      # Status of substance within the catalogue e.g. approved.
      'status' => {
        'type'=>'CodeableConcept',
        'path'=>'SubstanceSpecification.status',
        'min'=>0,
        'max'=>1
      },
      ##
      # If the substance applies to only human or veterinary use.
      'domain' => {
        'type'=>'CodeableConcept',
        'path'=>'SubstanceSpecification.domain',
        'min'=>0,
        'max'=>1
      },
      ##
      # Textual description of the substance.
      'description' => {
        'type'=>'string',
        'path'=>'SubstanceSpecification.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # Supporting literature.
      'source' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference'],
        'type'=>'Reference',
        'path'=>'SubstanceSpecification.source',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Textual comment about this record of a substance.
      'comment' => {
        'type'=>'string',
        'path'=>'SubstanceSpecification.comment',
        'min'=>0,
        'max'=>1
      },
      ##
      # Moiety, for structural modifications.
      'moiety' => {
        'type'=>'SubstanceSpecification::Moiety',
        'path'=>'SubstanceSpecification.moiety',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # General specifications for this substance, including how it is related to other substances.
      'property' => {
        'type'=>'SubstanceSpecification::Property',
        'path'=>'SubstanceSpecification.property',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # General information detailing this substance.
      'referenceInformation' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/SubstanceReferenceInformation'],
        'type'=>'Reference',
        'path'=>'SubstanceSpecification.referenceInformation',
        'min'=>0,
        'max'=>1
      },
      ##
      # Structural information.
      'structure' => {
        'type'=>'SubstanceSpecification::Structure',
        'path'=>'SubstanceSpecification.structure',
        'min'=>0,
        'max'=>1
      },
      ##
      # Codes associated with the substance.
      'code' => {
        'type'=>'SubstanceSpecification::Code',
        'path'=>'SubstanceSpecification.code',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Names applicable to this substance.
      'name' => {
        'type'=>'SubstanceSpecification::Name',
        'path'=>'SubstanceSpecification.name',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The molecular weight or weight range (for proteins, polymers or nucleic acids).
      'molecularWeight' => {
        'type'=>'SubstanceSpecification::Structure::Isotope::MolecularWeight',
        'path'=>'SubstanceSpecification.molecularWeight',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A link between this substance and another, with details of the relationship.
      'relationship' => {
        'type'=>'SubstanceSpecification::Relationship',
        'path'=>'SubstanceSpecification.relationship',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Data items specific to nucleic acids.
      'nucleicAcid' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/SubstanceNucleicAcid'],
        'type'=>'Reference',
        'path'=>'SubstanceSpecification.nucleicAcid',
        'min'=>0,
        'max'=>1
      },
      ##
      # Data items specific to polymers.
      'polymer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/SubstancePolymer'],
        'type'=>'Reference',
        'path'=>'SubstanceSpecification.polymer',
        'min'=>0,
        'max'=>1
      },
      ##
      # Data items specific to proteins.
      'protein' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/SubstanceProtein'],
        'type'=>'Reference',
        'path'=>'SubstanceSpecification.protein',
        'min'=>0,
        'max'=>1
      },
      ##
      # Material or taxonomic/anatomical source for the substance.
      'sourceMaterial' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/SubstanceSourceMaterial'],
        'type'=>'Reference',
        'path'=>'SubstanceSpecification.sourceMaterial',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Moiety, for structural modifications.
    class Moiety < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'amount[x]' => ['Quantity', 'string']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Moiety.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Moiety.extension',
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
          'path'=>'Moiety.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Role that the moiety is playing.
        'role' => {
          'type'=>'CodeableConcept',
          'path'=>'Moiety.role',
          'min'=>0,
          'max'=>1
        },
        ##
        # Identifier by which this moiety substance is known.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'Moiety.identifier',
          'min'=>0,
          'max'=>1
        },
        ##
        # Textual name for this moiety substance.
        'name' => {
          'type'=>'string',
          'path'=>'Moiety.name',
          'min'=>0,
          'max'=>1
        },
        ##
        # Stereochemistry type.
        'stereochemistry' => {
          'type'=>'CodeableConcept',
          'path'=>'Moiety.stereochemistry',
          'min'=>0,
          'max'=>1
        },
        ##
        # Optical activity type.
        'opticalActivity' => {
          'type'=>'CodeableConcept',
          'path'=>'Moiety.opticalActivity',
          'min'=>0,
          'max'=>1
        },
        ##
        # Molecular formula.
        'molecularFormula' => {
          'type'=>'string',
          'path'=>'Moiety.molecularFormula',
          'min'=>0,
          'max'=>1
        },
        ##
        # Quantitative value for this moiety.
        'amountQuantity' => {
          'type'=>'Quantity',
          'path'=>'Moiety.amount[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Quantitative value for this moiety.
        'amountString' => {
          'type'=>'String',
          'path'=>'Moiety.amount[x]',
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
      # Role that the moiety is playing.
      attr_accessor :role                           # 0-1 CodeableConcept
      ##
      # Identifier by which this moiety substance is known.
      attr_accessor :identifier                     # 0-1 Identifier
      ##
      # Textual name for this moiety substance.
      attr_accessor :name                           # 0-1 string
      ##
      # Stereochemistry type.
      attr_accessor :stereochemistry                # 0-1 CodeableConcept
      ##
      # Optical activity type.
      attr_accessor :opticalActivity                # 0-1 CodeableConcept
      ##
      # Molecular formula.
      attr_accessor :molecularFormula               # 0-1 string
      ##
      # Quantitative value for this moiety.
      attr_accessor :amountQuantity                 # 0-1 Quantity
      ##
      # Quantitative value for this moiety.
      attr_accessor :amountString                   # 0-1 String
    end

    ##
    # General specifications for this substance, including how it is related to other substances.
    class Property < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'definingSubstance[x]' => ['CodeableConcept', 'Reference'],
        'amount[x]' => ['Quantity', 'string']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Property.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Property.extension',
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
          'path'=>'Property.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A category for this property, e.g. Physical, Chemical, Enzymatic.
        'category' => {
          'type'=>'CodeableConcept',
          'path'=>'Property.category',
          'min'=>0,
          'max'=>1
        },
        ##
        # Property type e.g. viscosity, pH, isoelectric point.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Property.code',
          'min'=>0,
          'max'=>1
        },
        ##
        # Parameters that were used in the measurement of a property (e.g. for viscosity: measured at 20C with a pH of 7.1).
        'parameters' => {
          'type'=>'string',
          'path'=>'Property.parameters',
          'min'=>0,
          'max'=>1
        },
        ##
        # A substance upon which a defining property depends (e.g. for solubility: in water, in alcohol).
        'definingSubstanceCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Property.definingSubstance[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # A substance upon which a defining property depends (e.g. for solubility: in water, in alcohol).
        'definingSubstanceReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/SubstanceSpecification', 'http://hl7.org/fhir/StructureDefinition/Substance'],
          'type'=>'Reference',
          'path'=>'Property.definingSubstance[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Quantitative value for this property.
        'amountQuantity' => {
          'type'=>'Quantity',
          'path'=>'Property.amount[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Quantitative value for this property.
        'amountString' => {
          'type'=>'String',
          'path'=>'Property.amount[x]',
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
      # A category for this property, e.g. Physical, Chemical, Enzymatic.
      attr_accessor :category                       # 0-1 CodeableConcept
      ##
      # Property type e.g. viscosity, pH, isoelectric point.
      attr_accessor :code                           # 0-1 CodeableConcept
      ##
      # Parameters that were used in the measurement of a property (e.g. for viscosity: measured at 20C with a pH of 7.1).
      attr_accessor :parameters                     # 0-1 string
      ##
      # A substance upon which a defining property depends (e.g. for solubility: in water, in alcohol).
      attr_accessor :definingSubstanceCodeableConcept # 0-1 CodeableConcept
      ##
      # A substance upon which a defining property depends (e.g. for solubility: in water, in alcohol).
      attr_accessor :definingSubstanceReference     # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/SubstanceSpecification|http://hl7.org/fhir/StructureDefinition/Substance)
      ##
      # Quantitative value for this property.
      attr_accessor :amountQuantity                 # 0-1 Quantity
      ##
      # Quantitative value for this property.
      attr_accessor :amountString                   # 0-1 String
    end

    ##
    # Structural information.
    class Structure < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Structure.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Structure.extension',
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
          'path'=>'Structure.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Stereochemistry type.
        'stereochemistry' => {
          'type'=>'CodeableConcept',
          'path'=>'Structure.stereochemistry',
          'min'=>0,
          'max'=>1
        },
        ##
        # Optical activity type.
        'opticalActivity' => {
          'type'=>'CodeableConcept',
          'path'=>'Structure.opticalActivity',
          'min'=>0,
          'max'=>1
        },
        ##
        # Molecular formula.
        'molecularFormula' => {
          'type'=>'string',
          'path'=>'Structure.molecularFormula',
          'min'=>0,
          'max'=>1
        },
        ##
        # Specified per moiety according to the Hill system, i.e. first C, then H, then alphabetical, each moiety separated by a dot.
        'molecularFormulaByMoiety' => {
          'type'=>'string',
          'path'=>'Structure.molecularFormulaByMoiety',
          'min'=>0,
          'max'=>1
        },
        ##
        # Applicable for single substances that contain a radionuclide or a non-natural isotopic ratio.
        'isotope' => {
          'type'=>'SubstanceSpecification::Structure::Isotope',
          'path'=>'Structure.isotope',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The molecular weight or weight range (for proteins, polymers or nucleic acids).
        'molecularWeight' => {
          'type'=>'SubstanceSpecification::Structure::Isotope::MolecularWeight',
          'path'=>'Structure.molecularWeight',
          'min'=>0,
          'max'=>1
        },
        ##
        # Supporting literature.
        'source' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference'],
          'type'=>'Reference',
          'path'=>'Structure.source',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Molecular structural representation.
        'representation' => {
          'type'=>'SubstanceSpecification::Structure::Representation',
          'path'=>'Structure.representation',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Applicable for single substances that contain a radionuclide or a non-natural isotopic ratio.
      class Isotope < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Isotope.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Isotope.extension',
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
            'path'=>'Isotope.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Substance identifier for each non-natural or radioisotope.
          'identifier' => {
            'type'=>'Identifier',
            'path'=>'Isotope.identifier',
            'min'=>0,
            'max'=>1
          },
          ##
          # Substance name for each non-natural or radioisotope.
          'name' => {
            'type'=>'CodeableConcept',
            'path'=>'Isotope.name',
            'min'=>0,
            'max'=>1
          },
          ##
          # The type of isotopic substitution present in a single substance.
          'substitution' => {
            'type'=>'CodeableConcept',
            'path'=>'Isotope.substitution',
            'min'=>0,
            'max'=>1
          },
          ##
          # Half life - for a non-natural nuclide.
          'halfLife' => {
            'type'=>'Quantity',
            'path'=>'Isotope.halfLife',
            'min'=>0,
            'max'=>1
          },
          ##
          # The molecular weight or weight range (for proteins, polymers or nucleic acids).
          'molecularWeight' => {
            'type'=>'SubstanceSpecification::Structure::Isotope::MolecularWeight',
            'path'=>'Isotope.molecularWeight',
            'min'=>0,
            'max'=>1
          }
        }

        ##
        # The molecular weight or weight range (for proteins, polymers or nucleic acids).
        class MolecularWeight < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'string',
              'path'=>'MolecularWeight.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'MolecularWeight.extension',
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
              'path'=>'MolecularWeight.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # The method by which the molecular weight was determined.
            'method' => {
              'local_name'=>'local_method'
              'type'=>'CodeableConcept',
              'path'=>'MolecularWeight.method',
              'min'=>0,
              'max'=>1
            },
            ##
            # Type of molecular weight such as exact, average (also known as. number average), weight average.
            'type' => {
              'type'=>'CodeableConcept',
              'path'=>'MolecularWeight.type',
              'min'=>0,
              'max'=>1
            },
            ##
            # Used to capture quantitative values for a variety of elements. If only limits are given, the arithmetic mean would be the average. If only a single definite value for a given element is given, it would be captured in this field.
            'amount' => {
              'type'=>'Quantity',
              'path'=>'MolecularWeight.amount',
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
          # The method by which the molecular weight was determined.
          attr_accessor :local_method                   # 0-1 CodeableConcept
          ##
          # Type of molecular weight such as exact, average (also known as. number average), weight average.
          attr_accessor :type                           # 0-1 CodeableConcept
          ##
          # Used to capture quantitative values for a variety of elements. If only limits are given, the arithmetic mean would be the average. If only a single definite value for a given element is given, it would be captured in this field.
          attr_accessor :amount                         # 0-1 Quantity
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
        # Substance identifier for each non-natural or radioisotope.
        attr_accessor :identifier                     # 0-1 Identifier
        ##
        # Substance name for each non-natural or radioisotope.
        attr_accessor :name                           # 0-1 CodeableConcept
        ##
        # The type of isotopic substitution present in a single substance.
        attr_accessor :substitution                   # 0-1 CodeableConcept
        ##
        # Half life - for a non-natural nuclide.
        attr_accessor :halfLife                       # 0-1 Quantity
        ##
        # The molecular weight or weight range (for proteins, polymers or nucleic acids).
        attr_accessor :molecularWeight                # 0-1 SubstanceSpecification::Structure::Isotope::MolecularWeight
      end

      ##
      # Molecular structural representation.
      class Representation < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Representation.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Representation.extension',
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
            'path'=>'Representation.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # The type of structure (e.g. Full, Partial, Representative).
          'type' => {
            'type'=>'CodeableConcept',
            'path'=>'Representation.type',
            'min'=>0,
            'max'=>1
          },
          ##
          # The structural representation as text string in a format e.g. InChI, SMILES, MOLFILE, CDX.
          'representation' => {
            'type'=>'string',
            'path'=>'Representation.representation',
            'min'=>0,
            'max'=>1
          },
          ##
          # An attached file with the structural representation.
          'attachment' => {
            'type'=>'Attachment',
            'path'=>'Representation.attachment',
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
        # The type of structure (e.g. Full, Partial, Representative).
        attr_accessor :type                           # 0-1 CodeableConcept
        ##
        # The structural representation as text string in a format e.g. InChI, SMILES, MOLFILE, CDX.
        attr_accessor :representation                 # 0-1 string
        ##
        # An attached file with the structural representation.
        attr_accessor :attachment                     # 0-1 Attachment
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
      # Stereochemistry type.
      attr_accessor :stereochemistry                # 0-1 CodeableConcept
      ##
      # Optical activity type.
      attr_accessor :opticalActivity                # 0-1 CodeableConcept
      ##
      # Molecular formula.
      attr_accessor :molecularFormula               # 0-1 string
      ##
      # Specified per moiety according to the Hill system, i.e. first C, then H, then alphabetical, each moiety separated by a dot.
      attr_accessor :molecularFormulaByMoiety       # 0-1 string
      ##
      # Applicable for single substances that contain a radionuclide or a non-natural isotopic ratio.
      attr_accessor :isotope                        # 0-* [ SubstanceSpecification::Structure::Isotope ]
      ##
      # The molecular weight or weight range (for proteins, polymers or nucleic acids).
      attr_accessor :molecularWeight                # 0-1 SubstanceSpecification::Structure::Isotope::MolecularWeight
      ##
      # Supporting literature.
      attr_accessor :source                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
      ##
      # Molecular structural representation.
      attr_accessor :representation                 # 0-* [ SubstanceSpecification::Structure::Representation ]
    end

    ##
    # Codes associated with the substance.
    class Code < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Code.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Code.extension',
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
          'path'=>'Code.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The specific code.
        'code' => {
          'type'=>'CodeableConcept',
          'path'=>'Code.code',
          'min'=>0,
          'max'=>1
        },
        ##
        # Status of the code assignment.
        'status' => {
          'type'=>'CodeableConcept',
          'path'=>'Code.status',
          'min'=>0,
          'max'=>1
        },
        ##
        # The date at which the code status is changed as part of the terminology maintenance.
        'statusDate' => {
          'type'=>'dateTime',
          'path'=>'Code.statusDate',
          'min'=>0,
          'max'=>1
        },
        ##
        # Any comment can be provided in this field, if necessary.
        'comment' => {
          'type'=>'string',
          'path'=>'Code.comment',
          'min'=>0,
          'max'=>1
        },
        ##
        # Supporting literature.
        'source' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference'],
          'type'=>'Reference',
          'path'=>'Code.source',
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
      # The specific code.
      attr_accessor :code                           # 0-1 CodeableConcept
      ##
      # Status of the code assignment.
      attr_accessor :status                         # 0-1 CodeableConcept
      ##
      # The date at which the code status is changed as part of the terminology maintenance.
      attr_accessor :statusDate                     # 0-1 dateTime
      ##
      # Any comment can be provided in this field, if necessary.
      attr_accessor :comment                        # 0-1 string
      ##
      # Supporting literature.
      attr_accessor :source                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    end

    ##
    # Names applicable to this substance.
    class Name < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Name.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Name.extension',
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
          'path'=>'Name.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The actual name.
        'name' => {
          'type'=>'string',
          'path'=>'Name.name',
          'min'=>1,
          'max'=>1
        },
        ##
        # Name type.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Name.type',
          'min'=>0,
          'max'=>1
        },
        ##
        # The status of the name.
        'status' => {
          'type'=>'CodeableConcept',
          'path'=>'Name.status',
          'min'=>0,
          'max'=>1
        },
        ##
        # If this is the preferred name for this substance.
        'preferred' => {
          'type'=>'boolean',
          'path'=>'Name.preferred',
          'min'=>0,
          'max'=>1
        },
        ##
        # Language of the name.
        'language' => {
          'type'=>'CodeableConcept',
          'path'=>'Name.language',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The use context of this name for example if there is a different name a drug active ingredient as opposed to a food colour additive.
        'domain' => {
          'type'=>'CodeableConcept',
          'path'=>'Name.domain',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The jurisdiction where this name applies.
        'jurisdiction' => {
          'type'=>'CodeableConcept',
          'path'=>'Name.jurisdiction',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A synonym of this name.
        'synonym' => {
          'type'=>'SubstanceSpecification::Name',
          'path'=>'Name.synonym',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A translation for this name.
        'translation' => {
          'type'=>'SubstanceSpecification::Name',
          'path'=>'Name.translation',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Details of the official nature of this name.
        'official' => {
          'type'=>'SubstanceSpecification::Name::Official',
          'path'=>'Name.official',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Supporting literature.
        'source' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference'],
          'type'=>'Reference',
          'path'=>'Name.source',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Details of the official nature of this name.
      class Official < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Official.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Official.extension',
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
            'path'=>'Official.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Which authority uses this official name.
          'authority' => {
            'type'=>'CodeableConcept',
            'path'=>'Official.authority',
            'min'=>0,
            'max'=>1
          },
          ##
          # The status of the official name.
          'status' => {
            'type'=>'CodeableConcept',
            'path'=>'Official.status',
            'min'=>0,
            'max'=>1
          },
          ##
          # Date of official name change.
          'date' => {
            'type'=>'dateTime',
            'path'=>'Official.date',
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
        # Which authority uses this official name.
        attr_accessor :authority                      # 0-1 CodeableConcept
        ##
        # The status of the official name.
        attr_accessor :status                         # 0-1 CodeableConcept
        ##
        # Date of official name change.
        attr_accessor :date                           # 0-1 dateTime
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
      # The actual name.
      attr_accessor :name                           # 1-1 string
      ##
      # Name type.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # The status of the name.
      attr_accessor :status                         # 0-1 CodeableConcept
      ##
      # If this is the preferred name for this substance.
      attr_accessor :preferred                      # 0-1 boolean
      ##
      # Language of the name.
      attr_accessor :language                       # 0-* [ CodeableConcept ]
      ##
      # The use context of this name for example if there is a different name a drug active ingredient as opposed to a food colour additive.
      attr_accessor :domain                         # 0-* [ CodeableConcept ]
      ##
      # The jurisdiction where this name applies.
      attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
      ##
      # A synonym of this name.
      attr_accessor :synonym                        # 0-* [ SubstanceSpecification::Name ]
      ##
      # A translation for this name.
      attr_accessor :translation                    # 0-* [ SubstanceSpecification::Name ]
      ##
      # Details of the official nature of this name.
      attr_accessor :official                       # 0-* [ SubstanceSpecification::Name::Official ]
      ##
      # Supporting literature.
      attr_accessor :source                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    end

    ##
    # A link between this substance and another, with details of the relationship.
    class Relationship < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'substance[x]' => ['CodeableConcept', 'Reference'],
        'amount[x]' => ['Quantity', 'Range', 'Ratio', 'string']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Relationship.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Relationship.extension',
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
          'path'=>'Relationship.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A pointer to another substance, as a resource or just a representational code.
        'substanceCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Relationship.substance[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # A pointer to another substance, as a resource or just a representational code.
        'substanceReference' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/SubstanceSpecification'],
          'type'=>'Reference',
          'path'=>'Relationship.substance[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # For example "salt to parent", "active moiety", "starting material".
        'relationship' => {
          'type'=>'CodeableConcept',
          'path'=>'Relationship.relationship',
          'min'=>0,
          'max'=>1
        },
        ##
        # For example where an enzyme strongly bonds with a particular substance, this is a defining relationship for that enzyme, out of several possible substance relationships.
        'isDefining' => {
          'type'=>'boolean',
          'path'=>'Relationship.isDefining',
          'min'=>0,
          'max'=>1
        },
        ##
        # A numeric factor for the relationship, for instance to express that the salt of a substance has some percentage of the active substance in relation to some other.
        'amountQuantity' => {
          'type'=>'Quantity',
          'path'=>'Relationship.amount[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # A numeric factor for the relationship, for instance to express that the salt of a substance has some percentage of the active substance in relation to some other.
        'amountRange' => {
          'type'=>'Range',
          'path'=>'Relationship.amount[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # A numeric factor for the relationship, for instance to express that the salt of a substance has some percentage of the active substance in relation to some other.
        'amountRatio' => {
          'type'=>'Ratio',
          'path'=>'Relationship.amount[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # A numeric factor for the relationship, for instance to express that the salt of a substance has some percentage of the active substance in relation to some other.
        'amountString' => {
          'type'=>'String',
          'path'=>'Relationship.amount[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # For use when the numeric.
        'amountRatioLowLimit' => {
          'type'=>'Ratio',
          'path'=>'Relationship.amountRatioLowLimit',
          'min'=>0,
          'max'=>1
        },
        ##
        # An operator for the amount, for example "average", "approximately", "less than".
        'amountType' => {
          'type'=>'CodeableConcept',
          'path'=>'Relationship.amountType',
          'min'=>0,
          'max'=>1
        },
        ##
        # Supporting literature.
        'source' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/DocumentReference'],
          'type'=>'Reference',
          'path'=>'Relationship.source',
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
      # A pointer to another substance, as a resource or just a representational code.
      attr_accessor :substanceCodeableConcept       # 0-1 CodeableConcept
      ##
      # A pointer to another substance, as a resource or just a representational code.
      attr_accessor :substanceReference             # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/SubstanceSpecification)
      ##
      # For example "salt to parent", "active moiety", "starting material".
      attr_accessor :relationship                   # 0-1 CodeableConcept
      ##
      # For example where an enzyme strongly bonds with a particular substance, this is a defining relationship for that enzyme, out of several possible substance relationships.
      attr_accessor :isDefining                     # 0-1 boolean
      ##
      # A numeric factor for the relationship, for instance to express that the salt of a substance has some percentage of the active substance in relation to some other.
      attr_accessor :amountQuantity                 # 0-1 Quantity
      ##
      # A numeric factor for the relationship, for instance to express that the salt of a substance has some percentage of the active substance in relation to some other.
      attr_accessor :amountRange                    # 0-1 Range
      ##
      # A numeric factor for the relationship, for instance to express that the salt of a substance has some percentage of the active substance in relation to some other.
      attr_accessor :amountRatio                    # 0-1 Ratio
      ##
      # A numeric factor for the relationship, for instance to express that the salt of a substance has some percentage of the active substance in relation to some other.
      attr_accessor :amountString                   # 0-1 String
      ##
      # For use when the numeric.
      attr_accessor :amountRatioLowLimit            # 0-1 Ratio
      ##
      # An operator for the amount, for example "average", "approximately", "less than".
      attr_accessor :amountType                     # 0-1 CodeableConcept
      ##
      # Supporting literature.
      attr_accessor :source                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
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
    # Identifier by which this substance is known.
    attr_accessor :identifier                     # 0-1 Identifier
    ##
    # High level categorization, e.g. polymer or nucleic acid.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # Status of substance within the catalogue e.g. approved.
    attr_accessor :status                         # 0-1 CodeableConcept
    ##
    # If the substance applies to only human or veterinary use.
    attr_accessor :domain                         # 0-1 CodeableConcept
    ##
    # Textual description of the substance.
    attr_accessor :description                    # 0-1 string
    ##
    # Supporting literature.
    attr_accessor :source                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/DocumentReference) ]
    ##
    # Textual comment about this record of a substance.
    attr_accessor :comment                        # 0-1 string
    ##
    # Moiety, for structural modifications.
    attr_accessor :moiety                         # 0-* [ SubstanceSpecification::Moiety ]
    ##
    # General specifications for this substance, including how it is related to other substances.
    attr_accessor :property                       # 0-* [ SubstanceSpecification::Property ]
    ##
    # General information detailing this substance.
    attr_accessor :referenceInformation           # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/SubstanceReferenceInformation)
    ##
    # Structural information.
    attr_accessor :structure                      # 0-1 SubstanceSpecification::Structure
    ##
    # Codes associated with the substance.
    attr_accessor :code                           # 0-* [ SubstanceSpecification::Code ]
    ##
    # Names applicable to this substance.
    attr_accessor :name                           # 0-* [ SubstanceSpecification::Name ]
    ##
    # The molecular weight or weight range (for proteins, polymers or nucleic acids).
    attr_accessor :molecularWeight                # 0-* [ SubstanceSpecification::Structure::Isotope::MolecularWeight ]
    ##
    # A link between this substance and another, with details of the relationship.
    attr_accessor :relationship                   # 0-* [ SubstanceSpecification::Relationship ]
    ##
    # Data items specific to nucleic acids.
    attr_accessor :nucleicAcid                    # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/SubstanceNucleicAcid)
    ##
    # Data items specific to polymers.
    attr_accessor :polymer                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/SubstancePolymer)
    ##
    # Data items specific to proteins.
    attr_accessor :protein                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/SubstanceProtein)
    ##
    # Material or taxonomic/anatomical source for the substance.
    attr_accessor :sourceMaterial                 # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/SubstanceSourceMaterial)

    def resourceType
      'SubstanceSpecification'
    end
  end
end
