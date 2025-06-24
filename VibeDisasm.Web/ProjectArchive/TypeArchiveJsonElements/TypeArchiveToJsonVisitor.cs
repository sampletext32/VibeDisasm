using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

public class TypeArchiveToJsonVisitor : RuntimeDatabaseTypeVisitor<TypeArchiveJsonElement>
{
    public override TypeArchiveJsonElement VisitPrimitive(RuntimePrimitiveType type) =>
        new PrimitiveArchiveJsonElement() { Id = type.Id, Name = type.Name };

    public override TypeArchiveJsonElement VisitStruct(RuntimeStructureType type) =>
        new StructArchiveJsonElement()
        {
            Id = type.Id,
            Name = type.Name,
            Fields = type.Fields.Select(f => new StructFieldArchiveJsonElement()
            {
                Type = new TypeRefJsonElement() { Id = f.Type.Id, Namespace = f.Type.Namespace, },
                Name = f.Name,
            }).ToList()
        };

    public override TypeArchiveJsonElement VisitPointer(RuntimePointerType type) => new PointerArchiveJsonElement()
    {
        Id = type.Id,
        PointedType = new TypeRefJsonElement() { Id = type.PointedType.Id, Namespace = type.PointedType.Namespace }
    };

    public override TypeArchiveJsonElement VisitFunction(RuntimeFunctionType type) => new FunctionArchiveJsonElement()
    {
        Id = type.Id,
        Name = type.Name,
        ReturnType = new TypeRefJsonElement() { Id = type.ReturnType.Id, Namespace = type.ReturnType.Namespace },
        Arguments = type.Arguments.Select(x => new FunctionArgumentJsonElement()
        {
            Type = new TypeRefJsonElement() { Id = x.Type.Id, Namespace = x.Type.Namespace }, Name = x.Name
        }).ToList()
    };

    public override TypeArchiveJsonElement VisitArray(RuntimeArrayType type) => new ArrayArchiveJsonElement()
    {
        Id = type.Id,
        Name = type.Name,
        ElementType = new TypeRefJsonElement() { Id = type.ElementType.Id, Namespace = type.ElementType.Namespace },
        ElementCount = type.ElementCount
    };

    public override TypeArchiveJsonElement VisitRef(RuntimeTypeRefType type) => new TypeRefJsonElement()
    {
        Id = type.Id, Namespace = type.Namespace,
    };
    
    public override TypeArchiveJsonElement VisitEnum(RuntimeEnumType type) => new EnumArchiveJsonElement()
    {
        Id = type.Id,
        Name = type.Name,
        UnderlyingType = new TypeRefJsonElement() { Id = type.UnderlyingType.Id, Namespace = type.UnderlyingType.Namespace },
        Members = type.Members.Select(m => new EnumMemberJsonElement()
        {
            Name = m.Name,
            Value = m.Value
        }).ToList()
    };
}
