namespace VainZero.Solotter.Desktop

open System
open VainZero.Solotter

type AuthState =
  /// Indicates we have neither application/user access token.
  | AppAuth
  /// Indicates we have an app access token but no user one.
  | UserAuth
    of ApplicationAccessToken
  | CompleteAuth
    of ApplicationAccessToken * UserAccessToken

type IAuthPage =
  inherit IObservable<AuthState>
  inherit IDisposable

  abstract Auth: option<Auth>
