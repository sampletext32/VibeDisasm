namespace VibeDisasm.Web.Models.Types;

public abstract class DatabaseTypeVisitor<TReturn>
{
    public TReturn Visit(DatabaseType databaseType) => databaseType.Accept(this);

    public abstract TReturn VisitPrimitive(PrimitiveType type);
    public abstract TReturn VisitStruct(StructureType type);
    public abstract TReturn VisitPointer(PointerType type);
    public abstract TReturn VisitFunction(FunctionType type);
    public abstract TReturn VisitArray(ArrayType type);
}
