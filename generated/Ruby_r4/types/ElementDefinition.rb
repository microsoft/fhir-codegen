module FHIR

  ##
  # Base StructureDefinition for ElementDefinition Type: Captures constraints on each element within the resource, profile, or extension.
  class ElementDefinition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    MULTIPLE_TYPES = {
      'defaultValue[x]' => ['Address', 'Age', 'Annotation', 'Attachment', 'base64Binary', 'boolean', 'canonical', 'code', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'date', 'dateTime', 'decimal', 'Distance', 'Dosage', 'Duration', 'Expression', 'HumanName', 'id', 'Identifier', 'instant', 'integer', 'markdown', 'Meta', 'Money', 'oid', 'ParameterDefinition', 'Period', 'positiveInt', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'string', 'time', 'Timing', 'TriggerDefinition', 'unsignedInt', 'uri', 'url', 'UsageContext', 'uuid'],
      'fixed[x]' => ['Address', 'Age', 'Annotation', 'Attachment', 'base64Binary', 'boolean', 'canonical', 'code', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'date', 'dateTime', 'decimal', 'Distance', 'Dosage', 'Duration', 'Expression', 'HumanName', 'id', 'Identifier', 'instant', 'integer', 'markdown', 'Meta', 'Money', 'oid', 'ParameterDefinition', 'Period', 'positiveInt', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'string', 'time', 'Timing', 'TriggerDefinition', 'unsignedInt', 'uri', 'url', 'UsageContext', 'uuid'],
      'pattern[x]' => ['Address', 'Age', 'Annotation', 'Attachment', 'base64Binary', 'boolean', 'canonical', 'code', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'date', 'dateTime', 'decimal', 'Distance', 'Dosage', 'Duration', 'Expression', 'HumanName', 'id', 'Identifier', 'instant', 'integer', 'markdown', 'Meta', 'Money', 'oid', 'ParameterDefinition', 'Period', 'positiveInt', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'string', 'time', 'Timing', 'TriggerDefinition', 'unsignedInt', 'uri', 'url', 'UsageContext', 'uuid'],
      'minValue[x]' => ['date', 'dateTime', 'decimal', 'instant', 'integer', 'positiveInt', 'Quantity', 'time', 'unsignedInt'],
      'maxValue[x]' => ['date', 'dateTime', 'decimal', 'instant', 'integer', 'positiveInt', 'Quantity', 'time', 'unsignedInt']
    }
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'string',
        'path'=>'ElementDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ElementDefinition.extension',
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
        'path'=>'ElementDefinition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Path of the element in the hierarchy of elements
      # The path identifies the element and is expressed as a "."-separated list of ancestor elements, beginning with the name of the resource or extension.
      'path' => {
        'type'=>'string',
        'path'=>'ElementDefinition.path',
        'min'=>1,
        'max'=>1
      },
      ##
      # xmlAttr | xmlText | typeAttr | cdaText | xhtml
      # Codes that define how this element is represented in instances, when the deviation varies from the normal case.
      # In resources, this is rarely used except for special cases where the representation deviates from the normal, and can only be done in the base standard (and profiles must reproduce what the base standard does). This element is used quite commonly in Logical models when the logical models represent a specific serialization format (e.g. CDA, v2 etc.).
      'representation' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/property-representation'=>[ 'xmlAttr', 'xmlText', 'typeAttr', 'cdaText', 'xhtml' ]
        },
        'type'=>'code',
        'path'=>'ElementDefinition.representation',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/property-representation'}
      },
      ##
      # Name for this particular element (in a set of slices)
      # The name of this element definition slice, when slicing is working. The name must be a token with no dots or spaces. This is a unique name referring to a specific set of constraints applied to this element, used to provide a name to different slices of the same element.
      # The name SHALL be unique within the structure within the context of the constrained resource element.  (Though to avoid confusion, uniqueness across all elements is recommended.).
      'sliceName' => {
        'type'=>'string',
        'path'=>'ElementDefinition.sliceName',
        'min'=>0,
        'max'=>1
      },
      ##
      # If this slice definition constrains an inherited slice definition (or not)
      # If true, indicates that this slice definition is constraining a slice definition with the same name in an inherited profile. If false, the slice is not overriding any slice in an inherited profile. If missing, the slice might or might not be overriding a slice in an inherited profile, depending on the sliceName.
      # If set to true, an ancestor profile SHALL have a slicing definition with this name.  If set to false, no ancestor profile is permitted to have a slicing definition with this name.
      'sliceIsConstraining' => {
        'type'=>'boolean',
        'path'=>'ElementDefinition.sliceIsConstraining',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for element to display with or prompt for element
      # A single preferred label which is the text to display beside the element indicating its meaning or to use to prompt for the element in a user display or form.
      # See also the extension (http://hl7.org/fhir/StructureDefinition/elementdefinition-question)[extension-elementdefinition-question.html].
      'label' => {
        'type'=>'string',
        'path'=>'ElementDefinition.label',
        'min'=>0,
        'max'=>1
      },
      ##
      # Corresponding codes in terminologies
      # A code that has the same meaning as the element in a particular terminology.
      # The concept SHALL be properly aligned with the data element definition and other constraints, as defined in the code system, including relationships, of any code listed here.  Where multiple codes exist in a terminology that could correspond to the data element, the most granular code(s) should be selected, so long as they are not more restrictive than the data element itself. The mappings may be used to provide more or less granular or structured equivalences in the code system.
      'code' => {
        'type'=>'Coding',
        'path'=>'ElementDefinition.code',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # This element is sliced - slices follow
      # Indicates that the element is sliced into a set of alternative definitions (i.e. in a structure definition, there are multiple different constraints on a single element in the base resource). Slicing can be used in any resource that has cardinality ..* on the base resource, or any resource with a choice of types. The set of slices is any elements that come after this in the element sequence that have the same path, until a shorter path occurs (the shorter path terminates the set).
      # The first element in the sequence, the one that carries the slicing, is the definition that applies to all the slices. This is based on the unconstrained element, but can apply any constraints as appropriate. This may include the common constraints on the children of the element.
      'slicing' => {
        'type'=>'ElementDefinition::Slicing',
        'path'=>'ElementDefinition.slicing',
        'min'=>0,
        'max'=>1
      },
      ##
      # Concise definition for space-constrained presentation
      # A concise description of what this element means (e.g. for use in autogenerated summaries).
      # It is easy for a different short definition to change the meaning of an element and this can have nasty downstream consequences. Please be careful when providing short definitions in a profile.
      'short' => {
        'type'=>'string',
        'path'=>'ElementDefinition.short',
        'min'=>0,
        'max'=>1
      },
      ##
      # Full formal definition as narrative text
      # Provides a complete explanation of the meaning of the data element for human readability.  For the case of elements derived from existing elements (e.g. constraints), the definition SHALL be consistent with the base definition, but convey the meaning of the element in the particular context of use of the resource. (Note: The text you are reading is specified in ElementDefinition.definition).
      # It is easy for a different definition to change the meaning of an element and this can have nasty downstream consequences. Please be careful when providing definitions in a profile.
      'definition' => {
        'type'=>'markdown',
        'path'=>'ElementDefinition.definition',
        'min'=>0,
        'max'=>1
      },
      ##
      # Comments about the use of this element
      # Explanatory notes and implementation guidance about the data element, including notes about how to use the data properly, exceptions to proper use, etc. (Note: The text you are reading is specified in ElementDefinition.comment).
      # If it is possible to capture usage rules using constraints, that mechanism should be used in preference to this element.
      'comment' => {
        'type'=>'markdown',
        'path'=>'ElementDefinition.comment',
        'min'=>0,
        'max'=>1
      },
      ##
      # Why this resource has been created
      # This element is for traceability of why the element was created and why the constraints exist as they do. This may be used to point to source materials or specifications that drove the structure of this element.
      # This element does not describe the usage of the element (that's done in comments), rather it's for traceability of *why* the element is either needed or why the constraints exist as they do.  This may be used to point to source materials or specifications that drove the structure of this data element.
      'requirements' => {
        'type'=>'markdown',
        'path'=>'ElementDefinition.requirements',
        'min'=>0,
        'max'=>1
      },
      ##
      # Other names
      # Identifies additional names by which this element might also be known.
      'alias' => {
        'local_name'=>'local_alias'
        'type'=>'string',
        'path'=>'ElementDefinition.alias',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Minimum Cardinality
      # The minimum number of times this element SHALL appear in the instance.
      'min' => {
        'type'=>'unsignedInt',
        'path'=>'ElementDefinition.min',
        'min'=>0,
        'max'=>1
      },
      ##
      # Maximum Cardinality (a number or *)
      # The maximum number of times this element is permitted to appear in the instance.
      'max' => {
        'type'=>'string',
        'path'=>'ElementDefinition.max',
        'min'=>0,
        'max'=>1
      },
      ##
      # Base definition information for tools
      # Information about the base definition of the element, provided to make it unnecessary for tools to trace the deviation of the element through the derived and related profiles. When the element definition is not the original definition of an element - i.g. either in a constraint on another type, or for elements from a super type in a snap shot - then the information in provided in the element definition may be different to the base definition. On the original definition of the element, it will be same.
      # The base information does not carry any information that could not be determined from the path and related profiles, but making this determination requires both that the related profiles are available, and that the algorithm to determine them be available. For tooling simplicity, the base information must always be populated in element definitions in snap shots, even if it is the same.
      'base' => {
        'type'=>'ElementDefinition::Base',
        'path'=>'ElementDefinition.base',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reference to definition of content for the element
      # Identifies an element defined elsewhere in the definition whose content rules should be applied to the current element. ContentReferences bring across all the rules that are in the ElementDefinition for the element, including definitions, cardinality constraints, bindings, invariants etc.
      # ContentReferences can only be defined in specializations, not constrained types, and they cannot be changed and always reference the non-constrained definition.
      'contentReference' => {
        'type'=>'uri',
        'path'=>'ElementDefinition.contentReference',
        'min'=>0,
        'max'=>1
      },
      ##
      # Data type and Profile for this element
      # The data type or resource that the value of this element is permitted to be.
      # The Type of the element can be left blank in a differential constraint, in which case the type is inherited from the resource. Abstract types are not permitted to appear as a type when multiple types are listed.  (I.e. Abstract types cannot be part of a choice).
      'type' => {
        'type'=>'ElementDefinition::Type',
        'path'=>'ElementDefinition.type',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueAddress' => {
        'type'=>'Address',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueAge' => {
        'type'=>'Age',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueAnnotation' => {
        'type'=>'Annotation',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueAttachment' => {
        'type'=>'Attachment',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueBase64Binary' => {
        'type'=>'Base64Binary',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueBoolean' => {
        'type'=>'Boolean',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueCanonical' => {
        'type'=>'Canonical',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueCode' => {
        'type'=>'Code',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueCoding' => {
        'type'=>'Coding',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueContactDetail' => {
        'type'=>'ContactDetail',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueContactPoint' => {
        'type'=>'ContactPoint',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueContributor' => {
        'type'=>'Contributor',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueCount' => {
        'type'=>'Count',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueDataRequirement' => {
        'type'=>'DataRequirement',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueDate' => {
        'type'=>'Date',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueDateTime' => {
        'type'=>'DateTime',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueDecimal' => {
        'type'=>'Decimal',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueDistance' => {
        'type'=>'Distance',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueDosage' => {
        'type'=>'Dosage',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueDuration' => {
        'type'=>'Duration',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueExpression' => {
        'type'=>'Expression',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueHumanName' => {
        'type'=>'HumanName',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueId' => {
        'type'=>'Id',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueIdentifier' => {
        'type'=>'Identifier',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueInstant' => {
        'type'=>'Instant',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueInteger' => {
        'type'=>'Integer',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueMarkdown' => {
        'type'=>'Markdown',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueMeta' => {
        'type'=>'Meta',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueMoney' => {
        'type'=>'Money',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueOid' => {
        'type'=>'Oid',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueParameterDefinition' => {
        'type'=>'ParameterDefinition',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValuePeriod' => {
        'type'=>'Period',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValuePositiveInt' => {
        'type'=>'PositiveInt',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueQuantity' => {
        'type'=>'Quantity',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueRange' => {
        'type'=>'Range',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueRatio' => {
        'type'=>'Ratio',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueReference' => {
        'type'=>'Reference',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueRelatedArtifact' => {
        'type'=>'RelatedArtifact',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueSampledData' => {
        'type'=>'SampledData',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueSignature' => {
        'type'=>'Signature',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueString' => {
        'type'=>'String',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueTime' => {
        'type'=>'Time',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueTiming' => {
        'type'=>'Timing',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueTriggerDefinition' => {
        'type'=>'TriggerDefinition',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueUnsignedInt' => {
        'type'=>'UnsignedInt',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueUri' => {
        'type'=>'Uri',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueUrl' => {
        'type'=>'Url',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueUsageContext' => {
        'type'=>'UsageContext',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specified value if missing from instance
      # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
      # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
      # 
      # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
      'defaultValueUuid' => {
        'type'=>'Uuid',
        'path'=>'ElementDefinition.defaultValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Implicit meaning when this element is missing
      # The Implicit meaning that is to be understood when this element is missing (e.g. 'when this element is missing, the period is ongoing').
      # Implicit meanings for missing values can only be specified on a resource, data type, or extension definition, and never in a profile that applies to one of these. An implicit meaning for a missing value can never be changed, and specifying one has the consequence that constraining its use in profiles eliminates use cases as possibilities, not merely moving them out of scope.
      'meaningWhenMissing' => {
        'type'=>'markdown',
        'path'=>'ElementDefinition.meaningWhenMissing',
        'min'=>0,
        'max'=>1
      },
      ##
      # What the order of the elements means
      # If present, indicates that the order of the repeating element has meaning and describes what that meaning is.  If absent, it means that the order of the element has no meaning.
      # This element can only be asserted on repeating elements and can only be introduced when defining resources or data types.  It can be further refined profiled elements but if absent in the base type, a profile cannot assert meaning.
      'orderMeaning' => {
        'type'=>'string',
        'path'=>'ElementDefinition.orderMeaning',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedAddress' => {
        'type'=>'Address',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedAge' => {
        'type'=>'Age',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedAnnotation' => {
        'type'=>'Annotation',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedAttachment' => {
        'type'=>'Attachment',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedBase64Binary' => {
        'type'=>'Base64Binary',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedBoolean' => {
        'type'=>'Boolean',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedCanonical' => {
        'type'=>'Canonical',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedCode' => {
        'type'=>'Code',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedCoding' => {
        'type'=>'Coding',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedContactDetail' => {
        'type'=>'ContactDetail',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedContactPoint' => {
        'type'=>'ContactPoint',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedContributor' => {
        'type'=>'Contributor',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedCount' => {
        'type'=>'Count',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedDataRequirement' => {
        'type'=>'DataRequirement',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedDate' => {
        'type'=>'Date',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedDateTime' => {
        'type'=>'DateTime',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedDecimal' => {
        'type'=>'Decimal',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedDistance' => {
        'type'=>'Distance',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedDosage' => {
        'type'=>'Dosage',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedDuration' => {
        'type'=>'Duration',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedExpression' => {
        'type'=>'Expression',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedHumanName' => {
        'type'=>'HumanName',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedId' => {
        'type'=>'Id',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedIdentifier' => {
        'type'=>'Identifier',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedInstant' => {
        'type'=>'Instant',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedInteger' => {
        'type'=>'Integer',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedMarkdown' => {
        'type'=>'Markdown',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedMeta' => {
        'type'=>'Meta',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedMoney' => {
        'type'=>'Money',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedOid' => {
        'type'=>'Oid',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedParameterDefinition' => {
        'type'=>'ParameterDefinition',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedPeriod' => {
        'type'=>'Period',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedPositiveInt' => {
        'type'=>'PositiveInt',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedQuantity' => {
        'type'=>'Quantity',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedRange' => {
        'type'=>'Range',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedRatio' => {
        'type'=>'Ratio',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedReference' => {
        'type'=>'Reference',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedRelatedArtifact' => {
        'type'=>'RelatedArtifact',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedSampledData' => {
        'type'=>'SampledData',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedSignature' => {
        'type'=>'Signature',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedString' => {
        'type'=>'String',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedTime' => {
        'type'=>'Time',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedTiming' => {
        'type'=>'Timing',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedTriggerDefinition' => {
        'type'=>'TriggerDefinition',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedUnsignedInt' => {
        'type'=>'UnsignedInt',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedUri' => {
        'type'=>'Uri',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedUrl' => {
        'type'=>'Url',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedUsageContext' => {
        'type'=>'UsageContext',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must be exactly this
      # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
      # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
      'fixedUuid' => {
        'type'=>'Uuid',
        'path'=>'ElementDefinition.fixed[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternAddress' => {
        'type'=>'Address',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternAge' => {
        'type'=>'Age',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternAnnotation' => {
        'type'=>'Annotation',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternAttachment' => {
        'type'=>'Attachment',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternBase64Binary' => {
        'type'=>'Base64Binary',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternBoolean' => {
        'type'=>'Boolean',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternCanonical' => {
        'type'=>'Canonical',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternCode' => {
        'type'=>'Code',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternCodeableConcept' => {
        'type'=>'CodeableConcept',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternCoding' => {
        'type'=>'Coding',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternContactDetail' => {
        'type'=>'ContactDetail',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternContactPoint' => {
        'type'=>'ContactPoint',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternContributor' => {
        'type'=>'Contributor',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternCount' => {
        'type'=>'Count',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternDataRequirement' => {
        'type'=>'DataRequirement',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternDate' => {
        'type'=>'Date',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternDateTime' => {
        'type'=>'DateTime',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternDecimal' => {
        'type'=>'Decimal',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternDistance' => {
        'type'=>'Distance',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternDosage' => {
        'type'=>'Dosage',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternDuration' => {
        'type'=>'Duration',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternExpression' => {
        'type'=>'Expression',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternHumanName' => {
        'type'=>'HumanName',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternId' => {
        'type'=>'Id',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternIdentifier' => {
        'type'=>'Identifier',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternInstant' => {
        'type'=>'Instant',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternInteger' => {
        'type'=>'Integer',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternMarkdown' => {
        'type'=>'Markdown',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternMeta' => {
        'type'=>'Meta',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternMoney' => {
        'type'=>'Money',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternOid' => {
        'type'=>'Oid',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternParameterDefinition' => {
        'type'=>'ParameterDefinition',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternPeriod' => {
        'type'=>'Period',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternPositiveInt' => {
        'type'=>'PositiveInt',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternQuantity' => {
        'type'=>'Quantity',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternRange' => {
        'type'=>'Range',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternRatio' => {
        'type'=>'Ratio',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternReference' => {
        'type'=>'Reference',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternRelatedArtifact' => {
        'type'=>'RelatedArtifact',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternSampledData' => {
        'type'=>'SampledData',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternSignature' => {
        'type'=>'Signature',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternString' => {
        'type'=>'String',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternTime' => {
        'type'=>'Time',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternTiming' => {
        'type'=>'Timing',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternTriggerDefinition' => {
        'type'=>'TriggerDefinition',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternUnsignedInt' => {
        'type'=>'UnsignedInt',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternUri' => {
        'type'=>'Uri',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternUrl' => {
        'type'=>'Url',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternUsageContext' => {
        'type'=>'UsageContext',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Value must have at least these property values
      # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
      # 
      # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
      # 
      # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
      # 
      # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
      # 
      # 1. If primitive: it must match exactly the pattern value
      # 2. If a complex object: it must match (recursively) the pattern value
      # 3. If an array: it must match (recursively) the pattern value.
      # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
      'patternUuid' => {
        'type'=>'Uuid',
        'path'=>'ElementDefinition.pattern[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Example value (as defined for type)
      # A sample value for this element demonstrating the type of information that would typically be found in the element.
      # Examples will most commonly be present for data where it's not implicitly obvious from either the data type or value set what the values might be.  (I.e. Example values for dates or quantities would generally be unnecessary.)  If the example value is fully populated, the publication tool can generate an instance automatically.
      'example' => {
        'type'=>'ElementDefinition::Example',
        'path'=>'ElementDefinition.example',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Minimum Allowed Value (for some types)
      # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
      'minValueDate' => {
        'type'=>'Date',
        'path'=>'ElementDefinition.minValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Minimum Allowed Value (for some types)
      # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
      'minValueDateTime' => {
        'type'=>'DateTime',
        'path'=>'ElementDefinition.minValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Minimum Allowed Value (for some types)
      # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
      'minValueDecimal' => {
        'type'=>'Decimal',
        'path'=>'ElementDefinition.minValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Minimum Allowed Value (for some types)
      # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
      'minValueInstant' => {
        'type'=>'Instant',
        'path'=>'ElementDefinition.minValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Minimum Allowed Value (for some types)
      # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
      'minValueInteger' => {
        'type'=>'Integer',
        'path'=>'ElementDefinition.minValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Minimum Allowed Value (for some types)
      # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
      'minValuePositiveInt' => {
        'type'=>'PositiveInt',
        'path'=>'ElementDefinition.minValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Minimum Allowed Value (for some types)
      # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
      'minValueQuantity' => {
        'type'=>'Quantity',
        'path'=>'ElementDefinition.minValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Minimum Allowed Value (for some types)
      # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
      'minValueTime' => {
        'type'=>'Time',
        'path'=>'ElementDefinition.minValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Minimum Allowed Value (for some types)
      # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
      'minValueUnsignedInt' => {
        'type'=>'UnsignedInt',
        'path'=>'ElementDefinition.minValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Maximum Allowed Value (for some types)
      # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
      'maxValueDate' => {
        'type'=>'Date',
        'path'=>'ElementDefinition.maxValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Maximum Allowed Value (for some types)
      # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
      'maxValueDateTime' => {
        'type'=>'DateTime',
        'path'=>'ElementDefinition.maxValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Maximum Allowed Value (for some types)
      # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
      'maxValueDecimal' => {
        'type'=>'Decimal',
        'path'=>'ElementDefinition.maxValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Maximum Allowed Value (for some types)
      # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
      'maxValueInstant' => {
        'type'=>'Instant',
        'path'=>'ElementDefinition.maxValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Maximum Allowed Value (for some types)
      # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
      'maxValueInteger' => {
        'type'=>'Integer',
        'path'=>'ElementDefinition.maxValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Maximum Allowed Value (for some types)
      # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
      'maxValuePositiveInt' => {
        'type'=>'PositiveInt',
        'path'=>'ElementDefinition.maxValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Maximum Allowed Value (for some types)
      # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
      'maxValueQuantity' => {
        'type'=>'Quantity',
        'path'=>'ElementDefinition.maxValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Maximum Allowed Value (for some types)
      # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
      'maxValueTime' => {
        'type'=>'Time',
        'path'=>'ElementDefinition.maxValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Maximum Allowed Value (for some types)
      # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
      # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
      'maxValueUnsignedInt' => {
        'type'=>'UnsignedInt',
        'path'=>'ElementDefinition.maxValue[x]',
        'min'=>0,
        'max'=>1
      },
      ##
      # Max length for strings
      # Indicates the maximum length in characters that is permitted to be present in conformant instances and which is expected to be supported by conformant consumers that support the element.
      # Receivers are not required to reject instances that exceed the maximum length.  The full length could be stored.  In some cases, data might be truncated, though truncation should be undertaken with care and an understanding of the consequences of doing so. If not specified, there is no conformance expectation for length support.
      'maxLength' => {
        'type'=>'integer',
        'path'=>'ElementDefinition.maxLength',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reference to invariant about presence
      # A reference to an invariant that may make additional statements about the cardinality or value in the instance.
      'condition' => {
        'type'=>'id',
        'path'=>'ElementDefinition.condition',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Condition that must evaluate to true
      # Formal constraints such as co-occurrence and other constraints that can be computationally evaluated within the context of the instance.
      # Constraints should be declared on the "context" element - the lowest element in the hierarchy that is common to all nodes referenced by the constraint.
      'constraint' => {
        'type'=>'ElementDefinition::Constraint',
        'path'=>'ElementDefinition.constraint',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # If the element must be supported
      # If true, implementations that produce or consume resources SHALL provide "support" for the element in some meaningful way.  If false, the element may be ignored and not supported. If false, whether to populate or use the data element in any way is at the discretion of the implementation.
      # "Something useful" is context dependent and impossible to describe in the base FHIR specification. For this reason, tue mustSupport flag is never set to true by the FHIR specification itself - it is only set to true in profiles.  A profile on a type can always make musSupport = true if it is false in the base type but cannot make mustSupport = false if it is true in the base type.   This is done in [Resource Profiles](profiling.html#mustsupport), where the profile labels an element as mustSupport=true. When a profile does this, it SHALL also make clear exactly what kind of "support" is required, as this can mean many things.    Note that an element that has the property IsModifier is not necessarily a "key" element (e.g. one of the important elements to make use of the resource), nor is it automatically mustSupport - however both of these things are more likely to be true for IsModifier elements than for other elements.
      'mustSupport' => {
        'type'=>'boolean',
        'path'=>'ElementDefinition.mustSupport',
        'min'=>0,
        'max'=>1
      },
      ##
      # If this modifies the meaning of other elements
      # If true, the value of this element affects the interpretation of the element or resource that contains it, and the value of the element cannot be ignored. Typically, this is used for status, negation and qualification codes. The effect of this is that the element cannot be ignored by systems: they SHALL either recognize the element and process it, and/or a pre-determination has been made that it is not relevant to their particular system.
      # Only the definition of an element can set IsModifier true - either the specification itself or where an extension is originally defined. Once set, it cannot be changed in derived profiles. An element/extension that has isModifier=true SHOULD also have a minimum cardinality of 1, so that there is no lack of clarity about what to do if it is missing. If it can be missing, the definition SHALL make the meaning of a missing element clear.
      'isModifier' => {
        'type'=>'boolean',
        'path'=>'ElementDefinition.isModifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # Reason that this element is marked as a modifier
      # Explains how that element affects the interpretation of the resource or element that contains it.
      'isModifierReason' => {
        'type'=>'string',
        'path'=>'ElementDefinition.isModifierReason',
        'min'=>0,
        'max'=>1
      },
      ##
      # Include when _summary = true?
      # Whether the element should be included if a client requests a search with the parameter _summary=true.
      # Some resources include a set of simple metadata, and some very large data. This element is used to reduce the quantity of data returned in searches. Note that servers may pre-cache summarized resources for optimal performance, so servers might not support per-profile use of the isSummary flag. When a request is made with _summary=true, serailisers only include elements marked as 'isSummary = true'. Other than Attachment.data, all data type properties are included in the summary form. In resource and data type definitions, if an element is at the root or has a parent that is 'mustSupport' and the minimum cardinality is 1 or the element is a modifier, it must be marked as isSummary=true.
      'isSummary' => {
        'type'=>'boolean',
        'path'=>'ElementDefinition.isSummary',
        'min'=>0,
        'max'=>1
      },
      ##
      # ValueSet details if this is coded
      # Binds to a value set if this element is coded (code, Coding, CodeableConcept, Quantity), or the data types (string, uri).
      # For a CodeableConcept, when no codes are allowed - only text, use a binding of strength "required" with a description explaining that no coded values are allowed and what sort of information to put in the "text" element.
      'binding' => {
        'type'=>'ElementDefinition::Binding',
        'path'=>'ElementDefinition.binding',
        'min'=>0,
        'max'=>1
      },
      ##
      # Map element to another set of definitions
      # Identifies a concept from an external specification that roughly corresponds to this element.
      # Mappings are not necessarily specific enough for safe translation.
      'mapping' => {
        'type'=>'ElementDefinition::Mapping',
        'path'=>'ElementDefinition.mapping',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # This element is sliced - slices follow
    # Indicates that the element is sliced into a set of alternative definitions (i.e. in a structure definition, there are multiple different constraints on a single element in the base resource). Slicing can be used in any resource that has cardinality ..* on the base resource, or any resource with a choice of types. The set of slices is any elements that come after this in the element sequence that have the same path, until a shorter path occurs (the shorter path terminates the set).
    # The first element in the sequence, the one that carries the slicing, is the definition that applies to all the slices. This is based on the unconstrained element, but can apply any constraints as appropriate. This may include the common constraints on the children of the element.
    class Slicing < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Slicing.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Slicing.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Element values that are used to distinguish the slices
        # Designates which child elements are used to discriminate between the slices when processing an instance. If one or more discriminators are provided, the value of the child elements in the instance data SHALL completely distinguish which slice the element in the resource matches based on the allowed values for those elements in each of the slices.
        # If there is no discriminator, the content is hard to process, so this should be avoided.
        'discriminator' => {
          'type'=>'ElementDefinition::Slicing::Discriminator',
          'path'=>'Slicing.discriminator',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Text description of how slicing works (or not)
        # A human-readable text description of how the slicing works. If there is no discriminator, this is required to be present to provide whatever information is possible about how the slices can be differentiated.
        # If it's really not possible to differentiate them, the design should be re-evaluated to make the content usable.
        'description' => {
          'type'=>'string',
          'path'=>'Slicing.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # If elements must be in same order as slices
        # If the matching elements have to occur in the same order as defined in the profile.
        # Order should only be required when it is a pressing concern for presentation. Profile authors should consider making the order a feature of the rules about the narrative, not the rules about the data - requiring ordered data makes the profile much less re-usable.
        'ordered' => {
          'type'=>'boolean',
          'path'=>'Slicing.ordered',
          'min'=>0,
          'max'=>1
        },
        ##
        # closed | open | openAtEnd
        # Whether additional slices are allowed or not. When the slices are ordered, profile authors can also say that additional slices are only allowed at the end.
        # Allowing additional elements makes for a much for flexible template - it's open for use in wider contexts, but also means that the content of the resource is not closed, and applications have to decide how to handle content not described by the profile.
        'rules' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/resource-slicing-rules'=>[ 'closed', 'open', 'openAtEnd' ]
          },
          'type'=>'code',
          'path'=>'Slicing.rules',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/resource-slicing-rules'}
        }
      }

      ##
      # Element values that are used to distinguish the slices
      # Designates which child elements are used to discriminate between the slices when processing an instance. If one or more discriminators are provided, the value of the child elements in the instance data SHALL completely distinguish which slice the element in the resource matches based on the allowed values for those elements in each of the slices.
      # If there is no discriminator, the content is hard to process, so this should be avoided.
      class Discriminator < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Discriminator.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Discriminator.extension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # value | exists | pattern | type | profile
          # How the element value is interpreted when discrimination is evaluated.
          'type' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/discriminator-type'=>[ 'value', 'exists', 'pattern', 'type', 'profile' ]
            },
            'type'=>'code',
            'path'=>'Discriminator.type',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/discriminator-type'}
          },
          ##
          # Path to element value
          # A FHIRPath expression, using [the simple subset of FHIRPath](fhirpath.html#simple), that is used to identify the element on which discrimination is based.
          # The only FHIRPath functions that are allowed are as(type), resolve(), and extension(url).
          'path' => {
            'type'=>'string',
            'path'=>'Discriminator.path',
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
        # value | exists | pattern | type | profile
        # How the element value is interpreted when discrimination is evaluated.
        attr_accessor :type                           # 1-1 code
        ##
        # Path to element value
        # A FHIRPath expression, using [the simple subset of FHIRPath](fhirpath.html#simple), that is used to identify the element on which discrimination is based.
        # The only FHIRPath functions that are allowed are as(type), resolve(), and extension(url).
        attr_accessor :path                           # 1-1 string
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
      # Element values that are used to distinguish the slices
      # Designates which child elements are used to discriminate between the slices when processing an instance. If one or more discriminators are provided, the value of the child elements in the instance data SHALL completely distinguish which slice the element in the resource matches based on the allowed values for those elements in each of the slices.
      # If there is no discriminator, the content is hard to process, so this should be avoided.
      attr_accessor :discriminator                  # 0-* [ ElementDefinition::Slicing::Discriminator ]
      ##
      # Text description of how slicing works (or not)
      # A human-readable text description of how the slicing works. If there is no discriminator, this is required to be present to provide whatever information is possible about how the slices can be differentiated.
      # If it's really not possible to differentiate them, the design should be re-evaluated to make the content usable.
      attr_accessor :description                    # 0-1 string
      ##
      # If elements must be in same order as slices
      # If the matching elements have to occur in the same order as defined in the profile.
      # Order should only be required when it is a pressing concern for presentation. Profile authors should consider making the order a feature of the rules about the narrative, not the rules about the data - requiring ordered data makes the profile much less re-usable.
      attr_accessor :ordered                        # 0-1 boolean
      ##
      # closed | open | openAtEnd
      # Whether additional slices are allowed or not. When the slices are ordered, profile authors can also say that additional slices are only allowed at the end.
      # Allowing additional elements makes for a much for flexible template - it's open for use in wider contexts, but also means that the content of the resource is not closed, and applications have to decide how to handle content not described by the profile.
      attr_accessor :rules                          # 1-1 code
    end

    ##
    # Base definition information for tools
    # Information about the base definition of the element, provided to make it unnecessary for tools to trace the deviation of the element through the derived and related profiles. When the element definition is not the original definition of an element - i.g. either in a constraint on another type, or for elements from a super type in a snap shot - then the information in provided in the element definition may be different to the base definition. On the original definition of the element, it will be same.
    # The base information does not carry any information that could not be determined from the path and related profiles, but making this determination requires both that the related profiles are available, and that the algorithm to determine them be available. For tooling simplicity, the base information must always be populated in element definitions in snap shots, even if it is the same.
    class Base < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Base.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Base.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Path that identifies the base element
        # The Path that identifies the base element - this matches the ElementDefinition.path for that element. Across FHIR, there is only one base definition of any element - that is, an element definition on a [StructureDefinition](structuredefinition.html#) without a StructureDefinition.base.
        'path' => {
          'type'=>'string',
          'path'=>'Base.path',
          'min'=>1,
          'max'=>1
        },
        ##
        # Min cardinality of the base element
        # Minimum cardinality of the base element identified by the path.
        # This is provided for consistency with max, and may affect code generation of mandatory elements of the base resource are generated differently (some reference implementations have done this).
        'min' => {
          'type'=>'unsignedInt',
          'path'=>'Base.min',
          'min'=>1,
          'max'=>1
        },
        ##
        # Max cardinality of the base element
        # Maximum cardinality of the base element identified by the path.
        # This is provided to code generation, since the serialization representation in JSON differs depending on whether the base element has max > 1. Also, some forms of code generation may differ.
        'max' => {
          'type'=>'string',
          'path'=>'Base.max',
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
      # Path that identifies the base element
      # The Path that identifies the base element - this matches the ElementDefinition.path for that element. Across FHIR, there is only one base definition of any element - that is, an element definition on a [StructureDefinition](structuredefinition.html#) without a StructureDefinition.base.
      attr_accessor :path                           # 1-1 string
      ##
      # Min cardinality of the base element
      # Minimum cardinality of the base element identified by the path.
      # This is provided for consistency with max, and may affect code generation of mandatory elements of the base resource are generated differently (some reference implementations have done this).
      attr_accessor :min                            # 1-1 unsignedInt
      ##
      # Max cardinality of the base element
      # Maximum cardinality of the base element identified by the path.
      # This is provided to code generation, since the serialization representation in JSON differs depending on whether the base element has max > 1. Also, some forms of code generation may differ.
      attr_accessor :max                            # 1-1 string
    end

    ##
    # Data type and Profile for this element
    # The data type or resource that the value of this element is permitted to be.
    # The Type of the element can be left blank in a differential constraint, in which case the type is inherited from the resource. Abstract types are not permitted to appear as a type when multiple types are listed.  (I.e. Abstract types cannot be part of a choice).
    class Type < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Type.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Type.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Data type or Resource (reference to definition)
        # URL of Data type or Resource that is a(or the) type used for this element. References are URLs that are relative to http://hl7.org/fhir/StructureDefinition e.g. "string" is a reference to http://hl7.org/fhir/StructureDefinition/string. Absolute URLs are only allowed in logical models.
        # If the element is a reference to another resource, this element contains "Reference", and the targetProfile element defines what resources can be referenced. The targetProfile may be a reference to the general definition of a resource (e.g. http://hl7.org/fhir/StructureDefinition/Patient).
        'code' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/data-types'=>[ 'Address', 'Age', 'Annotation', 'Attachment', 'BackboneElement', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'Distance', 'Dosage', 'Duration', 'Element', 'ElementDefinition', 'Expression', 'Extension', 'HumanName', 'Identifier', 'MarketingStatus', 'Meta', 'Money', 'MoneyQuantity', 'Narrative', 'ParameterDefinition', 'Period', 'Population', 'ProdCharacteristic', 'ProductShelfLife', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'SimpleQuantity', 'SubstanceAmount', 'Timing', 'TriggerDefinition', 'UsageContext', 'base64Binary', 'boolean', 'canonical', 'code', 'date', 'dateTime', 'decimal', 'id', 'instant', 'integer', 'markdown', 'oid', 'positiveInt', 'string', 'time', 'unsignedInt', 'uri', 'url', 'uuid', 'xhtml' ],
            'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
          },
          'type'=>'uri',
          'path'=>'Type.code',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/defined-types'}
        },
        ##
        # Profiles (StructureDefinition or IG) - one must apply
        # Identifies a profile structure or implementation Guide that applies to the datatype this element refers to. If any profiles are specified, then the content must conform to at least one of them. The URL can be a local reference - to a contained StructureDefinition, or a reference to another StructureDefinition or Implementation Guide by a canonical URL. When an implementation guide is specified, the type SHALL conform to at least one profile defined in the implementation guide.
        # It is possible to profile  backbone element (e.g. part of a resource), using the [profile-element](extension-elementdefinition-profile-element.html) extension.
        'profile' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition', 'http://hl7.org/fhir/StructureDefinition/ImplementationGuide'],
          'type'=>'canonical',
          'path'=>'Type.profile',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Profile (StructureDefinition or IG) on the Reference/canonical target - one must apply
        # Used when the type is "Reference" or "canonical", and identifies a profile structure or implementation Guide that applies to the target of the reference this element refers to. If any profiles are specified, then the content must conform to at least one of them. The URL can be a local reference - to a contained StructureDefinition, or a reference to another StructureDefinition or Implementation Guide by a canonical URL. When an implementation guide is specified, the target resource SHALL conform to at least one profile defined in the implementation guide.
        'targetProfile' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition', 'http://hl7.org/fhir/StructureDefinition/ImplementationGuide'],
          'type'=>'canonical',
          'path'=>'Type.targetProfile',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # contained | referenced | bundled - how aggregated
        # If the type is a reference to another resource, how the resource is or can be aggregated - is it a contained resource, or a reference, and if the context is a bundle, is it included in the bundle.
        # See [Aggregation Rules](elementdefinition.html#aggregation) for further clarification.
        'aggregation' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/resource-aggregation-mode'=>[ 'contained', 'referenced', 'bundled' ]
          },
          'type'=>'code',
          'path'=>'Type.aggregation',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/resource-aggregation-mode'}
        },
        ##
        # either | independent | specific
        # Whether this reference needs to be version specific or version independent, or whether either can be used.
        # The base specification never makes a rule as to which form is allowed, but implementation guides may do this. See [Aggregation Rules](elementdefinition.html#aggregation) for further clarification.
        'versioning' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/reference-version-rules'=>[ 'either', 'independent', 'specific' ]
          },
          'type'=>'code',
          'path'=>'Type.versioning',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/reference-version-rules'}
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
      # Data type or Resource (reference to definition)
      # URL of Data type or Resource that is a(or the) type used for this element. References are URLs that are relative to http://hl7.org/fhir/StructureDefinition e.g. "string" is a reference to http://hl7.org/fhir/StructureDefinition/string. Absolute URLs are only allowed in logical models.
      # If the element is a reference to another resource, this element contains "Reference", and the targetProfile element defines what resources can be referenced. The targetProfile may be a reference to the general definition of a resource (e.g. http://hl7.org/fhir/StructureDefinition/Patient).
      attr_accessor :code                           # 1-1 uri
      ##
      # Profiles (StructureDefinition or IG) - one must apply
      # Identifies a profile structure or implementation Guide that applies to the datatype this element refers to. If any profiles are specified, then the content must conform to at least one of them. The URL can be a local reference - to a contained StructureDefinition, or a reference to another StructureDefinition or Implementation Guide by a canonical URL. When an implementation guide is specified, the type SHALL conform to at least one profile defined in the implementation guide.
      # It is possible to profile  backbone element (e.g. part of a resource), using the [profile-element](extension-elementdefinition-profile-element.html) extension.
      attr_accessor :profile                        # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition|http://hl7.org/fhir/StructureDefinition/ImplementationGuide) ]
      ##
      # Profile (StructureDefinition or IG) on the Reference/canonical target - one must apply
      # Used when the type is "Reference" or "canonical", and identifies a profile structure or implementation Guide that applies to the target of the reference this element refers to. If any profiles are specified, then the content must conform to at least one of them. The URL can be a local reference - to a contained StructureDefinition, or a reference to another StructureDefinition or Implementation Guide by a canonical URL. When an implementation guide is specified, the target resource SHALL conform to at least one profile defined in the implementation guide.
      attr_accessor :targetProfile                  # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition|http://hl7.org/fhir/StructureDefinition/ImplementationGuide) ]
      ##
      # contained | referenced | bundled - how aggregated
      # If the type is a reference to another resource, how the resource is or can be aggregated - is it a contained resource, or a reference, and if the context is a bundle, is it included in the bundle.
      # See [Aggregation Rules](elementdefinition.html#aggregation) for further clarification.
      attr_accessor :aggregation                    # 0-* [ code ]
      ##
      # either | independent | specific
      # Whether this reference needs to be version specific or version independent, or whether either can be used.
      # The base specification never makes a rule as to which form is allowed, but implementation guides may do this. See [Aggregation Rules](elementdefinition.html#aggregation) for further clarification.
      attr_accessor :versioning                     # 0-1 code
    end

    ##
    # Example value (as defined for type)
    # A sample value for this element demonstrating the type of information that would typically be found in the element.
    # Examples will most commonly be present for data where it's not implicitly obvious from either the data type or value set what the values might be.  (I.e. Example values for dates or quantities would generally be unnecessary.)  If the example value is fully populated, the publication tool can generate an instance automatically.
    class Example < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'value[x]' => ['Address', 'Age', 'Annotation', 'Attachment', 'base64Binary', 'boolean', 'canonical', 'code', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'date', 'dateTime', 'decimal', 'Distance', 'Dosage', 'Duration', 'Expression', 'HumanName', 'id', 'Identifier', 'instant', 'integer', 'markdown', 'Meta', 'Money', 'oid', 'ParameterDefinition', 'Period', 'positiveInt', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'string', 'time', 'Timing', 'TriggerDefinition', 'unsignedInt', 'uri', 'url', 'UsageContext', 'uuid']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Example.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Example.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Describes the purpose of this example amoung the set of examples.
        'label' => {
          'type'=>'string',
          'path'=>'Example.label',
          'min'=>1,
          'max'=>1
        },
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueAddress' => {
          'type'=>'Address',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueAge' => {
          'type'=>'Age',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueAnnotation' => {
          'type'=>'Annotation',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueAttachment' => {
          'type'=>'Attachment',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueBase64Binary' => {
          'type'=>'Base64Binary',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueBoolean' => {
          'type'=>'Boolean',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueCanonical' => {
          'type'=>'Canonical',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueCode' => {
          'type'=>'Code',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueCodeableConcept' => {
          'type'=>'CodeableConcept',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueCoding' => {
          'type'=>'Coding',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueContactDetail' => {
          'type'=>'ContactDetail',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueContactPoint' => {
          'type'=>'ContactPoint',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueContributor' => {
          'type'=>'Contributor',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueCount' => {
          'type'=>'Count',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueDataRequirement' => {
          'type'=>'DataRequirement',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueDate' => {
          'type'=>'Date',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueDateTime' => {
          'type'=>'DateTime',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueDecimal' => {
          'type'=>'Decimal',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueDistance' => {
          'type'=>'Distance',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueDosage' => {
          'type'=>'Dosage',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueDuration' => {
          'type'=>'Duration',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueExpression' => {
          'type'=>'Expression',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueHumanName' => {
          'type'=>'HumanName',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueId' => {
          'type'=>'Id',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueIdentifier' => {
          'type'=>'Identifier',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueInstant' => {
          'type'=>'Instant',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueInteger' => {
          'type'=>'Integer',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueMarkdown' => {
          'type'=>'Markdown',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueMeta' => {
          'type'=>'Meta',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueMoney' => {
          'type'=>'Money',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueOid' => {
          'type'=>'Oid',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueParameterDefinition' => {
          'type'=>'ParameterDefinition',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valuePeriod' => {
          'type'=>'Period',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valuePositiveInt' => {
          'type'=>'PositiveInt',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueQuantity' => {
          'type'=>'Quantity',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueRange' => {
          'type'=>'Range',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueRatio' => {
          'type'=>'Ratio',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueReference' => {
          'type'=>'Reference',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueRelatedArtifact' => {
          'type'=>'RelatedArtifact',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueSampledData' => {
          'type'=>'SampledData',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueSignature' => {
          'type'=>'Signature',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueString' => {
          'type'=>'String',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueTime' => {
          'type'=>'Time',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueTiming' => {
          'type'=>'Timing',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueTriggerDefinition' => {
          'type'=>'TriggerDefinition',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueUnsignedInt' => {
          'type'=>'UnsignedInt',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueUri' => {
          'type'=>'Uri',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueUrl' => {
          'type'=>'Url',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueUsageContext' => {
          'type'=>'UsageContext',
          'path'=>'Example.value[x]',
          'min'=>1,
          'max'=>1
        }
        ##
        # Value of Example (one of allowed types)
        # The actual value for the element, which must be one of the types allowed for this element.
        'valueUuid' => {
          'type'=>'Uuid',
          'path'=>'Example.value[x]',
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
      # Describes the purpose of this example amoung the set of examples.
      attr_accessor :label                          # 1-1 string
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueAddress                   # 1-1 Address
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueAge                       # 1-1 Age
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueAnnotation                # 1-1 Annotation
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueAttachment                # 1-1 Attachment
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueBase64Binary              # 1-1 Base64Binary
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueBoolean                   # 1-1 Boolean
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueCanonical                 # 1-1 Canonical
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueCode                      # 1-1 Code
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueCodeableConcept           # 1-1 CodeableConcept
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueCoding                    # 1-1 Coding
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueContactDetail             # 1-1 ContactDetail
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueContactPoint              # 1-1 ContactPoint
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueContributor               # 1-1 Contributor
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueCount                     # 1-1 Count
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueDataRequirement           # 1-1 DataRequirement
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueDate                      # 1-1 Date
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueDateTime                  # 1-1 DateTime
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueDecimal                   # 1-1 Decimal
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueDistance                  # 1-1 Distance
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueDosage                    # 1-1 Dosage
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueDuration                  # 1-1 Duration
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueExpression                # 1-1 Expression
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueHumanName                 # 1-1 HumanName
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueId                        # 1-1 Id
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueIdentifier                # 1-1 Identifier
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueInstant                   # 1-1 Instant
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueInteger                   # 1-1 Integer
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueMarkdown                  # 1-1 Markdown
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueMeta                      # 1-1 Meta
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueMoney                     # 1-1 Money
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueOid                       # 1-1 Oid
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueParameterDefinition       # 1-1 ParameterDefinition
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valuePeriod                    # 1-1 Period
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valuePositiveInt               # 1-1 PositiveInt
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueQuantity                  # 1-1 Quantity
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueRange                     # 1-1 Range
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueRatio                     # 1-1 Ratio
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueReference                 # 1-1 Reference
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueRelatedArtifact           # 1-1 RelatedArtifact
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueSampledData               # 1-1 SampledData
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueSignature                 # 1-1 Signature
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueString                    # 1-1 String
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueTime                      # 1-1 Time
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueTiming                    # 1-1 Timing
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueTriggerDefinition         # 1-1 TriggerDefinition
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueUnsignedInt               # 1-1 UnsignedInt
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueUri                       # 1-1 Uri
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueUrl                       # 1-1 Url
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueUsageContext              # 1-1 UsageContext
      ##
      # Value of Example (one of allowed types)
      # The actual value for the element, which must be one of the types allowed for this element.
      attr_accessor :valueUuid                      # 1-1 Uuid
    end

    ##
    # Condition that must evaluate to true
    # Formal constraints such as co-occurrence and other constraints that can be computationally evaluated within the context of the instance.
    # Constraints should be declared on the "context" element - the lowest element in the hierarchy that is common to all nodes referenced by the constraint.
    class Constraint < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Constraint.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Constraint.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Target of 'condition' reference above
        # Allows identification of which elements have their cardinalities impacted by the constraint.  Will not be referenced for constraints that do not affect cardinality.
        'key' => {
          'type'=>'id',
          'path'=>'Constraint.key',
          'min'=>1,
          'max'=>1
        },
        ##
        # Why this constraint is necessary or appropriate
        # Description of why this constraint is necessary or appropriate.
        # To be used if the reason for the constraint might not be intuitive to all implementers.
        'requirements' => {
          'type'=>'string',
          'path'=>'Constraint.requirements',
          'min'=>0,
          'max'=>1
        },
        ##
        # error | warning
        # Identifies the impact constraint violation has on the conformance of the instance.
        # This allows constraints to be asserted as "shall" (error) and "should" (warning).
        'severity' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/constraint-severity'=>[ 'error', 'warning' ]
          },
          'type'=>'code',
          'path'=>'Constraint.severity',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/constraint-severity'}
        },
        ##
        # Human description of constraint
        # Text that can be used to describe the constraint in messages identifying that the constraint has been violated.
        # Should be expressed in business terms as much as possible.
        'human' => {
          'type'=>'string',
          'path'=>'Constraint.human',
          'min'=>1,
          'max'=>1
        },
        ##
        # FHIRPath expression of constraint
        # A [FHIRPath](fhirpath.html) expression of constraint that can be executed to see if this constraint is met.
        # In the absense of an expression, the expression is likely not enforceable by validators, and might be missed by many systems.
        'expression' => {
          'type'=>'string',
          'path'=>'Constraint.expression',
          'min'=>0,
          'max'=>1
        },
        ##
        # XPath expression of constraint
        # An XPath expression of constraint that can be executed to see if this constraint is met.
        # Elements SHALL use "f" as the namespace prefix for the FHIR namespace, and "x" for the xhtml namespace, and SHALL NOT use any other prefixes.     Note: XPath is generally considered not useful because it does not apply to JSON and other formats and because of XSLT implementation issues, and may be removed in the future.
        'xpath' => {
          'type'=>'string',
          'path'=>'Constraint.xpath',
          'min'=>0,
          'max'=>1
        },
        ##
        # Reference to original source of constraint
        # A reference to the original source of the constraint, for traceability purposes.
        # This is used when, e.g. rendering, where it is not useful to present inherited constraints when rendering the snapshot.
        'source' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
          'type'=>'canonical',
          'path'=>'Constraint.source',
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
      # Target of 'condition' reference above
      # Allows identification of which elements have their cardinalities impacted by the constraint.  Will not be referenced for constraints that do not affect cardinality.
      attr_accessor :key                            # 1-1 id
      ##
      # Why this constraint is necessary or appropriate
      # Description of why this constraint is necessary or appropriate.
      # To be used if the reason for the constraint might not be intuitive to all implementers.
      attr_accessor :requirements                   # 0-1 string
      ##
      # error | warning
      # Identifies the impact constraint violation has on the conformance of the instance.
      # This allows constraints to be asserted as "shall" (error) and "should" (warning).
      attr_accessor :severity                       # 1-1 code
      ##
      # Human description of constraint
      # Text that can be used to describe the constraint in messages identifying that the constraint has been violated.
      # Should be expressed in business terms as much as possible.
      attr_accessor :human                          # 1-1 string
      ##
      # FHIRPath expression of constraint
      # A [FHIRPath](fhirpath.html) expression of constraint that can be executed to see if this constraint is met.
      # In the absense of an expression, the expression is likely not enforceable by validators, and might be missed by many systems.
      attr_accessor :expression                     # 0-1 string
      ##
      # XPath expression of constraint
      # An XPath expression of constraint that can be executed to see if this constraint is met.
      # Elements SHALL use "f" as the namespace prefix for the FHIR namespace, and "x" for the xhtml namespace, and SHALL NOT use any other prefixes.     Note: XPath is generally considered not useful because it does not apply to JSON and other formats and because of XSLT implementation issues, and may be removed in the future.
      attr_accessor :xpath                          # 0-1 string
      ##
      # Reference to original source of constraint
      # A reference to the original source of the constraint, for traceability purposes.
      # This is used when, e.g. rendering, where it is not useful to present inherited constraints when rendering the snapshot.
      attr_accessor :source                         # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition)
    end

    ##
    # ValueSet details if this is coded
    # Binds to a value set if this element is coded (code, Coding, CodeableConcept, Quantity), or the data types (string, uri).
    # For a CodeableConcept, when no codes are allowed - only text, use a binding of strength "required" with a description explaining that no coded values are allowed and what sort of information to put in the "text" element.
    class Binding < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Binding.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Binding.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # required | extensible | preferred | example
        # Indicates the degree of conformance expectations associated with this binding - that is, the degree to which the provided value set must be adhered to in the instances.
        # For further discussion, see [Using Terminologies](terminologies.html).
        'strength' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/binding-strength'=>[ 'required', 'extensible', 'preferred', 'example' ]
          },
          'type'=>'code',
          'path'=>'Binding.strength',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/binding-strength'}
        },
        ##
        # Human explanation of the value set
        # Describes the intended use of this particular set of codes.
        'description' => {
          'type'=>'string',
          'path'=>'Binding.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # Source of value set
        # Refers to the value set that identifies the set of codes the binding refers to.
        # The reference may be version-specific or not (e.g. have a |[version] at the end of the canonical URL).
        'valueSet' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ValueSet'],
          'type'=>'canonical',
          'path'=>'Binding.valueSet',
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
      # required | extensible | preferred | example
      # Indicates the degree of conformance expectations associated with this binding - that is, the degree to which the provided value set must be adhered to in the instances.
      # For further discussion, see [Using Terminologies](terminologies.html).
      attr_accessor :strength                       # 1-1 code
      ##
      # Human explanation of the value set
      # Describes the intended use of this particular set of codes.
      attr_accessor :description                    # 0-1 string
      ##
      # Source of value set
      # Refers to the value set that identifies the set of codes the binding refers to.
      # The reference may be version-specific or not (e.g. have a |[version] at the end of the canonical URL).
      attr_accessor :valueSet                       # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/ValueSet)
    end

    ##
    # Map element to another set of definitions
    # Identifies a concept from an external specification that roughly corresponds to this element.
    # Mappings are not necessarily specific enough for safe translation.
    class Mapping < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Mapping.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Mapping.extension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Reference to mapping declaration
        # An internal reference to the definition of a mapping.
        'identity' => {
          'type'=>'id',
          'path'=>'Mapping.identity',
          'min'=>1,
          'max'=>1
        },
        ##
        # Computable language of mapping
        # Identifies the computable language in which mapping.map is expressed.
        # If omitted, then there can be no expectation of computational interpretation of the mapping.
        'language' => {
          'type'=>'code',
          'path'=>'Mapping.language',
          'min'=>0,
          'max'=>1
        },
        ##
        # Details of the mapping
        # Expresses what part of the target specification corresponds to this element.
        # For most mappings, the syntax is undefined.  Syntax will be provided for mappings to the RIM.  Multiple mappings may be possible and may include constraints on other resource elements that identify when a particular mapping applies.
        'map' => {
          'type'=>'string',
          'path'=>'Mapping.map',
          'min'=>1,
          'max'=>1
        },
        ##
        # Comments about the mapping or its use
        # Comments that provide information about the mapping or its use.
        'comment' => {
          'type'=>'string',
          'path'=>'Mapping.comment',
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
      # Reference to mapping declaration
      # An internal reference to the definition of a mapping.
      attr_accessor :identity                       # 1-1 id
      ##
      # Computable language of mapping
      # Identifies the computable language in which mapping.map is expressed.
      # If omitted, then there can be no expectation of computational interpretation of the mapping.
      attr_accessor :language                       # 0-1 code
      ##
      # Details of the mapping
      # Expresses what part of the target specification corresponds to this element.
      # For most mappings, the syntax is undefined.  Syntax will be provided for mappings to the RIM.  Multiple mappings may be possible and may include constraints on other resource elements that identify when a particular mapping applies.
      attr_accessor :map                            # 1-1 string
      ##
      # Comments about the mapping or its use
      # Comments that provide information about the mapping or its use.
      attr_accessor :comment                        # 0-1 string
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
    # Path of the element in the hierarchy of elements
    # The path identifies the element and is expressed as a "."-separated list of ancestor elements, beginning with the name of the resource or extension.
    attr_accessor :path                           # 1-1 string
    ##
    # xmlAttr | xmlText | typeAttr | cdaText | xhtml
    # Codes that define how this element is represented in instances, when the deviation varies from the normal case.
    # In resources, this is rarely used except for special cases where the representation deviates from the normal, and can only be done in the base standard (and profiles must reproduce what the base standard does). This element is used quite commonly in Logical models when the logical models represent a specific serialization format (e.g. CDA, v2 etc.).
    attr_accessor :representation                 # 0-* [ code ]
    ##
    # Name for this particular element (in a set of slices)
    # The name of this element definition slice, when slicing is working. The name must be a token with no dots or spaces. This is a unique name referring to a specific set of constraints applied to this element, used to provide a name to different slices of the same element.
    # The name SHALL be unique within the structure within the context of the constrained resource element.  (Though to avoid confusion, uniqueness across all elements is recommended.).
    attr_accessor :sliceName                      # 0-1 string
    ##
    # If this slice definition constrains an inherited slice definition (or not)
    # If true, indicates that this slice definition is constraining a slice definition with the same name in an inherited profile. If false, the slice is not overriding any slice in an inherited profile. If missing, the slice might or might not be overriding a slice in an inherited profile, depending on the sliceName.
    # If set to true, an ancestor profile SHALL have a slicing definition with this name.  If set to false, no ancestor profile is permitted to have a slicing definition with this name.
    attr_accessor :sliceIsConstraining            # 0-1 boolean
    ##
    # Name for element to display with or prompt for element
    # A single preferred label which is the text to display beside the element indicating its meaning or to use to prompt for the element in a user display or form.
    # See also the extension (http://hl7.org/fhir/StructureDefinition/elementdefinition-question)[extension-elementdefinition-question.html].
    attr_accessor :label                          # 0-1 string
    ##
    # Corresponding codes in terminologies
    # A code that has the same meaning as the element in a particular terminology.
    # The concept SHALL be properly aligned with the data element definition and other constraints, as defined in the code system, including relationships, of any code listed here.  Where multiple codes exist in a terminology that could correspond to the data element, the most granular code(s) should be selected, so long as they are not more restrictive than the data element itself. The mappings may be used to provide more or less granular or structured equivalences in the code system.
    attr_accessor :code                           # 0-* [ Coding ]
    ##
    # This element is sliced - slices follow
    # Indicates that the element is sliced into a set of alternative definitions (i.e. in a structure definition, there are multiple different constraints on a single element in the base resource). Slicing can be used in any resource that has cardinality ..* on the base resource, or any resource with a choice of types. The set of slices is any elements that come after this in the element sequence that have the same path, until a shorter path occurs (the shorter path terminates the set).
    # The first element in the sequence, the one that carries the slicing, is the definition that applies to all the slices. This is based on the unconstrained element, but can apply any constraints as appropriate. This may include the common constraints on the children of the element.
    attr_accessor :slicing                        # 0-1 ElementDefinition::Slicing
    ##
    # Concise definition for space-constrained presentation
    # A concise description of what this element means (e.g. for use in autogenerated summaries).
    # It is easy for a different short definition to change the meaning of an element and this can have nasty downstream consequences. Please be careful when providing short definitions in a profile.
    attr_accessor :short                          # 0-1 string
    ##
    # Full formal definition as narrative text
    # Provides a complete explanation of the meaning of the data element for human readability.  For the case of elements derived from existing elements (e.g. constraints), the definition SHALL be consistent with the base definition, but convey the meaning of the element in the particular context of use of the resource. (Note: The text you are reading is specified in ElementDefinition.definition).
    # It is easy for a different definition to change the meaning of an element and this can have nasty downstream consequences. Please be careful when providing definitions in a profile.
    attr_accessor :definition                     # 0-1 markdown
    ##
    # Comments about the use of this element
    # Explanatory notes and implementation guidance about the data element, including notes about how to use the data properly, exceptions to proper use, etc. (Note: The text you are reading is specified in ElementDefinition.comment).
    # If it is possible to capture usage rules using constraints, that mechanism should be used in preference to this element.
    attr_accessor :comment                        # 0-1 markdown
    ##
    # Why this resource has been created
    # This element is for traceability of why the element was created and why the constraints exist as they do. This may be used to point to source materials or specifications that drove the structure of this element.
    # This element does not describe the usage of the element (that's done in comments), rather it's for traceability of *why* the element is either needed or why the constraints exist as they do.  This may be used to point to source materials or specifications that drove the structure of this data element.
    attr_accessor :requirements                   # 0-1 markdown
    ##
    # Other names
    # Identifies additional names by which this element might also be known.
    attr_accessor :local_alias                    # 0-* [ string ]
    ##
    # Minimum Cardinality
    # The minimum number of times this element SHALL appear in the instance.
    attr_accessor :min                            # 0-1 unsignedInt
    ##
    # Maximum Cardinality (a number or *)
    # The maximum number of times this element is permitted to appear in the instance.
    attr_accessor :max                            # 0-1 string
    ##
    # Base definition information for tools
    # Information about the base definition of the element, provided to make it unnecessary for tools to trace the deviation of the element through the derived and related profiles. When the element definition is not the original definition of an element - i.g. either in a constraint on another type, or for elements from a super type in a snap shot - then the information in provided in the element definition may be different to the base definition. On the original definition of the element, it will be same.
    # The base information does not carry any information that could not be determined from the path and related profiles, but making this determination requires both that the related profiles are available, and that the algorithm to determine them be available. For tooling simplicity, the base information must always be populated in element definitions in snap shots, even if it is the same.
    attr_accessor :base                           # 0-1 ElementDefinition::Base
    ##
    # Reference to definition of content for the element
    # Identifies an element defined elsewhere in the definition whose content rules should be applied to the current element. ContentReferences bring across all the rules that are in the ElementDefinition for the element, including definitions, cardinality constraints, bindings, invariants etc.
    # ContentReferences can only be defined in specializations, not constrained types, and they cannot be changed and always reference the non-constrained definition.
    attr_accessor :contentReference               # 0-1 uri
    ##
    # Data type and Profile for this element
    # The data type or resource that the value of this element is permitted to be.
    # The Type of the element can be left blank in a differential constraint, in which case the type is inherited from the resource. Abstract types are not permitted to appear as a type when multiple types are listed.  (I.e. Abstract types cannot be part of a choice).
    attr_accessor :type                           # 0-* [ ElementDefinition::Type ]
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueAddress            # 0-1 Address
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueAge                # 0-1 Age
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueAnnotation         # 0-1 Annotation
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueAttachment         # 0-1 Attachment
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueBase64Binary       # 0-1 Base64Binary
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueBoolean            # 0-1 Boolean
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueCanonical          # 0-1 Canonical
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueCode               # 0-1 Code
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueCodeableConcept    # 0-1 CodeableConcept
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueCoding             # 0-1 Coding
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueContactDetail      # 0-1 ContactDetail
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueContactPoint       # 0-1 ContactPoint
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueContributor        # 0-1 Contributor
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueCount              # 0-1 Count
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueDataRequirement    # 0-1 DataRequirement
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueDate               # 0-1 Date
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueDateTime           # 0-1 DateTime
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueDecimal            # 0-1 Decimal
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueDistance           # 0-1 Distance
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueDosage             # 0-1 Dosage
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueDuration           # 0-1 Duration
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueExpression         # 0-1 Expression
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueHumanName          # 0-1 HumanName
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueId                 # 0-1 Id
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueIdentifier         # 0-1 Identifier
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueInstant            # 0-1 Instant
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueInteger            # 0-1 Integer
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueMarkdown           # 0-1 Markdown
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueMeta               # 0-1 Meta
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueMoney              # 0-1 Money
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueOid                # 0-1 Oid
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueParameterDefinition # 0-1 ParameterDefinition
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValuePeriod             # 0-1 Period
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValuePositiveInt        # 0-1 PositiveInt
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueQuantity           # 0-1 Quantity
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueRange              # 0-1 Range
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueRatio              # 0-1 Ratio
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueReference          # 0-1 Reference
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueRelatedArtifact    # 0-1 RelatedArtifact
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueSampledData        # 0-1 SampledData
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueSignature          # 0-1 Signature
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueString             # 0-1 String
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueTime               # 0-1 Time
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueTiming             # 0-1 Timing
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueTriggerDefinition  # 0-1 TriggerDefinition
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueUnsignedInt        # 0-1 UnsignedInt
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueUri                # 0-1 Uri
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueUrl                # 0-1 Url
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueUsageContext       # 0-1 UsageContext
    ##
    # Specified value if missing from instance
    # The value that should be used if there is no value stated in the instance (e.g. 'if not otherwise specified, the abstract is false').
    # Specifying a default value means that the property can never been unknown - it must always have a value. Further, the default value can never be changed, or changed in constraints on content models. Defining default values creates many difficulties in implementation (e.g. when is a value missing?). For these reasons, default values are (and should be) used extremely sparingly. 
    # 
    # No default values are ever defined in the FHIR specification, nor can they be defined in constraints ("profiles") on data types or resources. This element only exists so that default values may be defined in logical models.
    attr_accessor :defaultValueUuid               # 0-1 Uuid
    ##
    # Implicit meaning when this element is missing
    # The Implicit meaning that is to be understood when this element is missing (e.g. 'when this element is missing, the period is ongoing').
    # Implicit meanings for missing values can only be specified on a resource, data type, or extension definition, and never in a profile that applies to one of these. An implicit meaning for a missing value can never be changed, and specifying one has the consequence that constraining its use in profiles eliminates use cases as possibilities, not merely moving them out of scope.
    attr_accessor :meaningWhenMissing             # 0-1 markdown
    ##
    # What the order of the elements means
    # If present, indicates that the order of the repeating element has meaning and describes what that meaning is.  If absent, it means that the order of the element has no meaning.
    # This element can only be asserted on repeating elements and can only be introduced when defining resources or data types.  It can be further refined profiled elements but if absent in the base type, a profile cannot assert meaning.
    attr_accessor :orderMeaning                   # 0-1 string
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedAddress                   # 0-1 Address
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedAge                       # 0-1 Age
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedAnnotation                # 0-1 Annotation
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedAttachment                # 0-1 Attachment
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedBase64Binary              # 0-1 Base64Binary
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedBoolean                   # 0-1 Boolean
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedCanonical                 # 0-1 Canonical
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedCode                      # 0-1 Code
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedCodeableConcept           # 0-1 CodeableConcept
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedCoding                    # 0-1 Coding
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedContactDetail             # 0-1 ContactDetail
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedContactPoint              # 0-1 ContactPoint
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedContributor               # 0-1 Contributor
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedCount                     # 0-1 Count
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedDataRequirement           # 0-1 DataRequirement
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedDate                      # 0-1 Date
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedDateTime                  # 0-1 DateTime
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedDecimal                   # 0-1 Decimal
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedDistance                  # 0-1 Distance
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedDosage                    # 0-1 Dosage
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedDuration                  # 0-1 Duration
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedExpression                # 0-1 Expression
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedHumanName                 # 0-1 HumanName
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedId                        # 0-1 Id
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedIdentifier                # 0-1 Identifier
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedInstant                   # 0-1 Instant
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedInteger                   # 0-1 Integer
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedMarkdown                  # 0-1 Markdown
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedMeta                      # 0-1 Meta
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedMoney                     # 0-1 Money
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedOid                       # 0-1 Oid
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedParameterDefinition       # 0-1 ParameterDefinition
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedPeriod                    # 0-1 Period
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedPositiveInt               # 0-1 PositiveInt
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedQuantity                  # 0-1 Quantity
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedRange                     # 0-1 Range
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedRatio                     # 0-1 Ratio
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedReference                 # 0-1 Reference
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedRelatedArtifact           # 0-1 RelatedArtifact
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedSampledData               # 0-1 SampledData
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedSignature                 # 0-1 Signature
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedString                    # 0-1 String
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedTime                      # 0-1 Time
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedTiming                    # 0-1 Timing
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedTriggerDefinition         # 0-1 TriggerDefinition
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedUnsignedInt               # 0-1 UnsignedInt
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedUri                       # 0-1 Uri
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedUrl                       # 0-1 Url
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedUsageContext              # 0-1 UsageContext
    ##
    # Value must be exactly this
    # Specifies a value that SHALL be exactly the value  for this element in the instance. For purposes of comparison, non-significant whitespace is ignored, and all values must be an exact match (case and accent sensitive). Missing elements/attributes must also be missing.
    # This is not recommended for Coding and CodeableConcept since these often have highly contextual properties such as version or display.
    attr_accessor :fixedUuid                      # 0-1 Uuid
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternAddress                 # 0-1 Address
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternAge                     # 0-1 Age
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternAnnotation              # 0-1 Annotation
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternAttachment              # 0-1 Attachment
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternBase64Binary            # 0-1 Base64Binary
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternBoolean                 # 0-1 Boolean
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternCanonical               # 0-1 Canonical
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternCode                    # 0-1 Code
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternCodeableConcept         # 0-1 CodeableConcept
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternCoding                  # 0-1 Coding
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternContactDetail           # 0-1 ContactDetail
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternContactPoint            # 0-1 ContactPoint
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternContributor             # 0-1 Contributor
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternCount                   # 0-1 Count
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternDataRequirement         # 0-1 DataRequirement
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternDate                    # 0-1 Date
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternDateTime                # 0-1 DateTime
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternDecimal                 # 0-1 Decimal
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternDistance                # 0-1 Distance
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternDosage                  # 0-1 Dosage
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternDuration                # 0-1 Duration
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternExpression              # 0-1 Expression
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternHumanName               # 0-1 HumanName
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternId                      # 0-1 Id
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternIdentifier              # 0-1 Identifier
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternInstant                 # 0-1 Instant
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternInteger                 # 0-1 Integer
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternMarkdown                # 0-1 Markdown
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternMeta                    # 0-1 Meta
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternMoney                   # 0-1 Money
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternOid                     # 0-1 Oid
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternParameterDefinition     # 0-1 ParameterDefinition
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternPeriod                  # 0-1 Period
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternPositiveInt             # 0-1 PositiveInt
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternQuantity                # 0-1 Quantity
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternRange                   # 0-1 Range
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternRatio                   # 0-1 Ratio
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternReference               # 0-1 Reference
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternRelatedArtifact         # 0-1 RelatedArtifact
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternSampledData             # 0-1 SampledData
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternSignature               # 0-1 Signature
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternString                  # 0-1 String
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternTime                    # 0-1 Time
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternTiming                  # 0-1 Timing
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternTriggerDefinition       # 0-1 TriggerDefinition
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternUnsignedInt             # 0-1 UnsignedInt
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternUri                     # 0-1 Uri
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternUrl                     # 0-1 Url
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternUsageContext            # 0-1 UsageContext
    ##
    # Value must have at least these property values
    # Specifies a value that the value in the instance SHALL follow - that is, any value in the pattern must be found in the instance. Other additional values may be found too. This is effectively constraint by example.  
    # 
    # When pattern[x] is used to constrain a primitive, it means that the value provided in the pattern[x] must match the instance value exactly.
    # 
    # When pattern[x] is used to constrain an array, it means that each element provided in the pattern[x] array must (recursively) match at least one element from the instance array.
    # 
    # When pattern[x] is used to constrain a complex object, it means that each property in the pattern must be present in the complex object, and its value must recursively match -- i.e.,
    # 
    # 1. If primitive: it must match exactly the pattern value
    # 2. If a complex object: it must match (recursively) the pattern value
    # 3. If an array: it must match (recursively) the pattern value.
    # Mostly used for fixing values of CodeableConcept. In general, pattern[x] is not intended for use with primitive types, where is has the same meaning as fixed[x].
    attr_accessor :patternUuid                    # 0-1 Uuid
    ##
    # Example value (as defined for type)
    # A sample value for this element demonstrating the type of information that would typically be found in the element.
    # Examples will most commonly be present for data where it's not implicitly obvious from either the data type or value set what the values might be.  (I.e. Example values for dates or quantities would generally be unnecessary.)  If the example value is fully populated, the publication tool can generate an instance automatically.
    attr_accessor :example                        # 0-* [ ElementDefinition::Example ]
    ##
    # Minimum Allowed Value (for some types)
    # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
    attr_accessor :minValueDate                   # 0-1 Date
    ##
    # Minimum Allowed Value (for some types)
    # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
    attr_accessor :minValueDateTime               # 0-1 DateTime
    ##
    # Minimum Allowed Value (for some types)
    # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
    attr_accessor :minValueDecimal                # 0-1 Decimal
    ##
    # Minimum Allowed Value (for some types)
    # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
    attr_accessor :minValueInstant                # 0-1 Instant
    ##
    # Minimum Allowed Value (for some types)
    # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
    attr_accessor :minValueInteger                # 0-1 Integer
    ##
    # Minimum Allowed Value (for some types)
    # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
    attr_accessor :minValuePositiveInt            # 0-1 PositiveInt
    ##
    # Minimum Allowed Value (for some types)
    # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
    attr_accessor :minValueQuantity               # 0-1 Quantity
    ##
    # Minimum Allowed Value (for some types)
    # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
    attr_accessor :minValueTime                   # 0-1 Time
    ##
    # Minimum Allowed Value (for some types)
    # The minimum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the minValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of minValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is subtracted from the current clock to determine the minimum allowable value.   A minimum value for a Quantity is interpreted as an canonical minimum - e.g. you cannot provide 100mg if the minimum value is 10g.
    attr_accessor :minValueUnsignedInt            # 0-1 UnsignedInt
    ##
    # Maximum Allowed Value (for some types)
    # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
    attr_accessor :maxValueDate                   # 0-1 Date
    ##
    # Maximum Allowed Value (for some types)
    # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
    attr_accessor :maxValueDateTime               # 0-1 DateTime
    ##
    # Maximum Allowed Value (for some types)
    # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
    attr_accessor :maxValueDecimal                # 0-1 Decimal
    ##
    # Maximum Allowed Value (for some types)
    # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
    attr_accessor :maxValueInstant                # 0-1 Instant
    ##
    # Maximum Allowed Value (for some types)
    # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
    attr_accessor :maxValueInteger                # 0-1 Integer
    ##
    # Maximum Allowed Value (for some types)
    # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
    attr_accessor :maxValuePositiveInt            # 0-1 PositiveInt
    ##
    # Maximum Allowed Value (for some types)
    # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
    attr_accessor :maxValueQuantity               # 0-1 Quantity
    ##
    # Maximum Allowed Value (for some types)
    # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
    attr_accessor :maxValueTime                   # 0-1 Time
    ##
    # Maximum Allowed Value (for some types)
    # The maximum allowed value for the element. The value is inclusive. This is allowed for the types date, dateTime, instant, time, decimal, integer, and Quantity.
    # Except for date/date/instant, the type of the maxValue[x] SHALL be the same as the specified type of the element. For the date/dateTime/instant values, the type of maxValue[x] SHALL be either the same, or a [Duration](datatypes.html#Duration) which specifies a relative time limit to the current time. The duration value is positive, and is added to the current clock to determine the maximum allowable value.   A maximum value for a Quantity is interpreted as an canonical maximum - e.g. you cannot provide 10g if the maximum value is 50mg.
    attr_accessor :maxValueUnsignedInt            # 0-1 UnsignedInt
    ##
    # Max length for strings
    # Indicates the maximum length in characters that is permitted to be present in conformant instances and which is expected to be supported by conformant consumers that support the element.
    # Receivers are not required to reject instances that exceed the maximum length.  The full length could be stored.  In some cases, data might be truncated, though truncation should be undertaken with care and an understanding of the consequences of doing so. If not specified, there is no conformance expectation for length support.
    attr_accessor :maxLength                      # 0-1 integer
    ##
    # Reference to invariant about presence
    # A reference to an invariant that may make additional statements about the cardinality or value in the instance.
    attr_accessor :condition                      # 0-* [ id ]
    ##
    # Condition that must evaluate to true
    # Formal constraints such as co-occurrence and other constraints that can be computationally evaluated within the context of the instance.
    # Constraints should be declared on the "context" element - the lowest element in the hierarchy that is common to all nodes referenced by the constraint.
    attr_accessor :constraint                     # 0-* [ ElementDefinition::Constraint ]
    ##
    # If the element must be supported
    # If true, implementations that produce or consume resources SHALL provide "support" for the element in some meaningful way.  If false, the element may be ignored and not supported. If false, whether to populate or use the data element in any way is at the discretion of the implementation.
    # "Something useful" is context dependent and impossible to describe in the base FHIR specification. For this reason, tue mustSupport flag is never set to true by the FHIR specification itself - it is only set to true in profiles.  A profile on a type can always make musSupport = true if it is false in the base type but cannot make mustSupport = false if it is true in the base type.   This is done in [Resource Profiles](profiling.html#mustsupport), where the profile labels an element as mustSupport=true. When a profile does this, it SHALL also make clear exactly what kind of "support" is required, as this can mean many things.    Note that an element that has the property IsModifier is not necessarily a "key" element (e.g. one of the important elements to make use of the resource), nor is it automatically mustSupport - however both of these things are more likely to be true for IsModifier elements than for other elements.
    attr_accessor :mustSupport                    # 0-1 boolean
    ##
    # If this modifies the meaning of other elements
    # If true, the value of this element affects the interpretation of the element or resource that contains it, and the value of the element cannot be ignored. Typically, this is used for status, negation and qualification codes. The effect of this is that the element cannot be ignored by systems: they SHALL either recognize the element and process it, and/or a pre-determination has been made that it is not relevant to their particular system.
    # Only the definition of an element can set IsModifier true - either the specification itself or where an extension is originally defined. Once set, it cannot be changed in derived profiles. An element/extension that has isModifier=true SHOULD also have a minimum cardinality of 1, so that there is no lack of clarity about what to do if it is missing. If it can be missing, the definition SHALL make the meaning of a missing element clear.
    attr_accessor :isModifier                     # 0-1 boolean
    ##
    # Reason that this element is marked as a modifier
    # Explains how that element affects the interpretation of the resource or element that contains it.
    attr_accessor :isModifierReason               # 0-1 string
    ##
    # Include when _summary = true?
    # Whether the element should be included if a client requests a search with the parameter _summary=true.
    # Some resources include a set of simple metadata, and some very large data. This element is used to reduce the quantity of data returned in searches. Note that servers may pre-cache summarized resources for optimal performance, so servers might not support per-profile use of the isSummary flag. When a request is made with _summary=true, serailisers only include elements marked as 'isSummary = true'. Other than Attachment.data, all data type properties are included in the summary form. In resource and data type definitions, if an element is at the root or has a parent that is 'mustSupport' and the minimum cardinality is 1 or the element is a modifier, it must be marked as isSummary=true.
    attr_accessor :isSummary                      # 0-1 boolean
    ##
    # ValueSet details if this is coded
    # Binds to a value set if this element is coded (code, Coding, CodeableConcept, Quantity), or the data types (string, uri).
    # For a CodeableConcept, when no codes are allowed - only text, use a binding of strength "required" with a description explaining that no coded values are allowed and what sort of information to put in the "text" element.
    attr_accessor :binding                        # 0-1 ElementDefinition::Binding
    ##
    # Map element to another set of definitions
    # Identifies a concept from an external specification that roughly corresponds to this element.
    # Mappings are not necessarily specific enough for safe translation.
    attr_accessor :mapping                        # 0-* [ ElementDefinition::Mapping ]
  end
end
