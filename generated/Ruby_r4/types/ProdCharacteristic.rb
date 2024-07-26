module FHIR

  ##
  # Base StructureDefinition for ProdCharacteristic Type: The marketing status describes the date when a medicinal product is actually put on the market or the date as of which it is no longer available.
  class ProdCharacteristic < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'id',
        'path'=>'ProdCharacteristic.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ProdCharacteristic.extension',
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
        'path'=>'ProdCharacteristic.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Where applicable, the height can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
      'height' => {
        'type'=>'Quantity',
        'path'=>'ProdCharacteristic.height',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where applicable, the width can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
      'width' => {
        'type'=>'Quantity',
        'path'=>'ProdCharacteristic.width',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where applicable, the depth can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
      'depth' => {
        'type'=>'Quantity',
        'path'=>'ProdCharacteristic.depth',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where applicable, the weight can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
      'weight' => {
        'type'=>'Quantity',
        'path'=>'ProdCharacteristic.weight',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where applicable, the nominal volume can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
      'nominalVolume' => {
        'type'=>'Quantity',
        'path'=>'ProdCharacteristic.nominalVolume',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where applicable, the external diameter can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
      'externalDiameter' => {
        'type'=>'Quantity',
        'path'=>'ProdCharacteristic.externalDiameter',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where applicable, the shape can be specified An appropriate controlled vocabulary shall be used The term and the term identifier shall be used.
      'shape' => {
        'type'=>'string',
        'path'=>'ProdCharacteristic.shape',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where applicable, the color can be specified An appropriate controlled vocabulary shall be used The term and the term identifier shall be used.
      'color' => {
        'type'=>'string',
        'path'=>'ProdCharacteristic.color',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Where applicable, the imprint can be specified as text.
      'imprint' => {
        'type'=>'string',
        'path'=>'ProdCharacteristic.imprint',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Where applicable, the image can be provided The format of the image attachment shall be specified by regional implementations.
      'image' => {
        'type'=>'Attachment',
        'path'=>'ProdCharacteristic.image',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Where applicable, the scoring can be specified An appropriate controlled vocabulary shall be used The term and the term identifier shall be used.
      'scoring' => {
        'type'=>'CodeableConcept',
        'path'=>'ProdCharacteristic.scoring',
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
    # Where applicable, the height can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    attr_accessor :height                         # 0-1 Quantity
    ##
    # Where applicable, the width can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    attr_accessor :width                          # 0-1 Quantity
    ##
    # Where applicable, the depth can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    attr_accessor :depth                          # 0-1 Quantity
    ##
    # Where applicable, the weight can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    attr_accessor :weight                         # 0-1 Quantity
    ##
    # Where applicable, the nominal volume can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    attr_accessor :nominalVolume                  # 0-1 Quantity
    ##
    # Where applicable, the external diameter can be specified using a numerical value and its unit of measurement The unit of measurement shall be specified in accordance with ISO 11240 and the resulting terminology The symbol and the symbol identifier shall be used.
    attr_accessor :externalDiameter               # 0-1 Quantity
    ##
    # Where applicable, the shape can be specified An appropriate controlled vocabulary shall be used The term and the term identifier shall be used.
    attr_accessor :shape                          # 0-1 string
    ##
    # Where applicable, the color can be specified An appropriate controlled vocabulary shall be used The term and the term identifier shall be used.
    attr_accessor :color                          # 0-* [ string ]
    ##
    # Where applicable, the imprint can be specified as text.
    attr_accessor :imprint                        # 0-* [ string ]
    ##
    # Where applicable, the image can be provided The format of the image attachment shall be specified by regional implementations.
    attr_accessor :image                          # 0-* [ Attachment ]
    ##
    # Where applicable, the scoring can be specified An appropriate controlled vocabulary shall be used The term and the term identifier shall be used.
    attr_accessor :scoring                        # 0-1 CodeableConcept
  end
end
