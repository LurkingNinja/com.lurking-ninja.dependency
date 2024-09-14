using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LurkingNinja.SourceGenerator
{
    internal static class Helper
    {
        private const string _FILENAME_POSTFIX = "_codegen.cs";

        /*
         * {0} name space if exists
         * {1} closing bracket for namespace if needed
         * {2} class definition
         * {3} using directives
         */
        private const string _FILE_TEMPLATE = @"#pragma warning disable CS0105 // multiple using directive warning
using System.Linq;
using System.Collections.Generic;
{3}
#pragma warning restore CS0105 // multiple using directive warning

{0}
    {2}
{1}";

        internal static string FileTemplateResolve(string usingDirectives, string nameSpace, string source)
        {
            var ns = GetNamespaceTemplate(nameSpace);
            return string.Format(_FILE_TEMPLATE,
                /*{0}*/ns.Item1,
                /*{1}*/ns.Item2,
                /*{2}*/source,
                /*{3}*/usingDirectives);
        }

        /*
         * {0} class accessor
         * {1} "sealed " or empty
         * {2} class name
         * {3} class source
         */
        private const string _CLASS_TEMPLATE = @"{0} partial {1}class {2} {{
        {3}
}}";

        internal static string ClassTemplateResolve(
                string accessor, string sealedOrEmpty, string className, string source) =>
            string.Format(_CLASS_TEMPLATE,
                /*{0}*/accessor,
                /*{1}*/sealedOrEmpty,
                /*{2}*/className,
                /*{3}*/source);

        /*
         * {0} source
         * {1} accessor -  private, in tests: public
         */
        private const string _INITIALIZE_EDITOR_TEMPLATE = @"#if UNITY_EDITOR
{1} void InitializeInEditor() {{
    {0}
}}
#endif";

        internal static string InEditorResolve(string methodSource, bool isPublic = false) =>
            string.Format(_INITIALIZE_EDITOR_TEMPLATE,
                /*{0}*/methodSource,
                /*{1}*/Toggle(isPublic, "public", "private"));
        
        /*
         * {0} source
         */
        private const string _INITIALIZE_RUNTIME_TEMPLATE = @"{1} void InitializeInRuntime() {{
    {0}
}}";

        internal static string InRuntimeResolve(string methodSource, bool isPublic = false) =>
            string.Format(_INITIALIZE_RUNTIME_TEMPLATE,
                /*{0}*/methodSource,
                /*{1}*/Toggle(isPublic, "public", "private"));

        internal const string AWAKE_TEMPLATE = @"private void private void Awake() => InitializeInRuntime();";

        internal const string ON_VALIDATE_TEMPLATE = @"#if UNITY_EDITOR
private void OnValidate() => InitializeInEditor();
#endif";
        
        internal static string GetBaseType(string type) => type
                .Replace("[]", string.Empty)
                .Replace("List<", string.Empty)
                .Replace(">", "");

        internal static string GetClassNameOf(SyntaxNode node) =>
            GetClassOf(node).Identifier.ValueText;

        internal static string GetIfSealed(ClassDeclarationSyntax cds) =>
            IsSealed(cds) ? "sealed " : string.Empty;

        private static bool IsSealed(MemberDeclarationSyntax cds) =>
            cds.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.SealedKeyword));

        private static bool IsPublic(ClassDeclarationSyntax cds) =>
            cds.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword));

        private static bool IsPrivate(ClassDeclarationSyntax cds) =>
            cds.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PrivateKeyword));

        internal static bool IsCollection(SyntaxNode syntaxNode, string whatCollection  = "List")
        {
            switch (syntaxNode)
            {
                case FieldDeclarationSyntax fds:
                    return fds.Declaration.Type.ToString().Contains(whatCollection);
                case PropertyDeclarationSyntax pds:
                    return pds.Type.ToString().Contains(whatCollection);
                default:
                    return false;
            }
        }
        
        internal static bool IsArray(SyntaxNode syntaxNode) 
        {
            switch (syntaxNode)
            {
                case FieldDeclarationSyntax fds:
                    return fds.Declaration.Type.IsKind(SyntaxKind.ArrayType);
                case PropertyDeclarationSyntax pds:
                    return pds.Type.IsKind(SyntaxKind.ArrayType);
                default:
                    return false;
            }
        }

        internal static string GetAccessOfClass(ClassDeclarationSyntax cds) =>
            IsPublic(cds) ? "public" : IsPrivate(cds) ? "private" : "internal";

        internal static ClassDeclarationSyntax GetClassOf(SyntaxNode node)
        {
            foreach (var syntaxNode in node.Ancestors())
            {
                if (syntaxNode is ClassDeclarationSyntax cds) return cds;
            }

            return null;
        }
        
        internal static bool IsPartial(ClassDeclarationSyntax cds) =>
            cds.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));

        internal static SyntaxNode HasAnyValidAttribute(GeneratorSyntaxContext ctx, CancellationToken token) =>
            Attribute.HasAnyValidAttribute(ctx.Node) ? ctx.Node : null;

        private static SyntaxList<AttributeListSyntax> GetAttributeList(SyntaxNode syntaxNode)
        {
            switch (syntaxNode)
            {
                case FieldDeclarationSyntax fds:
                    return fds.AttributeLists;
                case PropertyDeclarationSyntax pds:
                    return pds.AttributeLists;
                case ClassDeclarationSyntax pds:
                    return pds.AttributeLists;
                default:
                    return new SyntaxList<AttributeListSyntax>();
            }
        }
        internal static bool HasAttribute(SyntaxNode syntaxNode, string attributeName) =>
            GetAttributeList(syntaxNode)
                .SelectMany(nodeAttribute => nodeAttribute.Attributes)
                .Any(attribute => attribute.Name.ToString().Trim().ToLower()
                    .Equals(attributeName.Trim().ToLower()));

        internal static bool TryGetFieldOrPropertyData(SyntaxNode syntaxNode, out string type, out string fieldName)
        {
            switch (syntaxNode)
            {
                case PropertyDeclarationSyntax pds:
                    type = pds.Type.ToString();
                    fieldName = pds.Identifier.ValueText;
                    return true;
                case FieldDeclarationSyntax fds:
                {
                    type = fds.Declaration.Type.ToString();
                    fieldName = fds.Declaration.Variables[0].Identifier.ValueText;
                    return true;
                }
            }
            
            type = null;
            fieldName = null;
            return false;
        }

        internal static string GetAttributeParam(SyntaxNode syntaxNode, string attribute, string parameter, Compilation comp)
        {
            var parentSyntaxTree = syntaxNode.Parent?.SyntaxTree;
            if (parentSyntaxTree is null) return string.Empty;
            
            var semanticModel = comp?.GetSemanticModel(parentSyntaxTree, true);
            ISymbol symbol;
            switch (syntaxNode)
            {
                case FieldDeclarationSyntax fds:
                    symbol = semanticModel.GetDeclaredSymbol(fds.Declaration.Variables.First());
                    break;
                case PropertyDeclarationSyntax pds:
                    symbol = semanticModel.GetDeclaredSymbol(pds);
                    break;
                default:
                    return null;
            }

            if (symbol is null) return null;

            foreach (var attributeData in symbol.GetAttributes())
            {
                if (attributeData.AttributeClass is null
                    || !attributeData.AttributeClass.Name.Equals(attribute)) continue;
                if (attributeData.ConstructorArguments.Length > 0)
                    return attributeData.ConstructorArguments.First().Value?.ToString();
                    
                if (attributeData.NamedArguments.Length < 1) return null;

                parameter = parameter.Trim();
                foreach (var namedArgument in attributeData.NamedArguments)
                {
                    if (namedArgument.Key.Trim() == parameter)
                        return namedArgument.Value.ToString();
                }
            }

            return null;
        }
        
        private static void AddSource(SourceProductionContext context, string fileName, string source) =>
            context.AddSource($"{fileName}{_FILENAME_POSTFIX}", source);
        
        internal static void AddSourceNs(SourceProductionContext ctx, string filename,
                string usingDirectives, ClassDeclarationSyntax cds, string source, bool log = false)
        {
            source = FileTemplateResolve(usingDirectives, GetNamespace(cds), source);
            AddSource(ctx, filename, source);
            
            if (!log) return;
            //ctx.ReportDiagnostic(Diagnostic.Create(logMessage, cds.GetLocation(), source));
            Log(source);
        }

        internal static string Toggle(bool isOn, string ifOn) => isOn ? ifOn : string.Empty;
        internal static string Toggle(bool isOn, string ifOn, string ifOff) => isOn ? ifOn : ifOff;

        private static string GetNamespace(SyntaxNode node)
        {
            var nameSpace = string.Empty;
            var potentialNamespaceParent = node.Parent;

            while (potentialNamespaceParent != null
                   && !(potentialNamespaceParent is NamespaceDeclarationSyntax) 
                   && !(potentialNamespaceParent is FileScopedNamespaceDeclarationSyntax))
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }
            

            if (!(potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)) return nameSpace;
            
            nameSpace = namespaceParent.Name.ToString();

            while (true)
            {
                if (!(namespaceParent.Parent is NamespaceDeclarationSyntax parent)) break;

                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }

            return string.IsNullOrEmpty(nameSpace)
                ? string.Empty
                : nameSpace;
        }

        private static (string, string) GetNamespaceTemplate(string potentialNamespace)
        {
            var isNullOrEmpty = string.IsNullOrEmpty(potentialNamespace); 
            return (
                isNullOrEmpty
                    ? string.Empty
                    : $"namespace {potentialNamespace}\n{{",
                isNullOrEmpty
                    ? string.Empty
                    : "}");
        }

        private static void GetFileScopedNamespaceUsings(ClassDeclarationSyntax cds, HashSet<string> usings)
        {
            SyntaxNode toCheck = cds;
            while (toCheck != null)
            {
                if (toCheck.IsKind(SyntaxKind.NamespaceDeclaration))
                {
                    if (!(toCheck is NamespaceDeclarationSyntax ns)) continue;
                    
                    foreach (var namespaceChild in ns.ChildNodes())
                    {
                        if (namespaceChild.IsKind(SyntaxKind.UsingDirective))
                            usings.Add(namespaceChild.ToString());
                    }
                }
                toCheck = toCheck.Parent;
            }
        }
        
        internal static string GetUsingDirectives(ClassDeclarationSyntax cds)
        {
            var usingDirectives = new HashSet<string>();
            foreach (var child in cds.SyntaxTree.GetRoot().ChildNodes())
                if (child.IsKind(SyntaxKind.UsingDirective))
                    usingDirectives.Add(child.ToString());

            GetFileScopedNamespaceUsings(cds, usingDirectives);
            usingDirectives.Add("using System;");
            usingDirectives.Add("using UnityEngine;");

            return string.Join("\n", usingDirectives);
        }

        internal static string GetLeftHandSide(
            bool hasSkipNullCheck, string fieldName, bool isArray, bool isList)
        {
            var oneLine = new StringBuilder();
            if (!hasSkipNullCheck)
            {
                oneLine.Append("if (").Append(fieldName).Append(" == null");
                if (isArray) oneLine
                    .Append(" || ").Append(fieldName).Append(".Length == 0");
                else if (isList) oneLine
                    .Append(" || ").Append(fieldName).Append(".Count == 0");
                oneLine.Append(") ");
            }
            oneLine.Append(fieldName).Append(" = ");
            
            return oneLine.ToString();
        }

        internal static void Log(string text) =>
            File.AppendAllText("D:\\DI-SourceGenerator.log", $"{text}\n");
    }
}