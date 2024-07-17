module FHIR

  ##
  # A set of rules of how a particular interoperability or standards problem is solved - typically through the use of FHIR resources. This resource is used to gather all the parts of an implementation guide into a logical whole and to publish a computable definition of all the parts.
  # An implementation guide is able to define default profiles that must apply to any use of a resource, so validation services may need to take one or more implementation guide resources when validating.
  class ImplementationGuide < FHIR::Model
    include FHIR::Hashable
    include FHIR::Json
    include FHIR::Xml

    SEARCH_PARAMS = ['context-quantity', 'context-type-quantity', 'context-type-value', 'context-type', 'context', 'date', 'depends-on', 'description', 'experimental', 'global', 'jurisdiction', 'name', 'publisher', 'resource', 'status', 'title', 'url', 'version']
    METADATA = {
      ##
      # Logical id of this artifact
      # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
      # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
      'id' => {
        'type'=>'string',
        'path'=>'ImplementationGuide.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Metadata about the resource
      # The metadata about the resource. This is content that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
      'meta' => {
        'type'=>'Meta',
        'path'=>'ImplementationGuide.meta',
        'min'=>0,
        'max'=>1
      },
      ##
      # A set of rules under which this content was created
      # A reference to a set of rules that were followed when the resource was constructed, and which must be understood when processing the content. Often, this is a reference to an implementation guide that defines the special rules along with other profiles etc.
      # Asserting this rule set restricts the content to be only understood by a limited set of trading partners. This inherently limits the usefulness of the data in the long term. However, the existing health eco-system is highly fractured, and not yet ready to define, collect, and exchange data in a generally computable sense. Wherever possible, implementers and/or specification writers should avoid using this element. Often, when used, the URL is a reference to an implementation guide that defines these special rules as part of it's narrative along with other profiles, value sets, etc.
      'implicitRules' => {
        'type'=>'uri',
        'path'=>'ImplementationGuide.implicitRules',
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
        'path'=>'ImplementationGuide.language',
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
        'path'=>'ImplementationGuide.text',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contained, inline Resources
      # These resources do not have an independent existence apart from the resource that contains them - they cannot be identified independently, and nor can they have their own independent transaction scope.
      # This should never be done when the content can be identified properly, as once identification is lost, it is extremely difficult (and context dependent) to restore it again. Contained resources may have profiles and tags In their meta elements, but SHALL NOT have security labels.
      'contained' => {
        'type'=>'Resource',
        'path'=>'ImplementationGuide.contained',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the resource. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'ImplementationGuide.extension',
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
        'path'=>'ImplementationGuide.modifierExtension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Canonical identifier for this implementation guide, represented as a URI (globally unique)
      # An absolute URI that is used to identify this implementation guide when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this implementation guide is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the implementation guide is stored on different servers.
      # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
      # 
      # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
      # 
      # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
      'url' => {
        'type'=>'uri',
        'path'=>'ImplementationGuide.url',
        'min'=>1,
        'max'=>1
      },
      ##
      # Business version of the implementation guide
      # The identifier that is used to identify this version of the implementation guide when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the implementation guide author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
      # There may be different implementation guide instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the implementation guide with the format [url]|[version].
      'version' => {
        'type'=>'string',
        'path'=>'ImplementationGuide.version',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name for this implementation guide (computer friendly)
      # A natural language name identifying the implementation guide. This name should be usable as an identifier for the module by machine processing applications such as code generation.
      # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
      'name' => {
        'type'=>'string',
        'path'=>'ImplementationGuide.name',
        'min'=>1,
        'max'=>1
      },
      ##
      # Name for this implementation guide (human friendly)
      # A short, descriptive, user-friendly title for the implementation guide.
      # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
      'title' => {
        'type'=>'string',
        'path'=>'ImplementationGuide.title',
        'min'=>0,
        'max'=>1
      },
      ##
      # draft | active | retired | unknown
      # The status of this implementation guide. Enables tracking the life-cycle of the content.
      # Allows filtering of implementation guides that are appropriate for use versus not.
      'status' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/publication-status'=>[ 'draft', 'active', 'retired', 'unknown' ]
        },
        'type'=>'code',
        'path'=>'ImplementationGuide.status',
        'min'=>1,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/publication-status'}
      },
      ##
      # For testing purposes, not real usage
      # A Boolean value to indicate that this implementation guide is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
      # Allows filtering of implementation guides that are appropriate for use versus not.
      'experimental' => {
        'type'=>'boolean',
        'path'=>'ImplementationGuide.experimental',
        'min'=>0,
        'max'=>1
      },
      ##
      # Date last changed
      # The date  (and optionally time) when the implementation guide was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the implementation guide changes.
      # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the implementation guide. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
      'date' => {
        'type'=>'dateTime',
        'path'=>'ImplementationGuide.date',
        'min'=>0,
        'max'=>1
      },
      ##
      # Name of the publisher (organization or individual)
      # The name of the organization or individual that published the implementation guide.
      # Usually an organization but may be an individual. The publisher (or steward) of the implementation guide is the organization or individual primarily responsible for the maintenance and upkeep of the implementation guide. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the implementation guide. This item SHOULD be populated unless the information is available from context.
      'publisher' => {
        'type'=>'string',
        'path'=>'ImplementationGuide.publisher',
        'min'=>0,
        'max'=>1
      },
      ##
      # Contact details for the publisher
      # Contact details to assist a user in finding and communicating with the publisher.
      # May be a web site, an email address, a telephone number, etc.
      'contact' => {
        'type'=>'ContactDetail',
        'path'=>'ImplementationGuide.contact',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Natural language description of the implementation guide
      # A free text natural language description of the implementation guide from a consumer's perspective.
      # This description can be used to capture details such as why the implementation guide was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the implementation guide as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the implementation guide is presumed to be the predominant language in the place the implementation guide was created).
      'description' => {
        'type'=>'markdown',
        'path'=>'ImplementationGuide.description',
        'min'=>0,
        'max'=>1
      },
      ##
      # The context that the content is intended to support
      # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate implementation guide instances.
      # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
      'useContext' => {
        'type'=>'UsageContext',
        'path'=>'ImplementationGuide.useContext',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Intended jurisdiction for implementation guide (if applicable)
      # A legal or geographic region in which the implementation guide is intended to be used.
      # It may be possible for the implementation guide to be used in jurisdictions other than those for which it was originally designed or intended.
      'jurisdiction' => {
        'type'=>'CodeableConcept',
        'path'=>'ImplementationGuide.jurisdiction',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Use and/or publishing restrictions
      # A copyright statement relating to the implementation guide and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the implementation guide.
      'copyright' => {
        'type'=>'markdown',
        'path'=>'ImplementationGuide.copyright',
        'min'=>0,
        'max'=>1
      },
      ##
      # NPM Package name for IG
      # The NPM package name for this Implementation Guide, used in the NPM package distribution, which is the primary mechanism by which FHIR based tooling manages IG dependencies. This value must be globally unique, and should be assigned with care.
      # Many (if not all) IG publishing tools will require that this element be present. For implementation guides published through HL7 or the FHIR foundation, the FHIR product director assigns package IDs.
      'packageId' => {
        'type'=>'id',
        'path'=>'ImplementationGuide.packageId',
        'min'=>1,
        'max'=>1
      },
      ##
      # SPDX license code for this IG (or not-open-source)
      # The license that applies to this Implementation Guide, using an SPDX license code, or 'not-open-source'.
      'license' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/spdx-license'=>[ 'not-open-source', '0BSD', 'AAL', 'Abstyles', 'Adobe-2006', 'Adobe-Glyph', 'ADSL', 'AFL-1.1', 'AFL-1.2', 'AFL-2.0', 'AFL-2.1', 'AFL-3.0', 'Afmparse', 'AGPL-1.0-only', 'AGPL-1.0-or-later', 'AGPL-3.0-only', 'AGPL-3.0-or-later', 'Aladdin', 'AMDPLPA', 'AML', 'AMPAS', 'ANTLR-PD', 'Apache-1.0', 'Apache-1.1', 'Apache-2.0', 'APAFML', 'APL-1.0', 'APSL-1.0', 'APSL-1.1', 'APSL-1.2', 'APSL-2.0', 'Artistic-1.0-cl8', 'Artistic-1.0-Perl', 'Artistic-1.0', 'Artistic-2.0', 'Bahyph', 'Barr', 'Beerware', 'BitTorrent-1.0', 'BitTorrent-1.1', 'Borceux', 'BSD-1-Clause', 'BSD-2-Clause-FreeBSD', 'BSD-2-Clause-NetBSD', 'BSD-2-Clause-Patent', 'BSD-2-Clause', 'BSD-3-Clause-Attribution', 'BSD-3-Clause-Clear', 'BSD-3-Clause-LBNL', 'BSD-3-Clause-No-Nuclear-License-2014', 'BSD-3-Clause-No-Nuclear-License', 'BSD-3-Clause-No-Nuclear-Warranty', 'BSD-3-Clause', 'BSD-4-Clause-UC', 'BSD-4-Clause', 'BSD-Protection', 'BSD-Source-Code', 'BSL-1.0', 'bzip2-1.0.5', 'bzip2-1.0.6', 'Caldera', 'CATOSL-1.1', 'CC-BY-1.0', 'CC-BY-2.0', 'CC-BY-2.5', 'CC-BY-3.0', 'CC-BY-4.0', 'CC-BY-NC-1.0', 'CC-BY-NC-2.0', 'CC-BY-NC-2.5', 'CC-BY-NC-3.0', 'CC-BY-NC-4.0', 'CC-BY-NC-ND-1.0', 'CC-BY-NC-ND-2.0', 'CC-BY-NC-ND-2.5', 'CC-BY-NC-ND-3.0', 'CC-BY-NC-ND-4.0', 'CC-BY-NC-SA-1.0', 'CC-BY-NC-SA-2.0', 'CC-BY-NC-SA-2.5', 'CC-BY-NC-SA-3.0', 'CC-BY-NC-SA-4.0', 'CC-BY-ND-1.0', 'CC-BY-ND-2.0', 'CC-BY-ND-2.5', 'CC-BY-ND-3.0', 'CC-BY-ND-4.0', 'CC-BY-SA-1.0', 'CC-BY-SA-2.0', 'CC-BY-SA-2.5', 'CC-BY-SA-3.0', 'CC-BY-SA-4.0', 'CC0-1.0', 'CDDL-1.0', 'CDDL-1.1', 'CDLA-Permissive-1.0', 'CDLA-Sharing-1.0', 'CECILL-1.0', 'CECILL-1.1', 'CECILL-2.0', 'CECILL-2.1', 'CECILL-B', 'CECILL-C', 'ClArtistic', 'CNRI-Jython', 'CNRI-Python-GPL-Compatible', 'CNRI-Python', 'Condor-1.1', 'CPAL-1.0', 'CPL-1.0', 'CPOL-1.02', 'Crossword', 'CrystalStacker', 'CUA-OPL-1.0', 'Cube', 'curl', 'D-FSL-1.0', 'diffmark', 'DOC', 'Dotseqn', 'DSDP', 'dvipdfm', 'ECL-1.0', 'ECL-2.0', 'EFL-1.0', 'EFL-2.0', 'eGenix', 'Entessa', 'EPL-1.0', 'EPL-2.0', 'ErlPL-1.1', 'EUDatagrid', 'EUPL-1.0', 'EUPL-1.1', 'EUPL-1.2', 'Eurosym', 'Fair', 'Frameworx-1.0', 'FreeImage', 'FSFAP', 'FSFUL', 'FSFULLR', 'FTL', 'GFDL-1.1-only', 'GFDL-1.1-or-later', 'GFDL-1.2-only', 'GFDL-1.2-or-later', 'GFDL-1.3-only', 'GFDL-1.3-or-later', 'Giftware', 'GL2PS', 'Glide', 'Glulxe', 'gnuplot', 'GPL-1.0-only', 'GPL-1.0-or-later', 'GPL-2.0-only', 'GPL-2.0-or-later', 'GPL-3.0-only', 'GPL-3.0-or-later', 'gSOAP-1.3b', 'HaskellReport', 'HPND', 'IBM-pibs', 'ICU', 'IJG', 'ImageMagick', 'iMatix', 'Imlib2', 'Info-ZIP', 'Intel-ACPI', 'Intel', 'Interbase-1.0', 'IPA', 'IPL-1.0', 'ISC', 'JasPer-2.0', 'JSON', 'LAL-1.2', 'LAL-1.3', 'Latex2e', 'Leptonica', 'LGPL-2.0-only', 'LGPL-2.0-or-later', 'LGPL-2.1-only', 'LGPL-2.1-or-later', 'LGPL-3.0-only', 'LGPL-3.0-or-later', 'LGPLLR', 'Libpng', 'libtiff', 'LiLiQ-P-1.1', 'LiLiQ-R-1.1', 'LiLiQ-Rplus-1.1', 'Linux-OpenIB', 'LPL-1.0', 'LPL-1.02', 'LPPL-1.0', 'LPPL-1.1', 'LPPL-1.2', 'LPPL-1.3a', 'LPPL-1.3c', 'MakeIndex', 'MirOS', 'MIT-0', 'MIT-advertising', 'MIT-CMU', 'MIT-enna', 'MIT-feh', 'MIT', 'MITNFA', 'Motosoto', 'mpich2', 'MPL-1.0', 'MPL-1.1', 'MPL-2.0-no-copyleft-exception', 'MPL-2.0', 'MS-PL', 'MS-RL', 'MTLL', 'Multics', 'Mup', 'NASA-1.3', 'Naumen', 'NBPL-1.0', 'NCSA', 'Net-SNMP', 'NetCDF', 'Newsletr', 'NGPL', 'NLOD-1.0', 'NLPL', 'Nokia', 'NOSL', 'Noweb', 'NPL-1.0', 'NPL-1.1', 'NPOSL-3.0', 'NRL', 'NTP', 'OCCT-PL', 'OCLC-2.0', 'ODbL-1.0', 'OFL-1.0', 'OFL-1.1', 'OGTSL', 'OLDAP-1.1', 'OLDAP-1.2', 'OLDAP-1.3', 'OLDAP-1.4', 'OLDAP-2.0.1', 'OLDAP-2.0', 'OLDAP-2.1', 'OLDAP-2.2.1', 'OLDAP-2.2.2', 'OLDAP-2.2', 'OLDAP-2.3', 'OLDAP-2.4', 'OLDAP-2.5', 'OLDAP-2.6', 'OLDAP-2.7', 'OLDAP-2.8', 'OML', 'OpenSSL', 'OPL-1.0', 'OSET-PL-2.1', 'OSL-1.0', 'OSL-1.1', 'OSL-2.0', 'OSL-2.1', 'OSL-3.0', 'PDDL-1.0', 'PHP-3.0', 'PHP-3.01', 'Plexus', 'PostgreSQL', 'psfrag', 'psutils', 'Python-2.0', 'Qhull', 'QPL-1.0', 'Rdisc', 'RHeCos-1.1', 'RPL-1.1', 'RPL-1.5', 'RPSL-1.0', 'RSA-MD', 'RSCPL', 'Ruby', 'SAX-PD', 'Saxpath', 'SCEA', 'Sendmail', 'SGI-B-1.0', 'SGI-B-1.1', 'SGI-B-2.0', 'SimPL-2.0', 'SISSL-1.2', 'SISSL', 'Sleepycat', 'SMLNJ', 'SMPPL', 'SNIA', 'Spencer-86', 'Spencer-94', 'Spencer-99', 'SPL-1.0', 'SugarCRM-1.1.3', 'SWL', 'TCL', 'TCP-wrappers', 'TMate', 'TORQUE-1.1', 'TOSL', 'Unicode-DFS-2015', 'Unicode-DFS-2016', 'Unicode-TOU', 'Unlicense', 'UPL-1.0', 'Vim', 'VOSTROM', 'VSL-1.0', 'W3C-19980720', 'W3C-20150513', 'W3C', 'Watcom-1.0', 'Wsuipa', 'WTFPL', 'X11', 'Xerox', 'XFree86-1.1', 'xinetd', 'Xnet', 'xpp', 'XSkat', 'YPL-1.0', 'YPL-1.1', 'Zed', 'Zend-2.0', 'Zimbra-1.3', 'Zimbra-1.4', 'zlib-acknowledgement', 'Zlib', 'ZPL-1.1', 'ZPL-2.0', 'ZPL-2.1' ]
        },
        'type'=>'code',
        'path'=>'ImplementationGuide.license',
        'min'=>0,
        'max'=>1,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/spdx-license'}
      },
      ##
      # FHIR Version(s) this Implementation Guide targets
      # The version(s) of the FHIR specification that this ImplementationGuide targets - e.g. describes how to use. The value of this element is the formal version of the specification, without the revision number, e.g. [publication].[major].[minor], which is 4.0.1. for this version.
      # Most implementation guides target a single version - e.g. they describe how to use a particular version, and the profiles and examples etc are valid for that version. But some implementation guides describe how to use multiple different versions of FHIR to solve the same problem, or in concert with each other. Typically, the requirement to support multiple versions arises as implementation matures and different implementation communities are stuck at different versions by regulation or market dynamics.
      'fhirVersion' => {
        'valid_codes'=>{
          'http://hl7.org/fhir/FHIR-version'=>[ '0.01', '0.05', '0.06', '0.11', '0.0.80', '0.0.81', '0.0.82', '0.4.0', '0.5.0', '1.0.0', '1.0.1', '1.0.2', '1.1.0', '1.4.0', '1.6.0', '1.8.0', '3.0.0', '3.0.1', '3.3.0', '3.5.0', '4.0.0', '4.0.1' ]
        },
        'type'=>'code',
        'path'=>'ImplementationGuide.fhirVersion',
        'min'=>1,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/FHIR-version'}
      },
      ##
      # Another Implementation guide this depends on
      # Another implementation guide that this implementation depends on. Typically, an implementation guide uses value sets, profiles etc.defined in other implementation guides.
      'dependsOn' => {
        'type'=>'ImplementationGuide::DependsOn',
        'path'=>'ImplementationGuide.dependsOn',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Profiles that apply globally
      # A set of profiles that all resources covered by this implementation guide must conform to.
      # See [Default Profiles](implementationguide.html#default) for a discussion of which resources are 'covered' by an implementation guide.
      'global' => {
        'type'=>'ImplementationGuide::Global',
        'path'=>'ImplementationGuide.global',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Information needed to build the IG
      # The information needed by an IG publisher tool to publish the whole implementation guide.
      # Principally, this consists of information abuot source resource and file locations, and build parameters and templates.
      'definition' => {
        'type'=>'ImplementationGuide::Definition',
        'path'=>'ImplementationGuide.definition',
        'min'=>0,
        'max'=>1
      },
      ##
      # Information about an assembled IG
      # Information about an assembled implementation guide, created by the publication tooling.
      'manifest' => {
        'type'=>'ImplementationGuide::Manifest',
        'path'=>'ImplementationGuide.manifest',
        'min'=>0,
        'max'=>1
      }
    }

    ##
    # Another Implementation guide this depends on
    # Another implementation guide that this implementation depends on. Typically, an implementation guide uses value sets, profiles etc.defined in other implementation guides.
    class DependsOn < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'DependsOn.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'DependsOn.extension',
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
          'path'=>'DependsOn.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Identity of the IG that this depends on
        # A canonical reference to the Implementation guide for the dependency.
        # Usually, A canonical reference to the implementation guide is the same as the master location at which the implementation guide is published.
        'uri' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/ImplementationGuide'],
          'type'=>'canonical',
          'path'=>'DependsOn.uri',
          'min'=>1,
          'max'=>1
        },
        ##
        # NPM Package name for IG this depends on
        # The NPM package name for the Implementation Guide that this IG depends on.
        'packageId' => {
          'type'=>'id',
          'path'=>'DependsOn.packageId',
          'min'=>0,
          'max'=>1
        },
        ##
        # Version of the IG
        # The version of the IG that is depended on, when the correct version is required to understand the IG correctly.
        # This follows the syntax of the NPM packaging version field - see [[reference]].
        'version' => {
          'type'=>'string',
          'path'=>'DependsOn.version',
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # Identity of the IG that this depends on
      # A canonical reference to the Implementation guide for the dependency.
      # Usually, A canonical reference to the implementation guide is the same as the master location at which the implementation guide is published.
      attr_accessor :uri                            # 1-1 canonical(http://hl7.org/fhir/StructureDefinition/ImplementationGuide)
      ##
      # NPM Package name for IG this depends on
      # The NPM package name for the Implementation Guide that this IG depends on.
      attr_accessor :packageId                      # 0-1 id
      ##
      # Version of the IG
      # The version of the IG that is depended on, when the correct version is required to understand the IG correctly.
      # This follows the syntax of the NPM packaging version field - see [[reference]].
      attr_accessor :version                        # 0-1 string
    end

    ##
    # Profiles that apply globally
    # A set of profiles that all resources covered by this implementation guide must conform to.
    # See [Default Profiles](implementationguide.html#default) for a discussion of which resources are 'covered' by an implementation guide.
    class Global < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Global.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Global.extension',
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
          'path'=>'Global.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Type this profile applies to
        # The type of resource that all instances must conform to.
        # The type must match that of the profile that is referred to but is made explicit here as a denormalization so that a system processing the implementation guide resource knows which resources the profile applies to even if the profile itself is not available.
        'type' => {
          'valid_codes'=>{
            'http://hl7.org/fhir/resource-types'=>[ 'Account', 'ActivityDefinition', 'AdverseEvent', 'AllergyIntolerance', 'Appointment', 'AppointmentResponse', 'AuditEvent', 'Basic', 'Binary', 'BiologicallyDerivedProduct', 'BodyStructure', 'Bundle', 'CapabilityStatement', 'CarePlan', 'CareTeam', 'CatalogEntry', 'ChargeItem', 'ChargeItemDefinition', 'Claim', 'ClaimResponse', 'ClinicalImpression', 'CodeSystem', 'Communication', 'CommunicationRequest', 'CompartmentDefinition', 'Composition', 'ConceptMap', 'Condition', 'Consent', 'Contract', 'Coverage', 'CoverageEligibilityRequest', 'CoverageEligibilityResponse', 'DetectedIssue', 'Device', 'DeviceDefinition', 'DeviceMetric', 'DeviceRequest', 'DeviceUseStatement', 'DiagnosticReport', 'DocumentManifest', 'DocumentReference', 'DomainResource', 'EffectEvidenceSynthesis', 'Encounter', 'Endpoint', 'EnrollmentRequest', 'EnrollmentResponse', 'EpisodeOfCare', 'EventDefinition', 'Evidence', 'EvidenceVariable', 'ExampleScenario', 'ExplanationOfBenefit', 'FamilyMemberHistory', 'Flag', 'Goal', 'GraphDefinition', 'Group', 'GuidanceResponse', 'HealthcareService', 'ImagingStudy', 'Immunization', 'ImmunizationEvaluation', 'ImmunizationRecommendation', 'ImplementationGuide', 'InsurancePlan', 'Invoice', 'Library', 'Linkage', 'List', 'Location', 'Measure', 'MeasureReport', 'Media', 'Medication', 'MedicationAdministration', 'MedicationDispense', 'MedicationKnowledge', 'MedicationRequest', 'MedicationStatement', 'MedicinalProduct', 'MedicinalProductAuthorization', 'MedicinalProductContraindication', 'MedicinalProductIndication', 'MedicinalProductIngredient', 'MedicinalProductInteraction', 'MedicinalProductManufactured', 'MedicinalProductPackaged', 'MedicinalProductPharmaceutical', 'MedicinalProductUndesirableEffect', 'MessageDefinition', 'MessageHeader', 'MolecularSequence', 'NamingSystem', 'NutritionOrder', 'Observation', 'ObservationDefinition', 'OperationDefinition', 'OperationOutcome', 'Organization', 'OrganizationAffiliation', 'Parameters', 'Patient', 'PaymentNotice', 'PaymentReconciliation', 'Person', 'PlanDefinition', 'Practitioner', 'PractitionerRole', 'Procedure', 'Provenance', 'Questionnaire', 'QuestionnaireResponse', 'RelatedPerson', 'RequestGroup', 'ResearchDefinition', 'ResearchElementDefinition', 'ResearchStudy', 'ResearchSubject', 'Resource', 'RiskAssessment', 'RiskEvidenceSynthesis', 'Schedule', 'SearchParameter', 'ServiceRequest', 'Slot', 'Specimen', 'SpecimenDefinition', 'StructureDefinition', 'StructureMap', 'Subscription', 'Substance', 'SubstanceNucleicAcid', 'SubstancePolymer', 'SubstanceProtein', 'SubstanceReferenceInformation', 'SubstanceSourceMaterial', 'SubstanceSpecification', 'SupplyDelivery', 'SupplyRequest', 'Task', 'TerminologyCapabilities', 'TestReport', 'TestScript', 'ValueSet', 'VerificationResult', 'VisionPrescription' ]
          },
          'type'=>'code',
          'path'=>'Global.type',
          'min'=>1,
          'max'=>1,
          'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/resource-types'}
        },
        ##
        # Profile that all resources must conform to
        # A reference to the profile that all instances must conform to.
        'profile' => {
          'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
          'type'=>'canonical',
          'path'=>'Global.profile',
          'min'=>1,
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # Type this profile applies to
      # The type of resource that all instances must conform to.
      # The type must match that of the profile that is referred to but is made explicit here as a denormalization so that a system processing the implementation guide resource knows which resources the profile applies to even if the profile itself is not available.
      attr_accessor :type                           # 1-1 code
      ##
      # Profile that all resources must conform to
      # A reference to the profile that all instances must conform to.
      attr_accessor :profile                        # 1-1 canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition)
    end

    ##
    # Information needed to build the IG
    # The information needed by an IG publisher tool to publish the whole implementation guide.
    # Principally, this consists of information abuot source resource and file locations, and build parameters and templates.
    class Definition < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Definition.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Definition.extension',
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
          'path'=>'Definition.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Grouping used to present related resources in the IG
        # A logical group of resources. Logical groups can be used when building pages.
        # Groupings are arbitrary sub-divisions of content. Typically, they are used to help build Table of Contents automatically.
        'grouping' => {
          'type'=>'ImplementationGuide::Definition::Grouping',
          'path'=>'Definition.grouping',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Resource in the implementation guide
        # A resource that is part of the implementation guide. Conformance resources (value set, structure definition, capability statements etc.) are obvious candidates for inclusion, but any kind of resource can be included as an example resource.
        'resource' => {
          'type'=>'ImplementationGuide::Definition::Resource',
          'path'=>'Definition.resource',
          'min'=>1,
          'max'=>Float::INFINITY
        },
        ##
        # Page/Section in the Guide
        # A page / section in the implementation guide. The root page is the implementation guide home page.
        # Pages automatically become sections if they have sub-pages. By convention, the home page is called index.html.
        'page' => {
          'type'=>'ImplementationGuide::Definition::Page',
          'path'=>'Definition.page',
          'min'=>0,
          'max'=>1
        },
        ##
        # Defines how IG is built by tools.
        'parameter' => {
          'type'=>'ImplementationGuide::Definition::Parameter',
          'path'=>'Definition.parameter',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # A template for building resources.
        'template' => {
          'type'=>'ImplementationGuide::Definition::Template',
          'path'=>'Definition.template',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Grouping used to present related resources in the IG
      # A logical group of resources. Logical groups can be used when building pages.
      # Groupings are arbitrary sub-divisions of content. Typically, they are used to help build Table of Contents automatically.
      class Grouping < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Grouping.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Grouping.extension',
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
            'path'=>'Grouping.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Descriptive name for the package
          # The human-readable title to display for the package of resources when rendering the implementation guide.
          'name' => {
            'type'=>'string',
            'path'=>'Grouping.name',
            'min'=>1,
            'max'=>1
          },
          ##
          # Human readable text describing the package.
          'description' => {
            'type'=>'string',
            'path'=>'Grouping.description',
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
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :modifierExtension              # 0-* [ Extension ]
        ##
        # Descriptive name for the package
        # The human-readable title to display for the package of resources when rendering the implementation guide.
        attr_accessor :name                           # 1-1 string
        ##
        # Human readable text describing the package.
        attr_accessor :description                    # 0-1 string
      end

      ##
      # Resource in the implementation guide
      # A resource that is part of the implementation guide. Conformance resources (value set, structure definition, capability statements etc.) are obvious candidates for inclusion, but any kind of resource can be included as an example resource.
      class Resource < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'example[x]' => ['boolean', 'canonical']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Resource.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Resource.extension',
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
            'path'=>'Resource.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Location of the resource
          # Where this resource is found.
          # Usually this is a relative URL that locates the resource within the implementation guide. If you authoring an implementation guide, and will publish it using the FHIR publication tooling, use a URI that may point to a resource, or to one of various alternative representations (e.g. spreadsheet). The tooling will convert this when it publishes it.
          'reference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
            'type'=>'Reference',
            'path'=>'Resource.reference',
            'min'=>1,
            'max'=>1
          },
          ##
          # Versions this applies to (if different to IG)
          # Indicates the FHIR Version(s) this artifact is intended to apply to. If no versions are specified, the resource is assumed to apply to all the versions stated in ImplementationGuide.fhirVersion.
          # The resource SHALL be valid against all the versions it is specified to apply to. If the resource referred to is a StructureDefinition, the fhirVersion stated in the StructureDefinition cannot disagree with the version specified here; the specified versions SHALL include the version specified by the StructureDefinition, and may include additional versions using the [applicable-version](extension-structuredefinition-applicable-version.html) extension.
          'fhirVersion' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/FHIR-version'=>[ '0.01', '0.05', '0.06', '0.11', '0.0.80', '0.0.81', '0.0.82', '0.4.0', '0.5.0', '1.0.0', '1.0.1', '1.0.2', '1.1.0', '1.4.0', '1.6.0', '1.8.0', '3.0.0', '3.0.1', '3.3.0', '3.5.0', '4.0.0', '4.0.1' ]
            },
            'type'=>'code',
            'path'=>'Resource.fhirVersion',
            'min'=>0,
            'max'=>Float::INFINITY,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/FHIR-version'}
          },
          ##
          # Human Name for the resource
          # A human assigned name for the resource. All resources SHOULD have a name, but the name may be extracted from the resource (e.g. ValueSet.name).
          'name' => {
            'type'=>'string',
            'path'=>'Resource.name',
            'min'=>0,
            'max'=>1
          },
          ##
          # Reason why included in guide
          # A description of the reason that a resource has been included in the implementation guide.
          # This is mostly used with examples to explain why it is present (though they can have extensive comments in the examples).
          'description' => {
            'type'=>'string',
            'path'=>'Resource.description',
            'min'=>0,
            'max'=>1
          },
          ##
          # Is an example/What is this an example of?
          # If true or a reference, indicates the resource is an example instance.  If a reference is present, indicates that the example is an example of the specified profile.
          # Examples: 
          # 
          # * StructureDefinition -> Any 
          # * ValueSet -> expansion 
          # * OperationDefinition -> Parameters 
          # * Questionnaire -> QuestionnaireResponse.
          'exampleBoolean' => {
            'type'=>'Boolean',
            'path'=>'Resource.example[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Is an example/What is this an example of?
          # If true or a reference, indicates the resource is an example instance.  If a reference is present, indicates that the example is an example of the specified profile.
          # Examples: 
          # 
          # * StructureDefinition -> Any 
          # * ValueSet -> expansion 
          # * OperationDefinition -> Parameters 
          # * Questionnaire -> QuestionnaireResponse.
          'exampleCanonical' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
            'type'=>'Canonical',
            'path'=>'Resource.example[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Grouping this is part of
          # Reference to the id of the grouping this resource appears in.
          # This must correspond to a package.id element within this implementation guide.
          'groupingId' => {
            'type'=>'id',
            'path'=>'Resource.groupingId',
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
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :modifierExtension              # 0-* [ Extension ]
        ##
        # Location of the resource
        # Where this resource is found.
        # Usually this is a relative URL that locates the resource within the implementation guide. If you authoring an implementation guide, and will publish it using the FHIR publication tooling, use a URI that may point to a resource, or to one of various alternative representations (e.g. spreadsheet). The tooling will convert this when it publishes it.
        attr_accessor :reference                      # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
        ##
        # Versions this applies to (if different to IG)
        # Indicates the FHIR Version(s) this artifact is intended to apply to. If no versions are specified, the resource is assumed to apply to all the versions stated in ImplementationGuide.fhirVersion.
        # The resource SHALL be valid against all the versions it is specified to apply to. If the resource referred to is a StructureDefinition, the fhirVersion stated in the StructureDefinition cannot disagree with the version specified here; the specified versions SHALL include the version specified by the StructureDefinition, and may include additional versions using the [applicable-version](extension-structuredefinition-applicable-version.html) extension.
        attr_accessor :fhirVersion                    # 0-* [ code ]
        ##
        # Human Name for the resource
        # A human assigned name for the resource. All resources SHOULD have a name, but the name may be extracted from the resource (e.g. ValueSet.name).
        attr_accessor :name                           # 0-1 string
        ##
        # Reason why included in guide
        # A description of the reason that a resource has been included in the implementation guide.
        # This is mostly used with examples to explain why it is present (though they can have extensive comments in the examples).
        attr_accessor :description                    # 0-1 string
        ##
        # Is an example/What is this an example of?
        # If true or a reference, indicates the resource is an example instance.  If a reference is present, indicates that the example is an example of the specified profile.
        # Examples: 
        # 
        # * StructureDefinition -> Any 
        # * ValueSet -> expansion 
        # * OperationDefinition -> Parameters 
        # * Questionnaire -> QuestionnaireResponse.
        attr_accessor :exampleBoolean                 # 0-1 Boolean
        ##
        # Is an example/What is this an example of?
        # If true or a reference, indicates the resource is an example instance.  If a reference is present, indicates that the example is an example of the specified profile.
        # Examples: 
        # 
        # * StructureDefinition -> Any 
        # * ValueSet -> expansion 
        # * OperationDefinition -> Parameters 
        # * Questionnaire -> QuestionnaireResponse.
        attr_accessor :exampleCanonical               # 0-1 Canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition)
        ##
        # Grouping this is part of
        # Reference to the id of the grouping this resource appears in.
        # This must correspond to a package.id element within this implementation guide.
        attr_accessor :groupingId                     # 0-1 id
      end

      ##
      # Page/Section in the Guide
      # A page / section in the implementation guide. The root page is the implementation guide home page.
      # Pages automatically become sections if they have sub-pages. By convention, the home page is called index.html.
      class Page < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'name[x]' => ['Reference', 'url']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Page.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Page.extension',
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
            'path'=>'Page.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Where to find that page
          # The source address for the page.
          # The publishing tool will autogenerate source for list (source = n/a) and inject included implementations for include (source = uri of guide to include).
          'nameReference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Binary'],
            'type'=>'Reference',
            'path'=>'Page.name[x]',
            'min'=>1,
            'max'=>1
          },
          ##
          # Where to find that page
          # The source address for the page.
          # The publishing tool will autogenerate source for list (source = n/a) and inject included implementations for include (source = uri of guide to include).
          'nameUrl' => {
            'type'=>'Url',
            'path'=>'Page.name[x]',
            'min'=>1,
            'max'=>1
          },
          ##
          # Short title shown for navigational assistance
          # A short title used to represent this page in navigational structures such as table of contents, bread crumbs, etc.
          'title' => {
            'type'=>'string',
            'path'=>'Page.title',
            'min'=>1,
            'max'=>1
          },
          ##
          # html | markdown | xml | generated
          # A code that indicates how the page is generated.
          'generation' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/guide-page-generation'=>[ 'html', 'markdown', 'xml', 'generated' ]
            },
            'type'=>'code',
            'path'=>'Page.generation',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/guide-page-generation'}
          },
          ##
          # Nested Pages / Sections
          # Nested Pages/Sections under this page.
          # The implementation guide breadcrumbs are generated from this structure.
          'page' => {
            'type'=>'ImplementationGuide::Definition::Page',
            'path'=>'Page.page',
            'min'=>0,
            'max'=>Float::INFINITY
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
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :modifierExtension              # 0-* [ Extension ]
        ##
        # Where to find that page
        # The source address for the page.
        # The publishing tool will autogenerate source for list (source = n/a) and inject included implementations for include (source = uri of guide to include).
        attr_accessor :nameReference                  # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Binary)
        ##
        # Where to find that page
        # The source address for the page.
        # The publishing tool will autogenerate source for list (source = n/a) and inject included implementations for include (source = uri of guide to include).
        attr_accessor :nameUrl                        # 1-1 Url
        ##
        # Short title shown for navigational assistance
        # A short title used to represent this page in navigational structures such as table of contents, bread crumbs, etc.
        attr_accessor :title                          # 1-1 string
        ##
        # html | markdown | xml | generated
        # A code that indicates how the page is generated.
        attr_accessor :generation                     # 1-1 code
        ##
        # Nested Pages / Sections
        # Nested Pages/Sections under this page.
        # The implementation guide breadcrumbs are generated from this structure.
        attr_accessor :page                           # 0-* [ ImplementationGuide::Definition::Page ]
      end

      ##
      # Defines how IG is built by tools.
      class Parameter < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Parameter.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Parameter.extension',
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
            'path'=>'Parameter.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # apply | path-resource | path-pages | path-tx-cache | expansion-parameter | rule-broken-links | generate-xml | generate-json | generate-turtle | html-template.
          'code' => {
            'valid_codes'=>{
              'http://hl7.org/fhir/guide-parameter-code'=>[ 'apply', 'path-resource', 'path-pages', 'path-tx-cache', 'expansion-parameter', 'rule-broken-links', 'generate-xml', 'generate-json', 'generate-turtle', 'html-template' ]
            },
            'type'=>'code',
            'path'=>'Parameter.code',
            'min'=>1,
            'max'=>1,
            'binding'=>{'strength'=>'required', 'uri'=>'http://hl7.org/fhir/ValueSet/guide-parameter-code'}
          },
          ##
          # Value for named type.
          'value' => {
            'type'=>'string',
            'path'=>'Parameter.value',
            'min'=>1,
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
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :modifierExtension              # 0-* [ Extension ]
        ##
        # apply | path-resource | path-pages | path-tx-cache | expansion-parameter | rule-broken-links | generate-xml | generate-json | generate-turtle | html-template.
        attr_accessor :code                           # 1-1 code
        ##
        # Value for named type.
        attr_accessor :value                          # 1-1 string
      end

      ##
      # A template for building resources.
      class Template < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Template.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Template.extension',
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
            'path'=>'Template.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Type of template specified.
          'code' => {
            'type'=>'code',
            'path'=>'Template.code',
            'min'=>1,
            'max'=>1
          },
          ##
          # The source location for the template.
          'source' => {
            'type'=>'string',
            'path'=>'Template.source',
            'min'=>1,
            'max'=>1
          },
          ##
          # The scope in which the template applies.
          'scope' => {
            'type'=>'string',
            'path'=>'Template.scope',
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
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :modifierExtension              # 0-* [ Extension ]
        ##
        # Type of template specified.
        attr_accessor :code                           # 1-1 code
        ##
        # The source location for the template.
        attr_accessor :source                         # 1-1 string
        ##
        # The scope in which the template applies.
        attr_accessor :scope                          # 0-1 string
      end
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # Grouping used to present related resources in the IG
      # A logical group of resources. Logical groups can be used when building pages.
      # Groupings are arbitrary sub-divisions of content. Typically, they are used to help build Table of Contents automatically.
      attr_accessor :grouping                       # 0-* [ ImplementationGuide::Definition::Grouping ]
      ##
      # Resource in the implementation guide
      # A resource that is part of the implementation guide. Conformance resources (value set, structure definition, capability statements etc.) are obvious candidates for inclusion, but any kind of resource can be included as an example resource.
      attr_accessor :resource                       # 1-* [ ImplementationGuide::Definition::Resource ]
      ##
      # Page/Section in the Guide
      # A page / section in the implementation guide. The root page is the implementation guide home page.
      # Pages automatically become sections if they have sub-pages. By convention, the home page is called index.html.
      attr_accessor :page                           # 0-1 ImplementationGuide::Definition::Page
      ##
      # Defines how IG is built by tools.
      attr_accessor :parameter                      # 0-* [ ImplementationGuide::Definition::Parameter ]
      ##
      # A template for building resources.
      attr_accessor :template                       # 0-* [ ImplementationGuide::Definition::Template ]
    end

    ##
    # Information about an assembled IG
    # Information about an assembled implementation guide, created by the publication tooling.
    class Manifest < FHIR::Model
      include FHIR::Hashable
      include FHIR::Json
      include FHIR::Xml

      METADATA = {
        ##
        # Unique id for inter-element referencing
        # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
        'id' => {
          'type'=>'string',
          'path'=>'Manifest.id',
          'min'=>0,
          'max'=>1
        },
        ##
        # Additional content defined by implementations
        # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        'extension' => {
          'type'=>'Extension',
          'path'=>'Manifest.extension',
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
          'path'=>'Manifest.modifierExtension',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Location of rendered implementation guide
        # A pointer to official web page, PDF or other rendering of the implementation guide.
        'rendering' => {
          'type'=>'url',
          'path'=>'Manifest.rendering',
          'min'=>0,
          'max'=>1
        },
        ##
        # Resource in the implementation guide
        # A resource that is part of the implementation guide. Conformance resources (value set, structure definition, capability statements etc.) are obvious candidates for inclusion, but any kind of resource can be included as an example resource.
        'resource' => {
          'type'=>'ImplementationGuide::Manifest::Resource',
          'path'=>'Manifest.resource',
          'min'=>1,
          'max'=>Float::INFINITY
        },
        ##
        # HTML page within the parent IG
        # Information about a page within the IG.
        'page' => {
          'type'=>'ImplementationGuide::Manifest::Page',
          'path'=>'Manifest.page',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Image within the IG
        # Indicates a relative path to an image that exists within the IG.
        'image' => {
          'type'=>'string',
          'path'=>'Manifest.image',
          'min'=>0,
          'max'=>Float::INFINITY
        },
        ##
        # Additional linkable file in IG
        # Indicates the relative path of an additional non-page, non-image file that is part of the IG - e.g. zip, jar and similar files that could be the target of a hyperlink in a derived IG.
        'other' => {
          'type'=>'string',
          'path'=>'Manifest.other',
          'min'=>0,
          'max'=>Float::INFINITY
        }
      }

      ##
      # Resource in the implementation guide
      # A resource that is part of the implementation guide. Conformance resources (value set, structure definition, capability statements etc.) are obvious candidates for inclusion, but any kind of resource can be included as an example resource.
      class Resource < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        MULTIPLE_TYPES = {
          'example[x]' => ['boolean', 'canonical']
        }
        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Resource.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Resource.extension',
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
            'path'=>'Resource.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # Location of the resource
          # Where this resource is found.
          # Usually this is a relative URL that locates the resource within the implementation guide. If you authoring an implementation guide, and will publish it using the FHIR publication tooling, use a URI that may point to a resource, or to one of various alternative representations (e.g. spreadsheet). The tooling will convert this when it publishes it.
          'reference' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/Resource'],
            'type'=>'Reference',
            'path'=>'Resource.reference',
            'min'=>1,
            'max'=>1
          },
          ##
          # Is an example/What is this an example of?
          # If true or a reference, indicates the resource is an example instance.  If a reference is present, indicates that the example is an example of the specified profile.
          # Typically, conformance resources and knowledge resources are directly part of the implementation guide, with their normal meaning, and patient linked resources are usually examples. However this is not always true.
          'exampleBoolean' => {
            'type'=>'Boolean',
            'path'=>'Resource.example[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Is an example/What is this an example of?
          # If true or a reference, indicates the resource is an example instance.  If a reference is present, indicates that the example is an example of the specified profile.
          # Typically, conformance resources and knowledge resources are directly part of the implementation guide, with their normal meaning, and patient linked resources are usually examples. However this is not always true.
          'exampleCanonical' => {
            'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
            'type'=>'Canonical',
            'path'=>'Resource.example[x]',
            'min'=>0,
            'max'=>1
          },
          ##
          # Relative path for page in IG
          # The relative path for primary page for this resource within the IG.
          # Appending 'rendering' + "/" + this should resolve to the resource page.
          'relativePath' => {
            'type'=>'url',
            'path'=>'Resource.relativePath',
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
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :modifierExtension              # 0-* [ Extension ]
        ##
        # Location of the resource
        # Where this resource is found.
        # Usually this is a relative URL that locates the resource within the implementation guide. If you authoring an implementation guide, and will publish it using the FHIR publication tooling, use a URI that may point to a resource, or to one of various alternative representations (e.g. spreadsheet). The tooling will convert this when it publishes it.
        attr_accessor :reference                      # 1-1 Reference(http://hl7.org/fhir/StructureDefinition/Resource)
        ##
        # Is an example/What is this an example of?
        # If true or a reference, indicates the resource is an example instance.  If a reference is present, indicates that the example is an example of the specified profile.
        # Typically, conformance resources and knowledge resources are directly part of the implementation guide, with their normal meaning, and patient linked resources are usually examples. However this is not always true.
        attr_accessor :exampleBoolean                 # 0-1 Boolean
        ##
        # Is an example/What is this an example of?
        # If true or a reference, indicates the resource is an example instance.  If a reference is present, indicates that the example is an example of the specified profile.
        # Typically, conformance resources and knowledge resources are directly part of the implementation guide, with their normal meaning, and patient linked resources are usually examples. However this is not always true.
        attr_accessor :exampleCanonical               # 0-1 Canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition)
        ##
        # Relative path for page in IG
        # The relative path for primary page for this resource within the IG.
        # Appending 'rendering' + "/" + this should resolve to the resource page.
        attr_accessor :relativePath                   # 0-1 url
      end

      ##
      # HTML page within the parent IG
      # Information about a page within the IG.
      class Page < FHIR::Model
        include FHIR::Hashable
        include FHIR::Json
        include FHIR::Xml

        METADATA = {
          ##
          # Unique id for inter-element referencing
          # Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.
          'id' => {
            'type'=>'string',
            'path'=>'Page.id',
            'min'=>0,
            'max'=>1
          },
          ##
          # Additional content defined by implementations
          # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
          # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
          'extension' => {
            'type'=>'Extension',
            'path'=>'Page.extension',
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
            'path'=>'Page.modifierExtension',
            'min'=>0,
            'max'=>Float::INFINITY
          },
          ##
          # HTML page name
          # Relative path to the page.
          # Appending 'rendering' + "/" + this should resolve to the page.
          'name' => {
            'type'=>'string',
            'path'=>'Page.name',
            'min'=>1,
            'max'=>1
          },
          ##
          # Title of the page, for references
          # Label for the page intended for human display.
          'title' => {
            'type'=>'string',
            'path'=>'Page.title',
            'min'=>0,
            'max'=>1
          },
          ##
          # Anchor available on the page
          # The name of an anchor available on the page.
          # Appending 'rendering' + "/" + page.name + "#" + page.anchor should resolve to the anchor.
          'anchor' => {
            'type'=>'string',
            'path'=>'Page.anchor',
            'min'=>0,
            'max'=>Float::INFINITY
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
        # Extensions that cannot be ignored even if unrecognized
        # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
        # 
        # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
        # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
        attr_accessor :modifierExtension              # 0-* [ Extension ]
        ##
        # HTML page name
        # Relative path to the page.
        # Appending 'rendering' + "/" + this should resolve to the page.
        attr_accessor :name                           # 1-1 string
        ##
        # Title of the page, for references
        # Label for the page intended for human display.
        attr_accessor :title                          # 0-1 string
        ##
        # Anchor available on the page
        # The name of an anchor available on the page.
        # Appending 'rendering' + "/" + page.name + "#" + page.anchor should resolve to the anchor.
        attr_accessor :anchor                         # 0-* [ string ]
      end
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
      # Extensions that cannot be ignored even if unrecognized
      # May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants. Usually modifier elements provide negation or qualification. To make the use of extensions safe and manageable, there is a strict set of governance applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension. Applications processing a resource are required to check for modifier extensions.
      # 
      # Modifier extensions SHALL NOT change the meaning of any elements on Resource or DomainResource (including cannot change the meaning of modifierExtension itself).
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      attr_accessor :modifierExtension              # 0-* [ Extension ]
      ##
      # Location of rendered implementation guide
      # A pointer to official web page, PDF or other rendering of the implementation guide.
      attr_accessor :rendering                      # 0-1 url
      ##
      # Resource in the implementation guide
      # A resource that is part of the implementation guide. Conformance resources (value set, structure definition, capability statements etc.) are obvious candidates for inclusion, but any kind of resource can be included as an example resource.
      attr_accessor :resource                       # 1-* [ ImplementationGuide::Manifest::Resource ]
      ##
      # HTML page within the parent IG
      # Information about a page within the IG.
      attr_accessor :page                           # 0-* [ ImplementationGuide::Manifest::Page ]
      ##
      # Image within the IG
      # Indicates a relative path to an image that exists within the IG.
      attr_accessor :image                          # 0-* [ string ]
      ##
      # Additional linkable file in IG
      # Indicates the relative path of an additional non-page, non-image file that is part of the IG - e.g. zip, jar and similar files that could be the target of a hyperlink in a derived IG.
      attr_accessor :other                          # 0-* [ string ]
    end
    ##
    # Logical id of this artifact
    # The logical id of the resource, as used in the URL for the resource. Once assigned, this value never changes.
    # The only time that a resource does not have an id is when it is being submitted to the server using a create operation.
    attr_accessor :id                             # 0-1 string
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
    # Canonical identifier for this implementation guide, represented as a URI (globally unique)
    # An absolute URI that is used to identify this implementation guide when it is referenced in a specification, model, design or an instance; also called its canonical identifier. This SHOULD be globally unique and SHOULD be a literal address at which at which an authoritative instance of this implementation guide is (or will be) published. This URL can be the target of a canonical reference. It SHALL remain the same when the implementation guide is stored on different servers.
    # Can be a urn:uuid: or a urn:oid: but real http: addresses are preferred.  Multiple instances may share the same URL if they have a distinct version.
    # 
    # The determination of when to create a new version of a resource (same url, new version) vs. defining a new artifact is up to the author.  Considerations for making this decision are found in [Technical and Business Versions](resource.html#versions). 
    # 
    # In some cases, the resource can no longer be found at the stated url, but the url itself cannot change. Implementations can use the [meta.source](resource.html#meta) element to indicate where the current master source of the resource can be found.
    attr_accessor :url                            # 1-1 uri
    ##
    # Business version of the implementation guide
    # The identifier that is used to identify this version of the implementation guide when it is referenced in a specification, model, design or instance. This is an arbitrary value managed by the implementation guide author and is not expected to be globally unique. For example, it might be a timestamp (e.g. yyyymmdd) if a managed version is not available. There is also no expectation that versions can be placed in a lexicographical sequence.
    # There may be different implementation guide instances that have the same identifier but different versions.  The version can be appended to the url in a reference to allow a reference to a particular business version of the implementation guide with the format [url]|[version].
    attr_accessor :version                        # 0-1 string
    ##
    # Name for this implementation guide (computer friendly)
    # A natural language name identifying the implementation guide. This name should be usable as an identifier for the module by machine processing applications such as code generation.
    # The name is not expected to be globally unique. The name should be a simple alphanumeric type name to ensure that it is machine-processing friendly.
    attr_accessor :name                           # 1-1 string
    ##
    # Name for this implementation guide (human friendly)
    # A short, descriptive, user-friendly title for the implementation guide.
    # This name does not need to be machine-processing friendly and may contain punctuation, white-space, etc.
    attr_accessor :title                          # 0-1 string
    ##
    # draft | active | retired | unknown
    # The status of this implementation guide. Enables tracking the life-cycle of the content.
    # Allows filtering of implementation guides that are appropriate for use versus not.
    attr_accessor :status                         # 1-1 code
    ##
    # For testing purposes, not real usage
    # A Boolean value to indicate that this implementation guide is authored for testing purposes (or education/evaluation/marketing) and is not intended to be used for genuine usage.
    # Allows filtering of implementation guides that are appropriate for use versus not.
    attr_accessor :experimental                   # 0-1 boolean
    ##
    # Date last changed
    # The date  (and optionally time) when the implementation guide was published. The date must change when the business version changes and it must change if the status code changes. In addition, it should change when the substantive content of the implementation guide changes.
    # Note that this is not the same as the resource last-modified-date, since the resource may be a secondary representation of the implementation guide. Additional specific dates may be added as extensions or be found by consulting Provenances associated with past versions of the resource.
    attr_accessor :date                           # 0-1 dateTime
    ##
    # Name of the publisher (organization or individual)
    # The name of the organization or individual that published the implementation guide.
    # Usually an organization but may be an individual. The publisher (or steward) of the implementation guide is the organization or individual primarily responsible for the maintenance and upkeep of the implementation guide. This is not necessarily the same individual or organization that developed and initially authored the content. The publisher is the primary point of contact for questions or issues with the implementation guide. This item SHOULD be populated unless the information is available from context.
    attr_accessor :publisher                      # 0-1 string
    ##
    # Contact details for the publisher
    # Contact details to assist a user in finding and communicating with the publisher.
    # May be a web site, an email address, a telephone number, etc.
    attr_accessor :contact                        # 0-* [ ContactDetail ]
    ##
    # Natural language description of the implementation guide
    # A free text natural language description of the implementation guide from a consumer's perspective.
    # This description can be used to capture details such as why the implementation guide was built, comments about misuse, instructions for clinical use and interpretation, literature references, examples from the paper world, etc. It is not a rendering of the implementation guide as conveyed in the 'text' field of the resource itself. This item SHOULD be populated unless the information is available from context (e.g. the language of the implementation guide is presumed to be the predominant language in the place the implementation guide was created).
    attr_accessor :description                    # 0-1 markdown
    ##
    # The context that the content is intended to support
    # The content was developed with a focus and intent of supporting the contexts that are listed. These contexts may be general categories (gender, age, ...) or may be references to specific programs (insurance plans, studies, ...) and may be used to assist with indexing and searching for appropriate implementation guide instances.
    # When multiple useContexts are specified, there is no expectation that all or any of the contexts apply.
    attr_accessor :useContext                     # 0-* [ UsageContext ]
    ##
    # Intended jurisdiction for implementation guide (if applicable)
    # A legal or geographic region in which the implementation guide is intended to be used.
    # It may be possible for the implementation guide to be used in jurisdictions other than those for which it was originally designed or intended.
    attr_accessor :jurisdiction                   # 0-* [ CodeableConcept ]
    ##
    # Use and/or publishing restrictions
    # A copyright statement relating to the implementation guide and/or its contents. Copyright statements are generally legal restrictions on the use and publishing of the implementation guide.
    attr_accessor :copyright                      # 0-1 markdown
    ##
    # NPM Package name for IG
    # The NPM package name for this Implementation Guide, used in the NPM package distribution, which is the primary mechanism by which FHIR based tooling manages IG dependencies. This value must be globally unique, and should be assigned with care.
    # Many (if not all) IG publishing tools will require that this element be present. For implementation guides published through HL7 or the FHIR foundation, the FHIR product director assigns package IDs.
    attr_accessor :packageId                      # 1-1 id
    ##
    # SPDX license code for this IG (or not-open-source)
    # The license that applies to this Implementation Guide, using an SPDX license code, or 'not-open-source'.
    attr_accessor :license                        # 0-1 code
    ##
    # FHIR Version(s) this Implementation Guide targets
    # The version(s) of the FHIR specification that this ImplementationGuide targets - e.g. describes how to use. The value of this element is the formal version of the specification, without the revision number, e.g. [publication].[major].[minor], which is 4.0.1. for this version.
    # Most implementation guides target a single version - e.g. they describe how to use a particular version, and the profiles and examples etc are valid for that version. But some implementation guides describe how to use multiple different versions of FHIR to solve the same problem, or in concert with each other. Typically, the requirement to support multiple versions arises as implementation matures and different implementation communities are stuck at different versions by regulation or market dynamics.
    attr_accessor :fhirVersion                    # 1-* [ code ]
    ##
    # Another Implementation guide this depends on
    # Another implementation guide that this implementation depends on. Typically, an implementation guide uses value sets, profiles etc.defined in other implementation guides.
    attr_accessor :dependsOn                      # 0-* [ ImplementationGuide::DependsOn ]
    ##
    # Profiles that apply globally
    # A set of profiles that all resources covered by this implementation guide must conform to.
    # See [Default Profiles](implementationguide.html#default) for a discussion of which resources are 'covered' by an implementation guide.
    attr_accessor :global                         # 0-* [ ImplementationGuide::Global ]
    ##
    # Information needed to build the IG
    # The information needed by an IG publisher tool to publish the whole implementation guide.
    # Principally, this consists of information abuot source resource and file locations, and build parameters and templates.
    attr_accessor :definition                     # 0-1 ImplementationGuide::Definition
    ##
    # Information about an assembled IG
    # Information about an assembled implementation guide, created by the publication tooling.
    attr_accessor :manifest                       # 0-1 ImplementationGuide::Manifest

    def resourceType
      'ImplementationGuide'
    end
  end
end
