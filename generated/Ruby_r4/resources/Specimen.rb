module FHIR

  ##
  # A sample to be used for analysis.
  class Specimen < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['accession', 'bodysite', 'collected', 'collector', 'container-id', 'container', 'identifier', 'parent', 'patient', 'status', 'subject', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Specimen.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Specimen.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Specimen.implicitRules',
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
        'path'=>'Specimen.language',
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
        'path'=>'Specimen.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Specimen.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Specimen.extension',
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
        'path'=>'Specimen.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # External Identifier
      # Id for specimen.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Specimen.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identifier assigned by the lab
      # The identifier assigned by the lab when accessioning specimen(s). This is not necessarily the same as the specimen identifier, depending on local lab procedures.
      'accessionIdentifier' => {
        'type'=>'Identifier',
        'path'=>'Specimen.accessionIdentifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # available | unavailable | unsatisfactory | entered-in-error
      # The availability of the specimen.
      # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/specimen-status'=>[ 'available', 'unavailable', 'unsatisfactory', 'entered-in-error' ]
        },
        'type'=>'code',
        'path'=>'Specimen.status',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/specimen-status'}
      },
      ##
      # Kind of material that forms the specimen
      # The kind of material that forms the specimen.
      # The type can change the way that a specimen is handled and drives what kind of analyses can properly be performed on the specimen. It is frequently used in diagnostic work flow decision making systems.
      'type' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v2-0487'=>[ 'ABS', 'ACNE', 'ACNFLD', 'AIRS', 'ALL', 'AMN', 'AMP', 'ANGI', 'ARTC', 'ASERU', 'ASP', 'ATTE', 'AUTOA', 'AUTOC', 'AUTP', 'BBL', 'BCYST', 'BDY', 'BIFL', 'BITE', 'BLD', 'BLDA', 'BLDCO', 'BLDV', 'BLEB', 'BLIST', 'BOIL', 'BON', 'BOWL', 'BPH', 'BPU', 'BRN', 'BRSH', 'BRTH', 'BRUS', 'BUB', 'BULLA', 'BX', 'CALC', 'CARBU', 'CAT', 'CBITE', 'CDM', 'CLIPP', 'CNJT', 'CNL', 'COL', 'CONE', 'CSCR', 'CSERU', 'CSF', 'CSITE', 'CSMY', 'CST', 'CSVR', 'CTP', 'CUR', 'CVM', 'CVPS', 'CVPT', 'CYN', 'CYST', 'DBITE', 'DCS', 'DEC', 'DEION', 'DIA', 'DIAF', 'DISCHG', 'DIV', 'DRN', 'DRNG', 'DRNGP', 'DUFL', 'EARW', 'EBRUSH', 'EEYE', 'EFF', 'EFFUS', 'EFOD', 'EISO', 'ELT', 'ENVIR', 'EOS', 'EOTH', 'ESOI', 'ESOS', 'ETA', 'ETTP', 'ETTUB', 'EWHI', 'EXG', 'EXS', 'EXUDTE', 'FAW', 'FBLOOD', 'FGA', 'FIB', 'FIST', 'FLD', 'FLT', 'FLU', 'FLUID', 'FOLEY', 'FRS', 'FSCLP', 'FUR', 'GAS', 'GASA', 'GASAN', 'GASBR', 'GASD', 'GAST', 'GENL', 'GENV', 'GRAFT', 'GRAFTS', 'GRANU', 'GROSH', 'GSOL', 'GSPEC', 'GT', 'GTUBE', 'HAR', 'HBITE', 'HBLUD', 'HEMAQ', 'HEMO', 'HERNI', 'HEV', 'HIC', 'HYDC', 'IBITE', 'ICYST', 'IDC', 'IHG', 'ILEO', 'ILLEG', 'IMP', 'INCI', 'INFIL', 'INS', 'INTRD', 'ISLT', 'IT', 'IUD', 'IVCAT', 'IVFLD', 'IVTIP', 'JEJU', 'JNTFLD', 'JP', 'KELOI', 'KIDFLD', 'LAVG', 'LAVGG', 'LAVGP', 'LAVPG', 'LENS1', 'LENS2', 'LESN', 'LIQ', 'LIQO', 'LNA', 'LNV', 'LSAC', 'LYM', 'MAC', 'MAHUR', 'MAR', 'MASS', 'MBLD', 'MEC', 'MILK', 'MLK', 'MUCOS', 'MUCUS', 'NAIL', 'NASDR', 'NEDL', 'NEPH', 'NGASP', 'NGAST', 'NGS', 'NODUL', 'NSECR', 'ORH', 'ORL', 'OTH', 'PACEM', 'PAFL', 'PCFL', 'PDSIT', 'PDTS', 'PELVA', 'PENIL', 'PERIA', 'PILOC', 'PINS', 'PIS', 'PLAN', 'PLAS', 'PLB', 'PLC', 'PLEVS', 'PLR', 'PMN', 'PND', 'POL', 'POPGS', 'POPLG', 'POPLV', 'PORTA', 'PPP', 'PROST', 'PRP', 'PSC', 'PUNCT', 'PUS', 'PUSFR', 'PUST', 'QC3', 'RANDU', 'RBC', 'RBITE', 'RECT', 'RECTA', 'RENALC', 'RENC', 'RES', 'SAL', 'SCAR', 'SCLV', 'SCROA', 'SECRE', 'SER', 'SHU', 'SHUNF', 'SHUNT', 'SITE', 'SKBP', 'SKN', 'SMM', 'SMN', 'SNV', 'SPRM', 'SPRP', 'SPRPB', 'SPS', 'SPT', 'SPTC', 'SPTT', 'SPUT1', 'SPUTIN', 'SPUTSP', 'STER', 'STL', 'STONE', 'SUBMA', 'SUBMX', 'SUMP', 'SUP', 'SUTUR', 'SWGZ', 'SWT', 'TASP', 'TEAR', 'THRB', 'TISS', 'TISU', 'TLC', 'TRAC', 'TRANS', 'TSERU', 'TSTES', 'TTRA', 'TUBES', 'TUMOR', 'TZANC', 'UDENT', 'UMED', 'UR', 'URC', 'URINB', 'URINC', 'URINM', 'URINN', 'URINP', 'URNS', 'URT', 'USCOP', 'USPEC', 'USUB', 'VASTIP', 'VENT', 'VITF', 'VOM', 'WASH', 'WASI', 'WAT', 'WB', 'WBC', 'WEN', 'WICK', 'WND', 'WNDA', 'WNDD', 'WNDE', 'WORM', 'WRT', 'WWA', 'WWO', 'WWT' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Specimen.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0487'}
      },
      ##
      # Where the specimen came from. This may be from patient(s), from a location (e.g., the source of an environmental sample), or a sampling of a substance or a device.
      'subject' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Group', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Substance', 'http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'Specimen.subject',
        'min'=>0,
        'max'=>1
      },
      ##
      # The time when specimen was received for processing
      # Time when specimen was received for processing or testing.
      'receivedTime' => {
        'type'=>'dateTime',
        'path'=>'Specimen.receivedTime',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specimen from which this specimen originated
      # Reference to the parent (source) specimen which is used when the specimen was either derived from or a component of another specimen.
      # The parent specimen could be the source from which the current specimen is derived by some processing step (e.g. an aliquot or isolate or extracted nucleic acids from clinical samples) or one of many specimens that were combined to create a pooled sample.
      'parent' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Specimen'],
        'type'=>'Reference',
        'path'=>'Specimen.parent',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why the specimen was collected
      # Details concerning a service request that required a specimen to be collected.
      # The request may be explicit or implied such with a ServiceRequest that requires a blood draw.
      'request' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ServiceRequest'],
        'type'=>'Reference',
        'path'=>'Specimen.request',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Collection details
      # Details concerning the specimen collection.
      'collection' => {
        'type'=>'Specimen::Collection',
        'path'=>'Specimen.collection',
        'min'=>0,
        'max'=>1
      },
      ##
      # Processing and processing step details
      # Details concerning processing and processing steps for the specimen.
      'processing' => {
        'type'=>'Specimen::Processing',
        'path'=>'Specimen.processing',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Direct container of specimen (tube/slide, etc.)
      # The container holding the specimen.  The recursive nature of containers; i.e. blood in tube in tray in rack is not addressed here.
      'container' => {
        'type'=>'Specimen::Container',
        'path'=>'Specimen.container',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # State of the specimen
      # A mode or state of being that describes the nature of the specimen.
      # Specimen condition is an observation made about the specimen.  It's a point-in-time assessment.  It can be used to assess its quality or appropriateness for a specific test.
      'condition' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v2-0493'=>[ 'AUT', 'CFU', 'CLOT', 'CON', 'COOL', 'FROZ', 'HEM', 'LIVE', 'ROOM', 'SNR' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Specimen.condition',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0493'}
      },
      ##
      # Comments
      # To communicate any details or issues about the specimen or during the specimen collection. (for example: broken vial, sent with patient, frozen).
      'note' => {
        'type'=>'Annotation',
        'path'=>'Specimen.note',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Collection details
    # Details concerning the specimen collection.
    class Collection < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'collected[x]' => ['dateTime', 'Period'],
        'fastingStatus[x]' => ['CodeableConcept', 'Duration']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Collection.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Collection.extension',
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
          'path'=>'Collection.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Who collected the specimen
        # Person who collected the specimen.
        'collector' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
          'type'=>'Reference',
          'path'=>'Collection.collector',
          'min'=>0,
          'max'=>1
        },
        ##
        # Collection time
        # Time when specimen was collected from subject - the physiologically relevant time.
        'collectedDateTime' => {
          'type'=>'DateTime',
          'path'=>'Collection.collected[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # Collection time
        # Time when specimen was collected from subject - the physiologically relevant time.
        'collectedPeriod' => {
          'type'=>'Period',
          'path'=>'Collection.collected[x]',
          'min'=>0,
          'max'=>1
        },
        ##
        # How long it took to collect specimen
        # The span of time over which the collection of a specimen occurred.
        'duration' => {
          'type'=>'Duration',
          'path'=>'Collection.duration',
          'min'=>0,
          'max'=>1
        },
        ##
        # The quantity of specimen collected; for instance the volume of a blood sample, or the physical measurement of an anatomic pathology sample.
        'quantity' => {
          'type'=>'Quantity',
          'path'=>'Collection.quantity',
          'min'=>0,
          'max'=>1
        },
        ##
        # Technique used to perform collection
        # A coded value specifying the technique that is used to perform the procedure.
        'method' => {
          'local_name'=>'local_method'
          'type'=>'CodeableConcept',
          'path'=>'Collection.method',
          'min'=>0,
          'max'=>1
        },
        ##
        # Anatomical collection site
        # Anatomical location from which the specimen was collected (if subject is a patient). This is the target site.  This element is not used for environmental specimens.
        # If the use case requires  BodySite to be handled as a separate resource instead of an inline coded element (e.g. to identify and track separately)  then use the standard extension [bodySite](extension-bodysite.html).
        'bodySite' => {
          'type'=>'CodeableConcept',
          'path'=>'Collection.bodySite',
          'min'=>0,
          'max'=>1
        },
        ##
        # Whether or how long patient abstained from food and/or drink
        # Abstinence or reduction from some or all food, drink, or both, for a period of time prior to sample collection.
        # Representing fasting status using this element is preferred to representing it with an observation using a 'pre-coordinated code'  such as  LOINC 2005-7 (Calcium [Moles/​time] in 2 hour Urine --12 hours fasting), or  using  a component observation ` such as `Observation.component code`  = LOINC 49541-6 (Fasting status - Reported).
        'fastingStatusCodeableConcept' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v2-0916'=>[ 'F', 'NF', 'NG' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Collection.fastingStatus[x]',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0916'}
        }
        ##
        # Whether or how long patient abstained from food and/or drink
        # Abstinence or reduction from some or all food, drink, or both, for a period of time prior to sample collection.
        # Representing fasting status using this element is preferred to representing it with an observation using a 'pre-coordinated code'  such as  LOINC 2005-7 (Calcium [Moles/​time] in 2 hour Urine --12 hours fasting), or  using  a component observation ` such as `Observation.component code`  = LOINC 49541-6 (Fasting status - Reported).
        'fastingStatusDuration' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v2-0916'=>[ 'F', 'NF', 'NG' ]
          },
          'type'=>'Duration',
          'path'=>'Collection.fastingStatus[x]',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0916'}
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
      # Who collected the specimen
      # Person who collected the specimen.
      attr_accessor :collector                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
      ##
      # Collection time
      # Time when specimen was collected from subject - the physiologically relevant time.
      attr_accessor :collectedDateTime              # 0-1 DateTime
      ##
      # Collection time
      # Time when specimen was collected from subject - the physiologically relevant time.
      attr_accessor :collectedPeriod                # 0-1 Period
      ##
      # How long it took to collect specimen
      # The span of time over which the collection of a specimen occurred.
      attr_accessor :duration                       # 0-1 Duration
      ##
      # The quantity of specimen collected; for instance the volume of a blood sample, or the physical measurement of an anatomic pathology sample.
      attr_accessor :quantity                       # 0-1 Quantity
      ##
      # Technique used to perform collection
      # A coded value specifying the technique that is used to perform the procedure.
      attr_accessor :local_method                   # 0-1 CodeableConcept
      ##
      # Anatomical collection site
      # Anatomical location from which the specimen was collected (if subject is a patient). This is the target site.  This element is not used for environmental specimens.
      # If the use case requires  BodySite to be handled as a separate resource instead of an inline coded element (e.g. to identify and track separately)  then use the standard extension [bodySite](extension-bodysite.html).
      attr_accessor :bodySite                       # 0-1 CodeableConcept
      ##
      # Whether or how long patient abstained from food and/or drink
      # Abstinence or reduction from some or all food, drink, or both, for a period of time prior to sample collection.
      # Representing fasting status using this element is preferred to representing it with an observation using a 'pre-coordinated code'  such as  LOINC 2005-7 (Calcium [Moles/​time] in 2 hour Urine --12 hours fasting), or  using  a component observation ` such as `Observation.component code`  = LOINC 49541-6 (Fasting status - Reported).
      attr_accessor :fastingStatusCodeableConcept   # 0-1 CodeableConcept
      ##
      # Whether or how long patient abstained from food and/or drink
      # Abstinence or reduction from some or all food, drink, or both, for a period of time prior to sample collection.
      # Representing fasting status using this element is preferred to representing it with an observation using a 'pre-coordinated code'  such as  LOINC 2005-7 (Calcium [Moles/​time] in 2 hour Urine --12 hours fasting), or  using  a component observation ` such as `Observation.component code`  = LOINC 49541-6 (Fasting status - Reported).
      attr_accessor :fastingStatusDuration          # 0-1 Duration
    end

    ##
    # Processing and processing step details
    # Details concerning processing and processing steps for the specimen.
    class Processing < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'time[x]' => ['dateTime', 'Period']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Processing.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Processing.extension',
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
          'path'=>'Processing.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Textual description of procedure.
        'description' => {
          'type'=>'string',
          'path'=>'Processing.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # Indicates the treatment step  applied to the specimen
        # A coded value specifying the procedure used to process the specimen.
        'procedure' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v2-0373'=>[ 'ACID', 'ALK', 'DEFB', 'FILT', 'LDLP', 'NEUT', 'RECA', 'UFIL' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Processing.procedure',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/specimen-processing-procedure'}
        },
        ##
        # Material used in the processing step.
        'additive' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Substance'],
          'type'=>'Reference',
          'path'=>'Processing.additive',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Date and time of specimen processing
        # A record of the time or period when the specimen processing occurred.  For example the time of sample fixation or the period of time the sample was in formalin.
        'timeDateTime' => {
          'type'=>'DateTime',
          'path'=>'Processing.time[x]',
          'min'=>0,
          'max'=>1
        }
        ##
        # Date and time of specimen processing
        # A record of the time or period when the specimen processing occurred.  For example the time of sample fixation or the period of time the sample was in formalin.
        'timePeriod' => {
          'type'=>'Period',
          'path'=>'Processing.time[x]',
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
      # Textual description of procedure.
      attr_accessor :description                    # 0-1 string
      ##
      # Indicates the treatment step  applied to the specimen
      # A coded value specifying the procedure used to process the specimen.
      attr_accessor :procedure                      # 0-1 CodeableConcept
      ##
      # Material used in the processing step.
      attr_accessor :additive                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Substance) ]
      ##
      # Date and time of specimen processing
      # A record of the time or period when the specimen processing occurred.  For example the time of sample fixation or the period of time the sample was in formalin.
      attr_accessor :timeDateTime                   # 0-1 DateTime
      ##
      # Date and time of specimen processing
      # A record of the time or period when the specimen processing occurred.  For example the time of sample fixation or the period of time the sample was in formalin.
      attr_accessor :timePeriod                     # 0-1 Period
    end

    ##
    # Direct container of specimen (tube/slide, etc.)
    # The container holding the specimen.  The recursive nature of containers; i.e. blood in tube in tray in rack is not addressed here.
    class Container < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      MULTIPLE_TYPES = {
        'additive[x]' => ['CodeableConcept', 'Reference']
      }
      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Container.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Container.extension',
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
          'path'=>'Container.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Id for the container
        # Id for container. There may be multiple; a manufacturer's bar code, lab assigned identifier, etc. The container ID may differ from the specimen id in some circumstances.
        'identifier' => {
          'type'=>'Identifier',
          'path'=>'Container.identifier',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Textual description of the container.
        'description' => {
          'type'=>'string',
          'path'=>'Container.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # Kind of container directly associated with specimen
        # The type of container associated with the specimen (e.g. slide, aliquot, etc.).
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Container.type',
          'min'=>0,
          'max'=>1
        },
        ##
        # Container volume or size
        # The capacity (volume or other measure) the container may contain.
        'capacity' => {
          'type'=>'Quantity',
          'path'=>'Container.capacity',
          'min'=>0,
          'max'=>1
        },
        ##
        # Quantity of specimen within container
        # The quantity of specimen in the container; may be volume, dimensions, or other appropriate measurements, depending on the specimen type.
        'specimenQuantity' => {
          'type'=>'Quantity',
          'path'=>'Container.specimenQuantity',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additive associated with container
        # Introduced substance to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
        'additiveCodeableConcept' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v2-0371'=>[ 'ACDA', 'ACDB', 'ACET', 'AMIES', 'BACTM', 'BF10', 'BOR', 'BOUIN', 'BSKM', 'C32', 'C38', 'CARS', 'CARY', 'CHLTM', 'CTAD', 'EDTK', 'EDTK15', 'EDTK75', 'EDTN', 'ENT', 'ENT+', 'F10', 'FDP', 'FL10', 'FL100', 'HCL6', 'HEPA', 'HEPL', 'HEPN', 'HNO3', 'JKM', 'KARN', 'KOX', 'LIA', 'M4', 'M4RT', 'M5', 'MICHTM', 'MMDTM', 'NAF', 'NAPS', 'NONE', 'PAGE', 'PHENOL', 'PVA', 'RLM', 'SILICA', 'SPS', 'SST', 'STUTM', 'THROM', 'THYMOL', 'THYO', 'TOLU', 'URETM', 'VIRTM', 'WEST' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Container.additive[x]',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0371'}
        }
        ##
        # Additive associated with container
        # Introduced substance to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
        'additiveReference' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v2-0371'=>[ 'ACDA', 'ACDB', 'ACET', 'AMIES', 'BACTM', 'BF10', 'BOR', 'BOUIN', 'BSKM', 'C32', 'C38', 'CARS', 'CARY', 'CHLTM', 'CTAD', 'EDTK', 'EDTK15', 'EDTK75', 'EDTN', 'ENT', 'ENT+', 'F10', 'FDP', 'FL10', 'FL100', 'HCL6', 'HEPA', 'HEPL', 'HEPN', 'HNO3', 'JKM', 'KARN', 'KOX', 'LIA', 'M4', 'M4RT', 'M5', 'MICHTM', 'MMDTM', 'NAF', 'NAPS', 'NONE', 'PAGE', 'PHENOL', 'PVA', 'RLM', 'SILICA', 'SPS', 'SST', 'STUTM', 'THROM', 'THYMOL', 'THYO', 'TOLU', 'URETM', 'VIRTM', 'WEST' ]
          },
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Substance'],
          'type'=>'Reference',
          'path'=>'Container.additive[x]',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0371'}
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
      # Id for the container
      # Id for container. There may be multiple; a manufacturer's bar code, lab assigned identifier, etc. The container ID may differ from the specimen id in some circumstances.
      attr_accessor :identifier                     # 0-* [ Identifier ]
      ##
      # Textual description of the container.
      attr_accessor :description                    # 0-1 string
      ##
      # Kind of container directly associated with specimen
      # The type of container associated with the specimen (e.g. slide, aliquot, etc.).
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Container volume or size
      # The capacity (volume or other measure) the container may contain.
      attr_accessor :capacity                       # 0-1 Quantity
      ##
      # Quantity of specimen within container
      # The quantity of specimen in the container; may be volume, dimensions, or other appropriate measurements, depending on the specimen type.
      attr_accessor :specimenQuantity               # 0-1 Quantity
      ##
      # Additive associated with container
      # Introduced substance to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
      attr_accessor :additiveCodeableConcept        # 0-1 CodeableConcept
      ##
      # Additive associated with container
      # Introduced substance to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
      attr_accessor :additiveReference              # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Substance)
    end
    ##
    # Logical id of this artifact
    # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
    # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
    attr_accessor :id                             # 0-1 id
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
    # External Identifier
    # Id for specimen.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Identifier assigned by the lab
    # The identifier assigned by the lab when accessioning specimen(s). This is not necessarily the same as the specimen identifier, depending on local lab procedures.
    attr_accessor :accessionIdentifier            # 0-1 Identifier
    ##
    # available | unavailable | unsatisfactory | entered-in-error
    # The availability of the specimen.
    # This element is labeled as a modifier because the status contains codes that mark the resource as not currently valid.
    attr_accessor :status                         # 0-1 code
    ##
    # Kind of material that forms the specimen
    # The kind of material that forms the specimen.
    # The type can change the way that a specimen is handled and drives what kind of analyses can properly be performed on the specimen. It is frequently used in diagnostic work flow decision making systems.
    attr_accessor :type                           # 0-1 CodeableConcept
    ##
    # Where the specimen came from. This may be from patient(s), from a location (e.g., the source of an environmental sample), or a sampling of a substance or a device.
    attr_accessor :subject                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Group|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Substance|http://hl7.org/fhir/StructureDefinition/Location)
    ##
    # The time when specimen was received for processing
    # Time when specimen was received for processing or testing.
    attr_accessor :receivedTime                   # 0-1 dateTime
    ##
    # Specimen from which this specimen originated
    # Reference to the parent (source) specimen which is used when the specimen was either derived from or a component of another specimen.
    # The parent specimen could be the source from which the current specimen is derived by some processing step (e.g. an aliquot or isolate or extracted nucleic acids from clinical samples) or one of many specimens that were combined to create a pooled sample.
    attr_accessor :parent                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Specimen) ]
    ##
    # Why the specimen was collected
    # Details concerning a service request that required a specimen to be collected.
    # The request may be explicit or implied such with a ServiceRequest that requires a blood draw.
    attr_accessor :request                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ServiceRequest) ]
    ##
    # Collection details
    # Details concerning the specimen collection.
    attr_accessor :collection                     # 0-1 Specimen::Collection
    ##
    # Processing and processing step details
    # Details concerning processing and processing steps for the specimen.
    attr_accessor :processing                     # 0-* [ Specimen::Processing ]
    ##
    # Direct container of specimen (tube/slide, etc.)
    # The container holding the specimen.  The recursive nature of containers; i.e. blood in tube in tray in rack is not addressed here.
    attr_accessor :container                      # 0-* [ Specimen::Container ]
    ##
    # State of the specimen
    # A mode or state of being that describes the nature of the specimen.
    # Specimen condition is an observation made about the specimen.  It's a point-in-time assessment.  It can be used to assess its quality or appropriateness for a specific test.
    attr_accessor :condition                      # 0-* [ CodeableConcept ]
    ##
    # Comments
    # To communicate any details or issues about the specimen or during the specimen collection. (for example: broken vial, sent with patient, frozen).
    attr_accessor :note                           # 0-* [ Annotation ]

    def resourceType
      'Specimen'
    end
  end
end
