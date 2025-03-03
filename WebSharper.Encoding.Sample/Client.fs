namespace WebSharper.Encoding.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Encoding

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    let utf8Input = Var.Create ""
    let utf8Encoded = Var.Create "Waiting for input..."
    let utf8Decoded = Var.Create ""

    // UTF-8 Encoding Function
    let encodeUTF8 () =
        promise {
            let text = utf8Input.Value
            let encoder = new TextEncoder()
            let encoded = encoder.Encode(text)

            // Convert Uint8Array to comma-separated string
            let encodedString =
                Seq.init encoded.Length (fun i -> encoded.Get(i).ToString())
                |> String.concat ", "

            utf8Encoded.Value <- $"UTF-8 Encoded: {encodedString}"
        }

    // UTF-8 Decoding Function
    let decodeUTF8 () =
        promise {
            let encodedText =
                utf8Encoded.Value.Split([| ':' |])
                |> Array.tryItem 1 
                |> Option.defaultValue ""

            if encodedText.Trim() = "" then ()
            else
                let encodedArray =
                    encodedText.Split([| ',' |])
                    |> Array.map byte
                    |> Uint8Array

                let decoder = new TextDecoder()
                let decodedText = decoder.Decode(encodedArray)

                utf8Decoded.Value <- $"UTF-8 Decoded: {decodedText}"
        }

    [<SPAEntryPoint>]
    let Main () =

        IndexTemplate.Main()
            .Utf8Input(utf8Input)
            .Utf8Encoded(utf8Encoded.V)
            .Utf8Decoded(utf8Decoded.V)
            .EncodeUTF8(fun _ -> 
                async { 
                    do! encodeUTF8() |> Promise.AsAsync 
                } 
                |> Async.StartImmediate
            )
            .DecodeUTF8(fun _ -> 
                async { 
                    do! decodeUTF8() |> Promise.AsAsync 
                } 
                |> Async.StartImmediate
            )
            .Doc()
        |> Doc.RunById "main"
