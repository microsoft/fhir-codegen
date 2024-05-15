/**
 * Define a grammar called FhirMapper
 */
 grammar FmlMapping;

// starting point for parsing a mapping file
// in case we need nested ConceptMaps, we need to have this rule:
// structureMap : mapId conceptMap* structure* imports* group+

structureMap
  : metadataDeclaration*? conceptMapDeclaration*? mapDeclaration? structureDeclaration*? importDeclaration*? constantDeclaration*? groupDeclaration+ EOF
  ;

conceptMapDeclaration
  : 'conceptmap' url '{' conceptMapPrefix+ conceptMapCodeMap+ '}'
  ;

conceptMapPrefix
  : 'prefix' ID '=' url
  ;

conceptMapCodeMap
  : conceptMapSource '-' conceptMapTarget
  ;

conceptMapSource
  : ID ':' code
  ;

conceptMapTarget
  : ID ':' code
  ;

code
  : ID
  | SINGLE_QUOTED_STRING
  | DOUBLE_QUOTED_STRING
  ;

mapDeclaration
	: 'map' url '=' identifier
	;

metadataDeclaration
  : METADATA_PREFIX qualifiedIdentifier '=' (literal | markdownLiteral)?  // value is optional to allow descendant maps to remove values from parents
  ;

markdownLiteral
  : TRIPLE_QUOTED_STRING_LITERAL
  ;

url
  : SINGLE_QUOTED_STRING
  | DOUBLE_QUOTED_STRING
  ;

identifier
  : ID
  | IDENTIFIER
  | DELIMITED_IDENTIFIER
  ;

structureDeclaration
	: 'uses' url structureAlias? 'as'  modelMode 
	;

structureAlias
  : 'alias' identifier
  ;

importDeclaration
	: 'imports' url
	;

constantDeclaration 
  : 'let' ID '=' fpExpression ';' // which might just be a literal
  ;

groupDeclaration
	: 'group' ID parameters extends? typeMode? groupExpressions
	;

// fhirPath
//   : fpExpression
//   ;

groupExpressions
  : '{' expression* '}'
  ;

typeMode
  : '<<' groupTypeMode '>>'
  ;

extends
  : 'extends' ID
  ;

parameters
  : '(' parameter (',' parameter)+ ')'
  ;

parameter
  : inputMode ID typeIdentifier?
  ;

typeIdentifier
  : ':' identifier
  ;

expression
 	: qualifiedIdentifier '->' qualifiedIdentifier ';'  #mapSimpleCopy
 	| fpExpression ';'                                  #mapFhirPath               
  | mapExpression ';'                                 #mapFhirMarkup
 	;

mapExpression
  : mapExpressionSource (',' mapExpressionSource)* ('->' mapExpressionTarget)? dependentExpression? mapExpressionName?
  ;

// mapLine
//   : 
//   ;

// mapLine
//  	: fpExpression ';'
//  	| mapLineSources ('->' mapLineTargets)? dependent? mapLineName? ';'
//  	;

mapExpressionName
  : DOUBLE_QUOTED_STRING
  ;

// mapLineSources
//   : mapLineSource (',' mapLineSource)*
//   ;

mapExpressionSource
  :  qualifiedIdentifier typeIdentifier? sourceCardinality? sourceDefault? sourceListMode? alias? whereClause? checkClause? log?
  ;

mapExpressionTarget
  : mapLineTarget (',' mapLineTarget)*
  ;

sourceCardinality
  : INTEGER '..' upperBound
  ;

upperBound
  : INTEGER
  | '*'
  ;

qualifiedIdentifier
  : identifier ('.' identifier '[x]'?)*
  ;

sourceDefault
  : 'default' '(' fpExpression ')'
  ;

alias
  : 'as' identifier
  ;

whereClause
  : 'where' '(' fpExpression ')'
  ;

checkClause
  : 'check' '(' fpExpression ')'
  ;

log
  : 'log' '(' fpExpression ')'
  ;

dependentExpression
  : 'then' (invocation (',' invocation)* groupExpressions? | groupExpressions)
  ;

mapLineTarget
  : qualifiedIdentifier ('=' transform)? alias? targetListMode?
  | invocation alias?     // alias is not required when simply invoking a group
  ;

transform
  : literal           // trivial constant transform
  | qualifiedIdentifier       // 'copy' transform
  | invocation        // other named transforms
  ;

invocation
  : identifier '(' paramList? ')'
  ;

paramList
  : param (',' param)*
  ;

param
  : literal
  | ID
  ;

fpExpression
        : fpTerm                                                      #termExpression
        | fpExpression '.' fpInvocation                               #invocationExpression
        | fpExpression '[' fpExpression ']'                           #indexerExpression
        | fpPolarityLiteral fpExpression                              #polarityExpression
        | fpExpression fpMultiplicativeLiteral fpExpression           #multiplicativeExpression
        | fpExpression fpAdditiveLiteral fpExpression                 #additiveExpression
        | fpExpression fpTypeAssertionLiteral fpTypeSpecifier         #typeExpression
        | fpExpression fpUnionLiteral fpExpression                    #unionExpression
        | fpExpression fpInequalityLiteral fpExpression               #inequalityExpression
        | fpExpression fpEqualityLiteral fpExpression                 #equalityExpression
        | fpExpression fpMembershipLiteral fpExpression               #membershipExpression
        | fpExpression fpAndLiteral fpExpression                      #andExpression
        | fpExpression fpOrLiteral fpExpression                       #orExpression
        | fpExpression fpImpliesLiteral fpExpression                  #impliesExpression
        //| (IDENTIFIER)? '=>' fpExpression                           #lambdaExpression
        ;

fpPolarityLiteral
  : '+' | '-'
  ;

fpMultiplicativeLiteral
  : '*' | '/' | 'div' | 'mod'
  ;

fpAdditiveLiteral
  : '+' | '-' | '&'
  ;

fpTypeAssertionLiteral
  : 'is' | 'as'
  ;

fpUnionLiteral
  : '|'
  ;

fpInequalityLiteral
  : '<=' | '<' | '>' | '>='
  ;

fpEqualityLiteral
  : '=' | '~' | '!=' | '!~'
  ;

fpMembershipLiteral
  : 'in' | 'contains'
  ;

fpAndLiteral
  : 'and'
  ;

fpOrLiteral
  : 'or' | 'xor'
  ;

fpImpliesLiteral
  : 'implies'
  ;

fpTerm
        : fpInvocation                                            #invocationTerm
        | literal                                                 #literalTerm
        | fpExternalConstant                                      #externalConstantTerm
        | '(' fpExpression ')'                                    #parenthesizedTerm
        ;

fpInvocation                          // Terms that can be used after the function/member invocation '.'
        : fpFunction                                            #functionInvocation
        | identifier                                            #memberInvocation
        | '$this'                                               #thisInvocation
        | '$index'                                              #indexInvocation
        | '$total'                                              #totalInvocation
        ;

fpExternalConstant
        : '%' ( identifier | SINGLE_QUOTED_STRING )
        ;

fpFunction
        : identifier '(' fpParamList? ')'
        ;

fpParamList
        : fpExpression (',' fpExpression)*
        ;

fpTypeSpecifier
        : qualifiedIdentifier
        ;

constant
  : ID
  ;


literal
  : NULL_LITERAL                                          #nullLiteral
  | BOOL                                                  #booleanLiteral
  | fpQuantity                                            #quantityLiteral
  | LONG_INTEGER                                          #longNumberLiteral
  | (INTEGER | DECIMAL)                                   #numberLiteral
  | DATE                                                  #dateLiteral
  | DATE_TIME                                             #dateTimeLiteral
  | TIME                                                  #timeLiteral
  | SINGLE_QUOTED_STRING                                  #stringLiteral
  | DOUBLE_QUOTED_STRING                                  #quotedStringLiteral
  ;

  // : BOOL
  // | DATE
  // | DATE_TIME
  // | TIME
  // | NUMBER
  // // | ID            // added to allow for constant use
  // | DELIMITED_IDENTIFIER
  // | SINGLE_QUOTED_STRING
  // | DOUBLE_QUOTED_STRING
  // ;

// note that quantity has to require units here because if not there is no differentiator from a bare number
fpQuantity
    : (INTEGER | DECIMAL) fpUnit
    ;

fpUnit
    : fpDateTimePrecision
    | fpPluralDateTimePrecision
    | SINGLE_QUOTED_STRING // UCUM syntax for units of measure
    ;

fpDateTimePrecision
        : 'year' | 'month' | 'week' | 'day' | 'hour' | 'minute' | 'second' | 'millisecond'
        ;

fpPluralDateTimePrecision
        : 'years' | 'months' | 'weeks' | 'days' | 'hours' | 'minutes' | 'seconds' | 'milliseconds'
        ;

groupTypeMode
  : 'types' | 'type+'
  ;

sourceListMode
  : 'first' | 'not_first' | 'last' | 'not_last' | 'only_one'
  ;

targetListMode
  : 'first' | 'share' | 'last' | 'single'
  ;

inputMode
  : 'source' | 'target'
  ;

modelMode           // StructureMapModelMode binding
  : 'source' | 'queried' | 'target' | 'produced'
  ;



    /*
     * Syntax for embedded ConceptMaps excluded for now
     *
    conceptMap
        : 'conceptMap' '"#'	IDENTIFIER '{' (prefix)+ conceptMapping '}'
        ;

    prefix
    	: 'prefix' conceptMappingVar '=' URL
    	;

    conceptMappingVar
    	:  IDENTIFIER
    	;
    conceptMapping
    	:  conceptMappingVar ':' field
    	   (('<=' | '=' | '==' | '!=' '>=' '>-' | '<-' | '~') conceptMappingVar ':' field) | '--'
    	;
    */




/****************************************************************
    Lexical rules from FhirPath
*****************************************************************/

NULL_LITERAL
  : '{' '}'
  ;

BOOL
    : 'true'
    | 'false'
    ;

DATE
    : '@' DATE_FORMAT
    ;

DATE_TIME
    : '@' DATE_FORMAT 'T' (TIME_FORMAT TIMEZONE_OFFSET_FORMAT?)?
    ;

TIME
    : '@' 'T' TIME_FORMAT
    ;

fragment DATE_FORMAT
    : [0-9][0-9][0-9][0-9] ('-'[0-9][0-9] ('-'[0-9][0-9])?)?
    ;

fragment TIME_FORMAT
    : [0-9][0-9] (':'[0-9][0-9] (':'[0-9][0-9] ('.'[0-9]+)?)?)?
    ;

fragment TIMEZONE_OFFSET_FORMAT
    : ('Z' | ('+' | '-') [0-9][0-9]':'[0-9][0-9])
    ;

LONG_INTEGER
    : [0-9]+ 'L'
    ;

DECIMAL
    : [0-9]* '.' [0-9]+
    ;

INTEGER
    : [0-9]+
    ;

// // Also allows leading zeroes now (just like CQL and XSD)
// NUMBER
//     : [0-9]+('.' [0-9]+)?
//     ;

ID
    : ([A-Za-z])([A-Za-z0-9])*
    ;


// FHIR_ELEMENT_PATH_WITH_SLICE
//   : ID ('.' ID (':' ID)? '[x]'?)*
//   ;

IDENTIFIER
    : ([A-Za-z] | '_')([A-Za-z0-9] | '_')*            // Added _ to support CQL (FHIR could constrain it out)
    ;

DELIMITED_IDENTIFIER
    : '`' (ESC | .)*? '`'
    ;

SINGLE_QUOTED_STRING
    : '\'' (ESC | .)*? '\''
    ;

// SINGLE_QUOTED_STRING
//   : '\'' ( ~["\r\n] )* '\'' 
//   ;

DOUBLE_QUOTED_STRING
  : '"' (ESC | .)*? '"'
  // : '"' ( ~["\r\n] )* '"' 
  ;

TRIPLE_QUOTED_STRING_LITERAL
  : '"""' [\r\n] (.)*? [\r\n] '"""' ('\r\n'|'\r'|'\n'|EOF)
  ;


// Pipe whitespace to the HIDDEN channel to support retrieving source text through the parser.
WS
    : [ \r\n\t]+ -> channel(HIDDEN)
    ;

BLOCK_COMMENT
        : '/*' .*? '*/' -> channel(HIDDEN)
        ;

METADATA_PREFIX
      : '/// '
      ;

LINE_COMMENT
        : '//' ~[/] ~[\r\n]* -> channel(HIDDEN)
        ;

// INLINE_COMMENT
//   : [ \t]* C_STYLE_COMMENT
//   | [ \t]* LINE_COMMENT
//   ;

fragment ESC
        : '\\' (["'\\/fnrt] | UNICODE)    // allow \", \', \\, \/, \f, etc. and \uXXX
        ;

fragment UNICODE
        : 'u' HEX HEX HEX HEX
        ;

fragment HEX
        : [0-9a-fA-F]
        ;
