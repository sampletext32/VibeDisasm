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
