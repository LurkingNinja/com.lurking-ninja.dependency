namespace LurkingNinja.SourceGenerator
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Text;
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            var (compilation, listOfOriginalNodes) = results;
            if (listOfOriginalNodes.IsDefaultOrEmpty) return;

            var listOfNodes = new Dictionary<string, List<SyntaxNode>>();
            foreach (var node in listOfOriginalNodes)
            {
                var className = Helper.GetClassNameOf(node);
                if (!listOfNodes.ContainsKey(className))
                    listOfNodes.Add(className, new List<SyntaxNode>());
                listOfNodes[className].Add(node);
            }

            foreach (var nodeKeyValue in listOfNodes)
            {
                var className = nodeKeyValue.Key;
                var nodes = nodeKeyValue.Value;
                
                var cds = Helper.GetClassOf(nodes[0]);
                if (!Helper.IsPartial(cds)) return;

                var sealedOrEmpty = Helper.GetIfSealed(cds);
                var usingDirectives = Helper.GetUsingDirectives(cds);
                var classAccessor = Helper.GetAccessOfClass(cds);
                var doTestHelpers = Attribute.HasTestHelpers(cds);

                var numberOfEntriesEditor = 0;
                var editorSource = new StringBuilder();
                var numberOfEntriesRuntime = 0;
                var runtimeSource = new StringBuilder();

                var oneLine = new StringBuilder();
                var testLines = new StringBuilder();
                foreach (var syntaxNode in nodes)
                {
                    if (!Helper.TryGetFieldOrPropertyData(syntaxNode, out var type, out var fieldName)
                        || !Attribute.HasAnyValidAttribute(syntaxNode)) continue;

                    var ignoreSelf = Attribute.HasIgnoreSelf(syntaxNode);
                    var injectInPlay = Attribute.HasInjectInRuntime(syntaxNode);
                    var injectInEditor = Attribute.HasInjectInEditor(syntaxNode) || !injectInPlay;

                    var isArray = Helper.IsArray(syntaxNode);
                    var isList = Helper.IsCollection(syntaxNode);
                    var isCollection = isArray || isList;

                    var baseType = Helper.GetBaseType(type);
                    var isGameObjectBase = baseType == "GameObject";

                    oneLine.Clear().Append("            ");

                    var leftHandSide =
                        Helper.GetLeftHandSide(!Attribute.HasSkipNullCheck(syntaxNode), fieldName, isArray, isList);

                    var hasFind = Attribute.HasFind(syntaxNode);
                    var hasGet = Attribute.HasGet(syntaxNode);
                    var hasAdd = Attribute.HasAdd(syntaxNode);
                    var hasFindWithTag = Attribute.HasFindWithTag(syntaxNode);
                    var hasInChildren = Attribute.HasGetInChildren(syntaxNode);
                    var hasInParent = Attribute.HasGetInParent(syntaxNode);
                    
                    if (!isCollection && isGameObjectBase && hasFind)
                    {
                        // Attribute: [Find("Main Camera")] -> Object.FindObjectsByType<GameObject>.First
                        // Type: GameObject
                        // Available tag-attributes: IncludeInactive, StableSort, IgnoreSelf

                        var findParam = Attribute.GetFindParam(syntaxNode, compilation);
                        var strIncludeInactive = Helper.Toggle(Attribute.HasIncludeInactive(syntaxNode),
                            "Include", "Exclude");
                        var stableSort = Helper.Toggle(Attribute.HasStableSort(syntaxNode),
                            "InstanceID", "None");
                        var doIgnoreSelf = Helper.Toggle(ignoreSelf, " go != gameObject &&");
                        
                        oneLine.Append(leftHandSide).Append("FindObjectsByType<GameObject>(FindObjectsInactive.")
                            .Append(strIncludeInactive).Append(", FindObjectsSortMode.").Append(stableSort)
                            .Append(").First(go =>").Append(doIgnoreSelf).Append(@" go.name == """)
                            .Append(findParam).Append(@""")")
                            .AppendLine(";");

                    } else if (isCollection && isGameObjectBase && hasFind)
                    {
                        // Attribute: [Find("ChildWithAudioSource")] -> Object.FindObjectsByType<GameObject>
                        // Type: GameObject[]
                        // Available tag-attributes: IncludeInactive, StableSort, IgnoreSelf
                        
                        var findParam = Attribute.GetFindParam(syntaxNode, compilation);
                        var strIncludeInactive = Helper.Toggle(Attribute.HasIncludeInactive(syntaxNode),
                            "Include", "Exclude");
                        var stableSort = Helper.Toggle(Attribute.HasStableSort(syntaxNode),
                            "InstanceID", "None");
                        var doIgnoreSelf = Helper.Toggle(ignoreSelf, " go != gameObject &&");
                        var doArray = Helper.Toggle(isArray, ".ToArray()", ".ToList()");

                        oneLine.Append(leftHandSide).Append("FindObjectsByType<GameObject>(")
                            .Append("FindObjectsInactive.").Append(strIncludeInactive)
                            .Append(", FindObjectsSortMode.").Append(stableSort)
                            .Append(").Where(go =>").Append(doIgnoreSelf).Append(@" go.name == """)
                            .Append(findParam).Append(@""")").Append(doArray)
                            .AppendLine(";");
                        
                    } else if (!isCollection && !isGameObjectBase && hasGet && !hasFindWithTag)
                    {
                        //  Attribute: [Get] -> GetComponent<BaseType>()
                        // Type: Component

                        oneLine.Append("{ var temp = ").Append("GetComponent<").Append(baseType).AppendLine(">();");
                        if (hasAdd)
                        {
                            oneLine.Append("            if (temp == null) temp = gameObject.AddComponent<")
                                    .Append(baseType).AppendLine(">();");
                        }

                        oneLine.Append("            ").Append(leftHandSide).Append("temp").AppendLine("; }");

                    } else if (isCollection && !isGameObjectBase && hasGet && !hasFind && !hasFindWithTag)
                    {
                        // Attribute: [Get] -> GetComponents<BaseType>()
                        // Type: Component[]
                        // // [Add] support
                        
                        var doArray = Helper.Toggle(isArray, ".ToArray()");
                        
                        oneLine.Append("{ List<").Append(baseType).Append("> temp = new(").Append("GetComponents<")
                            .Append(baseType).AppendLine(">());");
                        if (hasAdd)
                        {
                            oneLine.Append("            if (temp == null || temp.Count == 0)")
                                    .Append(" temp.Add(gameObject.AddComponent<").Append(baseType).AppendLine(">());");
                        }

                        oneLine.Append("            ").Append(leftHandSide).Append("temp").Append(doArray).AppendLine("; }");
                        
                    } else if (!isCollection && !isGameObjectBase && hasFind && hasGet)
                    {
                        // Attribute: [Find("name")][Get] ->  Object.FindObjectsByType<GameObject>.foreach().GetComponent<BaseType).First()
                        // Type: Component
                        // Available tag-attributes: IncludeInactive, StableSort, IgnoreSelf
                        // No [Add] support
                        
                        var findParam = Attribute.GetFindParam(syntaxNode, compilation);
                        var strIncludeInactive = Helper.Toggle(Attribute.HasIncludeInactive(syntaxNode),
                            "Include", "Exclude");
                        var stableSort = Helper.Toggle(Attribute.HasStableSort(syntaxNode),
                            "InstanceID", "None");
                        var doIgnoreSelf = Helper.Toggle(ignoreSelf, " go != gameObject && "); 

                        oneLine
                            .Append("{")
                                .Append(baseType).AppendLine(" res = null;")
                                .Append("           foreach(var go in FindObjectsByType<GameObject>(")
                                .Append("FindObjectsInactive.").Append(strIncludeInactive)
                                .Append(", FindObjectsSortMode.").Append(stableSort).AppendLine(")) {")
                                    .Append("           if (").Append(doIgnoreSelf)
                                    .Append("go.name == \"").Append(findParam).AppendLine("\") {")
                                    .Append("           if (go.TryGetComponent<").Append(baseType).AppendLine(">(out res)) break;")
                                    .AppendLine("           }}")
                                .Append("           ").Append(leftHandSide).Append("res")
                                .AppendLine(";")
                            .AppendLine("           }");

                    } else if (isCollection && !isGameObjectBase && hasFind && hasGet)
                    {
                        // Attribute: [Find("name")][Get] ->   Object.FindObjectsByType<GameObject>.foreach().GetComponents<BaseType).All()
                        // Type: Component[]
                        // Available tag-attributes: IncludeInactive, StableSort, IgnoreSelf
                        // No [Add] support

                        var findParam = Attribute.GetFindParam(syntaxNode, compilation);
                        var strIncludeInactive = Helper.Toggle(Attribute.HasIncludeInactive(syntaxNode),
                            "Include", "Exclude");
                        var stableSort = Helper.Toggle(Attribute.HasStableSort(syntaxNode),
                            "InstanceID", "None");
                        var doIgnoreSelf = Helper.Toggle(ignoreSelf, " go != gameObject && ");
                        var doArray = Helper.Toggle(isArray, ".ToArray()");

                        oneLine
                            .Append("{")
                                .Append("var res = new List<").Append(baseType).AppendLine(">();")
                                .Append("           foreach(var go in FindObjectsByType<GameObject>(")
                                .Append("FindObjectsInactive.").Append(strIncludeInactive)
                                .Append(", FindObjectsSortMode.").Append(stableSort).AppendLine(")) {")
                                    .Append("           if (").Append(doIgnoreSelf)
                                    .Append("go.name == \"").Append(findParam).AppendLine("\") {")
                                    .Append("           res.AddRange(go.GetComponents<").Append(baseType).AppendLine(">());")
                                    .AppendLine("           }}")
                                .Append("           ").Append(leftHandSide).Append("res").Append(doArray).AppendLine(";")
                            .AppendLine("           }");
                        
                    } else if (!isCollection && isGameObjectBase && hasFindWithTag)
                    {
                        // Attribute: [FindWithTag("tag")] -> GameObject.FindWithTag(tagName)
                        // Type: GameObject
                        
                        var findParam = Attribute.GetFindWithTagParam(syntaxNode, compilation);

                        oneLine
                            .Append(leftHandSide)
                            .Append("GameObject.FindWithTag(\"").Append(findParam).AppendLine("\");");
                        
                    } else if (isCollection && isGameObjectBase && hasFindWithTag)
                    {
                        // Attribute: [FindWithTag("tag")] -> GameObject.FindGameObjectsWithTag(tagName)
                        // Type: GameObject[]
                        
                        var findParam = Attribute.GetFindWithTagParam(syntaxNode, compilation);
                        
                        oneLine
                            .Append(leftHandSide)
                            .Append("GameObject.FindGameObjectsWithTag(\"").Append(findParam).AppendLine("\");");
                        
                    } else if (!isCollection && !isGameObjectBase && hasFindWithTag)
                    {
                        // Attribute: [FindWithTag("tag")][Get] -> GameObject.FindGameObjectsWithTag(tagName).each().GetComponent<BaseType>().First()
                        // Type: Component

                        var findParam = Attribute.GetFindWithTagParam(syntaxNode, compilation);

                        oneLine
                            .Append("{")
                                .Append(baseType).AppendLine(" res = null; ")
                                .Append("            foreach (var go in GameObject.FindGameObjectsWithTag(\"")
                                    .Append(findParam).Append("\")) { if (go.TryGetComponent<")
                                    .Append(baseType).AppendLine(">(out res)) break; }")
                                .Append("            ").Append(leftHandSide).Append(" res").Append(";")
                            .AppendLine(" }");

                    } else if (isCollection && !isGameObjectBase && hasFindWithTag && hasGet)
                    {
                        // Attribute: [FindWithTag("tag")][Get] -> GameObject.FindGameObjectsWithTag(tagName).each().GetComponents<BaseType>().All()
                        // Type: Component[]
                        // No [Add] support 

                        var findParam = Attribute.GetFindWithTagParam(syntaxNode, compilation);
                        var doArray = Helper.Toggle(isArray, ".ToArray()");

                        oneLine
                            .Append("{")
                                .Append("var res = new List<").Append(baseType).AppendLine(">();")
                                .Append("           foreach (var go in GameObject.FindGameObjectsWithTag(\"")
                                    .Append(findParam).Append("\")) { res.AddRange(go.GetComponents<")
                                    .Append(baseType).AppendLine(">()); }")
                                .Append("           ").Append(leftHandSide).Append(" res").Append(doArray).AppendLine(";")
                            .AppendLine("           }");

                    } else if (!isCollection && !isGameObjectBase && hasInChildren)
                    {
                        // Attribute: [GetInChildren] -> GetComponentsInChildren<BaseType>()
                        // Type: Component
                        // Available tag-attributes: IncludeInactive, IgnoreSelf
                        
                        var strIncludeInactive = Helper.Toggle(Attribute.HasIncludeInactive(syntaxNode),
                            "true", "false");
                        var strIgnoreSelf = Helper.Toggle(ignoreSelf,
                            "comp  => comp.gameObject != gameObject");

                        oneLine.Append(leftHandSide).Append("GetComponentsInChildren<")
                            .Append(baseType).Append(">(").Append(strIncludeInactive).Append(").First(");
                        oneLine.Append(strIgnoreSelf).AppendLine(");");

                    } else if (isCollection && !isGameObjectBase && hasInChildren)
                    {
                        // Attribute: [GetInChildren] -> GetComponentsInChildren<BaseType>()
                        // Type: Component[]
                        // Available tag-attributes: IncludeInactive, IgnoreSelf
                        
                        var strIncludeInactive = Helper.Toggle(Attribute.HasIncludeInactive(syntaxNode),
                            "true", "false");
                        var strIgnoreSelf = Helper.Toggle(ignoreSelf,
                            ".Where(comp  => comp.gameObject != gameObject)");
                        var doArray = Helper.Toggle(isArray, ".ToArray()", ".ToList()");
                        
                        oneLine.Append(leftHandSide).Append("GetComponentsInChildren<")
                            .Append(baseType).Append(">(").Append(strIncludeInactive).Append(")");
                        oneLine.Append(strIgnoreSelf).Append(doArray).AppendLine(";");

                    } else if (!isCollection && !isGameObjectBase && hasInParent)
                    {
                        // Attribute: [GetInParent] -> GetComponentsInParent<BaseType>()
                        // Type: Component
                        // Available tag-attributes: IncludeInactive, IgnoreSelf
                        
                        var strIncludeInactive = Helper.Toggle(Attribute.HasIncludeInactive(syntaxNode),
                            "true", "false");
                        var strIgnoreSelf = Helper.Toggle(ignoreSelf,
                            "comp  => comp.gameObject != gameObject");
                        // First(comp => comp.gameObject != gameObject);
                        oneLine.Append(leftHandSide).Append("GetComponentsInParent<")
                            .Append(baseType).Append(">(").Append(strIncludeInactive).Append(").First(");
                        oneLine.Append(strIgnoreSelf).AppendLine(");");

                    }
                    else if (isCollection && !isGameObjectBase && hasInParent)
                    {
                        // Attribute: [GetInParent] -> GetComponentsInParent<BaseType>()
                        // Type: Component[]
                        // Available tag-attributes: IncludeInactive, IgnoreSelf
                        
                        var strIncludeInactive = Helper.Toggle(Attribute.HasIncludeInactive(syntaxNode),
                            "true", "false");
                        var strIgnoreSelf = Helper.Toggle(ignoreSelf,
                            ".Where(comp  => comp.gameObject != gameObject)");
                        var doArray = Helper.Toggle(isArray, ".ToArray()", ".ToList()");
                        
                        oneLine.Append(leftHandSide).Append("GetComponentsInParent<")
                            .Append(baseType).Append(">(").Append(strIncludeInactive).Append(")");
                        oneLine.Append(strIgnoreSelf).Append(doArray).AppendLine(";");

                    } else continue;

                    if (doTestHelpers)
                    {
                        testLines.Append("public ").Append(type).Append(" Get").Append(fieldName)
                                .Append(" { get => ").Append(fieldName)
                                .Append("; set => ").Append(fieldName).AppendLine(" = value; }");
                    }

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
                    sourceBuilder.AppendLine(Helper.InEditorResolve(editorSource.ToString(), doTestHelpers));
                if (numberOfEntriesRuntime > 0)
                    sourceBuilder.AppendLine(Helper.InRuntimeResolve(runtimeSource.ToString(), doTestHelpers));
     
                if (Attribute.HasGenerateOnValidate(cds)) sourceBuilder.AppendLine(Helper._ON_VALIDATE_TEMPLATE);
                if (Attribute.HasGenerateAwake(cds)) sourceBuilder.AppendLine(Helper._AWAKE_TEMPLATE);

                if (doTestHelpers) sourceBuilder.AppendLine(testLines.ToString());
                
                Helper.AddSourceNs(ctx, className, usingDirectives, cds, Helper.ClassTemplateResolve(
                    classAccessor, sealedOrEmpty, className, sourceBuilder.ToString()));
            }
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