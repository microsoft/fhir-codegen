module FHIR

  ##
  # Base StructureDefinition for RelatedArtifact Type: Related artifacts such as additional documentation, justification, or bibliographic references.
  # Knowledge resources must be able to provide enough information for consumers of the content (and/or interventions or results produced by the content) to be able to determine and understand the justification for and evidence in support of the content.
  class RelatedArtifact < FHIR::Model
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
        'path'=>'RelatedArtifact.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'RelatedArtifact.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # documentation | justification | citation | predecessor | successor | derived-from | depends-on | composed-of
      # The type of relationship to the related artifact.
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/related-artifact-type'=>[ 'documentation', 'justification', 'citation', 'predecessor', 'successor', 'derived-from', 'depends-on', 'composed-of' ]
        },
        'type'=>'code',
        'path'=>'RelatedArtifact.type',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/related-artifact-type'}
      },
      ##
      # Short label
      # A short label that can be used to reference the citation from elsewhere in the containing artifact, such as a footnote index.
      'label' => {
        'type'=>'string',
        'path'=>'RelatedArtifact.label',
        'min'=>0,
        'max'=>1
      },
      ##
      # Brief description of the related artifact
      # A brief description of the document or knowledge resource being referenced, suitable for display to a consumer.
      'display' => {
        'type'=>'string',
        'path'=>'RelatedArtifact.display',
        'min'=>0,
        'max'=>1
      },
      ##
      # Bibliographic citation for the artifact
      # A bibliographic citation for the related artifact. This text SHOULD be formatted according to an accepted citation format.
      # Additional structured information about citations should be captured as extensions.
      'citation' => {
        'type'=>'markdown',
        'path'=>'RelatedArtifact.citation',
        'min'=>0,
        'max'=>1
      },
      ##
      # Where the artifact can be accessed
      # A url for the artifact that can be followed to access the actual content.
      # If a document or resource element is present, this element SHALL NOT be provided (use the url or reference in the Attachment or resource reference).
      'url' => {
        'type'=>'url',
        'path'=>'RelatedArtifact.url',
        'min'=>0,
        'max'=>1
      },
      ##
      # What document is being referenced
      # The document being referenced, represented as an attachment. This is exclusive with the resource element.
      'document' => {
        'type'=>'Attachment',
        'path'=>'RelatedArtifact.document',
        'min'=>0,
        'max'=>1
      },
      ##
      # What resource is being referenced
      # The related resource, such as a library, value set, profile, or other knowledge resource.
      # If the type is predecessor, this is a reference to the succeeding knowledge resource. If the type is successor, this is a reference to the prior knowledge resource.
      'resource' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
        'type'=>'canonical',
        'path'=>'RelatedArtifact.resource',
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
    # documentation | justification | citation | predecessor | successor | derived-from | depends-on | composed-of
    # The type of relationship to the related artifact.
    attr_accessor :type                           # 1-1 code
    ##
    # Short label
    # A short label that can be used to reference the citation from elsewhere in the containing artifact, such as a footnote index.
    attr_accessor :label                          # 0-1 string
    ##
    # Brief description of the related artifact
    # A brief description of the document or knowledge resource being referenced, suitable for display to a consumer.
    attr_accessor :display                        # 0-1 string
    ##
    # Bibliographic citation for the artifact
    # A bibliographic citation for the related artifact. This text SHOULD be formatted according to an accepted citation format.
    # Additional structured information about citations should be captured as extensions.
    attr_accessor :citation                       # 0-1 markdown
    ##
    # Where the artifact can be accessed
    # A url for the artifact that can be followed to access the actual content.
    # If a document or resource element is present, this element SHALL NOT be provided (use the url or reference in the Attachment or resource reference).
    attr_accessor :url                            # 0-1 url
    ##
    # What document is being referenced
    # The document being referenced, represented as an attachment. This is exclusive with the resource element.
    attr_accessor :document                       # 0-1 Attachment
    ##
    # What resource is being referenced
    # The related resource, such as a library, value set, profile, or other knowledge resource.
    # If the type is predecessor, this is a reference to the succeeding knowledge resource. If the type is successor, this is a reference to the prior knowledge resource.
    attr_accessor :resource                       # 0-1 canonical(http://hl7.org/fhir/StructureDefinition/Resource)
  end
end
