Imports Npgsql
Imports System.Configuration
Imports System.Data

Public Class ManageEnrollments
    Inherits System.Web.UI.Page

    Private ReadOnly Property SupabaseConnectionString As String
        Get
            Return ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindDropdowns()
            BindEnrollments()
            BindChart()
        End If
        btnEnroll.Enabled = ddlCourses.Items.Count > 0
    End Sub

    ' Bind both student and course dropdowns
    Private Sub BindDropdowns()
        BindStudentsDropdown()
        BindCoursesDropdown()
    End Sub

    ' Fetch students from the "students" table
    Private Sub BindStudentsDropdown()
        Dim dt As New DataTable()
        Using conn As New NpgsqlConnection(SupabaseConnectionString)
            conn.Open()
            Dim sql As String = "SELECT id, first_name || ' ' || last_name AS full_name FROM students ORDER BY first_name"
            Using cmd As New NpgsqlCommand(sql, conn)
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using
        ddlStudents.DataSource = dt
        ddlStudents.DataTextField = "full_name"
        ddlStudents.DataValueField = "id"
        ddlStudents.DataBind()
    End Sub
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

        btnEnroll.Enabled = ddlCourses.Items.Count > 0
    End Sub

    ' Bind enrollments with an optional search term to filter results
    Private Sub BindEnrollments(Optional searchTerm As String = "")
        Dim dt As New DataTable()
        Using conn As New NpgsqlConnection(SupabaseConnectionString)
            conn.Open()
            Dim sql As String = "SELECT e.enrollment_id, s.first_name, s.last_name, c.course_name, e.enrollment_date " &
                                "FROM enrollments e " &
                                "JOIN students s ON e.student_id = s.id " &
                                "JOIN courses c ON e.course_id = c.course_id "
            If Not String.IsNullOrEmpty(searchTerm) Then
                sql &= "WHERE s.first_name ILIKE @search OR s.last_name ILIKE @search OR c.course_name ILIKE @search
"
            End If
            sql &= "ORDER BY e.enrollment_date DESC"

            Using cmd As New NpgsqlCommand(sql, conn)
                If Not String.IsNullOrEmpty(searchTerm) Then
                    cmd.Parameters.AddWithValue("@search", "%" & searchTerm.ToLower() & "%")
                End If
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using
        gvEnrollments.DataSource = dt
        gvEnrollments.DataBind()
    End Sub

    ' Bind chart data: count enrollments per course
    Private Sub BindChart()
        Dim dt As New DataTable()
        Using conn As New NpgsqlConnection(SupabaseConnectionString)
            conn.Open()
            Dim sql As String = "SELECT c.course_name, COUNT(e.enrollment_id) AS enrollment_count " &
                                "FROM courses c " &
                                "LEFT JOIN enrollments e ON c.course_id = e.course_id " &
                                "GROUP BY c.course_name ORDER BY c.course_name"
            Using cmd As New NpgsqlCommand(sql, conn)
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        Dim labels As New List(Of String)
        Dim data As New List(Of Integer)
        For Each row As DataRow In dt.Rows
            labels.Add(row("course_name").ToString())
            data.Add(Convert.ToInt32(row("enrollment_count").ToString()))
        Next

        ' Pass the chart data to the client
        Dim labelsJson As String = Newtonsoft.Json.JsonConvert.SerializeObject(labels)
        Dim dataJson As String = Newtonsoft.Json.JsonConvert.SerializeObject(data)
        Dim script As String = $"renderChart({labelsJson}, {dataJson});"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "chartScript", script, True)
    End Sub

    ' Helper method for showing toast notifications
    Private Sub ShowToast(message As String, type As String)
        Dim safeMessage As String = message.Replace("'", "\'")
        Dim script As String = $"showToast('{safeMessage}', '{type}');"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastMessage", script, True)
    End Sub

    ' Enroll button click event handler
    Protected Sub btnEnroll_Click(sender As Object, e As EventArgs)
        Try
            Dim studentId As Integer = Convert.ToInt32(ddlStudents.SelectedValue)
            Dim courseId As Integer = Convert.ToInt32(ddlCourses.SelectedValue)
            Dim enrollmentDate As DateTime = DateTime.Now

            Using conn As New NpgsqlConnection(SupabaseConnectionString)
                conn.Open()
                Dim checkSql As String = "SELECT COUNT(*) FROM enrollments WHERE student_id = @student_id AND course_id = @course_id"
                Using checkCmd As New NpgsqlCommand(checkSql, conn)
                    checkCmd.Parameters.AddWithValue("@student_id", studentId)
                    checkCmd.Parameters.AddWithValue("@course_id", courseId)
                    Dim count As Integer = Convert.ToInt32(checkCmd.ExecuteScalar())
                    If count > 0 Then
                        ShowToast("Student is already enrolled in this course.", "error")
                        Return
                    End If
                End Using

                ' Check course capacity
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
                    insertCmd.Parameters.AddWithValue("@student_id", studentId)
                    insertCmd.Parameters.AddWithValue("@course_id", courseId)
                    insertCmd.Parameters.AddWithValue("@enrollment_date", enrollmentDate)
                    insertCmd.ExecuteNonQuery()
                End Using
            End Using

            BindEnrollments()
            BindCoursesDropdown()
            BindChart()
            ShowToast("Student enrolled successfully!", "success")
        Catch ex As Exception
            ' In production, log ex details and show a generic error message
            ShowToast("Error enrolling student: " & ex.Message, "error")
        End Try
    End Sub

    ' Delete enrollment event handler
    Protected Sub gvEnrollments_RowDeleting(sender As Object, e As GridViewDeleteEventArgs)
        Try
            Dim enrollmentId As Integer = Convert.ToInt32(gvEnrollments.DataKeys(e.RowIndex).Value)
            Using conn As New NpgsqlConnection(SupabaseConnectionString)
                conn.Open()
                Dim sql As String = "DELETE FROM enrollments WHERE enrollment_id = @enrollment_id"
                Using cmd As New NpgsqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@enrollment_id", enrollmentId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            BindEnrollments()
            BindChart()
            ShowToast("Enrollment deleted successfully!", "success")
        Catch ex As Exception
            ShowToast("Error deleting enrollment: " & ex.Message, "error")
        End Try
    End Sub


    ' Search text changed event: filter the enrollments
    Protected Sub txtSearch_TextChanged(sender As Object, e As EventArgs)
        Dim searchTerm As String = txtSearch.Text.Trim()
        BindEnrollments(searchTerm)
        BindChart()
    End Sub
End Class
