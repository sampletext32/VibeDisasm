namespace VibeDisasm.Web.Models.Types;

public abstract class RuntimeDatabaseTypeVisitor<TReturn>
{
    public TReturn Visit(RuntimeDatabaseType databaseType) => databaseType.Accept(this);

    public abstract TReturn VisitPrimitive(RuntimePrimitiveType type);
    public abstract TReturn VisitStruct(RuntimeStructureType type);
    public abstract TReturn VisitPointer(RuntimePointerType type);
    public abstract TReturn VisitFunction(RuntimeFunctionType type);
    public abstract TReturn VisitArray(RuntimeArrayType type);
    public abstract TReturn VisitRef(RuntimeTypeRefType type);
    public abstract TReturn VisitEnum(RuntimeEnumType type);
}
