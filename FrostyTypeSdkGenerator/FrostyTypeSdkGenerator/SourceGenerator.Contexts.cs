using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FrostyTypeSdkGenerator;

public sealed partial class SourceGenerator
{   
    private readonly record struct TypeContext(string? Namespace, string Name, bool IsValueType, ImmutableArray<FieldContext> Fields);
    
    private readonly record struct FieldContext(string Name, string Type, ImmutableArray<string> Attributes);

    private sealed class TypeContextEqualityComparer : IEqualityComparer<TypeContext>
    {
        private TypeContextEqualityComparer() { }

        public static TypeContextEqualityComparer Instance { get; } = new();

        public bool Equals(TypeContext x, TypeContext y)
        {
            return x.Namespace == y.Namespace &&
                   x.Name == y.Name &&
                   x.IsValueType == y.IsValueType &&
                   x.Fields.SequenceEqual(y.Fields, FieldContextEqualityComparer.Instance);
        }

        public int GetHashCode(TypeContext obj)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class FieldContextEqualityComparer : IEqualityComparer<FieldContext>
    {
        private FieldContextEqualityComparer() { }

        public static FieldContextEqualityComparer Instance { get; } = new();

        public bool Equals(FieldContext x, FieldContext y)
        {
            return x.Name == y.Name &&
                   x.Type == y.Type &&
                   x.Attributes.SequenceEqual(y.Attributes);
        }

        public int GetHashCode(FieldContext obj)
        {
            throw new NotImplementedException();
        }
    }
}