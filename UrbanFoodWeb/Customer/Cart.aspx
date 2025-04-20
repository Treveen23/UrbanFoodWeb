<%@ Page Title="" Language="C#" MasterPageFile="~/Customer/Customer.Master" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="UrbanFoodWeb.Customer.Cart" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function decreaseQuantity(inputId) {
            var input = document.getElementById(inputId);
            var value = parseInt(input.value);
            if (value > 1) {
                input.value = value - 1;
            }
            return false;
        }

        function increaseQuantity(inputId) {
            var input = document.getElementById(inputId);
            var value = parseInt(input.value);
            input.value = value + 1;
            return false;
        }
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <!-- Single Page Header start -->
    <div class="container-fluid page-header py-5">
        <h1 class="text-center text-white display-6">Cart</h1>
        <ol class="breadcrumb justify-content-center mb-0">
            <li class="breadcrumb-item"><a href="Default.aspx">Home</a></li>
            <li class="breadcrumb-item active text-white">Cart</li>
        </ol>
    </div>
    <!-- Single Page Header End -->

    <!-- Cart Page Start -->
    <div class="container-fluid py-5">
        <div class="container py-5">
            <!-- Empty Cart Panel -->
            <asp:Panel ID="pnlEmptyCart" runat="server" Visible="false">
                <div class="text-center">
                    <h3>Your cart is empty</h3>
                    <p>Looks like you haven't added any products to your cart yet.</p>
                    <a href="Shop.aspx" class="btn border-secondary rounded-pill px-4 py-3 text-primary">Continue Shopping</a>
                </div>
            </asp:Panel>
            
            <!-- Cart Content Panel -->
            <asp:Panel ID="pnlCartContent" runat="server" Visible="true">
                <div class="table-responsive">
                    <table class="table">
                        <thead>
                          <tr>
                            <th scope="col">Products</th>
                            <th scope="col">Name</th>
                            <th scope="col">Price</th>
                            <th scope="col">Quantity</th>
                            <th scope="col">Total</th>
                            <th scope="col">Handle</th>
                          </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptCartItems" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <th scope="row">
                                            <div class="d-flex align-items-center">
                                                <img src='<%# "~/ProductImages/" + Eval("ImageURL") %>' class="img-fluid me-5 rounded-circle" style="width: 80px; height: 80px;" alt="<%# Eval("ProductName") %>">
                                            </div>
                                        </th>
                                        <td>
                                            <p class="mb-0 mt-4"><%# Eval("ProductName") %></p>
                                        </td>
                                        <td>
                                            <p class="mb-0 mt-4">$<%# Eval("Price", "{0:0.00}") %></p>
                                        </td>
                                        <td>
                                            <div class="input-group quantity mt-4" style="width: 100px;">
                                                <div class="input-group-btn">
                                                    <button type="button" class="btn btn-sm btn-minus rounded-circle bg-light border" 
                                                            onclick='<%# "return decreaseQuantity(\"txtQuantity_" + Container.ItemIndex + "\");" %>'>
                                                        <i class="fa fa-minus"></i>
                                                    </button>
                                                </div>
                                                <asp:TextBox runat="server" ID="txtQuantity" ClientIDMode="Static"
             CssClass="form-control form-control-sm text-center border-0"
             Text='<%# Eval("Quantity") %>'></asp:TextBox>
                                                <div class="input-group-btn">
                                                    <button type="button" class="btn btn-sm btn-plus rounded-circle bg-light border"
                                                            onclick='<%# "return increaseQuantity(\"txtQuantity_" + Container.ItemIndex + "\");" %>'>
                                                        <i class="fa fa-plus"></i>
                                                    </button>
                                                </div>
                                            </div>
                                            <asp:LinkButton ID="btnUpdateQuantity" runat="server" CssClass="btn btn-sm btn-link text-primary"
                                                            CommandName="UpdateQuantity" CommandArgument='<%# Eval("ProductID") + "," + Container.ItemIndex %>'
                                                            OnCommand="btnUpdateQuantity_Command">
                                                Update
                                            </asp:LinkButton>
                                        </td>
                                        <td>
                                            <p class="mb-0 mt-4">$<%# Eval("TotalPrice", "{0:0.00}") %></p>
                                        </td>
                                        <td>
                                            <asp:LinkButton ID="btnRemoveItem" runat="server" CssClass="btn btn-md rounded-circle bg-light border mt-4"
                                                            CommandName="RemoveItem" CommandArgument='<%# Eval("ProductID") %>'
                                                            OnCommand="btnRemoveItem_Command">
                                                <i class="fa fa-times text-danger"></i>
                                            </asp:LinkButton>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
                
                <div class="row g-4 justify-content-end">
                    <div class="col-8"></div>
                    <div class="col-sm-8 col-md-7 col-lg-6 col-xl-4">
                        <div class="bg-light rounded">
                            <div class="p-4">
                                <h1 class="display-6 mb-4">Cart <span class="fw-normal">Total</span></h1>
                                <div class="d-flex justify-content-between mb-4">
                                    <h5 class="mb-0 me-4">Subtotal:</h5>
                                    <asp:Label ID="lblSubtotal" runat="server" CssClass="mb-0">$0.00</asp:Label>
                                </div>
                                <div class="d-flex justify-content-between">
                                    <h5 class="mb-0 me-4">Shipping</h5>
                                    <div class="">
                                        <asp:Label ID="lblShipping" runat="server" CssClass="mb-0">$3.00</asp:Label>
                                    </div>
                                </div>
                                <p class="mb-0 text-end">Shipping to default address.</p>
                            </div>
                            <div class="py-4 mb-4 border-top border-bottom d-flex justify-content-between">
                                <h5 class="mb-0 ps-4 me-4">Total</h5>
                                <asp:Label ID="lblTotal" runat="server" CssClass="mb-0 pe-4">$0.00</asp:Label>
                            </div>
                            <asp:Button ID="btnProceedCheckout" runat="server" Text="Proceed Checkout" 
                                        CssClass="btn border-secondary rounded-pill px-4 py-3 text-primary text-uppercase mb-4 ms-4"
                                        OnClick="btnProceedCheckout_Click" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
    <!-- Cart Page End -->

</asp:Content>
