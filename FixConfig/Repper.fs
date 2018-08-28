module Repper 
// configuration.appSettings.add#key{GebruikersBeheerServerName}#value = remappfitest
// configuration.connectionStrings.add#name{TestName}#connectionString = replaceConn($value, "someserver\\sql")


type AttributeNode = string * string 
type Selector = 
    | Node of string * Selector 
    | LeafNode of string 
    | Attribute of AttributeNode * Selector
    | LeafAttribute of string
    | InvalidNode 

type Repper = Selector * string

let attributeSelector = '#'
let attributeSelectorStr = attributeSelector.ToString()

module Helpers = 
    let createAttributeNode(attribute: string): AttributeNode = 
        let split = attribute.Split('{')
        let name = split |> Array.head
        let value = (split |> Array.tail |> Array.head).Split('}') |> Array.head
        AttributeNode(name, value)
        
    let rec getAttributes (list: string list): Selector = 
        match list with 
        | [] -> InvalidNode
        | (attr :: tail) -> 
            match tail with 
            | [] -> LeafAttribute attr
            | _ -> 
                let attributeNode = attr |> createAttributeNode
                let extraAttributes = tail |> getAttributes
                Attribute(attributeNode, extraAttributes)

    let createNode(node: string): Selector = 
        let node_split = node.Split(attributeSelector) |> Array.toList
        let attributes = node_split |> List.tail |> getAttributes
        Node (node_split |> List.head, attributes)
        
    let rec getSelector (list: string list): Selector = 
        match list with 
        | [] -> InvalidNode 
        | (node :: tail) -> 
            match tail with 
            | [] ->
                match node.Contains(attributeSelectorStr) with 
                | false -> LeafNode node 
                | true -> node |> createNode
            | _ -> Node (node, getSelector(tail))


let getSelector (str: string): Selector = 
    str.Split('.')
        |> Array.toList
        |> Helpers.getSelector
        

let getRepper (line: string): Repper = 
    let splits = line.Split('=') |> Array.map (fun x -> x.Trim()) |> List.ofArray
    let selector = splits |> List.head |> getSelector
    let value = splits |> List.tail |> List.head 
    Repper(selector, value)
    
  
