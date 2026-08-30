# Synthetic search environment

`generate.py` creates an isolated fake MW5 installation containing many tiny
mods. It does not copy or modify real mods, game files, or normal LOC settings.

The generated manifests deliberately contain both broadly shared and grouped
assets so conflict coloring is exercised as well as ordinary text matching.

Launch LOC with these environment variables:

- `MW5_LOC_SETTINGS_DIRECTORY=<generated settings directory>`
- `MW5_LOC_SEARCH_TIMING=1`
- `MW5_LOC_TEST_LABEL=Synthetic Search Baseline`

The most recent search duration is shown in the left side of the status bar.

`measure-search.ps1` drives a running synthetic instance through both search
modes and reports the application's measured search time and resulting visible
row count.

Pass `--broken-link-index <index>` to replace one generated pak with a link to
a missing target. This exercises the inaccessible Vortex-style deployment path
without copying a real mod. When generating from an MSYS Python interpreter,
run `create-native-broken-link.ps1` for that file afterward so the fixture uses
a native Windows reparse point rather than an MSYS link representation.
