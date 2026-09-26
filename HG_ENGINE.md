# HG-engine save editing

Clone `the-new-kai-lu/PKHeX` beside this repository. This fork builds against that source checkout by default; it does not use the retail PKHeX.Core NuGet package. A different checkout location can be supplied with `-p:PKHeXSourcePath=/absolute/path/to/PKHeX.Core/PKHeX.Core.csproj`. CI checks out a pinned revision of the same fork.

Open/export saves normally. The matching closest-stock hg-engine save profile is detected automatically, and a notice identifies the custom ROM. All eleven custom species are available for editing, with their names, stats, types, abilities and growth rates. Original retail saves still follow their original behavior. Custom species currently use the unknown-species sprite preview.

Automatic retail legalization is disabled for these saves because retail encounter rules cannot validate this ROM. Custom evolution mechanics should be exercised in-game; the retail evolution shortcut is not offered for custom species.

Supported format: 18 boxes, 0xFDB0 general block, storage at 0xFE00, 0x700-byte Dex. Older 30-box/expanded-bag hg-engine saves are a different profile and are not automatically migrated. The corresponding PKHeX fork documents byte preservation and format checks in `HG_ENGINE.md`.

Local verification: `dotnet format`, Web Debug build and test-project Debug build. Repository policy leaves test execution to GitHub Actions; the build-and-test workflow runs on main and checks out the matching PKHeX fork. Upstream's production deployment workflow is restricted to its original repository so pushing this fork does not require the upstream Azure/Pages secrets.
