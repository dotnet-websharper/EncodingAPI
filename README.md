# WebSharper Encoding API Binding

This repository provides an F# [WebSharper](https://websharper.com/) binding for the [Encoding API](https://developer.mozilla.org/en-US/docs/Web/API/TextEncoder), enabling WebSharper applications to encode and decode text using UTF-8.

## Repository Structure

The repository consists of two main projects:

1. **Binding Project**:

   - Contains the F# WebSharper binding for the Encoding API.

2. **Sample Project**:
   - Demonstrates how to use the Encoding API with WebSharper syntax.
   - Includes a GitHub Pages demo: [View Demo](https://dotnet-websharper.github.io/EncodingAPI/).

## Installation

To use this package in your WebSharper project, add the NuGet package:

```bash
   dotnet add package WebSharper.Encoding
```

## Building

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on your machine.

### Steps

1. Clone the repository:

   ```bash
   git clone https://github.com/dotnet-websharper/Encoding.git
   cd Encoding
   ```

2. Build the Binding Project:

   ```bash
   dotnet build WebSharper.Encoding/WebSharper.Encoding.fsproj
   ```

3. Build and Run the Sample Project:

   ```bash
   cd WebSharper.Encoding.Sample
   dotnet build
   dotnet run
   ```

4. Open the hosted demo to see the Sample project in action:
   [https://dotnet-websharper.github.io/EncodingAPI/](https://dotnet-websharper.github.io/EncodingAPI/)

## Example Usage

Below is an example of how to use the Encoding API in a WebSharper project:

```fsharp
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

    // Variable to store user input text
    let utf8Input = Var.Create ""
    // Variable to store the UTF-8 encoded output
    let utf8Encoded = Var.Create "Waiting for input..."
    // Variable to store the UTF-8 decoded output
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

            // Update the UI with encoded output
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
                // Convert comma-separated values back to Uint8Array
                let encodedArray =
                    encodedText.Split([| ',' |])
                    |> Array.map byte
                    |> Uint8Array

                let decoder = new TextDecoder()
                let decodedText = decoder.Decode(encodedArray)

                // Update the UI with decoded output
                utf8Decoded.Value <- $"UTF-8 Decoded: {decodedText}"
        }

    [<SPAEntryPoint>]
    let Main () =

        IndexTemplate.Main()
            // Bind user input field
            .Utf8Input(utf8Input)
            // Bind encoded output to the UI
            .Utf8Encoded(utf8Encoded.V)
            // Bind decoded output to the UI
            .Utf8Decoded(utf8Decoded.V)
            // Attach encoding function to the button click
            .EncodeUTF8(fun _ ->
                async {
                    do! encodeUTF8() |> Promise.AsAsync
                }
                |> Async.StartImmediate
            )
            // Attach decoding function to the button click
            .DecodeUTF8(fun _ ->
                async {
                    do! decodeUTF8() |> Promise.AsAsync
                }
                |> Async.StartImmediate
            )
            .Doc()
        |> Doc.RunById "main"
```

This example demonstrates how to encode and decode text using UTF-8 with the Encoding API in WebSharper.
