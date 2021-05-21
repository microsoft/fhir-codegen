// <auto-generated />
// Built from: hl7.fhir.r4.core version: 4.0.1
  // Option: "NAMESPACE" = "fhirCsR4"

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using fhirCsR4.Serialization;

namespace fhirCsR4.Models
{
  /// <summary>
  /// Both Bundle.link and Bundle.entry.link are defined to support providing additional context when Bundles are used (e.g. [HATEOAS](http://en.wikipedia.org/wiki/HATEOAS)). 
  /// Bundle.entry.link corresponds to links found in the HTTP header if the resource in the entry was [read](http.html#read) directly.
  /// This specification defines some specific uses of Bundle.link for [searching](search.html#conformance) and [paging](http.html#paging), but no specific uses for Bundle.entry.link, and no defined function in a transaction - the meaning is implementation specific.
  /// </summary>
  [JsonConverter(typeof(fhirCsR4.Serialization.JsonStreamComponentConverter<BundleLink>))]
  public class BundleLink : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// A name which details the functional use for this link - see [http://www.iana.org/assignments/link-relations/link-relations.xhtml#link-relations-1](http://www.iana.org/assignments/link-relations/link-relations.xhtml#link-relations-1).
    /// </summary>
    public string Relation { get; set; }
    /// <summary>
    /// Extension container element for Relation
    /// </summary>
    public Element _Relation { get; set; }
    /// <summary>
    /// The reference details for the link.
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// Extension container element for Url
    /// </summary>
    public Element _Url { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      ((fhirCsR4.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if (!string.IsNullOrEmpty(Relation))
      {
        writer.WriteString("relation", (string)Relation!);
      }

      if (_Relation != null)
      {
        writer.WritePropertyName("_relation");
        _Relation.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Url))
      {
        writer.WriteString("url", (string)Url!);
      }

      if (_Url != null)
      {
        writer.WritePropertyName("_url");
        _Url.SerializeJson(writer, options);
      }

      if (includeStartObject)
      {
        writer.WriteEndObject();
      }
    }
    /// <summary>
    /// Deserialize a JSON property
    /// </summary>
    public new void DeserializeJsonProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "relation":
          Relation = reader.GetString();
          break;

        case "_relation":
          _Relation = new fhirCsR4.Models.Element();
          _Relation.DeserializeJson(ref reader, options);
          break;

        case "url":
          Url = reader.GetString();
          break;

        case "_url":
          _Url = new fhirCsR4.Models.Element();
          _Url.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR4.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Deserialize a JSON object
    /// </summary>
    public new void DeserializeJson(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
      string propertyName;

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
        {
          return;
        }

        if (reader.TokenType == JsonTokenType.PropertyName)
        {
          propertyName = reader.GetString();
          reader.Read();
          this.DeserializeJsonProperty(ref reader, options, propertyName);
        }
      }

      throw new JsonException();
    }
  }
  /// <summary>
  /// Information about the search process that lead to the creation of this entry.
  /// </summary>
  [JsonConverter(typeof(fhirCsR4.Serialization.JsonStreamComponentConverter<BundleEntrySearch>))]
  public class BundleEntrySearch : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// There is only one mode. In some corner cases, a resource may be included because it is both a match and an include. In these circumstances, 'match' takes precedence.
    /// </summary>
    public string Mode { get; set; }
    /// <summary>
    /// Extension container element for Mode
    /// </summary>
    public Element _Mode { get; set; }
    /// <summary>
    /// Servers are not required to return a ranking score. 1 is most relevant, and 0 is least relevant. Often, search results are sorted by score, but the client may specify a different sort order.
    /// See [Patient Match](patient-operation-match.html) for the EMPI search which relates to this element.
    /// </summary>
    public decimal? Score { get; set; }
    /// <summary>
    /// Extension container element for Score
    /// </summary>
    public Element _Score { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      ((fhirCsR4.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if (!string.IsNullOrEmpty(Mode))
      {
        writer.WriteString("mode", (string)Mode!);
      }

      if (_Mode != null)
      {
        writer.WritePropertyName("_mode");
        _Mode.SerializeJson(writer, options);
      }

      if (Score != null)
      {
        writer.WriteNumber("score", (decimal)Score!);
      }

      if (_Score != null)
      {
        writer.WritePropertyName("_score");
        _Score.SerializeJson(writer, options);
      }

      if (includeStartObject)
      {
        writer.WriteEndObject();
      }
    }
    /// <summary>
    /// Deserialize a JSON property
    /// </summary>
    public new void DeserializeJsonProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "mode":
          Mode = reader.GetString();
          break;

        case "_mode":
          _Mode = new fhirCsR4.Models.Element();
          _Mode.DeserializeJson(ref reader, options);
          break;

        case "score":
          Score = reader.GetDecimal();
          break;

        case "_score":
          _Score = new fhirCsR4.Models.Element();
          _Score.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR4.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Deserialize a JSON object
    /// </summary>
    public new void DeserializeJson(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
      string propertyName;

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
        {
          return;
        }

        if (reader.TokenType == JsonTokenType.PropertyName)
        {
          propertyName = reader.GetString();
          reader.Read();
          this.DeserializeJsonProperty(ref reader, options, propertyName);
        }
      }

      throw new JsonException();
    }
  }
  /// <summary>
  /// Code Values for the Bundle.entry.search.mode field
  /// </summary>
  public static class BundleEntrySearchModeCodes {
    public const string MATCH = "match";
    public const string INCLUDE = "include";
    public const string OUTCOME = "outcome";
  }
  /// <summary>
  /// Additional information about how this entry should be processed as part of a transaction or batch.  For history, it shows how the entry was processed to create the version contained in the entry.
  /// </summary>
  [JsonConverter(typeof(fhirCsR4.Serialization.JsonStreamComponentConverter<BundleEntryRequest>))]
  public class BundleEntryRequest : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Only perform the operation if the Etag value matches. For more information, see the API section ["Managing Resource Contention"](http.html#concurrency).
    /// </summary>
    public string IfMatch { get; set; }
    /// <summary>
    /// Extension container element for IfMatch
    /// </summary>
    public Element _IfMatch { get; set; }
    /// <summary>
    /// Only perform the operation if the last updated date matches. See the API documentation for ["Conditional Read"](http.html#cread).
    /// </summary>
    public string IfModifiedSince { get; set; }
    /// <summary>
    /// Extension container element for IfModifiedSince
    /// </summary>
    public Element _IfModifiedSince { get; set; }
    /// <summary>
    /// Instruct the server not to perform the create if a specified resource already exists. For further information, see the API documentation for ["Conditional Create"](http.html#ccreate). This is just the query portion of the URL - what follows the "?" (not including the "?").
    /// </summary>
    public string IfNoneExist { get; set; }
    /// <summary>
    /// Extension container element for IfNoneExist
    /// </summary>
    public Element _IfNoneExist { get; set; }
    /// <summary>
    /// If the ETag values match, return a 304 Not Modified status. See the API documentation for ["Conditional Read"](http.html#cread).
    /// </summary>
    public string IfNoneMatch { get; set; }
    /// <summary>
    /// Extension container element for IfNoneMatch
    /// </summary>
    public Element _IfNoneMatch { get; set; }
    /// <summary>
    /// In a transaction or batch, this is the HTTP action to be executed for this entry. In a history bundle, this indicates the HTTP action that occurred.
    /// </summary>
    public string Method { get; set; }
    /// <summary>
    /// Extension container element for Method
    /// </summary>
    public Element _Method { get; set; }
    /// <summary>
    /// E.g. for a Patient Create, the method would be "POST" and the URL would be "Patient". For a Patient Update, the method would be PUT and the URL would be "Patient/[id]".
    /// </summary>
    public string Url { get; set; }
    /// <summary>
    /// Extension container element for Url
    /// </summary>
    public Element _Url { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      ((fhirCsR4.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if (!string.IsNullOrEmpty(Method))
      {
        writer.WriteString("method", (string)Method!);
      }

      if (_Method != null)
      {
        writer.WritePropertyName("_method");
        _Method.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Url))
      {
        writer.WriteString("url", (string)Url!);
      }

      if (_Url != null)
      {
        writer.WritePropertyName("_url");
        _Url.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(IfNoneMatch))
      {
        writer.WriteString("ifNoneMatch", (string)IfNoneMatch!);
      }

      if (_IfNoneMatch != null)
      {
        writer.WritePropertyName("_ifNoneMatch");
        _IfNoneMatch.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(IfModifiedSince))
      {
        writer.WriteString("ifModifiedSince", (string)IfModifiedSince!);
      }

      if (_IfModifiedSince != null)
      {
        writer.WritePropertyName("_ifModifiedSince");
        _IfModifiedSince.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(IfMatch))
      {
        writer.WriteString("ifMatch", (string)IfMatch!);
      }

      if (_IfMatch != null)
      {
        writer.WritePropertyName("_ifMatch");
        _IfMatch.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(IfNoneExist))
      {
        writer.WriteString("ifNoneExist", (string)IfNoneExist!);
      }

      if (_IfNoneExist != null)
      {
        writer.WritePropertyName("_ifNoneExist");
        _IfNoneExist.SerializeJson(writer, options);
      }

      if (includeStartObject)
      {
        writer.WriteEndObject();
      }
    }
    /// <summary>
    /// Deserialize a JSON property
    /// </summary>
    public new void DeserializeJsonProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "ifMatch":
          IfMatch = reader.GetString();
          break;

        case "_ifMatch":
          _IfMatch = new fhirCsR4.Models.Element();
          _IfMatch.DeserializeJson(ref reader, options);
          break;

        case "ifModifiedSince":
          IfModifiedSince = reader.GetString();
          break;

        case "_ifModifiedSince":
          _IfModifiedSince = new fhirCsR4.Models.Element();
          _IfModifiedSince.DeserializeJson(ref reader, options);
          break;

        case "ifNoneExist":
          IfNoneExist = reader.GetString();
          break;

        case "_ifNoneExist":
          _IfNoneExist = new fhirCsR4.Models.Element();
          _IfNoneExist.DeserializeJson(ref reader, options);
          break;

        case "ifNoneMatch":
          IfNoneMatch = reader.GetString();
          break;

        case "_ifNoneMatch":
          _IfNoneMatch = new fhirCsR4.Models.Element();
          _IfNoneMatch.DeserializeJson(ref reader, options);
          break;

        case "method":
          Method = reader.GetString();
          break;

        case "_method":
          _Method = new fhirCsR4.Models.Element();
          _Method.DeserializeJson(ref reader, options);
          break;

        case "url":
          Url = reader.GetString();
          break;

        case "_url":
          _Url = new fhirCsR4.Models.Element();
          _Url.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR4.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Deserialize a JSON object
    /// </summary>
    public new void DeserializeJson(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
      string propertyName;

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
        {
          return;
        }

        if (reader.TokenType == JsonTokenType.PropertyName)
        {
          propertyName = reader.GetString();
          reader.Read();
          this.DeserializeJsonProperty(ref reader, options, propertyName);
        }
      }

      throw new JsonException();
    }
  }
  /// <summary>
  /// Code Values for the Bundle.entry.request.method field
  /// </summary>
  public static class BundleEntryRequestMethodCodes {
    public const string GET = "GET";
    public const string HEAD = "HEAD";
    public const string POST = "POST";
    public const string PUT = "PUT";
    public const string DELETE = "DELETE";
    public const string PATCH = "PATCH";
  }
  /// <summary>
  /// Indicates the results of processing the corresponding 'request' entry in the batch or transaction being responded to or what the results of an operation where when returning history.
  /// </summary>
  [JsonConverter(typeof(fhirCsR4.Serialization.JsonStreamComponentConverter<BundleEntryResponse>))]
  public class BundleEntryResponse : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// Etags match the Resource.meta.versionId. The ETag has to match the version id in the header if a resource is included.
    /// </summary>
    public string Etag { get; set; }
    /// <summary>
    /// Extension container element for Etag
    /// </summary>
    public Element _Etag { get; set; }
    /// <summary>
    /// This has to match the same time in the meta header (meta.lastUpdated) if a resource is included.
    /// </summary>
    public string LastModified { get; set; }
    /// <summary>
    /// Extension container element for LastModified
    /// </summary>
    public Element _LastModified { get; set; }
    /// <summary>
    /// The location header created by processing this operation, populated if the operation returns a location.
    /// </summary>
    public string Location { get; set; }
    /// <summary>
    /// Extension container element for Location
    /// </summary>
    public Element _Location { get; set; }
    /// <summary>
    /// For a POST/PUT operation, this is the equivalent outcome that would be returned for prefer = operationoutcome - except that the resource is always returned whether or not the outcome is returned.
    /// This outcome is not used for error responses in batch/transaction, only for hints and warnings. In a batch operation, the error will be in Bundle.entry.response, and for transaction, there will be a single OperationOutcome instead of a bundle in the case of an error.
    /// </summary>
    public Resource Outcome { get; set; }
    /// <summary>
    /// The status code returned by processing this entry. The status SHALL start with a 3 digit HTTP code (e.g. 404) and may contain the standard HTTP description associated with the status code.
    /// </summary>
    public string Status { get; set; }
    /// <summary>
    /// Extension container element for Status
    /// </summary>
    public Element _Status { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      ((fhirCsR4.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if (!string.IsNullOrEmpty(Status))
      {
        writer.WriteString("status", (string)Status!);
      }

      if (_Status != null)
      {
        writer.WritePropertyName("_status");
        _Status.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Location))
      {
        writer.WriteString("location", (string)Location!);
      }

      if (_Location != null)
      {
        writer.WritePropertyName("_location");
        _Location.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Etag))
      {
        writer.WriteString("etag", (string)Etag!);
      }

      if (_Etag != null)
      {
        writer.WritePropertyName("_etag");
        _Etag.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(LastModified))
      {
        writer.WriteString("lastModified", (string)LastModified!);
      }

      if (_LastModified != null)
      {
        writer.WritePropertyName("_lastModified");
        _LastModified.SerializeJson(writer, options);
      }

      if (Outcome != null)
      {
        writer.WritePropertyName("outcome");
        JsonSerializer.Serialize<fhirCsR4.Models.Resource>(writer, (fhirCsR4.Models.Resource)Outcome, options);
      }

      if (includeStartObject)
      {
        writer.WriteEndObject();
      }
    }
    /// <summary>
    /// Deserialize a JSON property
    /// </summary>
    public new void DeserializeJsonProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "etag":
          Etag = reader.GetString();
          break;

        case "_etag":
          _Etag = new fhirCsR4.Models.Element();
          _Etag.DeserializeJson(ref reader, options);
          break;

        case "lastModified":
          LastModified = reader.GetString();
          break;

        case "_lastModified":
          _LastModified = new fhirCsR4.Models.Element();
          _LastModified.DeserializeJson(ref reader, options);
          break;

        case "location":
          Location = reader.GetString();
          break;

        case "_location":
          _Location = new fhirCsR4.Models.Element();
          _Location.DeserializeJson(ref reader, options);
          break;

        case "outcome":
          Outcome = JsonSerializer.Deserialize<fhirCsR4.Models.Resource>(ref reader, options);
          break;

        case "status":
          Status = reader.GetString();
          break;

        case "_status":
          _Status = new fhirCsR4.Models.Element();
          _Status.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR4.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Deserialize a JSON object
    /// </summary>
    public new void DeserializeJson(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
      string propertyName;

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
        {
          return;
        }

        if (reader.TokenType == JsonTokenType.PropertyName)
        {
          propertyName = reader.GetString();
          reader.Read();
          this.DeserializeJsonProperty(ref reader, options, propertyName);
        }
      }

      throw new JsonException();
    }
  }
  /// <summary>
  /// An entry in a bundle resource - will either contain a resource or information about a resource (transactions and history only).
  /// </summary>
  [JsonConverter(typeof(fhirCsR4.Serialization.JsonStreamComponentConverter<BundleEntry>))]
  public class BundleEntry : BackboneElement,  IFhirJsonSerializable {
    /// <summary>
    /// fullUrl might not be [unique in the context of a resource](bundle.html#bundle-unique). Note that since [FHIR resources do not need to be served through the FHIR API](references.html), the fullURL might be a URN or an absolute URL that does not end with the logical id of the resource (Resource.id). However, but if the fullUrl does look like a RESTful server URL (e.g. meets the [regex](references.html#regex), then the 'id' portion of the fullUrl SHALL end with the Resource.id.
    /// Note that the fullUrl is not the same as the canonical URL - it's an absolute url for an endpoint serving the resource (these will happen to have the same value on the canonical server for the resource with the canonical URL).
    /// </summary>
    public string FullUrl { get; set; }
    /// <summary>
    /// Extension container element for FullUrl
    /// </summary>
    public Element _FullUrl { get; set; }
    /// <summary>
    /// A series of links that provide context to this entry.
    /// </summary>
    public List<BundleLink> Link { get; set; }
    /// <summary>
    /// Additional information about how this entry should be processed as part of a transaction or batch.  For history, it shows how the entry was processed to create the version contained in the entry.
    /// </summary>
    public BundleEntryRequest Request { get; set; }
    /// <summary>
    /// The Resource for the entry. The purpose/meaning of the resource is determined by the Bundle.type.
    /// </summary>
    public Resource Resource { get; set; }
    /// <summary>
    /// Indicates the results of processing the corresponding 'request' entry in the batch or transaction being responded to or what the results of an operation where when returning history.
    /// </summary>
    public BundleEntryResponse Response { get; set; }
    /// <summary>
    /// Information about the search process that lead to the creation of this entry.
    /// </summary>
    public BundleEntrySearch Search { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      ((fhirCsR4.Models.BackboneElement)this).SerializeJson(writer, options, false);

      if ((Link != null) && (Link.Count != 0))
      {
        writer.WritePropertyName("link");
        writer.WriteStartArray();

        foreach (BundleLink valLink in Link)
        {
          valLink.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (!string.IsNullOrEmpty(FullUrl))
      {
        writer.WriteString("fullUrl", (string)FullUrl!);
      }

      if (_FullUrl != null)
      {
        writer.WritePropertyName("_fullUrl");
        _FullUrl.SerializeJson(writer, options);
      }

      if (Resource != null)
      {
        writer.WritePropertyName("resource");
        JsonSerializer.Serialize<fhirCsR4.Models.Resource>(writer, (fhirCsR4.Models.Resource)Resource, options);
      }

      if (Search != null)
      {
        writer.WritePropertyName("search");
        Search.SerializeJson(writer, options);
      }

      if (Request != null)
      {
        writer.WritePropertyName("request");
        Request.SerializeJson(writer, options);
      }

      if (Response != null)
      {
        writer.WritePropertyName("response");
        Response.SerializeJson(writer, options);
      }

      if (includeStartObject)
      {
        writer.WriteEndObject();
      }
    }
    /// <summary>
    /// Deserialize a JSON property
    /// </summary>
    public new void DeserializeJsonProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "fullUrl":
          FullUrl = reader.GetString();
          break;

        case "_fullUrl":
          _FullUrl = new fhirCsR4.Models.Element();
          _FullUrl.DeserializeJson(ref reader, options);
          break;

        case "link":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Link = new List<BundleLink>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.BundleLink objLink = new fhirCsR4.Models.BundleLink();
            objLink.DeserializeJson(ref reader, options);
            Link.Add(objLink);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Link.Count == 0)
          {
            Link = null;
          }

          break;

        case "request":
          Request = new fhirCsR4.Models.BundleEntryRequest();
          Request.DeserializeJson(ref reader, options);
          break;

        case "resource":
          Resource = JsonSerializer.Deserialize<fhirCsR4.Models.Resource>(ref reader, options);
          break;

        case "response":
          Response = new fhirCsR4.Models.BundleEntryResponse();
          Response.DeserializeJson(ref reader, options);
          break;

        case "search":
          Search = new fhirCsR4.Models.BundleEntrySearch();
          Search.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR4.Models.BackboneElement)this).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Deserialize a JSON object
    /// </summary>
    public new void DeserializeJson(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
      string propertyName;

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
        {
          return;
        }

        if (reader.TokenType == JsonTokenType.PropertyName)
        {
          propertyName = reader.GetString();
          reader.Read();
          this.DeserializeJsonProperty(ref reader, options, propertyName);
        }
      }

      throw new JsonException();
    }
  }
  /// <summary>
  /// A container for a collection of resources.
  /// </summary>
  [JsonConverter(typeof(fhirCsR4.Serialization.JsonStreamComponentConverter<Bundle>))]
  public class Bundle : Resource,  IFhirJsonSerializable {
    /// <summary>
    /// Resource Type Name
    /// </summary>
    public string ResourceType => "Bundle";
    /// <summary>
    /// An entry in a bundle resource - will either contain a resource or information about a resource (transactions and history only).
    /// </summary>
    public List<BundleEntry> Entry { get; set; }
    /// <summary>
    /// Persistent identity generally only matters for batches of type Document, Message, and Collection. It would not normally be populated for search and history results and servers ignore Bundle.identifier when processing batches and transactions. For Documents  the .identifier SHALL be populated such that the .identifier is globally unique.
    /// </summary>
    public Identifier Identifier { get; set; }
    /// <summary>
    /// Both Bundle.link and Bundle.entry.link are defined to support providing additional context when Bundles are used (e.g. [HATEOAS](http://en.wikipedia.org/wiki/HATEOAS)). 
    /// Bundle.entry.link corresponds to links found in the HTTP header if the resource in the entry was [read](http.html#read) directly.
    /// This specification defines some specific uses of Bundle.link for [searching](search.html#conformance) and [paging](http.html#paging), but no specific uses for Bundle.entry.link, and no defined function in a transaction - the meaning is implementation specific.
    /// </summary>
    public List<BundleLink> Link { get; set; }
    /// <summary>
    /// The signature could be created by the "author" of the bundle or by the originating device.   Requirements around inclusion of a signature, verification of signatures and treatment of signed/non-signed bundles is implementation-environment specific.
    /// </summary>
    public Signature Signature { get; set; }
    /// <summary>
    /// For many bundles, the timestamp is equal to .meta.lastUpdated, because they are not stored (e.g. search results). When a bundle is placed in a persistent store, .meta.lastUpdated will be usually be changed by the server. When the bundle is a message, a middleware agent altering the message (even if not stored) SHOULD update .meta.lastUpdated. .timestamp is used to track the original time of the Bundle, and SHOULD be populated.  
    /// Usage:
    /// * document : the date the document was created. Note: the composition may predate the document, or be associated with multiple documents. The date of the composition - the authoring time - may be earlier than the document assembly time
    /// * message : the date that the content of the message was assembled. This date is not changed by middleware engines unless they add additional data that changes the meaning of the time of the message
    /// * history : the date that the history was assembled. This time would be used as the _since time to ask for subsequent updates
    /// * searchset : the time that the search set was assembled. Note that different pages MAY have different timestamps but need not. Having different timestamps does not imply that subsequent pages will represent or include changes made since the initial query
    /// * transaction | transaction-response | batch | batch-response | collection : no particular assigned meaning
    /// The timestamp value should be greater than the lastUpdated and other timestamps in the resources in the bundle, and it should be equal or earlier than the .meta.lastUpdated on the Bundle itself.
    /// </summary>
    public string Timestamp { get; set; }
    /// <summary>
    /// Extension container element for Timestamp
    /// </summary>
    public Element _Timestamp { get; set; }
    /// <summary>
    /// Only used if the bundle is a search result set. The total does not include resources such as OperationOutcome and included resources, only the total number of matching resources.
    /// </summary>
    public uint? Total { get; set; }
    /// <summary>
    /// It's possible to use a bundle for other purposes (e.g. a document can be accepted as a transaction). This is primarily defined so that there can be specific rules for some of the bundle types.
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// Extension container element for Type
    /// </summary>
    public Element _Type { get; set; }
    /// <summary>
    /// Serialize to a JSON object
    /// </summary>
    public new void SerializeJson(Utf8JsonWriter writer, JsonSerializerOptions options, bool includeStartObject = true)
    {
      if (includeStartObject)
      {
        writer.WriteStartObject();
      }
      if (!string.IsNullOrEmpty(ResourceType))
      {
        writer.WriteString("resourceType", (string)ResourceType!);
      }


      ((fhirCsR4.Models.Resource)this).SerializeJson(writer, options, false);

      if (Identifier != null)
      {
        writer.WritePropertyName("identifier");
        Identifier.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Type))
      {
        writer.WriteString("type", (string)Type!);
      }

      if (_Type != null)
      {
        writer.WritePropertyName("_type");
        _Type.SerializeJson(writer, options);
      }

      if (!string.IsNullOrEmpty(Timestamp))
      {
        writer.WriteString("timestamp", (string)Timestamp!);
      }

      if (_Timestamp != null)
      {
        writer.WritePropertyName("_timestamp");
        _Timestamp.SerializeJson(writer, options);
      }

      if (Total != null)
      {
        writer.WriteNumber("total", (uint)Total!);
      }

      if ((Link != null) && (Link.Count != 0))
      {
        writer.WritePropertyName("link");
        writer.WriteStartArray();

        foreach (BundleLink valLink in Link)
        {
          valLink.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if ((Entry != null) && (Entry.Count != 0))
      {
        writer.WritePropertyName("entry");
        writer.WriteStartArray();

        foreach (BundleEntry valEntry in Entry)
        {
          valEntry.SerializeJson(writer, options, true);
        }

        writer.WriteEndArray();
      }

      if (Signature != null)
      {
        writer.WritePropertyName("signature");
        Signature.SerializeJson(writer, options);
      }

      if (includeStartObject)
      {
        writer.WriteEndObject();
      }
    }
    /// <summary>
    /// Deserialize a JSON property
    /// </summary>
    public new void DeserializeJsonProperty(ref Utf8JsonReader reader, JsonSerializerOptions options, string propertyName)
    {
      switch (propertyName)
      {
        case "entry":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Entry = new List<BundleEntry>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.BundleEntry objEntry = new fhirCsR4.Models.BundleEntry();
            objEntry.DeserializeJson(ref reader, options);
            Entry.Add(objEntry);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Entry.Count == 0)
          {
            Entry = null;
          }

          break;

        case "identifier":
          Identifier = new fhirCsR4.Models.Identifier();
          Identifier.DeserializeJson(ref reader, options);
          break;

        case "link":
          if ((reader.TokenType != JsonTokenType.StartArray) || (!reader.Read()))
          {
            throw new JsonException();
          }

          Link = new List<BundleLink>();

          while (reader.TokenType != JsonTokenType.EndArray)
          {
            fhirCsR4.Models.BundleLink objLink = new fhirCsR4.Models.BundleLink();
            objLink.DeserializeJson(ref reader, options);
            Link.Add(objLink);

            if (!reader.Read())
            {
              throw new JsonException();
            }
          }

          if (Link.Count == 0)
          {
            Link = null;
          }

          break;

        case "signature":
          Signature = new fhirCsR4.Models.Signature();
          Signature.DeserializeJson(ref reader, options);
          break;

        case "timestamp":
          Timestamp = reader.GetString();
          break;

        case "_timestamp":
          _Timestamp = new fhirCsR4.Models.Element();
          _Timestamp.DeserializeJson(ref reader, options);
          break;

        case "total":
          Total = reader.GetUInt32();
          break;

        case "type":
          Type = reader.GetString();
          break;

        case "_type":
          _Type = new fhirCsR4.Models.Element();
          _Type.DeserializeJson(ref reader, options);
          break;

        default:
          ((fhirCsR4.Models.Resource)this).DeserializeJsonProperty(ref reader, options, propertyName);
          break;
      }
    }

    /// <summary>
    /// Deserialize a JSON object
    /// </summary>
    public new void DeserializeJson(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
      string propertyName;

      while (reader.Read())
      {
        if (reader.TokenType == JsonTokenType.EndObject)
        {
          return;
        }

        if (reader.TokenType == JsonTokenType.PropertyName)
        {
          propertyName = reader.GetString();
          reader.Read();
          this.DeserializeJsonProperty(ref reader, options, propertyName);
        }
      }

      throw new JsonException();
    }
  }
  /// <summary>
  /// Code Values for the Bundle.type field
  /// </summary>
  public static class BundleTypeCodes {
    public const string DOCUMENT = "document";
    public const string MESSAGE = "message";
    public const string TRANSACTION = "transaction";
    public const string TRANSACTION_RESPONSE = "transaction-response";
    public const string BATCH = "batch";
    public const string BATCH_RESPONSE = "batch-response";
    public const string HISTORY = "history";
    public const string SEARCHSET = "searchset";
    public const string COLLECTION = "collection";
  }
}
