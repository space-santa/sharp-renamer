open System.Drawing
open System.Text.RegularExpressions
open System.IO

/// Changes timestamp string 'yyyy:dd:mm hh:mm:ss' to 'yyyy-mm-dd_hh.mm.ss'.
let changeTimeStampString (dt:string) =
    let t = dt.Split(':', ' ')
    // [yyyy; mm; dd; hh; mm; ss]
    t.[0] + "-" + t.[1] + "-" + t.[2] + "_" + t.[3] + "." + t.[4] + "." + t.[5]

let removeUnwantedChars (arr:string) =
    System.String(Array.filter (System.Char.IsControl >> not) (arr.ToCharArray()))

let rec getOrigDateTimeProp (propertyItems:Imaging.PropertyItem list) =
    match propertyItems with
    | x::xs -> match x.Id.ToString("x") with
                | "9003" -> removeUnwantedChars(System.Text.Encoding.ASCII.GetString x.Value)
                | _-> getOrigDateTimeProp xs
    | [] -> ""

let getOriginalDateTime (path:string) =
    let img = new Bitmap(path)
    let propItems = img.PropertyItems
    getOrigDateTimeProp (propItems |> Array.toList)

let renameFileByTag (path:string) =
    try
        let odt = getOriginalDateTime path
        if System.String.IsNullOrEmpty(odt) then
            printfn "Can't rename %s, no EXIF original date time found" <| path
        else
            let newName = (getOriginalDateTime path |> changeTimeStampString) + Path.GetExtension(path)
            let oldName = Path.GetFileName(path)
            if oldName = newName then
                printfn "%s is already renamed" <| oldName
            else
                File.Move(path, newName)
    with
    | :? System.Exception -> printfn "%s is not a jpg" <| path

let renameFileByName(path:string) =
    let oldName = Path.GetFileName(path)
    let ext = Path.GetExtension(path)
    let regex = Regex @"\d{8}_\d{6}"
    let result = regex.Match(oldName)
    let dts = result.Value

    if System.String.IsNullOrEmpty(dts) then
        printfn "%s is not a valid filename" <| oldName
    else
        let newStr = dts.[0..3] + "-" + dts.[4..5] + "-" + dts.[6..7] + "_" + dts.[9..10] + "." + dts.[11..12] + "." + dts.[13..14] + ext
        File.Move(path, newStr)

let rec renameFiles (paths:string list) =
    match paths with
    | x::xs -> match x with
                | a when Regex.Match(a,@".+\.(jpg|JPG)$").Success ->
                    renameFileByTag x
                    renameFiles xs
                | a when Regex.Match(a,@".+\.(mp4|MP4)$").Success ->
                    renameFileByName x
                    renameFiles xs
                | _ ->
                    printfn "Can't rename %s" <| x
                    renameFiles xs
    | [] -> ignore

[<EntryPoint>]
let main args=
    Array.toList args |> renameFiles
    0
