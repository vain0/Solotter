namespace VainZero.Solotter

open System.IO
open System.Runtime.Serialization
open System

type AccessTokenRepo(filePath: string) =
  let deserialize (stream: Stream) =
    let serializer = DataContractSerializer(typeof<AccessToken>)
    serializer.ReadObject(stream) :?> AccessToken

  let serialize (stream: Stream) accessToken =
    let serializer = DataContractSerializer(typeof<AccessToken>)
    serializer.WriteObject(stream, accessToken)

  let file () =
    FileInfo(filePath)

  member this.Find() =
    use stream = File.OpenRead(filePath)
    deserialize stream

  member this.Save(accessToken: AccessToken) =
    let file = file ()
    use stream = file.Create()
    stream.SetLength(0L)
    accessToken |> serialize stream

  static member Create() =
    AccessTokenRepo(@"VainZero.Solotter.AccessToken.xml")
