namespace WebSharper.Encoding.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Encoding

[<JavaScript>]
module Client =
    // The template is loaded from the DOM, so you can edit index.html and refresh
    // the browser without recompiling, unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    /// Format raw UTF-8 bytes as a comma-separated list for display.
    let private formatBytes (bytes: Uint8Array) =
        Seq.init bytes.Length (fun i -> string (bytes.Get(i)))
        |> String.concat ", "

    let utf8Input = Var.Create ""

    // Keep the raw encoded bytes so decoding does not have to parse a display string.
    let encodedBytes : Var<Uint8Array option> = Var.Create None

    let utf8Encoded : View<string> =
        encodedBytes.View.Map(fun (bytes: Uint8Array option) ->
            match bytes with
            | None -> "Waiting for input..."
            | Some b -> sprintf "UTF-8 encoded (%d bytes): %s" b.Length (formatBytes b))

    let utf8Decoded = Var.Create "Waiting for decode..."

    let decoderInfo =
        let encoder = new TextEncoder()
        let decoder = new TextDecoder()
        sprintf "TextEncoder.encoding = %s | TextDecoder.encoding = %s | fatal = %b | ignoreBOM = %b"
            encoder.Encoding decoder.Encoding decoder.Fatal decoder.IgnoreBOM

    let encodeUTF8 () =
        let encoder = new TextEncoder()
        let bytes = encoder.Encode(utf8Input.Value)
        encodedBytes.Value <- Some bytes
        utf8Decoded.Value <- "Waiting for decode..."

    let decodeUTF8 () =
        match encodedBytes.Value with
        | None -> utf8Decoded.Value <- "Nothing to decode yet."
        | Some bytes ->
            let decoder = new TextDecoder()
            utf8Decoded.Value <- decoder.Decode(bytes)

    [<SPAEntryPoint>]
    let Main () =
        IndexTemplate.Main()
            .Utf8Input(utf8Input)
            .Utf8Encoded(utf8Encoded)
            .Utf8Decoded(utf8Decoded.V)
            .DecoderInfo(decoderInfo)
            .EncodeUTF8(fun _ -> encodeUTF8())
            .DecodeUTF8(fun _ -> decodeUTF8())
            .Doc()
        |> Doc.RunById "main"
