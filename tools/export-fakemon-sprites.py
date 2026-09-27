#!/usr/bin/env python3
"""Export existing approved HGSS front frames as local web PNGs (Pillow required).

This is native-asset format conversion: no creature redesign or image generation.
Normal/shiny RGB555 palettes come from the game's actual battle resources.
"""
import argparse
import hashlib
import json
import struct
from pathlib import Path
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, default=ROOT.parent / "pokeheartgold/files/fakemon")
    parser.add_argument("--output", type=Path, default=ROOT / "Pkmds.Rcl/wwwroot/sprites/fakemon")
    args = parser.parse_args()
    metadata = json.loads((args.source / "species.json").read_text())
    species = sorted((mon for family in metadata["lines"] for mon in family["designs"]), key=lambda mon: mon["engine_species_id"])
    report = {"source": "pokeheartgold/files/fakemon", "method": "First 80x80 approved front frame, using native normal/shiny RGB555 battle palettes; transparent index zero", "images": []}
    for mon in species:
        folder = args.source / mon["key"]
        source = folder / "source/male/front.png"
        with Image.open(source) as image:
            assert image.mode == "P" and image.size == (160, 80), source
            frame = image.crop((0, 0, 80, 80))
        for shiny in (False, True):
            palette_file = folder / f"battle-{5 if shiny else 4}.bin"
            data = palette_file.read_bytes()
            assert len(data) == 72 and data[:4] == b"RLCN"
            palette = [(c & 31, c >> 5 & 31, c >> 10 & 31) for c in struct.unpack_from("<16H", data, 40)]
            converted = frame.copy()
            converted.putpalette([v * 255 // 31 for rgb in palette for v in rgb])
            relative = Path("shiny") / (mon["key"] + ".png") if shiny else Path(mon["key"] + ".png")
            target = args.output / relative
            target.parent.mkdir(parents=True, exist_ok=True)
            converted.save(target, transparency=0)
            report["images"].append({"species": mon["engine_species_id"], "name": mon["name"], "shiny": shiny, "file": relative.as_posix(), "sha256": hashlib.sha256(target.read_bytes()).hexdigest(), "source_frame_sha256": hashlib.sha256(source.read_bytes()).hexdigest(), "source_palette_sha256": hashlib.sha256(data).hexdigest()})
    (args.output / "manifest.json").write_text(json.dumps(report, indent=2) + "\n")
    print(f"Exported {len(report['images'])} normal/shiny local images.")

if __name__ == "__main__":
    main()
