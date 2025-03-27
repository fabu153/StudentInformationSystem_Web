<%@ Page Language="VB" AutoEventWireup="false" CodeBehind="ManageStudents.aspx.vb" 
    Inherits="StudentInformationSystem_Web.ManageStudents" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .form-check-input {
            vertical-align: middle;
            margin-top: -7px;
        }

        .form-check-label {
            vertical-align: middle;
        }
    </style>
    <!-- Row for Heading and Checkbox -->
    <div class="row mb-2 align-items-center">
        <!-- Heading on the left -->
        <div class="col-auto">
            <h2 class="mb-0">Manage Students</h2>
        </div>
        <!-- Checkbox on the right -->
        <div class="col text-end">
            <div class="form-check form-check-inline d-inline-flex align-items-center">
                <asp:CheckBox ID="chkCreateAccount" runat="server" CssClass="form-check-input me-2" />
                <label for="chkCreateAccount" class="form-check-label mb-0">Create Website login</label>
            </div>
        </div>
    </div>
    <hr />
    <!-- ADD NEW STUDENT FORM using Floating Labels -->
    <div class="row g-3 mb-3">
        <div class="col-md-2">
            <div class="form-floating form-floating-sm">
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" ClientIDMode="Static" placeholder=" "></asp:TextBox>
                <label for="<%= txtFirstName.ClientID %>">First Name</label>
            </div>
        </div>
        <div class="col-md-2">
            <div class="form-floating form-floating-sm">
                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" ClientIDMode="Static" placeholder=" "></asp:TextBox>
                <label for="<%= txtLastName.ClientID %>">Last Name</label>
            </div>
        </div>
        <div class="col-md-3">
            <div class="form-floating form-floating-sm">
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" ClientIDMode="Static" placeholder=" "></asp:TextBox>
                <label for="<%= txtEmail.ClientID %>">Email</label>
            </div>
        </div>
        <div class="col-md-3">
            <div class="form-floating form-floating-sm">
                <asp:TextBox ID="txtEnrollmentDate" runat="server" CssClass="form-control" 
                             TextMode="Date" ClientIDMode="Static" placeholder=" "></asp:TextBox>
                <label for="<%= txtEnrollmentDate.ClientID %>">Enrollment Date</label>
            </div>
        </div>
        <div class="col-md-2 d-flex align-items-center">
            <asp:Button ID="btnAddStudent" runat="server" Text="Add Student"
                        CssClass="button-28-small" 
                        OnClick="btnAddStudent_Click"
                        OnClientClick="return confirmAddStudent();" />
        </div>
    </div>

    <!-- STUDENTS GRID -->
    <asp:GridView ID="gvStudents" runat="server" CssClass="table table-striped"
                  AutoGenerateColumns="False" DataKeyNames="id"
                  OnRowEditing="gvStudents_RowEditing"
                  OnRowCancelingEdit="gvStudents_RowCancelingEdit"
                  OnRowUpdating="gvStudents_RowUpdating"
                  OnRowDeleting="gvStudents_RowDeleting">
        <Columns>
            <asp:BoundField DataField="id" HeaderText="ID" ReadOnly="True" />
            <asp:BoundField DataField="first_name" HeaderText="First Name" />
            <asp:BoundField DataField="last_name" HeaderText="Last Name" />
            <asp:BoundField DataField="email" HeaderText="Email" />
            <asp:BoundField DataField="enrollment_date" HeaderText="Enrollment Date"
                            DataFormatString="{0:yyyy-MM-dd}" HtmlEncode="False" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Edit" Text="Edit" />
                    &nbsp;
                    <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" Text="Delete"
                        OnClientClick="return confirmDeleteStudent();">
                    </asp:LinkButton>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:LinkButton ID="lnkUpdate" runat="server" CommandName="Update" Text="Update" />
                    &nbsp;
                    <asp:LinkButton ID="lnkCancel" runat="server" CommandName="Cancel" Text="Cancel" />
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <!-- JAVASCRIPT FUNCTIONS -->
    <script type="text/javascript">
        // Function to confirm before adding a student
        function confirmAddStudent() {
            return confirm("Are you sure you want to add this student?");
        }

        // Function to confirm before deleting a student 
        function confirmDeleteStudent() {
            return confirm("Are you sure you want to delete this student?");
        }
    </script>
</asp:Content>
