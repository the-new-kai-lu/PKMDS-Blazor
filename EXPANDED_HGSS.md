# Expanded HGSS campaign saves

This fork consumes the sibling `the-new-kai-lu/PKHeX` source checkout through
`Directory.Build.targets`; it does not substitute the retail NuGet package.
The build-and-test workflow pins matching Core revision
`b7c732fd64d80f77ac947c5ec101b6ba3f2b9de0`. A custom checkout
location can be supplied with `-p:PKHeXSourcePath=/absolute/path/to/PKHeX.Core.csproj`.

The shared Core implements the opt-in Expanded HGSS campaign format separately
from vanilla HGSS and hg-engine. Its versioned 2 KiB allocation contains separate
Emerald and Platinum story-variable/flag namespaces. Standard Pokémon records
remain standard; unrelated edits must retain both campaign allocations and
opaque/reserved bytes.

Open and export through the usual controls. A notice and the Save File Info
dialog identify this format. Use the matching expanded game and editor forks;
unmodified games/editors are not compatibility targets for these files.
Unsupported tagged versions are rejected instead of silently exported without
their campaign state. No automatic or in-place migration is offered.

Automatic retail legalization is disabled for this format because retail
encounter rules do not describe imported campaign encounters. Manual Pokémon
editing remains available. The existing vanilla and hg-engine paths remain
separate.

This is in-development format support. Synthetic Core preservation tests are
not proof that a native game created or reloaded a save, and are not a substitute
for an actual application export/reopen test. Native-build and GUI validation
must be recorded separately before release.

The Web Debug build and test-project Debug build pass. The shared Core's 89
focused synthetic/regression cases pass; those tests run in the Core repository.
This repository adds loader/preservation, format-label and legalization-safety
tests, but does not execute them locally under its CI-only test policy.

Local repository policy permits formatting and Debug builds, including building
the test project; test execution is left to CI. Do not clear site data, delete
IndexedDB backups, or replace a user's original save while testing this support.