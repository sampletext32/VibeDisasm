using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;

namespace VibeDisasm.Web.ProjectArchive;

public class TypeSizeVisitor(RuntimeUserProgram program) : RuntimeDatabaseTypeVisitor<int>
{
    // primitive types always have a fixed size
    public override int VisitPrimitive(RuntimePrimitiveType type) => type.Size;

    // size of struct is the sum of sizes of all its fields
    public override int VisitStruct(RuntimeStructureType type) => type.Fields.Sum(x => Visit(x.Type));

    // pointer size is determined by the architecture of the program (x86, x64, arm etc.)
    public override int VisitPointer(RuntimePointerType type) => program.GetPointerSize();

    // function doesn't have a size in the traditional sense, only the pointer to it does
    public override int VisitFunction(RuntimeFunctionType type) => 0;

    // array size is the size of the element type multiplied by the number of elements in the array
    public override int VisitArray(RuntimeArrayType type) => Visit(type.ElementType) * type.ElementCount;

    // ref type is a reference to another type, so its size is the size of type it's pointing to
    public override int VisitRef(RuntimeTypeRefType type)
    {
        var resolvedType = program.Database.TypeStorage.DeepResolveTypeRef(type);
        return resolvedType is null ? 0 : Visit(resolvedType);
    }

    // enum is basically it's underlying type (e.g. int), so technically it's size is the same as of the underlying type
    public override int VisitEnum(RuntimeEnumType type) => Visit(type.UnderlyingType);
}
