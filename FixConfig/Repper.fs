module Repper

open System.IO

// configuration.appSettings.add#key{GebruikersBeheerServerName}#value = remappfitest
// configuration.connectionStrings.add#name{FinancienNet_Financien}#connectionString = replaceConn($value, "remappfitest\\sql")


type AttributeNode = string * string 
type Selector = 
    | Node of string * Selector 
    | LeafNode of string 
    | Attribute of AttributeNode * Selector
    | LeafAttribute of string

type Repper = Selector * string

let attributeSelector = '#'

let getSelector (str: string): Selector = 
    let createAttributeNode(attribute: string): AttributeNode = 
        let split = attribute.Split('{')
        let name = Array.head(split)
        let value = Array.head(Array.head(Array.tail(split)).Split('}'))
        AttributeNode(name, value)

    let createNode(node: string): Selector = 
        let node_split = Array.toList(node.Split(attributeSelector))
        let attributes = node_split.Tail
        let rec getAttributes (n: string list) = 
            match n with 
            | [] -> LeafAttribute "Invalid"
            | attr :: attr_tail -> 
                match attr_tail with 
                | [] -> LeafAttribute attr
                | _ -> Attribute(createAttributeNode(attr), getAttributes(attr_tail))
        Node(node_split.Head, getAttributes(attributes))

    let rec getSelector_ ((node :: tail): string list): Selector = 
            match tail with 
            | [] ->
                match node.Contains(attributeSelector.ToString()) with 
                | false -> LeafNode node 
                | true -> createNode(node)
            | _ -> Node(node, getSelector_(tail))

    let validateArray(strings: string list): Selector = 
        match strings with 
        | [] -> LeafNode "Invalid"
        | _ -> getSelector_ strings

    str.Split('.')
        |> Array.toList
        |> validateArray


let testXml = 
    @"C:\temp\test.config" 
        |> File.ReadAllLines 
        |> Array.toList

let testRepper = Node("configuration", Node("connectionStrings", Node("add", Attribute(("name", "FinancienNet_Financien"), ("connectionString", ""))))), "NewConnection!!!"

let getLine (repper: Repper, contents: string): string = ""

let repReplace (repper: Repper, line: string): string = snd repper

let repIt (repper: Repper, contents: string): string = 
    let currentLine = getLine(repper, contents)
    let newLine = repReplace(repper, currentLine)
    contents.Replace(currentLine, newLine)
