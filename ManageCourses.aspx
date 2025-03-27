<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ManageCourses.aspx.vb" Inherits="StudentInformationSystem_Web.ManageCourses" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div class="d-flex justify-content-between align-items-center mb-3">
    <h2 class="mb-0">Course Management</h2>
</div>
<hr />
<!-- SEARCH & ADD COURSE BUTTON -->
<div class="row mb-3">
    <div class="col-md-8 d-flex align-items-center">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control w-100"
                     Placeholder="Search courses..."
                     AutoPostBack="True"
                     OnTextChanged="txtSearch_TextChanged"
                     onkeyup="debounceSearch()" />
    </div>
    <div class="col-md-4 text-end">
        <button type="button" class="button-28-small" data-bs-toggle="modal" data-bs-target="#addCourseModal">
            Add Course
        </button>
    </div>
</div>

    <!-- UPDATEPANEL: Only the grid is refreshed -->
    <asp:UpdatePanel ID="upCoursesGrid" runat="server">
        <ContentTemplate>
            <!-- COURSES GRID -->
            <asp:GridView ID="gvCourses" runat="server" CssClass="table table-striped"
                          AutoGenerateColumns="False" DataKeyNames="course_id"
                          OnRowDeleting="gvCourses_RowDeleting">
                <Columns>
                    <asp:BoundField DataField="course_id" HeaderText="ID" Visible="False" ReadOnly="True" />
                    <asp:BoundField DataField="course_name" HeaderText="Course Name" />
                    <asp:BoundField DataField="instructor" HeaderText="Instructor" />
                    <asp:BoundField DataField="ects" HeaderText="ECTS" />
                    <asp:BoundField DataField="hours" HeaderText="Hours" />
                    <asp:BoundField DataField="format" HeaderText="Format" />
                    <asp:BoundField DataField="capacity" HeaderText="Capacity" />
                    <asp:BoundField DataField="enrolled_students" HeaderText="Enrolled" />
                    <asp:CommandField ShowDeleteButton="True" ButtonType="Link" DeleteText="Delete" />
                </Columns>
            </asp:GridView>

            <!-- No Results Label -->
            <asp:Label ID="lblNoResults" runat="server" Text="No courses found." CssClass="text-muted" Visible="False" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="txtSearch" EventName="TextChanged" />
        </Triggers>
    </asp:UpdatePanel>

    <!-- ADD COURSE MODAL -->
    <div class="modal fade" id="addCourseModal" tabindex="-1" aria-labelledby="addCourseModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="addCourseModalLabel">Add New Course</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">

                    <!-- Section Header -->
                    <h6 class="text-secondary mb-3">Basic Info</h6>
                    <div class="row g-2 mb-4">
                        <div class="col-md-6">
                            <div class="form-floating form-floating-sm">
                                <asp:TextBox ID="txtCourseName" runat="server" CssClass="form-control" placeholder=" " />
                                <label for="<%= txtCourseName.ClientID %>">Course Name</label>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-floating form-floating-sm">
                                <asp:TextBox ID="txtInstructor" runat="server" CssClass="form-control" placeholder=" " />
                                <label for="<%= txtInstructor.ClientID %>">Instructor</label>
                            </div>
                        </div>
                    </div>

                    <!-- Section Header -->
                    <h6 class="text-secondary mb-3">Course Details</h6>
                    <div class="row g-2 mb-4">
                        <div class="col-md-4">
                            <div class="form-floating form-floating-sm">
                                <asp:TextBox ID="txtEcts" runat="server" CssClass="form-control" TextMode="Number" placeholder=" " />
                                <label for="<%= txtEcts.ClientID %>">ECTS</label>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-floating form-floating-sm">
                                <asp:TextBox ID="txtHours" runat="server" CssClass="form-control" TextMode="Number" placeholder=" " />
                                <label for="<%= txtHours.ClientID %>">Hours</label>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-floating form-floating-sm">
                                <asp:DropDownList ID="ddlFormat" runat="server" CssClass="form-select" placeholder=" ">
                                    <asp:ListItem Text="Online" Value="online" />
                                    <asp:ListItem Text="Campus" Value="campus" />
                                    <asp:ListItem Text="Blended" Value="blended" />
                                </asp:DropDownList>
                                <label for="<%= ddlFormat.ClientID %>">Format</label>
                            </div>
                        </div>
                    </div>

                    <!-- Section Header -->
                    <h6 class="text-secondary mb-3">Capacity</h6>
                    <div class="row g-2 mb-4">
                        <div class="col-md-6">
                            <div class="form-floating form-floating-sm">
                                <asp:TextBox ID="txtCapacity" runat="server" CssClass="form-control" TextMode="Number" placeholder=" " />
                                <label for="<%= txtCapacity.ClientID %>">Capacity (Unlimited = 0)</label>
                            </div>
                        </div>
                    </div>

                </div> <!-- /modal-body -->

                <div class="modal-footer">
                    <!-- Use your custom button style for the Add Course button -->
                    <asp:Button ID="btnAddCourse" runat="server" Text="Add Course" CssClass="button-28"
                        OnClick="btnAddCourse_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Debounce JavaScript for Live Search -->
    <script type="text/javascript">
        var debounceTimer;
        function debounceSearch() {
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(function () {
                __doPostBack('<%= txtSearch.UniqueID %>', '');
            }, 300);
        }
    </script>
</asp:Content>
