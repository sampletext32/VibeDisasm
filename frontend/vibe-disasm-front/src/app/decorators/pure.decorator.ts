/**
 * Pure decorator for methods that don't alter model state.
 * This is equivalent to the [Pure] attribute in C#.
 */
export function Pure(target: any, propertyKey: string, descriptor: PropertyDescriptor) {
  return descriptor;
}
