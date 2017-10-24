namespace VainZero.Solotter.Desktop

  open System
  open System.Runtime.Serialization
  open VainZero.Solotter

  type AuthState =
    /// Indicates we have neither app/user access token.
    | AppAuth
    /// Indicates we have an app access token but no user one.
    | UserAuth
      of AppAccessToken
    | CompleteAuth
      of AppAccessToken * UserAccessToken

  type IAuthPage =
    inherit IObservable<AuthState>
    inherit IDisposable
