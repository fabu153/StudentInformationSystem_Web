<%@ Page Title="Manage Account" Language="vb" Async="true" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Manage.aspx.vb" Inherits="StudentInformationSystem_Web.Manage" %>

<%@ Import Namespace="StudentInformationSystem_Web" %>
<%@ Import Namespace="Microsoft.AspNet.Identity" %>
<%@ Register Src="~/Account/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <main aria-labelledby="title">
        <h2 id="title"><%: Title %></h2>

        <div>
            <asp:PlaceHolder runat="server" ID="SuccessMessagePlaceHolder" Visible="false" ViewStateMode="Disabled">
                <p class="text-success"><%: SuccessMessage %></p>
            </asp:PlaceHolder>
        </div>

        <hr />

        <dl class="dl-horizontal">
            <!-- Password Section -->
            <dt>Password:</dt>
            <dd>
                <asp:HyperLink NavigateUrl="/Account/ManagePassword" Text="[Change]" Visible="false" ID="ChangePassword" runat="server" />
                <asp:HyperLink NavigateUrl="/Account/ManagePassword" Text="[Create]" Visible="false" ID="CreatePassword" runat="server" />
            </dd>

            <!-- Email Notifications Settings Section -->
            <dt>Email Notifications Settings:</dt>
            <dd>
                <div class="checkbox">
                    <label>
                        <asp:CheckBox ID="chkEmailNotifications" runat="server" />
                        &nbsp; Enable enrollment email notifications
                    </label>
                </div>
                <div style="margin-top:20px;">
                    <asp:Button 
                        ID="btnSaveEmailSettings" 
                        runat="server" 
                        Text="Save Settings" 
                        CssClass="btn btn-primary"
                        OnClick="btnSaveEmailSettings_Click" />
                    <asp:Label 
                        ID="lblEmailSettingsStatus" 
                        runat="server" 
                        CssClass="text-success" 
                        Style="margin-left:15px;" />
                </div>
            </dd>
        </dl>
    </main>
</asp:Content>
