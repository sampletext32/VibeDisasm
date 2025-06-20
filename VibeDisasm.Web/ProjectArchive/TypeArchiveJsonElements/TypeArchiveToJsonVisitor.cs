using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

public class TypeArchiveToJsonVisitor : DatabaseTypeVisitor<TypeArchiveJsonElement>
{
    public override TypeArchiveJsonElement VisitPrimitive(PrimitiveType type) =>
        new PrimitiveArchiveJsonElement() { Id = type.Id, Name = type.Name };

    public override TypeArchiveJsonElement VisitStruct(StructureType type) =>
        new StructArchiveJsonElement()
        {
            Id = type.Id,
            Name = type.Name,
            Fields = type.Fields.Select(f => new StructFieldArchiveJsonElement()
            {
                TypeId = f.Type.Id, Name = f.Name,
            }).ToList()
        };

    public override TypeArchiveJsonElement VisitPointer(PointerType type) => new PointerArchiveJsonElement()
    {
        Id = type.Id, Name = type.Name, PointedTypeId = type.PointedType.Id
    };

    public override TypeArchiveJsonElement VisitFunction(FunctionType type) => new FunctionArchiveJsonElement()
    {
        Id = type.Id,
        Name = type.Name,
        ReturnTypeId = type.ReturnType.Id,
        Arguments = type.Arguments.Select(x => new FunctionArgumentJsonElement()
        {
            TypeId = x.Type.Id,
            Name = x.Name
        }).ToList()
    };

    public override TypeArchiveJsonElement VisitArray(ArrayType type) => new ArrayArchiveJsonElement()
    {
        Id = type.Id,
        Name = type.Name,
        ElementTypeId = type.ElementType.Id,
        ElementCount = type.ElementCount
    };
}
