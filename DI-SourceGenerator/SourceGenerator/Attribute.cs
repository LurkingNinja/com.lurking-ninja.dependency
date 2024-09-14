namespace LurkingNinja.SourceGenerator
{
    using Dependency.Attributes;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class Attribute
    {
        private const string _FIND = nameof(Find);
        private const string _FIND_WITH_TAG = nameof(FindWithTag);
        private const string _GENERATE_AWAKE = nameof(GenerateAwake);
        private const string _GENERATE_INITIALIZERS = nameof(GenerateInitializers);
        private const string _GENERATE_ON_VALIDATE = nameof(GenerateOnValidate);
        private const string _GET = nameof(Get);
        private const string _ADD = nameof(Add);
        private const string _GET_IN_CHILDREN = nameof(GetInChildren);
        private const string _GET_IN_PARENT = nameof(GetInParent);
        private const string _IGNORE_SELF = nameof(IgnoreSelf);
        private const string _INCLUDE_INACTIVE = nameof(IncludeInactive);
        private const string _INJECT_IN_EDITOR = nameof(InjectInEditor);
        private const string _INJECT_IN_RUNTIME = nameof(InjectInRuntime);
        private const string _SKIP_NULL_CHECK = nameof(SkipNullCheck);
        private const string _STABLE_SORT = nameof(StableSort);
        private const string _DI_TEST_HELPERS = nameof(DiTestHelpers);

        private static readonly string[] _ValidBaseAttributes =
        {
            _FIND, _FIND_WITH_TAG, _GET, _ADD, _GET_IN_CHILDREN, _GET_IN_PARENT
        };

        private static readonly string[] _ValidGameObjectAttributes =
        {
            _FIND, _FIND_WITH_TAG
        };

        private static readonly string[] _ValidComponentAttributes =
        {
            _GET, _ADD, _GET_IN_CHILDREN, _GET_IN_PARENT
        };

        internal static bool HasFind(SyntaxNode node) => Helper.HasAttribute(node, _FIND);
        internal static bool HasFindWithTag(SyntaxNode node) => Helper.HasAttribute(node, _FIND_WITH_TAG);
        
        private static bool HasGenerateInitializers(ClassDeclarationSyntax node) =>
            Helper.HasAttribute(node, _GENERATE_INITIALIZERS);

        internal static bool HasGenerateAwake(ClassDeclarationSyntax node) =>
            Helper.HasAttribute(node, _GENERATE_AWAKE)
            || HasGenerateInitializers(node);
        internal static bool HasGenerateOnValidate(ClassDeclarationSyntax node) =>
            Helper.HasAttribute(node, _GENERATE_ON_VALIDATE)
            || HasGenerateInitializers(node);
        internal static bool HasGet(SyntaxNode node) =>
            Helper.HasAttribute(node, _GET) || Helper.HasAttribute(node, _ADD);
        internal static bool HasAdd(SyntaxNode node) => Helper.HasAttribute(node, _ADD);
        internal static bool HasGetInChildren(SyntaxNode node) => Helper.HasAttribute(node, _GET_IN_CHILDREN);
        internal static bool HasGetInParent(SyntaxNode node) => Helper.HasAttribute(node, _GET_IN_PARENT);
        internal static bool HasIgnoreSelf(SyntaxNode node) => Helper.HasAttribute(node, _IGNORE_SELF);
        internal static bool HasIncludeInactive(SyntaxNode node) => Helper.HasAttribute(node, _INCLUDE_INACTIVE);
        internal static bool HasInjectInEditor(SyntaxNode node) => Helper.HasAttribute(node, _INJECT_IN_EDITOR);
        internal static bool HasInjectInRuntime(SyntaxNode node) => Helper.HasAttribute(node, _INJECT_IN_RUNTIME);
        internal static bool HasSkipNullCheck(SyntaxNode node) => Helper.HasAttribute(node, _SKIP_NULL_CHECK);
        internal static bool HasStableSort(SyntaxNode node) => Helper.HasAttribute(node, _STABLE_SORT);

        internal static string GetFindParam(SyntaxNode syntaxNode, Compilation compilation) =>
            Helper.GetAttributeParam(syntaxNode, _FIND, "gameObjectName", compilation);

        internal static string GetFindWithTagParam(SyntaxNode syntaxNode, Compilation compilation) =>
            Helper.GetAttributeParam(syntaxNode, _FIND_WITH_TAG, "tagName", compilation);

        internal static bool HasAnyValidAttribute(SyntaxNode syntaxNode) =>
            _ValidBaseAttributes.Any(attribute => Helper.HasAttribute(syntaxNode, attribute));
        
        internal static bool HasGameObjectAttribute(SyntaxNode syntaxNode) =>
            _ValidGameObjectAttributes.Any(attribute => Helper.HasAttribute(syntaxNode, attribute));

        internal static bool HasComponentAttribute(SyntaxNode syntaxNode) =>
            _ValidComponentAttributes.Any(attribute => Helper.HasAttribute(syntaxNode, attribute));

        internal static bool HasTestHelpers(ClassDeclarationSyntax syntaxNode) =>
            Helper.HasAttribute(syntaxNode, _DI_TEST_HELPERS);

        internal static string GetIncludeInactive(SyntaxNode syntaxNode) =>
            Helper.Toggle(HasIncludeInactive(syntaxNode), "true");

        internal static string GetInHierarchy(SyntaxNode syntaxNode, bool forceChildren =  false) =>
            Helper.Toggle(forceChildren || HasGetInChildren(syntaxNode), "InChildren", "InParent");

    }
}