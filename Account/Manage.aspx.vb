Imports System
Imports System.Threading.Tasks
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin.Security
Imports Owin

Partial Public Class Manage
    Inherits System.Web.UI.Page

    ' -------------------------------------------------
    ' Existing Properties and Fields
    ' -------------------------------------------------
    Protected Property SuccessMessage() As String
        Get
            Return m_SuccessMessage
        End Get
        Private Set(value As String)
            m_SuccessMessage = value
        End Set
    End Property
    Private m_SuccessMessage As String

    Private Function HasPassword(manager As ApplicationUserManager) As Boolean
        Dim appUser = manager.FindById(User.Identity.GetUserId())
        Return (appUser IsNot Nothing AndAlso appUser.PasswordHash IsNot Nothing)
    End Function

    Protected Property HasPhoneNumber() As Boolean
        Get
            Return m_HasPhoneNumber
        End Get
        Private Set(value As Boolean)
            m_HasPhoneNumber = value
        End Set
    End Property
    Private m_HasPhoneNumber As Boolean

    Protected Property TwoFactorEnabled() As Boolean
        Get
            Return m_TwoFactorEnabled
        End Get
        Private Set(value As Boolean)
            m_TwoFactorEnabled = value
        End Set
    End Property
    Private m_TwoFactorEnabled As Boolean

    Protected Property TwoFactorBrowserRemembered() As String
        Get
            Return m_TwoFactorBrowserRemembered
        End Get
        Private Set(value As String)
            m_TwoFactorBrowserRemembered = value
        End Set
    End Property
    Private m_TwoFactorBrowserRemembered As String

    Public Property LoginsCount As Integer

    ' -------------------------------------------------
    ' Page Load (Async) - Load the user's EmailNotificationsEnabled setting
    ' -------------------------------------------------
    Protected Async Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim userManager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
        Dim userId = User.Identity.GetUserId()

        ' Load other settings
        HasPhoneNumber = String.IsNullOrEmpty(userManager.GetPhoneNumber(userId))
        TwoFactorEnabled = userManager.GetTwoFactorEnabled(userId)
        LoginsCount = userManager.GetLogins(userId).Count

        Dim authenticationManager = HttpContext.Current.GetOwinContext().Authentication

        If Not IsPostBack Then
            ' Determine the sections to render
            If HasPassword(userManager) Then
                ChangePassword.Visible = True
            Else
                CreatePassword.Visible = True
                ChangePassword.Visible = False
            End If

            ' Render success message (if any)
            Dim message = Request.QueryString("m")
            If message IsNot Nothing Then
                Form.Action = ResolveUrl("~/Account/Manage")
                SuccessMessage = If(message = "ChangePwdSuccess", "Your password has been changed.",
                    If(message = "SetPwdSuccess", "Your password has been set.",
                    If(message = "RemoveLoginSuccess", "The account was removed.",
                    If(message = "AddPhoneNumberSuccess", "Phone number has been added",
                    If(message = "RemovePhoneNumberSuccess", "Phone number was removed", String.Empty)))))
                SuccessMessagePlaceHolder.Visible = Not String.IsNullOrEmpty(SuccessMessage)
            End If

            ' NEW: Load the current Email Notification setting from the database
            Dim user = Await userManager.FindByIdAsync(userId)
            If user IsNot Nothing Then
                chkEmailNotifications.Checked = user.EmailNotificationsEnabled
            End If
        End If
    End Sub

    Private Sub AddErrors(result As IdentityResult)
        For Each [error] As String In result.Errors
            ModelState.AddModelError("", [error])
        Next
    End Sub

    ' -------------------------------------------------
    ' Existing Methods (RemovePhone, TwoFactor, etc.)
    ' -------------------------------------------------
    Protected Sub RemovePhone_Click(sender As Object, e As EventArgs)
        Dim userManager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
        Dim signInManager = Context.GetOwinContext().Get(Of ApplicationSignInManager)()
        Dim result = userManager.SetPhoneNumber(User.Identity.GetUserId(), Nothing)
        If Not result.Succeeded Then
            Return
        End If
        Dim userInfo = userManager.FindById(User.Identity.GetUserId())
        If userInfo IsNot Nothing Then
            signInManager.SignIn(userInfo, isPersistent:=False, rememberBrowser:=False)
            Response.Redirect("/Account/Manage?m=RemovePhoneNumberSuccess")
        End If
    End Sub

    Protected Sub TwoFactorDisable_Click(sender As Object, e As EventArgs)
        Dim userManager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
        userManager.SetTwoFactorEnabled(User.Identity.GetUserId(), False)
        Response.Redirect("/Account/Manage")
    End Sub

    Protected Sub TwoFactorEnable_Click(sender As Object, e As EventArgs)
        Dim userManager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
        userManager.SetTwoFactorEnabled(User.Identity.GetUserId(), True)
        Response.Redirect("/Account/Manage")
    End Sub

    ' -------------------------------------------------
    ' NEW: Save Email Notification Setting (Async)
    ' -------------------------------------------------
    Protected Async Sub btnSaveEmailSettings_Click(sender As Object, e As EventArgs)
        Try
            Dim userManager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
            Dim currentUserId = Me.User.Identity.GetUserId()
            Dim currentUser = Await userManager.FindByIdAsync(currentUserId)
            If currentUser IsNot Nothing Then
                currentUser.EmailNotificationsEnabled = chkEmailNotifications.Checked
                Dim result = Await userManager.UpdateAsync(currentUser)
                If result.Succeeded Then
                    lblEmailSettingsStatus.Text = "Settings saved. Email notifications are " &
                    If(chkEmailNotifications.Checked, "enabled.", "disabled.")
                Else
                    lblEmailSettingsStatus.Text = "Error saving settings."
                End If
            End If
        Catch ex As Exception
            lblEmailSettingsStatus.Text = "Error saving email settings: " & ex.Message
        End Try
    End Sub
End Class
