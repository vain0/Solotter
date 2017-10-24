namespace VainZero.Solotter

open System.IO
open System.Runtime.Serialization

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module AccessToken =
  let deserialize (stream: Stream) =
    let serializer = DataContractSerializer(typeof<AccessToken>)
    serializer.ReadObject(stream) :?> AccessToken

  let serialize (stream: Stream) accessToken =
    let serializer = DataContractSerializer(typeof<AccessToken>)
    serializer.WriteObject(stream, accessToken)

  let private filePath =
    @"VainZero.Solotter.AccessToken.xml"

  let load () =
    use stream = File.OpenRead(filePath)
    deserialize stream

  let save accessToken =
    let file = FileInfo(filePath)
    if file.Exists then file.Delete()
    use stream = file.Create()
    accessToken |> serialize stream
