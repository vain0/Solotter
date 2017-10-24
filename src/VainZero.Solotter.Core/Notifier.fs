namespace VainZero.Solotter

open System.Windows

[<Sealed>]
type MessageBoxNotifier(caption: string) =
  inherit Notifier()

  override this.NotifyInfo(message) =
    MessageBox.Show(message, caption) |> ignore

  override this.Confirm(message) =
    MessageBox.Show(message, caption, MessageBoxButton.YesNo) = MessageBoxResult.Yes
