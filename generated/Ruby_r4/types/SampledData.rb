module FHIR

  ##
  # Base StructureDefinition for SampledData Type: A series of measurements taken by a device, with upper and lower limits. There may be more than one dimension in the data.
  # There is a need for a concise way to handle the data produced by devices that sample a physical state at a high frequency.
  class SampledData < FHIR::Model
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
        'path'=>'SampledData.id',
        'min'=>0,
        'max'=>1
      },
      ##
      # Additional content defined by implementations
      # May be used to represent additional information that is not part of the basic definition of the element. To make the use of extensions safe and manageable, there is a strict set of governance  applied to the definition and use of extensions. Though any implementer can define an extension, there is a set of requirements that SHALL be met as part of the definition of the extension.
      # There can be no stigma associated with the use of extensions by any application, project, or standard - regardless of the institution or jurisdiction that uses or defines the extensions.  The use of extensions is what allows the FHIR specification to retain a core level of simplicity for everyone.
      'extension' => {
        'type'=>'Extension',
        'path'=>'SampledData.extension',
        'min'=>0,
        'max'=>Float::INFINITY
      },
      ##
      # Zero value and units
      # The base quantity that a measured value of zero represents. In addition, this provides the units of the entire measurement series.
      'origin' => {
        'type'=>'Quantity',
        'path'=>'SampledData.origin',
        'min'=>1,
        'max'=>1
      },
      ##
      # Number of milliseconds between samples
      # The length of time between sampling times, measured in milliseconds.
      # This is usually a whole number.
      'period' => {
        'type'=>'decimal',
        'path'=>'SampledData.period',
        'min'=>1,
        'max'=>1
      },
      ##
      # Multiply data by this before adding to origin
      # A correction factor that is applied to the sampled data points before they are added to the origin.
      'factor' => {
        'type'=>'decimal',
        'path'=>'SampledData.factor',
        'min'=>0,
        'max'=>1
      },
      ##
      # Lower limit of detection
      # The lower limit of detection of the measured points. This is needed if any of the data points have the value "L" (lower than detection limit).
      'lowerLimit' => {
        'type'=>'decimal',
        'path'=>'SampledData.lowerLimit',
        'min'=>0,
        'max'=>1
      },
      ##
      # Upper limit of detection
      # The upper limit of detection of the measured points. This is needed if any of the data points have the value "U" (higher than detection limit).
      'upperLimit' => {
        'type'=>'decimal',
        'path'=>'SampledData.upperLimit',
        'min'=>0,
        'max'=>1
      },
      ##
      # Number of sample points at each time point
      # The number of sample points at each time point. If this value is greater than one, then the dimensions will be interlaced - all the sample points for a point in time will be recorded at once.
      # If there is more than one dimension, the code for the type of data will define the meaning of the dimensions (typically ECG data).
      'dimensions' => {
        'type'=>'positiveInt',
        'path'=>'SampledData.dimensions',
        'min'=>1,
        'max'=>1
      },
      ##
      # Decimal values with spaces, or "E" | "U" | "L"
      # A series of data points which are decimal values separated by a single space (character u20). The special values "E" (error), "L" (below detection limit) and "U" (above detection limit) can also be used in place of a decimal value.
      # Data may be missing if it is omitted for summarization purposes. In general, data is required for any actual use of a SampledData.
      'data' => {
        'type'=>'string',
        'path'=>'SampledData.data',
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
    # Zero value and units
    # The base quantity that a measured value of zero represents. In addition, this provides the units of the entire measurement series.
    attr_accessor :origin                         # 1-1 Quantity
    ##
    # Number of milliseconds between samples
    # The length of time between sampling times, measured in milliseconds.
    # This is usually a whole number.
    attr_accessor :period                         # 1-1 decimal
    ##
    # Multiply data by this before adding to origin
    # A correction factor that is applied to the sampled data points before they are added to the origin.
    attr_accessor :factor                         # 0-1 decimal
    ##
    # Lower limit of detection
    # The lower limit of detection of the measured points. This is needed if any of the data points have the value "L" (lower than detection limit).
    attr_accessor :lowerLimit                     # 0-1 decimal
    ##
    # Upper limit of detection
    # The upper limit of detection of the measured points. This is needed if any of the data points have the value "U" (higher than detection limit).
    attr_accessor :upperLimit                     # 0-1 decimal
    ##
    # Number of sample points at each time point
    # The number of sample points at each time point. If this value is greater than one, then the dimensions will be interlaced - all the sample points for a point in time will be recorded at once.
    # If there is more than one dimension, the code for the type of data will define the meaning of the dimensions (typically ECG data).
    attr_accessor :dimensions                     # 1-1 positiveInt
    ##
    # Decimal values with spaces, or "E" | "U" | "L"
    # A series of data points which are decimal values separated by a single space (character u20). The special values "E" (error), "L" (below detection limit) and "U" (above detection limit) can also be used in place of a decimal value.
    # Data may be missing if it is omitted for summarization purposes. In general, data is required for any actual use of a SampledData.
    attr_accessor :data                           # 0-1 string
  end
end
