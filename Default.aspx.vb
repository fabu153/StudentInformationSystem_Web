Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If User.Identity.IsAuthenticated Then
                If User.IsInRole("Admin") Then
                    hlGetStarted.NavigateUrl = ResolveUrl("~/ManageStudents.aspx")
                Else
                    hlGetStarted.NavigateUrl = ResolveUrl("~/StudentDashboard.aspx")
                End If
            Else
                hlGetStarted.NavigateUrl = ResolveUrl("~/Account/Login.aspx")
            End If
        End If
    End Sub
End Class
