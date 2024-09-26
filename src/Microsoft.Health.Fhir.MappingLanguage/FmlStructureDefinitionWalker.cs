/* 
 * Copyright (c) 2019, Firely (info@fire.ly) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/FirelyTeam/firely-net-sdk/master/LICENSE
 */


#nullable enable

using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Introspection;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification;
using Hl7.Fhir.Specification.Navigation;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Utility;
using T = System.Threading.Tasks;

namespace Microsoft.Health.Fhir.MappingLanguage
{
    /// <summary>
    /// This class implements basic functions to walk deeper into a StructureDefinition, 
    /// using a child name, disambiguating on type, navigating across references or directly
    /// to expected extensions. This functionality mirrors the potential navigation concepts
    /// on a discriminator, as documented in http://hl7.org/fhir/profiling.html#discriminator.
    ///
    /// NOTE: This is a direct port of the Firely routine, except that it can walk into the primitive value property
    ///       (which is required in the FML maps)
    /// </summary>
    public class FmlStructureDefinitionWalker
    {
        public IResourceResolver? Resolver => _resolver as IResourceResolver;
        public IAsyncResourceResolver AsyncResolver => _resolver.AsAsync();

#pragma warning disable CS0618 // Type or member is obsolete
        private readonly ISyncOrAsyncResourceResolver _resolver;
#pragma warning restore CS0618 // Type or member is obsolete

        public readonly ElementDefinitionNavigator Current;

        /// <summary>
        /// When walked into a Reference, the targetprofiles are copied to here
        /// </summary>
        private readonly string[]? _targetProfile;

#pragma warning disable CS0618 // Type or member is obsolete
        public FmlStructureDefinitionWalker(StructureDefinition sd, ISyncOrAsyncResourceResolver resolver)
#pragma warning restore CS0618 // Type or member is obsolete
            : this(ElementDefinitionNavigator.ForSnapshot(sd), resolver)
        {
            // nothing more
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public FmlStructureDefinitionWalker(ElementDefinitionNavigator element, ISyncOrAsyncResourceResolver resolver)
#pragma warning restore CS0618 // Type or member is obsolete
        {
            Current = element.ShallowCopy();
            _resolver = resolver;

            // Make sure there is always a current item
            if (Current.AtRoot) Current.MoveToFirstChild();
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public FmlStructureDefinitionWalker(ElementDefinitionNavigator element, IEnumerable<string> targetProfiles, ISyncOrAsyncResourceResolver resolver) :
#pragma warning restore CS0618 // Type or member is obsolete
            this(element, resolver)
        {
            _targetProfile = targetProfiles?.ToArray();
        }

        public FmlStructureDefinitionWalker(FmlStructureDefinitionWalker other)
        {
            Current = other.Current.ShallowCopy();
            _resolver = other._resolver;
        }

        public FmlStructureDefinitionWalker FromCanonical(string canonical, IEnumerable<string>? targetProfiles = null) =>
            TaskHelper.Await(() => FromCanonicalAsync(canonical, targetProfiles));

        public async T.Task<FmlStructureDefinitionWalker> FromCanonicalAsync(string canonical, IEnumerable<string>? targetProfiles = null)
        {
            var sd = await AsyncResolver.FindStructureDefinitionAsync(canonical).ConfigureAwait(false);
            if (sd == null)
                throw new StructureDefinitionWalkerException($"Cannot walk into unknown StructureDefinition with canonical '{canonical}' at '{Current.CanonicalPath()}'");

            return (targetProfiles is not null)
                ? new FmlStructureDefinitionWalker(ElementDefinitionNavigator.ForSnapshot(sd), targetProfiles, _resolver)
                : new FmlStructureDefinitionWalker(ElementDefinitionNavigator.ForSnapshot(sd), _resolver);
        }

        /// <summary>
        /// Returns a new walker that represents the definition for the given child.
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when there is no childName given.</exception>
        public FmlStructureDefinitionWalker Child(string childName)
        {
            if (childName == null) throw Error.ArgumentNull(nameof(childName));

            var definitions = childDefinitions(childName).ToList();

            if (definitions.Count == 0)
                throw new Hl7.Fhir.Specification.StructureDefinitionWalkerException($"Cannot walk into unknown child '{childName}' at '{Current.CanonicalPath()}'.");
            else if (definitions.Count == 1) // Single element, no slice
                return new FmlStructureDefinitionWalker(definitions.Single(), _resolver);
            else if (definitions.Count == 2) // element with an entry + single slice
                return new FmlStructureDefinitionWalker(definitions[1], _resolver);
            else
                throw new Hl7.Fhir.Specification.StructureDefinitionWalkerException($"Child with name '{childName}' is sliced to more than one choice and cannot be used as a discriminator at '{Current.CanonicalPath()}' ");
        }

        private IEnumerable<ElementDefinitionNavigator> childDefinitions(string? childName = null)
        {
            var canonicals = Current.Current.Type.Select(t => t.GetTypeProfile()).Distinct().ToArray();
            // if (canonicals.Length > 1)
            //    throw new Hl7.Fhir.Specification.StructureDefinitionWalkerException($"Cannot determine which child to select, since there are multiple paths leading from here ('{Current.CanonicalPath()}'), use 'ofType()' to disambiguate");

            // Take First(), since we have determined above that there's just one distinct result to expect.
            // (this will be the case when Type=R
            var expandedCol = Expand();
            foreach (var expanded in expandedCol)
            {
                var nav = expanded.Current.ShallowCopy();

                if (!nav.MoveToFirstChild()) yield break;

                do
                {
                    // if (nav.Current.IsPrimitiveValueConstraint()) continue;      // ignore value attribute
                    if (childName != null && nav.Current.MatchesName(childName)) yield return nav.ShallowCopy();
                }
                while (nav.MoveToNext());
            }
        }


        /// <summary>
        /// Returns a set of walkers containing the children of the current node.
        /// </summary>
        /// <returns></returns>
        /// <remarks>There are three cases:
        /// 1. If the walker contains an ElementDefinition with children, it returns itself. 
        /// 2. If the ElementDefinition has a NameReference, it returns the node referred to by the namereference.
        /// 3. If not 1 or 2, it returns a set of walkers representing the type(s) the ElementDefinition refers to.
        /// 
        /// This order ensures that local ("inline") constraints for the children in the snapshot take
        /// precedence over following the type.profile link.
        /// </remarks>
        public IEnumerable<FmlStructureDefinitionWalker> Expand()
        {
            if (Current.HasChildren)
                return new[] { this };
            else if (Current.Current.ContentReference != null)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (!TryFollowContentReference(Current, s => Resolver.FindStructureDefinition(s), out var reference))
                    throw new Hl7.Fhir.Specification.StructureDefinitionWalkerException($"The contentReference '{reference}' cannot be resolved.");
#pragma warning restore CS0618 // Type or member is obsolete

                return new[] { new FmlStructureDefinitionWalker(reference!, _resolver) };
            }
            else if (Current.Current.Type.Count >= 1)
            {
                return Current.Current.Type
                    .GroupBy(t => t.GetTypeProfile(), t => t.TargetProfile)
                    .Select(group => FromCanonical(group.Key!, group.SelectMany(g => g))); // no use returning multiple "reference" profiles when they only differ in targetReference
            }

            throw new Hl7.Fhir.Specification.StructureDefinitionWalkerException("Invalid StructureDefinition: element misses either a type reference or " +
                $"a value in ElementDefinition.contentReference at '{Current.CanonicalPath()}'");
        }

        /// <summary>
        /// Resolve a the contentReference in a navigator and returns a navigator that is located on the target of the contentReference.
        /// </summary>
        /// <remarks>The current navigator must be located at an element that contains a contentReference.</remarks>
        public static bool TryFollowContentReference(ElementDefinitionNavigator sourceNavigator, Func<string, StructureDefinition?> resolver, [NotNullWhen(true)] out ElementDefinitionNavigator? targetNavigator)
        {
            targetNavigator = null;

            var reference = sourceNavigator.Current.ContentReference;
            if (reference is null) return false;

            var profileRef = ProfileReference.Parse(reference);

            if (profileRef.IsAbsolute && profileRef.CanonicalUrl != sourceNavigator.StructureDefinition.Url)
            {
                // an external reference (e.g. http://hl7.org/fhir/StructureDefinition/Questionnaire#Questionnaire.item)

                var profile = resolver(profileRef.CanonicalUrl!);
                if (profile is null) return false;
                targetNavigator = ElementDefinitionNavigator.ForSnapshot(profile);
            }
            else
            {
                // a local reference
                targetNavigator = sourceNavigator.ShallowCopy();
            }

            return targetNavigator.JumpToNameReference("#" + profileRef.ElementName);
        }
    };

    internal class ProfileReference
    {
        private ProfileReference(string url)
        {
            if (url == null) { throw new ArgumentNullException(nameof(url)); }

            var parts = url.Split('#');

            if (parts.Length == 1)
            {
                // Just the canonical, no '#' present
                CanonicalUrl = parts[0];
                ElementName = null;
            }
            else
            {
                // There's a '#', so both or just the element are present
                CanonicalUrl = parts[0].Length > 0 ? parts[0] : null;
                ElementName = parts[1].Length > 0 ? parts[1] : null;
            }
        }

        /// <summary>Initialize a new <see cref="ProfileReference"/> instance from the specified url.</summary>
        /// <param name="url">A resource reference to a profile.</param>
        /// <returns>A new <see cref="ProfileReference"/> structure.</returns>
        public static ProfileReference Parse(string url) => new(url);

        /// <summary>Returns the canonical url of the profile.</summary>
        public string? CanonicalUrl { get; }

        /// <summary>Returns an optional profile element name, if included in the reference.</summary>
        public string? ElementName { get; }

        /// <summary>Returns <c>true</c> if the profile reference includes an element name, <c>false</c> otherwise.</summary>
        public bool IsComplex => ElementName is not null;

        /// <summary>
        /// Returns <c>true</c> of the profile reference includes a canonical url.
        /// </summary>
        public bool IsAbsolute => CanonicalUrl is not null;
    }
}

#nullable restore
