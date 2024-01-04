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
                    transform: Common.HasAnyAttribute)
                .Where(IsNotNull);

            IncrementalValueProvider<(Compilation, ImmutableArray<SyntaxNode>)> compilationAndNodes
                = context.CompilationProvider.Combine(syntaxNodes.Collect());
            
            context.RegisterSourceOutput(compilationAndNodes, Execute);
        }

        private static void Execute(SourceProductionContext ctx, (Compilation, ImmutableArray<SyntaxNode>) results)
        {
            var (compilation, listOfNodes) = results;
            if (listOfNodes.IsDefaultOrEmpty) return;
            
            var className = Common.GetClassNameOf(listOfNodes[0]);
            var cds = Common.GetClassOf(listOfNodes[0]);

            if (!Common.IsPartial(cds)) return;
            
            var usingDirectives = Common.GetUsingDirectives(cds);
            var isSealed = Common.GetIfSealed(cds);
            var classAccessor = Common.GetAccessOfClass(cds);
            var source = new StringBuilder();
            var sourceForEditor = new StringBuilder();
            var sourceForPlay = new StringBuilder();
            
            /*BeginClass*/source.AppendLine($"{classAccessor} partial {isSealed}class {className} {{");
            
            sourceForEditor.AppendLine("#if UNITY_EDITOR");
            sourceForEditor.AppendLine("\tprivate void InjectInEditor() {");

            sourceForPlay.AppendLine("\tprivate void InjectInPlay() {");

            var numberOfEntriesEditor = 0;
            var numberOfEntriesPlay = 0;
            
            var theLine = new StringBuilder();
            foreach (var syntaxNode in listOfNodes)
            {
                if (!Common.TryGetFieldOrPropertyData(syntaxNode, out var type, out var fieldName)) continue;
                
                var skipNullCheck = Common.HasAttribute(syntaxNode, Common.SKIP_NULL_CHECK_ATTRIBUTE);
                var ignoreSelf = Common.HasAttribute(syntaxNode, Common.IGNORE_SELF_ATTRIBUTE);
                var injectInPlay = Common.HasAttribute(syntaxNode, Common.INJECT_IN_PLAY_ATTRIBUTE);
                
                var isArray =  Common.IsArray(syntaxNode);
                var isCollection = Common.IsCollection(syntaxNode, "Collection");
                var isList = Common.IsCollection(syntaxNode);
                var isCompoundType = isArray || isList || isCollection;
                
                if (isArray) type = type
                        .Replace("[]", string.Empty);
                if (isList) type = type
                        .Replace("List<",  string.Empty)
                        .Replace(">", string.Empty);
                if (isCollection) type = type
                        .Replace("Collection<",  string.Empty)
                        .Replace(">", string.Empty);
                sourceForEditor.Append("\t\t");

                theLine.Clear();
                /* Get */if (Common.HasAttribute(syntaxNode, Common.GET_ATTRIBUTE))
                {
                    var s = Common.Toggle(isCompoundType, "s");
                    var oneLine = $"{fieldName} = GetComponent{s}<{type}>();";
                    var orEmpty = Common.Toggle(isCollection, $" || {fieldName}.Length == 0");
                    oneLine = Common.Toggle(
                        skipNullCheck, oneLine, $"if({fieldName} == null{orEmpty}) {oneLine}");
                    
                    theLine.AppendLine(oneLine);
                }
                /* GetByName */else if (Common.HasAttribute(syntaxNode, Common.GET_BY_NAME_ATTRIBUTE))
                {
                    var gameObjectName = Common.GetAttributeParam(
                        syntaxNode, Common.GET_BY_NAME_ATTRIBUTE, "gameObjectName", compilation);
                    var s = Common.Toggle(isCompoundType, "s");
                    
                    var oneLine = $"{fieldName} = GameObject.Find(\"{gameObjectName}\").GetComponent{s}<{type}>();";
                    oneLine = Common.Toggle(skipNullCheck, oneLine, $"if({fieldName} == null) {oneLine}");
                    
                    theLine.AppendLine(oneLine);
                }
                /* GetByTag */else if (Common.HasAttribute(syntaxNode, Common.GET_BY_TAG_ATTRIBUTE))
                {
                    var tagName = Common.GetAttributeParam(
                        syntaxNode, Common.GET_BY_TAG_ATTRIBUTE, "tagName", compilation);
                    var s = Common.Toggle(isCompoundType, "s");
                    
                    var oneLine = $"{fieldName} = GameObject.FindWithTag(\"{tagName}\").GetComponent{s}<{type}>();";
                    oneLine = Common.Toggle(skipNullCheck, oneLine, $"if({fieldName} == null) {oneLine}");
                    
                    theLine.AppendLine(oneLine);                    
                }
                /* GetInAssets */else if (Common.HasAttribute(syntaxNode, Common.GET_IN_ASSETS_ATTRIBUTE))
                {
                    var assetPath = Common.GetAttributeParam(
                        syntaxNode, Common.GET_IN_ASSETS_ATTRIBUTE, "assetSearchPath", compilation);

                    // Can't inject AssetDatabase in Play.
                    injectInPlay = false;
                    
                    theLine.Append("{");
                    if (!skipNullCheck) theLine.Append($"if({fieldName} == null) {{ ");
                    theLine.Append($" var guids = UnityEditor.AssetDatabase.FindAssets(\"t:{type} {assetPath}\"); ");
                    theLine.Append($"if (guids.Length  > 0) {{ {fieldName} = UnityEditor.AssetDatabase.LoadAssetAtPath<{type}>(UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0])); }} ");
                    if (!skipNullCheck) theLine.AppendLine("}");
                    theLine.AppendLine("}");
                }
                /* GetInChildren */else if (Common.HasAttribute(syntaxNode, Common.GET_IN_CHILDREN_ATTRIBUTE))
                {
                    var includeInactive = Common.HasAttribute(syntaxNode,
                        Common.INCLUDE_INACTIVE_ATTRIBUTE) ? "true" : string.Empty;
                    var s = Common.Toggle(isCompoundType, "s");

                    string oneLine;
                    if (isList || isCollection || ignoreSelf)
                    {
                        var lineSb = new StringBuilder();
                        lineSb.AppendLine($"{{var tempList = new System.Collections.Generic.List<{type}>(); ");
                        lineSb.AppendLine($"foreach(var item  in GetComponent{s}InChildren<{type}>({includeInactive})) {{");
                        lineSb.AppendLine("if (item.gameObject != gameObject) {tempList.Add(item);}}");
                        lineSb.AppendLine(isArray ? $"{fieldName}=tempList.ToArray();}} " : isList ? $"{fieldName}=tempList;}}" : $"{fieldName}=new Collection<{type}>(tempList);}}");
                        oneLine = lineSb.ToString();  
                    } else
                    {
                        oneLine = $"{fieldName} = GetComponent{s}InChildren<{type}>({includeInactive});";
                        oneLine = Common.Toggle(skipNullCheck, oneLine, $"if({fieldName} == null) {oneLine}");
                    }
                    
                    theLine.AppendLine(oneLine);
                }
                /* GetInParent */else if (Common.HasAttribute(syntaxNode, Common.GET_IN_PARENT_ATTRIBUTE))
                {
                    var includeInactive = Common.HasAttribute(syntaxNode,
                        Common.INCLUDE_INACTIVE_ATTRIBUTE) ? "true" : string.Empty;
                    var s = Common.Toggle(isCompoundType, "s");

                    string oneLine;
                    if (isList || isCollection || ignoreSelf)
                    {
                        var lineSb = new StringBuilder();
                        lineSb.AppendLine($"{{var tempList = new System.Collections.Generic.List<{type}>(); ");
                        lineSb.AppendLine($"foreach(var item  in GetComponent{s}InParent<{type}>({includeInactive})) {{");
                        lineSb.AppendLine("if (item.gameObject != gameObject) {tempList.Add(item);}}");
                        lineSb.AppendLine(isArray ? $"{fieldName}=tempList.ToArray();}} " : isList ? $"{fieldName}=tempList;}}" : $"{fieldName}=new Collection<{type}>(tempList);}}");
                        oneLine = lineSb.ToString();  
                    } else
                    {
                        oneLine = $"{fieldName} = GetComponent{s}InParent<{type}>({includeInactive});";
                        oneLine = Common.Toggle(skipNullCheck, oneLine, $"if({fieldName} == null) {oneLine}");
                    }
                    
                    theLine.AppendLine(oneLine);
                }

                if (injectInPlay)
                {
                    sourceForPlay.AppendLine(theLine.ToString());
                    numberOfEntriesPlay++;
                    continue;
                }
                
                sourceForEditor.AppendLine(theLine.ToString());
                numberOfEntriesEditor++;
            }

            sourceForEditor.AppendLine("\t}");
            /*EndInjectInEditorMethod*/sourceForEditor.AppendLine("#endif");
            /*EndInjectInPlayMethod*/sourceForPlay.AppendLine("\t}");
            
            if (numberOfEntriesEditor > 0)
                source.AppendLine(sourceForEditor.ToString());
            if (numberOfEntriesPlay > 0)
                source.AppendLine(sourceForPlay.ToString());
            /*EndClass*/source.AppendLine("\n}");

            Common.AddSourceNs(ctx, className, usingDirectives, cds, source.ToString(), true);
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