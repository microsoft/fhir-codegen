FHIR Package Resolution:

## General Notes

Finding FHIR packages is unfortunately complex.  Below is a set of resolution rules based on various entry points.

## CI Version of Core Package

Notes:
* Cache directive resolves as `#current`, e.g., `hl7.fhir.r6.core#current`.

```mermaid
flowchart TD
    a["Core FHIR\nCurrent"]
    a --> a1

    a1["Fixed URL:\nhttp://build.fhir.org/"]
    a1 --> common01
    
    b["Core FHIR\nBranch"]
    b --> b1

    b1["Construct URL:\nhttp://build.fhir.org/branches/{input}"]
    b1 --> common01
    
    common01["Download `version.info`"]
    common01 --> common02
    
    common02["Check for local package"]
    common02 -- yes -->common03
    common02 -- no -->common10
    
    common03["Grab cached build date"]
    common03 --> common04
    
    common04["Compare build dates"]
    common04 -- Local is new enough -->common99
    common04 -- CI is newer -->common10
    
    common10["Build package name:\n{qualified name}.tgz"]
    common10 --> common11
    
    common11["Delete local contents"]
    common11 --> common12

    common12["Download package"]    
    common12 --> common13

    common13["Extract"]
    common13 --> common99

    common99["Done"]
```


## IG CI Build

Notes:
* I do not know of a method of resolving a package name into a branch name, which means you always have to start with a branch name or URL.
* Main branch cache directive resolves as `#current`, e.g., `hl7.fhir.us.core#current`.
* Named branches resolve as `#current${branchName}`, e.g., `hl7.fhir.us.core#current$FHIR-xxx`

```mermaid
flowchart TD
    a[IG Directive]
    a --> a1

    a1["Construct root URL:\nhttp://build.fhir.org/ig/{directive}"]
    a1 --> common01
    
    b[IG Directive and Branch]
    b --> b1
    
    b1["Construct root URL:\nhttp://build.fhir.org/ig/{directive}/branches/{branch}"]
    b1 --> common01
    
    c[Full URL]
    c --> c1
    
    c1["Construct root URL:\nStrip .html, etc."]
    c1 --> common01
        
    common01["Download `package.manifest.json`"]
    common01 --> common02a
    
    common02a["Determine local directive\n(as noted above)"]
    common02a --> common02
    
    common02["Check for local package"]
    common02 -- yes -->common03
    common02 -- no -->common10
    
    common03["Grab cached build date"]
    common03 --> common04
    
    common04["Compare build dates"]
    common04 -- Local is new enough -->common99
    common04 -- CI is newer -->common10
    
    common10["Build package url:\n{resolved root url}/package.tgz"]
    common10 --> common11
    
    common11["Delete local contents"]
    common11 --> common12

    common12["Download package"]    
    common12 --> common13

    common13["Extract"]
    common13 --> common99

    common99["Done"]
```

## Published Package With Explicit Version

Notes:
* Some packages now have FHIR-version specific sub-packages

```mermaid
flowchart TD
    a["Full Directive, generic, e.g.:\nhl7.fhir.uv.subscriptions-backport#1.1.0"]
    a --> common1
    
    b["Full Directive with FHIR version, e.g.,:\nhl7.fhir.uv.subscriptions-backport.r4#1.1.0"]
    b --> common1

    common1["Check for local package"]
    common1 -- yes -->common99
    common1 -- "no: unknown FHIR version" -->common02a
    common1 -- "no: known FHIR version" -->common02b

    common02a["Ask *each* package server for package versions\n.../{package name}\n.../{package name}.r4\n.../{package name}.r4b\n.../{package name}.r5..."]
    common02a --> common03

    common02b["Ask *each* package server for package versions\n.../{package name}"]
    common02b --> common03
    
    common03["Parse version info and sort:\n- dist-tags: latest\n- version date\n- server preference"]
    common03 --> common10
        
    common10["Grab tarball URL"]
    common10 --> common12
    
    common12["Download package"]    
    common12 --> common13

    common13["Extract"]
    common13 --> common99

    common99["Done"]
```


## Published Package Without Explicit Version

Notes:
* Some packages now have FHIR-version specific sub-packages

```mermaid
flowchart TD
    a["Name only, generic, e.g.:\nhl7.fhir.uv.subscriptions-backport"]
    a --> common01
    
    b["Name only with FHIR version, e.g.,:\nhl7.fhir.uv.subscriptions-backport.r4"]
    b --> common01

    common01["Query *each* package server\n.../catalog?op=find&name=..."]
    common01 -- "Has Version and latest tag" --> common02a
    common01 -- "Otherwise: known FHIR version" --> common02b
    common01 -- "Otherwise: unknown FHIR version" --> common02c
    
    common02a["Sort results:\n- FHIR Version\n- Package Version*\n- Server order"]
    common02a --> common03

    common02b["Ask for package versions for each package\n.../{package name}"]
    common02b --> common02a

    common02c["Ask for package versions for each package\n.../{package name}\n.../{package name}.r4\n.../{package name}.r4b\n.../{package name}.r5"]
    common02c --> common02a

    common03["Parse version info and sort:\n- dist-tags: latest\n- version date\n- server preference"]
    common03 --> common10
        
    common10["Grab tarball URL"]
    common10 --> common12
    
    common12["Download package"]    
    common12 --> common13

    common13["Extract"]
    common13 --> common99

    common99["Done"]
```
