// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
namespace XmlParser

module Main = 
    open System
        [<EntryPoint>]
        let main argv = 
            let bxml = new Read.CSharpOutputForCorrecting()
            bxml.PrintElements "POIndicator"
            bxml.PrintElements "SecondaryUnit"
            bxml.PrintElements "Directional"
            bxml.PrintElements "Suffix"
            0 // return an integer exit code