module Nodder

open System.Xml.Linq


type NodderAttribute = string * string 

type Value = string 
type Name = string 

type Nodder =  
    | Node of Name * NodderAttribute list * Nodder list 
    | LeafNode of Name * Value * NodderAttribute list 

let getAttributes (element: XElement): NodderAttribute list = 
    let rec getAttributes_ (attributes: XAttribute list): NodderAttribute list = 
        match attributes with 
        | [] -> []
        | attr :: tail -> (attr.Name.LocalName, attr.Value) :: getAttributes_(tail)
    match element.HasAttributes with 
        | true -> getAttributes_(List.ofSeq(element.Attributes()))
        | false -> []

let rec fromXNodeList (nodes: XElement list): Nodder list = 
    let fromXNode (n: XElement): Nodder = 
        let attributes = getAttributes(n)
        match n.HasElements with 
        | true -> Node(n.Name.LocalName, attributes, fromXNodeList(List.ofSeq(n.Elements())))
        | _ -> LeafNode(n.Name.LocalName, n.Value, attributes)
    match nodes with 
    | [] -> []
    | node :: tail -> fromXNode(node) :: fromXNodeList(tail)

let fromXDocument (doc: XDocument): Nodder = 
    let attributes = getAttributes doc.Root
    let l = fromXNodeList(List.ofSeq(Seq.tail(doc.Descendants())))
    Node(doc.Root.Name.LocalName, attributes, l)

let fromXml (path: string): Nodder = 
    fromXDocument(XDocument.Load(path))