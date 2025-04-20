<%@ Page Title="" Language="C#" MasterPageFile="~/Supplier/SupplierHome.Master" AutoEventWireup="true" CodeBehind="Order.aspx.cs" Inherits="UrbanFoodWeb.Supplier.Order" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    
  
    <form id="form1" runat="server">


        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Order Management</h4>
                        <div class="row mb-3">
                            <div class="col-md-3">
                                <asp:DropDownList ID="ddlOrderStatus" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlOrderStatus_SelectedIndexChanged">
                                    <asp:ListItem Value="" Text="All Orders" />
                                    <asp:ListItem Value="New" Text="New" />
                                    <asp:ListItem Value="Confirmed" Text="Confirmed" />
                                    <asp:ListItem Value="Pending" Text="Pending" />
                                    <asp:ListItem Value="Processing" Text="Processing" />
                                    <asp:ListItem Value="Shipped" Text="Shipped" />
                                    <asp:ListItem Value="Delivered" Text="Delivered" />
                                    <asp:ListItem Value="Cancelled" Text="Cancelled" />
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-3">
                                <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn btn-info" OnClick="btnFilter_Click" />
                                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-outline-secondary" OnClick="btnReset_Click"/>
                            </div>
                        </div>
                        <div class="table-responsive">
                            <asp:GridView ID="gvOrders" runat="server" CssClass="table table-striped table-bordered" 
                                AutoGenerateColumns="False" OnRowCommand="gvOrders_RowCommand"
                                DataKeyNames="OrderId">
                                <Columns>
                                    <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                    <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                                    <asp:BoundField DataField="OrderDate" HeaderText="Order Date" DataFormatString="{0:dd-MMM-yyyy}" />
                                    <asp:BoundField DataField="TotalAmount" HeaderText="Total Amount" DataFormatString="{0:C}" />
                                    <asp:TemplateField HeaderText="Status">
                                        <ItemTemplate>
                                            <span class='<%# GetStatusBadgeClass(Eval("OrderStatus").ToString()) %>'>
                                                <%# Eval("OrderStatus") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="PaymentMethod" HeaderText="Payment Method" />
                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            

                                            <asp:LinkButton ID="lnkView" runat="server" CommandName="ViewOrder"
    CommandArgument='<%# Eval("OrderId") %>' CssClass="btn btn-sm btn-info"
    UseSubmitBehavior="false">
    <i data-feather="eye" class="feather-icon"></i> View
</asp:LinkButton>
                                            
                                            <!-- Show confirmation buttons only for New orders -->
                                            <asp:Panel ID="pnlConfirmationBtns" runat="server" Visible='<%# Eval("OrderStatus").ToString() == "New" %>'>
                                               <asp:LinkButton ID="lnkConfirm" runat="server" CommandName="ConfirmOrder"
    CommandArgument='<%# Eval("OrderId") %>' CssClass="btn btn-sm btn-success"
    UseSubmitBehavior="false">
    <i data-feather="check" class="feather-icon"></i> Confirm
</asp:LinkButton>

<asp:LinkButton ID="lnkReject" runat="server" CommandName="RejectOrder"
    CommandArgument='<%# Eval("OrderId") %>' CssClass="btn btn-sm btn-danger"
    UseSubmitBehavior="false">
    <i data-feather="x" class="feather-icon"></i> Reject
</asp:LinkButton>
                                            </asp:Panel>
                                            
                                            <!-- Show Update Status for orders not in New state -->
                                          <asp:LinkButton ID="lnkUpdateStatus" runat="server" CommandName="UpdateStatus"
    CommandArgument='<%# Eval("OrderId") + "|" + Eval("OrderStatus") %>' CssClass="btn btn-sm btn-primary"
    UseSubmitBehavior="false"
    Visible='<%# Eval("OrderStatus").ToString() != "New" && Eval("OrderStatus").ToString() != "Cancelled" %>'>
    <i data-feather="edit-3" class="feather-icon"></i> Update Status
</asp:LinkButton>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Order Details Modal -->
        <div class="modal fade" id="orderModal" tabindex="-1" role="dialog" aria-labelledby="orderModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="orderModalLabel">Order Details</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-6">
                                <h6>Order Information</h6>
                                <table class="table table-borderless">
                                    <tr>
                                        <td>Order ID:</td>
                                        <td><asp:Label ID="lblOrderId" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>Order Date:</td>
                                        <td><asp:Label ID="lblOrderDate" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>Status:</td>
                                        <td>
                                            <asp:Label ID="lblOrderStatus" runat="server"></asp:Label>
                                            <asp:Panel ID="pnlConfirmPanel" runat="server" Visible="false" CssClass="mt-2">
                                                <asp:Button ID="btnQuickConfirm" runat="server" Text="Confirm Order" 
                                                    CssClass="btn btn-sm btn-success mr-2" OnClick="btnQuickConfirm_Click"/>
                                                <asp:Button ID="btnQuickReject" runat="server" Text="Reject Order" 
                                                    CssClass="btn btn-sm btn-danger" OnClick="btnQuickReject_Click"/>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>Payment Method:</td>
                                        <td><asp:Label ID="lblPaymentMethod" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>Total Amount:</td>
                                        <td><asp:Label ID="lblTotalAmount" runat="server"></asp:Label></td>
                                    </tr>
                                </table>
                            </div>
                            <div class="col-md-6">
                                <h6>Customer Information</h6>
                                <table class="table table-borderless">
                                    <tr>
                                        <td>Name:</td>
                                        <td><asp:Label ID="lblCustomerName" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>Email:</td>
                                        <td><asp:Label ID="lblCustomerEmail" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>Phone:</td>
                                        <td><asp:Label ID="lblCustomerPhone" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td>Address:</td>
                                        <td><asp:Label ID="lblCustomerAddress" runat="server"></asp:Label></td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-12">
                                <h6>Order Items</h6>
                                <asp:GridView ID="gvOrderItems" runat="server" CssClass="table table-bordered" 
                                    AutoGenerateColumns="False">
                                    <Columns>
                                        <asp:BoundField DataField="ProductName" HeaderText="Product" />
                                        <asp:TemplateField HeaderText="Image">
                                            <ItemTemplate>
                                                <asp:Image ID="imgProduct" runat="server" ImageUrl='<%# Eval("ProductImage") %>' 
                                                    Width="50" Height="50" CssClass="rounded" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:C}" />
                                        <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                                        <asp:BoundField DataField="Subtotal" HeaderText="Subtotal" DataFormatString="{0:C}" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>

        <!-- Update Order Status Modal -->
        <div class="modal fade" id="updateStatusModal" tabindex="-1" role="dialog" aria-labelledby="updateStatusModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="updateStatusModalLabel">Update Order Status</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <label for="ddlUpdateStatus">Order Status</label>
                            <asp:DropDownList ID="ddlUpdateStatus" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Confirmed" Text="Confirmed" />
                                <asp:ListItem Value="Pending" Text="Pending" />
                                <asp:ListItem Value="Processing" Text="Processing" />
                                <asp:ListItem Value="Shipped" Text="Shipped" />
                                <asp:ListItem Value="Delivered" Text="Delivered" />
                                <asp:ListItem Value="Cancelled" Text="Cancelled" />
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtStatusNotes">Notes</label>
                            <asp:TextBox ID="txtStatusNotes" runat="server" CssClass="form-control" 
                                TextMode="MultiLine" Rows="3" placeholder="Enter notes about this status change (optional)"></asp:TextBox>
                        </div>
                        <asp:HiddenField ID="hdnOrderIdForStatus" runat="server" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnUpdateOrderStatus" runat="server" Text="Update Status" 
                            CssClass="btn btn-primary" OnClick="btnUpdateOrderStatus_Click" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Order Rejection Modal -->
        <div class="modal fade" id="rejectOrderModal" tabindex="-1" role="dialog" aria-labelledby="rejectOrderModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="rejectOrderModalLabel">Reject Order</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="alert alert-warning">
                            <i class="fas fa-exclamation-triangle"></i> You are about to reject this order. This action cannot be undone.
                        </div>
                        <div class="form-group">
                            <label for="txtRejectionReason">Rejection Reason</label>
                            <asp:TextBox ID="txtRejectionReason" runat="server" CssClass="form-control" 
                                TextMode="MultiLine" Rows="3" placeholder="Enter reason for rejecting this order (required)"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvRejectionReason" runat="server" 
                                ControlToValidate="txtRejectionReason" CssClass="text-danger"
                                ErrorMessage="Please provide a reason for rejection" ValidationGroup="RejectOrder" />
                        </div>
                        <div class="form-group">
                            <div class="custom-control custom-checkbox">
                                <asp:CheckBox ID="chkNotifyCustomer" runat="server" Checked="true" />
                                <label class="form-check-label" for="<%= chkNotifyCustomer.ClientID %>">
                                    Notify customer via email
                                </label>
                            </div>
                        </div>
                        <asp:HiddenField ID="hdnOrderIdForRejection" runat="server" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnConfirmReject" runat="server" Text="Reject Order" 
                            CssClass="btn btn-danger" OnClick="btnConfirmReject_Click" ValidationGroup="RejectOrder" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Order Confirmation Modal -->
        <div class="modal fade" id="confirmOrderModal" tabindex="-1" role="dialog" aria-labelledby="confirmOrderModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="confirmOrderModalLabel">Confirm Order</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="alert alert-info">
                            <i class="fas fa-info-circle"></i> You are about to confirm this order. This will initiate order processing.
                        </div>
                        <div class="form-group">
                            <label for="txtEstimatedDelivery">Estimated Delivery Date</label>
                            <asp:TextBox ID="txtEstimatedDelivery" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtConfirmationNotes">Notes</label>
                            <asp:TextBox ID="txtConfirmationNotes" runat="server" CssClass="form-control" 
                                TextMode="MultiLine" Rows="3" placeholder="Enter notes for customer (optional)"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <div class="custom-control custom-checkbox">
                                <asp:CheckBox ID="chkSendConfirmation" runat="server" Checked="true" />
                                <label class="form-check-label" for="<%= chkSendConfirmation.ClientID %>">
                                    Send confirmation email to customer
                                </label>
                            </div>
                        </div>
                        <asp:HiddenField ID="hdnOrderIdForConfirmation" runat="server" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnConfirmOrder" runat="server" Text="Confirm Order" 
                            CssClass="btn btn-success" OnClick="btnConfirmOrder_Click" />
                    </div>
                </div>
            </div>
        </div>
      <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/js/bootstrap.bundle.min.js"></script>

<script type="text/javascript">
    function showOrderModal() {
        $('#orderModal').modal('show');
    }
</script>

    </form>

    
         <script>


             // Function to show update status modal
             function showUpdateStatusModal() {
                 $('#updateStatusModal').modal('show');
                 return false;
             }

             // Function to show reject order modal
             function showRejectOrderModal() {
                 $('#rejectOrderModal').modal('show');
                 return false;
             }

             // Function to show confirm order modal
             function showConfirmOrderModal() {
                 $('#confirmOrderModal').modal('show');
                 return false;
             }
         </script>


</asp:Content>
