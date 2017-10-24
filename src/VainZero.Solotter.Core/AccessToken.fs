namespace VainZero.Solotter

open System.IO
open System.Runtime.Serialization

[<DataContract>]
type ApplicationAccessToken =
  {
    [<field: DataMember>]
    ConsumerKey:
      string
    [<field: DataMember>]
    ConsumerSecret:
      string
  }

[<DataContract>]
type UserAccessToken =
  {
    [<field: DataMember>]
    AccessToken:
      string
    [<field: DataMember>]
    AccessSecret:
      string
  }

[<DataContract>]
type AccessToken =
  {
    [<field: DataMember>]
    ApplicationAccessToken:
      ApplicationAccessToken
    [<field: DataMember>]
    UserAccessToken:
      option<UserAccessToken>
  }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module AccessToken =
  let login userAccessToken accessToken =
    { accessToken with UserAccessToken = Some userAccessToken }

  let logout accessToken =
    { accessToken with UserAccessToken = None }

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
