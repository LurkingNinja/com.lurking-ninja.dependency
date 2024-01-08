using System.Collections.Immutable;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LurkingNinja.SourceGenerator
{
    [Generator]
    internal class DiSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var syntaxNodes = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: IsSyntaxTargetForGeneration,
                    transform: Helper.HasAnyValidAttribute)
                .Where(IsNotNull);

            IncrementalValueProvider<(Compilation, ImmutableArray<SyntaxNode>)> compilationAndNodes
                = context.CompilationProvider.Combine(syntaxNodes.Collect());
            
            context.RegisterSourceOutput(compilationAndNodes, Execute);
        }

        private static void Execute(SourceProductionContext ctx, (Compilation, ImmutableArray<SyntaxNode>) results)
        {
            var (compilation, listOfNodes) = results;
            if (listOfNodes.IsDefaultOrEmpty) return;

            var className = Helper.GetClassNameOf(listOfNodes[0]);
            var cds = Helper.GetClassOf(listOfNodes[0]);

            if (!Helper.IsPartial(cds)) return;

            var sealedOrEmpty = Helper.GetIfSealed(cds);
            var usingDirectives = Helper.GetUsingDirectives(cds);
            var classAccessor = Helper.GetAccessOfClass(cds);

            var numberOfEntriesEditor = 0;
            var editorSource = new StringBuilder();
            var numberOfEntriesRuntime = 0;
            var runtimeSource = new StringBuilder();

            var oneLine = new StringBuilder();
            foreach (var syntaxNode in listOfNodes)
            {
                if (!Helper.TryGetFieldOrPropertyData(syntaxNode, out var type, out var fieldName)
                    || !Attribute.HasAnyValidAttribute(syntaxNode)) continue;

                var skipNullCheck = Attribute.HasSkipNullCheck(syntaxNode);
                var ignoreSelf = Attribute.HasIgnoreSelf(syntaxNode);
                var injectInPlay = Attribute.HasInjectInRuntime(syntaxNode);
                var injectInEditor = Attribute.HasInjectInEditor(syntaxNode) || !injectInPlay;

                var isArray = Helper.IsArray(syntaxNode);
                var isList = Helper.IsCollection(syntaxNode);
                var isCollection = isArray || isList;

                var baseType = Helper.GetBaseType(type);
                var hasGameObjectQuery = Attribute.HasGameObjectAttribute(syntaxNode);
                var hasComponentQuery = Attribute.HasComponentAttribute(syntaxNode);

                oneLine.Clear();
                
                if (!skipNullCheck)
                {
                    oneLine.Append("if (").Append(fieldName).Append(" == null");
                    if (isCollection) oneLine
                        .Append(" || ").Append(fieldName).Append(".Length == 0");
                    oneLine.Append(") ");

                }
                
                oneLine.Append(fieldName).Append(" = ");
                
                if (hasGameObjectQuery)
                    /* Find */          
                    if (Attribute.HasFind(syntaxNode))
                        if (isCollection) oneLine
                            .Append("UnityEngine.Object.FindObjectsByType")
                            .Append("<").Append(baseType).Append(">(FindObjectsSortMode.None)")
                            .Append(".Where(go => go.name == \"")
                            .Append(Attribute.GetFindParam(syntaxNode, compilation))
                            .Append("\")")
                            .Append(isList ? ".ToList()" : ".ToArray()");
                        else oneLine
                            .Append("GameObject.Find(\"")
                            .Append(Attribute.GetFindParam(syntaxNode, compilation))
                            .Append("\")");
                    /*FindWithTag*/  
                    else if (Attribute.HasFindByTag(syntaxNode)) oneLine
                        .Append("GameObject.FindWithTag(\"")
                        .Append(Attribute.GetFindByTagParam(syntaxNode, compilation))
                        .Append("\")");
                    else continue;
                else oneLine.Append("gameObject");

                if (hasComponentQuery)
                {
                    /* Get */
                    if (Attribute.HasGet(syntaxNode)) oneLine
                        .Append(".GetComponent")
                        .Append(Helper.Toggle(isCollection, "s"))
                        .Append("<").Append(type).Append(">").Append("()");
                    /* GetInChildren && GetInParent */
                    else if (Attribute.HasGetInChildren(syntaxNode)
                             || Attribute.HasGetInParent(syntaxNode))
                        if (isList || ignoreSelf) oneLine
                            .Append(".GetComponent")
                            .Append(Helper.Toggle(isCollection, "s"))
                            .Append(Attribute.GetInHierarchy(syntaxNode))
                            .Append("<").Append(baseType).Append(">(")
                            .Append(Attribute.GetIncludeInactive(syntaxNode))
                            .Append(")")
                            .Append(".Where(item => item.gameObject != gameObject)")
                            .Append(Helper.Toggle(isList, ".ToList()"));
                        else oneLine
                            .Append(".GetComponent")
                            .Append(Helper.Toggle(isCollection, "s"))
                            .Append(Attribute.GetInHierarchy(syntaxNode))
                            .Append("<").Append(baseType).Append(">(")
                            .Append(Attribute.GetIncludeInactive(syntaxNode))
                            .Append(")");
                        
                }

                if (hasGameObjectQuery && Attribute.HasFind(syntaxNode) && !isCollection)
                    oneLine.Append(".gameObject ");
                oneLine.Append(";");

                if (injectInEditor)
                {
                    editorSource.AppendLine(oneLine.ToString());
                    numberOfEntriesEditor++;
                    continue;
                }

                runtimeSource.AppendLine(oneLine.ToString());
                numberOfEntriesRuntime++;
            }
            
            var sourceBuilder = new StringBuilder();
            if (numberOfEntriesEditor > 0)
                sourceBuilder.AppendLine(Helper.InEditorResolve(editorSource.ToString()));
            if (numberOfEntriesRuntime > 0)
                sourceBuilder.AppendLine(Helper.InRuntimeResolve(runtimeSource.ToString()));
 
            if (Attribute.HasGenerateOnValidate(cds)) sourceBuilder.AppendLine(Helper.ON_VALIDATE_TEMPLATE);
            if (Attribute.HasGenerateAwake(cds)) sourceBuilder.AppendLine(Helper.AWAKE_TEMPLATE);
            
            Helper.AddSourceNs(
                ctx,
                className,
                usingDirectives,
                cds,
                Helper.ClassTemplateResolve(classAccessor, sealedOrEmpty, className, sourceBuilder.ToString()),
                true);
        }

        private static bool IsNotNull(SyntaxNode syntaxNode) => !(syntaxNode is null);

        private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode, CancellationToken token)
        {
            var pds = syntaxNode as PropertyDeclarationSyntax;
            var fds = syntaxNode as FieldDeclarationSyntax;
            var isProperty = !(pds is null);
            var isField = !(fds is null);

            if (!isProperty && !isField) return false;

            var attributeList = isProperty ? pds.AttributeLists : fds.AttributeLists;
            return attributeList.Count > 0;
        }
    }
}