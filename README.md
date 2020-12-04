# .NET 5 WebApi + Blazor server + Auth0 JWT & cookies
A demo of .NET 5 Web API + Blazor server + Auth0 with JWT and cookie authentication

## Setup
Make sure you first follow the Auth0 instructions for setting up your Auth0 account:

#### Tenant setup

https://auth0.com/docs/get-started/learn-the-basics

#### Application setup

https://auth0.com/docs/applications/set-up-an-application/register-native-applications

(Make sure you properly set the callback values in Auth0!!! Usually `https://localhost:5001/callback`)

#### API setup

https://auth0.com/docs/get-started/set-up-apis

#### Setup identity providers (Connections)

https://auth0.com/docs/identityproviders

#### Edit appsettings

Then replace the values in `appsettings.Development.json` with the values from your Auth0 settings.

![Auth0_settings_explanation](Auth0_settings_explanation.png)

## Try it out

When you debug the app, you should be presented with a Blazor app that looks very similar to a default Blazor app. You'll see a "Login" link in the upper-right which allows you to login with Auth0.

![app1](app1.jpg)

The "Swagger Doc" menu item will present you with interactive documentation for the Web API controllers.

![app2](app2.jpg)

#### PublicWeatherForecast

This controller requires no authentication. You can use it without logging in.

#### CookieWeatherForecast

This controller requires cookie authentication. If you try to execute its endpoint before logging in, you'll see an error message. 

I'm not advocating for using cookies for API controllers (one of the reasons being that cookie-based sessions don't scale up as nicely as JWT tokens), but the controller is in the project to demonstrate that as long as you're logged in to the Blazor web app, and because the entire Blazor/WebAPI combo app is using cookie auth by default, this controller requires a valid cookie-based session in order to produce successful responses.

The CookieWeatherForecastController is marked with the `[Authorize]` attribute. It will use whichever authentication scheme has been set as the default in Startup.cs; in the case of this app, that's cookie-based auth.

#### JwtWeatherForecast

This controller requires JWT authentication. In order to make successful requests, you'll need to issue requests to the endpoint using something like [Postman](https://www.postman.com/), [HttpClient](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0), or [Refit](https://github.com/reactiveui/refit) (my preferred library for creating http clients for use in Xamarin apps). Each HTTP request must contain an `Authorization` header with contents of `Bearer {your user's JWT token}`.

The JwtWeatherForecastController is marked with the `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]` attribute. By specifying the `AuthenticationSchemes` parameter to use JWT, we override the default auth scheme of cookies that has been set in Startup.cs. In order for Blazor to perform login and auth, those defaults need to be set to cookie, and then we override them wherever needed on the API controllers in order to force JWT.

By using JWT auth on a controller, we force ASP.NET to parse and decode the `Authorization` header on any inbound HTTP requests, and assign it to a `User` property available in the HttpContext. The authorization header format conforms to the standard `Authorization: Bearer {your user's JWT token}`. If the header is present in an HTTP request, but the `AuthenticationSchemes` param hasn't been set to JWT, then ASP.NET won't look for the header and the `User` property in HttpContext will be null.

#### Extending authorization to role-based permissions
Auth0 supports [Role-based Access Control (RBAC)](https://auth0.com/docs/authorization/rbac), and you can learn more about how to configure it [here](https://auth0.com/docs/authorization/how-to-use-auth0s-core-authorization-feature-set). This allows you to partition access to your app / endpoints based on which users should have access to which resources. When implemented, it's as easy as annotating your controllers / pages with something like:

```[Authorize(Roles = "Admin, ContentCurator, SomeOtherFancyRole")]```

And of course, you can also add in the `AuthenticationSchemes` param:

```[Authorize(Roles = "Admin, ContentCurator, SomeOtherFancyRole", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]```


The really nice part about this is using Auth0 to manage roles, assigning them to users in Auth0, and then having those roles flow through to the app via either cookie-based or JWT-based mechanisms.
