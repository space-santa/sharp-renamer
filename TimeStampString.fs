module TimeStampString
open System.Text.RegularExpressions

/// Provide means to build a DateTime from different strings.
/// yyyy:MM:dd hh:mm:ss
/// yyyyMMdd_hhmmss
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
