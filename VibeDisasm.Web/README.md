# Web project, containing web api for the frontend

## Developer notes

1. Everything with "Runtime" prefix is only supposed to be in RAM.
2. [JsonSerializerOptionsPresets.cs](JsonSerializerOptionsPresets.cs) holds presets for serializing inherited types to json
3. All endpoints are defined in [Endpoints](Endpoints)
4. Project is stored as a zip-archive.
5. Type archive is stored as a raw JSON for now.

## Type System

The type system currently supports the following types:
- Primitive types (int, byte, etc.)
- Structures with fields
- Pointers
- Arrays
- Functions with arguments
- Type references
- Enumerations with named values

### Future Type System Additions

Potential types to add in the future:

1. **Union Type** - For representing C/C++ unions where multiple fields can occupy the same memory location
2. **Bitfield Type** - For representing bit fields within structures (common in system programming and hardware interfaces)
3. **Typedef/Type Alias** - A way to create aliases for existing types without creating new types
4. **Void Type** - A specific type for representing void returns and parameters
5. **Generic/Template Type** - For representing C++ templates or generic types
6. **Class Type** - For object-oriented code representation with inheritance, methods, and fields
7. **Interface Type** - For representing abstract interfaces
8. **Function Pointer Type** - A dedicated type for function pointers
9. **Variadic Function Type** - For functions with variable arguments (like printf)
10. **Const/Volatile Type Modifiers** - For representing type qualifiers
