namespace VainZero.Solotter

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Authentication =
  let create applicationAccessToken userAccessToken twitter =
    {
      ApplicationAccessToken =
        applicationAccessToken
      UserAccessToken =
        userAccessToken
      Twitter =
        twitter
    }

  let fromAccessToken applicationAccessToken userAccessToken =
    let (a: ApplicationAccessToken) = applicationAccessToken
    let (u: UserAccessToken) = userAccessToken
    let twitter =
      Tweetinvi.Auth.SetUserCredentials
        ( a.ConsumerKey
        , a.ConsumerSecret
        , u.AccessToken
        , u.AccessSecret
        )
    create a u twitter
