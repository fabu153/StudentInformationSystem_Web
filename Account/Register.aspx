<%@ Page Title="Register" Language="vb" AutoEventWireup="false" 
    MasterPageFile="~/Site.Master" CodeBehind="Register.aspx.vb" 
    Inherits="StudentInformationSystem_Web.Register" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
  <!-- Load the Poppins font from Google Fonts -->
  <link href="https://fonts.googleapis.com/css?family=Poppins:300,400,500,600,700,800,900&display=swap" rel="stylesheet" />
  
  <!-- Inline CSS for minimal styling using Bootstrap classes -->
  <style type="text/css">
    /* Force Poppins for the entire page */
    body { font-family: 'Poppins', sans-serif !important; }
    
    /* Two-column layout for the registration page */
    .register-container {
      display: flex;
      gap: 30px;
      padding: 30px;
      margin-top: 30px;
    }
    .register-form {
      flex: 1;
      padding: 20px;
      border: 1px solid #ddd;
      border-radius: 8px;
      box-shadow: 0 2px 5px rgba(0,0,0,0.1);
    }
    .register-form h2 {
      font-weight: 700;
      margin-bottom: 20px;
      font-size: 28px;
      color: #222;
    }
    /* Floating label styles using Bootstrap's form-floating */
    .form-floating {
      position: relative;
      margin-bottom: 1rem;
    }
    .form-floating .form-control {
      padding: 1rem 0.75rem;
    }
    /* Minimal custom styling for the Register button */
    .btn-register {
      background-color: #6dabe4;
      color: #fff;
      padding: 10px 20px;
      border: none;
      border-radius: 4px;
      font-size: 16px;
      cursor: pointer;
    }
    .btn-register:hover {
      background-color: #4292dc;
    }
    /* Image column styling */
    .register-image {
      flex: 1;
      text-align: center;
      display: flex;
      align-items: center;
      justify-content: center;
    }
    .register-image img {
      max-width: 100%;
      border-radius: 8px;
    }
  </style>

  <div class="container">
    <div class="register-container">
      <!-- Left Column: Registration Form -->
      <div class="register-form">
        <h2>Sign Up</h2>
        <asp:ValidationSummary runat="server" CssClass="text-danger" />
        <p class="text-danger">
          <asp:Literal runat="server" ID="ErrorMessage" />
        </p>

        <div class="form-floating">
          <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder=" " />
          <label for="<%= txtFirstName.ClientID %>">First Name</label>
          <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" 
              ErrorMessage="First Name is required." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="form-floating">
          <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder=" " />
          <label for="<%= txtLastName.ClientID %>">Last Name</label>
          <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" 
              ErrorMessage="Last Name is required." CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="form-floating">
          <asp:TextBox ID="Email" runat="server" CssClass="form-control" TextMode="Email" placeholder=" " />
          <label for="<%= Email.ClientID %>">Email</label>
          <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="Email" 
              ErrorMessage="Email is required." CssClass="text-danger" Display="Dynamic" />
          <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="Email"
              ErrorMessage="Invalid email format." 
              ValidationExpression="\w+([-+.'']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
              CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="form-floating">
          <asp:TextBox ID="Password" runat="server" CssClass="form-control" TextMode="Password" placeholder=" " />
          <label for="<%= Password.ClientID %>">Password</label>
          <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="Password" 
              ErrorMessage="Password is required." CssClass="text-danger" Display="Dynamic" />
          <asp:RegularExpressionValidator ID="revPassword" runat="server" ControlToValidate="Password"
              ErrorMessage="Password must be at least 6 characters long." 
              ValidationExpression=".{6,}" CssClass="text-danger" Display="Dynamic" />
        </div>
        <div class="form-floating">
          <asp:TextBox ID="ConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder=" " />
          <label for="<%= ConfirmPassword.ClientID %>">Confirm Password</label>
          <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="ConfirmPassword" 
              ErrorMessage="Please confirm your password." CssClass="text-danger" Display="Dynamic" />
          <!-- CompareValidator to ensure passwords match -->
          <asp:CompareValidator ID="cvPasswords" runat="server" 
              ControlToValidate="ConfirmPassword" ControlToCompare="Password" 
              ErrorMessage="Passwords do not match." CssClass="text-danger" Display="Dynamic" />
        </div>

        <!-- Register Button -->
        <div class="mb-3">
          <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-primary btn-register" OnClick="CreateUser_Click" />
        </div>
      </div>

      <!-- Right Column: Signup Image -->
      <div class="register-image">
        <img src='<%= ResolveUrl("~/Content/Colorlib/images/signup-image.jpg") %>' alt="Sign Up Image" />
      </div>
    </div>
  </div>
</asp:Content>
