// <auto-generated/>
// Contents of: hl7.fhir.r2.core version: 1.0.2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Hl7.Fhir.Introspection;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification;
using Hl7.Fhir.Utility;
using Hl7.Fhir.Validation;

/*
  Copyright (c) 2011+, HL7, Inc.
  All rights reserved.
  
  Redistribution and use in source and binary forms, with or without modification, 
  are permitted provided that the following conditions are met:
  
   * Redistributions of source code must retain the above copyright notice, this 
     list of conditions and the following disclaimer.
   * Redistributions in binary form must reproduce the above copyright notice, 
     this list of conditions and the following disclaimer in the documentation 
     and/or other materials provided with the distribution.
   * Neither the name of HL7 nor the names of its contributors may be used to 
     endorse or promote products derived from this software without specific 
     prior written permission.
  
  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
  IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
  POSSIBILITY OF SUCH DAMAGE.
  
*/

namespace Hl7.Fhir.Model
{
  /// <summary>
  /// PaymentReconciliation resource
  /// </summary>
  [Serializable]
  [DataContract]
  [FhirType("PaymentReconciliation","http://hl7.org/fhir/StructureDefinition/PaymentReconciliation", IsResource=true)]
  public partial class PaymentReconciliation : Hl7.Fhir.Model.DomainResource
  {
    /// <summary>
    /// FHIR Type Name
    /// </summary>
    public override string TypeName { get { return "PaymentReconciliation"; } }

    /// <summary>
    /// Details
    /// </summary>
    [Serializable]
    [DataContract]
    [FhirType("PaymentReconciliation#Details", IsNestedType=true)]
    public partial class DetailsComponent : Hl7.Fhir.Model.BackboneElement
    {
      /// <summary>
      /// FHIR Type Name
      /// </summary>
      public override string TypeName { get { return "PaymentReconciliation#Details"; } }

      /// <summary>
      /// Type code
      /// </summary>
      [FhirElement("type", InSummary=true, Order=40)]
      [Cardinality(Min=1,Max=1)]
      [DataMember]
      public Hl7.Fhir.Model.Coding Type
      {
        get { return _Type; }
        set { _Type = value; OnPropertyChanged("Type"); }
      }

      private Hl7.Fhir.Model.Coding _Type;

      /// <summary>
      /// Claim
      /// </summary>
      [FhirElement("request", InSummary=true, Order=50)]
      [CLSCompliant(false)]
      [References("Resource")]
      [DataMember]
      public Hl7.Fhir.Model.ResourceReference Request
      {
        get { return _Request; }
        set { _Request = value; OnPropertyChanged("Request"); }
      }

      private Hl7.Fhir.Model.ResourceReference _Request;

      /// <summary>
      /// Claim Response
      /// </summary>
      [FhirElement("responce", InSummary=true, Order=60)]
      [CLSCompliant(false)]
      [References("Resource")]
      [DataMember]
      public Hl7.Fhir.Model.ResourceReference Responce
      {
        get { return _Responce; }
        set { _Responce = value; OnPropertyChanged("Responce"); }
      }

      private Hl7.Fhir.Model.ResourceReference _Responce;

      /// <summary>
      /// Submitter
      /// </summary>
      [FhirElement("submitter", InSummary=true, Order=70)]
      [CLSCompliant(false)]
      [References("Organization")]
      [DataMember]
      public Hl7.Fhir.Model.ResourceReference Submitter
      {
        get { return _Submitter; }
        set { _Submitter = value; OnPropertyChanged("Submitter"); }
      }

      private Hl7.Fhir.Model.ResourceReference _Submitter;

      /// <summary>
      /// Payee
      /// </summary>
      [FhirElement("payee", InSummary=true, Order=80)]
      [CLSCompliant(false)]
      [References("Organization")]
      [DataMember]
      public Hl7.Fhir.Model.ResourceReference Payee
      {
        get { return _Payee; }
        set { _Payee = value; OnPropertyChanged("Payee"); }
      }

      private Hl7.Fhir.Model.ResourceReference _Payee;

      /// <summary>
      /// Invoice date
      /// </summary>
      [FhirElement("date", InSummary=true, Order=90)]
      [DataMember]
      public Hl7.Fhir.Model.Date DateElement
      {
        get { return _DateElement; }
        set { _DateElement = value; OnPropertyChanged("DateElement"); }
      }

      private Hl7.Fhir.Model.Date _DateElement;

      /// <summary>
      /// Invoice date
      /// </summary>
      /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
      [IgnoreDataMember]
      public string Date
      {
        get { return DateElement != null ? DateElement.Value : null; }
        set
        {
          if (value == null)
            DateElement = null;
          else
            DateElement = new Hl7.Fhir.Model.Date(value);
          OnPropertyChanged("Date");
        }
      }

      /// <summary>
      /// Detail amount
      /// </summary>
      [FhirElement("amount", InSummary=true, Order=100)]
      [DataMember]
      public Hl7.Fhir.Model.Quantity Amount
      {
        get { return _Amount; }
        set { _Amount = value; OnPropertyChanged("Amount"); }
      }

      private Hl7.Fhir.Model.Quantity _Amount;

      public override IDeepCopyable CopyTo(IDeepCopyable other)
      {
        var dest = other as DetailsComponent;

        if (dest == null)
        {
          throw new ArgumentException("Can only copy to an object of the same type", "other");
        }

        base.CopyTo(dest);
        if(Type != null) dest.Type = (Hl7.Fhir.Model.Coding)Type.DeepCopy();
        if(Request != null) dest.Request = (Hl7.Fhir.Model.ResourceReference)Request.DeepCopy();
        if(Responce != null) dest.Responce = (Hl7.Fhir.Model.ResourceReference)Responce.DeepCopy();
        if(Submitter != null) dest.Submitter = (Hl7.Fhir.Model.ResourceReference)Submitter.DeepCopy();
        if(Payee != null) dest.Payee = (Hl7.Fhir.Model.ResourceReference)Payee.DeepCopy();
        if(DateElement != null) dest.DateElement = (Hl7.Fhir.Model.Date)DateElement.DeepCopy();
        if(Amount != null) dest.Amount = (Hl7.Fhir.Model.Quantity)Amount.DeepCopy();
        return dest;
      }

      public override IDeepCopyable DeepCopy()
      {
        return CopyTo(new DetailsComponent());
      }

      ///<inheritdoc />
      public override bool Matches(IDeepComparable other)
      {
        var otherT = other as DetailsComponent;
        if(otherT == null) return false;

        if(!base.Matches(otherT)) return false;
        if( !DeepComparable.Matches(Type, otherT.Type)) return false;
        if( !DeepComparable.Matches(Request, otherT.Request)) return false;
        if( !DeepComparable.Matches(Responce, otherT.Responce)) return false;
        if( !DeepComparable.Matches(Submitter, otherT.Submitter)) return false;
        if( !DeepComparable.Matches(Payee, otherT.Payee)) return false;
        if( !DeepComparable.Matches(DateElement, otherT.DateElement)) return false;
        if( !DeepComparable.Matches(Amount, otherT.Amount)) return false;

        return true;
      }

      public override bool IsExactly(IDeepComparable other)
      {
        var otherT = other as DetailsComponent;
        if(otherT == null) return false;

        if(!base.IsExactly(otherT)) return false;
        if( !DeepComparable.IsExactly(Type, otherT.Type)) return false;
        if( !DeepComparable.IsExactly(Request, otherT.Request)) return false;
        if( !DeepComparable.IsExactly(Responce, otherT.Responce)) return false;
        if( !DeepComparable.IsExactly(Submitter, otherT.Submitter)) return false;
        if( !DeepComparable.IsExactly(Payee, otherT.Payee)) return false;
        if( !DeepComparable.IsExactly(DateElement, otherT.DateElement)) return false;
        if( !DeepComparable.IsExactly(Amount, otherT.Amount)) return false;

        return true;
      }

      [IgnoreDataMember]
      public override IEnumerable<Base> Children
      {
        get
        {
          foreach (var item in base.Children) yield return item;
          if (Type != null) yield return Type;
          if (Request != null) yield return Request;
          if (Responce != null) yield return Responce;
          if (Submitter != null) yield return Submitter;
          if (Payee != null) yield return Payee;
          if (DateElement != null) yield return DateElement;
          if (Amount != null) yield return Amount;
        }
      }

      [IgnoreDataMember]
      public override IEnumerable<ElementValue> NamedChildren
      {
        get
        {
          foreach (var item in base.NamedChildren) yield return item;
          if (Type != null) yield return new ElementValue("type", Type);
          if (Request != null) yield return new ElementValue("request", Request);
          if (Responce != null) yield return new ElementValue("responce", Responce);
          if (Submitter != null) yield return new ElementValue("submitter", Submitter);
          if (Payee != null) yield return new ElementValue("payee", Payee);
          if (DateElement != null) yield return new ElementValue("date", DateElement);
          if (Amount != null) yield return new ElementValue("amount", Amount);
        }
      }

      protected override bool TryGetValue(string key, out object value)
      {
        switch (key)
        {
          case "type":
            value = Type;
            return Type is not null;
          case "request":
            value = Request;
            return Request is not null;
          case "responce":
            value = Responce;
            return Responce is not null;
          case "submitter":
            value = Submitter;
            return Submitter is not null;
          case "payee":
            value = Payee;
            return Payee is not null;
          case "date":
            value = DateElement;
            return DateElement is not null;
          case "amount":
            value = Amount;
            return Amount is not null;
          default:
            return base.TryGetValue(key, out value);
        };

      }

      protected override IEnumerable<KeyValuePair<string, object>> GetElementPairs()
      {
        foreach (var kvp in base.GetElementPairs()) yield return kvp;
        if (Type is not null) yield return new KeyValuePair<string,object>("type",Type);
        if (Request is not null) yield return new KeyValuePair<string,object>("request",Request);
        if (Responce is not null) yield return new KeyValuePair<string,object>("responce",Responce);
        if (Submitter is not null) yield return new KeyValuePair<string,object>("submitter",Submitter);
        if (Payee is not null) yield return new KeyValuePair<string,object>("payee",Payee);
        if (DateElement is not null) yield return new KeyValuePair<string,object>("date",DateElement);
        if (Amount is not null) yield return new KeyValuePair<string,object>("amount",Amount);
      }

    }

    /// <summary>
    /// Note text
    /// </summary>
    [Serializable]
    [DataContract]
    [FhirType("PaymentReconciliation#Notes", IsNestedType=true)]
    public partial class NotesComponent : Hl7.Fhir.Model.BackboneElement
    {
      /// <summary>
      /// FHIR Type Name
      /// </summary>
      public override string TypeName { get { return "PaymentReconciliation#Notes"; } }

      /// <summary>
      /// display | print | printoper
      /// </summary>
      [FhirElement("type", InSummary=true, Order=40)]
      [DataMember]
      public Hl7.Fhir.Model.Coding Type
      {
        get { return _Type; }
        set { _Type = value; OnPropertyChanged("Type"); }
      }

      private Hl7.Fhir.Model.Coding _Type;

      /// <summary>
      /// Notes text
      /// </summary>
      [FhirElement("text", InSummary=true, Order=50)]
      [DataMember]
      public Hl7.Fhir.Model.FhirString TextElement
      {
        get { return _TextElement; }
        set { _TextElement = value; OnPropertyChanged("TextElement"); }
      }

      private Hl7.Fhir.Model.FhirString _TextElement;

      /// <summary>
      /// Notes text
      /// </summary>
      /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
      [IgnoreDataMember]
      public string Text
      {
        get { return TextElement != null ? TextElement.Value : null; }
        set
        {
          if (value == null)
            TextElement = null;
          else
            TextElement = new Hl7.Fhir.Model.FhirString(value);
          OnPropertyChanged("Text");
        }
      }

      public override IDeepCopyable CopyTo(IDeepCopyable other)
      {
        var dest = other as NotesComponent;

        if (dest == null)
        {
          throw new ArgumentException("Can only copy to an object of the same type", "other");
        }

        base.CopyTo(dest);
        if(Type != null) dest.Type = (Hl7.Fhir.Model.Coding)Type.DeepCopy();
        if(TextElement != null) dest.TextElement = (Hl7.Fhir.Model.FhirString)TextElement.DeepCopy();
        return dest;
      }

      public override IDeepCopyable DeepCopy()
      {
        return CopyTo(new NotesComponent());
      }

      ///<inheritdoc />
      public override bool Matches(IDeepComparable other)
      {
        var otherT = other as NotesComponent;
        if(otherT == null) return false;

        if(!base.Matches(otherT)) return false;
        if( !DeepComparable.Matches(Type, otherT.Type)) return false;
        if( !DeepComparable.Matches(TextElement, otherT.TextElement)) return false;

        return true;
      }

      public override bool IsExactly(IDeepComparable other)
      {
        var otherT = other as NotesComponent;
        if(otherT == null) return false;

        if(!base.IsExactly(otherT)) return false;
        if( !DeepComparable.IsExactly(Type, otherT.Type)) return false;
        if( !DeepComparable.IsExactly(TextElement, otherT.TextElement)) return false;

        return true;
      }

      [IgnoreDataMember]
      public override IEnumerable<Base> Children
      {
        get
        {
          foreach (var item in base.Children) yield return item;
          if (Type != null) yield return Type;
          if (TextElement != null) yield return TextElement;
        }
      }

      [IgnoreDataMember]
      public override IEnumerable<ElementValue> NamedChildren
      {
        get
        {
          foreach (var item in base.NamedChildren) yield return item;
          if (Type != null) yield return new ElementValue("type", Type);
          if (TextElement != null) yield return new ElementValue("text", TextElement);
        }
      }

      protected override bool TryGetValue(string key, out object value)
      {
        switch (key)
        {
          case "type":
            value = Type;
            return Type is not null;
          case "text":
            value = TextElement;
            return TextElement is not null;
          default:
            return base.TryGetValue(key, out value);
        };

      }

      protected override IEnumerable<KeyValuePair<string, object>> GetElementPairs()
      {
        foreach (var kvp in base.GetElementPairs()) yield return kvp;
        if (Type is not null) yield return new KeyValuePair<string,object>("type",Type);
        if (TextElement is not null) yield return new KeyValuePair<string,object>("text",TextElement);
      }

    }

    /// <summary>
    /// Business Identifier
    /// </summary>
    [FhirElement("identifier", InSummary=true, Order=90)]
    [Cardinality(Min=0,Max=-1)]
    [DataMember]
    public List<Hl7.Fhir.Model.Identifier> Identifier
    {
      get { if(_Identifier==null) _Identifier = new List<Hl7.Fhir.Model.Identifier>(); return _Identifier; }
      set { _Identifier = value; OnPropertyChanged("Identifier"); }
    }

    private List<Hl7.Fhir.Model.Identifier> _Identifier;

    /// <summary>
    /// Claim reference
    /// </summary>
    [FhirElement("request", InSummary=true, Order=100)]
    [CLSCompliant(false)]
    [References("ProcessRequest")]
    [DataMember]
    public Hl7.Fhir.Model.ResourceReference Request
    {
      get { return _Request; }
      set { _Request = value; OnPropertyChanged("Request"); }
    }

    private Hl7.Fhir.Model.ResourceReference _Request;

    /// <summary>
    /// complete | error
    /// </summary>
    [FhirElement("outcome", InSummary=true, Order=110)]
    [DeclaredType(Type = typeof(Code))]
    [DataMember]
    public Code<Hl7.Fhir.Model.RemittanceOutcome> OutcomeElement
    {
      get { return _OutcomeElement; }
      set { _OutcomeElement = value; OnPropertyChanged("OutcomeElement"); }
    }

    private Code<Hl7.Fhir.Model.RemittanceOutcome> _OutcomeElement;

    /// <summary>
    /// complete | error
    /// </summary>
    /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
    [IgnoreDataMember]
    public Hl7.Fhir.Model.RemittanceOutcome? Outcome
    {
      get { return OutcomeElement != null ? OutcomeElement.Value : null; }
      set
      {
        if (value == null)
          OutcomeElement = null;
        else
          OutcomeElement = new Code<Hl7.Fhir.Model.RemittanceOutcome>(value);
        OnPropertyChanged("Outcome");
      }
    }

    /// <summary>
    /// Disposition Message
    /// </summary>
    [FhirElement("disposition", InSummary=true, Order=120)]
    [DataMember]
    public Hl7.Fhir.Model.FhirString DispositionElement
    {
      get { return _DispositionElement; }
      set { _DispositionElement = value; OnPropertyChanged("DispositionElement"); }
    }

    private Hl7.Fhir.Model.FhirString _DispositionElement;

    /// <summary>
    /// Disposition Message
    /// </summary>
    /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
    [IgnoreDataMember]
    public string Disposition
    {
      get { return DispositionElement != null ? DispositionElement.Value : null; }
      set
      {
        if (value == null)
          DispositionElement = null;
        else
          DispositionElement = new Hl7.Fhir.Model.FhirString(value);
        OnPropertyChanged("Disposition");
      }
    }

    /// <summary>
    /// Resource version
    /// </summary>
    [FhirElement("ruleset", InSummary=true, Order=130)]
    [DataMember]
    public Hl7.Fhir.Model.Coding Ruleset
    {
      get { return _Ruleset; }
      set { _Ruleset = value; OnPropertyChanged("Ruleset"); }
    }

    private Hl7.Fhir.Model.Coding _Ruleset;

    /// <summary>
    /// Original version
    /// </summary>
    [FhirElement("originalRuleset", InSummary=true, Order=140)]
    [DataMember]
    public Hl7.Fhir.Model.Coding OriginalRuleset
    {
      get { return _OriginalRuleset; }
      set { _OriginalRuleset = value; OnPropertyChanged("OriginalRuleset"); }
    }

    private Hl7.Fhir.Model.Coding _OriginalRuleset;

    /// <summary>
    /// Creation date
    /// </summary>
    [FhirElement("created", InSummary=true, Order=150)]
    [DataMember]
    public Hl7.Fhir.Model.FhirDateTime CreatedElement
    {
      get { return _CreatedElement; }
      set { _CreatedElement = value; OnPropertyChanged("CreatedElement"); }
    }

    private Hl7.Fhir.Model.FhirDateTime _CreatedElement;

    /// <summary>
    /// Creation date
    /// </summary>
    /// <remarks>This uses the native .NET datatype, rather than the FHIR equivalent</remarks>
    [IgnoreDataMember]
    public string Created
    {
      get { return CreatedElement != null ? CreatedElement.Value : null; }
      set
      {
        if (value == null)
          CreatedElement = null;
        else
          CreatedElement = new Hl7.Fhir.Model.FhirDateTime(value);
        OnPropertyChanged("Created");
      }
    }

    /// <summary>
    /// Period covered
    /// </summary>
    [FhirElement("period", InSummary=true, Order=160)]
    [DataMember]
    public Hl7.Fhir.Model.Period Period
    {
      get { return _Period; }
      set { _Period = value; OnPropertyChanged("Period"); }
    }

    private Hl7.Fhir.Model.Period _Period;

    /// <summary>
    /// Insurer
    /// </summary>
    [FhirElement("organization", InSummary=true, Order=170)]
    [CLSCompliant(false)]
    [References("Organization")]
    [DataMember]
    public Hl7.Fhir.Model.ResourceReference Organization
    {
      get { return _Organization; }
      set { _Organization = value; OnPropertyChanged("Organization"); }
    }

    private Hl7.Fhir.Model.ResourceReference _Organization;

    /// <summary>
    /// Responsible practitioner
    /// </summary>
    [FhirElement("requestProvider", InSummary=true, Order=180)]
    [CLSCompliant(false)]
    [References("Practitioner")]
    [DataMember]
    public Hl7.Fhir.Model.ResourceReference RequestProvider
    {
      get { return _RequestProvider; }
      set { _RequestProvider = value; OnPropertyChanged("RequestProvider"); }
    }

    private Hl7.Fhir.Model.ResourceReference _RequestProvider;

    /// <summary>
    /// Responsible organization
    /// </summary>
    [FhirElement("requestOrganization", InSummary=true, Order=190)]
    [CLSCompliant(false)]
    [References("Organization")]
    [DataMember]
    public Hl7.Fhir.Model.ResourceReference RequestOrganization
    {
      get { return _RequestOrganization; }
      set { _RequestOrganization = value; OnPropertyChanged("RequestOrganization"); }
    }

    private Hl7.Fhir.Model.ResourceReference _RequestOrganization;

    /// <summary>
    /// Details
    /// </summary>
    [FhirElement("detail", InSummary=true, Order=200)]
    [Cardinality(Min=0,Max=-1)]
    [DataMember]
    public List<Hl7.Fhir.Model.PaymentReconciliation.DetailsComponent> Detail
    {
      get { if(_Detail==null) _Detail = new List<Hl7.Fhir.Model.PaymentReconciliation.DetailsComponent>(); return _Detail; }
      set { _Detail = value; OnPropertyChanged("Detail"); }
    }

    private List<Hl7.Fhir.Model.PaymentReconciliation.DetailsComponent> _Detail;

    /// <summary>
    /// Printed Form Identifier
    /// </summary>
    [FhirElement("form", InSummary=true, Order=210)]
    [DataMember]
    public Hl7.Fhir.Model.Coding Form
    {
      get { return _Form; }
      set { _Form = value; OnPropertyChanged("Form"); }
    }

    private Hl7.Fhir.Model.Coding _Form;

    /// <summary>
    /// Total amount of Payment
    /// </summary>
    [FhirElement("total", InSummary=true, Order=220)]
    [Cardinality(Min=1,Max=1)]
    [DataMember]
    public Hl7.Fhir.Model.Quantity Total
    {
      get { return _Total; }
      set { _Total = value; OnPropertyChanged("Total"); }
    }

    private Hl7.Fhir.Model.Quantity _Total;

    /// <summary>
    /// Note text
    /// </summary>
    [FhirElement("note", InSummary=true, Order=230)]
    [Cardinality(Min=0,Max=-1)]
    [DataMember]
    public List<Hl7.Fhir.Model.PaymentReconciliation.NotesComponent> Note
    {
      get { if(_Note==null) _Note = new List<Hl7.Fhir.Model.PaymentReconciliation.NotesComponent>(); return _Note; }
      set { _Note = value; OnPropertyChanged("Note"); }
    }

    private List<Hl7.Fhir.Model.PaymentReconciliation.NotesComponent> _Note;

    public override IDeepCopyable CopyTo(IDeepCopyable other)
    {
      var dest = other as PaymentReconciliation;

      if (dest == null)
      {
        throw new ArgumentException("Can only copy to an object of the same type", "other");
      }

      base.CopyTo(dest);
      if(Identifier != null) dest.Identifier = new List<Hl7.Fhir.Model.Identifier>(Identifier.DeepCopy());
      if(Request != null) dest.Request = (Hl7.Fhir.Model.ResourceReference)Request.DeepCopy();
      if(OutcomeElement != null) dest.OutcomeElement = (Code<Hl7.Fhir.Model.RemittanceOutcome>)OutcomeElement.DeepCopy();
      if(DispositionElement != null) dest.DispositionElement = (Hl7.Fhir.Model.FhirString)DispositionElement.DeepCopy();
      if(Ruleset != null) dest.Ruleset = (Hl7.Fhir.Model.Coding)Ruleset.DeepCopy();
      if(OriginalRuleset != null) dest.OriginalRuleset = (Hl7.Fhir.Model.Coding)OriginalRuleset.DeepCopy();
      if(CreatedElement != null) dest.CreatedElement = (Hl7.Fhir.Model.FhirDateTime)CreatedElement.DeepCopy();
      if(Period != null) dest.Period = (Hl7.Fhir.Model.Period)Period.DeepCopy();
      if(Organization != null) dest.Organization = (Hl7.Fhir.Model.ResourceReference)Organization.DeepCopy();
      if(RequestProvider != null) dest.RequestProvider = (Hl7.Fhir.Model.ResourceReference)RequestProvider.DeepCopy();
      if(RequestOrganization != null) dest.RequestOrganization = (Hl7.Fhir.Model.ResourceReference)RequestOrganization.DeepCopy();
      if(Detail != null) dest.Detail = new List<Hl7.Fhir.Model.PaymentReconciliation.DetailsComponent>(Detail.DeepCopy());
      if(Form != null) dest.Form = (Hl7.Fhir.Model.Coding)Form.DeepCopy();
      if(Total != null) dest.Total = (Hl7.Fhir.Model.Quantity)Total.DeepCopy();
      if(Note != null) dest.Note = new List<Hl7.Fhir.Model.PaymentReconciliation.NotesComponent>(Note.DeepCopy());
      return dest;
    }

    public override IDeepCopyable DeepCopy()
    {
      return CopyTo(new PaymentReconciliation());
    }

    ///<inheritdoc />
    public override bool Matches(IDeepComparable other)
    {
      var otherT = other as PaymentReconciliation;
      if(otherT == null) return false;

      if(!base.Matches(otherT)) return false;
      if( !DeepComparable.Matches(Identifier, otherT.Identifier)) return false;
      if( !DeepComparable.Matches(Request, otherT.Request)) return false;
      if( !DeepComparable.Matches(OutcomeElement, otherT.OutcomeElement)) return false;
      if( !DeepComparable.Matches(DispositionElement, otherT.DispositionElement)) return false;
      if( !DeepComparable.Matches(Ruleset, otherT.Ruleset)) return false;
      if( !DeepComparable.Matches(OriginalRuleset, otherT.OriginalRuleset)) return false;
      if( !DeepComparable.Matches(CreatedElement, otherT.CreatedElement)) return false;
      if( !DeepComparable.Matches(Period, otherT.Period)) return false;
      if( !DeepComparable.Matches(Organization, otherT.Organization)) return false;
      if( !DeepComparable.Matches(RequestProvider, otherT.RequestProvider)) return false;
      if( !DeepComparable.Matches(RequestOrganization, otherT.RequestOrganization)) return false;
      if( !DeepComparable.Matches(Detail, otherT.Detail)) return false;
      if( !DeepComparable.Matches(Form, otherT.Form)) return false;
      if( !DeepComparable.Matches(Total, otherT.Total)) return false;
      if( !DeepComparable.Matches(Note, otherT.Note)) return false;

      return true;
    }

    public override bool IsExactly(IDeepComparable other)
    {
      var otherT = other as PaymentReconciliation;
      if(otherT == null) return false;

      if(!base.IsExactly(otherT)) return false;
      if( !DeepComparable.IsExactly(Identifier, otherT.Identifier)) return false;
      if( !DeepComparable.IsExactly(Request, otherT.Request)) return false;
      if( !DeepComparable.IsExactly(OutcomeElement, otherT.OutcomeElement)) return false;
      if( !DeepComparable.IsExactly(DispositionElement, otherT.DispositionElement)) return false;
      if( !DeepComparable.IsExactly(Ruleset, otherT.Ruleset)) return false;
      if( !DeepComparable.IsExactly(OriginalRuleset, otherT.OriginalRuleset)) return false;
      if( !DeepComparable.IsExactly(CreatedElement, otherT.CreatedElement)) return false;
      if( !DeepComparable.IsExactly(Period, otherT.Period)) return false;
      if( !DeepComparable.IsExactly(Organization, otherT.Organization)) return false;
      if( !DeepComparable.IsExactly(RequestProvider, otherT.RequestProvider)) return false;
      if( !DeepComparable.IsExactly(RequestOrganization, otherT.RequestOrganization)) return false;
      if( !DeepComparable.IsExactly(Detail, otherT.Detail)) return false;
      if( !DeepComparable.IsExactly(Form, otherT.Form)) return false;
      if( !DeepComparable.IsExactly(Total, otherT.Total)) return false;
      if( !DeepComparable.IsExactly(Note, otherT.Note)) return false;

      return true;
    }

    [IgnoreDataMember]
    public override IEnumerable<Base> Children
    {
      get
      {
        foreach (var item in base.Children) yield return item;
        foreach (var elem in Identifier) { if (elem != null) yield return elem; }
        if (Request != null) yield return Request;
        if (OutcomeElement != null) yield return OutcomeElement;
        if (DispositionElement != null) yield return DispositionElement;
        if (Ruleset != null) yield return Ruleset;
        if (OriginalRuleset != null) yield return OriginalRuleset;
        if (CreatedElement != null) yield return CreatedElement;
        if (Period != null) yield return Period;
        if (Organization != null) yield return Organization;
        if (RequestProvider != null) yield return RequestProvider;
        if (RequestOrganization != null) yield return RequestOrganization;
        foreach (var elem in Detail) { if (elem != null) yield return elem; }
        if (Form != null) yield return Form;
        if (Total != null) yield return Total;
        foreach (var elem in Note) { if (elem != null) yield return elem; }
      }
    }

    [IgnoreDataMember]
    public override IEnumerable<ElementValue> NamedChildren
    {
      get
      {
        foreach (var item in base.NamedChildren) yield return item;
        foreach (var elem in Identifier) { if (elem != null) yield return new ElementValue("identifier", elem); }
        if (Request != null) yield return new ElementValue("request", Request);
        if (OutcomeElement != null) yield return new ElementValue("outcome", OutcomeElement);
        if (DispositionElement != null) yield return new ElementValue("disposition", DispositionElement);
        if (Ruleset != null) yield return new ElementValue("ruleset", Ruleset);
        if (OriginalRuleset != null) yield return new ElementValue("originalRuleset", OriginalRuleset);
        if (CreatedElement != null) yield return new ElementValue("created", CreatedElement);
        if (Period != null) yield return new ElementValue("period", Period);
        if (Organization != null) yield return new ElementValue("organization", Organization);
        if (RequestProvider != null) yield return new ElementValue("requestProvider", RequestProvider);
        if (RequestOrganization != null) yield return new ElementValue("requestOrganization", RequestOrganization);
        foreach (var elem in Detail) { if (elem != null) yield return new ElementValue("detail", elem); }
        if (Form != null) yield return new ElementValue("form", Form);
        if (Total != null) yield return new ElementValue("total", Total);
        foreach (var elem in Note) { if (elem != null) yield return new ElementValue("note", elem); }
      }
    }

    protected override bool TryGetValue(string key, out object value)
    {
      switch (key)
      {
        case "identifier":
          value = Identifier;
          return Identifier?.Any() == true;
        case "request":
          value = Request;
          return Request is not null;
        case "outcome":
          value = OutcomeElement;
          return OutcomeElement is not null;
        case "disposition":
          value = DispositionElement;
          return DispositionElement is not null;
        case "ruleset":
          value = Ruleset;
          return Ruleset is not null;
        case "originalRuleset":
          value = OriginalRuleset;
          return OriginalRuleset is not null;
        case "created":
          value = CreatedElement;
          return CreatedElement is not null;
        case "period":
          value = Period;
          return Period is not null;
        case "organization":
          value = Organization;
          return Organization is not null;
        case "requestProvider":
          value = RequestProvider;
          return RequestProvider is not null;
        case "requestOrganization":
          value = RequestOrganization;
          return RequestOrganization is not null;
        case "detail":
          value = Detail;
          return Detail?.Any() == true;
        case "form":
          value = Form;
          return Form is not null;
        case "total":
          value = Total;
          return Total is not null;
        case "note":
          value = Note;
          return Note?.Any() == true;
        default:
          return base.TryGetValue(key, out value);
      };

    }

    protected override IEnumerable<KeyValuePair<string, object>> GetElementPairs()
    {
      foreach (var kvp in base.GetElementPairs()) yield return kvp;
      if (Identifier?.Any() == true) yield return new KeyValuePair<string,object>("identifier",Identifier);
      if (Request is not null) yield return new KeyValuePair<string,object>("request",Request);
      if (OutcomeElement is not null) yield return new KeyValuePair<string,object>("outcome",OutcomeElement);
      if (DispositionElement is not null) yield return new KeyValuePair<string,object>("disposition",DispositionElement);
      if (Ruleset is not null) yield return new KeyValuePair<string,object>("ruleset",Ruleset);
      if (OriginalRuleset is not null) yield return new KeyValuePair<string,object>("originalRuleset",OriginalRuleset);
      if (CreatedElement is not null) yield return new KeyValuePair<string,object>("created",CreatedElement);
      if (Period is not null) yield return new KeyValuePair<string,object>("period",Period);
      if (Organization is not null) yield return new KeyValuePair<string,object>("organization",Organization);
      if (RequestProvider is not null) yield return new KeyValuePair<string,object>("requestProvider",RequestProvider);
      if (RequestOrganization is not null) yield return new KeyValuePair<string,object>("requestOrganization",RequestOrganization);
      if (Detail?.Any() == true) yield return new KeyValuePair<string,object>("detail",Detail);
      if (Form is not null) yield return new KeyValuePair<string,object>("form",Form);
      if (Total is not null) yield return new KeyValuePair<string,object>("total",Total);
      if (Note?.Any() == true) yield return new KeyValuePair<string,object>("note",Note);
    }

  }

}

// end of file