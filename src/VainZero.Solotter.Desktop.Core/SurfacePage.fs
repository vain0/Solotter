﻿namespace VainZero.Solotter.Desktop

open System
open System.Reactive.Linq
open System.Windows.Input
open Reactive.Bindings
open VainZero.Solotter

type TabItem =
  {
    Header:
      string
    Content:
      obj
  }
with
  static member Create(name, content) =
    {
      Header =
        name
      Content =
        content
    }

[<Sealed>]
type SurfacePage(auth: Auth, userConfig: UserConfig, notifier: Notifier) =
  let twitter =
    auth.Twitter

  let tweetEditor =
    new TweetEditor(twitter, userConfig.EditorFontFamily, notifier)

  let selfTimeline =
    new SelfTimeline(twitter, notifier)

  let tabItems =
    [|
      TabItem.Create("Empty", null)
      TabItem.Create("Self", selfTimeline)
    |]

  let selectedTabItem =
    new ReactiveProperty<_>(initialValue = tabItems.[0])

  let logoutCommand =
    new ReactiveCommand()

  let unsubmitCommandSubscription =
    tweetEditor.UnsubmitCommand |> Observable.subscribe
      (fun () ->
        let latestTweet = selfTimeline.Timeline.Tweets |> Seq.tryHead
        async {
          match latestTweet with
          | Some latestTweet ->
            let! deleted = Tweet.deleteAsync twitter notifier latestTweet
            return ()
          | None ->
            ()
        } |> Async.Start
      )

  let dispose () =
    logoutCommand.Dispose()
    selfTimeline.Dispose()

  member this.TweetEditor =
    tweetEditor

  member this.SelfTimeline =
    selfTimeline

  member this.TabItems =
    tabItems

  member this.SelectedTabItem =
    selectedTabItem

  member this.LogoutCommand =
    logoutCommand :> ICommand

  member this.Dispose() =
    dispose ()

  interface IObservable<AuthState> with
    override this.Subscribe(observer) =
      logoutCommand
        .Select(fun _ -> UserAuth auth.AppAccessToken)
        .Subscribe(observer)

  interface IDisposable with
    override this.Dispose() =
      this.Dispose()

  interface IAuthPage
