<%@ Page Title="" Language="C#" MasterPageFile="~/Supplier/SupplierHome.Master" AutoEventWireup="true" CodeBehind="Delivery.aspx.cs" Inherits="UrbanFoodWeb.Supplier.Delivery" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
  
    <form id="form1" runat="server">
        <div class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-body">
                        <h4 class="card-title">Delivery Management</h4>
                        <h6 class="card-subtitle">View and manage delivery requests from customers</h6>
                        <div class="table-responsive mt-3">
                            <asp:GridView ID="gvDeliveryRequests" runat="server" AutoGenerateColumns="False" 
                                CssClass="table table-striped table-bordered" 
                                OnRowCommand="gvDeliveryRequests_RowCommand" 
                                OnRowDataBound="gvDeliveryRequests_RowDataBound" 
                                DataKeyNames="DeliveryID">
                                <Columns>
                                    <asp:BoundField DataField="DeliveryID" HeaderText="ID" ReadOnly="True" />
                                    <asp:BoundField DataField="OrderID" HeaderText="Order ID" />
                                    <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                                    <asp:BoundField DataField="Address" HeaderText="Delivery Address" />
                                    <asp:BoundField DataField="RequestDate" HeaderText="Request Date" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:BoundField DataField="Status" HeaderText="Status" />
                                    <asp:BoundField DataField="DeliveryDate" HeaderText="Delivery Date" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnView" runat="server" CommandName="ViewDetails" CommandArgument='<%# Eval("DeliveryID") %>'
                                                CssClass="btn btn-info btn-sm" Text="View" />
                                            <asp:LinkButton ID="btnUpdate" runat="server" CommandName="UpdateStatus" CommandArgument='<%# Eval("DeliveryID") %>'
                                                CssClass="btn btn-primary btn-sm" Text="Update" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
        <!-- Delivery Details Modal -->
        <div class="modal fade" id="deliveryDetailsModal" tabindex="-1" role="dialog" aria-labelledby="deliveryDetailsModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="deliveryDetailsModalLabel">Delivery Details</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="lblDeliveryID">Delivery ID:</label>
                                    <asp:Label ID="lblDeliveryID" runat="server" CssClass="form-control"></asp:Label>
                                </div>
                                <div class="form-group">
                                    <label for="lblOrderID">Order ID:</label>
                                    <asp:Label ID="lblOrderID" runat="server" CssClass="form-control"></asp:Label>
                                </div>
                                <div class="form-group">
                                    <label for="lblCustomerName">Customer Name:</label>
                                    <asp:Label ID="lblCustomerName" runat="server" CssClass="form-control"></asp:Label>
                                </div>
                                <div class="form-group">
                                    <label for="lblContact">Contact Number:</label>
                                    <asp:Label ID="lblContact" runat="server" CssClass="form-control"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <label for="lblAddress">Delivery Address:</label>
                                    <asp:Label ID="lblAddress" runat="server" CssClass="form-control" style="height: auto;"></asp:Label>
                                </div>
                                <div class="form-group">
                                    <label for="lblRequestDate">Request Date:</label>
                                    <asp:Label ID="lblRequestDate" runat="server" CssClass="form-control"></asp:Label>
                                </div>
                                <div class="form-group">
                                    <label for="lblDeliveryDate">Delivery Date:</label>
                                    <asp:Label ID="lblDeliveryDate" runat="server" CssClass="form-control"></asp:Label>
                                </div>
                                <div class="form-group">
                                    <label for="lblStatus">Status:</label>
                                    <asp:Label ID="lblStatus" runat="server" CssClass="form-control"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-12">
                                <div class="form-group">
                                    <label for="lblNotes">Special Instructions:</label>
                                    <asp:Label ID="lblNotes" runat="server" CssClass="form-control" style="height: auto;"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col-12">
                                <h5>Order Items</h5>
                                <asp:GridView ID="gvOrderItems" runat="server" AutoGenerateColumns="False" 
                                    CssClass="table table-striped table-bordered" DataKeyNames="ItemID">
                                    <Columns>
                                        <asp:BoundField DataField="ItemID" HeaderText="Item ID" ReadOnly="True" />
                                        <asp:BoundField DataField="ProductName" HeaderText="Product" />
                                        <asp:BoundField DataField="Quantity" HeaderText="Quantity" />
                                        <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:C}" />
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

        <!-- Update Status Modal -->
        <div class="modal fade" id="updateStatusModal" tabindex="-1" role="dialog" aria-labelledby="updateStatusModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="updateStatusModalLabel">Update Delivery Status</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <asp:HiddenField ID="hdnDeliveryID" runat="server" />
                        <div class="form-group">
                            <label for="ddlStatus">Status:</label>
                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Pending" Value="Pending" />
                                <asp:ListItem Text="Processing" Value="Processing" />
                                <asp:ListItem Text="Out for Delivery" Value="Out for Delivery" />
                                <asp:ListItem Text="Delivered" Value="Delivered" />
                                <asp:ListItem Text="Cancelled" Value="Cancelled" />
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtDeliveryDate">Delivery Date:</label>
                            <asp:TextBox ID="txtDeliveryDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtNotes">Notes:</label>
                            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        <asp:Button ID="btnSaveStatus" runat="server" Text="Save Changes" CssClass="btn btn-primary" OnClick="btnSaveStatus_Click" />
                    </div>
                </div>
            </div>
        </div>
        <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>


        <!-- Script to show modals -->
        <script type="text/javascript">
            function showDeliveryDetailsModal() {
                $('#deliveryDetailsModal').modal('show');
            }
            function showUpdateStatusModal() {
                $('#updateStatusModal').modal('show');
            }
        </script>
    </form>
</asp:Content>
