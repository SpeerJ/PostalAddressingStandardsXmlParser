namespace XmlParser
open FSharp.Data
open System.Xml.Linq
open Microsoft.FSharp.Collections      
module List =
    let ofSeqXElements(seqXE : seq<XElement>) = 
        seqXE
            |>  List.ofSeq
            |>  List.groupBy(fun x -> x.Value)
            |>  List.map(fun (key, x) -> (key, (List.map(fun (y : XElement) -> y.Name.ToString()) x)))
    let ofSeqXElementsStrings(seqXE : seq<XElement>) =
        seqXE
            |> List.ofSeq
            |> List.map(fun x -> (x.Value, x.Name.ToString()))

module Map =
    let ofSeqXElements(seqXE : seq<XElement>) : Map<string, List<string>> = 
        seqXE
            |>  List.ofSeqXElements
            |>  Map.ofList