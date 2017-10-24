namespace VainZero.Solotter.Desktop

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module UserConfig =
  open VainZero.Solotter

  let preset =
    {
      EditorFontFamily =
        "Yu Gothic Medium, Meiryo"
      ThemeColorName =
        "Green"
    }

  let fileSystemConfigRepo executablePath =
    let storage = JsonSerializableConfigShape(preset, isPortable = true)
    FileSystemConfigRepo.Create("DesktopUserConfig.xml", storage, executablePath) :> IConfigRepo<_>
