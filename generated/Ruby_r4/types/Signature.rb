module FHIR

  ##
  # Base StructureDefinition for Signature Type: A signature along with supporting context. The signature may be a digital signature that is cryptographic in nature, or some other signature acceptable to the domain. This other signature may be as simple as a graphical image representing a hand-written signature, or a signature ceremony Different signature approaches have different utilities.
  # There are a number of places where content must be signed in healthcare.
  class Signature < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = []
    METADATA = {
      ##
      # Unique id for inter-element referencing
      # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
      'id' => {
        'type'=>'id',
        'path'=>'Signature.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Signature.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Indication of the reason the entity signed the object(s)
      # An indication of the reason that the entity signed this document. This may be explicitly included as part of the signature information and can be used when determining accountability for various actions concerning the document.
      # Examples include attesting to: authorship, correct transcription, and witness of specific event. Also known as a &quot;Commitment Type Indication&quot;.
      'type' => {
        'valid_codes'=>{
          'urn:iso-astm:E1762-95:2013'=>[ '1.2.840.10065.1.12.1.1', '1.2.840.10065.1.12.1.2', '1.2.840.10065.1.12.1.3', '1.2.840.10065.1.12.1.4', '1.2.840.10065.1.12.1.5', '1.2.840.10065.1.12.1.6', '1.2.840.10065.1.12.1.7', '1.2.840.10065.1.12.1.8', '1.2.840.10065.1.12.1.9', '1.2.840.10065.1.12.1.10', '1.2.840.10065.1.12.1.11', '1.2.840.10065.1.12.1.12', '1.2.840.10065.1.12.1.13', '1.2.840.10065.1.12.1.14', '1.2.840.10065.1.12.1.15', '1.2.840.10065.1.12.1.16', '1.2.840.10065.1.12.1.17', '1.2.840.10065.1.12.1.18' ]
        },
        'type'=>'Coding',
        'path'=>'Signature.type',
        'min'=>1,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/signature-type'}
      },
      ##
      # When the signature was created
      # When the digital signature was signed.
      # This should agree with the information in the signature.
      'when' => {
        'local_name'=>'local_when'
        'type'=>'instant',
        'path'=>'Signature.when',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who signed
      # A reference to an application-usable description of the identity that signed  (e.g. the signature used their private key).
      # This should agree with the information in the signature.
      'who' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Signature.who',
        'min'=>1,
        'max'=>1
      },
      ##
      # The party represented
      # A reference to an application-usable description of the identity that is represented by the signature.
      # The party that can't sign. For example a child.
      'onBehalfOf' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Practitioner', 'http://hl7.org/fhir/StructureDefinition/PractitionerRole', 'http://hl7.org/fhir/StructureDefinition/RelatedPerson', 'http://hl7.org/fhir/StructureDefinition/Patient', 'http://hl7.org/fhir/StructureDefinition/Device', 'http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'Signature.onBehalfOf',
        'min'=>0,
        'max'=>1
      },
      ##
      # The technical format of the signed resources
      # A mime type that indicates the technical format of the target resources signed by the signature.
      # "xml", "json" and "ttl" are allowed, which describe the simple encodings described in the specification (and imply appropriate bundle support). Otherwise, mime types are legal here.
      'targetFormat' => {
        'type'=>'code',
        'path'=>'Signature.targetFormat',
        'min'=>0,
        'max'=>1
      },
      ##
      # The technical format of the signature
      # A mime type that indicates the technical format of the signature. Important mime types are application/signature+xml for X ML DigSig, application/jose for JWS, and image/* for a graphical image of a signature, etc.
      'sigFormat' => {
        'type'=>'code',
        'path'=>'Signature.sigFormat',
        'min'=>0,
        'max'=>1
      },
      ##
      # The actual signature content (XML DigSig. JWS, picture, etc.)
      # The base64 encoding of the Signature content. When signature is not recorded electronically this element would be empty.
      # Where the signature type is an XML DigSig, the signed content is a FHIR Resource(s), the signature is of the XML form of the Resource(s) using  XML-Signature (XMLDIG) "Detached Signature" form.
      'data' => {
        'type'=>'base64Binary',
        'path'=>'Signature.data',
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
    # Indication of the reason the entity signed the object(s)
    # An indication of the reason that the entity signed this document. This may be explicitly included as part of the signature information and can be used when determining accountability for various actions concerning the document.
    # Examples include attesting to: authorship, correct transcription, and witness of specific event. Also known as a &quot;Commitment Type Indication&quot;.
    attr_accessor :type                           # 1-* [ Coding ]
    ##
    # When the signature was created
    # When the digital signature was signed.
    # This should agree with the information in the signature.
    attr_accessor :local_when                     # 1-1 instant
    ##
    # Who signed
    # A reference to an application-usable description of the identity that signed  (e.g. the signature used their private key).
    # This should agree with the information in the signature.
    attr_accessor :who                            # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # The party represented
    # A reference to an application-usable description of the identity that is represented by the signature.
    # The party that can't sign. For example a child.
    attr_accessor :onBehalfOf                     # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Practitioner|http://hl7.org/fhir/StructureDefinition/PractitionerRole|http://hl7.org/fhir/StructureDefinition/RelatedPerson|http://hl7.org/fhir/StructureDefinition/Patient|http://hl7.org/fhir/StructureDefinition/Device|http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # The technical format of the signed resources
    # A mime type that indicates the technical format of the target resources signed by the signature.
    # "xml", "json" and "ttl" are allowed, which describe the simple encodings described in the specification (and imply appropriate bundle support). Otherwise, mime types are legal here.
    attr_accessor :targetFormat                   # 0-1 code
    ##
    # The technical format of the signature
    # A mime type that indicates the technical format of the signature. Important mime types are application/signature+xml for X ML DigSig, application/jose for JWS, and image/* for a graphical image of a signature, etc.
    attr_accessor :sigFormat                      # 0-1 code
    ##
    # The actual signature content (XML DigSig. JWS, picture, etc.)
    # The base64 encoding of the Signature content. When signature is not recorded electronically this element would be empty.
    # Where the signature type is an XML DigSig, the signed content is a FHIR Resource(s), the signature is of the XML form of the Resource(s) using  XML-Signature (XMLDIG) "Detached Signature" form.
    attr_accessor :data                           # 0-1 base64Binary
  end
end
