module FHIR

  ##
  # Base StructureDefinition for Meta Type: The metadata about a resource. This is content in the resource that is maintained by the infrastructure. Changes to the content might not always be associated with version changes to the resource.
  class Meta < FHIR::Model
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
        'path'=>'Meta.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'Meta.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Version specific identifier
      # The version specific identifier, as it appears in the version portion of the URL. This value changes when the resource is created, updated, or deleted.
      # The server assigns this value, and ignores what the client specifies, except in the case that the server is imposing version integrity on updates/deletes.
      'versionId' => {
        'type'=>'id',
        'path'=>'Meta.versionId',
        'min'=>0,
        'max'=>1
      },
      ##
      # When the resource version last changed
      # When the resource last changed - e.g. when the version changed.
      # This value is always populated except when the resource is first being created. The server / resource manager sets this value; what a client provides is irrelevant. This is equivalent to the HTTP Last-Modified and SHOULD have the same value on a [read](http.html#read) interaction.
      'lastUpdated' => {
        'type'=>'instant',
        'path'=>'Meta.lastUpdated',
        'min'=>0,
        'max'=>1
      },
      ##
      # Identifies where the resource comes from
      # A uri that identifies the source system of the resource. This provides a minimal amount of [Provenance](provenance.html#) information that can be used to track or differentiate the source of information in the resource. The source may identify another FHIR server, document, message, database, etc.
      # In the provenance resource, this corresponds to Provenance.entity.what[x]. The exact use of the source (and the implied Provenance.entity.role) is left to implementer discretion. Only one nominated source is allowed; for additional provenance details, a full Provenance resource should be used. 
      # 
      # This element can be used to indicate where the current master source of a resource that has a canonical URL if the resource is no longer hosted at the canonical URL.
      'source' => {
        'type'=>'uri',
        'path'=>'Meta.source',
        'min'=>0,
        'max'=>1
      },
      ##
      # Profiles this resource claims to conform to
      # A list of profiles (references to [StructureDefinition](structuredefinition.html#) resources) that this resource claims to conform to. The URL is a reference to [StructureDefinition.url](structuredefinition-definitions.html#StructureDefinition.url).
      # It is up to the server and/or other infrastructure of policy to determine whether/how these claims are verified and/or updated over time.  The list of profile URLs is a set.
      'profile' => {
        'type_profiles'=>['http://hl7.org/fhir/StructureDefinition/StructureDefinition'],
        'type'=>'canonical',
        'path'=>'Meta.profile',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Security Labels applied to this resource
      # Security labels applied to this resource. These tags connect specific resources to the overall security policy and infrastructure.
      # The security labels can be updated without changing the stated version of the resource. The list of security labels is a set. Uniqueness is based the system/code, and version and display are ignored.
      'security' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/v3-ActCode'=>[ 'ETH', 'GDIS', 'HIV', 'MST', 'SCA', 'SDV', 'SEX', 'SPI', 'BH', 'COGN', 'DVD', 'EMOTDIS', 'MH', 'PSY', 'PSYTHPN', 'SUD', 'ETHUD', 'OPIOIDUD', 'STD', 'TBOO', 'VIO', 'SICKLE', 'DEMO', 'DOB', 'GENDER', 'LIVARG', 'MARST', 'RACE', 'REL', 'B', 'EMPL', 'LOCIS', 'SSP', 'ADOL', 'CEL', 'DIA', 'DRGIS', 'EMP', 'PDS', 'PHY', 'PRS', 'COMPT', 'ACOCOMPT', 'CTCOMPT', 'FMCOMPT', 'HRCOMPT', 'LRCOMPT', 'PACOMPT', 'RESCOMPT', 'RMGTCOMPT', 'SecurityPolicy', 'AUTHPOL', 'ACCESSCONSCHEME', 'DELEPOL', 'ObligationPolicy', 'ANONY', 'AOD', 'AUDIT', 'AUDTR', 'CPLYCC', 'CPLYCD', 'CPLYJPP', 'CPLYOPP', 'CPLYOSP', 'CPLYPOL', 'DECLASSIFYLABEL', 'DEID', 'DELAU', 'DOWNGRDLABEL', 'DRIVLABEL', 'ENCRYPT', 'ENCRYPTR', 'ENCRYPTT', 'ENCRYPTU', 'HUAPRV', 'LABEL', 'MASK', 'MINEC', 'PERSISTLABEL', 'PRIVMARK', 'PSEUD', 'REDACT', 'UPGRDLABEL', 'RefrainPolicy', 'NOAUTH', 'NOCOLLECT', 'NODSCLCD', 'NODSCLCDS', 'NOINTEGRATE', 'NOLIST', 'NOMOU', 'NOORGPOL', 'NOPAT', 'NOPERSISTP', 'NORDSCLCD', 'NORDSCLCDS', 'NORDSCLW', 'NORELINK', 'NOREUSE', 'NOVIP', 'ORCON' ],
          'http://terminology.hl7.org/CodeSystem/v3-ActReason'=>[ 'PurposeOfUse', 'HMARKT', 'HOPERAT', 'CAREMGT', 'DONAT', 'FRAUD', 'GOV', 'HACCRED', 'HCOMPL', 'HDECD', 'HDIRECT', 'HDM', 'HLEGAL', 'HOUTCOMS', 'HPRGRP', 'HQUALIMP', 'HSYSADMIN', 'LABELING', 'METAMGT', 'MEMADMIN', 'MILCDM', 'PATADMIN', 'PATSFTY', 'PERFMSR', 'RECORDMGT', 'SYSDEV', 'HTEST', 'TRAIN', 'HPAYMT', 'CLMATTCH', 'COVAUTH', 'COVERAGE', 'ELIGDTRM', 'ELIGVER', 'ENROLLM', 'MILDCRG', 'REMITADV', 'HRESCH', 'BIORCH', 'CLINTRCH', 'CLINTRCHNPC', 'CLINTRCHPC', 'PRECLINTRCH', 'DSRCH', 'POARCH', 'TRANSRCH', 'PATRQT', 'FAMRQT', 'PWATRNY', 'SUPNWK', 'PUBHLTH', 'DISASTER', 'THREAT', 'TREAT', 'CLINTRL', 'COC', 'ETREAT', 'BTG', 'ERTREAT', 'POPHLTH' ],
          'http://terminology.hl7.org/CodeSystem/v3-Confidentiality'=>[ 'U', 'L', 'M', 'N', 'R', 'V' ],
          'http://terminology.hl7.org/CodeSystem/v3-ObservationValue'=>[ 'ABSTRED', 'AGGRED', 'ANONYED', 'MAPPED', 'MASKED', 'PSEUDED', 'REDACTED', 'SUBSETTED', 'SYNTAC', 'TRSLT', 'VERSIONED', 'CRYTOHASH', 'DIGSIG', 'HRELIABLE', 'RELIABLE', 'UNCERTREL', 'UNRELIABLE', 'CLINAST', 'DEVAST', 'HCPAST', 'PACQAST', 'PATAST', 'PAYAST', 'PROAST', 'SDMAST', 'CLINRPT', 'DEVRPT', 'HCPRPT', 'PACQRPT', 'PATRPT', 'PAYRPT', 'PRORPT', 'SDMRPT' ]
        },
        'type'=>'Coding',
        'path'=>'Meta.security',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'extensible', 'uri'=>'http://hl7.org/fhir/ValueSet/security-labels'}
      },
      ##
      # Tags applied to this resource. Tags are intended to be used to identify and relate resources to process and workflow, and applications are not required to consider the tags when interpreting the meaning of a resource.
      # The tags can be updated without changing the stated version of the resource. The list of tags is a set. Uniqueness is based the system/code, and version and display are ignored.
      'tag' => {
        'valid_codes'=>{
          'http://terminology.hl7.org/CodeSystem/common-tags'=>[ 'actionable' ]
        },
        'type'=>'Coding',
        'path'=>'Meta.tag',
        'min'=>0,
        'max'=>Float::INFINITY,
        'binding'=>{'strength'=>'example', 'uri'=>'http://hl7.org/fhir/ValueSet/common-tags'}
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
    # Version specific identifier
    # The version specific identifier, as it appears in the version portion of the URL. This value changes when the resource is created, updated, or deleted.
    # The server assigns this value, and ignores what the client specifies, except in the case that the server is imposing version integrity on updates/deletes.
    attr_accessor :versionId                      # 0-1 id
    ##
    # When the resource version last changed
    # When the resource last changed - e.g. when the version changed.
    # This value is always populated except when the resource is first being created. The server / resource manager sets this value; what a client provides is irrelevant. This is equivalent to the HTTP Last-Modified and SHOULD have the same value on a [read](http.html#read) interaction.
    attr_accessor :lastUpdated                    # 0-1 instant
    ##
    # Identifies where the resource comes from
    # A uri that identifies the source system of the resource. This provides a minimal amount of [Provenance](provenance.html#) information that can be used to track or differentiate the source of information in the resource. The source may identify another FHIR server, document, message, database, etc.
    # In the provenance resource, this corresponds to Provenance.entity.what[x]. The exact use of the source (and the implied Provenance.entity.role) is left to implementer discretion. Only one nominated source is allowed; for additional provenance details, a full Provenance resource should be used. 
    # 
    # This element can be used to indicate where the current master source of a resource that has a canonical URL if the resource is no longer hosted at the canonical URL.
    attr_accessor :source                         # 0-1 uri
    ##
    # Profiles this resource claims to conform to
    # A list of profiles (references to [StructureDefinition](structuredefinition.html#) resources) that this resource claims to conform to. The URL is a reference to [StructureDefinition.url](structuredefinition-definitions.html#StructureDefinition.url).
    # It is up to the server and/or other infrastructure of policy to determine whether/how these claims are verified and/or updated over time.  The list of profile URLs is a set.
    attr_accessor :profile                        # 0-* [ canonical(http://hl7.org/fhir/StructureDefinition/StructureDefinition) ]
    ##
    # Security Labels applied to this resource
    # Security labels applied to this resource. These tags connect specific resources to the overall security policy and infrastructure.
    # The security labels can be updated without changing the stated version of the resource. The list of security labels is a set. Uniqueness is based the system/code, and version and display are ignored.
    attr_accessor :security                       # 0-* [ Coding ]
    ##
    # Tags applied to this resource. Tags are intended to be used to identify and relate resources to process and workflow, and applications are not required to consider the tags when interpreting the meaning of a resource.
    # The tags can be updated without changing the stated version of the resource. The list of tags is a set. Uniqueness is based the system/code, and version and display are ignored.
    attr_accessor :tag                            # 0-* [ Coding ]
  end
end
