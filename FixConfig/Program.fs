// Learn more about F# at http://fsharp.org

open System
open Repper
open System.IO
open Nodder
open Newtonsoft.Json

let xmlFile = @"C:\temp\test.config"
let outXmlFile = @"C:\temp\test_out.json"

let rec getReppers(contents: string list): Repper list = 
    match contents with 
        | [] -> []
        | (line :: tail) -> getRepper(line) :: getReppers(tail)

let nodderTest = Nodder("test", [], NodeValue [Nodder("haha", [], StringValue "Gotcha")])
let selectorTest = Selector.Node("test", Selector.LeafNode("haha"))
let selectorTest2 = Selector.LeafNode("test")

let rec find(nodder: Nodder, selector: Selector): Nodder = 
    let nameMatches x = nodderName(nodder) = x
    let leafer name = 
        match nameMatches(name) with  
        | false -> InvalidNode
        | true -> nodder

    let rec helper (nr, r) =
        match nr with 
        | [] -> InvalidNodder
        | (node :: tail) -> 
            match find(node, r) with 
            | InvalidNodder -> helper (tail, r)
            | x -> x

    match selector with 
    | Selector.InvalidNode -> nodder 
    | Selector.Attribute(_) -> nodder
    | Selector.Node(name, rest) -> 
        match nameMatches(name) with 
        | true -> helper((nodderValue nodder), selector)
        | false -> InvalidNode
    | Selector.LeafNode(name) -> leafer(name)
    | Selector.LeafAttribute(name) -> leafer(name)
    
let apply(selector: Selector): Nodder -> Nodder = 
    fun x -> find(x, selector)

let replace(nodder: Nodder, repper: Repper): Nodder = 
    nodder
        |> apply(fst repper)
        |> setNodderValue(StringValue(snd repper))


let replaceAll (nodder: Nodder, reppers: Repper list): Nodder =
    let foldFun nodder = fun repper -> replace(nodder, repper)
    List.fold foldFun nodder reppers

[<EntryPoint>]
let main argv =
    let serialize = fun x -> JsonConvert.SerializeObject(x, Formatting.Indented)
    let serializeAndWriteToFile filename = 
        serialize >> (fun y -> File.WriteAllText(filename, y))
        
    let filename = @"C:\temp\fsharp.txt"
    let inFile = @""
    let repperFile = @""

    //let nodder = 
    //    inFile 
    //        |> fromXml
    // let reppers = 
    //     repperFile 
    //         |> File.ReadAllLines
    //         |> Array.toList
    //         |> getReppers

    let repperTest = (selectorTest2, "Hello")
    replaceAll (nodderTest, [repperTest])
        |> serialize 
        |> Console.WriteLine

    find(nodderTest, selectorTest2)
        |> setNodderName("NIEUW!!")
        |> serialize 
        |> Console.WriteLine
    
    // "configuration.appSettings.add#key{GebruikersBeheerServerName}#value = Server"
    //     |> getRepper
    //     |> serializeAndWriteToFile filename

    "Done" |> Console.WriteLine |> ignore
    Console.ReadLine() |> ignore
    0 // return an integer exit code

