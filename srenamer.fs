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

let renameFile (path:string) =
    let newName = (getOriginalDateTime path |> changeTimeStampString) + Path.GetExtension(path)
    printfn "%s -> %s" <| path <| newName
    File.Move(path, newName)

let rec renameFiles (paths:string list) =
    match paths with
    | x::xs -> match x with
                | a when Regex.Match(a,@".+\.[jpg|JPG]").Success ->
                    renameFile x
                    renameFiles xs
                | a when Regex.Match(a,@".+\.[mp4|MP4]").Success ->
                    printfn "this is an mp4"
                    renameFiles xs
                | _ -> renameFiles xs
    | [] -> ignore

[<EntryPoint>]
let main args=
    renameFiles (args |> Array.toList)
    0