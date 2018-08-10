module Repper

open System.IO

// configuration.appSettings.add#key{GebruikersBeheerServerName}#value = remappfitest
// configuration.connectionStrings.add#name{FinancienNet_Financien}#connectionString = replaceConn($value, "remappfitest\\sql")


type Attribute = string * string 
type Selector = 
    | Node of string * Selector 
    | LeafNode of string 
    | Attribute of Attribute * Attribute
    | LeafAttribute of string

type Repper = Selector * string

let getSelector (str: string): Selector = 
    let rec getSelector_ (strings: string list): Selector = 
        match strings with 
        | [] -> LeafNode "Invalid"
        | node :: tail -> 
            match tail with 
            | [] -> LeafNode node 
            | _ -> Node(node, getSelector_(tail))
    str.Split('.')
        |> Array.toList
        |> getSelector_


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
