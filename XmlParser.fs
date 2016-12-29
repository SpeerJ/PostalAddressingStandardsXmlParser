namespace XmlParser
open FSharp.Data
open System.Xml.Linq
open Microsoft.FSharp.Core.Operators
module Read =
    type ElementProvider = XmlProvider<"SecondaryUnit.xml">

    // lowers first character
    let varCase (x : string) : string =
        x.Substring(0,1).ToLower() + x.Substring(1) + "s" 
    let tab = "    "
    let comma i length =
                if i <> length - 1 then "," else ""
    
    //Gives a basic printer compatible with each type
    type Outputter(header0 : string, footer0 : string, fileName : string, bodyFunc) =
        let header = header0
        let footer = footer0
        let xml = ElementProvider.Load (fileName + ".xml")
        member x.Body =
           bodyFunc (xml.XElement.Elements())
        member x.PrintElements =
            printfn "%s" header
            x.Body
            printfn "%s" footer

    
    type XmlOutput() =
        member x.PrintElements (fileName : string) =
            let outputter = new Outputter("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<Elements>\n", "</Elements>", fileName,
                // take xElements sorts them by value merges value groups into maps x.value: x.name.toString() 
                Map.ofSeqXElements >> Map.iter(fun k (s : list<string>) -> 
                    printfn "<Element>\n\t<Definition>%s</Definition>\n\t<Aliases>" k
                    s
                        |> List.iter(fun x ->
                            printfn "%s<Alias>%s</Alias>" (tab + tab) x
                        )
                    printfn "    </Aliases>\n</Element>\n"
                )
            )
            outputter.PrintElements

    type CSharpOutput() =
        member x.PrintElements (fileName : string) =
            let dictName = varCase fileName
            let tab singleLine count =
                if singleLine then "" else (String.replicate count "    ")
            let print_single_k_v (k, elements: list<string>) =
                    //for the sake of readibility i have divided the output into 2 styles, one which displays every element on a single line({asdf,{a,b,c,d}}), and one on seperate lines({asdf,{a,\nb,\nc,\nd\n}}).
                    // this determines whether the size is less than 7 and should be the former style
                    let singleLine = elements.Length < 7 
                    // \n if > 7
                    let newLine = if singleLine then "" else "\r\n"
                    printf "%s{ \"%s\", new List<string> %s%s{%s" (tab false 1) k newLine (tab singleLine 2) newLine
                    elements
                        |> List.iteri(fun i x ->
                            let sComma = comma i elements.Length
                            printf " %s\"%s\"%s%s" (tab singleLine 3 ) x sComma newLine
                        )
                    printf "%s}%s" (tab singleLine 2) newLine
                    printf "%s}" (tab singleLine 1)
            let bodyFunc x = 
                let mapOfXElem = List.ofSeqXElements x
                print_single_k_v (mapOfXElem.[0])
                mapOfXElem
                    |> List.iter(fun (k, s : list<string>) -> 
                        printfn(",")
                        print_single_k_v (k, s)
                )
            let outputter = new Outputter("Dictionary<string, List<string>> " + dictName + " = new Dictionary<string, List<string>>\n{", "\n};", fileName, bodyFunc)
            outputter.PrintElements

    type CSharpOutputForCorrecting() =
        member x.PrintElements (fileName : string) =
            let dictName = varCase fileName
            let bodyFunc (x:seq<XElement>) =
                let list = List.ofSeqXElementsStrings x
                list
                    |> List.iteri(fun i (k, s) ->
                        let y = comma i list.Length
                        printfn "    { \"%s\", \"%s\"}%s" k s y )
            let outputter = new Outputter("Dictionary<string, string> " + dictName + " = new Dictionary<string, string>()\n{", "};", fileName, bodyFunc)
            outputter.PrintElements