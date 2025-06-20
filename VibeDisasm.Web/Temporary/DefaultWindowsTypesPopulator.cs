using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Temporary;

public static class DefaultWindowsTypesPopulator
{
    public static void Populate(TypeStorage typeStorage)
    {
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "undefined"));

        // ----

        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "char"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "byte"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "uint8_t"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "int8_t"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "short"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "uint16_t"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "int16_t"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "int"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "uint"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "uint32_t"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "int32_t"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "long long"));
        var uint64tType = typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "uint64_t")).MakeRef();
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "int64_t"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "float"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "double"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "void"));
        typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "builtin", "bool"));

        var dwordType = typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "win32", "DWORD")).MakeRef();
        var wordType = typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "win32", "WORD")).MakeRef();
        var byteType = typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "win32", "BYTE")).MakeRef();
        var handleType = typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "win32", "HANDLE")).MakeRef();
        var hinstanceType = typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "win32", "HINSTANCE")).MakeRef();
        var hwndType = typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "win32", "HWND")).MakeRef();
        var lpvoidType = typeStorage.AddType(new PrimitiveType(Guid.NewGuid(), "win32", "LPVOID")).MakeRef();
        var eresType = typeStorage.AddType(new ArrayType(Guid.NewGuid(), "win32", wordType, 4)).MakeRef();
        var eres2Type = typeStorage.AddType(new ArrayType(Guid.NewGuid(), "win32", wordType, 10)).MakeRef();

        typeStorage.AddType(new StructureType(Guid.NewGuid(), "win32", "IMAGE_DOS_HEADER", [
            new StructureTypeField(wordType, "e_magic"), /* 00: MZ Header signature */
            new StructureTypeField(wordType, "e_cblp"), /* 02: Bytes on last page of file */
            new StructureTypeField(wordType, "e_cp"), /* 04: Pages in file */
            new StructureTypeField(wordType, "e_crlc"), /* 06: Relocations */
            new StructureTypeField(wordType, "e_cparhdr"), /* 08: Size of header in paragraphs */
            new StructureTypeField(wordType, "e_minalloc"), /* 0a: Minimum extra paragraphs needed */
            new StructureTypeField(wordType, "e_maxalloc"), /* 0c: Maximum extra paragraphs needed */
            new StructureTypeField(wordType, "e_ss"), /* 0e: Initial (relative) SS value */
            new StructureTypeField(wordType, "e_sp"), /* 10: Initial SP value */
            new StructureTypeField(wordType, "e_csum"), /* 12: Checksum */
            new StructureTypeField(wordType, "e_ip"), /* 14: Initial IP value */
            new StructureTypeField(wordType, "e_cs"), /* 16: Initial (relative) CS value */
            new StructureTypeField(wordType, "e_lfarlc"), /* 18: File address of relocation table */
            new StructureTypeField(wordType, "e_ovno"), /* 1a: Overlay number */
            new StructureTypeField(eresType, "e_res"), /* 1c: Reserved words */
            new StructureTypeField(wordType, "e_oemid"), /* 24: OEM identifier (for e_oeminfo) */
            new StructureTypeField(wordType, "e_oeminfo"), /* 26: OEM information; e_oemid specific */
            new StructureTypeField(eres2Type, "e_res2"), /* 28: Reserved words */
            new StructureTypeField(dwordType, "e_lfanew"), /* 3c: Offset to extended header */
        ]));

        var imageFileHeader = typeStorage.AddType(new StructureType(Guid.NewGuid(), "win32", "IMAGE_FILE_HEADER", [
            new(wordType, "Machine"),
            new(wordType, "NumberOfSections"),
            new(dwordType, "TimeDateStamp"),
            new(dwordType, "PointerToSymbolTable"),
            new(dwordType, "NumberOfSymbols"),
            new(wordType, "SizeOfOptionalHeader"),
            new(wordType, "Characteristics"),
        ])).MakeRef();

        var imageDataDirectoryType = typeStorage.AddType(new StructureType(Guid.NewGuid(), "win32",
            "IMAGE_DATA_DIRECTORY", [
                new(dwordType, "VirtualAddress"),
                new(dwordType, "Size"),
            ])).MakeRef();

        var dataDirectoryType = typeStorage.AddType(new ArrayType(Guid.NewGuid(), "win32", imageDataDirectoryType, 16)).MakeRef();

        var imageOptionalHeader32Type = typeStorage.AddType(new StructureType(Guid.NewGuid(), "win32",
            "IMAGE_OPTIONAL_HEADER32", [
                new(wordType, "Magic"),
                new(byteType, "MajorLinkerVersion"),
                new(byteType, "MinorLinkerVersion"),
                new(dwordType, "SizeOfCode"),
                new(dwordType, "SizeOfInitializedData"),
                new(dwordType, "SizeOfUninitializedData"),
                new(dwordType, "AddressOfEntryPoint"),
                new(dwordType, "BaseOfCode"),
                new(dwordType, "BaseOfData"),
                new(dwordType, "ImageBase"),
                new(dwordType, "SectionAlignment"),
                new(dwordType, "FileAlignment"),
                new(wordType, "MajorOperatingSystemVersion"),
                new(wordType, "MinorOperatingSystemVersion"),
                new(wordType, "MajorImageVersion"),
                new(wordType, "MinorImageVersion"),
                new(wordType, "MajorSubsystemVersion"),
                new(wordType, "MinorSubsystemVersion"),
                new(dwordType, "Win32VersionValue"),
                new(dwordType, "SizeOfImage"),
                new(dwordType, "SizeOfHeaders"),
                new(dwordType, "CheckSum"),
                new(wordType, "Subsystem"),
                new(wordType, "DllCharacteristics"),
                new(dwordType, "SizeOfStackReserve"),
                new(dwordType, "SizeOfStackCommit"),
                new(dwordType, "SizeOfHeapReserve"),
                new(dwordType, "SizeOfHeapCommit"),
                new(dwordType, "LoaderFlags"),
                new(dwordType, "NumberOfRvaAndSizes"),
                new(dataDirectoryType, "DataDirectory")
            ])).MakeRef();

        var imageOptionalHeader64Type = typeStorage.AddType(new StructureType(Guid.NewGuid(), "win32",
            "IMAGE_OPTIONAL_HEADER64", [
                new(wordType, "Magic"),
                new(byteType, "MajorLinkerVersion"),
                new(byteType, "MinorLinkerVersion"),
                new(dwordType, "SizeOfCode"),
                new(dwordType, "SizeOfInitializedData"),
                new(dwordType, "SizeOfUninitializedData"),
                new(dwordType, "AddressOfEntryPoint"),
                new(dwordType, "BaseOfCode"),
                new(uint64tType, "ImageBase"),
                new(dwordType, "SectionAlignment"),
                new(dwordType, "FileAlignment"),
                new(wordType, "MajorOperatingSystemVersion"),
                new(wordType, "MinorOperatingSystemVersion"),
                new(wordType, "MajorImageVersion"),
                new(wordType, "MinorImageVersion"),
                new(wordType, "MajorSubsystemVersion"),
                new(wordType, "MinorSubsystemVersion"),
                new(dwordType, "Win32VersionValue"),
                new(dwordType, "SizeOfImage"),
                new(dwordType, "SizeOfHeaders"),
                new(dwordType, "CheckSum"),
                new(wordType, "Subsystem"),
                new(wordType, "DllCharacteristics"),
                new(uint64tType, "SizeOfStackReserve"),
                new(uint64tType, "SizeOfStackCommit"),
                new(uint64tType, "SizeOfHeapReserve"),
                new(uint64tType, "SizeOfHeapCommit"),
                new(dwordType, "LoaderFlags"),
                new(dwordType, "NumberOfRvaAndSizes"),
                new(dataDirectoryType, "DataDirectory")
            ])).MakeRef();

        typeStorage.AddType(new StructureType(Guid.NewGuid(), "win32", "IMAGE_NT_HEADERS32", [
            new(dwordType, "Signature"),
            new(imageFileHeader, "FileHeader"),
            new(imageOptionalHeader32Type, "OptionalHeader")
        ]));
        typeStorage.AddType(new StructureType(Guid.NewGuid(), "win32", "IMAGE_NT_HEADERS64", [
            new(dwordType, "Signature"),
            new(imageFileHeader, "FileHeader"),
            new(imageOptionalHeader64Type, "OptionalHeader")
        ]));
    }
}
