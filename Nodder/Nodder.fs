module Nodder 

open System.Xml.Linq

type Nodder = Name * Attribute list * Value
and Attribute = string * string 
and Name = string 
and Value = 
    | StringValue of string 
    | NodeValue of Nodder list

let InvalidNode = Nodder("Invalid", [], StringValue(""))

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
            let value = 
                match n.HasElements with 
                | true -> NodeValue (fromXNodeList(n.Elements() |> List.ofSeq))
                | _ -> StringValue n.Value
            Nodder(n.Name.LocalName, (n |> getAttributes), value)
        match nodes with 
        | [] -> [Nodder("invalid", [], StringValue "")]
        | node :: tail -> fromXNode(node) :: fromXNodeList(tail)

    let fromXDocument (doc: XDocument): Nodder = 
        let attributes = doc.Root |> getAttributes 
        let l = doc.Descendants() |> Seq.tail |> Seq.toList |> fromXNodeList
        let name = doc.Root.Name.LocalName 
        Nodder (name, attributes, NodeValue (l))

    let name: Nodder -> Name = 
        fun nodder -> 
            match nodder with 
            | (a, b, c) -> a
            
    let value: Nodder -> Value = 
        fun nodder -> 
            match nodder with 
            | (a, b, c) -> c

    let attributes: Nodder -> Attribute list = 
        fun nodder -> 
            match nodder with 
            | (a, b, c) -> b

    let setName(newName: string): Nodder -> Nodder = 
        fun nodder -> 
            match nodder with 
            | (a, b, c) -> (newName, b, c)
            
    let setAttributes(newAttributes: Attribute list): Nodder -> Nodder = 
        fun nodder -> 
            match nodder with 
            | (a, b, c) -> (a, newAttributes, c)
            
    let setValue(newValue: Value): Nodder -> Nodder = 
        fun nodder -> 
            match nodder with 
            | (a, b, c) -> (a, b, newValue)

let fromXml (path: string): Nodder = 
    path 
        |> XDocument.Load 
        |> Helpers.fromXDocument

let nodderName = Helpers.name
let nodderValue = Helpers.value
let nodderAttributes = Helpers.attributes

let setNodderName = Helpers.setName
let setNodderValue = Helpers.setValue
let setNodderAttributes = Helpers.setAttributes

