# pokeheartgold Fakemon save editing

## Goal and branch

The `fakemon-stock` branch supports the separate stock-based pokeheartgold ROM. It retains the existing hg-engine profile, while `main` remains the hg-engine editor branch. Build against the sibling PKHeX checkout on its matching `fakemon-stock` branch. CI pins the matching PKHeX commit.

## Changes

Opening a save from the modified ROM automatically selects its version-1 profile. The app exposes all eleven custom species (1076–1086), their stats, types, abilities, breeding and growth data, and the seven compact-ID moves. Move names, PP, type icons, damage categories and descriptions use the port's mapping. The original 493 species retain stock HGSS personal data. The save retains the original 512 KiB layout and 18 boxes.

| Saved ID | Move |
| --- | --- |
| 468 | Wild Charge |
| 469 | Snarl |
| 470 | Incinerate |
| 471 | Fire Lash |
| 472 | Icicle Crash |
| 473 | Bulldoze |
| 474 | Hurricane |

The profile is distinct from hg-engine: these move IDs and save layout differ. No automatic conversion between ROM profiles is implemented. All eleven custom species have bundled normal and shiny portraits for party, box, form-picker and trade previews. HOME, game-style and embedded sprite modes resolve these local overrides before looking up a public image. Custom Pokédex caught, seen and gender bits are preserved; custom language/form records have no storage slot. Retail automatic legalization and move suggestions are disabled for custom data; edit the desired values directly.

The PKHeX core recognizes the marker `FK` in Pokédex language padding (general offsets 0x15EA–0x15EB), plus version 1 at 0x15F7. The ROM writes this marker. Unmarked retail saves remain retail until opened and saved by the modified ROM. Standalone `.pk4` files do not contain a ROM profile marker; view them in the matching save before editing compact moves.

## Validation and manual end-to-end checks

Local verification uses `dotnet format` and Debug Web/test-project builds. Repository policy reserves test execution for GitHub Actions. Automated cases cover the added moves' names, types, categories and PP, sparse species selection, retail personal data, and protection against retail move suggestions. Formatting, the Web Debug build and the test-project Debug build passed. CI run [36285068803](https://github.com/the-new-kai-lu/PKMDS-Blazor/actions/runs/36285068803) passed all 475 tests. A subsequent headless HeartGold/editor save round trip verified the actual in-game save (both block counters advanced 2847→2848), valid CRCs, party Voltuff and all eleven custom box entries with compact moves. That test exposed and fixed an inherited PKHeX loader mutation at offset 0x1C; this branch pins the corrected core. Interactive browser acceptance and the wider in-game behavior checklist below remain pending.

For emulator and browser acceptance testing:

1. Save in the modified ROM, open that save in this branch, and verify the pokeheartgold Fakemon notice appears. A retail save must not show it; an hg-engine save must show its separate notice.
2. Insert each custom species into a party or box slot. Check names, typing, abilities, level, six stats, gender, nature, EVs/IVs, egg status and shiny flag. Save and reopen the export.
3. Select each of the seven moves. Confirm name/type/category, set PP Ups to 0 and 3, heal PP, then export. The saved IDs must remain 468–474.
4. Load the export in the matching modified ROM. Inspect summary screens, withdraw/deposit creatures, use added moves, save in-game, and reopen that save in this editor. Confirm species, moves, PP, stats, inventory, party/boxes and Dex flags survived.
5. Repeat with canonical Pokémon, Deoxys forms, both genders, and a retail save opened separately. Custom Dex bits must not overwrite Deoxys form history.
6. Check actual evolution in-game: level 16/49, EXP curve conversion, held-Icicle-Plate Ice Fang substitution, and the same-KO NeverMeltIce rare evolution. Editor legality/evolution shortcuts cannot verify these mechanics.

These instructions describe acceptance behavior; a successful build alone does not establish emulator or browser end-to-end success.

## Local artwork

`Pkmds.Core/Utilities/LocalPokemonSprites.cs` maps custom species IDs to local
PNG files under `Pkmds.Rcl/wwwroot/sprites/fakemon/`. Add an entry and its normal
and shiny files to support additional locally designed species. Retail Pokémon
keep their existing image sources.

The current portraits are the approved 80×80 battle sprites, not newly generated
HOME-style models. Regenerate them from the adjacent game checkout with:

```sh
python3 tools/export-fakemon-sprites.py --source ../pokeheartgold/files/fakemon
```

Pillow is required. The exporter preserves palette indices, uses the game's
normal/shiny RGB555 palettes and makes index 0 transparent. `manifest.json`
records input and output hashes. No external image host is needed for these 22
images; they are also covered by the existing PWA static-asset manifest.

Validation: Debug Web and Tests projects build without warnings; formatting
passes. Browser testing loaded a separate test save containing all 22 custom
normal/shiny variants in the box and the three starters in the party. Every
custom image loaded from the app origin; Pikachu still loaded from the existing
HOME CDN. URL regression tests run in CI (not locally, per AGENTS.md).

After updating a running local instance, restart `dotnet run` and hard-refresh
the browser. If an installed PWA still shows stale artwork, unregister only its
service worker; preserve IndexedDB save backups and the Pokémon Bank.
