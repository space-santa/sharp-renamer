module SharpRenamer
open System.IO

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
        let baseDir = Path.GetDirectoryName(path)
        let newFullPath = baseDir + "/" + newName

        if oldName = newName then
            printfn "%s is already renamed" <| oldName
        else
            File.Move(path, newFullPath)

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
