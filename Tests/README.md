# Load-order regression tests

`MW5.LoadOrder.Tests` exercises the production model-order mutation and load-order recomputation with:

- generated five-mod scenario matrices;
- low-to-high and high-to-low views;
- single and multiple selections;
- move-to-top and move-to-bottom operations;
- a sanitized metadata fixture derived from locally available MW5 JSON metadata.

The reference fixture contains only folder identifiers, display names, enabled states, load-order values, versions, and build numbers. It contains no local paths, manifests, descriptions, images, archives, or game data.

Run the tests with:

```bash
dotnet test "Tests/MW5.LoadOrder.Tests/MW5.LoadOrder.Tests.csproj"
```

The suite guards against the `4.0` multi-selection move-to-bottom regression by requiring the displayed order, backing-model order, and recomputed load orders to remain synchronized.
