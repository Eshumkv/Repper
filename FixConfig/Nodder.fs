module Nodder 

open System.Xml.Linq

type Nodder =  
    | Node of Name * Attribute list * Nodder list 
    | LeafNode of Name * Value * Attribute list 
    | InvalidNodder
and Attribute = string * string 
and Name = string 
and Value = string
    
module Helpers = 
    let getAttributes (element: XElement): Attribute list = 
        let rec getAttributes_ (attributes: XAttribute list): Attribute list = 
            match attributes with 
            | [] -> []
            | attr :: tail -> (attr.Name.LocalName, attr.Value) :: getAttributes_(tail)
        match element.HasAttributes with 
            | true -> element.Attributes() |> List.ofSeq |> getAttributes_
            | false -> []

    let rec fromXNodeList (nodes: XElement list): Nodder list = 
        let fromXNode (n: XElement): Nodder = 
            let attributes = n |> getAttributes
            match n.HasElements with 
            | true -> Node (n.Name.LocalName, attributes, fromXNodeList(List.ofSeq(n.Elements())))
            | _ -> LeafNode(n.Name.LocalName, n.Value, attributes)
        match nodes with 
        | [] -> []
        | node :: tail -> fromXNode(node) :: fromXNodeList(tail)

    let fromXDocument (doc: XDocument): Nodder = 
        let attributes = doc.Root |> getAttributes 
        let l = doc.Descendants() |> Seq.tail |> Seq.toList |> fromXNodeList
        let name = doc.Root.Name.LocalName 
        Node (name, attributes, l)

    let name: Nodder -> Name = 
        fun nodder -> 
            match nodder with 
            | Node(name, _, _) -> name 
            | LeafNode(name, _, _) -> name 
            | InvalidNodder -> ""
            
    let value: Nodder -> Value = 
        fun nodder -> 
            match nodder with 
            | Node(_) -> "" 
            | LeafNode(_, value, _) -> value
            | InvalidNodder -> ""

    let attributes: Nodder -> Attribute list = 
        fun nodder -> 
            match nodder with 
            | Node(_, attr, _) -> attr
            | LeafNode(_, _, attr) -> attr
            | InvalidNodder -> []

    let list: Nodder -> Nodder list = 
        fun nodder -> 
            match nodder with 
            | Node(_, _, list) -> list
            | LeafNode(_) -> []
            | InvalidNodder -> []

    let setName(newName: string): Nodder -> Nodder = 
        fun nodder -> 
            match nodder with 
            | Node(name, _, _) -> Node(newName, (attributes nodder), (list nodder)) 
            | LeafNode(name, _, _) -> LeafNode(newName, (value nodder), (attributes nodder)) 
            | InvalidNodder -> InvalidNodder
            
    let setAttributes(newAttributes: Attribute list): Nodder -> Nodder = 
        fun nodder -> 
            match nodder with 
            | Node(_, _, _) -> Node((name nodder), newAttributes, (list nodder)) 
            | LeafNode(_, _, _) -> LeafNode((name nodder), (value nodder), newAttributes) 
            | InvalidNodder -> InvalidNodder
            
    let setValue(newValue: string): Nodder -> Nodder = 
        fun nodder -> 
            match nodder with 
            | Node(_, _, _) -> Node((name nodder), (attributes nodder), (list nodder)) 
            | LeafNode(_, _, _) -> LeafNode((name nodder), newValue, (attributes nodder)) 
            | InvalidNodder -> InvalidNodder

    let setList(newNodders: Nodder list): Nodder -> Nodder = 
        fun nodder -> 
            match nodder with 
            | Node(_, _, _) -> Node((name nodder), (attributes nodder), newNodders) 
            | LeafNode(_, _, _) -> LeafNode((name nodder), (value nodder), (attributes nodder)) 
            | InvalidNodder -> InvalidNodder

let fromXml (path: string): Nodder = 
    path 
        |> XDocument.Load 
        |> Helpers.fromXDocument

let nodderName = Helpers.name
let noddervalue = Helpers.value
let nodderAttributes = Helpers.attributes
let nodderNodders = Helpers.list

let setNodderName = Helpers.setName
let setNodderValue = Helpers.setValue
let setNodderAttributes = Helpers.setAttributes
let setNodderNodders = Helpers.setList

