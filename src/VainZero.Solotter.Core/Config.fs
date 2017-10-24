namespace VainZero.Solotter

open System.Runtime.Serialization
open System.IO
open System.Diagnostics
open System

type ConfigFileStorage<'TConfig>
  ( directory: DirectoryInfo
  , fileName: string
  , shape: IConfigShape<'TConfig>
  ) =
  let filePath () =
    Path.Combine(directory.FullName, fileName)

  member this.EnsureDirectoryExists() =
    if Directory.Exists(directory.FullName) |> not then
      Directory.CreateDirectory(directory.FullName) |> ignore

  member this.Find() =
    try
      use stream = File.OpenRead(filePath ())
      shape.ReadFromStream(stream)
    with
    | e ->
      Debug.WriteLine(e |> string)
      shape.Empty

  member this.Save(config: 'TConfig) =
    this.EnsureDirectoryExists()
    let file = FileInfo(filePath ())
    use stream = file.Create()
    stream.SetLength(0L)
    shape.WriteToStream(config, stream)

type FileSystemConfigRepo<'TConfig>
  ( storage: ConfigFileStorage<'TConfig>
  ) =
  interface IConfigRepo<'TConfig> with
    override this.Find() =
      storage.Find()

    override this.Save(config) =
      storage.Save(config)

  static member Create
    ( fileName: string
    , configShape: IConfigShape<'TConfig>
    , executablePath: string
    ) =
    let nonportableDirectory () =
      let localAppDirectory =
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
      Path.Combine(localAppDirectory, "VainZero.Solotter")
    let portableDirectory () =
      let binDirectory =
        Path.GetDirectoryName(executablePath)
      let rootDirectory =
        Path.GetDirectoryName(binDirectory)
      Path.Combine(rootDirectory, "config")
    let directory =
      let path =
        if configShape.IsPortable
        then portableDirectory ()
        else nonportableDirectory ()
      DirectoryInfo(path)
    let storage =
      ConfigFileStorage(directory, fileName, configShape)
    FileSystemConfigRepo(storage)

type JsonSerializableConfigShape<'TConfig>(empty: 'TConfig, isPortable: bool) =
  let deserialize (stream: Stream) =
    let serializer = DataContractSerializer(typeof<'TConfig>)
    serializer.ReadObject(stream) :?> 'TConfig

  let serialize (stream: Stream) config =
    let serializer = DataContractSerializer(typeof<'TConfig>)
    serializer.WriteObject(stream, config)

  interface IConfigShape<'TConfig> with
    override this.IsPortable = isPortable

    override this.Empty = empty

    override this.ReadFromStream(stream) =
      deserialize stream

    override this.WriteToStream(config, stream) =
      serialize stream config
