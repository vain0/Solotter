namespace VainZero.Solotter.Desktop

open System
open VainZero.Solotter

type AuthState =
  | Login
    of UserAccessToken
  | Logout

type IAuthPage =
  inherit IObservable<AuthState>
  inherit IDisposable

  abstract Auth: option<Auth>
