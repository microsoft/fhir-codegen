module FHIR

  ##
  # A process where a researcher or organization plans and then executes a series of steps intended to increase the field of healthcare-related knowledge.  This includes studies of safety, efficacy, comparative effectiveness and other information about medications, devices, therapies and other interventional and investigative techniques.  A ResearchStudy involves the gathering of information about human or animal subjects.
  class ResearchStudy < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['category', 'date', 'focus', 'identifier', 'keyword', 'location', 'partof', 'principalinvestigator', 'protocol', 'site', 'sponsor', 'status', 'title']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'ResearchStudy.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ResearchStudy.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ResearchStudy.implicitRules',
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
        'path'=>'ResearchStudy.language',
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
        'path'=>'ResearchStudy.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ResearchStudy.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ResearchStudy.extension',
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
        'path'=>'ResearchStudy.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Business Identifier for study
      # Identifiers assigned to this research study by the sponsor or other systems.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'ResearchStudy.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Name for this study
      # A short, descriptive user-friendly label for the study.
      'title' => {
        'type'=>'string',
        'path'=>'ResearchStudy.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # Steps followed in executing study
      # The set of steps expected to be performed as part of the execution of the study.
      'protocol' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/PlanDefinition'],
        'type'=>'Reference',
        'path'=>'ResearchStudy.protocol',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Part of larger study
      # A larger research study of which this particular study is a component or step.
      'partOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ResearchStudy'],
        'type'=>'Reference',
        'path'=>'ResearchStudy.partOf',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | administratively-completed | approved | closed-to-accrual | closed-to-accrual-and-intervention | completed | disapproved | in-review | temporarily-closed-to-accrual | temporarily-closed-to-accrual-and-intervention | withdrawn
      # The current state of the study.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/research-study-status'=>[ 'active', 'administratively-completed', 'approved', 'closed-to-accrual', 'closed-to-accrual-and-intervention', 'completed', 'disapproved', 'in-review', 'temporarily-closed-to-accrual', 'temporarily-closed-to-accrual-and-intervention', 'withdrawn' ]
        },
        'type'=>'code',
        'path'=>'ResearchStudy.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/research-study-status'}
      },
      ##
      # treatment | prevention | diagnostic | supportive-care | screening | health-services-research | basic-science | device-feasibility
      # The type of study based upon the intent of the study's activities. A classification of the intent of the study.
      'primaryPurposeType' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/research-study-prim-purp-type'=>[ 'treatment', 'prevention', 'diagnostic', 'supportive-care', 'screening', 'health-services-research', 'basic-science', 'device-feasibility' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ResearchStudy.primaryPurposeType',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/research-study-prim-purp-type'}
      },
      ##
      # n-a | early-phase-1 | phase-1 | phase-1-phase-2 | phase-2 | phase-2-phase-3 | phase-3 | phase-4
      # The stage in the progression of a therapy from initial experimental use in humans in clinical trials to post-market evaluation.
      'phase' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/research-study-phase'=>[ 'n-a', 'early-phase-1', 'phase-1', 'phase-1-phase-2', 'phase-2', 'phase-2-phase-3', 'phase-3', 'phase-4' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ResearchStudy.phase',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/research-study-phase'}
      },
      ##
      # Classifications for the study
      # Codes categorizing the type of study such as investigational vs. observational, type of blinding, type of randomization, safety vs. efficacy, etc.
      'category' => {
        'type'=>'CodeableConcept',
        'path'=>'ResearchStudy.category',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Drugs, devices, etc. under study
      # The medication(s), food(s), therapy(ies), device(s) or other concerns or interventions that the study is seeking to gain more information about.
      'focus' => {
        'type'=>'CodeableConcept',
        'path'=>'ResearchStudy.focus',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Condition being studied
      # The condition that is the focus of the study.  For example, In a study to examine risk factors for Lupus, might have as an inclusion criterion "healthy volunteer", but the target condition code would be a Lupus SNOMED code.
      'condition' => {
        'type'=>'CodeableConcept',
        'path'=>'ResearchStudy.condition',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Contact details for the study
      # Contact details to assist a user in learning more about or engaging with the study.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'ResearchStudy.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # References and dependencies
      # Citations, references and other related documents.
      'relatedArtifact' => {
        'type'=>'RelatedArtifact',
        'path'=>'ResearchStudy.relatedArtifact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Used to search for the study
      # Key terms to aid in searching for or filtering the study.
      'keyword' => {
        'type'=>'CodeableConcept',
        'path'=>'ResearchStudy.keyword',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Geographic region(s) for study
      # Indicates a country, state or other region where the study is taking place.
      'location' => {
        'type'=>'CodeableConcept',
        'path'=>'ResearchStudy.location',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # What this is study doing
      # A full description of how the study is being conducted.
      'description' => {
        'type'=>'markdown',
        'path'=>'ResearchStudy.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # Inclusion & exclusion criteria
      # Reference to a Group that defines the criteria for and quantity of subjects participating in the study.  E.g. " 200 female Europeans between the ages of 20 and 45 with early onset diabetes".
      # The Group referenced should not generally enumerate specific subjects.  Subjects will be linked to the study using the ResearchSubject resource.
      'enrollment' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Group'],
        'type'=>'Reference',
        'path'=>'ResearchStudy.enrollment',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # When the study began and ended
      # Identifies the start date and the expected (or actual, depending on status) end date for the study.
      'period' => {
        'type'=>'Period',
        'path'=>'ResearchStudy.period',
        'min'=>0,
        'max'=>1
      },
      ##
      # Organization that initiates and is legally responsible for the study
      # An organization that initiates the investigation and is legally responsible for the study.
      'sponsor' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'ResearchStudy.sponsor',
        'min'=>0,
        'max'=>1
      },
      ##
      # Researcher who oversees multiple aspects of the study
      # A researcher in a study who oversees multiple aspects of the study, such as concept development, protocol writing, protocol submission for IRB approval, participant recruitment, informed consent, data collection, analysis, interpretation and presentation.
      'principalInvestigator' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole'],
        'type'=>'Reference',
        'path'=>'ResearchStudy.principalInvestigator',
        'min'=>0,
        'max'=>1
      },
      ##
      # Facility where study activities are conducted
      # A facility in which study activities are conducted.
      'site' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Location'],
        'type'=>'Reference',
        'path'=>'ResearchStudy.site',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # accrual-goal-met | closed-due-to-toxicity | closed-due-to-lack-of-study-progress | temporarily-closed-per-study-design
      # A description and/or code explaining the premature termination of the study.
      'reasonStopped' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/research-study-reason-stopped'=>[ 'accrual-goal-met', 'closed-due-to-toxicity', 'closed-due-to-lack-of-study-progress', 'temporarily-closed-per-study-design' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'ResearchStudy.reasonStopped',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/research-study-reason-stopped'}
      },
      ##
      # Comments made about the study by the performer, subject or other participants.
      'note' => {
        'type'=>'Annotation',
        'path'=>'ResearchStudy.note',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Defined path through the study for a subject
      # Describes an expected sequence of events for one of the participants of a study.  E.g. Exposure to drug A, wash-out, exposure to drug B, wash-out, follow-up.
      'arm' => {
        'type'=>'ResearchStudy::Arm',
        'path'=>'ResearchStudy.arm',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A goal for the study
      # A goal that the study is aiming to achieve in terms of a scientific question to be answered by the analysis of data collected during the study.
      'objective' => {
        'type'=>'ResearchStudy::Objective',
        'path'=>'ResearchStudy.objective',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # Defined path through the study for a subject
    # Describes an expected sequence of events for one of the participants of a study.  E.g. Exposure to drug A, wash-out, exposure to drug B, wash-out, follow-up.
    class Arm < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Arm.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Arm.extension',
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
          'path'=>'Arm.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Label for study arm
        # Unique, human-readable label for this arm of the study.
        'name' => {
          'type'=>'string',
          'path'=>'Arm.name',
          'min'=>1,
          'max'=>1
        },
        ##
        # Categorization of study arm, e.g. experimental, active comparator, placebo comparater.
        'type' => {
          'type'=>'CodeableConcept',
          'path'=>'Arm.type',
          'min'=>0,
          'max'=>1
        },
        ##
        # Short explanation of study path
        # A succinct description of the path through the study that would be followed by a subject adhering to this arm.
        'description' => {
          'type'=>'string',
          'path'=>'Arm.description',
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
      # Label for study arm
      # Unique, human-readable label for this arm of the study.
      attr_accessor :name                           # 1-1 string
      ##
      # Categorization of study arm, e.g. experimental, active comparator, placebo comparater.
      attr_accessor :type                           # 0-1 CodeableConcept
      ##
      # Short explanation of study path
      # A succinct description of the path through the study that would be followed by a subject adhering to this arm.
      attr_accessor :description                    # 0-1 string
    end

    ##
    # A goal for the study
    # A goal that the study is aiming to achieve in terms of a scientific question to be answered by the analysis of data collected during the study.
    class Objective < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Objective.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Objective.extension',
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
          'path'=>'Objective.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Label for the objective
        # Unique, human-readable label for this objective of the study.
        'name' => {
          'type'=>'string',
          'path'=>'Objective.name',
          'min'=>0,
          'max'=>1
        },
        ##
        # primary | secondary | exploratory
        # The kind of study objective.
        'type' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/research-study-objective-type'=>[ 'primary', 'secondary', 'exploratory' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'Objective.type',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/research-study-objective-type'}
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
      # Label for the objective
      # Unique, human-readable label for this objective of the study.
      attr_accessor :name                           # 0-1 string
      ##
      # primary | secondary | exploratory
      # The kind of study objective.
      attr_accessor :type                           # 0-1 CodeableConcept
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
    # Business Identifier for study
    # Identifiers assigned to this research study by the sponsor or other systems.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # Name for this study
    # A short, descriptive user-friendly label for the study.
    attr_accessor :title                          # 0-1 string
    ##
    # Steps followed in executing study
    # The set of steps expected to be performed as part of the execution of the study.
    attr_accessor :protocol                       # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/PlanDefinition) ]
    ##
    # Part of larger study
    # A larger research study of which this particular study is a component or step.
    attr_accessor :partOf                         # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/ResearchStudy) ]
    ##
    # active | administratively-completed | approved | closed-to-accrual | closed-to-accrual-and-intervention | completed | disapproved | in-review | temporarily-closed-to-accrual | temporarily-closed-to-accrual-and-intervention | withdrawn
    # The current state of the study.
    attr_accessor :status                         # 1-1 code
    ##
    # treatment | prevention | diagnostic | supportive-care | screening | health-services-research | basic-science | device-feasibility
    # The type of study based upon the intent of the study's activities. A classification of the intent of the study.
    attr_accessor :primaryPurposeType             # 0-1 CodeableConcept
    ##
    # n-a | early-phase-1 | phase-1 | phase-1-phase-2 | phase-2 | phase-2-phase-3 | phase-3 | phase-4
    # The stage in the progression of a therapy from initial experimental use in humans in clinical trials to post-market evaluation.
    attr_accessor :phase                          # 0-1 CodeableConcept
    ##
    # Classifications for the study
    # Codes categorizing the type of study such as investigational vs. observational, type of blinding, type of randomization, safety vs. efficacy, etc.
    attr_accessor :category                       # 0-* [ CodeableConcept ]
    ##
    # Drugs, devices, etc. under study
    # The medication(s), food(s), therapy(ies), device(s) or other concerns or interventions that the study is seeking to gain more information about.
    attr_accessor :focus                          # 0-* [ CodeableConcept ]
    ##
    # Condition being studied
    # The condition that is the focus of the study.  For example, In a study to examine risk factors for Lupus, might have as an inclusion criterion "healthy volunteer", but the target condition code would be a Lupus SNOMED code.
    attr_accessor :condition                      # 0-* [ CodeableConcept ]
    ##
    # Contact details for the study
    # Contact details to assist a user in learning more about or engaging with the study.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # References and dependencies
    # Citations, references and other related documents.
    attr_accessor :relatedArtifact                # 0-* [ RelatedArtifact ]
    ##
    # Used to search for the study
    # Key terms to aid in searching for or filtering the study.
    attr_accessor :keyword                        # 0-* [ CodeableConcept ]
    ##
    # Geographic region(s) for study
    # Indicates a country, state or other region where the study is taking place.
    attr_accessor :location                       # 0-* [ CodeableConcept ]
    ##
    # What this is study doing
    # A full description of how the study is being conducted.
    attr_accessor :description                    # 0-1 markdown
    ##
    # Inclusion & exclusion criteria
    # Reference to a Group that defines the criteria for and quantity of subjects participating in the study.  E.g. " 200 female Europeans between the ages of 20 and 45 with early onset diabetes".
    # The Group referenced should not generally enumerate specific subjects.  Subjects will be linked to the study using the ResearchSubject resource.
    attr_accessor :enrollment                     # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Group) ]
    ##
    # When the study began and ended
    # Identifies the start date and the expected (or actual, depending on status) end date for the study.
    attr_accessor :period                         # 0-1 Period
    ##
    # Organization that initiates and is legally responsible for the study
    # An organization that initiates the investigation and is legally responsible for the study.
    attr_accessor :sponsor                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Researcher who oversees multiple aspects of the study
    # A researcher in a study who oversees multiple aspects of the study, such as concept development, protocol writing, protocol submission for IRB approval, participant recruitment, informed consent, data collection, analysis, interpretation and presentation.
    attr_accessor :principalInvestigator          # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole)
    ##
    # Facility where study activities are conducted
    # A facility in which study activities are conducted.
    attr_accessor :site                           # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Location) ]
    ##
    # accrual-goal-met | closed-due-to-toxicity | closed-due-to-lack-of-study-progress | temporarily-closed-per-study-design
    # A description and/or code explaining the premature termination of the study.
    attr_accessor :reasonStopped                  # 0-1 CodeableConcept
    ##
    # Comments made about the study by the performer, subject or other participants.
    attr_accessor :note                           # 0-* [ Annotation ]
    ##
    # Defined path through the study for a subject
    # Describes an expected sequence of events for one of the participants of a study.  E.g. Exposure to drug A, wash-out, exposure to drug B, wash-out, follow-up.
    attr_accessor :arm                            # 0-* [ ResearchStudy::Arm ]
    ##
    # A goal for the study
    # A goal that the study is aiming to achieve in terms of a scientific question to be answered by the analysis of data collected during the study.
    attr_accessor :objective                      # 0-* [ ResearchStudy::Objective ]

    def resourceType
      'ResearchStudy'
    end
  end
end
