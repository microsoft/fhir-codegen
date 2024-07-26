module FHIR

  ##
  # A container for a collection of resources.
  class Bundle < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['composition', 'identifier', 'message', 'timestamp', 'type']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'Bundle.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'Bundle.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'Bundle.implicitRules',
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
        'path'=>'Bundle.language',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'preferred', 'uri'=>'http://hl7.org/fhir/ValueSet/languages'}
      },
      ##
      # Persistent identifier for the bundle
      # A persistent identifier for the bundle that won't change as a bundle is copied from server to server.
      # Persistent identity generally only matters for batches of type Document, Message, and Collection. It would not normally be populated for search and history results and servers ignore Bundle.identifier when processing batches and transactions. For Documents  the .identifier SHALL be populated such that the .identifier is globally unique.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'Bundle.identifier',
        'min'=>0,
        'max'=>1
      },
      ##
      # document | message | transaction | transaction-response | batch | batch-response | history | searchset | collection
      # Indicates the purpose of this bundle - how it is intended to be used.
      # It's possible to use a bundle for other purposes (e.g. a document can be accepted as a transaction). This is primarily defined so that there can be specific rules for some of the bundle types.
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/bundle-type'=>[ 'document', 'message', 'transaction', 'transaction-response', 'batch', 'batch-response', 'history', 'searchset', 'collection' ]
        },
        'type'=>'code',
        'path'=>'Bundle.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/bundle-type'}
      },
      ##
      # When the bundle was assembled
      # The date/time that the bundle was assembled - i.e. when the resources were placed in the bundle.
      # For many bundles, the timestamp is equal to .meta.lastUpdated, because they are not stored (e.g. search results). When a bundle is placed in a persistent store, .meta.lastUpdated will be usually be changed by the server. When the bundle is a message, a middleware agent altering the message (even if not stored) SHOULD update .meta.lastUpdated. .timestamp is used to track the original time of the Bundle, and SHOULD be populated.  
      # 
      # Usage:
      # 
      # * document : the date the document was created. Note: the composition may predate the document, or be associated with multiple documents. The date of the composition - the authoring time - may be earlier than the document assembly time
      # * message : the date that the content of the message was assembled. This date is not changed by middleware engines unless they add additional data that changes the meaning of the time of the message
      # * history : the date that the history was assembled. This time would be used as the _since time to ask for subsequent updates
      # * searchset : the time that the search set was assembled. Note that different pages MAY have different timestamps but need not. Having different timestamps does not imply that subsequent pages will represent or include changes made since the initial query
      # * transaction | transaction-response | batch | batch-response | collection : no particular assigned meaning
      # 
      # The timestamp value should be greater than the lastUpdated and other timestamps in the resources in the bundle, and it should be equal or earlier than the .meta.lastUpdated on the Bundle itself.
      'timestamp' => {
        'type'=>'instant',
        'path'=>'Bundle.timestamp',
        'min'=>0,
        'max'=>1
      },
      ##
      # If search, the total number of matches
      # If a set of search matches, this is the total number of entries of type 'match' across all pages in the search.  It does not include search.mode = 'include' or 'outcome' entries and it does not provide a count of the number of entries in the Bundle.
      # Only used if the bundle is a search result set. The total does not include resources such as OperationOutcome and included resources, only the total number of matching resources.
      'total' => {
        'type'=>'unsignedInt',
        'path'=>'Bundle.total',
        'min'=>0,
        'max'=>1
      },
      ##
      # Links related to this Bundle
      # A series of links that provide context to this bundle.
      # Both Bundle.link and Bundle.entry.link are defined to support providing additional context when Bundles are used (e.g. [HATEOAS](http://en.wikipedia.org/wiki/HATEOAS)). 
      # 
      # Bundle.entry.link corresponds to links found in the HTTP header if the resource in the entry was [read](http.html#read) directly.
      # 
      # This specification defines some specific uses of Bundle.link for [searching](search.html#conformance) and [paging](http.html#paging), but no specific uses for Bundle.entry.link, and no defined function in a transaction - the meaning is implementation specific.
      'link' => {
        'type'=>'Bundle::Link',
        'path'=>'Bundle.link',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Entry in the bundle - will have a resource or information
      # An entry in a bundle resource - will either contain a resource or information about a resource (transactions and history only).
      'entry' => {
        'type'=>'Bundle::Entry',
        'path'=>'Bundle.entry',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Digital Signature - base64 encoded. XML-DSig or a JWT.
      # The signature could be created by the "author" of the bundle or by the originating device.   Requirements around inclusion of a signature, verification of signatures and treatment of signed/non-signed bundles is implementation-environment specific.
      'signature' => {
        'type'=>'Signature',
        'path'=>'Bundle.signature',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Links related to this Bundle
    # A series of links that provide context to this bundle.
    # Both Bundle.link and Bundle.entry.link are defined to support providing additional context when Bundles are used (e.g. [HATEOAS](http://en.wikipedia.org/wiki/HATEOAS)). 
    # 
    # Bundle.entry.link corresponds to links found in the HTTP header if the resource in the entry was [read](http.html#read) directly.
    # 
    # This specification defines some specific uses of Bundle.link for [searching](search.html#conformance) and [paging](http.html#paging), but no specific uses for Bundle.entry.link, and no defined function in a transaction - the meaning is implementation specific.
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
        # See http://www.iana.org/assignments/link-relations/link-relations.xhtml#link-relations-1
        # A name which details the functional use for this link - see [http://www.iana.org/assignments/link-relations/link-relations.xhtml#link-relations-1](http://www.iana.org/assignments/link-relations/link-relations.xhtml#link-relations-1).
        'relation' => {
          'type'=>'string',
          'path'=>'Link.relation',
          'min'=>1,
          'max'=>1
        },
        ##
        # Reference details for the link
        # The reference details for the link.
        'url' => {
          'type'=>'uri',
          'path'=>'Link.url',
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
      # See http://www.iana.org/assignments/link-relations/link-relations.xhtml#link-relations-1
      # A name which details the functional use for this link - see [http://www.iana.org/assignments/link-relations/link-relations.xhtml#link-relations-1](http://www.iana.org/assignments/link-relations/link-relations.xhtml#link-relations-1).
      attr_accessor :relation                       # 1-1 string
      ##
      # Reference details for the link
      # The reference details for the link.
      attr_accessor :url                            # 1-1 uri
    end

    ##
    # Entry in the bundle - will have a resource or information
    # An entry in a bundle resource - will either contain a resource or information about a resource (transactions and history only).
    class Entry < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Entry.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Entry.extension',
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
          'path'=>'Entry.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Links related to this entry
        # A series of links that provide context to this entry.
        'link' => {
          'type'=>'Bundle::Link',
          'path'=>'Entry.link',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # URI for resource (Absolute URL server address or URI for UUID/OID)
        # The Absolute URL for the resource.  The fullUrl SHALL NOT disagree with the id in the resource - i.e. if the fullUrl is not a urn:uuid, the URL shall be version-independent URL consistent with the Resource.id. The fullUrl is a version independent reference to the resource. The fullUrl element SHALL have a value except that: 
        # * fullUrl can be empty on a POST (although it does not need to when specifying a temporary id for reference in the bundle)
        # * Results from operations might involve resources that are not identified.
        # fullUrl might not be [unique in the context of a resource](bundle.html#bundle-unique). Note that since [FHIR resources do not need to be served through the FHIR API](references.html), the fullURL might be a URN or an absolute URL that does not end with the logical id of the resource (Resource.id). However, but if the fullUrl does look like a RESTful server URL (e.g. meets the [regex](references.html#regex), then the 'id' portion of the fullUrl SHALL end with the Resource.id.
        # 
        # Note that the fullUrl is not the same as the canonical URL - it's an absolute url for an endpoint serving the resource (these will happen to have the same value on the canonical server for the resource with the canonical URL).
        'fullUrl' => {
          'type'=>'uri',
          'path'=>'Entry.fullUrl',
          'min'=>0,
          'max'=>1
        },
        ##
        # A resource in the bundle
        # The Resource for the entry. The purpose/meaning of the resource is determined by the Bundle.type.
        'resource' => {
          'type'=>'Resource',
          'path'=>'Entry.resource',
          'min'=>0,
          'max'=>1
        },
        ##
        # Search related information
        # Information about the search process that lead to the creation of this entry.
        'search' => {
          'type'=>'Bundle::Entry::Search',
          'path'=>'Entry.search',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional execution information (transaction/batch/history)
        # Additional information about how this entry should be processed as part of a transaction or batch.  For history, it shows how the entry was processed to create the version contained in the entry.
        'request' => {
          'type'=>'Bundle::Entry::Request',
          'path'=>'Entry.request',
          'min'=>0,
          'max'=>1
        },
        ##
        # Results of execution (transaction/batch/history)
        # Indicates the results of processing the corresponding 'request' entry in the batch or transaction being responded to or what the results of an operation where when returning history.
        'response' => {
          'type'=>'Bundle::Entry::Response',
          'path'=>'Entry.response',
          'min'=>0,
          'max'=>1
        }
      }

      ##
      # Search related information
      # Information about the search process that lead to the creation of this entry.
      class Search < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Search.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Search.extension',
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
            'path'=>'Search.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # match | include | outcome - why this is in the result set
          # Why this entry is in the result set - whether it's included as a match or because of an _include requirement, or to convey information or warning information about the search process.
          # There is only one mode. In some corner cases, a resource may be included because it is both a match and an include. In these circumstances, 'match' takes precedence.
          'mode' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/search-entry-mode'=>[ 'match', 'include', 'outcome' ]
            },
            'type'=>'code',
            'path'=>'Search.mode',
            'min'=>0,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/search-entry-mode'}
          },
          ##
          # Search ranking (between 0 and 1)
          # When searching, the server's search ranking score for the entry.
          # Servers are not required to return a ranking score. 1 is most relevant, and 0 is least relevant. Often, search results are sorted by score, but the client may specify a different sort order.
          # 
          # See [Patient Match](patient-operation-match.html) for the EMPI search which relates to this element.
          'score' => {
            'type'=>'decimal',
            'path'=>'Search.score',
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
        # match | include | outcome - why this is in the result set
        # Why this entry is in the result set - whether it's included as a match or because of an _include requirement, or to convey information or warning information about the search process.
        # There is only one mode. In some corner cases, a resource may be included because it is both a match and an include. In these circumstances, 'match' takes precedence.
        attr_accessor :mode                           # 0-1 code
        ##
        # Search ranking (between 0 and 1)
        # When searching, the server's search ranking score for the entry.
        # Servers are not required to return a ranking score. 1 is most relevant, and 0 is least relevant. Often, search results are sorted by score, but the client may specify a different sort order.
        # 
        # See [Patient Match](patient-operation-match.html) for the EMPI search which relates to this element.
        attr_accessor :score                          # 0-1 decimal
      end

      ##
      # Additional execution information (transaction/batch/history)
      # Additional information about how this entry should be processed as part of a transaction or batch.  For history, it shows how the entry was processed to create the version contained in the entry.
      class Request < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Request.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Request.extension',
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
            'path'=>'Request.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # GET | HEAD | POST | PUT | DELETE | PATCH
          # In a transaction or batch, this is the HTTP action to be executed for this entry. In a history bundle, this indicates the HTTP action that occurred.
          'method' => {
            'local_name'=>'local_method'
            'valid_codes'=>{
              'http://hl7.org/fhir/http-verb'=>[ 'GET', 'HEAD', 'POST', 'PUT', 'DELETE', 'PATCH' ]
            },
            'type'=>'code',
            'path'=>'Request.method',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/http-verb'}
          },
          ##
          # URL for HTTP equivalent of this entry
          # The URL for this entry, relative to the root (the address to which the request is posted).
          # E.g. for a Patient Create, the method would be "POST" and the URL would be "Patient". For a Patient Update, the method would be PUT and the URL would be "Patient/[id]".
          'url' => {
            'type'=>'uri',
            'path'=>'Request.url',
            'min'=>1,
            'max'=>1
          },
          ##
          # For managing cache currency
          # If the ETag values match, return a 304 Not Modified status. See the API documentation for ["Conditional Read"](http.html#cread).
          'ifNoneMatch' => {
            'type'=>'string',
            'path'=>'Request.ifNoneMatch',
            'min'=>0,
            'max'=>1
          },
          ##
          # For managing cache currency
          # Only perform the operation if the last updated date matches. See the API documentation for ["Conditional Read"](http.html#cread).
          'ifModifiedSince' => {
            'type'=>'instant',
            'path'=>'Request.ifModifiedSince',
            'min'=>0,
            'max'=>1
          },
          ##
          # For managing update contention
          # Only perform the operation if the Etag value matches. For more information, see the API section ["Managing Resource Contention"](http.html#concurrency).
          'ifMatch' => {
            'type'=>'string',
            'path'=>'Request.ifMatch',
            'min'=>0,
            'max'=>1
          },
          ##
          # For conditional creates
          # Instruct the server not to perform the create if a specified resource already exists. For further information, see the API documentation for ["Conditional Create"](http.html#ccreate). This is just the query portion of the URL - what follows the "?" (not including the "?").
          'ifNoneExist' => {
            'type'=>'string',
            'path'=>'Request.ifNoneExist',
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
        # GET | HEAD | POST | PUT | DELETE | PATCH
        # In a transaction or batch, this is the HTTP action to be executed for this entry. In a history bundle, this indicates the HTTP action that occurred.
        attr_accessor :local_method                   # 1-1 code
        ##
        # URL for HTTP equivalent of this entry
        # The URL for this entry, relative to the root (the address to which the request is posted).
        # E.g. for a Patient Create, the method would be "POST" and the URL would be "Patient". For a Patient Update, the method would be PUT and the URL would be "Patient/[id]".
        attr_accessor :url                            # 1-1 uri
        ##
        # For managing cache currency
        # If the ETag values match, return a 304 Not Modified status. See the API documentation for ["Conditional Read"](http.html#cread).
        attr_accessor :ifNoneMatch                    # 0-1 string
        ##
        # For managing cache currency
        # Only perform the operation if the last updated date matches. See the API documentation for ["Conditional Read"](http.html#cread).
        attr_accessor :ifModifiedSince                # 0-1 instant
        ##
        # For managing update contention
        # Only perform the operation if the Etag value matches. For more information, see the API section ["Managing Resource Contention"](http.html#concurrency).
        attr_accessor :ifMatch                        # 0-1 string
        ##
        # For conditional creates
        # Instruct the server not to perform the create if a specified resource already exists. For further information, see the API documentation for ["Conditional Create"](http.html#ccreate). This is just the query portion of the URL - what follows the "?" (not including the "?").
        attr_accessor :ifNoneExist                    # 0-1 string
      end

      ##
      # Results of execution (transaction/batch/history)
      # Indicates the results of processing the corresponding 'request' entry in the batch or transaction being responded to or what the results of an operation where when returning history.
      class Response < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Response.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Response.extension',
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
            'path'=>'Response.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Status response code (text optional)
          # The status code returned by processing this entry. The status SHALL start with a 3 digit HTTP code (e.g. 404) and may contain the standard HTTP description associated with the status code.
          'status' => {
            'type'=>'string',
            'path'=>'Response.status',
            'min'=>1,
            'max'=>1
          },
          ##
          # The location (if the operation returns a location)
          # The location header created by processing this operation, populated if the operation returns a location.
          'location' => {
            'type'=>'uri',
            'path'=>'Response.location',
            'min'=>0,
            'max'=>1
          },
          ##
          # The Etag for the resource (if relevant)
          # The Etag for the resource, if the operation for the entry produced a versioned resource (see [Resource Metadata and Versioning](http.html#versioning) and [Managing Resource Contention](http.html#concurrency)).
          # Etags match the Resource.meta.versionId. The ETag has to match the version id in the header if a resource is included.
          'etag' => {
            'type'=>'string',
            'path'=>'Response.etag',
            'min'=>0,
            'max'=>1
          },
          ##
          # Server's date time modified
          # The date/time that the resource was modified on the server.
          # This has to match the same time in the meta header (meta.lastUpdated) if a resource is included.
          'lastModified' => {
            'type'=>'instant',
            'path'=>'Response.lastModified',
            'min'=>0,
            'max'=>1
          },
          ##
          # OperationOutcome with hints and warnings (for batch/transaction)
          # An OperationOutcome containing hints and warnings produced as part of processing this entry in a batch or transaction.
          # For a POST/PUT operation, this is the equivalent outcome that would be returned for prefer = operationoutcome - except that the resource is always returned whether or not the outcome is returned.
          # 
          # This outcome is not used for error responses in batch/transaction, only for hints and warnings. In a batch operation, the error will be in Bundle.entry.response, and for transaction, there will be a single OperationOutcome instead of a bundle in the case of an error.
          'outcome' => {
            'type'=>'Resource',
            'path'=>'Response.outcome',
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
        # Status response code (text optional)
        # The status code returned by processing this entry. The status SHALL start with a 3 digit HTTP code (e.g. 404) and may contain the standard HTTP description associated with the status code.
        attr_accessor :status                         # 1-1 string
        ##
        # The location (if the operation returns a location)
        # The location header created by processing this operation, populated if the operation returns a location.
        attr_accessor :location                       # 0-1 uri
        ##
        # The Etag for the resource (if relevant)
        # The Etag for the resource, if the operation for the entry produced a versioned resource (see [Resource Metadata and Versioning](http.html#versioning) and [Managing Resource Contention](http.html#concurrency)).
        # Etags match the Resource.meta.versionId. The ETag has to match the version id in the header if a resource is included.
        attr_accessor :etag                           # 0-1 string
        ##
        # Server's date time modified
        # The date/time that the resource was modified on the server.
        # This has to match the same time in the meta header (meta.lastUpdated) if a resource is included.
        attr_accessor :lastModified                   # 0-1 instant
        ##
        # OperationOutcome with hints and warnings (for batch/transaction)
        # An OperationOutcome containing hints and warnings produced as part of processing this entry in a batch or transaction.
        # For a POST/PUT operation, this is the equivalent outcome that would be returned for prefer = operationoutcome - except that the resource is always returned whether or not the outcome is returned.
        # 
        # This outcome is not used for error responses in batch/transaction, only for hints and warnings. In a batch operation, the error will be in Bundle.entry.response, and for transaction, there will be a single OperationOutcome instead of a bundle in the case of an error.
        attr_accessor :outcome                        # 0-1 Resource
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
      # Links related to this entry
      # A series of links that provide context to this entry.
      attr_accessor :link                           # 0-* [ Bundle::Link ]
      ##
      # URI for resource (Absolute URL server address or URI for UUID/OID)
      # The Absolute URL for the resource.  The fullUrl SHALL NOT disagree with the id in the resource - i.e. if the fullUrl is not a urn:uuid, the URL shall be version-independent URL consistent with the Resource.id. The fullUrl is a version independent reference to the resource. The fullUrl element SHALL have a value except that: 
      # * fullUrl can be empty on a POST (although it does not need to when specifying a temporary id for reference in the bundle)
      # * Results from operations might involve resources that are not identified.
      # fullUrl might not be [unique in the context of a resource](bundle.html#bundle-unique). Note that since [FHIR resources do not need to be served through the FHIR API](references.html), the fullURL might be a URN or an absolute URL that does not end with the logical id of the resource (Resource.id). However, but if the fullUrl does look like a RESTful server URL (e.g. meets the [regex](references.html#regex), then the 'id' portion of the fullUrl SHALL end with the Resource.id.
      # 
      # Note that the fullUrl is not the same as the canonical URL - it's an absolute url for an endpoint serving the resource (these will happen to have the same value on the canonical server for the resource with the canonical URL).
      attr_accessor :fullUrl                        # 0-1 uri
      ##
      # A resource in the bundle
      # The Resource for the entry. The purpose/meaning of the resource is determined by the Bundle.type.
      attr_accessor :resource                       # 0-1 Resource
      ##
      # Search related information
      # Information about the search process that lead to the creation of this entry.
      attr_accessor :search                         # 0-1 Bundle::Entry::Search
      ##
      # Additional execution information (transaction/batch/history)
      # Additional information about how this entry should be processed as part of a transaction or batch.  For history, it shows how the entry was processed to create the version contained in the entry.
      attr_accessor :request                        # 0-1 Bundle::Entry::Request
      ##
      # Results of execution (transaction/batch/history)
      # Indicates the results of processing the corresponding 'request' entry in the batch or transaction being responded to or what the results of an operation where when returning history.
      attr_accessor :response                       # 0-1 Bundle::Entry::Response
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
    # Persistent identifier for the bundle
    # A persistent identifier for the bundle that won't change as a bundle is copied from server to server.
    # Persistent identity generally only matters for batches of type Document, Message, and Collection. It would not normally be populated for search and history results and servers ignore Bundle.identifier when processing batches and transactions. For Documents  the .identifier SHALL be populated such that the .identifier is globally unique.
    attr_accessor :identifier                     # 0-1 Identifier
    ##
    # document | message | transaction | transaction-response | batch | batch-response | history | searchset | collection
    # Indicates the purpose of this bundle - how it is intended to be used.
    # It's possible to use a bundle for other purposes (e.g. a document can be accepted as a transaction). This is primarily defined so that there can be specific rules for some of the bundle types.
    attr_accessor :type                           # 1-1 code
    ##
    # When the bundle was assembled
    # The date/time that the bundle was assembled - i.e. when the resources were placed in the bundle.
    # For many bundles, the timestamp is equal to .meta.lastUpdated, because they are not stored (e.g. search results). When a bundle is placed in a persistent store, .meta.lastUpdated will be usually be changed by the server. When the bundle is a message, a middleware agent altering the message (even if not stored) SHOULD update .meta.lastUpdated. .timestamp is used to track the original time of the Bundle, and SHOULD be populated.  
    # 
    # Usage:
    # 
    # * document : the date the document was created. Note: the composition may predate the document, or be associated with multiple documents. The date of the composition - the authoring time - may be earlier than the document assembly time
    # * message : the date that the content of the message was assembled. This date is not changed by middleware engines unless they add additional data that changes the meaning of the time of the message
    # * history : the date that the history was assembled. This time would be used as the _since time to ask for subsequent updates
    # * searchset : the time that the search set was assembled. Note that different pages MAY have different timestamps but need not. Having different timestamps does not imply that subsequent pages will represent or include changes made since the initial query
    # * transaction | transaction-response | batch | batch-response | collection : no particular assigned meaning
    # 
    # The timestamp value should be greater than the lastUpdated and other timestamps in the resources in the bundle, and it should be equal or earlier than the .meta.lastUpdated on the Bundle itself.
    attr_accessor :timestamp                      # 0-1 instant
    ##
    # If search, the total number of matches
    # If a set of search matches, this is the total number of entries of type 'match' across all pages in the search.  It does not include search.mode = 'include' or 'outcome' entries and it does not provide a count of the number of entries in the Bundle.
    # Only used if the bundle is a search result set. The total does not include resources such as OperationOutcome and included resources, only the total number of matching resources.
    attr_accessor :total                          # 0-1 unsignedInt
    ##
    # Links related to this Bundle
    # A series of links that provide context to this bundle.
    # Both Bundle.link and Bundle.entry.link are defined to support providing additional context when Bundles are used (e.g. [HATEOAS](http://en.wikipedia.org/wiki/HATEOAS)). 
    # 
    # Bundle.entry.link corresponds to links found in the HTTP header if the resource in the entry was [read](http.html#read) directly.
    # 
    # This specification defines some specific uses of Bundle.link for [searching](search.html#conformance) and [paging](http.html#paging), but no specific uses for Bundle.entry.link, and no defined function in a transaction - the meaning is implementation specific.
    attr_accessor :link                           # 0-* [ Bundle::Link ]
    ##
    # Entry in the bundle - will have a resource or information
    # An entry in a bundle resource - will either contain a resource or information about a resource (transactions and history only).
    attr_accessor :entry                          # 0-* [ Bundle::Entry ]
    ##
    # Digital Signature - base64 encoded. XML-DSig or a JWT.
    # The signature could be created by the "author" of the bundle or by the originating device.   Requirements around inclusion of a signature, verification of signatures and treatment of signed/non-signed bundles is implementation-environment specific.
    attr_accessor :signature                      # 0-1 Signature

    def resourceType
      'Bundle'
    end
  end
end
