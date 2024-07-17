module FHIR

  ##
  # Base StructureDefinition for Expression Type: A expression that is evaluated in a specified context and returns a value. The context of use of the expression must specify the context in which the expression is evaluated, and how the result of the expression is used.
  class Expression < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'string',
        'path'=>'Expression.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Expression.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the condition
      # A brief, natural language description of the condition that effectively communicates the intended semantics.
      'description' => {
        'type'=>'string',
        'path'=>'Expression.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # Short name assigned to expression for reuse
      # A short name assigned to the expression to allow for multiple reuse of the expression in the context where it is defined.
      'name' => {
        'type'=>'id',
        'path'=>'Expression.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # text/cql | text/fhirpath | application/x-fhir-query | etc.
      # The media type of the language for the expression.
      'language' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/expression-language'=>[ 'text/cql', 'text/fhirpath', 'application/x-fhir-query' ]
        },
        'type'=>'code',
        'path'=>'Expression.language',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/expression-language'}
      },
      ##
      # Expression in specified language
      # An expression in the specified language that returns a value.
      'expression' => {
        'type'=>'string',
        'path'=>'Expression.expression',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where the expression is found
      # A URI that defines where the expression is found.
      # If both a reference and an expression is found, the reference SHALL point to the same expression.
      'reference' => {
        'type'=>'uri',
        'path'=>'Expression.reference',
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
    # Natural language description of the condition
    # A brief, natural language description of the condition that effectively communicates the intended semantics.
    attr_accessor :description                    # 0-1 string
    ##
    # Short name assigned to expression for reuse
    # A short name assigned to the expression to allow for multiple reuse of the expression in the context where it is defined.
    attr_accessor :name                           # 0-1 id
    ##
    # text/cql | text/fhirpath | application/x-fhir-query | etc.
    # The media type of the language for the expression.
    attr_accessor :language                       # 1-1 code
    ##
    # Expression in specified language
    # An expression in the specified language that returns a value.
    attr_accessor :expression                     # 0-1 string
    ##
    # Where the expression is found
    # A URI that defines where the expression is found.
    # If both a reference and an expression is found, the reference SHALL point to the same expression.
    attr_accessor :reference                      # 0-1 uri
  end
end
