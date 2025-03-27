Imports Npgsql
Imports System.Configuration
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin

Public Class ManageStudents
    Inherits System.Web.UI.Page

    Private ReadOnly Property SupabaseConnectionString As String
        Get
            Return ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindStudents()
            txtEnrollmentDate.Text = DateTime.Now.ToString("yyyy-MM-dd")
        End If
    End Sub

    ''' <summary>
    ''' Loads students from the "students" table in Supabase and binds to the GridView.
    ''' </summary>
    Private Sub BindStudents()
        Dim dt As New DataTable()

        Using conn As New NpgsqlConnection(SupabaseConnectionString)
            conn.Open()
            Using cmd As New NpgsqlCommand("SELECT id, first_name, last_name, email, enrollment_date FROM students ORDER BY id", conn)
                Using reader As NpgsqlDataReader = cmd.ExecuteReader()
                    dt.Load(reader)
                End Using
            End Using
        End Using

        gvStudents.DataSource = dt
        gvStudents.DataBind()
    End Sub

    ''' <summary>
    ''' Insert new student record with optional account creation.
    ''' If chkCreateAccount is checked, we first create the Identity user,
    ''' then insert the student row with identity_user_id.
    ''' Otherwise, we just insert the student row with no identity_user_id.
    ''' </summary>
    Protected Sub btnAddStudent_Click(sender As Object, e As EventArgs)
        Try
            ' Prevent empty inputs
            If String.IsNullOrWhiteSpace(txtFirstName.Text) OrElse
               String.IsNullOrWhiteSpace(txtLastName.Text) OrElse
               String.IsNullOrWhiteSpace(txtEmail.Text) Then

                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage",
                    "showToast('Please fill in all required fields.', 'error');", True)
                Exit Sub
            End If

            ' Step 1: If the admin wants to create an account, do that first.
            Dim identityUserId As String = Nothing
            If chkCreateAccount.Checked Then
                Dim manager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
                Dim existingUser = manager.FindByEmail(txtEmail.Text.Trim())

                If existingUser Is Nothing Then
                    ' Create a default password (adjust as needed)
                    Dim defaultPassword As String = txtLastName.Text.Trim().Substring(0, 1).ToUpper() &
                                                    txtFirstName.Text.Trim().ToLower() &
                                                    "Password789!"

                    Dim newUser As New ApplicationUser() With {
                        .UserName = txtEmail.Text.Trim(),
                        .Email = txtEmail.Text.Trim()
                    }

                    Dim createResult = manager.Create(newUser, defaultPassword)
                    If createResult.Succeeded Then
                        ' Add user to Student role
                        manager.AddToRole(newUser.Id, "Student")

                        ' We'll store this user's Id to link in the students table
                        identityUserId = newUser.Id
                    Else
                        Dim errors As String = String.Join(", ", createResult.Errors)
                        System.Diagnostics.Debug.WriteLine("Account creation error: " & errors)
                        ' Optionally show a toast if account creation fails
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage",
                            "showToast('Account creation error: " & errors.Replace("'", "\'") & "', 'error');", True)
                    End If
                Else
                    ' If an account already exists for that email, we won't create a new one
                    System.Diagnostics.Debug.WriteLine("User with that email already exists in Identity.")
                    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage",
                        "showToast('An account with that email already exists.', 'error');", True)
                End If
            End If

            ' Step 2: Insert the student row
            Using conn As New NpgsqlConnection(SupabaseConnectionString)
                conn.Open()

                Dim sql As String
                If String.IsNullOrEmpty(identityUserId) Then
                    ' If no account was created, or checkbox wasn't checked
                    sql = "INSERT INTO students (first_name, last_name, email, enrollment_date)
                           VALUES (@first_name, @last_name, @email, @enrollment_date)"
                Else
                    ' If an Identity user was created, link it with identity_user_id
                    sql = "INSERT INTO students (identity_user_id, first_name, last_name, email, enrollment_date)
                           VALUES (@identity_user_id, @first_name, @last_name, @email, @enrollment_date)"
                End If

                Using cmd As New NpgsqlCommand(sql, conn)
                    If Not String.IsNullOrEmpty(identityUserId) Then
                        cmd.Parameters.AddWithValue("@identity_user_id", identityUserId)
                    End If

                    cmd.Parameters.AddWithValue("@first_name", txtFirstName.Text)
                    cmd.Parameters.AddWithValue("@last_name", txtLastName.Text)
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text)

                    Dim enrollDate As DateTime
                    If DateTime.TryParse(txtEnrollmentDate.Text, enrollDate) Then
                        cmd.Parameters.AddWithValue("@enrollment_date", enrollDate)
                    Else
                        cmd.Parameters.AddWithValue("@enrollment_date", DBNull.Value)
                    End If

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' Clear input fields
            txtFirstName.Text = ""
            txtLastName.Text = ""
            txtEmail.Text = ""
            txtEnrollmentDate.Text = ""

            ' Rebind grid
            BindStudents()

            ' Show success toast
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage",
                "showToast('Student added successfully!', 'success');", True)

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage",
                "showToast('Error adding student: " & ex.Message.Replace("'", "\'") & "', 'error');", True)
        End Try
    End Sub

    ''' <summary>
    ''' Start editing a row in the GridView.
    ''' </summary>
    Protected Sub gvStudents_RowEditing(sender As Object, e As GridViewEditEventArgs)
        gvStudents.EditIndex = e.NewEditIndex
        BindStudents()
    End Sub

    ''' <summary>
    ''' Cancel editing.
    ''' </summary>
    Protected Sub gvStudents_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs)
        gvStudents.EditIndex = -1
        BindStudents()
    End Sub

    ''' <summary>
    ''' Update a student record.
    ''' </summary>
    Protected Sub gvStudents_RowUpdating(sender As Object, e As GridViewUpdateEventArgs)
        Try
            Dim studentId As Integer = Convert.ToInt32(gvStudents.DataKeys(e.RowIndex).Value)
            Dim row As GridViewRow = gvStudents.Rows(e.RowIndex)
            Dim newFirstName As String = CType(row.Cells(1).Controls(0), TextBox).Text
            Dim newLastName As String = CType(row.Cells(2).Controls(0), TextBox).Text
            Dim newEmail As String = CType(row.Cells(3).Controls(0), TextBox).Text
            Dim newEnrollmentDateStr As String = CType(row.Cells(4).Controls(0), TextBox).Text

            Using conn As New NpgsqlConnection(SupabaseConnectionString)
                conn.Open()
                Dim sql As String = "UPDATE students
                                     SET first_name = @first_name,
                                         last_name = @last_name,
                                         email = @email,
                                         enrollment_date = @enrollment_date
                                     WHERE id = @id"

                Using cmd As New NpgsqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@id", studentId)
                    cmd.Parameters.AddWithValue("@first_name", newFirstName)
                    cmd.Parameters.AddWithValue("@last_name", newLastName)
                    cmd.Parameters.AddWithValue("@email", newEmail)

                    Dim enrollDate As DateTime
                    If DateTime.TryParse(newEnrollmentDateStr, enrollDate) Then
                        cmd.Parameters.AddWithValue("@enrollment_date", enrollDate)
                    Else
                        cmd.Parameters.AddWithValue("@enrollment_date", DBNull.Value)
                    End If

                    cmd.ExecuteNonQuery()
                End Using
            End Using

            gvStudents.EditIndex = -1
            BindStudents()

            ' Show success toast
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage",
                "showToast('Student updated successfully!', 'success');", True)

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage",
                "showToast('Error updating student: " & ex.Message.Replace("'", "\'") & "', 'error');", True)
        End Try
    End Sub

    ''' <summary>
    ''' Delete a student record with confirmation.
    ''' </summary>
    Protected Sub gvStudents_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Try
            Dim studentId As Integer = Convert.ToInt32(gvStudents.DataKeys(e.RowIndex).Value)

            Using conn As New NpgsqlConnection(SupabaseConnectionString)
                conn.Open()
                Dim sql As String = "DELETE FROM students WHERE id = @id"

                Using cmd As New NpgsqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@id", studentId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            BindStudents()

            ' Show success toast
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage",
                "showToast('Student deleted successfully!', 'success');", True)

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage",
                "showToast('Error deleting student: " & ex.Message.Replace("'", "\'") & "', 'error');", True)
        End Try
    End Sub
End Class
