module FHIR

  ##
  # A list is a curated collection of resources.
  class List < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['code', 'date', 'empty-reason', 'encounter', 'identifier', 'item', 'notes', 'patient', 'source', 'status', 'subject', 'title']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'List.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'List.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'List.implicitRules',
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
        'path'=>'List.language',
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
        'path'=>'List.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'List.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'List.extension',
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
        'path'=>'List.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business identifier
      # Identifier for the List assigned for business purposes outside the context of FHIR.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'List.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # current | retired | entered-in-error
      # Indicates the current state of this list.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/list-status'=>[ 'current', 'retired', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'List.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/list-status'}
      },
      ##
      # working | snapshot | changes
      # How this list was prepared - whether it is a working list that is suitable for being maintained on an ongoing basis, or if it represents a snapshot of a list of items from another source, or whether it is a prepared list where items may be marked as added, modified or deleted.
      # This element is labeled as a modifier because a change list must not be misunderstood as a complete list.
      'mode' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/list-mode'=>[ 'working', 'snapshot', 'changes' ]
        },
        'type'=>'code',
        'path'=>'List.mode',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/list-mode'}
      },
      ##
      # Descriptive name for the list
      # A label for the list assigned by the author.
      'title' => {
        'type'=>'string',
        'path'=>'List.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # What the purpose of this list is
      # This code defines the purpose of the list - why it was created.
      # If there is no code, the purpose of the list is implied where it is used, such as in a document section using Document.section.code.
      'code' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/list-example-use-codes'=>[ 'alerts', 'adverserxns', 'allergies', 'medications', 'problems', 'worklist', 'waiting', 'protocols', 'plans' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'List.code',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/list-example-codes'}
      },
      ##
      # If all resources have the same subject
      # The common subject (or patient) of the resources that are in the list if there is one.
      # Some purely arbitrary lists do not have a common subject, so this is optional.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'List.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # Context in which list created
      # The encounter that is the context in which this list was created.
      'encounter' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Encounter'],
        'type'=>'Reference',
        'path'=>'List.encounter',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the list was prepared
      # The date that the list was prepared.
      # The actual important date is the date of currency of the resources that were summarized, but it is usually assumed that these are current when the preparation occurs.
      'date' => {
        'type'=>'dateTime',
        'path'=>'List.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who and/or what defined the list contents (aka Author)
      # The entity responsible for deciding what the contents of the list were. Where the list was created by a human, this is the same as the author of the list.
      # The primary source is the entity that made the decisions what items are in the list. This may be software or user.
      'source' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'List.source',
        'min'=>0,
        'max'=>1
      },
      ##
      # What order the list has
      # What order applies to the items in the list.
      # Applications SHOULD render ordered lists in the order provided, but MAY allow users to re-order based on their own preferences as well. If there is no order specified, the order is unknown, though there may still be some order.
      'orderedBy' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/list-order'=>[ 'user', 'system', 'event-date', 'entry-date', 'priority', 'alphabetic', 'category', 'patient' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'List.orderedBy',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/list-order'}
      },
      ##
      # Comments about the list
      # Comments that apply to the overall list.
      'note' => {
        'type'=>'Annotation',
        'path'=>'List.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Entries in the list
      # Entries in this list.
      # If there are no entries in the list, an emptyReason SHOULD be provided.
      'entry' => {
        'type'=>'List::Entry',
        'path'=>'List.entry',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why list is empty
      # If the list is empty, why the list is empty.
      # The various reasons for an empty list make a significant interpretation to its interpretation. Note that this code is for use when the entire list has been suppressed, and not for when individual items are omitted - implementers may consider using a text note or a flag on an entry in these cases.
      'emptyReason' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/list-empty-reason'=>[ 'nilknown', 'notasked', 'withheld', 'unavailable', 'notstarted', 'closed' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'List.emptyReason',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/list-empty-reason'}
      }
    }

    ##
    # Entries in the list
    # Entries in this list.
    # If there are no entries in the list, an emptyReason SHOULD be provided.
    class Entry < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Entry.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Entry.extension',
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
          'path'=>'Entry.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Status/Workflow information about this item
        # The flag allows the system constructing the list to indicate the role and significance of the item in the list.
        # The flag can only be understood in the context of the List.code. If the flag means that the entry has actually been deleted from the list, the deleted element SHALL be true. Deleted can only be used if the List.mode is "changes".
        'flag' => {
          'valid_codes'=>{
            'urn:oid:1.2.36.1.2001.1001.101.104.16592'=>[ '01', '02', '03', '04', '05', '06' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Entry.flag',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/list-item-flag'}
        },
        ##
        # If this item is actually marked as deleted
        # True if this item is marked as deleted in the list.
        # If the flag means that the entry has actually been deleted from the list, the deleted element SHALL be true. Both flag and deleted can only be used if the List.mode is "changes". A deleted entry should be displayed in narrative as deleted.  This element is labeled as a modifier because it indicates that an item is (to be) no longer in the list.
        'deleted' => {
          'type'=>'boolean',
          'path'=>'Entry.deleted',
          'min'=>0,
          'max'=>1
        },
        ##
        # When item added to list
        # When this item was added to the list.
        'date' => {
          'type'=>'dateTime',
          'path'=>'Entry.date',
          'min'=>0,
          'max'=>1
        },
        ##
        # Actual entry
        # A reference to the actual resource from which data was derived.
        'item' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Entry.item',
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
      # Status/Workflow information about this item
      # The flag allows the system constructing the list to indicate the role and significance of the item in the list.
      # The flag can only be understood in the context of the List.code. If the flag means that the entry has actually been deleted from the list, the deleted element SHALL be true. Deleted can only be used if the List.mode is "changes".
      attr_accessor :flag                           # 0-1 CodeableConcept
      ##
      # If this item is actually marked as deleted
      # True if this item is marked as deleted in the list.
      # If the flag means that the entry has actually been deleted from the list, the deleted element SHALL be true. Both flag and deleted can only be used if the List.mode is "changes". A deleted entry should be displayed in narrative as deleted.  This element is labeled as a modifier because it indicates that an item is (to be) no longer in the list.
      attr_accessor :deleted                        # 0-1 boolean
      ##
      # When item added to list
      # When this item was added to the list.
      attr_accessor :date                           # 0-1 dateTime
      ##
      # Actual entry
      # A reference to the actual resource from which data was derived.
      attr_accessor :item                           # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
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
    # Business identifier
    # Identifier for the List assigned for business purposes outside the context of FHIR.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # current | retired | entered-in-error
    # Indicates the current state of this list.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # working | snapshot | changes
    # How this list was prepared - whether it is a working list that is suitable for being maintained on an ongoing basis, or if it represents a snapshot of a list of items from another source, or whether it is a prepared list where items may be marked as added, modified or deleted.
    # This element is labeled as a modifier because a change list must not be misunderstood as a complete list.
    attr_accessor :mode                           # 1-1 code
    ##
    # Descriptive name for the list
    # A label for the list assigned by the author.
    attr_accessor :title                          # 0-1 string
    ##
    # What the purpose of this list is
    # This code defines the purpose of the list - why it was created.
    # If there is no code, the purpose of the list is implied where it is used, such as in a document section using Document.section.code.
    attr_accessor :code                           # 0-1 CodeableConcept
    ##
    # If all resources have the same subject
    # The common subject (or patient) of the resources that are in the list if there is one.
    # Some purely arbitrary lists do not have a common subject, so this is optional.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # Context in which list created
    # The encounter that is the context in which this list was created.
    attr_accessor :encounter                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Encounter)
    ##
    # When the list was prepared
    # The date that the list was prepared.
    # The actual important date is the date of currency of the resources that were summarized, but it is usually assumed that these are current when the preparation occurs.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Who and/or what defined the list contents (aka Author)
    # The entity responsible for deciding what the contents of the list were. Where the list was created by a human, this is the same as the author of the list.
    # The primary source is the entity that made the decisions what items are in the list. This may be software or user.
    attr_accessor :source                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # What order the list has
    # What order applies to the items in the list.
    # Applications SHOULD render ordered lists in the order provided, but MAY allow users to re-order based on their own preferences as well. If there is no order specified, the order is unknown, though there may still be some order.
    attr_accessor :orderedBy                      # 0-1 CodeableConcept
    ##
    # Comments about the list
    # Comments that apply to the overall list.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Entries in the list
    # Entries in this list.
    # If there are no entries in the list, an emptyReason SHOULD be provided.
    attr_accessor :entry                          # 0-* [ List::Entry ]
    ##
    # Why list is empty
    # If the list is empty, why the list is empty.
    # The various reasons for an empty list make a significant interpretation to its interpretation. Note that this code is for use when the entire list has been suppressed, and not for when individual items are omitted - implementers may consider using a text note or a flag on an entry in these cases.
    attr_accessor :emptyReason                    # 0-1 CodeableConcept

    def resourceType
      'List'
    end
  end
end
