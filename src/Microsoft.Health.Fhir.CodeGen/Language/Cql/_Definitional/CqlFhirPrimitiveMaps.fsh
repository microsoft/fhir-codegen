RuleSet: LinkTypes(fhirType, cqlType, relationship)
* group[=].element[+].code  = {fhirType}
* group[=].element[=].target[+].code = {cqlType}
* group[=].element[=].target[=].relationship = {relationship}


Instance:     CqlFhirPrimitiveMapR6
InstanceOf:   ConceptMap
Title:        "Primitive Type Mappings for CQL and FHIR R6"
Description:  "Map describing how to map between FHIR types and CQL types for FHIR R6."
* id                = "cql-fhir-primitives-r6"
* url               = "http://ginoc.io/cql/cql-fhir-primitives-r6"
* status            = #active
* jurisdiction      = http://unstats.un.org/unsd/methods/m49/m49.htm#001
* sourceScope       = "http://hl7.org/fhir/6.0/ValueSet/data-types"
* targetScope       = "http://cql.hl7.org/data-types"
* group[+].source   = "http://hl7.org/fhir/6.0/data-types"
* group[=].target   = "http://hl7.org/fhirpath"
* insert LinkTypes(#base64Binary,   #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#boolean,        #System.Boolean,    #equivalent)
* insert LinkTypes(#canonical,      #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#code,           #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#date,           #System.Date,       #equivalent)
* insert LinkTypes(#dateTime,       #System.DateTime,   #equivalent)
* insert LinkTypes(#decimal,        #System.Decimal,    #equivalent)
* insert LinkTypes(#id,             #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#instant,        #System.DateTime,   #equivalent)
* insert LinkTypes(#integer,        #System.Integer,    #equivalent)
* insert LinkTypes(#integer64,      #System.Long,       #equivalent)
* insert LinkTypes(#markdown,       #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#oid,            #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#positiveInt,    #System.Integer,    #source-is-narrower-than-target)
* insert LinkTypes(#string,         #System.String,     #equivalent)
* insert LinkTypes(#time,           #System.Time,       #equivalent)
* insert LinkTypes(#unsignedInt,    #System.Integer,    #source-is-narrower-than-target)
* insert LinkTypes(#uri,            #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#url,            #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#uuid,           #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#xhtml,          #System.String,     #source-is-narrower-than-target)


Instance:     CqlFhirPrimitiveMapR5
InstanceOf:   ConceptMap
Title:        "Primitive Type Mappings for CQL and FHIR R5"
Description:  "Map describing how to map between FHIR types and CQL types for FHIR R5."
* id                = "cql-fhir-primitives-r5"
* url               = "http://ginoc.io/cql/cql-fhir-primitives-r5"
* status            = #active
* jurisdiction      = http://unstats.un.org/unsd/methods/m49/m49.htm#001
* sourceScope       = "http://hl7.org/fhir/5.0/ValueSet/data-types"
* targetScope       = "http://cql.hl7.org/data-types"
* group[+].source   = "http://hl7.org/fhir/5.0/data-types"
* group[=].target   = "http://hl7.org/fhirpath"
* insert LinkTypes(#base64Binary,   #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#boolean,        #System.Boolean,    #equivalent)
* insert LinkTypes(#canonical,      #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#code,           #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#date,           #System.Date,       #equivalent)
* insert LinkTypes(#dateTime,       #System.DateTime,   #equivalent)
* insert LinkTypes(#decimal,        #System.Decimal,    #equivalent)
* insert LinkTypes(#id,             #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#instant,        #System.DateTime,   #equivalent)
* insert LinkTypes(#integer,        #System.Integer,    #equivalent)
* insert LinkTypes(#integer64,      #System.Long,       #equivalent)
* insert LinkTypes(#markdown,       #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#oid,            #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#positiveInt,    #System.Integer,    #source-is-narrower-than-target)
* insert LinkTypes(#string,         #System.String,     #equivalent)
* insert LinkTypes(#time,           #System.Time,       #equivalent)
* insert LinkTypes(#unsignedInt,    #System.Integer,    #source-is-narrower-than-target)
* insert LinkTypes(#uri,            #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#url,            #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#uuid,           #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#xhtml,          #System.String,     #source-is-narrower-than-target)


Instance:     CqlFhirPrimitiveMapR4
InstanceOf:   ConceptMap
Title:        "Primitive Type Mappings for CQL and FHIR R4"
Description:  "Map describing how to map between FHIR types and CQL types for FHIR R4."
* id                = "cql-fhir-primitives-r4"
* url               = "http://ginoc.io/cql/cql-fhir-primitives-r4"
* status            = #active
* jurisdiction      = http://unstats.un.org/unsd/methods/m49/m49.htm#001
* sourceScope       = "http://hl7.org/fhir/4.0/ValueSet/data-types"
* targetScope       = "http://cql.hl7.org/data-types"
* group[+].source   = "http://hl7.org/fhir/4.0/data-types"
* group[=].target   = "http://hl7.org/fhirpath"
* insert LinkTypes(#base64Binary,   #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#boolean,        #System.Boolean,    #equivalent)
* insert LinkTypes(#canonical,      #System.String,     #source-is-narrower-than-target)
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
* insert LinkTypes(#url,            #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#uuid,           #System.String,     #source-is-narrower-than-target)
* insert LinkTypes(#xhtml,          #System.String,     #source-is-narrower-than-target)


Instance:     CqlFhirPrimitiveMapR3
InstanceOf:   ConceptMap
Title:        "Primitive Type Mappings for CQL and FHIR STU3"
Description:  "Map describing how to map between FHIR types and CQL types for FHIR STU3."
* id                = "cql-fhir-primitives-r3"
* url               = "http://ginoc.io/cql/cql-fhir-primitives-r3"
* status            = #active
* jurisdiction      = http://unstats.un.org/unsd/methods/m49/m49.htm#001
* sourceScope       = "http://hl7.org/fhir/3.0/ValueSet/data-types"
* targetScope       = "http://cql.hl7.org/data-types"
* group[+].source   = "http://hl7.org/fhir/3.0/data-types"
* group[=].target   = "http://hl7.org/fhirpath"
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


Instance:     CqlFhirPrimitiveMapR2
InstanceOf:   ConceptMap
Title:        "Primitive Type Mappings for CQL and FHIR DSTU2"
Description:  "Map describing how to map between FHIR types and CQL types for FHIR DSTU2."
* id                = "cql-fhir-primitives-r2"
* url               = "http://ginoc.io/cql/cql-fhir-primitives-r2"
* status            = #active
* jurisdiction      = http://unstats.un.org/unsd/methods/m49/m49.htm#001
* sourceScope       = "http://hl7.org/fhir/2.0/ValueSet/data-types"
* targetScope       = "http://cql.hl7.org/data-types"
* group[+].source   = "http://hl7.org/fhir/2.0/data-types"
* group[=].target   = "http://hl7.org/fhirpath"
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
