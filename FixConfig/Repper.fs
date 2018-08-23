module Repper

open System.IO

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

let getSelector (str: string): Selector = 
    let createAttributeNode(attribute: string): AttributeNode = 
        let split = attribute.Split('{')
        let name = split |> Array.head
        let value = (split |> Array.tail |> Array.head).Split('}') |> Array.head
        AttributeNode(name, value)
        
    let rec getAttributes ([]: string list): Selector = InvalidNode
    let rec getAttributes ((attr :: tail): string list): Selector = 
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
        
    let rec getSelector_ ([]): Selector = InvalidNode
    let rec getSelector_ ((node :: tail): string list): Selector = 
        match tail with 
        | [] ->
            match node.Contains(attributeSelectorStr) with 
            | false -> LeafNode node 
            | true -> node |> createNode
        | _ -> Node (node, getSelector_(tail))

    str.Split('.')
        |> Array.toList
        |> getSelector_


let testXml = 
    @"C:\temp\test.config" 
        |> File.ReadAllLines 
        |> Array.toList
        
let getLine (repper: Repper, contents: string): string = ""

let repReplace (repper: Repper, line: string): string = snd repper

let getRepper (line: string): Repper = 
    let splits = line.Split('=') |> Array.map (fun x -> x.Trim()) |> List.ofArray
    let selector = splits |> List.head |> getSelector
    let value = splits |> List.tail |> List.head 
    Repper(selector, value)

let repIt (repper: Repper, contents: string): string = 
    let currentLine = getLine(repper, contents)
    let newLine = repReplace(repper, currentLine)
    contents.Replace(currentLine, newLine)
