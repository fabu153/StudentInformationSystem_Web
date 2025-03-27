Imports Npgsql
Imports System.Configuration
Imports System.Data

Public Class ManageCourses
    Inherits System.Web.UI.Page

    Private ReadOnly Property SupabaseConnectionString As String
        Get
            Return ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindCourses()
        End If
    End Sub

    ''' <summary>
    ''' Loads and optionally filters the courses from the "courses" table
    ''' </summary>
    Private Sub BindCourses(Optional searchTerm As String = "")
        Dim dt As New DataTable()
        Using conn As New NpgsqlConnection(SupabaseConnectionString)
            conn.Open()

            Dim sql As String = "SELECT c.course_id, c.course_name, c.instructor, c.ects, c.hours, c.""format"", c.capacity, " &
                                "COALESCE(e.enrolled, 0) AS enrolled_students " &
                                "FROM courses c " &
                                "LEFT JOIN (SELECT course_id, COUNT(*) AS enrolled FROM enrollments GROUP BY course_id) e " &
                                "ON c.course_id = e.course_id "
            If Not String.IsNullOrEmpty(searchTerm) Then
                sql &= "WHERE LOWER(c.course_name) ILIKE @search OR LOWER(c.instructor) ILIKE @search OR LOWER(c.""format"") ILIKE @search "
            End If
            sql &= "ORDER BY c.course_name"

            Using cmd As New NpgsqlCommand(sql, conn)
                If Not String.IsNullOrEmpty(searchTerm) Then
                    cmd.Parameters.AddWithValue("@search", "%" & searchTerm.ToLower() & "%")
                End If
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        gvCourses.DataSource = dt
        gvCourses.DataBind()
        lblNoResults.Visible = (dt.Rows.Count = 0)
    End Sub

    ''' <summary>
    ''' Inserts a new course record
    ''' </summary>
    Protected Sub btnAddCourse_Click(sender As Object, e As EventArgs)
        Try
            Dim courseName As String = txtCourseName.Text.Trim()
            Dim instructor As String = txtInstructor.Text.Trim()
            Dim ectsVal As Integer
            Dim hoursVal As Integer
            Dim formatVal As String = ddlFormat.SelectedValue
            Dim capacityVal As Integer

            ' Validate required fields and numeric inputs
            If String.IsNullOrEmpty(courseName) Then
                ShowToast("Course name is required.", "error")
                Return
            End If

            If Not Integer.TryParse(txtEcts.Text.Trim(), ectsVal) Then
                ShowToast("Please enter a valid number for ECTS.", "error")
                Return
            End If

            If Not Integer.TryParse(txtHours.Text.Trim(), hoursVal) Then
                ShowToast("Please enter a valid number for Hours.", "error")
                Return
            End If

            If Not Integer.TryParse(txtCapacity.Text.Trim(), capacityVal) Then
                ShowToast("Please enter a valid number for Capacity.", "error")
                Return
            End If

            ' Allow capacity of 0 (for unlimited) but no negative numbers.
            If capacityVal < 0 Then
                ShowToast("Capacity must be non-negative.", "error")
                Return
            End If

            If ectsVal < 0 OrElse hoursVal < 0 Then
                ShowToast("ECTS and Hours must be non-negative.", "error")
                Return
            End If

            Using conn As New NpgsqlConnection(SupabaseConnectionString)
                conn.Open()

                Dim insertSql As String = "INSERT INTO courses (course_name, instructor, ects, hours, ""format"", capacity) " &
                                          "VALUES (@course_name, @instructor, @ects, @hours, @format, @capacity)"
                Using cmd As New NpgsqlCommand(insertSql, conn)
                    cmd.Parameters.AddWithValue("@course_name", courseName)
                    cmd.Parameters.AddWithValue("@instructor", instructor)
                    cmd.Parameters.AddWithValue("@ects", ectsVal)
                    cmd.Parameters.AddWithValue("@hours", hoursVal)
                    cmd.Parameters.AddWithValue("@format", formatVal)
                    cmd.Parameters.AddWithValue("@capacity", capacityVal)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            ' Clear input fields
            txtCourseName.Text = ""
            txtInstructor.Text = ""
            txtEcts.Text = ""
            txtHours.Text = ""
            ' No longer used: txtFormat.Text = ""
            txtCapacity.Text = ""

            BindCourses()
            ShowToast("Course added successfully!", "success")

            ' Close the modal after successful insert
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closeModal", "var modalEl = document.getElementById('addCourseModal'); var modal = bootstrap.Modal.getInstance(modalEl); if (modal) { modal.hide(); } else { modal = new bootstrap.Modal(modalEl); modal.hide(); }", True)
        Catch ex As Exception
            ShowToast("Error adding course: " & ex.Message, "error")
        End Try
    End Sub

    ''' <summary>
    ''' Deletes a course record
    ''' </summary>
    Protected Sub gvCourses_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Try
            Dim courseId As Integer = Convert.ToInt32(gvCourses.DataKeys(e.RowIndex).Value)
            Using conn As New NpgsqlConnection(SupabaseConnectionString)
                conn.Open()
                Dim deleteSql As String = "DELETE FROM courses WHERE course_id = @course_id"
                Using cmd As New NpgsqlCommand(deleteSql, conn)
                    cmd.Parameters.AddWithValue("@course_id", courseId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            BindCourses()
            ShowToast("Course deleted successfully!", "success")
        Catch ex As Exception
            ShowToast("Error deleting course: " & ex.Message, "error")
        End Try
    End Sub

    ''' <summary>
    ''' Search text changed event: filter the courses
    ''' </summary>
    Protected Sub txtSearch_TextChanged(sender As Object, e As EventArgs)
        Dim searchTerm As String = txtSearch.Text.Trim()
        BindCourses(searchTerm)
    End Sub

    ''' <summary>
    ''' Helper method to show a toast message
    ''' </summary>
    Private Sub ShowToast(message As String, type As String)
        Dim safeMessage As String = message.Replace("'", "\'")
        Dim script As String = $"showToast('{safeMessage}', '{type}');"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage", script, True)
    End Sub
End Class
