module FHIR

  ##
  # The technical details of an endpoint that can be used for electronic services, such as for web services providing XDS.b or a REST endpoint for another FHIR server. This may include any security context information.
  class Endpoint < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['connection-type', 'identifier', 'name', 'organization', 'payload-type', 'status']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Endpoint.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Endpoint.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Endpoint.implicitRules',
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
        'path'=>'Endpoint.language',
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
        'path'=>'Endpoint.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'Endpoint.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Endpoint.extension',
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
        'path'=>'Endpoint.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Identifies this endpoint across multiple systems
      # Identifier for the organization that is used to identify the endpoint across multiple disparate systems.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Endpoint.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # active | suspended | error | off | entered-in-error | test
      # active | suspended | error | off | test.
      # This element is labeled as a modifier because the status contains codes that mark the endpoint as not currently valid.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/endpoint-status'=>[ 'active', 'suspended', 'error', 'off', 'entered-in-error', 'test' ]
        },
        'type'=>'code',
        'path'=>'Endpoint.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/endpoint-status'}
      },
      ##
      # Protocol/Profile/Standard to be used with this endpoint connection
      # A coded value that represents the technical details of the usage of this endpoint, such as what WSDLs should be used in what way. (e.g. XDS.b/DICOM/cds-hook).
      # For additional connectivity details for the protocol, extensions will be used at this point, as in the XDS example.
      'connectionType' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/endpoint-connection-type'=>[ 'ihe-xcpd', 'ihe-xca', 'ihe-xdr', 'ihe-xds', 'ihe-iid', 'dicom-wado-rs', 'dicom-qido-rs', 'dicom-stow-rs', 'dicom-wado-uri', 'hl7-fhir-rest', 'hl7-fhir-msg', 'hl7v2-mllp', 'secure-email', 'direct-project' ]
        },
        'type'=>'Coding',
        'path'=>'Endpoint.connectionType',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/endpoint-connection-type'}
      },
      ##
      # A name that this endpoint can be identified by
      # A friendly name that this endpoint can be referred to with.
      'name' => {
        'type'=>'string',
        'path'=>'Endpoint.name',
        'min'=>0,
        'max'=>1
      },
      ##
      # Organization that manages this endpoint (might not be the organization that exposes the endpoint)
      # The organization that manages this endpoint (even if technically another organization is hosting this in the cloud, it is the organization associated with the data).
      # This property is not typically used when searching for Endpoint resources for usage. The typical usage is via the reference from an applicable Organization/Location/Practitioner resource, which is where the context is provided. Multiple Locations may reference a single endpoint, and don't have to be within the same organization resource, but most likely within the same organizational hierarchy.
      'managingOrganization' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Endpoint.managingOrganization',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for source (e.g. troubleshooting)
      # Contact details for a human to contact about the subscription. The primary use of this for system administrator troubleshooting.
      'contact' => {
        'type'=>'ContactPoint',
        'path'=>'Endpoint.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Interval the endpoint is expected to be operational
      # The interval during which the endpoint is expected to be operational.
      'period' => {
        'type'=>'Period',
        'path'=>'Endpoint.period',
        'min'=>0,
        'max'=>1
      },
      ##
      # The type of content that may be used at this endpoint (e.g. XDS Discharge summaries)
      # The payload type describes the acceptable content that can be communicated on the endpoint.
      # The payloadFormat describes the serialization format of the data, where the payloadType indicates the specific document/schema that is being transferred; e.g. DischargeSummary or CarePlan.
      'payloadType' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/endpoint-payload-type'=>[ 'any', 'none' ],
          'urn:oid:1.3.6.1.4.1.19376.1.2.3'=>[ 'urn:ihe:pcc:handp:2008', 'urn:ihe:pcc:xphr:2007', 'urn:ihe:pcc:aps:2007', 'urn:ihe:pcc:xds-ms:2007', 'urn:ihe:pcc:xphr:2007', 'urn:ihe:pcc:edr:2007', 'urn:ihe:pcc:edes:2007', 'urn:ihe:pcc:apr:handp:2008', 'urn:ihe:pcc:apr:lab:2008', 'urn:ihe:pcc:apr:edu:2008', 'urn:ihe:pcc:irc:2008', 'urn:ihe:pcc:crc:2008', 'urn:ihe:pcc:cm:2008', 'urn:ihe:pcc:ic:2009', 'urn:ihe:pcc:tn:2007', 'urn:ihe:pcc:nn:2007', 'urn:ihe:pcc:ctn:2007', 'urn:ihe:pcc:edpn:2007', 'urn:ihe:pcc:hp:2008', 'urn:ihe:pcc:ldhp:2009', 'urn:ihe:pcc:lds:2009', 'urn:ihe:pcc:mds:2009', 'urn:ihe:pcc:nds:2010', 'urn:ihe:pcc:ppvs:2010', 'urn:ihe:pcc:trs:2011', 'urn:ihe:pcc:ets:2011', 'urn:ihe:pcc:its:2011', 'urn:ihe:iti:bppc:2007', 'urn:ihe:iti:bppc-sd:2007', 'urn:ihe:iti:xdw:2011:workflowDoc', 'urn:ihe:iti:dsg:detached:2014', 'urn:ihe:iti:dsg:enveloping:2014', 'urn:ihe:iti:xds-sd:pdf:2008', 'urn:ihe:iti:xds-sd:text:2008', 'urn:ihe:lab:xd-lab:2008', 'urn:ihe:rad:TEXT', 'urn:ihe:rad:PDF', 'urn:ihe:rad:CDA:ImagingReportStructuredHeadings:2013', 'urn:ihe:card:imaging:2011', 'urn:ihe:card:CRC:2012', 'urn:ihe:card:EPRC-IE:2014', 'urn:ihe:dent:TEXT', 'urn:ihe:dent:PDF', 'urn:ihe:dent:CDA:ImagingReportStructuredHeadings:2013', 'urn:ihe:pat:apsr:all:2010', 'urn:ihe:pat:apsr:cancer:all:2010', 'urn:ihe:pat:apsr:cancer:breast:2010', 'urn:ihe:pat:apsr:cancer:colon:2010', 'urn:ihe:pat:apsr:cancer:prostate:2010', 'urn:ihe:pat:apsr:cancer:thyroid:2010', 'urn:ihe:pat:apsr:cancer:lung:2010', 'urn:ihe:pat:apsr:cancer:skin:2010', 'urn:ihe:pat:apsr:cancer:kidney:2010', 'urn:ihe:pat:apsr:cancer:cervix:2010', 'urn:ihe:pat:apsr:cancer:endometrium:2010', 'urn:ihe:pat:apsr:cancer:ovary:2010', 'urn:ihe:pat:apsr:cancer:esophagus: 2010', 'urn:ihe:pat:apsr:cancer:stomach: 2010', 'urn:ihe:pat:apsr:cancer:liver:2010', 'urn:ihe:pat:apsr:cancer:pancreas: 2010', 'urn:ihe:pat:apsr:cancer:testis:2010', 'urn:ihe:pat:apsr:cancer:urinary_bladder:2010', 'urn:ihe:pat:apsr:cancer:lip_oral_cavity:2010', 'urn:ihe:pat:apsr:cancer:pharynx:2010', 'urn:ihe:pat:apsr:cancer:salivary_gland:2010', 'urn:ihe:pat:apsr:cancer:larynx:2010', 'urn:ihe:pharm:pre:2010', 'urn:ihe:pharm:padv:2010', 'urn:ihe:pharm:dis:2010', 'urn:ihe:pharm:pml:2013', 'urn:hl7-org:sdwg:ccda-structuredBody:1.1', 'urn:hl7-org:sdwg:ccda-nonXMLBody:1.1' ]
        },
        'type'=>'CodeableConcept',
        'path'=>'Endpoint.payloadType',
        'min'=>1,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/endpoint-payload-type'}
      },
      ##
      # Mimetype to send. If not specified, the content could be anything (including no payload, if the connectionType defined this)
      # The mime type to send the payload in - e.g. application/fhir+xml, application/fhir+json. If the mime type is not specified, then the sender could send any content (including no content depending on the connectionType).
      # Sending the payload has obvious security consequences. The server is responsible for ensuring that the content is appropriately secured.
      'payloadMimeType' => {
        'type'=>'code',
        'path'=>'Endpoint.payloadMimeType',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # The technical base address for connecting to this endpoint
      # The uri that describes the actual end-point to connect to.
      # For rest-hook, and websocket, the end-point must be an http: or https: URL; for email, a mailto: url, for sms, a tel: url, and for message the endpoint can be in any form of url the server understands (usually, http: or mllp:). The URI is allowed to be relative; in which case, it is relative to the server end-point (since there may be more than one, clients should avoid using relative URIs)
      # 
      # This address will be to the service base, without any parameters, or sub-services or resources tacked on.
      # 
      # E.g. for a WADO-RS endpoint, the url should be "https://pacs.hospital.org/wado-rs"
      # 
      # and not "https://pacs.hospital.org/wado-rs/studies/1.2.250.1.59.40211.12345678.678910/series/1.2.250.1.59.40211.789001276.14556172.67789/instances/...".
      'address' => {
        'type'=>'url',
        'path'=>'Endpoint.address',
        'min'=>1,
        'max'=>1
      },
      ##
      # Usage depends on the channel type
      # Additional headers / information to send as part of the notification.
      # Exactly what these mean depends on the channel type. The can convey additional information to the recipient and/or meet security requirements.
      'header' => {
        'type'=>'string',
        'path'=>'Endpoint.header',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }
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
    # Identifies this endpoint across multiple systems
    # Identifier for the organization that is used to identify the endpoint across multiple disparate systems.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # active | suspended | error | off | entered-in-error | test
    # active | suspended | error | off | test.
    # This element is labeled as a modifier because the status contains codes that mark the endpoint as not currently valid.
    attr_accessor :status                         # 1-1 code
    ##
    # Protocol/Profile/Standard to be used with this endpoint connection
    # A coded value that represents the technical details of the usage of this endpoint, such as what WSDLs should be used in what way. (e.g. XDS.b/DICOM/cds-hook).
    # For additional connectivity details for the protocol, extensions will be used at this point, as in the XDS example.
    attr_accessor :connectionType                 # 1-1 Coding
    ##
    # A name that this endpoint can be identified by
    # A friendly name that this endpoint can be referred to with.
    attr_accessor :name                           # 0-1 string
    ##
    # Organization that manages this endpoint (might not be the organization that exposes the endpoint)
    # The organization that manages this endpoint (even if technically another organization is hosting this in the cloud, it is the organization associated with the data).
    # This property is not typically used when searching for Endpoint resources for usage. The typical usage is via the reference from an applicable Organization/Location/Practitioner resource, which is where the context is provided. Multiple Locations may reference a single endpoint, and don't have to be within the same organization resource, but most likely within the same organizational hierarchy.
    attr_accessor :managingOrganization           # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # Contact details for source (e.g. troubleshooting)
    # Contact details for a human to contact about the subscription. The primary use of this for system administrator troubleshooting.
    attr_accessor :contact                        # 0-* [ ContactPoint ]
    ##
    # Interval the endpoint is expected to be operational
    # The interval during which the endpoint is expected to be operational.
    attr_accessor :period                         # 0-1 Period
    ##
    # The type of content that may be used at this endpoint (e.g. XDS Discharge summaries)
    # The payload type describes the acceptable content that can be communicated on the endpoint.
    # The payloadFormat describes the serialization format of the data, where the payloadType indicates the specific document/schema that is being transferred; e.g. DischargeSummary or CarePlan.
    attr_accessor :payloadType                    # 1-* [ CodeableConcept ]
    ##
    # Mimetype to send. If not specified, the content could be anything (including no payload, if the connectionType defined this)
    # The mime type to send the payload in - e.g. application/fhir+xml, application/fhir+json. If the mime type is not specified, then the sender could send any content (including no content depending on the connectionType).
    # Sending the payload has obvious security consequences. The server is responsible for ensuring that the content is appropriately secured.
    attr_accessor :payloadMimeType                # 0-* [ code ]
    ##
    # The technical base address for connecting to this endpoint
    # The uri that describes the actual end-point to connect to.
    # For rest-hook, and websocket, the end-point must be an http: or https: URL; for email, a mailto: url, for sms, a tel: url, and for message the endpoint can be in any form of url the server understands (usually, http: or mllp:). The URI is allowed to be relative; in which case, it is relative to the server end-point (since there may be more than one, clients should avoid using relative URIs)
    # 
    # This address will be to the service base, without any parameters, or sub-services or resources tacked on.
    # 
    # E.g. for a WADO-RS endpoint, the url should be "https://pacs.hospital.org/wado-rs"
    # 
    # and not "https://pacs.hospital.org/wado-rs/studies/1.2.250.1.59.40211.12345678.678910/series/1.2.250.1.59.40211.789001276.14556172.67789/instances/...".
    attr_accessor :address                        # 1-1 url
    ##
    # Usage depends on the channel type
    # Additional headers / information to send as part of the notification.
    # Exactly what these mean depends on the channel type. The can convey additional information to the recipient and/or meet security requirements.
    attr_accessor :header                         # 0-* [ string ]

    def resourceType
      'Endpoint'
    end
  end
end
