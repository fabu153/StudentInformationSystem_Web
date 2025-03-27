<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="StudentDashboard.aspx.vb" Inherits="StudentInformationSystem_Web.StudentDashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="container my-4">
      <!-- Header -->
      <div class="row">
          <div class="col-12">
              <h1 class="mb-4">Student Dashboard</h1>
          </div>
      </div>
      
      <!-- Profile Card -->
      <div class="row">
          <div class="col-md-4">
              <div class="card mb-4 shadow-sm">
                  <div class="card-header bg-primary text-white">
                      <h4 class="my-0">Profile Info</h4>
                  </div>
                  <div class="card-body">
                      <p><strong>Name:</strong> <asp:Label ID="lblName" runat="server" /></p>
                      <p><strong>Email:</strong> <asp:Label ID="lblEmail" runat="server" /></p>
                      <p><strong>Enrollment Date:</strong> <asp:Label ID="lblEnrollmentDate" runat="server" /></p>
                  </div>
              </div>
          </div>
          
          <!-- Enrolled Courses Card -->
          <div class="col-md-8">
              <div class="card mb-4 shadow-sm">
                  <div class="card-header bg-success text-white">
                      <h4 class="my-0">Enrolled Courses</h4>
                  </div>
                  <div class="card-body">
                      <asp:GridView ID="gvEnrolledCourses" runat="server" CssClass="table table-striped"
                                    AutoGenerateColumns="False">
                          <Columns>
                              <asp:BoundField DataField="course_name" HeaderText="Course Name" />
                              <asp:BoundField DataField="instructor" HeaderText="Instructor" />
                              <asp:BoundField DataField="enrollment_date" HeaderText="Enrolled On" DataFormatString="{0:yyyy-MM-dd}" />
                          </Columns>
                      </asp:GridView>
                  </div>
              </div>
          </div>
      </div>
      
      <!-- Enrollment Trends Chart -->
      <div class="row">
          <div class="col-12">
              <div class="card mb-4 shadow-sm">
                  <div class="card-header bg-info text-white">
                      <h4 class="my-0">Enrollment Trends</h4>
                  </div>
                  <div class="card-body">
                      <canvas id="dashboardChart" width="400" height="200"></canvas>
                  </div>
              </div>
          </div>
      </div>
  </div>

  <!-- Chart.js Script -->
  <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
  <script type="text/javascript">
      // Example: Render a bar chart with the provided labels and data.
      function renderDashboardChart(labels, data) {
          var ctx = document.getElementById('dashboardChart').getContext('2d');
          new Chart(ctx, {
              type: 'bar',
              data: {
                  labels: labels,
                  datasets: [{
                      label: 'Enrollments',
                      data: data,
                      backgroundColor: 'rgba(54, 162, 235, 0.6)',
                      borderColor: 'rgba(54, 162, 235, 1)',
                      borderWidth: 1
                  }]
              },
              options: {
                  scales: {
                      y: {
                          beginAtZero: true,
                          ticks: {
                              stepSize: 1
                          }
                      }
                  }
              }
          });
      }
  </script>
</asp:Content>

