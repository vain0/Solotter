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

  let fileSystemAccessTokenRepo executablePath =
    let configShape = JsonSerializableConfigShape<AccessToken>(empty, isPortable = false)
    FileSystemConfigRepo.Create("AccessToken.xml", configShape, executablePath) :> IConfigRepo<_>
