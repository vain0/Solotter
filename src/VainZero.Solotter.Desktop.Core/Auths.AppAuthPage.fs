namespace VainZero.Solotter.Desktop

open System
open System.Reactive.Linq
open System.Windows.Input
open Reactive.Bindings
open VainZero.Solotter
open System.Reactive.Disposables
open System.Reactive.Subjects

/// Represents a page to specify an application access token.
[<Sealed>]
type AppAuthPage
  ( notifier: Notifier
  , accessTokenRepo: AccessTokenRepo
  ) =
  let disposables = new CompositeDisposable()

  let consumerKey =
    new ReactiveProperty<string>("")
    |> tap disposables.Add

  let consumerSecret =
    new ReactiveProperty<string>("")
    |> tap disposables.Add

  let authStateChanged =
    new AsyncSubject<AuthState>()
    |> tap disposables.Add

  let goCommand =
    let condition =
      consumerKey.CombineLatest(consumerSecret, fun consumerKey consumerSecret ->
        String.IsNullOrEmpty(consumerKey) |> not
        && String.IsNullOrEmpty(consumerSecret) |> not
      )
    let execute () =
      let t =
        {
          ConsumerKey = consumerKey.Value
          ConsumerSecret = consumerSecret.Value
        }
      authStateChanged.OnNext(UserAuth t)
      authStateChanged.OnCompleted()
    condition.ToReactiveCommand<unit>()
    |> tap (fun command -> command |> Observable.subscribe execute |> disposables.Add)

  member this.ConsumerKey =
    consumerKey

  member this.ConsumerSecret =
    consumerSecret

  member this.GoCommand =
    goCommand :> ICommand

  member this.Dispose() =
    disposables.Dispose ()

  interface IObservable<AuthState> with
    override this.Subscribe(observer) =
      authStateChanged.Subscribe(observer)

  interface IDisposable with
    override this.Dispose() =
      this.Dispose()

  interface IAuthPage with
    override this.Auth =
      None
