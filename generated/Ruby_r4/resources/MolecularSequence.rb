module FHIR

  ##
  # Raw data describing a biological sequence.
  class MolecularSequence < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['chromosome-variant-coordinate', 'chromosome-window-coordinate', 'chromosome', 'identifier', 'patient', 'referenceseqid-variant-coordinate', 'referenceseqid-window-coordinate', 'referenceseqid', 'type', 'variant-end', 'variant-start', 'window-end', 'window-start']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'id',
        'path'=>'MolecularSequence.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'MolecularSequence.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'MolecularSequence.implicitRules',
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
        'path'=>'MolecularSequence.language',
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
        'path'=>'MolecularSequence.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'MolecularSequence.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'MolecularSequence.extension',
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
        'path'=>'MolecularSequence.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Unique ID for this particular sequence. This is a FHIR-defined id
      # A unique identifier for this particular sequence instance. This is a FHIR-defined id.
      'identifier' => {
        'type'=>'Identifier',
        'path'=>'MolecularSequence.identifier',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # aa | dna | rna
      # Amino Acid Sequence/ DNA Sequence / RNA Sequence.
      'type' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/sequence-type'=>[ 'aa', 'dna', 'rna' ]
        },
        'type'=>'code',
        'path'=>'MolecularSequence.type',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/sequence-type'}
      },
      ##
      # Base number of coordinate system (0 for 0-based numbering or coordinates, inclusive start, exclusive end, 1 for 1-based numbering, inclusive start, inclusive end)
      # Whether the sequence is numbered starting at 0 (0-based numbering or coordinates, inclusive start, exclusive end) or starting at 1 (1-based numbering, inclusive start and inclusive end).
      'coordinateSystem' => {
        'type'=>'integer',
        'path'=>'MolecularSequence.coordinateSystem',
        'min'=>1,
        'max'=>1
      },
      ##
      # Who and/or what this is about
      # The patient whose sequencing results are described by this resource.
      'patient' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Patient'],
        'type'=>'Reference',
        'path'=>'MolecularSequence.patient',
        'min'=>0,
        'max'=>1
      },
      ##
      # Specimen used for sequencing.
      'specimen' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Specimen'],
        'type'=>'Reference',
        'path'=>'MolecularSequence.specimen',
        'min'=>0,
        'max'=>1
      },
      ##
      # The method for sequencing, for example, chip information.
      'device' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Device'],
        'type'=>'Reference',
        'path'=>'MolecularSequence.device',
        'min'=>0,
        'max'=>1
      },
      ##
      # Who should be responsible for test result
      # The organization or lab that should be responsible for this result.
      'performer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Organization'],
        'type'=>'Reference',
        'path'=>'MolecularSequence.performer',
        'min'=>0,
        'max'=>1
      },
      ##
      # The number of copies of the sequence of interest.  (RNASeq)
      # The number of copies of the sequence of interest. (RNASeq).
      'quantity' => {
        'type'=>'Quantity',
        'path'=>'MolecularSequence.quantity',
        'min'=>0,
        'max'=>1
      },
      ##
      # A sequence used as reference
      # A sequence that is used as a reference to describe variants that are present in a sequence analyzed.
      'referenceSeq' => {
        'type'=>'MolecularSequence::ReferenceSeq',
        'path'=>'MolecularSequence.referenceSeq',
        'min'=>0,
        'max'=>1
      },
      ##
      # Variant in sequence
      # The definition of variant here originates from Sequence ontology ([variant_of](http://www.sequenceontology.org/browser/current_svn/term/variant_of)). This element can represent amino acid or nucleic sequence change(including insertion,deletion,SNP,etc.)  It can represent some complex mutation or segment variation with the assist of CIGAR string.
      'variant' => {
        'type'=>'MolecularSequence::Variant',
        'path'=>'MolecularSequence.variant',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Sequence that was observed. It is the result marked by referenceSeq along with variant records on referenceSeq. This shall start from referenceSeq.windowStart and end by referenceSeq.windowEnd.
      'observedSeq' => {
        'type'=>'string',
        'path'=>'MolecularSequence.observedSeq',
        'min'=>0,
        'max'=>1
      },
      ##
      # An set of value as quality of sequence
      # An experimental feature attribute that defines the quality of the feature in a quantitative way, such as a phred quality score ([SO:0001686](http://www.sequenceontology.org/browser/current_svn/term/SO:0001686)).
      'quality' => {
        'type'=>'MolecularSequence::Quality',
        'path'=>'MolecularSequence.quality',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Average number of reads representing a given nucleotide in the reconstructed sequence
      # Coverage (read depth or depth) is the average number of reads representing a given nucleotide in the reconstructed sequence.
      'readCoverage' => {
        'type'=>'integer',
        'path'=>'MolecularSequence.readCoverage',
        'min'=>0,
        'max'=>1
      },
      ##
      # External repository which contains detailed report related with observedSeq in this resource
      # Configurations of the external repository. The repository shall store target's observedSeq or records related with target's observedSeq.
      'repository' => {
        'type'=>'MolecularSequence::Repository',
        'path'=>'MolecularSequence.repository',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Pointer to next atomic sequence which at most contains one variant.
      'pointer' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MolecularSequence'],
        'type'=>'Reference',
        'path'=>'MolecularSequence.pointer',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Structural variant
      # Information about chromosome structure variation.
      'structureVariant' => {
        'type'=>'MolecularSequence::StructureVariant',
        'path'=>'MolecularSequence.structureVariant',
        'min'=>0,
        'max'=>Float::INFINITY
      }
    }

    ##
    # A sequence used as reference
    # A sequence that is used as a reference to describe variants that are present in a sequence analyzed.
    class ReferenceSeq < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'ReferenceSeq.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'ReferenceSeq.extension',
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
          'path'=>'ReferenceSeq.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Chromosome containing genetic finding
        # Structural unit composed of a nucleic acid molecule which controls its own replication through the interaction of specific proteins at one or more origins of replication ([SO:0000340](http://www.sequenceontology.org/browser/current_svn/term/SO:0000340)).
        'chromosome' => {
          'valid_codes'=>{
            'http://terminology.hl7.org/CodeSystem/chromosome-human'=>[ '1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', 'X', 'Y' ]
          },
          'type'=>'CodeableConcept',
          'path'=>'ReferenceSeq.chromosome',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/chromosome-human'}
        },
        ##
        # The Genome Build used for reference, following GRCh build versions e.g. 'GRCh 37'.  Version number must be included if a versioned release of a primary build was used.
        'genomeBuild' => {
          'type'=>'string',
          'path'=>'ReferenceSeq.genomeBuild',
          'min'=>0,
          'max'=>1
        },
        ##
        # sense | antisense
        # A relative reference to a DNA strand based on gene orientation. The strand that contains the open reading frame of the gene is the "sense" strand, and the opposite complementary strand is the "antisense" strand.
        'orientation' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/orientation-type'=>[ 'sense', 'antisense' ]
          },
          'type'=>'code',
          'path'=>'ReferenceSeq.orientation',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/orientation-type'}
        },
        ##
        # Reference identifier of reference sequence submitted to NCBI. It must match the type in the MolecularSequence.type field. For example, the prefix, “NG_” identifies reference sequence for genes, “NM_” for messenger RNA transcripts, and “NP_” for amino acid sequences.
        'referenceSeqId' => {
          'type'=>'CodeableConcept',
          'path'=>'ReferenceSeq.referenceSeqId',
          'min'=>0,
          'max'=>1
        },
        ##
        # A pointer to another MolecularSequence entity as reference sequence.
        'referenceSeqPointer' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/MolecularSequence'],
          'type'=>'Reference',
          'path'=>'ReferenceSeq.referenceSeqPointer',
          'min'=>0,
          'max'=>1
        },
        ##
        # A string to represent reference sequence
        # A string like "ACGT".
        'referenceSeqString' => {
          'type'=>'string',
          'path'=>'ReferenceSeq.referenceSeqString',
          'min'=>0,
          'max'=>1
        },
        ##
        # watson | crick
        # An absolute reference to a strand. The Watson strand is the strand whose 5'-end is on the short arm of the chromosome, and the Crick strand as the one whose 5'-end is on the long arm.
        'strand' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/strand-type'=>[ 'watson', 'crick' ]
          },
          'type'=>'code',
          'path'=>'ReferenceSeq.strand',
          'min'=>0,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/strand-type'}
        },
        ##
        # Start position of the window on the  reference sequence
        # Start position of the window on the reference sequence. If the coordinate system is either 0-based or 1-based, then start position is inclusive.
        'windowStart' => {
          'type'=>'integer',
          'path'=>'ReferenceSeq.windowStart',
          'min'=>0,
          'max'=>1
        },
        ##
        # End position of the window on the reference sequence. If the coordinate system is 0-based then end is exclusive and does not include the last position. If the coordinate system is 1-base, then end is inclusive and includes the last position.
        'windowEnd' => {
          'type'=>'integer',
          'path'=>'ReferenceSeq.windowEnd',
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
      # Chromosome containing genetic finding
      # Structural unit composed of a nucleic acid molecule which controls its own replication through the interaction of specific proteins at one or more origins of replication ([SO:0000340](http://www.sequenceontology.org/browser/current_svn/term/SO:0000340)).
      attr_accessor :chromosome                     # 0-1 CodeableConcept
      ##
      # The Genome Build used for reference, following GRCh build versions e.g. 'GRCh 37'.  Version number must be included if a versioned release of a primary build was used.
      attr_accessor :genomeBuild                    # 0-1 string
      ##
      # sense | antisense
      # A relative reference to a DNA strand based on gene orientation. The strand that contains the open reading frame of the gene is the "sense" strand, and the opposite complementary strand is the "antisense" strand.
      attr_accessor :orientation                    # 0-1 code
      ##
      # Reference identifier of reference sequence submitted to NCBI. It must match the type in the MolecularSequence.type field. For example, the prefix, “NG_” identifies reference sequence for genes, “NM_” for messenger RNA transcripts, and “NP_” for amino acid sequences.
      attr_accessor :referenceSeqId                 # 0-1 CodeableConcept
      ##
      # A pointer to another MolecularSequence entity as reference sequence.
      attr_accessor :referenceSeqPointer            # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/MolecularSequence)
      ##
      # A string to represent reference sequence
      # A string like "ACGT".
      attr_accessor :referenceSeqString             # 0-1 string
      ##
      # watson | crick
      # An absolute reference to a strand. The Watson strand is the strand whose 5'-end is on the short arm of the chromosome, and the Crick strand as the one whose 5'-end is on the long arm.
      attr_accessor :strand                         # 0-1 code
      ##
      # Start position of the window on the  reference sequence
      # Start position of the window on the reference sequence. If the coordinate system is either 0-based or 1-based, then start position is inclusive.
      attr_accessor :windowStart                    # 0-1 integer
      ##
      # End position of the window on the reference sequence. If the coordinate system is 0-based then end is exclusive and does not include the last position. If the coordinate system is 1-base, then end is inclusive and includes the last position.
      attr_accessor :windowEnd                      # 0-1 integer
    end

    ##
    # Variant in sequence
    # The definition of variant here originates from Sequence ontology ([variant_of](http://www.sequenceontology.org/browser/current_svn/term/variant_of)). This element can represent amino acid or nucleic sequence change(including insertion,deletion,SNP,etc.)  It can represent some complex mutation or segment variation with the assist of CIGAR string.
    class Variant < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Variant.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Variant.extension',
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
          'path'=>'Variant.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Start position of the variant on the  reference sequence. If the coordinate system is either 0-based or 1-based, then start position is inclusive.
        'start' => {
          'type'=>'integer',
          'path'=>'Variant.start',
          'min'=>0,
          'max'=>1
        },
        ##
        # End position of the variant on the reference sequence. If the coordinate system is 0-based then end is exclusive and does not include the last position. If the coordinate system is 1-base, then end is inclusive and includes the last position.
        'end' => {
          'local_name'=>'local_end'
          'type'=>'integer',
          'path'=>'Variant.end',
          'min'=>0,
          'max'=>1
        },
        ##
        # Allele that was observed
        # An allele is one of a set of coexisting sequence variants of a gene ([SO:0001023](http://www.sequenceontology.org/browser/current_svn/term/SO:0001023)).  Nucleotide(s)/amino acids from start position of sequence to stop position of sequence on the positive (+) strand of the observed  sequence. When the sequence  type is DNA, it should be the sequence on the positive (+) strand. This will lay in the range between variant.start and variant.end.
        'observedAllele' => {
          'type'=>'string',
          'path'=>'Variant.observedAllele',
          'min'=>0,
          'max'=>1
        },
        ##
        # Allele in the reference sequence
        # An allele is one of a set of coexisting sequence variants of a gene ([SO:0001023](http://www.sequenceontology.org/browser/current_svn/term/SO:0001023)). Nucleotide(s)/amino acids from start position of sequence to stop position of sequence on the positive (+) strand of the reference sequence. When the sequence  type is DNA, it should be the sequence on the positive (+) strand. This will lay in the range between variant.start and variant.end.
        'referenceAllele' => {
          'type'=>'string',
          'path'=>'Variant.referenceAllele',
          'min'=>0,
          'max'=>1
        },
        ##
        # Extended CIGAR string for aligning the sequence with reference bases. See detailed documentation [here](http://support.illumina.com/help/SequencingAnalysisWorkflow/Content/Vault/Informatics/Sequencing_Analysis/CASAVA/swSEQ_mCA_ExtendedCIGARFormat.htm).
        'cigar' => {
          'type'=>'string',
          'path'=>'Variant.cigar',
          'min'=>0,
          'max'=>1
        },
        ##
        # Pointer to observed variant information
        # A pointer to an Observation containing variant information.
        'variantPointer' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Observation'],
          'type'=>'Reference',
          'path'=>'Variant.variantPointer',
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
      # Start position of the variant on the  reference sequence. If the coordinate system is either 0-based or 1-based, then start position is inclusive.
      attr_accessor :start                          # 0-1 integer
      ##
      # End position of the variant on the reference sequence. If the coordinate system is 0-based then end is exclusive and does not include the last position. If the coordinate system is 1-base, then end is inclusive and includes the last position.
      attr_accessor :local_end                      # 0-1 integer
      ##
      # Allele that was observed
      # An allele is one of a set of coexisting sequence variants of a gene ([SO:0001023](http://www.sequenceontology.org/browser/current_svn/term/SO:0001023)).  Nucleotide(s)/amino acids from start position of sequence to stop position of sequence on the positive (+) strand of the observed  sequence. When the sequence  type is DNA, it should be the sequence on the positive (+) strand. This will lay in the range between variant.start and variant.end.
      attr_accessor :observedAllele                 # 0-1 string
      ##
      # Allele in the reference sequence
      # An allele is one of a set of coexisting sequence variants of a gene ([SO:0001023](http://www.sequenceontology.org/browser/current_svn/term/SO:0001023)). Nucleotide(s)/amino acids from start position of sequence to stop position of sequence on the positive (+) strand of the reference sequence. When the sequence  type is DNA, it should be the sequence on the positive (+) strand. This will lay in the range between variant.start and variant.end.
      attr_accessor :referenceAllele                # 0-1 string
      ##
      # Extended CIGAR string for aligning the sequence with reference bases. See detailed documentation [here](http://support.illumina.com/help/SequencingAnalysisWorkflow/Content/Vault/Informatics/Sequencing_Analysis/CASAVA/swSEQ_mCA_ExtendedCIGARFormat.htm).
      attr_accessor :cigar                          # 0-1 string
      ##
      # Pointer to observed variant information
      # A pointer to an Observation containing variant information.
      attr_accessor :variantPointer                 # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Observation)
    end

    ##
    # An set of value as quality of sequence
    # An experimental feature attribute that defines the quality of the feature in a quantitative way, such as a phred quality score ([SO:0001686](http://www.sequenceontology.org/browser/current_svn/term/SO:0001686)).
    class Quality < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Quality.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Quality.extension',
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
          'path'=>'Quality.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # indel | snp | unknown
        # INDEL / SNP / Undefined variant.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/quality-type'=>[ 'indel', 'snp', 'unknown' ]
          },
          'type'=>'code',
          'path'=>'Quality.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/quality-type'}
        },
        ##
        # Standard sequence for comparison
        # Gold standard sequence used for comparing against.
        'standardSequence' => {
          'type'=>'CodeableConcept',
          'path'=>'Quality.standardSequence',
          'min'=>0,
          'max'=>1
        },
        ##
        # Start position of the sequence. If the coordinate system is either 0-based or 1-based, then start position is inclusive.
        'start' => {
          'type'=>'integer',
          'path'=>'Quality.start',
          'min'=>0,
          'max'=>1
        },
        ##
        # End position of the sequence. If the coordinate system is 0-based then end is exclusive and does not include the last position. If the coordinate system is 1-base, then end is inclusive and includes the last position.
        'end' => {
          'local_name'=>'local_end'
          'type'=>'integer',
          'path'=>'Quality.end',
          'min'=>0,
          'max'=>1
        },
        ##
        # Quality score for the comparison
        # The score of an experimentally derived feature such as a p-value ([SO:0001685](http://www.sequenceontology.org/browser/current_svn/term/SO:0001685)).
        'score' => {
          'type'=>'Quantity',
          'path'=>'Quality.score',
          'min'=>0,
          'max'=>1
        },
        ##
        # Method to get quality
        # Which method is used to get sequence quality.
        'method' => {
          'local_name'=>'local_method'
          'type'=>'CodeableConcept',
          'path'=>'Quality.method',
          'min'=>0,
          'max'=>1
        },
        ##
        # True positives from the perspective of the truth data
        # True positives, from the perspective of the truth data, i.e. the number of sites in the Truth Call Set for which there are paths through the Query Call Set that are consistent with all of the alleles at this site, and for which there is an accurate genotype call for the event.
        'truthTP' => {
          'type'=>'decimal',
          'path'=>'Quality.truthTP',
          'min'=>0,
          'max'=>1
        },
        ##
        # True positives from the perspective of the query data
        # True positives, from the perspective of the query data, i.e. the number of sites in the Query Call Set for which there are paths through the Truth Call Set that are consistent with all of the alleles at this site, and for which there is an accurate genotype call for the event.
        'queryTP' => {
          'type'=>'decimal',
          'path'=>'Quality.queryTP',
          'min'=>0,
          'max'=>1
        },
        ##
        # False negatives, i.e. the number of sites in the Truth Call Set for which there is no path through the Query Call Set that is consistent with all of the alleles at this site, or sites for which there is an inaccurate genotype call for the event. Sites with correct variant but incorrect genotype are counted here.
        'truthFN' => {
          'type'=>'decimal',
          'path'=>'Quality.truthFN',
          'min'=>0,
          'max'=>1
        },
        ##
        # False positives, i.e. the number of sites in the Query Call Set for which there is no path through the Truth Call Set that is consistent with this site. Sites with correct variant but incorrect genotype are counted here.
        'queryFP' => {
          'type'=>'decimal',
          'path'=>'Quality.queryFP',
          'min'=>0,
          'max'=>1
        },
        ##
        # False positives where the non-REF alleles in the Truth and Query Call Sets match
        # The number of false positives where the non-REF alleles in the Truth and Query Call Sets match (i.e. cases where the truth is 1/1 and the query is 0/1 or similar).
        'gtFP' => {
          'type'=>'decimal',
          'path'=>'Quality.gtFP',
          'min'=>0,
          'max'=>1
        },
        ##
        # Precision of comparison
        # QUERY.TP / (QUERY.TP + QUERY.FP).
        'precision' => {
          'type'=>'decimal',
          'path'=>'Quality.precision',
          'min'=>0,
          'max'=>1
        },
        ##
        # Recall of comparison
        # TRUTH.TP / (TRUTH.TP + TRUTH.FN).
        'recall' => {
          'type'=>'decimal',
          'path'=>'Quality.recall',
          'min'=>0,
          'max'=>1
        },
        ##
        # F-score
        # Harmonic mean of Recall and Precision, computed as: 2 * precision * recall / (precision + recall).
        'fScore' => {
          'type'=>'decimal',
          'path'=>'Quality.fScore',
          'min'=>0,
          'max'=>1
        },
        ##
        # Receiver Operator Characteristic (ROC) Curve  to give sensitivity/specificity tradeoff.
        'roc' => {
          'type'=>'MolecularSequence::Quality::Roc',
          'path'=>'Quality.roc',
          'min'=>0,
          'max'=>1
        }
      }

      ##
      # Receiver Operator Characteristic (ROC) Curve  to give sensitivity/specificity tradeoff.
      class Roc < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Roc.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Roc.extension',
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
            'path'=>'Roc.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Genotype quality score
          # Invidual data point representing the GQ (genotype quality) score threshold.
          'score' => {
            'type'=>'integer',
            'path'=>'Roc.score',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Roc score true positive numbers
          # The number of true positives if the GQ score threshold was set to "score" field value.
          'numTP' => {
            'type'=>'integer',
            'path'=>'Roc.numTP',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Roc score false positive numbers
          # The number of false positives if the GQ score threshold was set to "score" field value.
          'numFP' => {
            'type'=>'integer',
            'path'=>'Roc.numFP',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Roc score false negative numbers
          # The number of false negatives if the GQ score threshold was set to "score" field value.
          'numFN' => {
            'type'=>'integer',
            'path'=>'Roc.numFN',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Precision of the GQ score
          # Calculated precision if the GQ score threshold was set to "score" field value.
          'precision' => {
            'type'=>'decimal',
            'path'=>'Roc.precision',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Sensitivity of the GQ score
          # Calculated sensitivity if the GQ score threshold was set to "score" field value.
          'sensitivity' => {
            'type'=>'decimal',
            'path'=>'Roc.sensitivity',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # FScore of the GQ score
          # Calculated fScore if the GQ score threshold was set to "score" field value.
          'fMeasure' => {
            'type'=>'decimal',
            'path'=>'Roc.fMeasure',
            'min'=>0,
            'max'=>Float::INFINITY
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
        # Genotype quality score
        # Invidual data point representing the GQ (genotype quality) score threshold.
        attr_accessor :score                          # 0-* [ integer ]
        ##
        # Roc score true positive numbers
        # The number of true positives if the GQ score threshold was set to "score" field value.
        attr_accessor :numTP                          # 0-* [ integer ]
        ##
        # Roc score false positive numbers
        # The number of false positives if the GQ score threshold was set to "score" field value.
        attr_accessor :numFP                          # 0-* [ integer ]
        ##
        # Roc score false negative numbers
        # The number of false negatives if the GQ score threshold was set to "score" field value.
        attr_accessor :numFN                          # 0-* [ integer ]
        ##
        # Precision of the GQ score
        # Calculated precision if the GQ score threshold was set to "score" field value.
        attr_accessor :precision                      # 0-* [ decimal ]
        ##
        # Sensitivity of the GQ score
        # Calculated sensitivity if the GQ score threshold was set to "score" field value.
        attr_accessor :sensitivity                    # 0-* [ decimal ]
        ##
        # FScore of the GQ score
        # Calculated fScore if the GQ score threshold was set to "score" field value.
        attr_accessor :fMeasure                       # 0-* [ decimal ]
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
      # indel | snp | unknown
      # INDEL / SNP / Undefined variant.
      attr_accessor :type                           # 1-1 code
      ##
      # Standard sequence for comparison
      # Gold standard sequence used for comparing against.
      attr_accessor :standardSequence               # 0-1 CodeableConcept
      ##
      # Start position of the sequence. If the coordinate system is either 0-based or 1-based, then start position is inclusive.
      attr_accessor :start                          # 0-1 integer
      ##
      # End position of the sequence. If the coordinate system is 0-based then end is exclusive and does not include the last position. If the coordinate system is 1-base, then end is inclusive and includes the last position.
      attr_accessor :local_end                      # 0-1 integer
      ##
      # Quality score for the comparison
      # The score of an experimentally derived feature such as a p-value ([SO:0001685](http://www.sequenceontology.org/browser/current_svn/term/SO:0001685)).
      attr_accessor :score                          # 0-1 Quantity
      ##
      # Method to get quality
      # Which method is used to get sequence quality.
      attr_accessor :local_method                   # 0-1 CodeableConcept
      ##
      # True positives from the perspective of the truth data
      # True positives, from the perspective of the truth data, i.e. the number of sites in the Truth Call Set for which there are paths through the Query Call Set that are consistent with all of the alleles at this site, and for which there is an accurate genotype call for the event.
      attr_accessor :truthTP                        # 0-1 decimal
      ##
      # True positives from the perspective of the query data
      # True positives, from the perspective of the query data, i.e. the number of sites in the Query Call Set for which there are paths through the Truth Call Set that are consistent with all of the alleles at this site, and for which there is an accurate genotype call for the event.
      attr_accessor :queryTP                        # 0-1 decimal
      ##
      # False negatives, i.e. the number of sites in the Truth Call Set for which there is no path through the Query Call Set that is consistent with all of the alleles at this site, or sites for which there is an inaccurate genotype call for the event. Sites with correct variant but incorrect genotype are counted here.
      attr_accessor :truthFN                        # 0-1 decimal
      ##
      # False positives, i.e. the number of sites in the Query Call Set for which there is no path through the Truth Call Set that is consistent with this site. Sites with correct variant but incorrect genotype are counted here.
      attr_accessor :queryFP                        # 0-1 decimal
      ##
      # False positives where the non-REF alleles in the Truth and Query Call Sets match
      # The number of false positives where the non-REF alleles in the Truth and Query Call Sets match (i.e. cases where the truth is 1/1 and the query is 0/1 or similar).
      attr_accessor :gtFP                           # 0-1 decimal
      ##
      # Precision of comparison
      # QUERY.TP / (QUERY.TP + QUERY.FP).
      attr_accessor :precision                      # 0-1 decimal
      ##
      # Recall of comparison
      # TRUTH.TP / (TRUTH.TP + TRUTH.FN).
      attr_accessor :recall                         # 0-1 decimal
      ##
      # F-score
      # Harmonic mean of Recall and Precision, computed as: 2 * precision * recall / (precision + recall).
      attr_accessor :fScore                         # 0-1 decimal
      ##
      # Receiver Operator Characteristic (ROC) Curve  to give sensitivity/specificity tradeoff.
      attr_accessor :roc                            # 0-1 MolecularSequence::Quality::Roc
    end

    ##
    # External repository which contains detailed report related with observedSeq in this resource
    # Configurations of the external repository. The repository shall store target's observedSeq or records related with target's observedSeq.
    class Repository < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'Repository.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Repository.extension',
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
          'path'=>'Repository.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # directlink | openapi | login | oauth | other
        # Click and see / RESTful API / Need login to see / RESTful API with authentication / Other ways to see resource.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/repository-type'=>[ 'directlink', 'openapi', 'login', 'oauth', 'other' ]
          },
          'type'=>'code',
          'path'=>'Repository.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/repository-type'}
        },
        ##
        # URI of the repository
        # URI of an external repository which contains further details about the genetics data.
        'url' => {
          'type'=>'uri',
          'path'=>'Repository.url',
          'min'=>0,
          'max'=>1
        },
        ##
        # Repository's name
        # URI of an external repository which contains further details about the genetics data.
        'name' => {
          'type'=>'string',
          'path'=>'Repository.name',
          'min'=>0,
          'max'=>1
        },
        ##
        # Id of the dataset that used to call for dataset in repository
        # Id of the variant in this external repository. The server will understand how to use this id to call for more info about datasets in external repository.
        'datasetId' => {
          'type'=>'string',
          'path'=>'Repository.datasetId',
          'min'=>0,
          'max'=>1
        },
        ##
        # Id of the variantset that used to call for variantset in repository
        # Id of the variantset in this external repository. The server will understand how to use this id to call for more info about variantsets in external repository.
        'variantsetId' => {
          'type'=>'string',
          'path'=>'Repository.variantsetId',
          'min'=>0,
          'max'=>1
        },
        ##
        # Id of the read in this external repository.
        'readsetId' => {
          'type'=>'string',
          'path'=>'Repository.readsetId',
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
      # directlink | openapi | login | oauth | other
      # Click and see / RESTful API / Need login to see / RESTful API with authentication / Other ways to see resource.
      attr_accessor :type                           # 1-1 code
      ##
      # URI of the repository
      # URI of an external repository which contains further details about the genetics data.
      attr_accessor :url                            # 0-1 uri
      ##
      # Repository's name
      # URI of an external repository which contains further details about the genetics data.
      attr_accessor :name                           # 0-1 string
      ##
      # Id of the dataset that used to call for dataset in repository
      # Id of the variant in this external repository. The server will understand how to use this id to call for more info about datasets in external repository.
      attr_accessor :datasetId                      # 0-1 string
      ##
      # Id of the variantset that used to call for variantset in repository
      # Id of the variantset in this external repository. The server will understand how to use this id to call for more info about variantsets in external repository.
      attr_accessor :variantsetId                   # 0-1 string
      ##
      # Id of the read in this external repository.
      attr_accessor :readsetId                      # 0-1 string
    end

    ##
    # Structural variant
    # Information about chromosome structure variation.
    class StructureVariant < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'id',
          'path'=>'StructureVariant.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'StructureVariant.extension',
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
          'path'=>'StructureVariant.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Structural variant change type
        # Information about chromosome structure variation DNA change type.
        'variantType' => {
          'type'=>'CodeableConcept',
          'path'=>'StructureVariant.variantType',
          'min'=>0,
          'max'=>1
        },
        ##
        # Does the structural variant have base pair resolution breakpoints?
        # Used to indicate if the outer and inner start-end values have the same meaning.
        'exact' => {
          'type'=>'boolean',
          'path'=>'StructureVariant.exact',
          'min'=>0,
          'max'=>1
        },
        ##
        # Structural variant length
        # Length of the variant chromosome.
        'length' => {
          'type'=>'integer',
          'path'=>'StructureVariant.length',
          'min'=>0,
          'max'=>1
        },
        ##
        # Structural variant outer.
        'outer' => {
          'type'=>'MolecularSequence::StructureVariant::Outer',
          'path'=>'StructureVariant.outer',
          'min'=>0,
          'max'=>1
        },
        ##
        # Structural variant inner.
        'inner' => {
          'type'=>'MolecularSequence::StructureVariant::Inner',
          'path'=>'StructureVariant.inner',
          'min'=>0,
          'max'=>1
        }
      }

      ##
      # Structural variant outer.
      class Outer < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Outer.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Outer.extension',
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
            'path'=>'Outer.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Structural variant outer start. If the coordinate system is either 0-based or 1-based, then start position is inclusive.
          'start' => {
            'type'=>'integer',
            'path'=>'Outer.start',
            'min'=>0,
            'max'=>1
          },
          ##
          # Structural variant outer end. If the coordinate system is 0-based then end is exclusive and does not include the last position. If the coordinate system is 1-base, then end is inclusive and includes the last position.
          'end' => {
            'local_name'=>'local_end'
            'type'=>'integer',
            'path'=>'Outer.end',
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
        # Structural variant outer start. If the coordinate system is either 0-based or 1-based, then start position is inclusive.
        attr_accessor :start                          # 0-1 integer
        ##
        # Structural variant outer end. If the coordinate system is 0-based then end is exclusive and does not include the last position. If the coordinate system is 1-base, then end is inclusive and includes the last position.
        attr_accessor :local_end                      # 0-1 integer
      end

      ##
      # Structural variant inner.
      class Inner < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'id',
            'path'=>'Inner.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Inner.extension',
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
            'path'=>'Inner.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Structural variant inner start. If the coordinate system is either 0-based or 1-based, then start position is inclusive.
          'start' => {
            'type'=>'integer',
            'path'=>'Inner.start',
            'min'=>0,
            'max'=>1
          },
          ##
          # Structural variant inner end. If the coordinate system is 0-based then end is exclusive and does not include the last position. If the coordinate system is 1-base, then end is inclusive and includes the last position.
          'end' => {
            'local_name'=>'local_end'
            'type'=>'integer',
            'path'=>'Inner.end',
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
        # Structural variant inner start. If the coordinate system is either 0-based or 1-based, then start position is inclusive.
        attr_accessor :start                          # 0-1 integer
        ##
        # Structural variant inner end. If the coordinate system is 0-based then end is exclusive and does not include the last position. If the coordinate system is 1-base, then end is inclusive and includes the last position.
        attr_accessor :local_end                      # 0-1 integer
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
      # Structural variant change type
      # Information about chromosome structure variation DNA change type.
      attr_accessor :variantType                    # 0-1 CodeableConcept
      ##
      # Does the structural variant have base pair resolution breakpoints?
      # Used to indicate if the outer and inner start-end values have the same meaning.
      attr_accessor :exact                          # 0-1 boolean
      ##
      # Structural variant length
      # Length of the variant chromosome.
      attr_accessor :length                         # 0-1 integer
      ##
      # Structural variant outer.
      attr_accessor :outer                          # 0-1 MolecularSequence::StructureVariant::Outer
      ##
      # Structural variant inner.
      attr_accessor :inner                          # 0-1 MolecularSequence::StructureVariant::Inner
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
    # Unique ID for this particular sequence. This is a FHIR-defined id
    # A unique identifier for this particular sequence instance. This is a FHIR-defined id.
    attr_accessor :identifier                     # 0-* [ Identifier ]
    ##
    # aa | dna | rna
    # Amino Acid Sequence/ DNA Sequence / RNA Sequence.
    attr_accessor :type                           # 0-1 code
    ##
    # Base number of coordinate system (0 for 0-based numbering or coordinates, inclusive start, exclusive end, 1 for 1-based numbering, inclusive start, inclusive end)
    # Whether the sequence is numbered starting at 0 (0-based numbering or coordinates, inclusive start, exclusive end) or starting at 1 (1-based numbering, inclusive start and inclusive end).
    attr_accessor :coordinateSystem               # 1-1 integer
    ##
    # Who and/or what this is about
    # The patient whose sequencing results are described by this resource.
    attr_accessor :patient                        # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Patient)
    ##
    # Specimen used for sequencing.
    attr_accessor :specimen                       # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Specimen)
    ##
    # The method for sequencing, for example, chip information.
    attr_accessor :device                         # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Device)
    ##
    # Who should be responsible for test result
    # The organization or lab that should be responsible for this result.
    attr_accessor :performer                      # 0-1 Reference(http://hl7.org/fhir/StructureDefinition/Organization)
    ##
    # The number of copies of the sequence of interest.  (RNASeq)
    # The number of copies of the sequence of interest. (RNASeq).
    attr_accessor :quantity                       # 0-1 Quantity
    ##
    # A sequence used as reference
    # A sequence that is used as a reference to describe variants that are present in a sequence analyzed.
    attr_accessor :referenceSeq                   # 0-1 MolecularSequence::ReferenceSeq
    ##
    # Variant in sequence
    # The definition of variant here originates from Sequence ontology ([variant_of](http://www.sequenceontology.org/browser/current_svn/term/variant_of)). This element can represent amino acid or nucleic sequence change(including insertion,deletion,SNP,etc.)  It can represent some complex mutation or segment variation with the assist of CIGAR string.
    attr_accessor :variant                        # 0-* [ MolecularSequence::Variant ]
    ##
    # Sequence that was observed. It is the result marked by referenceSeq along with variant records on referenceSeq. This shall start from referenceSeq.windowStart and end by referenceSeq.windowEnd.
    attr_accessor :observedSeq                    # 0-1 string
    ##
    # An set of value as quality of sequence
    # An experimental feature attribute that defines the quality of the feature in a quantitative way, such as a phred quality score ([SO:0001686](http://www.sequenceontology.org/browser/current_svn/term/SO:0001686)).
    attr_accessor :quality                        # 0-* [ MolecularSequence::Quality ]
    ##
    # Average number of reads representing a given nucleotide in the reconstructed sequence
    # Coverage (read depth or depth) is the average number of reads representing a given nucleotide in the reconstructed sequence.
    attr_accessor :readCoverage                   # 0-1 integer
    ##
    # External repository which contains detailed report related with observedSeq in this resource
    # Configurations of the external repository. The repository shall store target's observedSeq or records related with target's observedSeq.
    attr_accessor :repository                     # 0-* [ MolecularSequence::Repository ]
    ##
    # Pointer to next atomic sequence which at most contains one variant.
    attr_accessor :pointer                        # 0-* [ Reference(http://hl7.org/fhir/StructureDefinition/MolecularSequence) ]
    ##
    # Structural variant
    # Information about chromosome structure variation.
    attr_accessor :structureVariant               # 0-* [ MolecularSequence::StructureVariant ]

    def resourceType
      'MolecularSequence'
    end
  end
end
