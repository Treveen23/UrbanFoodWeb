<%@ Page Title="" Language="C#" MasterPageFile="~/Customer/Customer.Master" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="UrbanFoodWeb.Customer.Orders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Page Header Start -->
    <div class="container-fluid page-header py-5">
        <h1 class="text-center text-white display-6">Orders</h1>
        <ol class="breadcrumb justify-content-center mb-0">
            <li class="breadcrumb-item"><a href="#">Home</a></li>
            <li class="breadcrumb-item active text-white">Orders</li>
        </ol>
    </div>
    <!-- Page Header End -->

    <div class="container-fluid fruite py-5">
        <div class="container py-5">

            <asp:Panel ID="pnlNoOrders" runat="server" CssClass="alert alert-info" Visible="false">
                <p>You don't have any orders yet. <asp:HyperLink ID="lnkShop" runat="server" NavigateUrl="~/Customer/Shop.aspx">Start shopping</asp:HyperLink></p>
            </asp:Panel>

            <asp:Panel ID="pnlOrders" runat="server">
                <asp:Repeater ID="rptOrders" runat="server" OnItemCommand="rptOrders_ItemCommand" OnItemDataBound="rptOrders_ItemDataBound">
                    <ItemTemplate>
                        <div class="card mb-4">
                            <div class="card-header d-flex justify-content-between align-items-center">
                                <div>
                                    <h5>Order #<%# Eval("OrderID") %></h5>
                                    <small><%# Convert.ToDateTime(Eval("OrderDate")).ToString("MMMM dd, yyyy") %></small>
                                </div>
                                <div>
                                    <span class="badge bg-primary"><%# Eval("OrderStatus") %></span>
                                    <span class="badge bg-info ms-2">Delivery: <%# Eval("DeliveryStatus") %></span>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-6">
                                        <p><strong>Total:</strong> $<%# string.Format("{0:0.00}", Eval("TotalAmount")) %></p>
                                        <p><strong>Delivery Date:</strong> 
                                            <%# Eval("DeliveryDate") != DBNull.Value ? 
                                                Convert.ToDateTime(Eval("DeliveryDate")).ToString("MMMM dd, yyyy") : 
                                                "Not scheduled yet" %>
                                        </p>
                                        <p><strong>Delivery Notes:</strong> <%# Eval("Notes") ?? "None" %></p>
                                    </div>
                                    <div class="col-md-6 text-end">
                                        <asp:Button ID="btnViewDetails" runat="server" Text="View Details" 
                                            CssClass="btn btn-outline-primary" 
                                            CommandName="ViewDetails" 
                                            CommandArgument='<%# Eval("OrderID") %>' />

                                        <asp:Button ID="btnCancelOrder" runat="server" Text="Cancel Order" 
                                            CssClass="btn btn-outline-danger ms-2" 
                                            CommandName="CancelOrder" 
                                            CommandArgument='<%# Eval("OrderID") %>'
                                            OnClientClick="return confirm('Are you sure you want to cancel this order? This cannot be undone.');" 
                                            Visible='<%# Eval("OrderStatus").ToString() == "Pending" %>' />
                                    </div>
                                </div>

                                <asp:Panel ID="pnlOrderDetails" runat="server" Visible="false" CssClass="mt-4">
                                    <h6>Order Items</h6>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvOrderItems" runat="server" AutoGenerateColumns="false" 
                                            CssClass="table table-striped table-hover">
                                            <Columns>
                                                <asp:BoundField DataField="ProductName" HeaderText="Product" />
                                                <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                                                <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="${0:0.00}" />
                                                <asp:BoundField DataField="ItemTotal" HeaderText="Total" DataFormatString="${0:0.00}" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </asp:Panel>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>

            <div class="mt-4">
                <asp:Button ID="btnBackToShop" runat="server" Text="Back to Shop" CssClass="btn btn-primary" 
                    OnClick="btnBackToShop_Click" />
            </div>

        </div>
    </div>

    <!-- Success message for order cancellation -->
    <asp:Panel ID="pnlMessage" runat="server" CssClass="toast-container position-fixed bottom-0 end-0 p-3" Visible="false">
        <div class="toast show" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header">
                <strong class="me-auto">Notification</strong>
                <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close" onclick="document.getElementById('<%= pnlMessage.ClientID %>').style.display='none';"></button>
            </div>
            <div class="toast-body">
                <asp:Literal ID="litMessage" runat="server"></asp:Literal>
            </div>
        </div>
    </asp:Panel>

</asp:Content>
