module Tests

open System
open Xunit
open FsCheck 
open FsCheck.Xunit
open Nodder 
open System.Xml.Linq


let getOutput: string -> Nodder = XDocument.Parse >> Helpers.fromXDocument
let doAssert (nod: Nodder): string -> Unit = 
    fun str -> Assert.Equal(nod, (str |> getOutput))

[<Fact>] 
let ``Simplest`` () = 
    "<test></test>" 
        |> doAssert ("test", [], StringValue "")


[<Fact>]
let ``Parses correctly`` () =
    let output = "<test1><test2></test2><test3><test4>test5</test4></test3></test1>" |> getOutput
    let should = Nodder.Nodder ("test1", [], Value.NodeValue ([
        Nodder("test2", [], Value.StringValue "");
        Nodder("test3", [], Value.NodeValue([
            Nodder("test4", [], Value.StringValue "test5")
        ]))
    ]))
    Assert.Equal(output, should)
