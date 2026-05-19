# Master Workspace Asset-Class Constitution

## Purpose

This note defines the asset-class posture of the master workspace root.

It exists to keep four authorities distinct:

- lab formation authority
- derived payload authority
- runtime-admitted substrate authority
- public-readable derivative authority

The governing invariant is:

> source authority, formation authority, runtime authority, and public
> availability must not collapse into one undifferentiated space

## Workspace Classes

The master workspace now carries four bounded asset classes.

### `LabFormationAsset`

`LabFormationAsset` is:

- source-authoritative
- lab-governed
- non-public by default
- formative rather than runtime-owning

This class is the default home for:

- `RootAtlas`-adjacent source assets
- pre-`GEL` formation assets
- source-authority predicate atlases
- formative templating assets

The default governed surface for this class is:

- `Lab SaaS Assets/`

### `DerivedPayload`

`DerivedPayload` is:

- emitted from lab-governed assets
- signed and versioned before local install use
- bounded to declared derivation purpose
- not equivalent to source authority

This class is the bridge form between lab-governed source assets and local
runtime-admitted substrates.

### `RuntimeAdmittedSubstrate`

`RuntimeAdmittedSubstrate` is:

- local
- hydrated from bounded derived inputs
- lawful for build-line/runtime use
- not the same thing as remote source authority

This class is the correct reading for local install-side substrates such as:

- `Sanctuary.GEL`

### `PublicReadableDerivative`

`PublicReadableDerivative` is:

- outward-facing
- non-formative
- non-authoritative
- safe for explanation, witness, or bounded operational readout

This class must not grant raw formation, templating, or source-authority power.

## Build-Line Posture

The line-local `build/` folders remain:

- build-policy surfaces
- verification surfaces
- executable truth surfaces for their respective lines

They are not the default home for:

- `RootAtlas`
- raw pre-`GEL` formation assets
- source-authority predicate atlases
- ungated formative templating assets

`OAN Mortalis V1.1.1/` and `OAN Mortalis V1.2.1/` may consume only bounded
derived inputs where the local line requires them.

## Canonical Bridge Pattern

The canonical bridge pattern for source-authority hydration is now:

```text
LabFormationAsset -> DerivedPayload -> RuntimeAdmittedSubstrate
```

For `V1.2.1`, that means:

- `RootAtlas` remains lab-side and remote
- signed/versioned payloads are derived from `RootAtlas`
- `Sanctuary.GEL` is the first lawful local substrate

`Sanctuary.GEL` is therefore:

- `RuntimeAdmittedSubstrate`

and not:

- remote source authority

`RootAtlas` is therefore:

- `LabFormationAsset`

and not:

- local install substrate

## HTTP Bridge Posture

When a future HTTP bridge is introduced, it must:

- resolve against master-workspace targets
- access `Lab SaaS Assets/` only through configured, bounded target classes
- refuse raw unsafe path expansion
- refuse research-source impersonation through ordinary runtime paths
- refuse ungated public templating access

No future bridge may collapse the distinction between:

- lab-governed source assets
- derived payloads
- local runtime-admitted substrates
- public-readable derivatives

## Working Summary

The repository root is the master workspace.

`Lab SaaS Assets/` is the default source/formation authority surface.

`OAN Mortalis V1.1.1/` and `OAN Mortalis V1.2.1/` remain executable build
lines that may prove and consume runtime truth, but they do not own raw lab
formation authority by default.
