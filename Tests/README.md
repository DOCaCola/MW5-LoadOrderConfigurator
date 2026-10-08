# Load-order regression tests

`MW5.LoadOrder.Tests` exercises the production model-order mutation and load-order recomputation with:

- generated five-mod scenario matrices;
- low-to-high and high-to-low views;
- single and multiple selections;
- move-to-top and move-to-bottom operations;
- a sanitized metadata fixture derived from locally available MW5 JSON metadata.

`ModDeploymentTests` uses temporary game and settings directories to cover version
selection, synchronized metadata and cached priorities, original-priority recovery,
external changes in either priority source, pak cache refresh, atomic-write failures,
and UI reload/Apply round trips. It does not launch the game or edit installed mods.
It also checks that resetting to defaults and exporting text preserves displayed
row order and working state in both display directions, including tied priorities,
disabled mods, and display names that differ from folder order.
The export timestamp is checked for UTC and identical formatting under US,
German, and Saudi Arabic locales.
The MW5MO import cases follow the text format and ascending export order verified
for MW5MO 3.0.0.5. They exercise parsing, installed-mod matching,
list population, enabled states, Apply/reload, both display directions, LF/CRLF,
and three locales. Distinct, tied, and mixed priorities preserve the exported
order, including when display-name order differs from folder order.

`LinkedFileWriteTests` exercises the production save helper with ordinary files,
absolute and relative file symlinks, symlink chains, and hard links, each inside
ordinary directories, directory symlinks, and junctions. It also covers a symlink
to a hard-linked file, broken links, link loops, new files, and locked/read-only
targets. Tests require updates to reach the
original file, preserve links, truncate shorter content, and clean up temporary
files. All fixtures are temporary; installed mods are untouched. Symlink creation
permission failures are reported as inconclusive, not passed. The writer replaces
ordinary files and resolved symlink targets atomically, but writes files with
multiple hard links in place to preserve their shared identity.

The test assembly redirects application settings to a temporary directory with an
empty game fixture, so UI startup does not use personal settings or open recovery
dialogs. Appearance tests cover repeated theme changes and docking-handler
lifetime. The synthetic DPI test sizes its window to fit the source monitor at
144 DPI; it is inconclusive if the monitor cannot accommodate the minimum size.

The reference fixture contains only folder identifiers, display names, enabled states, load-order values, versions, and build numbers. It contains no local paths, manifests, descriptions, images, archives, or game data.

Run the tests with:

```bash
dotnet test "Tests/MW5.LoadOrder.Tests/MW5.LoadOrder.Tests.csproj"
```

The suite guards against the `4.0` multi-selection move-to-bottom regression by requiring the displayed order, backing-model order, and recomputed load orders to remain synchronized.
