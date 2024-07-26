module FHIR

  ##
  # A kind of specimen with associated set of requirements.
  class SpecimenDefinition < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['container', 'identifier', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'SpecimenDefinition.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'SpecimenDefinition.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'SpecimenDefinition.implicitRules',
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
        'path'=>'SpecimenDefinition.language',
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
        'path'=>'SpecimenDefinition.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'SpecimenDefinition.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'SpecimenDefinition.extension',
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
        'path'=>'SpecimenDefinition.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business identifier of a kind of specimen
      # A business identifier associated with the kind of specimen.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'SpecimenDefinition.identifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # Kind of material to collect
      # The kind of material to be collected.
      'typeCollected' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v2-0487'=>[ 'ABS', 'ACNE', 'ACNFLD', 'AIRS', 'ALL', 'AMN', 'AMP', 'ANGI', 'ARTC', 'ASERU', 'ASP', 'ATTE', 'AUTOA', 'AUTOC', 'AUTP', 'BBL', 'BCYST', 'BDY', 'BIFL', 'BITE', 'BLD', 'BLDA', 'BLDCO', 'BLDV', 'BLEB', 'BLIST', 'BOIL', 'BON', 'BOWL', 'BPH', 'BPU', 'BRN', 'BRSH', 'BRTH', 'BRUS', 'BUB', 'BULLA', 'BX', 'CALC', 'CARBU', 'CAT', 'CBITE', 'CDM', 'CLIPP', 'CNJT', 'CNL', 'COL', 'CONE', 'CSCR', 'CSERU', 'CSF', 'CSITE', 'CSMY', 'CST', 'CSVR', 'CTP', 'CUR', 'CVM', 'CVPS', 'CVPT', 'CYN', 'CYST', 'DBITE', 'DCS', 'DEC', 'DEION', 'DIA', 'DIAF', 'DISCHG', 'DIV', 'DRN', 'DRNG', 'DRNGP', 'DUFL', 'EARW', 'EBRUSH', 'EEYE', 'EFF', 'EFFUS', 'EFOD', 'EISO', 'ELT', 'ENVIR', 'EOS', 'EOTH', 'ESOI', 'ESOS', 'ETA', 'ETTP', 'ETTUB', 'EWHI', 'EXG', 'EXS', 'EXUDTE', 'FAW', 'FBLOOD', 'FGA', 'FIB', 'FIST', 'FLD', 'FLT', 'FLU', 'FLUID', 'FOLEY', 'FRS', 'FSCLP', 'FUR', 'GAS', 'GASA', 'GASAN', 'GASBR', 'GASD', 'GAST', 'GENL', 'GENV', 'GRAFT', 'GRAFTS', 'GRANU', 'GROSH', 'GSOL', 'GSPEC', 'GT', 'GTUBE', 'HAR', 'HBITE', 'HBLUD', 'HEMAQ', 'HEMO', 'HERNI', 'HEV', 'HIC', 'HYDC', 'IBITE', 'ICYST', 'IDC', 'IHG', 'ILEO', 'ILLEG', 'IMP', 'INCI', 'INFIL', 'INS', 'INTRD', 'ISLT', 'IT', 'IUD', 'IVCAT', 'IVFLD', 'IVTIP', 'JEJU', 'JNTFLD', 'JP', 'KELOI', 'KIDFLD', 'LAVG', 'LAVGG', 'LAVGP', 'LAVPG', 'LENS1', 'LENS2', 'LESN', 'LIQ', 'LIQO', 'LNA', 'LNV', 'LSAC', 'LYM', 'MAC', 'MAHUR', 'MAR', 'MASS', 'MBLD', 'MEC', 'MILK', 'MLK', 'MUCOS', 'MUCUS', 'NAIL', 'NASDR', 'NEDL', 'NEPH', 'NGASP', 'NGAST', 'NGS', 'NODUL', 'NSECR', 'ORH', 'ORL', 'OTH', 'PACEM', 'PAFL', 'PCFL', 'PDSIT', 'PDTS', 'PELVA', 'PENIL', 'PERIA', 'PILOC', 'PINS', 'PIS', 'PLAN', 'PLAS', 'PLB', 'PLC', 'PLEVS', 'PLR', 'PMN', 'PND', 'POL', 'POPGS', 'POPLG', 'POPLV', 'PORTA', 'PPP', 'PROST', 'PRP', 'PSC', 'PUNCT', 'PUS', 'PUSFR', 'PUST', 'QC3', 'RANDU', 'RBC', 'RBITE', 'RECT', 'RECTA', 'RENALC', 'RENC', 'RES', 'SAL', 'SCAR', 'SCLV', 'SCROA', 'SECRE', 'SER', 'SHU', 'SHUNF', 'SHUNT', 'SITE', 'SKBP', 'SKN', 'SMM', 'SMN', 'SNV', 'SPRM', 'SPRP', 'SPRPB', 'SPS', 'SPT', 'SPTC', 'SPTT', 'SPUT1', 'SPUTIN', 'SPUTSP', 'STER', 'STL', 'STONE', 'SUBMA', 'SUBMX', 'SUMP', 'SUP', 'SUTUR', 'SWGZ', 'SWT', 'TASP', 'TEAR', 'THRB', 'TISS', 'TISU', 'TLC', 'TRAC', 'TRANS', 'TSERU', 'TSTES', 'TTRA', 'TUBES', 'TUMOR', 'TZANC', 'UDENT', 'UMED', 'UR', 'URC', 'URINB', 'URINC', 'URINM', 'URINN', 'URINP', 'URNS', 'URT', 'USCOP', 'USPEC', 'USUB', 'VASTIP', 'VENT', 'VITF', 'VOM', 'WASH', 'WASI', 'WAT', 'WB', 'WBC', 'WEN', 'WICK', 'WND', 'WNDA', 'WNDD', 'WNDE', 'WORM', 'WRT', 'WWA', 'WWO', 'WWT' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'SpecimenDefinition.typeCollected',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0487'}
      },
      ##
      # Patient preparation for collection
      # Preparation of the patient for specimen collection.
      'patientPreparation' => {
        'type'=>'CodeableConcept',
        'path'=>'SpecimenDefinition.patientPreparation',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Time aspect for collection
      # Time aspect of specimen collection (duration or offset).
      'timeAspect' => {
        'type'=>'string',
        'path'=>'SpecimenDefinition.timeAspect',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specimen collection procedure
      # The action to be performed for collecting the specimen.
      'collection' => {
        'type'=>'CodeableConcept',
        'path'=>'SpecimenDefinition.collection',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Specimen in container intended for testing by lab
      # Specimen conditioned in a container as expected by the testing laboratory.
      'typeTested' => {
        'type'=>'SpecimenDefinition::TypeTested',
        'path'=>'SpecimenDefinition.typeTested',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Specimen in container intended for testing by lab
    # Specimen conditioned in a container as expected by the testing laboratory.
    class TypeTested < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'TypeTested.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'TypeTested.extension',
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
          'path'=>'TypeTested.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Primary or secondary specimen
        # Primary of secondary specimen.
        'isDerived' => {
          'type'=>'boolean',
          'path'=>'TypeTested.isDerived',
          'min'=>0,
          'max'=>1
        },
        ##
        # Type of intended specimen
        # The kind of specimen conditioned for testing expected by lab.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/v2-0487'=>[ 'ABS', 'ACNE', 'ACNFLD', 'AIRS', 'ALL', 'AMN', 'AMP', 'ANGI', 'ARTC', 'ASERU', 'ASP', 'ATTE', 'AUTOA', 'AUTOC', 'AUTP', 'BBL', 'BCYST', 'BDY', 'BIFL', 'BITE', 'BLD', 'BLDA', 'BLDCO', 'BLDV', 'BLEB', 'BLIST', 'BOIL', 'BON', 'BOWL', 'BPH', 'BPU', 'BRN', 'BRSH', 'BRTH', 'BRUS', 'BUB', 'BULLA', 'BX', 'CALC', 'CARBU', 'CAT', 'CBITE', 'CDM', 'CLIPP', 'CNJT', 'CNL', 'COL', 'CONE', 'CSCR', 'CSERU', 'CSF', 'CSITE', 'CSMY', 'CST', 'CSVR', 'CTP', 'CUR', 'CVM', 'CVPS', 'CVPT', 'CYN', 'CYST', 'DBITE', 'DCS', 'DEC', 'DEION', 'DIA', 'DIAF', 'DISCHG', 'DIV', 'DRN', 'DRNG', 'DRNGP', 'DUFL', 'EARW', 'EBRUSH', 'EEYE', 'EFF', 'EFFUS', 'EFOD', 'EISO', 'ELT', 'ENVIR', 'EOS', 'EOTH', 'ESOI', 'ESOS', 'ETA', 'ETTP', 'ETTUB', 'EWHI', 'EXG', 'EXS', 'EXUDTE', 'FAW', 'FBLOOD', 'FGA', 'FIB', 'FIST', 'FLD', 'FLT', 'FLU', 'FLUID', 'FOLEY', 'FRS', 'FSCLP', 'FUR', 'GAS', 'GASA', 'GASAN', 'GASBR', 'GASD', 'GAST', 'GENL', 'GENV', 'GRAFT', 'GRAFTS', 'GRANU', 'GROSH', 'GSOL', 'GSPEC', 'GT', 'GTUBE', 'HAR', 'HBITE', 'HBLUD', 'HEMAQ', 'HEMO', 'HERNI', 'HEV', 'HIC', 'HYDC', 'IBITE', 'ICYST', 'IDC', 'IHG', 'ILEO', 'ILLEG', 'IMP', 'INCI', 'INFIL', 'INS', 'INTRD', 'ISLT', 'IT', 'IUD', 'IVCAT', 'IVFLD', 'IVTIP', 'JEJU', 'JNTFLD', 'JP', 'KELOI', 'KIDFLD', 'LAVG', 'LAVGG', 'LAVGP', 'LAVPG', 'LENS1', 'LENS2', 'LESN', 'LIQ', 'LIQO', 'LNA', 'LNV', 'LSAC', 'LYM', 'MAC', 'MAHUR', 'MAR', 'MASS', 'MBLD', 'MEC', 'MILK', 'MLK', 'MUCOS', 'MUCUS', 'NAIL', 'NASDR', 'NEDL', 'NEPH', 'NGASP', 'NGAST', 'NGS', 'NODUL', 'NSECR', 'ORH', 'ORL', 'OTH', 'PACEM', 'PAFL', 'PCFL', 'PDSIT', 'PDTS', 'PELVA', 'PENIL', 'PERIA', 'PILOC', 'PINS', 'PIS', 'PLAN', 'PLAS', 'PLB', 'PLC', 'PLEVS', 'PLR', 'PMN', 'PND', 'POL', 'POPGS', 'POPLG', 'POPLV', 'PORTA', 'PPP', 'PROST', 'PRP', 'PSC', 'PUNCT', 'PUS', 'PUSFR', 'PUST', 'QC3', 'RANDU', 'RBC', 'RBITE', 'RECT', 'RECTA', 'RENALC', 'RENC', 'RES', 'SAL', 'SCAR', 'SCLV', 'SCROA', 'SECRE', 'SER', 'SHU', 'SHUNF', 'SHUNT', 'SITE', 'SKBP', 'SKN', 'SMM', 'SMN', 'SNV', 'SPRM', 'SPRP', 'SPRPB', 'SPS', 'SPT', 'SPTC', 'SPTT', 'SPUT1', 'SPUTIN', 'SPUTSP', 'STER', 'STL', 'STONE', 'SUBMA', 'SUBMX', 'SUMP', 'SUP', 'SUTUR', 'SWGZ', 'SWT', 'TASP', 'TEAR', 'THRB', 'TISS', 'TISU', 'TLC', 'TRAC', 'TRANS', 'TSERU', 'TSTES', 'TTRA', 'TUBES', 'TUMOR', 'TZANC', 'UDENT', 'UMED', 'UR', 'URC', 'URINB', 'URINC', 'URINM', 'URINN', 'URINP', 'URNS', 'URT', 'USCOP', 'USPEC', 'USUB', 'VASTIP', 'VENT', 'VITF', 'VOM', 'WASH', 'WASI', 'WAT', 'WB', 'WBC', 'WEN', 'WICK', 'WND', 'WNDA', 'WNDD', 'WNDE', 'WORM', 'WRT', 'WWA', 'WWO', 'WWT' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'TypeTested.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0487'}
        },
        ##
        # preferred | alternate
        # The preference for this type of conditioned specimen.
        'preference' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/specimen-contained-preference'=>[ 'preferred', 'alternate' ]
          },
          'type'=>'code',
          'path'=>'TypeTested.preference',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/specimen-contained-preference'}
        },
        ##
        # The specimen's container.
        'container' => {
          'type'=>'SpecimenDefinition::TypeTested::Container',
          'path'=>'TypeTested.container',
          'min'=>0,
          'max'=>1
        },
        ##
        # Specimen requirements
        # Requirements for delivery and special handling of this kind of conditioned specimen.
        'requirement' => {
          'type'=>'string',
          'path'=>'TypeTested.requirement',
          'min'=>0,
          'max'=>1
        },
        ##
        # Specimen retention time
        # The usual time that a specimen of this kind is retained after the ordered tests are completed, for the purpose of additional testing.
        'retentionTime' => {
          'type'=>'Duration',
          'path'=>'TypeTested.retentionTime',
          'min'=>0,
          'max'=>1
        },
        ##
        # Rejection criterion
        # Criterion for rejection of the specimen in its container by the laboratory.
        'rejectionCriterion' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/rejection-criteria'=>[ 'hemolized', 'insufficient', 'broken', 'clotted', 'wrong-temperature' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'TypeTested.rejectionCriterion',
          'min'=>0,
          'max'=>Float::INFINITY,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/rejection-criteria'}
        },
        ##
        # Specimen handling before testing
        # Set of instructions for preservation/transport of the specimen at a defined temperature interval, prior the testing process.
        'handling' => {
          'type'=>'SpecimenDefinition::TypeTested::Handling',
          'path'=>'TypeTested.handling',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # The specimen's container.
      class Container < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'minimumVolume[x]' => ['Quantity', 'string']
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
          # Container material
          # The type of material of the container.
          'material' => {
            'type'=>'CodeableConcept',
            'path'=>'Container.material',
            'min'=>0,
            'max'=>1
          },
          ##
          # Kind of container associated with the kind of specimen
          # The type of container used to contain this kind of specimen.
          'type' => {
            'type'=>'CodeableConcept',
            'path'=>'Container.type',
            'min'=>0,
            'max'=>1
          },
          ##
          # Color of container cap.
          'cap' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/container-cap'=>[ 'red', 'yellow', 'dark-yellow', 'grey', 'light-blue', 'black', 'green', 'light-green', 'lavender', 'brown', 'white', 'pink' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Container.cap',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/container-cap'}
          },
          ##
          # Container description
          # The textual description of the kind of container.
          'description' => {
            'type'=>'string',
            'path'=>'Container.description',
            'min'=>0,
            'max'=>1
          },
          ##
          # Container capacity
          # The capacity (volume or other measure) of this kind of container.
          'capacity' => {
            'type'=>'Quantity',
            'path'=>'Container.capacity',
            'min'=>0,
            'max'=>1
          },
          ##
          # Minimum volume
          # The minimum volume to be conditioned in the container.
          'minimumVolumeQuantity' => {
            'type'=>'Quantity',
            'path'=>'Container.minimumVolume[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Minimum volume
          # The minimum volume to be conditioned in the container.
          'minimumVolumeString' => {
            'type'=>'String',
            'path'=>'Container.minimumVolume[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additive associated with container
          # Substance introduced in the kind of container to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
          'additive' => {
            'type'=>'SpecimenDefinition::TypeTested::Container::Additive',
            'path'=>'Container.additive',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Specimen container preparation
          # Special processing that should be applied to the container for this kind of specimen.
          'preparation' => {
            'type'=>'string',
            'path'=>'Container.preparation',
            'min'=>0,
            'max'=>1
          }
        }

        ##
        # Additive associated with container
        # Substance introduced in the kind of container to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
        class Additive < FHIR::Model
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
              'path'=>'Additive.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Additive.extension',
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
              'path'=>'Additive.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Additive associated with container
            # Substance introduced in the kind of container to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
            'additiveCodeableConcept' => {
              'valid_codes'=>{
                'http://terminology.hl7.org/CodeSystem/v2-0371'=>[ 'ACDA', 'ACDB', 'ACET', 'AMIES', 'BACTM', 'BF10', 'BOR', 'BOUIN', 'BSKM', 'C32', 'C38', 'CARS', 'CARY', 'CHLTM', 'CTAD', 'EDTK', 'EDTK15', 'EDTK75', 'EDTN', 'ENT', 'ENT+', 'F10', 'FDP', 'FL10', 'FL100', 'HCL6', 'HEPA', 'HEPL', 'HEPN', 'HNO3', 'JKM', 'KARN', 'KOX', 'LIA', 'M4', 'M4RT', 'M5', 'MICHTM', 'MMDTM', 'NAF', 'NAPS', 'NONE', 'PAGE', 'PHENOL', 'PVA', 'RLM', 'SILICA', 'SPS', 'SST', 'STUTM', 'THROM', 'THYMOL', 'THYO', 'TOLU', 'URETM', 'VIRTM', 'WEST' ]
              },
              'type'=>'CodeableConcept',
              'path'=>'Additive.additive[x]',
              'min'=>1,
              'max'=>1,
              'binding'=>{'strength'=>'example', 'uri'=>'http://terminology.hl7.org/ValueSet/v2-0371'}
            }
            ##
            # Additive associated with container
            # Substance introduced in the kind of container to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
            'additiveReference' => {
              'valid_codes'=>{
                'http://terminology.hl7.org/CodeSystem/v2-0371'=>[ 'ACDA', 'ACDB', 'ACET', 'AMIES', 'BACTM', 'BF10', 'BOR', 'BOUIN', 'BSKM', 'C32', 'C38', 'CARS', 'CARY', 'CHLTM', 'CTAD', 'EDTK', 'EDTK15', 'EDTK75', 'EDTN', 'ENT', 'ENT+', 'F10', 'FDP', 'FL10', 'FL100', 'HCL6', 'HEPA', 'HEPL', 'HEPN', 'HNO3', 'JKM', 'KARN', 'KOX', 'LIA', 'M4', 'M4RT', 'M5', 'MICHTM', 'MMDTM', 'NAF', 'NAPS', 'NONE', 'PAGE', 'PHENOL', 'PVA', 'RLM', 'SILICA', 'SPS', 'SST', 'STUTM', 'THROM', 'THYMOL', 'THYO', 'TOLU', 'URETM', 'VIRTM', 'WEST' ]
              },
              'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Substance'],
              'type'=>'Reference',
              'path'=>'Additive.additive[x]',
              'min'=>1,
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
          # Additive associated with container
          # Substance introduced in the kind of container to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
          attr_accessor :additiveCodeableConcept        # 1-1 CodeableConcept
          ##
          # Additive associated with container
          # Substance introduced in the kind of container to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
          attr_accessor :additiveReference              # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Substance)
        end
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
        # Container material
        # The type of material of the container.
        attr_accessor :material                       # 0-1 CodeableConcept
        ##
        # Kind of container associated with the kind of specimen
        # The type of container used to contain this kind of specimen.
        attr_accessor :type                           # 0-1 CodeableConcept
        ##
        # Color of container cap.
        attr_accessor :cap                            # 0-1 CodeableConcept
        ##
        # Container description
        # The textual description of the kind of container.
        attr_accessor :description                    # 0-1 string
        ##
        # Container capacity
        # The capacity (volume or other measure) of this kind of container.
        attr_accessor :capacity                       # 0-1 Quantity
        ##
        # Minimum volume
        # The minimum volume to be conditioned in the container.
        attr_accessor :minimumVolumeQuantity          # 0-1 Quantity
        ##
        # Minimum volume
        # The minimum volume to be conditioned in the container.
        attr_accessor :minimumVolumeString            # 0-1 String
        ##
        # Additive associated with container
        # Substance introduced in the kind of container to preserve, maintain or enhance the specimen. Examples: Formalin, Citrate, EDTA.
        attr_accessor :additive                       # 0-* [ SpecimenDefinition::TypeTested::Container::Additive ]
        ##
        # Specimen container preparation
        # Special processing that should be applied to the container for this kind of specimen.
        attr_accessor :preparation                    # 0-1 string
      end

      ##
      # Specimen handling before testing
      # Set of instructions for preservation/transport of the specimen at a defined temperature interval, prior the testing process.
      class Handling < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Handling.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Handling.extension',
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
            'path'=>'Handling.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Temperature qualifier
          # It qualifies the interval of temperature, which characterizes an occurrence of handling. Conditions that are not related to temperature may be handled in the instruction element.
          'temperatureQualifier' => {
            'valid_codes'=>{
              'http://terminology.hl7.org/CodeSystem/handling-condition'=>[ 'room', 'refrigerated', 'frozen' ]
            },
            'type'=>'CodeableConcept',
            'path'=>'Handling.temperatureQualifier',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/handling-condition'}
          },
          ##
          # Temperature range
          # The temperature interval for this set of handling instructions.
          'temperatureRange' => {
            'type'=>'Range',
            'path'=>'Handling.temperatureRange',
            'min'=>0,
            'max'=>1
          },
          ##
          # Maximum preservation time
          # The maximum time interval of preservation of the specimen with these conditions.
          'maxDuration' => {
            'type'=>'Duration',
            'path'=>'Handling.maxDuration',
            'min'=>0,
            'max'=>1
          },
          ##
          # Preservation instruction
          # Additional textual instructions for the preservation or transport of the specimen. For instance, 'Protect from light exposure'.
          'instruction' => {
            'type'=>'string',
            'path'=>'Handling.instruction',
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
        # Temperature qualifier
        # It qualifies the interval of temperature, which characterizes an occurrence of handling. Conditions that are not related to temperature may be handled in the instruction element.
        attr_accessor :temperatureQualifier           # 0-1 CodeableConcept
        ##
        # Temperature range
        # The temperature interval for this set of handling instructions.
        attr_accessor :temperatureRange               # 0-1 Range
        ##
        # Maximum preservation time
        # The maximum time interval of preservation of the specimen with these conditions.
        attr_accessor :maxDuration                    # 0-1 Duration
        ##
        # Preservation instruction
        # Additional textual instructions for the preservation or transport of the specimen. For instance, 'Protect from light exposure'.
        attr_accessor :instruction                    # 0-1 string
      end
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
      # Primary or secondary specimen
      # Primary of secondary specimen.
      attr_accessor :isDerived                      # 0-1 boolean
      ##
      # Type of intended specimen
      # The kind of specimen conditioned for testing expected by lab.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # preferred | alternate
      # The preference for this type of conditioned specimen.
      attr_accessor :preference                     # 1-1 code
      ##
      # The specimen's container.
      attr_accessor :container                      # 0-1 SpecimenDefinition::TypeTested::Container
      ##
      # Specimen requirements
      # Requirements for delivery and special handling of this kind of conditioned specimen.
      attr_accessor :requirement                    # 0-1 string
      ##
      # Specimen retention time
      # The usual time that a specimen of this kind is retained after the ordered tests are completed, for the purpose of additional testing.
      attr_accessor :retentionTime                  # 0-1 Duration
      ##
      # Rejection criterion
      # Criterion for rejection of the specimen in its container by the laboratory.
      attr_accessor :rejectionCriterion             # 0-* [ CodeableConcept ]
      ##
      # Specimen handling before testing
      # Set of instructions for preservation/transport of the specimen at a defined temperature interval, prior the testing process.
      attr_accessor :handling                       # 0-* [ SpecimenDefinition::TypeTested::Handling ]
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
    # Business identifier of a kind of specimen
    # A business identifier associated with the kind of specimen.
    attr_accessor :identifier                     # 0-1 Identifier
    ##
    # Kind of material to collect
    # The kind of material to be collected.
    attr_accessor :typeCollected                  # 0-1 CodeableConcept
    ##
    # Patient preparation for collection
    # Preparation of the patient for specimen collection.
    attr_accessor :patientPreparation             # 0-* [ CodeableConcept ]
    ##
    # Time aspect for collection
    # Time aspect of specimen collection (duration or offset).
    attr_accessor :timeAspect                     # 0-1 string
    ##
    # Specimen collection procedure
    # The action to be performed for collecting the specimen.
    attr_accessor :collection                     # 0-* [ CodeableConcept ]
    ##
    # Specimen in container intended for testing by lab
    # Specimen conditioned in a container as expected by the testing laboratory.
    attr_accessor :typeTested                     # 0-* [ SpecimenDefinition::TypeTested ]

    def resourceType
      'SpecimenDefinition'
    end
  end
end
