# About

This project is intended to allow for easy use of FHIR specification packages.  The FHIR specification (both core and implementation guides) are defined via FHIR models.  FHIR definitional models are detailed computable structures for (e.g.):
* defining and supporting the publication of the specifications,
* consumption by applications to control behavior,
* etc.

Because of the targets of these structures, they are often are not too "friendly" for downstream projects or usage.

Additionally, the FHIR specification has, and continues to, evolve over time.

To that end, this project attempts to normalize the definitional aspects of FHIR into more developer-friendly models and abstract many of the internal changes that occur within the specifications.

# Navigation

Articles about the projects or major topics are in the navigation links (either side-bar or as 'Table of Contents').

API documentation is automatically generated from the source code and available [here](/fhir-codegen/api/index.html).


# History / Development FAQs 

## Why C#?

The decision was somewhat arbitrary - when building tooling intended to be used in multiple language pipelines, one has to be chosen.  I enjoy working in C# (both the language itself and associated tooling), it is performant, and it is cross-platform.

## Documentation Site

[DocFX](https://dotnet.github.io/docfx/) is used to generate this documentation site.  Articles are written in Markdown in this repo, while the API documentation is generated via the XMLDoc comments in the source code.

[GitHub Pages](https://pages.github.com/) is used to host this documentation site.