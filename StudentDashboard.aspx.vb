Imports Npgsql
Imports System.Configuration
Imports Microsoft.AspNet.Identity

Partial Class StudentDashboard
    Inherits System.Web.UI.Page

    Private ReadOnly Property SupabaseConnectionString As String
        Get
            Return ConfigurationManager.ConnectionStrings("SupabaseConnection").ConnectionString
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Request.IsAuthenticated Then
            Response.Redirect("~/Account/Login.aspx")
        End If

        If Not IsPostBack Then
            Dim currentUserId As String = User.Identity.GetUserId()
            If String.IsNullOrEmpty(currentUserId) Then
                Response.Redirect("~/Account/Login.aspx")
            End If

            LoadStudentInfo(currentUserId)
            BindEnrolledCourses(currentUserId)
            BindChartData(currentUserId)
        End If
    End Sub

    ' Loads basic student info from the "students" table using the GUID stored in identity_user_id.
    Private Sub LoadStudentInfo(identityUserId As String)
        Using conn As New NpgsqlConnection(SupabaseConnectionString)
            conn.Open()
            Dim sql As String = "SELECT first_name, last_name, email, enrollment_date FROM students WHERE identity_user_id = @userId"
            Using cmd As New NpgsqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@userId", identityUserId)
                Using reader = cmd.ExecuteReader()
                    If reader.Read() Then
                        lblName.Text = reader("first_name").ToString() & " " & reader("last_name").ToString()
                        lblEmail.Text = reader("email").ToString()
                        lblEnrollmentDate.Text = Convert.ToDateTime(reader("enrollment_date")).ToShortDateString()
                    Else
                        lblName.Text = "Not Found"
                    End If
                End Using
            End Using
        End Using
    End Sub

    ' Binds the grid view with the current student's enrollments.
    Private Sub BindEnrolledCourses(identityUserId As String)
        Using conn As New NpgsqlConnection(SupabaseConnectionString)
            conn.Open()
            Dim sql As String = "SELECT c.course_name, c.instructor, e.enrollment_date " &
                                "FROM enrollments e " &
                                "JOIN courses c ON e.course_id = c.course_id " &
                                "JOIN students s ON s.id = e.student_id " &
                                "WHERE s.identity_user_id = @userId " &
                                "ORDER BY e.enrollment_date DESC"
            Using cmd As New NpgsqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@userId", identityUserId)
                Dim dt As New DataTable()
                dt.Load(cmd.ExecuteReader())
                gvEnrolledCourses.DataSource = dt
                gvEnrolledCourses.DataBind()
            End Using
        End Using
    End Sub

    ' Binds a chart showing the number of enrollments per course for the student.
    Private Sub BindChartData(identityUserId As String)
        Dim labels As New List(Of String)()
        Dim data As New List(Of Integer)()

        Using conn As New NpgsqlConnection(SupabaseConnectionString)
            conn.Open()
            Dim sql As String = "SELECT c.course_name, COUNT(e.enrollment_id) AS enrollment_count " &
                                "FROM enrollments e " &
                                "JOIN courses c ON e.course_id = c.course_id " &
                                "JOIN students s ON s.id = e.student_id " &
                                "WHERE s.identity_user_id = @userId " &
                                "GROUP BY c.course_name ORDER BY c.course_name"
            Using cmd As New NpgsqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@userId", identityUserId)
                Dim dt As New DataTable()
                dt.Load(cmd.ExecuteReader())
                For Each row As DataRow In dt.Rows
                    labels.Add(row("course_name").ToString())
                    data.Add(Convert.ToInt32(row("enrollment_count")))
                Next
            End Using
        End Using

        Dim labelsJson As String = Newtonsoft.Json.JsonConvert.SerializeObject(labels)
        Dim dataJson As String = Newtonsoft.Json.JsonConvert.SerializeObject(data)
        Dim script As String = $"renderDashboardChart({labelsJson}, {dataJson});"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "renderChart", script, True)
    End Sub
End Class
