module FHIR

  ##
  # Base StructureDefinition for Attachment Type: For referring to data content defined in other formats.
  # Many models need to include data defined in other specifications that is complex and opaque to the healthcare model. This includes documents, media recordings, structured data, etc.
  class Attachment < FHIR::Model
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
        'path'=>'Attachment.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Attachment.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Mime type of the content, with charset etc.
      # Identifies the type of the data in the attachment and allows a method to be chosen to interpret or render the data. Includes mime type parameters such as charset where appropriate.
      'contentType' => {
        'type'=>'code',
        'path'=>'Attachment.contentType',
        'min'=>0,
        'max'=>1
      },
      ##
      # Human language of the content (BCP-47)
      # The human language of the content. The value can be any valid value according to BCP 47.
      'language' => {
        'valid_codes'=>{
          'urn:ietf:bcp:47'=>[ 'ar', 'bn', 'cs', 'da', 'de', 'de-AT', 'de-CH', 'de-DE', 'el', 'en', 'en-AU', 'en-CA', 'en-GB', 'en-IN', 'en-NZ', 'en-SG', 'en-US', 'es', 'es-AR', 'es-ES', 'es-UY', 'fi', 'fr', 'fr-BE', 'fr-CH', 'fr-FR', 'fy', 'fy-NL', 'hi', 'hr', 'it', 'it-CH', 'it-IT', 'ja', 'ko', 'nl', 'nl-BE', 'nl-NL', 'no', 'no-NO', 'pa', 'pl', 'pt', 'pt-BR', 'ru', 'ru-RU', 'sr', 'sr-RS', 'sv', 'sv-SE', 'te', 'zh', 'zh-CN', 'zh-HK', 'zh-SG', 'zh-TW' ]
        },
        'type'=>'code',
        'path'=>'Attachment.language',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
      },
      ##
      # Data inline, base64ed
      # The actual data of the attachment - a sequence of bytes, base64 encoded.
      # The base64-encoded data SHALL be expressed in the same character set as the base resource XML or JSON.
      'data' => {
        'type'=>'base64Binary',
        'path'=>'Attachment.data',
        'min'=>0,
        'max'=>1
      },
      ##
      # Uri where the data can be found
      # A location where the data can be accessed.
      # If both data and url are provided, the url SHALL point to the same content as the data contains. Urls may be relative references or may reference transient locations such as a wrapping envelope using cid: though this has ramifications for using signatures. Relative URLs are interpreted relative to the service url, like a resource reference, rather than relative to the resource itself. If a URL is provided, it SHALL resolve to actual data.
      'url' => {
        'type'=>'url',
        'path'=>'Attachment.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # Number of bytes of content (if url provided)
      # The number of bytes of data that make up this attachment (before base64 encoding, if that is done).
      # The number of bytes is redundant if the data is provided as a base64binary, but is useful if the data is provided as a url reference.
      'size' => {
        'type'=>'unsignedInt',
        'path'=>'Attachment.size',
        'min'=>0,
        'max'=>1
      },
      ##
      # Hash of the data (sha-1, base64ed)
      # The calculated hash of the data using SHA-1. Represented using base64.
      # The hash is calculated on the data prior to base64 encoding, if the data is based64 encoded. The hash is not intended to support digital signatures. Where protection against malicious threats a digital signature should be considered, see [Provenance.signature](provenance-definitions.html#Provenance.signature) for mechanism to protect a resource with a digital signature.
      'hash' => {
        'type'=>'base64Binary',
        'path'=>'Attachment.hash',
        'min'=>0,
        'max'=>1
      },
      ##
      # Label to display in place of the data
      # A label or set of text to display in place of the data.
      'title' => {
        'type'=>'string',
        'path'=>'Attachment.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date attachment was first created
      # The date that the attachment was first created.
      'creation' => {
        'type'=>'dateTime',
        'path'=>'Attachment.creation',
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
    # Mime type of the content, with charset etc.
    # Identifies the type of the data in the attachment and allows a method to be chosen to interpret or render the data. Includes mime type parameters such as charset where appropriate.
    attr_accessor :contentType                    # 0-1 code
    ##
    # Human language of the content (BCP-47)
    # The human language of the content. The value can be any valid value according to BCP 47.
    attr_accessor :language                       # 0-1 code
    ##
    # Data inline, base64ed
    # The actual data of the attachment - a sequence of bytes, base64 encoded.
    # The base64-encoded data SHALL be expressed in the same character set as the base resource XML or JSON.
    attr_accessor :data                           # 0-1 base64Binary
    ##
    # Uri where the data can be found
    # A location where the data can be accessed.
    # If both data and url are provided, the url SHALL point to the same content as the data contains. Urls may be relative references or may reference transient locations such as a wrapping envelope using cid: though this has ramifications for using signatures. Relative URLs are interpreted relative to the service url, like a resource reference, rather than relative to the resource itself. If a URL is provided, it SHALL resolve to actual data.
    attr_accessor :url                            # 0-1 url
    ##
    # Number of bytes of content (if url provided)
    # The number of bytes of data that make up this attachment (before base64 encoding, if that is done).
    # The number of bytes is redundant if the data is provided as a base64binary, but is useful if the data is provided as a url reference.
    attr_accessor :size                           # 0-1 unsignedInt
    ##
    # Hash of the data (sha-1, base64ed)
    # The calculated hash of the data using SHA-1. Represented using base64.
    # The hash is calculated on the data prior to base64 encoding, if the data is based64 encoded. The hash is not intended to support digital signatures. Where protection against malicious threats a digital signature should be considered, see [Provenance.signature](provenance-definitions.html#Provenance.signature) for mechanism to protect a resource with a digital signature.
    attr_accessor :hash                           # 0-1 base64Binary
    ##
    # Label to display in place of the data
    # A label or set of text to display in place of the data.
    attr_accessor :title                          # 0-1 string
    ##
    # Date attachment was first created
    # The date that the attachment was first created.
    attr_accessor :creation                       # 0-1 dateTime
  end
end
