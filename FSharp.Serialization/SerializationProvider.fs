namespace ProviderImplementation

open Microsoft.FSharp.Core.CompilerServices
open Samples.FSharp.ProvidedTypes
open System.Reflection
open System.IO
open Microsoft.FSharp.Quotations

[<TypeProvider>]
type public SerializationProvider(cfg : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces()

    let thisAssembly = Assembly.GetExecutingAssembly()
    let rootNamespace = "FSharp.Serialization"
    let baseTy = typeof<obj>

    let binaryTy = ProvidedTypeDefinition(thisAssembly, rootNamespace, "Binary", Some baseTy)
    let staticParams = [ProvidedStaticParameter("Assembly", typeof<string>)]
    let types = ResizeArray()
    let addFunction typeName (parameterValues : obj []) =
        match parameterValues with
        | [| :? string as assembly |] ->
            let binaryTy = ProvidedTypeDefinition(thisAssembly, rootNamespace, "MyThing", Some baseTy)    
            binaryTy.AddMember (ProvidedMethod(assembly, [], typeof<string>, InvokeCode = fun args -> Expr.Value "Hello World"))
            binaryTy.IsErased <- false
            binaryTy.SuppressRelocation <- false
            types.Add binaryTy
            binaryTy
        | _ -> failwith "unexpected parameter values"

    do binaryTy.DefineStaticParameters(staticParameters = staticParams, apply = addFunction)
    let providedAssembly = ProvidedAssembly(Path.ChangeExtension(Path.GetTempFileName(), ".dll"))

    let methods = ProvidedMethod("F1", [ProvidedParameter("i", typeof<int>)], typeof<int>, InvokeCode = fun args -> Expr.Value 1)

    let ctor = ProvidedConstructor(parameters = [], InvokeCode = fun args -> <@@ obj() @@>)

    do
        providedAssembly.AddTypes (types |> List.ofSeq)
        binaryTy.AddMember ctor
        binaryTy.AddMember methods

    do System.AppDomain.CurrentDomain.add_AssemblyResolve(fun _ args ->
        let name = AssemblyName args.Name
        let existingAssembly =
            System.AppDomain.CurrentDomain.GetAssemblies()
            |> Seq.tryFind(fun a -> AssemblyName.ReferenceMatchesDefinition(name, a.GetName()))
        defaultArg existingAssembly null)
    do 
        this.AddNamespace (rootNamespace, [binaryTy])

[<TypeProviderAssembly>]
do ()