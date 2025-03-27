Imports System
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports Npgsql
Imports System.Configuration
Imports System.Data
Imports System.Net.Mail

Public Class EnrollCourse
    Inherits System.Web.UI.Page

    Private ReadOnly Property SupabaseConnectionString As String
        Get
            Return ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString
        End Get
    End Property

    ' Store the student's numeric ID after lookup.
    Private _studentId As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Check if the user is authenticated; if not, redirect to login.
        If Not Request.IsAuthenticated Then
            Response.Redirect("~/Account/Login.aspx")
            Return
        End If

        If Not IsPostBack Then
            ' Use the GUID from Identity to look up the student's numeric ID.
            Dim currentUserId As String = User.Identity.GetUserId()
            _studentId = GetStudentIdFromUser(currentUserId)
            If _studentId = 0 Then
                Response.Write("Error: Student record not found for this user.")
                Return
            End If

            BindCoursesDropdown()
            BindStudentEnrollments()
        Else
            If ViewState("StudentId") IsNot Nothing Then
                _studentId = Convert.ToInt32(ViewState("StudentId"))
            End If
        End If
    End Sub

    ''' <summary>
    ''' Retrieves the student's numeric ID by matching the identity_user_id column with the given GUID.
    ''' </summary>
    Private Function GetStudentIdFromUser(identityUserId As String) As Integer
        Using conn As New NpgsqlConnection(SupabaseConnectionString)
            conn.Open()
            Dim sql As String = "SELECT id FROM students WHERE identity_user_id = @userId LIMIT 1"
            Using cmd As New NpgsqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@userId", identityUserId)
                Dim resultObj = cmd.ExecuteScalar()
                If resultObj IsNot Nothing AndAlso Not IsDBNull(resultObj) Then
                    Return Convert.ToInt32(resultObj)
                End If
            End Using
        End Using
        Return 0
    End Function

    ''' <summary>
    ''' Populates the Courses dropdown with available courses and the number of seats left.
    ''' </summary>
    Private Sub BindCoursesDropdown()
        Dim dt As New DataTable()
        Using conn As New NpgsqlConnection(SupabaseConnectionString)
            conn.Open()
            Dim sql As String = "SELECT c.course_id, c.course_name, c.capacity, " &
                                "(c.capacity - COALESCE(e.enrolled, 0)) AS seats_left " &
                                "FROM courses c " &
                                "LEFT JOIN (SELECT course_id, COUNT(*) AS enrolled FROM enrollments GROUP BY course_id) e " &
                                "ON c.course_id = e.course_id " &
                                "ORDER BY c.course_name"
            Using cmd As New NpgsqlCommand(sql, conn)
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        For Each row As DataRow In dt.Rows
            row("course_name") = row("course_name").ToString() & " (Seats Left: " & row("seats_left").ToString() & ")"
        Next

        ddlCourses.DataSource = dt
        ddlCourses.DataTextField = "course_name"
        ddlCourses.DataValueField = "course_id"
        ddlCourses.DataBind()
    End Sub

    ''' <summary>
    ''' Binds the GridView with the current student's enrollments.
    ''' </summary>
    Private Sub BindStudentEnrollments()
        Dim dt As New DataTable()
        Using conn As New NpgsqlConnection(SupabaseConnectionString)
            conn.Open()
            Dim sql As String = "SELECT e.enrollment_id, c.course_name, e.enrollment_date " &
                                "FROM enrollments e " &
                                "JOIN courses c ON e.course_id = c.course_id " &
                                "WHERE e.student_id = @student_id " &
                                "ORDER BY e.enrollment_date DESC"
            Using cmd As New NpgsqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@student_id", _studentId)
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        gvStudentEnrollments.DataSource = dt
        gvStudentEnrollments.DataBind()
        lblNoResults.Visible = (dt.Rows.Count = 0)
    End Sub

    ''' <summary>
    ''' Handles enrollment when the student clicks the Enroll button.
    ''' </summary>
    Protected Sub btnEnroll_Click(sender As Object, e As EventArgs)
        Try
            If _studentId = 0 Then
                ShowToast("Student record not found.", "error")
                Return
            End If

            Dim courseId As Integer = Convert.ToInt32(ddlCourses.SelectedValue)
            Dim enrollmentDate As DateTime = DateTime.Now

            Using conn As New NpgsqlConnection(SupabaseConnectionString)
                conn.Open()

                ' Check if already enrolled
                Dim checkSql As String = "SELECT COUNT(*) FROM enrollments WHERE student_id = @student_id AND course_id = @course_id"
                Using checkCmd As New NpgsqlCommand(checkSql, conn)
                    checkCmd.Parameters.AddWithValue("@student_id", _studentId)
                    checkCmd.Parameters.AddWithValue("@course_id", courseId)
                    Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
                    If count > 0 Then
                        ShowToast("You are already enrolled in this course.", "error")
                        Return
                    End If
                End Using

                ' Check course capacity if limited (capacity > 0)
                Dim capacitySql As String = "SELECT capacity FROM courses WHERE course_id = @course_id"
                Dim courseCapacity As Integer = 0
                Using capCmd As New NpgsqlCommand(capacitySql, conn)
                    capCmd.Parameters.AddWithValue("@course_id", courseId)
                    Dim capacityObj = capCmd.ExecuteScalar()
                    If capacityObj IsNot Nothing AndAlso Not IsDBNull(capacityObj) Then
                        courseCapacity = Convert.ToInt32(capacityObj)
                    End If
                End Using

                If courseCapacity > 0 Then
                    Dim enrollmentCountSql As String = "SELECT COUNT(*) FROM enrollments WHERE course_id = @course_id"
                    Dim enrolledCount As Integer = 0
                    Using countCmd As New NpgsqlCommand(enrollmentCountSql, conn)
                        countCmd.Parameters.AddWithValue("@course_id", courseId)
                        enrolledCount = Convert.ToInt32(countCmd.ExecuteScalar())
                    End Using
                    If enrolledCount >= courseCapacity Then
                        ShowToast("Course capacity reached. Cannot enroll more students.", "error")
                        Return
                    End If
                End If

                ' Insert new enrollment
                Dim insertSql As String = "INSERT INTO enrollments (student_id, course_id, enrollment_date) VALUES (@student_id, @course_id, @enrollment_date)"
                Using insertCmd As New NpgsqlCommand(insertSql, conn)
                    insertCmd.Parameters.AddWithValue("@student_id", _studentId)
                    insertCmd.Parameters.AddWithValue("@course_id", courseId)
                    insertCmd.Parameters.AddWithValue("@enrollment_date", enrollmentDate)
                    insertCmd.ExecuteNonQuery()
                End Using
            End Using

            ' Refresh UI
            BindStudentEnrollments()
            BindCoursesDropdown() ' Refresh available seats

            ' Retrieve the current user from ASP.NET Identity (synchronously)
            Dim userManager = Context.GetOwinContext().GetUserManager(Of ApplicationUserManager)()
            Dim currentUser = userManager.FindById(User.Identity.GetUserId())
            If currentUser IsNot Nothing AndAlso currentUser.EmailNotificationsEnabled Then
                ' Retrieve the selected course name (adjust if needed)
                Dim courseName As String = ddlCourses.SelectedItem.Text
                Dim emailSubject As String = "Enrollment Confirmation"
                Dim emailBody As String = "Hello " & currentUser.UserName & ",<br/><br/>" &
                                          "You have been successfully enrolled in the course: <strong>" & courseName & "</strong>.<br/><br/>" &
                                          "Thank you!"
                ' Send the enrollment notification email.
                EmailHelper.SendEnrollmentNotification(currentUser.Email, emailSubject, emailBody)
            End If

            ShowToast("Enrolled successfully!", "success")
        Catch ex As Exception
            ShowToast("Error enrolling: " & ex.Message, "error")
        End Try
    End Sub

    ''' <summary>
    ''' Handles the GridView RowDeleting event to allow unenrollment.
    ''' </summary>
    Protected Sub gvStudentEnrollments_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Try
            Dim enrollmentId As Integer = Convert.ToInt32(gvStudentEnrollments.DataKeys(e.RowIndex).Value)
            Using conn As New NpgsqlConnection(SupabaseConnectionString)
                conn.Open()
                Dim sql As String = "DELETE FROM enrollments WHERE enrollment_id = @enrollment_id AND student_id = @student_id"
                Using cmd As New NpgsqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@enrollment_id", enrollmentId)
                    cmd.Parameters.AddWithValue("@student_id", _studentId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            BindStudentEnrollments()
            BindCoursesDropdown()
            ShowToast("You have unenrolled from the course.", "success")
        Catch ex As Exception
            ShowToast("Error unenrolling: " & ex.Message, "error")
        End Try
    End Sub

    ''' <summary>
    ''' Helper method to show toast notifications using the site.master toast script.
    ''' </summary>
    Private Sub ShowToast(message As String, type As String)
        Dim safeMessage As String = message.Replace("'", "\'")
        Dim script As String = $"showToast('{safeMessage}', '{type}');"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage", script, True)
    End Sub

    ''' <summary>
    ''' Preserve _studentId across postbacks using ViewState.
    ''' </summary>
    Protected Overrides Sub OnPreRender(e As EventArgs)
        MyBase.OnPreRender(e)
        If _studentId > 0 Then
            ViewState("StudentId") = _studentId
        End If
    End Sub
End Class

Public Class EmailHelper
    Public Shared Sub SendEnrollmentNotification(toEmail As String, subject As String, body As String)
        Try
            Dim mail As New MailMessage()
            ' Make sure the From address matches the one in web.config.
            mail.From = New MailAddress("your-email@gmail.com")
            mail.To.Add(toEmail)
            mail.Subject = subject
            mail.Body = body
            mail.IsBodyHtml = True

            ' Create the SMTP client using web.config settings.
            Using smtp As New SmtpClient()
                smtp.Send(mail)
            End Using

        Catch ex As Exception
            Throw New ApplicationException("Error sending enrollment email: " & ex.Message, ex)
        End Try
    End Sub
End Class
