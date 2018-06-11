module ExifReader

open System.Drawing

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
