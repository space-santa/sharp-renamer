open System
open System.Drawing

let getOriginalDateTime (path:string) =
    let img = new Bitmap(path)
    let propItems = img.PropertyItems
    let mutable retval = ""

    for p in propItems do
        if p.Id.ToString("x")= "9003" then
            retval <- System.Text.Encoding.ASCII.GetString p.Value

    retval


[<EntryPoint>]
let main args=
    printfn "sup dude %s" <| getOriginalDateTime "IMG_20180303_153239.jpg"
    0