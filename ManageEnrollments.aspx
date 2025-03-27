<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ManageEnrollments.aspx.vb" Inherits="StudentInformationSystem_Web.ManageEnrollments" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Enrollment Form -->
    <div class="row mb-3">
        <div class="col-md-4">
            <label for="ddlStudents" class="form-label">Select Student</label>
            <asp:DropDownList ID="ddlStudents" runat="server" CssClass="form-control"></asp:DropDownList>
        </div>
        <div class="col-md-4">
            <label for="ddlCourses" class="form-label">Select Course</label>
            <asp:DropDownList ID="ddlCourses" runat="server" CssClass="form-control"></asp:DropDownList>
        </div>
        <div class="col-md-4 d-flex align-items-end">
            <asp:Button ID="btnEnroll" runat="server" Text="Enroll Student" CssClass="btn btn-primary" OnClick="btnEnroll_Click" />
        </div>
    </div>

    <!-- SEARCH BAR -->
    <div class="row mb-3">
        <div class="col-md-4">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" 
                Placeholder="Search Enrollments..." 
                AutoPostBack="False"
                OnTextChanged="txtSearch_TextChanged"
                onkeyup="debounceSearch()" />
        </div>
    </div>

    <!-- UPDATEPANEL: Wraps the Grid and Chart only -->
    <asp:UpdatePanel ID="updPanel" runat="server">
        <ContentTemplate>
            <!-- Enrollment Grid -->
            <asp:GridView ID="gvEnrollments" runat="server" CssClass="table table-striped" AutoGenerateColumns="False"
                DataKeyNames="enrollment_id" OnRowDeleting="gvEnrollments_RowDeleting">
                <Columns>
                    <asp:BoundField DataField="enrollment_id" HeaderText="Enrollment ID" ReadOnly="True" Visible="False" />
                    <asp:BoundField DataField="first_name" HeaderText="First Name" />
                    <asp:BoundField DataField="last_name" HeaderText="Last Name" />
                    <asp:BoundField DataField="course_name" HeaderText="Course" />
                    <asp:BoundField DataField="enrollment_date" HeaderText="Enrollment Date" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:CommandField ShowDeleteButton="True" ButtonType="Link" DeleteText="Delete" />
                </Columns>
            </asp:GridView>

            <!-- Chart for Enrollments Per Course -->
            <div class="mt-4">
                <h3>Enrollments per Course</h3>
                <canvas id="enrollmentChart" width="400" height="200"></canvas>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="txtSearch" EventName="TextChanged" />
        </Triggers>
    </asp:UpdatePanel>

    <!-- JavaScript: Chart rendering & Debounced Search -->
    <script type="text/javascript">
        var myChart = null;
        // Render chart with given labels and data arrays
        function renderChart(labels, data) {
            var ctx = document.getElementById('enrollmentChart').getContext('2d');
            if (myChart) {
                myChart.destroy();
            }
            myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: '# of Enrollments',
                        data: data,
                        borderWidth: 1
                    }]
                },
                options: {
                    scales: {
                        y: {
                            beginAtZero: true,
                            precision: 0
                        }
                    }
                }
            });
        }

        var debounceTimer;
        function debounceSearch() {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(function () {
                __doPostBack('<%= txtSearch.UniqueID %>', '');
            }, 300);
        }
    </script>
    <!-- Include Chart.js from CDN -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>
