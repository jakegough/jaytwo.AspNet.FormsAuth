# jaytwo.AspNet.FormsAuth

Using ASP.NET FormsAuthentication to offer roles-based security and more complex user objects.

Keeping track of the currently logged in user's name is fine, but often I want to hold on to the user's roles, integer Id and TimeZone as well.  This library simply puts all that in the `FormsAuthenticationTicket`, which is managed by ASP.NET as an encrypted cookie. 

## Installation

Get the nuget package: [jaytwo.AspNet.FormsAuth](https://www.nuget.org/packages/jaytwo.AspNet.FormsAuth/)

The package contains binaries for .NET frameworks: `3.5`/`4.0`/`4.5.`.

## Implementation and Usage

### Web.config

Important Web.config settings:
- /configuration/appSettings/autoFormsAuthentication (for MVC3+)
- /configuration/appSettings/enableSimpleMembership (for MVC3+)
- /configuration/system.web/machineKey
- /configuration/system.web/authentication
- /configuration/system.web/authorization
- /configuration/system.web/roleManager
- /configuration/location

```xml
<configuration>
  <appSettings>
    <!-- If you're using MVC 3+, you need to disable autoFormsAuthentication and enableSimpleMembership, see http://stackoverflow.com/questions/4626647 -->
    <add key="autoFormsAuthentication" value="false"/>
    <add key="enableSimpleMembership" value="false"/>
  </appSettings>
  <system.web>
    <!-- though not strictly necessary, explicitly setting the machineKey will ensure that a FormsAuthenticationTicket encrypted by your app will successfully be decrypted by your app later (e.g., across server nodes and app pool recycles) -->
    <!-- And for goodness sake, don't copy and paste these example machineKey values, go generate new ones! -->
    <machineKey        
      validationKey="8553736E684799FF0D9C3E3E6A84E0F2DF654603048F15C69F1EC702A82D9E84C4FB6876F2ECE59CC77C7DD9FC43A9BD3A141E2CC840F8C25C7DA6E359C1F744"
      decryptionKey="ED03F197152F98288414994982469DC617D8CD015C9EC640"
      validation="SHA1" decryption="AES" />
    <!-- turn on Forms authentication -->
    <authentication mode="Forms">
      <forms name="ASPXFORMSAUTH" protection="All" path="/" loginUrl="~/Login/" defaultUrl="~/" timeout="120"/>
    </authentication>
    <!-- set default permissions -->
    <authorization>
      <allow users="*"/>
    </authorization>
    <!-- turn on the FormsAuthenticationTicketRoleProvider -->
    <roleManager defaultProvider="FormsAuthenticationTicketRoleProvider" enabled="true">
      <providers>
        <clear/>
        <add name="FormsAuthenticationTicketRoleProvider" type="jaytwo.AspNet.FormsAuth.FormsAuthenticationTicketRoleProvider"/>
      </providers>
    </roleManager>
  </system.web>
  <!-- set specific permissions for a given path, in this case restrict to users with the 'user' role -->
  <location path="Protected">
    <system.web>
      <authorization>
        <allow roles="user"/>
        <deny users="*"/>
      </authorization>
    </system.web>
  </location>
</configuration>
```

### Define your `IUserProfile`

_If you are only using this library to leverage the role provider, feel free to skip this section._

The only thing to keep in mind is that this will be serialized, so keep it a simple property bag.

```cs
using jaytwo.AspNet.FormsAuth

namespace MyApplication.Web.Security
{
  public class AppUser : IUserProfile
  {
    public static SimpleUserProfile Current
    {
      get
      {
        return FormsAuthenticationAppHost.GetSignedInUserProfileAs<AppUser>();
      }
    }
    
    public string UserName { get; set; }
    public int UserId { get; set; }
    public string TimeZone { get; set; }
  }
}
```

Then to use `AppUser`:
```cs
using MyApplication.Web.Security
...

var userInfo = GetUserInfoByUserId(AppUser.Current.UserId);
var userLocalTime = TimeZoneUtility.UtcToLocalTime(DateTime.UtcNow, AppUser.Current.TimeZone);
```

### Signing In and Out

Use `jaytwo.AspNet.FormsAuth.FormsAuthenticationAppHost` to sign in and out just like you would using the static methods in  `System.Web.Security.FormsAuthentication`.

Currently there is no `SignIn` overload with a `createPersistentCookie` parameter.  This is mostly because I was afraid of holding on to stale data.  My own use cases are usually for some kind of delegated authentication, so asking a user to sign in every time is no big deal.

```cs
using jaytwo.AspNet.FormsAuth
using MyApplication.Web.Security
...

// sign in without a custom IUserProfile
var userName = "Jake";
var roles = new[] { "user", "admin" };
FormsAuthenticationAppHost.SignIn(userName, roles);

// sign in with a custom IUserProfile
var profile = new AppUser() 
  {
    UserName = "Jake",
    UserId = 1234,
    TimeZone = "America/Denver"
  };
var roles = new[] { "user", "admin" };
FormsAuthenticationAppHost.SignIn(profile, roles);

// sign out
FormsAuthenticationAppHost.SignOut();
```

### `FormsAuthenticationAppHost` members

The easiest way to see what's available is to just look at the class definition:

```cs
namespace jaytwo.AspNet.FormsAuth
{
  public static class FormsAuthenticationAppHost
  {
    // events
    public delegate void SignedInHandler(IUserProfile profile, string[] roles);
    public static event SignedInHandler SignedIn;
    public delegate void SignedOutHandler(IUserProfile profile);
    public static event SignedOutHandler SignedOut;
    
    // sign in
    public static void SignIn(string userName);
    public static void SignIn(string userName, string[] roles);
    public static void SignIn(IUserProfile profile);
    public static void SignIn(IUserProfile profile, string[] roles);
    
    // sign out
    public static void SignOut();
    
    // other
    public static IUserProfile SignedInUserProfile { get; }
    public static T GetSignedInUserProfileAs<T>();
    public static string[] SignedInUserRoles { get; }
    public static string SignedInUserName();
    public static DateTime? SignedInTimestampUtc { get; }
  }
}
```