<%@ Page Title="" Language="C#" MasterPageFile="~/Customer/Customer.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="UrbanFoodWeb.Customer.Profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    body {
    background-color: #f8f9fa;
}

.container {
    max-width: 700px;
}

h2 {
    color: #4caf50;
}

.btn-primary {
    background-color: #4caf50;
    border-color: #4caf50;
}

.btn-primary:hover {
    background-color: #45a049;
    border-color: #45a049;
}

.form-check-label a {
    color: #4caf50;
}

/* Fix header visibility issue with navbar */
.pt-5 {
    padding-top: 80px !important;
}


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

   <asp:ScriptManager ID="ScriptManager1" runat="server" />
     <!-- Page Header Start -->
 <div class="container-fluid page-header py-5">
     <h1 class="text-center text-white display-6">My Profile</h1>
     <ol class="breadcrumb justify-content-center mb-0">
         <li class="breadcrumb-item"><a href="#">Home</a></li>
         <li class="breadcrumb-item active text-white">My Profile</li>
     </ol>
 </div>
<div class="container mt-5 pt-5">
    <h2 class="mb-4">Profile</h2>
    <asp:Label ID="lblMessage" runat="server" CssClass="text-success" />

    <div class="row">
        <div class="col-md-6 mb-3">
            <asp:Label AssociatedControlID="txtFirstName" runat="server" Text="First Name" CssClass="form-label" />
            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" />
        </div>
        <div class="col-md-6 mb-3">
            <asp:Label AssociatedControlID="txtLastName" runat="server" Text="Last Name" CssClass="form-label" />
            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" />
        </div>
    </div>

    <div class="row">
        <div class="col-md-6 mb-3">
            <asp:Label AssociatedControlID="txtEmail" runat="server" Text="Email Address" CssClass="form-label" />
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" ReadOnly="true" />
        </div>
        <div class="col-md-6 mb-3">
            <asp:Label AssociatedControlID="txtPhoneNumber" runat="server" Text="Phone Number" CssClass="form-label" />
            <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" />
        </div>
    </div>

    <div class="row">
        <div class="col-md-6 mb-3">
            <asp:Label AssociatedControlID="txtPassword" runat="server" Text="Password" CssClass="form-label" />
            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" />
        </div>
        <div class="col-md-6 mb-3">
            <asp:Label AssociatedControlID="txtConfirmPassword" runat="server" Text="Confirm Password" CssClass="form-label" />
            <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="form-control" />
        </div>
    </div>

    <div class="mb-3">
        <asp:Label AssociatedControlID="txtAddress" runat="server" Text="Address" CssClass="form-label" />
        <asp:TextBox ID="txtAddress" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
    </div>

    <div class="row mb-3">
        <div class="col-md-6">
            <asp:Label AssociatedControlID="ddlDietary" runat="server" Text="Dietary Preferences" CssClass="form-label" />
            <asp:DropDownList ID="ddlDietary" runat="server" CssClass="form-control">
                <asp:ListItem Text="-- Select Preference --" Value="" />
                <asp:ListItem Text="Vegetarian" Value="Vegetarian" />
                <asp:ListItem Text="Non-Vegetarian" Value="Non-Vegetarian" />
                <asp:ListItem Text="Vegan" Value="Vegan" />
                <asp:ListItem Text="Gluten-Free" Value="Gluten-Free" />
                <asp:ListItem Text="Keto" Value="Keto" />
            </asp:DropDownList>
        </div>
    </div>

    <asp:Button ID="btnUpdate" runat="server" CssClass="btn btn-primary" Text="Save Changes" OnClick="btnUpdate_Click" />
</div>


</asp:Content>
