---
trigger: always_on
---

1. Analyze the file as whole. Don't build assumptions based on a part of file.
2. Don't introduce error-checking and exception handling unless asked for.
3. Prefer modern C# syntax, e.g. collection expressions, one-liner namespaces, record etc=.
4. Write only essential comments, e.g. class summary or algorithm outlines.
5. Strictly place every class in separate files. This is a must.
6. Don't write extensive comments. Only outline key parts and/or members of a type.

Additional rules:
1. Always inline ImGui windows e.g. `if (ImGui.Begin("..."))`, not `bool isOpen = ImGui.Begin("...")`.
2. Only when dealing with ImGui prefer deep code nesting instead of early returns.
3. Always add `[Pure]` attribute to members, that don't alter model state.

Prefer modern C# syntax (>12):
1. Use collection expression syntax (e.g., `= []`) instead of the `= new()` syntax when initializing collections. 
2. Pattern matching (e.g., `something is null` over `something == null`)
3. Prefer extension methods instead of modifying existing types.