namespace VainZero.Solotter

  open System.Runtime.Serialization
  open Prism.Commands

  type RaisableCommand<'TParameter> =
    DelegateCommand<'TParameter>

  [<AbstractClass>]
  type Notifier() =
    abstract NotifyInfo: string -> unit
    abstract Confirm: string -> bool

  [<DataContract>]
  type ApplicationAccessToken =
    {
      [<field: DataMember>]
      ConsumerKey:
        string
      [<field: DataMember>]
      ConsumerSecret:
        string
    }

  [<DataContract>]
  type UserAccessToken =
    {
      [<field: DataMember>]
      AccessToken:
        string
      [<field: DataMember>]
      AccessSecret:
        string
    }

  [<DataContract>]
  type AccessToken =
    {
      [<field: DataMember>]
      ApplicationAccessToken:
        ApplicationAccessToken
      [<field: DataMember>]
      UserAccessToken:
        option<UserAccessToken>
    }

  type Authentication =
    {
      ApplicationAccessToken:
        ApplicationAccessToken
      UserAccessToken:
        UserAccessToken
      Twitter:
        Tweetinvi.Models.ITwitterCredentials
    }
