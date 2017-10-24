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

  static member Create
    ( initialState: AuthState
    , accessTokenRepo: AccessTokenRepo
    , notifier: Notifier
    ) =
    let disposables =
      new CompositeDisposable()

    let pageDisposable =
      new SerialDisposable()
      |> tap disposables.Add

    let authStateChanged =
      new Subject<AuthState>()
      |> tap disposables.Add

    let accessTokenFromState =
      function
      | AppAuth ->
        AccessToken.empty
      | UserAuth appAccessToken ->
        AccessToken.create (Some appAccessToken) None
      | CompleteAuth (appAccessToken, userAccessToken) ->
        AccessToken.create (Some appAccessToken) (Some userAccessToken)

    let saveAccessToken state =
      let accessToken = accessTokenFromState state
      accessTokenRepo.Save(accessToken)

    authStateChanged |> Observable.subscribe saveAccessToken
    |> disposables.Add

    let pageFromState action =
      match action with
      | AppAuth ->
        new AppAuthPage(notifier, accessTokenRepo) :> IAuthPage
      | UserAuth appAccessToken ->
        new UserAuthPage(appAccessToken, notifier) :> IAuthPage
      | CompleteAuth (appAccessToken, userAccessToken) ->
        let auth = Auth.fromAccessToken appAccessToken userAccessToken
        new SurfacePage(auth, notifier) :> IAuthPage

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

  static member Create(accessTokenRepo: AccessTokenRepo, notifier: Notifier) =
    let accessToken = accessTokenRepo.Find()
    let initialState =
      match (accessToken.AppAccessToken, accessToken.UserAccessToken) with
      | (None, None)
      | (None, Some _) ->
        AppAuth
      | (Some appAccessToken, None) ->
        UserAuth appAccessToken
      | (Some appAccessToken, Some userAccessToken) ->
        CompleteAuth (appAccessToken, userAccessToken)
    AuthFrame.Create(initialState, accessTokenRepo, notifier)

  member this.Content =
    content

  member this.Dispose() =
    dispose ()

  interface IDisposable with
    override this.Dispose() =
      this.Dispose()
