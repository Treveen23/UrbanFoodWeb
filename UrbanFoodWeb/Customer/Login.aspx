<%@ Page Title="" Language="C#" MasterPageFile="~/Customer/Customer.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UrbanFoodWeb.Customer.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

       <div class="container login-container">
       <div class="row mb-4">
           <div class="col-12 text-center">
               <h2 class="mb-3">Login to Your Account</h2>
               <p class="text-muted mb-4">Welcome back! Please enter your credentials</p>
               
               <div class="d-flex justify-content-center mb-4">
                   <asp:Button ID="btnCustomer" runat="server" Text="Customer" CssClass="btn btn-toggle toggle-active me-2" OnClick="btnCustomer_Click" />
                   <asp:Button ID="btnSupplier" runat="server" Text="Supplier" CssClass="btn btn-toggle toggle-inactive" OnClick="btnSupplier_Click" />
               </div>
           </div>
       </div>

       <asp:Panel ID="pnlLoginForm" runat="server" DefaultButton="btnLogin">
           <div class="row mb-3">
               <div class="col-12">
                   <asp:Label ID="lblEmail" runat="server" CssClass="form-label" Text="Email Address"></asp:Label>
                   <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Enter your email" TextMode="Email"></asp:TextBox>
                 
           </div>

           <div class="row mb-2">
               <div class="col-12">
                   <asp:Label ID="lblPassword" runat="server" CssClass="form-label" Text="Password"></asp:Label>
                   <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Enter your password" TextMode="Password"></asp:TextBox>
                   
               </div>
           </div>

           

           <div class="row">
               <div class="col-12">
                   <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary login-btn w-100" OnClick="btnLogin_Click" />
               </div>
           </div>

           <div class="or-divider">
               <span>OR</span>
           </div>

           <div class="row mb-3">
               <div class="col-12">
                   <asp:Panel ID="pnlCustomerMessage" runat="server">
                       <p class="text-center">Don't have a customer account yet?</p>
                   </asp:Panel>
                   <asp:Panel ID="pnlSupplierMessage" runat="server" Visible="false">
                       <p class="text-center">Don't have a supplier account yet?</p>
                   </asp:Panel>
                   <a href="Register.aspx" class="btn btn-outline-primary w-100">Create New Account</a>
               </div>
           </div>

           <asp:Panel ID="pnlErrorMessage" runat="server" Visible="false" CssClass="alert alert-danger mt-3">
               <asp:Literal ID="litErrorMessage" runat="server"></asp:Literal>
           </asp:Panel>
       </asp:Panel>
   </div>

</asp:Content>
