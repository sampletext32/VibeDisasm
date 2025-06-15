using VibeDisasm.Web.Extensions;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Temporary;

public static class DefaultWindowsTypesPopulator
{
    public static void Populate(TypeStorage typeStorage)
    {
        typeStorage.AddType(new PrimitiveType(1, "char").AsReadonly());
        typeStorage.AddType(new PrimitiveType(1, "byte").AsReadonly());
        typeStorage.AddType(new PrimitiveType(1, "uint8_t").AsReadonly());
        typeStorage.AddType(new PrimitiveType(1, "int8_t").AsReadonly());
        typeStorage.AddType(new PrimitiveType(2, "short").AsReadonly());
        typeStorage.AddType(new PrimitiveType(2, "uint16_t").AsReadonly());
        typeStorage.AddType(new PrimitiveType(2, "int16_t").AsReadonly());
        typeStorage.AddType(new PrimitiveType(4, "int").AsReadonly());
        typeStorage.AddType(new PrimitiveType(4, "uint").AsReadonly());
        typeStorage.AddType(new PrimitiveType(4, "uint32_t").AsReadonly());
        typeStorage.AddType(new PrimitiveType(4, "int32_t").AsReadonly());
        typeStorage.AddType(new PrimitiveType(8, "long long").AsReadonly());
        var uint64tType = typeStorage.AddType(new PrimitiveType(8, "uint64_t").AsReadonly());
        typeStorage.AddType(new PrimitiveType(8, "int64_t").AsReadonly());
        typeStorage.AddType(new PrimitiveType(4, "float").AsReadonly());
        typeStorage.AddType(new PrimitiveType(8, "double").AsReadonly());
        typeStorage.AddType(new PrimitiveType(0, "void").AsReadonly());
        typeStorage.AddType(new PrimitiveType(4, "bool").AsReadonly());

        var dwordType = typeStorage.AddType(new PrimitiveType(4, "DWORD").AsReadonly());
        var wordType = typeStorage.AddType(new PrimitiveType(2, "WORD").AsReadonly());
        var byteType = typeStorage.AddType(new PrimitiveType(1, "BYTE").AsReadonly());
        var handleType = typeStorage.AddType(new PrimitiveType(4, "HANDLE").AsReadonly());
        var hinstanceType = typeStorage.AddType(new PrimitiveType(4, "HINSTANCE").AsReadonly());
        var hwndType = typeStorage.AddType(new PrimitiveType(4, "HWND").AsReadonly());
        var lpvoidType = typeStorage.AddType(new PrimitiveType(4, "LPVOID").AsReadonly());
        var eresType = typeStorage.AddType(new ArrayType(wordType, 4));
        var eres2Type = typeStorage.AddType(new ArrayType(wordType, 10));

        typeStorage.AddType(new StructureType("IMAGE_DOS_HEADER", [
            new(wordType,  "e_magic"),      /* 00: MZ Header signature */
            new(wordType,  "e_cblp"),       /* 02: Bytes on last page of file */
            new(wordType,  "e_cp"),         /* 04: Pages in file */
            new(wordType,  "e_crlc"),       /* 06: Relocations */
            new(wordType,  "e_cparhdr"),    /* 08: Size of header in paragraphs */
            new(wordType,  "e_minalloc"),   /* 0a: Minimum extra paragraphs needed */
            new(wordType,  "e_maxalloc"),   /* 0c: Maximum extra paragraphs needed */
            new(wordType,  "e_ss"),         /* 0e: Initial (relative) SS value */
            new(wordType,  "e_sp"),         /* 10: Initial SP value */
            new(wordType,  "e_csum"),       /* 12: Checksum */
            new(wordType,  "e_ip"),         /* 14: Initial IP value */
            new(wordType,  "e_cs"),         /* 16: Initial (relative) CS value */
            new(wordType,  "e_lfarlc"),     /* 18: File address of relocation table */
            new(wordType,  "e_ovno"),       /* 1a: Overlay number */
            new(eresType,  "e_res"),        /* 1c: Reserved words */
            new(wordType,  "e_oemid"),      /* 24: OEM identifier (for e_oeminfo) */
            new(wordType,  "e_oeminfo"),    /* 26: OEM information; e_oemid specific */
            new(eres2Type,  "e_res2"),      /* 28: Reserved words */
            new(dwordType, "e_lfanew"),     /* 3c: Offset to extended header */
        ]).AsReadonly());

        var imageFileHeader = typeStorage.AddType(new StructureType("IMAGE_FILE_HEADER", [
            new(wordType, "Machine"),
            new (wordType, "NumberOfSections"),
            new (dwordType, "TimeDateStamp"),
            new (dwordType, "PointerToSymbolTable"),
            new (dwordType, "NumberOfSymbols"),
            new (wordType, "SizeOfOptionalHeader"),
            new (wordType, "Characteristics"),
        ]).AsReadonly());

        var imageDataDirectoryType = typeStorage.AddType(new StructureType("IMAGE_DATA_DIRECTORY", [
            new(dwordType, "VirtualAddress"),
            new(dwordType, "Size"),
        ]).AsReadonly());

        var dataDirectoryType = typeStorage.AddType(new ArrayType(imageDataDirectoryType, 16));

        var imageOptionalHeader32Type = typeStorage.AddType(new StructureType("IMAGE_OPTIONAL_HEADER32", [
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
        ]).AsReadonly());

        var imageOptionalHeader64Type = typeStorage.AddType(new StructureType("IMAGE_OPTIONAL_HEADER64", [
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
        ]).AsReadonly());

        typeStorage.AddType(new StructureType("IMAGE_NT_HEADERS32", [
            new(dwordType, "Signature"),
            new(imageFileHeader, "FileHeader"),
            new (imageOptionalHeader32Type, "OptionalHeader")
        ]).AsReadonly());
        typeStorage.AddType(new StructureType("IMAGE_NT_HEADERS64", [
            new(dwordType, "Signature"),
            new(imageFileHeader, "FileHeader"),
            new (imageOptionalHeader64Type, "OptionalHeader")
        ]).AsReadonly());
    }
}
