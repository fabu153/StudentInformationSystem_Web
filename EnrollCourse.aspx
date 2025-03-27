<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="EnrollCourse.aspx.vb" Inherits="StudentInformationSystem_Web.EnrollCourse" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Enroll in a Course</h2>
    <hr />

    <!-- Courses Dropdown & Enroll Button -->
    <div class="row mb-3">
        <div class="col-md-4">
            <label for="ddlCourses" class="form-label">Select Course</label>
            <asp:DropDownList ID="ddlCourses" runat="server" CssClass="form-control"></asp:DropDownList>
        </div>
        <div class="col-md-4 d-flex align-items-end">
            <asp:Button ID="btnEnroll" runat="server" Text="Enroll" CssClass="btn btn-primary" OnClick="btnEnroll_Click" />
        </div>
    </div>

    <!-- Grid of This Student's Enrollments -->
    <asp:GridView ID="gvStudentEnrollments" runat="server" CssClass="table table-striped"
                  AutoGenerateColumns="False" DataKeyNames="enrollment_id"
                  OnRowDeleting="gvStudentEnrollments_RowDeleting">
        <Columns>
            <asp:BoundField DataField="enrollment_id" HeaderText="Enrollment ID" Visible="False" ReadOnly="True" />
            <asp:BoundField DataField="course_name" HeaderText="Course" />
            <asp:BoundField DataField="enrollment_date" HeaderText="Enrollment Date" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:CommandField ShowDeleteButton="True" ButtonType="Link" DeleteText="Unenroll" />
        </Columns>
    </asp:GridView>

    <!-- Optional: Label if no enrollments found -->
    <asp:Label ID="lblNoResults" runat="server" CssClass="text-muted" Text="No enrollments found." Visible="False"></asp:Label>
</asp:Content>
