<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="StudentInformationSystem_Web._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="container mt-5">
    <!-- Hero Section -->
    <div class="p-5 mb-4 bg-light rounded-3 shadow">
      <div class="container-fluid py-5 text-center">
        <h1 class="display-4 fw-bold">Student Information System</h1>
        <p class="col-md-8 fs-4 mx-auto">Simple management of student data</p>
<asp:HyperLink ID="hlGetStarted" runat="server" CssClass="btn btn-primary btn-lg" Role="button" Text="Get Started"></asp:HyperLink>
      </div>
    </div>

    <!-- Image Cards Section -->
    <div class="row g-4">
      <div class="col-md-4">
        <div class="card shadow">
          <img src="https://images.unsplash.com/photo-1588072432836-e10032774350?fm=jpg&q=60&w=3000&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" class="card-img-top" alt="Library">
          <div class="card-body">
            <h5 class="card-title">Extensive Resources</h5>
            <p class="card-text">Discover a wealth of materials and learning tools designed for academic success.</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card shadow">
          <img src="https://plus.unsplash.com/premium_photo-1664300897489-fd98eee64faf?fm=jpg&q=60&w=3000&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" class="card-img-top" alt="Graduation">
          <div class="card-body">
            <h5 class="card-title">Pathway to Success</h5>
            <p class="card-text">Track progress and celebrate achievements as you pave your way to success.</p>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card shadow">
          <img src="https://images.unsplash.com/photo-1519389950473-47ba0277781c?fm=jpg&q=60&w=3000&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" class="card-img-top" alt="Innovation">
          <div class="card-body">
            <h5 class="card-title">Innovative Tools</h5>
            <p class="card-text">Utilize modern tools and technology to enhance your educational experience.</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Footer -->
    <footer class="pt-5 my-5 text-muted border-top text-center">
      &copy; 2025 - Student Information System
    </footer>
  </div>
</asp:Content>
