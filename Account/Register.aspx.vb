Imports System
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports Npgsql
Imports System.Configuration
Imports System.Globalization

Partial Public Class Register
    Inherits Page

    Protected Sub CreateUser_Click(sender As Object, e As EventArgs)
        ' Server-side check for password match (backup)
        If Password.Text <> ConfirmPassword.Text Then
            ErrorMessage.Text = "Passwords do not match."
            Return
        End If

        ' Validate required fields (server-side)
        If String.IsNullOrWhiteSpace(Email.Text) OrElse
           String.IsNullOrWhiteSpace(Password.Text) OrElse
           String.IsNullOrWhiteSpace(txtFirstName.Text) OrElse
           String.IsNullOrWhiteSpace(txtLastName.Text) Then

            ErrorMessage.Text = "All fields are required."
            Return
        End If

        ' Convert first and last names to proper title case.
        Dim ti As TextInfo = CultureInfo.CurrentCulture.TextInfo
        Dim firstNameProper As String = ti.ToTitleCase(txtFirstName.Text.ToLower())
        Dim lastNameProper As String = ti.ToTitleCase(txtLastName.Text.ToLower())

        Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
        Dim signInManager = Context.GetOwinContext().Get(Of ApplicationSignInManager)()

        Dim user = New ApplicationUser() With {
            .UserName = Email.Text.Trim(),
            .Email = Email.Text.Trim()
        }
        Dim result = manager.Create(user, Password.Text)
        If result.Succeeded Then
            Try
                ' Assign the user to the "Student" role
                Dim roleResult = manager.AddToRole(user.Id, "Student")
                If Not roleResult.Succeeded Then
                    ErrorMessage.Text = "User created, but role assignment failed."
                    Return
                End If

                ' Insert the student record into Supabase using proper case names
                InsertStudentRecord(user.Id, firstNameProper, lastNameProper, Email.Text.Trim())
            Catch ex As Exception
                ' Log the exception as needed
                ErrorMessage.Text = "An error occurred while creating your student record: " & ex.Message
                Return
            End Try

            ' Automatically sign in the newly registered user
            signInManager.SignIn(user, isPersistent:=False, rememberBrowser:=False)

            ' Redirect to the ReturnUrl or a default page
            IdentityHelper.RedirectToReturnUrl(Request.QueryString("ReturnUrl"), Response)
        Else
            Dim err = result.Errors.FirstOrDefault()
            If err.ToLower().Contains("already taken") Then
                ErrorMessage.Text = "An account with that email already exists."
            Else
                ErrorMessage.Text = err
            End If
        End If
    End Sub

    ''' <summary>
    ''' Inserts a student record into the Supabase 'students' table.
    ''' Assumes your students table has columns: identity_user_id, first_name, last_name, email, enrollment_date.
    ''' </summary>
    Private Sub InsertStudentRecord(identityUserId As String, firstName As String, lastName As String, email As String)
        Dim connStr As String = ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString
        Using conn As New NpgsqlConnection(connStr)
            conn.Open()
            Dim sql As String = "INSERT INTO students (identity_user_id, first_name, last_name, email, enrollment_date) " &
                                "VALUES (@userId, @fName, @lName, @em, @enrollDate)"
            Using cmd As New NpgsqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@userId", identityUserId)
                cmd.Parameters.AddWithValue("@fName", firstName)
                cmd.Parameters.AddWithValue("@lName", lastName)
                cmd.Parameters.AddWithValue("@em", email)
                cmd.Parameters.AddWithValue("@enrollDate", DateTime.Now)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
End Class
