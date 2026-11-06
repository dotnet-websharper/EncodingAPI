namespace WebSharper.Encoding

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =

    let TextDecoderOptions = 
        Pattern.Config "TextDecoderOptions" {
            Required = []
            Optional = [
                "fatal", T<bool>
                "ignoreBOM", T<bool>
            ]
        }

    let TypedArray = T<Int8Array> + T<Uint8Array> + T<Uint8ClampedArray> + T<Int16Array> + T<Uint16Array>
                        + T<Int32Array> + T<Uint32Array> + T<Float64Array> + T<Float32Array> 
    let BufferData = T<ArrayBuffer> + T<DataView> + TypedArray

    let DecodeOptions = 
        Pattern.Config "DecodeOptions" {
            Required = []
            Optional = [
                ("stream", T<bool>)
            ]
        }

    let EncodeIntoResult = 
        Pattern.Config "EncodeIntoResult" {
            Required = [
                "read", T<int>
                "written", T<int>
            ]
            Optional = []
        }

    let TextDecoder =
        Class "TextDecoder"
        |+> Static [
            Constructor (!?T<string>?label * !?TextDecoderOptions?options)
        ]
        |+> Instance [
            "encoding" =? T<string>
            "fatal" =? T<bool>
            "ignoreBOM" =? T<bool>

            "decode" => !?BufferData?buffer * !?DecodeOptions?options ^-> T<string>
        ]

    let TextDecoderStream =
        Class "TextDecoderStream"
        |+> Static [
            Constructor (T<string>?label * !?TextDecoderOptions?options)
        ]
        |+> Instance [
            "encoding" =? T<string>
            "fatal" =? T<bool>
            "ignoreBOM" =? T<bool>
            "readable" =? T<ReadableStream> 
            "writable" =? T<WritableStream> 
        ]

    let TextEncoder =
        Class "TextEncoder"
        |+> Static [
            Constructor T<unit>
        ]
        |+> Instance [
            "encoding" =? T<string>

            "encode" => T<string>?string ^-> T<Uint8Array> 
            "encodeInto" => T<string>?string * T<Uint8Array>?uint8Array ^-> EncodeIntoResult 
        ]

    let TextEncoderStream =
        Class "TextEncoderStream"
        |+> Static [
            Constructor T<unit>
        ]
        |+> Instance [
            "encoding" =? T<string>
            "readable" =? T<ReadableStream> 
            "writable" =? T<WritableStream> 
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.Encoding" [
                TextEncoderStream
                TextEncoder
                TextDecoderStream
                TextDecoder
                EncodeIntoResult
                DecodeOptions
                TextDecoderOptions
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
