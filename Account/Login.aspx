<%@ Page Title="Log in" Language="vb" AutoEventWireup="false" 
    MasterPageFile="~/Site.Master" CodeBehind="Login.aspx.vb" 
    Inherits="StudentInformationSystem_Web.Login" Async="true" %>
<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
  <!-- Load the Poppins font -->
  <link href="https://fonts.googleapis.com/css?family=Poppins:300,400,500,600,700,800,900&display=swap" rel="stylesheet" />
  
  <!-- Inline CSS for a two-column login layout using Bootstrap -->
  <style type="text/css">
    /* Ensure the page uses Poppins */
    body { font-family: 'Poppins', sans-serif !important; }
    
    /* Container for the login page */
    .login-container {
      display: flex;
      gap: 30px;
      padding: 30px;
      margin-top: 30px;
    }
    /* Left column: Local login form */
    .login-form {
      flex: 1;
      padding: 20px;
      border: 1px solid #ddd;
      border-radius: 8px;
      box-shadow: 0 2px 5px rgba(0,0,0,0.1);
    }
    .login-form h2 {
      font-weight: 700;
      margin-bottom: 20px;
      font-size: 28px;
      color: #222;
    }
    /* Remove negative margin so error messages push the next field down */
    .form-floating {
      position: relative;
      margin-bottom: 1rem; /* changed from -0.5rem to 1rem */
    }
    .form-floating .form-control {
      padding: 0rem 0.75rem;
    }
    /* Checkbox styling */
    .checkbox-container {
      margin-bottom: 20px;
    }
    /* Login button styling */
    .btn-login {
      background-color: #6dabe4;
      color: #fff;
      padding: 10px 20px;
      border: none;
      border-radius: 4px;
      font-size: 16px;
      cursor: pointer;
    }
    .btn-login:hover {
      background-color: #4292dc;
    }
    /* Right column: Social login and sign-in image */
    .social-login-container {
      flex: 0 0 33.33%;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 20px;
      padding: 20px;
    }
    .social-login-container h4 {
      margin-bottom: 20px;
      font-weight: 600;
    }
    .login-image {
      max-width: 100%;
      border-radius: 8px;
    }
    /* Compact styling for the validation summary container */
    .compact-validation-summary {
      margin-bottom: 10px;
      padding: 5px;
    }
  </style>
  
  <div class="container">
    <div class="login-container">
      <!-- Left Column: Local Login Form -->
      <div class="login-form">
        <h2>Log In.</h2>
        <hr />
        <!-- Container for ValidationSummary with a compact style -->
        <div class="compact-validation-summary">
          <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
              CssClass="text-danger" HeaderText="Please fix the following errors:" />
        </div>
        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
          <p class="text-danger">
            <asp:Literal runat="server" ID="FailureText" />
          </p>
        </asp:PlaceHolder>
        
        <div class="form-floating">
          <asp:TextBox ID="Email" runat="server" CssClass="form-control" TextMode="Email" placeholder=" " />
          <label for="<%= Email.ClientID %>">Email</label>
          <!-- Set Display="Dynamic" so the error pushes the layout down -->
          <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
              ControlToValidate="Email" 
              CssClass="text-danger" 
              ErrorMessage="The email field is required." 
              Display="Dynamic" />
        </div>
        <div class="form-floating">
          <asp:TextBox ID="Password" runat="server" CssClass="form-control" TextMode="Password" placeholder=" " />
          <label for="<%= Password.ClientID %>">Password</label>
          <!-- Set Display="Dynamic" so the error pushes the layout down -->
          <asp:RequiredFieldValidator ID="rfvPassword" runat="server" 
              ControlToValidate="Password" 
              CssClass="text-danger" 
              ErrorMessage="The password field is required." 
              Display="Dynamic" />
        </div>
        <div class="form check checkbox-container">
          <asp:CheckBox ID="RememberMe" runat="server" CssClass="form-check-input" />
          <label for="<%= RememberMe.ClientID %>" class="form-check-label">Remember me?</label>
        </div>
        <div class="mb-3">
          <asp:Button ID="btnLogin" runat="server" Text="Log in" CssClass="btn btn-outline-dark btn-login" OnClick="LogIn" />
        </div>
        <p>
          <asp:HyperLink ID="RegisterHyperLink" runat="server" NavigateUrl="~/Account/Register" ViewStateMode="Disabled">
            Register as a new user
          </asp:HyperLink>
        </p>
      </div>
      
      <!-- Right Column: Sign-in Image and Social Login -->
      <div class="social-login-container">
        <div>
          <img src='<%= ResolveUrl("~/Content/Colorlib/images/signin-image.jpg") %>' alt="Sign In Image" class="login-image" />
        </div>
      </div>
    </div>
  </div>
</asp:Content>
