using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.TypeInterpretation;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.Temporary;

public static class DefaultWindowsTypesPopulator
{
    public static RuntimeTypeArchive CreateBuiltinArchive()
    {
        var archive = new RuntimeTypeArchive("builtin", true);

        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("41ef9508-44c8-4bd8-86c7-0959cab26301"),
                "builtin",
                "undefined",
                1,
                InterpretAs.Bytes
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("c33d3a33-7c76-4c95-9892-b48c2b303413"),
                "builtin",
                "char",
                1,
                InterpretAs.AsciiString
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("20aa32d7-f391-4eb8-9a64-154f15095df1"),
                "builtin",
                "byte",
                1,
                InterpretAs.Bytes
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("a9dbd582-0081-4510-980e-1bfc914b52ca"),
                "builtin",
                "uint8_t",
                1,
                InterpretAs.UnsignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("8611976c-52ea-494f-b9e1-0ec99ba8db23"),
                "builtin",
                "int8_t",
                1,
                InterpretAs.SignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("8ddb9a93-18e5-441d-9a74-52afd9112fe8"),
                "builtin",
                "short",
                2,
                InterpretAs.SignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("e1531c7a-46b7-4efe-9c7f-9b944bf08afc"),
                "builtin",
                "uint16_t",
                2,
                InterpretAs.UnsignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("db921120-a838-4e21-af49-4ed7a749c2ec"),
                "builtin",
                "int16_t",
                2,
                InterpretAs.SignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("fa826057-5d46-43db-a112-eaed51862495"),
                "builtin",
                "int",
                4,
                InterpretAs.SignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("42a2383d-3d35-46bf-b0ae-b7640d6fb126"),
                "builtin",
                "uint",
                4,
                InterpretAs.UnsignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("51e4da38-242a-44b5-a58e-0396c2af25b8"),
                "builtin",
                "uint32_t",
                4,
                InterpretAs.UnsignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("0fd7081a-7218-4a10-803d-a9dc8f2d8cf5"),
                "builtin",
                "int32_t",
                4,
                InterpretAs.SignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("7eaa8b0a-d990-4016-8fbd-deba7a4ae724"),
                "builtin",
                "long long",
                8,
                InterpretAs.SignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("c3da3985-9498-4b07-9d80-60436a09a3ec"),
                "builtin",
                "uint64_t",
                8,
                InterpretAs.UnsignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("7e570c0b-4ff9-4d39-8a79-6d99cd4137ed"),
                "builtin",
                "int64_t",
                8,
                InterpretAs.SignedIntegerLE
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("bc1373d6-57b3-4c23-b7ba-bc6b3ede9514"),
                "builtin",
                "float",
                4,
                InterpretAs.FloatingPoint
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("73e7d3d7-3a03-4376-852b-4bf21a2525c7"),
                "builtin",
                "double",
                8,
                InterpretAs.FloatingPoint
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("baf641c2-71f4-4f8f-9d16-bc6cf22792b9"),
                "builtin",
                "void",
                0,
                InterpretAs.Bytes
            )
        );
        archive.AddType(
            new RuntimePrimitiveType(
                new Guid("6e43fd89-6d81-421a-a265-cef33f9d6e54"),
                "builtin",
                "bool",
                1,
                InterpretAs.Boolean
            )
        );

        return archive;
    }

    public static RuntimeTypeArchive CreateWin32Archive(RuntimeTypeArchive builtinArchive)
    {
        var archive = new RuntimeTypeArchive("win32", true);

        var dwordType = archive.AddType(
                new RuntimePrimitiveType(
                    new Guid("0c5024d9-0dec-43d3-9185-e4c5978abf62"),
                    "win32",
                    "DWORD",
                    4,
                    InterpretAs.UnsignedIntegerLE
                )
            );
        var wordType = archive.AddType(
                new RuntimePrimitiveType(
                    new Guid("26505cc8-6d60-44fc-b403-6e8ce18240dc"),
                    "win32",
                    "WORD",
                    2,
                    InterpretAs.UnsignedIntegerLE
                )
            );
        var byteType = archive.AddType(
                new RuntimePrimitiveType(
                    new Guid("eadf3262-d4d3-480c-925e-01ab50f790e4"),
                    "win32",
                    "BYTE",
                    1,
                    InterpretAs.UnsignedIntegerLE
                )
            );
        var handleType = archive.AddType(
                new RuntimePrimitiveType(new Guid("e906809e-ecce-4be1-9b6d-210c6ac701d8"), "win32", "HANDLE", 4)
            );
        var hinstanceType = archive.AddType(
                new RuntimePrimitiveType(
                    new Guid("7430d3b7-616d-43e9-b323-71714e9b37b4"),
                    "win32",
                    "HINSTANCE",
                    4
                )
            );
        var hwndType = archive.AddType(
                new RuntimePrimitiveType(new Guid("06b1795a-aa1a-427d-9b74-24f49601b02c"), "win32", "HWND", 4)
            );
        var lpvoidType = archive.AddType(
                new RuntimePrimitiveType(new Guid("152665c2-abf1-4907-9db7-66ce39b87551"), "win32", "LPVOID", 4)
            );
        var eresType = archive.AddType(
                new RuntimeArrayType(new Guid("bd30205d-1db6-4e71-9441-29556ca12b52"), "win32", wordType, 4)
            );
        var eres2Type = archive.AddType(
                new RuntimeArrayType(new Guid("c14ff7e2-0589-4e0d-9946-17612fa3ffe6"), "win32", wordType, 10)
            );

        archive.AddType(
            new RuntimeStructureType(
                new Guid("65feccf9-6b71-43a4-bf4b-523c34b1738e"),
                "win32",
                "IMAGE_DOS_HEADER",
                [
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
                ]
            )
        );

        var imageFileHeader = archive.AddType(
                new RuntimeStructureType(
                    new Guid("585acd44-04ff-4fd7-85cf-c157e58e06e5"),
                    "win32",
                    "IMAGE_FILE_HEADER",
                    [
                        new(wordType, "Machine"),
                        new(wordType, "NumberOfSections"),
                        new(dwordType, "TimeDateStamp"),
                        new(dwordType, "PointerToSymbolTable"),
                        new(dwordType, "NumberOfSymbols"),
                        new(wordType, "SizeOfOptionalHeader"),
                        new(wordType, "Characteristics"),
                    ]
                )
            );

        var imageDataDirectoryType = archive.AddType(
                new RuntimeStructureType(
                    new Guid("a448286e-d976-4270-9cc3-eea8a24abe83"),
                    "win32",
                    "IMAGE_DATA_DIRECTORY",
                    [
                        new(dwordType, "VirtualAddress"),
                        new(dwordType, "Size"),
                    ]
                )
            );

        var dataDirectoryType = archive
            .AddType(
                new RuntimeArrayType(
                    new Guid("96e2ba5e-6331-44f7-9eac-8e1da21e21a8"),
                    "win32",
                    imageDataDirectoryType,
                    16
                )
            );

        var imageOptionalHeader32Type = archive.AddType(
                new RuntimeStructureType(
                    new Guid("b2d2a6f9-5a71-48ad-be7a-9deaebb3788f"),
                    "win32",
                    "IMAGE_OPTIONAL_HEADER32",
                    [
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
                    ]
                )
            );

        var uint64tType = builtinArchive.FindRequiredType("uint64_t");

        var imageOptionalHeader64Type = archive.AddType(
                new RuntimeStructureType(
                    new Guid("3998571f-1050-4fae-9d95-5265f83bb248"),
                    "win32",
                    "IMAGE_OPTIONAL_HEADER64",
                    [
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
                    ]
                )
            );

        archive.AddType(
            new RuntimeStructureType(
                new Guid("6d24e1cb-80c6-4f94-8281-4c890b49ae7d"),
                "win32",
                "IMAGE_NT_HEADERS32",
                [
                    new(dwordType, "Signature"),
                    new(imageFileHeader, "FileHeader"),
                    new(imageOptionalHeader32Type, "OptionalHeader")
                ]
            )
        );
        archive.AddType(
            new RuntimeStructureType(
                new Guid("4a4fc2a8-eb31-4e14-9778-ccefa6561e82"),
                "win32",
                "IMAGE_NT_HEADERS64",
                [
                    new(dwordType, "Signature"),
                    new(imageFileHeader, "FileHeader"),
                    new(imageOptionalHeader64Type, "OptionalHeader")
                ]
            )
        );
        return archive;
    }
}
