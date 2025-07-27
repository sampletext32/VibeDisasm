using VibeDisasm.Web.Models;
using VibeDisasm.Web.Models.Types;
using VibeDisasm.Web.ProjectArchive.TypeArchiveJsonElements;

namespace VibeDisasm.Web.ProjectArchive;

public partial class TypeArchiveService
{
    private void TypeResolvePass(
        HashSet<LaterResolvedType> typesToResolve,
        Dictionary<string, RuntimeTypeArchive> loadedTypeArchives
    )
    {
        Dictionary<Guid, (RuntimeDatabaseType, TypeArchiveJsonElement)> partiallyResolvedTypes = [];

        // TODO: add validations to file format, all ids unique, struct fields have unique names, etc.

        // first pass, resolve all types that can be resolved immediately or create placeholders
        foreach (var typeToResolve in typesToResolve)
        {
            RuntimeDatabaseType resolvedType;
            switch (typeToResolve.Type)
            {
                case ArrayArchiveJsonElement arrayJson:
                    resolvedType = new RuntimeArrayType(
                        arrayJson.Id,
                        typeToResolve.Archive.Namespace,
                        elementType: null!,
                        arrayJson.ElementCount
                    );
                    partiallyResolvedTypes.Add(resolvedType.Id, (resolvedType, arrayJson));
                    break;
                case FunctionArchiveJsonElement functionJson:
                    resolvedType = new RuntimeFunctionType(
                        functionJson.Id,
                        typeToResolve.Archive.Namespace,
                        functionJson.Name,
                        returnType: null!,
                        arguments: []
                    );
                    partiallyResolvedTypes.Add(resolvedType.Id, (resolvedType, functionJson));
                    break;
                case PointerArchiveJsonElement pointerJson:
                    resolvedType = new RuntimePointerType(
                        pointerJson.Id,
                        typeToResolve.Archive.Namespace,
                        pointedType: null!
                    );
                    partiallyResolvedTypes.Add(resolvedType.Id, (resolvedType, pointerJson));
                    break;
                case PrimitiveArchiveJsonElement primitiveJson:
                    resolvedType = new RuntimePrimitiveType(
                        primitiveJson.Id,
                        typeToResolve.Archive.Namespace,
                        primitiveJson.Name,
                        primitiveJson.Size
                    );
                    break;
                case StructArchiveJsonElement structJson:
                    resolvedType = new RuntimeStructureType(
                        structJson.Id,
                        typeToResolve.Archive.Namespace,
                        structJson.Name,
                        fields: []
                    );
                    partiallyResolvedTypes.Add(resolvedType.Id, (resolvedType, structJson));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown type of resolveType", (Exception?)null);
            }

            typeToResolve.Archive.AddType(resolvedType);
        }

        // second pass, resolve types that were partially resolved in the first pass e.g. resolve placeholders
        var progressMade = true;
        while (progressMade && partiallyResolvedTypes.Count > 0)
        {
            progressMade = false;

            foreach (var (id, (runtimeType, jsonElement)) in partiallyResolvedTypes.ToList())
            {
                switch (runtimeType, jsonElement)
                {
                    case (RuntimeArrayType arrayType, ArrayArchiveJsonElement arrayJson):
                    {
                        var resolvedElementType = FindType(arrayJson.ElementType, loadedTypeArchives);
                        if (resolvedElementType is not null)
                        {
                            arrayType.ElementType = resolvedElementType;
                            partiallyResolvedTypes.Remove(id);
                            progressMade = true;
                        }

                        break;
                    }

                    case (RuntimePointerType pointerType, PointerArchiveJsonElement pointerJson):
                    {
                        var resolvedPointedType = FindType(pointerJson.PointedType, loadedTypeArchives);
                        if (resolvedPointedType is not null)
                        {
                            pointerType.PointedType = resolvedPointedType;
                            partiallyResolvedTypes.Remove(id);
                            progressMade = true;
                        }

                        break;
                    }

                    case (RuntimeFunctionType functionType, FunctionArchiveJsonElement functionJson):
                    {
                        var resolvedReturnType = FindType(functionJson.ReturnType, loadedTypeArchives);
                        if (resolvedReturnType is null)
                        {
                            break;
                        }

                        Dictionary<string, RuntimeDatabaseType> argumentsByName = new();
                        var hasUnresolvedArg = false;

                        foreach (var arg in functionJson.Arguments)
                        {
                            var resolvedArgType = FindType(arg.Type, loadedTypeArchives);
                            if (resolvedArgType is null)
                            {
                                hasUnresolvedArg = true;
                                break;
                            }

                            argumentsByName[arg.Name] = resolvedArgType;
                        }

                        if (hasUnresolvedArg)
                        {
                            break;
                        }

                        functionType.ReturnType = resolvedReturnType;
                        functionType.Arguments.AddRange(
                            functionJson.Arguments.Select(arg =>
                                new FunctionArgument(argumentsByName[arg.Name], arg.Name)
                            )
                        );

                        partiallyResolvedTypes.Remove(id);
                        progressMade = true;
                        break;
                    }

                    case (RuntimeStructureType structType, StructArchiveJsonElement structJson):
                    {
                        Dictionary<string, RuntimeDatabaseType> fieldsByName = new();
                        var hasUnresolvedField = false;

                        foreach (var field in structJson.Fields)
                        {
                            var resolvedFieldType = FindType(field.Type, loadedTypeArchives);
                            if (resolvedFieldType is null)
                            {
                                hasUnresolvedField = true;
                                break;
                            }

                            fieldsByName[field.Name] = resolvedFieldType;
                        }

                        if (hasUnresolvedField)
                        {
                            break;
                        }

                        structType.Fields.AddRange(
                            structJson.Fields.Select(field =>
                                new RuntimeStructureTypeField(fieldsByName[field.Name], field.Name)
                            )
                        );

                        partiallyResolvedTypes.Remove(id);
                        progressMade = true;
                        break;
                    }
                }
            }
        }

        if (partiallyResolvedTypes.Count > 0)
        {
            logger.LogError(
                "Unresolved types remain: {Ids}",
                string.Join(", ", partiallyResolvedTypes.Keys)
            );
            throw new InvalidOperationException("Type resolution incomplete.");
        }
    }

    private RuntimeDatabaseType? FindType(
        TypeRefJsonElement reference,
        Dictionary<string, RuntimeTypeArchive> loadedTypeArchives
    )
    {
        // first try to find in embedded types, then in loaded type archives

        var embeddedArchive =
            typeArchiveStorage.TypeArchives.FirstOrDefault(x => x.Namespace == reference.Namespace);

        var foundType = embeddedArchive?.FindById(reference.Id);

        if (foundType != null)
        {
            return foundType;
        }

        if (loadedTypeArchives.TryGetValue(reference.Namespace, out var archive))
        {
            foundType = archive.FindById(reference.Id);
            if (foundType != null)
            {
                return foundType;
            }
        }

        logger.LogWarning(
            "Failed to resolve type {TypeId} in namespace {Namespace}. Type may not work.",
            reference.Id,
            reference.Namespace
        );

        return null;
    }
}

