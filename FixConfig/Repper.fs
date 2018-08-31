module Repper 
// configuration.appSettings.add#key{GebruikersBeheerServerName}#value = remappfitest
// configuration.connectionStrings.add#name{TestName}#connectionString = replaceConn($value, "someserver\\sql")

// Node ("configuration", 
//    NodeValue( Node ("connectionStrings", 
//        NodeValue( Node ("add", 
//            AttributeValue ( Attribute ("name" 
//                AttibuteValue ("TestName", LeafAttribute ("connectionString")))))))))



type Repper = Selector * string
and Selector = 
    | Node of Name * SelectorValue 
    | Attribute of Name * SelectorValue
    | LeafAttribute of string 
and SelectorValue = 
    | StringValue of string 
    | NodeValue of Selector
    | AttributeValue of string * Selector 
    | NoValue

and Name = string 

let attributeSelector = '#'
let attributeSelectorStr = attributeSelector.ToString()

let InvalidNode = Node("Invalid", StringValue "")

module Helpers = 
    
    let createAttributeNode(attribute: string) = 
        match attribute.Contains("{") with 
        | true ->
            let split = attribute.Split('{')
            let name = split |> Array.head
            let value = (split |> Array.tail |> Array.head).Split('}') |> Array.head
            (name, value)
        | false -> (attribute, "")
        
    let rec getAttribute (list: string list): SelectorValue = 
        match list with 
        | [] -> NoValue
        | (attr :: tail) -> 
            let (name, value) = attr |> createAttributeNode
            match tail with 
            | [] -> StringValue value
            | _ -> 
                let extraAttributes = tail |> List.reduce(fun x -> fun y -> x + attributeSelectorStr + y)
                AttributeNode(name, (extraAttributes |> createAttributeSelector))
    and createAttributeSelector: string -> Selector = 
        fun node -> 
            let node_split = node.Split(attributeSelector) |> Array.toList
            let (name, value) = node_split |> List.head |> createAttributeNode
            let attribute = node_split |> List.tail |> String.concat attributeSelectorStr
            Attribute (name, AttributeNode (value, createAttributeSelector(attribute)))

    let rec createAttributeNode2: string -> Selector = 
        fun node -> 
            let attributes = 
                node.Split(attributeSelector) 
                    |> Array.toList
                    |> List.map 
            Attribute(node, NoValue)

    let rec getSelector (list: string list): Selector = 
        match list with 
        | [] -> InvalidNode 
        | (node :: tail) -> 
            match tail with 
            | [] ->
                match node.Contains(attributeSelectorStr) with 
                | false -> Node(node, NoValue) 
                | true -> node |> createAttributeNode2
            | _ -> Node (node, NodeValue(getSelector(tail)))


let getSelector (str: string): Selector = 
    str.Split('.')
        |> Array.toList
        |> Helpers.getSelector
        

let getRepper (line: string): Repper = 
    let splits = line.Split('=') |> Array.map (fun x -> x.Trim()) |> List.ofArray
    let selector = splits |> List.head |> getSelector
    let value = splits |> List.tail |> List.head 
    Repper(selector, value)
    
  
