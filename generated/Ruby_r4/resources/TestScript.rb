module FHIR

  ##
  # A structured set of tests against a FHIR server or client implementation to determine compliance against the FHIR specification.
  class TestScript < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'description', 'identifier', 'jurisdiction', 'name', 'publisher', 'status', 'testscript-capability', 'title', 'url', 'version']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'TestScript.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'TestScript.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'TestScript.implicitRules',
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
        'path'=>'TestScript.language',
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
        'path'=>'TestScript.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'TestScript.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'TestScript.extension',
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
        'path'=>'TestScript.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this test script, represented as a URI (globally unique)
      # An absolute URI that is used to identify this test script when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this test script is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the test script is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'TestScript.url',
        'min'=>1,
        'max'=>1
      },
      ##
      # Additional identifier for the test script
      # A formal identifier that is used to identify this test script when it is represented in other formats, or referenced in a specification, model, design or an instance.
      # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this test script outside of FHIR, where it is not possible to use the logical URI.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'TestScript.identifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # Business version of the test script
      # The identifier that is used to identify this version of the test script when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the test script author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
      # There may be different test script instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the test script with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'TestScript.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this test script (computer friendly)
      # A natural language name identifying the test script. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'TestScript.name',
        'min'=>1,
        'max'=>1
      },
      ##
      # Name for this test script (human friendly)
      # A short, descriptive, user-friendly title for the test script.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'TestScript.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this test script. Enables tracking the life-cycle of the content.
      # Allows filtering of test scripts that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'TestScript.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this test script is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of test scripts that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'TestScript.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the test script was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the test script changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the test script. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'TestScript.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the test script.
      # Usually an organization but may be an individual. The publisher (or steward) of the test script is the organization or individual primarily responsible for the maintenance and upkeep of the test script. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the test script. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'TestScript.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'TestScript.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the test script
      # A free text natural language description of the test script from a consumer's perspective.
      # This description can be used to capture details such as why the test script was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the test script as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the test script is presumed to be the predominant language in the place the test script was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'TestScript.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate test script instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'TestScript.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for test script (if applicable)
      # A legal or geographic region in which the test script is intended to be used.
      # It may be possible for the test script to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'TestScript.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Why this test script is defined
      # Explanation of why this test script is needed and why it has been designed as it has.
      # This element does not describe the usage of the test script. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this test script.
      'purpose' => {
        'type'=>'markdown',
        'path'=>'TestScript.purpose',
        'min'=>0,
        'max'=>1
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the test script and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the test script.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'TestScript.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # An abstract server representing a client or sender in a message exchange
      # An abstract server used in operations within this test script in the origin element.
      # The purpose of this element is to define the profile of an origin element used elsewhere in the script.  Test engines could then use the origin-profile mapping to offer a filtered list of test systems that can serve as the sender for the interaction.
      'origin' => {
        'type'=>'TestScript::Origin',
        'path'=>'TestScript.origin',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # An abstract server representing a destination or receiver in a message exchange
      # An abstract server used in operations within this test script in the destination element.
      # The purpose of this element is to define the profile of a destination element used elsewhere in the script.  Test engines could then use the destination-profile mapping to offer a filtered list of test systems that can serve as the receiver for the interaction.
      'destination' => {
        'type'=>'TestScript::Destination',
        'path'=>'TestScript.destination',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Required capability that is assumed to function correctly on the FHIR server being tested
      # The required capability must exist and are assumed to function correctly on the FHIR server being tested.
      'metadata' => {
        'type'=>'TestScript::Metadata',
        'path'=>'TestScript.metadata',
        'min'=>0,
        'max'=>1
      },
      ##
      # Fixture in the test script - by reference (uri). All fixtures are required for the test script to execute.
      'fixture' => {
        'type'=>'TestScript::Fixture',
        'path'=>'TestScript.fixture',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Reference of the validation profile
      # Reference to the profile to be used for validation.
      # See http://build.fhir.org/resourcelist.html for complete list of resource types.
      'profile' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'Reference',
        'path'=>'TestScript.profile',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Placeholder for evaluated elements
      # Variable is set based either on element value in response body or on header field value in the response headers.
      # Variables would be set based either on XPath/JSONPath expressions against fixtures (static and response), or headerField evaluations against response headers. If variable evaluates to nodelist or anything other than a primitive value, then test engine would report an error.  Variables would be used to perform clean replacements in "operation.params", "operation.requestHeader.value", and "operation.url" element values during operation calls and in "assert.value" during assertion evaluations. This limits the places that test engines would need to look for placeholders "${}".  Variables are scoped to the whole script. They are NOT evaluated at declaration. They are evaluated by test engine when used for substitutions in "operation.params", "operation.requestHeader.value", and "operation.url" element values during operation calls and in "assert.value" during assertion evaluations.  See example testscript-search.xml.
      'variable' => {
        'type'=>'TestScript::Variable',
        'path'=>'TestScript.variable',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A series of required setup operations before tests are executed.
      'setup' => {
        'type'=>'TestScript::Setup',
        'path'=>'TestScript.setup',
        'min'=>0,
        'max'=>1
      },
      ##
      # A test in this script.
      'test' => {
        'type'=>'TestScript::Test',
        'path'=>'TestScript.test',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # A series of required clean up steps
      # A series of operations required to clean up after all the tests are executed (successfully or otherwise).
      'teardown' => {
        'type'=>'TestScript::Teardown',
        'path'=>'TestScript.teardown',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # An abstract server representing a client or sender in a message exchange
    # An abstract server used in operations within this test script in the origin element.
    # The purpose of this element is to define the profile of an origin element used elsewhere in the script.  Test engines could then use the origin-profile mapping to offer a filtered list of test systems that can serve as the sender for the interaction.
    class Origin < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Origin.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Origin.extension',
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
          'path'=>'Origin.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The index of the abstract origin server starting at 1
        # Abstract name given to an origin server in this test script.  The name is provided as a number starting at 1.
        # A given origin index (e.g. 1) can appear only once in the list (e.g. Origin 1 cannot be specified twice ... once as FormFiller and again as FormProcessor within the same script as that could get confusing during test configuration). 
        # 
        # Different origin indices could play the same actor in the same test script (e.g. You could have two different test systems acting as Form-Filler).
        # 
        # The origin indices provided elsewhere in the test script must be one of these origin indices.
        'index' => {
          'type'=>'integer',
          'path'=>'Origin.index',
          'min'=>1,
          'max'=>1
        },
        ##
        # FHIR-Client | FHIR-SDC-FormFiller
        # The type of origin profile the test system supports.
        # Must be a "sender"/"client" profile.
        'profile' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/testscript-profile-origin-types'=>[ 'FHIR-Client', 'FHIR-SDC-FormFiller' ]
          },
          'type'=>'Coding',
          'path'=>'Origin.profile',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/testscript-profile-origin-types'}
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
      # The index of the abstract origin server starting at 1
      # Abstract name given to an origin server in this test script.  The name is provided as a number starting at 1.
      # A given origin index (e.g. 1) can appear only once in the list (e.g. Origin 1 cannot be specified twice ... once as FormFiller and again as FormProcessor within the same script as that could get confusing during test configuration). 
      # 
      # Different origin indices could play the same actor in the same test script (e.g. You could have two different test systems acting as Form-Filler).
      # 
      # The origin indices provided elsewhere in the test script must be one of these origin indices.
      attr_accessor :index                          # 1-1 integer
      ##
      # FHIR-Client | FHIR-SDC-FormFiller
      # The type of origin profile the test system supports.
      # Must be a "sender"/"client" profile.
      attr_accessor :profile                        # 1-1 Coding
    end

    ##
    # An abstract server representing a destination or receiver in a message exchange
    # An abstract server used in operations within this test script in the destination element.
    # The purpose of this element is to define the profile of a destination element used elsewhere in the script.  Test engines could then use the destination-profile mapping to offer a filtered list of test systems that can serve as the receiver for the interaction.
    class Destination < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Destination.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Destination.extension',
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
          'path'=>'Destination.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # The index of the abstract destination server starting at 1
        # Abstract name given to a destination server in this test script.  The name is provided as a number starting at 1.
        # A given destination index (e.g. 1) can appear only once in the list (e.g. Destination 1 cannot be specified twice ... once as Form-Manager and again as Form-Processor within the same script as that could get confusing during test configuration). 
        # 
        # Different destination indices could play the same actor in the same test script (e.g. You could have two different test systems acting as Form-Manager).
        # 
        # The destination indices provided elsewhere in the test script must be one of these destination indices.
        'index' => {
          'type'=>'integer',
          'path'=>'Destination.index',
          'min'=>1,
          'max'=>1
        },
        ##
        # FHIR-Server | FHIR-SDC-FormManager | FHIR-SDC-FormReceiver | FHIR-SDC-FormProcessor
        # The type of destination profile the test system supports.
        # Must be a "receiver"/"server" profile.
        'profile' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/testscript-profile-destination-types'=>[ 'FHIR-Server', 'FHIR-SDC-FormManager', 'FHIR-SDC-FormProcessor', 'FHIR-SDC-FormReceiver' ]
          },
          'type'=>'Coding',
          'path'=>'Destination.profile',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/testscript-profile-destination-types'}
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
      # The index of the abstract destination server starting at 1
      # Abstract name given to a destination server in this test script.  The name is provided as a number starting at 1.
      # A given destination index (e.g. 1) can appear only once in the list (e.g. Destination 1 cannot be specified twice ... once as Form-Manager and again as Form-Processor within the same script as that could get confusing during test configuration). 
      # 
      # Different destination indices could play the same actor in the same test script (e.g. You could have two different test systems acting as Form-Manager).
      # 
      # The destination indices provided elsewhere in the test script must be one of these destination indices.
      attr_accessor :index                          # 1-1 integer
      ##
      # FHIR-Server | FHIR-SDC-FormManager | FHIR-SDC-FormReceiver | FHIR-SDC-FormProcessor
      # The type of destination profile the test system supports.
      # Must be a "receiver"/"server" profile.
      attr_accessor :profile                        # 1-1 Coding
    end

    ##
    # Required capability that is assumed to function correctly on the FHIR server being tested
    # The required capability must exist and are assumed to function correctly on the FHIR server being tested.
    class Metadata < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Metadata.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Metadata.extension',
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
          'path'=>'Metadata.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Links to the FHIR specification
        # A link to the FHIR specification that this test is covering.
        'link' => {
          'type'=>'TestScript::Metadata::Link',
          'path'=>'Metadata.link',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Capabilities  that are assumed to function correctly on the FHIR server being tested
        # Capabilities that must exist and are assumed to function correctly on the FHIR server being tested.
        # When the metadata capabilities section is defined at TestScript.metadata or at TestScript.setup.metadata, and the server's conformance statement does not contain the elements defined in the minimal conformance statement, then all the tests in the TestScript are skipped.  When the metadata capabilities section is defined at TestScript.test.metadata and the server's conformance statement does not contain the elements defined in the minimal conformance statement, then only that test is skipped.  The "metadata.capabilities.required" and "metadata.capabilities.validated" elements only indicate whether the capabilities are the primary focus of the test script or not.  They do not impact the skipping logic.  Capabilities whose "metadata.capabilities.validated" flag is true are the primary focus of the test script.
        'capability' => {
          'type'=>'TestScript::Metadata::Capability',
          'path'=>'Metadata.capability',
          'min'=>1,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Links to the FHIR specification
      # A link to the FHIR specification that this test is covering.
      class Link < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Link.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Link.extension',
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
            'path'=>'Link.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # URL to the specification
          # URL to a particular requirement or feature within the FHIR specification.
          'url' => {
            'type'=>'uri',
            'path'=>'Link.url',
            'min'=>1,
            'max'=>1
          },
          ##
          # Short description of the link.
          'description' => {
            'type'=>'string',
            'path'=>'Link.description',
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
        # URL to the specification
        # URL to a particular requirement or feature within the FHIR specification.
        attr_accessor :url                            # 1-1 uri
        ##
        # Short description of the link.
        attr_accessor :description                    # 0-1 string
      end

      ##
      # Capabilities  that are assumed to function correctly on the FHIR server being tested
      # Capabilities that must exist and are assumed to function correctly on the FHIR server being tested.
      # When the metadata capabilities section is defined at TestScript.metadata or at TestScript.setup.metadata, and the server's conformance statement does not contain the elements defined in the minimal conformance statement, then all the tests in the TestScript are skipped.  When the metadata capabilities section is defined at TestScript.test.metadata and the server's conformance statement does not contain the elements defined in the minimal conformance statement, then only that test is skipped.  The "metadata.capabilities.required" and "metadata.capabilities.validated" elements only indicate whether the capabilities are the primary focus of the test script or not.  They do not impact the skipping logic.  Capabilities whose "metadata.capabilities.validated" flag is true are the primary focus of the test script.
      class Capability < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Capability.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Capability.extension',
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
            'path'=>'Capability.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Are the capabilities required?
          # Whether or not the test execution will require the given capabilities of the server in order for this test script to execute.
          'required' => {
            'type'=>'boolean',
            'path'=>'Capability.required',
            'min'=>1,
            'max'=>1
          },
          ##
          # Are the capabilities validated?
          # Whether or not the test execution will validate the given capabilities of the server in order for this test script to execute.
          'validated' => {
            'type'=>'boolean',
            'path'=>'Capability.validated',
            'min'=>1,
            'max'=>1
          },
          ##
          # The expected capabilities of the server
          # Description of the capabilities that this test script is requiring the server to support.
          'description' => {
            'type'=>'string',
            'path'=>'Capability.description',
            'min'=>0,
            'max'=>1
          },
          ##
          # Which origin server these requirements apply to.
          'origin' => {
            'type'=>'integer',
            'path'=>'Capability.origin',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Which server these requirements apply to.
          'destination' => {
            'type'=>'integer',
            'path'=>'Capability.destination',
            'min'=>0,
            'max'=>1
          },
          ##
          # Links to the FHIR specification that describes this interaction and the resources involved in more detail.
          'link' => {
            'type'=>'uri',
            'path'=>'Capability.link',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Required Capability Statement
          # Minimum capabilities required of server for test script to execute successfully.   If server does not meet at a minimum the referenced capability statement, then all tests in this script are skipped.
          # The conformance statement of the server has to contain at a minimum the contents of the reference pointed to by this element.
          'capabilities' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/CapabilityStatement'],
            'type'=>'canonical',
            'path'=>'Capability.capabilities',
            'min'=>1,
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
        # Are the capabilities required?
        # Whether or not the test execution will require the given capabilities of the server in order for this test script to execute.
        attr_accessor :required                       # 1-1 boolean
        ##
        # Are the capabilities validated?
        # Whether or not the test execution will validate the given capabilities of the server in order for this test script to execute.
        attr_accessor :validated                      # 1-1 boolean
        ##
        # The expected capabilities of the server
        # Description of the capabilities that this test script is requiring the server to support.
        attr_accessor :description                    # 0-1 string
        ##
        # Which origin server these requirements apply to.
        attr_accessor :origin                         # 0-* [ integer ]
        ##
        # Which server these requirements apply to.
        attr_accessor :destination                    # 0-1 integer
        ##
        # Links to the FHIR specification that describes this interaction and the resources involved in more detail.
        attr_accessor :link                           # 0-* [ uri ]
        ##
        # Required Capability Statement
        # Minimum capabilities required of server for test script to execute successfully.   If server does not meet at a minimum the referenced capability statement, then all tests in this script are skipped.
        # The conformance statement of the server has to contain at a minimum the contents of the reference pointed to by this element.
        attr_accessor :capabilities                   # 1-1 canonical(http://hl7.org/fhir/StructureDefinition/CapabilityStatement)
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
      # Links to the FHIR specification
      # A link to the FHIR specification that this test is covering.
      attr_accessor :link                           # 0-* [ TestScript::Metadata::Link ]
      ##
      # Capabilities  that are assumed to function correctly on the FHIR server being tested
      # Capabilities that must exist and are assumed to function correctly on the FHIR server being tested.
      # When the metadata capabilities section is defined at TestScript.metadata or at TestScript.setup.metadata, and the server's conformance statement does not contain the elements defined in the minimal conformance statement, then all the tests in the TestScript are skipped.  When the metadata capabilities section is defined at TestScript.test.metadata and the server's conformance statement does not contain the elements defined in the minimal conformance statement, then only that test is skipped.  The "metadata.capabilities.required" and "metadata.capabilities.validated" elements only indicate whether the capabilities are the primary focus of the test script or not.  They do not impact the skipping logic.  Capabilities whose "metadata.capabilities.validated" flag is true are the primary focus of the test script.
      attr_accessor :capability                     # 1-* [ TestScript::Metadata::Capability ]
    end

    ##
    # Fixture in the test script - by reference (uri). All fixtures are required for the test script to execute.
    class Fixture < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Fixture.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Fixture.extension',
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
          'path'=>'Fixture.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Whether or not to implicitly create the fixture during setup. If true, the fixture is automatically created on each server being tested during setup, therefore no create operation is required for this fixture in the TestScript.setup section.
        'autocreate' => {
          'type'=>'boolean',
          'path'=>'Fixture.autocreate',
          'min'=>1,
          'max'=>1
        },
        ##
        # Whether or not to implicitly delete the fixture during teardown. If true, the fixture is automatically deleted on each server being tested during teardown, therefore no delete operation is required for this fixture in the TestScript.teardown section.
        'autodelete' => {
          'type'=>'boolean',
          'path'=>'Fixture.autodelete',
          'min'=>1,
          'max'=>1
        },
        ##
        # Reference of the resource
        # Reference to the resource (containing the contents of the resource needed for operations).
        # See http://build.fhir.org/resourcelist.html for complete list of resource types.
        'resource' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
          'type'=>'Reference',
          'path'=>'Fixture.resource',
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
      # Whether or not to implicitly create the fixture during setup. If true, the fixture is automatically created on each server being tested during setup, therefore no create operation is required for this fixture in the TestScript.setup section.
      attr_accessor :autocreate                     # 1-1 boolean
      ##
      # Whether or not to implicitly delete the fixture during teardown. If true, the fixture is automatically deleted on each server being tested during teardown, therefore no delete operation is required for this fixture in the TestScript.teardown section.
      attr_accessor :autodelete                     # 1-1 boolean
      ##
      # Reference of the resource
      # Reference to the resource (containing the contents of the resource needed for operations).
      # See http://build.fhir.org/resourcelist.html for complete list of resource types.
      attr_accessor :resource                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
    end

    ##
    # Placeholder for evaluated elements
    # Variable is set based either on element value in response body or on header field value in the response headers.
    # Variables would be set based either on XPath/JSONPath expressions against fixtures (static and response), or headerField evaluations against response headers. If variable evaluates to nodelist or anything other than a primitive value, then test engine would report an error.  Variables would be used to perform clean replacements in "operation.params", "operation.requestHeader.value", and "operation.url" element values during operation calls and in "assert.value" during assertion evaluations. This limits the places that test engines would need to look for placeholders "${}".  Variables are scoped to the whole script. They are NOT evaluated at declaration. They are evaluated by test engine when used for substitutions in "operation.params", "operation.requestHeader.value", and "operation.url" element values during operation calls and in "assert.value" during assertion evaluations.  See example testscript-search.xml.
    class Variable < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Variable.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Variable.extension',
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
          'path'=>'Variable.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Descriptive name for this variable.
        # Placeholders would contain the variable name wrapped in ${} in "operation.params", "operation.requestHeader.value", and "operation.url" elements.  These placeholders would need to be replaced by the variable value before the operation is executed.
        'name' => {
          'type'=>'string',
          'path'=>'Variable.name',
          'min'=>1,
          'max'=>1
        },
        ##
        # Default, hard-coded, or user-defined value for this variable
        # A default, hard-coded, or user-defined value for this variable.
        # The purpose of this element is to allow for a pre-defined value that can be used as a default or as an override value. Test engines can optionally use this as a placeholder for user-defined execution time values.
        'defaultValue' => {
          'type'=>'string',
          'path'=>'Variable.defaultValue',
          'min'=>0,
          'max'=>1
        },
        ##
        # Natural language description of the variable
        # A free text natural language description of the variable and its purpose.
        'description' => {
          'type'=>'string',
          'path'=>'Variable.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # The FHIRPath expression against the fixture body
        # The FHIRPath expression to evaluate against the fixture body. When variables are defined, only one of either expression, headerField or path must be specified.
        # If headerField is defined, then the variable will be evaluated against the headers that sourceId is pointing to.  If expression or path is defined, then the variable will be evaluated against the fixture body that sourceId is pointing to.  It is an error to define any combination of expression, headerField and path.
        'expression' => {
          'type'=>'string',
          'path'=>'Variable.expression',
          'min'=>0,
          'max'=>1
        },
        ##
        # HTTP header field name for source
        # Will be used to grab the HTTP header field value from the headers that sourceId is pointing to.
        # If headerField is defined, then the variable will be evaluated against the headers that sourceId is pointing to.  If path is defined, then the variable will be evaluated against the fixture body that sourceId is pointing to.  It is an error to define both headerField and path.
        'headerField' => {
          'type'=>'string',
          'path'=>'Variable.headerField',
          'min'=>0,
          'max'=>1
        },
        ##
        # Hint help text for default value to enter
        # Displayable text string with hint help information to the user when entering a default value.
        'hint' => {
          'type'=>'string',
          'path'=>'Variable.hint',
          'min'=>0,
          'max'=>1
        },
        ##
        # XPath or JSONPath against the fixture body
        # XPath or JSONPath to evaluate against the fixture body.  When variables are defined, only one of either expression, headerField or path must be specified.
        # If headerField is defined, then the variable will be evaluated against the headers that sourceId is pointing to.  If expression or path is defined, then the variable will be evaluated against the fixture body that sourceId is pointing to.  It is an error to define any combination of expression, headerField and path.
        'path' => {
          'type'=>'string',
          'path'=>'Variable.path',
          'min'=>0,
          'max'=>1
        },
        ##
        # Fixture Id of source expression or headerField within this variable
        # Fixture to evaluate the XPath/JSONPath expression or the headerField  against within this variable.
        # This can be a statically defined fixture (at the top of the TestScript) or a dynamically set fixture created by responseId of the `action.operation` element.
        'sourceId' => {
          'type'=>'id',
          'path'=>'Variable.sourceId',
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
      # Descriptive name for this variable.
      # Placeholders would contain the variable name wrapped in ${} in "operation.params", "operation.requestHeader.value", and "operation.url" elements.  These placeholders would need to be replaced by the variable value before the operation is executed.
      attr_accessor :name                           # 1-1 string
      ##
      # Default, hard-coded, or user-defined value for this variable
      # A default, hard-coded, or user-defined value for this variable.
      # The purpose of this element is to allow for a pre-defined value that can be used as a default or as an override value. Test engines can optionally use this as a placeholder for user-defined execution time values.
      attr_accessor :defaultValue                   # 0-1 string
      ##
      # Natural language description of the variable
      # A free text natural language description of the variable and its purpose.
      attr_accessor :description                    # 0-1 string
      ##
      # The FHIRPath expression against the fixture body
      # The FHIRPath expression to evaluate against the fixture body. When variables are defined, only one of either expression, headerField or path must be specified.
      # If headerField is defined, then the variable will be evaluated against the headers that sourceId is pointing to.  If expression or path is defined, then the variable will be evaluated against the fixture body that sourceId is pointing to.  It is an error to define any combination of expression, headerField and path.
      attr_accessor :expression                     # 0-1 string
      ##
      # HTTP header field name for source
      # Will be used to grab the HTTP header field value from the headers that sourceId is pointing to.
      # If headerField is defined, then the variable will be evaluated against the headers that sourceId is pointing to.  If path is defined, then the variable will be evaluated against the fixture body that sourceId is pointing to.  It is an error to define both headerField and path.
      attr_accessor :headerField                    # 0-1 string
      ##
      # Hint help text for default value to enter
      # Displayable text string with hint help information to the user when entering a default value.
      attr_accessor :hint                           # 0-1 string
      ##
      # XPath or JSONPath against the fixture body
      # XPath or JSONPath to evaluate against the fixture body.  When variables are defined, only one of either expression, headerField or path must be specified.
      # If headerField is defined, then the variable will be evaluated against the headers that sourceId is pointing to.  If expression or path is defined, then the variable will be evaluated against the fixture body that sourceId is pointing to.  It is an error to define any combination of expression, headerField and path.
      attr_accessor :path                           # 0-1 string
      ##
      # Fixture Id of source expression or headerField within this variable
      # Fixture to evaluate the XPath/JSONPath expression or the headerField  against within this variable.
      # This can be a statically defined fixture (at the top of the TestScript) or a dynamically set fixture created by responseId of the `action.operation` element.
      attr_accessor :sourceId                       # 0-1 id
    end

    ##
    # A series of required setup operations before tests are executed.
    class Setup < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Setup.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Setup.extension',
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
          'path'=>'Setup.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A setup operation or assert to perform
        # Action would contain either an operation or an assertion.
        # An action should contain either an operation or an assertion but not both.  It can contain any number of variables.
        'action' => {
          'type'=>'TestScript::Setup::Action',
          'path'=>'Setup.action',
          'min'=>1,
          'max'=>Float::INFINITY
        }
      }

      ##
      # A setup operation or assert to perform
      # Action would contain either an operation or an assertion.
      # An action should contain either an operation or an assertion but not both.  It can contain any number of variables.
      class Action < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Action.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Action.extension',
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
            'path'=>'Action.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # The setup operation to perform
          # The operation to perform.
          'operation' => {
            'type'=>'TestScript::Setup::Action::Operation',
            'path'=>'Action.operation',
            'min'=>0,
            'max'=>1
          },
          ##
          # The assertion to perform
          # Evaluates the results of previous operations to determine if the server under test behaves appropriately.
          # In order to evaluate an assertion, the request, response, and results of the most recently executed operation must always be maintained by the test engine.
          'assert' => {
            'type'=>'TestScript::Setup::Action::Assert',
            'path'=>'Action.assert',
            'min'=>0,
            'max'=>1
          }
        }

        ##
        # The setup operation to perform
        # The operation to perform.
        class Operation < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'id',
              'path'=>'Operation.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Operation.extension',
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
              'path'=>'Operation.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # The operation code type that will be executed
            # Server interaction or operation type.
            # See http://build.fhir.org/http.html for list of server interactions.
            'type' => {
              'valid_codes'=>{
                'http://terminology.hl7.org/CodeSystem/testscript-operation-codes'=>[ 'read', 'vread', 'update', 'updateCreate', 'patch', 'delete', 'deleteCondSingle', 'deleteCondMultiple', 'history', 'create', 'search', 'batch', 'transaction', 'capabilities', 'apply', 'closure', 'find-matches', 'conforms', 'data-requirements', 'document', 'evaluate', 'evaluate-measure', 'everything', 'expand', 'find', 'graphql', 'implements', 'lastn', 'lookup', 'match', 'meta', 'meta-add', 'meta-delete', 'populate', 'populatehtml', 'populatelink', 'process-message', 'questionnaire', 'stats', 'subset', 'subsumes', 'transform', 'translate', 'validate', 'validate-code' ]
              },
              'type'=>'Coding',
              'path'=>'Operation.type',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/testscript-operation-codes'}
            },
            ##
            # Resource type
            # The type of the resource.  See http://build.fhir.org/resourcelist.html.
            # If "url" element is specified, then "targetId", "params", and "resource" elements will be ignored as "url" element will have everything needed for constructing the request url.  If "params" element is specified, then "targetId" element is ignored. For FHIR operations that require a resource (e.g. "read" and "vread" operations), the "resource" element must be specified when "params" element is specified.  If "url" and "params" elements are absent, then the request url will be constructed from "targetId" fixture if present. For "read" operation, the resource and id values will be extracted from "targetId" fixture and used to construct the url. For "vread" and "history" operations, the versionId value will also be used.
            'resource' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/data-types'=>[ 'Address', 'Age', 'Annotation', 'Attachment', 'BackboneElement', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'Distance', 'Dosage', 'Duration', 'Element', 'ElementDefinition', 'Expression', 'Extension', 'HumanName', 'Identifier', 'MarketingStatus', 'Meta', 'Money', 'MoneyQuantity', 'Narrative', 'ParameterDefinition', 'Period', 'Population', 'ProdCharacteristic', 'ProductShelfLife', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'SimpleQuantity', 'SubstanceAmount', 'Timing', 'TriggerDefinition', 'UsageContext', 'base64Binary', 'boolean', 'canonical', 'code', 'date', 'dateTime', 'decimal', 'id', 'instant', 'integer', 'markdown', 'oid', 'positiveInt', 'string', 'time', 'unsignedInt', 'uri', 'url', 'uuid', 'xhtml' ],
                'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
              },
              'type'=>'code',
              'path'=>'Operation.resource',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/defined-types'}
            },
            ##
            # Tracking/logging operation label
            # The label would be used for tracking/logging purposes by test engines.
            # This has no impact on the verification itself.
            'label' => {
              'type'=>'string',
              'path'=>'Operation.label',
              'min'=>0,
              'max'=>1
            },
            ##
            # Tracking/reporting operation description
            # The description would be used by test engines for tracking and reporting purposes.
            # This has no impact on the verification itself.
            'description' => {
              'type'=>'string',
              'path'=>'Operation.description',
              'min'=>0,
              'max'=>1
            },
            ##
            # Mime type to accept in the payload of the response, with charset etc.
            # The mime-type to use for RESTful operation in the 'Accept' header.
            # If this is specified, then test engine shall set the 'Accept' header to the corresponding value.  If you'd like to explicitly set the 'Accept' to some other value then use the 'requestHeader' element.
            'accept' => {
              'type'=>'code',
              'path'=>'Operation.accept',
              'min'=>0,
              'max'=>1
            },
            ##
            # Mime type of the request payload contents, with charset etc.
            # The mime-type to use for RESTful operation in the 'Content-Type' header.
            # If this is specified, then test engine shall set the 'Content-Type' header to the corresponding value.  If you'd like to explicitly set the 'Content-Type' to some other value then use the 'requestHeader' element.
            'contentType' => {
              'type'=>'code',
              'path'=>'Operation.contentType',
              'min'=>0,
              'max'=>1
            },
            ##
            # Server responding to the request
            # The server where the request message is destined for.  Must be one of the server numbers listed in TestScript.destination section.
            # If multiple TestScript.destination elements are defined and operation.destination is undefined, test engine will report an error as it cannot determine what destination to use for the exchange.
            'destination' => {
              'type'=>'integer',
              'path'=>'Operation.destination',
              'min'=>0,
              'max'=>1
            },
            ##
            # Whether or not to send the request url in encoded format
            # Whether or not to implicitly send the request url in encoded format. The default is true to match the standard RESTful client behavior. Set to false when communicating with a server that does not support encoded url paths.
            'encodeRequestUrl' => {
              'type'=>'boolean',
              'path'=>'Operation.encodeRequestUrl',
              'min'=>1,
              'max'=>1
            },
            ##
            # delete | get | options | patch | post | put | head
            # The HTTP method the test engine MUST use for this operation regardless of any other operation details.
            # The primary purpose of the explicit HTTP method is support of  HTTP POST method invocation of the FHIR search. Other uses will include support of negative testing.
            'method' => {
              'local_name'=>'local_method'
              'valid_codes'=>{
                'http://hl7.org/fhir/http-operations'=>[ 'delete', 'get', 'options', 'patch', 'post', 'put', 'head' ]
              },
              'type'=>'code',
              'path'=>'Operation.method',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/http-operations'}
            },
            ##
            # Server initiating the request
            # The server where the request message originates from.  Must be one of the server numbers listed in TestScript.origin section.
            # If absent, test engine will send the message.  When present, test engine will not send the request message but will wait for the request message to be sent from this origin server.
            'origin' => {
              'type'=>'integer',
              'path'=>'Operation.origin',
              'min'=>0,
              'max'=>1
            },
            ##
            # Explicitly defined path parameters
            # Path plus parameters after [type].  Used to set parts of the request URL explicitly.
            # If "url" element is specified, then "targetId", "params", and "resource" elements will be ignored as "url" element will have everything needed for constructing the request url.  If "params" element is specified, then "targetId" element is ignored.  For FHIR operations that require a resource (e.g. "read" and "vread" operations), the "resource" element must be specified when "params" element is specified.  If "url" and "params" elements are absent, then the request url will be constructed from "targetId" fixture if present.  For "read" operation, the resource and id values will be extracted from "targetId" fixture and used to construct the url.  For "vread" and "history" operations, the versionId value will also be used.   Test engines would append whatever is specified for "params" to the URL after the resource type without tampering with the string (beyond encoding the URL for HTTP).  The "params" element does not correspond exactly to "search parameters".  Nor is it the "path".  It corresponds to the part of the URL that comes after the [type] (when "resource" element is specified); e.g. It corresponds to "/[id]/_history/[vid] {?_format=[mime-type]}" in the following operation: GET [base]/[type]/[id]/_history/[vid] {?_format=[mime-type]}  Test engines do have to look for placeholders (${}) and replace the variable placeholders with the variable values at runtime before sending the request.
            'params' => {
              'type'=>'string',
              'path'=>'Operation.params',
              'min'=>0,
              'max'=>1
            },
            ##
            # Each operation can have one or more header elements
            # Header elements would be used to set HTTP headers.
            # This gives control to test-script writers to set headers explicitly based on test requirements.  It will allow for testing using:  - "If-Modified-Since" and "If-None-Match" headers.  See http://build.fhir.org/http.html#2.1.0.5.1 - "If-Match" header.  See http://build.fhir.org/http.html#2.1.0.11 - Conditional Create using "If-None-Exist".  See http://build.fhir.org/http.html#2.1.0.13.1 - Invalid "Content-Type" header for negative testing. - etc.
            'requestHeader' => {
              'type'=>'TestScript::Setup::Action::Operation::RequestHeader',
              'path'=>'Operation.requestHeader',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Fixture Id of mapped request
            # The fixture id (maybe new) to map to the request.
            # If a requestId is supplied, then the resulting request (both headers and body) is mapped to the fixture ID (which may be entirely new and previously undeclared) designated by "requestId".  If requestId is not specified, it is the test engine's responsibility to store the request and use it as the requestId in subsequent assertions when assertion path and/or headerField is specified, direction is equal to request, and the requestId in not specified.
            'requestId' => {
              'type'=>'id',
              'path'=>'Operation.requestId',
              'min'=>0,
              'max'=>1
            },
            ##
            # Fixture Id of mapped response
            # The fixture id (maybe new) to map to the response.
            # If a responseId is supplied, and the server responds, then the resulting response (both headers and body) is mapped to the fixture ID (which may be entirely new and previously undeclared) designated by "responseId".  If responseId is not specified, it is the test engine's responsibility to store the response and use it as the responseId in subsequent assertions when assertion path and/or headerField is specified and the responseId is not specified.
            'responseId' => {
              'type'=>'id',
              'path'=>'Operation.responseId',
              'min'=>0,
              'max'=>1
            },
            ##
            # Fixture Id of body for PUT and POST requests
            # The id of the fixture used as the body of a PUT or POST request.
            'sourceId' => {
              'type'=>'id',
              'path'=>'Operation.sourceId',
              'min'=>0,
              'max'=>1
            },
            ##
            # Id of fixture used for extracting the [id],  [type], and [vid] for GET requests.
            # If "url" element is specified, then "targetId", "params", and "resource" elements will be ignored as "url" element will have everything needed for constructing the request url.  If "params" element is specified, then "targetId" element is ignored.  For FHIR operations that require a resource (e.g. "read" and "vread" operations), the "resource" element must be specified when "params" element is specified.  If "url" and "params" elements are absent, then the request url will be constructed from "targetId" fixture if present.  For "read" operation, the resource and id values will be extracted from "targetId" fixture and used to construct the url.  For "vread" and "history" operations, the versionId value will also be used.
            'targetId' => {
              'type'=>'id',
              'path'=>'Operation.targetId',
              'min'=>0,
              'max'=>1
            },
            ##
            # Request URL
            # Complete request URL.
            # Used to set the request URL explicitly.  If "url" element is defined, then "targetId", "resource", and "params" elements will be ignored.  Test engines would use whatever is specified in "url" without tampering with the string (beyond encoding the URL for HTTP).  Test engines do have to look for placeholders (${}) and replace the variable placeholders with the variable values at runtime before sending the request.
            'url' => {
              'type'=>'string',
              'path'=>'Operation.url',
              'min'=>0,
              'max'=>1
            }
          }

          ##
          # Each operation can have one or more header elements
          # Header elements would be used to set HTTP headers.
          # This gives control to test-script writers to set headers explicitly based on test requirements.  It will allow for testing using:  - "If-Modified-Since" and "If-None-Match" headers.  See http://build.fhir.org/http.html#2.1.0.5.1 - "If-Match" header.  See http://build.fhir.org/http.html#2.1.0.11 - Conditional Create using "If-None-Exist".  See http://build.fhir.org/http.html#2.1.0.13.1 - Invalid "Content-Type" header for negative testing. - etc.
          class RequestHeader < FHIR::Model
            include FHIR::Hashable
            include FHIR::Json
            include FHIR::Xml

            METADATA = {
              ##
              # Unique id for inter-element referencing
              # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
              'id' => {
                'type'=>'id',
                'path'=>'RequestHeader.id',
                'min'=>0,
                'max'=>1
              },
              ##
              # Additional content defined by implementations
              # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
              # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
              'extension' => {
                'type'=>'Extension',
                'path'=>'RequestHeader.extension',
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
                'path'=>'RequestHeader.modifierExtension',
                'min'=>0,
                'max'=>Float::INFINITY
              },
              ##
              # HTTP header field name
              # The HTTP header field e.g. "Accept".
              # If header element is specified, then field is required.
              'field' => {
                'type'=>'string',
                'path'=>'RequestHeader.field',
                'min'=>1,
                'max'=>1
              },
              ##
              # HTTP headerfield value
              # The value of the header e.g. "application/fhir+xml".
              # If header element is specified, then value is required.  No conversions will be done by the test engine e.g. "xml" to "application/fhir+xml".  The values will be set in HTTP headers "as-is".  Test engines do have to look for placeholders (${}) and replace the variable placeholders with the variable values at runtime before sending the request.
              'value' => {
                'type'=>'string',
                'path'=>'RequestHeader.value',
                'min'=>1,
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
            # HTTP header field name
            # The HTTP header field e.g. "Accept".
            # If header element is specified, then field is required.
            attr_accessor :field                          # 1-1 string
            ##
            # HTTP headerfield value
            # The value of the header e.g. "application/fhir+xml".
            # If header element is specified, then value is required.  No conversions will be done by the test engine e.g. "xml" to "application/fhir+xml".  The values will be set in HTTP headers "as-is".  Test engines do have to look for placeholders (${}) and replace the variable placeholders with the variable values at runtime before sending the request.
            attr_accessor :value                          # 1-1 string
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
          # The operation code type that will be executed
          # Server interaction or operation type.
          # See http://build.fhir.org/http.html for list of server interactions.
          attr_accessor :type                           # 0-1 Coding
          ##
          # Resource type
          # The type of the resource.  See http://build.fhir.org/resourcelist.html.
          # If "url" element is specified, then "targetId", "params", and "resource" elements will be ignored as "url" element will have everything needed for constructing the request url.  If "params" element is specified, then "targetId" element is ignored. For FHIR operations that require a resource (e.g. "read" and "vread" operations), the "resource" element must be specified when "params" element is specified.  If "url" and "params" elements are absent, then the request url will be constructed from "targetId" fixture if present. For "read" operation, the resource and id values will be extracted from "targetId" fixture and used to construct the url. For "vread" and "history" operations, the versionId value will also be used.
          attr_accessor :resource                       # 0-1 code
          ##
          # Tracking/logging operation label
          # The label would be used for tracking/logging purposes by test engines.
          # This has no impact on the verification itself.
          attr_accessor :label                          # 0-1 string
          ##
          # Tracking/reporting operation description
          # The description would be used by test engines for tracking and reporting purposes.
          # This has no impact on the verification itself.
          attr_accessor :description                    # 0-1 string
          ##
          # Mime type to accept in the payload of the response, with charset etc.
          # The mime-type to use for RESTful operation in the 'Accept' header.
          # If this is specified, then test engine shall set the 'Accept' header to the corresponding value.  If you'd like to explicitly set the 'Accept' to some other value then use the 'requestHeader' element.
          attr_accessor :accept                         # 0-1 code
          ##
          # Mime type of the request payload contents, with charset etc.
          # The mime-type to use for RESTful operation in the 'Content-Type' header.
          # If this is specified, then test engine shall set the 'Content-Type' header to the corresponding value.  If you'd like to explicitly set the 'Content-Type' to some other value then use the 'requestHeader' element.
          attr_accessor :contentType                    # 0-1 code
          ##
          # Server responding to the request
          # The server where the request message is destined for.  Must be one of the server numbers listed in TestScript.destination section.
          # If multiple TestScript.destination elements are defined and operation.destination is undefined, test engine will report an error as it cannot determine what destination to use for the exchange.
          attr_accessor :destination                    # 0-1 integer
          ##
          # Whether or not to send the request url in encoded format
          # Whether or not to implicitly send the request url in encoded format. The default is true to match the standard RESTful client behavior. Set to false when communicating with a server that does not support encoded url paths.
          attr_accessor :encodeRequestUrl               # 1-1 boolean
          ##
          # delete | get | options | patch | post | put | head
          # The HTTP method the test engine MUST use for this operation regardless of any other operation details.
          # The primary purpose of the explicit HTTP method is support of  HTTP POST method invocation of the FHIR search. Other uses will include support of negative testing.
          attr_accessor :local_method                   # 0-1 code
          ##
          # Server initiating the request
          # The server where the request message originates from.  Must be one of the server numbers listed in TestScript.origin section.
          # If absent, test engine will send the message.  When present, test engine will not send the request message but will wait for the request message to be sent from this origin server.
          attr_accessor :origin                         # 0-1 integer
          ##
          # Explicitly defined path parameters
          # Path plus parameters after [type].  Used to set parts of the request URL explicitly.
          # If "url" element is specified, then "targetId", "params", and "resource" elements will be ignored as "url" element will have everything needed for constructing the request url.  If "params" element is specified, then "targetId" element is ignored.  For FHIR operations that require a resource (e.g. "read" and "vread" operations), the "resource" element must be specified when "params" element is specified.  If "url" and "params" elements are absent, then the request url will be constructed from "targetId" fixture if present.  For "read" operation, the resource and id values will be extracted from "targetId" fixture and used to construct the url.  For "vread" and "history" operations, the versionId value will also be used.   Test engines would append whatever is specified for "params" to the URL after the resource type without tampering with the string (beyond encoding the URL for HTTP).  The "params" element does not correspond exactly to "search parameters".  Nor is it the "path".  It corresponds to the part of the URL that comes after the [type] (when "resource" element is specified); e.g. It corresponds to "/[id]/_history/[vid] {?_format=[mime-type]}" in the following operation: GET [base]/[type]/[id]/_history/[vid] {?_format=[mime-type]}  Test engines do have to look for placeholders (${}) and replace the variable placeholders with the variable values at runtime before sending the request.
          attr_accessor :params                         # 0-1 string
          ##
          # Each operation can have one or more header elements
          # Header elements would be used to set HTTP headers.
          # This gives control to test-script writers to set headers explicitly based on test requirements.  It will allow for testing using:  - "If-Modified-Since" and "If-None-Match" headers.  See http://build.fhir.org/http.html#2.1.0.5.1 - "If-Match" header.  See http://build.fhir.org/http.html#2.1.0.11 - Conditional Create using "If-None-Exist".  See http://build.fhir.org/http.html#2.1.0.13.1 - Invalid "Content-Type" header for negative testing. - etc.
          attr_accessor :requestHeader                  # 0-* [ TestScript::Setup::Action::Operation::RequestHeader ]
          ##
          # Fixture Id of mapped request
          # The fixture id (maybe new) to map to the request.
          # If a requestId is supplied, then the resulting request (both headers and body) is mapped to the fixture ID (which may be entirely new and previously undeclared) designated by "requestId".  If requestId is not specified, it is the test engine's responsibility to store the request and use it as the requestId in subsequent assertions when assertion path and/or headerField is specified, direction is equal to request, and the requestId in not specified.
          attr_accessor :requestId                      # 0-1 id
          ##
          # Fixture Id of mapped response
          # The fixture id (maybe new) to map to the response.
          # If a responseId is supplied, and the server responds, then the resulting response (both headers and body) is mapped to the fixture ID (which may be entirely new and previously undeclared) designated by "responseId".  If responseId is not specified, it is the test engine's responsibility to store the response and use it as the responseId in subsequent assertions when assertion path and/or headerField is specified and the responseId is not specified.
          attr_accessor :responseId                     # 0-1 id
          ##
          # Fixture Id of body for PUT and POST requests
          # The id of the fixture used as the body of a PUT or POST request.
          attr_accessor :sourceId                       # 0-1 id
          ##
          # Id of fixture used for extracting the [id],  [type], and [vid] for GET requests.
          # If "url" element is specified, then "targetId", "params", and "resource" elements will be ignored as "url" element will have everything needed for constructing the request url.  If "params" element is specified, then "targetId" element is ignored.  For FHIR operations that require a resource (e.g. "read" and "vread" operations), the "resource" element must be specified when "params" element is specified.  If "url" and "params" elements are absent, then the request url will be constructed from "targetId" fixture if present.  For "read" operation, the resource and id values will be extracted from "targetId" fixture and used to construct the url.  For "vread" and "history" operations, the versionId value will also be used.
          attr_accessor :targetId                       # 0-1 id
          ##
          # Request URL
          # Complete request URL.
          # Used to set the request URL explicitly.  If "url" element is defined, then "targetId", "resource", and "params" elements will be ignored.  Test engines would use whatever is specified in "url" without tampering with the string (beyond encoding the URL for HTTP).  Test engines do have to look for placeholders (${}) and replace the variable placeholders with the variable values at runtime before sending the request.
          attr_accessor :url                            # 0-1 string
        end

        ##
        # The assertion to perform
        # Evaluates the results of previous operations to determine if the server under test behaves appropriately.
        # In order to evaluate an assertion, the request, response, and results of the most recently executed operation must always be maintained by the test engine.
        class Assert < FHIR::Model
          include FHIR::Hashable
          include FHIR::Json
          include FHIR::Xml

          METADATA = {
            ##
            # Unique id for inter-element referencing
            # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
            'id' => {
              'type'=>'id',
              'path'=>'Assert.id',
              'min'=>0,
              'max'=>1
            },
            ##
            # Additional content defined by implementations
            # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
            # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
            'extension' => {
              'type'=>'Extension',
              'path'=>'Assert.extension',
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
              'path'=>'Assert.modifierExtension',
              'min'=>0,
              'max'=>Float::INFINITY
            },
            ##
            # Tracking/logging assertion label
            # The label would be used for tracking/logging purposes by test engines.
            # This has no impact on the verification itself.
            'label' => {
              'type'=>'string',
              'path'=>'Assert.label',
              'min'=>0,
              'max'=>1
            },
            ##
            # Tracking/reporting assertion description
            # The description would be used by test engines for tracking and reporting purposes.
            # This has no impact on the verification itself.
            'description' => {
              'type'=>'string',
              'path'=>'Assert.description',
              'min'=>0,
              'max'=>1
            },
            ##
            # response | request
            # The direction to use for the assertion.
            # If the direction is specified as "response" (the default), then the processing of this assert is against the received response message. If the direction is specified as "request", then the processing of this assert is against the sent request message.
            'direction' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/assert-direction-codes'=>[ 'response', 'request' ]
              },
              'type'=>'code',
              'path'=>'Assert.direction',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/assert-direction-codes'}
            },
            ##
            # Id of the source fixture to be evaluated
            # Id of the source fixture used as the contents to be evaluated by either the "source/expression" or "sourceId/path" definition.
            'compareToSourceId' => {
              'type'=>'string',
              'path'=>'Assert.compareToSourceId',
              'min'=>0,
              'max'=>1
            },
            ##
            # The FHIRPath expression to evaluate against the source fixture. When compareToSourceId is defined, either compareToSourceExpression or compareToSourcePath must be defined, but not both.
            # Thefhirpath expression to be evaluated against the expected fixture to compare to. Ignored if "assert.value" is used. The evaluation will be done before the assertion is evaluated.
            'compareToSourceExpression' => {
              'type'=>'string',
              'path'=>'Assert.compareToSourceExpression',
              'min'=>0,
              'max'=>1
            },
            ##
            # XPath or JSONPath expression to evaluate against the source fixture. When compareToSourceId is defined, either compareToSourceExpression or compareToSourcePath must be defined, but not both.
            # The XPath or JSONPath expression to be evaluated against the expected fixture to compare to. Ignored if "assert.value" is used. The evaluation will be done before the assertion is evaluated.
            'compareToSourcePath' => {
              'type'=>'string',
              'path'=>'Assert.compareToSourcePath',
              'min'=>0,
              'max'=>1
            },
            ##
            # Mime type to compare against the 'Content-Type' header
            # The mime-type contents to compare against the request or response message 'Content-Type' header.
            # If this is specified, then test engine shall confirm that the content-type of the last operation's headers is set to this value.  If "assert.sourceId" element is specified, then the evaluation will be done against the headers mapped to that sourceId (and not the last operation's headers).  If you'd like to have more control over the string, then use 'assert.headerField' instead.
            'contentType' => {
              'type'=>'code',
              'path'=>'Assert.contentType',
              'min'=>0,
              'max'=>1
            },
            ##
            # The FHIRPath expression to be evaluated against the request or response message contents - HTTP headers and payload.
            # If both "expression" and a "fixtureId" are specified, then the expression will be evaluated against the request or response body mapped to the fixtureId.  If "expression" is specified and a "fixtureId" is not, then the expression will be evaluated against the response body of the last operation.  Test engines are to store the request and response body and headers of the last operation at all times for subsequent assertions.
            'expression' => {
              'type'=>'string',
              'path'=>'Assert.expression',
              'min'=>0,
              'max'=>1
            },
            ##
            # HTTP header field name
            # The HTTP header field name e.g. 'Location'.
            # If "headerField" is specified then "value" must be specified.  If "sourceId" is not specified, then "headerField" will be evaluated against the last operation's response headers.  Test engines are to keep track of the last operation's response body and response headers.
            'headerField' => {
              'type'=>'string',
              'path'=>'Assert.headerField',
              'min'=>0,
              'max'=>1
            },
            ##
            # Fixture Id of minimum content resource
            # The ID of a fixture.  Asserts that the response contains at a minimum the fixture specified by minimumId.
            # Asserts that the response contains all the element/content in another fixture pointed to by minimumId.  This can be a statically defined fixture or one that is dynamically set via responseId.
            'minimumId' => {
              'type'=>'string',
              'path'=>'Assert.minimumId',
              'min'=>0,
              'max'=>1
            },
            ##
            # Perform validation on navigation links?
            # Whether or not the test execution performs validation on the bundle navigation links.
            # Asserts that the Bundle contains first, last, and next links.
            'navigationLinks' => {
              'type'=>'boolean',
              'path'=>'Assert.navigationLinks',
              'min'=>0,
              'max'=>1
            },
            ##
            # equals | notEquals | in | notIn | greaterThan | lessThan | empty | notEmpty | contains | notContains | eval
            # The operator type defines the conditional behavior of the assert. If not defined, the default is equals.
            # Operators are useful especially for negative testing.  If operator is not specified, then the "equals" operator is assumed; e.g. ```<code>   <assert>  <operator value="in" />  <responseCode value="200,201,204" />    </assert>    <assert>  <operator value="notEquals" />  <response value="okay"/>   </assert>    <assert>  <operator value="greaterThan" />    <responseHeader>     <field value="Content-Length" />     <value value="0" />    </responseHeader/>   </assert> </code> ```.
            'operator' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/assert-operator-codes'=>[ 'equals', 'notEquals', 'in', 'notIn', 'greaterThan', 'lessThan', 'empty', 'notEmpty', 'contains', 'notContains', 'eval' ]
              },
              'type'=>'code',
              'path'=>'Assert.operator',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/assert-operator-codes'}
            },
            ##
            # XPath or JSONPath expression
            # The XPath or JSONPath expression to be evaluated against the fixture representing the response received from server.
            # If both "path" and a "fixtureId" are specified, then the path will be evaluated against the request or response body mapped to the fixtureId.  If "path" is specified and a "fixtureId" is not, then the path will be evaluated against the response body of the last operation.  Test engines are to store the request and response body and headers of the last operation at all times for subsequent assertions.
            'path' => {
              'type'=>'string',
              'path'=>'Assert.path',
              'min'=>0,
              'max'=>1
            },
            ##
            # delete | get | options | patch | post | put | head
            # The request method or HTTP operation code to compare against that used by the client system under test.
            # If "requestMethod" is specified then it will be used in place of "value". The "requestMethod" will evaluate against the last operation's request HTTP operation.
            'requestMethod' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/http-operations'=>[ 'delete', 'get', 'options', 'patch', 'post', 'put', 'head' ]
              },
              'type'=>'code',
              'path'=>'Assert.requestMethod',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/http-operations'}
            },
            ##
            # Request URL comparison value
            # The value to use in a comparison against the request URL path string.
            # If "requestURL" is specified then it will be used in place of "value". The "requestURL" will evaluate against the last operation's full request URL path string.
            'requestURL' => {
              'type'=>'string',
              'path'=>'Assert.requestURL',
              'min'=>0,
              'max'=>1
            },
            ##
            # Resource type
            # The type of the resource.  See http://build.fhir.org/resourcelist.html.
            # This will be expected resource type in response body e.g. in read, vread, search, etc.  See http://build.fhir.org/resourcelist.html for complete list of resource types; e.g. <assert > <resourceType value="Patient" </assert>.
            'resource' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/data-types'=>[ 'Address', 'Age', 'Annotation', 'Attachment', 'BackboneElement', 'CodeableConcept', 'Coding', 'ContactDetail', 'ContactPoint', 'Contributor', 'Count', 'DataRequirement', 'Distance', 'Dosage', 'Duration', 'Element', 'ElementDefinition', 'Expression', 'Extension', 'HumanName', 'Identifier', 'MarketingStatus', 'Meta', 'Money', 'MoneyQuantity', 'Narrative', 'ParameterDefinition', 'Period', 'Population', 'ProdCharacteristic', 'ProductShelfLife', 'Quantity', 'Range', 'Ratio', 'Reference', 'RelatedArtifact', 'SampledData', 'Signature', 'SimpleQuantity', 'SubstanceAmount', 'Timing', 'TriggerDefinition', 'UsageContext', 'base64Binary', 'boolean', 'canonical', 'code', 'date', 'dateTime', 'decimal', 'id', 'instant', 'integer', 'markdown', 'oid', 'positiveInt', 'string', 'time', 'unsignedInt', 'uri', 'url', 'uuid', 'xhtml' ],
                'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
              },
              'type'=>'code',
              'path'=>'Assert.resource',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/defined-types'}
            },
            ##
            # okay | created | noContent | notModified | bad | forbidden | notFound | methodNotAllowed | conflict | gone | preconditionFailed | unprocessable.
            # This is a shorter way of achieving similar verifications via "assert.responseCode".  If you need more control, then use "assert.responseCode"  e.g. <assert>  <contentType value="json" />  <response value="okay"/> </assert>.
            'response' => {
              'valid_codes'=>{
                'http://hl7.org/fhir/assert-response-code-types'=>[ 'okay', 'created', 'noContent', 'notModified', 'bad', 'forbidden', 'notFound', 'methodNotAllowed', 'conflict', 'gone', 'preconditionFailed', 'unprocessable' ]
              },
              'type'=>'code',
              'path'=>'Assert.response',
              'min'=>0,
              'max'=>1,
              'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/assert-response-code-types'}
            },
            ##
            # HTTP response code to test
            # The value of the HTTP response code to be tested.
            # To be used with "operator" attribute value. Asserts that the response code equals this value if "operator" is not specified.   If the operator is "in" or "notIn" then the responseCode would be a comma-separated list of values e.g. "200,201". Otherwise, it's expected to be a numeric value.   If "fixture" is not specified, then the "responseBodyId" value of the last operation is assumed.
            'responseCode' => {
              'type'=>'string',
              'path'=>'Assert.responseCode',
              'min'=>0,
              'max'=>1
            },
            ##
            # Fixture Id of source expression or headerField
            # Fixture to evaluate the XPath/JSONPath expression or the headerField  against.
            # This can be a statically defined fixture (at the top of the testscript) or a dynamically set fixture created by responseId of the action.operation element.
            'sourceId' => {
              'type'=>'id',
              'path'=>'Assert.sourceId',
              'min'=>0,
              'max'=>1
            },
            ##
            # Profile Id of validation profile reference
            # The ID of the Profile to validate against.
            # The ID of a Profile fixture. Asserts that the response is valid according to the Profile specified by validateProfileId.
            'validateProfileId' => {
              'type'=>'id',
              'path'=>'Assert.validateProfileId',
              'min'=>0,
              'max'=>1
            },
            ##
            # The value to compare to.
            # The string-representation of a number, string, or boolean that is expected.  Test engines do have to look for placeholders (${}) and replace the variable placeholders with the variable values at runtime before comparing this value to the actual value.
            'value' => {
              'type'=>'string',
              'path'=>'Assert.value',
              'min'=>0,
              'max'=>1
            },
            ##
            # Will this assert produce a warning only on error?
            # Whether or not the test execution will produce a warning only on error for this assert.
            # If this element is specified and it is true, then assertion failures can be logged by test engine but should not stop the test script execution from proceeding.  There are likely cases where the spec is not clear on what should happen. If the spec says something is optional (maybe a response header for example), but a server doesn’t do it, we could choose to issue a warning.
            'warningOnly' => {
              'type'=>'boolean',
              'path'=>'Assert.warningOnly',
              'min'=>1,
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
          # Tracking/logging assertion label
          # The label would be used for tracking/logging purposes by test engines.
          # This has no impact on the verification itself.
          attr_accessor :label                          # 0-1 string
          ##
          # Tracking/reporting assertion description
          # The description would be used by test engines for tracking and reporting purposes.
          # This has no impact on the verification itself.
          attr_accessor :description                    # 0-1 string
          ##
          # response | request
          # The direction to use for the assertion.
          # If the direction is specified as "response" (the default), then the processing of this assert is against the received response message. If the direction is specified as "request", then the processing of this assert is against the sent request message.
          attr_accessor :direction                      # 0-1 code
          ##
          # Id of the source fixture to be evaluated
          # Id of the source fixture used as the contents to be evaluated by either the "source/expression" or "sourceId/path" definition.
          attr_accessor :compareToSourceId              # 0-1 string
          ##
          # The FHIRPath expression to evaluate against the source fixture. When compareToSourceId is defined, either compareToSourceExpression or compareToSourcePath must be defined, but not both.
          # Thefhirpath expression to be evaluated against the expected fixture to compare to. Ignored if "assert.value" is used. The evaluation will be done before the assertion is evaluated.
          attr_accessor :compareToSourceExpression      # 0-1 string
          ##
          # XPath or JSONPath expression to evaluate against the source fixture. When compareToSourceId is defined, either compareToSourceExpression or compareToSourcePath must be defined, but not both.
          # The XPath or JSONPath expression to be evaluated against the expected fixture to compare to. Ignored if "assert.value" is used. The evaluation will be done before the assertion is evaluated.
          attr_accessor :compareToSourcePath            # 0-1 string
          ##
          # Mime type to compare against the 'Content-Type' header
          # The mime-type contents to compare against the request or response message 'Content-Type' header.
          # If this is specified, then test engine shall confirm that the content-type of the last operation's headers is set to this value.  If "assert.sourceId" element is specified, then the evaluation will be done against the headers mapped to that sourceId (and not the last operation's headers).  If you'd like to have more control over the string, then use 'assert.headerField' instead.
          attr_accessor :contentType                    # 0-1 code
          ##
          # The FHIRPath expression to be evaluated against the request or response message contents - HTTP headers and payload.
          # If both "expression" and a "fixtureId" are specified, then the expression will be evaluated against the request or response body mapped to the fixtureId.  If "expression" is specified and a "fixtureId" is not, then the expression will be evaluated against the response body of the last operation.  Test engines are to store the request and response body and headers of the last operation at all times for subsequent assertions.
          attr_accessor :expression                     # 0-1 string
          ##
          # HTTP header field name
          # The HTTP header field name e.g. 'Location'.
          # If "headerField" is specified then "value" must be specified.  If "sourceId" is not specified, then "headerField" will be evaluated against the last operation's response headers.  Test engines are to keep track of the last operation's response body and response headers.
          attr_accessor :headerField                    # 0-1 string
          ##
          # Fixture Id of minimum content resource
          # The ID of a fixture.  Asserts that the response contains at a minimum the fixture specified by minimumId.
          # Asserts that the response contains all the element/content in another fixture pointed to by minimumId.  This can be a statically defined fixture or one that is dynamically set via responseId.
          attr_accessor :minimumId                      # 0-1 string
          ##
          # Perform validation on navigation links?
          # Whether or not the test execution performs validation on the bundle navigation links.
          # Asserts that the Bundle contains first, last, and next links.
          attr_accessor :navigationLinks                # 0-1 boolean
          ##
          # equals | notEquals | in | notIn | greaterThan | lessThan | empty | notEmpty | contains | notContains | eval
          # The operator type defines the conditional behavior of the assert. If not defined, the default is equals.
          # Operators are useful especially for negative testing.  If operator is not specified, then the "equals" operator is assumed; e.g. ```<code>   <assert>  <operator value="in" />  <responseCode value="200,201,204" />    </assert>    <assert>  <operator value="notEquals" />  <response value="okay"/>   </assert>    <assert>  <operator value="greaterThan" />    <responseHeader>     <field value="Content-Length" />     <value value="0" />    </responseHeader/>   </assert> </code> ```.
          attr_accessor :operator                       # 0-1 code
          ##
          # XPath or JSONPath expression
          # The XPath or JSONPath expression to be evaluated against the fixture representing the response received from server.
          # If both "path" and a "fixtureId" are specified, then the path will be evaluated against the request or response body mapped to the fixtureId.  If "path" is specified and a "fixtureId" is not, then the path will be evaluated against the response body of the last operation.  Test engines are to store the request and response body and headers of the last operation at all times for subsequent assertions.
          attr_accessor :path                           # 0-1 string
          ##
          # delete | get | options | patch | post | put | head
          # The request method or HTTP operation code to compare against that used by the client system under test.
          # If "requestMethod" is specified then it will be used in place of "value". The "requestMethod" will evaluate against the last operation's request HTTP operation.
          attr_accessor :requestMethod                  # 0-1 code
          ##
          # Request URL comparison value
          # The value to use in a comparison against the request URL path string.
          # If "requestURL" is specified then it will be used in place of "value". The "requestURL" will evaluate against the last operation's full request URL path string.
          attr_accessor :requestURL                     # 0-1 string
          ##
          # Resource type
          # The type of the resource.  See http://build.fhir.org/resourcelist.html.
          # This will be expected resource type in response body e.g. in read, vread, search, etc.  See http://build.fhir.org/resourcelist.html for complete list of resource types; e.g. <assert > <resourceType value="Patient" </assert>.
          attr_accessor :resource                       # 0-1 code
          ##
          # okay | created | noContent | notModified | bad | forbidden | notFound | methodNotAllowed | conflict | gone | preconditionFailed | unprocessable.
          # This is a shorter way of achieving similar verifications via "assert.responseCode".  If you need more control, then use "assert.responseCode"  e.g. <assert>  <contentType value="json" />  <response value="okay"/> </assert>.
          attr_accessor :response                       # 0-1 code
          ##
          # HTTP response code to test
          # The value of the HTTP response code to be tested.
          # To be used with "operator" attribute value. Asserts that the response code equals this value if "operator" is not specified.   If the operator is "in" or "notIn" then the responseCode would be a comma-separated list of values e.g. "200,201". Otherwise, it's expected to be a numeric value.   If "fixture" is not specified, then the "responseBodyId" value of the last operation is assumed.
          attr_accessor :responseCode                   # 0-1 string
          ##
          # Fixture Id of source expression or headerField
          # Fixture to evaluate the XPath/JSONPath expression or the headerField  against.
          # This can be a statically defined fixture (at the top of the testscript) or a dynamically set fixture created by responseId of the action.operation element.
          attr_accessor :sourceId                       # 0-1 id
          ##
          # Profile Id of validation profile reference
          # The ID of the Profile to validate against.
          # The ID of a Profile fixture. Asserts that the response is valid according to the Profile specified by validateProfileId.
          attr_accessor :validateProfileId              # 0-1 id
          ##
          # The value to compare to.
          # The string-representation of a number, string, or boolean that is expected.  Test engines do have to look for placeholders (${}) and replace the variable placeholders with the variable values at runtime before comparing this value to the actual value.
          attr_accessor :value                          # 0-1 string
          ##
          # Will this assert produce a warning only on error?
          # Whether or not the test execution will produce a warning only on error for this assert.
          # If this element is specified and it is true, then assertion failures can be logged by test engine but should not stop the test script execution from proceeding.  There are likely cases where the spec is not clear on what should happen. If the spec says something is optional (maybe a response header for example), but a server doesn’t do it, we could choose to issue a warning.
          attr_accessor :warningOnly                    # 1-1 boolean
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
        # The setup operation to perform
        # The operation to perform.
        attr_accessor :operation                      # 0-1 TestScript::Setup::Action::Operation
        ##
        # The assertion to perform
        # Evaluates the results of previous operations to determine if the server under test behaves appropriately.
        # In order to evaluate an assertion, the request, response, and results of the most recently executed operation must always be maintained by the test engine.
        attr_accessor :assert                         # 0-1 TestScript::Setup::Action::Assert
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
      # A setup operation or assert to perform
      # Action would contain either an operation or an assertion.
      # An action should contain either an operation or an assertion but not both.  It can contain any number of variables.
      attr_accessor :action                         # 1-* [ TestScript::Setup::Action ]
    end

    ##
    # A test in this script.
    class Test < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Test.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Test.extension',
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
          'path'=>'Test.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Tracking/logging name of this test
        # The name of this test used for tracking/logging purposes by test engines.
        'name' => {
          'type'=>'string',
          'path'=>'Test.name',
          'min'=>0,
          'max'=>1
        },
        ##
        # Tracking/reporting short description of the test
        # A short description of the test used by test engines for tracking and reporting purposes.
        'description' => {
          'type'=>'string',
          'path'=>'Test.description',
          'min'=>0,
          'max'=>1
        },
        ##
        # A test operation or assert to perform
        # Action would contain either an operation or an assertion.
        # An action should contain either an operation or an assertion but not both.  It can contain any number of variables.
        'action' => {
          'type'=>'TestScript::Test::Action',
          'path'=>'Test.action',
          'min'=>1,
          'max'=>Float::INFINITY
        }
      }

      ##
      # A test operation or assert to perform
      # Action would contain either an operation or an assertion.
      # An action should contain either an operation or an assertion but not both.  It can contain any number of variables.
      class Action < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Action.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Action.extension',
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
            'path'=>'Action.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # The setup operation to perform
          # An operation would involve a REST request to a server.
          'operation' => {
            'type'=>'TestScript::Setup::Action::Operation',
            'path'=>'Action.operation',
            'min'=>0,
            'max'=>1
          },
          ##
          # The setup assertion to perform
          # Evaluates the results of previous operations to determine if the server under test behaves appropriately.
          # In order to evaluate an assertion, the request, response, and results of the most recently executed operation must always be maintained by the test engine.
          'assert' => {
            'type'=>'TestScript::Setup::Action::Assert',
            'path'=>'Action.assert',
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
        # The setup operation to perform
        # An operation would involve a REST request to a server.
        attr_accessor :operation                      # 0-1 TestScript::Setup::Action::Operation
        ##
        # The setup assertion to perform
        # Evaluates the results of previous operations to determine if the server under test behaves appropriately.
        # In order to evaluate an assertion, the request, response, and results of the most recently executed operation must always be maintained by the test engine.
        attr_accessor :assert                         # 0-1 TestScript::Setup::Action::Assert
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
      # Tracking/logging name of this test
      # The name of this test used for tracking/logging purposes by test engines.
      attr_accessor :name                           # 0-1 string
      ##
      # Tracking/reporting short description of the test
      # A short description of the test used by test engines for tracking and reporting purposes.
      attr_accessor :description                    # 0-1 string
      ##
      # A test operation or assert to perform
      # Action would contain either an operation or an assertion.
      # An action should contain either an operation or an assertion but not both.  It can contain any number of variables.
      attr_accessor :action                         # 1-* [ TestScript::Test::Action ]
    end

    ##
    # A series of required clean up steps
    # A series of operations required to clean up after all the tests are executed (successfully or otherwise).
    class Teardown < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Teardown.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Teardown.extension',
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
          'path'=>'Teardown.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # One or more teardown operations to perform
        # The teardown action will only contain an operation.
        # An action should contain either an operation or an assertion but not both.  It can contain any number of variables.
        'action' => {
          'type'=>'TestScript::Teardown::Action',
          'path'=>'Teardown.action',
          'min'=>1,
          'max'=>Float::INFINITY
        }
      }

      ##
      # One or more teardown operations to perform
      # The teardown action will only contain an operation.
      # An action should contain either an operation or an assertion but not both.  It can contain any number of variables.
      class Action < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Action.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Action.extension',
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
            'path'=>'Action.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # The teardown operation to perform
          # An operation would involve a REST request to a server.
          'operation' => {
            'type'=>'TestScript::Setup::Action::Operation',
            'path'=>'Action.operation',
            'min'=>1,
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
        # The teardown operation to perform
        # An operation would involve a REST request to a server.
        attr_accessor :operation                      # 1-1 TestScript::Setup::Action::Operation
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
      # One or more teardown operations to perform
      # The teardown action will only contain an operation.
      # An action should contain either an operation or an assertion but not both.  It can contain any number of variables.
      attr_accessor :action                         # 1-* [ TestScript::Teardown::Action ]
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
    # Canonical identifier for this test script, represented as a URI (globally unique)
    # An absolute URI that is used to identify this test script when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this test script is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the test script is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 1-1 uri
    ##
    # Additional identifier for the test script
    # A formal identifier that is used to identify this test script when it is represented in other formats, or referenced in a specification, model, design or an instance.
    # Typically, this is used for identifiers that can go in an HL7 V3 II (instance identifier) data type, and can then identify this test script outside of FHIR, where it is not possible to use the logical URI.
    attr_accessor :identifier                     # 0-1 Identifier
    ##
    # Business version of the test script
    # The identifier that is used to identify this version of the test script when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the test script author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
    # There may be different test script instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the test script with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this test script (computer friendly)
    # A natural language name identifying the test script. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 1-1 string
    ##
    # Name for this test script (human friendly)
    # A short, descriptive, user-friendly title for the test script.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # draft | active | retired | unknown
    # The status of this test script. Enables tracking the life-cycle of the content.
    # Allows filtering of test scripts that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this test script is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of test scripts that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Date last changed
    # The date  (and optionally time) when the test script was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the test script changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the test script. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the test script.
    # Usually an organization but may be an individual. The publisher (or steward) of the test script is the organization or individual primarily responsible for the maintenance and upkeep of the test script. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the test script. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the test script
    # A free text natural language description of the test script from a consumer's perspective.
    # This description can be used to capture details such as why the test script was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the test script as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the test script is presumed to be the predominant language in the place the test script was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate test script instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for test script (if applicable)
    # A legal or geographic region in which the test script is intended to be used.
    # It may be possible for the test script to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Why this test script is defined
    # Explanation of why this test script is needed and why it has been designed as it has.
    # This element does not describe the usage of the test script. Instead, it provides traceability of ''why'' the resource is either needed or ''why'' it is defined as it is.  This may be used to point to source materials or specifications that drove the structure of this test script.
    attr_accessor :purpose                        # 0-1 markdown
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the test script and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the test script.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # An abstract server representing a client or sender in a message exchange
    # An abstract server used in operations within this test script in the origin element.
    # The purpose of this element is to define the profile of an origin element used elsewhere in the script.  Test engines could then use the origin-profile mapping to offer a filtered list of test systems that can serve as the sender for the interaction.
    attr_accessor :origin                         # 0-* [ TestScript::Origin ]
    ##
    # An abstract server representing a destination or receiver in a message exchange
    # An abstract server used in operations within this test script in the destination element.
    # The purpose of this element is to define the profile of a destination element used elsewhere in the script.  Test engines could then use the destination-profile mapping to offer a filtered list of test systems that can serve as the receiver for the interaction.
    attr_accessor :destination                    # 0-* [ TestScript::Destination ]
    ##
    # Required capability that is assumed to function correctly on the FHIR server being tested
    # The required capability must exist and are assumed to function correctly on the FHIR server being tested.
    attr_accessor :metadata                       # 0-1 TestScript::Metadata
    ##
    # Fixture in the test script - by reference (uri). All fixtures are required for the test script to execute.
    attr_accessor :fixture                        # 0-* [ TestScript::Fixture ]
    ##
    # Reference of the validation profile
    # Reference to the profile to be used for validation.
    # See http://build.fhir.org/resourcelist.html for complete list of resource types.
    attr_accessor :profile                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/Resource) ]
    ##
    # Placeholder for evaluated elements
    # Variable is set based either on element value in response body or on header field value in the response headers.
    # Variables would be set based either on XPath/JSONPath expressions against fixtures (static and response), or headerField evaluations against response headers. If variable evaluates to nodelist or anything other than a primitive value, then test engine would report an error.  Variables would be used to perform clean replacements in "operation.params", "operation.requestHeader.value", and "operation.url" element values during operation calls and in "assert.value" during assertion evaluations. This limits the places that test engines would need to look for placeholders "${}".  Variables are scoped to the whole script. They are NOT evaluated at declaration. They are evaluated by test engine when used for substitutions in "operation.params", "operation.requestHeader.value", and "operation.url" element values during operation calls and in "assert.value" during assertion evaluations.  See example testscript-search.xml.
    attr_accessor :variable                       # 0-* [ TestScript::Variable ]
    ##
    # A series of required setup operations before tests are executed.
    attr_accessor :setup                          # 0-1 TestScript::Setup
    ##
    # A test in this script.
    attr_accessor :test                           # 0-* [ TestScript::Test ]
    ##
    # A series of required clean up steps
    # A series of operations required to clean up after all the tests are executed (successfully or otherwise).
    attr_accessor :teardown                       # 0-1 TestScript::Teardown

    def resourceType
      'TestScript'
    end
  end
end
