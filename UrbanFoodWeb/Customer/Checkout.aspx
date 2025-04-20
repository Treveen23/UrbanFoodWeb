<%@ Page Title="" Language="C#" MasterPageFile="~/Customer/Customer.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="UrbanFoodWeb.Customer.Checkout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    
        <!-- Single Page Header start -->
    <div class="container-fluid page-header py-5">
        <h1 class="text-center text-white display-6">Checkout</h1>
        <ol class="breadcrumb justify-content-center mb-0">
            <li class="breadcrumb-item"><a href="Default.aspx">Home</a></li>
            <li class="breadcrumb-item"><a href="Cart.aspx">Cart</a></li>
            <li class="breadcrumb-item active text-white">Checkout</li>
        </ol>
    </div>
    <!-- Single Page Header End -->

    <!-- Checkout Page Start -->
    <div class="container-fluid py-5">
        <div class="container py-5">
            <div class="row g-5">
                <div class="col-lg-6">
                    <div class="section-title mb-4">
                        <h1 class="display-5 mb-0">Billing Details</h1>
                    </div>
                    <div class="row g-3">
                        <div class="col-md-6">
                            <div class="form-floating">
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="First Name"></asp:TextBox>
                                <label for="txtFirstName">First Name</label>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-floating">
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Last Name"></asp:TextBox>
                                <label for="txtLastName">Last Name</label>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-floating">
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Email" TextMode="Email"></asp:TextBox>
                                <label for="txtEmail">Email</label>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-floating">
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" placeholder="Phone Number"></asp:TextBox>
                                <label for="txtPhone">Phone Number</label>
                            </div>
                        </div>
                        <div class="col-12">
                            <div class="form-floating">
                                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" placeholder="Address Line 1"></asp:TextBox>
                                <label for="txtAddress">Address Line 1</label>
                            </div>
                        </div>
                        <div class="col-12">
                            <div class="form-floating">
                                <asp:TextBox ID="txtAddress2" runat="server" CssClass="form-control" placeholder="Address Line 2"></asp:TextBox>
                                <label for="txtAddress2">Address Line 2</label>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-floating">
                                <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" placeholder="City"></asp:TextBox>
                                <label for="txtCity">City</label>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-floating">
                                <asp:TextBox ID="txtState" runat="server" CssClass="form-control" placeholder="State"></asp:TextBox>
                                <label for="txtState">State</label>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-floating">
                                <asp:TextBox ID="txtZip" runat="server" CssClass="form-control" placeholder="Zip Code"></asp:TextBox>
                                <label for="txtZip">Zip Code</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-6">
                    <div class="section-title mb-4">
                        <h1 class="display-5 mb-0">Order Summary</h1>
                    </div>
                    <div class="bg-light p-4 mb-4">
                        <div class="row g-0">
                            <div class="col-sm-6">
                                <strong>Product</strong>
                            </div>
                            <div class="col-sm-6 text-end">
                                <strong>Total</strong>
                            </div>
                        </div>
                        <hr>
                        <asp:Repeater ID="rptOrderSummary" runat="server">
                            <ItemTemplate>
                                <div class="row g-0 py-1">
                                    <div class="col-sm-6">
                                        <%# Eval("ProductName") %> x <%# Eval("Quantity") %>
                                    </div>
                                    <div class="col-sm-6 text-end">
                                        $<%# Eval("TotalPrice", "{0:0.00}") %>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <hr>
                        <div class="row g-0 py-1">
                            <div class="col-sm-6">
                                Subtotal
                            </div>
                            <div class="col-sm-6 text-end">
                                <asp:Label ID="lblSubtotal" runat="server">$0.00</asp:Label>
                            </div>
                        </div>
                        <div class="row g-0 py-1">
                            <div class="col-sm-6">
                                Shipping
                            </div>
                            <div class="col-sm-6 text-end">
                                <asp:Label ID="lblShipping" runat="server">$3.00</asp:Label>
                            </div>
                        </div>
                        <hr>
                        <div class="row g-0 py-1">
                            <div class="col-sm-6">
                                <strong>Total</strong>
                            </div>
                            <div class="col-sm-6 text-end">
                                <strong><asp:Label ID="lblTotal" runat="server">$0.00</asp:Label></strong>
                            </div>
                        </div>
                    </div>
                    <div class="section-title mb-4">
                        <h4>Payment Method</h4>
                    </div>
                    <div class="mb-4">
                        <div class="form-check mb-2">
    <asp:RadioButton ID="rdoPayPal" runat="server" GroupName="payment" CssClass="form-check-input" Checked="true"  />
    <label class="form-check-label" for="rdoPayPal">PayPal</label>
</div>
<div class="form-check mb-2">
    <asp:RadioButton ID="rdoCashOnDelivery" runat="server" GroupName="payment" CssClass="form-check-input" />
    <label class="form-check-label" for="rdoCashOnDelivery">Cash On Delivery</label>
</div>
<div class="form-check mb-2">
    <asp:RadioButton ID="rdoBankTransfer" runat="server" GroupName="payment" CssClass="form-check-input" />
    <label class="form-check-label" for="rdoBankTransfer">Bank Transfer</label>
</div>
                    </div>
                    <div class="py-4">
                        <asp:Button ID="btnPlaceOrder" runat="server" Text="Place Order" CssClass="btn border-secondary rounded-pill px-4 py-3 text-primary" OnClick="btnPlaceOrder_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Checkout Page End -->


</asp:Content>
