<%@ Page Title="" Language="C#" MasterPageFile="~/Customer/Customer.Master" AutoEventWireup="true" CodeBehind="OrderConfirmation.aspx.cs" Inherits="UrbanFoodWeb.Customer.OrderConfirmation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <!-- Single Page Header start -->
    <div class="container-fluid page-header py-5">
        <h1 class="text-center text-white display-6">Order Confirmation</h1>
        <ol class="breadcrumb justify-content-center mb-0">
            <li class="breadcrumb-item"><a href="Default.aspx">Home</a></li>
            <li class="breadcrumb-item active text-white">Order Confirmation</li>
        </ol>
    </div>
    <!-- Single Page Header End -->

    <!-- Order Confirmation Start -->
    <div class="container-fluid py-5">
        <div class="container py-5 text-center">
            <div class="row justify-content-center">
                <div class="col-lg-8">
                    <i class="bi bi-check-circle-fill text-success d-block mb-3" style="font-size: 5rem;"></i>
                    <h1 class="mb-4">Thank You!</h1>
                    <p class="mb-4">Your order has been placed successfully. We've received your order and will begin processing it soon.</p>
                    
                    <div class="bg-light p-4 mb-4 mx-auto" style="max-width: 500px;">
                        <h4 class="mb-3">Order Summary</h4>
                        <div class="d-flex justify-content-between mb-2">
                            <span>Order Number:</span>
                            <span><asp:Label ID="lblOrderNumber" runat="server"></asp:Label></span>
                        </div>
                        <div class="d-flex justify-content-between mb-2">
                            <span>Order Date:</span>
                            <span><asp:Label ID="lblOrderDate" runat="server"></asp:Label></span>
                        </div>
                        <div class="d-flex justify-content-between mb-2">
                            <span>Order Total:</span>
                            <span><asp:Label ID="lblOrderTotal" runat="server"></asp:Label></span>
                        </div>
                        <div class="d-flex justify-content-between mb-2">
                            <span>Payment Method:</span>
                            <span><asp:Label ID="lblPaymentMethod" runat="server"></asp:Label></span>
                        </div>
                    </div>
                    
                    <p class="mb-4">You will receive an email confirmation shortly at <asp:Label ID="lblEmail" runat="server"></asp:Label></p>
                    
                    <div class="d-flex justify-content-center gap-3">
                        <asp:Button ID="btnContinueShopping" runat="server" Text="Continue Shopping" 
                                    CssClass="btn border-secondary rounded-pill px-4 py-3 text-primary"
                                    OnClick="btnContinueShopping_Click" />
                        
                        <asp:Button ID="btnViewOrders" runat="server" Text="View Orders" 
                                    CssClass="btn border-secondary rounded-pill px-4 py-3 text-primary"
                                    OnClick="btnViewOrders_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Order Confirmation End -->


</asp:Content>
