Imports System
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin

Partial Public Class Login
    Inherits Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim rawReturnUrl As String = Request.QueryString("ReturnUrl")
        If Not IsPostBack AndAlso Not String.IsNullOrEmpty(rawReturnUrl) AndAlso rawReturnUrl.Contains("/Account/Login") Then
            Response.Redirect("~/Default.aspx")
            Return
        End If

        ' Set the Register hyperlink to point to the Register page.
        RegisterHyperLink.NavigateUrl = "Register"

        ' Append ReturnUrl if present.
        Dim returnUrl As String = HttpUtility.UrlEncode(rawReturnUrl)
        If Not String.IsNullOrEmpty(returnUrl) Then
            RegisterHyperLink.NavigateUrl &= "?ReturnUrl=" & returnUrl
        End If
    End Sub

    Protected Sub LogIn(sender As Object, e As EventArgs)
        If Page.IsValid Then
            Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
            Dim signInManager = Context.GetOwinContext().GetUserManager(Of ApplicationSignInManager)()

            Dim emailVal As String = Email.Text.Trim()
            Dim passwordVal As String = Password.Text
            Dim rememberVal As Boolean = RememberMe.Checked

            Dim result = signInManager.PasswordSignIn(emailVal, passwordVal, rememberVal, shouldLockout:=False)
            Select Case result
                Case SignInStatus.Success
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString("ReturnUrl"), Response)
                Case SignInStatus.LockedOut
                    Response.Redirect("/Account/Lockout")
                Case SignInStatus.RequiresVerification
                    Response.Redirect(String.Format("/Account/TwoFactorAuthenticationSignIn?ReturnUrl={0}&RememberMe={1}",
                                                    HttpUtility.UrlEncode(Request.QueryString("ReturnUrl")),
                                                    rememberVal), True)
                Case Else
                    FailureText.Text = "Invalid login attempt."
                    ErrorMessage.Visible = True
            End Select
        End If
    End Sub
End Class
