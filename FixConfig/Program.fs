// Learn more about F# at http://fsharp.org

open System
open Repper
open System.IO
open Nodder
open Newtonsoft.Json

let filename = @"C:\temp\fsharp.txt"
let xmlFile = @"C:\temp\test.config"
let outXmlFile = @"C:\temp\test_out.json"

let WriteToFile input = 
    File.WriteAllLines (filename, input) |> ignore

let getOutputLines (input: string) = 
    input.Split("\n")

let getInput (filename: string) = 
    filename.Replace("\r\n", "\n")

[<EntryPoint>]
let main argv =
    "Hello?\r\nIs it me you're looking for? "
        |> getInput
        |> getOutputLines 
        |> WriteToFile

    let serialize x = JsonConvert.SerializeObject(x, Formatting.Indented) 
    let serializeAndWrite x = x |> serialize |> fun y -> File.WriteAllText(filename, y)
    // xmlFile 
    //     |> fromXml 
    //     |> serialize 
    //     |> fun x -> File.WriteAllText(outXmlFile, x)

    "configuration.appSettings.add#key{GebruikersBeheerServerName}#value = Server"
        |> getSelector
        |> serializeAndWrite

    Console.ReadLine |> ignore 
    0 // return an integer exit code

