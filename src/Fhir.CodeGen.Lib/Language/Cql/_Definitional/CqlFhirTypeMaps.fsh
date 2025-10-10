RuleSet: LinkTypes(fhirType, cqlType, relationship)
* group[=].element[+].code  = {fhirType}
* group[=].element[=].target[+].code = {cqlType}
* group[=].element[=].target[=].relationship = {relationship}


Instance:     CqlFhirTypeMapR6
InstanceOf:   ConceptMap
Title:        "Type Mappings for CQL and FHIR R6"
Description:  "Map describing how to map between FHIR types and CQL types for FHIR R6."
Usage:        #definition
* id                    = "cql-fhir-types-r6"
* url                   = "http://ginoc.io/cql/cql-fhir-types-r6"
* status                = #active
* jurisdiction          = http://unstats.un.org/unsd/methods/m49/m49.htm#001
* sourceScopeCanonical  = "http://hl7.org/fhir/ValueSet/data-types|6.0"
* targetScopeCanonical  = "http://cql.hl7.org/data-types"
* group[+].source       = "http://hl7.org/fhir/fhir-types|6.0"
* group[=].target       = "http://hl7.org/fhirpath"
* insert LinkTypes(#base64Binary,       #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#boolean,            #System.Boolean,            #equivalent)
* insert LinkTypes(#canonical,          #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#code,               #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#date,               #System.Date,               #equivalent)
* insert LinkTypes(#dateTime,           #System.DateTime,           #equivalent)
* insert LinkTypes(#decimal,            #System.Decimal,            #equivalent)
* insert LinkTypes(#id,                 #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#instant,            #System.DateTime,           #equivalent)
* insert LinkTypes(#integer,            #System.Integer,            #equivalent)
* insert LinkTypes(#integer64,          #System.Long,               #equivalent)
* insert LinkTypes(#markdown,           #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#oid,                #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#positiveInt,        #System.Integer,            #source-is-narrower-than-target)
* insert LinkTypes(#string,             #System.String,             #equivalent)
* insert LinkTypes(#time,               #System.Time,               #equivalent)
* insert LinkTypes(#unsignedInt,        #System.Integer,            #source-is-narrower-than-target)
* insert LinkTypes(#uri,                #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#url,                #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#uuid,               #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#xhtml,              #System.String,             #source-is-narrower-than-target)

* insert LinkTypes(#Coding,             #System.Code,               #source-is-narrower-than-target)
* insert LinkTypes(#CodeableConcept,    #System.Concept,            #source-is-narrower-than-target)
* insert LinkTypes(#Period,             #Interval<System.DateTime>, #source-is-narrower-than-target)
* insert LinkTypes(#Range,              #Interval<System.Quantity>, #source-is-narrower-than-target)
* insert LinkTypes(#Ratio,              #System.Ratio,              #source-is-narrower-than-target)
* insert LinkTypes(#Quantity,           #System.Quantity,           #equivalent)
* insert LinkTypes(#Age,                #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Distance,           #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#SimpleQuantity,     #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Duration,           #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Count,              #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#MoneyQuantity,      #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Money,              #System.Decimal,            #source-is-narrower-than-target)


Instance:     CqlFhirTypeMapR5
InstanceOf:   ConceptMap
Title:        "Type Mappings for CQL and FHIR R5"
Description:  "Map describing how to map between FHIR types and CQL types for FHIR R5."
Usage:        #definition
* id                    = "cql-fhir-types-r5"
* url                   = "http://ginoc.io/cql/cql-fhir-types-r5"
* status                = #active
* jurisdiction          = http://unstats.un.org/unsd/methods/m49/m49.htm#001
* sourceScopeCanonical  = "http://hl7.org/fhir/ValueSet/data-types|5.0"
* targetScopeCanonical  = "http://cql.hl7.org/data-types"
* group[+].source       = "http://hl7.org/fhir/fhir-types|5.0"
* group[=].target       = "http://hl7.org/fhirpath"
* insert LinkTypes(#base64Binary,       #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#boolean,            #System.Boolean,            #equivalent)
* insert LinkTypes(#canonical,          #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#code,               #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#date,               #System.Date,               #equivalent)
* insert LinkTypes(#dateTime,           #System.DateTime,           #equivalent)
* insert LinkTypes(#decimal,            #System.Decimal,            #equivalent)
* insert LinkTypes(#id,                 #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#instant,            #System.DateTime,           #equivalent)
* insert LinkTypes(#integer,            #System.Integer,            #equivalent)
* insert LinkTypes(#integer64,          #System.Long,               #equivalent)
* insert LinkTypes(#markdown,           #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#oid,                #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#positiveInt,        #System.Integer,            #source-is-narrower-than-target)
* insert LinkTypes(#string,             #System.String,             #equivalent)
* insert LinkTypes(#time,               #System.Time,               #equivalent)
* insert LinkTypes(#unsignedInt,        #System.Integer,            #source-is-narrower-than-target)
* insert LinkTypes(#uri,                #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#url,                #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#uuid,               #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#xhtml,              #System.String,             #source-is-narrower-than-target)

* insert LinkTypes(#Coding,             #System.Code,               #source-is-narrower-than-target)
* insert LinkTypes(#CodeableConcept,    #System.Concept,            #source-is-narrower-than-target)
* insert LinkTypes(#Period,             #Interval<System.DateTime>, #source-is-narrower-than-target)
* insert LinkTypes(#Range,              #Interval<System.Quantity>, #source-is-narrower-than-target)
* insert LinkTypes(#Ratio,              #System.Ratio,              #source-is-narrower-than-target)
* insert LinkTypes(#Quantity,           #System.Quantity,           #equivalent)
* insert LinkTypes(#Age,                #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Distance,           #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#SimpleQuantity,     #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Duration,           #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Count,              #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#MoneyQuantity,      #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Money,              #System.Decimal,            #source-is-narrower-than-target)


Instance:     CqlFhirTypeMapR4
InstanceOf:   ConceptMap
Title:        "Type Mappings for CQL and FHIR R4"
Description:  "Map describing how to map between FHIR types and CQL types for FHIR R4."
Usage:        #definition
* id                    = "cql-fhir-types-r4"
* url                   = "http://ginoc.io/cql/cql-fhir-types-r4"
* status                = #active
* jurisdiction          = http://unstats.un.org/unsd/methods/m49/m49.htm#001
* sourceScopeCanonical  = "http://hl7.org/fhir/ValueSet/data-types|4.0"
* targetScopeCanonical  = "http://cql.hl7.org/data-types"
* group[+].source       = "http://hl7.org/fhir/data-types|4.0"
* group[=].target       = "http://hl7.org/fhirpath"
* insert LinkTypes(#base64Binary,       #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#boolean,            #System.Boolean,            #equivalent)
* insert LinkTypes(#canonical,          #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#code,               #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#date,               #System.Date,               #equivalent)
* insert LinkTypes(#dateTime,           #System.DateTime,           #equivalent)
* insert LinkTypes(#decimal,            #System.Decimal,            #equivalent)
* insert LinkTypes(#id,                 #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#instant,            #System.DateTime,           #equivalent)
* insert LinkTypes(#integer,            #System.Integer,            #equivalent)
* insert LinkTypes(#markdown,           #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#oid,                #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#positiveInt,        #System.Integer,            #source-is-narrower-than-target)
* insert LinkTypes(#string,             #System.String,             #equivalent)
* insert LinkTypes(#time,               #System.Time,               #equivalent)
* insert LinkTypes(#unsignedInt,        #System.Integer,            #source-is-narrower-than-target)
* insert LinkTypes(#uri,                #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#url,                #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#uuid,               #System.String,             #source-is-narrower-than-target)
* insert LinkTypes(#xhtml,              #System.String,             #source-is-narrower-than-target)

* insert LinkTypes(#Coding,             #System.Code,               #source-is-narrower-than-target)
* insert LinkTypes(#CodeableConcept,    #System.Concept,            #source-is-narrower-than-target)
* insert LinkTypes(#Period,             #Interval<System.DateTime>, #source-is-narrower-than-target)
* insert LinkTypes(#Range,              #Interval<System.Quantity>, #source-is-narrower-than-target)
* insert LinkTypes(#Ratio,              #System.Ratio,              #source-is-narrower-than-target)
* insert LinkTypes(#Quantity,           #System.Quantity,           #equivalent)
* insert LinkTypes(#Age,                #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Distance,           #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#SimpleQuantity,     #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Duration,           #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Count,              #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#MoneyQuantity,      #System.Quantity,           #source-is-narrower-than-target)
* insert LinkTypes(#Money,              #System.Decimal,            #source-is-narrower-than-target)


Instance:     CqlFhirTypeMapR3
InstanceOf:   ConceptMap
Title:        "Type Mappings for CQL and FHIR STU3"
Description:  "Map describing how to map between FHIR types and CQL types for FHIR STU3."
Usage:        #definition
* id                    = "cql-fhir-types-r3"
* url                   = "http://ginoc.io/cql/cql-fhir-types-r3"
* status                = #active
* jurisdiction          = http://unstats.un.org/unsd/methods/m49/m49.htm#001
* sourceScopeCanonical  = "http://hl7.org/fhir/ValueSet/data-types|3.0"
* targetScopeCanonical  = "http://cql.hl7.org/data-types"
* group[+].source       = "http://hl7.org/fhir/data-types|3.0"
* group[=].target       = "http://hl7.org/fhirpath"
* insert LinkTypes(#base64Binary,   #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#boolean,        #System.Boolean,    #equivalent)
* insert LinkTypes(#code,           #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#date,           #System.Date,       #equivalent)
* insert LinkTypes(#dateTime,       #System.DateTime,   #equivalent)
* insert LinkTypes(#decimal,        #System.Decimal,    #equivalent)
* insert LinkTypes(#id,             #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#instant,        #System.DateTime,   #equivalent)
* insert LinkTypes(#integer,        #System.Integer,    #equivalent)
* insert LinkTypes(#markdown,       #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#oid,            #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#positiveInt,    #System.Integer,    #source-is-narrower-than-target)
* insert LinkTypes(#string,         #System.String,     #equivalent)
* insert LinkTypes(#time,           #System.Time,       #equivalent)
* insert LinkTypes(#unsignedInt,    #System.Integer,    #source-is-narrower-than-target)
* insert LinkTypes(#uri,            #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#xhtml,          #System.String,     #source-is-narrower-than-target)


Instance:     CqlFhirTypeMapR2
InstanceOf:   ConceptMap
Title:        "Type Mappings for CQL and FHIR DSTU2"
Description:  "Map describing how to map between FHIR types and CQL types for FHIR DSTU2."
Usage:        #definition
* id                    = "cql-fhir-types-r2"
* url                   = "http://ginoc.io/cql/cql-fhir-types-r2"
* status                = #active
* jurisdiction          = http://unstats.un.org/unsd/methods/m49/m49.htm#001
* sourceScopeCanonical  = "http://hl7.org/fhir/ValueSet/data-types|2.0"
* targetScopeCanonical  = "http://cql.hl7.org/data-types"
* group[+].source       = "http://hl7.org/fhir/data-types|2.0"
* group[=].target       = "http://hl7.org/fhirpath"
* insert LinkTypes(#base64Binary,   #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#boolean,        #System.Boolean,    #equivalent)
* insert LinkTypes(#code,           #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#date,           #System.Date,       #equivalent)
* insert LinkTypes(#dateTime,       #System.DateTime,   #equivalent)
* insert LinkTypes(#decimal,        #System.Decimal,    #equivalent)
* insert LinkTypes(#id,             #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#instant,        #System.DateTime,   #equivalent)
* insert LinkTypes(#integer,        #System.Integer,    #equivalent)
* insert LinkTypes(#markdown,       #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#oid,            #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#positiveInt,    #System.Integer,    #source-is-narrower-than-target)
* insert LinkTypes(#string,         #System.String,     #equivalent)
* insert LinkTypes(#time,           #System.Time,       #equivalent)
* insert LinkTypes(#unsignedInt,    #System.Integer,    #source-is-narrower-than-target)
* insert LinkTypes(#uri,            #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#xhtml,          #System.String,     #source-is-narrower-than-target)
