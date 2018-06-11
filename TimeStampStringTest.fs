module TestTimeStampString

open NUnit.Framework
open FsUnit
open TimeStampString

[<TestFixture>]
type TestTimeStampString() =
    [<Test>] member x.
        ``yyyyMMdd_hhmmss`` () =
            TimeStampString.makeTimeStampString "20180303_123456" |> should equal "2018-03-03_12.34.56"

    [<Test>] member x.
        ``yyyyMMdd hhmmss`` () =
            TimeStampString.makeTimeStampString "20180303 123456" |> should equal "2018-03-03_12.34.56"

    [<Test>] member x.
        ``VID_yyyyMMdd hhmmss`` () =
            TimeStampString.makeTimeStampString "VID_20180303 123456" |> should equal "2018-03-03_12.34.56"

    [<Test>] member x.
        ``yyyy:MM:dd hh:mm:ss`` () =
            TimeStampString.makeTimeStampString "2018:03:03 12:34:56" |> should equal "2018-03-03_12.34.56"

    [<Test>] member x.
        ``invalid string`` () =
            (fun () -> TimeStampString.makeTimeStampString "" |> ignore) |>
                should throw typeof<TimeStampString.InvalidDateStringException>