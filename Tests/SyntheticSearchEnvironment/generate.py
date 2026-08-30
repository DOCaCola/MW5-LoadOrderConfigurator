#!/usr/bin/env python3

import argparse
import json
import shutil
from pathlib import Path


def build_manifest(mod_index: int) -> list[str]:
    conflict_group = mod_index % 20
    return [
        "/Game/Synthetic/Common/SharedCore.uasset",
        f"/Game/Synthetic/Groups/Group_{conflict_group:02d}/Balance.uasset",
        f"/Game/Synthetic/Groups/Group_{conflict_group:02d}/Effects.uasset",
        f"/Game/Synthetic/Systems/System_{mod_index % 8:02d}.uasset",
        f"/Game/Synthetic/Unique/Mod_{mod_index:04d}.uasset",
    ]


def main() -> None:
    parser = argparse.ArgumentParser(
        description="Generate an isolated MW5-LOC installation with many small synthetic mods."
    )
    parser.add_argument("--count", type=int, default=600)
    parser.add_argument(
        "--broken-link-index",
        type=int,
        default=None,
        help="Replace one generated pak with a link to a missing target.",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=Path(__file__).resolve().parent / "generated",
    )
    args = parser.parse_args()

    output = args.output.resolve()
    if output.exists():
        shutil.rmtree(output)

    game_root = output / "game"
    mods_root = game_root / "MW5Mercs" / "Mods"
    settings_root = output / "settings"
    mods_root.mkdir(parents=True)
    settings_root.mkdir(parents=True)

    (game_root / "MechWarrior.exe").write_bytes(b"")

    mod_status: dict[str, dict[str, bool]] = {}
    categories = (
        "Weapons",
        "Visuals",
        "Career",
        "Missions",
        "Audio",
        "Mechs",
        "Balance",
        "Interface",
    )

    for index in range(args.count):
        folder = f"Synthetic_Mod_{index:04d}"
        category = categories[index % len(categories)]
        mod_root = mods_root / folder
        pak_root = mod_root / "Paks"
        pak_root.mkdir(parents=True)

        mod_json = {
            "displayName": f"Synthetic {category} Test Mod {index:04d}",
            "version": f"{1 + index % 5}.{index % 17}.{index % 23}",
            "buildNumber": index + 1,
            "description": (
                f"Synthetic search and conflict fixture {index:04d}; "
                f"category {category}; group {index % 20:02d}."
            ),
            "author": f"Synthetic Author {index % 47:02d}",
            "authorURL": "",
            "defaultLoadOrder": float(index),
            "locOriginalLoadOrder": float(index),
            "gameVersion": "1.1.361",
            "manifest": build_manifest(index),
            "steamPublishedFileId": 0,
            "steamLastSubmittedBuildNumber": 0,
            "steamModVisibility": "Private",
        }
        (mod_root / "mod.json").write_text(
            json.dumps(mod_json, indent=2),
            encoding="utf-8",
        )
        pak_path = pak_root / f"{folder}.pak"
        pak_path.write_bytes(
            (f"synthetic-pak-{index:04d}\n" * 8).encode("ascii")
        )
        if index == args.broken_link_index:
            pak_path.unlink()
            pak_path.symlink_to(pak_root / f"{folder}.missing.pak")
        mod_status[folder] = {"bEnabled": True}

    mod_list = {
        "gameVersion": "1.1.361",
        "modStatus": mod_status,
    }
    (mods_root / "modlist.json").write_text(
        json.dumps(mod_list, indent=2),
        encoding="utf-8",
    )

    settings = {
        "platform": "Generic",
        "InstallPath": str(game_root),
        "ListSortOrder": "HighToLow",
        "EnableFileWatch": False,
        "AllowDarkMode": True,
    }
    (settings_root / "Settings.json").write_text(
        json.dumps(settings, indent=2),
        encoding="utf-8",
    )

    metadata = {
        "modCount": args.count,
        "gameRoot": str(game_root),
        "modsRoot": str(mods_root),
        "settingsRoot": str(settings_root),
        "searchTerms": [
            "Synthetic",
            "Weapons",
            "Author 23",
            "Synthetic_Mod_0599",
            "no-such-mod",
        ],
        "brokenLinkIndex": args.broken_link_index,
    }
    (output / "environment.json").write_text(
        json.dumps(metadata, indent=2),
        encoding="utf-8",
    )

    print(json.dumps(metadata, indent=2))


if __name__ == "__main__":
    main()
