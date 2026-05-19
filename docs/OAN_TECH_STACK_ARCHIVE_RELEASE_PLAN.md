# OAN Tech Stack Archive Release Plan

Date: 2026-05-19
Branch: `archive/oan-tech-stack-root-fold-20260519`

## Release Intent

Fold the OAN Tech Stack into Codex Mirror as an inspectable archive body that lets external readers run, inspect, and pressure-test the current governed cognition instrument.

The release target is a playable research harness, not an autonomous agent release.

## Archive Placement

The OAN root is preserved under:

```text
OAN-Tech-Stack/
```

This avoids overwriting the existing Codex Mirror root identity while preserving the OAN repository's own root files, folders, wrappers, docs, and line structure inside the archive.

## Included

- public repository governance files
- root build and test wrappers
- public docs and examples
- `Build Contracts`
- `datasets`
- `tools`
- `Modules` minus local generated telemetry and oversized local binaries
- `OAN Mortalis V1.1.1` retained-parent source
- `OAN Mortalis V1.2.1` current runnable-truth source
- SLI.Lisp membrane files
- audit test suites
- line manifests and build policy files

## Excluded

- `.git`
- `.audit`
- `runtime`
- `Lab SaaS Assets`
- generated preflight artifact receipts containing local machine paths
- SymbolicCryptic generated telemetry sidecars containing local machine paths
- build outputs (`bin`, `obj`, `TestResults`)
- local IDE settings
- oversized local binary payloads such as `Antigravity.exe`
- model/container artifacts
- private corpus and local runtime lanes

## Publication Claims

Allowed claim:

```text
This is an inspectable governed cognition instrumentation harness for testing rehearsal, residue, refusal, review, and non-collapse boundaries.
```

Refused claims:

```text
This is not CME.Actual.
This is not Sanctuary.Actual.
This is not an autonomous mind.
This is not a diagnostic system.
This is not an action-authorized agent.
This is not a personification proof.
```

## Safe Documentation Tasks

1. Add a root README link to `OAN-Tech-Stack/PLAYTEST_README.md`.
2. Add a short release note explaining `1.3.18` as current runnable truth.
3. Add a minimal "first ride" walkthrough that runs the test suite and points to key boundary tests.
4. Add a non-claims ledger for public readers.
5. Add a redaction/hygiene note explaining excluded local lanes.

## Required Verification Before Push

1. Confirm no copied file exceeds GitHub's normal file size limits.
2. Confirm no build outputs are staged.
3. Confirm no local absolute paths remain except scanner configuration strings.
4. Run `dotnet test ".\OAN-Tech-Stack\OAN Mortalis V1.2.1\San.sln"`.
5. Run `git status --short` and review staged scope.
6. Commit on the archive branch only.
7. Push the branch for review before merging to `main`.

