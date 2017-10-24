namespace VainZero.Solotter.Desktop

open System
open System.Reactive.Disposables
open System.Reactive.Linq
open System.Reactive.Subjects
open Reactive.Bindings
open VainZero.Solotter

[<Sealed>]
type AuthFrame
  private
  ( content: IReadOnlyReactiveProperty<IAuthPage>
  , disposable: IDisposable
  ) =
  let dispose () =
    disposable.Dispose()

  private new
    ( applicationAccessToken
    , initialState: AuthState
    , accessTokenRepo: AccessTokenRepo
    ) =
    let notifier =
      MessageBoxNotifier("Solotter")

    let disposables =
      new CompositeDisposable()

    let pageDisposable =
      new SerialDisposable()
      |> tap disposables.Add

    let authStateChanged =
      new Subject<AuthState>()
      |> tap disposables.Add

    let saveAccessToken action =
      let userAccessToken =
        match action with
        | Login userAccessToken ->
          Some userAccessToken
        | Logout ->
          None
      let accessToken =
        {
          ApplicationAccessToken =
            applicationAccessToken
          UserAccessToken =
            userAccessToken
        }
      accessTokenRepo.Save(accessToken)

    authStateChanged |> Observable.subscribe saveAccessToken
    |> disposables.Add

    let pageFromState action =
      match action with
      | Login userAccessToken ->
        let authentication =
          Auth.fromAccessToken applicationAccessToken userAccessToken
        new SurfacePage(authentication, notifier) :> IAuthPage
      | Logout ->
        new UserAuthPage(applicationAccessToken, notifier) :> IAuthPage

    let content =
      authStateChanged
        .StartWith(initialState)
        .Select(pageFromState)
        .Do(fun page ->
          pageDisposable.Disposable <-
            StableCompositeDisposable.Create
              ( page
              , page |> Observable.subscribe authStateChanged.OnNext
              )
          )
        .ToReadOnlyReactiveProperty()
      |> tap disposables.Add

    new AuthFrame(content, disposables)

  new(accessTokenRepo: AccessTokenRepo) =
    let accessToken = accessTokenRepo.Find()
    let applicationAccessToken =
      accessToken.ApplicationAccessToken
    let initialState =
      match accessToken.UserAccessToken with
      | Some userAccessToken ->
        Login userAccessToken
      | None ->
        Logout
    new AuthFrame(applicationAccessToken, Logout, accessTokenRepo)

  new() =
    new AuthFrame(AccessTokenRepo.Create())

  member this.Content =
    content

  member this.Dispose() =
    dispose ()

  interface IDisposable with
    override this.Dispose() =
      this.Dispose()
