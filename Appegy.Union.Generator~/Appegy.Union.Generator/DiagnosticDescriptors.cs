using Microsoft.CodeAnalysis;

namespace Appegy.Union.Generator;

public static class DiagnosticDescriptors
{
    private const string Category = "Union";

    public static DiagnosticDescriptor NotStruct { get; } = new(
        id: "UNION001",
        title: "Type has to be struct",
        messageFormat: "The type '{0}' must be a struct to use [Union].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor NotPartial { get; } = new(
        id: "UNION002",
        title: "Struct is not partial",
        messageFormat: "The struct '{0}' must be marked as 'partial' to use [Union] attribute.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor NestedNotPartial { get; } = new(
        id: "UNION003",
        title: "Nested struct is not partial",
        messageFormat: "The nested types must be marked as 'partial' to use [Union].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor NoTypesProvided { get; } = new(
        id: "UNION004",
        title: "No types provided in [Union] attribute",
        messageFormat: "The [Union] attribute must specify at least one type.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor DuplicateExposeAttribute { get; } = new(
        id: "UNION005",
        title: "Duplicate [Expose] attribute",
        messageFormat: "The interface '{0}' is already specified in [Expose].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ExposeTypeMustBeInterface { get; } = new(
        id: "UNION006",
        title: "Invalid type in [Expose] attribute",
        messageFormat: "The type '{0}' must be an interface to be used in [Expose].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor TypeDoesNotImplementInterface { get; } = new(
        id: "UNION007",
        title: "Type does not implement interface",
        messageFormat: "The type '{0}' does not implement the interface '{1}' specified in [Expose].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ExposeRequiresUnion { get; } = new(
        id: "UNION008",
        title: "ExposeAttribute requires UnionAttribute",
        messageFormat: "The [Expose] attribute must be used together with [Union] attribute.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor NoInterfacesProvided { get; } = new(
        id: "UNION009",
        title: "No interfaces provided in [Expose] attribute",
        messageFormat: "The [Expose] attribute must specify at least one interface.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor DuplicateUnionType { get; } = new(
        id: "UNION010",
        title: "Duplicate type in [Union] attribute",
        messageFormat: "The type '{0}' is specified more than once in [Union].",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}