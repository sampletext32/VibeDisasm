/**
 * Type definitions for the type archive system
 */

// Base type for type references
export interface TypeRef {
  $type: 'ref';
  Type: DatabaseType;
  Id: string;
}

// Base interface for all database types
export interface DatabaseTypeBase {
  Id: string;
}

// Discriminated union for all database types
export type DatabaseType = 
  | PrimitiveType
  | ArrayType
  | StructType
  | PointerType
  | FunctionType;

// Primitive type (like DWORD, WORD, BYTE, etc.)
export interface PrimitiveType extends DatabaseTypeBase {
  $type: 'primitive';
  Name: string;
}

// Array type
export interface ArrayType extends DatabaseTypeBase {
  $type: 'array';
  ElementType: TypeRef;
  ElementCount: number;
}

// Structure field
export interface StructField {
  Type: TypeRef;
  Name: string;
}

// Structure type
export interface StructType extends DatabaseTypeBase {
  $type: 'struct';
  Name: string;
  Fields: StructField[];
}

// Pointer type
export interface PointerType extends DatabaseTypeBase {
  $type: 'pointer';
  PointedType: TypeRef;
}

// Function argument
export interface FunctionArgument {
  Type: TypeRef;
  Name: string;
}

// Function type
export interface FunctionType extends DatabaseTypeBase {
  $type: 'function';
  Name: string;
  ReturnType: TypeRef;
  Arguments: FunctionArgument[];
}

// Type Archive
export interface TypeArchive {
  Namespace: string;
  Types: DatabaseType[];
}
