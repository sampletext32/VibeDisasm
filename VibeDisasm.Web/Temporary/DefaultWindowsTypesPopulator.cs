using System.Collections;
using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Temporary;

public static class DefaultWindowsTypesPopulator
{
    public static RuntimeTypeArchive CreateBuiltinArchive()
    {
        var archive = new RuntimeTypeArchive("builtin");

        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "undefined"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "char"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "byte"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "uint8_t"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "int8_t"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "short"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "uint16_t"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "int16_t"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "int"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "uint"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "uint32_t"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "int32_t"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "long long"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "uint64_t"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "int64_t"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "float"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "double"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "void"));
        archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "builtin", "bool"));

        return archive;
    }

    public static RuntimeTypeArchive CreateWin32Archive(RuntimeTypeStorage storage)
    {
        var archive = new RuntimeTypeArchive("win32");

        var dwordType = archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "win32", "DWORD")).MakeRef();
        var wordType = archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "win32", "WORD")).MakeRef();
        var byteType = archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "win32", "BYTE")).MakeRef();
        var handleType = archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "win32", "HANDLE")).MakeRef();
        var hinstanceType = archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "win32", "HINSTANCE")).MakeRef();
        var hwndType = archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "win32", "HWND")).MakeRef();
        var lpvoidType = archive.AddType(new RuntimePrimitiveType(Guid.NewGuid(), "win32", "LPVOID")).MakeRef();
        var eresType = archive.AddType(new RuntimeArrayType(Guid.NewGuid(), "win32", wordType, 4)).MakeRef();
        var eres2Type = archive.AddType(new RuntimeArrayType(Guid.NewGuid(), "win32", wordType, 10)).MakeRef();

        archive.AddType(new RuntimeStructureType(Guid.NewGuid(), "win32", "IMAGE_DOS_HEADER", [
            new RuntimeStructureTypeField(wordType, "e_magic"), /* 00: MZ Header signature */
            new RuntimeStructureTypeField(wordType, "e_cblp"), /* 02: Bytes on last page of file */
            new RuntimeStructureTypeField(wordType, "e_cp"), /* 04: Pages in file */
            new RuntimeStructureTypeField(wordType, "e_crlc"), /* 06: Relocations */
            new RuntimeStructureTypeField(wordType, "e_cparhdr"), /* 08: Size of header in paragraphs */
            new RuntimeStructureTypeField(wordType, "e_minalloc"), /* 0a: Minimum extra paragraphs needed */
            new RuntimeStructureTypeField(wordType, "e_maxalloc"), /* 0c: Maximum extra paragraphs needed */
            new RuntimeStructureTypeField(wordType, "e_ss"), /* 0e: Initial (relative) SS value */
            new RuntimeStructureTypeField(wordType, "e_sp"), /* 10: Initial SP value */
            new RuntimeStructureTypeField(wordType, "e_csum"), /* 12: Checksum */
            new RuntimeStructureTypeField(wordType, "e_ip"), /* 14: Initial IP value */
            new RuntimeStructureTypeField(wordType, "e_cs"), /* 16: Initial (relative) CS value */
            new RuntimeStructureTypeField(wordType, "e_lfarlc"), /* 18: File address of relocation table */
            new RuntimeStructureTypeField(wordType, "e_ovno"), /* 1a: Overlay number */
            new RuntimeStructureTypeField(eresType, "e_res"), /* 1c: Reserved words */
            new RuntimeStructureTypeField(wordType, "e_oemid"), /* 24: OEM identifier (for e_oeminfo) */
            new RuntimeStructureTypeField(wordType, "e_oeminfo"), /* 26: OEM information; e_oemid specific */
            new RuntimeStructureTypeField(eres2Type, "e_res2"), /* 28: Reserved words */
            new RuntimeStructureTypeField(dwordType, "e_lfanew"), /* 3c: Offset to extended header */
        ]));

        var imageFileHeader = archive.AddType(new RuntimeStructureType(Guid.NewGuid(), "win32", "IMAGE_FILE_HEADER", [
            new(wordType, "Machine"),
            new(wordType, "NumberOfSections"),
            new(dwordType, "TimeDateStamp"),
            new(dwordType, "PointerToSymbolTable"),
            new(dwordType, "NumberOfSymbols"),
            new(wordType, "SizeOfOptionalHeader"),
            new(wordType, "Characteristics"),
        ])).MakeRef();

        var imageDataDirectoryType = archive.AddType(new RuntimeStructureType(Guid.NewGuid(), "win32",
            "IMAGE_DATA_DIRECTORY", [
                new(dwordType, "VirtualAddress"),
                new(dwordType, "Size"),
            ])).MakeRef();

        var dataDirectoryType = archive
            .AddType(new RuntimeArrayType(Guid.NewGuid(), "win32", imageDataDirectoryType, 16)).MakeRef();

        var imageOptionalHeader32Type = archive.AddType(new RuntimeStructureType(Guid.NewGuid(), "win32",
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

        var uint64tType = storage.FindType("builtin", "uint64_t")!.MakeRef();

        var imageOptionalHeader64Type = archive.AddType(new RuntimeStructureType(Guid.NewGuid(), "win32",
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

        archive.AddType(new RuntimeStructureType(Guid.NewGuid(), "win32", "IMAGE_NT_HEADERS32", [
            new(dwordType, "Signature"),
            new(imageFileHeader, "FileHeader"),
            new(imageOptionalHeader32Type, "OptionalHeader")
        ]));
        archive.AddType(new RuntimeStructureType(Guid.NewGuid(), "win32", "IMAGE_NT_HEADERS64", [
            new(dwordType, "Signature"),
            new(imageFileHeader, "FileHeader"),
            new(imageOptionalHeader64Type, "OptionalHeader")
        ]));
        return archive;
    }
}
