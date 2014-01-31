module FSharp.Serialization.Reflection
open System.Reflection
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Reflection
open Microsoft.FSharp.Reflection.FSharpReflectionExtensions
open Microsoft.FSharp.Quotations
open System

let types = Assembly.GetExecutingAssembly().GetTypes()

let isPrimitive = function
    | ty when ty = typeof<bool> -> true
    | ty when ty = typeof<char> -> true
    | ty when ty = typeof<sbyte> -> true
    | ty when ty = typeof<byte> -> true
    | ty when ty = typeof<int16> -> true
    | ty when ty = typeof<uint16> -> true
    | ty when ty = typeof<int32> -> true
    | ty when ty = typeof<uint32> -> true
    | ty when ty = typeof<int64> -> true
    | ty when ty = typeof<uint64> -> true
    | ty when ty = typeof<float32> -> true
    | ty when ty = typeof<float> -> true
    | _ -> false

let generateBasicTypes = function
    | ty when ty = typeof<bool> -> Some <@@ fun (x:bool) -> BitConverter.GetBytes x @@>
    | ty when ty = typeof<char> -> Some <@@ fun (x:char) -> BitConverter.GetBytes x @@>
    | ty when ty = typeof<int16> -> Some <@@ fun (x:int16) -> BitConverter.GetBytes x @@>
    | ty when ty = typeof<uint16> -> Some <@@ fun (x:uint16) -> BitConverter.GetBytes x @@>
    | ty when ty = typeof<int32> -> Some <@@ fun (x:int32) -> BitConverter.GetBytes x @@>
    | ty when ty = typeof<uint32> -> Some <@@ fun (x:uint32) -> BitConverter.GetBytes x @@>
    | ty when ty = typeof<int64> -> Some <@@ fun (x:int64) -> BitConverter.GetBytes x @@>
    | ty when ty = typeof<uint64> -> Some <@@ fun (x:uint64) -> BitConverter.GetBytes x @@>
    | ty when ty = typeof<float32> -> Some <@@ fun (x:float32) -> BitConverter.GetBytes x @@>
    | ty when ty = typeof<float> -> Some <@@ fun (x:float) -> BitConverter.GetBytes x @@>
    | ty when ty = typeof<byte> -> Some <@@ fun (x:byte) -> [|x|] @@>
    | ty when ty = typeof<byte> -> Some <@@ fun (x:sbyte) -> [|byte x|] @@>
    | _ -> None
