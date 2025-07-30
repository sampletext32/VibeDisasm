namespace VibeDisasm.Web.Dtos;

public abstract record TypeArchiveElementDto(Guid Id);

public record TypeArchivePrimitiveElementDto(Guid Id, string Name, int Size) : TypeArchiveElementDto(Id);

public record TypeArchivePointerElementDto(Guid Id, TypeArchiveElementDto Type) : TypeArchiveElementDto(Id);

public record TypeArchiveArrayElementDto(Guid Id, TypeArchiveElementDto ElementType, int ElementCount)
    : TypeArchiveElementDto(Id);

public record TypeArchiveStructureElementDto(
    Guid Id,
    string Name,
    IEnumerable<TypeArchiveElementDto> Fields
) : TypeArchiveElementDto(Id);

public record TypeArchiveStructureFieldElementDto(TypeArchiveElementDto Type, string Name)
    : TypeArchiveElementDto(null!);

public record TypeArchiveFunctionElementDto(
    Guid Id,
    string Name,
    TypeArchiveElementDto ReturnType,
    IEnumerable<TypeArchiveFunctionArgumentElementDto> Arguments
) : TypeArchiveElementDto(Id);

public record TypeArchiveFunctionArgumentElementDto(TypeArchiveElementDto Type, string Name);

public record TypeArchiveTypeRefElementDto(Guid Id, TypeArchiveElementDto Type) : TypeArchiveElementDto(Id);

public record TypeArchiveEnumElementDto(
    Guid Id,
    string Name,
    TypeArchiveElementDto UnderlyingType,
    IEnumerable<TypeArchiveEnumMemberElementDto> Members
) : TypeArchiveElementDto(Id);

public record TypeArchiveEnumMemberElementDto(string Name, long Value);
