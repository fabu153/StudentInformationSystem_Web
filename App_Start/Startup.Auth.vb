Imports System
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin
Imports Microsoft.Owin.Security.Cookies
Imports Microsoft.Owin.Security.DataProtection
Imports Microsoft.Owin.Security.Google
Imports Owin

Partial Public Class Startup
    Public Sub ConfigureAuth(app As IAppBuilder)
        ' Configure the db context, user manager and signin manager to use a single instance per request
        app.CreatePerOwinContext(AddressOf ApplicationDbContext.Create)
        app.CreatePerOwinContext(Of ApplicationUserManager)(AddressOf ApplicationUserManager.Create)
        app.CreatePerOwinContext(Of ApplicationSignInManager)(AddressOf ApplicationSignInManager.Create)

        ' Enable the application to use a cookie to store information for the signed in user
        app.UseCookieAuthentication(New CookieAuthenticationOptions() With {
            .AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            .Provider = New CookieAuthenticationProvider() With {
                .OnValidateIdentity = SecurityStampValidator.OnValidateIdentity(Of ApplicationUserManager, ApplicationUser)(
                    validateInterval:=TimeSpan.FromMinutes(30),
                    regenerateIdentity:=Function(manager, user) user.GenerateUserIdentityAsync(manager))
            },
            .LoginPath = New PathString("/Account/Login")
        })

        ' Use a cookie to temporarily store information about a user logging in with a third party login provider
        app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie)

        ' Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
        app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5))

        ' Enables the application to remember the second login verification factor such as phone or email.
        app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie)

        ' -- Third party logins (Facebook, Google, etc.) would go here --

        ' -------------------------------------------------------------
        ' CREATE ROLES & ADMIN USER (Add your code at the end of ConfigureAuth)
        ' -------------------------------------------------------------
        Using context As New ApplicationDbContext()
            Dim roleManager = New RoleManager(Of IdentityRole)(
                New RoleStore(Of IdentityRole)(context))

            ' 1) Create Student role if it doesn’t exist
            If Not roleManager.RoleExists("Student") Then
                roleManager.Create(New IdentityRole("Student"))
            End If

            ' 2) Create Admin role if it doesn’t exist
            If Not roleManager.RoleExists("Admin") Then
                roleManager.Create(New IdentityRole("Admin"))
            End If

            ' 3) Optionally create an admin user if none exists
            Dim userManager = New UserManager(Of ApplicationUser)(
                New UserStore(Of ApplicationUser)(context))

            Dim adminEmail As String = "admin@example.com"
            Dim adminUser = userManager.FindByName(adminEmail)
            If adminUser Is Nothing Then
                adminUser = New ApplicationUser() With {
                    .UserName = adminEmail,
                    .Email = adminEmail
                }
                Dim result = userManager.Create(adminUser, "AdminPassword123!")
                If result.Succeeded Then
                    userManager.AddToRole(adminUser.Id, "Admin")
                End If
            End If
        End Using
    End Sub
End Class
