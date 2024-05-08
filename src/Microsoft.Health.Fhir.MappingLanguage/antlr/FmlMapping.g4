/**
 * Define a grammar called FhirMapper
 */
 grammar FmlMapping;

// starting point for parsing a mapping file
// in case we need nested ConceptMaps, we need to have this rule:
// structureMap : mapId conceptMap* structure* imports* group+

structureMap
  : header structure* imports* const* group+ EOF
  ;

header
  : (mapId | mapUrl | mapName | mapTitle | mapStatus | mapDescription)* ;

mapId
	: LINE_COMMENT* 'map' url '=' identifier
	;

mapUrl
	: HEADER_URL '=' stringValue
	;

mapName
	: HEADER_NAME '=' stringValue
	;

mapTitle
	: HEADER_TITLE '=' stringValue
	;

mapStatus
	: HEADER_STATUS '=' stringValue
	;

mapDescription
	: HEADER_DESCRIPTION '=' stringValue
	;

stringValue
  : DOUBLE_QUOTED_STRING
  ;

url
  : STRING
  | DOUBLE_QUOTED_STRING
  ;

identifier
  : ID
  | IDENTIFIER
  | DELIMITED_IDENTIFIER
  ;

structure
	: LINE_COMMENT* 'uses' url structureAlias? 'as'  modelMode
	;

structureAlias
  : 'alias' identifier
  ;

imports
	: LINE_COMMENT* 'imports' url
	;

const 
  : LINE_COMMENT* 'let' ID '=' fhirPath ';' // which might just be a literal
  ;

group
	: LINE_COMMENT* 'group' ID parameters extends? typeMode? rules
	;

rules
  : '{' rule* '}'
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
  : inputMode ID type?
  ;

type
  : ':' identifier
  ;

rule
 	: LINE_COMMENT* ruleSources ('->' ruleTargets)? dependent? ruleName? ';'
 	;

ruleName
  : ID
  ;

ruleSources
  : ruleSource (',' ruleSource)*
  ;

ruleSource
  :  ruleContext sourceType? sourceCardinality? sourceDefault? sourceListMode? alias? whereClause? checkClause? log?
  ;

ruleTargets
  : ruleTarget (',' ruleTarget)*
  ;

sourceType
  : ':' identifier
  ;

sourceCardinality
  : INTEGER '..' upperBound
  ;

upperBound
  : INTEGER
  | '*'
  ;

ruleContext
  : identifier ('.' identifier)*
  ;

sourceDefault
  : 'default' '(' fhirPath ')'
  ;

alias
  : 'as' identifier
  ;

whereClause
  : 'where' '(' fhirPath ')'
  ;

checkClause
  : 'check' '(' fhirPath ')'
  ;

log
  : 'log' '(' fhirPath ')'
  ;

dependent
  : 'then' (invocation (',' invocation)* rules? | rules)
  ;

ruleTarget
  : ruleContext ('=' transform)? alias? targetListMode?
  | invocation alias?     // alias is not required when simply invoking a group
  ;

transform
  : literal           // trivial constant transform
  | ruleContext       // 'copy' transform
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

fhirPath
  : literal       // insert reference to FhirPath grammar here
  ;

literal
  : INTEGER
  | NUMBER
  | STRING
  | DATETIME
  | DATE
  | TIME
  | BOOL
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

BOOL
        : 'true'
        | 'false'
        ;

DATE
        : '@' DATEFORMAT
        ;

DATETIME
        : '@' DATEFORMAT 'T' (TIMEFORMAT TIMEZONEOFFSETFORMAT?)?
        ;

TIME
        : '@' 'T' TIMEFORMAT
        ;

fragment DATEFORMAT
        : [0-9][0-9][0-9][0-9] ('-'[0-9][0-9] ('-'[0-9][0-9])?)?
        ;

fragment TIMEFORMAT
        : [0-9][0-9] (':'[0-9][0-9] (':'[0-9][0-9] ('.'[0-9]+)?)?)?
        ;

fragment TIMEZONEOFFSETFORMAT
        : ('Z' | ('+' | '-') [0-9][0-9]':'[0-9][0-9])
        ;

ID
        : ([A-Za-z])([A-Za-z0-9])*
        ;

IDENTIFIER
        : ([A-Za-z] | '_')([A-Za-z0-9] | '_')*            // Added _ to support CQL (FHIR could constrain it out)
        ;

DELIMITED_IDENTIFIER
        : '`' (ESC | .)*? '`'
        ;

STRING
        : '\'' (ESC | .)*? '\''
        ;

INTEGER
    : [0-9]+
    ;

// Also allows leading zeroes now (just like CQL and XSD)
NUMBER
    : INTEGER ('.' [0-9]+)?
    ;

// SINGLE_QUOTED_STRING
//   : '\'' ( ~["\r\n] )* '\'' 
//   ;

DOUBLE_QUOTED_STRING
  : '"' (ESC | .)*? '"'
  // : '"' ( ~["\r\n] )* '"' 
  ;


// note that these need to be expressed like this to avoid greedy collection of the header keys as other tokens
HEADER_URL
  : '/// url'
  ;

// note that these need to be expressed like this to avoid greedy collection of the header keys as other tokens
HEADER_NAME
  : '/// name'
  ;

// note that these need to be expressed like this to avoid greedy collection of the header keys as other tokens
HEADER_TITLE
  : '/// title'
  ;

// note that these need to be expressed like this to avoid greedy collection of the header keys as other tokens
HEADER_STATUS
  : '/// status'
  ;

// note that these need to be expressed like this to avoid greedy collection of the header keys as other tokens
HEADER_DESCRIPTION
  : '/// description'
  ;

// Pipe whitespace to the HIDDEN channel to support retrieving source text through the parser.
WS
    : [ \r\n\t]+ -> channel(HIDDEN)
    ;

COMMENT
        : '/*' .*? '*/' // -> channel(HIDDEN)
        ;

LINE_COMMENT
        : '//' ~[/\r\n] ~[\r\n]* // -> channel(HIDDEN)
        ;

fragment ESC
        : '\\' (["'\\/fnrt] | UNICODE)    // allow \", \', \\, \/, \f, etc. and \uXXX
        ;

fragment UNICODE
        : 'u' HEX HEX HEX HEX
        ;

fragment HEX
        : [0-9a-fA-F]
        ;
