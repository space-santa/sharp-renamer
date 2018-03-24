module SharpRenamer

open System.Drawing
open System.Text.RegularExpressions
open System.IO
open System

/// Provide means to build a DateTime from different strings.
/// yyyy:MM:dd hh:mm:ss
/// yyyyMMdd_hhmmss
module TimeStampString =
    exception InvalidDateStringException of string

    let (|PlainString|_|) input =
        let m = Regex.Match(input, @"\d{8}[ |_]\d{6}")
        if (m.Success) then Some m.Value else None

    let (|TagString|_|) input =
        let m = Regex.Match(input, @"\d{4}:\d\d:\d\d[ |_]\d\d:\d\d:\d\d")
        if (m.Success) then Some m.Value else None

    let processPlainString (str:string) =
        str.[0..3] + "-" + str.[4..5] + "-" + str.[6..7] + "_" + str.[9..10] + "." + str.[11..12] + "." + str.[13..14]

    let processTagString (str:string) =
        let t = str.Split(':', ' ')
        // [yyyy; mm; dd; hh; mm; ss]
        t.[0] + "-" + t.[1] + "-" + t.[2] + "_" + t.[3] + "." + t.[4] + "." + t.[5]

    let makeTimeStampString (str:string) =
        match str with
        | PlainString x -> processPlainString x
        | TagString x -> processTagString x
        | _ -> raise (InvalidDateStringException "str")

module ExifReader =
    exception NoExifTag of string
    let removeUnwantedChars (arr:string) =
        System.String(Array.filter (System.Char.IsControl >> not) (arr.ToCharArray()))

    let rec getOrigDateTimeProp (propertyItems:Imaging.PropertyItem list) =
        match propertyItems with
        | x::xs -> match x.Id.ToString("x") with
                    | "9003" -> removeUnwantedChars(System.Text.Encoding.ASCII.GetString x.Value)
                    | _-> getOrigDateTimeProp xs
        | [] -> ""

    let getOriginalDateTime (path:string) =
        try
            let img = new Bitmap(path)
            let propItems = img.PropertyItems
            getOrigDateTimeProp (propItems |> Array.toList)
        with
        | :? System.Exception ->
            printfn "%s is not a jpg" <| path
            ""

let renameFileByName(path:string) =
    let oldName = Path.GetFileName(path)
    let ext = Path.GetExtension(path)
    let dts = TimeStampString.makeTimeStampString oldName

    if System.String.IsNullOrEmpty(dts) then
        printfn "%s is not a valid filename" <| oldName
    else
        let newStr = dts + ext
        File.Move(path, newStr)

let renameFileByTag (path:string) =
    let odt = ExifReader.getOriginalDateTime path
    if System.String.IsNullOrEmpty(odt) then
        renameFileByName path
    else
        let newName = (TimeStampString.makeTimeStampString odt) + Path.GetExtension(path)
        let oldName = Path.GetFileName(path)
        if oldName = newName then
            printfn "%s is already renamed" <| oldName
        else
            File.Move(path, newName)

let rec renameFiles (paths:string list) =
    match paths with
    | x::xs ->
        renameFileByTag x
        renameFiles xs
    | [] -> ignore

[<EntryPoint>]
let main args=
    Array.toList args |> renameFiles
    0
