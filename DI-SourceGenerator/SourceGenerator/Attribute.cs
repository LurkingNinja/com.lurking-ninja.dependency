using System.Linq;
using LurkingNinja.Attributes;
using Microsoft.CodeAnalysis;

namespace LurkingNinja.SourceGenerator
{
    internal static class Attribute
    {
        private const string FIND = nameof(Find);
        private const string FIND_BY_TAG = nameof(FindByTag);
        private const string GENERATE_AWAKE = nameof(GenerateAwake);
        private const string GENERATE_INITIALIZERS = nameof(GenerateInitializers);
        private const string GENERATE_ON_VALIDATE = nameof(GenerateOnValidate);
        private const string GET = nameof(Get);
        private const string GET_IN_CHILDREN = nameof(GetInChildren);
        private const string GET_IN_PARENT = nameof(GetInParent);
        private const string IGNORE_SELF = nameof(IgnoreSelf);
        private const string INCLUDE_INACTIVE = nameof(IncludeInactive);
        private const string INJECT_IN_EDITOR = nameof(InjectInEditor);
        private const string INJECT_IN_RUNTIME = nameof(InjectInRuntime);
        private const string SKIP_NULL_CHECK = nameof(SkipNullCheck);

        private static readonly string[] _validBaseAttributes =
        {
            FIND, FIND_BY_TAG, GET, GET_IN_CHILDREN, GET_IN_PARENT
        };

        private static readonly string[] _validGameObjectAttributes =
        {
            FIND, FIND_BY_TAG, GET, GET_IN_CHILDREN, GET_IN_PARENT
        };

        private static readonly string[] _validComponentAttributes =
        {
            GET, GET_IN_CHILDREN, GET_IN_PARENT
        };

        internal static bool HasFind(SyntaxNode node) => Helper.HasAttribute(node, FIND);
        internal static bool HasFindByTag(SyntaxNode node) => Helper.HasAttribute(node, FIND_BY_TAG);
        
        private static bool HasGenerateInitializers(SyntaxNode node) =>
            Helper.HasAttribute(node, GENERATE_INITIALIZERS);

        internal static bool HasGenerateAwake(SyntaxNode node) =>
            Helper.HasAttribute(node, GENERATE_AWAKE)
            || HasGenerateInitializers(node);
        internal static bool HasGenerateOnValidate(SyntaxNode node) =>
            Helper.HasAttribute(node, GENERATE_ON_VALIDATE)
            || HasGenerateInitializers(node);
        internal static bool HasGet(SyntaxNode node) => Helper.HasAttribute(node, GET);
        internal static bool HasGetInChildren(SyntaxNode node) => Helper.HasAttribute(node, GET_IN_CHILDREN);
        internal static bool HasGetInParent(SyntaxNode node) => Helper.HasAttribute(node, GET_IN_PARENT);
        internal static bool HasIgnoreSelf(SyntaxNode node) => Helper.HasAttribute(node, IGNORE_SELF);
        internal static bool HasIncludeInactive(SyntaxNode node) => Helper.HasAttribute(node, INCLUDE_INACTIVE);
        internal static bool HasInjectInEditor(SyntaxNode node) => Helper.HasAttribute(node, INJECT_IN_EDITOR);
        internal static bool HasInjectInRuntime(SyntaxNode node) => Helper.HasAttribute(node, INJECT_IN_RUNTIME);
        internal static bool HasSkipNullCheck(SyntaxNode node) => Helper.HasAttribute(node, SKIP_NULL_CHECK);

        internal static string GetFindParam(SyntaxNode syntaxNode, Compilation compilation) =>
            Helper.GetAttributeParam(syntaxNode, FIND, "gameObjectName", compilation);

        internal static string GetFindByTagParam(SyntaxNode syntaxNode, Compilation compilation) =>
            Helper.GetAttributeParam(syntaxNode, FIND_BY_TAG, "tagName", compilation);

        internal static bool HasAnyValidAttribute(SyntaxNode syntaxNode) =>
            _validBaseAttributes.Any(attribute => Helper.HasAttribute(syntaxNode, attribute));
        
        internal static bool HasGameObjectAttribute(SyntaxNode syntaxNode) =>
            _validGameObjectAttributes.Any(attribute => Helper.HasAttribute(syntaxNode, attribute));

        internal static bool HasComponentAttribute(SyntaxNode syntaxNode) =>
            _validComponentAttributes.Any(attribute => Helper.HasAttribute(syntaxNode, attribute));

        internal static string GetIncludeInactive(SyntaxNode syntaxNode) =>
            Helper.Toggle(HasIncludeInactive(syntaxNode), "true");

        internal static string GetInHierarchy(SyntaxNode syntaxNode) =>
            Helper.Toggle(HasGetInChildren(syntaxNode), "InChildren", "InParent");

    }
}