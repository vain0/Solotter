namespace VainZero.Solotter

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Auth =
  let create appAccessToken userAccessToken twitter =
    {
      AppAccessToken =
        appAccessToken
      UserAccessToken =
        userAccessToken
      Twitter =
        twitter
    }

  let fromAccessToken appAccessToken userAccessToken =
    let (a: AppAccessToken) = appAccessToken
    let (u: UserAccessToken) = userAccessToken
    let twitter =
      Tweetinvi.Auth.SetUserCredentials
        ( a.ConsumerKey
        , a.ConsumerSecret
        , u.AccessToken
        , u.AccessSecret
        )
    create a u twitter
