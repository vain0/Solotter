namespace VainZero.Solotter

open System.IO
open System.Runtime.Serialization
open System
open System.Diagnostics

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module AccessToken =
  let create a u: AccessToken =
    {
      AppAccessToken =
        a
      UserAccessToken =
        u
    }

  let empty =
    create None None

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
    try
      use stream = File.OpenRead(filePath)
      deserialize stream
    with
    | e ->
      Debug.WriteLine(e |> string)
      AccessToken.empty

  member this.Save(accessToken: AccessToken) =
    let file = file ()
    use stream = file.Create()
    stream.SetLength(0L)
    accessToken |> serialize stream

  static member CreateForWindows() =
    let localAppDirectory =
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
    let configDirectory =
      Path.Combine(localAppDirectory, "VainZero.Solotter")
    if Directory.Exists(configDirectory) |> not then
      Directory.CreateDirectory(configDirectory) |> ignore
    let filePath =
      Path.Combine(configDirectory, @"AccessToken.xml")
    AccessTokenRepo(filePath)
